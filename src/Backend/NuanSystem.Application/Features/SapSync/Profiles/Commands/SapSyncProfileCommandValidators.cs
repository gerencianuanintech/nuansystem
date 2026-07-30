using FluentValidation;
using NuanSystem.Application.Features.SapSync.Profiles.Queries;

namespace NuanSystem.Application.Features.SapSync.Profiles.Commands;

public sealed class SaveSapSyncScheduleRequestValidator : AbstractValidator<SaveSapSyncScheduleRequest>
{
    public SaveSapSyncScheduleRequestValidator()
    {
        RuleFor(request => request.ScheduleType)
            .NotEmpty().WithErrorCode("SAP_SYNC_PROFILE_SCHEDULE_TYPE_REQUIRED")
            .MaximumLength(20).WithErrorCode("SAP_SYNC_PROFILE_SCHEDULE_TYPE_MAX_LENGTH");
        RuleFor(request => request.TimeZoneId)
            .MaximumLength(100).WithErrorCode("SAP_SYNC_PROFILE_TIME_ZONE_MAX_LENGTH");
        RuleFor(request => request.RowVersion)
            .Must(IsOptionalRowVersion)
            .WithMessage("RowVersion debe contener exactamente 8 bytes.")
            .WithErrorCode("SAP_SYNC_PROFILE_ROW_VERSION_INVALID");
    }

    private static bool IsOptionalRowVersion(byte[]? value) => value is null || value.Length == 8;
}

public sealed class SaveSapSyncProfileEntityRequestValidator : AbstractValidator<SaveSapSyncProfileEntityRequest>
{
    public SaveSapSyncProfileEntityRequestValidator()
    {
        RuleFor(request => request.EntityCode)
            .NotEmpty().WithErrorCode("SAP_SYNC_PROFILE_ENTITY_CODE_REQUIRED")
            .MaximumLength(80).WithErrorCode("SAP_SYNC_PROFILE_ENTITY_CODE_MAX_LENGTH");
        RuleFor(request => request.Direction)
            .NotEmpty().WithErrorCode("SAP_SYNC_PROFILE_DIRECTION_REQUIRED")
            .MaximumLength(20).WithErrorCode("SAP_SYNC_PROFILE_DIRECTION_MAX_LENGTH");
        RuleFor(request => request.SyncMode)
            .NotEmpty().WithErrorCode("SAP_SYNC_PROFILE_SYNC_MODE_REQUIRED")
            .MaximumLength(20).WithErrorCode("SAP_SYNC_PROFILE_SYNC_MODE_MAX_LENGTH");
        RuleFor(request => request.BatchSize)
            .InclusiveBetween(1, 10000).WithErrorCode("SAP_SYNC_PROFILE_BATCH_SIZE_RANGE");
        RuleFor(request => request.MaxAttempts)
            .InclusiveBetween(1, 20).WithErrorCode("SAP_SYNC_PROFILE_MAX_ATTEMPTS_RANGE");
        RuleFor(request => request.ExecutionOrder)
            .InclusiveBetween(0, 100000).WithErrorCode("SAP_SYNC_PROFILE_EXECUTION_ORDER_RANGE");
        RuleFor(request => request.ExecutionTimeoutMinutes)
            .InclusiveBetween(1, 1440).WithErrorCode("SAP_SYNC_PROFILE_TIMEOUT_RANGE");
        RuleFor(request => request.RowVersion)
            .Must(value => value is null || value.Length == 8)
            .WithMessage("RowVersion debe contener exactamente 8 bytes.")
            .WithErrorCode("SAP_SYNC_PROFILE_ROW_VERSION_INVALID");
        RuleFor(request => request.Schedule)
            .NotNull().WithErrorCode("SAP_SYNC_PROFILE_SCHEDULE_REQUIRED")
            .SetValidator(new SaveSapSyncScheduleRequestValidator());
    }
}

public sealed class SaveSapSyncProfileRequestValidator : AbstractValidator<SaveSapSyncProfileRequest>
{
    public SaveSapSyncProfileRequestValidator()
    {
        RuleFor(request => request.CompanyId)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_COMPANY_ID_INVALID");
        RuleFor(request => request.Code)
            .NotEmpty().WithErrorCode("SAP_SYNC_PROFILE_CODE_REQUIRED")
            .MaximumLength(80).WithErrorCode("SAP_SYNC_PROFILE_CODE_MAX_LENGTH");
        RuleFor(request => request.Name)
            .NotEmpty().WithErrorCode("SAP_SYNC_PROFILE_NAME_REQUIRED")
            .MaximumLength(160).WithErrorCode("SAP_SYNC_PROFILE_NAME_MAX_LENGTH");
        RuleFor(request => request.Description)
            .MaximumLength(500).WithErrorCode("SAP_SYNC_PROFILE_DESCRIPTION_MAX_LENGTH");
        RuleFor(request => request.Entities)
            .NotNull().WithErrorCode(SapSyncProfileErrorCodes.EntityRequired)
            .NotEmpty().WithErrorCode(SapSyncProfileErrorCodes.EntityRequired);
        RuleForEach(request => request.Entities)
            .SetValidator(new SaveSapSyncProfileEntityRequestValidator());
    }
}

public sealed class CreateSapSyncProfileCommandValidator : AbstractValidator<CreateSapSyncProfileCommand>
{
    public CreateSapSyncProfileCommandValidator()
    {
        RuleFor(command => command.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
        RuleFor(command => command.Profile)
            .NotNull()
            .SetValidator(new SaveSapSyncProfileRequestValidator());
    }
}

public sealed class UpdateSapSyncProfileCommandValidator : AbstractValidator<UpdateSapSyncProfileCommand>
{
    public UpdateSapSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_ID_INVALID");
        RuleFor(command => command.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
        RuleFor(command => command.Request)
            .NotNull();
        RuleFor(command => command.Request.Profile)
            .NotNull()
            .SetValidator(new SaveSapSyncProfileRequestValidator());
        RuleFor(command => command.Request.RowVersion)
            .Must(SapSyncProfileValidatorHelpers.IsRowVersion)
            .WithMessage("RowVersion debe contener exactamente 8 bytes.")
            .WithErrorCode("SAP_SYNC_PROFILE_ROW_VERSION_INVALID");
    }
}

public sealed class DeleteSapSyncProfileCommandValidator : AbstractValidator<DeleteSapSyncProfileCommand>
{
    public DeleteSapSyncProfileCommandValidator() => AddVersionRules(this);

    private static void AddVersionRules(AbstractValidator<DeleteSapSyncProfileCommand> validator)
    {
        validator.RuleFor(command => command.Id)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_ID_INVALID");
        validator.RuleFor(command => command.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
        validator.RuleFor(command => command.RowVersion)
            .Must(SapSyncProfileValidatorHelpers.IsRowVersion)
            .WithMessage("RowVersion debe contener exactamente 8 bytes.")
            .WithErrorCode("SAP_SYNC_PROFILE_ROW_VERSION_INVALID");
    }
}

public sealed class ValidateSapSyncProfileCommandValidator : AbstractValidator<ValidateSapSyncProfileCommand>
{
    public ValidateSapSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_ID_INVALID");
        RuleFor(command => command.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
    }
}

public sealed class ActivateSapSyncProfileCommandValidator : AbstractValidator<ActivateSapSyncProfileCommand>
{
    public ActivateSapSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_ID_INVALID");
        RuleFor(command => command.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
        RuleFor(command => command.RowVersion)
            .Must(SapSyncProfileValidatorHelpers.IsRowVersion)
            .WithMessage("RowVersion debe contener exactamente 8 bytes.")
            .WithErrorCode("SAP_SYNC_PROFILE_ROW_VERSION_INVALID");
    }
}

public sealed class DeactivateSapSyncProfileCommandValidator : AbstractValidator<DeactivateSapSyncProfileCommand>
{
    public DeactivateSapSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_ID_INVALID");
        RuleFor(command => command.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
        RuleFor(command => command.RowVersion)
            .Must(SapSyncProfileValidatorHelpers.IsRowVersion)
            .WithMessage("RowVersion debe contener exactamente 8 bytes.")
            .WithErrorCode("SAP_SYNC_PROFILE_ROW_VERSION_INVALID");
    }
}

public sealed class GetSapSyncProfilesQueryValidator : AbstractValidator<GetSapSyncProfilesQuery>
{
    public GetSapSyncProfilesQueryValidator()
    {
        RuleFor(query => query.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
        RuleFor(query => query.Filter.PageNumber)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_PAGE_NUMBER_INVALID");
        RuleFor(query => query.Filter.PageSize)
            .InclusiveBetween(1, 500).WithErrorCode("SAP_SYNC_PROFILE_PAGE_SIZE_INVALID");
        RuleFor(query => query.Filter.CompanyId)
            .GreaterThan(0)
            .When(query => query.Filter.CompanyId.HasValue)
            .WithErrorCode("SAP_SYNC_PROFILE_COMPANY_ID_INVALID");
        RuleFor(query => query.Filter.EntityCode)
            .MaximumLength(80).WithErrorCode("SAP_SYNC_PROFILE_ENTITY_CODE_MAX_LENGTH");
        RuleFor(query => query.Filter.Search)
            .MaximumLength(160).WithErrorCode("SAP_SYNC_PROFILE_SEARCH_MAX_LENGTH");
    }
}

public sealed class GetSapSyncProfileByIdQueryValidator : AbstractValidator<GetSapSyncProfileByIdQuery>
{
    public GetSapSyncProfileByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0).WithErrorCode("SAP_SYNC_PROFILE_ID_INVALID");
        RuleFor(query => query.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
    }
}

public sealed class GetSapSyncProfileCatalogQueryValidator : AbstractValidator<GetSapSyncProfileCatalogQuery>
{
    public GetSapSyncProfileCatalogQueryValidator() =>
        RuleFor(query => query.UserId)
            .GreaterThan(0).WithErrorCode(SapSyncProfileErrorCodes.CompanyAccessDenied);
}

internal static class SapSyncProfileValidatorHelpers
{
    public static bool IsRowVersion(byte[]? value) => value is { Length: 8 };
}
