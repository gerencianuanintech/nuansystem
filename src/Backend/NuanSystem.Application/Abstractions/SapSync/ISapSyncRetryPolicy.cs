using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncRetryPolicy
{
    SapSyncRetryDecision Evaluate(string? errorCode, string? errorMessage, Exception? exception, int attemptCount, int maxRetryCount, int backoffSeconds, DateTime utcNow);
}
