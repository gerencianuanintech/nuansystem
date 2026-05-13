using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityFields.Commands;

public sealed class DeleteSecurityFieldCommandHandler(ISecurityFieldRepository fieldRepository)
    : ICommandHandler<DeleteSecurityFieldCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteSecurityFieldCommand request, CancellationToken cancellationToken)
    {
        var deleted = await fieldRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Campo eliminado correctamente.")
            : Result<bool>.Failure("Campo no encontrado.", [new ApiError("SecurityFieldNotFound", "El campo no existe.", nameof(request.Id))]);
    }
}
