using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.GeneralInventory;

public sealed class ItemGroupSyncPublishingTests
{
    private readonly IItemGroupRepository _repository = Substitute.For<IItemGroupRepository>();
    private readonly IChartOfAccountRepository _chartRepository = Substitute.For<IChartOfAccountRepository>();
    private readonly IItemGroupLocalOutboxWriter _writer = Substitute.For<IItemGroupLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task Create_WritesLocalOutboxInsideTheSameTransaction()
    {
        var itemGroup = CreateItemGroup();
        _repository.ExistsByCodeAsync(
                "INV-PAP", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.CreateAsync(
                Arg.Any<CreateItemGroupData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemGroup.Id);
        _repository.GetByIdAsync(
                itemGroup.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemGroup);
        var handler = new CreateItemGroupCommandHandler(
            _repository, _chartRepository, _transactionRunner, _writer);

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            itemGroup,
            SyncOperation.Created,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
        _transactionRunner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WritesDisabledOperation_WhenItemGroupBecomesInactive()
    {
        var itemGroup = CreateItemGroup(isActive: false);
        _repository.GetByIdAsync(
                itemGroup.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemGroup);
        _repository.ExistsByCodeAsync(
                "INV-PAP", itemGroup.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.UpdateWithResultAsync(
                Arg.Any<UpdateItemGroupData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(1);
        var handler = new UpdateItemGroupCommandHandler(
            _repository, _chartRepository, _transactionRunner, _writer);

        var result = await handler.Handle(UpdateCommand(itemGroup.Id, isActive: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            itemGroup,
            SyncOperation.Disabled,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_WritesLocalOutboxInsideTheSameTransaction()
    {
        var itemGroup = CreateItemGroup();
        _repository.GetByIdAsync(
                itemGroup.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemGroup);
        _repository.DeleteWithResultAsync(
                itemGroup.Id,
                7,
                "admin",
                _transactionRunner.Connection,
                _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(1);
        var handler = new DeleteItemGroupCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(
            new DeleteItemGroupCommand(itemGroup.Id, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            itemGroup,
            SyncOperation.Deleted,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_RejectsInactiveCanonicalAccount()
    {
        _chartRepository.GetLookupAsync(Arg.Any<CancellationToken>())
            .Returns([
                new ChartOfAccountLookupDto(9, "5199", "Ajuste", "Expense", null, 1, false)
            ]);
        var handler = new UpdateItemGroupCommandHandler(
            _repository, _chartRepository, _transactionRunner, _writer);
        var command = new UpdateItemGroupCommand(
            4, "INV-PAP", "Papeleria", null, null, null, true,
            null, null, null, null, null, null, "5199", null, 0,
            null, null, "114", "114", 7, "admin");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error =>
            error.Code == "ChartOfAccountNotFound" &&
            error.Field == nameof(UpdateItemGroupCommand.InventoryAdjustmentAccountCode));
        await _repository.DidNotReceiveWithAnyArgs().UpdateWithResultAsync(
            default!, default!, default!, default);
    }

    [Fact]
    public async Task Create_RollsBackWhenLocalOutboxFails()
    {
        var itemGroup = CreateItemGroup();
        _repository.ExistsByCodeAsync(
                "INV-PAP", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.CreateAsync(
                Arg.Any<CreateItemGroupData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemGroup.Id);
        _repository.GetByIdAsync(
                itemGroup.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemGroup);
        _writer.EnqueueAsync(
                Arg.Any<ItemGroupDto>(),
                Arg.Any<SyncOperation>(),
                Arg.Any<IDbConnection>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("Controlled outbox failure"));
        var handler = new CreateItemGroupCommandHandler(
            _repository, _chartRepository, _transactionRunner, _writer);

        var action = () => handler.Handle(CreateCommand(), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Controlled outbox failure");
        _transactionRunner.RolledBack.Should().BeTrue();
        _transactionRunner.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Writer_CreatesLimitedPayloadWithSapReference()
    {
        var itemGroup = CreateItemGroup();
        var companyContext = Substitute.For<ICompanyContext>();
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(Company());
        var localOutbox = Substitute.For<ILocalSyncOutboxRepository>();
        CreateLocalSyncOutboxData? captured = null;
        localOutbox.CreateAsync(
                Arg.Do<CreateLocalSyncOutboxData>(value => captured = value),
                _transactionRunner.Connection,
                _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(10);
        var writer = new ItemGroupLocalOutboxWriter(
            companyContext,
            new SyncEventPayloadFactory(),
            localOutbox);

        var eventId = await writer.EnqueueAsync(
            itemGroup,
            SyncOperation.Created,
            _transactionRunner.Connection,
            _transactionRunner.Transaction);

        eventId.Should().NotBeNull().And.NotBe(Guid.Empty);
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("ItemGroups");
        captured.EntityGlobalId.Should().Be(itemGroup.GlobalId!.Value);
        captured.PayloadJson.Should()
            .Contain("\"operation\":\"Created\"")
            .And.Contain("\"sapGroupCode\":\"114\"")
            .And.NotContain("\"createdByUserName\"")
            .And.NotContain("\"inventoryAccountName\"");
    }

    [Fact]
    public void Handlers_DoNotUseDirectMasterPublisher()
    {
        var handlers = string.Join(
            Environment.NewLine,
            ReadSourceFile("CreateItemGroupCommandHandler.cs"),
            ReadSourceFile("UpdateItemGroupCommandHandler.cs"),
            ReadSourceFile("DeleteItemGroupCommandHandler.cs"));

        handlers.Should().Contain("IItemGroupLocalOutboxWriter")
            .And.Contain("ExecuteInTenantTransactionAsync")
            .And.NotContain("ISyncEventPublisher")
            .And.NotContain(".PublishAsync(");
    }

    private static CreateItemGroupCommand CreateCommand() =>
        new("inv-pap", "Papeleria", null, true, null, null, null, null, "114", "114", 7, "admin");

    private static UpdateItemGroupCommand UpdateCommand(int id, bool isActive) =>
        new(id, "inv-pap", "Papeleria", null, isActive, null, null, null, null, "114", "114", 7, "admin");

    private static ItemGroupDto CreateItemGroup(bool isActive = true) =>
        new()
        {
            Id = 4,
            GlobalId = Guid.NewGuid(),
            Code = "INV-PAP",
            Name = "Papeleria",
            SapGroupCode = "114",
            SapCode = "114",
            IsActive = isActive,
            ExternalSystem = "SAPB1",
            ExternalCode = "114",
            CreatedAt = new DateTime(2026, 7, 17, 10, 0, 0, DateTimeKind.Utc)
        };

    private static CompanyConnectionInfo Company() =>
        new(
            CompanyId: 10,
            CompanyCode: "MASTER",
            CommercialName: "Empresa Master",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: true);

    private static string ReadSourceFile(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(
                directory.FullName,
                "src",
                "Backend",
                "NuanSystem.Application",
                "Features",
                "GeneralInventory",
                "ItemGroups",
                "Commands",
                fileName);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro {fileName}.");
    }

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public async Task ExecuteInTenantTransactionAsync(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        {
            await ExecuteInTenantTransactionAsync<object?>(
                async (connection, transaction, token) =>
                {
                    await operation(connection, transaction, token);
                    return null;
                },
                cancellationToken);
        }

        public async Task<T> ExecuteInTenantTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await operation(Connection, Transaction, cancellationToken);
                Committed = true;
                return result;
            }
            catch
            {
                RolledBack = true;
                throw;
            }
        }
    }
}
