using System.Text.Json.Serialization;

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
    public string? IncomeAccountCode { get; set; }
    public string? CostOfSalesAccountCode { get; set; }
    public string? SalesReturnAccountCode { get; set; }
    public string? PurchaseReturnAccountCode { get; set; }
    public string? CostVarianceAccountCode { get; set; }
    public string? InventoryAdjustmentAccountCode { get; set; }
    public string? PurchaseExpenseAccountCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    // Aliases transitorios para payloads/despliegues anteriores.
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

[method: JsonConstructor]
public sealed record ItemGroupSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    string? InventoryAccountCode,
    string? IncomeAccountCode,
    string? CostOfSalesAccountCode,
    string? SalesReturnAccountCode,
    string? PurchaseReturnAccountCode,
    string? CostVarianceAccountCode,
    string? InventoryAdjustmentAccountCode,
    string? PurchaseExpenseAccountCode,
    int SortOrder,
    bool IsSystem,
    string? SalesAccountCode,
    string? PurchaseAccountCode,
    string? SapGroupCode,
    string? SapCode,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public ItemGroupSyncPayload(Guid globalId, string code, string name, string? description,
        string? inventoryAccountCode, string? costOfSalesAccountCode, string? salesAccountCode,
        string? purchaseAccountCode, string? sapGroupCode, string? sapCode, bool isActive,
        string? externalSystem, string? externalCode, DateTime createdAt, DateTime? updatedAt)
        : this(globalId, code, name, description, inventoryAccountCode, salesAccountCode,
            costOfSalesAccountCode, null, null, null, null, purchaseAccountCode, 0, false,
            salesAccountCode, purchaseAccountCode, sapGroupCode, sapCode, isActive,
            externalSystem, externalCode, createdAt, updatedAt) { }
}

public sealed class ItemGroupLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ItemGroupAuditChangeDto
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
