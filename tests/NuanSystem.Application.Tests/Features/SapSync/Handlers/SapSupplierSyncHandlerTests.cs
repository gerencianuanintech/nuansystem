using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Handlers;

namespace NuanSystem.Application.Tests.Features.SapSync.Handlers;

public sealed class SapSupplierSyncHandlerTests
{
    [Fact]
    public async Task ImportFromSapAsync_CallsImportAsync_WithWorkerOptions()
    {
        var service = Substitute.For<ISapSupplierImportService>();
        var context = Context();
        service.ImportAsync(context.CompanyId, Arg.Any<SapSupplierImportOptions>(), Arg.Any<CancellationToken>())
            .Returns(Summary(totalRead: 1, failed: 0));
        var handler = new SapSupplierSyncHandler(service);

        await handler.ImportFromSapAsync(context);

        await service.Received(1).ImportAsync(
            context.CompanyId,
            Arg.Is<SapSupplierImportOptions>(options =>
                options.AuditUserId == null
                && options.AuditUserName == "SAP Sync Worker"
                && !options.WritePublicSapLog
                && options.WriteInbox
                && options.UseIncrementalWatermark
                && options.WorkerInstance == context.WorkerInstance
                && options.CorrelationId == context.CorrelationId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportFromSapAsync_ReturnsSynced_WhenImportHasNoFailuresAndReadsSuppliers()
    {
        var service = Substitute.For<ISapSupplierImportService>();
        var context = Context();
        service.ImportAsync(context.CompanyId, Arg.Any<SapSupplierImportOptions>(), Arg.Any<CancellationToken>())
            .Returns(Summary(totalRead: 2, created: 1, updated: 1, failed: 0));
        var handler = new SapSupplierSyncHandler(service);

        var result = await handler.ImportFromSapAsync(context);

        result.Status.Should().Be(SapSyncStatus.Synced);
        result.ProcessedCount.Should().Be(2);
        result.FailedCount.Should().Be(0);
        result.ErrorCode.Should().BeNull();
    }

    [Fact]
    public async Task ImportFromSapAsync_ReturnsSkipped_WhenImportReadsNoSuppliers()
    {
        var service = Substitute.For<ISapSupplierImportService>();
        var context = Context();
        service.ImportAsync(context.CompanyId, Arg.Any<SapSupplierImportOptions>(), Arg.Any<CancellationToken>())
            .Returns(Summary(totalRead: 0, failed: 0));
        var handler = new SapSupplierSyncHandler(service);

        var result = await handler.ImportFromSapAsync(context);

        result.Status.Should().Be(SapSyncStatus.Skipped);
        result.Message.Should().Be("No hay proveedores SAP cambiados.");
        result.ProcessedCount.Should().Be(0);
        result.FailedCount.Should().Be(0);
    }

    [Fact]
    public async Task ImportFromSapAsync_ReturnsFailed_WhenImportHasFailures()
    {
        var service = Substitute.For<ISapSupplierImportService>();
        var context = Context();
        service.ImportAsync(context.CompanyId, Arg.Any<SapSupplierImportOptions>(), Arg.Any<CancellationToken>())
            .Returns(Summary(totalRead: 3, created: 1, failed: 1));
        var handler = new SapSupplierSyncHandler(service);

        var result = await handler.ImportFromSapAsync(context);

        result.Status.Should().Be(SapSyncStatus.Failed);
        result.ProcessedCount.Should().Be(3);
        result.FailedCount.Should().Be(1);
        result.ErrorCode.Should().Be("SUPPLIER_IMPORT_FAILED");
        result.ErrorMessage.Should().Be("Uno o mas proveedores no pudieron importarse.");
    }

    [Fact]
    public async Task ImportFromSapAsync_DoesNotFailGlobalExecution_WhenOnlyBusinessConflictsAreSkipped()
    {
        var service = Substitute.For<ISapSupplierImportService>();
        var context = Context();
        service.ImportAsync(context.CompanyId, Arg.Any<SapSupplierImportOptions>(), Arg.Any<CancellationToken>())
            .Returns(Summary(totalRead: 2, skipped: 1, failed: 0));
        var handler = new SapSupplierSyncHandler(service);

        var result = await handler.ImportFromSapAsync(context);

        result.Status.Should().Be(SapSyncStatus.Synced);
        result.ProcessedCount.Should().Be(2);
        result.FailedCount.Should().Be(0);
        result.ErrorCode.Should().BeNull();
    }

    [Fact]
    public async Task ExportToSapAsync_RemainsNotImplemented()
    {
        var service = Substitute.For<ISapSupplierImportService>();
        var handler = new SapSupplierSyncHandler(service);

        var result = await handler.ExportToSapAsync(Context());

        result.Status.Should().Be(SapSyncStatus.NotImplemented);
        result.Message.Should().Contain("pendiente para fase 3");
    }

    private static SapSyncExecutionContext Context()
        => new(
            CompanyId: 10,
            CompanyCode: "NUAN",
            EntityCode: SapSyncEntityCode.Suppliers,
            Direction: SapSyncDirection.SapToErp,
            Operation: SapSyncOperation.Import,
            WorkerInstance: "worker-01",
            CorrelationId: "correlation-01",
            AttemptCount: 0,
            StartedAtUtc: DateTime.UtcNow);

    private static SapSupplierImportResultDto Summary(
        int totalRead,
        int created = 0,
        int updated = 0,
        int unchanged = 0,
        int skipped = 0,
        int failed = 0)
        => new(
            totalRead,
            created,
            updated,
            unchanged,
            skipped,
            failed,
            []);
}
