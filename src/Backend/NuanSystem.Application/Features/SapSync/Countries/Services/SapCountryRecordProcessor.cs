using MediatR;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Features.SapSync.Countries.Services;

public sealed class SapCountryRecordProcessor(IGeographyRepository geographyRepository, ISender sender)
{
    private const string SapExternalSystem = "SAP_B1";
    private List<CountryDto>? localCountryCache;

    public async Task<SapCountryRecordProcessResult> ProcessAsync(
        SapCountrySnapshot snapshot,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var code = Normalize(snapshot.CountryCode).ToUpperInvariant();
        var name = Normalize(snapshot.CountryName);
        if (code.Length == 0 || name.Length == 0)
        {
            return Result(SapSyncExecutionDetailActions.Skip, SapSyncExecutionDetailStatuses.Skipped,
                null, SapCountryResultCodes.Invalid, "El pais SAP no tiene codigo o nombre.");
        }

        var localCountries = await GetLocalCountriesAsync(cancellationToken);
        var externalMatches = localCountries
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, code))
            .ToArray();
        if (externalMatches.Length > 1)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapCountryResultCodes.IdentityConflict,
                "Existe mas de un pais local con la misma referencia externa SAP.");
        }

        var local = externalMatches.SingleOrDefault();
        if (local is null)
        {
            var codeMatches = localCountries.Where(item => EqualsCode(item.Code, code)).ToArray();
            if (codeMatches.Length > 0)
            {
                return Result(SapSyncExecutionDetailActions.Approval,
                    SapSyncExecutionDetailStatuses.ApprovalRequired, codeMatches[0],
                    SapCountryResultCodes.CodeCollisionApprovalRequired,
                    "Existe un pais con el mismo codigo, pero su relacion SAP requiere aprobacion.");
            }

            var created = await sender.Send(new CreateCountryCommand(
                Code: code,
                Name: name,
                Iso2: NormalizeOptional(snapshot.Iso2),
                Iso3: NormalizeOptional(snapshot.Iso3),
                PhonePrefix: null,
                IsActive: true,
                AuditUserId: auditUserId,
                AuditUserName: auditUserName,
                ExternalSystem: SapExternalSystem,
                ExternalCode: code), cancellationToken);

            if (!created.IsSuccess || created.Value is null)
            {
                return Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Failed,
                    null, SapCountryResultCodes.SaveFailed, SafeMessage(created.Message));
            }

            UpdateCache(created.Value);
            return Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Created,
                created.Value, SapCountryResultCodes.Created, "Pais creado desde SAP.");
        }

        if (!HasRelevantChanges(snapshot, local))
        {
            return Result(SapSyncExecutionDetailActions.NoChange, SapSyncExecutionDetailStatuses.Unchanged,
                local, SapCountryResultCodes.Unchanged, "El pais local ya esta actualizado.");
        }

        var updated = await sender.Send(new UpdateCountryCommand(
            Id: local.Id,
            Code: local.Code,
            Name: name,
            Iso2: NormalizeOptional(snapshot.Iso2),
            Iso3: NormalizeOptional(snapshot.Iso3),
            PhonePrefix: local.PhonePrefix,
            IsActive: local.IsActive,
            AuditUserId: auditUserId,
            AuditUserName: auditUserName,
            ExternalSystem: SapExternalSystem,
            ExternalCode: code), cancellationToken);

        if (!updated.IsSuccess || updated.Value is null)
        {
            return Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Failed,
                local, SapCountryResultCodes.SaveFailed, SafeMessage(updated.Message));
        }

        UpdateCache(updated.Value);
        return Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Updated,
            updated.Value, SapCountryResultCodes.Updated, "Pais actualizado desde SAP.");
    }

    public async Task<IReadOnlyCollection<CountryDto>> GetLocalCountriesAsync(
        CancellationToken cancellationToken = default)
    {
        localCountryCache ??= (await geographyRepository.GetCountriesAsync(cancellationToken)).ToList();
        return localCountryCache;
    }

    private void UpdateCache(CountryDto country)
    {
        localCountryCache ??= [];
        localCountryCache.RemoveAll(item => item.Id == country.Id);
        localCountryCache.Add(country);
    }

    private static bool HasRelevantChanges(SapCountrySnapshot snapshot, CountryDto local) =>
        !EqualsText(snapshot.CountryName, local.Name)
        || !EqualsCode(snapshot.Iso2, local.Iso2)
        || !EqualsCode(snapshot.Iso3, local.Iso3)
        || !EqualsCode(local.ExternalSystem, SapExternalSystem)
        || !EqualsCode(local.ExternalCode, snapshot.CountryCode);

    private static SapCountryRecordProcessResult Result(
        string action, string status, CountryDto? local, string resultCode, string safeMessage) =>
        new(action, status, local?.Id, local?.GlobalId, resultCode, safeMessage);

    private static bool EqualsCode(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static bool EqualsText(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static string SafeMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? "No fue posible guardar el pais." : message.Trim();

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
