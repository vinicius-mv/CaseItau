using CaseItau.Application.Abstractions.Data;
using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Domain.Abstractions;
using Dapper;

namespace CaseItau.Application.Fundos.ListarFundos;

public class ListarFundosQueryHandler : IQueryHandler<ListarFundosQuery, IReadOnlyList<FundoResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public ListarFundosQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<FundoResponse>>> Handle(ListarFundosQuery request, CancellationToken cancellationToken)
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
            JOIN TIPO_FUNDO tf ON tf.CODIGO = f.CODIGO_TIPO;
        """;

        var fundsResponse = Enumerable.Empty<FundoResponse>();

        var fundos = await connection.QueryAsync<FundoResponse>(sql);

        return fundos.ToList();
    }
}
