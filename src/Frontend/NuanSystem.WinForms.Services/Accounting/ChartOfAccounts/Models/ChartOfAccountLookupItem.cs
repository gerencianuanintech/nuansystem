namespace NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;

public sealed record ChartOfAccountLookupItem(
    int Id,
    string Code,
    string Name,
    string AccountType,
    int? ParentAccountId,
    int Level,
    bool IsActive)
{
    public string DisplayText => $"{Code} - {Name}";
}
