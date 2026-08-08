using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

internal sealed record WarehouseGeography(
    int? CountryId,
    string? Country,
    int? ProvinceId,
    string? Province,
    int? CityId,
    string? City);

internal static class WarehouseGeographyResolver
{
    public static async Task<(WarehouseGeography? Value, ApiError? Error)> ResolveAsync(
        IGeographyRepository repository,
        int? countryId,
        int? provinceId,
        int? cityId,
        string? country,
        string? province,
        string? city,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        if (provinceId.HasValue && !countryId.HasValue)
        {
            return (null, new ApiError("WarehouseCountryRequired", "Seleccione el pais antes de la provincia.", nameof(countryId)));
        }

        if (cityId.HasValue && (!countryId.HasValue || !provinceId.HasValue))
        {
            return (null, new ApiError("WarehouseProvinceRequired", "Seleccione el pais y la provincia antes de la ciudad.", nameof(provinceId)));
        }

        var resolvedCountry = WarehouseCommandHelpers.NormalizeOptional(country);
        if (countryId.HasValue)
        {
            var item = await repository.GetCountryByIdAsync(countryId.Value, connection, transaction, cancellationToken);
            if (item is null)
            {
                return (null, new ApiError("WarehouseCountryNotFound", "El pais seleccionado no existe.", nameof(countryId)));
            }

            resolvedCountry = item.Name;
        }

        var resolvedProvince = WarehouseCommandHelpers.NormalizeOptional(province);
        if (provinceId.HasValue)
        {
            var item = await repository.GetProvinceByIdAsync(provinceId.Value, connection, transaction, cancellationToken);
            if (item is null)
            {
                return (null, new ApiError("WarehouseProvinceNotFound", "La provincia seleccionada no existe.", nameof(provinceId)));
            }

            if (item.CountryId != countryId)
            {
                return (null, new ApiError("WarehouseProvinceCountryMismatch", "La provincia no pertenece al pais seleccionado.", nameof(provinceId)));
            }

            resolvedProvince = item.Name;
        }

        var resolvedCity = WarehouseCommandHelpers.NormalizeOptional(city);
        if (cityId.HasValue)
        {
            var item = await repository.GetCityByIdAsync(cityId.Value, connection, transaction, cancellationToken);
            if (item is null)
            {
                return (null, new ApiError("WarehouseCityNotFound", "La ciudad seleccionada no existe.", nameof(cityId)));
            }

            if (item.CountryId != countryId || item.ProvinceId != provinceId)
            {
                return (null, new ApiError("WarehouseCityHierarchyMismatch", "La ciudad no pertenece al pais y provincia seleccionados.", nameof(cityId)));
            }

            resolvedCity = item.Name;
        }

        return (new WarehouseGeography(countryId, resolvedCountry, provinceId, resolvedProvince, cityId, resolvedCity), null);
    }
}
