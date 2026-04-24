using CaseItau.Application.Exceptions;
using CaseItau.Application.Fundos.MovimentarPatrimonioFundo;
using CaseItau.Application.IntegrationTests.Common;
using CaseItau.Domain.Fundos;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.MovimentarPatrimonioFundo;

public class MovimentarPatrimonioFundoTests : BaseIntegrationTest
{
    public MovimentarPatrimonioFundoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarSucesso_Quando_Command_Valido()
    {
        // Arrange
        var command = new MovimentarPatrimonioFundoCommand("ITAURF123", 5_000m);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarErro_Quando_FundoNaoEncontrado()
    {
        // Arrange
        var command = new MovimentarPatrimonioFundoCommand("INEXISTENTE", 1_000m);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.NaoEncontrado("INEXISTENTE"));
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_LancarValidationException_Quando_CodigoVazio()
    {
        // Arrange
        var command = new MovimentarPatrimonioFundoCommand(string.Empty, 1_000m);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_LancarValidationException_Quando_CodigoExcedeTamanhoMaximo()
    {
        // Arrange – código com mais de 20 caracteres
        var codigoLongo = new string('A', FundoCodigo.MaxLength + 1);
        var command = new MovimentarPatrimonioFundoCommand(codigoLongo, 1_000m);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_LancarValidationException_Quando_ValorMovimentacaoIgualAZero()
    {
        // Arrange
        var command = new MovimentarPatrimonioFundoCommand("ITAURF123", 0m);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    // -------------------------------------------------------------------------
    // EventHandler – VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler
    //
    // Regras do EventHandler:
    //   – movimentações com valor absoluto < R$ 1.000.000 são sempre regulares
    //   – depósitos (valor > 0) nunca configuram saque de alto risco
    //   – saques com valor absoluto > R$ 1.000.000 E > 20% do patrimônio anterior
    //     são considerados de alto risco e disparam notificação
    //   – em qualquer cenário o EventHandler NÃO interrompe o comando principal
    // -------------------------------------------------------------------------

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarSucesso_Quando_MovimentacaoAbaixoDoLimiteDeVerificacao()
    {
        // Arrange – R$ 500.000 está abaixo do limite mínimo de verificação (R$ 1.000.000),
        // portanto o EventHandler conclui que é uma movimentação regular sem análise adicional.
        var command = new MovimentarPatrimonioFundoCommand("ITAUMM999", 500_000m);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarSucesso_Quando_DepositoDeAltoValor()
    {
        // Arrange – depósito de R$ 5.000.000: valor absoluto > R$ 1.000.000, mas depósitos
        // (valor positivo) nunca configuram saque de alto risco no EventHandler.
        var command = new MovimentarPatrimonioFundoCommand("ITAUMM999", 5_000_000m);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarSucesso_Quando_SaqueAbaixoDoLimiteAltoRisco()
    {
        // Arrange – patrimônio de ITAURF321 é R$ 7.875.421,58; 20% = R$ 1.575.084,32.
        // Saque de R$ 1.200.000 aciona a verificação (> R$ 1.000.000) mas está abaixo
        // do limiar de 20%, portanto o EventHandler classifica como movimentação regular.
        var command = new MovimentarPatrimonioFundoCommand("ITAURF321", -1_200_000m);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MovimentarPatrimonio_Deve_RetornarSucesso_Quando_SaqueDeAltoRiscoNaoInterrompeComando()
    {
        // Arrange – patrimônio de ITAUAC546 é R$ 66.421.254,83; 20% = R$ 13.284.250,97.
        // Saque de R$ 15.000.000 ultrapassa R$ 1.000.000 E excede o limiar de 20%,
        // configurando saque de alto risco. O EventHandler detecta e envia notificação,
        // mas NÃO interrompe o fluxo: o comando deve retornar sucesso normalmente.
        var command = new MovimentarPatrimonioFundoCommand("ITAUAC546", -15_000_000m);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
