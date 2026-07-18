using FluentAssertions;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Services;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncEntityDependencyPlannerTests
{
    [Fact]
    public void Plan_PlacesDependencyBeforeEntityDespiteManualOrder()
    {
        var entities = new[]
        {
            Entity("Item", 1),
            Entity("ItemGroups", 99)
        };

        var result = SyncEntityDependencyPlanner.Plan(
            entities,
            [],
            ["Item", "ItemGroups"],
            [Definition("Item", ["ItemGroups"]), Definition("ItemGroups", [])]);

        result.Select(entity => entity.EntityCode).Should().Equal("ItemGroups", "Item");
    }

    [Fact]
    public void Plan_RequestedEntityIncludesTransitiveDependencies()
    {
        var entities = new[]
        {
            Entity("PurchaseOrder", 1),
            Entity("Item", 2),
            Entity("ItemGroups", 3)
        };

        var result = SyncEntityDependencyPlanner.Plan(
            entities,
            ["PurchaseOrder"],
            ["PurchaseOrder", "Item", "ItemGroups"],
            [
                Definition("PurchaseOrder", ["Item"]),
                Definition("Item", ["ItemGroups"]),
                Definition("ItemGroups", [])
            ]);

        result.Select(entity => entity.EntityCode).Should().Equal("ItemGroups", "Item", "PurchaseOrder");
    }

    [Fact]
    public void Plan_RejectsUnexpectedCycle()
    {
        var action = () => SyncEntityDependencyPlanner.Plan(
            [Entity("A", 1), Entity("B", 2)],
            [],
            ["A", "B"],
            [Definition("A", ["B"]), Definition("B", ["A"])]);

        action.Should().Throw<InvalidOperationException>().WithMessage("*ciclo*");
    }

    private static SyncProfileEntityRecord Entity(string code, int order)
    {
        return new SyncProfileEntityRecord(
            1, 1, code, code, order, "Full", "Code", "UpdatedAt", null, null,
            true, true, true, false, 100, true);
    }

    private static SyncEntityDefinitionLookupDto Definition(
        string code,
        IReadOnlyCollection<string> dependencies)
    {
        return new SyncEntityDefinitionLookupDto(
            1, code, code, null, 100, true, true, true, true, "Code", "UpdatedAt",
            true, true, true, true, dependencies);
    }
}
