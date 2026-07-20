using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ReferenceCatalogSyncEventApplierTests
{
    [Fact]
    public async Task PaymentTerms_AreAppliedWithApprovedFields()
    {
        var repository = Substitute.For<IReferenceCatalogSyncApplyRepository>();
        var payload = new ReferenceCatalogSyncPayload(
            Guid.NewGuid(), "7", "Credito 30", null, null, null, null, false, true,
            "SAP_B1", "7", DateTime.UtcNow, null, 30, true);
        var wrapper = new
        {
            entityName = SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms,
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = SyncOperation.Updated.ToString(),
            payload
        };
        var context = new SyncEventApplyContext(
            Guid.NewGuid(), 1, SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms, payload.GlobalId,
            SyncOperation.Updated.ToString(),
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            2, 10);
        repository.ApplyAsync(2, context.EntityName, context, Arg.Any<ReferenceCatalogSyncPayload>(),
                SyncOperation.Updated, Arg.Any<CancellationToken>())
            .Returns(new ReferenceCatalogSyncApplyResult(true, false, 5, "Aplicada"));
        var applier = new ReferenceCatalogSyncEventApplier(repository);

        applier.CanApply(SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms).Should().BeTrue();
        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeTrue();
        await repository.Received(1).ApplyAsync(
            2,
            SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms,
            context,
            Arg.Is<ReferenceCatalogSyncPayload>(value =>
                value.GlobalId == payload.GlobalId && value.Days == 30 && value.IsCredit == true
                && value.ExternalSystem == "SAP_B1" && value.ExternalCode == "7"),
            SyncOperation.Updated,
            Arg.Any<CancellationToken>());
    }
}
