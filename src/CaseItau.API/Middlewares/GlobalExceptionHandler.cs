using CaseItau.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CaseItau.API.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId} | Path: {Path} | Method: {Method}",
            httpContext.TraceIdentifier,
            httpContext.Request.Path,
            httpContext.Request.Method);

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx => BuildValidationProblem(validationEx, httpContext),
            _ => BuildServerErrorProblem(httpContext)
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            problemDetails.GetType(),
            cancellationToken: cancellationToken);

        return true;
    }

    private static (int StatusCode, ProblemDetails ProblemDetails) BuildValidationProblem(
        ValidationException exception,
        HttpContext httpContext)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var problemDetails = CreateProblemDetails(
            new ValidationProblemDetails(errors),
            StatusCodes.Status400BadRequest,
            title: "Validation Error",
            detail: "One or more validation errors occurred.",
            httpContext,
            includeTraceId: false);

        return (StatusCode: StatusCodes.Status400BadRequest, ProblemDetails: problemDetails);
    }

    private static (int StatusCode, ProblemDetails ProblemDetails) BuildServerErrorProblem(
        HttpContext httpContext)
    {
        var problemDetails = CreateProblemDetails(
            new ProblemDetails(),
            StatusCodes.Status500InternalServerError,
            title: "Server Error",
            detail: "An unexpected error occurred.",
            httpContext,
            includeTraceId: true);

        return (StatusCode: StatusCodes.Status500InternalServerError, ProblemDetails: problemDetails);
    }

    private static T CreateProblemDetails<T>(
        T problemDetails,
        int statusCode,
        string title,
        string detail,
        HttpContext httpContext,
        bool includeTraceId) where T : ProblemDetails
    {
        problemDetails.Status = statusCode;
        problemDetails.Title = title;
        problemDetails.Detail = detail;
        problemDetails.Instance = httpContext.Request.Path;

        if (includeTraceId)
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        return problemDetails;
    }
}