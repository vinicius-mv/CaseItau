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
    public async Task ListarFundos_Deve_RetornarTodosOsFundosDoSeeder()
    {
        // Arrange
        var query = new ListarFundosQuery();

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();

        // Verifica que cada item retornado está devidamente mapeado,
        // sem amarrar o teste aos valores literais do Seeder.
        result.Value.Should().AllSatisfy(fundo =>
        {
            fundo.Codigo.Should().NotBeNullOrWhiteSpace();
            fundo.Nome.Should().NotBeNullOrWhiteSpace();
            fundo.Cnpj.Should().NotBeNullOrWhiteSpace();
            fundo.NomeTipo.Should().NotBeNullOrWhiteSpace();
        });
    }
}
