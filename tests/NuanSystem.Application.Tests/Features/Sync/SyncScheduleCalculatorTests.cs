using FluentAssertions;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Services;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncScheduleCalculatorTests
{
    [Fact]
    public void CalculateNextExecution_ReturnsNull_ForManualSchedule()
    {
        var calculator = new SyncScheduleCalculator();
        var schedule = new SyncScheduleDefinition(
            1,
            "Manual",
            null,
            null,
            "America/Guayaquil",
            null,
            new DateTimeOffset(2026, 7, 11, 10, 0, 0, TimeSpan.Zero));

        var next = calculator.CalculateNextExecution(schedule, new DateTimeOffset(2026, 7, 11, 11, 0, 0, TimeSpan.Zero));

        next.Should().BeNull();
    }

    [Fact]
    public void CalculateNextExecution_UsesLastSuccessfulExecution_ForIntervalSchedule()
    {
        var calculator = new SyncScheduleCalculator();
        var schedule = new SyncScheduleDefinition(
            1,
            "Interval",
            30,
            null,
            "America/Guayaquil",
            new DateTimeOffset(2026, 7, 11, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 11, 8, 0, 0, TimeSpan.Zero));

        var next = calculator.CalculateNextExecution(schedule, new DateTimeOffset(2026, 7, 11, 10, 45, 0, TimeSpan.Zero));

        next.Should().Be(new DateTimeOffset(2026, 7, 11, 11, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void CalculateNextExecution_ConvertsDailyLocalTime_ToUtc()
    {
        var calculator = new SyncScheduleCalculator();
        var schedule = new SyncScheduleDefinition(
            1,
            "Daily",
            null,
            new TimeSpan(3, 0, 0),
            "America/Guayaquil",
            null,
            new DateTimeOffset(2026, 7, 11, 0, 0, 0, TimeSpan.Zero));

        var next = calculator.CalculateNextExecution(schedule, new DateTimeOffset(2026, 7, 11, 7, 0, 0, TimeSpan.Zero));

        next.Should().Be(new DateTimeOffset(2026, 7, 11, 8, 0, 0, TimeSpan.Zero));
    }
}
