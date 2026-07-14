using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Features.Sync.Execution.Services;

public sealed class SyncScheduleCalculator : ISyncScheduleCalculator
{
    public DateTimeOffset? CalculateNextExecution(
        SyncScheduleDefinition schedule,
        DateTimeOffset referenceTimeUtc)
    {
        if (string.Equals(schedule.ScheduleType, "Manual", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (string.Equals(schedule.ScheduleType, "Interval", StringComparison.OrdinalIgnoreCase))
        {
            if (!schedule.IntervalMinutes.HasValue || schedule.IntervalMinutes.Value <= 0)
            {
                return null;
            }

            var interval = TimeSpan.FromMinutes(schedule.IntervalMinutes.Value);
            var next = (schedule.LastSuccessfulScheduledExecutionAt ?? schedule.ConfiguredAt).ToUniversalTime() + interval;
            if (next > referenceTimeUtc)
            {
                return next;
            }

            var elapsedTicks = referenceTimeUtc.UtcTicks - next.UtcTicks;
            var intervalsToSkip = (elapsedTicks / interval.Ticks) + 1;
            return next.AddTicks(interval.Ticks * intervalsToSkip);
        }

        if (string.Equals(schedule.ScheduleType, "Daily", StringComparison.OrdinalIgnoreCase))
        {
            if (!schedule.ExecutionTime.HasValue)
            {
                return null;
            }

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId);
            var referenceLocal = TimeZoneInfo.ConvertTime(referenceTimeUtc, timeZone);
            var localDate = DateOnly.FromDateTime(referenceLocal.DateTime);
            var executionTime = TimeOnly.FromTimeSpan(schedule.ExecutionTime.Value);
            var candidateLocal = localDate.ToDateTime(executionTime, DateTimeKind.Unspecified);

            if (candidateLocal <= referenceLocal.DateTime)
            {
                candidateLocal = candidateLocal.AddDays(1);
            }

            var candidateUtc = TimeZoneInfo.ConvertTimeToUtc(candidateLocal, timeZone);
            return new DateTimeOffset(candidateUtc, TimeSpan.Zero);
        }

        return null;
    }
}
