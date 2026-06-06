using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed record GetSecurityFormFieldAccessQuery(
    int RoleId,
    int FormId,
    bool OnlyActive = true,
    string? Search = null) : IQuery<IReadOnlyCollection<SecurityFormFieldAccessDto>>;
