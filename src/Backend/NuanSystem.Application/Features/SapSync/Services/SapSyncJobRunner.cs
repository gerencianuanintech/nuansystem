using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncJobRunner : ISapSyncJobRunner
{
    public Task<SapSyncExecutionResult> RunOutboxAsync(SapSyncOutboxItemDto item, string workerInstance, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.NotImplemented("Envio ERP a SAP queda pendiente para fase 3."));

    public Task<SapSyncExecutionResult> RunInboxAsync(SapSyncInboxItemDto item, string workerInstance, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.Skipped("Inbox de proveedores se procesa directamente en el handler de fase 2."));
}
