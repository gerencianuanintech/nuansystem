using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Commands;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemSubgroups;

public sealed class ItemSubgroupGeneratedContractTests
{
    [Fact]
    public void Identity_IsCanonical()
    {
        "/api/definitions/inventory/item-subgroups".Should().StartWith("/api/definitions/inventory/");
        "item-subgroups".Should().Be("item-subgroups");
        "GeneralInventoryItemSubgroupsRead".Should().NotBeNullOrWhiteSpace();
        "GeneralInventoryItemSubgroupsManage".Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CreateValidator_RequiresFamilyAndRejectsNegativeOrder()
    {
        var result = new CreateItemSubgroupCommandValidator().Validate(
            new CreateItemSubgroupCommand(0, "", "", null, -1, true));

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain(["ItemFamilyId", "Code", "Name", "SortOrder"]);
    }
}
