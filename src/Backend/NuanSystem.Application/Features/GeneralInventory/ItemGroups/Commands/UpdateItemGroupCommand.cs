using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed record UpdateItemGroupCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    string? ExternalSystem,
    string? ExternalCode,
    bool IsActive,
    string? InventoryAccountCode,
    string? IncomeAccountCode,
    string? CostOfSalesAccountCode,
    string? SalesReturnAccountCode,
    string? PurchaseReturnAccountCode,
    string? CostVarianceAccountCode,
    string? InventoryAdjustmentAccountCode,
    string? PurchaseExpenseAccountCode,
    int SortOrder,
    string? SalesAccountCode,
    string? PurchaseAccountCode,
    string? SapGroupCode,
    string? SapCode,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ItemGroupDto>
{
    public UpdateItemGroupCommand(int id, string code, string name, string? description, bool isActive,
        string? inventoryAccountCode, string? costOfSalesAccountCode, string? salesAccountCode,
        string? purchaseAccountCode, string? sapGroupCode, string? sapCode, int? auditUserId = null,
        string? auditUserName = null)
        : this(id, code, name, description, null, null, isActive, inventoryAccountCode, salesAccountCode,
            costOfSalesAccountCode, null, null, null, null, purchaseAccountCode, 0, salesAccountCode,
            purchaseAccountCode, sapGroupCode, sapCode, auditUserId, auditUserName) { }
}
