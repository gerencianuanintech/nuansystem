using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Customers.Commands;

public sealed class DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<DeleteCustomerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var current = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<bool>.Failure(
                "Cliente no encontrado.",
                new[] { new ApiError("CustomerNotFound", "No existe el cliente indicado.", nameof(request.Id)) });
        }

        var deleted = await customerRepository.SetActiveStateAsync(request.Id, false, cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Cliente eliminado correctamente.")
            : Result<bool>.Failure("No se pudo eliminar el cliente.");
    }
}
