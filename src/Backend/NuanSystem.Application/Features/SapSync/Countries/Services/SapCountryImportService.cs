using System.Text.Json;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Features.SapSync.Countries.Services;

public sealed class SapCountryImportService(
    ISapCountryReader reader,
    SapCountryRecordProcessor recordProcessor,
    ISapSyncLogRepository sapSyncLogRepository) : ISapCountryImportService
{
    private const string SapExternalSystem = "SAP_B1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<SapCountryPreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var sapCountries = await reader.GetCountriesAsync(companyId, cancellationToken);
        var localCountries = await recordProcessor.GetLocalCountriesAsync(cancellationToken);

        return sapCountries
            .OrderBy(item => item.CountryCode, StringComparer.OrdinalIgnoreCase)
            .Select(item => BuildPreviewItem(item, localCountries))
            .ToArray();
    }

    public async Task<SapCountryImportResultDto> ImportAsync(
        int companyId,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        var sapCountries = await reader.GetCountriesAsync(companyId, cancellationToken);
        var results = new List<SapCountryImportItemResultDto>(sapCountries.Count);

        foreach (var row in sapCountries.OrderBy(item => item.CountryCode, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            SapCountryRecordProcessResult processed;
            try
            {
                processed = await recordProcessor.ProcessAsync(
                    SapCountrySnapshot.FromRecord(row), auditUserId, auditUserName, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                processed = new(
                    SapSyncExecutionDetailActions.Skip,
                    SapSyncExecutionDetailStatuses.Failed,
                    null,
                    null,
                    SapCountryResultCodes.SaveFailed,
                    $"No fue posible importar el pais: {exception.GetType().Name}.");
            }

            results.Add(new(
                Normalize(row.CountryCode),
                Normalize(row.CountryName),
                processed.Status,
                processed.SafeMessage,
                processed.LocalCountryId,
                processed.LocalGlobalId,
                processed.ResultCode));
        }

        var summary = new SapCountryImportResultDto(
            sapCountries.Count,
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
        SapCountryImportResultDto summary,
        CancellationToken cancellationToken)
    {
        var hasFailures = summary.Failed > 0;
        var message = $"Paises SAP procesados: {summary.TotalRead}; creados: {summary.Created}; actualizados: {summary.Updated}; sin cambios: {summary.Unchanged}; aprobacion: {summary.ApprovalRequired}; conflictos: {summary.Conflicts}; omitidos: {summary.Skipped}; fallidos: {summary.Failed}.";

        await sapSyncLogRepository.CreateAsync(new CreateSapSyncLogData(
            companyId,
            "Country",
            "Countries",
            "Countries",
            null,
            JsonSerializer.Serialize(summary, JsonOptions),
            hasFailures ? "Failed" : "Succeeded",
            hasFailures ? message : null,
            null,
            null,
            DateTime.UtcNow), cancellationToken);
    }

    private static SapCountryPreviewItemDto BuildPreviewItem(
        SapCountryRecord sap,
        IReadOnlyCollection<CountryDto> localCountries)
    {
        var code = Normalize(sap.CountryCode);
        var externalMatches = localCountries
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, code))
            .ToArray();
        if (externalMatches.Length > 1)
        {
            return ToPreview(sap, "Conflict", "Conflicto", null,
                "Existe mas de un pais local con la misma referencia externa SAP.",
                SapCountryResultCodes.IdentityConflict);
        }

        var local = externalMatches.SingleOrDefault();
        if (local is null)
        {
            var codeMatch = localCountries.FirstOrDefault(item => EqualsCode(item.Code, code));
            return codeMatch is null
                ? ToPreview(sap, "New", "Nuevo", null, null, null)
                : ToPreview(sap, "ApprovalRequired", "Requiere aprobacion", codeMatch,
                    "Existe un pais con el mismo codigo, pero sin relacion SAP confirmada.",
                    SapCountryResultCodes.CodeCollisionApprovalRequired);
        }

        var differences = BuildDifferences(sap, local);
        return ToPreview(
            sap,
            differences.Count == 0 ? "Existing" : "Different",
            differences.Count == 0 ? "Existente" : "Diferente",
            local,
            differences.Count == 0 ? null : string.Join(" | ", differences),
            differences.Count == 0 ? SapCountryResultCodes.Unchanged : SapCountryResultCodes.Updated);
    }

    private static List<string> BuildDifferences(SapCountryRecord sap, CountryDto local)
    {
        var differences = new List<string>();
        AddDifference(differences, "Nombre", sap.CountryName, local.Name);
        AddDifference(differences, "ISO2", sap.Iso2, local.Iso2);
        AddDifference(differences, "ISO3", sap.Iso3, local.Iso3);
        return differences;
    }

    private static void AddDifference(
        ICollection<string> differences,
        string label,
        string? sapValue,
        string? localValue)
    {
        if (!EqualsCode(sapValue, localValue))
        {
            differences.Add(label);
        }
    }

    private static SapCountryPreviewItemDto ToPreview(
        SapCountryRecord sap,
        string status,
        string statusName,
        CountryDto? local,
        string? differenceSummary,
        string? resultCode) =>
        new(
            Normalize(sap.CountryCode),
            Normalize(sap.CountryName),
            NormalizeOptional(sap.Iso2),
            NormalizeOptional(sap.Iso3),
            status,
            statusName,
            local?.Id,
            local?.Code,
            local?.Name,
            differenceSummary,
            resultCode);

    private static int Count(IEnumerable<SapCountryImportItemResultDto> results, string status) =>
        results.Count(item => item.Status == status);

    private static bool EqualsCode(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
