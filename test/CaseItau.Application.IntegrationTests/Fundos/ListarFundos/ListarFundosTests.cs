using CaseItau.Application.Fundos;
using CaseItau.Application.Fundos.ListarFundos;
using CaseItau.Application.IntegrationTests.Common;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.ListarFundos;

public class ListarFundosTests : BaseIntegrationTest
{
    public ListarFundosTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ListarFundos_DeveRetornarListaDeFundos()
    {
        // Arrange
        var query = new ListarFundosQuery();

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<List<FundoResponse>>();
        result.Value.Should().NotBeEmpty();

        // Focando no resultado: garantindo que os itens retornados estão devidamente preenchidos (mapeamento correto)
        // sem amarrar o teste aos valores literais do Seeder, evitando testes frágeis.
        result.Value.Should().AllSatisfy(fundo =>
        {
            fundo.Codigo.Should().NotBeNullOrWhiteSpace();
            fundo.Nome.Should().NotBeNullOrWhiteSpace();
            fundo.Cnpj.Should().NotBeNullOrWhiteSpace();
            fundo.NomeTipo.Should().NotBeNullOrWhiteSpace();
            fundo.Patrimonio.Should().BeGreaterThanOrEqualTo(0);
        });
    }
}
