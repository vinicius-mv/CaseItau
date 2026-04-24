using CaseItau.Application.Exceptions;
using CaseItau.Application.Fundos.AdicionarFundo;
using CaseItau.Application.IntegrationTests.Common;
using CaseItau.Domain.Fundos;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.AdicionarFundo;

public class AdicionarFundoTests : BaseIntegrationTest
{
    public AdicionarFundoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    // -------------------------------------------------------------------------
    // CommandHandler – casos de sucesso
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarSucesso_Quando_Command_Valido()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "TESTE123",
            "Fundo Teste Integracao",
            "60701190000104", // CNPJ válido (Itaú Unibanco)
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("TESTE123");
    }

    // -------------------------------------------------------------------------
    // CommandHandler – erros de domínio
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarErro_Quando_CodigoJaExiste()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "ITAURF123", // código já existe pelo Seeder
            "Outro Fundo",
            "33592510000154",
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.CodigoIndisponivel(command.Codigo));
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarErro_Quando_CnpjJaExiste()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "NOVO123",
            "Outro Fundo",
            "94094799000176", // CNPJ já existe pelo Seeder (ITAURF123)
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.CnpjIndisponivel(command.Cnpj));
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarErro_Quando_TipoFundoInvalido()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "NOVO456",
            "Outro Fundo",
            "86952316000107",
            999); // tipo inexistente

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.TipoFundoInvalido(command.CodigoTipo));
    }

    // -------------------------------------------------------------------------
    // CommandValidator – erros de validação lançam ValidationException
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AdicionarFundo_Deve_LancarValidationException_Quando_CodigoVazio()
    {
        // Arrange
        var command = new AdicionarFundoCommand(string.Empty, "Fundo Qualquer", "60701190000104", 1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AdicionarFundo_Deve_LancarValidationException_Quando_NomeVazio()
    {
        // Arrange
        var command = new AdicionarFundoCommand("NOVO789", string.Empty, "60701190000104", 1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AdicionarFundo_Deve_LancarValidationException_Quando_CnpjVazio()
    {
        // Arrange
        var command = new AdicionarFundoCommand("NOVO789", "Fundo Qualquer", string.Empty, 1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AdicionarFundo_Deve_LancarValidationException_Quando_CnpjComNumerosInsuficientes()
    {
        // Arrange – CNPJ precisa ter exatamente 14 dígitos
        var command = new AdicionarFundoCommand("NOVO789", "Fundo Qualquer", "1234567", 1);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task AdicionarFundo_Deve_LancarValidationException_Quando_CodigoTipoMenorOuIgualAZero()
    {
        // Arrange
        var command = new AdicionarFundoCommand("NOVO789", "Fundo Qualquer", "60701190000104", 0);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
