using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed record CreatePurchaseOrderCommand(
    int? BranchId,
    int? DocumentSeriesId,
    string SeriesCode,
    string DocumentNumber,
    int SupplierId,
    string SupplierCode,
    string SupplierName,
    string? SupplierTaxId,
    string? ContactName,
    string? Phone,
    string? Email,
    DateTime DocumentDate,
    DateTime DeliveryDate,
    string CurrencyCode,
    decimal ExchangeRate,
    int? PaymentTermId,
    int? PriceListId,
    int? BuyerId,
    int? MainWarehouseId,
    int? ProjectId,
    int? CostCenterId,
    int? PurchaseTypeId,
    string? Comments,
    decimal DiscountPercent,
    IReadOnlyCollection<PurchaseOrderLineSaveRequest> Lines,
    IReadOnlyCollection<PurchaseOrderAddressSaveRequest> Addresses,
    IReadOnlyCollection<PurchaseOrderRelatedDocumentSaveRequest>? RelatedDocuments,
    IReadOnlyCollection<PurchaseOrderAttachmentSaveRequest>? Attachments,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>
{
    public PurchaseOrderSaveRequest ToRequest()
    {
        return new PurchaseOrderSaveRequest(
            BranchId,
            DocumentSeriesId,
            SeriesCode,
            DocumentNumber,
            SupplierId,
            SupplierCode,
            SupplierName,
            SupplierTaxId,
            ContactName,
            Phone,
            Email,
            DocumentDate,
            DeliveryDate,
            CurrencyCode,
            ExchangeRate,
            PaymentTermId,
            PriceListId,
            BuyerId,
            MainWarehouseId,
            ProjectId,
            CostCenterId,
            PurchaseTypeId,
            Comments,
            DiscountPercent,
            Lines,
            Addresses,
            RelatedDocuments,
            Attachments);
    }
}
