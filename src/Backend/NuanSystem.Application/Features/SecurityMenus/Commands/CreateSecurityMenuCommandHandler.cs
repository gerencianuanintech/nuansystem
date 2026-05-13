using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityMenus.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityMenus.Commands;

public sealed class CreateSecurityMenuCommandHandler(ISecurityMenuRepository menuRepository)
    : ICommandHandler<CreateSecurityMenuCommand, SecurityMenuDto>
{
    public async Task<Result<SecurityMenuDto>> Handle(CreateSecurityMenuCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await menuRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<SecurityMenuDto>.Failure(
                "Ya existe un menu con el codigo indicado.",
                [new ApiError("SecurityMenuCodeAlreadyExists", "El codigo de menu ya existe.", nameof(request.Code))]);
        }

        var id = await menuRepository.CreateAsync(new CreateSecurityMenuData(
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

        var menu = await menuRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El menu fue creado pero no pudo consultarse.");

        return Result<SecurityMenuDto>.Success(menu, "Menu creado correctamente.");
    }
}
