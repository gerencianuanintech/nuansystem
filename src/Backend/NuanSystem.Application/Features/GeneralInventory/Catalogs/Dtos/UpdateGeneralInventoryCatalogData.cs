namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

public sealed record UpdateGeneralInventoryCatalogData(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
