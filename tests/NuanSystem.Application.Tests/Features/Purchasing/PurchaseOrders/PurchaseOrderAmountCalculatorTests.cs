using NuanSystem.Domain.Purchasing;
using static NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders.PurchaseOrderCalculationCharacterizationTests;

namespace NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderAmountCalculatorTests
{
    [Fact]
    public void Domain_calculates_amounts_from_numeric_values_without_document_or_integration_metadata()
    {
        var result = PurchaseOrderAmountCalculator.Calculate([new(2m, 100m, 10m, 0.15m)], 10m);
        Assert.Equal(180m, result.Subtotal);
        Assert.Equal(18m, result.DiscountAmount);
        Assert.Equal(24.3m, result.TaxAmount);
        Assert.Equal(186.3m, result.Total);
        Assert.Equal(2m, result.TotalQuantity);
        Assert.Equal(207m, Assert.Single(result.Lines).Total);
    }

    [Theory]
    [InlineData("0.0000005", "0")]
    [InlineData("0.0000015", "0.000002")]
    public void Tax_midpoints_round_to_even_at_six_decimal_places(string rate, string expected)
    {
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        var result = PurchaseOrderAmountCalculator.Calculate([new(1m, 1m, 0m, decimal.Parse(rate, culture))], 0m);
        Assert.Equal(decimal.Parse(expected, culture), result.TaxAmount);
        Assert.Equal(result.TaxAmount, Assert.Single(result.Lines).TaxAmount);
    }

    [Theory]
    [InlineData("0.00005", "0")]
    [InlineData("0.00015", "0.000002")]
    public void Discount_midpoints_round_to_even_at_six_decimal_places(string percent, string expected)
    {
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        var value = decimal.Parse(percent, culture);
        var result = PurchaseOrderAmountCalculator.Calculate([new(1m, 1m, value, 0m)], 0m);
        Assert.Equal(decimal.Parse(expected, culture), Assert.Single(result.Lines).DiscountAmount);
        var header = PurchaseOrderAmountCalculator.Calculate([new(1m, 1m, 0m, 0m)], value);
        Assert.Equal(decimal.Parse(expected, culture), header.DiscountAmount);
    }

    [Fact]
    public void Empty_and_fully_discounted_bases_do_not_divide_by_zero()
    {
        var empty = PurchaseOrderAmountCalculator.Calculate([], 100m);
        Assert.Equal(0m, empty.Total);
        var discounted = PurchaseOrderAmountCalculator.Calculate([new(1m, 100m, 100m, 0.15m)], 50m);
        Assert.Equal(0m, discounted.Subtotal);
        Assert.Equal(0m, discounted.TaxAmount);
        Assert.Equal(0m, discounted.Total);
    }

    [Fact]
    public void Decimal_boundary_without_multiplication_overflow_is_preserved()
    {
        var result = PurchaseOrderAmountCalculator.Calculate([new(1m, decimal.MaxValue, 0m, 0m)], 0m);
        Assert.Equal(decimal.MaxValue, result.Total);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Persistence_adapter_requires_numbering_instead_of_synthesizing_it(bool missingSeries)
    {
        var request = Request(Line(1m, 100m));
        request = missingSeries ? request with { SeriesCode = " " } : request with { DocumentNumber = " " };
        Assert.Throws<ArgumentException>(() => Calculate(request));
    }
}
