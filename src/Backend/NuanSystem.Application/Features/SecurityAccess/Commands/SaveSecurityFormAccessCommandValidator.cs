using FluentValidation;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed class SaveSecurityFormAccessCommandValidator : AbstractValidator<SaveSecurityFormAccessCommand>
{
    public SaveSecurityFormAccessCommandValidator()
    {
        RuleFor(command => command.RoleId).GreaterThan(0);
        RuleFor(command => command.FormId).GreaterThan(0);
        RuleFor(command => command.Operations).NotNull();
        RuleForEach(command => command.Operations).ChildRules(operation =>
        {
            operation.RuleFor(item => item.OperationId).GreaterThan(0);
        });
    }
}
