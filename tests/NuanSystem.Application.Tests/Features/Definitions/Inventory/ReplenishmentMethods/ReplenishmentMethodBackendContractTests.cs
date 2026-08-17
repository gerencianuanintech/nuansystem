using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Commands;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ReplenishmentMethods;

public sealed class ReplenishmentMethodBackendContractTests
{
    [Fact]
    public void CodeNormalization_TrimsWithoutChangingCasing()
    {
        var method = typeof(CreateReplenishmentMethodCommandHandler).GetMethod(
            "NormalizeCode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;

        method.Invoke(null, [" Comprar "]).Should().Be("Comprar");
        method.Invoke(null, [" FABRICAR "]).Should().Be("FABRICAR");
    }

    [Fact]
    public async Task Validator_AllowsZeroSortOrderAndEitherActiveState()
    {
        var active = await new CreateReplenishmentMethodCommandValidator().ValidateAsync(
            new CreateReplenishmentMethodCommand("COMPRAR", "Comprar", null, 0, true));
        var inactive = await new CreateReplenishmentMethodCommandValidator().ValidateAsync(
            new CreateReplenishmentMethodCommand("FABRICAR", "Fabricar", null, 0, false));

        active.IsValid.Should().BeTrue();
        inactive.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_RejectsInvalidBasicShape()
    {
        var result = await new CreateReplenishmentMethodCommandValidator().ValidateAsync(
            new CreateReplenishmentMethodCommand("", "", new string('D', 501), -1, true));

        result.IsValid.Should().BeFalse();
        result.Errors.Select(x => x.PropertyName).Should().Contain(["Code", "Name", "Description", "SortOrder"]);
    }
}
