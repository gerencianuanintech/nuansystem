using FluentAssertions;
using NuanSystem.Application.Features.BusinessPartners.Policies;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSapCardCodePolicyTests
{
    [Theory]
    [InlineData(BusinessPartnerSapPrefixMode.NationalForeign, "Customer", "04", "0999999999001", "CN0999999999001")]
    [InlineData(BusinessPartnerSapPrefixMode.NationalForeign, "Customer", "pas", "AB123", "CEAB123")]
    [InlineData(BusinessPartnerSapPrefixMode.NationalForeign, "Supplier", "05", "0999999999001", "PL0999999999001")]
    [InlineData(BusinessPartnerSapPrefixMode.NationalForeign, "Supplier", "PAS", "AB123", "PEAB123")]
    [InlineData(BusinessPartnerSapPrefixMode.RoleOnly, "Customer", "PAS", "AB123", "CAB123")]
    [InlineData(BusinessPartnerSapPrefixMode.RoleOnly, "Supplier", "04", "0999999999001", "P0999999999001")]
    public void CreateSapCardCode_UsesClosedPrefixMatrix(
        BusinessPartnerSapPrefixMode prefixMode,
        string partnerType,
        string identificationTypeCode,
        string normalizedIdentificationNumber,
        string expected)
    {
        var result = BusinessPartnerSapCardCodePolicy.CreateSapCardCode(
            new BusinessPartnerSapCodePolicyData(prefixMode, "PAS"),
            partnerType,
            identificationTypeCode,
            normalizedIdentificationNumber);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }

    [Fact]
    public void CreateSapCardCode_RejectsUnsupportedPartnerType()
    {
        var result = BusinessPartnerSapCardCodePolicy.CreateSapCardCode(
            new BusinessPartnerSapCodePolicyData(BusinessPartnerSapPrefixMode.NationalForeign, "PAS"),
            "Carrier",
            "04",
            "0999999999001");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_ROLE_INVALID" && error.Field == "partnerType");
    }

    [Fact]
    public void CreateSapCardCode_RejectsEmptyNormalizedIdentification()
    {
        var result = BusinessPartnerSapCardCodePolicy.CreateSapCardCode(
            new BusinessPartnerSapCodePolicyData(BusinessPartnerSapPrefixMode.NationalForeign, "PAS"),
            "Customer",
            "04",
            "");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_IDENTIFICATION_REQUIRED" && error.Field == "normalizedIdentificationNumber");
    }

    [Fact]
    public void CreateSapCardCode_RejectsCardCodeLongerThanFifteenCharacters()
    {
        var result = BusinessPartnerSapCardCodePolicy.CreateSapCardCode(
            new BusinessPartnerSapCodePolicyData(BusinessPartnerSapPrefixMode.RoleOnly, "PAS"),
            "Customer",
            "04",
            "123456789012345");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_SAP_CARD_CODE_TOO_LONG" && error.Field == "normalizedIdentificationNumber");
    }
}
