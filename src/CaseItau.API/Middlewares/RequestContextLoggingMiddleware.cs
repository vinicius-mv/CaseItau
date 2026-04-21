using CaseItau.API.Extensions;
using Serilog.Context;

namespace CaseItau.API.Middlewares;

public class RequestContextLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestContextLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var correlationId = context.GetCorrelationId();
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            return _next(context);
        }
    }
}
