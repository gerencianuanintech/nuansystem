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

        return family.ItemGroupId == itemGroupId
            ? null
            : new ApiError(
                "ItemFamilyGroupMismatch",
                "La familia seleccionada no pertenece al grupo indicado.",
                "ItemFamilyId");
    }
}
