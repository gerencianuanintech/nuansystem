namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

public sealed record CreateGeneralSupplierCatalogData(
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

