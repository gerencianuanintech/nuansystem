using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class UpdateItemGroupCommandHandler(
    IItemGroupRepository itemGroupRepository,
    IChartOfAccountRepository chartOfAccountRepository,
    ISyncEventPublisher syncEventPublisher,
    ICompanyContext companyContext)
    : ICommandHandler<UpdateItemGroupCommand, ItemGroupDto>
{
    public async Task<Result<ItemGroupDto>> Handle(UpdateItemGroupCommand request, CancellationToken cancellationToken)
    {
        if (await itemGroupRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<ItemGroupDto>.Failure(
                "Grupo de artículos no encontrado.",
                [new ApiError("ItemGroupNotFound", "No existe el grupo de artículos indicado.", nameof(request.Id))]);
        }

        var code = CreateItemGroupCommandHandler.NormalizeCode(request.Code);
        if (await itemGroupRepository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<ItemGroupDto>.Failure(
                "Ya existe otro grupo de artículos con el código indicado.",
                [new ApiError("ItemGroupCodeAlreadyExists", "El código de grupo ya existe.", nameof(request.Code))]);
        }

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

        var updated = await itemGroupRepository.UpdateAsync(new UpdateItemGroupData(
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
            CreateItemGroupCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);

        if (!updated)
        {
            return Result<ItemGroupDto>.Failure("No se pudo actualizar el grupo de artículos.");
        }

        var itemGroup = await itemGroupRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El grupo de artículos fue actualizado pero no pudo consultarse.");

        var syncResult = await ItemGroupSyncPublisher.PublishAsync(
            syncEventPublisher,
            companyContext,
            itemGroup,
            itemGroup.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
            cancellationToken);

        if (syncResult is { IsSuccess: false })
        {
            return Result<ItemGroupDto>.Failure(syncResult.Message, syncResult.Errors);
        }

        return Result<ItemGroupDto>.Success(itemGroup, "Grupo de artículos actualizado correctamente.");
    }
}
