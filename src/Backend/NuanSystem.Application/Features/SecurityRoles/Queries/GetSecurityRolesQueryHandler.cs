using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityRoles.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityRoles.Queries;

public sealed class GetSecurityRolesQueryHandler(ISecurityRoleRepository roleRepository)
    : IQueryHandler<GetSecurityRolesQuery, IReadOnlyCollection<SecurityRoleDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityRoleDto>>> Handle(
        GetSecurityRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<SecurityRoleDto>>.Success(roles);
    }
}
