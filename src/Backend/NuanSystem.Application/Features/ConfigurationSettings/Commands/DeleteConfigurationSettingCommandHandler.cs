using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationSettings.Commands;

public sealed class DeleteConfigurationSettingCommandHandler(IConfigurationSettingRepository settingRepository)
    : ICommandHandler<DeleteConfigurationSettingCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteConfigurationSettingCommand request, CancellationToken cancellationToken)
    {
        var setting = await settingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (setting is null)
        {
            return Result<bool>.Failure("Parametro no encontrado.", [new ApiError("ConfigurationSettingNotFound", "El parametro no existe.", nameof(request.Id))]);
        }

        if (setting.IsSystemParameter)
        {
            return Result<bool>.Failure("No se puede eliminar un parametro del sistema.", [new ApiError("ConfigurationSettingSystemParameter", "El parametro esta protegido.", nameof(request.Id))]);
        }

        await settingRepository.DeleteAsync(request.Id, request.AuditUserId, request.AuditUserName?.Trim(), cancellationToken);
        return Result<bool>.Success(true, "Parametro eliminado correctamente.");
    }
}
