using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;

namespace NuanSystem.Application.Features.Definitions.General.Cities.Commands;

public sealed record CreateCityCommand(int CountryId, int ProvinceId, string Code, string Name, bool IsActive, int? AuditUserId, string? AuditUserName, string? ExternalSystem = null, string? ExternalCode = null) : ICommand<CityDto>;
public sealed record UpdateCityCommand(int Id, int CountryId, int ProvinceId, string Code, string Name, bool IsActive, int? AuditUserId, string? AuditUserName) : ICommand<CityDto>;
public sealed record DeleteCityCommand(int Id, int? AuditUserId, string? AuditUserName) : ICommand<bool>;
