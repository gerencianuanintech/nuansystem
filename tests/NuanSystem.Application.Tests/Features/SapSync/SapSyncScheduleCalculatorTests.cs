using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Scheduling;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncScheduleCalculatorTests
{
    private readonly SapSyncScheduleCalculator calculator = new();

    [Fact]
    public void Manual_NeverProducesAnAutomaticExecution()
    {
        var result = Calculate(SapSyncScheduleTypes.Manual);

        result.IsValid.Should().BeTrue();
        result.NextExecutionAtUtc.Should().BeNull();
    }

    [Fact]
    public void Interval_UsesAcceptedScheduleAnchorAndSkipsMissedSlotsWithoutDrift()
    {
        var now = Utc(2026, 7, 30, 12, 7);
        var result = calculator.CalculateNext(
            SapSyncScheduleTypes.Interval,
            30,
            null,
            null,
            Utc(2026, 7, 30, 10, 0),
            Utc(2026, 7, 30, 10, 0),
            null,
            now);

        result.IsValid.Should().BeTrue();
        result.NextExecutionAtUtc.Should().Be(Utc(2026, 7, 30, 12, 30));
    }

    [Fact]
    public void Interval_InitializesFromCurrentPollOnlyOnce()
    {
        var now = Utc(2026, 7, 30, 12, 0);
        var result = calculator.CalculateNext(
            SapSyncScheduleTypes.Interval,
            45,
            null,
            null,
            null,
            null,
            null,
            now);

        result.NextExecutionAtUtc.Should().Be(Utc(2026, 7, 30, 12, 45));
    }

    [Fact]
    public void Daily_DefaultGuayaquilZone_RollsToTheNextLocalDay()
    {
        var result = calculator.CalculateNext(
            SapSyncScheduleTypes.Daily,
            null,
            TimeSpan.FromHours(8),
            null,
            null,
            null,
            null,
            Utc(2026, 7, 30, 14, 0));

        result.IsValid.Should().BeTrue();
        result.NextExecutionAtUtc.Should().Be(Utc(2026, 7, 31, 13, 0));
    }

    [Fact]
    public void Daily_NonexistentLocalHour_IsSkipped()
    {
        var result = calculator.CalculateNext(
            SapSyncScheduleTypes.Daily,
            null,
            new TimeSpan(2, 30, 0),
            "America/New_York",
            null,
            null,
            null,
            Utc(2026, 3, 8, 6, 0));

        result.IsValid.Should().BeTrue();
        result.NextExecutionAtUtc.Should().Be(Utc(2026, 3, 9, 6, 30));
    }

    [Fact]
    public void Daily_DuplicatedLocalHour_UsesExactlyTheFirstUtcOccurrence()
    {
        var result = calculator.CalculateNext(
            SapSyncScheduleTypes.Daily,
            null,
            new TimeSpan(1, 30, 0),
            "America/New_York",
            null,
            null,
            null,
            Utc(2026, 11, 1, 4, 0));

        result.IsValid.Should().BeTrue();
        result.NextExecutionAtUtc.Should().Be(Utc(2026, 11, 1, 5, 30));
    }

    private SapSyncScheduleCalculation Calculate(string scheduleType) =>
        calculator.CalculateNext(
            scheduleType,
            null,
            null,
            null,
            null,
            null,
            null,
            Utc(2026, 7, 30, 12, 0));

    private static DateTime Utc(
        int year,
        int month,
        int day,
        int hour,
        int minute) =>
        new(year, month, day, hour, minute, 0, DateTimeKind.Utc);
}
