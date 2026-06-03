namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncRetryDecision(
    bool IsRetryable,
    bool MoveToDeadLetter,
    DateTime? NextAttemptAtUtc,
    string Reason);
