using CaseItau.Application.Abstractions.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;

namespace CaseItau.Infra.Data.Seed;

public partial class DatabaseSeeder
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();

        using var connection = sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sql = """
                SELECT COUNT(1) > 0 FROM "TIPO_FUNDO"
            """;

            var hasTipoFundo = await connection.ExecuteScalarAsync<bool>(sql);
            if (hasTipoFundo)
                return;

            logger.LogInformation("Seeding database...");

            await SeedTipoFundo(transaction);
            await SeedFundo(transaction);

            transaction.Commit();

            logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedTipoFundo(IDbTransaction transaction)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "Seed", "TipoFundo.json");
        var json = await File.ReadAllTextAsync(path);
        var tipoFundos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TipoFundoSeedDto>>(json);

        const string sql = """
                INSERT INTO "TIPO_FUNDO" ("CODIGO", "NOME")
                VALUES (@Codigo, @Nome)
            """;
        await transaction.Connection!.ExecuteAsync(sql, tipoFundos);
    }

    private static async Task SeedFundo(IDbTransaction transaction)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "Seed", "Fundo.json");
        var json = await File.ReadAllTextAsync(path);
        var fundos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FundoSeedDto>>(json);

        const string sql = """
                INSERT INTO "FUNDO" ("CODIGO", "NOME", "CNPJ", "CODIGO_TIPO", "PATRIMONIO")
                VALUES (@Codigo, @Nome, @Cnpj, @CodigoTipo, @Patrimonio)
            """;

        await transaction.Connection!.ExecuteAsync(sql, fundos);
    }
}
