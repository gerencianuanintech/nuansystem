using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IBusinessPartnerProposalApplyRepository
{
    Task<BusinessPartnerProposalApplyResult> ApplyAsync(
        int centralCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerProposalPayloadV1 proposal,
        CancellationToken cancellationToken = default);
}
