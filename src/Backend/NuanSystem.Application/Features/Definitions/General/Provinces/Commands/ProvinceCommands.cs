using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;

namespace NuanSystem.Application.Features.Definitions.General.Provinces.Commands;

public sealed record CreateProvinceCommand(int CountryId, string Code, string Name, bool IsActive, int? AuditUserId, string? AuditUserName, string? ExternalSystem = null, string? ExternalCode = null) : ICommand<ProvinceDto>;
public sealed record UpdateProvinceCommand(int Id, int CountryId, string Code, string Name, bool IsActive, int? AuditUserId, string? AuditUserName, string? ExternalSystem = null, string? ExternalCode = null) : ICommand<ProvinceDto>;
public sealed record DeleteProvinceCommand(int Id, int? AuditUserId, string? AuditUserName) : ICommand<bool>;
