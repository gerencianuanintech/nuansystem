using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Queries;

public sealed class GetConfigurationCompanyByIdQueryHandler(IConfigurationCompanyRepository companyRepository)
    : IQueryHandler<GetConfigurationCompanyByIdQuery, ConfigurationCompanyDto>
{
    public async Task<Result<ConfigurationCompanyDto>> Handle(
        GetConfigurationCompanyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByIdAsync(request.Id, cancellationToken);
        return company is null
            ? Result<ConfigurationCompanyDto>.Failure(
                "Compania no encontrada.",
                [new ApiError("ConfigurationCompanyNotFound", "La compania no existe.", nameof(request.Id))])
            : Result<ConfigurationCompanyDto>.Success(company);
    }
}
