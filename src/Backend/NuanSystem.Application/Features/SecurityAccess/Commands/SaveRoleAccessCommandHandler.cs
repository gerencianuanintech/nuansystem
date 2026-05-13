using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed class SaveRoleAccessCommandHandler(ISecurityAccessRepository securityAccessRepository)
    : ICommandHandler<SaveRoleAccessCommand, bool>
{
    public async Task<Result<bool>> Handle(SaveRoleAccessCommand request, CancellationToken cancellationToken)
    {
        await securityAccessRepository.SaveRoleAccessAsync(
            request.RoleId,
            request.Menus,
            request.Operations,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return Result<bool>.Success(true, "Accesos guardados correctamente.");
    }
}
