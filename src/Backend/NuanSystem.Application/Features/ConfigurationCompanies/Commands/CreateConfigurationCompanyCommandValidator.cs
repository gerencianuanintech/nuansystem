using FluentValidation;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed class CreateConfigurationCompanyCommandValidator : AbstractValidator<CreateConfigurationCompanyCommand>
{
    public CreateConfigurationCompanyCommandValidator()
    {
        Include(new ConfigurationCompanyCommandValidator<CreateConfigurationCompanyCommand>());

        RuleFor(command => command.DatabasePassword)
            .NotEmpty();
    }
}
