using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityUsers.Dtos;

namespace NuanSystem.Application.Features.SecurityUsers.Queries;

public sealed class GetUsersQueryHandler(IUserAdminRepository repository)
    : IQueryHandler<GetUsersQuery, IReadOnlyCollection<UserAdminDto>>
{
    public async Task<Result<IReadOnlyCollection<UserAdminDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await repository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<UserAdminDto>>.Success(users);
    }
}

