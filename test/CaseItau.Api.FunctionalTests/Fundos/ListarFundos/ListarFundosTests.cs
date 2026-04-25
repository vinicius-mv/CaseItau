using CaseItau.Api.FunctionalTests.Common;
using CaseItau.Application.Fundos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CaseItau.Api.FunctionalTests.Fundos.ListarFundos;

public class ListarFundosTests : BaseFunctionalTest
{
    private const string Endpoint = "/api/Fundo";

    public ListarFundosTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ListarFundos_Deve_RetornarOk_Com_ListaDeFundos()
    {
        // Act
        var response = await HttpClient.GetAsync(Endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var fundos = await response.Content.ReadFromJsonAsync<List<FundoResponse>>();
        fundos.Should().NotBeNullOrEmpty();
        fundos!.Should().AllSatisfy(fundo =>
        {
            fundo.Codigo.Should().NotBeNullOrWhiteSpace();
            fundo.Nome.Should().NotBeNullOrWhiteSpace();
            fundo.Cnpj.Should().NotBeNullOrWhiteSpace();
            fundo.NomeTipo.Should().NotBeNullOrWhiteSpace();
        });
    }
}
