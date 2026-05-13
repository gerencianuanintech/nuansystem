using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class UpdateItemGroupCommandHandler(IItemGroupRepository itemGroupRepository)
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

        return Result<ItemGroupDto>.Success(itemGroup, "Grupo de artículos actualizado correctamente.");
    }
}
