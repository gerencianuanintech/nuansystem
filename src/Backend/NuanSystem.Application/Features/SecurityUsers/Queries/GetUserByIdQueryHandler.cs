using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityUsers.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityUsers.Queries;

public sealed class GetUserByIdQueryHandler(IUserAdminRepository repository)
    : IQueryHandler<GetUserByIdQuery, UserAdminDto>
{
    public async Task<Result<UserAdminDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken);
        return user is null
            ? Result<UserAdminDto>.Failure("Usuario no encontrado.", [new ApiError("SecurityUserNotFound", "El usuario no existe.", nameof(request.Id))])
            : Result<UserAdminDto>.Success(user);
    }
}

