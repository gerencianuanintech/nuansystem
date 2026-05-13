using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed record GetCurrentFormOperationsQuery(int UserId, string FormKey) : IQuery<IReadOnlyCollection<FormOperationAccessDto>>;
