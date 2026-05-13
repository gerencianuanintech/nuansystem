using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityRoles.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityRoles.Queries;

public sealed class GetSecurityRoleByIdQueryHandler(ISecurityRoleRepository roleRepository)
    : IQueryHandler<GetSecurityRoleByIdQuery, SecurityRoleDto>
{
    public async Task<Result<SecurityRoleDto>> Handle(
        GetSecurityRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        return role is null
            ? Result<SecurityRoleDto>.Failure(
                "Rol no encontrado.",
                [new ApiError("SecurityRoleNotFound", "El rol no existe.", nameof(request.Id))])
            : Result<SecurityRoleDto>.Success(role);
    }
}
