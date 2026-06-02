using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Geography.Dtos;

namespace NuanSystem.Application.Features.Geography.Commands;

public sealed record CreateCountryCommand(
    string Code,
    string Name,
    string? Iso2,
    string? Iso3,
    string? PhonePrefix,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName) : ICommand<CountryDto>;

public sealed record UpdateCountryCommand(
    int Id,
    string Code,
    string Name,
    string? Iso2,
    string? Iso3,
    string? PhonePrefix,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName) : ICommand<CountryDto>;

public sealed record DeleteCountryCommand(int Id, int? AuditUserId, string? AuditUserName) : ICommand<bool>;

public sealed record CreateProvinceCommand(
    int CountryId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName) : ICommand<ProvinceDto>;

public sealed record UpdateProvinceCommand(
    int Id,
    int CountryId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName) : ICommand<ProvinceDto>;

public sealed record DeleteProvinceCommand(int Id, int? AuditUserId, string? AuditUserName) : ICommand<bool>;

public sealed record CreateCityCommand(
    int CountryId,
    int ProvinceId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName) : ICommand<CityDto>;

public sealed record UpdateCityCommand(
    int Id,
    int CountryId,
    int ProvinceId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName) : ICommand<CityDto>;

public sealed record DeleteCityCommand(int Id, int? AuditUserId, string? AuditUserName) : ICommand<bool>;
