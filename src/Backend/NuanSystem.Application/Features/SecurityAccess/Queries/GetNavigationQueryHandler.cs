using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetNavigationQueryHandler(ISecurityAccessRepository securityAccessRepository)
    : IQueryHandler<GetNavigationQuery, IReadOnlyCollection<NavigationMenuDto>>
{
    public async Task<Result<IReadOnlyCollection<NavigationMenuDto>>> Handle(GetNavigationQuery request, CancellationToken cancellationToken)
    {
        var menus = await securityAccessRepository.GetNavigationAsync(request.UserId, cancellationToken);
        return Result<IReadOnlyCollection<NavigationMenuDto>>.Success(menus);
    }
}
