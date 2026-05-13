using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class CreateItemCommandHandler(IItemRepository itemRepository)
    : ICommandHandler<CreateItemCommand, ItemDto>
{
    public async Task<Result<ItemDto>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await itemRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<ItemDto>.Failure(
                "Ya existe un articulo con el codigo indicado.",
                [new ApiError("ItemCodeAlreadyExists", "El codigo de articulo ya existe.", nameof(request.Code))]);
        }

        var id = await itemRepository.CreateAsync(new CreateItemData(
            code,
            request.Name.Trim(),
            request.Description?.Trim(),
            request.ItemGroupId,
            request.ItemType.Trim(),
            request.InventoryUnitOfMeasureId,
            request.PurchaseUnitOfMeasureId,
            request.SalesUnitOfMeasureId,
            request.IsPurchaseItem,
            request.IsSalesItem,
            request.IsInventoryItem,
            request.PurchaseTaxId,
            request.SalesTaxId,
            request.ValuationMethod.Trim(),
            request.ManagedBy.Trim(),
            request.BatchSerialManagementMethod.Trim(),
            request.PreferredVendorCode?.Trim(),
            request.VendorCatalogCode?.Trim(),
            request.BaseSalesPrice,
            request.ReferenceCost,
            request.PurchaseFactor,
            request.SalesFactor,
            request.AllowDiscount,
            request.AllowSaleWithoutStock,
            request.Remarks?.Trim(),
            request.IsActive,
            NormalizeBarcodes(request.Barcodes),
            NormalizeWarehouses(request.Warehouses),
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var item = await itemRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El articulo fue creado pero no pudo consultarse.");

        return Result<ItemDto>.Success(item, "Articulo creado correctamente.");
    }

    internal static IReadOnlyCollection<SaveItemBarcodeData> NormalizeBarcodes(IReadOnlyCollection<SaveItemBarcodeData>? barcodes)
    {
        return (barcodes ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item.Barcode))
            .Select(item => item with
            {
                Barcode = item.Barcode.Trim(),
                BarcodeType = string.IsNullOrWhiteSpace(item.BarcodeType) ? "Internal" : item.BarcodeType.Trim()
            })
            .ToArray();
    }

    internal static IReadOnlyCollection<SaveItemWarehouseData> NormalizeWarehouses(IReadOnlyCollection<SaveItemWarehouseData>? warehouses)
    {
        return (warehouses ?? [])
            .Where(item => item.WarehouseId > 0)
            .Select(item => item with { DefaultLocationCode = item.DefaultLocationCode?.Trim() })
            .ToArray();
    }
}
