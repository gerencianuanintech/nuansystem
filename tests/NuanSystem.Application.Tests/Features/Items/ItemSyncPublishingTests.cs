using FluentAssertions;
using NSubstitute;
using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Items.Commands;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Items;

public sealed class ItemSyncPublishingTests
{
    private readonly IItemRepository _repository = Substitute.For<IItemRepository>();
    private readonly IItemGroupRepository _groupRepository = Substitute.For<IItemGroupRepository>();
    private readonly IItemFamilyRepository _familyRepository = Substitute.For<IItemFamilyRepository>();
    private readonly IItemSubgroupRepository _subgroupRepository = Substitute.For<IItemSubgroupRepository>();
    private readonly IItemOriginRepository _originRepository = Substitute.For<IItemOriginRepository>();
    private readonly IReplenishmentMethodRepository _replenishmentMethodRepository = Substitute.For<IReplenishmentMethodRepository>();
    private readonly IStorageConditionRepository _storageConditionRepository = Substitute.For<IStorageConditionRepository>();
    private readonly IItemLocalOutboxWriter _writer = Substitute.For<IItemLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task CreateValidator_AcceptsBarcodeAndWarehouseCollections_WithoutRuntimeException()
    {
        var validator = new CreateItemCommandValidator();
        var command = CreateCommand() with { IsExternalImport = true };

        var action = async () => await validator.ValidateAsync(command);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateValidator_AcceptsBarcodeAndWarehouseCollections_WithoutRuntimeException()
    {
        var validator = new UpdateItemCommandValidator();
        var command = UpdateCommand(25) with { IsExternalImport = true };

        var action = async () => await validator.ValidateAsync(command);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CreateValidator_RejectsAttachmentWithInvalidValidityRange()
    {
        var validator = new CreateItemCommandValidator();
        var attachment = new ItemAttachmentData(
            "Imagen producto", "articulo.png", null, "Comercial", "PNG", "1 MB",
            DateTime.Today, "admin", true, true, false, true, "Activo",
            ValidFrom: new DateTime(2026, 8, 11),
            ValidTo: new DateTime(2026, 8, 10));
        var command = CreateCommand() with
        {
            MasterData = new ItemMasterData(Attachments: new ItemAttachmentsData([attachment]))
        };

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage.Contains("vigencia coherente"));
    }

    [Fact]
    public async Task Create_RejectsSubgroupWithoutFamily()
    {
        var command = CreateCommand() with
        {
            ItemFamilyId = null,
            MasterData = new ItemMasterData(General: new ItemGeneralData(
                AlternateCode: null,
                SupplierSku: null,
                LongDescription: null,
                ProductType: null,
                Origin: null,
                Line: null,
                SubGroup: "YOGUR",
                Model: null,
                Reference: null,
                SalesActive: true,
                PurchaseActive: true,
                ManageInventory: true,
                IsService: false,
                IsKit: false,
                BatchManaged: false,
                SerialManaged: false,
                Perishable: false,
                ExpirationManaged: false,
                RequiresScale: false,
                AllowDiscount: true,
                AffectsInventory: true))
        };
        var handler = CreateCreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "ItemSubgroupRequiresFamily");
        await _repository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Create_WritesLocalOutboxInsideTheSameTransaction()
    {
        var item = CreateItem();
        _repository.ExistsByCodeAsync("ART-001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateItemData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item.Id);
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            item, SyncOperation.Created, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
        _transactionRunner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WritesLocalOutboxInsideTheSameTransaction()
    {
        var item = CreateItem(name: "Articulo actualizado");
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001", item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(UpdateCommand(item.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            item, SyncOperation.Updated, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_WritesDisabledOperation_WhenItemBecomesInactive()
    {
        var item = CreateItem(isActive: false);
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001", item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(UpdateCommand(item.Id, isActive: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            item, SyncOperation.Disabled, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_WritesLocalOutboxInsideTheSameTransaction()
    {
        var item = CreateItem();
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _repository.DeleteAsync(item.Id, 7, "admin", _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(new DeleteItemCommand(item.Id, 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            item, SyncOperation.Deleted, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenLocalOutboxFails()
    {
        var item = CreateItem();
        _repository.ExistsByCodeAsync("ART-001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateItemData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item.Id);
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _writer.EnqueueAsync(Arg.Any<ItemDto>(), Arg.Any<SyncOperation>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("Controlled outbox failure"));
        var handler = CreateCreateHandler();

        var action = () => handler.Handle(CreateCommand(), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Controlled outbox failure");
        _transactionRunner.RolledBack.Should().BeTrue();
        _transactionRunner.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Writer_SkipsStandaloneOrDisabledCompany()
    {
        var item = CreateItem();
        var companyContext = Substitute.For<ICompanyContext>();
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(Company(syncEnabled: false));
        var localOutbox = Substitute.For<ILocalSyncOutboxRepository>();
        var writer = new ItemLocalOutboxWriter(companyContext, new SyncEventPayloadFactory(), localOutbox);

        var eventId = await writer.EnqueueAsync(
            item, SyncOperation.Created, _transactionRunner.Connection, _transactionRunner.Transaction);

        eventId.Should().BeNull();
        await localOutbox.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Writer_CreatesLimitedPayloadAndStableEventIdentity()
    {
        var item = CreateItem(sapCode: null);
        var companyContext = Substitute.For<ICompanyContext>();
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(Company(syncEnabled: true));
        var localOutbox = Substitute.For<ILocalSyncOutboxRepository>();
        CreateLocalSyncOutboxData? captured = null;
        localOutbox.CreateAsync(
                Arg.Do<CreateLocalSyncOutboxData>(value => captured = value),
                _transactionRunner.Connection,
                _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(10);
        var writer = new ItemLocalOutboxWriter(companyContext, new SyncEventPayloadFactory(), localOutbox);

        var eventId = await writer.EnqueueAsync(
            item, SyncOperation.Created, _transactionRunner.Connection, _transactionRunner.Transaction);

        eventId.Should().NotBeNull().And.NotBe(Guid.Empty);
        captured.Should().NotBeNull();
        captured!.EventId.Should().Be(eventId!.Value);
        captured.EntityGlobalId.Should().Be(item.GlobalId);
        captured.EntityName.Should().Be("Item");
        captured.PayloadJson.Should().Contain("\"operation\":\"Created\"")
            .And.NotContain("\"baseSalesPrice\"")
            .And.NotContain("\"referenceCost\"")
            .And.NotContain("\"warehouses\"")
            .And.NotContain("\"masterData\"");
    }

    private CreateItemCommandHandler CreateCreateHandler()
    {
        ConfigureValidClassification();
        return new(_repository, _groupRepository, _familyRepository, _subgroupRepository, _originRepository, _replenishmentMethodRepository, _storageConditionRepository, _transactionRunner, _writer);
    }

    [Fact]
    public async Task Update_PreservesUnchangedHistoricalOrigin_WithoutCatalogLookup()
    {
        var item = CreateItem();
        item.MasterData = MasterDataWithOrigin("Legacy-Origin");
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001", item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateUpdateHandler().Handle(
            UpdateCommand(item.Id) with { MasterData = MasterDataWithOrigin("Legacy-Origin") }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _originRepository.DidNotReceiveWithAnyArgs().GetByCodeAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Update_RejectsChangedInactiveOrigin()
    {
        var item = CreateItem();
        item.MasterData = MasterDataWithOrigin("Legacy-Origin");
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _originRepository.GetByCodeAsync("Mixed", _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(new NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos.ItemOriginDto
            { Id = 3, GlobalId = Guid.NewGuid(), Code = "Mixed", Name = "Mixto", IsActive = false });

        var result = await CreateUpdateHandler().Handle(
            UpdateCommand(item.Id) with { MasterData = MasterDataWithOrigin("Mixed") }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Code == "ItemOriginInactive");
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Update_PreservesUnchangedHistoricalReplenishmentMethod_WithoutCatalogLookup()
    {
        var item = CreateItem();
        item.MasterData = MasterDataWithReplenishmentMethod("LEGACY");
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001", item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateUpdateHandler().Handle(
            UpdateCommand(item.Id) with { MasterData = MasterDataWithReplenishmentMethod("LEGACY") }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _replenishmentMethodRepository.DidNotReceiveWithAnyArgs().GetByCodeAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Update_RejectsChangedInactiveReplenishmentMethod()
    {
        var item = CreateItem();
        item.MasterData = MasterDataWithReplenishmentMethod("LEGACY");
        _repository.GetByIdAsync(item.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(item);
        _replenishmentMethodRepository.GetByCodeAsync("FABRICAR", _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(new NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos.ReplenishmentMethodDto
            { Id = 2, GlobalId = Guid.NewGuid(), Code = "FABRICAR", Name = "Fabricar", IsActive = false });

        var result = await CreateUpdateHandler().Handle(
            UpdateCommand(item.Id) with { MasterData = MasterDataWithReplenishmentMethod("FABRICAR") }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Code == "ReplenishmentMethodInactive");
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Update_PreservesUnchangedHistoricalStorageCondition_WithoutCatalogLookup()
    {
        var item=CreateItem();item.MasterData=MasterDataWithStorageCondition("LEGACY");
        _repository.GetByIdAsync(item.Id,_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001",item.Id,_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(),_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(true);
        var result=await CreateUpdateHandler().Handle(UpdateCommand(item.Id) with { MasterData=MasterDataWithStorageCondition("LEGACY") },CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await _storageConditionRepository.DidNotReceiveWithAnyArgs().GetByCodeAsync(default!,default!,default!,default);
    }

    [Fact]
    public async Task Update_RejectsChangedInactiveStorageCondition()
    {
        var item=CreateItem();item.MasterData=MasterDataWithStorageCondition("LEGACY");
        _repository.GetByIdAsync(item.Id,_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(item);
        _storageConditionRepository.GetByCodeAsync("REFRIGERADO",_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(new NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos.StorageConditionDto{Id=2,GlobalId=Guid.NewGuid(),Code="REFRIGERADO",Name="Refrigerado",IsActive=false});
        var result=await CreateUpdateHandler().Handle(UpdateCommand(item.Id) with { MasterData=MasterDataWithStorageCondition("REFRIGERADO") },CancellationToken.None);
        result.IsSuccess.Should().BeFalse();result.Errors.Should().Contain(x=>x.Code=="StorageConditionInactive");
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!,default!,default!,default);
    }

    [Fact]
    public async Task Update_AllowsIntentionalClearOfExistingStorageCondition()
    {
        var item=CreateItem();item.MasterData=MasterDataWithStorageCondition("LEGACY");
        _repository.GetByIdAsync(item.Id,_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(item);
        _repository.ExistsByCodeAsync("ART-001",item.Id,_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateItemData>(),_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(true);
        var result=await CreateUpdateHandler().Handle(UpdateCommand(item.Id) with { MasterData=MasterDataWithStorageCondition(null) },CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).UpdateAsync(Arg.Any<UpdateItemData>(),_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_RejectsStorageConditionWithDifferentCasingEvenIfRepositoryCollationMatches()
    {
        var item=CreateItem();item.MasterData=MasterDataWithStorageCondition("LEGACY");
        _repository.GetByIdAsync(item.Id,_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(item);
        _storageConditionRepository.GetByCodeAsync("ambiente",_transactionRunner.Connection,_transactionRunner.Transaction,Arg.Any<CancellationToken>()).Returns(new NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos.StorageConditionDto{Id=1,GlobalId=Guid.NewGuid(),Code="AMBIENTE",Name="Ambiente",IsActive=true});
        var result=await CreateUpdateHandler().Handle(UpdateCommand(item.Id) with { MasterData=MasterDataWithStorageCondition("ambiente") },CancellationToken.None);
        result.IsSuccess.Should().BeFalse();result.Errors.Should().Contain(x=>x.Code=="StorageConditionNotFound");
    }

    private UpdateItemCommandHandler CreateUpdateHandler()
    {
        ConfigureValidClassification();
        return new(_repository, _groupRepository, _familyRepository, _subgroupRepository, _originRepository, _replenishmentMethodRepository, _storageConditionRepository, _transactionRunner, _writer);
    }

    private DeleteItemCommandHandler CreateDeleteHandler() => new(_repository, _transactionRunner, _writer);

    private void ConfigureValidClassification()
    {
        _groupRepository.GetByIdAsync(3, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(new NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos.ItemGroupDto { Id = 3, Code = "GENERAL", Name = "General", IsActive = true });
        _familyRepository.GetByIdAsync(4, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(new NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos.ItemFamilyDto { Id = 4, ItemGroupId = 3, Code = "FAM", Name = "Familia", IsActive = true });
        _subgroupRepository.ExistsActiveByFamilyAndCodeAsync(4, Arg.Any<string>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(true);
    }

    private static CompanyConnectionInfo Company(bool syncEnabled) =>
        new(
            CompanyId: 10,
            CompanyCode: "MASTER",
            CommercialName: "Empresa Master",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: syncEnabled);

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

    private static ItemMasterData MasterDataWithOrigin(string? origin) => new(
        General: new ItemGeneralData(null, null, null, null, origin, null, null, null, null,
            true, true, true, false, false, false, false, false, false, false, true, true));

    private static ItemMasterData MasterDataWithReplenishmentMethod(string? method) => new(
        Inventory: new ItemInventoryData(false, null, null, false, false, false, 0, 0, 0, 0, 0,
            null, null, method, null, null, null, null, false, false, false, false, null, null));

    private static ItemMasterData MasterDataWithStorageCondition(string? condition) => new(
        Inventory: new ItemInventoryData(false,null,null,false,false,false,0,0,0,0,0,null,null,null,null,null,null,condition,false,false,false,false,null,null));

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public async Task ExecuteInTenantTransactionAsync(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        {
            await ExecuteInTenantTransactionAsync<object?>(
                async (connection, transaction, token) =>
                {
                    await operation(connection, transaction, token);
                    return null;
                },
                cancellationToken);
        }

        public async Task<T> ExecuteInTenantTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await operation(Connection, Transaction, cancellationToken);
                Committed = true;
                return result;
            }
            catch
            {
                RolledBack = true;
                throw;
            }
        }
    }
}
