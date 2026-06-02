using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders;

internal static class PurchaseOrderCalculator
{
    public static PurchaseOrderPersistData BuildPersistData(
        int? id,
        PurchaseOrderSaveRequest request,
        string status,
        int? auditUserId,
        string? auditUserName,
        string? sapStatus = null)
    {
        var lines = request.Lines
            .Where(line => line.ItemId > 0 && line.Quantity > 0)
            .Select((line, index) => CalculateLine(line, index + 1))
            .ToArray();

        var subtotal = lines.Sum(line => line.LineSubtotal);
        var discountPercent = NormalizePercent(request.DiscountPercent);
        var discountAmount = Math.Round(subtotal * discountPercent / 100m, 6);
        var taxableBase = Math.Max(0m, subtotal - discountAmount);
        var lineTaxBase = lines.Sum(line => line.LineSubtotal) == 0
            ? 0m
            : taxableBase / lines.Sum(line => line.LineSubtotal);
        var taxAmount = Math.Round(lines.Sum(line => line.TaxAmount * lineTaxBase), 6);
        var totalAmount = taxableBase + taxAmount;

        return new PurchaseOrderPersistData(
            id,
            request.BranchId,
            request.DocumentSeriesId,
            Trim(request.SeriesCode, 50) ?? "OC-2026",
            Trim(request.DocumentNumber, 50) ?? "OC-000001",
            request.SupplierId,
            Trim(request.SupplierCode, 50) ?? string.Empty,
            Trim(request.SupplierName, 200) ?? string.Empty,
            Trim(request.SupplierTaxId, 50),
            Trim(request.ContactName, 200),
            Trim(request.Phone, 80),
            Trim(request.Email, 200),
            request.DocumentDate.Date,
            request.DeliveryDate.Date,
            Trim(request.CurrencyCode, 10) ?? "USD",
            request.ExchangeRate <= 0 ? 1 : request.ExchangeRate,
            request.PaymentTermId,
            request.PriceListId,
            request.BuyerId,
            request.MainWarehouseId,
            request.ProjectId,
            request.CostCenterId,
            request.PurchaseTypeId,
            Trim(request.Comments, 2000),
            status,
            subtotal,
            discountPercent,
            discountAmount,
            taxAmount,
            totalAmount,
            lines.Length,
            lines.Sum(line => line.Quantity),
            0m,
            "22",
            sapStatus ?? PurchaseOrderSapStatuses.Pending,
            lines,
            request.Addresses ?? Array.Empty<PurchaseOrderAddressSaveRequest>(),
            request.RelatedDocuments ?? Array.Empty<PurchaseOrderRelatedDocumentSaveRequest>(),
            request.Attachments ?? Array.Empty<PurchaseOrderAttachmentSaveRequest>(),
            auditUserId,
            auditUserName);
    }

    private static PurchaseOrderLinePersistData CalculateLine(PurchaseOrderLineSaveRequest line, int fallbackLineNumber)
    {
        var quantity = Math.Max(0m, line.Quantity);
        var unitPrice = Math.Max(0m, line.UnitPrice);
        var discountPercent = NormalizePercent(line.DiscountPercent);
        var lineGross = Math.Round(quantity * unitPrice, 6);
        var discountAmount = Math.Round(lineGross * discountPercent / 100m, 6);
        var lineSubtotal = Math.Max(0m, lineGross - discountAmount);
        var taxRate = Math.Max(0m, line.TaxRate);
        var taxAmount = Math.Round(lineSubtotal * taxRate, 6);

        return new PurchaseOrderLinePersistData(
            line.Id,
            line.LineNumber > 0 ? line.LineNumber : fallbackLineNumber,
            line.ItemId,
            Trim(line.ItemCode, 50) ?? string.Empty,
            Trim(line.ItemName, 200) ?? string.Empty,
            line.UnitId,
            Trim(line.UnitCode, 50),
            quantity,
            quantity,
            unitPrice,
            discountPercent,
            discountAmount,
            line.TaxId,
            Trim(line.TaxCode, 50) ?? string.Empty,
            taxRate,
            taxAmount,
            line.WarehouseId,
            Trim(line.WarehouseCode, 50) ?? string.Empty,
            line.DeliveryDate.Date,
            line.CostCenterId,
            line.ProjectId,
            lineSubtotal,
            lineSubtotal + taxAmount,
            PurchaseOrderLineStatuses.Open);
    }

    private static decimal NormalizePercent(decimal value)
    {
        if (value < 0)
        {
            return 0;
        }

        return value > 100 ? 100 : value;
    }

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
