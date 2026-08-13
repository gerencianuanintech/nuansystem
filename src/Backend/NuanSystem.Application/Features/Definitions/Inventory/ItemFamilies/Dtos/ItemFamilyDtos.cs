namespace NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;

public sealed class ItemFamilyDto
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public int ItemGroupId { get; set; }
    public Guid? ItemGroupGlobalId { get; set; }
    public string ItemGroupCode { get; set; } = string.Empty;
    public string ItemGroupName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapFamilyCode { get; set; }
    public string? SapCode { get; set; }
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

public sealed class ItemFamilyLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public int ItemGroupId { get; set; }
    public Guid ItemGroupGlobalId { get; set; }
    public string ItemGroupCode { get; set; } = string.Empty;
    public string ItemGroupName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ItemFamilyAuditChangeDto
{
    public string RecordId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed record CreateItemFamilyData(
    Guid GlobalId,
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
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdateItemFamilyData(
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
    int? UpdatedByUserId,
    string? UpdatedByUserName);

public sealed record ItemFamilySyncPayload(
    Guid GlobalId,
    Guid ItemGroupGlobalId,
    string ItemGroupCode,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? SapFamilyCode,
    string? SapCode,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int SortOrder = 0);
