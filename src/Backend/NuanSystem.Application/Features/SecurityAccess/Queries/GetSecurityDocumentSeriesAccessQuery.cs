using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed record GetSecurityDocumentSeriesAccessQuery(
    int RoleId,
    string CompanyCode,
    string FormKey,
    string? Search,
    string? DocumentType,
    bool? IsActive) : IQuery<IReadOnlyCollection<SecurityDocumentSeriesAccessDto>>;
