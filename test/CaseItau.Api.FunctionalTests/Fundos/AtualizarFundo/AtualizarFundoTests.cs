using CaseItau.Api.FunctionalTests.Common;
using CaseItau.API.Controllers.Fundos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CaseItau.Api.FunctionalTests.Fundos.AtualizarFundo;

public class AtualizarFundoTests : BaseFunctionalTest
{
    private const string BaseEndpoint = "/api/Fundo";

    public AtualizarFundoTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    // -------------------------------------------------------------------------
    // Sucesso – 204 No Content
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarNoContent_Quando_RequestValido()
    {
        // Arrange
        var request = new AtualizarFundoRequest(
            Nome: "ITAU ACOES DIVIDENDO ATUALIZADO",
            Cnpj: "60701190000104",   // CNPJ válido (Itaú Unibanco), não usado por outro fundo
            CodigoTipo: 2);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAUAC546", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarNoContent_Quando_CnpjPertenceAoProprioFundo()
    {
        // Arrange – manter o mesmo CNPJ do fundo não deve gerar conflito
        var request = new AtualizarFundoRequest(
            Nome: "ITAU LONGO PRAZO RF PLUS",
            Cnpj: "74061531000120",   // próprio CNPJ do fundo ITAURF321
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF321", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // -------------------------------------------------------------------------
    // Erros de domínio – 400 Bad Request
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_FundoNaoEncontrado()
    {
        // Arrange
        var request = new AtualizarFundoRequest(
            Nome: "Fundo Qualquer",
            Cnpj: "60701190000104",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/INEXISTENTE", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_CnpjJaExisteEmOutroFundo()
    {
        // Arrange – "27462582000184" é o CNPJ do fundo ITAUMM999 (Seeder)
        var request = new AtualizarFundoRequest(
            Nome: "ITAU JUROS RF +",
            Cnpj: "27462582000184",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_TipoFundoInvalido()
    {
        // Arrange – usa o próprio CNPJ de ITAURF123 para evitar conflito de CNPJ
        var request = new AtualizarFundoRequest(
            Nome: "ITAU JUROS RF +",
            Cnpj: "94094799000176",   // próprio CNPJ do fundo ITAURF123
            CodigoTipo: 999);         // tipo inexistente

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // Erros de validação – 400 Bad Request via GlobalExceptionHandler
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_NomeVazio()
    {
        // Arrange
        var request = new AtualizarFundoRequest(
            Nome: string.Empty,
            Cnpj: "60701190000104",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_CnpjVazio()
    {
        // Arrange
        var request = new AtualizarFundoRequest(
            Nome: "ITAU JUROS RF +",
            Cnpj: string.Empty,
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_CnpjComNumerosInsuficientes()
    {
        // Arrange – CNPJ precisa ter exatamente 14 dígitos
        var request = new AtualizarFundoRequest(
            Nome: "ITAU JUROS RF +",
            Cnpj: "1234567",   // menos de 14 dígitos
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_CodigoTipoMenorOuIgualAZero()
    {
        // Arrange
        var request = new AtualizarFundoRequest(
            Nome: "ITAU JUROS RF +",
            Cnpj: "60701190000104",
            CodigoTipo: 0);   // CodigoTipo deve ser maior que 0

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/ITAURF123", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarBadRequest_Quando_CodigoExcedeTamanhoMaximo()
    {
        // Arrange – código com mais de 20 caracteres na URL dispara a validação
        var codigoLongo = new string('A', 21);
        var request = new AtualizarFundoRequest(
            Nome: "ITAU JUROS RF +",
            Cnpj: "60701190000104",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{BaseEndpoint}/{codigoLongo}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
