using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Settings.Dtos;

namespace NuanSystem.Application.Features.Settings.Commands;

public sealed record UpsertCompanyParameterCommand(
    string Key,
    string? Value,
    string? Description) : ICommand<CompanyParameterDto>;
