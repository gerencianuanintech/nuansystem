using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class SendPurchaseOrderToApprovalCommandHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
    ICompanyContext companyContext)
    : ICommandHandler<SendPurchaseOrderToApprovalCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(SendPurchaseOrderToApprovalCommand request, CancellationToken cancellationToken)
    {
        return await PurchaseOrderWorkflow.ChangeStatusAsync(
            repository,
            seriesAccessRepository,
            companyContext,
            request.Id,
            PurchaseOrderStatuses.PendingApproval,
            "Orden enviada a aprobacion.",
            [PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected],
            PurchaseOrderWorkflowPolicy.EnsureCanSendToApproval,
            PurchaseOrderSecurity.ActionApprove,
            PurchaseOrderWorkflowActions.RequestApproval,
            observation: null,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class ApprovePurchaseOrderCommandHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
    ICompanyContext companyContext)
    : ICommandHandler<ApprovePurchaseOrderCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(ApprovePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        return await PurchaseOrderWorkflow.ChangeStatusAsync(
            repository,
            seriesAccessRepository,
            companyContext,
            request.Id,
            PurchaseOrderStatuses.Approved,
            "Orden aprobada correctamente.",
            [PurchaseOrderStatuses.PendingApproval],
            PurchaseOrderWorkflowPolicy.EnsureCanApprove,
            PurchaseOrderSecurity.ActionApprove,
            PurchaseOrderWorkflowActions.Approve,
            request.Observation,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class RejectPurchaseOrderCommandHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
    ICompanyContext companyContext)
    : ICommandHandler<RejectPurchaseOrderCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(RejectPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        return await PurchaseOrderWorkflow.ChangeStatusAsync(
            repository,
            seriesAccessRepository,
            companyContext,
            request.Id,
            PurchaseOrderStatuses.Rejected,
            "Orden rechazada.",
            [PurchaseOrderStatuses.PendingApproval],
            PurchaseOrderWorkflowPolicy.EnsureCanReject,
            PurchaseOrderSecurity.ActionReject,
            PurchaseOrderWorkflowActions.Reject,
            request.Observation,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

internal static class PurchaseOrderWorkflow
{
    public static async Task<Result<PurchaseOrderDto>> ChangeStatusAsync(
        IPurchaseOrderRepository repository,
        ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
        ICompanyContext companyContext,
        int id,
        string status,
        string message,
        IReadOnlyCollection<string> expectedCurrentStatuses,
        Func<string, Result<bool>> validateCurrentStatus,
        string actionKey,
        string workflowAction,
        string? observation,
        int? userId,
        string? userName,
        CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.", FailureKind.NotFound,
                [new ApiError("PURCHASE_ORDER_NOT_FOUND", "La orden de compra no existe.", "Id")]);
        }

        var validation = validateCurrentStatus(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(validation.Message, FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", validation.Message)]);
        }

        var seriesValidation = await PurchaseOrderSecurityGuard.ValidateSeriesOperationAsync(
            seriesAccessRepository,
            companyContext,
            userId,
            current.DocumentSeriesId,
            actionKey,
            cancellationToken);
        if (!seriesValidation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(seriesValidation.Message,
                seriesValidation.FailureKind ?? FailureKind.Validation, seriesValidation.Errors);
        }

        var transition = await repository.TransitionWorkflowAsync(
            new PurchaseOrderWorkflowTransitionData(
                id,
                expectedCurrentStatuses,
                status,
                workflowAction,
                ApprovalLevel: 1,
                observation,
                userId!.Value,
                userName),
            cancellationToken);
        if (transition.Outcome == PurchaseOrderWorkflowTransitionOutcome.NotFound)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.", FailureKind.NotFound,
                [new ApiError("PURCHASE_ORDER_NOT_FOUND", "La orden de compra no existe.", "Id")]);
        }

        if (transition.Outcome != PurchaseOrderWorkflowTransitionOutcome.Updated)
        {
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar el estado de la orden.", FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", "La orden ya no admite la transicion solicitada.")]);
        }

        if (transition.Order is null)
        {
            return Result<PurchaseOrderDto>.Failure(
                "No se pudo recuperar la orden de compra.",
                FailureKind.Unexpected);
        }

        return Result<PurchaseOrderDto>.Success(transition.Order, message);
    }
}
