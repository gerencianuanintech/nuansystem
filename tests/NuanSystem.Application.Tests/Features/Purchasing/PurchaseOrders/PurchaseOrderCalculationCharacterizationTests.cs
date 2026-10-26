using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderCalculationCharacterizationTests
{
    [Fact]
    public void Line_and_header_discounts_keep_six_decimal_amounts_and_proportional_tax()
    {
        var result = Calculate(Request(Line(2m, 100m, 10m, 0.15m)) with { DiscountPercent = 10m });
        var line = Assert.Single(result.Lines);
        Assert.Equal(20m, line.DiscountAmount);
        Assert.Equal(180m, line.LineSubtotal);
        Assert.Equal(27m, line.TaxAmount);
        Assert.Equal(207m, line.LineTotal);
        Assert.Equal(180m, result.Subtotal);
        Assert.Equal(18m, result.DiscountAmount);
        Assert.Equal(24.3m, result.TaxAmount);
        Assert.Equal(186.3m, result.TotalAmount);
    }

    [Fact]
    public void Header_discount_scales_mixed_tax_without_reallocating_persisted_line_amounts()
    {
        var result = Calculate(Request(Line(1m, 100m, tax: 0.15m), Line(1m, 100m)) with { DiscountPercent = 10m });
        Assert.Equal(200m, result.Subtotal);
        Assert.Equal(20m, result.DiscountAmount);
        Assert.Equal(13.5m, result.TaxAmount);
        Assert.Equal(193.5m, result.TotalAmount);
        Assert.Equal(215m, result.Lines.Sum(line => line.LineTotal));
    }

    [Theory]
    [InlineData("1.0000005", "1.000000")]
    [InlineData("1.0000015", "1.000002")]
    public void Gross_rounding_is_to_even_at_six_places(string price, string expected)
    {
        var result = Calculate(Request(Line(1m, decimal.Parse(price, System.Globalization.CultureInfo.InvariantCulture))));
        Assert.Equal(decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture), result.TotalAmount);
    }

    [Theory]
    [InlineData(-1, 0, 100)]
    [InlineData(101, 100, 0)]
    public void Header_discount_is_clamped_without_changing_the_line(int discount, int normalized, int total)
    {
        var result = Calculate(Request(Line(1m, 100m)) with { DiscountPercent = discount });
        Assert.Equal(normalized, result.DiscountPercent);
        Assert.Equal(total, result.TotalAmount);
        Assert.Equal(100m, Assert.Single(result.Lines).LineTotal);
    }

    [Theory]
    [InlineData(-1, 0, 100)]
    [InlineData(101, 100, 0)]
    public void Line_discount_is_clamped(int discount, int normalized, int total)
    {
        var result = Calculate(Request(Line(1m, 100m, discount)));
        Assert.Equal(normalized, Assert.Single(result.Lines).DiscountPercent);
        Assert.Equal(total, result.TotalAmount);
    }

    [Fact]
    public void Empty_or_filtered_lines_produce_zero_totals_and_sequential_fallback_numbers()
    {
        var empty = Calculate(Request());
        Assert.Empty(empty.Lines);
        Assert.Equal(0m, empty.TotalAmount);
        var filtered = Calculate(Request(Line(0m, 100m), Line(-1m, 100m),
            Line(1m, 100m) with { ItemId = 0 }, Line(2m, 3m), Line(1m, 4m) with { LineNumber = 9 }));
        Assert.Equal(2, filtered.TotalItems);
        Assert.Equal(3m, filtered.TotalQuantity);
        Assert.Equal(10m, filtered.TotalAmount);
        Assert.Equal(new[] { 1, 9 }, filtered.Lines.Select(line => line.LineNumber));
    }

    [Fact]
    public void Negative_price_and_tax_normalize_to_zero_and_tax_rate_is_not_capped()
    {
        var zero = Calculate(Request(Line(2m, -1m, tax: -0.15m)));
        Assert.Equal(0m, zero.TotalAmount);
        Assert.Equal(0m, Assert.Single(zero.Lines).UnitPrice);
        Assert.Equal(0m, Assert.Single(zero.Lines).TaxRate);
        Assert.Equal(250m, Calculate(Request(Line(1m, 100m, tax: 1.5m))).TotalAmount);
    }

    [Fact]
    public void Quantity_and_unit_price_precision_are_preserved_while_gross_is_rounded()
    {
        var result = Calculate(Request(Line(0.123456789m, 1m)));
        Assert.Equal(0.123456789m, result.TotalQuantity);
        Assert.Equal(0.123457m, result.TotalAmount);
        Assert.Equal(0.123456789m, Assert.Single(result.Lines).OpenQuantity);
    }

    [Fact]
    public void Currency_and_exchange_rate_are_metadata_without_currency_conversion()
    {
        var result = Calculate(Request(Line(1m, 100m)) with { CurrencyCode = " EUR ", ExchangeRate = 7.123456789m });
        Assert.Equal("EUR", result.CurrencyCode);
        Assert.Equal(7.123456789m, result.ExchangeRate);
        Assert.Equal(100m, result.TotalAmount);
        var defaults = Calculate(Request(Line(1m, 100m)) with { CurrencyCode = " ", ExchangeRate = 0m });
        Assert.Equal("USD", defaults.CurrencyCode);
        Assert.Equal(1m, defaults.ExchangeRate);
    }

    [Fact]
    public void Persistence_mapping_preserves_metadata_identity_audit_and_legacy_SAP_values()
    {
        var request = Request(Line(1m, 3m) with { Id = 99, ItemCode = " ITEM ", DeliveryDate = new DateTime(2026, 10, 23, 13, 0, 0) });
        var result = PurchaseOrderCalculator.BuildPersistData(42, request, "Draft", 7, "auditor", "Synced");
        Assert.Equal(42, result.Id);
        Assert.Equal(99, Assert.Single(result.Lines).Id);
        Assert.Equal("ITEM", Assert.Single(result.Lines).ItemCode);
        Assert.Equal(new DateTime(2026, 10, 23), Assert.Single(result.Lines).DeliveryDate);
        Assert.Equal(7, result.AuditUserId);
        Assert.Equal("auditor", result.AuditUserName);
        Assert.Equal("22", result.SapObjectType);
        Assert.Equal("Synced", result.SapStatus);
        Assert.Same(request.Addresses, result.Addresses);
        Assert.Same(request.Attachments, result.Attachments);
        Assert.Same(request.RelatedDocuments, result.RelatedDocuments);
    }

    [Fact]
    public void Decimal_overflow_remains_explicit()
    {
        Assert.Throws<OverflowException>(() => Calculate(Request(Line(2m, decimal.MaxValue))));
    }

    internal static NuanSystem.Application.Abstractions.Data.PurchaseOrderPersistData Calculate(PurchaseOrderSaveRequest request) =>
        PurchaseOrderCalculator.BuildPersistData(null, request, "Draft", 7, "auditor");

    internal static PurchaseOrderLineSaveRequest Line(decimal quantity, decimal price, decimal discount = 0m, decimal tax = 0m) =>
        new(null, 0, 1, "ITEM", "Item", 1, "UN", quantity, price, discount, 1, "TAX", tax, 1, "WH", new DateTime(2026, 10, 23), null, null);

    internal static PurchaseOrderSaveRequest Request(params PurchaseOrderLineSaveRequest[] lines) => new(
        BranchId: 1, DocumentSeriesId: 3, SeriesCode: "OC-TEST", DocumentNumber: "OC-000042",
        SupplierId: 10, SupplierCode: "SUP", SupplierName: "Supplier", SupplierTaxId: null,
        ContactName: null, Phone: null, Email: null, DocumentDate: new DateTime(2026, 10, 23),
        DeliveryDate: new DateTime(2026, 10, 24), CurrencyCode: "USD", ExchangeRate: 1m,
        PaymentTermId: 1, PriceListId: null, BuyerId: 1, MainWarehouseId: 1, ProjectId: null,
        CostCenterId: null, PurchaseTypeId: null, Comments: null, DiscountPercent: 0m,
        Lines: lines, Addresses: [], RelatedDocuments: [], Attachments: []);
}
