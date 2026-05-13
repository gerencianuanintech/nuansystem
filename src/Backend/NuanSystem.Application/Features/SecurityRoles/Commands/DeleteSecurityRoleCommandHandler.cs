using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed class DeleteSecurityRoleCommandHandler(ISecurityRoleRepository roleRepository)
    : ICommandHandler<DeleteSecurityRoleCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteSecurityRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
        {
            return Result<bool>.Failure(
                "Rol no encontrado.",
                [new ApiError("SecurityRoleNotFound", "El rol no existe.", nameof(request.Id))]);
        }

        if (role.IsSystemRole)
        {
            return Result<bool>.Failure(
                "No se puede eliminar un rol del sistema.",
                [new ApiError("SecurityRoleSystemRole", "El rol esta protegido por el sistema.", nameof(request.Id))]);
        }

        await roleRepository.DeleteAsync(request.Id, request.AuditUserId, request.AuditUserName?.Trim(), cancellationToken);
        return Result<bool>.Success(true, "Rol eliminado correctamente.");
    }
}
