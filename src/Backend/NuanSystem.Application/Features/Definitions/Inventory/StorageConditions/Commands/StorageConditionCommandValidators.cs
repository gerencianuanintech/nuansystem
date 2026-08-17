using FluentValidation;
namespace NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Commands;
internal static class StorageConditionValidationRules
{
    public static void Apply<T>(AbstractValidator<T> v,Func<T,string> code,Func<T,string> name,Func<T,string?> description,Func<T,int> sortOrder)
    {
        v.RuleFor(x=>code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        v.RuleFor(x=>name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        v.RuleFor(x=>description(x)).MaximumLength(500).WithName("Description");
        v.RuleFor(x=>sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}
public sealed class CreateStorageConditionCommandValidator:AbstractValidator<CreateStorageConditionCommand>
{ public CreateStorageConditionCommandValidator()=>StorageConditionValidationRules.Apply(this,x=>x.Code,x=>x.Name,x=>x.Description,x=>x.SortOrder); }
public sealed class UpdateStorageConditionCommandValidator:AbstractValidator<UpdateStorageConditionCommand>
{ public UpdateStorageConditionCommandValidator(){RuleFor(x=>x.Id).GreaterThan(0);StorageConditionValidationRules.Apply(this,x=>x.Code,x=>x.Name,x=>x.Description,x=>x.SortOrder);} }
public sealed class DeleteStorageConditionCommandValidator:AbstractValidator<DeleteStorageConditionCommand>
{ public DeleteStorageConditionCommandValidator()=>RuleFor(x=>x.Id).GreaterThan(0); }
