using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.BusinessPartners.SyncConflicts;

public sealed record GetBusinessPartnerSyncConflictsQuery(string? Status = "Open")
    : IQuery<IReadOnlyCollection<BusinessPartnerSyncConflictDto>>;
