using FluentValidation;

namespace NuanSystem.Application.Features.SriTxtImports.Commands;

public sealed class UploadSriTxtImportCommandValidator : AbstractValidator<UploadSriTxtImportCommand>
{
    public UploadSriTxtImportCommandValidator()
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .MaximumLength(SriTxtImportLimits.MaxFileNameLength)
            .Must(name => string.Equals(Path.GetExtension(name), ".txt", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Solo se permiten archivos con extension .txt.")
            .WithErrorCode("SRI_TXT_EXTENSION_INVALID");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(SriTxtImportLimits.MaxFileSizeBytes)
            .WithErrorCode("SRI_TXT_FILE_SIZE_INVALID");

        RuleFor(x => x.DeclaredContentType)
            .Must(contentType =>
                string.IsNullOrWhiteSpace(contentType) ||
                !contentType.Contains("iso-8859-1", StringComparison.OrdinalIgnoreCase))
            .WithMessage("La codificacion ISO-8859-1 no esta permitida.")
            .WithErrorCode("SRI_TXT_ENCODING_NOT_ALLOWED");

        RuleFor(x => x.Content)
            .NotNull()
            .Must(stream => stream.CanRead && stream.CanSeek)
            .WithMessage("El flujo del archivo no es valido.")
            .WithErrorCode("SRI_TXT_STREAM_INVALID");
    }
}

public sealed class EnqueueSriTxtImportCommandValidator : AbstractValidator<EnqueueSriTxtImportCommand>
{
    public EnqueueSriTxtImportCommandValidator()
    {
        RuleFor(x => x.ImportId).GreaterThan(0).WithErrorCode("SRI_TXT_IMPORT_ID_INVALID");
        RuleFor(x => x.RowVersion)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(value => value.Length == 8)
            .WithErrorCode("SRI_TXT_ROW_VERSION_INVALID");
    }
}
