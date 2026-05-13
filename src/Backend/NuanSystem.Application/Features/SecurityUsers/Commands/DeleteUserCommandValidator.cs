using FluentValidation;

namespace NuanSystem.Application.Features.SecurityUsers.Commands;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}

