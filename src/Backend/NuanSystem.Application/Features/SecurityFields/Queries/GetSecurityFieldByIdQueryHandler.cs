using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityFields.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityFields.Queries;

public sealed class GetSecurityFieldByIdQueryHandler(ISecurityFieldRepository fieldRepository)
    : IQueryHandler<GetSecurityFieldByIdQuery, SecurityFieldDto>
{
    public async Task<Result<SecurityFieldDto>> Handle(GetSecurityFieldByIdQuery request, CancellationToken cancellationToken)
    {
        var field = await fieldRepository.GetByIdAsync(request.Id, cancellationToken);
        return field is null
            ? Result<SecurityFieldDto>.Failure("Campo no encontrado.", [new ApiError("SecurityFieldNotFound", "El campo no existe.", nameof(request.Id))])
            : Result<SecurityFieldDto>.Success(field);
    }
}
