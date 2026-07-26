using FluentAssertions;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Warehouses;

public sealed class WarehouseSyncPublishingTests
{
    [Fact]
    public void Factory_UsesGlobalIdAndMinimalCorporatePayload()
    {
        var warehouse = CreateWarehouse();

        var request = WarehouseSyncEventFactory.Create(10, warehouse, SyncOperation.Created);

        request.EntityName.Should().Be("Warehouse");
        request.EntityGlobalId.Should().Be(warehouse.GlobalId);
        request.EntityCode.Should().Be(warehouse.Code);
        var payload = request.Payload.Should().BeOfType<WarehouseSyncPayload>().Subject;
        payload.Code.Should().Be("BOD-AME");
        payload.Name.Should().Be("Bodega Mega Americas");
        payload.GetType().GetProperty("Description").Should().BeNull();
        payload.GetType().GetProperty("Address").Should().BeNull();
        payload.GetType().GetProperty("AllowsSales").Should().BeNull();
        payload.GetType().GetProperty("Stock").Should().BeNull();
        payload.GetType().GetProperty("Cost").Should().BeNull();
    }

    [Theory]
    [InlineData(SyncOperation.Disabled)]
    [InlineData(SyncOperation.Deleted)]
    public void Factory_ForcesInactiveForTerminalOrDisabledOperations(SyncOperation operation)
    {
        var request = WarehouseSyncEventFactory.Create(10, CreateWarehouse(), operation);
        request.Payload.Should().BeOfType<WarehouseSyncPayload>().Subject.IsActive.Should().BeFalse();
    }

    private static WarehouseDto CreateWarehouse() => new()
    {
        Id = 25,
        GlobalId = Guid.NewGuid(),
        Code = "BOD-AME",
        Name = "Bodega Mega Americas",
        Description = "Local only",
        Address = "Local only",
        AllowsSales = true,
        IsActive = true,
        ExternalSystem = "SAP_B1",
        ExternalCode = "01",
        SapCode = "01",
        CreatedAt = new DateTime(2026, 7, 10, 10, 0, 0, DateTimeKind.Utc)
    };
}
