namespace NuanSystem.WinForms.Forms.BusinessPartners;

internal sealed class SupplierPurchaseHistoryViewModel
{
    public DateTime PurchaseDate { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int AverageDeliveryDays { get; set; }
}
