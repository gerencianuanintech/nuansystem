using FluentValidation;

namespace NuanSystem.Application.Features.SecurityUsers.Commands;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

        RuleFor(command => command.UserName)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Email)
            .EmailAddress()
            .MaximumLength(256)
            .When(command => !string.IsNullOrWhiteSpace(command.Email));

        RuleFor(command => command.PhoneNumber)
            .MaximumLength(30);

        RuleFor(command => command.FirstName)
            .MaximumLength(120);

        RuleFor(command => command.LastName)
            .MaximumLength(120);

        RuleFor(command => command.ProfileImageUrl)
            .MaximumLength(500);

        RuleFor(command => command.Password)
            .MinimumLength(8)
            .When(command => !string.IsNullOrWhiteSpace(command.Password));
    }
}

