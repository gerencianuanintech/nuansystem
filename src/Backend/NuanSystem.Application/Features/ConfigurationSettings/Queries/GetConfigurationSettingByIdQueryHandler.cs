using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.ConfigurationSettings.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationSettings.Queries;

public sealed class GetConfigurationSettingByIdQueryHandler(IConfigurationSettingRepository settingRepository)
    : IQueryHandler<GetConfigurationSettingByIdQuery, ConfigurationSettingDto>
{
    public async Task<Result<ConfigurationSettingDto>> Handle(GetConfigurationSettingByIdQuery request, CancellationToken cancellationToken)
    {
        var setting = await settingRepository.GetByIdAsync(request.Id, cancellationToken);
        return setting is null
            ? Result<ConfigurationSettingDto>.Failure("Parametro no encontrado.", [new ApiError("ConfigurationSettingNotFound", "El parametro no existe.", nameof(request.Id))])
            : Result<ConfigurationSettingDto>.Success(setting);
    }
}
