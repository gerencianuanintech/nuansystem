using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.TaxCatalogs.Catalogs.Queries;

public sealed class GetTaxCatalogsQueryHandler(ITaxCatalogRepository catalogRepository)
    : IQueryHandler<GetTaxCatalogsQuery, IReadOnlyCollection<TaxCatalogDto>>
{
    public async Task<Result<IReadOnlyCollection<TaxCatalogDto>>> Handle(GetTaxCatalogsQuery request, CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetAllAsync(request.CatalogKey, cancellationToken);
        return Result<IReadOnlyCollection<TaxCatalogDto>>.Success(catalogs);
    }
}

public sealed class GetTaxCatalogByIdQueryHandler(ITaxCatalogRepository catalogRepository)
    : IQueryHandler<GetTaxCatalogByIdQuery, TaxCatalogDto>
{
    public async Task<Result<TaxCatalogDto>> Handle(GetTaxCatalogByIdQuery request, CancellationToken cancellationToken)
    {
        var catalog = await catalogRepository.GetByIdAsync(request.CatalogKey, request.Id, cancellationToken);
        return catalog is null
            ? Result<TaxCatalogDto>.Failure("No se encontro el catalogo tributario.", [new ApiError("TAX_CATALOG_NOT_FOUND", "El registro no existe.", nameof(request.Id))])
            : Result<TaxCatalogDto>.Success(catalog);
    }
}

public sealed class GetTaxCatalogLookupQueryHandler(ITaxCatalogRepository catalogRepository)
    : IQueryHandler<GetTaxCatalogLookupQuery, IReadOnlyCollection<TaxCatalogLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<TaxCatalogLookupDto>>> Handle(GetTaxCatalogLookupQuery request, CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetLookupAsync(request.CatalogKey, cancellationToken);
        return Result<IReadOnlyCollection<TaxCatalogLookupDto>>.Success(catalogs);
    }
}

public sealed class GetRetentionConceptsQueryHandler(ITaxCatalogRepository catalogRepository)
    : IQueryHandler<GetRetentionConceptsQuery, IReadOnlyCollection<RetentionConceptDto>>
{
    public async Task<Result<IReadOnlyCollection<RetentionConceptDto>>> Handle(GetRetentionConceptsQuery request, CancellationToken cancellationToken)
    {
        var concepts = await catalogRepository.GetRetentionConceptsAsync(cancellationToken);
        return Result<IReadOnlyCollection<RetentionConceptDto>>.Success(concepts);
    }
}

public sealed class GetRetentionConceptByIdQueryHandler(ITaxCatalogRepository catalogRepository)
    : IQueryHandler<GetRetentionConceptByIdQuery, RetentionConceptDto>
{
    public async Task<Result<RetentionConceptDto>> Handle(GetRetentionConceptByIdQuery request, CancellationToken cancellationToken)
    {
        var concept = await catalogRepository.GetRetentionConceptByIdAsync(request.Id, cancellationToken);
        return concept is null
            ? Result<RetentionConceptDto>.Failure("No se encontro el concepto de retencion.", [new ApiError("RETENTION_CONCEPT_NOT_FOUND", "El registro no existe.", nameof(request.Id))])
            : Result<RetentionConceptDto>.Success(concept);
    }
}

public sealed class GetRetentionConceptLookupQueryHandler(ITaxCatalogRepository catalogRepository)
    : IQueryHandler<GetRetentionConceptLookupQuery, IReadOnlyCollection<RetentionConceptLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<RetentionConceptLookupDto>>> Handle(GetRetentionConceptLookupQuery request, CancellationToken cancellationToken)
    {
        var concepts = await catalogRepository.GetRetentionConceptLookupAsync(cancellationToken);
        return Result<IReadOnlyCollection<RetentionConceptLookupDto>>.Success(concepts);
    }
}
