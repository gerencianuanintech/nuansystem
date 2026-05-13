using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityForms.Dtos;

namespace NuanSystem.Application.Features.SecurityForms.Queries;

public sealed record GetSecurityFormByIdQuery(int Id) : IQuery<SecurityFormDto>;
