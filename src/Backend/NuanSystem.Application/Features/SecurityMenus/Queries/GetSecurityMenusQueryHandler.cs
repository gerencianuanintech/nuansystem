using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityMenus.Dtos;

namespace NuanSystem.Application.Features.SecurityMenus.Queries;

public sealed class GetSecurityMenusQueryHandler(ISecurityMenuRepository menuRepository)
    : IQueryHandler<GetSecurityMenusQuery, IReadOnlyCollection<SecurityMenuDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityMenuDto>>> Handle(GetSecurityMenusQuery request, CancellationToken cancellationToken)
    {
        var menus = await menuRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<SecurityMenuDto>>.Success(menus);
    }
}
