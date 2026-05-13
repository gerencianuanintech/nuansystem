namespace NuanSystem.WinForms.Services.Customers.Models;

public sealed record SaveCustomerRequest(
    string Code,
    string Name,
    string? TaxIdentification,
    string? Email,
    string? Phone,
    string? AddressLine,
    bool IsActive = true);
