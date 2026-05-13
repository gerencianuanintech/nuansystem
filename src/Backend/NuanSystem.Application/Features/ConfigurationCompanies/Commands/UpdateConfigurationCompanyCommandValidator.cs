using FluentValidation;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed class UpdateConfigurationCompanyCommandValidator : AbstractValidator<UpdateConfigurationCompanyCommand>
{
    public UpdateConfigurationCompanyCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

        Include(new ConfigurationCompanyCommandValidator<UpdateConfigurationCompanyCommand>());
    }
}
