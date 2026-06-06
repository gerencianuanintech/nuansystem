using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Queries;

public sealed record GetSecurityDocumentSeriesQuery(
    string? Search,
    string? DocumentType,
    bool? IsActive) : IQuery<IReadOnlyCollection<SecurityDocumentSeriesDto>>;

public sealed record GetSecurityDocumentSeriesByIdQuery(int Id) : IQuery<SecurityDocumentSeriesDto>;

public sealed record GetSecurityDocumentSeriesLookupQuery(string? DocumentType)
    : IQuery<IReadOnlyCollection<SecurityDocumentSeriesLookupDto>>;
