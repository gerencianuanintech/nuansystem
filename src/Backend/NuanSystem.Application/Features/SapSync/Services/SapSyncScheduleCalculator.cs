using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Profiles.Services;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncScheduleCalculator
{
    private const int MaximumDailySearchDays = 370;

    public SapSyncScheduleCalculation CalculateNext(
        string scheduleType,
        int? intervalMinutes,
        TimeSpan? executionTime,
        string? timeZoneId,
        DateTime? nextExecutionAtUtc,
        DateTime? lastScheduledAtUtc,
        DateTime? lastExecutionAtUtc,
        DateTime utcNow)
    {
        utcNow = AsUtc(utcNow);

        if (scheduleType.Equals(SapSyncScheduleTypes.Manual, StringComparison.OrdinalIgnoreCase))
        {
            return new SapSyncScheduleCalculation(true, null);
        }

        if (scheduleType.Equals(SapSyncScheduleTypes.Interval, StringComparison.OrdinalIgnoreCase))
        {
            if (intervalMinutes is not >= 1)
            {
                return Invalid();
            }

            var interval = TimeSpan.FromMinutes(intervalMinutes.Value);
            var anchor = nextExecutionAtUtc.HasValue
                ? AsUtc(nextExecutionAtUtc.Value)
                : lastScheduledAtUtc.HasValue
                    ? AsUtc(lastScheduledAtUtc.Value)
                    : lastExecutionAtUtc.HasValue
                        ? AsUtc(lastExecutionAtUtc.Value)
                        : utcNow;

            var next = nextExecutionAtUtc.HasValue ? anchor : anchor.Add(interval);
            if (next <= utcNow)
            {
                var elapsedTicks = utcNow.Ticks - next.Ticks;
                var intervalsToSkip = (elapsedTicks / interval.Ticks) + 1;
                try
                {
                    next = next.AddTicks(checked(interval.Ticks * intervalsToSkip));
                }
                catch (OverflowException)
                {
                    return Invalid();
                }
            }

            return new SapSyncScheduleCalculation(true, next);
        }

        if (!scheduleType.Equals(SapSyncScheduleTypes.Daily, StringComparison.OrdinalIgnoreCase)
            || executionTime is null
            || executionTime < TimeSpan.Zero
            || executionTime >= TimeSpan.FromDays(1))
        {
            return Invalid();
        }

        TimeZoneInfo timeZone;
        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                string.IsNullOrWhiteSpace(timeZoneId)
                    ? SapSyncProfileValidationService.DefaultTimeZoneId
                    : timeZoneId.Trim());
        }
        catch (TimeZoneNotFoundException)
        {
            return Invalid();
        }
        catch (InvalidTimeZoneException)
        {
            return Invalid();
        }

        var reference = new DateTimeOffset(utcNow, TimeSpan.Zero);
        var referenceLocal = TimeZoneInfo.ConvertTime(reference, timeZone);
        for (var offset = 0; offset < MaximumDailySearchDays; offset++)
        {
            var localDate = DateOnly.FromDateTime(referenceLocal.DateTime).AddDays(offset);
            var candidateLocal = DateTime.SpecifyKind(
                localDate.ToDateTime(TimeOnly.FromTimeSpan(executionTime.Value)),
                DateTimeKind.Unspecified);

            if (timeZone.IsInvalidTime(candidateLocal))
            {
                continue;
            }

            var candidateUtc = ToDeterministicUtc(candidateLocal, timeZone);
            if (candidateUtc > utcNow)
            {
                return new SapSyncScheduleCalculation(true, candidateUtc);
            }
        }

        return Invalid();
    }

    private static DateTime ToDeterministicUtc(DateTime local, TimeZoneInfo timeZone)
    {
        if (!timeZone.IsAmbiguousTime(local))
        {
            return TimeZoneInfo.ConvertTimeToUtc(local, timeZone);
        }

        // En una hora local duplicada se elige una sola ocurrencia: la primera en UTC.
        var offset = timeZone.GetAmbiguousTimeOffsets(local).Max();
        return DateTime.SpecifyKind(local - offset, DateTimeKind.Utc);
    }

    private static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    private static SapSyncScheduleCalculation Invalid() =>
        new(false, null, SapSyncScheduleRejectionCodes.ScheduleInvalid);
}
