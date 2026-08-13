using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemTypes;

public sealed class ItemTypeCommandHandlerTests
{
    private readonly IItemTypeRepository _repository = Substitute.For<IItemTypeRepository>();

    [Fact]
    public async Task Create_NormalizesValuesAndGeneratesGlobalIdentity()
    {
        _repository.CreateAsync(Arg.Any<CreateItemTypeData>(), Arg.Any<CancellationToken>())
            .Returns(new CreateItemTypeResult(7, DuplicateCode: false));
        _repository.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(ItemType(7));

        var result = await new CreateItemTypeCommandHandler(_repository).Handle(
            new CreateItemTypeCommand(" product ", " Producto ", " Comercial ", "product", true, true, true, 10, true, 3, " admin "),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).CreateAsync(
            Arg.Is<CreateItemTypeData>(data =>
                data.GlobalId != Guid.Empty &&
                data.Code == "PRODUCT" &&
                data.Name == "Producto" &&
                data.Description == "Comercial" &&
                data.BehaviorCode == "Product" &&
                data.CreatedByUserName == "admin"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ReturnsStableError_WhenDatabaseDetectsConcurrentDuplicate()
    {
        _repository.CreateAsync(Arg.Any<CreateItemTypeData>(), Arg.Any<CancellationToken>())
            .Returns(new CreateItemTypeResult(null, DuplicateCode: true));

        var result = await new CreateItemTypeCommandHandler(_repository).Handle(
            new CreateItemTypeCommand("PRODUCT", "Producto", null, "Product", true, true, true, 10, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "ITEM_TYPE_DUPLICATED_CODE" && error.Field == "Code");
    }

    [Fact]
    public async Task Update_RejectsChangingBehaviorOfSystemType()
    {
        _repository.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(ItemType(7));

        var result = await new UpdateItemTypeCommandHandler(_repository).Handle(
            new UpdateItemTypeCommand(7, "PRODUCT", "Producto", null, "Supply", true, true, true, 10, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "ITEM_TYPE_SYSTEM_PROTECTED");
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<UpdateItemTypeData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_ReturnsInUse_WhenDatabaseFindsAssociatedItems()
    {
        _repository.GetByIdAsync(8, Arg.Any<CancellationToken>()).Returns(ItemType(8, isSystem: false));
        _repository.DeleteAsync(Arg.Any<DeleteItemTypeData>(), Arg.Any<CancellationToken>())
            .Returns(new DeleteItemTypeResult(Deleted: false, SystemProtected: false, InUse: true));

        var result = await new DeleteItemTypeCommandHandler(_repository).Handle(
            new DeleteItemTypeCommand(8, 3, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "ITEM_TYPE_IN_USE");
    }

    private static ItemTypeDto ItemType(int id, bool isSystem = true) => new()
    {
        Id = id,
        GlobalId = Guid.NewGuid(),
        Code = "PRODUCT",
        Name = "Producto",
        BehaviorCode = "Product",
        DefaultIsPurchaseItem = true,
        DefaultIsSalesItem = true,
        DefaultIsInventoryItem = true,
        SortOrder = 10,
        IsSystem = isSystem,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
}
