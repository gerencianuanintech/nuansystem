namespace NuanSystem.WinForms.Services.Customers.Models;

public sealed record CustomerItem(
    int Id,
    string Code,
    string Name,
    string? TaxIdentification,
    string? Email,
    string? Phone,
    string? AddressLine,
    bool IsActive);
