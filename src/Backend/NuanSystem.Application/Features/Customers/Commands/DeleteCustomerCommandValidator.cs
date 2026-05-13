using FluentValidation;

namespace NuanSystem.Application.Features.Customers.Commands;

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}
