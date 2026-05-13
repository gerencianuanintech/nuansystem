using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Customers.Dtos;

namespace NuanSystem.Application.Features.Customers.Commands;

public sealed record CreateCustomerCommand(
    string Code,
    string Name,
    string? TaxIdentification,
    string? Email,
    string? Phone,
    string? AddressLine) : ICommand<CustomerDto>;
