namespace NuanSystem.Application.Features.OperationalCatalogs.Dtos;

public sealed class OperationalCatalogDto
{
    public int Id { get; set; }
    public string CatalogKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ParentCatalogKey { get; set; }
    public string? ParentCode { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public sealed class OperationalCatalogLookupDto
{
    public int Id { get; set; }
    public string CatalogKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ParentCatalogKey { get; set; }
    public string? ParentCode { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public sealed record OperationalCatalogFilterData(
    string CatalogKey,
    string? Search,
    string? ParentCatalogKey,
    string? ParentCode,
    bool? IsActive);

public sealed record CreateOperationalCatalogData(
    string CatalogKey,
    string Code,
    string Name,
    string? Description,
    string? ParentCatalogKey,
    string? ParentCode,
    int DisplayOrder,
    bool IsDefault,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdateOperationalCatalogData(
    int Id,
    string CatalogKey,
    string Code,
    string Name,
    string? Description,
    string? ParentCatalogKey,
    string? ParentCode,
    int DisplayOrder,
    bool IsDefault,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
