using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

internal static class ItemClassificationValidator
{
    public static async Task<ApiError?> ValidateAsync(
        int? itemGroupId,
        int? itemFamilyId,
        IItemGroupRepository itemGroupRepository,
        IItemFamilyRepository itemFamilyRepository,
        IItemSubgroupRepository itemSubgroupRepository,
        string? itemSubgroupCode,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        if (itemFamilyId.HasValue && !itemGroupId.HasValue)
        {
            return new ApiError(
                "ItemFamilyRequiresGroup",
                "Debe seleccionar el grupo al que pertenece la familia.",
                "ItemGroupId");
        }

        if (itemGroupId.HasValue)
        {
            var group = await itemGroupRepository.GetByIdAsync(
                itemGroupId.Value, connection, transaction, cancellationToken);
            if (group is null)
            {
                return new ApiError(
                    "ItemGroupNotFound",
                    "No existe el grupo de artículos indicado.",
                    "ItemGroupId");
            }

            if (!group.IsActive)
            {
                return new ApiError(
                    "ItemGroupInactive",
                    "El grupo de artículos indicado está inactivo.",
                    "ItemGroupId");
            }
        }

        if (!itemFamilyId.HasValue && !string.IsNullOrWhiteSpace(itemSubgroupCode))
        {
            return new ApiError(
                "ItemSubgroupRequiresFamily",
                "Debe seleccionar la familia a la que pertenece el subgrupo.",
                "ItemFamilyId");
        }

        if (!itemFamilyId.HasValue)
        {
            return null;
        }

        var family = await itemFamilyRepository.GetByIdAsync(
            itemFamilyId.Value, connection, transaction, cancellationToken);
        if (family is null)
        {
            return new ApiError(
                "ItemFamilyNotFound",
                "No existe la familia de artículos indicada.",
                "ItemFamilyId");
        }

        if (!family.IsActive)
        {
            return new ApiError(
                "ItemFamilyInactive",
                "La familia de artículos indicada está inactiva.",
                "ItemFamilyId");
        }

        if (family.ItemGroupId != itemGroupId)
        {
            return new ApiError(
                "ItemFamilyGroupMismatch",
                "La familia seleccionada no pertenece al grupo indicado.",
                "ItemFamilyId");
        }

        if (string.IsNullOrWhiteSpace(itemSubgroupCode)) return null;
        var subgroupExists = await itemSubgroupRepository.ExistsActiveByFamilyAndCodeAsync(
            itemFamilyId.Value, itemSubgroupCode.Trim(), connection, transaction, cancellationToken);
        return !subgroupExists
            ? new ApiError("ItemSubgroupFamilyMismatch", "El subgrupo indicado no pertenece a la familia seleccionada o está inactivo.", "MasterData.General.SubGroup")
            : null;
    }
}
