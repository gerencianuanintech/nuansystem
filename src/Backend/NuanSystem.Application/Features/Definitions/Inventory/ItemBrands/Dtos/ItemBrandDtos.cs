namespace NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;

public sealed class ItemBrandDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapManufacturerCode { get; set; }
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

public sealed class ItemBrandLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ItemBrandAuditChangeDto
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

public sealed record CreateItemBrandData(
    Guid GlobalId, string Code, string Name, string? Description, int SortOrder, bool IsActive,
    string? ExternalSystem, string? ExternalCode, string? SapManufacturerCode, string? SapCode,
    int? CreatedByUserId, string? CreatedByUserName);

public sealed record UpdateItemBrandData(
    int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive,
    string? ExternalSystem, string? ExternalCode, string? SapManufacturerCode, string? SapCode,
    int? UpdatedByUserId, string? UpdatedByUserName);

public sealed record ItemBrandSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    bool IsDeleted,
    DateTime UpdatedAt);
