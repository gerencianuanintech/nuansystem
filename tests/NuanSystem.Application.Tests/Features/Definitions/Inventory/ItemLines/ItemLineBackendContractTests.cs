using System.Data;
using System.Runtime.CompilerServices;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemLines;

public sealed class ItemLineBackendContractTests
{
    [Fact]
    public async Task Validator_EnforcesLengthsAndNonNegativeOrder()
    {
        var command = new CreateItemLineCommand("", "", new string('D', 501), -1, true);
        var result = await new CreateItemLineCommandValidator().ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(x => x.PropertyName).Should().Contain([
            "Code", "Name", "Description", "SortOrder"]);
    }

    [Fact]
    public void Endpoint_UsesCanonicalRoutePermissionsAndOperations()
    {
        var endpoint = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory",
            "ItemLines", "ItemLineEndpoints.cs");
        var catalog = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "InventoryCatalogEndpoints.cs");

        endpoint.Should().Contain("/api/definitions/inventory/item-lines")
            .And.Contain("private const string FormKey = \"item-lines\"")
            .And.Contain("GeneralInventoryItemLinesRead")
            .And.Contain("GeneralInventoryItemLinesManage")
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")")
            .And.Contain("RequireFormOperation(FormKey, \"consult\")")
            .And.Contain("RequireFormOperation(FormKey, \"history\")")
            .And.Contain("RequireFormOperation(FormKey, \"create\")")
            .And.Contain("RequireFormOperation(FormKey, \"update\")")
            .And.Contain("RequireFormOperation(FormKey, \"delete\")");
        catalog.Should().Contain("app.MapItemLineEndpoints();")
            .And.NotContain("\"item-lines\",");
    }

    [Fact]
    public async Task Create_NormalizesAndWritesOutboxInSameTransaction()
    {
        var repository = Substitute.For<IItemLineRepository>();
        var writer = Substitute.For<IItemLineLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var line = Line();
        repository.ExistsByCodeAsync("REFRIG", null, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateAsync(Arg.Is<CreateItemLineData>(x =>
                x.Code == "REFRIG" && x.Name == "Refrigerados" && x.SortOrder == 20),
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(line.Id);
        repository.GetByIdAsync(line.Id, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(line);

        var result = await new CreateItemLineCommandHandler(repository, runner, writer).Handle(
            new(" refrig ", " Refrigerados ", " Conservacion controlada ", 20, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        runner.Committed.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(line, SyncOperation.Created,
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Inactive_WritesDisabledOperation()
    {
        var repository = Substitute.For<IItemLineRepository>();
        var writer = Substitute.For<IItemLineLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var line = Line(false);
        repository.GetByIdAsync(line.Id, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(line);
        repository.ExistsByCodeAsync(line.Code, line.Id, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(false);
        repository.UpdateAsync(Arg.Any<UpdateItemLineData>(), runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(1);

        var result = await new UpdateItemLineCommandHandler(repository, runner, writer).Handle(
            new(line.Id, line.Code, line.Name, null, 20, false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(line, SyncOperation.Disabled,
            runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_InUse_ReturnsStableErrorAndDoesNotPublish()
    {
        var repository = Substitute.For<IItemLineRepository>();
        var writer = Substitute.For<IItemLineLocalOutboxWriter>();
        var runner = new ImmediateTransactionRunner();
        var line = Line();
        repository.GetByIdAsync(line.Id, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(line);
        repository.DeleteAsync(line.Id, null, null, runner.Connection, runner.Transaction,
            Arg.Any<CancellationToken>()).Returns(-3);

        var result = await new DeleteItemLineCommandHandler(repository, runner, writer)
            .Handle(new(line.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x => x.Code == "ItemLineInUse");
        await writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public void Contract_IsIndependentAndDoesNotPersistSapOrExternalFields()
    {
        var dto = Read("src", "Backend", "NuanSystem.Application", "Features", "Definitions", "Inventory",
            "ItemLines", "Dtos", "ItemLineDtos.cs");
        var endpoint = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory",
            "ItemLines", "ItemLineEndpoints.cs");

        dto.Should().Contain("Guid GlobalId").And.Contain("int SortOrder")
            .And.NotContain("SapCode").And.NotContain("ExternalSystem").And.NotContain("IsSystem");
        endpoint.Should().NotContain("SapCode").And.NotContain("ExternalCode");
    }

    [Fact]
    public void SyncVertical_RegistersFullApplyWorkerAndRemovesGenericOwner()
    {
        var persistence = Read("src", "Backend", "NuanSystem.Persistence", "DependencyInjection",
            "PersistenceServiceRegistration.cs");
        var worker = Read("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Program.cs");
        var full = Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync",
            "SyncFullEntitySources.cs");
        var generic = Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "GeneralInventory",
            "GeneralInventoryCatalogRepository.cs");

        persistence.Should().Contain("AddScoped<ISyncFullEntitySource, ItemLineFullEntitySource>()")
            .And.Contain("AddScoped<IItemLineSyncApplyRepository, ItemLineSyncApplyRepository>()");
        worker.Should().Contain("AddScoped<ISyncEntityEventApplier, ItemLineSyncEventApplier>()");
        full.Should().Contain("SP_NA_GET_ITEM_LINE_SYNC_FULL")
            .And.Contain("new { AfterId = afterId, BatchSize = requested }")
            .And.Contain("LastOrDefault()?.Id.ToString()");
        generic.Should().NotContain("[\"item-lines\"]");
    }

    private static ItemLineDto Line(bool active = true) => new()
    {
        Id = 9, GlobalId = Guid.NewGuid(), Code = "REFRIG", Name = "Refrigerados",
        SortOrder = 20, IsActive = active, CreatedAt = DateTime.UtcNow
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
