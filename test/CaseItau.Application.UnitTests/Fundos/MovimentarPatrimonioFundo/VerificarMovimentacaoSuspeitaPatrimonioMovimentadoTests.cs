using CaseItau.Application.Abstractions.Notifications;
using CaseItau.Application.Fundos.MovimentarPatrimonioFundo;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CaseItau.Application.UnitTests.Fundos.MovimentarPatrimonioFundo;

public class VerificarMovimentacaoSuspeitaPatrimonioMovimentadoTests
{
    private readonly INotificationService _notificationServiceMock;
    private readonly ILogger<VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler> _loggerMock;
    private readonly VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler _handler;

    public VerificarMovimentacaoSuspeitaPatrimonioMovimentadoTests()
    {
        _notificationServiceMock = Substitute.For<INotificationService>();
        _loggerMock = Substitute.For<ILogger<VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler>>();
        _handler = new VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler(_notificationServiceMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_Deve_NaoEnviarNotificacao_Quando_MovimentacaoForAbaixoDoLimite()
    {
        // Arrange
        var notification = new PatrimonioMovimentadoDomainEvent(FundoData.FundoCodigo.Value, 500_000m, 1_500_000m);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _notificationServiceMock.DidNotReceiveWithAnyArgs().EnviarAsync(default!, default!, default);
    }

    [Fact]
    public async Task Handle_Deve_NaoEnviarNotificacao_Quando_MovimentacaoForAporteAcimaDoLimite()
    {
        // Arrange
        var notification = new PatrimonioMovimentadoDomainEvent(FundoData.FundoCodigo.Value, 2_000_000m, 5_000_000m);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _notificationServiceMock.DidNotReceiveWithAnyArgs().EnviarAsync(default!, default!, default);
    }

    [Fact]
    public async Task Handle_Deve_NaoEnviarNotificacao_Quando_MovimentacaoForSaqueAbaixoDeVintePorCento()
    {
        // Arrange
        // Saque de 1.000.000, com patrimônio anterior de 10.000.000 (patrimônio atual = 9.000.000)
        var notification = new PatrimonioMovimentadoDomainEvent(FundoData.FundoCodigo.Value, -1_000_000m, 9_000_000m);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _notificationServiceMock.DidNotReceiveWithAnyArgs().EnviarAsync(default!, default!, default);
    }

    [Fact]
    public async Task Handle_Deve_EnviarNotificacao_Quando_MovimentacaoForSaqueDeAltoRisco()
    {
        // Arrange
        // Saque de 3.000.000, com patrimônio anterior de 10.000.000 (patrimônio atual = 7.000.000)
        var notification = new PatrimonioMovimentadoDomainEvent(FundoData.FundoCodigo.Value, -3_000_000m, 7_000_000m);

        _notificationServiceMock.EnviarAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _notificationServiceMock.Received(1).EnviarAsync(
            Arg.Is<string>(t => t.Contains("Alerta de movimentação suspeita")),
            Arg.Is<string>(m => m.Contains("Patrimônio atual: 7000000")),
            CancellationToken.None);
    }

    [Fact]
    public async Task Handle_Deve_LogarErro_Quando_FalhaAoEnviarNotificacao()
    {
        // Arrange
        var notification = new PatrimonioMovimentadoDomainEvent(FundoData.FundoCodigo.Value, -3_000_000m, 7_000_000m);
        var erroEnvio = Error.Failure("Notification.Failed", "Falha no serviço de email");

        _notificationServiceMock.EnviarAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(erroEnvio));

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _notificationServiceMock.Received(1).EnviarAsync(Arg.Any<string>(), Arg.Any<string>(), CancellationToken.None);
    }
}
