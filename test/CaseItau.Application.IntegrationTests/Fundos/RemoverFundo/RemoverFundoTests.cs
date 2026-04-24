using CaseItau.Application.Exceptions;
using CaseItau.Application.Fundos.RemoverFundo;
using CaseItau.Application.IntegrationTests.Common;
using CaseItau.Domain.Fundos;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.RemoverFundo;

public class RemoverFundoTests : BaseIntegrationTest
{
    public RemoverFundoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task RemoverFundo_Deve_RetornarSucesso_Quando_Command_Valido()
    {
        // Arrange
        var command = new RemoverFundoCommand("ITAUMM999");

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RemoverFundo_Deve_TornarFundoInexistente_Apos_Remocao()
    {
        // Arrange
        var remover = new RemoverFundoCommand("ITAURF321");

        // Act – remove o fundo
        await Sender.Send(remover);

        // Assert – tentar remover novamente deve retornar "não encontrado",
        // comprovando que o fundo foi de fato excluído do banco de dados.
        var segundaTentativa = await Sender.Send(remover);
        segundaTentativa.IsSuccess.Should().BeFalse();
        segundaTentativa.Error.Should().Be(FundoErrors.NaoEncontrado("ITAURF321"));
    }

    [Fact]
    public async Task RemoverFundo_Deve_RetornarErro_Quando_FundoNaoEncontrado()
    {
        // Arrange
        var command = new RemoverFundoCommand("INEXISTENTE");

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.NaoEncontrado("INEXISTENTE"));
    }

    [Fact]
    public async Task RemoverFundo_Deve_LancarValidationException_Quando_CodigoVazio()
    {
        // Arrange
        var command = new RemoverFundoCommand(string.Empty);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task RemoverFundo_Deve_LancarValidationException_Quando_CodigoExcedeTamanhoMaximo()
    {
        // Arrange – código com mais de 20 caracteres
        var codigoLongo = new string('A', FundoCodigo.MaxLength + 1);
        var command = new RemoverFundoCommand(codigoLongo);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
