using FluentValidation;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed class UpdateConfigurationCompanyCommandValidator : AbstractValidator<UpdateConfigurationCompanyCommand>
{
    public UpdateConfigurationCompanyCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

        Include(new ConfigurationCompanyCommandValidator<UpdateConfigurationCompanyCommand>());

        RuleFor(command => command.ParentCompanyId)
            .Null()
            .When(command => command.IsMaster == true)
            .WithMessage("Una empresa maestra no puede tener empresa padre.");

        RuleFor(command => command.BranchCode)
            .Empty()
            .When(command => command.IsMaster == true)
            .WithMessage("Una empresa maestra no puede tener codigo de sucursal.");

        RuleFor(command => command.ParentCompanyId)
            .NotNull()
            .GreaterThan(0)
            .When(command => command.IsMaster == false)
            .WithMessage("Una sucursal debe indicar la empresa maestra padre.");

        RuleFor(command => command.BranchCode)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9_-]+$")
            .When(command => command.IsMaster == false)
            .WithMessage("Una sucursal debe tener un codigo valido de sucursal.");

        RuleFor(command => command.SyncEnabled)
            .NotEqual(true)
            .When(command => !command.IsActive)
            .WithMessage("No se puede habilitar sincronizacion para una empresa inactiva.");
    }
}
