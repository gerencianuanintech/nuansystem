using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.ConfigurationSettings.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationSettings.Queries;

public sealed class GetConfigurationSettingsQueryHandler(IConfigurationSettingRepository settingRepository)
    : IQueryHandler<GetConfigurationSettingsQuery, IReadOnlyCollection<ConfigurationSettingDto>>
{
    public async Task<Result<IReadOnlyCollection<ConfigurationSettingDto>>> Handle(GetConfigurationSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await settingRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<ConfigurationSettingDto>>.Success(settings);
    }
}
