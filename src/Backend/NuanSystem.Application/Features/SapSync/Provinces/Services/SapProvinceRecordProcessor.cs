using MediatR;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Commands;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;

namespace NuanSystem.Application.Features.SapSync.Provinces.Services;

public sealed class SapProvinceRecordProcessor(IGeographyRepository geographyRepository, ISender sender)
{
    private const string SapExternalSystem = "SAP_B1";
    private List<CountryDto>? localCountryCache;
    private List<ProvinceDto>? localProvinceCache;

    public async Task<SapProvinceRecordProcessResult> ProcessAsync(
        SapProvinceSnapshot snapshot,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var countryCode = NormalizeCode(snapshot.CountryCode);
        var provinceCode = NormalizeCode(snapshot.ProvinceCode);
        var name = Normalize(snapshot.ProvinceName);
        if (countryCode.Length == 0 || provinceCode.Length == 0 || name.Length == 0)
        {
            return Result(SapSyncExecutionDetailActions.Skip, SapSyncExecutionDetailStatuses.Skipped,
                null, SapProvinceResultCodes.Invalid,
                "La provincia SAP no tiene pais, codigo o nombre valido.");
        }

        var countries = await GetLocalCountriesAsync(cancellationToken);
        var countryMatches = countries
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, countryCode))
            .ToArray();
        if (countryMatches.Length > 1)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapProvinceResultCodes.CountryIdentityConflict,
                "Existe mas de un pais local con la misma referencia externa SAP.");
        }

        var country = countryMatches.SingleOrDefault();
        if (country is null)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapProvinceResultCodes.CountryNotFound,
                "El pais SAP de la provincia no tiene un vinculo local confirmado.");
        }

        var externalCode = SapProvinceSnapshot.BuildExternalCode(countryCode, provinceCode);
        var provinces = await GetLocalProvincesAsync(cancellationToken);
        var externalMatches = provinces
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, externalCode))
            .ToArray();
        if (externalMatches.Length > 1)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapProvinceResultCodes.IdentityConflict,
                "Existe mas de una provincia local con la misma referencia externa SAP.");
        }

        var local = externalMatches.SingleOrDefault();
        if (local is not null && local.CountryId != country.Id)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                local, SapProvinceResultCodes.IdentityConflict,
                "La referencia externa de la provincia apunta a un pais local diferente.");
        }

        if (local is null)
        {
            var codeMatches = provinces
                .Where(item => item.CountryId == country.Id && EqualsCode(item.Code, provinceCode))
                .ToArray();
            if (codeMatches.Length > 0)
            {
                return Result(SapSyncExecutionDetailActions.Approval,
                    SapSyncExecutionDetailStatuses.ApprovalRequired, codeMatches[0],
                    SapProvinceResultCodes.CodeCollisionApprovalRequired,
                    "Existe una provincia con el mismo codigo en el pais, pero su relacion SAP requiere aprobacion.");
            }

            var created = await sender.Send(new CreateProvinceCommand(
                CountryId: country.Id,
                Code: provinceCode,
                Name: name,
                IsActive: true,
                AuditUserId: auditUserId,
                AuditUserName: auditUserName,
                ExternalSystem: SapExternalSystem,
                ExternalCode: externalCode), cancellationToken);

            if (!created.IsSuccess || created.Value is null)
            {
                return Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Failed,
                    null, SapProvinceResultCodes.SaveFailed, SafeMessage(created.Message));
            }

            UpdateCache(created.Value);
            return Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Created,
                created.Value, SapProvinceResultCodes.Created, "Provincia creada desde SAP.");
        }

        if (!HasRelevantChanges(snapshot, externalCode, local))
        {
            return Result(SapSyncExecutionDetailActions.NoChange, SapSyncExecutionDetailStatuses.Unchanged,
                local, SapProvinceResultCodes.Unchanged, "La provincia local ya esta actualizada.");
        }

        var updated = await sender.Send(new UpdateProvinceCommand(
            Id: local.Id,
            CountryId: local.CountryId,
            Code: local.Code,
            Name: name,
            IsActive: local.IsActive,
            AuditUserId: auditUserId,
            AuditUserName: auditUserName,
            ExternalSystem: SapExternalSystem,
            ExternalCode: externalCode), cancellationToken);

        if (!updated.IsSuccess || updated.Value is null)
        {
            return Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Failed,
                local, SapProvinceResultCodes.SaveFailed, SafeMessage(updated.Message));
        }

        UpdateCache(updated.Value);
        return Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Updated,
            updated.Value, SapProvinceResultCodes.Updated, "Provincia actualizada desde SAP.");
    }

    public async Task<IReadOnlyCollection<CountryDto>> GetLocalCountriesAsync(
        CancellationToken cancellationToken = default)
    {
        localCountryCache ??= (await geographyRepository.GetCountriesAsync(cancellationToken)).ToList();
        return localCountryCache;
    }

    public async Task<IReadOnlyCollection<ProvinceDto>> GetLocalProvincesAsync(
        CancellationToken cancellationToken = default)
    {
        localProvinceCache ??= (await geographyRepository.GetProvincesAsync(cancellationToken)).ToList();
        return localProvinceCache;
    }

    private void UpdateCache(ProvinceDto province)
    {
        localProvinceCache ??= [];
        localProvinceCache.RemoveAll(item => item.Id == province.Id);
        localProvinceCache.Add(province);
    }

    private static bool HasRelevantChanges(
        SapProvinceSnapshot snapshot,
        string externalCode,
        ProvinceDto local) =>
        !EqualsText(snapshot.ProvinceName, local.Name)
        || !EqualsCode(local.ExternalSystem, SapExternalSystem)
        || !EqualsCode(local.ExternalCode, externalCode);

    private static SapProvinceRecordProcessResult Result(
        string action, string status, ProvinceDto? local, string resultCode, string safeMessage) =>
        new(action, status, local?.Id, local?.GlobalId, resultCode, safeMessage);

    private static bool EqualsCode(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static bool EqualsText(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static string SafeMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? "No fue posible guardar la provincia." : message.Trim();

    private static string NormalizeCode(string? value) => Normalize(value).ToUpperInvariant();

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
}
