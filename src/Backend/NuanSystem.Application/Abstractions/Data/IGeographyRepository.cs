using System.Data;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IGeographyRepository : IRepository
{
    Task<IReadOnlyCollection<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken = default);

    Task<CountryPageDto> SearchCountriesAsync(CountryListFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default);

    Task<ProvincePageDto> SearchProvincesAsync(ProvinceListFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CityDto>> GetCitiesAsync(CancellationToken cancellationToken = default);

    Task<CityPageDto> SearchCitiesAsync(CityListFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeographyLookupDto>> GetCountryLookupAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeographyLookupDto>> GetProvinceLookupAsync(string? countryCode = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeographyLookupDto>> GetCityLookupAsync(string? countryCode = null, string? provinceCode = null, CancellationToken cancellationToken = default);

    Task<CountryDto?> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CountryDto?> GetCountryByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<ProvinceDto?> GetProvinceByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProvinceDto?> GetProvinceByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<CityDto?> GetCityByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CityDto?> GetCityByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> CountryCodeExistsAsync(string code, int? excludingId = null, CancellationToken cancellationToken = default);

    Task<bool> CountryCodeExistsAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> ProvinceCodeExistsAsync(int countryId, string code, int? excludingId = null, CancellationToken cancellationToken = default);

    Task<bool> ProvinceCodeExistsAsync(int countryId, string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> CityCodeExistsAsync(int provinceId, string code, int? excludingId = null, CancellationToken cancellationToken = default);

    Task<bool> CityCodeExistsAsync(int provinceId, string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<int> CreateCountryAsync(SaveCountryData data, CancellationToken cancellationToken = default);

    Task<int> CreateCountryAsync(SaveCountryData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<int> CreateProvinceAsync(SaveProvinceData data, CancellationToken cancellationToken = default);

    Task<int> CreateProvinceAsync(SaveProvinceData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<int> CreateCityAsync(SaveCityData data, CancellationToken cancellationToken = default);

    Task<int> CreateCityAsync(SaveCityData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> UpdateCountryAsync(SaveCountryData data, CancellationToken cancellationToken = default);

    Task<bool> UpdateCountryAsync(SaveCountryData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> UpdateProvinceAsync(SaveProvinceData data, CancellationToken cancellationToken = default);

    Task<bool> UpdateProvinceAsync(SaveProvinceData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> UpdateCityAsync(SaveCityData data, CancellationToken cancellationToken = default);

    Task<bool> UpdateCityAsync(SaveCityData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> DeleteCountryAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default);

    Task<bool> DeleteCountryAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> DeleteProvinceAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default);

    Task<bool> DeleteProvinceAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> DeleteCityAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default);

    Task<bool> DeleteCityAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
