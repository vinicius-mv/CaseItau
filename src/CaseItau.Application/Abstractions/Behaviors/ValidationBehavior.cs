using CaseItau.Application.Abstractions.Messaging;
using CaseItau.Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CaseItau.Application.Abstractions.Behaviors;

public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseCommand
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToList();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationFailures = await ValidateAsync(request);

        if (validationFailures.Length != 0)
        {
            var validationErrors = CreateValidationError(validationFailures);
            throw new Exceptions.ValidationException(validationErrors);
        }
        return await next();
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
    {
        if (_validators.Count == 0) 
            return Array.Empty<ValidationFailure>();

        var context = new ValidationContext<TRequest>(request);

        var validationResultTasks = _validators.Select(validator => validator.ValidateAsync(context));
        var validationResults = await Task.WhenAll(validationResultTasks);

        var validationFailures = validationResults.Where(vr => !vr.IsValid).SelectMany(vr => vr.Errors);

        return validationFailures.ToArray();
    }

    private static IEnumerable<ValidationError> CreateValidationError(IEnumerable<ValidationFailure> validationFailures)
    {
        foreach (var validationFailure in validationFailures)
        {
            yield return new ValidationError(validationFailure.PropertyName, validationFailure.ErrorMessage);
        }
    }
}
