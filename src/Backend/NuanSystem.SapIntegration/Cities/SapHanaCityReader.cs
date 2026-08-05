using System.Data.Common;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Cities.Configuration;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SapIntegration.Hana;

namespace NuanSystem.SapIntegration.Cities;

public sealed class SapHanaCityReader(
    ISapCompanySettingsRepository settingsRepository,
    ISapHanaQueryClient hanaQueryClient) : ISapCityReader
{
    public async Task<IReadOnlyCollection<SapCityRecord>> GetCitiesAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var settings = await settingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
        var query = settings?.CitiesSelectQuery;
        if (!SapCitySelectQueryPolicy.TryValidate(query, out var error))
        {
            throw new InvalidOperationException($"SAP_CITY_QUERY_INVALID: {error}");
        }

        return await hanaQueryClient.QueryAsync(
            companyId,
            SapCitySelectQueryPolicy.Normalize(query!),
            Map,
            cancellationToken: cancellationToken);
    }

    private static SapCityRecord Map(DbDataReader reader) =>
        new(
            ReadRequiredString(reader, "CountryCode"),
            ReadRequiredString(reader, "ProvinceCode"),
            ReadRequiredString(reader, "CityCode"),
            ReadRequiredString(reader, "CityName"));

    private static string ReadRequiredString(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal)
            ? string.Empty
            : Convert.ToString(reader.GetValue(ordinal))?.Trim() ?? string.Empty;
    }
}
