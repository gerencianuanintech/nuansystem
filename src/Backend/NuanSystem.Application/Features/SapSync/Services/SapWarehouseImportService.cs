using System.Text.Json;
using MediatR;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Warehouses.Contracts;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapWarehouseImportService(
    ISapWarehouseReader sapWarehouseReader,
    IWarehouseRepository warehouseRepository,
    ISapSyncLogRepository sapSyncLogRepository,
    ISender sender) : ISapWarehouseImportService
{
    private const string SapExternalSystem = "SAP_B1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<SapWarehousePreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var sapWarehouses = await sapWarehouseReader.GetWarehousesAsync(companyId, cancellationToken);
        var localWarehouses = await warehouseRepository.GetAllAsync(cancellationToken);
        var index = BuildIndex(localWarehouses);

        return sapWarehouses
            .OrderBy(item => item.WarehouseCode)
            .Select(item => BuildPreviewItem(item, index))
            .ToArray();
    }

    public async Task<SapWarehouseImportResultDto> ImportAsync(
        int companyId,
        IReadOnlyCollection<SapWarehouseBranchMappingDto> mappings,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        var sapWarehouses = await sapWarehouseReader.GetWarehousesAsync(companyId, cancellationToken);
        var localWarehouses = await warehouseRepository.GetAllAsync(cancellationToken);
        var index = BuildIndex(localWarehouses);
        var results = new List<SapWarehouseImportItemResultDto>();
        var mappingBySapCode = mappings.ToDictionary(
            item => NormalizeKey(item.SapWarehouseCode),
            item => Normalize(item.BranchCode),
            StringComparer.OrdinalIgnoreCase);
        var importedSapCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sapWarehouse in sapWarehouses.OrderBy(item => item.WarehouseCode))
        {
            var sapCodeKey = NormalizeKey(sapWarehouse.WarehouseCode);
            importedSapCodes.Add(sapCodeKey);
            var hasExplicitMapping = mappingBySapCode.TryGetValue(sapCodeKey, out var branchCode);

            var result = await ImportOneAsync(
                sapWarehouse,
                branchCode,
                hasExplicitMapping,
                index,
                auditUserId,
                auditUserName,
                cancellationToken);
            results.Add(result);

            if (result.LocalWarehouseId is not null && result.Status is ("Created" or "Updated"))
            {
                var refreshed = await warehouseRepository.GetByIdAsync(result.LocalWarehouseId.Value, cancellationToken);
                if (refreshed is not null)
                {
                    AddToIndex(index, refreshed);
                }
            }
        }

        foreach (var missingMapping in mappings
                     .Where(item => !importedSapCodes.Contains(NormalizeKey(item.SapWarehouseCode)))
                     .OrderBy(item => item.SapWarehouseCode))
        {
            results.Add(new SapWarehouseImportItemResultDto(
                Normalize(missingMapping.SapWarehouseCode),
                string.Empty,
                "Skipped",
                "La bodega configurada en la matriz no existe en la respuesta actual de SAP.",
                null));
        }

        var summary = new SapWarehouseImportResultDto(
            sapWarehouses.Count,
            results.Count(item => item.Status == "Created"),
            results.Count(item => item.Status == "Updated"),
            results.Count(item => item.Status == "Unchanged"),
            results.Count(item => item.Status is "Skipped" or "Conflict" or "ApprovalRequired"),
            results.Count(item => item.Status == "Failed"),
            results);

        await WritePublicLogAsync(companyId, summary, cancellationToken);
        return summary;
    }

    private async Task<SapWarehouseImportItemResultDto> ImportOneAsync(
        SapWarehouseRecord sap,
        string? mappedBranchCode,
        bool hasExplicitMapping,
        LocalWarehouseIndex index,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken)
    {
        var code = Normalize(sap.WarehouseCode);
        var name = Normalize(sap.WarehouseName);
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            return ToResult(sap, "Skipped", "La bodega SAP no tiene codigo o nombre.", null);
        }

        var match = FindMatch(code, index);
        if (match.IsConflict)
        {
            return ToResult(
                sap,
                match.IsCodeCollision ? "ApprovalRequired" : "Conflict",
                match.Message,
                match.Warehouse?.Id,
                match.IsCodeCollision
                    ? SapWarehouseResultCodes.CodeCollisionApprovalRequired
                    : SapWarehouseResultCodes.IdentityConflict);
        }

        if (match.Warehouse is null && !sap.IsActive)
        {
            return ToResult(
                sap,
                "Skipped",
                "La bodega SAP nueva esta inactiva y no se crea automaticamente.",
                null,
                SapWarehouseResultCodes.Inactive);
        }

        if (match.Warehouse is { IsActive: true } && !sap.IsActive)
        {
            return ToResult(
                sap,
                "ApprovalRequired",
                "SAP reporta la bodega inactiva; la bodega local permanece activa hasta aprobacion.",
                match.Warehouse.Id,
                SapWarehouseResultCodes.ApprovalRequired);
        }

        try
        {
            var branchCode = hasExplicitMapping
                ? NormalizeOptional(mappedBranchCode)
                : match.Warehouse?.BranchCode;

            if (match.Warehouse is null)
            {
                var createResult = await sender.Send(new CreateWarehouseCommand(
                    GlobalId: null,
                    Code: code,
                    Name: name,
                    Description: "Importada desde SAP Business One.",
                    BranchCode: branchCode,
                    Address: NormalizeOptional(sap.Street),
                    City: NormalizeOptional(sap.City),
                    Province: NormalizeOptional(sap.Province),
                    Country: NormalizeOptional(sap.Country),
                    Phone: null,
                    Email: null,
                    ManagerName: null,
                    AllowsSales: true,
                    AllowsPurchases: true,
                    AllowsTransfers: true,
                    AllowsProduction: false,
                    IsDefault: false,
                    ExternalSystem: SapExternalSystem,
                    ExternalCode: code,
                    SapCode: code,
                    IsActive: sap.IsActive,
                    AuditUserId: auditUserId,
                    AuditUserName: auditUserName), cancellationToken);

                return createResult.IsSuccess && createResult.Value is not null
                    ? ToResult(sap, "Created", "Bodega creada desde SAP.", createResult.Value.Id)
                    : ToResult(sap, "Failed", createResult.Message, null);
            }

            var local = match.Warehouse;
            if (!HasRelevantChanges(sap, branchCode, local))
            {
                var message = sap.IsActive == local.IsActive
                    ? "La bodega local ya esta actualizada."
                    : "Los datos ya estan actualizados; el cambio de estado SAP queda pendiente de aprobacion manual.";

                return ToResult(sap, "Unchanged", message, local.Id);
            }

            var updateResult = await sender.Send(new UpdateWarehouseCommand(
                local.Id,
                local.GlobalId,
                local.Code,
                name,
                local.Description,
                branchCode,
                NormalizeOptional(sap.Street),
                NormalizeOptional(sap.City),
                NormalizeOptional(sap.Province),
                NormalizeOptional(sap.Country),
                local.Phone,
                local.Email,
                local.ManagerName,
                local.AllowsSales,
                local.AllowsPurchases,
                local.AllowsTransfers,
                local.AllowsProduction,
                local.IsDefault,
                SapExternalSystem,
                code,
                code,
                local.IsActive,
                auditUserId,
                auditUserName), cancellationToken);

            var statusMessage = sap.IsActive == local.IsActive
                ? "Bodega actualizada desde SAP."
                : "Bodega actualizada; el cambio de estado SAP queda pendiente de aprobacion manual.";

            return updateResult.IsSuccess && updateResult.Value is not null
                ? ToResult(sap, "Updated", statusMessage, updateResult.Value.Id)
                : ToResult(sap, "Failed", updateResult.Message, local.Id);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return ToResult(sap, "Failed", $"No fue posible importar la bodega: {exception.GetType().Name}.", match.Warehouse?.Id);
        }
    }

    private async Task WritePublicLogAsync(
        int companyId,
        SapWarehouseImportResultDto summary,
        CancellationToken cancellationToken)
    {
        var status = summary.Failed > 0 ? "Failed" : "Succeeded";
        var message = $"Bodegas SAP procesadas: {summary.TotalRead}; creadas: {summary.Created}; actualizadas: {summary.Updated}; sin cambios: {summary.Unchanged}; omitidas/conflictos: {summary.Skipped}; fallidas: {summary.Failed}.";

        await sapSyncLogRepository.CreateAsync(new CreateSapSyncLogData(
            companyId,
            "Warehouse",
            "Warehouses",
            "Warehouses",
            null,
            JsonSerializer.Serialize(summary, JsonOptions),
            status,
            summary.Failed > 0 ? message : null,
            null,
            null,
            DateTime.UtcNow), cancellationToken);
    }

    private static SapWarehousePreviewItemDto BuildPreviewItem(SapWarehouseRecord sap, LocalWarehouseIndex index)
    {
        var code = Normalize(sap.WarehouseCode);
        var match = FindMatch(code, index);
        if (match.IsConflict)
        {
            return ToPreview(sap, "Conflict", "Conflicto", match.Warehouse, match.Message);
        }

        if (match.Warehouse is null)
        {
            return ToPreview(sap, "New", "Nuevo", null, null);
        }

        var differences = BuildDifferences(sap, match.Warehouse);
        return ToPreview(
            sap,
            differences.Count == 0 ? "Existing" : "Different",
            differences.Count == 0 ? "Existente" : "Diferente",
            match.Warehouse,
            differences.Count == 0 ? null : string.Join(" | ", differences));
    }

    private static LocalMatch FindMatch(string code, LocalWarehouseIndex index)
    {
        index.BySapCode.TryGetValue(NormalizeKey(code), out var sapMatches);
        if (sapMatches is { Count: > 1 })
        {
            return new LocalMatch(null, true, false, "Existe mas de una bodega local con el mismo codigo SAP.");
        }

        if (sapMatches is { Count: 1 })
        {
            return new LocalMatch(sapMatches[0], false, false, "Bodega relacionada por codigo SAP.");
        }

        index.ByCode.TryGetValue(NormalizeKey(code), out var codeMatches);
        if (codeMatches is { Count: > 0 })
        {
            return new LocalMatch(codeMatches[0], true, true, "Existe una bodega con el mismo codigo, pero sin relacion SAP confirmada.");
        }

        return new LocalMatch(null, false, false, "Bodega nueva.");
    }

    private static LocalWarehouseIndex BuildIndex(IReadOnlyCollection<WarehouseDto> warehouses)
    {
        var index = new LocalWarehouseIndex(
            new Dictionary<string, List<WarehouseDto>>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, List<WarehouseDto>>(StringComparer.OrdinalIgnoreCase));

        foreach (var warehouse in warehouses)
        {
            AddToIndex(index, warehouse);
        }

        return index;
    }

    private static void AddToIndex(LocalWarehouseIndex index, WarehouseDto warehouse)
    {
        Add(index.ByCode, warehouse.Code, warehouse);
        Add(index.BySapCode, warehouse.SapCode, warehouse);
    }

    private static void Add(Dictionary<string, List<WarehouseDto>> dictionary, string? value, WarehouseDto warehouse)
    {
        var key = NormalizeKey(value);
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        if (!dictionary.TryGetValue(key, out var rows))
        {
            rows = [];
            dictionary[key] = rows;
        }

        rows.RemoveAll(item => item.Id == warehouse.Id);
        rows.Add(warehouse);
    }

    private static bool HasRelevantChanges(SapWarehouseRecord sap, string? branchCode, WarehouseDto local)
        => BuildImportableDifferences(sap, local).Count > 0
           || !string.Equals(Normalize(local.BranchCode), Normalize(branchCode), StringComparison.OrdinalIgnoreCase)
           || !string.Equals(Normalize(local.SapCode), Normalize(sap.WarehouseCode), StringComparison.OrdinalIgnoreCase)
           || !string.Equals(Normalize(local.ExternalSystem), SapExternalSystem, StringComparison.OrdinalIgnoreCase)
           || !string.Equals(Normalize(local.ExternalCode), Normalize(sap.WarehouseCode), StringComparison.OrdinalIgnoreCase);

    private static List<string> BuildDifferences(SapWarehouseRecord sap, WarehouseDto local)
    {
        var differences = BuildImportableDifferences(sap, local);
        if (sap.IsActive != local.IsActive)
        {
            differences.Add("Estado activo (requiere aprobacion)");
        }

        return differences;
    }

    private static List<string> BuildImportableDifferences(SapWarehouseRecord sap, WarehouseDto local)
    {
        var differences = new List<string>();
        AddDifference(differences, "Nombre", sap.WarehouseName, local.Name);
        AddDifference(differences, "Direccion", sap.Street, local.Address);
        AddDifference(differences, "Ciudad", sap.City, local.City);
        AddDifference(differences, "Provincia", sap.Province, local.Province);
        AddDifference(differences, "Pais", sap.Country, local.Country);

        return differences;
    }

    private static void AddDifference(ICollection<string> differences, string label, string? sapValue, string? localValue)
    {
        if (!string.Equals(Normalize(sapValue), Normalize(localValue), StringComparison.OrdinalIgnoreCase))
        {
            differences.Add(label);
        }
    }

    private static SapWarehousePreviewItemDto ToPreview(
        SapWarehouseRecord sap,
        string status,
        string statusName,
        WarehouseDto? local,
        string? differenceSummary)
        => new(
            Normalize(sap.WarehouseCode),
            Normalize(sap.WarehouseName),
            NormalizeOptional(sap.Street),
            NormalizeOptional(sap.City),
            NormalizeOptional(sap.Province),
            NormalizeOptional(sap.Country),
            sap.IsActive,
            status,
            statusName,
            local?.Id,
            local?.Code,
            local?.Name,
            differenceSummary);

    private static SapWarehouseImportItemResultDto ToResult(
        SapWarehouseRecord sap,
        string status,
        string message,
        int? localWarehouseId,
        string? resultCode = null)
        => new(Normalize(sap.WarehouseCode), Normalize(sap.WarehouseName), status, message, localWarehouseId, resultCode);

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
    private static string NormalizeKey(string? value) => Normalize(value).ToUpperInvariant();
    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record LocalWarehouseIndex(
        Dictionary<string, List<WarehouseDto>> BySapCode,
        Dictionary<string, List<WarehouseDto>> ByCode);

    private sealed record LocalMatch(WarehouseDto? Warehouse, bool IsConflict, bool IsCodeCollision, string Message);
}
