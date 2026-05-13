namespace NuanSystem.Application.Features.Customers.Dtos;

public sealed record UpdateCustomerData(
    int Id,
    string Code,
    string Name,
    string? TaxIdentification,
    string? Email,
    string? Phone,
    string? AddressLine,
    bool IsActive);
