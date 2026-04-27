using CaseItau.Api.FunctionalTests.Common;
using CaseItau.API.Controllers.Fundos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CaseItau.Api.FunctionalTests.Fundos.AdicionarFundo;

public class AdicionarFundoTests : BaseFunctionalTest
{
    private const string Endpoint = "/api/Fundo";

    public AdicionarFundoTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    // -------------------------------------------------------------------------
    // Sucesso
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarCreated_Quando_RequestValido()
    {
        // Arrange
        var request = new AdicionarFundoRequest(
            Codigo: "FUND001",
            Nome: "Fundo de Investimento Funcional",
            Cnpj: "60701190000104",   // CNPJ válido (Itaú Unibanco)
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().EndWith($"/{request.Codigo}");

        // O controller retorna a string do código como text/plain
        var codigoCriado = await response.Content.ReadAsStringAsync();
        codigoCriado.Should().Contain(request.Codigo);
    }

    // -------------------------------------------------------------------------
    // Erros de domínio – 400 Bad Request
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_CodigoJaExiste()
    {
        // Arrange – "ITAURF123" já existe pelo Seeder
        var request = new AdicionarFundoRequest(
            Codigo: "ITAURF123",
            Nome: "Fundo Duplicado",
            Cnpj: "33592510000154",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_CnpjJaExiste()
    {
        // Arrange – "94094799000176" é o CNPJ do fundo ITAURF123 (Seeder)
        var request = new AdicionarFundoRequest(
            Codigo: "NOVOFUNDO",
            Nome: "Fundo Novo",
            Cnpj: "94094799000176",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_TipoFundoInvalido()
    {
        // Arrange
        var request = new AdicionarFundoRequest(
            Codigo: "NOVOFUNDO",
            Nome: "Fundo Novo",
            Cnpj: "86952316000107",
            CodigoTipo: 999);   // tipo inexistente

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // Erros de validação – 400 Bad Request via GlobalExceptionHandler
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_CodigoVazio()
    {
        // Arrange
        var request = new AdicionarFundoRequest(
            Codigo: string.Empty,
            Nome: "Fundo Qualquer",
            Cnpj: "60701190000104",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_NomeVazio()
    {
        // Arrange
        var request = new AdicionarFundoRequest(
            Codigo: "FUND002",
            Nome: string.Empty,
            Cnpj: "60701190000104",
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_CnpjVazio()
    {
        // Arrange
        var request = new AdicionarFundoRequest(
            Codigo: "FUND002",
            Nome: "Fundo Qualquer",
            Cnpj: string.Empty,
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_CnpjComNumerosInsuficientes()
    {
        // Arrange – CNPJ precisa ter exatamente 14 dígitos
        var request = new AdicionarFundoRequest(
            Codigo: "FUND002",
            Nome: "Fundo Qualquer",
            Cnpj: "1234567",   // menos de 14 dígitos
            CodigoTipo: 1);

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarBadRequest_Quando_CodigoTipoMenorOuIgualAZero()
    {
        // Arrange
        var request = new AdicionarFundoRequest(
            Codigo: "FUND002",
            Nome: "Fundo Qualquer",
            Cnpj: "60701190000104",
            CodigoTipo: 0);   // CodigoTipo deve ser maior que 0

        // Act
        var response = await HttpClient.PostAsJsonAsync(Endpoint, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
