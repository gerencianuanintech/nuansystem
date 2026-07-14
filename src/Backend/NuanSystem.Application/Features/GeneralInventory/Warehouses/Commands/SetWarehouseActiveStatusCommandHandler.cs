using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class SetWarehouseActiveStatusCommandHandler(
    IWarehouseRepository warehouseRepository,
    ISyncEventPublisher syncEventPublisher,
    ICompanyContext companyContext)
    : ICommandHandler<SetWarehouseActiveStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(SetWarehouseActiveStatusCommand request, CancellationToken cancellationToken)
    {
        var current = await warehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<bool>.Failure(
                "La bodega solicitada no existe.",
                new[] { new ApiError("WarehouseNotFound", "No se encontro la bodega.", nameof(request.Id)) });
        }

        var updated = await warehouseRepository.SetActiveStatusAsync(
            request.Id,
            request.IsActive,
            request.AuditUserId,
            WarehouseCommandHelpers.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        if (!updated)
        {
            return Result<bool>.Failure(
                "La bodega solicitada no existe.",
                new[] { new ApiError("WarehouseNotFound", "No se encontro la bodega.", nameof(request.Id)) });
        }

        var warehouse = await warehouseRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("La bodega fue actualizada pero no pudo consultarse.");

        var syncResult = await WarehouseSyncPublisher.PublishAsync(
            syncEventPublisher,
            companyContext,
            warehouse,
            request.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
            cancellationToken);

        if (syncResult is { IsSuccess: false })
        {
            return Result<bool>.Failure(syncResult.Message, syncResult.Errors);
        }

        return Result<bool>.Success(true, request.IsActive ? "Bodega activada correctamente." : "Bodega inactivada correctamente.");
    }
}
