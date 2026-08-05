using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;

namespace NuanSystem.Application.Features.SapSync.Cities.Commands;

public sealed record ImportCitiesFromSapCommand(
    [property: JsonIgnore] int? AuditUserId = null,
    [property: JsonIgnore] string? AuditUserName = null)
    : ICommand<SapCityImportResultDto>;
