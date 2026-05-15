namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

public sealed record CreateItemFamilyData(
    int ItemGroupId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? SapFamilyCode,
    string? SapCode,
    int? CreatedByUserId,
    string? CreatedByUserName);
