using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;

namespace NuanSystem.Application.Features.TaxCatalogs.Taxes.Queries;

public sealed record GetTaxesQuery : IQuery<IReadOnlyCollection<TaxDto>>;
public sealed record GetTaxLookupQuery : IQuery<IReadOnlyCollection<TaxLookupDto>>;
public sealed record GetTaxByIdQuery(int Id) : IQuery<TaxDto>;
public sealed record GetTaxHistoryQuery(int Id) : IQuery<IReadOnlyCollection<TaxAuditChangeDto>>;
