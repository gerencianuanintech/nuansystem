using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Features.BusinessPartners.Queries;

public sealed class GetBusinessPartnerLookupsQueryHandler(IBusinessPartnerRepository repository)
    : IQueryHandler<GetBusinessPartnerLookupsQuery, BusinessPartnerLookupsDto>
{
    public async Task<Result<BusinessPartnerLookupsDto>> Handle(GetBusinessPartnerLookupsQuery request, CancellationToken cancellationToken)
    {
        var lookups = await repository.GetLookupsAsync(cancellationToken);
        return Result<BusinessPartnerLookupsDto>.Success(lookups);
    }
}
