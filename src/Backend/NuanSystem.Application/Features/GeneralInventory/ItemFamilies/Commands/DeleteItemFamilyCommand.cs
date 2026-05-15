using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed record DeleteItemFamilyCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;
