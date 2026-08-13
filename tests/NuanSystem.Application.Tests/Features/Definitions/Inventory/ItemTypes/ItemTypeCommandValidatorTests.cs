using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Commands;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemTypes;

public sealed class ItemTypeCommandValidatorTests
{
    [Theory]
    [InlineData("Product")]
    [InlineData("Service")]
    [InlineData("Supply")]
    [InlineData("Asset")]
    [InlineData("Kit")]
    public void CreateValidator_AcceptsSupportedBehaviorCodes(string behaviorCode)
    {
        var command = ValidCreate() with
        {
            BehaviorCode = behaviorCode,
            DefaultIsInventoryItem = behaviorCode != "Service"
        };

        new CreateItemTypeCommandValidator().Validate(command).IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateValidator_RejectsUnknownBehaviorCode()
    {
        var result = new CreateItemTypeCommandValidator().Validate(
            ValidCreate() with { BehaviorCode = "Unknown" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorCode == "ITEM_TYPE_INVALID_BEHAVIOR");
    }

    [Fact]
    public void CreateValidator_RejectsInventoryDefaultForService()
    {
        var result = new CreateItemTypeCommandValidator().Validate(
            ValidCreate() with { BehaviorCode = "Service", DefaultIsInventoryItem = true });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorCode == "ITEM_TYPE_SERVICE_INVENTORY_DEFAULT_INVALID");
    }

    [Fact]
    public void UpdateValidator_RequiresPositiveIdentifierAndSortOrder()
    {
        var result = new UpdateItemTypeCommandValidator().Validate(
            new UpdateItemTypeCommand(0, "PRODUCT", "Producto", null, "Product", true, true, true, -1, true));

        result.Errors.Should().Contain(error => error.ErrorCode == "ITEM_TYPE_ID_INVALID");
        result.Errors.Should().Contain(error => error.ErrorCode == "ITEM_TYPE_SORT_ORDER_INVALID");
    }

    private static CreateItemTypeCommand ValidCreate() =>
        new("PRODUCT", "Producto", "Articulo comercial", "Product", true, true, true, 10, true);
}
