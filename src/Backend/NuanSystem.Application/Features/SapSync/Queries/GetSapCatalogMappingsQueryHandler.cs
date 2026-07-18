using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed class GetSapCatalogMappingsQueryHandler(ICompanyContext companyContext, ISapCatalogMappingRepository repository)
    : IQueryHandler<GetSapCatalogMappingsQuery, IReadOnlyCollection<SapCatalogMappingDto>>
{
    public async Task<Result<IReadOnlyCollection<SapCatalogMappingDto>>> Handle(GetSapCatalogMappingsQuery request, CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
            return Result<IReadOnlyCollection<SapCatalogMappingDto>>.Failure("No hay empresa activa.", [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa.", "X-Company-Code")]);

        var rows = await repository.GetByCompanyIdAsync(companyContext.CurrentCompany!.CompanyId, cancellationToken);
        return Result<IReadOnlyCollection<SapCatalogMappingDto>>.Success(rows);
    }
}
