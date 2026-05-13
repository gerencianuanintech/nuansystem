using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetRoleAccessQueryHandler(ISecurityAccessRepository securityAccessRepository)
    : IQueryHandler<GetRoleAccessQuery, RoleAccessDto>
{
    public async Task<Result<RoleAccessDto>> Handle(GetRoleAccessQuery request, CancellationToken cancellationToken)
    {
        var access = await securityAccessRepository.GetRoleAccessAsync(request.RoleId, cancellationToken);
        return Result<RoleAccessDto>.Success(access);
    }
}
