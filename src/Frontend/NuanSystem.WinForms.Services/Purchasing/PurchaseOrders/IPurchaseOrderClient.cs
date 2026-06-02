using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders.Models;

namespace NuanSystem.WinForms.Services.Purchasing.PurchaseOrders;

public interface IPurchaseOrderClient
{
    Task<IReadOnlyCollection<PurchaseOrderItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrderLookups> GetLookupsAsync(CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> CreateAsync(SavePurchaseOrderRequest request, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> UpdateAsync(int id, SavePurchaseOrderRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> SendToApprovalAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> ApproveAsync(int id, string? observation, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> RejectAsync(int id, string? observation, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> SyncSapAsync(int id, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> AddRelatedDocumentAsync(int id, PurchaseOrderRelatedDocumentItem relatedDocument, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> DeleteRelatedDocumentAsync(int id, int relatedId, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> AddAttachmentAsync(int id, PurchaseOrderAttachmentItem attachment, CancellationToken cancellationToken = default);

    Task<PurchaseOrderDetail> DeleteAttachmentAsync(int id, int attachmentId, CancellationToken cancellationToken = default);
}
