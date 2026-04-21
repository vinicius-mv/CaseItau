using CaseItau.Application.Abstractions.Notifications;
using CaseItau.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace CaseItau.Infra.Notifications;

internal sealed class FakeNotificationService : INotificationService
{
    private readonly ILogger _logger;

    public FakeNotificationService(ILogger<FakeNotificationService> logger)
    {
        _logger = logger;
    }

    public Task<Result> EnviarAsync(string titulo, string mensagem, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("[FAKE NOTIFICATION] {Titulo}: {Mensagem}", titulo, mensagem);

        return Task.FromResult(Result.Success());
    }
}
