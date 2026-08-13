using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Queries;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemTypes;

public sealed class ItemTypeQueryHandlerTests
{
    private readonly IItemTypeRepository _repository = Substitute.For<IItemTypeRepository>();

    [Fact]
    public async Task Lookup_ReturnsRepositoryContract()
    {
        ItemTypeLookupDto[] values = [new() { Id = 1, Code = "PRODUCT", Name = "Producto", BehaviorCode = "Product" }];
        _repository.GetLookupAsync(Arg.Any<CancellationToken>()).Returns(values);

        var result = await new GetItemTypeLookupQueryHandler(_repository).Handle(new GetItemTypeLookupQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(values);
    }

    [Fact]
    public async Task Detail_ReturnsStableNotFoundError()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((ItemTypeDto?)null);

        var result = await new GetItemTypeByIdQueryHandler(_repository).Handle(new GetItemTypeByIdQuery(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "ITEM_TYPE_NOT_FOUND");
    }

    [Fact]
    public async Task History_ForwardsItemTypeIdToRepository()
    {
        ItemTypeAuditChangeDto[] changes = [new() { RecordId = "12", Action = "UPDATE", FieldName = "Name" }];
        _repository.GetHistoryAsync(12, Arg.Any<CancellationToken>()).Returns(changes);

        var result = await new GetItemTypeHistoryQueryHandler(_repository).Handle(
            new GetItemTypeHistoryQuery(12),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(changes);
        await _repository.Received(1).GetHistoryAsync(12, Arg.Any<CancellationToken>());
    }
}
