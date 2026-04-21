using CaseItau.Application.Abstractions.Notifications;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using CaseItau.Domain.Fundos.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CaseItau.Application.Fundos.MovimentarPatrimonioFundo;

internal sealed class VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler
    : INotificationHandler<PatrimonioMovimentadoDomainEvent>
{
    private const decimal LimiteMinimoVerificacao = 1_000_000m;
    private const decimal FatorSaqueAltoRisco = 0.20m;

    private readonly INotificationService _notificationService;
    private readonly ILogger<VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler> _logger;

    public VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler(
        INotificationService notificationService,
        ILogger<VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(PatrimonioMovimentadoDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processando movimentação de patrimônio. Fundo: {Codigo} | Valor: {Valor} | Patrimônio atual: {Patrimonio}",
            notification.Codigo,
            notification.ValorMovimentacao,
            notification.PatrimonioAtual);

        var verificacao = VerificarAtividadeRegular(notification);

        if (verificacao.IsSuccess)
        {
            _logger.LogInformation(
                "Movimentação regular confirmada para o fundo {Codigo}.",
                notification.Codigo);
            return;
        }

        _logger.LogWarning(
            "Movimentação não regular detectada! Fundo: {Codigo} | Motivo: {Motivo}",
            notification.Codigo,
            verificacao.Error.Description);

        var notificacao = await _notificationService.EnviarAsync(
            titulo: $"Alerta de movimentação suspeita — Fundo {notification.Codigo}",
            mensagem: $"{verificacao.Error.Description} | Patrimônio atual: {notification.PatrimonioAtual}.", 
            cancellationToken);

        if (notificacao.IsFailure)
            _logger.LogError("Falha ao enviar notificação. Fundo: {Codigo} | Erro: {Erro}",
                notification.Codigo,
                notificacao.Error.Description);
    }

    private static Result VerificarAtividadeRegular(PatrimonioMovimentadoDomainEvent notification)
    {
        var valorAbsolutoMovimentacao = Math.Abs(notification.ValorMovimentacao);

        if (valorAbsolutoMovimentacao < LimiteMinimoVerificacao)
            return Result.Success();

        if (notification.ValorMovimentacao < 0)
        {
            var patrimonioAnterior = notification.PatrimonioAtual - notification.ValorMovimentacao;
            if (valorAbsolutoMovimentacao > patrimonioAnterior * FatorSaqueAltoRisco)
                return Result.Failure(
                    FundoErrors.SaqueDeAltoRisco(valorAbsolutoMovimentacao, FatorSaqueAltoRisco, patrimonioAnterior));
        }

        return Result.Success();
    }
}
