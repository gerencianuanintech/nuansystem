using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed record CreateWarehouseCommand(
    Guid? GlobalId,
    string Code,
    string Name,
    string? Description,
    string? BranchCode,
    string? Address,
    string? City,
    string? Province,
    string? Country,
    string? Phone,
    string? Email,
    string? ManagerName,
    bool AllowsSales,
    bool AllowsPurchases,
    bool AllowsTransfers,
    bool AllowsProduction,
    bool IsDefault,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapCode,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<WarehouseDto>;
