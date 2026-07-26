using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class UpdateWarehouseCommandHandler(
    IWarehouseRepository warehouseRepository,
    ITransactionRunner transactionRunner,
    IWarehouseLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<UpdateWarehouseCommand, WarehouseDto>
{
    public async Task<Result<WarehouseDto>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var code = WarehouseCommandHelpers.NormalizeCode(request.Code);
        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await warehouseRepository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                {
                    return Result<WarehouseDto>.Failure(
                        "Ya existe una bodega con el codigo indicado.",
                        [new ApiError("WarehouseCodeAlreadyExists", "El codigo de bodega ya existe.", nameof(request.Code))]);
                }

                var current = await warehouseRepository.GetByIdAsync(request.Id, connection, transaction, token);
                if (current is null)
                {
                    return Result<WarehouseDto>.Failure(
                        "La bodega solicitada no existe.",
                        [new ApiError("WarehouseNotFound", "No se encontro la bodega.", nameof(request.Id))]);
                }

                var updated = await warehouseRepository.UpdateAsync(new UpdateWarehouseData(
                    request.Id,
                    current.GlobalId,
                    code,
                    request.Name.Trim(),
                    WarehouseCommandHelpers.NormalizeOptional(request.Description),
                    WarehouseCommandHelpers.NormalizeOptional(request.BranchCode),
                    WarehouseCommandHelpers.NormalizeOptional(request.Address),
                    WarehouseCommandHelpers.NormalizeOptional(request.City),
                    WarehouseCommandHelpers.NormalizeOptional(request.Province),
                    WarehouseCommandHelpers.NormalizeOptional(request.Country),
                    WarehouseCommandHelpers.NormalizeOptional(request.Phone),
                    WarehouseCommandHelpers.NormalizeOptional(request.Email),
                    WarehouseCommandHelpers.NormalizeOptional(request.ManagerName),
                    request.AllowsSales,
                    request.AllowsPurchases,
                    request.AllowsTransfers,
                    request.AllowsProduction,
                    request.IsDefault,
                    WarehouseCommandHelpers.NormalizeOptional(request.ExternalSystem),
                    WarehouseCommandHelpers.NormalizeOptional(request.ExternalCode),
                    WarehouseCommandHelpers.NormalizeOptional(request.SapCode),
                    request.IsActive,
                    request.AuditUserId,
                    WarehouseCommandHelpers.NormalizeOptional(request.AuditUserName)), connection, transaction, token);

                if (!updated)
                {
                    return Result<WarehouseDto>.Failure("No fue posible actualizar la bodega.");
                }

                var warehouse = await warehouseRepository.GetByIdAsync(request.Id, connection, transaction, token)
                    ?? throw new InvalidOperationException("La bodega fue actualizada pero no pudo consultarse.");
                await localOutboxWriter.EnqueueAsync(
                    warehouse,
                    warehouse.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                    connection,
                    transaction,
                    token);
                return Result<WarehouseDto>.Success(warehouse, "Bodega actualizada correctamente.");
            },
            cancellationToken);
    }
}
