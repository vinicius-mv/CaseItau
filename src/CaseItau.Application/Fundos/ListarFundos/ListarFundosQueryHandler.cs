using CaseItau.Application.Abstractions.Data;
using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using Dapper;

namespace CaseItau.Application.Fundos.ListarFundos;

internal sealed class ListarFundosQueryHandler : IQueryHandler<ListarFundosQuery, IReadOnlyList<FundoResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public ListarFundosQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<FundoResponse>>> Handle(ListarFundosQuery request, CancellationToken cancellationToken)
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
}
