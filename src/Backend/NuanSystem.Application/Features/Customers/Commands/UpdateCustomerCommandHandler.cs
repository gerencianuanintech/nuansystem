using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Customers.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Customers.Commands;

public sealed class UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<UpdateCustomerCommand, CustomerDto>
{
    public async Task<Result<CustomerDto>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var current = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<CustomerDto>.Failure(
                "Cliente no encontrado.",
                new[] { new ApiError("CustomerNotFound", "No existe el cliente indicado.", nameof(request.Id)) });
        }

        var code = request.Code.Trim().ToUpperInvariant();
        if (await customerRepository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<CustomerDto>.Failure(
                "Ya existe otro cliente con el codigo indicado.",
                new[] { new ApiError("CustomerCodeAlreadyExists", "El codigo de cliente ya existe.", nameof(request.Code)) });
        }

        var updated = await customerRepository.UpdateAsync(new UpdateCustomerData(
            request.Id,
            code,
            request.Name.Trim(),
            request.TaxIdentification?.Trim(),
            request.Email?.Trim(),
            request.Phone?.Trim(),
            request.AddressLine?.Trim(),
            request.IsActive), cancellationToken);

        if (!updated)
        {
            return Result<CustomerDto>.Failure("No se pudo actualizar el cliente.");
        }

        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El cliente fue actualizado pero no pudo consultarse.");

        return Result<CustomerDto>.Success(customer, "Cliente actualizado correctamente.");
    }
}
