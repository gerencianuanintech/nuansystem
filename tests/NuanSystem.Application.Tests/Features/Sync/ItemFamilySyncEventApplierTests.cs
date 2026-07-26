using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ItemFamilySyncEventApplierTests
{
    [Fact]
    public async Task Created_RetriesWhileItemGroupDependencyIsMissing()
    {
        var repository = Substitute.For<IItemFamilySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.ItemGroupExistsAsync(1002, payload.ItemGroupGlobalId, Arg.Any<CancellationToken>())
            .Returns(false);
        var applier = new ItemFamilySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeTrue();
        result.Terminal.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_ITEM_FAMILY_ITEM_GROUP_PENDING");
        await repository.DidNotReceiveWithAnyArgs()
            .UpsertFromSyncAsync(default, default!, default!, default, default);
    }

    [Fact]
    public async Task Created_AppliesByGlobalIdAfterItemGroupExists()
    {
        var repository = Substitute.For<IItemFamilySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.ItemGroupExistsAsync(1002, payload.ItemGroupGlobalId, Arg.Any<CancellationToken>())
            .Returns(true);
        repository.UpsertFromSyncAsync(
                1002, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ItemFamilySyncApplyResult(true, false, false, 25, "Creado."));
        var applier = new ItemFamilySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        result.Terminal.Should().BeFalse();
        await repository.Received(1).UpsertFromSyncAsync(
            1002,
            context,
            Arg.Is<ItemFamilySyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.ItemGroupGlobalId == payload.ItemGroupGlobalId &&
                value.Code == "FAM"),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CodeCollision_IsTerminalAndNeverAdoptedAutomatically()
    {
        var repository = Substitute.For<IItemFamilySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Updated);
        repository.ItemGroupExistsAsync(1002, payload.ItemGroupGlobalId, Arg.Any<CancellationToken>())
            .Returns(true);
        repository.UpsertFromSyncAsync(
                1002, context, payload, SyncOperation.Updated, Arg.Any<CancellationToken>())
            .Returns(new ItemFamilySyncApplyResult(
                false,
                false,
                true,
                null,
                "Codigo ocupado por otro GlobalId.",
                "SYNC_ITEM_FAMILY_CODE_CONFLICT"));
        var applier = new ItemFamilySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_ITEM_FAMILY_CODE_CONFLICT");
    }

    [Fact]
    public void Persistence_UsesStoredProcedureAndTerminalInboxWithoutCodeAdoption()
    {
        var repository = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "ItemFamilySyncApplyRepository.cs");
        var script = ReadSourceFile("database", "sql", "127_tenant_item_family_master_branch_sync.sql");

        repository.Should().Contain("SP_NA_POST_ITEM_FAMILY_SYNC_APPLY");
        repository.Should().Contain("CommandType.StoredProcedure");
        repository.Should().Contain("Status = N'DeadLetter'");
        script.Should().Contain("CONVERT(int, -2) AS ResultCode");
        script.Should().Contain("@ConflictingItemFamilyId IS NOT NULL");
        script.Should().NotContain("SET @GlobalId =");
    }

    [Fact]
    public void FullSource_UsesCompositeGroupAndFamilyKeyForStablePagination()
    {
        var source = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "SyncFullEntitySources.cs");

        source.Should().Contain("CONCAT(itemGroup.Code, N'|', family.Code) AS EntityKey");
        source.Should().Contain("CONCAT(itemGroup.Code, N'|', family.Code) > @LastKey");
        source.Should().Contain("ORDER BY itemGroup.Code, family.Code");
    }

    [Fact]
    public void RuntimeAndSqlRegistration_KeepItemFamilyDisabledWithRequiredDependencies()
    {
        var catalog = ReadSourceFile(
            "src", "Backend", "NuanSystem.Application", "Features", "Sync", "Configuration", "SyncMasterBranchEntityCatalog.cs");
        var worker = ReadSourceFile(
            "src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Program.cs");
        var persistence = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "DependencyInjection", "PersistenceServiceRegistration.cs");
        var tenantInitializer = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");
        var masterInitializer = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        var masterScript = ReadSourceFile("database", "sql", "128_master_item_family_sync_registration.sql");

        catalog.Should().Contain("new(ItemFamilies, ItemFamilies")
            .And.Contain("Dependencies: [ItemGroups]")
            .And.Contain("Dependencies: [ItemGroups, ItemFamilies]");
        worker.Should().Contain("ItemFamilySyncEventApplier");
        persistence.Should().Contain("ItemFamilyFullEntitySource")
            .And.Contain("ItemFamilySyncApplyRepository");
        tenantInitializer.Should().Contain("127_tenant_item_family_master_branch_sync.sql");
        masterInitializer.Should().Contain("128_master_item_family_sync_registration.sql");
        masterScript.Should().Contain("N'ItemFamilies'")
            .And.Contain("CONVERT(bit, 0)")
            .And.Contain("N'MasterToBranch'")
            .And.Contain("N'MasterWins'");
    }

    private static ItemFamilySyncPayload CreatePayload() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "GENERAL",
            "FAM",
            "Familia",
            "Descripcion",
            true,
            "114",
            "114",
            "SAPB1",
            "114",
            new DateTime(2026, 7, 25, 10, 0, 0, DateTimeKind.Utc),
            null);

    private static SyncEventApplyContext CreateContext(
        ItemFamilySyncPayload payload,
        SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "ItemFamilies",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };

        return new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "ItemFamilies",
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
