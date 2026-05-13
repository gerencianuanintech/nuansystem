using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Settings.Dtos;

namespace NuanSystem.Application.Features.Settings.Queries;

public sealed class GetCompanyParametersQueryHandler(ICompanyParameterRepository repository)
    : IQueryHandler<GetCompanyParametersQuery, IReadOnlyCollection<CompanyParameterDto>>
{
    public async Task<Result<IReadOnlyCollection<CompanyParameterDto>>> Handle(
        GetCompanyParametersQuery request,
        CancellationToken cancellationToken)
    {
        var parameters = await repository.GetForCurrentCompanyAsync(cancellationToken);
        return Result<IReadOnlyCollection<CompanyParameterDto>>.Success(parameters);
    }
}
