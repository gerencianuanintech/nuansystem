using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.ConfigurationSettings.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationSettings.Commands;

public sealed class UpdateConfigurationSettingCommandHandler(IConfigurationSettingRepository settingRepository)
    : ICommandHandler<UpdateConfigurationSettingCommand, ConfigurationSettingDto>
{
    public async Task<Result<ConfigurationSettingDto>> Handle(UpdateConfigurationSettingCommand request, CancellationToken cancellationToken)
    {
        var current = await settingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<ConfigurationSettingDto>.Failure("Parametro no encontrado.", [new ApiError("ConfigurationSettingNotFound", "El parametro no existe.", nameof(request.Id))]);
        }

        if (!current.IsEditable)
        {
            return Result<ConfigurationSettingDto>.Failure("El parametro no permite edicion.", [new ApiError("ConfigurationSettingNotEditable", "El parametro esta bloqueado para edicion.", nameof(request.Id))]);
        }

        var key = request.Key.Trim();
        if (await settingRepository.ExistsByKeyAsync(key, request.Id, cancellationToken))
        {
            return Result<ConfigurationSettingDto>.Failure("Ya existe un parametro con la clave indicada.", [new ApiError("ConfigurationSettingKeyAlreadyExists", "La clave ya existe.", nameof(request.Key))]);
        }

        await settingRepository.UpdateAsync(new UpdateConfigurationSettingData(
            0, request.Id, key, Clean(request.Value), Clean(request.Description), request.DataType.Trim(), Clean(request.Category),
            request.IsEncrypted, request.IsSystemParameter, request.IsEditable, request.DisplayOrder, Clean(request.DefaultValue),
            Clean(request.ValidationExpression), request.IsActive, request.AuditUserId, Clean(request.AuditUserName)), cancellationToken);

        var setting = await settingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El parametro fue actualizado pero no pudo consultarse.");

        return Result<ConfigurationSettingDto>.Success(setting, "Parametro actualizado correctamente.");
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
