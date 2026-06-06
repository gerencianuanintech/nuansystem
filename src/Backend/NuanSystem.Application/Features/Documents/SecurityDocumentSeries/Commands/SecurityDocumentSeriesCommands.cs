using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;

public sealed record CreateSecurityDocumentSeriesCommand(
    string DocumentType,
    string Code,
    string Name,
    string? Description,
    string Prefix,
    string Establishment,
    string EmissionPoint,
    int InitialNumber,
    int CurrentNumber,
    int NextNumber,
    int NumberLength,
    string? SapObjectType,
    int? SapSeriesId,
    string? SapSeriesName,
    bool IsDefault,
    bool IsActive,
    bool IsSapIntegrationActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SecurityDocumentSeriesDto>;

public sealed record UpdateSecurityDocumentSeriesCommand(
    int Id,
    string DocumentType,
    string Code,
    string Name,
    string? Description,
    string Prefix,
    string Establishment,
    string EmissionPoint,
    int InitialNumber,
    int CurrentNumber,
    int NextNumber,
    int NumberLength,
    string? SapObjectType,
    int? SapSeriesId,
    string? SapSeriesName,
    bool IsDefault,
    bool IsActive,
    bool IsSapIntegrationActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SecurityDocumentSeriesDto>;

public sealed record DeleteSecurityDocumentSeriesCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

public sealed record ReserveSecurityDocumentNumberCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ReserveSecurityDocumentNumberResult>;
