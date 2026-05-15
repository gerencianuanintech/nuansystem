namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;

public sealed record ChartOfAccountLookupDto(
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
