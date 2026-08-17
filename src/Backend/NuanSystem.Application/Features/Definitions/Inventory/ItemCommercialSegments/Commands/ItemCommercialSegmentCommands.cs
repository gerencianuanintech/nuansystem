using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Commands;

public sealed record CreateItemCommercialSegmentCommand(string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemCommercialSegmentDto>;
public sealed record UpdateItemCommercialSegmentCommand(int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemCommercialSegmentDto>;
public sealed record DeleteItemCommercialSegmentCommand(int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;

