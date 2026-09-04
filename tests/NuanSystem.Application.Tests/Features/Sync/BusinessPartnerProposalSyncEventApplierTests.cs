using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.MasterBranchSyncWorker.Services;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class BusinessPartnerProposalSyncEventApplierTests
{
    private static readonly Guid PartnerId = Guid.Parse("10000000-0000-0000-0000-000000000008");

    [Fact]
    public void CanApply_UsesOnlyProposalEntityCode()
    {
        var applier = CreateApplier(out _, out _);

        applier.CanApply("BusinessPartnerProposal").Should().BeTrue();
        applier.CanApply("businesspartnerproposal").Should().BeTrue();
        applier.CanApply("BusinessPartner").Should().BeFalse();
    }

    [Fact]
    public async Task Apply_InvalidJson_IsTerminalAndDoesNotReachRepository()
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(Payload()) with { PayloadJson = "{invalid" };

        var result = await applier.ApplyAsync(context);

        result.Should().BeEquivalentTo(new SyncEventApplyResult(
            false,
            "Payload de propuesta de socio no es JSON valido.",
            "SYNC_PAYLOAD_INVALID",
            Retryable: false,
            Terminal: true));
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_UnsupportedSchema_IsTerminal()
    {
        var applier = CreateApplier(out var repository, out _);
        var payload = Payload() with { SchemaVersion = 2 };

        var result = await applier.ApplyAsync(Context(payload));

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_PROPOSAL_SCHEMA_UNSUPPORTED");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_WrongEntityName_IsTerminalAndDoesNotReachRepository()
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(Payload()) with { EntityName = "BusinessPartner" };

        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_ENTITY_UNSUPPORTED");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_NullProposedAggregate_IsInvalidPayloadAndDoesNotReachRepository()
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(Payload() with { Proposed = null! });

        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_SourceMustBeRegisteredBranch()
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>()).Returns(Central(21));

        var result = await applier.ApplyAsync(Context(Payload()));

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_SOURCE_BRANCH_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_UnsupportedOperation_IsTerminal()
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(Payload()) with { Operation = "Deleted" };

        var result = await applier.ApplyAsync(context);

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_OPERATION_UNSUPPORTED");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_EntityGlobalIdMustMatchPayload()
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(Payload()) with { EntityGlobalId = Guid.NewGuid() };

        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_GLOBAL_ID_MISMATCH");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_TargetIsRequired()
    {
        var applier = CreateApplier(out var repository, out _);

        var result = await applier.ApplyAsync(Context(Payload()) with { TargetCompanyId = null });

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_TARGET_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_TargetMustBeCentral()
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>()).Returns(Branch(10, parentId: 1));

        var result = await applier.ApplyAsync(Context(Payload()));

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_TARGET_CENTRAL_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_TargetMustBeTheSourceBranchParent()
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>()).Returns(Branch(21, parentId: 99));

        var result = await applier.ApplyAsync(Context(Payload()));

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_PARENT_MISMATCH");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task Apply_BothCompaniesMustHaveSyncEnabled(bool sourceEnabled, bool targetEnabled)
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>())
            .Returns(Branch(21, parentId: 10, syncEnabled: sourceEnabled));
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>())
            .Returns(Central(10, syncEnabled: targetEnabled));

        var result = await applier.ApplyAsync(Context(Payload()));

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_DISABLED");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData(BusinessPartnerProposalApplyOutcome.Accepted)]
    [InlineData(BusinessPartnerProposalApplyOutcome.Rejected)]
    [InlineData(BusinessPartnerProposalApplyOutcome.Conflict)]
    [InlineData(BusinessPartnerProposalApplyOutcome.Duplicate)]
    public async Task Apply_DurableBusinessOutcomeConsumesInbox(
        BusinessPartnerProposalApplyOutcome outcome)
    {
        var applier = CreateApplier(out var repository, out _);
        repository.ApplyAsync(10, Arg.Any<SyncEventApplyContext>(), Arg.Any<BusinessPartnerProposalPayloadV1>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerProposalApplyResult(outcome, 7, "Durable."));

        var result = await applier.ApplyAsync(Context(Payload()));

        result.Applied.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.Terminal.Should().BeFalse();
    }

    [Fact]
    public async Task Apply_MissingStableReferenceRemainsRetryableAndUnconsumed()
    {
        var applier = CreateApplier(out var repository, out _);
        repository.ApplyAsync(10, Arg.Any<SyncEventApplyContext>(), Arg.Any<BusinessPartnerProposalPayloadV1>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerProposalApplyResult(
                BusinessPartnerProposalApplyOutcome.RetryableFailure,
                0,
                "Referencia pendiente.",
                "BP_SYNC_REFERENCE_NOT_FOUND"));

        var result = await applier.ApplyAsync(Context(Payload()));

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeTrue();
        result.Terminal.Should().BeFalse();
        result.ErrorCode.Should().Be("BP_SYNC_REFERENCE_NOT_FOUND");
    }

    [Fact]
    public async Task Apply_TechnicalRepositoryFailureEscapesForWorkerRetry()
    {
        var applier = CreateApplier(out var repository, out _);
        repository.ApplyAsync(10, Arg.Any<SyncEventApplyContext>(), Arg.Any<BusinessPartnerProposalPayloadV1>(), Arg.Any<CancellationToken>())
            .Returns<Task<BusinessPartnerProposalApplyResult>>(_ => throw new InvalidOperationException("SQL unavailable"));

        var action = () => applier.ApplyAsync(Context(Payload()));

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SQL unavailable");
    }

    private static BusinessPartnerProposalSyncEventApplier CreateApplier(
        out IBusinessPartnerProposalApplyRepository repository,
        out ICompanyResolver companies)
    {
        repository = Substitute.For<IBusinessPartnerProposalApplyRepository>();
        companies = Substitute.For<ICompanyResolver>();
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>()).Returns(Branch(21, parentId: 10));
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>()).Returns(Central(10));
        return new BusinessPartnerProposalSyncEventApplier(repository, companies);
    }

    private static SyncEventApplyContext Context(BusinessPartnerProposalPayloadV1 payload)
    {
        var wrapper = new
        {
            entityName = "BusinessPartnerProposal",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = "Created",
            payload
        };

        return new SyncEventApplyContext(
            Guid.Parse("80000000-0000-0000-0000-000000000001"),
            21,
            "BusinessPartnerProposal",
            payload.GlobalId,
            "Created",
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            10,
            81);
    }

    private static BusinessPartnerProposalPayloadV1 Payload() =>
        new(
            1,
            PartnerId,
            "BP-10000000000000000000000000000008",
            "Customer",
            "RUC",
            "09.999-999 99001",
            "0999999999001",
            0,
            7,
            "branch-user",
            null,
            Snapshot(),
            ["Name"]);

    private static BusinessPartnerCanonicalSnapshot Snapshot() =>
        new(
            PartnerId,
            "BP-10000000000000000000000000000008",
            "Partner",
            null,
            "Customer",
            "RUC",
            "09.999-999 99001",
            "0999999999001",
            null,
            null,
            null,
            true,
            [],
            []);

    private static CompanyConnectionInfo Central(int id, bool syncEnabled = true) =>
        new(id, $"C{id}", "Central", DatabaseEngine.SqlServer, "Server=central;", SapIntegrationMode.None,
            CompanyOperationMode.Standalone, IsMaster: true, ParentCompanyId: null, BranchCode: null, SyncEnabled: syncEnabled);

    private static CompanyConnectionInfo Branch(int id, int parentId, bool syncEnabled = true) =>
        new(id, $"B{id}", "Branch", DatabaseEngine.SqlServer, "Server=branch;", SapIntegrationMode.None,
            CompanyOperationMode.Standalone, IsMaster: false, ParentCompanyId: parentId, BranchCode: $"B{id}", SyncEnabled: syncEnabled);
}
