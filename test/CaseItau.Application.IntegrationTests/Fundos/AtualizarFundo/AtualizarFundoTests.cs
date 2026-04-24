using CaseItau.Application.Exceptions;
using CaseItau.Application.Fundos.AtualizarFundo;
using CaseItau.Application.IntegrationTests.Common;
using CaseItau.Domain.Fundos;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.AtualizarFundo;

public class AtualizarFundoTests : BaseIntegrationTest
{
    public AtualizarFundoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarSucesso_Quando_Command_Valido()
    {
        // Arrange
        var command = new AtualizarFundoCommand(
            "ITAUAC546",                // fundo existente via Seeder
            "ITAU ACOES DIVIDENDO PLUS",
            "60701190000104",           // CNPJ válido (Itaú Unibanco), não usado por outro fundo
            2);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarSucesso_Quando_CnpjPertenceAoProprioFundo()
    {
        // Arrange – atualizar mantendo o mesmo CNPJ do fundo não deve gerar conflito
        var command = new AtualizarFundoCommand(
            "ITAURF321",
            "ITAU LONGO PRAZO RF PLUS",
            "74061531000120",           // mesmo CNPJ do fundo ITAURF321
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarErro_Quando_FundoNaoEncontrado()
    {
        // Arrange
        var command = new AtualizarFundoCommand(
            "INEXISTENTE",
            "Fundo Qualquer",
            "60701190000104",
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.NaoEncontrado("INEXISTENTE"));
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarErro_Quando_CnpjJaExisteEmOutroFundo()
    {
        // Arrange – tentar usar o CNPJ do fundo ITAUMM999 ao atualizar ITAURF123
        var command = new AtualizarFundoCommand(
            "ITAURF123",
            "ITAU JUROS RF +",
            "27462582000184",           // CNPJ do fundo ITAUMM999 (Seeder)
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.CnpjIndisponivel("27462582000184"));
    }

    [Fact]
    public async Task AtualizarFundo_Deve_RetornarErro_Quando_TipoFundoInvalido()
    {
        // Arrange – usa o próprio CNPJ do fundo para não gerar conflito de CNPJ
        var command = new AtualizarFundoCommand(
            "ITAURF123",
            "ITAU JUROS RF +",
            "94094799000176",           // próprio CNPJ de ITAURF123 (sem conflito)
            999);                       // tipo inexistente

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.TipoFundoInvalido(999));
    }

    [Fact]
    public async Task AtualizarFundo_Deve_LancarValidationException_Quando_CodigoVazio()
    {
        // Arrange
        var command = new AtualizarFundoCommand(
            string.Empty,
            "Fundo Qualquer",
            "60701190000104",
            1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AtualizarFundo_Deve_LancarValidationException_Quando_NomeVazio()
    {
        // Arrange
        var command = new AtualizarFundoCommand(
            "ITAURF123",
            string.Empty,
            "60701190000104",
            1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AtualizarFundo_Deve_LancarValidationException_Quando_CnpjVazio()
    {
        // Arrange
        var command = new AtualizarFundoCommand(
            "ITAURF123",
            "ITAU JUROS RF +",
            string.Empty,
            1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AtualizarFundo_Deve_LancarValidationException_Quando_CnpjComNumerosInsuficientes()
    {
        // Arrange – CNPJ precisa ter exatamente 14 dígitos
        var command = new AtualizarFundoCommand(
            "ITAURF123",
            "ITAU JUROS RF +",
            "1234567",                  // menos de 14 dígitos
            1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AtualizarFundo_Deve_LancarValidationException_Quando_CodigoTipoMenorOuIgualAZero()
    {
        // Arrange
        var command = new AtualizarFundoCommand(
            "ITAURF123",
            "ITAU JUROS RF +",
            "60701190000104",
            0);                         // CodigoTipo deve ser maior que 0

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
