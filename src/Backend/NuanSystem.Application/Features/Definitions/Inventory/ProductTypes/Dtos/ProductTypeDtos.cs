namespace NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;

public sealed class ProductTypeDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string NatureCode { get; set; } = ProductTypeNatureCodes.Other;
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
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

public sealed class ProductTypeLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NatureCode { get; set; } = ProductTypeNatureCodes.Other;
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ProductTypeAuditChangeDto
{
    public long Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Source { get; set; }
    public DateTime CreatedAt { get; set; }
}

public static class ProductTypeNatureCodes
{
    public const string Merchandise = "Merchandise";
    public const string FinishedGood = "FinishedGood";
    public const string RawMaterial = "RawMaterial";
    public const string SemiFinished = "SemiFinished";
    public const string Supply = "Supply";
    public const string Packaging = "Packaging";
    public const string ByProduct = "ByProduct";
    public const string Other = "Other";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    { Merchandise, FinishedGood, RawMaterial, SemiFinished, Supply, Packaging, ByProduct, Other };

    public static string Normalize(string value) =>
        All.First(code => code.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
}

public sealed record CreateProductTypeData(
    string Code, string Name, string? Description, string NatureCode, int SortOrder, bool IsActive,
    Guid GlobalId, int? CreatedByUserId, string? CreatedByUserName);

public sealed record UpdateProductTypeData(
    int Id, string Code, string Name, string? Description, string NatureCode, int SortOrder, bool IsActive,
    int? UpdatedByUserId, string? UpdatedByUserName);

public sealed record ProductTypeSyncPayload(
    Guid GlobalId, string Code, string Name, string? Description, string NatureCode, int SortOrder,
    bool IsSystem, bool IsActive, bool IsDeleted, DateTime UpdatedAt);
