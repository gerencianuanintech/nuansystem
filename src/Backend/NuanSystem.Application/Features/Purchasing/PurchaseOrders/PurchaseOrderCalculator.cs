using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Domain.Purchasing;
using NuanSystem.Application.Features.SapSync.Mapping;
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
        var selectedLines = request.Lines.Where(line => line.ItemId > 0 && line.Quantity > 0).ToArray();
        var amounts = PurchaseOrderAmountCalculator.Calculate(
            selectedLines.Select(line => new PurchaseOrderLineValues(line.Quantity, line.UnitPrice, line.DiscountPercent, line.TaxRate)),
            request.DiscountPercent);
        var lines = selectedLines.Select((line, index) => MapLine(line, amounts.Lines[index], index + 1)).ToArray();

        return new PurchaseOrderPersistData(
            id,
            request.BranchId,
            request.DocumentSeriesId,
            RequireIdentifier(request.SeriesCode, nameof(request.SeriesCode)),
            RequireIdentifier(request.DocumentNumber, nameof(request.DocumentNumber)),
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
            amounts.Subtotal,
            amounts.DiscountPercent,
            amounts.DiscountAmount,
            amounts.TaxAmount,
            amounts.Total,
            lines.Length,
            amounts.TotalQuantity,
            0m,
            SapPurchaseOrderMapping.ObjectType,
            sapStatus ?? PurchaseOrderSapStatuses.Pending,
            lines,
            request.Addresses ?? Array.Empty<PurchaseOrderAddressSaveRequest>(),
            request.RelatedDocuments ?? Array.Empty<PurchaseOrderRelatedDocumentSaveRequest>(),
            request.Attachments ?? Array.Empty<PurchaseOrderAttachmentSaveRequest>(),
            auditUserId,
            auditUserName);
    }

    private static PurchaseOrderLinePersistData MapLine(
        PurchaseOrderLineSaveRequest line, PurchaseOrderLineAmounts amounts, int fallbackLineNumber)
    {
        return new PurchaseOrderLinePersistData(
            line.Id,
            line.LineNumber > 0 ? line.LineNumber : fallbackLineNumber,
            line.ItemId,
            Trim(line.ItemCode, 50) ?? string.Empty,
            Trim(line.ItemName, 200) ?? string.Empty,
            line.UnitId,
            Trim(line.UnitCode, 50),
            amounts.Quantity,
            amounts.Quantity,
            amounts.UnitPrice,
            amounts.DiscountPercent,
            amounts.DiscountAmount,
            line.TaxId,
            Trim(line.TaxCode, 50) ?? string.Empty,
            amounts.TaxRate,
            amounts.TaxAmount,
            line.WarehouseId,
            Trim(line.WarehouseCode, 50) ?? string.Empty,
            line.DeliveryDate.Date,
            line.CostCenterId,
            line.ProjectId,
            amounts.Subtotal,
            amounts.Total,
            PurchaseOrderLineStatuses.Open);
    }

    private static string RequireIdentifier(string? value, string field) =>
        Trim(value, 50) ?? throw new ArgumentException("La numeracion del documento es obligatoria.", field);

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
