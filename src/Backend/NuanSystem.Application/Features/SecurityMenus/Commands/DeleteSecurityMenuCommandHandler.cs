using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityMenus.Commands;

public sealed class DeleteSecurityMenuCommandHandler(ISecurityMenuRepository menuRepository)
    : ICommandHandler<DeleteSecurityMenuCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteSecurityMenuCommand request, CancellationToken cancellationToken)
    {
        var deleted = await menuRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Menu eliminado correctamente.")
            : Result<bool>.Failure("Menu no encontrado.", [new ApiError("SecurityMenuNotFound", "El menu no existe.", nameof(request.Id))]);
    }
}
