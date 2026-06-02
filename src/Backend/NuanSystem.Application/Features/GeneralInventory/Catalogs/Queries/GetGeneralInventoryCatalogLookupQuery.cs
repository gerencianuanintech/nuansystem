using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Queries;

public sealed record GetGeneralInventoryCatalogLookupQuery(string CatalogKey)
    : IQuery<IReadOnlyCollection<GeneralInventoryCatalogLookupDto>>;
