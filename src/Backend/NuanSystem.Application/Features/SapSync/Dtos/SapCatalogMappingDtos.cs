namespace NuanSystem.Application.Features.SapSync.Dtos;

public static class SapCatalogMappingTypes
{
    public const string ItemGroup = "ItemGroup";
    public const string UnitOfMeasure = "UnitOfMeasure";
    public const string Tax = "Tax";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ItemGroup, UnitOfMeasure, Tax
    };
}

public sealed record SapCatalogMappingDto(
    long Id,
    int CompanyId,
    string MappingType,
    string SapCode,
    string NuanCode,
    bool IsActive,
    DateTime? UpdatedAt);

public sealed record SaveSapCatalogMappingDto(
    string MappingType,
    string SapCode,
    string NuanCode,
    bool IsActive = true);

public sealed record ReplaceSapCatalogMappingsData(
    int CompanyId,
    IReadOnlyCollection<SaveSapCatalogMappingDto> Mappings,
    int? AuditUserId,
    string? AuditUserName);
