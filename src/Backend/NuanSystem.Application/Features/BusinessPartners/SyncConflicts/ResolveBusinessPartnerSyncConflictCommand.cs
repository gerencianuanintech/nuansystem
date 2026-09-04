using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.BusinessPartners.SyncConflicts;

public sealed record ResolveBusinessPartnerSyncConflictCommand(
    long ConflictId,
    string Resolution,
    string Reason,
    string ExpectedRowVersion,
    [property: JsonIgnore] int? AuditUserId,
    [property: JsonIgnore] string? AuditUserName) : ICommand<BusinessPartnerSyncConflictDto>;
