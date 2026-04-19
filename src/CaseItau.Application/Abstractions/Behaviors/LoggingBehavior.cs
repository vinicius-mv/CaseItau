using CaseItau.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CaseItau.Application.Abstractions.Behaviors;


public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest  // IBaseRequest is a market interface to ensure that only commands are processed by this behavior
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var commandName = request.GetType().Name;

        try
        {
            _logger.LogInformation("Executing commnad {Command}", commandName);

            var result = await next();

            _logger.LogInformation("Commnad {Command} processed succesfully", commandName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Command {Command} processing failed", commandName);
            throw;
        }
    }
}
