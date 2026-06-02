using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Features.BusinessPartners.Queries;

public sealed class GetBusinessPartnersQueryHandler(IBusinessPartnerRepository repository)
    : IQueryHandler<GetBusinessPartnersQuery, IReadOnlyCollection<BusinessPartnerDto>>
{
    public async Task<Result<IReadOnlyCollection<BusinessPartnerDto>>> Handle(GetBusinessPartnersQuery request, CancellationToken cancellationToken)
    {
        var partners = await repository.GetAllAsync(NormalizePartnerType(request.PartnerType), cancellationToken);
        return Result<IReadOnlyCollection<BusinessPartnerDto>>.Success(partners);
    }

    private static string? NormalizePartnerType(string? partnerType)
    {
        return string.IsNullOrWhiteSpace(partnerType) ? null : partnerType.Trim();
    }
}
