using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Companies.Dtos;

namespace NuanSystem.Application.Features.Companies.Queries;

public sealed class GetCompaniesQueryHandler(ICompanyAdminRepository companyRepository)
    : IQueryHandler<GetCompaniesQuery, IReadOnlyCollection<CompanyDto>>
{
    public async Task<Result<IReadOnlyCollection<CompanyDto>>> Handle(
        GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var companies = await companyRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<CompanyDto>>.Success(companies);
    }
}
