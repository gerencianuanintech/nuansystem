using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncRetryPolicy : ISapSyncRetryPolicy
{
    private static readonly string[] RetryableMarkers = ["timeout", "httprequestexception", "taskcanceledexception", "http 408", "http 429", "http 500", "http 502", "http 503", "http 504", "deadlock", "hana"];
    private static readonly string[] NonRetryableMarkers = ["datos obligatorios", "mandatory", "duplicado", "duplicate", "conflict", "validacion de negocio"];

    public SapSyncRetryDecision Evaluate(string? errorCode, string? errorMessage, Exception? exception, int attemptCount, int maxRetryCount, int backoffSeconds, DateTime utcNow)
    {
        var marker = $"{errorCode} {errorMessage} {exception?.GetType().Name} {exception?.Message}";
        var nonRetryable = NonRetryableMarkers.Any(item => marker.Contains(item, StringComparison.OrdinalIgnoreCase));
        var retryable = !nonRetryable && RetryableMarkers.Any(item => marker.Contains(item, StringComparison.OrdinalIgnoreCase));

        if (!retryable)
        {
            return new SapSyncRetryDecision(false, true, null, "Error no reintentable.");
        }

        if (attemptCount >= maxRetryCount)
        {
            return new SapSyncRetryDecision(true, true, null, "Maximo de reintentos superado.");
        }

        var delaySeconds = Math.Min(3600, Math.Pow(2, Math.Max(0, attemptCount)) * Math.Max(1, backoffSeconds));
        return new SapSyncRetryDecision(true, false, utcNow.AddSeconds(delaySeconds), "Reintento programado.");
    }
}
