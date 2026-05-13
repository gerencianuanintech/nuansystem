using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityMenus.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityMenus.Commands;

public sealed class UpdateSecurityMenuCommandHandler(ISecurityMenuRepository menuRepository)
    : ICommandHandler<UpdateSecurityMenuCommand, SecurityMenuDto>
{
    public async Task<Result<SecurityMenuDto>> Handle(UpdateSecurityMenuCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentId == request.Id)
        {
            return Result<SecurityMenuDto>.Failure(
                "Un menu no puede ser padre de si mismo.",
                [new ApiError("SecurityMenuInvalidParent", "El padre seleccionado no es valido.", nameof(request.ParentId))]);
        }

        if (await menuRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<SecurityMenuDto>.Failure(
                "Menu no encontrado.",
                [new ApiError("SecurityMenuNotFound", "El menu no existe.", nameof(request.Id))]);
        }

        var code = request.Code.Trim().ToUpperInvariant();
        if (await menuRepository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<SecurityMenuDto>.Failure(
                "Ya existe un menu con el codigo indicado.",
                [new ApiError("SecurityMenuCodeAlreadyExists", "El codigo de menu ya existe.", nameof(request.Code))]);
        }

        await menuRepository.UpdateAsync(new UpdateSecurityMenuData(
            request.Id,
            request.ParentId,
            code,
            request.Name.Trim(),
            request.Description?.Trim(),
            request.MenuType,
            request.FormKey?.Trim(),
            request.IconLarge?.Trim(),
            request.IconSmall?.Trim(),
            request.DisplayOrder,
            request.IsVisible,
            request.IsActive,
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var menu = await menuRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El menu fue actualizado pero no pudo consultarse.");

        return Result<SecurityMenuDto>.Success(menu, "Menu actualizado correctamente.");
    }
}
