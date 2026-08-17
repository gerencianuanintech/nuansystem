using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemOriginSyncEventApplierTests
{
    [Fact]
    public async Task Applier_PropagatesTerminalCodeCollision()
    {
        var repository = Substitute.For<IItemOriginSyncApplyRepository>();
        var payload = Payload(); var context = Context(payload);
        repository.ApplyAsync(2, context, Arg.Any<ItemOriginSyncPayload>(), SyncOperation.Created,
                Arg.Any<CancellationToken>())
            .Returns(new ItemOriginSyncApplyResult(false, false, true, null, "Codigo ocupado.",
                "SYNC_ITEM_ORIGIN_CODE_CONFLICT"));

        var result = await new ItemOriginSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_ITEM_ORIGIN_CODE_CONFLICT");
    }

    [Fact]
    public async Task Applier_RejectsInvalidOrderBeforeRepository()
    {
        var repository = Substitute.For<IItemOriginSyncApplyRepository>();
        var payload = Payload() with { SortOrder = -1 };
        var result = await new ItemOriginSyncEventApplier(repository).ApplyAsync(Context(payload));
        result.ErrorCode.Should().Be("SYNC_ITEM_ORIGIN_PAYLOAD_INVALID");
        result.Terminal.Should().BeTrue();
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default, default);
    }

    [Fact]
    public async Task Applier_RejectsGlobalIdMismatchBeforeRepository()
    {
        var repository = Substitute.For<IItemOriginSyncApplyRepository>();
        var payload = Payload();
        var result = await new ItemOriginSyncEventApplier(repository).ApplyAsync(
            Context(payload) with { EntityGlobalId = Guid.NewGuid() });
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");
        result.Terminal.Should().BeTrue();
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default, default);
    }

    private static ItemOriginSyncPayload Payload() =>
        new(Guid.NewGuid(), "Local", "Local", null, 10, true, false, DateTime.UtcNow);

    private static SyncEventApplyContext Context(ItemOriginSyncPayload payload) =>
        new(Guid.NewGuid(), 1, "ItemOrigin", payload.GlobalId, SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)), 2, 3);
}
