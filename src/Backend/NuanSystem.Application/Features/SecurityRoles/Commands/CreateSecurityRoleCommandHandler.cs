using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityRoles.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed class CreateSecurityRoleCommandHandler(ISecurityRoleRepository roleRepository)
    : ICommandHandler<CreateSecurityRoleCommand, SecurityRoleDto>
{
    public async Task<Result<SecurityRoleDto>> Handle(
        CreateSecurityRoleCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await roleRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<SecurityRoleDto>.Failure(
                "Ya existe un rol con el codigo indicado.",
                [new ApiError("SecurityRoleCodeAlreadyExists", "El codigo del rol ya existe.", nameof(request.Code))]);
        }

        var name = request.Name.Trim();
        if (await roleRepository.ExistsByNameAsync(name, cancellationToken))
        {
            return Result<SecurityRoleDto>.Failure(
                "Ya existe un rol con el nombre indicado.",
                [new ApiError("SecurityRoleNameAlreadyExists", "El nombre del rol ya existe.", nameof(request.Name))]);
        }

        var id = await roleRepository.CreateAsync(new CreateSecurityRoleData(
            code,
            name,
            request.Description?.Trim(),
            request.DisplayOrder,
            request.IsSystemRole,
            request.IsAssignable,
            request.IsActive,
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var role = await roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El rol fue creado pero no pudo consultarse.");

        return Result<SecurityRoleDto>.Success(role, "Rol creado correctamente.");
    }
}
