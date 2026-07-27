namespace NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;

public sealed class PriceListDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencyName { get; set; } = string.Empty;
    public Guid CurrencyGlobalId { get; set; }
    public string AppliesTo { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapCode { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed record PriceListLookupDto(int Id, string Code, string Name, bool IsActive);
public sealed record PriceListCurrencyDto(string Code, string Name, Guid GlobalId);

public sealed record CreatePriceListData(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    string CurrencyCode,
    string AppliesTo,
    bool IsDefault,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdatePriceListData(
    int Id,
    string Code,
    string Name,
    string? Description,
    string CurrencyCode,
    string AppliesTo,
    bool IsDefault,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);

public sealed record PriceListSyncPayloadV2(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    Guid CurrencyGlobalId,
    string CurrencyCode,
    string AppliesTo,
    bool IsDefault,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
