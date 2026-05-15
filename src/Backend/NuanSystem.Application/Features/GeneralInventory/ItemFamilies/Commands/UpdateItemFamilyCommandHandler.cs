using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed class UpdateItemFamilyCommandHandler(IItemFamilyRepository itemFamilyRepository, IItemGroupRepository itemGroupRepository)
    : ICommandHandler<UpdateItemFamilyCommand, ItemFamilyDto>
{
    public async Task<Result<ItemFamilyDto>> Handle(UpdateItemFamilyCommand request, CancellationToken cancellationToken)
    {
        if (await itemFamilyRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<ItemFamilyDto>.Failure(
                "Linea/familia no encontrada.",
                [new ApiError("ItemFamilyNotFound", "No existe la linea/familia indicada.", nameof(request.Id))]);
        }

        if (await itemGroupRepository.GetByIdAsync(request.ItemGroupId, cancellationToken) is null)
        {
            return Result<ItemFamilyDto>.Failure(
                "Grupo de articulos no encontrado.",
                [new ApiError("ItemGroupNotFound", "No existe el grupo de articulos indicado.", nameof(request.ItemGroupId))]);
        }

        var code = CreateItemFamilyCommandHandler.NormalizeCode(request.Code);
        if (await itemFamilyRepository.ExistsByCodeAsync(request.ItemGroupId, code, request.Id, cancellationToken))
        {
            return Result<ItemFamilyDto>.Failure(
                "Ya existe otra linea/familia con el codigo indicado dentro del grupo.",
                [new ApiError("ItemFamilyCodeAlreadyExists", "El codigo de linea/familia ya existe para el grupo.", nameof(request.Code))]);
        }

        var updated = await itemFamilyRepository.UpdateAsync(new UpdateItemFamilyData(
            request.Id,
            request.ItemGroupId,
            code,
            request.Name.Trim(),
            CreateItemFamilyCommandHandler.NormalizeOptional(request.Description),
            request.IsActive,
            CreateItemFamilyCommandHandler.NormalizeOptional(request.SapFamilyCode),
            CreateItemFamilyCommandHandler.NormalizeOptional(request.SapCode),
            request.AuditUserId,
            CreateItemFamilyCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);

        if (!updated)
        {
            return Result<ItemFamilyDto>.Failure("No se pudo actualizar la linea/familia.");
        }

        var itemFamily = await itemFamilyRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("La linea/familia fue actualizada pero no pudo consultarse.");

        return Result<ItemFamilyDto>.Success(itemFamily, "Linea/familia actualizada correctamente.");
    }
}
