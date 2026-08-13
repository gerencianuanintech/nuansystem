using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Commands;

public sealed record CreateUnitMeasureCommand(
    string Code, string Name, string? Description, string? Symbol, string MagnitudeCode,
    int SortOrder, bool IsActive, string? ExternalSystem, string? ExternalCode,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<UnitMeasureDto>;

public sealed record UpdateUnitMeasureCommand(
    int Id, string Code, string Name, string? Description, string? Symbol, string MagnitudeCode,
    int SortOrder, bool IsActive, string? ExternalSystem, string? ExternalCode,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<UnitMeasureDto>;

public sealed record DeleteUnitMeasureCommand(
    int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;
