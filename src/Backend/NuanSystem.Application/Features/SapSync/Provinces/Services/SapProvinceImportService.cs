using System.Text.Json;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;

namespace NuanSystem.Application.Features.SapSync.Provinces.Services;

public sealed class SapProvinceImportService(
    ISapProvinceReader reader,
    SapProvinceRecordProcessor recordProcessor,
    ISapSyncLogRepository sapSyncLogRepository) : ISapProvinceImportService
{
    private const string SapExternalSystem = "SAP_B1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<SapProvincePreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var sapProvinces = await reader.GetProvincesAsync(companyId, cancellationToken);
        var countries = await recordProcessor.GetLocalCountriesAsync(cancellationToken);
        var provinces = await recordProcessor.GetLocalProvincesAsync(cancellationToken);

        return sapProvinces
            .OrderBy(item => item.CountryCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.ProvinceCode, StringComparer.OrdinalIgnoreCase)
            .Select(item => BuildPreviewItem(item, countries, provinces))
            .ToArray();
    }

    public async Task<SapProvinceImportResultDto> ImportAsync(
        int companyId,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        var sapProvinces = await reader.GetProvincesAsync(companyId, cancellationToken);
        var results = new List<SapProvinceImportItemResultDto>(sapProvinces.Count);

        foreach (var row in sapProvinces
                     .OrderBy(item => item.CountryCode, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(item => item.ProvinceCode, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            SapProvinceRecordProcessResult processed;
            try
            {
                processed = await recordProcessor.ProcessAsync(
                    SapProvinceSnapshot.FromRecord(row), auditUserId, auditUserName, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                processed = new(
                    SapSyncExecutionDetailActions.Skip,
                    SapSyncExecutionDetailStatuses.Failed,
                    null,
                    null,
                    SapProvinceResultCodes.SaveFailed,
                    $"No fue posible importar la provincia: {exception.GetType().Name}.");
            }

            results.Add(new(
                NormalizeCode(row.CountryCode),
                NormalizeCode(row.ProvinceCode),
                Normalize(row.ProvinceName),
                processed.Status,
                processed.SafeMessage,
                processed.LocalProvinceId,
                processed.LocalGlobalId,
                processed.ResultCode));
        }

        var summary = new SapProvinceImportResultDto(
            sapProvinces.Count,
            Count(results, SapSyncExecutionDetailStatuses.Created),
            Count(results, SapSyncExecutionDetailStatuses.Updated),
            Count(results, SapSyncExecutionDetailStatuses.Unchanged),
            Count(results, SapSyncExecutionDetailStatuses.ApprovalRequired),
            Count(results, SapSyncExecutionDetailStatuses.Conflict),
            Count(results, SapSyncExecutionDetailStatuses.Skipped),
            Count(results, SapSyncExecutionDetailStatuses.Failed),
            results);

        await WritePublicLogAsync(companyId, summary, cancellationToken);
        return summary;
    }

    private async Task WritePublicLogAsync(
        int companyId,
        SapProvinceImportResultDto summary,
        CancellationToken cancellationToken)
    {
        var hasFailures = summary.Failed > 0;
        var message = $"Provincias SAP procesadas: {summary.TotalRead}; creadas: {summary.Created}; actualizadas: {summary.Updated}; sin cambios: {summary.Unchanged}; aprobacion: {summary.ApprovalRequired}; conflictos: {summary.Conflicts}; omitidas: {summary.Skipped}; fallidas: {summary.Failed}.";

        await sapSyncLogRepository.CreateAsync(new CreateSapSyncLogData(
            companyId,
            "Province",
            "Provinces",
            "Provinces",
            null,
            JsonSerializer.Serialize(summary, JsonOptions),
            hasFailures ? "Failed" : "Succeeded",
            hasFailures ? message : null,
            null,
            null,
            DateTime.UtcNow), cancellationToken);
    }

    private static SapProvincePreviewItemDto BuildPreviewItem(
        SapProvinceRecord sap,
        IReadOnlyCollection<CountryDto> countries,
        IReadOnlyCollection<ProvinceDto> provinces)
    {
        var countryCode = NormalizeCode(sap.CountryCode);
        var provinceCode = NormalizeCode(sap.ProvinceCode);
        var countryMatches = countries
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, countryCode))
            .ToArray();
        if (countryMatches.Length > 1)
        {
            return ToPreview(sap, "Conflict", "Conflicto", null,
                "Existe mas de un pais local con la misma referencia externa SAP.",
                SapProvinceResultCodes.CountryIdentityConflict);
        }

        var country = countryMatches.SingleOrDefault();
        if (country is null)
        {
            return ToPreview(sap, "Conflict", "Conflicto", null,
                "El pais SAP de la provincia no tiene un vinculo local confirmado.",
                SapProvinceResultCodes.CountryNotFound);
        }

        var externalCode = SapProvinceSnapshot.BuildExternalCode(countryCode, provinceCode);
        var externalMatches = provinces
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, externalCode))
            .ToArray();
        if (externalMatches.Length > 1)
        {
            return ToPreview(sap, "Conflict", "Conflicto", null,
                "Existe mas de una provincia local con la misma referencia externa SAP.",
                SapProvinceResultCodes.IdentityConflict);
        }

        var local = externalMatches.SingleOrDefault();
        if (local is not null && local.CountryId != country.Id)
        {
            return ToPreview(sap, "Conflict", "Conflicto", local,
                "La referencia externa de la provincia apunta a un pais local diferente.",
                SapProvinceResultCodes.IdentityConflict);
        }

        if (local is null)
        {
            var codeMatch = provinces.FirstOrDefault(item =>
                item.CountryId == country.Id && EqualsCode(item.Code, provinceCode));
            return codeMatch is null
                ? ToPreview(sap, "New", "Nuevo", null, null, null)
                : ToPreview(sap, "ApprovalRequired", "Requiere aprobacion", codeMatch,
                    "Existe una provincia con el mismo codigo en el pais, pero sin relacion SAP confirmada.",
                    SapProvinceResultCodes.CodeCollisionApprovalRequired);
        }

        var different = !EqualsText(sap.ProvinceName, local.Name);
        return ToPreview(
            sap,
            different ? "Different" : "Existing",
            different ? "Diferente" : "Existente",
            local,
            different ? "Nombre" : null,
            different ? SapProvinceResultCodes.Updated : SapProvinceResultCodes.Unchanged);
    }

    private static SapProvincePreviewItemDto ToPreview(
        SapProvinceRecord sap,
        string status,
        string statusName,
        ProvinceDto? local,
        string? differenceSummary,
        string? resultCode) =>
        new(
            NormalizeCode(sap.CountryCode),
            NormalizeCode(sap.ProvinceCode),
            Normalize(sap.ProvinceName),
            status,
            statusName,
            local?.Id,
            local?.CountryCode,
            local?.Code,
            local?.Name,
            differenceSummary,
            resultCode);

    private static int Count(IEnumerable<SapProvinceImportItemResultDto> results, string status) =>
        results.Count(item => item.Status == status);

    private static bool EqualsCode(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static bool EqualsText(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static string NormalizeCode(string? value) => Normalize(value).ToUpperInvariant();

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
}
