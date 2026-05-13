using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Settings.Dtos;

namespace NuanSystem.Application.Features.Settings.Commands;

public sealed class UpsertCompanyParameterCommandHandler(ICompanyParameterRepository repository)
    : ICommandHandler<UpsertCompanyParameterCommand, CompanyParameterDto>
{
    public async Task<Result<CompanyParameterDto>> Handle(
        UpsertCompanyParameterCommand request,
        CancellationToken cancellationToken)
    {
        var parameter = await repository.UpsertForCurrentCompanyAsync(
            new UpsertCompanyParameterData(
                request.Key.Trim(),
                string.IsNullOrWhiteSpace(request.Value) ? null : request.Value.Trim(),
                string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()),
            cancellationToken);

        return Result<CompanyParameterDto>.Success(parameter, "Parametro guardado correctamente.");
    }
}
