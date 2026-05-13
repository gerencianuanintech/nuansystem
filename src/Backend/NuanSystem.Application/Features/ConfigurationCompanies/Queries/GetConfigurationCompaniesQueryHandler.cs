using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Queries;

public sealed class GetConfigurationCompaniesQueryHandler(IConfigurationCompanyRepository companyRepository)
    : IQueryHandler<GetConfigurationCompaniesQuery, IReadOnlyCollection<ConfigurationCompanyDto>>
{
    public async Task<Result<IReadOnlyCollection<ConfigurationCompanyDto>>> Handle(
        GetConfigurationCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var companies = await companyRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<ConfigurationCompanyDto>>.Success(companies);
    }
}
