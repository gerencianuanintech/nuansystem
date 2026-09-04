using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;

namespace NuanSystem.Application.Features.BusinessPartners.Queries;

public sealed class GetBusinessPartnerLookupsQueryHandler(
    IBusinessPartnerRepository repository,
    ICompanyContext companyContext)
    : IQueryHandler<GetBusinessPartnerLookupsQuery, BusinessPartnerLookupsDto>
{
    public async Task<Result<BusinessPartnerLookupsDto>> Handle(GetBusinessPartnerLookupsQuery request, CancellationToken cancellationToken)
    {
        var lookups = await repository.GetLookupsAsync(cancellationToken);
        return Result<BusinessPartnerLookupsDto>.Success(
            lookups with { EditPolicy = BusinessPartnerWritePolicy.GetEditPolicy(companyContext.CurrentCompany) });
    }
}
