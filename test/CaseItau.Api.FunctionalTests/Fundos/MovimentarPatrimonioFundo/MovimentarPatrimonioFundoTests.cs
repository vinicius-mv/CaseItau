using CaseItau.Api.FunctionalTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CaseItau.Api.FunctionalTests.Fundos.MovimentarPatrimonioFundo;

public class MovimentarPatrimonioFundoTests : BaseFunctionalTest
{
    private const string BaseEndpoint = "/api/Fundo";

    public MovimentarPatrimonioFundoTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    // -------------------------------------------------------------------------
    // Sucesso – 204 No Content
    // -------------------------------------------------------------------------

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarNoContent_Quando_RequestValido()
    {
        // Arrange
        var valor = 5_000m;

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123/patrimonio", valor);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarNoContent_Quando_SaqueDeAltoRiscoNaoInterrompeRequisicao()
    {
        // Arrange – patrimônio de ITAUAC546 é R$ 66.421.254,83; 20% = R$ 13.284.250,97.
        // Saque de R$ 15.000.000 configura alto risco: o EventHandler detecta e notifica,
        // mas NÃO deve interromper a requisição – a API deve retornar 204 normalmente.
        var valor = -15_000_000m;

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAUAC546/patrimonio", valor);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // -------------------------------------------------------------------------
    // Erros de domínio – 400 Bad Request
    // -------------------------------------------------------------------------

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarBadRequest_Quando_FundoNaoEncontrado()
    {
        // Arrange
        var valor = 1_000m;

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/INEXISTENTE/patrimonio", valor);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // Erros de validação – 400 Bad Request via GlobalExceptionHandler
    // -------------------------------------------------------------------------

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarBadRequest_Quando_ValorMovimentacaoIgualAZero()
    {
        // Arrange
        var valor = 0m;

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123/patrimonio", valor);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarBadRequest_Quando_CodigoExcedeTamanhoMaximo()
    {
        // Arrange – código com mais de 20 caracteres na URL dispara a validação
        var codigoLongo = new string('A', 21);
        var valor = 1_000m;

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/{codigoLongo}/patrimonio", valor);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
