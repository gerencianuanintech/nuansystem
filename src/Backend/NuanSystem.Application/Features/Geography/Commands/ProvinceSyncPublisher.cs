using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Geography.Commands;

internal static class ProvinceSyncPublisher
{
    private const string EntityName = SyncMasterBranchEntityCodes.Provinces;

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        ProvinceDto province,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            return null;
        }

        var payload = new ProvinceSyncPayload(
            province.GlobalId,
            province.CountryGlobalId,
            province.CountryCode,
            province.Code,
            province.Name,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && province.IsActive,
            province.CreatedAt,
            province.UpdatedAt);

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                province.GlobalId,
                $"{province.CountryCode}|{province.Code}",
                operation,
                payload,
                SourceSystem: null,
                SourceReference: province.Id.ToString()),
            cancellationToken);
    }
}
