using CaseItau.Api.FunctionalTests.Common;
using CaseItau.API.Controllers.Fundos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CaseItau.Api.FunctionalTests.Fundos.RemoverFundo;

public class RemoverFundoTests : BaseFunctionalTest
{
    private const string BaseEndpoint = "/api/Fundo";

    public RemoverFundoTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    // -------------------------------------------------------------------------
    // Sucesso – 204 No Content
    // -------------------------------------------------------------------------

    [Fact]
    public async Task RemoverFundo_Deve_RetornarNoContent_Quando_RemoverFundoComSucesso()
    {
        // Arrange – cria um fundo para ser removido sem afetar dados do Seeder
        var codigo = "RMVTST001";
        var criarRequest = new AdicionarFundoRequest(
            Codigo: codigo,
            Nome: "Fundo Para Remover",
            Cnpj: "11222333000181",
            CodigoTipo: 1);

        await HttpClient.PostAsJsonAsync(BaseEndpoint, criarRequest);

        // Act
        var response = await HttpClient.DeleteAsync($"{BaseEndpoint}/{codigo}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoverFundo_Deve_TornarFundoInexistente_Apos_Remocao()
    {
        // Arrange – cria um fundo exclusivo para este teste
        var codigo = "RMVTST002";
        var criarRequest = new AdicionarFundoRequest(
            Codigo: codigo,
            Nome: "Fundo Para Remover Verificacao",
            Cnpj: "22333444000100",
            CodigoTipo: 1);

        await HttpClient.PostAsJsonAsync(BaseEndpoint, criarRequest);

        // Act
        await HttpClient.DeleteAsync($"{BaseEndpoint}/{codigo}");

        // Assert – tentativa de obter o fundo removido deve retornar 404
        var getResponse = await HttpClient.GetAsync($"{BaseEndpoint}/{codigo}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------------------
    // Erros de domínio – 400 Bad Request
    // -------------------------------------------------------------------------

    [Fact]
    public async Task RemoverFundo_Deve_RetornarBadRequest_Quando_CodigoNaoExiste()
    {
        // Act
        var response = await HttpClient.DeleteAsync($"{BaseEndpoint}/INEXISTENTE");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // Erros de validação – 400 Bad Request via GlobalExceptionHandler
    // -------------------------------------------------------------------------

    [Fact]
    public async Task RemoverFundo_Deve_RetornarBadRequest_Quando_CodigoExcedeTamanhoMaximo()
    {
        // Arrange – código com mais de 20 caracteres na URL dispara a validação
        var codigoLongo = new string('A', 21);

        // Act
        var response = await HttpClient.DeleteAsync($"{BaseEndpoint}/{codigoLongo}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
