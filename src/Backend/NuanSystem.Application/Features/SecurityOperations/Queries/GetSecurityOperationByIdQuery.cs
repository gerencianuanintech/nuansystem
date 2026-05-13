using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityOperations.Dtos;

namespace NuanSystem.Application.Features.SecurityOperations.Queries;

public sealed record GetSecurityOperationByIdQuery(int Id) : IQuery<SecurityOperationDto>;
