using FluentAssertions;
using NuanSystem.Application.Features.BusinessPartners.Policies;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerIdentityPolicyTests
{
    [Theory]
    [InlineData(" 09.999-999 99001 ", "0999999999001")]
    [InlineData(" ab-12. 3 ", "AB123")]
    public void NormalizeIdentification_RemovesFormattingAndUppercases(string raw, string expected)
    {
        BusinessPartnerIdentityPolicy.NormalizeIdentification(raw).Should().Be(expected);
    }

    [Fact]
    public void CreateInternalCode_UsesStableGlobalIdentity()
    {
        var id = Guid.Parse("7f777a58-4bc5-4a4c-b29a-50f3e6c2b0cd");

        BusinessPartnerIdentityPolicy.CreateInternalCode(id)
            .Should().Be("BP-7F777A584BC54A4CB29A50F3E6C2B0CD");
    }
}
