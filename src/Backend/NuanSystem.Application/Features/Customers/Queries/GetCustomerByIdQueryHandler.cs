using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Customers.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Customers.Queries;

public sealed class GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetCustomerByIdQuery, CustomerDto>
{
    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerDto>.Failure(
                "Cliente no encontrado.",
                new[] { new ApiError("CustomerNotFound", "No existe el cliente indicado.", nameof(request.Id)) });
        }

        return Result<CustomerDto>.Success(customer);
    }
}
