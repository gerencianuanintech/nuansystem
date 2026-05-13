using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Customers.Dtos;

namespace NuanSystem.Application.Features.Customers.Queries;

public sealed class GetCustomersQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetCustomersQuery, IReadOnlyCollection<CustomerDto>>
{
    public async Task<Result<IReadOnlyCollection<CustomerDto>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<CustomerDto>>.Success(customers);
    }
}
