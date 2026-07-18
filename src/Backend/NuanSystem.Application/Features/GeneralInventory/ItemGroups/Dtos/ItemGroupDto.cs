namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

public sealed class ItemGroupDto
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? InventoryAccountCode { get; set; }
    public string? CostOfSalesAccountCode { get; set; }
    public string? SalesAccountCode { get; set; }
    public string? PurchaseAccountCode { get; set; }
    public string? SapGroupCode { get; set; }
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

public sealed record ItemGroupSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    string? InventoryAccountCode,
    string? CostOfSalesAccountCode,
    string? SalesAccountCode,
    string? PurchaseAccountCode,
    string? SapGroupCode,
    string? SapCode,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
