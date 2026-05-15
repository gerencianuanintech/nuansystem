namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

public sealed record UpdateItemFamilyData(
    int Id,
    int ItemGroupId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? SapFamilyCode,
    string? SapCode,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
