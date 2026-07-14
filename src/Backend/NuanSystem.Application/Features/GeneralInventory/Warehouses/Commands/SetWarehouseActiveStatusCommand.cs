using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed record SetWarehouseActiveStatusCommand(
    int Id,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;
