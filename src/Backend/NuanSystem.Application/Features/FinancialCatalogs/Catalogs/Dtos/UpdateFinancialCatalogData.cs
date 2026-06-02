namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

public sealed record UpdateFinancialCatalogData(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
