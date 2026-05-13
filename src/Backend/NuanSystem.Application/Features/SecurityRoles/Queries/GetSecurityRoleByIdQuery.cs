using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityRoles.Dtos;

namespace NuanSystem.Application.Features.SecurityRoles.Queries;

public sealed record GetSecurityRoleByIdQuery(int Id) : IQuery<SecurityRoleDto>;
