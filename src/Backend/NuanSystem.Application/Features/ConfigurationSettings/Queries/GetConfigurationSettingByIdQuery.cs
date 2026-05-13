using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.ConfigurationSettings.Dtos;

namespace NuanSystem.Application.Features.ConfigurationSettings.Queries;

public sealed record GetConfigurationSettingByIdQuery(int Id) : IQuery<ConfigurationSettingDto>;
