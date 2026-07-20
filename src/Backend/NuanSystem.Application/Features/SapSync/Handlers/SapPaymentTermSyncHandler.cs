using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Handlers;

public sealed class SapPaymentTermSyncHandler(ISapPaymentTermImportService service) : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.PaymentTerms;

    public async Task<SapSyncExecutionResult> ImportFromSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var result = await service.ImportFullAsync(context.CompanyId, null, "SAP Sync Worker", cancellationToken);
        var details = result.Items
            .Where(item => item.Status is "Conflict" or "Failed")
            .Take(5)
            .Select(item => $"SAP {item.GroupNumber}: {item.Message}")
            .ToArray();
        var detailText = details.Length == 0 ? string.Empty : $" Detalle: {string.Join(" | ", details)}";
        var message = $"Condiciones SAP leidas: {result.TotalRead}, creadas: {result.Created}, actualizadas: {result.Updated}, sin cambios: {result.Unchanged}, conflictos: {result.Conflicted}, fallidas: {result.Failed}.{detailText}";
        return result.Failed > 0
            ? new(SapSyncStatus.Failed, message, result.TotalRead, result.Failed, "PAYMENT_TERMS_IMPORT_FAILED", message)
            : new(SapSyncStatus.Synced, message, result.TotalRead);
    }

    public Task<SapSyncExecutionResult> ExportToSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(SapSyncExecutionResult.NotImplemented("Condiciones de pago ERP a SAP no forman parte del alcance aprobado."));
}
