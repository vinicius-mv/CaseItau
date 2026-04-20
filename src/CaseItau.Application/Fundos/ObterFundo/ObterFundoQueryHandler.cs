using CaseItau.Application.Abstractions.Data;
using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using Dapper;

namespace CaseItau.Application.Fundos.ObterFundo;

internal sealed class ObterFundoQueryHandler : IQueryHandler<ObterFundoQuery, FundoResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public ObterFundoQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<FundoResponse>> Handle(ObterFundoQuery request, CancellationToken cancellationToken)
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

        var queryParams = new { request.Codigo };

        var fundo = await connection.QueryFirstOrDefaultAsync<FundoResponse>(sql, queryParams);

        if (fundo is null)
            return Result.Failure<FundoResponse>(FundoErrors.NaoEncontrado(request.Codigo));

        return fundo;
    }
}
