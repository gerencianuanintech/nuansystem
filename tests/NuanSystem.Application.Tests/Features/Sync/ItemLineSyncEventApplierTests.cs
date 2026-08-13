using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemLineSyncEventApplierTests
{
    [Fact]
    public async Task Applier_PropagatesTerminalCodeCollision()
    {
        var repository = Substitute.For<IItemLineSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload);
        repository.ApplyAsync(2, context, Arg.Any<ItemLineSyncPayload>(), SyncOperation.Created,
                Arg.Any<CancellationToken>())
            .Returns(new ItemLineSyncApplyResult(false, false, true, null, "Codigo ocupado.",
                "SYNC_ITEM_LINE_CODE_CONFLICT"));

        var result = await new ItemLineSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_ITEM_LINE_CODE_CONFLICT");
    }

    [Fact]
    public async Task Applier_RejectsInvalidOrderBeforeRepository()
    {
        var repository = Substitute.For<IItemLineSyncApplyRepository>();
        var payload = Payload() with { SortOrder = -1 };
        var context = Context(payload);

        var result = await new ItemLineSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_ITEM_LINE_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default, default);
    }

    [Fact]
    public async Task Applier_RejectsGlobalIdMismatchBeforeRepository()
    {
        var repository = Substitute.For<IItemLineSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload) with { EntityGlobalId = Guid.NewGuid() };

        var result = await new ItemLineSyncEventApplier(repository).ApplyAsync(context);

        result.ErrorCode.Should().Be("SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");
        result.Terminal.Should().BeTrue();
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default, default);
    }

    private static ItemLineSyncPayload Payload() =>
        new(Guid.NewGuid(), "REFRIG", "Refrigerados", null, 20, true, false, DateTime.UtcNow);

    private static SyncEventApplyContext Context(ItemLineSyncPayload payload) =>
        new(Guid.NewGuid(), 1, "ItemLine", payload.GlobalId, SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)), 2, 3);
}
