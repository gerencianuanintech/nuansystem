using FluentValidation;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        Include(new PurchaseOrderSaveValidator<CreatePurchaseOrderCommand>(
            command => command.SupplierId,
            command => command.DocumentSeriesId,
            command => command.SeriesCode,
            command => command.DocumentDate,
            command => command.DeliveryDate,
            command => command.CurrencyCode,
            command => command.PaymentTermId,
            command => command.BuyerId,
            command => command.MainWarehouseId,
            command => command.DiscountPercent,
            command => command.Lines,
            command => command.Addresses));
    }
}

public sealed class UpdatePurchaseOrderCommandValidator : AbstractValidator<UpdatePurchaseOrderCommand>
{
    public UpdatePurchaseOrderCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);

        Include(new PurchaseOrderSaveValidator<UpdatePurchaseOrderCommand>(
            command => command.SupplierId,
            command => command.DocumentSeriesId,
            command => command.SeriesCode,
            command => command.DocumentDate,
            command => command.DeliveryDate,
            command => command.CurrencyCode,
            command => command.PaymentTermId,
            command => command.BuyerId,
            command => command.MainWarehouseId,
            command => command.DiscountPercent,
            command => command.Lines,
            command => command.Addresses));
    }
}

internal sealed class PurchaseOrderSaveValidator<TCommand> : AbstractValidator<TCommand>
{
    public PurchaseOrderSaveValidator(
        Func<TCommand, int> supplierId,
        Func<TCommand, int?> documentSeriesId,
        Func<TCommand, string> seriesCode,
        Func<TCommand, DateTime> documentDate,
        Func<TCommand, DateTime> deliveryDate,
        Func<TCommand, string> currencyCode,
        Func<TCommand, int?> paymentTermId,
        Func<TCommand, int?> buyerId,
        Func<TCommand, int?> mainWarehouseId,
        Func<TCommand, decimal> discountPercent,
        Func<TCommand, IReadOnlyCollection<PurchaseOrderLineSaveRequest>> lines,
        Func<TCommand, IReadOnlyCollection<PurchaseOrderAddressSaveRequest>> addresses)
    {
        RuleFor(command => supplierId(command)).GreaterThan(0).WithMessage("Proveedor es obligatorio.");
        RuleFor(command => documentSeriesId(command)).NotNull().GreaterThan(0).WithMessage("Serie es obligatoria.");
        RuleFor(command => seriesCode(command)).NotEmpty().MaximumLength(50).WithMessage("Serie es obligatoria.");
        RuleFor(command => documentDate(command)).NotEmpty().WithMessage("Fecha documento es obligatoria.");
        RuleFor(command => deliveryDate(command)).NotEmpty().WithMessage("Fecha entrega es obligatoria.");
        RuleFor(command => currencyCode(command)).NotEmpty().MaximumLength(10).WithMessage("Moneda es obligatoria.");
        RuleFor(command => paymentTermId(command)).NotNull().WithMessage("Condicion de pago es obligatoria.");
        RuleFor(command => buyerId(command)).NotNull().WithMessage("Comprador es obligatorio.");
        RuleFor(command => mainWarehouseId(command)).NotNull().WithMessage("Bodega principal es obligatoria.");
        RuleFor(command => discountPercent(command)).InclusiveBetween(0, 100);
        RuleFor(command => lines(command))
            .NotEmpty()
            .OverridePropertyName("Lines")
            .WithMessage("Debe existir al menos una linea valida.")
            .Custom(ValidateLines);
        RuleFor(command => addresses(command))
            .Must(items => items.Any(item => item.AddressType == "Delivery") && items.Any(item => item.AddressType == "Billing"))
            .OverridePropertyName("Addresses")
            .WithMessage("Deben existir direccion de entrega y direccion de factura.");
    }

    private static void ValidateLines(
        IReadOnlyCollection<PurchaseOrderLineSaveRequest> items,
        ValidationContext<TCommand> context)
    {
        var index = 0;
        foreach (var item in items)
        {
            if (item.ItemId <= 0)
            {
                context.AddFailure($"Lines[{index}].ItemId", "Articulo es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(item.ItemCode) || item.ItemCode.Length > 50)
            {
                context.AddFailure($"Lines[{index}].ItemCode", "Codigo de articulo es obligatorio y no debe superar 50 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(item.ItemName) || item.ItemName.Length > 200)
            {
                context.AddFailure($"Lines[{index}].ItemName", "Nombre de articulo es obligatorio y no debe superar 200 caracteres.");
            }

            if (item.Quantity <= 0)
            {
                context.AddFailure($"Lines[{index}].Quantity", "Cantidad debe ser mayor a cero.");
            }

            if (item.UnitPrice < 0)
            {
                context.AddFailure($"Lines[{index}].UnitPrice", "Precio unitario no puede ser negativo.");
            }

            if (item.UnitId is null)
            {
                context.AddFailure($"Lines[{index}].UnitId", "Unidad es obligatoria.");
            }

            if (item.TaxId is null)
            {
                context.AddFailure($"Lines[{index}].TaxId", "Impuesto es obligatorio.");
            }

            if (item.WarehouseId <= 0)
            {
                context.AddFailure($"Lines[{index}].WarehouseId", "Bodega es obligatoria.");
            }

            if (item.DeliveryDate == default)
            {
                context.AddFailure($"Lines[{index}].DeliveryDate", "Fecha de entrega es obligatoria.");
            }

            if (item.DiscountPercent is < 0 or > 100)
            {
                context.AddFailure($"Lines[{index}].DiscountPercent", "Descuento debe estar entre 0 y 100.");
            }

            index++;
        }
    }
}
