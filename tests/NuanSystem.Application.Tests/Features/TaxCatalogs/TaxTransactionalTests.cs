using System.Data;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Commands;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.TaxCatalogs;

public sealed class TaxTransactionalTests
{
    private readonly ITaxRepository repository = Substitute.For<ITaxRepository>();
    private readonly ITaxLocalOutboxWriter writer = Substitute.For<ITaxLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner transaction = new();

    [Fact]
    public async Task Create_WritesTaxAndOutboxInSameTransaction()
    {
        var tax = Item();
        repository.CreateAsync(Arg.Any<CreateTaxData>(), transaction.Connection, transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(tax.Id);
        repository.GetByIdAsync(tax.Id, transaction.Connection, transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(tax);
        var handler = new CreateTaxCommandHandler(repository, transaction, writer);

        var result = await handler.Handle(new("iva15", "IVA 15%", null, .15m, true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        transaction.Committed.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(tax, SyncOperation.Created,
            transaction.Connection, transaction.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenOutboxFails()
    {
        var tax = Item();
        repository.CreateAsync(Arg.Any<CreateTaxData>(), transaction.Connection, transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(tax.Id);
        repository.GetByIdAsync(tax.Id, transaction.Connection, transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(tax);
        writer.EnqueueAsync(Arg.Any<TaxDto>(), Arg.Any<SyncOperation>(), Arg.Any<IDbConnection>(),
                Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("controlled"));
        var handler = new CreateTaxCommandHandler(repository, transaction, writer);

        var action = () => handler.Handle(new("IVA15", "IVA 15%", null, .15m, true), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("controlled");
        transaction.RolledBack.Should().BeTrue();
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(1.01)]
    public async Task Create_RejectsRateOutsideDecimalContract(double value)
    {
        var handler = new CreateTaxCommandHandler(repository, transaction, writer);
        var result = await handler.Handle(new("IVA", "IVA", null, Convert.ToDecimal(value), true), CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "TAX_RATE_OUT_OF_RANGE");
    }

    [Fact]
    public async Task Delete_WithActiveItems_IsBlockedWithoutOutbox()
    {
        var tax = Item();
        repository.GetByIdAsync(tax.Id, transaction.Connection, transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(tax);
        repository.HasActiveItemReferencesAsync(tax.Id, transaction.Connection, transaction.Transaction, Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new DeleteTaxCommandHandler(repository, transaction, writer);

        var result = await handler.Handle(new(tax.Id), CancellationToken.None);

        result.Errors.Should().Contain(error => error.Code == "TAX_ACTIVE_ITEM_REFERENCES");
        await writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public async Task CodeCollision_IsTerminalWithoutAdoption()
    {
        var applyRepository = Substitute.For<ITaxSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload);
        applyRepository.ApplyAsync(2, context, Arg.Any<TaxSyncPayloadV1>(), SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new TaxSyncApplyResult(false, false, true, null, "Conflicto.", "SYNC_TAX_CODE_CONFLICT"));
        var result = await new TaxSyncEventApplier(applyRepository).ApplyAsync(context);
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_TAX_CODE_CONFLICT");
    }

    [Fact]
    public void Contracts_ReserveTombstonesAndKeepWorkersDisabled()
    {
        var tenant = ReadSource("database", "sql", "144_tenant_tax_transactional_outbox.sql");
        var master = ReadSource("database", "sql", "145_master_tax_transactional_registration.sql");
        var designer = ReadSource("src", "Frontend", "NuanSystem.WinForms.Forms", "TaxCatalogs", "Taxes", "TaxEditForm.Designer.cs");

        tenant.Should().Contain("CREATE UNIQUE INDEX UQ_Taxes_Code ON dbo.Taxes(Code)")
            .And.Contain("Rate < 0 OR Rate > 1")
            .And.Contain("GlobalId<>@GlobalId")
            .And.Contain("Status=N'DeadLetter'")
            .And.Contain("SP_NA_GET_TAXES_HISTORIAL")
            .And.Contain("dbo.AuditCatalogChanges")
            .And.Contain("N'MasterBranchSyncWorker'");
        master.Should().Contain("N'Tax'").And.Contain("CONVERT(bit,0)")
            .And.NotContain("RolePermissions").And.NotContain("SecurityRoleMenus");
        designer.Should().Contain("Controls.SetChildIndex(btnGuardar")
            .And.NotContain("SimpleButton btnSave").And.NotContain("SimpleButton btnCancel");
    }

    private static TaxDto Item() => new()
    {
        Id = 15, GlobalId = Guid.NewGuid(), Code = "IVA15", Name = "IVA 15%",
        Rate = .15m, IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static TaxSyncPayloadV1 Payload() =>
        new(Guid.NewGuid(), "IVA15", "IVA 15%", null, .15m, true, null, null, DateTime.UtcNow, null);

    private static SyncEventApplyContext Context(TaxSyncPayloadV1 payload)
    {
        var wrapper = new { entityName = "Tax", globalId = payload.GlobalId, code = payload.Code, operation = "Created", payload };
        return new(Guid.NewGuid(), 1, "Tax", payload.GlobalId, "Created",
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)), 2, 10);
    }

    private static string ReadSource(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. parts]);
            if (File.Exists(path)) return File.ReadAllText(path);
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
        public Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default) =>
            ExecuteInTenantTransactionAsync<object?>(async (connection, tx, token) => { await operation(connection, tx, token); return null; }, cancellationToken);
        public async Task<T> ExecuteInTenantTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            try { var result = await operation(Connection, Transaction, cancellationToken); Committed = true; return result; }
            catch { RolledBack = true; throw; }
        }
    }
}
