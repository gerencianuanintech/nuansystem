using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class WarehouseSyncEventApplierTests
{
    [Fact]
    public async Task WarehouseApplier_Created_UpsertsByGlobalId()
    {
        var repository = Substitute.For<IWarehouseSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new WarehouseSyncApplyResult(true, false, false, 100, "Creado."));
        var applier = new WarehouseSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<WarehouseSyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.Code == payload.Code &&
                value.SapCode == null),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WarehouseApplier_Disabled_MarksInactive()
    {
        var repository = Substitute.For<IWarehouseSyncApplyRepository>();
        var payload = CreatePayload(isActive: false);
        var context = CreateContext(payload, SyncOperation.Disabled);
        repository.DisableFromSyncAsync(2, context, payload, false, Arg.Any<CancellationToken>())
            .Returns(new WarehouseSyncApplyResult(true, false, false, 100, "Desactivado."));
        var applier = new WarehouseSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).DisableFromSyncAsync(
            2,
            context,
            Arg.Is<WarehouseSyncPayload>(value => value.GlobalId == payload.GlobalId && value.IsActive == false),
            markDeleted: false,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WarehouseApplier_ReapplyingSameEventId_IsIdempotentAndDoesNotDuplicate()
    {
        var repository = new InMemoryWarehouseSyncApplyRepository();
        var payload = CreatePayload(sapCode: "SAP-BOD-AME");
        var eventId = Guid.NewGuid();
        var context = CreateContext(payload, SyncOperation.Created, eventId);
        var applier = new WarehouseSyncEventApplier(repository);

        var firstResult = await applier.ApplyAsync(context, CancellationToken.None);
        var secondResult = await applier.ApplyAsync(context, CancellationToken.None);

        firstResult.Applied.Should().BeTrue();
        secondResult.Applied.Should().BeTrue();
        secondResult.Message.Should().Contain("ya aplicado");
        repository.WarehouseCount.Should().Be(1);
        repository.SyncInboxCount.Should().Be(1);
        repository.WarehouseWriteCount.Should().Be(1);
        repository.GetWarehouseName(payload.GlobalId).Should().Be(payload.Name);
        repository.GetSyncInboxStatus(eventId).Should().Be(SyncEventStatus.Applied);
        repository.UsedLocalIdAsIdentity.Should().BeFalse();
        repository.UsedSapCodeAsIdentity.Should().BeFalse();
        repository.TouchedStock.Should().BeFalse();
        repository.TouchedKardex.Should().BeFalse();
        repository.TouchedCost.Should().BeFalse();
    }

    [Fact]
    public void WarehouseSyncPath_DoesNotUseSapCodeAsIdentityOrOperationalInventory()
    {
        var applier = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.MasterBranchSyncWorker",
            "Services",
            "WarehouseSyncEventApplier.cs");
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "WarehouseSyncApplyRepository.cs");

        applier.Should().NotContain("SapCode");
        repository.Should().Contain("SP_NA_POST_WAREHOUSE_SYNC_APPLY");
        repository.Should().Contain("EventId = @EventId");
        repository.Should().Contain("Status = N'Applied'");
        repository.Should().Contain("SapCode = NormalizeOptional(payload.SapCode");
        repository.Should().NotContain("payload.Description");
        repository.Should().NotContain("payload.Address");
        repository.Should().NotContain("payload.AllowsSales");
        repository.Should().NotContain("WarehouseStock");
        repository.Should().NotContain("Kardex");
        repository.Should().NotContain("AverageCost");
        repository.Should().NotContain("LastPurchasePrice");
        repository.Should().NotContain("Batch");
        repository.Should().NotContain("Serial");
    }

    private static SyncEventApplyContext CreateContext(
        WarehouseSyncPayload payload,
        SyncOperation operation,
        Guid? eventId = null)
    {
        return new SyncEventApplyContext(
            eventId ?? Guid.NewGuid(),
            SourceCompanyId: 1,
            EntityName: "Warehouse",
            EntityGlobalId: payload.GlobalId,
            Operation: operation.ToString(),
            PayloadJson: CreatePayloadJson(payload, operation),
            TargetCompanyId: 2,
            TargetId: 10);
    }

    private static WarehouseSyncPayload CreatePayload(
        bool isActive = true,
        string? sapCode = null)
    {
        return new WarehouseSyncPayload(
            GlobalId: Guid.NewGuid(),
            Code: "BOD-AME",
            Name: "Bodega Mega Americas",
            IsActive: isActive,
            ExternalSystem: null,
            ExternalCode: null,
            SapCode: sapCode,
            CreatedAt: new DateTime(2026, 7, 10, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt: null);
    }

    private static string CreatePayloadJson(WarehouseSyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "Warehouse",
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

    private sealed class InMemoryWarehouseSyncApplyRepository : IWarehouseSyncApplyRepository
    {
        private readonly Dictionary<Guid, WarehouseSyncPayload> _warehousesByGlobalId = [];
        private readonly Dictionary<Guid, SyncEventStatus> _syncInboxByEventId = [];

        public int WarehouseCount => _warehousesByGlobalId.Count;

        public int SyncInboxCount => _syncInboxByEventId.Count;

        public int WarehouseWriteCount { get; private set; }

        public bool UsedLocalIdAsIdentity { get; private set; }

        public bool UsedSapCodeAsIdentity { get; private set; }

        public bool TouchedStock { get; private set; }

        public bool TouchedKardex { get; private set; }

        public bool TouchedCost { get; private set; }

        public Task<bool> ExistsByGlobalIdAsync(
            int branchCompanyId,
            Guid globalId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_warehousesByGlobalId.ContainsKey(globalId));
        }

        public Task<WarehouseSyncApplyResult> UpsertFromSyncAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            WarehouseSyncPayload payload,
            SyncOperation operation,
            CancellationToken cancellationToken = default)
        {
            if (_syncInboxByEventId.TryGetValue(context.EventId, out var status) &&
                status == SyncEventStatus.Applied)
            {
                return Task.FromResult(new WarehouseSyncApplyResult(
                    true,
                    true,
                    false,
                    WarehouseId: null,
                    Message: "Evento ya aplicado en SyncInbox."));
            }

            _syncInboxByEventId[context.EventId] = SyncEventStatus.Pending;
            _warehousesByGlobalId[payload.GlobalId] = payload;
            WarehouseWriteCount++;
            _syncInboxByEventId[context.EventId] = SyncEventStatus.Applied;

            return Task.FromResult(new WarehouseSyncApplyResult(
                true,
                false,
                false,
                WarehouseId: 1,
                Message: "Warehouse sincronizado por GlobalId."));
        }

        public Task<WarehouseSyncApplyResult> DisableFromSyncAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            WarehouseSyncPayload payload,
            bool markDeleted,
            CancellationToken cancellationToken = default)
        {
            return UpsertFromSyncAsync(branchCompanyId, context, payload with { IsActive = false }, SyncOperation.Disabled, cancellationToken);
        }

        public string? GetWarehouseName(Guid globalId)
        {
            return _warehousesByGlobalId.TryGetValue(globalId, out var payload)
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
