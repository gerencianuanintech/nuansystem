using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityRoles.Dtos;

namespace NuanSystem.Application.Features.SecurityRoles.Queries;

public sealed record GetSecurityRolesQuery : IQuery<IReadOnlyCollection<SecurityRoleDto>>;
