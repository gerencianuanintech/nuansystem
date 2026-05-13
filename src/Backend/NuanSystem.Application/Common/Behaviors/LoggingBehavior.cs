using MediatR;
using Microsoft.Extensions.Logging;

namespace NuanSystem.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Procesando caso de uso {RequestName}", requestName);

        var response = await next(cancellationToken);

        logger.LogInformation("Caso de uso {RequestName} completado", requestName);
        return response;
    }
}
