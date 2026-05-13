using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Customers.Dtos;

namespace NuanSystem.Application.Features.Customers.Commands;

public sealed record UpdateCustomerCommand(
    int Id,
    string Code,
    string Name,
    string? TaxIdentification,
    string? Email,
    string? Phone,
    string? AddressLine,
    bool IsActive) : ICommand<CustomerDto>;
