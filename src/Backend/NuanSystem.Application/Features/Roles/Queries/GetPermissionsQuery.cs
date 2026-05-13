using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Roles.Dtos;

namespace NuanSystem.Application.Features.Roles.Queries;

public sealed record GetPermissionsQuery : IQuery<IReadOnlyCollection<PermissionDto>>;
