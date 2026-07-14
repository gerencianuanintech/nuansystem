using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

internal static class BusinessPartnerSyncPublisher
{
    private const string EntityName = "BusinessPartner";

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        BusinessPartnerDto partner,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            return null;
        }

        var payload = new BusinessPartnerSyncPayload(
            partner.GlobalId,
            partner.Code,
            partner.Name,
            partner.CommercialName,
            partner.PartnerType,
            partner.IdentificationTypeCode,
            partner.IdentificationNumber,
            partner.Email,
            partner.Phone,
            partner.IsActive,
            partner.ExternalSystem,
            partner.ExternalCode);

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                partner.GlobalId,
                partner.Code,
                operation,
                payload,
                SourceSystem: null,
                SourceReference: partner.Id.ToString()),
            cancellationToken);
    }
}
