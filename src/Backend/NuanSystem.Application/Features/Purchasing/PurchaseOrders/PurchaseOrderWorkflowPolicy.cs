using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders;

public static class PurchaseOrderWorkflowPolicy
{
    public const string ConfirmInvalidMessage = "La orden de compra no se puede confirmar desde el estado actual.";
    public const string SendToApprovalInvalidMessage = "La orden de compra no se puede enviar a aprobacion desde el estado actual.";
    public const string ApproveInvalidMessage = "La orden de compra solo puede aprobarse cuando esta pendiente de aprobacion.";
    public const string RejectInvalidMessage = "La orden de compra solo puede rechazarse cuando esta pendiente de aprobacion.";
    public const string SapSyncInvalidMessage = "La orden de compra no se puede sincronizar con SAP desde el estado actual.";
    public const string EditInvalidMessage = "La orden de compra no se puede modificar desde el estado actual.";
    public const string DeleteInvalidMessage = "La orden de compra no se puede anular desde el estado actual.";
    public const string ModifyCollectionsInvalidMessage = "La orden de compra no permite modificar anexos o documentos relacionados desde el estado actual.";

    public static bool CanConfirm(string status)
    {
        return status is PurchaseOrderStatuses.Draft or PurchaseOrderStatuses.Rejected;
    }

    public static string GetStatusAfterConfirmation(string status, bool requiresApproval)
    {
        var validation = EnsureCanConfirm(status);
        if (!validation.IsSuccess)
        {
            throw new InvalidOperationException(validation.Message);
        }

        // TODO: Definir ApprovalPolicy para determinar si una OC requiere aprobacion.
        // TODO: Parametrizar aprobacion por empresa, tipo de compra, monto, proveedor, centro de costo, usuario, bodega u otras reglas.
        // TODO: Crear ConfirmPurchaseOrderCommand para decidir el destino segun requiresApproval.
        return requiresApproval
            ? PurchaseOrderStatuses.PendingApproval
            : PurchaseOrderStatuses.Approved;
    }

    public static bool CanEdit(string status)
    {
        return status is PurchaseOrderStatuses.Draft or PurchaseOrderStatuses.Rejected or PurchaseOrderStatuses.SapError;
    }

    public static bool CanDelete(string status)
    {
        return status is PurchaseOrderStatuses.Draft or PurchaseOrderStatuses.Rejected;
    }

    public static bool CanSendToApproval(string status)
    {
        return CanConfirm(status);
    }

    public static bool CanApprove(string status)
    {
        return status == PurchaseOrderStatuses.PendingApproval;
    }

    public static bool CanReject(string status)
    {
        return status == PurchaseOrderStatuses.PendingApproval;
    }

    public static bool CanRequestSapSync(string status)
    {
        return status == PurchaseOrderStatuses.Approved;
    }

    public static bool CanRetrySapSync(string status)
    {
        return status == PurchaseOrderStatuses.SapError;
    }

    public static bool CanViewRelatedDocuments(string status)
    {
        return IsKnownStatus(status);
    }

    public static bool CanModifyRelatedDocuments(string status)
    {
        return status is PurchaseOrderStatuses.Draft
            or PurchaseOrderStatuses.Rejected
            or PurchaseOrderStatuses.PendingApproval
            or PurchaseOrderStatuses.Approved
            or PurchaseOrderStatuses.SapError;
    }

    public static bool CanViewAttachments(string status)
    {
        return IsKnownStatus(status);
    }

    public static bool CanModifyAttachments(string status)
    {
        return CanModifyRelatedDocuments(status);
    }

    public static Result<bool> EnsureCanConfirm(string status)
    {
        return CanConfirm(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(ConfirmInvalidMessage);
    }

    public static Result<bool> EnsureCanSendToApproval(string status)
    {
        return CanSendToApproval(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(SendToApprovalInvalidMessage);
    }

    public static Result<bool> EnsureCanApprove(string status)
    {
        return CanApprove(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(ApproveInvalidMessage);
    }

    public static Result<bool> EnsureCanReject(string status)
    {
        return CanReject(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(RejectInvalidMessage);
    }

    public static Result<bool> EnsureCanRequestSapSync(string status)
    {
        return CanRequestSapSync(status) || CanRetrySapSync(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(SapSyncInvalidMessage);
    }

    public static Result<bool> EnsureCanEdit(string status)
    {
        return CanEdit(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(EditInvalidMessage);
    }

    public static Result<bool> EnsureCanDelete(string status)
    {
        return CanDelete(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(DeleteInvalidMessage);
    }

    public static Result<bool> EnsureCanModifyAttachmentsOrRelatedDocuments(string status)
    {
        return CanModifyAttachments(status) && CanModifyRelatedDocuments(status)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(ModifyCollectionsInvalidMessage);
    }

    private static bool IsKnownStatus(string status)
    {
        return status is PurchaseOrderStatuses.Draft
            or PurchaseOrderStatuses.PendingApproval
            or PurchaseOrderStatuses.Approved
            or PurchaseOrderStatuses.Rejected
            or PurchaseOrderStatuses.SapPending
            or PurchaseOrderStatuses.SapSynced
            or PurchaseOrderStatuses.SapError
            or PurchaseOrderStatuses.Closed
            or PurchaseOrderStatuses.Cancelled;
    }
}
