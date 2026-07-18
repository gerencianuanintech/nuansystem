using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

internal static class CurrencySyncPublisher
{
    private const string CatalogKey = "currencies";
    private const string EntityName = SyncMasterBranchEntityCodes.Currencies;

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        string catalogKey,
        FinancialCatalogDto currency,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(catalogKey, CatalogKey, StringComparison.OrdinalIgnoreCase)
            || !companyContext.HasActiveCompany
            || companyContext.CurrentCompany is null)
        {
            return null;
        }

        if (currency.GlobalId is null || currency.GlobalId == Guid.Empty)
        {
            return Result<SyncPublishResult>.Failure(
                "La moneda no tiene GlobalId y no puede publicarse para sincronizacion.",
                [new ApiError("SYNC_CURRENCY_GLOBAL_ID_REQUIRED", "Currencies requiere GlobalId.", nameof(currency.GlobalId))]);
        }

        var payload = new CurrencySyncPayload(
            currency.GlobalId.Value,
            currency.Code,
            currency.Name,
            currency.Symbol,
            currency.Description,
            currency.IsBaseCurrency,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && currency.IsActive,
            currency.ExternalSystem,
            currency.ExternalCode,
            currency.CreatedAt,
            currency.UpdatedAt);

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                currency.GlobalId.Value,
                currency.Code,
                operation,
                payload,
                SourceSystem: currency.ExternalSystem,
                SourceReference: currency.Id.ToString()),
            cancellationToken);
    }
}
