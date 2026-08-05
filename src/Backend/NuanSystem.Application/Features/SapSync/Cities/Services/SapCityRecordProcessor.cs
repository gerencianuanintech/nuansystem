using MediatR;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.General.Cities.Commands;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Features.SapSync.Cities.Services;

public sealed class SapCityRecordProcessor(IGeographyRepository geographyRepository, ISender sender)
{
    private const string SapExternalSystem = "SAP_B1";
    private List<CountryDto>? localCountryCache;
    private List<ProvinceDto>? localProvinceCache;
    private List<CityDto>? localCityCache;

    public async Task<SapCityRecordProcessResult> ProcessAsync(
        SapCitySnapshot snapshot,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var countryCode = NormalizeCode(snapshot.CountryCode);
        var provinceCode = NormalizeCode(snapshot.ProvinceCode);
        var cityCode = NormalizeCode(snapshot.CityCode);
        var cityName = Normalize(snapshot.CityName);
        if (countryCode.Length == 0 || provinceCode.Length == 0
            || cityCode.Length == 0 || cityName.Length == 0)
        {
            return Result(SapSyncExecutionDetailActions.Skip, SapSyncExecutionDetailStatuses.Skipped,
                null, SapCityResultCodes.Invalid,
                "La ciudad SAP no tiene pais, provincia, codigo o nombre valido.");
        }

        var countries = await GetLocalCountriesAsync(cancellationToken);
        var countryMatches = countries
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, countryCode))
            .ToArray();
        if (countryMatches.Length > 1)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapCityResultCodes.CountryIdentityConflict,
                "Existe mas de un pais local con la misma referencia externa SAP.");
        }

        var country = countryMatches.SingleOrDefault();
        if (country is null)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapCityResultCodes.CountryNotFound,
                "El pais SAP de la ciudad no tiene un vinculo local confirmado.");
        }

        var provinceExternalCode = SapCitySnapshot.BuildProvinceExternalCode(countryCode, provinceCode);
        var provinces = await GetLocalProvincesAsync(cancellationToken);
        var provinceMatches = provinces
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, provinceExternalCode))
            .ToArray();
        if (provinceMatches.Length > 1)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapCityResultCodes.ProvinceIdentityConflict,
                "Existe mas de una provincia local con la misma referencia externa SAP.");
        }

        var province = provinceMatches.SingleOrDefault();
        if (province is null)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapCityResultCodes.ProvinceNotFound,
                "La provincia SAP de la ciudad no tiene un vinculo local confirmado.");
        }
        if (province.CountryId != country.Id)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapCityResultCodes.ProvinceIdentityConflict,
                "La referencia externa de la provincia pertenece a otro pais local.");
        }

        var externalCode = SapCitySnapshot.BuildExternalCode(countryCode, provinceCode, cityCode);
        var cities = await GetLocalCitiesAsync(cancellationToken);
        var externalMatches = cities
            .Where(item => EqualsCode(item.ExternalSystem, SapExternalSystem)
                           && EqualsCode(item.ExternalCode, externalCode))
            .ToArray();
        if (externalMatches.Length > 1)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapCityResultCodes.IdentityConflict,
                "Existe mas de una ciudad local con la misma referencia externa SAP.");
        }

        var local = externalMatches.SingleOrDefault();
        if (local is not null
            && (local.CountryId != country.Id || local.ProvinceId != province.Id))
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                local, SapCityResultCodes.IdentityConflict,
                "La referencia externa de la ciudad apunta a una jerarquia local diferente.");
        }

        if (local is null)
        {
            var collision = cities.FirstOrDefault(item =>
                item.ProvinceId == province.Id && EqualsCode(item.Code, cityCode));
            if (collision is not null)
            {
                return Result(SapSyncExecutionDetailActions.Approval,
                    SapSyncExecutionDetailStatuses.ApprovalRequired, collision,
                    SapCityResultCodes.CodeCollisionApprovalRequired,
                    "Existe una ciudad con el mismo codigo en la provincia, pero su relacion SAP requiere aprobacion.");
            }

            var created = await sender.Send(new CreateCityCommand(
                country.Id,
                province.Id,
                cityCode,
                cityName,
                true,
                auditUserId,
                auditUserName,
                SapExternalSystem,
                externalCode), cancellationToken);
            if (!created.IsSuccess || created.Value is null)
            {
                return Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Failed,
                    null, SapCityResultCodes.SaveFailed, SafeMessage(created.Message));
            }

            UpdateCache(created.Value);
            return Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Created,
                created.Value, SapCityResultCodes.Created, "Ciudad creada desde SAP.");
        }

        if (EqualsText(snapshot.CityName, local.Name))
        {
            return Result(SapSyncExecutionDetailActions.NoChange, SapSyncExecutionDetailStatuses.Unchanged,
                local, SapCityResultCodes.Unchanged, "La ciudad local ya esta actualizada.");
        }

        var updated = await sender.Send(new UpdateCityCommand(
            local.Id,
            local.CountryId,
            local.ProvinceId,
            local.Code,
            cityName,
            local.IsActive,
            auditUserId,
            auditUserName), cancellationToken);
        if (!updated.IsSuccess || updated.Value is null)
        {
            return Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Failed,
                local, SapCityResultCodes.SaveFailed, SafeMessage(updated.Message));
        }

        UpdateCache(updated.Value);
        return Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Updated,
            updated.Value, SapCityResultCodes.Updated, "Ciudad actualizada desde SAP.");
    }

    public async Task<IReadOnlyCollection<CountryDto>> GetLocalCountriesAsync(CancellationToken cancellationToken = default)
    {
        localCountryCache ??= (await geographyRepository.GetCountriesAsync(cancellationToken)).ToList();
        return localCountryCache;
    }

    public async Task<IReadOnlyCollection<ProvinceDto>> GetLocalProvincesAsync(CancellationToken cancellationToken = default)
    {
        localProvinceCache ??= (await geographyRepository.GetProvincesAsync(cancellationToken)).ToList();
        return localProvinceCache;
    }

    public async Task<IReadOnlyCollection<CityDto>> GetLocalCitiesAsync(CancellationToken cancellationToken = default)
    {
        localCityCache ??= (await geographyRepository.GetCitiesAsync(cancellationToken)).ToList();
        return localCityCache;
    }

    private void UpdateCache(CityDto city)
    {
        localCityCache ??= [];
        localCityCache.RemoveAll(item => item.Id == city.Id);
        localCityCache.Add(city);
    }

    private static SapCityRecordProcessResult Result(
        string action, string status, CityDto? local, string resultCode, string safeMessage) =>
        new(action, status, local?.Id, local?.GlobalId, resultCode, safeMessage);

    private static bool EqualsCode(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static bool EqualsText(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    private static string SafeMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? "No fue posible guardar la ciudad." : message.Trim();

    private static string NormalizeCode(string? value) => Normalize(value).ToUpperInvariant();
    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
}
