using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityMenus.Dtos;

namespace NuanSystem.Application.Features.SecurityMenus.Queries;

public sealed record GetSecurityMenusQuery : IQuery<IReadOnlyCollection<SecurityMenuDto>>;
