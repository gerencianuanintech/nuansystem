using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class DeletePurchaseOrderCommandHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
    ICompanyContext companyContext)
    : ICommandHandler<DeletePurchaseOrderCommand, bool>
{
    public async Task<Result<bool>> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<bool>.Failure("No se encontro la orden de compra.", FailureKind.NotFound,
                [new ApiError("PURCHASE_ORDER_NOT_FOUND", "La orden de compra no existe.", nameof(request.Id))]);
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanDelete(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<bool>.Failure(validation.Message, FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", validation.Message)]);
        }

        var seriesValidation = await PurchaseOrderSecurityGuard.ValidateSeriesOperationAsync(
            seriesAccessRepository,
            companyContext,
            request.AuditUserId,
            current.DocumentSeriesId,
            PurchaseOrderSecurity.ActionDelete,
            cancellationToken);
        if (!seriesValidation.IsSuccess)
        {
            return Result<bool>.Failure(seriesValidation.Message,
                seriesValidation.FailureKind ?? FailureKind.Validation, seriesValidation.Errors);
        }

        var deleted = await repository.DeleteIfCurrentAsync(
            request.Id,
            [PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected],
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        return deleted
            ? Result<bool>.Success(true, "Orden de compra eliminada correctamente.")
            : Result<bool>.Failure("No se pudo eliminar la orden de compra.", FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", "La orden no admite la anulacion solicitada.")]);
    }
}
