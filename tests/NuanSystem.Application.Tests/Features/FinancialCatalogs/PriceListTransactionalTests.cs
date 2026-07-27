using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Commands;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.FinancialCatalogs;

public sealed class PriceListTransactionalTests
{
    private readonly IPriceListRepository _repository = Substitute.For<IPriceListRepository>();
    private readonly IPriceListLocalOutboxWriter _writer = Substitute.For<IPriceListLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transaction = new();

    [Fact]
    public async Task Create_WritesEntityAndOutboxInSameTransaction()
    {
        var priceList = Item();
        ConfigureValidDependencies();
        _repository.CreateAsync(Arg.Any<CreatePriceListData>(), _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(priceList.Id);
        _repository.GetByIdAsync(priceList.Id, _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(priceList);
        var handler = new CreatePriceListCommandHandler(_repository, _transaction, _writer);

        var result = await handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _transaction.Committed.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            priceList, SyncOperation.Created, _transaction.Connection, _transaction.Transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenOutboxFails()
    {
        var priceList = Item();
        ConfigureValidDependencies();
        _repository.CreateAsync(Arg.Any<CreatePriceListData>(), _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(priceList.Id);
        _repository.GetByIdAsync(priceList.Id, _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(priceList);
        _writer.EnqueueAsync(Arg.Any<PriceListDto>(), Arg.Any<SyncOperation>(), Arg.Any<IDbConnection>(),
                Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("controlled"));
        var handler = new CreatePriceListCommandHandler(_repository, _transaction, _writer);

        var action = () => handler.Handle(Command(), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("controlled");
        _transaction.RolledBack.Should().BeTrue();
        _transaction.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Create_DefaultBoth_RejectsOverlappingDefault()
    {
        ConfigureValidDependencies();
        _repository.HasDefaultConflictAsync("Both", null, _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new CreatePriceListCommandHandler(_repository, _transaction, _writer);

        var result = await handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "PRICE_LIST_DEFAULT_CONFLICT");
        await _writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public async Task Delete_WithActiveReferences_IsBlockedWithoutOutbox()
    {
        var priceList = Item();
        _repository.GetByIdAsync(priceList.Id, _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(priceList);
        _repository.HasActiveReferencesAsync(priceList.Id, priceList.Code, _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new DeletePriceListCommandHandler(_repository, _transaction, _writer);

        var result = await handler.Handle(new DeletePriceListCommand(priceList.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "PRICE_LIST_ACTIVE_REFERENCES");
        await _writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public void SqlContracts_ReserveTombstonesAndKeepEntityDisabled()
    {
        var tenant = ReadSource("database", "sql", "140_tenant_price_list_transactional_outbox.sql");
        var master = ReadSource("database", "sql", "141_master_price_list_transactional_registration.sql");

        tenant.Should().Contain("CREATE UNIQUE INDEX UX_PriceLists_Code ON dbo.PriceLists(Code)")
            .And.Contain("SP_NA_POST_PRICELIST_SYNC_APPLY_EVENT")
            .And.Contain("@CurrencyGlobalId")
            .And.Contain("GlobalId <> @GlobalId")
            .And.Contain("Status = N'DeadLetter'");
        master.Should().Contain("N'PriceList'")
            .And.Contain("N'Currencies'")
            .And.Contain("N'Item'")
            .And.Contain("CONVERT(bit, 0)")
            .And.Contain("20260727.141");
    }

    private void ConfigureValidDependencies()
    {
        _repository.ExistsByCodeAsync("PL-01", null, _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.GetCurrencyAsync("USD", _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(new PriceListCurrencyDto("USD", "Dólar", Guid.NewGuid()));
        _repository.HasDefaultConflictAsync("Both", null, _transaction.Connection, _transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(false);
    }

    private static CreatePriceListCommand Command() =>
        new("pl-01", "Lista principal", null, "usd", "Both", true, true, 7, "admin");

    private static PriceListDto Item() =>
        new()
        {
            Id = 10,
            GlobalId = Guid.NewGuid(),
            Code = "PL-01",
            Name = "Lista principal",
            CurrencyCode = "USD",
            CurrencyName = "Dólar",
            CurrencyGlobalId = Guid.NewGuid(),
            AppliesTo = "Both",
            IsDefault = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

    private static string ReadSource(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. parts]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(parts));
    }

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public Task ExecuteInTenantTransactionAsync(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default) =>
            ExecuteInTenantTransactionAsync<object?>(async (connection, transaction, token) =>
            {
                await operation(connection, transaction, token);
                return null;
            }, cancellationToken);

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
