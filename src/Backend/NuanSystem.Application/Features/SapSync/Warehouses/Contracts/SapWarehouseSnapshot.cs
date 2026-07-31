using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Warehouses.Contracts;

public sealed record SapWarehouseSnapshot(
    string WarehouseCode,
    string WarehouseName,
    string? Street,
    string? City,
    string? Province,
    string? Country,
    bool IsActive)
{
    public static SapWarehouseSnapshot FromRecord(SapWarehouseRecord record) =>
        new(record.WarehouseCode, record.WarehouseName, record.Street, record.City,
            record.Province, record.Country, record.IsActive);
}
