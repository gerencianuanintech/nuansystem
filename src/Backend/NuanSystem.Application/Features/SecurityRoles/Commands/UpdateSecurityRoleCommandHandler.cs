using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityRoles.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed class UpdateSecurityRoleCommandHandler(ISecurityRoleRepository roleRepository)
    : ICommandHandler<UpdateSecurityRoleCommand, SecurityRoleDto>
{
    public async Task<Result<SecurityRoleDto>> Handle(
        UpdateSecurityRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (await roleRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<SecurityRoleDto>.Failure(
                "Rol no encontrado.",
                [new ApiError("SecurityRoleNotFound", "El rol no existe.", nameof(request.Id))]);
        }

        var code = request.Code.Trim().ToUpperInvariant();
        if (await roleRepository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<SecurityRoleDto>.Failure(
                "Ya existe un rol con el codigo indicado.",
                [new ApiError("SecurityRoleCodeAlreadyExists", "El codigo del rol ya existe.", nameof(request.Code))]);
        }

        var name = request.Name.Trim();
        if (await roleRepository.ExistsByNameAsync(name, request.Id, cancellationToken))
        {
            return Result<SecurityRoleDto>.Failure(
                "Ya existe un rol con el nombre indicado.",
                [new ApiError("SecurityRoleNameAlreadyExists", "El nombre del rol ya existe.", nameof(request.Name))]);
        }

        await roleRepository.UpdateAsync(new UpdateSecurityRoleData(
            request.Id,
            code,
            name,
            request.Description?.Trim(),
            request.DisplayOrder,
            request.IsSystemRole,
            request.IsAssignable,
            request.IsActive,
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El rol fue actualizado pero no pudo consultarse.");

        return Result<SecurityRoleDto>.Success(role, "Rol actualizado correctamente.");
    }
}
