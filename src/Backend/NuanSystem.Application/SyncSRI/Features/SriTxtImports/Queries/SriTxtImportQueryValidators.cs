using FluentValidation;
using NuanSystem.Application.Features.SriDocuments;

namespace NuanSystem.Application.Features.SriTxtImports.Queries;

public sealed class GetSriTxtImportsQueryValidator : AbstractValidator<GetSriTxtImportsQuery>
{
    public GetSriTxtImportsQueryValidator()
    {
        RuleFor(x => x.Filter.Page).GreaterThan(0).WithErrorCode("SRI_TXT_PAGE_INVALID");
        RuleFor(x => x.Filter.PageSize).InclusiveBetween(1, 500).WithErrorCode("SRI_TXT_PAGE_SIZE_INVALID");
        RuleFor(x => x.Filter.Status).Must(SriTxtImportStatusCodes.IsValid).WithErrorCode("SRI_TXT_STATUS_INVALID");
        RuleFor(x => x.Filter.Environment)
            .Must(value => string.IsNullOrWhiteSpace(value) || SriEnvironmentCodes.IsValid(value))
            .WithErrorCode("SRI_TXT_ENVIRONMENT_INVALID");
        RuleFor(x => x.Filter.FileName)
            .MaximumLength(SriTxtImportLimits.MaxFileNameLength)
            .WithErrorCode("SRI_TXT_FILE_NAME_LENGTH");
        RuleFor(x => x.Filter)
            .Must(filter => filter.CreatedFrom is null || filter.CreatedTo is null || filter.CreatedFrom <= filter.CreatedTo)
            .WithErrorCode("SRI_TXT_DATE_RANGE_INVALID");
    }
}

public sealed class GetSriTxtImportByIdQueryValidator : AbstractValidator<GetSriTxtImportByIdQuery>
{
    public GetSriTxtImportByIdQueryValidator() =>
        RuleFor(x => x.ImportId).GreaterThan(0).WithErrorCode("SRI_TXT_IMPORT_ID_INVALID");
}

public sealed class GetSriTxtImportRowsQueryValidator : AbstractValidator<GetSriTxtImportRowsQuery>
{
    public GetSriTxtImportRowsQueryValidator()
    {
        RuleFor(x => x.ImportId).GreaterThan(0).WithErrorCode("SRI_TXT_IMPORT_ID_INVALID");
        RuleFor(x => x.Filter.Validity).Must(SriTxtRowValidityCodes.IsValid).WithErrorCode("SRI_TXT_VALIDITY_INVALID");
        RuleFor(x => x.Filter.Page).GreaterThan(0).WithErrorCode("SRI_TXT_PAGE_INVALID");
        RuleFor(x => x.Filter.PageSize).InclusiveBetween(1, 500).WithErrorCode("SRI_TXT_PAGE_SIZE_INVALID");
    }
}
