using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Commands;

public sealed record CreateCountryCommand(string Code, string Name, string? Iso2, string? Iso3, string? PhonePrefix, bool IsActive, int? AuditUserId, string? AuditUserName, string? ExternalSystem = null, string? ExternalCode = null) : ICommand<CountryDto>;
public sealed record UpdateCountryCommand(int Id, string Code, string Name, string? Iso2, string? Iso3, string? PhonePrefix, bool IsActive, int? AuditUserId, string? AuditUserName, string? ExternalSystem = null, string? ExternalCode = null) : ICommand<CountryDto>;
public sealed record DeleteCountryCommand(int Id, int? AuditUserId, string? AuditUserName) : ICommand<bool>;
