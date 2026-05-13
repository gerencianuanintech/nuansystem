using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityUsers.Dtos;

namespace NuanSystem.Application.Features.SecurityUsers.Queries;

public sealed class GetRolesQueryHandler(IUserAdminRepository repository)
    : IQueryHandler<GetRolesQuery, IReadOnlyCollection<RoleDto>>
{
    public async Task<Result<IReadOnlyCollection<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await repository.GetRolesAsync(cancellationToken);
        return Result<IReadOnlyCollection<RoleDto>>.Success(roles);
    }
}

