using FluentValidation;

namespace NuanSystem.Application.Features.SriDocuments.Commands;

public sealed class EnqueueSriDocumentCommandValidator : AbstractValidator<EnqueueSriDocumentCommand>
{
    public EnqueueSriDocumentCommandValidator()
    {
        RuleFor(x => x.Environment).NotEmpty().Must(SriEnvironmentCodes.IsValid).WithErrorCode("SRI_ENVIRONMENT_INVALID");
        RuleFor(x => x.AccessKey).Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode("SRI_ACCESS_KEY_REQUIRED")
            .Must(SriAccessKey.HasValidFormat).WithMessage("La clave de acceso debe contener 49 digitos.").WithErrorCode("SRI_ACCESS_KEY_FORMAT")
            .Must(SriAccessKey.HasValidCheckDigit).WithMessage("El digito verificador de la clave de acceso no es valido.").WithErrorCode("SRI_ACCESS_KEY_CHECK_DIGIT")
            .Must(SriAccessKey.IsSupportedPilotDocument).WithMessage("El piloto admite facturas, notas de credito y comprobantes de retencion.").WithErrorCode("SRI_DOCUMENT_TYPE_UNSUPPORTED");
        RuleFor(x => x.AccessKey).Must((command, key) => !SriEnvironmentCodes.IsValid(command.Environment) || SriAccessKey.MatchesEnvironment(key, command.Environment))
            .WithMessage("El ambiente incluido en la clave no coincide con el solicitado.").WithErrorCode("SRI_ACCESS_KEY_ENVIRONMENT_MISMATCH");
        RuleFor(x => x.SourceType).NotEmpty().Must(SriSourceTypeCodes.IsValid).WithErrorCode("SRI_SOURCE_TYPE_INVALID");
        RuleFor(x => x.SourceReference).NotEmpty().MaximumLength(200).WithErrorCode("SRI_SOURCE_REFERENCE_INVALID");
        RuleFor(x => x.BranchCode).MaximumLength(50).WithErrorCode("SRI_BRANCH_CODE_MAX_LENGTH");
        RuleFor(x => x.Priority).InclusiveBetween(1, 9).WithErrorCode("SRI_PRIORITY_INVALID");
    }
}

public sealed class CancelSriDocumentCommandValidator : AbstractValidator<CancelSriDocumentCommand>
{
    public CancelSriDocumentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithErrorCode("SRI_QUEUE_ID_INVALID");
        RuleFor(x => x.RowVersion).Cascade(CascadeMode.Stop).NotNull().Must(value => value.Length == 8).WithErrorCode("SRI_ROW_VERSION_INVALID");
        RuleFor(x => x.Reason).MaximumLength(500).WithErrorCode("SRI_REASON_MAX_LENGTH");
    }
}

public sealed class ReprocessSriDocumentCommandValidator : AbstractValidator<ReprocessSriDocumentCommand>
{
    public ReprocessSriDocumentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithErrorCode("SRI_QUEUE_ID_INVALID");
        RuleFor(x => x.RowVersion).Cascade(CascadeMode.Stop).NotNull().Must(value => value.Length == 8).WithErrorCode("SRI_ROW_VERSION_INVALID");
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500).WithErrorCode("SRI_REPROCESS_REASON_REQUIRED");
    }
}
