using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Features.TaxCatalogs.Catalogs.Queries;

public sealed record GetTaxCatalogsQuery(string CatalogKey) : IQuery<IReadOnlyCollection<TaxCatalogDto>>;

public sealed record GetTaxCatalogByIdQuery(string CatalogKey, int Id) : IQuery<TaxCatalogDto>;

public sealed record GetTaxCatalogLookupQuery(string CatalogKey) : IQuery<IReadOnlyCollection<TaxCatalogLookupDto>>;

public sealed record GetRetentionConceptsQuery : IQuery<IReadOnlyCollection<RetentionConceptDto>>;

public sealed record GetRetentionConceptByIdQuery(int Id) : IQuery<RetentionConceptDto>;

public sealed record GetRetentionConceptLookupQuery : IQuery<IReadOnlyCollection<RetentionConceptLookupDto>>;
