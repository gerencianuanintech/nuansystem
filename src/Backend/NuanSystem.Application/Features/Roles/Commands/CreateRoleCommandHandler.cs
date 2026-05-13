using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Roles.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Roles.Commands;

public sealed class CreateRoleCommandHandler(IRoleAdminRepository repository)
    : ICommandHandler<CreateRoleCommand, RoleAdminDto>
{
    public async Task<Result<RoleAdminDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await repository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<RoleAdminDto>.Failure(
                "Ya existe un rol con ese codigo.",
                new[] { new ApiError("RoleCodeAlreadyExists", "El codigo de rol ya existe.", nameof(request.Code)) });
        }

        var roleId = await repository.CreateRoleAsync(new CreateRoleData(
            code,
            request.Name.Trim(),
            string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            request.IsActive), cancellationToken);

        var role = await repository.GetRoleByIdAsync(roleId, cancellationToken)
            ?? throw new InvalidOperationException("El rol fue creado pero no pudo consultarse.");

        return Result<RoleAdminDto>.Success(role, "Rol creado correctamente.");
    }
}
