using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapBusinessPartnerIdentityGeneratorTests
{
    [Theory]
    [InlineData("0912345678", false, "PL0912345678")]
    [InlineData("AB123456", true, "PEAB123456")]
    public void Generates_supplier_prefix_by_taxpayer_type(string taxId, bool passport, string expected)
        => SapBusinessPartnerIdentityGenerator.GenerateSupplierCode(taxId, passport).Should().Be(expected);

    [Fact]
    public void Existing_internal_code_is_preserved()
        => SapBusinessPartnerIdentityGenerator.GenerateSupplierCode("0912345678", false, "PROV-42").Should().Be("PROV-42");
}
