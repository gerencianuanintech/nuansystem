using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class CreateWarehouseCommandHandler(
    IWarehouseRepository warehouseRepository,
    ITransactionRunner transactionRunner,
    IWarehouseLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<CreateWarehouseCommand, WarehouseDto>
{
    public async Task<Result<WarehouseDto>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var code = WarehouseCommandHelpers.NormalizeCode(request.Code);
        var data = new CreateWarehouseData(
            request.GlobalId.GetValueOrDefault(Guid.NewGuid()),
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
            WarehouseCommandHelpers.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await warehouseRepository.ExistsByCodeAsync(code, null, connection, transaction, token))
                {
                    return Result<WarehouseDto>.Failure(
                        "Ya existe una bodega con el codigo indicado.",
                        [new ApiError("WarehouseCodeAlreadyExists", "El codigo de bodega ya existe.", nameof(request.Code))]);
                }

                var id = await warehouseRepository.CreateAsync(data, connection, transaction, token);
                var warehouse = await warehouseRepository.GetByIdAsync(id, connection, transaction, token)
                    ?? throw new InvalidOperationException("La bodega fue creada pero no pudo consultarse.");
                await localOutboxWriter.EnqueueAsync(warehouse, SyncOperation.Created, connection, transaction, token);
                return Result<WarehouseDto>.Success(warehouse, "Bodega creada correctamente.");
            },
            cancellationToken);
    }
}
