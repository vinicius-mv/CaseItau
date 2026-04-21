using CaseItau.Domain.Abstractions;

namespace CaseItau.Application.Abstractions.Notifications;

public interface INotificationService
{
    Task<Result> EnviarAsync(string titulo, string mensagem, CancellationToken cancellationToken = default);
}
