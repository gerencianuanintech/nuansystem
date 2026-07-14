namespace NuanSystem.Domain.Inventory;

public sealed class Warehouse
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BranchCode { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ManagerName { get; set; }
    public bool AllowsSales { get; set; } = true;
    public bool AllowsPurchases { get; set; } = true;
    public bool AllowsTransfers { get; set; } = true;
    public bool AllowsProduction { get; set; }
    public bool IsDefault { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapCode { get; set; }
    public bool IsActive { get; set; } = true;
}
