namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

public sealed class WarehouseDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BranchCode { get; set; }
    public string? Address { get; set; }
    public int? CityId { get; set; }
    public string? CityCode { get; set; }
    public string? City { get; set; }
    public int? ProvinceId { get; set; }
    public string? ProvinceCode { get; set; }
    public string? Province { get; set; }
    public int? CountryId { get; set; }
    public string? CountryCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ManagerName { get; set; }
    public bool AllowsSales { get; set; }
    public bool AllowsPurchases { get; set; }
    public bool AllowsTransfers { get; set; }
    public bool AllowsProduction { get; set; }
    public bool IsDefault { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapCode { get; set; }
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

public sealed record WarehouseSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
