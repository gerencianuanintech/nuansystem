using System.Text.Json;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Features.SapSync.Cities.Services;

public sealed class SapCityImportService(
    ISapCityReader reader,
    SapCityRecordProcessor recordProcessor,
    ISapSyncLogRepository sapSyncLogRepository) : ISapCityImportService
{
    private const string SapExternalSystem = "SAP_B1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<SapCityPreviewItemDto>> PreviewAsync(
        int companyId, CancellationToken cancellationToken = default)
    {
        var rows = await reader.GetCitiesAsync(companyId, cancellationToken);
        var countries = await recordProcessor.GetLocalCountriesAsync(cancellationToken);
        var provinces = await recordProcessor.GetLocalProvincesAsync(cancellationToken);
        var cities = await recordProcessor.GetLocalCitiesAsync(cancellationToken);
        return rows.OrderBy(x => x.CountryCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.ProvinceCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.CityCode, StringComparer.OrdinalIgnoreCase)
            .Select(x => BuildPreview(x, countries, provinces, cities)).ToArray();
    }

    public async Task<SapCityImportResultDto> ImportAsync(
        int companyId, int? auditUserId, string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        var rows = await reader.GetCitiesAsync(companyId, cancellationToken);
        var items = new List<SapCityImportItemResultDto>(rows.Count);
        foreach (var row in rows.OrderBy(x => x.CountryCode, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(x => x.ProvinceCode, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(x => x.CityCode, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            SapCityRecordProcessResult result;
            try
            {
                result = await recordProcessor.ProcessAsync(
                    SapCitySnapshot.FromRecord(row), auditUserId, auditUserName, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                result = new(SapSyncExecutionDetailActions.Skip,
                    SapSyncExecutionDetailStatuses.Failed, null, null,
                    SapCityResultCodes.SaveFailed,
                    $"No fue posible importar la ciudad: {ex.GetType().Name}.");
            }

            items.Add(new(NormalizeCode(row.CountryCode), NormalizeCode(row.ProvinceCode),
                NormalizeCode(row.CityCode), Normalize(row.CityName), result.Status,
                result.SafeMessage, result.LocalCityId, result.LocalGlobalId, result.ResultCode));
        }

        var summary = new SapCityImportResultDto(rows.Count,
            Count(items, SapSyncExecutionDetailStatuses.Created),
            Count(items, SapSyncExecutionDetailStatuses.Updated),
            Count(items, SapSyncExecutionDetailStatuses.Unchanged),
            Count(items, SapSyncExecutionDetailStatuses.ApprovalRequired),
            Count(items, SapSyncExecutionDetailStatuses.Conflict),
            Count(items, SapSyncExecutionDetailStatuses.Skipped),
            Count(items, SapSyncExecutionDetailStatuses.Failed), items);
        await sapSyncLogRepository.CreateAsync(new CreateSapSyncLogData(
            companyId, "City", "Cities", "Cities", null,
            JsonSerializer.Serialize(summary, JsonOptions),
            summary.Failed > 0 ? "Failed" : "Succeeded",
            summary.Failed > 0 ? "Una o mas ciudades no pudieron importarse." : null,
            null, null, DateTime.UtcNow), cancellationToken);
        return summary;
    }

    private static SapCityPreviewItemDto BuildPreview(
        SapCityRecord sap, IReadOnlyCollection<CountryDto> countries,
        IReadOnlyCollection<ProvinceDto> provinces, IReadOnlyCollection<CityDto> cities)
    {
        var countryCode = NormalizeCode(sap.CountryCode);
        var provinceCode = NormalizeCode(sap.ProvinceCode);
        var cityCode = NormalizeCode(sap.CityCode);
        var countryMatches = countries.Where(x => IsSap(x.ExternalSystem)
            && EqualsCode(x.ExternalCode, countryCode)).ToArray();
        if (countryMatches.Length != 1)
            return Preview(sap, "Conflict", "Conflicto", null,
                countryMatches.Length == 0 ? "El pais SAP no tiene vinculo local confirmado." : "La referencia SAP del pais es ambigua.",
                countryMatches.Length == 0 ? SapCityResultCodes.CountryNotFound : SapCityResultCodes.CountryIdentityConflict);
        var country = countryMatches[0];
        var provinceExternal = SapCitySnapshot.BuildProvinceExternalCode(countryCode, provinceCode);
        var provinceMatches = provinces.Where(x => IsSap(x.ExternalSystem)
            && EqualsCode(x.ExternalCode, provinceExternal)).ToArray();
        if (provinceMatches.Length != 1 || provinceMatches[0].CountryId != country.Id)
            return Preview(sap, "Conflict", "Conflicto", null,
                provinceMatches.Length == 0 ? "La provincia SAP no tiene vinculo local confirmado." : "La referencia SAP de la provincia es ambigua o pertenece a otro pais.",
                provinceMatches.Length == 0 ? SapCityResultCodes.ProvinceNotFound : SapCityResultCodes.ProvinceIdentityConflict);
        var province = provinceMatches[0];
        var external = SapCitySnapshot.BuildExternalCode(countryCode, provinceCode, cityCode);
        var matches = cities.Where(x => IsSap(x.ExternalSystem) && EqualsCode(x.ExternalCode, external)).ToArray();
        if (matches.Length > 1 || (matches.Length == 1 && (matches[0].CountryId != country.Id || matches[0].ProvinceId != province.Id)))
            return Preview(sap, "Conflict", "Conflicto", matches.FirstOrDefault(),
                "La referencia SAP de la ciudad es ambigua o apunta a otra jerarquia.", SapCityResultCodes.IdentityConflict);
        var local = matches.SingleOrDefault();
        if (local is null)
        {
            var collision = cities.FirstOrDefault(x => x.ProvinceId == province.Id && EqualsCode(x.Code, cityCode));
            return collision is null
                ? Preview(sap, "New", "Nuevo", null, null, null)
                : Preview(sap, "ApprovalRequired", "Requiere aprobacion", collision,
                    "Existe el mismo codigo local sin relacion SAP confirmada.", SapCityResultCodes.CodeCollisionApprovalRequired);
        }
        var different = !EqualsText(sap.CityName, local.Name);
        return Preview(sap, different ? "Different" : "Existing",
            different ? "Diferente" : "Existente", local,
            different ? "Nombre" : null,
            different ? SapCityResultCodes.Updated : SapCityResultCodes.Unchanged);
    }

    private static SapCityPreviewItemDto Preview(SapCityRecord sap, string status,
        string statusName, CityDto? local, string? difference, string? resultCode) =>
        new(NormalizeCode(sap.CountryCode), NormalizeCode(sap.ProvinceCode),
            NormalizeCode(sap.CityCode), Normalize(sap.CityName), status, statusName,
            local?.Id, local?.CountryCode, local?.ProvinceCode, local?.Code,
            local?.Name, difference, resultCode);
    private static int Count(IEnumerable<SapCityImportItemResultDto> rows, string status) => rows.Count(x => x.Status == status);
    private static bool IsSap(string? value) => EqualsCode(value, SapExternalSystem);
    private static bool EqualsCode(string? x, string? y) => string.Equals(Normalize(x), Normalize(y), StringComparison.OrdinalIgnoreCase);
    private static bool EqualsText(string? x, string? y) => EqualsCode(x, y);
    private static string NormalizeCode(string? value) => Normalize(value).ToUpperInvariant();
    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
}
