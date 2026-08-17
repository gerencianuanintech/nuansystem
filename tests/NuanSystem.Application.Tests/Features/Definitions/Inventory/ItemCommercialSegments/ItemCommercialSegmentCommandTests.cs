using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Dtos;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemCommercialSegments;

public sealed class ItemCommercialSegmentCommandTests
{
    private readonly IItemCommercialSegmentRepository repository = Substitute.For<IItemCommercialSegmentRepository>();
    private readonly IDbConnection connection = Substitute.For<IDbConnection>();
    private readonly IDbTransaction transaction = Substitute.For<IDbTransaction>();

    [Fact]
    public void CreateValidator_AllowsZeroSortOrderAndInactiveState()
    {
        var result = new CreateItemCommercialSegmentCommandValidator().Validate(
            new CreateItemCommercialSegmentCommand("RETAIL", "Retail", null, 0, false));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateValidator_RejectsBlankCodeAndNegativeSortOrder()
    {
        var result = new CreateItemCommercialSegmentCommandValidator().Validate(
            new CreateItemCommercialSegmentCommand(" ", "Retail", null, -1, true));

        result.Errors.Should().Contain(error => error.PropertyName == "Code");
        result.Errors.Should().Contain(error => error.PropertyName == "SortOrder");
    }

    [Fact]
    public async Task Create_NormalizesValuesBeforePersistence()
    {
        repository.ExistsByCodeAsync("Retail", null, connection, transaction, Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateAsync(Arg.Any<CreateItemCommercialSegmentData>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(7);
        repository.GetByIdAsync(7, connection, transaction, Arg.Any<CancellationToken>()).Returns(Segment(7));
        var handler = new CreateItemCommercialSegmentCommandHandler(repository, Runner());

        var result = await handler.Handle(
            new CreateItemCommercialSegmentCommand(" Retail ", " Venta minorista ", " Tiendas ", 0, false, 3, " admin "),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await repository.Received(1).CreateAsync(
            Arg.Is<CreateItemCommercialSegmentData>(data =>
                data.Code == "Retail" &&
                data.Name == "Venta minorista" &&
                data.Description == "Tiendas" &&
                data.SortOrder == 0 &&
                !data.IsActive &&
                data.CreatedByUserName == "admin" &&
                data.GlobalId != Guid.Empty),
            connection,
            transaction,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ReturnsStableErrorWhenCodeAlreadyExists()
    {
        repository.ExistsByCodeAsync("Retail", null, connection, transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new CreateItemCommercialSegmentCommandHandler(repository, Runner());

        var result = await handler.Handle(
            new CreateItemCommercialSegmentCommand("Retail", "Venta minorista", null, 0, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "ItemCommercialSegmentCodeAlreadyExists" && error.Field == "Code");
        await repository.DidNotReceive().CreateAsync(Arg.Any<CreateItemCommercialSegmentData>(), connection, transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_ReturnsStableNotFoundError()
    {
        repository.GetByIdAsync(99, connection, transaction, Arg.Any<CancellationToken>()).Returns((ItemCommercialSegmentDto?)null);
        var handler = new DeleteItemCommercialSegmentCommandHandler(repository, Runner());

        var result = await handler.Handle(new DeleteItemCommercialSegmentCommand(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "ItemCommercialSegmentNotFound" && error.Field == "Id");
        await repository.DidNotReceive().DeleteAsync(Arg.Any<int>(), Arg.Any<int?>(), Arg.Any<string?>(), connection, transaction, Arg.Any<CancellationToken>());
    }

    private ITransactionRunner Runner() => new ImmediateTransactionRunner(connection, transaction);

    private static ItemCommercialSegmentDto Segment(int id) => new()
    {
        Id = id,
        GlobalId = Guid.NewGuid(),
        Code = "Retail",
        Name = "Venta minorista",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    private sealed class ImmediateTransactionRunner(IDbConnection connection, IDbTransaction transaction) : ITransactionRunner
    {
        public Task ExecuteInTenantTransactionAsync(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default) => operation(connection, transaction, cancellationToken);

        public Task<T> ExecuteInTenantTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default) => operation(connection, transaction, cancellationToken);
    }
}
