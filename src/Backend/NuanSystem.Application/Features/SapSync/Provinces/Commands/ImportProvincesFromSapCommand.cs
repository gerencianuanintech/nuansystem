using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;

namespace NuanSystem.Application.Features.SapSync.Provinces.Commands;

public sealed record ImportProvincesFromSapCommand(
    [property: JsonIgnore] int? AuditUserId = null,
    [property: JsonIgnore] string? AuditUserName = null)
    : ICommand<SapProvinceImportResultDto>;
