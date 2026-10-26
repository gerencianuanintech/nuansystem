using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapBusinessPartnerIdentityPolicyTests
{
    [Fact]
    public void Policy_generates_configured_supplier_code_for_passport()
    {
        var policy = new SapBusinessPartnerIdentityPolicy("CL", "CE", "PL", "PE", "RUC,CI,PASAPORTE", "PASAPORTE");

        policy.GenerateCode(SapBusinessPartnerRole.Supplier, "AB12345", "PASAPORTE")
            .Should().Be("PEAB12345");
    }

    [Fact]
    public void Policy_preserves_existing_code_and_rejects_empty_prefixes()
    {
        var policy = new SapBusinessPartnerIdentityPolicy("CL", "CE", "PL", "PE", "RUC", "PASAPORTE");

        policy.GenerateCode(SapBusinessPartnerRole.Customer, "0999999999", "RUC", "CLIENTE-7")
            .Should().Be("CLIENTE-7");

        FluentActions.Invoking(() => new SapBusinessPartnerIdentityPolicy("", "CE", "PL", "PE", "RUC", "PASAPORTE"))
            .Should().Throw<ArgumentException>();
    }
}
