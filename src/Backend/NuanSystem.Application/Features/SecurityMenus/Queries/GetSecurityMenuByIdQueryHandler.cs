using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityMenus.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityMenus.Queries;

public sealed class GetSecurityMenuByIdQueryHandler(ISecurityMenuRepository menuRepository)
    : IQueryHandler<GetSecurityMenuByIdQuery, SecurityMenuDto>
{
    public async Task<Result<SecurityMenuDto>> Handle(GetSecurityMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetByIdAsync(request.Id, cancellationToken);
        return menu is null
            ? Result<SecurityMenuDto>.Failure("Menu no encontrado.", [new ApiError("SecurityMenuNotFound", "El menu no existe.", nameof(request.Id))])
            : Result<SecurityMenuDto>.Success(menu);
    }
}
