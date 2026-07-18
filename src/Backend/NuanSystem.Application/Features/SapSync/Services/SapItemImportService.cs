using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Items.Commands;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapItemImportService(
    ISapItemReader sapItemReader,
    IItemRepository itemRepository,
    ISapCatalogMappingRepository catalogMappingRepository,
    ISapSyncLogRepository sapSyncLogRepository,
    ISender sender,
    ILogger<SapItemImportService> logger) : ISapItemImportService
{
    private const string SapExternalSystem = "SAP_B1";
    private const int MaxReturnedDetails = 200;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<SapItemPreviewItemDto>> PreviewAsync(
        int companyId,
        int take,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var sapItems = await sapItemReader.GetItemsAsync(
            companyId,
            new SapItemReadOptions(Math.Clamp(take, 1, 1000), search),
            cancellationToken);
        var localItems = await itemRepository.GetAllAsync(cancellationToken);
        var index = BuildIndex(localItems);
        var normalizedSearch = NormalizeOptional(search);

        return sapItems
            .Where(item => normalizedSearch is null
                || item.ItemCode.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase)
                || item.ItemName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.ItemCode)
            .Take(Math.Clamp(take, 1, 1000))
            .Select(item => BuildPreviewItem(item, index))
            .ToArray();
    }

    public async Task<SapItemImportResultDto> ImportAsync(
        int companyId,
        IReadOnlyCollection<string>? sapItemCodes,
        int? auditUserId,
        string? auditUserName,
        bool writePublicSapLog = true,
        CancellationToken cancellationToken = default)
    {
        var sapItems = await sapItemReader.GetItemsAsync(
            companyId,
            new SapItemReadOptions(ItemCodes: sapItemCodes),
            cancellationToken);
        var selectedCodes = sapItemCodes is { Count: > 0 }
            ? sapItemCodes.Select(NormalizeKey).ToHashSet(StringComparer.OrdinalIgnoreCase)
            : null;
        var selectedItems = selectedCodes is null
            ? sapItems.OrderBy(item => item.ItemCode).ToArray()
            : sapItems.Where(item => selectedCodes.Contains(NormalizeKey(item.ItemCode))).OrderBy(item => item.ItemCode).ToArray();

        var localItems = await itemRepository.GetAllAsync(cancellationToken);
        var lookups = await itemRepository.GetLookupsAsync(cancellationToken);
        var mappings = await catalogMappingRepository.GetByCompanyIdAsync(companyId, cancellationToken);
        var index = BuildIndex(localItems);
        var results = new List<SapItemImportItemResultDto>();

        foreach (var sapItem in selectedItems)
        {
            var result = await ImportOneAsync(sapItem, lookups, mappings, index, auditUserId, auditUserName, cancellationToken);
            results.Add(result);

            if (result.LocalItemId is not null && result.Status is "Created" or "Updated")
            {
                var refreshed = await itemRepository.GetByIdAsync(result.LocalItemId.Value, cancellationToken);
                if (refreshed is not null)
                {
                    AddToIndex(index, refreshed);
                }
            }
        }

        if (selectedCodes is not null)
        {
            var readCodes = sapItems.Select(item => NormalizeKey(item.ItemCode)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var missing in selectedCodes.Where(code => !readCodes.Contains(code)).OrderBy(code => code))
            {
                results.Add(new SapItemImportItemResultDto(missing, string.Empty, "Skipped", "El articulo seleccionado no existe en la respuesta actual de SAP.", null));
            }
        }

        var summary = new SapItemImportResultDto(
            sapItems.Count,
            selectedItems.Length,
            results.Count(item => item.Status == "Created"),
            results.Count(item => item.Status == "Updated"),
            results.Count(item => item.Status == "Unchanged"),
            results.Count(item => item.Status is "Skipped" or "Conflict"),
            results.Count(item => item.Status == "Failed"),
            results.Count > MaxReturnedDetails,
            results.Take(MaxReturnedDetails).ToArray());

        if (writePublicSapLog)
        {
            await WritePublicLogAsync(companyId, summary, cancellationToken);
        }

        return summary;
    }

    private async Task<SapItemImportItemResultDto> ImportOneAsync(
        SapItemRecord sap,
        ItemLookupsDto lookups,
        IReadOnlyCollection<SapCatalogMappingDto> mappings,
        LocalItemIndex index,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken)
    {
        var code = Normalize(sap.ItemCode).ToUpperInvariant();
        var name = Normalize(sap.ItemName);
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            return ToResult(sap, "Skipped", "El articulo SAP no tiene codigo o nombre.", null);
        }

        var match = FindMatch(code, index);
        if (match.IsConflict)
        {
            return ToResult(sap, "Conflict", match.Message, match.Item?.Id);
        }

        try
        {
            var mapped = MapReferences(sap, lookups, mappings);
            if (match.Item is null)
            {
                var createResult = await sender.Send(BuildCreateCommand(sap, mapped, auditUserId, auditUserName), cancellationToken);
                return createResult.IsSuccess && createResult.Value is not null
                    ? ToResult(sap, "Created", BuildReferenceMessage("Articulo creado desde SAP.", mapped), createResult.Value.Id)
                    : ToResult(sap, "Failed", createResult.Message, null);
            }

            var local = match.Item;
            if (!HasRelevantChanges(sap, mapped, local))
            {
                var message = sap.IsActive == local.IsActive
                    ? "El articulo local ya esta actualizado."
                    : "Los datos ya estan actualizados; el cambio de estado SAP queda pendiente de aprobacion manual.";
                return ToResult(sap, "Unchanged", BuildReferenceMessage(message, mapped), local.Id);
            }

            var updateResult = await sender.Send(BuildUpdateCommand(sap, mapped, local, auditUserId, auditUserName), cancellationToken);
            var statusMessage = sap.IsActive == local.IsActive
                ? "Articulo actualizado desde SAP."
                : "Articulo actualizado; el cambio de estado SAP queda pendiente de aprobacion manual.";

            return updateResult.IsSuccess && updateResult.Value is not null
                ? ToResult(sap, "Updated", BuildReferenceMessage(statusMessage, mapped), updateResult.Value.Id)
                : ToResult(sap, "Failed", updateResult.Message, local.Id);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Fallo al importar articulo SAP {SapItemCode}.",
                code);
            return ToResult(sap, "Failed", $"No fue posible importar el articulo: {exception.GetType().Name}.", match.Item?.Id);
        }
    }

    private static CreateItemCommand BuildCreateCommand(
        SapItemRecord sap,
        MappedReferences mapped,
        int? auditUserId,
        string? auditUserName)
        => new(
            Normalize(sap.ItemCode).ToUpperInvariant(), Normalize(sap.ItemName), "Importado desde SAP Business One.",
            mapped.ItemGroupId, null, MapItemType(sap.ItemType), mapped.InventoryUnitId, mapped.PurchaseUnitId, mapped.SalesUnitId,
            sap.IsPurchaseItem, sap.IsSalesItem, sap.IsInventoryItem, mapped.PurchaseTaxId, mapped.SalesTaxId,
            "MovingAverage", MapManagedBy(sap), "EveryTransaction", null, null, 0, 0, 1, 1, true, false, null,
            sap.IsActive, BuildInitialBarcodes(sap, mapped.InventoryUnitId), [], null, auditUserId, auditUserName,
            null, SapExternalSystem, Normalize(sap.ItemCode), Normalize(sap.ItemCode), true);

    private static UpdateItemCommand BuildUpdateCommand(
        SapItemRecord sap,
        MappedReferences mapped,
        ItemDto local,
        int? auditUserId,
        string? auditUserName)
        => new(
            local.Id, local.Code, Normalize(sap.ItemName), local.Description,
            mapped.ItemGroupId ?? local.ItemGroupId, local.ItemFamilyId, MapItemType(sap.ItemType),
            mapped.InventoryUnitId ?? local.InventoryUnitOfMeasureId,
            mapped.PurchaseUnitId ?? local.PurchaseUnitOfMeasureId,
            mapped.SalesUnitId ?? local.SalesUnitOfMeasureId,
            sap.IsPurchaseItem, sap.IsSalesItem, sap.IsInventoryItem,
            mapped.PurchaseTaxId ?? local.PurchaseTaxId, mapped.SalesTaxId ?? local.SalesTaxId,
            local.ValuationMethod, MapManagedBy(sap), local.BatchSerialManagementMethod,
            local.PreferredVendorCode, local.VendorCatalogCode, local.BaseSalesPrice, local.ReferenceCost,
            local.PurchaseFactor, local.SalesFactor, local.AllowDiscount, local.AllowSaleWithoutStock,
            local.Remarks, local.IsActive, ToSaveBarcodes(local.Barcodes), ToSaveWarehouses(local.Warehouses),
            local.MasterData, auditUserId, auditUserName, SapExternalSystem, Normalize(sap.ItemCode), Normalize(sap.ItemCode), true);

    private async Task WritePublicLogAsync(int companyId, SapItemImportResultDto summary, CancellationToken cancellationToken)
    {
        var message = $"Articulos SAP procesados: {summary.Selected}; creados: {summary.Created}; actualizados: {summary.Updated}; sin cambios: {summary.Unchanged}; omitidos/conflictos: {summary.Skipped}; fallidos: {summary.Failed}.";
        await sapSyncLogRepository.CreateAsync(new CreateSapSyncLogData(
            companyId, "Item", "Items", "Items", null, JsonSerializer.Serialize(summary, JsonOptions),
            summary.Failed > 0 ? "Failed" : "Succeeded", summary.Failed > 0 ? message : null,
            null, null, DateTime.UtcNow), cancellationToken);
    }

    private static SapItemPreviewItemDto BuildPreviewItem(SapItemRecord sap, LocalItemIndex index)
    {
        var match = FindMatch(sap.ItemCode, index);
        if (match.IsConflict)
            return ToPreview(sap, "Conflict", "Conflicto", match.Item, match.Message);
        if (match.Item is null)
            return ToPreview(sap, "New", "Nuevo", null, null);

        var differences = BuildBasicDifferences(sap, match.Item);
        return ToPreview(sap, differences.Count == 0 ? "Existing" : "Different",
            differences.Count == 0 ? "Existente" : "Diferente", match.Item,
            differences.Count == 0 ? null : string.Join(" | ", differences));
    }

    private static LocalMatch FindMatch(string code, LocalItemIndex index)
    {
        index.BySapCode.TryGetValue(NormalizeKey(code), out var sapMatches);
        if (sapMatches is { Count: > 1 }) return new(null, true, "Existe mas de un articulo local con el mismo codigo SAP.");
        if (sapMatches is { Count: 1 }) return new(sapMatches[0], false, "Articulo relacionado por codigo SAP.");
        index.ByCode.TryGetValue(NormalizeKey(code), out var codeMatches);
        return codeMatches is { Count: > 0 }
            ? new(codeMatches[0], true, "Existe un articulo con el mismo codigo, pero sin relacion SAP confirmada.")
            : new(null, false, "Articulo nuevo.");
    }

    private static LocalItemIndex BuildIndex(IReadOnlyCollection<ItemDto> items)
    {
        var index = new LocalItemIndex(new(StringComparer.OrdinalIgnoreCase), new(StringComparer.OrdinalIgnoreCase));
        foreach (var item in items) AddToIndex(index, item);
        return index;
    }

    private static void AddToIndex(LocalItemIndex index, ItemDto item)
    {
        Add(index.ByCode, item.Code, item);
        Add(index.BySapCode, item.SapCode, item);
    }

    private static void Add(Dictionary<string, List<ItemDto>> dictionary, string? value, ItemDto item)
    {
        var key = NormalizeKey(value);
        if (key.Length == 0) return;
        if (!dictionary.TryGetValue(key, out var rows)) dictionary[key] = rows = [];
        rows.RemoveAll(row => row.Id == item.Id);
        rows.Add(item);
    }

    private static MappedReferences MapReferences(
        SapItemRecord sap,
        ItemLookupsDto lookups,
        IReadOnlyCollection<SapCatalogMappingDto> mappings)
    {
        string? Resolve(string type, string? sapCode) => mappings.FirstOrDefault(row => row.IsActive
            && EqualsCode(row.MappingType, type) && EqualsCode(row.SapCode, sapCode))?.NuanCode ?? sapCode;
        int? FindUnit(string? code) => lookups.UnitOfMeasures.FirstOrDefault(item => EqualsCode(item.Code, Resolve(SapCatalogMappingTypes.UnitOfMeasure, code)))?.Id;
        int? FindTax(string? code) => lookups.Taxes.FirstOrDefault(item => EqualsCode(item.Code, Resolve(SapCatalogMappingTypes.Tax, code)))?.Id;
        var groupCode = sap.ItemGroupCode?.ToString();
        var resolvedGroupCode = Resolve(SapCatalogMappingTypes.ItemGroup, groupCode);
        return new(
            lookups.ItemGroups.FirstOrDefault(item => EqualsCode(item.Code, resolvedGroupCode))?.Id,
            FindUnit(sap.InventoryUnitCode), FindUnit(sap.PurchaseUnitCode), FindUnit(sap.SalesUnitCode),
            FindTax(sap.PurchaseTaxCode), FindTax(sap.SalesTaxCode),
            groupCode is not null && !lookups.ItemGroups.Any(item => EqualsCode(item.Code, resolvedGroupCode)),
            sap.InventoryUnitCode is not null && FindUnit(sap.InventoryUnitCode) is null,
            sap.PurchaseTaxCode is not null && FindTax(sap.PurchaseTaxCode) is null,
            sap.SalesTaxCode is not null && FindTax(sap.SalesTaxCode) is null);
    }

    private static bool HasRelevantChanges(SapItemRecord sap, MappedReferences mapped, ItemDto local)
        => BuildBasicDifferences(sap, local).Any(value => value != "Estado activo (requiere aprobacion)")
           || mapped.ItemGroupId is not null && mapped.ItemGroupId != local.ItemGroupId
           || mapped.InventoryUnitId is not null && mapped.InventoryUnitId != local.InventoryUnitOfMeasureId
           || mapped.PurchaseUnitId is not null && mapped.PurchaseUnitId != local.PurchaseUnitOfMeasureId
           || mapped.SalesUnitId is not null && mapped.SalesUnitId != local.SalesUnitOfMeasureId
           || mapped.PurchaseTaxId is not null && mapped.PurchaseTaxId != local.PurchaseTaxId
           || mapped.SalesTaxId is not null && mapped.SalesTaxId != local.SalesTaxId
           || !EqualsCode(local.ExternalSystem, SapExternalSystem) || !EqualsCode(local.ExternalCode, sap.ItemCode)
           || !EqualsCode(local.SapCode, sap.ItemCode);

    private static List<string> BuildBasicDifferences(SapItemRecord sap, ItemDto local)
    {
        var values = new List<string>();
        if (!string.Equals(Normalize(sap.ItemName), Normalize(local.Name), StringComparison.OrdinalIgnoreCase)) values.Add("Nombre");
        if (sap.IsPurchaseItem != local.IsPurchaseItem) values.Add("Articulo de compra");
        if (sap.IsSalesItem != local.IsSalesItem) values.Add("Articulo de venta");
        if (sap.IsInventoryItem != local.IsInventoryItem) values.Add("Articulo de inventario");
        if (MapManagedBy(sap) != local.ManagedBy) values.Add("Manejo por lote/serie");
        if (MapItemType(sap.ItemType) != local.ItemType) values.Add("Tipo");
        if (sap.IsActive != local.IsActive) values.Add("Estado activo (requiere aprobacion)");
        return values;
    }

    private static string BuildReferenceMessage(string message, MappedReferences mapped)
    {
        var missing = new List<string>();
        if (mapped.GroupMissing) missing.Add("grupo");
        if (mapped.InventoryUnitMissing) missing.Add("unidad de inventario");
        if (mapped.PurchaseTaxMissing) missing.Add("impuesto de compra");
        if (mapped.SalesTaxMissing) missing.Add("impuesto de venta");
        return missing.Count == 0 ? message : $"{message} Sin equivalencia local para: {string.Join(", ", missing)}.";
    }

    private static IReadOnlyCollection<SaveItemBarcodeData> BuildInitialBarcodes(SapItemRecord sap, int? unitId)
        => string.IsNullOrWhiteSpace(sap.Barcode) ? [] : [new(Normalize(sap.Barcode), unitId, "SAP", 1, true, true)];
    private static IReadOnlyCollection<SaveItemBarcodeData> ToSaveBarcodes(IReadOnlyCollection<ItemBarcodeDto> rows)
        => rows.Select(row => new SaveItemBarcodeData(row.Barcode, row.UnitOfMeasureId, row.BarcodeType, row.ConversionFactor, row.IsMain, row.IsActive)).ToArray();
    private static IReadOnlyCollection<SaveItemWarehouseData> ToSaveWarehouses(IReadOnlyCollection<ItemWarehouseDto> rows)
        => rows.Select(row => new SaveItemWarehouseData(row.WarehouseId, row.MinimumStock, row.MaximumStock, row.RequiredStock, row.ReorderPoint, row.DefaultLocationCode, row.WarehouseCost, row.IsDefaultWarehouse, row.IsLocked, row.IsActive)).ToArray();
    private static string MapItemType(string value) => value.Equals("itItems", StringComparison.OrdinalIgnoreCase) ? "Product" : "Service";
    private static string MapManagedBy(SapItemRecord sap) => !sap.IsInventoryItem ? "None" : sap.ManageSerialNumbers ? "Serial" : sap.ManageBatchNumbers ? "Batch" : "None";
    private static bool EqualsCode(string? left, string? right) => string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
    private static string NormalizeKey(string? value) => Normalize(value).ToUpperInvariant();
    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SapItemPreviewItemDto ToPreview(SapItemRecord sap, string status, string statusName, ItemDto? local, string? differences)
        => new(Normalize(sap.ItemCode), Normalize(sap.ItemName), sap.ItemGroupCode, NormalizeOptional(sap.InventoryUnitCode), sap.IsActive,
            status, statusName, local?.Id, local?.Code, local?.Name, differences);
    private static SapItemImportItemResultDto ToResult(SapItemRecord sap, string status, string message, int? localId)
        => new(Normalize(sap.ItemCode), Normalize(sap.ItemName), status, message, localId);

    private sealed record LocalItemIndex(Dictionary<string, List<ItemDto>> BySapCode, Dictionary<string, List<ItemDto>> ByCode);
    private sealed record LocalMatch(ItemDto? Item, bool IsConflict, string Message);
    private sealed record MappedReferences(
        int? ItemGroupId, int? InventoryUnitId, int? PurchaseUnitId, int? SalesUnitId,
        int? PurchaseTaxId, int? SalesTaxId, bool GroupMissing, bool InventoryUnitMissing,
        bool PurchaseTaxMissing, bool SalesTaxMissing);
}
