using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.General.Provinces.Queries;

public sealed class GetProvincesQueryHandler(IGeographyRepository repository) : IQueryHandler<GetProvincesQuery, IReadOnlyCollection<ProvinceDto>>
{
    public async Task<Result<IReadOnlyCollection<ProvinceDto>>> Handle(GetProvincesQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<ProvinceDto>>.Success(await repository.GetProvincesAsync(cancellationToken));
}

public sealed class SearchProvincesQueryHandler(IGeographyRepository repository) : IQueryHandler<SearchProvincesQuery, ProvincePageDto>
{
    public async Task<Result<ProvincePageDto>> Handle(SearchProvincesQuery request, CancellationToken cancellationToken)
    {
        var page = await repository.SearchProvincesAsync(
            new ProvinceListFilter(request.Search, request.PageNumber, request.PageSize),
            cancellationToken);
        return Result<ProvincePageDto>.Success(page);
    }
}

public sealed class GetProvinceLookupQueryHandler(IGeographyRepository repository) : IQueryHandler<GetProvinceLookupQuery, IReadOnlyCollection<GeographyLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeographyLookupDto>>> Handle(GetProvinceLookupQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<GeographyLookupDto>>.Success(await repository.GetProvinceLookupAsync(request.CountryCode, cancellationToken));
}

public sealed class GetProvinceByIdQueryHandler(IGeographyRepository repository) : IQueryHandler<GetProvinceByIdQuery, ProvinceDto>
{
    public async Task<Result<ProvinceDto>> Handle(GetProvinceByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetProvinceByIdAsync(request.Id, cancellationToken);
        return item is null ? Result<ProvinceDto>.Failure("No se encontro la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe.", nameof(request.Id))]) : Result<ProvinceDto>.Success(item);
    }
}
