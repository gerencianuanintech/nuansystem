using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders.Models;

namespace NuanSystem.WinForms.Services.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderClient(INuanApiClient apiClient) : IPurchaseOrderClient
{
    private const string BasePath = "/api/purchase-orders";

    public Task<IReadOnlyCollection<PurchaseOrderItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<PurchaseOrderItem>>(BasePath, cancellationToken);
    }

    public Task<PurchaseOrderDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<PurchaseOrderDetail>($"{BasePath}/{id}", cancellationToken);
    }

    public Task<PurchaseOrderLookups> GetLookupsAsync(string actionKey, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<PurchaseOrderLookups>($"{BasePath}/lookups?actionKey={Uri.EscapeDataString(actionKey)}", cancellationToken);
    }

    public Task<IReadOnlyCollection<PurchaseOrderFieldAccess>> GetFieldAccessAsync(int seriesId, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<PurchaseOrderFieldAccess>>($"{BasePath}/field-access?seriesId={seriesId}", cancellationToken);
    }

    public Task<PurchaseOrderDetail> CreateAsync(SavePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SavePurchaseOrderRequest, PurchaseOrderDetail>(BasePath, request, cancellationToken);
    }

    public Task<PurchaseOrderDetail> UpdateAsync(int id, SavePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SavePurchaseOrderRequest, PurchaseOrderDetail>($"{BasePath}/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"{BasePath}/{id}", cancellationToken);
    }

    public Task<PurchaseOrderDetail> SendToApprovalAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<object, PurchaseOrderDetail>($"{BasePath}/{id}/send-to-approval", new { }, cancellationToken);
    }

    public Task<PurchaseOrderDetail> ApproveAsync(int id, string? observation, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<PurchaseOrderWorkflowRequest, PurchaseOrderDetail>($"{BasePath}/{id}/approve", new PurchaseOrderWorkflowRequest(observation), cancellationToken);
    }

    public Task<PurchaseOrderDetail> RejectAsync(int id, string? observation, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<PurchaseOrderWorkflowRequest, PurchaseOrderDetail>($"{BasePath}/{id}/reject", new PurchaseOrderWorkflowRequest(observation), cancellationToken);
    }

    public Task<PurchaseOrderDetail> SyncSapAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<object, PurchaseOrderDetail>($"{BasePath}/{id}/sync-sap", new { }, cancellationToken);
    }

    public Task<PurchaseOrderDetail> AddRelatedDocumentAsync(int id, PurchaseOrderRelatedDocumentItem relatedDocument, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<PurchaseOrderRelatedDocumentItem, PurchaseOrderDetail>($"{BasePath}/{id}/related-documents", relatedDocument, cancellationToken);
    }

    public Task<PurchaseOrderDetail> DeleteRelatedDocumentAsync(int id, int relatedId, CancellationToken cancellationToken = default)
    {
        return apiClient.DeleteAsync<PurchaseOrderDetail>($"{BasePath}/{id}/related-documents/{relatedId}", cancellationToken);
    }

    public Task<PurchaseOrderDetail> AddAttachmentAsync(int id, PurchaseOrderAttachmentItem attachment, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<PurchaseOrderAttachmentItem, PurchaseOrderDetail>($"{BasePath}/{id}/attachments", attachment, cancellationToken);
    }

    public Task<PurchaseOrderDetail> DeleteAttachmentAsync(int id, int attachmentId, CancellationToken cancellationToken = default)
    {
        return apiClient.DeleteAsync<PurchaseOrderDetail>($"{BasePath}/{id}/attachments/{attachmentId}", cancellationToken);
    }
}
