using FluentValidation;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        Include(new PurchaseOrderSaveValidator<CreatePurchaseOrderCommand>(
            command => command.SupplierId,
            command => command.SeriesCode,
            command => command.DocumentNumber,
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
            command => command.SeriesCode,
            command => command.DocumentNumber,
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
        Func<TCommand, string> seriesCode,
        Func<TCommand, string> documentNumber,
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
        RuleFor(command => seriesCode(command)).NotEmpty().MaximumLength(50).WithMessage("Serie es obligatoria.");
        RuleFor(command => documentNumber(command)).NotEmpty().MaximumLength(50).WithMessage("Numero es obligatorio.");
        RuleFor(command => documentDate(command)).NotEmpty().WithMessage("Fecha documento es obligatoria.");
        RuleFor(command => deliveryDate(command)).NotEmpty().WithMessage("Fecha entrega es obligatoria.");
        RuleFor(command => currencyCode(command)).NotEmpty().MaximumLength(10).WithMessage("Moneda es obligatoria.");
        RuleFor(command => paymentTermId(command)).NotNull().WithMessage("Condicion de pago es obligatoria.");
        RuleFor(command => buyerId(command)).NotNull().WithMessage("Comprador es obligatorio.");
        RuleFor(command => mainWarehouseId(command)).NotNull().WithMessage("Bodega principal es obligatoria.");
        RuleFor(command => discountPercent(command)).InclusiveBetween(0, 100);
        RuleFor(command => lines(command)).NotEmpty().WithMessage("Debe existir al menos una linea valida.");
        RuleForEach(command => lines(command)).ChildRules(line =>
        {
            line.RuleFor(item => item.ItemId).GreaterThan(0).WithMessage("Articulo es obligatorio.");
            line.RuleFor(item => item.ItemCode).NotEmpty().MaximumLength(50);
            line.RuleFor(item => item.ItemName).NotEmpty().MaximumLength(200);
            line.RuleFor(item => item.Quantity).GreaterThan(0);
            line.RuleFor(item => item.UnitPrice).GreaterThanOrEqualTo(0);
            line.RuleFor(item => item.UnitId).NotNull();
            line.RuleFor(item => item.TaxId).NotNull();
            line.RuleFor(item => item.WarehouseId).GreaterThan(0);
            line.RuleFor(item => item.DeliveryDate).NotEmpty();
            line.RuleFor(item => item.DiscountPercent).InclusiveBetween(0, 100);
        });
        RuleFor(command => addresses(command))
            .Must(items => items.Any(item => item.AddressType == "Delivery") && items.Any(item => item.AddressType == "Billing"))
            .WithMessage("Deben existir direccion de entrega y direccion de factura.");
    }
}
