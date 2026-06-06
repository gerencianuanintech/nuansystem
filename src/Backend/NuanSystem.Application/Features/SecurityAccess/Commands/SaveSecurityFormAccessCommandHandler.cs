using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed class SaveSecurityFormAccessCommandHandler(ISecurityRoleFormAccessRepository repository)
    : ICommandHandler<SaveSecurityFormAccessCommand, bool>
{
    public async Task<Result<bool>> Handle(SaveSecurityFormAccessCommand request, CancellationToken cancellationToken)
    {
        await repository.SaveOperationsAsync(
            request.RoleId,
            request.FormId,
            request.Operations,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return Result<bool>.Success(true, "Accesos guardados correctamente.");
    }
}
