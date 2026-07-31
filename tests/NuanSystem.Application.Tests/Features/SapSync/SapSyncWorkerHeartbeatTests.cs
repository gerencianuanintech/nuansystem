using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.Operations;
using NuanSystem.Application.Features.SapSync.Scheduling;
using NuanSystem.SyncWorker.Options;
using NuanSystem.SyncWorker.Services;
using NuanSystem.SyncWorker.Workers;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncWorkerHeartbeatTests
{
    [Fact]
    public void RuntimeState_ExposesEveryApprovedLifecycleTransition()
    {
        var runtime = new SapSyncWorkerRuntimeState();
        runtime.Snapshot().LifecycleState.Should().Be(WorkerLifecycleStates.Starting);

        runtime.MarkStarted(Utc(12, 0));
        runtime.Snapshot().LifecycleState.Should().Be(WorkerLifecycleStates.Running);

        runtime.MarkCycleStarted(Utc(12, 1));
        runtime.MarkCycleCompleted(Utc(12, 2), 1000, true, 2);
        var running = runtime.Snapshot();
        running.LastCycleResult.Should().Be("Succeeded");
        running.LastSuccessfulCycleAtUtc.Should().Be(Utc(12, 2));
        running.EnabledCompanyCount.Should().Be(2);

        runtime.MarkStopping();
        runtime.Snapshot().LifecycleState.Should().Be(WorkerLifecycleStates.Stopping);
        runtime.MarkStopped();
        runtime.Snapshot().LifecycleState.Should().Be(WorkerLifecycleStates.Stopped);

        var disabled = new SapSyncWorkerRuntimeState();
        disabled.MarkDisabled();
        disabled.Snapshot().LifecycleState.Should().Be(WorkerLifecycleStates.Disabled);

        var faulted = new SapSyncWorkerRuntimeState();
        faulted.MarkFaulted();
        faulted.Snapshot().LifecycleState.Should().Be(WorkerLifecycleStates.Faulted);
        faulted.Snapshot().LastSafeErrorCode.Should().Be("SAP_WORKER_FATAL");
    }

    [Fact]
    public void LogicalIdentity_IsStableHashedAndDoesNotExposeRawInput()
    {
        var first = SapSyncWorkerRuntimeState.CreateStorageKey("HOST-01", "SAP-WORKER-A");
        var second = SapSyncWorkerRuntimeState.CreateStorageKey("HOST-01", "SAP-WORKER-A");
        var other = SapSyncWorkerRuntimeState.CreateStorageKey("HOST-01", "SAP-WORKER-B");

        first.Should().Be(second);
        first.Should().StartWith("SAP-").And.HaveLength(36);
        first.Should().NotContain("HOST-01").And.NotContain("WORKER");
        other.Should().NotBe(first);
    }

    [Fact]
    public void RuntimeState_SanitizesCurrentProfileAndEntity()
    {
        var runtime = new SapSyncWorkerRuntimeState();

        runtime.SetCurrent(1, " DEMO !! ", "PROFILE @ 1", "Items # 2");

        var snapshot = runtime.Snapshot();
        snapshot.CurrentCompanyCode.Should().Be("DEMO");
        snapshot.CurrentJob.Should().Be("PROFILE1/Items2");
        snapshot.ActiveLeaseCount.Should().Be(1);

        runtime.ClearCurrent();
        runtime.Snapshot().CurrentJob.Should().BeNull();
        runtime.Snapshot().ActiveLeaseCount.Should().Be(0);
    }

    [Fact]
    public void WorkerVersion_UsesInformationalVersion()
    {
        var assembly = typeof(SapSyncWorker).Assembly;
        var expected = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        SapSyncWorkerRuntimeState.ResolveVersion(assembly)
            .Should().Be(expected);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationArrivesDuringPoll_CompletesWithoutTaskCanceledException()
    {
        using var cancellation = new CancellationTokenSource();
        var scheduler = Substitute.For<ISapSyncScheduler>();
        scheduler.PollAsync(
                Arg.Any<SapSyncScheduleCursor>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                cancellation.Cancel();
                return Task.FromCanceled<SapSyncPollResult>(cancellation.Token);
            });
        var heartbeat = Substitute.For<IWorkerHeartbeatService>();
        var worker = CreateWorker(
            scheduler,
            heartbeat,
            new SapSyncWorkerRuntimeState(),
            enabled: true);

        var execution = InvokeExecuteAsync(worker, cancellation.Token);
        var action = async () => await execution;

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteAsync_WhenDisabled_ReportsSharedSapDisabledHeartbeat()
    {
        var scheduler = Substitute.For<ISapSyncScheduler>();
        var heartbeats = new List<WorkerHeartbeatDto>();
        var heartbeat = Substitute.For<IWorkerHeartbeatService>();
        heartbeat.BeatAsync(
                Arg.Do<WorkerHeartbeatDto>(heartbeats.Add),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        var worker = CreateWorker(
            scheduler,
            heartbeat,
            new SapSyncWorkerRuntimeState(),
            enabled: false);

        await InvokeExecuteAsync(worker, CancellationToken.None);

        heartbeats.Should().Contain(item =>
            item.WorkerType == WorkerTypes.SapSync
            && item.LifecycleState == WorkerLifecycleStates.Disabled
            && !item.IsEnabled);
        await scheduler.DidNotReceiveWithAnyArgs()
            .PollAsync(default, default, default!, default);
    }

    private static SapSyncWorker CreateWorker(
        ISapSyncScheduler scheduler,
        IWorkerHeartbeatService heartbeat,
        SapSyncWorkerRuntimeState runtime,
        bool enabled)
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(ISapSyncScheduler)).Returns(scheduler);
        provider.GetService(typeof(IWorkerHeartbeatService)).Returns(heartbeat);
        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(provider);
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);

        return new SapSyncWorker(
            scopeFactory,
            Options.Create(new WorkerOptions
            {
                Enabled = enabled,
                InstanceName = "SAP-WORKER-TEST",
                LoopDelaySeconds = 1
            }),
            Options.Create(new SapSyncOptions
            {
                SchedulerPageSize = 10,
                LockTimeoutMinutes = 10,
                LockRenewalSeconds = 60
            }),
            runtime,
            Substitute.For<ILogger<SapSyncWorker>>());
    }

    private static Task InvokeExecuteAsync(
        SapSyncWorker worker,
        CancellationToken cancellationToken)
    {
        var method = typeof(SapSyncWorker).GetMethod(
            "ExecuteAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method.Should().NotBeNull();
        return (Task)method!.Invoke(worker, [cancellationToken])!;
    }

    private static DateTime Utc(int hour, int minute) =>
        new(2026, 7, 30, hour, minute, 0, DateTimeKind.Utc);
}
