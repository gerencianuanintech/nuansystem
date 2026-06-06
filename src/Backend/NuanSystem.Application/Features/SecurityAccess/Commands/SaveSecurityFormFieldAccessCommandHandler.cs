using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed class SaveSecurityFormFieldAccessCommandHandler(ISecurityRoleFormFieldAccessRepository repository)
    : ICommandHandler<SaveSecurityFormFieldAccessCommand, bool>
{
    public async Task<Result<bool>> Handle(SaveSecurityFormFieldAccessCommand request, CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || request.FormId <= 0)
        {
            return Result<bool>.Failure("Debe seleccionar rol y formulario.");
        }

        await repository.SaveFieldsAsync(
            request.RoleId,
            request.FormId,
            request.Fields,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return Result<bool>.Success(true, "Accesos a campos guardados correctamente.");
    }
}
