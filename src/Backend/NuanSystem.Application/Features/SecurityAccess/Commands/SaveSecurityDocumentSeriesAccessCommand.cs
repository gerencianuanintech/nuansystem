using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed record SaveSecurityDocumentSeriesAccessCommand(
    int RoleId,
    string CompanyCode,
    string FormKey,
    string DocumentType,
    int SecurityDocumentSeriesId,
    bool IsSelected,
    IReadOnlyCollection<SaveSecurityDocumentSeriesOperationAccessData> Operations,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;
