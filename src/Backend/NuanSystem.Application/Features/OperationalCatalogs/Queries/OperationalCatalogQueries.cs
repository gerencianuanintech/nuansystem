using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.OperationalCatalogs.Dtos;

namespace NuanSystem.Application.Features.OperationalCatalogs.Queries;

public sealed record GetOperationalCatalogsQuery(
    string CatalogKey,
    string? Search,
    string? ParentCatalogKey,
    string? ParentCode,
    bool? IsActive) : IQuery<IReadOnlyCollection<OperationalCatalogDto>>;

public sealed record GetOperationalCatalogByIdQuery(string CatalogKey, int Id) : IQuery<OperationalCatalogDto>;

public sealed record GetOperationalCatalogLookupQuery(
    string CatalogKey,
    string? ParentCatalogKey,
    string? ParentCode,
    bool ActiveOnly = true) : IQuery<IReadOnlyCollection<OperationalCatalogLookupDto>>;
