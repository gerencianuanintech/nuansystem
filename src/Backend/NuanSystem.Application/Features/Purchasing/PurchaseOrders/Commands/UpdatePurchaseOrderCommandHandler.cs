using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class UpdatePurchaseOrderCommandHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
    ICompanyContext companyContext)
    : ICommandHandler<UpdatePurchaseOrderCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.", FailureKind.NotFound,
                [new ApiError("PURCHASE_ORDER_NOT_FOUND", "La orden de compra no existe.", nameof(request.Id))]);
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanEdit(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(validation.Message, FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", validation.Message)]);
        }

        var seriesValidation = await PurchaseOrderSecurityGuard.ValidateSeriesOperationAsync(
            seriesAccessRepository,
            companyContext,
            request.AuditUserId,
            current.DocumentSeriesId,
            PurchaseOrderSecurity.ActionUpdate,
            cancellationToken);
        if (!seriesValidation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(seriesValidation.Message,
                seriesValidation.FailureKind ?? FailureKind.Validation, seriesValidation.Errors);
        }

        if (string.IsNullOrWhiteSpace(current.SeriesCode) || string.IsNullOrWhiteSpace(current.DocumentNumber))
        {
            return PurchaseOrderNumberingFailure.Missing();
        }

        var saveRequest = request.ToRequest() with
        {
            DocumentSeriesId = current.DocumentSeriesId,
            SeriesCode = current.SeriesCode,
            DocumentNumber = current.DocumentNumber
        };

        var data = PurchaseOrderCalculator.BuildPersistData(
            request.Id,
            saveRequest,
            current.Status,
            request.AuditUserId,
            request.AuditUserName,
            current.SapStatus);

        var updated = await repository.UpdateIfEditableAsync(
            data,
            [PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected, PurchaseOrderStatuses.SapError],
            cancellationToken);
        if (!updated)
        {
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar la orden de compra.", FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", "La orden no admite la actualizacion solicitada.")]);
        }

        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra actualizada.", FailureKind.Unexpected)
            : Result<PurchaseOrderDto>.Success(order, "Orden de compra actualizada correctamente.");
    }
}
