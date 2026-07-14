using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Commands;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Items;

public sealed class ItemSyncPublishingTests
{
    private readonly IItemRepository _repository = Substitute.For<IItemRepository>();
    private readonly ISyncEventPublisher _syncEventPublisher = Substitute.For<ISyncEventPublisher>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();

    [Fact]
    public async Task Create_PublishesItemSyncEvent_WithGlobalIdAndCode()
    {
        SyncPublishRequest? captured = null;
        var item = CreateItem();
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.ExistsByCodeAsync("ART-001", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateItemData>(), Arg.Any<CancellationToken>()).Returns(item.Id);
        _repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.CompanyId.Should().Be(10);
        captured.EntityName.Should().Be("Item");
        captured.EntityGlobalId.Should().Be(item.GlobalId);
        captured.EntityGlobalId.Should().NotBe(Guid.Empty);
        captured.EntityCode.Should().Be(item.Code);
        captured.Operation.Should().Be(SyncOperation.Created);
        captured.Payload.Should().BeOfType<ItemSyncPayload>();
    }

    [Fact]
    public async Task Update_PublishesUpdatedItemSyncEvent_WithGlobalId()
    {
        SyncPublishRequest? captured = null;
        var item = CreateItem(name: "Articulo actualizado");
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001", item.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(UpdateCommand(item.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityGlobalId.Should().Be(item.GlobalId);
        captured.EntityCode.Should().Be(item.Code);
        captured.Operation.Should().Be(SyncOperation.Updated);
    }

    [Fact]
    public async Task Update_PublishesDisabledItemSyncEvent_WhenItemBecomesInactive()
    {
        SyncPublishRequest? captured = null;
        var item = CreateItem(isActive: false);
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001", item.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(UpdateCommand(item.Id, isActive: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Operation.Should().Be(SyncOperation.Disabled);
    }

    [Fact]
    public async Task Delete_PublishesDeletedItemSyncEvent_AfterLogicalDelete()
    {
        SyncPublishRequest? captured = null;
        var item = CreateItem();
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        _repository.DeleteAsync(item.Id, 7, "admin", Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(new DeleteItemCommand(item.Id, 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityGlobalId.Should().Be(item.GlobalId);
        captured.Operation.Should().Be(SyncOperation.Deleted);
    }

    [Fact]
    public async Task Create_KeepsStandaloneCrudWorking_WhenPublisherSkipsForDisabledSync()
    {
        var item = CreateItem();
        ConfigureActiveCompany(syncEnabled: false);
        _syncEventPublisher.PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(false, null, "La empresa no tiene sincronizacion habilitada.")));
        _repository.ExistsByCodeAsync("ART-001", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateItemData>(), Arg.Any<CancellationToken>()).Returns(item.Id);
        _repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _syncEventPublisher.Received(1).PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_DoesNotPublish_WhenNoActiveCompanyContext()
    {
        var item = CreateItem();
        ConfigureNoActiveCompany();
        _repository.ExistsByCodeAsync("ART-001", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateItemData>(), Arg.Any<CancellationToken>()).Returns(item.Id);
        _repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _syncEventPublisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
    }

    [Fact]
    public async Task Create_PayloadKeepsSapCodeOptionalAndExcludesOperationalInventoryValues()
    {
        SyncPublishRequest? captured = null;
        var item = CreateItem(sapCode: null);
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.ExistsByCodeAsync("ART-001", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateItemData>(), Arg.Any<CancellationToken>()).Returns(item.Id);
        _repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var payload = captured!.Payload.Should().BeOfType<ItemSyncPayload>().Subject;
        payload.SapCode.Should().BeNull();
        payload.GlobalId.Should().Be(item.GlobalId);
        payload.GetType().GetProperty(nameof(ItemDto.BaseSalesPrice)).Should().BeNull();
        payload.GetType().GetProperty(nameof(ItemDto.ReferenceCost)).Should().BeNull();
        payload.GetType().GetProperty(nameof(ItemDto.Warehouses)).Should().BeNull();
    }

    private CreateItemCommandHandler CreateCreateHandler() => new(_repository, _syncEventPublisher, _companyContext);

    private UpdateItemCommandHandler CreateUpdateHandler() => new(_repository, _syncEventPublisher, _companyContext);

    private DeleteItemCommandHandler CreateDeleteHandler() => new(_repository, _syncEventPublisher, _companyContext);

    private void ConfigureSyncPublisher(Action<SyncPublishRequest> capture)
    {
        _syncEventPublisher.PublishAsync(Arg.Do(capture), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 45, "Evento publicado.")));
    }

    private void ConfigureActiveCompany(bool syncEnabled)
    {
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            CompanyId: 10,
            CompanyCode: "MASTER",
            CommercialName: "Empresa Master",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: syncEnabled));
    }

    private void ConfigureNoActiveCompany()
    {
        _companyContext.HasActiveCompany.Returns(false);
        _companyContext.CurrentCompany.Returns((CompanyConnectionInfo?)null);
    }

    private static ItemDto CreateItem(string name = "Articulo Uno", bool isActive = true, string? sapCode = null)
    {
        return new ItemDto
        {
            Id = 25,
            GlobalId = Guid.NewGuid(),
            Code = "ART-001",
            Name = name,
            Description = "Articulo maestro",
            ItemType = "Product",
            ItemGroupId = 3,
            ItemGroupCode = "GENERAL",
            ItemFamilyId = 4,
            ItemFamilyCode = "FAM",
            InventoryUnitOfMeasureId = 5,
            InventoryUnitOfMeasureCode = "UND",
            IsInventoryItem = true,
            IsSalesItem = true,
            IsPurchaseItem = true,
            IsActive = isActive,
            ExternalSystem = "ExternalApi",
            ExternalCode = "EXT-ART-001",
            SapCode = sapCode,
            Barcodes = [new ItemBarcodeDto(1, 25, "1234567890", null, "Internal", 1, true, true)]
        };
    }

    private static CreateItemCommand CreateCommand()
    {
        return new CreateItemCommand(
            Code: "ART-001",
            Name: "Articulo Uno",
            Description: "Articulo maestro",
            ItemGroupId: 3,
            ItemFamilyId: 4,
            ItemType: "Product",
            InventoryUnitOfMeasureId: 5,
            PurchaseUnitOfMeasureId: 5,
            SalesUnitOfMeasureId: 5,
            IsPurchaseItem: true,
            IsSalesItem: true,
            IsInventoryItem: true,
            PurchaseTaxId: null,
            SalesTaxId: null,
            ValuationMethod: "MovingAverage",
            ManagedBy: "None",
            BatchSerialManagementMethod: "EveryTransaction",
            PreferredVendorCode: null,
            VendorCatalogCode: null,
            BaseSalesPrice: 9.99m,
            ReferenceCost: 3.50m,
            PurchaseFactor: 1,
            SalesFactor: 1,
            AllowDiscount: true,
            AllowSaleWithoutStock: false,
            Remarks: null,
            IsActive: true,
            Barcodes: [new SaveItemBarcodeData("1234567890", null, "Internal", 1, true, true)],
            Warehouses: [new SaveItemWarehouseData(1, 0, 0, 0, 0, null, 0, true, false, true)],
            AuditUserId: 7,
            AuditUserName: "admin");
    }

    private static UpdateItemCommand UpdateCommand(int id, bool isActive = true)
    {
        var create = CreateCommand();
        return new UpdateItemCommand(
            id,
            create.Code,
            create.Name,
            create.Description,
            create.ItemGroupId,
            create.ItemFamilyId,
            create.ItemType,
            create.InventoryUnitOfMeasureId,
            create.PurchaseUnitOfMeasureId,
            create.SalesUnitOfMeasureId,
            create.IsPurchaseItem,
            create.IsSalesItem,
            create.IsInventoryItem,
            create.PurchaseTaxId,
            create.SalesTaxId,
            create.ValuationMethod,
            create.ManagedBy,
            create.BatchSerialManagementMethod,
            create.PreferredVendorCode,
            create.VendorCatalogCode,
            create.BaseSalesPrice,
            create.ReferenceCost,
            create.PurchaseFactor,
            create.SalesFactor,
            create.AllowDiscount,
            create.AllowSaleWithoutStock,
            create.Remarks,
            isActive,
            create.Barcodes,
            create.Warehouses,
            create.MasterData,
            create.AuditUserId,
            create.AuditUserName);
    }
}
