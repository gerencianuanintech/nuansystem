using FluentValidation;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Commands;

public sealed class CreateItemTypeCommandValidator : AbstractValidator<CreateItemTypeCommand>
{
    public CreateItemTypeCommandValidator() => Configure(this);

    internal static void Configure(AbstractValidator<CreateItemTypeCommand> validator)
    {
        validator.RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode("ITEM_TYPE_CODE_REQUIRED")
            .MaximumLength(50).WithErrorCode("ITEM_TYPE_CODE_MAX_LENGTH");
        validator.RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("ITEM_TYPE_NAME_REQUIRED")
            .MaximumLength(150).WithErrorCode("ITEM_TYPE_NAME_MAX_LENGTH");
        validator.RuleFor(x => x.Description)
            .MaximumLength(500).WithErrorCode("ITEM_TYPE_DESCRIPTION_MAX_LENGTH");
        validator.RuleFor(x => x.BehaviorCode)
            .NotEmpty().WithErrorCode("ITEM_TYPE_BEHAVIOR_REQUIRED")
            .MaximumLength(30).WithErrorCode("ITEM_TYPE_BEHAVIOR_MAX_LENGTH")
            .Must(ItemTypeBehaviorCodes.IsValid)
            .WithMessage("El comportamiento del tipo de item no es valido.")
            .WithErrorCode("ITEM_TYPE_INVALID_BEHAVIOR");
        validator.RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithErrorCode("ITEM_TYPE_SORT_ORDER_INVALID");
        validator.RuleFor(x => x.DefaultIsInventoryItem)
            .Equal(false)
            .When(x => string.Equals(x.BehaviorCode?.Trim(), ItemTypeBehaviorCodes.Service, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Un servicio no puede activar inventario de forma predeterminada.")
            .WithErrorCode("ITEM_TYPE_SERVICE_INVENTORY_DEFAULT_INVALID");
    }
}

public sealed class UpdateItemTypeCommandValidator : AbstractValidator<UpdateItemTypeCommand>
{
    public UpdateItemTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithErrorCode("ITEM_TYPE_ID_INVALID");
        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode("ITEM_TYPE_CODE_REQUIRED")
            .MaximumLength(50).WithErrorCode("ITEM_TYPE_CODE_MAX_LENGTH");
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("ITEM_TYPE_NAME_REQUIRED")
            .MaximumLength(150).WithErrorCode("ITEM_TYPE_NAME_MAX_LENGTH");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithErrorCode("ITEM_TYPE_DESCRIPTION_MAX_LENGTH");
        RuleFor(x => x.BehaviorCode)
            .NotEmpty().WithErrorCode("ITEM_TYPE_BEHAVIOR_REQUIRED")
            .MaximumLength(30).WithErrorCode("ITEM_TYPE_BEHAVIOR_MAX_LENGTH")
            .Must(ItemTypeBehaviorCodes.IsValid)
            .WithMessage("El comportamiento del tipo de item no es valido.")
            .WithErrorCode("ITEM_TYPE_INVALID_BEHAVIOR");
        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithErrorCode("ITEM_TYPE_SORT_ORDER_INVALID");
        RuleFor(x => x.DefaultIsInventoryItem)
            .Equal(false)
            .When(x => string.Equals(x.BehaviorCode?.Trim(), ItemTypeBehaviorCodes.Service, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Un servicio no puede activar inventario de forma predeterminada.")
            .WithErrorCode("ITEM_TYPE_SERVICE_INVENTORY_DEFAULT_INVALID");
    }
}

public sealed class DeleteItemTypeCommandValidator : AbstractValidator<DeleteItemTypeCommand>
{
    public DeleteItemTypeCommandValidator() =>
        RuleFor(x => x.Id).GreaterThan(0).WithErrorCode("ITEM_TYPE_ID_INVALID");
}
