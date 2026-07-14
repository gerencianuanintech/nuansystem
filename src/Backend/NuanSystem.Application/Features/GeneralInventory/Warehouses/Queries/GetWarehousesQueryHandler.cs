using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Queries;

public sealed class GetWarehousesQueryHandler(IWarehouseRepository warehouseRepository)
    : IQueryHandler<GetWarehousesQuery, IReadOnlyCollection<WarehouseDto>>
{
    public async Task<Result<IReadOnlyCollection<WarehouseDto>>> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = await warehouseRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<WarehouseDto>>.Success(warehouses);
    }
}
