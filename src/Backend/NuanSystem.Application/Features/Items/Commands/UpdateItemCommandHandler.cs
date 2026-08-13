using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class UpdateItemCommandHandler(
    IItemRepository itemRepository,
    IItemGroupRepository itemGroupRepository,
    IItemFamilyRepository itemFamilyRepository,
    ITransactionRunner transactionRunner,
    IItemLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<UpdateItemCommand, ItemDto>
{
    public async Task<Result<ItemDto>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var data = new UpdateItemData(
            request.Id,
            code,
            request.Name.Trim(),
            request.Description?.Trim(),
            request.ItemGroupId,
            request.ItemFamilyId,
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
            CreateItemCommandHandler.NormalizeBarcodes(request.Barcodes),
            CreateItemCommandHandler.NormalizeWarehouses(request.Warehouses),
            CreateItemCommandHandler.NormalizeMasterData(request.MasterData),
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            request.ExternalSystem?.Trim(),
            request.ExternalCode?.Trim(),
            request.SapCode?.Trim());

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var classificationError = await ItemClassificationValidator.ValidateAsync(
                    request.ItemGroupId,
                    request.ItemFamilyId,
                    itemGroupRepository,
                    itemFamilyRepository,
                    connection,
                    transaction,
                    token);
                if (classificationError is not null)
                {
                    return Result<ItemDto>.Failure(
                        "La clasificación del artículo no es válida.",
                        [classificationError]);
                }

                if (await itemRepository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                {
                    return Result<ItemDto>.Failure(
                        "Articulo no encontrado.",
                        [new ApiError("ItemNotFound", "No existe el articulo indicado.", nameof(request.Id))]);
                }

                if (await itemRepository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                {
                    return Result<ItemDto>.Failure(
                        "Ya existe otro articulo con el codigo indicado.",
                        [new ApiError("ItemCodeAlreadyExists", "El codigo de articulo ya existe.", nameof(request.Code))]);
                }

                if (!await itemRepository.UpdateAsync(data, connection, transaction, token))
                {
                    return Result<ItemDto>.Failure("No se pudo actualizar el articulo.");
                }

                var item = await itemRepository.GetByIdAsync(request.Id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El articulo fue actualizado pero no pudo consultarse.");

                await localOutboxWriter.EnqueueAsync(
                    item,
                    item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                    connection,
                    transaction,
                    token);
                return Result<ItemDto>.Success(item, "Articulo actualizado correctamente.");
            },
            cancellationToken);
    }
}
