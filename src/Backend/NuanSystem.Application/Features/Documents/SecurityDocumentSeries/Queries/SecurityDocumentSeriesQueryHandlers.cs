using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Queries;

public sealed class GetSecurityDocumentSeriesQueryHandler(
    ISecurityDocumentSeriesRepository seriesRepository)
    : IQueryHandler<GetSecurityDocumentSeriesQuery, IReadOnlyCollection<SecurityDocumentSeriesDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityDocumentSeriesDto>>> Handle(
        GetSecurityDocumentSeriesQuery request,
        CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetAllAsync(
            new SecurityDocumentSeriesFilterData(
                NormalizeOptional(request.Search),
                NormalizeOptional(request.DocumentType),
                request.IsActive),
            cancellationToken);

        return Result<IReadOnlyCollection<SecurityDocumentSeriesDto>>.Success(series);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

public sealed class GetSecurityDocumentSeriesByIdQueryHandler(
    ISecurityDocumentSeriesRepository seriesRepository)
    : IQueryHandler<GetSecurityDocumentSeriesByIdQuery, SecurityDocumentSeriesDto>
{
    public async Task<Result<SecurityDocumentSeriesDto>> Handle(
        GetSecurityDocumentSeriesByIdQuery request,
        CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (series is null)
        {
            return Result<SecurityDocumentSeriesDto>.Failure("La serie de documento no existe.");
        }

        return Result<SecurityDocumentSeriesDto>.Success(series);
    }
}

public sealed class GetSecurityDocumentSeriesLookupQueryHandler(
    ISecurityDocumentSeriesRepository seriesRepository)
    : IQueryHandler<GetSecurityDocumentSeriesLookupQuery, IReadOnlyCollection<SecurityDocumentSeriesLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityDocumentSeriesLookupDto>>> Handle(
        GetSecurityDocumentSeriesLookupQuery request,
        CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetLookupAsync(
            NormalizeOptional(request.DocumentType),
            cancellationToken);

        return Result<IReadOnlyCollection<SecurityDocumentSeriesLookupDto>>.Success(series);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
