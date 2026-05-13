using FluentValidation;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed class AssignUserCompanyCommandValidator : AbstractValidator<AssignUserCompanyCommand>
{
    public AssignUserCompanyCommandValidator()
    {
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.CompanyId).GreaterThan(0);
    }
}
