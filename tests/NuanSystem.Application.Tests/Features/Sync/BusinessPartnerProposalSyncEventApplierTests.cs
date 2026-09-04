using System.Text.Json;
using System.Text.Json.Nodes;
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

    [Theory]
    [InlineData("[]")]
    [InlineData("null")]
    [InlineData("\"text\"")]
    [InlineData("123")]
    [InlineData("true")]
    public async Task Apply_NonObjectJsonRoot_IsInvalidAndDoesNotReachRepository(string json)
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(Payload()) with { PayloadJson = json };

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

    [Theory]
    [InlineData("proposal-code-null")]
    [InlineData("schema-version-zero")]
    [InlineData("proposal-role-empty")]
    [InlineData("proposal-identification-type-null")]
    [InlineData("proposal-identification-empty")]
    [InlineData("proposal-normalized-empty")]
    [InlineData("negative-base-version")]
    [InlineData("proposed-global-id-empty")]
    [InlineData("proposed-code-empty")]
    [InlineData("proposed-name-null")]
    [InlineData("proposed-role-empty")]
    [InlineData("proposed-identification-type-null")]
    [InlineData("proposed-identification-empty")]
    [InlineData("proposed-normalized-empty")]
    [InlineData("proposed-addresses-null")]
    [InlineData("proposed-contacts-null")]
    [InlineData("changed-fields-null")]
    [InlineData("changed-field-empty")]
    [InlineData("address-null")]
    [InlineData("address-global-id-empty")]
    [InlineData("address-type-null")]
    [InlineData("address-line-empty")]
    [InlineData("contact-null")]
    [InlineData("contact-global-id-empty")]
    [InlineData("contact-name-empty")]
    [InlineData("base-name-empty")]
    public async Task Apply_StructurallyInvalidPayload_IsTerminalBeforeRepository(string scenario)
    {
        var applier = CreateApplier(out var repository, out _);
        var operation = scenario.StartsWith("base-", StringComparison.Ordinal) ? "Updated" : "Created";

        var result = await applier.ApplyAsync(Context(InvalidPayload(scenario), operation));

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData("Created", 4, true)]
    [InlineData("Created", 0, true)]
    [InlineData("Updated", 0, false)]
    [InlineData("Updated", 0, true)]
    [InlineData("Updated", 4, false)]
    public async Task Apply_OperationMustMatchBaseShape(
        string operation,
        long baseCanonicalVersion,
        bool hasBase)
    {
        var applier = CreateApplier(out var repository, out _);
        var payload = Payload() with
        {
            BaseCanonicalVersion = baseCanonicalVersion,
            Base = hasBase ? Snapshot() : null
        };

        var result = await applier.ApplyAsync(Context(payload, operation));

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData("baseCanonicalVersion")]
    [InlineData("base")]
    [InlineData("proposed.isActive")]
    [InlineData("proposed.addresses[0].isPrimary")]
    [InlineData("proposed.contacts[0].receivesNotifications")]
    public async Task Apply_MissingRequiredJsonMember_IsTerminalBeforeRepository(string path)
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(PayloadWithChildren());
        var wrapper = JsonNode.Parse(context.PayloadJson)!.AsObject();
        var payload = wrapper["payload"]!.AsObject();

        switch (path)
        {
            case "baseCanonicalVersion":
            case "base":
                payload.Remove(path);
                break;
            case "proposed.isActive":
                payload["proposed"]!.AsObject().Remove("isActive");
                break;
            case "proposed.addresses[0].isPrimary":
                payload["proposed"]!["addresses"]![0]!.AsObject().Remove("isPrimary");
                break;
            case "proposed.contacts[0].receivesNotifications":
                payload["proposed"]!["contacts"]![0]!.AsObject().Remove("receivesNotifications");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(path), path, null);
        }

        var result = await applier.ApplyAsync(context with { PayloadJson = wrapper.ToJsonString() });

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

    private static SyncEventApplyContext Context(
        BusinessPartnerProposalPayloadV1 payload,
        string operation = "Created")
    {
        var wrapper = new
        {
            entityName = "BusinessPartnerProposal",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation,
            payload
        };

        return new SyncEventApplyContext(
            Guid.Parse("80000000-0000-0000-0000-000000000001"),
            21,
            "BusinessPartnerProposal",
            payload.GlobalId,
            operation,
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            10,
            81);
    }

    private static BusinessPartnerProposalPayloadV1 InvalidPayload(string scenario)
    {
        var payload = PayloadWithChildren();
        var snapshot = payload.Proposed;
        var address = snapshot.Addresses.Single();
        var contact = snapshot.Contacts.Single();

        return scenario switch
        {
            "schema-version-zero" => payload with { SchemaVersion = 0 },
            "proposal-code-null" => payload with { Code = null! },
            "proposal-role-empty" => payload with { PartnerType = " " },
            "proposal-identification-type-null" => payload with { IdentificationTypeCode = null! },
            "proposal-identification-empty" => payload with { IdentificationNumber = "" },
            "proposal-normalized-empty" => payload with { NormalizedIdentificationNumber = " " },
            "negative-base-version" => payload with { BaseCanonicalVersion = -1 },
            "proposed-global-id-empty" => payload with { Proposed = snapshot with { GlobalId = Guid.Empty } },
            "proposed-code-empty" => payload with { Proposed = snapshot with { Code = "" } },
            "proposed-name-null" => payload with { Proposed = snapshot with { Name = null! } },
            "proposed-role-empty" => payload with { Proposed = snapshot with { PartnerType = " " } },
            "proposed-identification-type-null" => payload with { Proposed = snapshot with { IdentificationTypeCode = null! } },
            "proposed-identification-empty" => payload with { Proposed = snapshot with { IdentificationNumber = "" } },
            "proposed-normalized-empty" => payload with { Proposed = snapshot with { NormalizedIdentificationNumber = " " } },
            "proposed-addresses-null" => payload with { Proposed = snapshot with { Addresses = null! } },
            "proposed-contacts-null" => payload with { Proposed = snapshot with { Contacts = null! } },
            "changed-fields-null" => payload with { ChangedFields = null! },
            "changed-field-empty" => payload with { ChangedFields = ["Name", " "] },
            "address-null" => payload with { Proposed = snapshot with { Addresses = [null!] } },
            "address-global-id-empty" => payload with { Proposed = snapshot with { Addresses = [address with { GlobalId = Guid.Empty }] } },
            "address-type-null" => payload with { Proposed = snapshot with { Addresses = [address with { AddressType = null! }] } },
            "address-line-empty" => payload with { Proposed = snapshot with { Addresses = [address with { Line1 = " " }] } },
            "contact-null" => payload with { Proposed = snapshot with { Contacts = [null!] } },
            "contact-global-id-empty" => payload with { Proposed = snapshot with { Contacts = [contact with { GlobalId = Guid.Empty }] } },
            "contact-name-empty" => payload with { Proposed = snapshot with { Contacts = [contact with { Name = "" }] } },
            "base-name-empty" => payload with
            {
                BaseCanonicalVersion = 4,
                Base = snapshot with { Name = " " }
            },
            _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
        };
    }

    private static BusinessPartnerProposalPayloadV1 PayloadWithChildren()
    {
        var address = new BusinessPartnerAddressSnapshot(
            Guid.Parse("11000000-0000-0000-0000-000000000008"),
            "BillTo",
            "Main street",
            null,
            "EC",
            "P",
            "C",
            null,
            null,
            null,
            true,
            true);
        var contact = new BusinessPartnerContactSnapshot(
            Guid.Parse("12000000-0000-0000-0000-000000000008"),
            "OWNER",
            "EMAIL",
            "Contact",
            null,
            null,
            null,
            null,
            null,
            "contact@example.test",
            null,
            true,
            true,
            true,
            null);
        return Payload() with
        {
            Proposed = Snapshot() with { Addresses = [address], Contacts = [contact] }
        };
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
