using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Queries;

public sealed record GetGeneralInventoryCatalogByIdQuery(string CatalogKey, int Id)
    : IQuery<GeneralInventoryCatalogDto>;
