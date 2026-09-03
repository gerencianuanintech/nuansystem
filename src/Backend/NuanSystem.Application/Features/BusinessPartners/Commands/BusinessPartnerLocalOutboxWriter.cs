using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class BusinessPartnerLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IBusinessPartnerLocalOutboxWriter
{
    private readonly BusinessPartnerSnapshotFactory snapshotFactory = new();

    public async Task<Guid?> EnqueueAsync(
        BusinessPartnerOutboxWriteRequest write,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(write);
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.SyncEnabled)
        {
            return null;
        }

        if (BusinessPartnerWritePolicy.RequiresLegacyReview(write.Current.MasterSyncStatus))
        {
            return null;
        }

        if (write.Current.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("BusinessPartner requiere GlobalId para registrar LocalOutbox.");
        }

        var current = snapshotFactory.Create(write.Current);
        SyncPublishRequest publish;
        int? targetCompanyId;
        if (company.IsMaster)
        {
            publish = BusinessPartnerSyncEventFactory.CreateCanonical(company.CompanyId, write, current);
            targetCompanyId = null;
        }
        else
        {
            if (company.ParentCompanyId is not > 0)
            {
                throw new InvalidOperationException(
                    "BP_BRANCH_PARENT_REQUIRED: la sucursal sincronizada requiere una empresa central padre.");
            }

            if (write.Operation != SyncOperation.Created && write.Base is null)
            {
                throw new InvalidOperationException("BusinessPartner requiere Base para publicar una actualizacion de sucursal.");
            }

            var @base = write.Operation == SyncOperation.Created || write.Base is null
                ? null
                : snapshotFactory.Create(write.Base);
            if (@base is not null && @base.GlobalId != current.GlobalId)
            {
                throw new InvalidOperationException("BusinessPartner Base y Current deben identificar el mismo GlobalId.");
            }
            publish = BusinessPartnerSyncEventFactory.CreateProposal(company.CompanyId, write, current, @base);
            targetCompanyId = company.ParentCompanyId;
        }

        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(
            new CreateLocalSyncOutboxData(
                eventId,
                company.CompanyId,
                publish.EntityName,
                write.Current.GlobalId,
                write.Current.Code,
                write.Operation,
                payloadFactory.CreatePayloadJson(publish),
                TargetCompanyId: targetCompanyId,
                CausationEventId: write.CausationEventId),
            connection,
            transaction,
            cancellationToken);
        return eventId;
    }
}
