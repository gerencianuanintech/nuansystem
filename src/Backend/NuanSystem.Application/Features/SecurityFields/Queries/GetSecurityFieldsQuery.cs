using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityFields.Dtos;

namespace NuanSystem.Application.Features.SecurityFields.Queries;

public sealed record GetSecurityFieldsQuery : IQuery<IReadOnlyCollection<SecurityFieldDto>>;
