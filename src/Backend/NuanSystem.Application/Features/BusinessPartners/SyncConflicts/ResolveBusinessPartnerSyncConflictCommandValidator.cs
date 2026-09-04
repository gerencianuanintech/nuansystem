using FluentValidation;

namespace NuanSystem.Application.Features.BusinessPartners.SyncConflicts;

public sealed class ResolveBusinessPartnerSyncConflictCommandValidator
    : AbstractValidator<ResolveBusinessPartnerSyncConflictCommand>
{
    public ResolveBusinessPartnerSyncConflictCommandValidator()
    {
        RuleFor(command => command.ConflictId)
            .GreaterThan(0)
            .WithErrorCode("BP_SYNC_CONFLICT_ID_INVALID");

        RuleFor(command => command.Resolution)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("BP_SYNC_CONFLICT_RESOLUTION_REQUIRED")
            .Must(value => value is "AcceptBranch" or "KeepCentral")
            .WithMessage("La resolucion debe ser AcceptBranch o KeepCentral.")
            .WithErrorCode("BP_SYNC_CONFLICT_RESOLUTION_INVALID");

        RuleFor(command => command.Reason)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("BP_SYNC_CONFLICT_REASON_REQUIRED")
            .MaximumLength(500)
            .WithErrorCode("BP_SYNC_CONFLICT_REASON_MAX_LENGTH");

        RuleFor(command => command.ExpectedRowVersion)
            .Must(IsEightByteBase64)
            .WithMessage("ExpectedRowVersion debe ser un rowversion base64 valido.")
            .WithErrorCode("BP_SYNC_CONFLICT_ROW_VERSION_INVALID");
    }

    internal static bool TryDecodeRowVersion(string? value, out byte[] rowVersion)
    {
        try
        {
            rowVersion = Convert.FromBase64String(value ?? string.Empty);
            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            rowVersion = [];
            return false;
        }
    }

    private static bool IsEightByteBase64(string? value) => TryDecodeRowVersion(value, out _);
}
