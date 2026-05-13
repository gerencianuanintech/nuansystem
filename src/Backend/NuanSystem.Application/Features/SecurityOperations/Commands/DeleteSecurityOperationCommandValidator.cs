using FluentValidation;

namespace NuanSystem.Application.Features.SecurityOperations.Commands;

public sealed class DeleteSecurityOperationCommandValidator : AbstractValidator<DeleteSecurityOperationCommand>
{
    public DeleteSecurityOperationCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}
