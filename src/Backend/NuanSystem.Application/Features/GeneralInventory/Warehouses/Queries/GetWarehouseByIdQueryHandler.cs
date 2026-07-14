using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Queries;

public sealed class GetWarehouseByIdQueryHandler(IWarehouseRepository warehouseRepository)
    : IQueryHandler<GetWarehouseByIdQuery, WarehouseDto>
{
    public async Task<Result<WarehouseDto>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse is null)
        {
            return Result<WarehouseDto>.Failure(
                "La bodega solicitada no existe.",
                new[] { new ApiError("WarehouseNotFound", "No se encontro la bodega.", nameof(request.Id)) });
        }

        return Result<WarehouseDto>.Success(warehouse);
    }
}
