namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

public sealed record CreateFinancialCatalogData(
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);
