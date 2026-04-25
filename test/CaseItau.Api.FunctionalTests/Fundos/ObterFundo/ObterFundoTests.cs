using CaseItau.Api.FunctionalTests.Common;
using CaseItau.Application.Fundos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CaseItau.Api.FunctionalTests.Fundos.ObterFundo;

public class ObterFundoTests : BaseFunctionalTest
{
    private const string BaseEndpoint = "/api/Fundo";

    public ObterFundoTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    // -------------------------------------------------------------------------
    // Sucesso – 200 OK
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ObterFundo_Deve_RetornarOk_Com_DadosDoFundo_Quando_CodigoExistente()
    {
        // Arrange – "ITAURF123" existe pelo Seeder
        var codigo = "ITAURF123";

        // Act
        var response = await HttpClient.GetAsync($"{BaseEndpoint}/{codigo}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var fundo = await response.Content.ReadFromJsonAsync<FundoResponse>();
        fundo.Should().NotBeNull();
        fundo!.Codigo.Should().Be(codigo);
        fundo.Nome.Should().NotBeNullOrWhiteSpace();
        fundo.Cnpj.Should().NotBeNullOrWhiteSpace();
        fundo.NomeTipo.Should().NotBeNullOrWhiteSpace();
        fundo.CodigoTipo.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ObterFundo_Deve_RetornarOk_Com_PatrimonioPreenchido_Quando_FundoPossuiPatrimonio()
    {
        // Arrange – "ITAUAC546" existe pelo Seeder e tem patrimônio definido
        var codigo = "ITAUAC546";

        // Act
        var response = await HttpClient.GetAsync($"{BaseEndpoint}/{codigo}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var fundo = await response.Content.ReadFromJsonAsync<FundoResponse>();
        fundo.Should().NotBeNull();
        fundo!.Patrimonio.Should().NotBeNull();
        fundo.Patrimonio.Should().BeGreaterThan(0);
    }

    // -------------------------------------------------------------------------
    // Não encontrado – 404 Not Found
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ObterFundo_Deve_RetornarNotFound_Quando_CodigoNaoExiste()
    {
        // Act
        var response = await HttpClient.GetAsync($"{BaseEndpoint}/INEXISTENTE");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
