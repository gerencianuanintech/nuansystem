using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ProductTypeSyncEventApplierTests
{
    [Fact]
    public async Task Applier_PropagatesTerminalCodeCollision()
    {
        var repository = Substitute.For<IProductTypeSyncApplyRepository>();
        var payload = new ProductTypeSyncPayload(Guid.NewGuid(), "PROD_TERM", "Producto terminado", null,
            ProductTypeNatureCodes.FinishedGood, 20, true, true, false, DateTime.UtcNow);
        var context = new SyncEventApplyContext(Guid.NewGuid(), 1, "ProductType", payload.GlobalId,
            SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)), 2, 3);
        repository.ApplyAsync(2, context, Arg.Any<ProductTypeSyncPayload>(), SyncOperation.Created,
                Arg.Any<CancellationToken>())
            .Returns(new ProductTypeSyncApplyResult(false, false, true, null, "Codigo ocupado.",
                "SYNC_PRODUCT_TYPE_CODE_CONFLICT"));

        var result = await new ProductTypeSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_PRODUCT_TYPE_CODE_CONFLICT");
    }

    [Fact]
    public async Task Applier_RejectsUnknownNatureBeforeRepository()
    {
        var repository = Substitute.For<IProductTypeSyncApplyRepository>();
        var payload = new ProductTypeSyncPayload(Guid.NewGuid(), "SERV", "Servicio", null,
            "Service", 20, false, true, false, DateTime.UtcNow);
        var context = new SyncEventApplyContext(Guid.NewGuid(), 1, "ProductType", payload.GlobalId,
            SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)), 2, 3);

        var result = await new ProductTypeSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PRODUCT_TYPE_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default, default);
    }
}
