using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed record GetSecurityDocumentSeriesFieldAccessQuery(
    int RoleId,
    string CompanyCode,
    int FormId,
    string DocumentType,
    int SecurityDocumentSeriesId,
    bool OnlyActive = true,
    string? Search = null) : IQuery<IReadOnlyCollection<SecurityFormFieldAccessDto>>;
