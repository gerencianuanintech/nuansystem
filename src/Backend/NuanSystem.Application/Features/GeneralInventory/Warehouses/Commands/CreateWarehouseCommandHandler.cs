using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class CreateWarehouseCommandHandler(
    IWarehouseRepository warehouseRepository,
    ISyncEventPublisher syncEventPublisher,
    ICompanyContext companyContext)
    : ICommandHandler<CreateWarehouseCommand, WarehouseDto>
{
    public async Task<Result<WarehouseDto>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var code = WarehouseCommandHelpers.NormalizeCode(request.Code);
        if (await warehouseRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<WarehouseDto>.Failure(
                "Ya existe una bodega con el codigo indicado.",
                new[] { new ApiError("WarehouseCodeAlreadyExists", "El codigo de bodega ya existe.", nameof(request.Code)) });
        }

        var id = await warehouseRepository.CreateAsync(new CreateWarehouseData(
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
            WarehouseCommandHelpers.NormalizeOptional(request.AuditUserName)), cancellationToken);

        var warehouse = await warehouseRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("La bodega fue creada pero no pudo consultarse.");

        var syncResult = await WarehouseSyncPublisher.PublishAsync(
            syncEventPublisher,
            companyContext,
            warehouse,
            SyncOperation.Created,
            cancellationToken);

        if (syncResult is { IsSuccess: false })
        {
            return Result<WarehouseDto>.Failure(syncResult.Message, syncResult.Errors);
        }

        return Result<WarehouseDto>.Success(warehouse, "Bodega creada correctamente.");
    }
}
