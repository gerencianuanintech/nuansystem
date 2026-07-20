using System.Text.Json;
using FluentAssertions;
using NuanSystem.SapIntegration.PaymentTerms;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapServiceLayerPaymentTermReaderTests
{
    [Fact]
    public void Map_TreatsNullOptionalNumericFieldsAsZero()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "GroupNumber": 5,
              "PaymentTermsGroupName": "Crédito 8 días",
              "NumberOfAdditionalDays": 8,
              "NumberOfAdditionalMonths": null,
              "NumberOfInstallments": null
            }
            """);

        var result = SapServiceLayerPaymentTermReader.Map(document.RootElement);

        result.GroupNumber.Should().Be(5);
        result.Name.Should().Be("Crédito 8 días");
        result.AdditionalDays.Should().Be(8);
        result.AdditionalMonths.Should().Be(0);
        result.NumberOfInstallments.Should().Be(0);
    }
}
