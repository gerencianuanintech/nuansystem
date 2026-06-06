using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed record SaveSecurityDocumentSeriesFieldAccessCommand(
    int RoleId,
    string CompanyCode,
    int FormId,
    string DocumentType,
    int SecurityDocumentSeriesId,
    IReadOnlyCollection<SaveSecurityFormFieldAccessData> Fields,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;
