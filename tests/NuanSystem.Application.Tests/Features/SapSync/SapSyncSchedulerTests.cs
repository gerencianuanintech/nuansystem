using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Scheduling;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncSchedulerTests
{
    private static readonly DateTime UtcNow =
        new(2026, 7, 30, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task PollAsync_ExcludesInactiveProfileEntityAndSchedule()
    {
        foreach (var candidate in new[]
                 {
                     Candidate() with { ProfileIsActive = false },
                     Candidate() with { EntityIsActive = false },
                     Candidate() with { ScheduleIsActive = false }
                 })
        {
            var (result, repository, handler) = await PollSingleAsync(candidate);

            result.Executions.Should().BeEmpty();
            result.Rejections.Should().ContainSingle()
                .Which.Code.Should().Be(SapSyncScheduleRejectionCodes.Inactive);
            repository.ReserveCalls.Should().Be(0);
            handler.CallCount.Should().Be(0);
        }
    }

    [Fact]
    public async Task PollAsync_RejectsManualBothPurchaseOrdersAndMissingHandler()
    {
        var items = new[]
        {
            Candidate(order: 10, entityId: 10) with
            {
                ScheduleType = SapSyncScheduleTypes.Manual,
                IntervalMinutes = null
            },
            Candidate(order: 20, entityId: 20) with
            {
                Direction = SapSyncDirection.Both
            },
            Candidate(order: 30, entityId: 30) with
            {
                EntityCode = SapSyncEntityCode.PurchaseOrders
            },
            Candidate(order: 40, entityId: 40) with
            {
                EntityCode = "MissingHandler"
            }
        };
        var repository = new FakeScheduleRepository(items);
        var handler = new RejectingHandler(SapSyncEntityCode.Suppliers);
        var scheduler = CreateScheduler(repository, handler);

        var result = await scheduler.PollAsync(
            SapSyncScheduleCursor.Start,
            20,
            "worker-a");

        result.Executions.Should().BeEmpty();
        result.Rejections.Select(item => item.Code).Should().BeEquivalentTo(
            SapSyncScheduleRejectionCodes.Manual,
            SapSyncScheduleRejectionCodes.BothUnsupported,
            SapSyncScheduleRejectionCodes.PurchaseOrdersUnsupported,
            SapSyncScheduleRejectionCodes.HandlerNotImplemented);
        handler.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task PollAsync_KeysetPagesAcrossTwoCompaniesWithoutStarvation()
    {
        var items = new[]
        {
            Candidate(companyId: 1, profileId: 11, order: 10, entityId: 101),
            Candidate(companyId: 1, profileId: 11, order: 20, entityId: 102),
            Candidate(companyId: 2, profileId: 21, order: 10, entityId: 201),
            Candidate(companyId: 2, profileId: 21, order: 20, entityId: 202)
        };
        var repository = new FakeScheduleRepository(items);
        var handler = new RejectingHandler(SapSyncEntityCode.Suppliers);
        var scheduler = CreateScheduler(repository, handler);

        var first = await scheduler.PollAsync(
            SapSyncScheduleCursor.Start,
            2,
            "worker-a");
        var second = await scheduler.PollAsync(
            first.NextCursor,
            2,
            "worker-a");
        var wrapped = await scheduler.PollAsync(
            second.NextCursor,
            2,
            "worker-a");

        first.Executions.Select(item => item.CompanyId).Should().OnlyContain(id => id == 1);
        second.Executions.Select(item => item.CompanyId).Should().OnlyContain(id => id == 2);
        first.Executions.Concat(second.Executions)
            .Select(item => item.ProfileEntityId)
            .Should().BeEquivalentTo(new long?[] { 101, 102, 201, 202 });
        second.NextCursor.Should().NotBe(SapSyncScheduleCursor.Start);
        wrapped.Executions.Select(item => item.CompanyId)
            .Should().OnlyContain(id => id == 1);
        handler.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task PollAsync_LegacyFallbackIsReadOnlyAndCarriesCompatibilityContract()
    {
        var legacy = Candidate() with
        {
            CandidateSource = SapSyncScheduleCandidateSources.LegacyFallback,
            ProfileId = null,
            ProfileEntityId = null,
            ScheduleId = null,
            ScheduleType = SapSyncScheduleCandidateSources.LegacyFallback,
            ScheduleRowVersion = null,
            LegacyFallbackEnabled = true,
            CompatibilityVersion = "Fase10.2-v1",
            RequiredSuccessfulCycles = 2,
            SortProfileId = 0
        };
        var (result, repository, handler) = await PollSingleAsync(legacy);

        result.Executions.Should().ContainSingle();
        result.Executions.Single().CandidateSource
            .Should().Be(SapSyncScheduleCandidateSources.LegacyFallback);
        result.Executions.Single().CompatibilityVersion.Should().Be("Fase10.2-v1");
        result.Executions.Single().RequiredSuccessfulCycles.Should().Be(2);
        repository.ReserveCalls.Should().Be(0, "el fallback no escribe perfiles ni settings legado");
        handler.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task PollAsync_WarehouseLegacyFallback_IsRejectedUntilProfileCutover()
    {
        var legacy = Candidate() with
        {
            CandidateSource = SapSyncScheduleCandidateSources.LegacyFallback,
            ProfileId = null,
            ProfileEntityId = null,
            EntityCode = SapSyncEntityCode.Warehouses,
            ScheduleId = null,
            ScheduleType = SapSyncScheduleCandidateSources.LegacyFallback,
            ScheduleRowVersion = null,
            LegacyFallbackEnabled = true,
            SortProfileId = 0
        };
        var repository = new FakeScheduleRepository([legacy]);
        var handler = new RejectingHandler(SapSyncEntityCode.Warehouses);

        var result = await CreateScheduler(repository, handler).PollAsync(
            SapSyncScheduleCursor.Start, 10, "worker-a");

        result.Executions.Should().BeEmpty();
        result.Rejections.Should().ContainSingle(item =>
            item.Code == SapSyncScheduleRejectionCodes.LegacyFallbackUnsupported);
        repository.ReserveCalls.Should().Be(0);
        handler.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task PollAsync_ConcurrentReservationDoesNotPrepareDuplicateContext()
    {
        var repository = new FakeScheduleRepository([Candidate()])
        {
            ReservationSucceeds = false
        };
        var scheduler = CreateScheduler(
            repository,
            new RejectingHandler(SapSyncEntityCode.Suppliers));

        var result = await scheduler.PollAsync(
            SapSyncScheduleCursor.Start,
            10,
            "worker-a");

        result.Executions.Should().BeEmpty();
        result.Rejections.Should().ContainSingle()
            .Which.Code.Should().Be(
                SapSyncScheduleRejectionCodes.ConcurrentReservation);
    }

    [Fact]
    public async Task PollAsync_NullNextExecutionInitializesButDoesNotRunImmediately()
    {
        var initial = Candidate() with
        {
            NextExecutionAtUtc = null,
            LastScheduledAtUtc = null,
            LastExecutionAtUtc = null
        };
        var (result, repository, _) = await PollSingleAsync(initial);

        result.Executions.Should().BeEmpty();
        result.InitializedScheduleCount.Should().Be(1);
        repository.Reservations.Should().ContainSingle();
        repository.Reservations.Single().ScheduledAtUtc.Should().BeNull();
        repository.Reservations.Single().NextExecutionAtUtc
            .Should().Be(UtcNow.AddMinutes(30));
    }

    private static async Task<(
        SapSyncPollResult Result,
        FakeScheduleRepository Repository,
        RejectingHandler Handler)> PollSingleAsync(
        SapSyncScheduleCandidate candidate)
    {
        var repository = new FakeScheduleRepository([candidate]);
        var handler = new RejectingHandler(SapSyncEntityCode.Suppliers);
        var scheduler = CreateScheduler(repository, handler);
        var result = await scheduler.PollAsync(
            SapSyncScheduleCursor.Start,
            10,
            "worker-a");
        return (result, repository, handler);
    }

    private static SapSyncScheduler CreateScheduler(
        ISapSyncScheduleRepository repository,
        params ISapSyncEntityHandler[] handlers)
    {
        var clock = Substitute.For<ISystemClock>();
        clock.UtcNow.Returns(new DateTimeOffset(UtcNow));
        return new SapSyncScheduler(
            repository,
            handlers,
            new SapSyncScheduleCalculator(),
            clock);
    }

    private static SapSyncScheduleCandidate Candidate(
        int companyId = 1,
        long profileId = 10,
        int order = 10,
        long entityId = 100) =>
        new(
            SapSyncScheduleCandidateSources.Profile,
            companyId,
            $"COMPANY-{companyId}",
            profileId,
            $"PROFILE-{profileId}",
            $"Profile {profileId}",
            true,
            entityId,
            SapSyncEntityCode.Suppliers,
            SapSyncDirection.SapToErp,
            SapSyncModes.Full,
            100,
            3,
            order,
            true,
            15,
            true,
            entityId + 1000,
            SapSyncScheduleTypes.Interval,
            30,
            null,
            "America/Guayaquil",
            true,
            UtcNow.AddMinutes(-1),
            UtcNow.AddMinutes(-31),
            UtcNow.AddMinutes(-31),
            true,
            [1, 2, 3, 4, 5, 6, 7, 8],
            true,
            false,
            true,
            true,
            true,
            true,
            false,
            null,
            0,
            profileId,
            entityId);

    private sealed class RejectingHandler(string entityCode) : ISapSyncEntityHandler
    {
        public string EntityCode { get; } = entityCode;
        public int CallCount { get; private set; }

        public Task<SapSyncExecutionResult> ImportFromSapAsync(
            SapSyncExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            throw new InvalidOperationException("El scheduler no debe invocar handlers en Fase 10.4.");
        }

        public Task<SapSyncExecutionResult> ExportToSapAsync(
            SapSyncExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            throw new InvalidOperationException("El scheduler no debe invocar handlers en Fase 10.4.");
        }
    }

    private sealed class FakeScheduleRepository(
        IEnumerable<SapSyncScheduleCandidate> candidates) : ISapSyncScheduleRepository
    {
        private readonly IReadOnlyCollection<SapSyncScheduleCandidate> candidates =
            candidates.OrderBy(item => item.CompanyId)
                .ThenBy(item => item.SortProfileId)
                .ThenBy(item => item.ExecutionOrder)
                .ThenBy(item => item.SortEntityId)
                .ToArray();

        public bool ReservationSucceeds { get; set; } = true;
        public int ReserveCalls => Reservations.Count;
        public List<SapSyncScheduleReservation> Reservations { get; } = [];

        public Task<SapSyncScheduleCandidatePage> GetCandidatesAsync(
            SapSyncScheduleCursor cursor,
            int pageSize,
            DateTime utcNow,
            CancellationToken cancellationToken = default)
        {
            var page = candidates
                .Where(item => IsAfter(item.Cursor, cursor))
                .Take(pageSize)
                .ToArray();
            return Task.FromResult(new SapSyncScheduleCandidatePage(
                page,
                candidates.Select(item => item.CompanyId).Distinct().Count()));
        }

        public Task<bool> TryReserveAsync(
            SapSyncScheduleReservation reservation,
            CancellationToken cancellationToken = default)
        {
            Reservations.Add(reservation);
            return Task.FromResult(ReservationSucceeds);
        }

        private static bool IsAfter(
            SapSyncScheduleCursor value,
            SapSyncScheduleCursor cursor) =>
            value.CompanyId > cursor.CompanyId
            || value.CompanyId == cursor.CompanyId
            && (value.ProfileId > cursor.ProfileId
                || value.ProfileId == cursor.ProfileId
                && (value.ExecutionOrder > cursor.ExecutionOrder
                    || value.ExecutionOrder == cursor.ExecutionOrder
                    && value.EntityId > cursor.EntityId));
    }
}
