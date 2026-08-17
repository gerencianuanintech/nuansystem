using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemSubgroupSyncEventApplierTests
{
    [Fact]
    public async Task Applier_RetriesWhenFamilyDependencyIsPending()
    {
        var repository = Substitute.For<IItemSubgroupSyncApplyRepository>();
        var payload = Payload();
        repository.ItemFamilyExistsAsync(2, payload.ItemFamilyGlobalId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await new ItemSubgroupSyncEventApplier(repository).ApplyAsync(Context(payload));

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeTrue();
        result.Terminal.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_ITEM_SUBGROUP_ITEM_FAMILY_PENDING");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default, default);
    }

    [Fact]
    public async Task Applier_RejectsInvalidOrderBeforeRepository()
    {
        var repository = Substitute.For<IItemSubgroupSyncApplyRepository>();
        var payload = Payload() with { SortOrder = -1 };

        var result = await new ItemSubgroupSyncEventApplier(repository).ApplyAsync(Context(payload));

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_ITEM_SUBGROUP_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default, default);
    }

    [Fact]
    public async Task Applier_PropagatesTerminalCodeCollision()
    {
        var repository = Substitute.For<IItemSubgroupSyncApplyRepository>();
        var payload = Payload();
        repository.ItemFamilyExistsAsync(2, payload.ItemFamilyGlobalId, Arg.Any<CancellationToken>()).Returns(true);
        repository.ApplyAsync(2, Arg.Any<SyncEventApplyContext>(), Arg.Any<ItemSubgroupSyncPayload>(), SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemSubgroupSyncApplyResult(false, false, true, null, "Código ocupado.", "SYNC_ITEM_SUBGROUP_CODE_CONFLICT"));

        var result = await new ItemSubgroupSyncEventApplier(repository).ApplyAsync(Context(payload));

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_ITEM_SUBGROUP_CODE_CONFLICT");
    }

    private static ItemSubgroupSyncPayload Payload() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "LACTEOS", "YOGUR", "Yogures", null, 20, true, false, DateTime.UtcNow, null);

    private static SyncEventApplyContext Context(ItemSubgroupSyncPayload payload) =>
        new(Guid.NewGuid(), 1, "ItemSubgroups", payload.GlobalId, SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)), 2, 3);
}
