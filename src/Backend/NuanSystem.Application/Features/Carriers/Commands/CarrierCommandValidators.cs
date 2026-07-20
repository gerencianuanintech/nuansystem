using FluentValidation;

namespace NuanSystem.Application.Features.Carriers.Commands;

public sealed class CreateCarrierCommandValidator : AbstractValidator<CreateCarrierCommand>
{
    public CreateCarrierCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode("CARRIER_CODE_REQUIRED")
            .MaximumLength(50).WithErrorCode("CARRIER_CODE_MAX_LENGTH");
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("CARRIER_NAME_REQUIRED")
            .MaximumLength(150).WithErrorCode("CARRIER_NAME_MAX_LENGTH");
        RuleFor(x => x.IdentificationTypeCode)
            .NotEmpty().WithErrorCode("CARRIER_IDENTIFICATION_TYPE_REQUIRED")
            .Length(2).WithErrorCode("CARRIER_IDENTIFICATION_TYPE_LENGTH")
            .Must(CarrierIdentificationTypeCodes.IsValid)
            .WithMessage("El tipo de identificacion debe ser 04, 05 o 06.")
            .WithErrorCode("CARRIER_IDENTIFICATION_TYPE_INVALID");
        RuleFor(x => x.IdentificationNumber)
            .NotEmpty().WithErrorCode("CARRIER_IDENTIFICATION_REQUIRED")
            .MaximumLength(30).WithErrorCode("CARRIER_IDENTIFICATION_MAX_LENGTH");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithErrorCode("CARRIER_DESCRIPTION_MAX_LENGTH");
    }
}

public sealed class UpdateCarrierCommandValidator : AbstractValidator<UpdateCarrierCommand>
{
    public UpdateCarrierCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithErrorCode("CARRIER_ID_INVALID");
        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode("CARRIER_CODE_REQUIRED")
            .MaximumLength(50).WithErrorCode("CARRIER_CODE_MAX_LENGTH");
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("CARRIER_NAME_REQUIRED")
            .MaximumLength(150).WithErrorCode("CARRIER_NAME_MAX_LENGTH");
        RuleFor(x => x.IdentificationTypeCode)
            .NotEmpty().WithErrorCode("CARRIER_IDENTIFICATION_TYPE_REQUIRED")
            .Length(2).WithErrorCode("CARRIER_IDENTIFICATION_TYPE_LENGTH")
            .Must(CarrierIdentificationTypeCodes.IsValid)
            .WithMessage("El tipo de identificacion debe ser 04, 05 o 06.")
            .WithErrorCode("CARRIER_IDENTIFICATION_TYPE_INVALID");
        RuleFor(x => x.IdentificationNumber)
            .NotEmpty().WithErrorCode("CARRIER_IDENTIFICATION_REQUIRED")
            .MaximumLength(30).WithErrorCode("CARRIER_IDENTIFICATION_MAX_LENGTH");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithErrorCode("CARRIER_DESCRIPTION_MAX_LENGTH");
    }
}

public sealed class DeleteCarrierCommandValidator : AbstractValidator<DeleteCarrierCommand>
{
    public DeleteCarrierCommandValidator() =>
        RuleFor(x => x.Id).GreaterThan(0).WithErrorCode("CARRIER_ID_INVALID");
}
