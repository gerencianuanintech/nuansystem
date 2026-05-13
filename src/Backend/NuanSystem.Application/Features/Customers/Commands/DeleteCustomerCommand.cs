using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.Customers.Commands;

public sealed record DeleteCustomerCommand(int Id) : ICommand<bool>;
