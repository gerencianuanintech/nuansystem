using FluentValidation;

namespace NuanSystem.Application.Features.Carriers.Commands;

public sealed class CreateCarrierCommandValidator : AbstractValidator<CreateCarrierCommand>
{
    public CreateCarrierCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.IdentificationTypeCode).NotEmpty().Length(2).Must(CarrierIdentificationTypeCodes.IsValid).WithMessage("El tipo de identificacion debe ser 04, 05 o 06.");
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class UpdateCarrierCommandValidator : AbstractValidator<UpdateCarrierCommand>
{
    public UpdateCarrierCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.IdentificationTypeCode).NotEmpty().Length(2).Must(CarrierIdentificationTypeCodes.IsValid).WithMessage("El tipo de identificacion debe ser 04, 05 o 06.");
        RuleFor(x => x.IdentificationNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class DeleteCarrierCommandValidator : AbstractValidator<DeleteCarrierCommand>
{
    public DeleteCarrierCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
