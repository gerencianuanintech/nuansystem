using System.Data;
using System.Runtime.CompilerServices;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.UnitMeasures;

public sealed class UnitMeasureBackendContractTests
{
    [Fact]
    public async Task Validator_EnforcesClosedMagnitudeLengthsOrderAndExternalPair()
    {
        var command = new CreateUnitMeasureCommand("", "", new string('D', 501), new string('S', 21),
            "Temperature", -1, true, "SAP_B1", null);

        var result = await new CreateUnitMeasureCommandValidator().ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(x => x.PropertyName).Should().Contain([
            "Code", "Name", "Description", "Symbol", "MagnitudeCode", "SortOrder", "ExternalCode"]);
    }

    [Fact]
    public void Endpoint_UsesOnlyCanonicalRouteOwnPermissionsAndOperations()
    {
        var endpoint = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory",
            "UnitMeasures", "UnitMeasureEndpoints.cs");
        var catalog = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "InventoryCatalogEndpoints.cs");

        endpoint.Should().Contain("/api/definitions/inventory/unit-measures")
            .And.Contain("private const string FormKey = \"unit-measures\"")
            .And.Contain("GeneralInventoryUnitMeasuresRead")
            .And.Contain("GeneralInventoryUnitMeasuresManage")
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")")
            .And.Contain("RequireFormOperation(FormKey, \"consult\")")
            .And.Contain("RequireFormOperation(FormKey, \"history\")")
            .And.Contain("RequireFormOperation(FormKey, \"create\")")
            .And.Contain("RequireFormOperation(FormKey, \"update\")")
            .And.Contain("RequireFormOperation(FormKey, \"delete\")");
        catalog.Should().Contain("app.MapUnitMeasureEndpoints();")
            .And.NotContain("\"unit-measures\",");
    }

    [Fact]
    public async Task Create_NormalizesAndWritesOutboxInSameTransaction()
    {
        var repository = Substitute.For<IUnitMeasureRepository>();
        var writer = Substitute.For<IUnitMeasureLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var unit = Unit();
        repository.ExistsByCodeAsync("UND", null, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateAsync(Arg.Is<CreateUnitMeasureData>(x => x.Code == "UND" && x.MagnitudeCode == "Quantity"),
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(unit.Id);
        repository.GetByIdAsync(unit.Id, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(unit);

        var result = await new CreateUnitMeasureCommandHandler(repository, runner, writer).Handle(
            new(" und ", " Unidad ", null, " UND ", "quantity", 10, true, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        runner.Committed.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(unit, SyncOperation.Created,
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_InUse_ReturnsStableErrorAndDoesNotPublish()
    {
        var repository = Substitute.For<IUnitMeasureRepository>();
        var writer = Substitute.For<IUnitMeasureLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var unit = Unit();
        repository.GetByIdAsync(unit.Id, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(unit);
        repository.DeleteAsync(unit.Id, null, null, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(-2);

        var result = await new DeleteUnitMeasureCommandHandler(repository, runner, writer)
            .Handle(new(unit.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Code == "UnitMeasureInUse");
        await writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public void Initializers_RegisterMigrations193Through197InTheirDatabaseOrder()
    {
        var tenant = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");
        tenant.IndexOf("194_tenant_unit_of_measures_master.sql", StringComparison.Ordinal).Should()
            .BeLessThan(tenant.IndexOf("196_tenant_unit_of_measures_incremental_sync.sql", StringComparison.Ordinal));
        var master = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        master.IndexOf("193_master_item_brands_dependency_repair.sql", StringComparison.Ordinal).Should()
            .BeLessThan(master.IndexOf("195_master_definitions_inventory_unit_of_measures_navigation.sql", StringComparison.Ordinal));
        master.IndexOf("195_master_definitions_inventory_unit_of_measures_navigation.sql", StringComparison.Ordinal).Should()
            .BeLessThan(master.IndexOf("197_master_unit_of_measures_sync_registration.sql", StringComparison.Ordinal));
    }

    private static UnitMeasureDto Unit() => new()
    {
        Id = 4, GlobalId = Guid.NewGuid(), Code = "UND", Name = "Unidad", Symbol = "UND",
        MagnitudeCode = "Quantity", SortOrder = 10, IsActive = true, CreatedAt = DateTime.UtcNow
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
