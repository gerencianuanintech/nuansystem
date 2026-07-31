using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Scheduling;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncExecutionLockTests
{
    [Fact]
    public async Task TryAcquireAsync_IsExclusiveForConcurrentOwners()
    {
        var repository = new InMemoryLockRepository();
        var service = CreateService(repository);

        var results = await Task.WhenAll(
            AcquireAsync(service, "worker-a"),
            AcquireAsync(service, "worker-b"));

        results.Count(item => item is not null).Should().Be(1);
        results.Count(item => item is null).Should().Be(1);
        repository.Count.Should().Be(1);
    }

    [Fact]
    public async Task RenewAsync_ExtendsOnlyTheCurrentOwnersLease()
    {
        var repository = new InMemoryLockRepository();
        var service = CreateService(repository);
        var acquired = await AcquireAsync(service, "worker-a");

        var renewed = await service.RenewAsync(
            acquired!,
            TimeSpan.FromMinutes(10));
        var foreignRenewed = await repository.RenewAsync(
            acquired!.Id,
            new string('F', 64),
            repository.UtcNow.AddMinutes(10));

        renewed.Should().BeTrue();
        foreignRenewed.Should().BeFalse();
        repository.RenewCalls.Should().Be(1);
    }

    [Fact]
    public async Task ExpiredLease_IsRecoveredExclusivelyAndPreservesLockIdentity()
    {
        var repository = new InMemoryLockRepository();
        var service = CreateService(repository);
        var first = await AcquireAsync(service, "worker-a");
        repository.UtcNow = repository.UtcNow.AddMinutes(11);

        var recovered = await AcquireAsync(service, "worker-b");
        var competing = await AcquireAsync(service, "worker-c");

        recovered.Should().NotBeNull();
        recovered!.Id.Should().Be(first!.Id);
        recovered.OwnerToken.Should().NotBe(first.OwnerToken);
        competing.Should().BeNull();
    }

    [Fact]
    public async Task Release_IsOwnerProtectedAndIdempotent()
    {
        var repository = new InMemoryLockRepository();
        var service = CreateService(repository);
        var acquired = await AcquireAsync(service, "worker-a");

        await repository.ReleaseAsync(acquired!.Id, new string('A', 64));
        repository.Count.Should().Be(1);

        await service.ReleaseAsync(acquired);
        await service.ReleaseAsync(acquired);

        repository.Count.Should().Be(0);
    }

    [Fact]
    public async Task LeaseCoordinator_RenewsWhilePreparationIsInProgressAndReleasesAtEnd()
    {
        var repository = new InMemoryLockRepository();
        var service = CreateService(repository);
        var preparer = new DelayedPreparer(TimeSpan.FromMilliseconds(90));
        var coordinator = new SapSyncExecutionLeaseCoordinator(service, preparer);

        var result = await coordinator.PrepareAsync(
            Context(),
            "worker-a",
            TimeSpan.FromSeconds(2),
            TimeSpan.FromMilliseconds(20));

        result.Status.Should().Be(SapSyncLeaseExecutionResult.Prepared);
        repository.RenewCalls.Should().BeGreaterThan(0);
        repository.Count.Should().Be(0);
        preparer.Calls.Should().Be(1);
    }

    [Fact]
    public async Task LeaseCoordinator_WhenForeignOwnerReplacesLease_ReturnsLeaseLost()
    {
        var repository = new InMemoryLockRepository
        {
            RejectRenewals = true
        };
        var service = CreateService(repository);
        var preparer = new DelayedPreparer(TimeSpan.FromSeconds(2));
        var coordinator = new SapSyncExecutionLeaseCoordinator(service, preparer);

        var result = await coordinator.PrepareAsync(
            Context(),
            "worker-a",
            TimeSpan.FromSeconds(2),
            TimeSpan.FromMilliseconds(20));

        result.Status.Should().Be(SapSyncLeaseExecutionResult.LeaseLost);
        result.SafeCode.Should().Be("SAP_SYNC_LOCK_LEASE_LOST");
        preparer.WasCancelled.Should().BeTrue();
    }

    [Fact]
    public async Task ScheduledExecutionPreparer_ValidatesContextAtThePhaseBoundary()
    {
        var preparer = new SapSyncScheduledExecutionPreparer();

        await preparer.Invoking(service =>
                service.PrepareAsync(Context() with { ExecutionUid = Guid.Empty }))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        await preparer.PrepareAsync(Context());
    }

    private static SapSyncLockService CreateService(
        InMemoryLockRepository repository)
    {
        var clock = Substitute.For<ISystemClock>();
        clock.UtcNow.Returns(_ => new DateTimeOffset(repository.UtcNow));
        return new SapSyncLockService(repository, clock);
    }

    private static Task<SapSyncLockDto?> AcquireAsync(
        ISapSyncLockService service,
        string worker) =>
        service.TryAcquireForExecutionAsync(
            1,
            "Suppliers",
            SapSyncDirection.SapToErp,
            worker,
            $"correlation-{worker}",
            Guid.NewGuid(),
            TimeSpan.FromMinutes(10));

    private static SapSyncScheduledExecutionContext Context() =>
        new(
            Guid.NewGuid(),
            "correlation-1",
            SapSyncScheduleCandidateSources.Profile,
            1,
            "DEMO",
            10,
            "PROFILE-1",
            "Profile 1",
            100,
            "Suppliers",
            SapSyncDirection.SapToErp,
            SapSyncModes.Full,
            100,
            3,
            10,
            true,
            15,
            1000,
            SapSyncScheduleTypes.Interval,
            "America/Guayaquil",
            DateTime.UtcNow,
            "worker-a",
            null,
            0);

    private sealed class DelayedPreparer(TimeSpan delay)
        : ISapSyncScheduledExecutionPreparer
    {
        public int Calls { get; private set; }
        public bool WasCancelled { get; private set; }

        public async Task PrepareAsync(
            SapSyncScheduledExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                WasCancelled = true;
                throw;
            }
        }
    }

    private sealed class InMemoryLockRepository : ISapSyncLockRepository
    {
        private readonly object sync = new();
        private SapSyncLockDto? current;
        private long nextId = 1;

        public DateTime UtcNow { get; set; } =
            new(2026, 7, 30, 12, 0, 0, DateTimeKind.Utc);
        public bool RejectRenewals { get; set; }
        public int RenewCalls { get; private set; }
        public int Count { get { lock (sync) return current is null ? 0 : 1; } }

        public Task<SapSyncLockDto?> TryAcquireAsync(
            int companyId,
            string entityCode,
            SapSyncDirection direction,
            string workerInstance,
            string correlationId,
            Guid? executionUid,
            string ownerToken,
            DateTime lockExpiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            lock (sync)
            {
                if (current is not null && current.LockExpiresAtUtc > UtcNow)
                {
                    return Task.FromResult<SapSyncLockDto?>(null);
                }

                current = new SapSyncLockDto(
                    current?.Id ?? nextId++,
                    companyId,
                    entityCode,
                    direction,
                    workerInstance,
                    correlationId,
                    executionUid,
                    ownerToken,
                    UtcNow,
                    null,
                    lockExpiresAtUtc);
                return Task.FromResult<SapSyncLockDto?>(current);
            }
        }

        public Task<bool> RenewAsync(
            long id,
            string ownerToken,
            DateTime lockExpiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            lock (sync)
            {
                if (RejectRenewals
                    || current is null
                    || current.Id != id
                    || current.OwnerToken != ownerToken
                    || current.LockExpiresAtUtc <= UtcNow)
                {
                    return Task.FromResult(false);
                }

                RenewCalls++;
                current = current with
                {
                    RenewedAtUtc = UtcNow,
                    LockExpiresAtUtc = lockExpiresAtUtc
                };
                return Task.FromResult(true);
            }
        }

        public Task ReleaseAsync(
            long id,
            string ownerToken,
            CancellationToken cancellationToken = default)
        {
            lock (sync)
            {
                if (current?.Id == id && current.OwnerToken == ownerToken)
                {
                    current = null;
                }
            }

            return Task.CompletedTask;
        }

        public Task<bool> ReleaseExpiredAsync(
            long id,
            string reason,
            int? auditUserId,
            string? auditUserName,
            CancellationToken cancellationToken = default)
        {
            lock (sync)
            {
                if (current?.Id != id || current.LockExpiresAtUtc > UtcNow)
                {
                    return Task.FromResult(false);
                }

                current = null;
                return Task.FromResult(true);
            }
        }
    }
}
