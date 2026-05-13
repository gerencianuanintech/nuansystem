using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityForms.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityForms.Queries;

public sealed class GetSecurityFormByIdQueryHandler(ISecurityFormRepository formRepository)
    : IQueryHandler<GetSecurityFormByIdQuery, SecurityFormDto>
{
    public async Task<Result<SecurityFormDto>> Handle(GetSecurityFormByIdQuery request, CancellationToken cancellationToken)
    {
        var form = await formRepository.GetByIdAsync(request.Id, cancellationToken);
        return form is null
            ? Result<SecurityFormDto>.Failure("Formulario no encontrado.", [new ApiError("SecurityFormNotFound", "El formulario no existe.", nameof(request.Id))])
            : Result<SecurityFormDto>.Success(form);
    }
}
