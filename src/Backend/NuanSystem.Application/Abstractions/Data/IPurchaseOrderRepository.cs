using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IPurchaseOrderRepository
{
    Task<IReadOnlyCollection<PurchaseOrderSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrderLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default);

    Task<int> CreateAsync(PurchaseOrderPersistData order, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(PurchaseOrderPersistData order, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);

    Task<bool> DeleteIfCurrentAsync(
        int id,
        IReadOnlyCollection<string> expectedCurrentStatuses,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(int id, string status, int? userId, string? userName, CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusIfCurrentAsync(
        int id,
        string nextStatus,
        IReadOnlyCollection<string> expectedCurrentStatuses,
        int? userId,
        string? userName,
        CancellationToken cancellationToken = default);

    Task<PurchaseOrderSapSyncLogDto> AddSapLogAsync(int id, string process, string status, string? message, int? userId, string? userName, CancellationToken cancellationToken = default);
}

public sealed record PurchaseOrderPersistData(
    int? Id,
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
    string Status,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    int TotalItems,
    decimal TotalQuantity,
    decimal TotalWeight,
    string SapObjectType,
    string SapStatus,
    IReadOnlyCollection<PurchaseOrderLinePersistData> Lines,
    IReadOnlyCollection<PurchaseOrderAddressSaveRequest> Addresses,
    IReadOnlyCollection<PurchaseOrderRelatedDocumentSaveRequest> RelatedDocuments,
    IReadOnlyCollection<PurchaseOrderAttachmentSaveRequest> Attachments,
    int? AuditUserId,
    string? AuditUserName);

public sealed record PurchaseOrderLinePersistData(
    int? Id,
    int LineNumber,
    int ItemId,
    string ItemCode,
    string ItemName,
    int? UnitId,
    string? UnitCode,
    decimal Quantity,
    decimal OpenQuantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal DiscountAmount,
    int? TaxId,
    string TaxCode,
    decimal TaxRate,
    decimal TaxAmount,
    int WarehouseId,
    string WarehouseCode,
    DateTime DeliveryDate,
    int? CostCenterId,
    int? ProjectId,
    decimal LineSubtotal,
    decimal LineTotal,
    string Status);
