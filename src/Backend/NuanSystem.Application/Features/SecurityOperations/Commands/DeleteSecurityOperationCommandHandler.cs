using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityOperations.Commands;

public sealed class DeleteSecurityOperationCommandHandler(ISecurityOperationRepository operationRepository)
    : ICommandHandler<DeleteSecurityOperationCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteSecurityOperationCommand request, CancellationToken cancellationToken)
    {
        var deleted = await operationRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);
        if (!deleted)
        {
            return Result<bool>.Failure(
                "Operacion no encontrada.",
                new[] { new ApiError("SecurityOperationNotFound", "La operacion no existe.", nameof(request.Id)) });
        }

        return Result<bool>.Success(true, "Operacion eliminada correctamente.");
    }
}
