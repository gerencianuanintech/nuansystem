using FluentValidation;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}
