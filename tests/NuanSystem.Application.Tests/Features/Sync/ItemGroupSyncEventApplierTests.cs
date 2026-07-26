using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemGroupSyncEventApplierTests
{
    [Fact]
    public async Task Created_UpsertsItemGroupByGlobalId()
    {
        var repository = Substitute.For<IItemGroupSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(1002, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemGroupSyncApplyResult(true, false, false, 1, "Creado."));
        var applier = new ItemGroupSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            1002,
            context,
            Arg.Is<ItemGroupSyncPayload>(value => value.GlobalId == payload.GlobalId && value.Code == "INV-PAP"),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deleted_MarksItemGroupAsDeleted()
    {
        var repository = Substitute.For<IItemGroupSyncApplyRepository>();
        var payload = CreatePayload(isActive: false);
        var context = CreateContext(payload, SyncOperation.Deleted);
        repository.DisableFromSyncAsync(1002, context, payload, true, Arg.Any<CancellationToken>())
            .Returns(new ItemGroupSyncApplyResult(true, false, false, 1, "Eliminado."));
        var applier = new ItemGroupSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).DisableFromSyncAsync(
            1002,
            context,
            payload,
            markDeleted: true,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CodeCollision_IsTerminalAndNeverAdoptedAutomatically()
    {
        var repository = Substitute.For<IItemGroupSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Updated);
        repository.UpsertFromSyncAsync(
                1002, context, payload, SyncOperation.Updated, Arg.Any<CancellationToken>())
            .Returns(new ItemGroupSyncApplyResult(
                false,
                false,
                true,
                null,
                "Codigo ocupado por otro GlobalId.",
                "SYNC_ITEM_GROUP_CODE_CONFLICT"));
        var applier = new ItemGroupSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_ITEM_GROUP_CODE_CONFLICT");
    }

    [Fact]
    public void Persistence_UsesStoredProcedureAndProtectsInboxIdempotency()
    {
        var repository = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "ItemGroupSyncApplyRepository.cs");
        var tenantScript = ReadSourceFile("database", "sql", "129_tenant_item_group_transactional_outbox.sql");

        repository.Should().Contain("SP_NA_POST_ITEM_GROUP_SYNC_APPLY");
        repository.Should().Contain("CommandType.StoredProcedure");
        repository.Should().Contain("WHERE EventId = @EventId");
        repository.Should().Contain("Status = N'Applied'");
        repository.Should().Contain("Status = N'DeadLetter'");
        tenantScript.Should().Contain("CONVERT(int, -2) AS ResultCode");
        tenantScript.Should().Contain("@ConflictingItemGroupId IS NOT NULL");
        tenantScript.Should().NotContain("SET GlobalId = @GlobalId")
            .And.NotContain("SET @GlobalId =");
    }

    private static ItemGroupSyncPayload CreatePayload(bool isActive = true)
    {
        return new ItemGroupSyncPayload(
            Guid.NewGuid(),
            "INV-PAP",
            "Papeleria",
            null,
            null,
            null,
            null,
            null,
            "114",
            "114",
            isActive,
            "SAPB1",
            "114",
            new DateTime(2026, 7, 17, 10, 0, 0, DateTimeKind.Utc),
            null);
    }

    private static SyncEventApplyContext CreateContext(ItemGroupSyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "ItemGroups",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };

        return new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "ItemGroups",
            payload.GlobalId,
            operation.ToString(),
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            1002,
            10);
    }

    private static string ReadSourceFile(params string[] pathParts)
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

        throw new FileNotFoundException($"No se encontro {Path.Combine(pathParts)}.");
    }
}
