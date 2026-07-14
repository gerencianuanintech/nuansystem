using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Queries;

public sealed record GetWarehouseByIdQuery(int Id) : IQuery<WarehouseDto>;
