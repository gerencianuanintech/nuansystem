using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;

namespace NuanSystem.Application.Features.SapSync.Countries.Commands;

public sealed record ImportCountriesFromSapCommand(
    [property: JsonIgnore] int? AuditUserId = null,
    [property: JsonIgnore] string? AuditUserName = null)
    : ICommand<SapCountryImportResultDto>;
