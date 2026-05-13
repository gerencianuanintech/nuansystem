using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Roles.Dtos;

namespace NuanSystem.Application.Features.Roles.Queries;

public sealed class GetPermissionsQueryHandler(IRoleAdminRepository repository)
    : IQueryHandler<GetPermissionsQuery, IReadOnlyCollection<PermissionDto>>
{
    public async Task<Result<IReadOnlyCollection<PermissionDto>>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await repository.GetPermissionsAsync(cancellationToken);
        return Result<IReadOnlyCollection<PermissionDto>>.Success(permissions);
    }
}
