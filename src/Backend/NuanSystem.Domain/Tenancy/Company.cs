namespace NuanSystem.Domain.Tenancy;

public sealed class Company
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string CommercialName { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public CompanyOperationMode OperationMode { get; init; }

    public bool IsMaster { get; init; }

    public int? ParentCompanyId { get; init; }

    public string? BranchCode { get; init; }

    public bool SyncEnabled { get; init; }
}
