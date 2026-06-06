using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed record GetSecurityFormAccessFormsQuery(
    int? FormType,
    bool OnlyActive = true,
    string? Search = null) : IQuery<IReadOnlyCollection<SecurityFormAccessFormDto>>;
