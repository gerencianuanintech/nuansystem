using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class SetWarehouseActiveStatusCommandHandler(
    IWarehouseRepository warehouseRepository,
    ITransactionRunner transactionRunner,
    IWarehouseLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<SetWarehouseActiveStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(SetWarehouseActiveStatusCommand request, CancellationToken cancellationToken)
    {
        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await warehouseRepository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                {
                    return Result<bool>.Failure(
                        "La bodega solicitada no existe.",
                        [new ApiError("WarehouseNotFound", "No se encontro la bodega.", nameof(request.Id))]);
                }

                var updated = await warehouseRepository.SetActiveStatusAsync(
                    request.Id,
                    request.IsActive,
                    request.AuditUserId,
                    WarehouseCommandHelpers.NormalizeOptional(request.AuditUserName),
                    connection,
                    transaction,
                    token);

                if (!updated)
                {
                    return Result<bool>.Failure("No fue posible cambiar el estado de la bodega.");
                }

                var warehouse = await warehouseRepository.GetByIdAsync(request.Id, connection, transaction, token)
                    ?? throw new InvalidOperationException("La bodega fue actualizada pero no pudo consultarse.");
                await localOutboxWriter.EnqueueAsync(
                    warehouse,
                    request.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                    connection,
                    transaction,
                    token);
                return Result<bool>.Success(true, request.IsActive ? "Bodega activada correctamente." : "Bodega inactivada correctamente.");
            },
            cancellationToken);
    }
}
