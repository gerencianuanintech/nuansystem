using System.Data;
using System.Runtime.CompilerServices;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemBrands;

public sealed class ItemBrandBackendContractTests
{
    [Fact]
    public async Task Validator_EnforcesFieldsExternalPairAndSapLengths()
    {
        var validator = new CreateItemBrandCommandValidator();
        var command = new CreateItemBrandCommand("", "", null, -1, true,
            "SAP_B1", null, new string('A', 51), new string('B', 51));

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(x => x.PropertyName).Should().Contain([
            "Code", "Name", "SortOrder", "ExternalCode", "SapManufacturerCode", "SapCode"]);
    }

    [Fact]
    public void Endpoint_UsesOnlyCanonicalRouteOwnPermissionsAndOperations()
    {
        var endpoint = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory",
            "ItemBrands", "ItemBrandEndpoints.cs");
        var catalog = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "InventoryCatalogEndpoints.cs");

        endpoint.Should().Contain("/api/definitions/inventory/item-brands")
            .And.Contain("private const string FormKey = \"item-brands\"")
            .And.Contain("GeneralInventoryItemBrandsRead")
            .And.Contain("GeneralInventoryItemBrandsManage")
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")")
            .And.Contain("RequireFormOperation(FormKey, \"consult\")")
            .And.Contain("RequireFormOperation(FormKey, \"history\")")
            .And.Contain("RequireFormOperation(FormKey, \"create\")")
            .And.Contain("RequireFormOperation(FormKey, \"update\")")
            .And.Contain("RequireFormOperation(FormKey, \"delete\")");
        catalog.Should().NotContain("\"item-brands\",");
    }

    [Fact]
    public async Task Create_NormalizesAndWritesOutboxInSameTransaction()
    {
        var repository = Substitute.For<IItemBrandRepository>();
        var writer = Substitute.For<IItemBrandLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var brand = Brand();
        repository.ExistsByCodeAsync("MONI", null, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateAsync(Arg.Is<CreateItemBrandData>(x => x.Code == "MONI" && x.Name == "Moni"),
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(brand.Id);
        repository.GetByIdAsync(brand.Id, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(brand);
        var handler = new CreateItemBrandCommandHandler(repository, runner, writer);

        var result = await handler.Handle(new(" moni ", " Moni ", null, 10, true,
            "SAP_B1", "42", "42", "42", 1, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        runner.Committed.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(brand, SyncOperation.Created,
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Inactive_WritesDisabledOperation()
    {
        var repository = Substitute.For<IItemBrandRepository>();
        var writer = Substitute.For<IItemBrandLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var brand = Brand(false);
        repository.GetByIdAsync(brand.Id, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(brand);
        repository.ExistsByCodeAsync("MONI", brand.Id, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        repository.UpdateAsync(Arg.Any<UpdateItemBrandData>(), runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(1);
        var handler = new UpdateItemBrandCommandHandler(repository, runner, writer);

        var result = await handler.Handle(new(brand.Id, "MONI", "Moni", null, 10, false,
            null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(brand, SyncOperation.Disabled,
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_InUse_ReturnsStableErrorAndDoesNotPublish()
    {
        var repository = Substitute.For<IItemBrandRepository>();
        var writer = Substitute.For<IItemBrandLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var brand = Brand();
        repository.GetByIdAsync(brand.Id, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(brand);
        repository.DeleteAsync(brand.Id, null, null, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(-2);
        var handler = new DeleteItemBrandCommandHandler(repository, runner, writer);

        var result = await handler.Handle(new(brand.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Code == "ItemBrandInUse");
        await writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public void SyncContract_UsesGlobalIdAndPreservesLocalExternalReferences()
    {
        var dto = Read("src", "Backend", "NuanSystem.Application", "Features", "Definitions", "Inventory",
            "ItemBrands", "Dtos", "ItemBrandDtos.cs");
        var apply = Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "ItemBrandSyncApplyRepository.cs");

        dto.Should().Contain("record ItemBrandSyncPayload").And.Contain("bool IsDeleted")
            .And.NotContain("BrandId");
        apply.Should().Contain("SP_NA_POST_ITEM_BRAND_SYNC_APPLY")
            .And.Contain("SYNC_ITEM_BRAND_CODE_CONFLICT")
            .And.NotContain("SapManufacturerCode =")
            .And.NotContain("ExternalSystem =");
    }

    [Fact]
    public void Initializers_RegisterMigrations190Through192()
    {
        Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs")
            .Should().Contain("190_tenant_item_brands_master.sql");
        var master = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        master.Should().Contain("191_master_definitions_inventory_item_brands_navigation.sql")
            .And.Contain("192_master_item_brands_sync_registration.sql");
    }

    private static ItemBrandDto Brand(bool active = true) => new()
    {
        Id = 8, GlobalId = Guid.NewGuid(), Code = "MONI", Name = "Moni", SortOrder = 10,
        IsActive = active, CreatedAt = new DateTime(2026, 8, 12, 10, 0, 0, DateTimeKind.Utc)
    };

    private static string Read(params string[] parts) => File.ReadAllText(Path.Combine([Root().FullName, .. parts]));
    private static DirectoryInfo Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "nuansystem.sln"))) return directory;
            directory = directory.Parent;
        }
        return FindRootFromSource();
    }
    private static DirectoryInfo FindRootFromSource([CallerFilePath] string path = "") =>
        new DirectoryInfo(path).Parent!.Parent!.Parent!.Parent!.Parent!.Parent!.Parent!;

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }
        public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken ct = default) =>
            await operation(Connection, Transaction, ct);
        public async Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken ct = default)
        {
            var result = await operation(Connection, Transaction, ct);
            Committed = true;
            return result;
        }
    }
}
