using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed record GetSecurityDocumentSeriesOperationsAccessQuery(
    int RoleId,
    string CompanyCode,
    string FormKey,
    string DocumentType,
    int SecurityDocumentSeriesId,
    bool OnlyActive = true,
    string? Search = null) : IQuery<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessDto>>;
