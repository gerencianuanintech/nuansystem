using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityUsers.Commands;

public sealed class DeleteUserCommandHandler(IUserAdminRepository repository)
    : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Usuario eliminado correctamente.")
            : Result<bool>.Failure("Usuario no encontrado.", [new ApiError("SecurityUserNotFound", "El usuario no existe.", nameof(request.Id))]);
    }
}

