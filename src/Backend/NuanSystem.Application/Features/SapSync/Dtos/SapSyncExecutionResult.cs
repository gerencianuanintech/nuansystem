using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncExecutionResult(
    SapSyncStatus Status,
    string Message,
    int ProcessedCount = 0,
    int FailedCount = 0,
    string? ErrorCode = null,
    string? ErrorMessage = null,
    DateTime? NextAttemptAtUtc = null)
{
    public static SapSyncExecutionResult Skipped(string message) => new(SapSyncStatus.Skipped, message);
    public static SapSyncExecutionResult NotImplemented(string message) => new(SapSyncStatus.NotImplemented, message);
    public static SapSyncExecutionResult Failed(string message, string? errorCode = null, string? errorMessage = null)
        => new(SapSyncStatus.Failed, message, ErrorCode: errorCode, ErrorMessage: errorMessage);
}
