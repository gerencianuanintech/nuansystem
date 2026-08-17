using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Commands;

internal static class ItemOriginValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator, Func<T,string> code, Func<T,string> name,
        Func<T,string?> description, Func<T,int> sortOrder)
    {
        validator.RuleFor(x => code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => description(x)).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}
public sealed class CreateItemOriginCommandValidator : AbstractValidator<CreateItemOriginCommand>
{ public CreateItemOriginCommandValidator() => ItemOriginValidationRules.Apply(this,x=>x.Code,x=>x.Name,x=>x.Description,x=>x.SortOrder); }
public sealed class UpdateItemOriginCommandValidator : AbstractValidator<UpdateItemOriginCommand>
{ public UpdateItemOriginCommandValidator(){ RuleFor(x=>x.Id).GreaterThan(0); ItemOriginValidationRules.Apply(this,x=>x.Code,x=>x.Name,x=>x.Description,x=>x.SortOrder); } }
public sealed class DeleteItemOriginCommandValidator : AbstractValidator<DeleteItemOriginCommand>
{ public DeleteItemOriginCommandValidator() => RuleFor(x=>x.Id).GreaterThan(0); }
