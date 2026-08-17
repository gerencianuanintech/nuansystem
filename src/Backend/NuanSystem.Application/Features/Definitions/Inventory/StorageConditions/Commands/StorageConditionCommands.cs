using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
namespace NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Commands;
public sealed record CreateStorageConditionCommand(string Code,string Name,string? Description,int SortOrder,bool IsActive,int? AuditUserId=null,string? AuditUserName=null):ICommand<StorageConditionDto>;
public sealed record UpdateStorageConditionCommand(int Id,string Code,string Name,string? Description,int SortOrder,bool IsActive,int? AuditUserId=null,string? AuditUserName=null):ICommand<StorageConditionDto>;
public sealed record DeleteStorageConditionCommand(int Id,int? AuditUserId=null,string? AuditUserName=null):ICommand<bool>;
