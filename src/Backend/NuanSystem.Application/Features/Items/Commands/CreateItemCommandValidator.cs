using FluentValidation;
using NuanSystem.Application.Features.Items.Dtos;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    private static readonly string[] ItemTypes = ["Product", "Service", "Supply", "Asset"];
    private static readonly string[] ValuationMethods = ["MovingAverage", "Standard", "FIFO", "SerialBatch"];
    private static readonly string[] ManagedByValues = ["None", "Batch", "Serial"];
    private static readonly string[] ManagementMethods = ["EveryTransaction", "IssueOnly"];

    public CreateItemCommandValidator()
    {
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Description).MaximumLength(500);
        RuleFor(command => command.ItemType).NotEmpty().Must(ItemTypes.Contains);
        RuleFor(command => command.ValuationMethod).NotEmpty().Must(ValuationMethods.Contains);
        RuleFor(command => command.ManagedBy).NotEmpty().Must(ManagedByValues.Contains);
        RuleFor(command => command.BatchSerialManagementMethod).NotEmpty().Must(ManagementMethods.Contains);
        RuleFor(command => command.BaseSalesPrice).GreaterThanOrEqualTo(0);
        RuleFor(command => command.ReferenceCost).GreaterThanOrEqualTo(0);
        RuleFor(command => command.PurchaseFactor).GreaterThan(0);
        RuleFor(command => command.SalesFactor).GreaterThan(0);
        RuleFor(command => command.Remarks).MaximumLength(1000);

        RuleFor(command => command.InventoryUnitOfMeasureId)
            .NotNull()
            .When(command => command.IsInventoryItem);

        RuleFor(command => command.PurchaseTaxId)
            .NotNull()
            .When(command => command.IsPurchaseItem);

        RuleFor(command => command.SalesTaxId)
            .NotNull()
            .When(command => command.IsSalesItem);

        RuleFor(command => command.ManagedBy)
            .Equal("None")
            .When(command => !command.IsInventoryItem)
            .WithMessage("Solo los articulos de inventario pueden manejar lote o serie.");

        RuleFor(command => command.Barcodes ?? Array.Empty<SaveItemBarcodeData>())
            .Must(items => items.Count(item => item.IsMain && item.IsActive) <= 1)
            .WithMessage("Solo puede existir un codigo de barra principal activo.");

        RuleFor(command => command.Warehouses ?? Array.Empty<SaveItemWarehouseData>())
            .Must(items => items.Count(item => item.IsDefaultWarehouse && item.IsActive) <= 1)
            .WithMessage("Solo puede existir una bodega predeterminada activa.");

        RuleForEach(command => command.Barcodes ?? Array.Empty<SaveItemBarcodeData>()).ChildRules(barcode =>
        {
            barcode.RuleFor(item => item.Barcode).NotEmpty().MaximumLength(120);
            barcode.RuleFor(item => item.BarcodeType).NotEmpty().MaximumLength(40);
            barcode.RuleFor(item => item.ConversionFactor).GreaterThan(0);
        });

        RuleForEach(command => command.Warehouses ?? Array.Empty<SaveItemWarehouseData>()).ChildRules(warehouse =>
        {
            warehouse.RuleFor(item => item.WarehouseId).GreaterThan(0);
            warehouse.RuleFor(item => item.MinimumStock).GreaterThanOrEqualTo(0);
            warehouse.RuleFor(item => item.MaximumStock).GreaterThanOrEqualTo(0);
            warehouse.RuleFor(item => item.RequiredStock).GreaterThanOrEqualTo(0);
            warehouse.RuleFor(item => item.ReorderPoint).GreaterThanOrEqualTo(0);
            warehouse.RuleFor(item => item.WarehouseCost).GreaterThanOrEqualTo(0);
            warehouse.RuleFor(item => item)
                .Must(item => item.MaximumStock == 0 || item.MinimumStock <= item.MaximumStock)
                .WithMessage("El stock minimo no puede ser mayor al stock maximo.");
        });
    }
}
