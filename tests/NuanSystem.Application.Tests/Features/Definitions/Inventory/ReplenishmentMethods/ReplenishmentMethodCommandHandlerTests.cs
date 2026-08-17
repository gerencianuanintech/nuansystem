using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ReplenishmentMethods;

public sealed class ReplenishmentMethodCommandHandlerTests
{
    private readonly IReplenishmentMethodRepository _repository = Substitute.For<IReplenishmentMethodRepository>();
    private readonly IReplenishmentMethodLocalOutboxWriter _writer = Substitute.For<IReplenishmentMethodLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task Create_PreservesCodeCasingAndWritesCreatedInsideTransaction()
    {
        var saved = Method(7, "Comprar", true);
        _repository.CreateAsync(Arg.Any<CreateReplenishmentMethodData>(), _transactionRunner.Connection,
            _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(7);
        _repository.GetByIdAsync(7, _transactionRunner.Connection, _transactionRunner.Transaction,
            Arg.Any<CancellationToken>()).Returns(saved);
        var handler = new CreateReplenishmentMethodCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(new CreateReplenishmentMethodCommand(
            " Comprar ", " Comprar ", " Compra regular ", 0, true, 10, " admin "), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).CreateAsync(
            Arg.Is<CreateReplenishmentMethodData>(x => x.Code == "Comprar" && x.Name == "Comprar" && x.SortOrder == 0),
            _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
        await _writer.Received(1).EnqueueAsync(saved, SyncOperation.Created,
            _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
        _transactionRunner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WritesDisabledWhenResultIsInactive()
    {
        var current = Method(7, "COMPRAR", true);
        var saved = Method(7, "COMPRAR", false);
        _repository.GetByIdAsync(7, _transactionRunner.Connection, _transactionRunner.Transaction,
            Arg.Any<CancellationToken>()).Returns(current, saved);
        _repository.UpdateAsync(Arg.Any<UpdateReplenishmentMethodData>(), _transactionRunner.Connection,
            _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(1);
        var handler = new UpdateReplenishmentMethodCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(new UpdateReplenishmentMethodCommand(
            7, "COMPRAR", "Comprar", null, 0, false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(saved, SyncOperation.Disabled,
            _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_WritesDeletedOnlyAfterSuccessfulMutation()
    {
        var current = Method(7, "COMPRAR", true);
        _repository.GetByIdAsync(7, _transactionRunner.Connection, _transactionRunner.Transaction,
            Arg.Any<CancellationToken>()).Returns(current);
        _repository.DeleteAsync(7, 10, "admin", _transactionRunner.Connection,
            _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(1);
        var handler = new DeleteReplenishmentMethodCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(new DeleteReplenishmentMethodCommand(7, 10, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(current, SyncOperation.Deleted,
            _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    private static ReplenishmentMethodDto Method(int id, string code, bool isActive) => new()
    {
        Id = id,
        GlobalId = Guid.NewGuid(),
        Code = code,
        Name = code,
        IsActive = isActive,
        CreatedAt = DateTime.UtcNow
    };

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }

        public async Task ExecuteInTenantTransactionAsync(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default) =>
            await operation(Connection, Transaction, cancellationToken);

        public async Task<T> ExecuteInTenantTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            var result = await operation(Connection, Transaction, cancellationToken);
            Committed = true;
            return result;
        }
    }
}
