using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Queries;

public sealed record GetGeneralSupplierCatalogsQuery(string CatalogKey)
    : IQuery<IReadOnlyCollection<GeneralSupplierCatalogDto>>;

