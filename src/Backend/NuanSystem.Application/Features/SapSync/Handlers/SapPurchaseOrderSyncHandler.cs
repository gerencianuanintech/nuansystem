using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Handlers;

public sealed class SapPurchaseOrderSyncHandler : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.PurchaseOrders;
    public Task<SapSyncExecutionResult> ImportFromSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.NotImplemented("Ordenes de compra queda pendiente para fase 3."));
    public Task<SapSyncExecutionResult> ExportToSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.NotImplemented("Envio ERP a SAP queda pendiente."));
}
