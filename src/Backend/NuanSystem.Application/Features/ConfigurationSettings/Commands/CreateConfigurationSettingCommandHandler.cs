using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.ConfigurationSettings.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationSettings.Commands;

public sealed class CreateConfigurationSettingCommandHandler(IConfigurationSettingRepository settingRepository)
    : ICommandHandler<CreateConfigurationSettingCommand, ConfigurationSettingDto>
{
    public async Task<Result<ConfigurationSettingDto>> Handle(CreateConfigurationSettingCommand request, CancellationToken cancellationToken)
    {
        var key = request.Key.Trim();
        if (await settingRepository.ExistsByKeyAsync(key, cancellationToken))
        {
            return Result<ConfigurationSettingDto>.Failure("Ya existe un parametro con la clave indicada.", [new ApiError("ConfigurationSettingKeyAlreadyExists", "La clave ya existe.", nameof(request.Key))]);
        }

        var id = await settingRepository.CreateAsync(new CreateConfigurationSettingData(
            0, key, Clean(request.Value), Clean(request.Description), request.DataType.Trim(), Clean(request.Category),
            request.IsEncrypted, request.IsSystemParameter, request.IsEditable, request.DisplayOrder, Clean(request.DefaultValue),
            Clean(request.ValidationExpression), request.IsActive, request.AuditUserId, Clean(request.AuditUserName)), cancellationToken);

        var setting = await settingRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El parametro fue creado pero no pudo consultarse.");

        return Result<ConfigurationSettingDto>.Success(setting, "Parametro creado correctamente.");
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
