using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

internal static class BusinessPartnerSyncEventFactory
{
    public static SyncPublishRequest Create(
        int companyId,
        BusinessPartnerDto partner,
        SyncOperation operation)
    {
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

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.BusinessPartner,
            partner.GlobalId,
            partner.Code,
            operation,
            payload,
            SourceSystem: null,
            SourceReference: partner.Id.ToString());
    }
}
