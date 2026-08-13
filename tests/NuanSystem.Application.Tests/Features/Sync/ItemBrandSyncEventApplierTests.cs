using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemBrandSyncEventApplierTests
{
    [Fact]
    public void CanApply_UsesCanonicalCode()
    {
        var applier = new ItemBrandSyncEventApplier(Substitute.For<IItemBrandSyncApplyRepository>());
        applier.CanApply("ItemBrands").Should().BeTrue();
        applier.CanApply("ItemFamilies").Should().BeFalse();
    }

    [Fact]
    public async Task Created_AppliesByGlobalId()
    {
        var repository = Substitute.For<IItemBrandSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload, SyncOperation.Created);
        repository.ApplyAsync(2, context, Arg.Any<ItemBrandSyncPayload>(), SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemBrandSyncApplyResult(true, false, false, 8, "Aplicada."));
        var result = await new ItemBrandSyncEventApplier(repository).ApplyAsync(context);
        result.Applied.Should().BeTrue();
        await repository.Received(1).ApplyAsync(2, context,
            Arg.Is<ItemBrandSyncPayload>(x => x.GlobalId == payload.GlobalId && x.Code == "MONI"),
            SyncOperation.Created, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GlobalIdMismatch_IsTerminal()
    {
        var repository = Substitute.For<IItemBrandSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload, SyncOperation.Updated) with { EntityGlobalId = Guid.NewGuid() };
        var result = await new ItemBrandSyncEventApplier(repository).ApplyAsync(context);
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task CodeCollision_RemainsTerminal()
    {
        var repository = Substitute.For<IItemBrandSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload, SyncOperation.Created);
        repository.ApplyAsync(2, context, Arg.Any<ItemBrandSyncPayload>(), SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemBrandSyncApplyResult(false, false, true, null, "Conflicto.", "SYNC_ITEM_BRAND_CODE_CONFLICT"));
        var result = await new ItemBrandSyncEventApplier(repository).ApplyAsync(context);
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_ITEM_BRAND_CODE_CONFLICT");
    }

    private static ItemBrandSyncPayload Payload() => new(
        Guid.NewGuid(), "MONI", "Moni", "Marca", 10, true, false,
        new DateTime(2026, 8, 12, 10, 0, 0, DateTimeKind.Utc));

    private static SyncEventApplyContext Context(ItemBrandSyncPayload payload, SyncOperation operation)
    {
        var json = JsonSerializer.Serialize(new
        {
            entityName = "ItemBrands", globalId = payload.GlobalId, code = payload.Code,
            operation = operation.ToString(), payload
        }, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return new(Guid.NewGuid(), 1, "ItemBrands", payload.GlobalId, operation.ToString(), json, 2, 10);
    }
}
