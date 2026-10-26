using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapFieldMappingEngineTests
{
    [Fact]
    public void Maps_trimmed_and_uppercased_values_to_destination_fields()
    {
        var mappings = new[]
        {
            new SapFieldMappingDefinition("CardName", "Name", SapMappingDirection.SapToErp, false, false, "Trim"),
            new SapFieldMappingDefinition("CardCode", "SapCardCode", SapMappingDirection.SapToErp, true, true, "UpperInvariant")
        };

        var result = SapFieldMappingEngine.Map(
            new Dictionary<string, object?> { ["CardName"] = "  proveedor  ", ["CardCode"] = "p001" },
            mappings,
            SapMappingDirection.SapToErp);

        result.IsValid.Should().BeTrue();
        result.Values["Name"].Should().Be("proveedor");
        result.Values["SapCardCode"].Should().Be("P001");
    }

    [Fact]
    public void Rejects_unknown_transformations_and_missing_required_values()
    {
        var mappings = new[]
        {
            new SapFieldMappingDefinition("TaxId", "IdentificationNumber", SapMappingDirection.SapToErp, true, false, "Unknown"),
            new SapFieldMappingDefinition("Missing", "Name", SapMappingDirection.SapToErp, true, false, "Trim")
        };

        var result = SapFieldMappingEngine.Map(new Dictionary<string, object?>(), mappings, SapMappingDirection.SapToErp);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.Code).Should().Contain(SapMappingValidationCodes.TransformationUnknown);
        result.Errors.Select(error => error.Code).Should().Contain(SapMappingValidationCodes.FieldRequired);
    }
}
