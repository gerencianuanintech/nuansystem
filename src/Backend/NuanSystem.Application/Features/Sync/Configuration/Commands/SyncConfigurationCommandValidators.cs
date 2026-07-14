using FluentValidation;

namespace NuanSystem.Application.Features.Sync.Configuration.Commands;

public sealed class CreateSyncProfileCommandValidator : AbstractValidator<CreateSyncProfileCommand>
{
    public CreateSyncProfileCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        RuleFor(command => command.Request.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Request.Name).NotEmpty().MaximumLength(150);
        RuleFor(command => command.Request.Description).MaximumLength(500);
        RuleFor(command => command.Request.CompanyId).GreaterThan(0);
    }
}

public sealed class UpdateSyncProfileCommandValidator : AbstractValidator<UpdateSyncProfileCommand>
{
    public UpdateSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        RuleFor(command => command.Request.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Request.Name).NotEmpty().MaximumLength(150);
        RuleFor(command => command.Request.Description).MaximumLength(500);
        RuleFor(command => command.Request.CompanyId).GreaterThan(0);
    }
}

public sealed class ActivateSyncProfileCommandValidator : AbstractValidator<ActivateSyncProfileCommand>
{
    public ActivateSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}

public sealed class DeactivateSyncProfileCommandValidator : AbstractValidator<DeactivateSyncProfileCommand>
{
    public DeactivateSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}

public sealed class DeleteSyncProfileCommandValidator : AbstractValidator<DeleteSyncProfileCommand>
{
    public DeleteSyncProfileCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}

public sealed class ValidateSyncProfileCommandValidator : AbstractValidator<ValidateSyncProfileCommand>
{
    public ValidateSyncProfileCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
    }
}
