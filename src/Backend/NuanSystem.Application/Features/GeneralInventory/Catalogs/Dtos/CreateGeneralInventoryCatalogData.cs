namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

public sealed record CreateGeneralInventoryCatalogData(
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);
