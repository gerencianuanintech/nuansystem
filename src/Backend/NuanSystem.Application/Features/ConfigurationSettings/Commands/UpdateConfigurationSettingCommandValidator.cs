using FluentValidation;

namespace NuanSystem.Application.Features.ConfigurationSettings.Commands;

public sealed class UpdateConfigurationSettingCommandValidator : AbstractValidator<UpdateConfigurationSettingCommand>
{
    public UpdateConfigurationSettingCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Key).NotEmpty().MaximumLength(120).Matches("^[A-Za-z0-9_.:-]+$");
        RuleFor(command => command.Description).MaximumLength(300);
        RuleFor(command => command.DataType).NotEmpty().MaximumLength(30);
        RuleFor(command => command.Category).MaximumLength(80);
        RuleFor(command => command.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(command => command.ValidationExpression).MaximumLength(300);
    }
}
