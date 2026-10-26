using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapMasterDataMappingContractTests
{
    [Fact]
    public void Contract_contains_the_three_approved_master_data_entities()
    {
        SapMasterDataMappingContract.EntityCodes.Should().BeEquivalentTo(
            ["Suppliers", "Customers", "Items"],
            options => options.WithStrictOrdering());
    }

    [Fact]
    public void Supplier_identity_mapping_keeps_internal_code_separate_from_sap_code()
    {
        var mappings = SapMasterDataMappingContract.SupplierMappings;

        mappings.Should().ContainEquivalentOf(new SapFieldMappingDefinition(
            "CardCode", "SapCardCode", SapMappingDirection.SapToErp, true, true, "Trim"));
        mappings.Should().ContainEquivalentOf(new SapFieldMappingDefinition(
            "Code", "Code", SapMappingDirection.Generated, true, false, "GenerateInternalCode"));
    }

    [Fact]
    public void Passport_is_the_foreign_taxpayer_rule_and_supplier_prefixes_are_role_specific()
    {
        SapMasterDataMappingContract.ForeignTaxpayerType.Should().Be("PASAPORTE");
        SapMasterDataMappingContract.SupplierNationalPrefixes.Should().BeEquivalentTo(["PL", "P"]);
        SapMasterDataMappingContract.SupplierForeignPrefixes.Should().BeEquivalentTo(["PE"]);
    }

    [Fact]
    public void Both_direction_is_not_allowed_until_export_is_executable()
    {
        SapMasterDataMappingContract.CanActivateBoth(importExecutable: true, exportExecutable: false)
            .Should().BeFalse();
        SapMasterDataMappingContract.CanActivateBoth(importExecutable: true, exportExecutable: true)
            .Should().BeTrue();
    }
}
