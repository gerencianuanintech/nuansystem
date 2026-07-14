namespace NuanSystem.Domain.Tenancy;

public sealed class Company
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string CommercialName { get; init; } = string.Empty;

    public DatabaseEngine DatabaseEngine { get; init; }

    public string Server { get; init; } = string.Empty;

    public int? Port { get; init; }

    public string DatabaseName { get; init; } = string.Empty;

    public string DatabaseUser { get; init; } = string.Empty;

    public string DatabasePasswordEncrypted { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public SapIntegrationMode SapIntegrationMode { get; init; }

    public CompanyOperationMode OperationMode { get; init; }

    public bool IsMaster { get; init; }

    public int? ParentCompanyId { get; init; }

    public string? BranchCode { get; init; }

    public bool SyncEnabled { get; init; }
}
