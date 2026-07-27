using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

internal static class CurrencySyncEventFactory
{
    public const string CatalogKey = "currencies";

    public static SyncPublishRequest Create(
        int companyId,
        FinancialCatalogDto currency,
        SyncOperation operation)
    {
        if (currency.GlobalId is null || currency.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("Currencies requiere GlobalId.");
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

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.Currencies,
            currency.GlobalId.Value,
            currency.Code,
            operation,
            payload,
            SourceSystem: currency.ExternalSystem,
            SourceReference: currency.Id.ToString());
    }
}
