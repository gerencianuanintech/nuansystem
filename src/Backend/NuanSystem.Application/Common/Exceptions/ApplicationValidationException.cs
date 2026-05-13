using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Common.Exceptions;

public sealed class ApplicationValidationException(IReadOnlyCollection<ApiError> errors)
    : Exception("La solicitud contiene errores de validacion.")
{
    public IReadOnlyCollection<ApiError> Errors { get; } = errors;
}
