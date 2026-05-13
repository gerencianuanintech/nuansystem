using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Roles.Dtos;

namespace NuanSystem.Application.Features.Roles.Queries;

public sealed class GetRolesAdminQueryHandler(IRoleAdminRepository repository)
    : IQueryHandler<GetRolesAdminQuery, IReadOnlyCollection<RoleAdminDto>>
{
    public async Task<Result<IReadOnlyCollection<RoleAdminDto>>> Handle(
        GetRolesAdminQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await repository.GetRolesAsync(cancellationToken);
        return Result<IReadOnlyCollection<RoleAdminDto>>.Success(roles);
    }
}
