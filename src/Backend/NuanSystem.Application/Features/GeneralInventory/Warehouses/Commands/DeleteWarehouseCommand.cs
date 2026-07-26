using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed record DeleteWarehouseCommand(
    int Id,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;
