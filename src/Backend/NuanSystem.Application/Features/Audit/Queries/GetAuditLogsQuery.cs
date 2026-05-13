using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Application.Features.Audit.Queries;

public sealed record GetAuditLogsQuery(int Take = 200) : IQuery<IReadOnlyCollection<AuditLogDto>>;
