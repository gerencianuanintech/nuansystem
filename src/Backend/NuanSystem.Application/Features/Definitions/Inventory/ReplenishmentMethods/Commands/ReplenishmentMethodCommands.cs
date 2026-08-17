using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Commands;

public sealed record CreateReplenishmentMethodCommand(string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ReplenishmentMethodDto>;
public sealed record UpdateReplenishmentMethodCommand(int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ReplenishmentMethodDto>;
public sealed record DeleteReplenishmentMethodCommand(int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;
