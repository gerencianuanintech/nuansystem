using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Queries;

public sealed class GetBusinessPartnerByIdQueryHandler(IBusinessPartnerRepository repository)
    : IQueryHandler<GetBusinessPartnerByIdQuery, BusinessPartnerDto>
{
    public async Task<Result<BusinessPartnerDto>> Handle(GetBusinessPartnerByIdQuery request, CancellationToken cancellationToken)
    {
        var partner = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (partner is null)
        {
            return Result<BusinessPartnerDto>.Failure(
                "Tercero comercial no encontrado.",
                [new ApiError("BusinessPartnerNotFound", "Tercero comercial no encontrado.", nameof(request.Id))]);
        }

        return Result<BusinessPartnerDto>.Success(partner);
    }
}
