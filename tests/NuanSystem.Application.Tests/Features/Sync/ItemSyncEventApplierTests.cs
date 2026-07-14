using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemSyncEventApplierTests
{
    [Fact]
    public async Task ItemApplier_Created_UpsertsByGlobalId()
    {
        var repository = Substitute.For<IItemSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemSyncApplyResult(true, false, 100, "Creado."));
        var applier = new ItemSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<ItemSyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.Code == payload.Code &&
                value.SapCode == null),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ItemApplier_Updated_UpdatesExistingByGlobalId()
    {
        var repository = Substitute.For<IItemSyncApplyRepository>();
        var payload = CreatePayload(name: "Articulo actualizado");
        var context = CreateContext(payload, SyncOperation.Updated);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Updated, Arg.Any<CancellationToken>())
            .Returns(new ItemSyncApplyResult(true, false, 100, "Actualizado."));
        var applier = new ItemSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<ItemSyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.Name == "Articulo actualizado"),
            SyncOperation.Updated,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ItemApplier_Disabled_MarksInactive()
    {
        var repository = Substitute.For<IItemSyncApplyRepository>();
        var payload = CreatePayload(isActive: false);
        var context = CreateContext(payload, SyncOperation.Disabled);
        repository.DisableFromSyncAsync(2, context, payload, false, Arg.Any<CancellationToken>())
            .Returns(new ItemSyncApplyResult(true, false, 100, "Desactivado."));
        var applier = new ItemSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).DisableFromSyncAsync(
            2,
            context,
            Arg.Is<ItemSyncPayload>(value => value.GlobalId == payload.GlobalId && value.IsActive == false),
            markDeleted: false,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ItemApplier_ReapplyingSameEventId_IsIdempotentAndDoesNotDuplicate()
    {
        var repository = new InMemoryItemSyncApplyRepository();
        var payload = CreatePayload(sapCode: "SAP-001");
        var eventId = Guid.NewGuid();
        var context = CreateContext(payload, SyncOperation.Created, eventId);
        var applier = new ItemSyncEventApplier(repository);

        var firstResult = await applier.ApplyAsync(context, CancellationToken.None);
        var secondResult = await applier.ApplyAsync(context, CancellationToken.None);

        firstResult.Applied.Should().BeTrue();
        secondResult.Applied.Should().BeTrue();
        secondResult.Message.Should().Contain("ya aplicado");
        repository.ItemCount.Should().Be(1);
        repository.SyncInboxCount.Should().Be(1);
        repository.ItemWriteCount.Should().Be(1);
        repository.GetItemName(payload.GlobalId).Should().Be(payload.Name);
        repository.GetSyncInboxStatus(eventId).Should().Be(SyncEventStatus.Applied);
        repository.UsedLocalIdAsIdentity.Should().BeFalse();
        repository.UsedSapCodeAsIdentity.Should().BeFalse();
        repository.TouchedStock.Should().BeFalse();
        repository.TouchedCost.Should().BeFalse();
        repository.TouchedPrices.Should().BeFalse();
    }

    [Fact]
    public async Task Dispatcher_DoesNotApplyItem_WhenSkeletonModeIsEnabled()
    {
        var entityApplier = Substitute.For<ISyncEntityEventApplier>();
        entityApplier.CanApply("Item").Returns(true);
        var dispatcher = new SyncEventApplierDispatcher(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(new MasterBranchSyncWorkerOptions
            {
                SkeletonMode = true,
                EnabledEntityAppliers = ["Item"]
            }),
            new[] { entityApplier });

        var result = await dispatcher.ApplyAsync(CreateContext(CreatePayload(), SyncOperation.Created), CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Message.Should().Contain("SkeletonMode");
        await entityApplier.DidNotReceiveWithAnyArgs().ApplyAsync(default!, default);
    }

    [Fact]
    public async Task Dispatcher_AppliesItemOnlyWhenSkeletonModeIsFalseAndItemIsEnabled()
    {
        var entityApplier = Substitute.For<ISyncEntityEventApplier>();
        entityApplier.CanApply("Item").Returns(true);
        entityApplier.ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncEventApplyResult(true, "Aplicado."));
        var dispatcher = new SyncEventApplierDispatcher(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(new MasterBranchSyncWorkerOptions
            {
                SkeletonMode = false,
                EnabledEntityAppliers = ["Item"]
            }),
            new[] { entityApplier });

        var result = await dispatcher.ApplyAsync(CreateContext(CreatePayload(), SyncOperation.Created), CancellationToken.None);

        result.Applied.Should().BeTrue();
        await entityApplier.Received(1).ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Dispatcher_DoesNotApplyItem_WhenItemIsNotEnabled()
    {
        var entityApplier = Substitute.For<ISyncEntityEventApplier>();
        entityApplier.CanApply("Item").Returns(true);
        var dispatcher = new SyncEventApplierDispatcher(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(new MasterBranchSyncWorkerOptions
            {
                SkeletonMode = false,
                EnabledEntityAppliers = ["BusinessPartner"]
            }),
            new[] { entityApplier });

        var result = await dispatcher.ApplyAsync(CreateContext(CreatePayload(), SyncOperation.Created), CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_ENTITY_APPLIER_DISABLED");
        await entityApplier.DidNotReceiveWithAnyArgs().ApplyAsync(default!, default);
    }

    [Fact]
    public void ItemSyncPath_DoesNotUseSapCodeAsIdentityOrOperationalInventory()
    {
        var applier = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.MasterBranchSyncWorker",
            "Services",
            "ItemSyncEventApplier.cs");
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "ItemSyncApplyRepository.cs");

        applier.Should().NotContain("SapCode");
        repository.Should().Contain("WHERE GlobalId = @GlobalId");
        repository.Should().Contain("EventId = @EventId");
        repository.Should().Contain("Status = N'Applied'");
        repository.Should().Contain("SapCode = @SapCode");
        repository.Should().NotContain("WarehouseStock");
        repository.Should().NotContain("Kardex");
        repository.Should().NotContain("AverageCost");
        repository.Should().NotContain("LastPurchasePrice");
        repository.Should().NotContain("PriceList");
    }

    private static SyncEventApplyContext CreateContext(
        ItemSyncPayload payload,
        SyncOperation operation,
        Guid? eventId = null)
    {
        return new SyncEventApplyContext(
            eventId ?? Guid.NewGuid(),
            SourceCompanyId: 1,
            EntityName: "Item",
            EntityGlobalId: payload.GlobalId,
            Operation: operation.ToString(),
            PayloadJson: CreatePayloadJson(payload, operation),
            TargetCompanyId: 2,
            TargetId: 10);
    }

    private static ItemSyncPayload CreatePayload(
        string name = "Articulo Uno",
        bool isActive = true,
        string? sapCode = null)
    {
        return new ItemSyncPayload(
            GlobalId: Guid.NewGuid(),
            Code: "ART-001",
            Name: name,
            Description: "Articulo maestro",
            ItemType: "Product",
            ItemGroupId: 1,
            ItemGroupCode: "GENERAL",
            ItemFamilyId: 2,
            ItemFamilyCode: "FAM",
            InventoryUnitOfMeasureId: 3,
            InventoryUnitOfMeasureCode: "UND",
            Barcode: "1234567890",
            IsInventoryItem: true,
            IsSalesItem: true,
            IsPurchaseItem: true,
            IsActive: isActive,
            ExternalSystem: null,
            ExternalCode: null,
            SapCode: sapCode);
    }

    private static string CreatePayloadJson(ItemSyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "Item",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };

        return JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var scriptPath = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(scriptPath))
            {
                return File.ReadAllText(scriptPath);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro el archivo {Path.Combine(pathParts)}.");
    }

    private sealed class StaticOptionsMonitor<T>(T currentValue) : IOptionsMonitor<T>
    {
        public T CurrentValue { get; } = currentValue;

        public T Get(string? name) => CurrentValue;

        public IDisposable? OnChange(Action<T, string?> listener) => null;
    }

    private sealed class InMemoryItemSyncApplyRepository : IItemSyncApplyRepository
    {
        private readonly Dictionary<Guid, ItemSyncPayload> _itemsByGlobalId = [];
        private readonly Dictionary<Guid, SyncEventStatus> _syncInboxByEventId = [];

        public int ItemCount => _itemsByGlobalId.Count;

        public int SyncInboxCount => _syncInboxByEventId.Count;

        public int ItemWriteCount { get; private set; }

        public bool UsedLocalIdAsIdentity { get; private set; }

        public bool UsedSapCodeAsIdentity { get; private set; }

        public bool TouchedStock { get; private set; }

        public bool TouchedCost { get; private set; }

        public bool TouchedPrices { get; private set; }

        public Task<bool> ExistsByGlobalIdAsync(
            int branchCompanyId,
            Guid globalId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_itemsByGlobalId.ContainsKey(globalId));
        }

        public Task<ItemSyncApplyResult> UpsertFromSyncAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            ItemSyncPayload payload,
            SyncOperation operation,
            CancellationToken cancellationToken = default)
        {
            if (_syncInboxByEventId.TryGetValue(context.EventId, out var status) &&
                status == SyncEventStatus.Applied)
            {
                return Task.FromResult(new ItemSyncApplyResult(
                    true,
                    true,
                    ItemId: null,
                    "Evento ya aplicado en SyncInbox."));
            }

            _syncInboxByEventId[context.EventId] = SyncEventStatus.Pending;
            _itemsByGlobalId[payload.GlobalId] = payload;
            ItemWriteCount++;
            _syncInboxByEventId[context.EventId] = SyncEventStatus.Applied;

            return Task.FromResult(new ItemSyncApplyResult(
                true,
                false,
                ItemId: 1,
                "Item sincronizado por GlobalId."));
        }

        public Task<ItemSyncApplyResult> DisableFromSyncAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            ItemSyncPayload payload,
            bool markDeleted,
            CancellationToken cancellationToken = default)
        {
            return UpsertFromSyncAsync(branchCompanyId, context, payload with { IsActive = false }, SyncOperation.Disabled, cancellationToken);
        }

        public string? GetItemName(Guid globalId)
        {
            return _itemsByGlobalId.TryGetValue(globalId, out var payload)
                ? payload.Name
                : null;
        }

        public SyncEventStatus? GetSyncInboxStatus(Guid eventId)
        {
            return _syncInboxByEventId.TryGetValue(eventId, out var status)
                ? status
                : null;
        }
    }
}
