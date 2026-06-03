using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Handlers;

public sealed class SapItemSyncHandler : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.Items;
    public Task<SapSyncExecutionResult> ImportFromSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.NotImplemented("Items queda pendiente para fase 3."));
    public Task<SapSyncExecutionResult> ExportToSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.NotImplemented("Items ERP a SAP queda pendiente."));
}
