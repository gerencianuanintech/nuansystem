using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Commands;

public sealed record CreateItemFamilyCommand(
    int ItemGroupId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapFamilyCode,
    string? SapCode,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ItemFamilyDto>
{
    public CreateItemFamilyCommand(int itemGroupId, string code, string name, string? description,
        bool isActive, string? sapFamilyCode, string? sapCode, int? auditUserId = null,
        string? auditUserName = null)
        : this(itemGroupId, code, name, description, 0, isActive, null, null,
            sapFamilyCode, sapCode, auditUserId, auditUserName) { }
}

public sealed record UpdateItemFamilyCommand(
    int Id,
    int ItemGroupId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapFamilyCode,
    string? SapCode,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ItemFamilyDto>
{
    public UpdateItemFamilyCommand(int id, int itemGroupId, string code, string name,
        string? description, bool isActive, string? sapFamilyCode, string? sapCode,
        int? auditUserId = null, string? auditUserName = null)
        : this(id, itemGroupId, code, name, description, 0, isActive, null, null,
            sapFamilyCode, sapCode, auditUserId, auditUserName) { }
}

public sealed record DeleteItemFamilyCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;
