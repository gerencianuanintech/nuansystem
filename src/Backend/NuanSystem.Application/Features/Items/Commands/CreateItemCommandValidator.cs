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
            .When(command => command.IsInventoryItem && !command.IsExternalImport);

        RuleFor(command => command.PurchaseTaxId)
            .NotNull()
            .When(command => command.IsPurchaseItem && !command.IsExternalImport);

        RuleFor(command => command.SalesTaxId)
            .NotNull()
            .When(command => command.IsSalesItem && !command.IsExternalImport);

        RuleFor(command => command.ManagedBy)
            .Equal("None")
            .When(command => !command.IsInventoryItem)
            .WithMessage("Solo los articulos de inventario pueden manejar lote o serie.");

        RuleFor(command => command.MasterData)
            .Must((command, masterData) => masterData?.General is null || !masterData.General.IsService || (!command.IsInventoryItem && !masterData.General.AffectsInventory))
            .WithMessage("Un servicio no debe manejar ni afectar inventario.");

        RuleFor(command => command.MasterData)
            .Must(masterData => masterData?.Units is null || masterData.Units.GrossWeight <= 0 || masterData.Units.NetWeight <= masterData.Units.GrossWeight)
            .WithMessage("El peso bruto no puede ser menor al peso neto.");

        RuleFor(command => command.MasterData)
            .Must(masterData => masterData?.Inventory is null || masterData.Inventory.GlobalMaximumStock == 0 || masterData.Inventory.GlobalMinimumStock <= masterData.Inventory.GlobalMaximumStock)
            .WithMessage("El stock minimo global no puede ser mayor al stock maximo global.");

        RuleFor(command => command.MasterData)
            .Must(masterData => masterData?.Inventory is null || masterData.Inventory.GlobalMaximumStock == 0 || masterData.Inventory.GlobalReorderPoint <= masterData.Inventory.GlobalMaximumStock)
            .WithMessage("El punto de reorden global no puede ser mayor al stock maximo global.");

        RuleFor(command => command.MasterData)
            .Must(masterData => masterData?.Inventory is null || !masterData.Inventory.BatchRequired || masterData.Traceability?.BatchControl == true || masterData.General?.BatchManaged == true)
            .WithMessage("El control de lote obligatorio requiere habilitar lotes en el articulo.");

        RuleFor(command => command.MasterData)
            .Must(masterData => masterData?.Inventory is null || !masterData.Inventory.SerialRequired || masterData.Traceability?.SerialControl == true || masterData.General?.SerialManaged == true)
            .WithMessage("El control de serie obligatorio requiere habilitar series en el articulo.");

        RuleFor(command => command.MasterData)
            .Must(masterData => (masterData?.Units?.Presentations ?? Array.Empty<ItemPresentationData>()).All(item => item.InventoryFactor > 0))
            .WithMessage("Todas las presentaciones deben tener un factor mayor a cero.");

        RuleFor(command => command.MasterData)
            .Must(masterData => (masterData?.Units?.Barcodes ?? Array.Empty<ItemBarcodeData>()).All(item => item.InventoryFactor > 0))
            .WithMessage("Todos los codigos de barra del articulo deben tener un factor mayor a cero.");

        RuleFor(command => command.MasterData)
            .Must(masterData => (masterData?.Units?.Barcodes ?? Array.Empty<ItemBarcodeData>()).Count(item => item.IsMain && item.IsActive) <= 1)
            .WithMessage("Solo puede existir un codigo de barra principal activo en la grilla de presentaciones.");

        RuleFor(command => command.MasterData)
            .Must(masterData => (masterData?.Units?.Presentations ?? Array.Empty<ItemPresentationData>())
                .Where(item => item.IsActive && !string.IsNullOrWhiteSpace(item.Barcode))
                .Select(item => item.Barcode!.Trim().ToUpperInvariant())
                .Distinct()
                .Count() == (masterData?.Units?.Presentations ?? Array.Empty<ItemPresentationData>())
                    .Count(item => item.IsActive && !string.IsNullOrWhiteSpace(item.Barcode)))
            .WithMessage("Los codigos de barra de presentaciones no deben duplicarse dentro del articulo.");

        RuleFor(command => command.MasterData)
            .Must(masterData => (masterData?.Units?.Barcodes ?? Array.Empty<ItemBarcodeData>())
                .Where(item => item.IsActive && !string.IsNullOrWhiteSpace(item.Barcode))
                .Select(item => item.Barcode.Trim().ToUpperInvariant())
                .Distinct()
                .Count() == (masterData?.Units?.Barcodes ?? Array.Empty<ItemBarcodeData>())
                    .Count(item => item.IsActive && !string.IsNullOrWhiteSpace(item.Barcode)))
            .WithMessage("Los codigos de barra no deben duplicarse dentro del articulo.");

        RuleFor(command => command.MasterData)
            .Must(masterData => (masterData?.Inventory?.Warehouses ?? Array.Empty<ItemWarehouseData>()).Count(item => item.IsDefaultWarehouse && item.IsActive) <= 1)
            .WithMessage("Solo puede existir una bodega predeterminada activa en la configuracion del articulo.");

        RuleFor(command => command.MasterData)
            .Must(masterData => (masterData?.Inventory?.Warehouses ?? Array.Empty<ItemWarehouseData>()).All(item => item.MaximumStock == 0 || item.MinimumStock <= item.MaximumStock))
            .WithMessage("El stock minimo por bodega no puede ser mayor al stock maximo.");

        RuleFor(command => command.MasterData)
            .Must(masterData => masterData?.Remarks is null || (masterData.Remarks.OperationalAlerts ?? Array.Empty<ItemOperationalAlertData>())
                .All(item => item.ValidTo is null || item.ValidTo.Value.Date >= item.ValidFrom.Date))
            .WithMessage("La fecha hasta de una alerta operativa no puede ser menor que la fecha desde.");

        RuleFor(command => command.Barcodes ?? Array.Empty<SaveItemBarcodeData>())
            .Must(items => items.Count(item => item.IsMain && item.IsActive) <= 1)
            .WithMessage("Solo puede existir un codigo de barra principal activo.");

        RuleFor(command => command.Warehouses ?? Array.Empty<SaveItemWarehouseData>())
            .Must(items => items.Count(item => item.IsDefaultWarehouse && item.IsActive) <= 1)
            .WithMessage("Solo puede existir una bodega predeterminada activa.");

        RuleForEach(command => command.Barcodes).ChildRules(barcode =>
        {
            barcode.RuleFor(item => item.Barcode).NotEmpty().MaximumLength(120);
            barcode.RuleFor(item => item.BarcodeType).NotEmpty().MaximumLength(40);
            barcode.RuleFor(item => item.ConversionFactor).GreaterThan(0);
        });

        RuleForEach(command => command.Warehouses).ChildRules(warehouse =>
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
