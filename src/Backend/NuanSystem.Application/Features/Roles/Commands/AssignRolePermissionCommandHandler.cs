using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.Roles.Commands;

public sealed class AssignRolePermissionCommandHandler(IRoleAdminRepository repository)
    : ICommandHandler<AssignRolePermissionCommand, bool>
{
    public async Task<Result<bool>> Handle(AssignRolePermissionCommand request, CancellationToken cancellationToken)
    {
        await repository.AssignPermissionAsync(request.RoleId, request.PermissionId, cancellationToken);
        return Result<bool>.Success(true, "Permiso asignado al rol correctamente.");
    }
}
