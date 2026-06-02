namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

public sealed record UpdateGeneralSupplierCatalogData(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);

