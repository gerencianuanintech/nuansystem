using FluentValidation;

namespace NuanSystem.Application.Features.ConfigurationSettings.Commands;

public sealed class DeleteConfigurationSettingCommandValidator : AbstractValidator<DeleteConfigurationSettingCommand>
{
    public DeleteConfigurationSettingCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
