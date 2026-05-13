using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityFields.Dtos;

namespace NuanSystem.Application.Features.SecurityFields.Queries;

public sealed class GetSecurityFieldsQueryHandler(ISecurityFieldRepository fieldRepository)
    : IQueryHandler<GetSecurityFieldsQuery, IReadOnlyCollection<SecurityFieldDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityFieldDto>>> Handle(GetSecurityFieldsQuery request, CancellationToken cancellationToken)
    {
        var fields = await fieldRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<SecurityFieldDto>>.Success(fields);
    }
}
