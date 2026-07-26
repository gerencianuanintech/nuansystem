using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class DeleteWarehouseCommandHandler(
    IWarehouseRepository warehouseRepository,
    ITransactionRunner transactionRunner,
    IWarehouseLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteWarehouseCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        return transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var current = await warehouseRepository.GetByIdAsync(request.Id, connection, transaction, token);
                if (current is null)
                {
                    return Result<bool>.Failure(
                        "La bodega solicitada no existe.",
                        [new ApiError("WarehouseNotFound", "No se encontro la bodega.", nameof(request.Id))]);
                }

                var deleted = await warehouseRepository.DeleteAsync(
                    request.Id,
                    request.AuditUserId,
                    WarehouseCommandHelpers.NormalizeOptional(request.AuditUserName),
                    connection,
                    transaction,
                    token);

                if (!deleted)
                {
                    return Result<bool>.Failure("No fue posible eliminar la bodega.");
                }

                await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Bodega eliminada correctamente.");
            },
            cancellationToken);
    }
}
