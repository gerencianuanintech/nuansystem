using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityUsers.Dtos;

namespace NuanSystem.Application.Features.SecurityUsers.Queries;

public sealed record GetUsersQuery : IQuery<IReadOnlyCollection<UserAdminDto>>;

