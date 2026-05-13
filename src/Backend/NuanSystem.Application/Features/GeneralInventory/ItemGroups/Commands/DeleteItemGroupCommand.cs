using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed record DeleteItemGroupCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;
