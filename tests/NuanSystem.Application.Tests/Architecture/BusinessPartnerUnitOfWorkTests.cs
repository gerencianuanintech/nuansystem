using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Transactions;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class BusinessPartnerUnitOfWorkTests
{
    [Fact]
    public async Task Repository_and_outbox_use_the_exact_same_connection_and_transaction()
    {
        var f = new Fixture();
        var partner = new BusinessPartnerDto { Id = 7, GlobalId = Guid.NewGuid() };
        await f.Unit.ExecuteAsync(async (context, token) =>
        {
            await context.ExistsByCodeAsync("CODE", null, token);
            await context.ExistsByIdentificationAsync(1, "TAX", null, token);
            await context.GetByIdAsync(7, token);
            await context.DeleteAsync(7, 42, "auditor", token);
            await context.EnqueueAsync(partner, SyncOperation.Deleted, token);
            return 7;
        });
        await f.Repository.Received(1).ExistsByCodeAsync("CODE", null, f.Connection, f.Transaction, Arg.Any<CancellationToken>());
        await f.Repository.Received(1).ExistsByIdentificationAsync(1, "TAX", null, f.Connection, f.Transaction, Arg.Any<CancellationToken>());
        await f.Repository.Received(1).GetByIdAsync(7, f.Connection, f.Transaction, Arg.Any<CancellationToken>());
        await f.Repository.Received(1).DeleteAsync(7, 42, "auditor", f.Connection, f.Transaction, Arg.Any<CancellationToken>());
        await f.Writer.Received(1).EnqueueAsync(partner, SyncOperation.Deleted, f.Connection, f.Transaction, Arg.Any<CancellationToken>());
        f.Transaction.Received(1).Commit();
        f.Transaction.DidNotReceive().Rollback();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Repository_or_outbox_failure_rolls_back_and_preserves_error(bool outbox)
    {
        var f = new Fixture();
        var original = new InvalidOperationException("Controlled persistence failure");
        if (outbox)
            f.Writer.EnqueueAsync(Arg.Any<BusinessPartnerDto>(), Arg.Any<SyncOperation>(), f.Connection, f.Transaction, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Guid?>(original));
        else
            f.Repository.DeleteAsync(7, 42, "auditor", f.Connection, f.Transaction, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<bool>(original));
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => f.Unit.ExecuteAsync(async (context, token) =>
        {
            await context.DeleteAsync(7, 42, "auditor", token);
            await context.EnqueueAsync(new BusinessPartnerDto { Id = 7, GlobalId = Guid.NewGuid() }, SyncOperation.Deleted, token);
            return 7;
        }));
        Assert.Same(original, actual);
        f.Transaction.Received(1).Rollback();
        f.Transaction.DidNotReceive().Commit();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Escaped_context_cannot_be_reused_after_success_or_failure(bool fail)
    {
        var f = new Fixture();
        IBusinessPartnerTransaction? escaped = null;
        try
        {
            await f.Unit.ExecuteAsync<int>((context, _) =>
            {
                escaped = context;
                return fail ? Task.FromException<int>(new InvalidOperationException("Fail")) : Task.FromResult(1);
            });
        }
        catch (InvalidOperationException) when (fail) { }
        Assert.NotNull(escaped);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => escaped.GetByIdAsync(7));
        Assert.Empty(f.Repository.ReceivedCalls());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Company_change_is_rejected_before_repository_use_or_commit(bool useRepository)
    {
        var f = new Fixture();
        await Assert.ThrowsAsync<InvalidOperationException>(() => f.Unit.ExecuteAsync(async (context, _) =>
        {
            f.CompanyContext.CurrentCompany.Returns(Company(2));
            if (useRepository) await context.GetByIdAsync(7);
            return 1;
        }));
        Assert.Empty(f.Repository.ReceivedCalls());
        f.Transaction.Received(1).Rollback();
        f.Transaction.DidNotReceive().Commit();
    }

    [Fact]
    public async Task Missing_company_and_pre_cancellation_do_not_open_a_transaction()
    {
        var f = new Fixture();
        f.CompanyContext.HasActiveCompany.Returns(false);
        await Assert.ThrowsAsync<InvalidOperationException>(() => f.Unit.ExecuteAsync((_, _) => Task.FromResult(1)));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Unit.ExecuteAsync((_, _) => Task.FromResult(1), cancellation.Token));
        f.Connection.DidNotReceive().Open();
        f.Connection.DidNotReceive().BeginTransaction();
    }

    [Fact]
    public async Task Nested_operation_is_rejected_without_a_second_transaction()
    {
        var f = new Fixture();
        await Assert.ThrowsAsync<InvalidOperationException>(() => f.Unit.ExecuteAsync(
            (_, _) => f.Unit.ExecuteAsync((_, _) => Task.FromResult(1))));
        f.Connection.Received(1).BeginTransaction();
        f.Transaction.Received(1).Rollback();
    }

    [Fact]
    public async Task Operation_cancellation_flows_to_calls_even_when_the_call_omits_its_token()
    {
        var f = new Fixture();
        using var cancellation = new CancellationTokenSource();
        f.Repository.GetByIdAsync(7, f.Connection, f.Transaction, cancellation.Token).Returns((BusinessPartnerDto?)null);
        await f.Unit.ExecuteAsync(async (context, _) => { await context.GetByIdAsync(7); return 1; }, cancellation.Token);
        await f.Repository.Received(1).GetByIdAsync(7, f.Connection, f.Transaction, cancellation.Token);
    }

    [Fact]
    public async Task Concurrent_operation_is_rejected_and_unit_can_be_used_after_completion()
    {
        var f = new Fixture();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = f.Unit.ExecuteAsync(async (_, _) => { entered.SetResult(); await release.Task; return 1; });
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        try
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => f.Unit.ExecuteAsync((_, _) => Task.FromResult(2)));
        }
        finally { release.SetResult(); }
        Assert.Equal(1, await first);
        Assert.Equal(3, await f.Unit.ExecuteAsync((_, _) => Task.FromResult(3)));
        f.Connection.Received(2).BeginTransaction();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task In_flight_operation_observes_root_cancellation_with_default_or_separate_call_token(bool separateToken)
    {
        var f = new Fixture();
        using var root = new CancellationTokenSource();
        using var separate = new CancellationTokenSource();
        CancellationToken observed = default;
        f.Repository.GetByIdAsync(7, f.Connection, f.Transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            observed = call.Arg<CancellationToken>();
            root.Cancel();
            return Task.FromCanceled<BusinessPartnerDto?>(observed);
        });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Unit.ExecuteAsync(async (context, _) =>
        {
            await context.GetByIdAsync(7, separateToken ? separate.Token : default);
            return 1;
        }, root.Token));
        Assert.True(observed.IsCancellationRequested);
        f.Transaction.Received(1).Rollback();
        f.Transaction.DidNotReceive().Commit();
    }

    private static CompanyConnectionInfo Company(int id = 1) => new(id, $"TEST-{id}", "Test", DatabaseEngine.SqlServer,
        string.Empty, SapIntegrationMode.None);

    private sealed class Fixture
    {
        public System.Data.IDbConnection Connection { get; }
        public System.Data.IDbTransaction Transaction { get; }
        public IBusinessPartnerRepository Repository { get; } = Substitute.For<IBusinessPartnerRepository>();
        public IBusinessPartnerLocalOutboxWriter Writer { get; } = Substitute.For<IBusinessPartnerLocalOutboxWriter>();
        public ICompanyContext CompanyContext { get; } = Substitute.For<ICompanyContext>();
        public BusinessPartnerUnitOfWork Unit { get; }
        public Fixture()
        {
            var (runner, connection, transaction, _) = SqlTransactionRunnerTests.Create();
            Connection = connection;
            Transaction = transaction;
            CompanyContext.HasActiveCompany.Returns(true);
            CompanyContext.CurrentCompany.Returns(Company());
            Unit = new BusinessPartnerUnitOfWork(runner, Repository, Writer, CompanyContext);
        }
    }
}
