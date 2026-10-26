namespace NuanSystem.Domain.Purchasing;

public sealed record PurchaseOrderLineValues(decimal Quantity, decimal UnitPrice, decimal DiscountPercent, decimal TaxRate);

public sealed record PurchaseOrderLineAmounts(
    decimal Quantity, decimal UnitPrice, decimal DiscountPercent, decimal DiscountAmount,
    decimal TaxRate, decimal TaxAmount, decimal Subtotal, decimal Total);

public sealed record PurchaseOrderAmounts(
    decimal Subtotal, decimal DiscountPercent, decimal DiscountAmount, decimal TaxAmount,
    decimal Total, decimal TotalQuantity, IReadOnlyList<PurchaseOrderLineAmounts> Lines);

/// <summary>Amounts in the document currency, without exchange conversion or external state.</summary>
public static class PurchaseOrderAmountCalculator
{
    public const int AmountDecimalPlaces = 6;
    public const MidpointRounding Rounding = MidpointRounding.ToEven;

    public static PurchaseOrderAmounts Calculate(IEnumerable<PurchaseOrderLineValues> values, decimal headerDiscountPercent)
    {
        ArgumentNullException.ThrowIfNull(values);
        var lines = values.Select(CalculateLine).ToArray();
        var subtotal = lines.Sum(line => line.Subtotal);
        var discountPercent = NormalizePercent(headerDiscountPercent);
        var discountAmount = Round(subtotal * discountPercent / 100m);
        var taxableBase = Math.Max(0m, subtotal - discountAmount);
        var taxScale = subtotal == 0m ? 0m : taxableBase / subtotal;
        var taxAmount = Round(lines.Sum(line => line.TaxAmount * taxScale));
        return new PurchaseOrderAmounts(subtotal, discountPercent, discountAmount, taxAmount,
            taxableBase + taxAmount, lines.Sum(line => line.Quantity), Array.AsReadOnly(lines));
    }

    private static PurchaseOrderLineAmounts CalculateLine(PurchaseOrderLineValues values)
    {
        var quantity = Math.Max(0m, values.Quantity);
        var price = Math.Max(0m, values.UnitPrice);
        var discountPercent = NormalizePercent(values.DiscountPercent);
        var gross = Round(quantity * price);
        var discount = Round(gross * discountPercent / 100m);
        var subtotal = Math.Max(0m, gross - discount);
        var taxRate = Math.Max(0m, values.TaxRate);
        var tax = Round(subtotal * taxRate);
        return new PurchaseOrderLineAmounts(quantity, price, discountPercent, discount, taxRate, tax, subtotal, subtotal + tax);
    }

    private static decimal Round(decimal value) => Math.Round(value, AmountDecimalPlaces, Rounding);
    private static decimal NormalizePercent(decimal value) => Math.Clamp(value, 0m, 100m);
}
