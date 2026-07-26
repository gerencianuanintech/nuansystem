using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.GeneralInventory;

public sealed class ItemFamilySyncPublishingTests
{
    private readonly IItemFamilyRepository _repository = Substitute.For<IItemFamilyRepository>();
    private readonly IItemGroupRepository _itemGroupRepository = Substitute.For<IItemGroupRepository>();
    private readonly IItemFamilyLocalOutboxWriter _writer = Substitute.For<IItemFamilyLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task Create_WritesLocalOutboxInsideTheSameTransaction()
    {
        var itemFamily = CreateItemFamily();
        _itemGroupRepository.GetByIdAsync(3, Arg.Any<CancellationToken>())
            .Returns(new ItemGroupDto { Id = 3, GlobalId = itemFamily.ItemGroupGlobalId, Code = "GENERAL" });
        _repository.ExistsByCodeAsync(
                3, "FAM", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.CreateAsync(
                Arg.Any<CreateItemFamilyData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemFamily.Id);
        _repository.GetByIdAsync(
                itemFamily.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemFamily);
        var handler = new CreateItemFamilyCommandHandler(
            _repository, _itemGroupRepository, _transactionRunner, _writer);

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            itemFamily,
            SyncOperation.Created,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
        _transactionRunner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WritesDisabledOperation_WhenItemFamilyBecomesInactive()
    {
        var itemFamily = CreateItemFamily(isActive: false);
        _itemGroupRepository.GetByIdAsync(3, Arg.Any<CancellationToken>())
            .Returns(new ItemGroupDto { Id = 3, GlobalId = itemFamily.ItemGroupGlobalId, Code = "GENERAL" });
        _repository.GetByIdAsync(
                itemFamily.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemFamily);
        _repository.ExistsByCodeAsync(
                3, "FAM", itemFamily.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.UpdateAsync(
                Arg.Any<UpdateItemFamilyData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new UpdateItemFamilyCommandHandler(
            _repository, _itemGroupRepository, _transactionRunner, _writer);

        var result = await handler.Handle(UpdateCommand(itemFamily.Id, isActive: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            itemFamily,
            SyncOperation.Disabled,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_WritesLocalOutboxInsideTheSameTransaction()
    {
        var itemFamily = CreateItemFamily();
        _repository.GetByIdAsync(
                itemFamily.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemFamily);
        _repository.DeleteAsync(
                itemFamily.Id,
                7,
                "admin",
                _transactionRunner.Connection,
                _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new DeleteItemFamilyCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(
            new DeleteItemFamilyCommand(itemFamily.Id, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            itemFamily,
            SyncOperation.Deleted,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenLocalOutboxFails()
    {
        var itemFamily = CreateItemFamily();
        _itemGroupRepository.GetByIdAsync(3, Arg.Any<CancellationToken>())
            .Returns(new ItemGroupDto { Id = 3, GlobalId = itemFamily.ItemGroupGlobalId, Code = "GENERAL" });
        _repository.ExistsByCodeAsync(
                3, "FAM", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.CreateAsync(
                Arg.Any<CreateItemFamilyData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemFamily.Id);
        _repository.GetByIdAsync(
                itemFamily.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(itemFamily);
        _writer.EnqueueAsync(
                Arg.Any<ItemFamilyDto>(),
                Arg.Any<SyncOperation>(),
                Arg.Any<IDbConnection>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("Controlled outbox failure"));
        var handler = new CreateItemFamilyCommandHandler(
            _repository, _itemGroupRepository, _transactionRunner, _writer);

        var action = () => handler.Handle(CreateCommand(), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Controlled outbox failure");
        _transactionRunner.RolledBack.Should().BeTrue();
        _transactionRunner.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Writer_CreatesLimitedPayloadWithParentIdentity()
    {
        var itemFamily = CreateItemFamily();
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
        var writer = new ItemFamilyLocalOutboxWriter(
            companyContext,
            new SyncEventPayloadFactory(),
            localOutbox);

        var eventId = await writer.EnqueueAsync(
            itemFamily,
            SyncOperation.Created,
            _transactionRunner.Connection,
            _transactionRunner.Transaction);

        eventId.Should().NotBeNull().And.NotBe(Guid.Empty);
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("ItemFamilies");
        captured.EntityGlobalId.Should().Be(itemFamily.GlobalId!.Value);
        captured.PayloadJson.Should()
            .Contain(itemFamily.ItemGroupGlobalId!.Value.ToString())
            .And.Contain("\"operation\":\"Created\"")
            .And.NotContain("\"itemGroupId\"")
            .And.NotContain("\"createdByUserName\"");
    }

    private static CreateItemFamilyCommand CreateCommand() =>
        new(3, "fam", "Familia", "Descripcion", true, "114", "114", 7, "admin");

    private static UpdateItemFamilyCommand UpdateCommand(int id, bool isActive) =>
        new(id, 3, "fam", "Familia", "Descripcion", isActive, "114", "114", 7, "admin");

    private static ItemFamilyDto CreateItemFamily(bool isActive = true) =>
        new()
        {
            Id = 25,
            GlobalId = Guid.NewGuid(),
            ItemGroupId = 3,
            ItemGroupGlobalId = Guid.NewGuid(),
            ItemGroupCode = "GENERAL",
            ItemGroupName = "General",
            Code = "FAM",
            Name = "Familia",
            Description = "Descripcion",
            IsActive = isActive,
            SapFamilyCode = "114",
            SapCode = "114",
            ExternalSystem = "SAPB1",
            ExternalCode = "114",
            CreatedAt = new DateTime(2026, 7, 25, 10, 0, 0, DateTimeKind.Utc)
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
