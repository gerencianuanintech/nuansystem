namespace NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;

public sealed class ItemSubgroupDto
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public int ItemFamilyId { get; set; }
    public Guid? ItemFamilyGlobalId { get; set; }
    public string ItemFamilyCode { get; set; } = string.Empty;
    public string ItemFamilyName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public sealed class ItemSubgroupLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public int ItemFamilyId { get; set; }
    public Guid ItemFamilyGlobalId { get; set; }
    public string ItemFamilyCode { get; set; } = string.Empty;
    public string ItemFamilyName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ItemSubgroupAuditChangeDto
{
    public long Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed record CreateItemSubgroupData(Guid GlobalId, int ItemFamilyId, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? CreatedByUserId, string? CreatedByUserName);
public sealed record UpdateItemSubgroupData(int Id, int ItemFamilyId, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? UpdatedByUserId, string? UpdatedByUserName);
public sealed record ItemSubgroupSyncPayload(
    Guid GlobalId,
    Guid ItemFamilyGlobalId,
    string ItemFamilyCode,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
