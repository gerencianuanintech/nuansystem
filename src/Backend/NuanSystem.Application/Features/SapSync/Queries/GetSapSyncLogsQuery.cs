using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed record GetSapSyncLogsQuery : IQuery<IReadOnlyCollection<SapSyncLogDto>>;
