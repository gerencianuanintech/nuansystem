using System.Data;
using System.Runtime.CompilerServices;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ProductTypes;

public sealed class ProductTypeBackendContractTests
{
    [Fact]
    public async Task Validator_EnforcesClosedNatureLengthsAndOrder()
    {
        var command = new CreateProductTypeCommand(
            "", "", new string('D', 501), "Service", -1, true);
        var result = await new CreateProductTypeCommandValidator().ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(x => x.PropertyName).Should().Contain([
            "Code", "Name", "Description", "NatureCode", "SortOrder"]);
    }

    [Fact]
    public void Endpoint_UsesOnlyCanonicalRoutePermissionsAndOperations()
    {
        var endpoint = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory",
            "ProductTypes", "ProductTypeEndpoints.cs");
        var catalog = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "InventoryCatalogEndpoints.cs");

        endpoint.Should().Contain("/api/definitions/inventory/product-types")
            .And.Contain("private const string FormKey = \"product-types\"")
            .And.Contain("GeneralInventoryProductTypesRead")
            .And.Contain("GeneralInventoryProductTypesManage")
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")")
            .And.Contain("RequireFormOperation(FormKey, \"consult\")")
            .And.Contain("RequireFormOperation(FormKey, \"history\")")
            .And.Contain("RequireFormOperation(FormKey, \"create\")")
            .And.Contain("RequireFormOperation(FormKey, \"update\")")
            .And.Contain("RequireFormOperation(FormKey, \"delete\")");
        catalog.Should().Contain("app.MapProductTypeEndpoints();")
            .And.NotContain("\"product-types\",");
    }

    [Fact]
    public async Task Create_NormalizesAndWritesOutboxInSameTransaction()
    {
        var repository = Substitute.For<IProductTypeRepository>();
        var writer = Substitute.For<IProductTypeLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var productType = ProductType();
        repository.ExistsByCodeAsync("PROD_TERM", null, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateAsync(Arg.Is<CreateProductTypeData>(x =>
                x.Code == "PROD_TERM" && x.NatureCode == ProductTypeNatureCodes.FinishedGood),
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(productType.Id);
        repository.GetByIdAsync(productType.Id, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(productType);

        var result = await new CreateProductTypeCommandHandler(repository, runner, writer).Handle(
            new(" prod_term ", " Producto terminado ", null, "finishedgood", 20, true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        runner.Committed.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(productType, SyncOperation.Created,
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(-2, "ProductTypeSystemProtected")]
    [InlineData(-3, "ProductTypeInUse")]
    public async Task Delete_ExpectedSqlOutcome_ReturnsStableErrorAndDoesNotPublish(int sqlOutcome, string expectedCode)
    {
        var repository = Substitute.For<IProductTypeRepository>();
        var writer = Substitute.For<IProductTypeLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var productType = ProductType();
        repository.GetByIdAsync(productType.Id, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(productType);
        repository.DeleteAsync(productType.Id, null, null, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(sqlOutcome);

        var result = await new DeleteProductTypeCommandHandler(repository, runner, writer)
            .Handle(new(productType.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Code == expectedCode);
        await writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public void SyncContract_UsesGlobalIdentityWithoutSapOrItemPayloadCoupling()
    {
        var catalog = Read("src", "Backend", "NuanSystem.Application", "Features", "Sync",
            "Configuration", "SyncMasterBranchEntityCatalog.cs");
        var dto = Read("src", "Backend", "NuanSystem.Application", "Features", "Definitions", "Inventory",
            "ProductTypes", "Dtos", "ProductTypeDtos.cs");

        catalog.Should().Contain("DefaultKeyField: \"GlobalId\"")
            .And.Contain("Dependencies: [ItemGroups, ItemFamilies, UnitOfMeasures]")
            .And.Contain("ProductType queda fuera de las dependencias hasta incorporarse al payload");
        var payload = dto[dto.IndexOf("public sealed record ProductTypeSyncPayload", StringComparison.Ordinal)..];
        payload.Should().Contain("Guid GlobalId").And.Contain("string NatureCode")
            .And.NotContain("ExternalSystem").And.NotContain("ExternalCode").And.NotContain("SapCode");
    }

    [Fact]
    public void InitializersAndFullSource_KeepGlobalIdentityWithStableLocalCursor()
    {
        var tenant = Read("src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerTenantDatabaseInitializer.cs");
        var master = Read("src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");
        var full = Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync",
            "SyncFullEntitySources.cs");

        tenant.Should().Contain("198_tenant_product_types_master.sql");
        master.IndexOf("199_master_definitions_inventory_product_types_navigation.sql", StringComparison.Ordinal)
            .Should().BeLessThan(master.IndexOf("200_master_product_types_sync_registration.sql", StringComparison.Ordinal));
        full.Should().Contain("SP_NA_GET_PRODUCT_TYPE_SYNC_FULL")
            .And.Contain("new { AfterId = afterId, BatchSize = requested }")
            .And.Contain("new SyncSourceRecord(row.GlobalId, row.Code")
            .And.Contain("rows.Take(take).LastOrDefault()?.Id.ToString()");
    }

    private static ProductTypeDto ProductType() => new()
    {
        Id = 7, GlobalId = Guid.NewGuid(), Code = "PROD_TERM", Name = "Producto terminado",
        NatureCode = ProductTypeNatureCodes.FinishedGood, SortOrder = 20, IsSystem = true,
        IsActive = true, CreatedAt = DateTime.UtcNow
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
        public async Task ExecuteInTenantTransactionAsync(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken ct = default) =>
            await operation(Connection, Transaction, ct);
        public async Task<T> ExecuteInTenantTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken ct = default)
        {
            var result = await operation(Connection, Transaction, ct);
            Committed = true;
            return result;
        }
    }
}
