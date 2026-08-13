namespace NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;

public sealed class ItemTypeDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BehaviorCode { get; set; } = string.Empty;
    public bool DefaultIsPurchaseItem { get; set; }
    public bool DefaultIsSalesItem { get; set; }
    public bool DefaultIsInventoryItem { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
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

public sealed class ItemTypeLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BehaviorCode { get; set; } = string.Empty;
    public bool DefaultIsPurchaseItem { get; set; }
    public bool DefaultIsSalesItem { get; set; }
    public bool DefaultIsInventoryItem { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ItemTypeAuditChangeDto
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

public sealed record CreateItemTypeData(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    string BehaviorCode,
    bool DefaultIsPurchaseItem,
    bool DefaultIsSalesItem,
    bool DefaultIsInventoryItem,
    int SortOrder,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdateItemTypeData(
    int Id,
    string Code,
    string Name,
    string? Description,
    string BehaviorCode,
    bool DefaultIsPurchaseItem,
    bool DefaultIsSalesItem,
    bool DefaultIsInventoryItem,
    int SortOrder,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);

public sealed record DeleteItemTypeData(int Id, int? DeletedByUserId, string? DeletedByUserName);

public sealed record CreateItemTypeResult(int? Id, bool DuplicateCode);
public sealed record UpdateItemTypeResult(bool Updated, bool DuplicateCode, bool SystemProtected);
public sealed record DeleteItemTypeResult(bool Deleted, bool SystemProtected, bool InUse);
