using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncScheduler(
    ISapSyncScheduleRepository repository,
    IEnumerable<ISapSyncEntityHandler> handlers,
    SapSyncScheduleCalculator calculator,
    ISystemClock clock) : ISapSyncScheduler
{
    private readonly IReadOnlySet<string> registeredHandlers = handlers
        .Select(handler => handler.EntityCode)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public async Task<SapSyncPollResult> PollAsync(
        SapSyncScheduleCursor cursor,
        int pageSize,
        string workerInstance,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerInstance);
        pageSize = Math.Clamp(pageSize, 1, 500);
        var utcNow = clock.UtcNow.UtcDateTime;
        var page = await repository.GetCandidatesAsync(cursor, pageSize, utcNow, cancellationToken);
        if (page.Items.Count == 0 && !cursor.IsStart)
        {
            cursor = SapSyncScheduleCursor.Start;
            page = await repository.GetCandidatesAsync(cursor, pageSize, utcNow, cancellationToken);
        }

        var executions = new List<SapSyncScheduledExecutionContext>();
        var rejections = new List<SapSyncScheduleRejection>();
        var initialized = 0;

        foreach (var candidate in page.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rejection = Validate(candidate);
            if (rejection is not null)
            {
                rejections.Add(ToRejection(candidate, rejection));
                continue;
            }

            if (candidate.IsLegacyFallback)
            {
                executions.Add(ToExecution(candidate, workerInstance, utcNow, utcNow));
                continue;
            }

            var calculation = calculator.CalculateNext(
                candidate.ScheduleType,
                candidate.IntervalMinutes,
                candidate.ExecutionTime,
                candidate.TimeZoneId,
                candidate.NextExecutionAtUtc,
                candidate.LastScheduledAtUtc,
                candidate.LastExecutionAtUtc,
                utcNow);
            if (!calculation.IsValid || calculation.NextExecutionAtUtc is null)
            {
                rejections.Add(ToRejection(
                    candidate,
                    calculation.ErrorCode ?? SapSyncScheduleRejectionCodes.ScheduleInvalid));
                continue;
            }

            var observedNext = candidate.NextExecutionAtUtc;
            var reserved = await repository.TryReserveAsync(
                new SapSyncScheduleReservation(
                    candidate.ScheduleId!.Value,
                    candidate.ScheduleRowVersion!,
                    utcNow,
                    observedNext,
                    observedNext,
                    calculation.NextExecutionAtUtc.Value),
                cancellationToken);
            if (!reserved)
            {
                rejections.Add(ToRejection(
                    candidate,
                    SapSyncScheduleRejectionCodes.ConcurrentReservation));
                continue;
            }

            if (observedNext is null)
            {
                initialized++;
                continue;
            }

            executions.Add(ToExecution(
                candidate,
                workerInstance,
                DateTime.SpecifyKind(observedNext.Value, DateTimeKind.Utc),
                utcNow));
        }

        var nextCursor = page.Items.Count == pageSize
            ? page.Items.Last().Cursor
            : SapSyncScheduleCursor.Start;
        return new SapSyncPollResult(
            executions,
            rejections,
            nextCursor,
            page.EnabledCompanyCount,
            initialized);
    }

    private string? Validate(SapSyncScheduleCandidate candidate)
    {
        if (!candidate.ProfileIsActive
            || !candidate.EntityIsActive
            || !candidate.ScheduleIsActive
            || (candidate.IsLegacyFallback && !candidate.LegacyFallbackEnabled))
        {
            return SapSyncScheduleRejectionCodes.Inactive;
        }

        if (!candidate.IsLegacyFallback
            && candidate.ScheduleType.Equals(SapSyncScheduleTypes.Manual, StringComparison.OrdinalIgnoreCase))
        {
            return SapSyncScheduleRejectionCodes.Manual;
        }

        if (candidate.Direction == SapSyncDirection.Both)
        {
            return SapSyncScheduleRejectionCodes.BothUnsupported;
        }

        if (candidate.EntityCode.Equals(
                SapSyncEntityCode.PurchaseOrders,
                StringComparison.OrdinalIgnoreCase))
        {
            return SapSyncScheduleRejectionCodes.PurchaseOrdersUnsupported;
        }

        if (!candidate.CapabilityIsActive
            || !candidate.CapabilityIsImplemented
            || !registeredHandlers.Contains(candidate.EntityCode))
        {
            return SapSyncScheduleRejectionCodes.HandlerNotImplemented;
        }

        if (candidate.Direction != SapSyncDirection.SapToErp
            || !candidate.SupportsSapToErp)
        {
            return SapSyncScheduleRejectionCodes.DirectionUnsupported;
        }

        var modeSupported =
            candidate.SyncMode.Equals(SapSyncModes.Full, StringComparison.OrdinalIgnoreCase)
                ? candidate.SupportsFull
                : candidate.SyncMode.Equals(SapSyncModes.Incremental, StringComparison.OrdinalIgnoreCase)
                  && candidate.SupportsIncremental;
        if (!modeSupported)
        {
            return SapSyncScheduleRejectionCodes.ModeUnsupported;
        }

        if (!candidate.IsLegacyFallback
            && (candidate.ScheduleId is null
                || candidate.ScheduleRowVersion is not { Length: 8 }
                || !candidate.PreventConcurrentExecutions))
        {
            return SapSyncScheduleRejectionCodes.ScheduleInvalid;
        }

        return null;
    }

    private static SapSyncScheduledExecutionContext ToExecution(
        SapSyncScheduleCandidate candidate,
        string workerInstance,
        DateTime scheduledForAtUtc,
        DateTime utcNow)
    {
        var executionUid = Guid.NewGuid();
        return new SapSyncScheduledExecutionContext(
            executionUid,
            $"sap-schedule-{executionUid:N}",
            candidate.CandidateSource,
            candidate.CompanyId,
            candidate.CompanyCode,
            candidate.ProfileId,
            candidate.ProfileCode,
            candidate.ProfileName,
            candidate.ProfileEntityId,
            candidate.EntityCode,
            candidate.Direction,
            candidate.SyncMode,
            candidate.BatchSize,
            candidate.MaxAttempts,
            candidate.ExecutionOrder,
            candidate.ContinueOnError,
            candidate.ExecutionTimeoutMinutes,
            candidate.ScheduleId,
            candidate.ScheduleType,
            candidate.TimeZoneId,
            scheduledForAtUtc,
            workerInstance,
            candidate.CompatibilityVersion,
            candidate.RequiredSuccessfulCycles);
    }

    private static SapSyncScheduleRejection ToRejection(
        SapSyncScheduleCandidate candidate,
        string code) =>
        new(
            candidate.CompanyId,
            candidate.CompanyCode,
            candidate.ProfileCode,
            candidate.EntityCode,
            code);
}
