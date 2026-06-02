using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Queries;

public sealed record GetFinancialCatalogsQuery(string CatalogKey) : IQuery<IReadOnlyCollection<FinancialCatalogDto>>;
