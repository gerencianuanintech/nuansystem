using System.Globalization;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapPaymentTermImportService(
    ISapPaymentTermReader reader,
    ISapPaymentTermImportRepository repository,
    ISyncEventPublisher syncEventPublisher) : ISapPaymentTermImportService
{
    private const string ExternalSystem = "SAP_B1";

    public async Task<SapPaymentTermImportResultDto> ImportFullAsync(
        int companyId,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        var rows = await reader.GetAllAsync(companyId, cancellationToken);
        var results = new List<SapPaymentTermImportItemResultDto>(rows.Count);
        var created = 0;
        var updated = 0;
        var unchanged = 0;
        var conflicts = 0;
        var failed = 0;

        foreach (var row in rows)
        {
            if (!IsRepresentable(row, out var reason))
            {
                conflicts++;
                results.Add(new(row.GroupNumber, row.Name, "Conflict", reason));
                continue;
            }

            try
            {
                var code = row.GroupNumber.ToString(CultureInfo.InvariantCulture);
                var saved = await repository.UpsertAsync(
                    new SapPaymentTermUpsertData(
                        Guid.NewGuid(), code, row.Name, row.AdditionalDays, row.AdditionalDays > 0,
                        ExternalSystem, code, auditUserId, auditUserName),
                    cancellationToken);

                if (saved.Status is "Created" or "Updated" or "Unchanged")
                {
                    var published = await PublishAsync(companyId, saved, cancellationToken);
                    if (!published.IsSuccess)
                        throw new InvalidOperationException(published.Message);
                }

                switch (saved.Status)
                {
                    case "Created": created++; break;
                    case "Updated": updated++; break;
                    case "Unchanged": unchanged++; break;
                    default: conflicts++; break;
                }

                results.Add(new(row.GroupNumber, row.Name, saved.Status, saved.Message, saved.Id));
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                failed++;
                results.Add(new(row.GroupNumber, row.Name, "Failed", exception.Message));
            }
        }

        return new(rows.Count, created, updated, unchanged, conflicts, failed, results);
    }

    private static bool IsRepresentable(SapPaymentTermRecord row, out string reason)
    {
        if (row.AdditionalDays < 0)
        {
            reason = "NumberOfAdditionalDays no puede ser negativo.";
            return false;
        }
        if (row.AdditionalMonths != 0)
        {
            reason = "La condicion usa meses adicionales y no puede representarse exactamente como Days.";
            return false;
        }
        if (row.NumberOfInstallments > 1)
        {
            reason = "La condicion usa cuotas y no puede representarse en el modelo local.";
            return false;
        }
        reason = string.Empty;
        return true;
    }

    private Task<Result<SyncPublishResult>> PublishAsync(
        int companyId,
        SapPaymentTermUpsertResult saved,
        CancellationToken cancellationToken)
    {
        var payload = new ReferenceCatalogSyncPayload(
            saved.GlobalId, saved.Code, saved.Name, null, null, null, null, false,
            saved.IsActive, saved.ExternalSystem, saved.ExternalCode, saved.CreatedAt, saved.UpdatedAt,
            saved.Days, saved.IsCredit);
        return syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyId, SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms, saved.GlobalId, saved.Code,
                saved.Status == "Created" ? SyncOperation.Created : SyncOperation.Updated,
                payload, saved.ExternalSystem, saved.ExternalCode),
            cancellationToken);
    }
}
