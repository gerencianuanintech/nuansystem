using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Services;
using NuanSystem.Domain.Tenancy;
using NSubstitute;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncDirectionalDispatchTests
{
    private static readonly SapSyncCompanyDto Company = new(1, "TEST", "Test", (SapIntegrationMode)1, true);
    private static readonly SapSyncEntitySettingsDto Settings = new(1, 1, "TEST", "Suppliers",
        SapSyncDirection.SapToErp, true, 100, 3, 10, DateTime.UtcNow, null);

    [Theory]
    [InlineData(SapSyncDirection.Both)]
    [InlineData(SapSyncDirection.ErpToSap)]
    [InlineData((SapSyncDirection)999)]
    public async Task Unsupported_direction_never_imports_or_acquires_a_lock(SapSyncDirection direction)
    {
        var importer = SapSyncRegistryFixture.Importer();
        var locks = Substitute.For<ISapSyncLockService>();
        var logs = Substitute.For<ISapSyncLogService>();
        var registry = new SapSyncProcessorRegistry([importer], [], [], []);
        var orchestrator = new SapSyncOrchestrator(registry, locks, logs);
        var result = await orchestrator.ExecuteAsync(Company, Settings, direction, "worker", TimeSpan.FromMinutes(1));
        Assert.Equal(SapSyncStatus.NotImplemented, result.Status);
        Assert.DoesNotContain(importer.ReceivedCalls(), call => call.GetMethodInfo().Name == "ImportFromSapAsync");
        Assert.Empty(locks.ReceivedCalls());
        await logs.Received(1).WriteAsync(Arg.Is<SapSyncLogWriteDto>(log => log.Direction == direction && log.Status == SapSyncStatus.NotImplemented), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(SapSyncStatus.Synced, false)]
    [InlineData(SapSyncStatus.Skipped, false)]
    [InlineData(SapSyncStatus.Failed, false)]
    [InlineData(SapSyncStatus.Synced, true)]
    [InlineData(SapSyncStatus.Failed, true)]
    public async Task Directional_dispatch_preserves_results_and_releases_lock(SapSyncStatus status, bool export)
    {
        var importer = SapSyncRegistryFixture.Importer();
        var exporter = Substitute.For<ISapSyncExporter>();
        exporter.EntityCode.Returns("Suppliers");
        exporter.SupportedModes.Returns(new[] { "Full" });
        var expected = new SapSyncExecutionResult(status, "result", 4, status == SapSyncStatus.Failed ? 1 : 0);
        importer.ImportFromSapAsync(Arg.Any<SapSyncExecutionContext>(), Arg.Any<CancellationToken>()).Returns(expected);
        exporter.ExportToSapAsync(Arg.Any<SapSyncExecutionContext>(), Arg.Any<CancellationToken>()).Returns(expected);
        var registry = new SapSyncProcessorRegistry([importer], [exporter], [], []);
        var locks = Substitute.For<ISapSyncLockService>();
        var logs = Substitute.For<ISapSyncLogService>();
        var direction = export ? SapSyncDirection.ErpToSap : SapSyncDirection.SapToErp;
        var lease = new SapSyncLockDto(1, 1, "Suppliers", direction, "worker", "correlation", null,
            "owner", DateTime.UtcNow, null, DateTime.UtcNow.AddMinutes(1));
        locks.TryAcquireAsync(1, "Suppliers", direction, "worker", Arg.Any<string>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(lease);
        var result = await new SapSyncOrchestrator(registry, locks, logs)
            .ExecuteAsync(Company, Settings, direction, "worker", TimeSpan.FromMinutes(1));
        Assert.Same(expected, result);
        if (export)
        {
            await exporter.Received(1).ExportToSapAsync(Arg.Is<SapSyncExecutionContext>(c => c.Operation == SapSyncOperation.Export && c.Direction == direction), Arg.Any<CancellationToken>());
            Assert.DoesNotContain(importer.ReceivedCalls(), call => call.GetMethodInfo().Name == "ImportFromSapAsync");
        }
        else
        {
            await importer.Received(1).ImportFromSapAsync(Arg.Is<SapSyncExecutionContext>(c => c.Operation == SapSyncOperation.Import && c.Direction == direction), Arg.Any<CancellationToken>());
            Assert.DoesNotContain(exporter.ReceivedCalls(), call => call.GetMethodInfo().Name == "ExportToSapAsync");
        }
        await locks.Received(1).ReleaseAsync(lease, Arg.Any<CancellationToken>());
        await logs.Received(1).WriteAsync(Arg.Is<SapSyncLogWriteDto>(log => log.Status == status), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancelled_request_does_not_dispatch_or_acquire_lock()
    {
        var locks = Substitute.For<ISapSyncLockService>();
        var logs = Substitute.For<ISapSyncLogService>();
        var registry = new SapSyncProcessorRegistry([], [], [], []);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new SapSyncOrchestrator(registry, locks, logs)
            .ExecuteAsync(Company, Settings, SapSyncDirection.SapToErp, "worker", TimeSpan.FromMinutes(1), cancellation.Token));
        Assert.Empty(locks.ReceivedCalls());
        Assert.Empty(logs.ReceivedCalls());
    }
}
