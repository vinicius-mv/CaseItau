using CaseItau.Application.Abstractions.Data;
using CaseItau.Application.Fundos;
using Dapper;

namespace CaseItau.Infra.Repositories;

internal sealed class FundoReadRepository : IFundoReadRepository
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public FundoReadRepository(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<IReadOnlyList<FundoResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = $"""
            SELECT 
                f."CODIGO" AS {nameof(FundoResponse.Codigo)}, 
                f."NOME" AS {nameof(FundoResponse.Nome)}, 
                f."CNPJ" AS {nameof(FundoResponse.Cnpj)}, 
                f."PATRIMONIO" AS {nameof(FundoResponse.Patrimonio)}, 
                tf."CODIGO" AS {nameof(FundoResponse.CodigoTipo)}, 
                tf."NOME" AS {nameof(FundoResponse.NomeTipo)}
            FROM "FUNDO" f
            JOIN "TIPO_FUNDO" tf ON tf."CODIGO" = f."CODIGO_TIPO";
        """;

        var fundos = await connection.QueryAsync<FundoResponse>(sql);

        return fundos.ToList();
    }

    public async Task<FundoResponse?> ObterAsync(string codigo, CancellationToken cancellationToken = default)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = $"""
            SELECT 
                f."CODIGO" AS {nameof(FundoResponse.Codigo)}, 
                f."NOME" AS {nameof(FundoResponse.Nome)}, 
                f."CNPJ" AS {nameof(FundoResponse.Cnpj)}, 
                f."PATRIMONIO" AS {nameof(FundoResponse.Patrimonio)}, 
                tf."CODIGO" AS {nameof(FundoResponse.CodigoTipo)}, 
                tf."NOME" AS {nameof(FundoResponse.NomeTipo)}
            FROM "FUNDO" f
            JOIN "TIPO_FUNDO" tf ON tf."CODIGO" = f."CODIGO_TIPO"
            WHERE f."CODIGO" = @Codigo;
        """;

        var fundo = await connection.QueryFirstOrDefaultAsync<FundoResponse>(sql, new { Codigo = codigo });

        return fundo;
    }
}
