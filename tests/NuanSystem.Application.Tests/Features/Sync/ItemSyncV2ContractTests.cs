using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemSyncV2ContractTests
{
    [Fact]
    public async Task Applier_PreservesFiveDependencyGlobalIdsSeparately()
    {
        var repository = Substitute.For<IItemSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload);
        repository.CheckDependenciesAsync(2, payload, Arg.Any<CancellationToken>())
            .Returns(ItemSyncDependencyCheckResult.Satisfied);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemSyncApplyResult(true, false, 10, "Aplicado."));

        var result = await new ItemSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<ItemSyncPayload>(value =>
                value.ItemGroupGlobalId == payload.ItemGroupGlobalId &&
                value.ItemFamilyGlobalId == payload.ItemFamilyGlobalId &&
                value.InventoryUnitOfMeasureGlobalId == payload.InventoryUnitOfMeasureGlobalId &&
                value.PurchaseUnitOfMeasureGlobalId == payload.PurchaseUnitOfMeasureGlobalId &&
                value.SalesUnitOfMeasureGlobalId == payload.SalesUnitOfMeasureGlobalId),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Applier_LegacyPayloadWithoutDependencyGlobalIds_IsRetryable()
    {
        var repository = Substitute.For<IItemSyncApplyRepository>();
        var globalId = Guid.NewGuid();
        var legacyPayload = new
        {
            globalId,
            code = "ITEM-LEGACY",
            name = "Legacy",
            description = (string?)null,
            itemType = "Product",
            itemGroupId = 7,
            itemGroupCode = "GROUP",
            inventoryUnitOfMeasureId = 8,
            inventoryUnitOfMeasureCode = "UND",
            isInventoryItem = true,
            isSalesItem = false,
            isPurchaseItem = false,
            isActive = true
        };
        var context = new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "Item",
            globalId,
            SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload = legacyPayload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            2,
            3);
        repository.CheckDependenciesAsync(
                2,
                Arg.Any<ItemSyncPayload>(),
                Arg.Any<CancellationToken>())
            .Returns(new ItemSyncDependencyCheckResult(
                false,
                "ItemGroups",
                "La dependencia ItemGroups no informa GlobalId."));

        var result = await new ItemSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeTrue();
        result.Terminal.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_DEPENDENCY_PENDING");
        await repository.DidNotReceiveWithAnyArgs().UpsertFromSyncAsync(default, default!, default!, default, default);
    }

    [Fact]
    public async Task Applier_PropagatesTerminalItemCodeCollision()
    {
        var repository = Substitute.For<IItemSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload);
        repository.CheckDependenciesAsync(2, payload, Arg.Any<CancellationToken>())
            .Returns(ItemSyncDependencyCheckResult.Satisfied);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemSyncApplyResult(
                false,
                false,
                null,
                "Codigo ocupado.",
                TerminalConflict: true,
                ErrorCode: "SYNC_ITEM_CODE_CONFLICT"));

        var result = await new ItemSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_ITEM_CODE_CONFLICT");
    }

    [Fact]
    public void Persistence_UsesGlobalIdOnlyForItemDependencies()
    {
        var source = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "ItemSyncApplyRepository.cs");

        source.Should().Contain("ResolveIdByGlobalIdAsync");
        source.Should().Contain("WHERE GlobalId = @GlobalId");
        source.Should().Contain("SYNC_ITEM_CODE_CONFLICT");
        source.Should().Contain("Status = N'DeadLetter'");
        source.Should().NotContain("ResolveIdByCodeOrIdAsync");
        source.Should().NotContain("payload.ItemGroupId");
        source.Should().NotContain("payload.ItemFamilyId");
        source.Should().NotContain("payload.InventoryUnitOfMeasureId");
    }

    [Fact]
    public void TenantMigration_ExposesAllFiveDependencyGlobalIds()
    {
        var script = ReadSource("database", "sql", "131_tenant_item_sync_payload_v2.sql");

        script.Should().Contain("ItemGroupGlobalId");
        script.Should().Contain("ItemFamilyGlobalId");
        script.Should().Contain("InventoryUnitOfMeasureGlobalId");
        script.Should().Contain("PurchaseUnitOfMeasureGlobalId");
        script.Should().Contain("SalesUnitOfMeasureGlobalId");
        script.Should().Contain("20260726.131");
        script.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_LISTAR");
        script.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ITEMS_BUSCARPORID");
    }

    private static ItemSyncPayload CreatePayload() => new(
        Guid.NewGuid(),
        "ITEM-V2",
        "Item v2",
        null,
        "Product",
        Guid.NewGuid(),
        "GROUP",
        Guid.NewGuid(),
        "FAMILY",
        Guid.NewGuid(),
        "UND-I",
        Guid.NewGuid(),
        "UND-P",
        Guid.NewGuid(),
        "UND-S",
        null,
        true,
        true,
        true,
        true,
        null,
        null,
        null);

    private static SyncEventApplyContext CreateContext(ItemSyncPayload payload) => new(
        Guid.NewGuid(),
        1,
        "Item",
        payload.GlobalId,
        SyncOperation.Created.ToString(),
        JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
        2,
        3);

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            directory = directory.Parent;
        }
        throw new FileNotFoundException(Path.Combine(pathParts));
    }
}
