namespace NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;

public sealed class ChartOfAccountItem
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ExternalCode { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string? AccountClass { get; set; }
    public int? ParentAccountId { get; set; }
    public string? ParentAccountCode { get; set; }
    public string? ParentAccountName { get; set; }
    public int Level { get; set; }
    public bool IsTitle { get; set; }
    public bool AllowsMovement { get; set; }
    public bool IsActive { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal Balance { get; set; }
    public bool IsConfidential { get; set; }
    public bool IsMonetaryAccount { get; set; }
    public bool IsAssociatedAccount { get; set; }
    public bool RevalueByIndex { get; set; }
    public bool BlockManualPosting { get; set; }
    public bool RelevantForCashFlow { get; set; }
    public bool RequiresCostCenter { get; set; }
    public bool RequiresThirdParty { get; set; }
    public bool RequiresProject { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public DateTime? DeletedAt { get; set; }

    public string ParentDisplay => string.IsNullOrWhiteSpace(ParentAccountCode)
        ? string.Empty
        : $"{ParentAccountCode} - {ParentAccountName}";
}
