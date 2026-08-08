using FluentValidation;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
{
    public CreateWarehouseCommandValidator()
    {
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(150);
        RuleFor(command => command.GlobalId).Must(value => value is null || value.Value != Guid.Empty);
        RuleFor(command => command.Email).EmailAddress().When(command => !string.IsNullOrWhiteSpace(command.Email));
        RuleFor(command => command.Description).MaximumLength(500);
        RuleFor(command => command.BranchCode).MaximumLength(50);
        RuleFor(command => command.Address).MaximumLength(250);
        RuleFor(command => command.City).MaximumLength(100);
        RuleFor(command => command.Province).MaximumLength(100);
        RuleFor(command => command.Country).MaximumLength(100);
        RuleFor(command => command.Phone).MaximumLength(50);
        RuleFor(command => command.ManagerName).MaximumLength(150);
        RuleFor(command => command.ExternalSystem).MaximumLength(50);
        RuleFor(command => command.ExternalCode).MaximumLength(100);
        RuleFor(command => command.SapCode).MaximumLength(100);
        RuleFor(command => command.ProvinceId)
            .Null().When(command => command.CountryId is null)
            .WithMessage("Seleccione el pais antes de la provincia.");
        RuleFor(command => command.CityId)
            .Null().When(command => command.CountryId is null || command.ProvinceId is null)
            .WithMessage("Seleccione el pais y la provincia antes de la ciudad.");
    }
}

public sealed class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(150);
        RuleFor(command => command.GlobalId).Must(value => value is null || value.Value != Guid.Empty);
        RuleFor(command => command.Email).EmailAddress().When(command => !string.IsNullOrWhiteSpace(command.Email));
        RuleFor(command => command.Description).MaximumLength(500);
        RuleFor(command => command.BranchCode).MaximumLength(50);
        RuleFor(command => command.Address).MaximumLength(250);
        RuleFor(command => command.City).MaximumLength(100);
        RuleFor(command => command.Province).MaximumLength(100);
        RuleFor(command => command.Country).MaximumLength(100);
        RuleFor(command => command.Phone).MaximumLength(50);
        RuleFor(command => command.ManagerName).MaximumLength(150);
        RuleFor(command => command.ExternalSystem).MaximumLength(50);
        RuleFor(command => command.ExternalCode).MaximumLength(100);
        RuleFor(command => command.SapCode).MaximumLength(100);
        RuleFor(command => command.ProvinceId)
            .Null().When(command => command.CountryId is null)
            .WithMessage("Seleccione el pais antes de la provincia.");
        RuleFor(command => command.CityId)
            .Null().When(command => command.CountryId is null || command.ProvinceId is null)
            .WithMessage("Seleccione el pais y la provincia antes de la ciudad.");
    }
}

public sealed class SetWarehouseActiveStatusCommandValidator : AbstractValidator<SetWarehouseActiveStatusCommand>
{
    public SetWarehouseActiveStatusCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
