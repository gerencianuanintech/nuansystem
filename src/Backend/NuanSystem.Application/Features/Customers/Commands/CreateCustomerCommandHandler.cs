using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Customers.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Customers.Commands;

public sealed class CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    public async Task<Result<CustomerDto>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await customerRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<CustomerDto>.Failure(
                "Ya existe un cliente con el codigo indicado.",
                new[] { new ApiError("CustomerCodeAlreadyExists", "El codigo de cliente ya existe.", nameof(request.Code)) });
        }

        var id = await customerRepository.CreateAsync(new CreateCustomerData(
            code,
            request.Name.Trim(),
            request.TaxIdentification?.Trim(),
            request.Email?.Trim(),
            request.Phone?.Trim(),
            request.AddressLine?.Trim()), cancellationToken);

        var customer = await customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El cliente fue creado pero no pudo consultarse.");

        return Result<CustomerDto>.Success(customer, "Cliente creado correctamente.");
    }
}
