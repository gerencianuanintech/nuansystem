using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed record UpdateSapServiceLayerSettingsCommand(
    bool IsEnabled,
    string ServiceLayerUrl,
    string SapCompanyDb,
    string SapUser,
    string? SapPassword,
    int MaxRetryCount = 3,
    [property: JsonIgnore] int? AuditUserId = null,
    [property: JsonIgnore] string? AuditUserName = null) : ICommand<SapServiceLayerSettingsDto>;
