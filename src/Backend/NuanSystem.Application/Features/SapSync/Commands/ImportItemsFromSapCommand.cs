using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed record ImportItemsFromSapCommand(
    IReadOnlyCollection<string>? SapItemCodes,
    [property: JsonIgnore] int? AuditUserId = null,
    [property: JsonIgnore] string? AuditUserName = null) : ICommand<SapItemImportResultDto>;
