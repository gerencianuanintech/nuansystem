using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Warehouses.Handlers;
using NuanSystem.Application.Features.SapSync.Warehouses.Services;
using MediatR;

namespace NuanSystem.Application.Tests.Features.SapSync.Warehouses;

public sealed class SapWarehouseSyncHandlerTests
{
    [Fact]
    public async Task ImportFromSapAsync_ProcessesRowsInCodeOrderAndContinuesAfterOneFailure()
    {
        var reader = Substitute.For<ISapWarehouseReader>();
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        reader.GetWarehousesAsync(10, Arg.Any<CancellationToken>()).Returns([
            Record("WH-02"),
            Record("WH-01")
        ]);
        var calls = 0;
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            calls++;
            if (calls == 1)
            {
                throw new TimeoutException("sensitive database detail");
            }

            return new[] { Local("WH-02") };
        });
        var handler = new SapWarehouseSyncHandler(
            reader,
            new SapWarehouseRecordProcessor(repository, sender));

        var result = await handler.ImportFromSapAsync(Context());

        result.Status.Should().Be(SapSyncStatus.Failed);
        result.ProcessedCount.Should().Be(2);
        result.FailedCount.Should().Be(1);
        result.Message.Should().Contain("fallidas: 1");
        result.Message.Should().NotContain("sensitive database detail");
        calls.Should().Be(2);
    }

    [Fact]
    public async Task ImportFromSapAsync_NoRows_ReturnsSkipped()
    {
        var reader = Substitute.For<ISapWarehouseReader>();
        reader.GetWarehousesAsync(10, Arg.Any<CancellationToken>()).Returns([]);
        var handler = new SapWarehouseSyncHandler(
            reader,
            new SapWarehouseRecordProcessor(
                Substitute.For<IWarehouseRepository>(), Substitute.For<ISender>()));

        var result = await handler.ImportFromSapAsync(Context());

        handler.EntityCode.Should().Be(SapSyncEntityCode.Warehouses);
        result.Status.Should().Be(SapSyncStatus.Skipped);
        result.ProcessedCount.Should().Be(0);
    }

    [Fact]
    public async Task ExportToSapAsync_RemainsOutsideApprovedScope()
    {
        var handler = new SapWarehouseSyncHandler(
            Substitute.For<ISapWarehouseReader>(),
            new SapWarehouseRecordProcessor(
                Substitute.For<IWarehouseRepository>(), Substitute.For<ISender>()));

        var result = await handler.ExportToSapAsync(Context());

        result.Status.Should().Be(SapSyncStatus.NotImplemented);
    }

    private static SapSyncExecutionContext Context() => new(
        CompanyId: 10,
        CompanyCode: "DEMO",
        EntityCode: SapSyncEntityCode.Warehouses,
        Direction: SapSyncDirection.SapToErp,
        Operation: SapSyncOperation.Import,
        WorkerInstance: "worker-01",
        CorrelationId: Guid.NewGuid().ToString(),
        AttemptCount: 0,
        StartedAtUtc: DateTime.UtcNow);

    private static SapWarehouseRecord Record(string code) =>
        new(code, $"Bodega {code}", "Direccion", "Cuenca", "Azuay", "EC", true);

    private static WarehouseDto Local(string code) => new()
    {
        Id = 10,
        GlobalId = Guid.NewGuid(),
        Code = code,
        Name = $"Bodega {code}",
        Address = "Direccion",
        City = "Cuenca",
        Province = "Azuay",
        Country = "EC",
        SapCode = code,
        ExternalSystem = "SAP_B1",
        ExternalCode = code,
        IsActive = true
    };
}
