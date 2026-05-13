using FluentValidation;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed class DeleteConfigurationCompanyCommandValidator : AbstractValidator<DeleteConfigurationCompanyCommand>
{
    public DeleteConfigurationCompanyCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}
