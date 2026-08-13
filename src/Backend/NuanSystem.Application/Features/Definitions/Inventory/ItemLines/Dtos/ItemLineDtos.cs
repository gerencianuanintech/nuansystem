namespace NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;

public sealed class ItemLineDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
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

public sealed class ItemLineLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ItemLineAuditChangeDto
{
    public long Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Source { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed record CreateItemLineData(
    Guid GlobalId, string Code, string Name, string? Description, int SortOrder, bool IsActive,
    int? CreatedByUserId, string? CreatedByUserName);

public sealed record UpdateItemLineData(
    int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive,
    int? UpdatedByUserId, string? UpdatedByUserName);

public sealed record ItemLineSyncPayload(
    Guid GlobalId, string Code, string Name, string? Description, int SortOrder,
    bool IsActive, bool IsDeleted, DateTime UpdatedAt);
