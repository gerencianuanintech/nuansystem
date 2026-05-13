using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityForms.Commands;

public sealed class DeleteSecurityFormCommandHandler(ISecurityFormRepository formRepository)
    : ICommandHandler<DeleteSecurityFormCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteSecurityFormCommand request, CancellationToken cancellationToken)
    {
        var deleted = await formRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Formulario eliminado correctamente.")
            : Result<bool>.Failure("Formulario no encontrado.", [new ApiError("SecurityFormNotFound", "El formulario no existe.", nameof(request.Id))]);
    }
}
