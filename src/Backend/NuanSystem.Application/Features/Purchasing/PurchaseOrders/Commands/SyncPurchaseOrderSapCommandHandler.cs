using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Mapping;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class SyncPurchaseOrderSapCommandHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
    ICompanyContext companyContext,
    ISapSyncProcessorRegistry sapSyncProcessors)
    : ICommandHandler<SyncPurchaseOrderSapCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(SyncPurchaseOrderSapCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.", FailureKind.NotFound,
                [new ApiError("PURCHASE_ORDER_NOT_FOUND", "La orden de compra no existe.", nameof(request.Id))]);
        }

        if (sapSyncProcessors.FindExporter(SapSyncEntityCode.PurchaseOrders) is null)
        {
            return Result<PurchaseOrderDto>.Failure(
                "La exportacion de ordenes de compra hacia SAP no esta disponible.",
                FailureKind.Unavailable,
                [new ApiError(
                    "SAP_EXPORT_NOT_AVAILABLE",
                    "No existe un exportador ejecutable para ordenes de compra.")]);
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanRequestSapSync(current.Status);
        if (!validation.IsSuccess)
        {
            await repository.AddSapLogAsync(request.Id, "PurchaseOrderSync", "Skipped", validation.Message, request.AuditUserId, request.AuditUserName, cancellationToken);
            return Result<PurchaseOrderDto>.Failure(validation.Message, FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", validation.Message)]);
        }

        if (current.SapStatus == PurchaseOrderSapStatuses.Synced)
        {
            return Result<PurchaseOrderDto>.Failure("La orden ya fue sincronizada con SAP.", FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", "La orden ya fue sincronizada con SAP.")]);
        }

        var seriesValidation = await PurchaseOrderSecurityGuard.ValidateSeriesOperationAsync(
            seriesAccessRepository,
            companyContext,
            request.AuditUserId,
            current.DocumentSeriesId,
            PurchaseOrderSecurity.ActionSyncSap,
            cancellationToken);
        if (!seriesValidation.IsSuccess)
        {
            await repository.AddSapLogAsync(request.Id, "PurchaseOrderSync", "Skipped", seriesValidation.Message, request.AuditUserId, request.AuditUserName, cancellationToken);
            return Result<PurchaseOrderDto>.Failure(seriesValidation.Message,
                seriesValidation.FailureKind ?? FailureKind.Validation, seriesValidation.Errors);
        }

        await repository.AddSapLogAsync(request.Id, "PurchaseOrderSync", "Pending", $"Pendiente de envio a SAP Business One. ObjectType {SapPurchaseOrderMapping.ObjectType}.", request.AuditUserId, request.AuditUserName, cancellationToken);
        var updated = await repository.UpdateStatusIfCurrentAsync(
            request.Id,
            PurchaseOrderStatuses.SapPending,
            [PurchaseOrderStatuses.Approved, PurchaseOrderStatuses.SapError],
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        if (!updated)
        {
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar el estado de la orden.", FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", "La orden ya no admite la transicion solicitada.")]);
        }

        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra.", FailureKind.Unexpected)
            : Result<PurchaseOrderDto>.Success(order, "Orden marcada como pendiente de sincronizacion SAP.");
    }
}
