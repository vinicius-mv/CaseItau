namespace CaseItau.API.Extensions;

public static class HttpContextExtension
{
    public const string CorrelationIdHeader = "X-Correlation-Id";

    public static string GetCorrelationId(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))  // for tracking across services
        {
            return correlationId.ToString();
        }
        return context.TraceIdentifier;
    }
}
