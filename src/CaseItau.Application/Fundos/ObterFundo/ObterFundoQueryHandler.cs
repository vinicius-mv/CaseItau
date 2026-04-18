using CaseItau.Application.Abstractions.Data;
using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
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

        const string sql = """
            SELECT 
                f.CODIGO AS Codigo, 
                f.NOME AS Nome, 
                f.CNPJ AS Cnpj, 
                f.PATRIMONIO AS Patrimonio, 
                tf.CODIGO AS CodigoTipo, 
                tf.NOME AS NomeTipo
            FROM FUNDO f
            JOIN TIPO_FUNDO tf ON tf.CODIGO = f.CODIGO_TIPO
            WHERE f.CODIGO = @Codigo;
        """;

        var queryParams = new { request.Codigo };

        var fundo = await connection.QueryFirstOrDefaultAsync<FundoResponse>(sql, queryParams);

        return fundo;
    }
}
