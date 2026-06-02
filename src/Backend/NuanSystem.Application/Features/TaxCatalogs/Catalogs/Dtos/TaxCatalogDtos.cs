namespace NuanSystem.Application.Features.TaxCatalogs.Catalogs.Dtos;

public class TaxCatalogDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class RetentionConceptDto : TaxCatalogDto
{
    public int? RetentionTypeId { get; set; }
    public string? RetentionTypeName { get; set; }
    public string? SriCode { get; set; }
    public decimal Percent { get; set; }
    public bool AppliesIva { get; set; }
    public bool AppliesIncome { get; set; }
}

public sealed record TaxCatalogLookupDto(int Id, string Code, string Name, bool IsActive = true);

public sealed record RetentionConceptLookupDto(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome);

public sealed record CreateTaxCatalogData(
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdateTaxCatalogData(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);

public sealed record SaveRetentionConceptData(
    int? Id,
    string Code,
    string Name,
    string? Description,
    int? RetentionTypeId,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName);
