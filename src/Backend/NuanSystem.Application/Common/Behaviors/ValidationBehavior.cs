using FluentValidation;
using MediatR;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .Select(error => new ApiError(error.ErrorCode, error.ErrorMessage, error.PropertyName))
            .ToArray();

        if (errors.Length > 0)
        {
            throw new ApplicationValidationException(errors);
        }

        return await next(cancellationToken);
    }
}
