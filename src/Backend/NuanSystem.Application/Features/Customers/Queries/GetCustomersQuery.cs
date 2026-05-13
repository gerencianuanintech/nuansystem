using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Customers.Dtos;

namespace NuanSystem.Application.Features.Customers.Queries;

public sealed record GetCustomersQuery : IQuery<IReadOnlyCollection<CustomerDto>>;
