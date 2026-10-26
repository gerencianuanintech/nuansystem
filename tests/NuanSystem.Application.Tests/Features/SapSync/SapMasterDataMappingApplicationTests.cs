using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapMasterDataMappingApplicationTests
{
    [Fact]
    public void Valid_supplier_configuration_passes_activation_validation()
    {
        var result = SapMappingValidationService.ValidateActivation(
            new SapMappingActivationSnapshot(
                QueryApproved: true,
                QueryActive: true,
                EntityCode: "Suppliers",
                Direction: SapMappingDirection.SapToErp,
                ImportExecutable: true,
                ExportExecutable: false,
                Mappings: SapMasterDataMappingContract.SupplierMappings));

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Activation_requires_an_approved_active_query()
    {
        var result = SapMappingValidationService.ValidateActivation(
            new SapMappingActivationSnapshot(
                QueryApproved: false,
                QueryActive: true,
                EntityCode: "Suppliers",
                Direction: SapMappingDirection.SapToErp,
                ImportExecutable: true,
                ExportExecutable: false,
                Mappings: SapMasterDataMappingContract.SupplierMappings));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == SapMappingValidationCodes.QueryNotApproved);
    }

    [Fact]
    public void Activation_requires_one_unique_required_identity_mapping()
    {
        var mappings = new[]
        {
            new SapFieldMappingDefinition("CardCode", "SapCardCode", SapMappingDirection.SapToErp, true, true, "Trim"),
            new SapFieldMappingDefinition("OtherCode", "SapCardCode", SapMappingDirection.SapToErp, true, true, "Trim")
        };

        var result = SapMappingValidationService.ValidateActivation(
            new SapMappingActivationSnapshot(true, true, "Suppliers", SapMappingDirection.SapToErp, true, false, mappings));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == SapMappingValidationCodes.DuplicateDestination);
        result.Errors.Should().Contain(error => error.Code == SapMappingValidationCodes.IdentityMappingMissing);
    }

    [Fact]
    public void Both_direction_requires_import_and_export_capabilities()
    {
        var result = SapMappingValidationService.ValidateActivation(
            new SapMappingActivationSnapshot(true, true, "Suppliers", SapMappingDirection.Both, true, false, SapMasterDataMappingContract.SupplierMappings));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == SapMappingValidationCodes.DirectionUnsupported);
    }

    [Fact]
    public void Unknown_transformation_is_rejected()
    {
        var mappings = new[]
        {
            new SapFieldMappingDefinition("CardCode", "SapCardCode", SapMappingDirection.SapToErp, true, true, "UntrustedSql")
        };

        var result = SapMappingValidationService.ValidateMappings(mappings);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == SapMappingValidationCodes.TransformationUnknown);
    }
}
