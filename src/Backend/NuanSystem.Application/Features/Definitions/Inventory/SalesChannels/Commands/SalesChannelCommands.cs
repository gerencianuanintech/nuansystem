using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Commands;

public sealed record CreateSalesChannelCommand(string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<SalesChannelDto>;
public sealed record UpdateSalesChannelCommand(int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<SalesChannelDto>;
public sealed record DeleteSalesChannelCommand(int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;


