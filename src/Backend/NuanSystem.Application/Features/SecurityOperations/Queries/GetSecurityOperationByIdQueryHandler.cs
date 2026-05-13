using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityOperations.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityOperations.Queries;

public sealed class GetSecurityOperationByIdQueryHandler(ISecurityOperationRepository operationRepository)
    : IQueryHandler<GetSecurityOperationByIdQuery, SecurityOperationDto>
{
    public async Task<Result<SecurityOperationDto>> Handle(
        GetSecurityOperationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var operation = await operationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (operation is null)
        {
            return Result<SecurityOperationDto>.Failure(
                "Operacion no encontrada.",
                new[] { new ApiError("SecurityOperationNotFound", "La operacion no existe.", nameof(request.Id)) });
        }

        return Result<SecurityOperationDto>.Success(operation);
    }
}
