using System.Text.Json;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSupplierImportService(
    ISapSupplierReader sapSupplierReader,
    IBusinessPartnerRepository businessPartnerRepository,
    ISapSyncLogRepository sapSyncLogRepository,
    ISapSyncInboxRepository? inboxRepository = null,
    ISapSyncWatermarkService? watermarkService = null)
    : ISapSupplierImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<SapSupplierPreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var sapSuppliers = await sapSupplierReader.GetSuppliersAsync(companyId, cancellationToken);
        var localSuppliers = await businessPartnerRepository.GetAllAsync("Supplier", cancellationToken);

        return BuildPreview(sapSuppliers, localSuppliers);
    }

    public async Task<SapSupplierImportResultDto> ImportAsync(
        int companyId,
        SapSupplierImportOptions options,
        CancellationToken cancellationToken = default)
    {
        var watermark = options.UseIncrementalWatermark && watermarkService is not null
            ? await watermarkService.GetAsync(companyId, SapSyncEntityCode.Suppliers, SapSyncDirection.SapToErp, cancellationToken)
            : null;

        var sapSuppliers = watermark?.LastSuccessfulSyncAtUtc is not null
            ? await sapSupplierReader.GetSuppliersChangedSinceAsync(companyId, watermark.LastSuccessfulSyncAtUtc.Value, cancellationToken)
            : await sapSupplierReader.GetSuppliersAsync(companyId, cancellationToken);

        var results = new List<SapSupplierImportItemResultDto>();
        foreach (var supplier in sapSuppliers.OrderBy(item => item.CardCode))
        {
            long? inboxId = null;
            if (options.WriteInbox && inboxRepository is not null && !string.IsNullOrWhiteSpace(supplier.CardCode))
            {
                inboxId = await inboxRepository.UpsertSupplierAsync(
                    companyId,
                    supplier.CardCode,
                    JsonSerializer.Serialize(supplier, JsonOptions),
                    SapSyncStatus.Processing,
                    options.WorkerInstance,
                    options.CorrelationId,
                    cancellationToken);
            }

            var itemResult = await ImportOneAsync(supplier, options.AuditUserId, options.AuditUserName, cancellationToken);
            results.Add(itemResult);

            if (options.WriteInbox && inboxRepository is not null && inboxId is not null)
            {
                await MarkInboxAsync(inboxRepository, inboxId.Value, itemResult, cancellationToken);
            }
        }

        var summary = BuildSummary(sapSuppliers.Count, results);

        if (options.WritePublicSapLog)
        {
            await RegisterPublicSyncLogAsync(companyId, summary, cancellationToken);
        }

        if (options.UseIncrementalWatermark && watermarkService is not null && summary.Failed == 0)
        {
            var lastSapKey = sapSuppliers
                .Where(item => !string.IsNullOrWhiteSpace(item.CardCode))
                .OrderBy(item => item.UpdatedAt ?? item.CreatedAt ?? DateTime.MinValue)
                .ThenBy(item => item.CardCode)
                .LastOrDefault()
                ?.CardCode;

            await watermarkService.UpsertSuccessAsync(
                companyId,
                SapSyncEntityCode.Suppliers,
                SapSyncDirection.SapToErp,
                DateTime.UtcNow,
                lastSapKey,
                JsonSerializer.Serialize(new { summary.TotalRead, summary.Created, summary.Updated, summary.Unchanged, summary.Skipped }, JsonOptions),
                cancellationToken);
        }

        return summary;
    }

    public async Task<SapSupplierImportItemResultDto> ImportOneAsync(
        SapSupplierRecord supplier,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        var localBusinessPartners = await businessPartnerRepository.GetAllAsync(null, cancellationToken);
        var localIndex = BuildLocalIndex(localBusinessPartners);
        return await ImportOneCoreAsync(supplier, localIndex, auditUserId, auditUserName, cancellationToken);
    }

    private async Task<SapSupplierImportItemResultDto> ImportOneCoreAsync(
        SapSupplierRecord supplier,
        LocalSupplierIndex localIndex,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(supplier.CardCode) || string.IsNullOrWhiteSpace(supplier.CardName))
        {
            return ToItemResult(supplier, "Skipped", "El proveedor SAP no tiene codigo o nombre.", null);
        }

        var match = FindLocalMatch(supplier, localIndex);
        if (match.Status == "Conflict")
        {
            return ToItemResult(supplier, "Conflict", match.Message, match.Supplier?.Id);
        }

        if (match.Supplier is not null && HasNoRelevantChanges(supplier, match.Supplier))
        {
            return ToItemResult(supplier, "Unchanged", "El proveedor local ya esta actualizado.", match.Supplier.Id);
        }

        try
        {
            var importResult = await businessPartnerRepository.ImportSupplierFromSapAsync(
                new BusinessPartnerSapImportData(
                    NormalizeRequired(supplier.CardCode),
                    NormalizeRequired(supplier.CardName),
                    Normalize(supplier.TaxIdentification),
                    NormalizeCardType(supplier.CardType),
                    Normalize(supplier.Phone),
                    Normalize(supplier.Email),
                    Normalize(supplier.Currency),
                    supplier.IsActive,
                    auditUserId,
                    Normalize(auditUserName)),
                cancellationToken);

            return ToItemResult(supplier, importResult.Action, importResult.Message, importResult.BusinessPartnerId);
        }
        catch (Exception exception)
        {
            return ToItemResult(supplier, "Failed", exception.Message, match.Supplier?.Id);
        }
    }

    private static SapSupplierImportResultDto BuildSummary(
        int totalRead,
        IReadOnlyCollection<SapSupplierImportItemResultDto> results)
    {
        return new SapSupplierImportResultDto(
            totalRead,
            results.Count(item => item.Status == "Created"),
            results.Count(item => item.Status == "Updated"),
            results.Count(item => item.Status == "Unchanged"),
            results.Count(item => item.Status is "Skipped" or "Conflict"),
            results.Count(item => item.Status == "Failed"),
            results);
    }

    private static async Task MarkInboxAsync(
        ISapSyncInboxRepository inboxRepository,
        long inboxId,
        SapSupplierImportItemResultDto result,
        CancellationToken cancellationToken)
    {
        switch (result.Status)
        {
            case "Created":
            case "Updated":
            case "Unchanged":
                await inboxRepository.MarkImportedAsync(inboxId, result.LocalBusinessPartnerId?.ToString(), cancellationToken);
                break;
            case "Conflict":
                await inboxRepository.MarkConflictAsync(inboxId, result.Message, cancellationToken);
                break;
            case "Skipped":
                await inboxRepository.MarkFailedAsync(inboxId, "SKIPPED", result.Message, null, cancellationToken);
                break;
            default:
                await inboxRepository.MarkFailedAsync(inboxId, "FAILED", result.Message, null, cancellationToken);
                break;
        }
    }

    private async Task RegisterPublicSyncLogAsync(
        int companyId,
        SapSupplierImportResultDto summary,
        CancellationToken cancellationToken)
    {
        var status = summary.Failed > 0 ? "Failed" : "Succeeded";
        var message = summary.Failed > 0
            ? $"Importacion con {summary.Failed} error(es). Creados: {summary.Created}, actualizados: {summary.Updated}, omitidos/conflictos: {summary.Skipped}."
            : $"Importacion completada. Creados: {summary.Created}, actualizados: {summary.Updated}, sin cambios: {summary.Unchanged}, omitidos/conflictos: {summary.Skipped}.";

        await sapSyncLogRepository.CreateAsync(
            new CreateSapSyncLogData(
                companyId,
                "BusinessPartner",
                "Suppliers",
                "OCRD",
                null,
                JsonSerializer.Serialize(summary, JsonOptions),
                status,
                message,
                null,
                null,
                DateTime.UtcNow),
            cancellationToken);
    }

    private static IReadOnlyCollection<SapSupplierPreviewItemDto> BuildPreview(
        IReadOnlyCollection<SapSupplierRecord> sapSuppliers,
        IReadOnlyCollection<BusinessPartnerDto> localSuppliers)
    {
        var localIndex = BuildLocalIndex(localSuppliers);
        return sapSuppliers
            .Select(item => BuildPreviewItem(item, localIndex))
            .OrderBy(item => item.SapCardCode)
            .ToList();
    }

    private static SapSupplierPreviewItemDto BuildPreviewItem(SapSupplierRecord sapSupplier, LocalSupplierIndex localIndex)
    {
        var match = FindLocalMatch(sapSupplier, localIndex);
        if (match.Status == "Conflict")
        {
            return ToPreview(sapSupplier, "Conflict", "Conflicto", match.Supplier, match.Message);
        }

        if (match.Supplier is not null)
        {
            var differences = BuildDifferences(sapSupplier, match.Supplier);
            return ToPreview(
                sapSupplier,
                differences.Count == 0 ? "Existing" : "Different",
                differences.Count == 0 ? "Existente" : "Diferente",
                match.Supplier,
                differences.Count == 0 ? null : string.Join(" | ", differences));
        }

        return ToPreview(sapSupplier, "New", "Nuevo", null, null);
    }

    public static LocalSupplierIndex BuildLocalIndex(IReadOnlyCollection<BusinessPartnerDto> suppliers)
    {
        return new LocalSupplierIndex(
            suppliers
                .Where(item => !string.IsNullOrWhiteSpace(item.SapCardCode))
                .GroupBy(item => NormalizeKey(item.SapCardCode))
                .ToDictionary(group => group.Key, group => group.ToList()),
            suppliers
                .GroupBy(item => NormalizeKey(item.Code))
                .ToDictionary(group => group.Key, group => group.ToList()),
            suppliers
                .Where(item => !string.IsNullOrWhiteSpace(item.IdentificationNumber))
                .GroupBy(item => NormalizeKey(item.IdentificationNumber))
                .ToDictionary(group => group.Key, group => group.ToList()));
    }

    public static LocalMatch FindLocalMatch(SapSupplierRecord supplier, LocalSupplierIndex localIndex)
    {
        var sapCodeKey = NormalizeKey(supplier.CardCode);
        var identificationKey = NormalizeKey(supplier.TaxIdentification);
        var matches = new List<BusinessPartnerDto>();

        if (localIndex.BySapCardCode.TryGetValue(sapCodeKey, out var bySap))
        {
            matches.AddRange(bySap);
        }

        if (localIndex.ByCode.TryGetValue(sapCodeKey, out var byCode))
        {
            matches.AddRange(byCode.Where(item => matches.All(match => match.Id != item.Id)));
        }

        if (matches.Count > 1)
        {
            return new LocalMatch("Conflict", "Existe mas de un proveedor local con el mismo codigo SAP.", null);
        }

        if (matches.Count == 1)
        {
            return new LocalMatch("Matched", "Proveedor local encontrado.", matches[0]);
        }

        if (!string.IsNullOrWhiteSpace(identificationKey)
            && localIndex.ByIdentification.TryGetValue(identificationKey, out var byIdentification)
            && byIdentification.Count > 0)
        {
            return new LocalMatch(
                "Conflict",
                "Existe un proveedor local con la misma identificacion, pero sin relacion SAP confirmada.",
                byIdentification[0]);
        }

        return new LocalMatch("New", "Proveedor nuevo.", null);
    }

    private static bool HasNoRelevantChanges(SapSupplierRecord supplier, BusinessPartnerDto local)
    {
        return string.Equals(NormalizeValue(supplier.CardName), NormalizeValue(local.Name), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeValue(supplier.Email), NormalizeValue(local.Email), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeValue(supplier.Phone), NormalizeValue(local.Phone), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeValue(supplier.Currency), NormalizeValue(local.PreferredCurrencyCode), StringComparison.OrdinalIgnoreCase)
            && supplier.IsActive == local.IsActive
            && string.Equals(NormalizeValue(supplier.CardCode), NormalizeValue(local.SapCardCode), StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyCollection<string> BuildDifferences(SapSupplierRecord sapSupplier, BusinessPartnerDto local)
    {
        var differences = new List<string>();
        AddDifference(differences, "Nombre", sapSupplier.CardName, local.Name);
        AddDifference(differences, "Email", sapSupplier.Email, local.Email);
        AddDifference(differences, "Telefono", sapSupplier.Phone, local.Phone);
        AddDifference(differences, "Moneda", sapSupplier.Currency, local.PreferredCurrencyCode);

        if (sapSupplier.IsActive != local.IsActive)
        {
            differences.Add("Estado activo");
        }

        return differences;
    }

    private static void AddDifference(ICollection<string> differences, string label, string? sapValue, string? localValue)
    {
        if (!string.Equals(NormalizeValue(sapValue), NormalizeValue(localValue), StringComparison.OrdinalIgnoreCase))
        {
            differences.Add(label);
        }
    }

    private static SapSupplierPreviewItemDto ToPreview(
        SapSupplierRecord sapSupplier,
        string status,
        string statusName,
        BusinessPartnerDto? local,
        string? differenceSummary)
    {
        return new SapSupplierPreviewItemDto(
            sapSupplier.CardCode,
            sapSupplier.CardName,
            sapSupplier.TaxIdentification,
            sapSupplier.Email,
            sapSupplier.Phone,
            sapSupplier.Currency,
            sapSupplier.IsActive,
            status,
            statusName,
            local?.Id,
            local?.Code,
            local?.Name,
            differenceSummary);
    }

    private static SapSupplierImportItemResultDto ToItemResult(SapSupplierRecord supplier, string status, string message, int? localBusinessPartnerId)
    {
        return new SapSupplierImportItemResultDto(NormalizeValue(supplier.CardCode), NormalizeValue(supplier.CardName), status, message, localBusinessPartnerId);
    }

    private static string NormalizeCardType(string? value)
    {
        var normalized = NormalizeValue(value).ToUpperInvariant();
        return string.IsNullOrWhiteSpace(normalized) ? "S" : normalized[..1];
    }

    private static string NormalizeRequired(string value) => value.Trim();
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string NormalizeKey(string? value) => NormalizeValue(value).ToUpperInvariant();
    private static string NormalizeValue(string? value) => value?.Trim() ?? string.Empty;

    public sealed record LocalSupplierIndex(
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> BySapCardCode,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> ByCode,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> ByIdentification);

    public sealed record LocalMatch(string Status, string Message, BusinessPartnerDto? Supplier);
}
