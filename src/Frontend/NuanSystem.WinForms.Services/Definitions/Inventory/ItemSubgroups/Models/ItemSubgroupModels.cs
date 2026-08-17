namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups.Models;

public sealed class ItemSubgroupItem
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
}

public sealed class ItemSubgroupLookupItem
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public int ItemFamilyId { get; set; }
    public Guid? ItemFamilyGlobalId { get; set; }
    public string ItemFamilyCode { get; set; } = string.Empty;
    public string ItemFamilyName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record SaveItemSubgroupRequest(
    int ItemFamilyId, string Code, string Name, string? Description, int SortOrder, bool IsActive);

public sealed class ItemSubgroupAuditChange
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
    public DateTime CreatedAt { get; set; }
}
