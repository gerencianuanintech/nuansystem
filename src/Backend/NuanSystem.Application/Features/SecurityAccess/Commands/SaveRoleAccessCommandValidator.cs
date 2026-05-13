using FluentValidation;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed class SaveRoleAccessCommandValidator : AbstractValidator<SaveRoleAccessCommand>
{
    public SaveRoleAccessCommandValidator()
    {
        RuleFor(command => command.RoleId).GreaterThan(0);
        RuleForEach(command => command.Menus).ChildRules(menu =>
        {
            menu.RuleFor(item => item.MenuId).GreaterThan(0);
        });
        RuleForEach(command => command.Operations).ChildRules(operation =>
        {
            operation.RuleFor(item => item.FormId).GreaterThan(0);
            operation.RuleFor(item => item.OperationId).GreaterThan(0);
        });
    }
}
