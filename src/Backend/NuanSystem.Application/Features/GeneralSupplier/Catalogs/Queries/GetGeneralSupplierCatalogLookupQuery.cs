using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Queries;

public sealed record GetGeneralSupplierCatalogLookupQuery(string CatalogKey)
    : IQuery<IReadOnlyCollection<GeneralSupplierCatalogLookupDto>>;

