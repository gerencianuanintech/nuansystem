using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class UpdateItemGroupCommandHandler(
    IItemGroupRepository itemGroupRepository,
    IChartOfAccountRepository chartOfAccountRepository,
    ITransactionRunner transactionRunner,
    IItemGroupLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<UpdateItemGroupCommand, ItemGroupDto>
{
    public async Task<Result<ItemGroupDto>> Handle(UpdateItemGroupCommand request, CancellationToken cancellationToken)
    {
        var code = CreateItemGroupCommandHandler.NormalizeCode(request.Code);
        var accountValidation = await CreateItemGroupCommandHandler.ValidateAccountCodesAsync(
            chartOfAccountRepository,
            [
                new CreateItemGroupCommandHandler.AccountCodeField(nameof(request.InventoryAccountCode), CreateItemGroupCommandHandler.NormalizeOptional(request.InventoryAccountCode), "cuenta de inventario"),
                new CreateItemGroupCommandHandler.AccountCodeField(nameof(request.CostOfSalesAccountCode), CreateItemGroupCommandHandler.NormalizeOptional(request.CostOfSalesAccountCode), "cuenta de costo de ventas"),
                new CreateItemGroupCommandHandler.AccountCodeField(nameof(request.SalesAccountCode), CreateItemGroupCommandHandler.NormalizeOptional(request.SalesAccountCode), "cuenta de ventas"),
                new CreateItemGroupCommandHandler.AccountCodeField(nameof(request.PurchaseAccountCode), CreateItemGroupCommandHandler.NormalizeOptional(request.PurchaseAccountCode), "cuenta de compras")
            ],
            cancellationToken);

        if (!accountValidation.IsSuccess)
        {
            return Result<ItemGroupDto>.Failure(accountValidation.Message, accountValidation.Errors);
        }

        var data = new UpdateItemGroupData(
            request.Id,
            code,
            request.Name.Trim(),
            CreateItemGroupCommandHandler.NormalizeOptional(request.Description),
            request.IsActive,
            CreateItemGroupCommandHandler.NormalizeOptional(request.InventoryAccountCode),
            CreateItemGroupCommandHandler.NormalizeOptional(request.CostOfSalesAccountCode),
            CreateItemGroupCommandHandler.NormalizeOptional(request.SalesAccountCode),
            CreateItemGroupCommandHandler.NormalizeOptional(request.PurchaseAccountCode),
            CreateItemGroupCommandHandler.NormalizeOptional(request.SapGroupCode),
            CreateItemGroupCommandHandler.NormalizeOptional(request.SapCode),
            request.AuditUserId,
            CreateItemGroupCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await itemGroupRepository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                {
                    return Result<ItemGroupDto>.Failure(
                        "Grupo de articulos no encontrado.",
                        [new ApiError("ItemGroupNotFound", "No existe el grupo de articulos indicado.", nameof(request.Id))]);
                }

                if (await itemGroupRepository.ExistsByCodeAsync(
                        code, request.Id, connection, transaction, token))
                {
                    return Result<ItemGroupDto>.Failure(
                        "Ya existe otro grupo de articulos con el codigo indicado.",
                        [new ApiError("ItemGroupCodeAlreadyExists", "El codigo de grupo ya existe.", nameof(request.Code))]);
                }

                if (!await itemGroupRepository.UpdateAsync(data, connection, transaction, token))
                {
                    return Result<ItemGroupDto>.Failure("No se pudo actualizar el grupo de articulos.");
                }

                var itemGroup = await itemGroupRepository.GetByIdAsync(request.Id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El grupo de articulos fue actualizado pero no pudo consultarse.");

                await localOutboxWriter.EnqueueAsync(
                    itemGroup,
                    itemGroup.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                    connection,
                    transaction,
                    token);
                return Result<ItemGroupDto>.Success(itemGroup, "Grupo de articulos actualizado correctamente.");
            },
            cancellationToken);
    }
}
