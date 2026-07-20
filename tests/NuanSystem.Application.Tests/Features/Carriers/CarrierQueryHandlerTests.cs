using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Carriers.Queries;

namespace NuanSystem.Application.Tests.Features.Carriers;

public sealed class CarrierQueryHandlerTests
{
    private readonly ICarrierRepository _repository = Substitute.For<ICarrierRepository>();

    [Fact]
    public async Task List_ReturnsRepositoryItems()
    {
        CarrierListItemDto[] items = [new() { Id = 1, Code = "TR-001", Name = "Transportes Uno" }];
        _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(items);

        var result = await new GetCarriersQueryHandler(_repository).Handle(new GetCarriersQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task Lookup_ReturnsRepositoryItems()
    {
        CarrierLookupDto[] items = [new() { Id = 1, Code = "TR-001", Name = "Transportes Uno", IsActive = true }];
        _repository.GetLookupAsync(Arg.Any<CancellationToken>()).Returns(items);

        var result = await new GetCarrierLookupQueryHandler(_repository).Handle(new GetCarrierLookupQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task Detail_ReturnsStableNotFoundError_WhenCarrierDoesNotExist()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((CarrierDetailDto?)null);

        var result = await new GetCarrierByIdQueryHandler(_repository).Handle(new GetCarrierByIdQuery(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "CARRIER_NOT_FOUND" && error.Field == "Id");
    }

    [Fact]
    public async Task History_ForwardsCarrierIdToRepository()
    {
        CarrierAuditChangeDto[] changes = [new() { RecordId = "12", Action = "UPDATE", FieldName = "Name" }];
        _repository.GetHistoryAsync(12, Arg.Any<CancellationToken>()).Returns(changes);

        var result = await new GetCarrierHistoryQueryHandler(_repository).Handle(new GetCarrierHistoryQuery(12), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(changes);
        await _repository.Received(1).GetHistoryAsync(12, Arg.Any<CancellationToken>());
    }
}
