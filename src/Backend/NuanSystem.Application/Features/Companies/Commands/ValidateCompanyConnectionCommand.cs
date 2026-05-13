using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Companies.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed record ValidateCompanyConnectionCommand(
    DatabaseEngine DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    string DatabasePassword) : ICommand<CompanyConnectionTestResult>;
