using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed record AssignUserCompanyCommand(int UserId, int CompanyId) : ICommand<bool>;
