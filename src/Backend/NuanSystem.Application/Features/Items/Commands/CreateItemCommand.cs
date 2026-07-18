using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Items.Dtos;
using System.Text.Json.Serialization;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed record CreateItemCommand(
    string Code,
    string Name,
    string? Description,
    int? ItemGroupId,
    int? ItemFamilyId,
    string ItemType,
    int? InventoryUnitOfMeasureId,
    int? PurchaseUnitOfMeasureId,
    int? SalesUnitOfMeasureId,
    bool IsPurchaseItem,
    bool IsSalesItem,
    bool IsInventoryItem,
    int? PurchaseTaxId,
    int? SalesTaxId,
    string ValuationMethod,
    string ManagedBy,
    string BatchSerialManagementMethod,
    string? PreferredVendorCode,
    string? VendorCatalogCode,
    decimal BaseSalesPrice,
    decimal ReferenceCost,
    decimal PurchaseFactor,
    decimal SalesFactor,
    bool AllowDiscount,
    bool AllowSaleWithoutStock,
    string? Remarks,
    bool IsActive,
    IReadOnlyCollection<SaveItemBarcodeData>? Barcodes,
    IReadOnlyCollection<SaveItemWarehouseData>? Warehouses,
    ItemMasterData? MasterData = null,
    int? AuditUserId = null,
    string? AuditUserName = null,
    [property: JsonIgnore] Guid? GlobalId = null,
    [property: JsonIgnore] string? ExternalSystem = null,
    [property: JsonIgnore] string? ExternalCode = null,
    [property: JsonIgnore] string? SapCode = null,
    [property: JsonIgnore] bool IsExternalImport = false) : ICommand<ItemDto>;
