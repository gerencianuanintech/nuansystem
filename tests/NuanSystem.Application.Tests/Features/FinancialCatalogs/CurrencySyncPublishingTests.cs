using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.FinancialCatalogs;

public sealed class CurrencySyncPublishingTests
{
    private readonly IFinancialCatalogRepository _repository = Substitute.For<IFinancialCatalogRepository>();
    private readonly ICurrencyLocalOutboxWriter _writer = Substitute.For<ICurrencyLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task CreateCurrency_WritesLocalOutboxInsideTheSameTransaction()
    {
        var currency = CreateCurrency();
        _repository.ExistsByCodeAsync(
                "currencies", "USD", null, _transactionRunner.Connection, _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.CreateAsync(
                "currencies", Arg.Any<CreateFinancialCatalogData>(), _transactionRunner.Connection,
                _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(currency.Id);
        _repository.GetByIdAsync(
                "currencies", currency.Id, _transactionRunner.Connection, _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(currency);
        var handler = new CreateFinancialCatalogCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(
            new CreateFinancialCatalogCommand("currencies", "usd", "Dolar", "Moneda base", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            currency,
            SyncOperation.Created,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
        _transactionRunner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateInactiveCurrency_WritesDisabledOperation()
    {
        var current = CreateCurrency();
        var inactive = CreateCurrency(isActive: false, globalId: current.GlobalId);
        _repository.GetByIdAsync(
                "currencies", current.Id, _transactionRunner.Connection, _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(current, inactive);
        _repository.ExistsByCodeAsync(
                "currencies", "USD", current.Id, _transactionRunner.Connection, _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.UpdateAsync(
                "currencies", Arg.Any<UpdateFinancialCatalogData>(), _transactionRunner.Connection,
                _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new UpdateFinancialCatalogCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(
            new UpdateFinancialCatalogCommand(
                "currencies", current.Id, "USD", "Dolar", "Moneda base", false, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            inactive,
            SyncOperation.Disabled,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCurrency_WritesDeletedOperation()
    {
        var currency = CreateCurrency();
        _repository.GetByIdAsync(
                "currencies", currency.Id, _transactionRunner.Connection, _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(currency);
        _repository.DeleteAsync(
                "currencies", currency.Id, 7, "admin", _transactionRunner.Connection,
                _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new DeleteFinancialCatalogCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(
            new DeleteFinancialCatalogCommand("currencies", currency.Id, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            currency,
            SyncOperation.Deleted,
            _transactionRunner.Connection,
            _transactionRunner.Transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCurrency_RollsBackWhenLocalOutboxFails()
    {
        var currency = CreateCurrency();
        _repository.ExistsByCodeAsync(
                "currencies", "USD", null, _transactionRunner.Connection, _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.CreateAsync(
                "currencies", Arg.Any<CreateFinancialCatalogData>(), _transactionRunner.Connection,
                _transactionRunner.Transaction, Arg.Any<CancellationToken>())
            .Returns(currency.Id);
        _repository.GetByIdAsync(
                "currencies", currency.Id, _transactionRunner.Connection, _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(currency);
        _writer.EnqueueAsync(
                Arg.Any<FinancialCatalogDto>(),
                Arg.Any<SyncOperation>(),
                Arg.Any<IDbConnection>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("Controlled outbox failure"));
        var handler = new CreateFinancialCatalogCommandHandler(_repository, _transactionRunner, _writer);

        var action = () => handler.Handle(
            new CreateFinancialCatalogCommand("currencies", "usd", "Dolar", null, true, 7, "admin"),
            CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Controlled outbox failure");
        _transactionRunner.RolledBack.Should().BeTrue();
        _transactionRunner.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Writer_PreservesExternalReferenceWithoutSapCode()
    {
        var currency = CreateCurrency();
        currency.ExternalSystem = "SAPB1";
        currency.ExternalCode = "USD";
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
        var writer = new CurrencyLocalOutboxWriter(
            companyContext,
            new SyncEventPayloadFactory(),
            localOutbox);

        var eventId = await writer.EnqueueAsync(
            currency,
            SyncOperation.Created,
            _transactionRunner.Connection,
            _transactionRunner.Transaction);

        eventId.Should().NotBeNull().And.NotBe(Guid.Empty);
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("Currencies");
        captured.EntityGlobalId.Should().Be(currency.GlobalId!.Value);
        captured.PayloadJson.Should()
            .Contain("\"externalSystem\":\"SAPB1\"")
            .And.Contain("\"externalCode\":\"USD\"")
            .And.NotContain("sapCode");
    }

    [Fact]
    public async Task CreateOtherFinancialCatalog_DoesNotUseCurrencyTransactionOrOutbox()
    {
        var bank = new FinancialCatalogDto
        {
            Id = 1,
            Code = "BANK",
            Name = "Banco",
            IsActive = true,
            CreatedAt = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc)
        };
        _repository.ExistsByCodeAsync("banks", "BANK", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync("banks", Arg.Any<CreateFinancialCatalogData>(), Arg.Any<CancellationToken>())
            .Returns(bank.Id);
        _repository.GetByIdAsync("banks", bank.Id, Arg.Any<CancellationToken>()).Returns(bank);
        var handler = new CreateFinancialCatalogCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(
            new CreateFinancialCatalogCommand("banks", "bank", "Banco", null, true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _transactionRunner.Executed.Should().BeFalse();
        await _writer.DidNotReceiveWithAnyArgs()
            .EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public void TenantMigration_ReservesTombstoneAndRejectsCodeAdoption()
    {
        var migration = ReadSource(
            "database", "sql", "136_tenant_currency_transactional_outbox.sql");

        migration.Should().Contain("CREATE UNIQUE INDEX UX_Currencies_Code ON dbo.Currencies(Code)")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CURRENCY_SYNC_APPLY")
            .And.Contain("WHERE Code = @Code")
            .And.Contain("GlobalId <> @GlobalId")
            .And.Contain("SELECT -2 AS ResultCode")
            .And.Contain("@ExternalSystem")
            .And.Contain("@ExternalCode")
            .And.NotContain("@SapCode");
    }

    [Fact]
    public void MasterMigration_KeepsCurrencyDisabledAndPriceListDependency()
    {
        var migration = ReadSource(
            "database", "sql", "137_master_currency_transactional_registration.sql");

        migration.Should().Contain("N'Currencies'")
            .And.Contain("N'PriceList'")
            .And.Contain("DependsOnEntityDefinitionId = @CurrencyDefinitionId")
            .And.Contain("company.Id, N'Currencies', CONVERT(bit, 0)")
            .And.Contain("Version = N'20260727.137'");
    }

    private static FinancialCatalogDto CreateCurrency(bool isActive = true, Guid? globalId = null) =>
        new()
        {
            Id = 1,
            GlobalId = globalId ?? Guid.NewGuid(),
            Code = "USD",
            Name = "Dolar",
            Symbol = "$",
            Description = "Moneda base",
            IsBaseCurrency = true,
            IsActive = isActive,
            CreatedAt = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = isActive ? null : new DateTime(2026, 7, 16, 11, 0, 0, DateTimeKind.Utc)
        };

    private static CompanyConnectionInfo Company() =>
        new(
            10,
            "MASTER",
            "Empresa Master",
            DatabaseEngine.SqlServer,
            "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode.None,
            CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: true);

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(pathParts));
    }

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Executed { get; private set; }
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
            Executed = true;
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
