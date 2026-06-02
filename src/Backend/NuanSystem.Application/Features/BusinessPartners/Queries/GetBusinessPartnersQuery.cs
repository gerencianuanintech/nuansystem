using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Features.BusinessPartners.Queries;

public sealed record GetBusinessPartnersQuery(string? PartnerType = null) : IQuery<IReadOnlyCollection<BusinessPartnerDto>>;
