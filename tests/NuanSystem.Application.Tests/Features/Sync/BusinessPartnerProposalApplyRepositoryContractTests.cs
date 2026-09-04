using FluentAssertions;
using System.Text.Json;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Persistence.Repositories.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class BusinessPartnerProposalApplyRepositoryContractTests
{
    private static readonly Guid PartnerId = Guid.Parse("10000000-0000-0000-0000-000000000008");
    private static readonly BusinessPartnerSapCodePolicyData SapPolicy =
        new(BusinessPartnerSapPrefixMode.NationalForeign, "PASSPORT");

    [Fact]
    public void DurableResultEnvelope_UsesProductionFactoryAndCarriesOriginAndProposalCausation()
    {
        var proposalEventId = Guid.Parse("80000000-0000-0000-0000-000000000008");
        var resultEventId = BusinessPartnerProposalApplyRepository.CreateDeterministicEventId(
            proposalEventId,
            "Rejected");

        var json = BusinessPartnerProposalApplyRepository.CreateResultPayloadJson(
            new SyncEventPayloadFactory(),
            companyId: 10,
            proposalEventId,
            originCompanyId: 21,
            PartnerId,
            status: "Rejected",
            message: "Duplicado.",
            canonicalVersion: 6,
            canonical: Snapshot());

        resultEventId.Should().Be(BusinessPartnerProposalApplyRepository.CreateDeterministicEventId(
            proposalEventId,
            "Rejected"));
        resultEventId.Should().NotBe(BusinessPartnerProposalApplyRepository.CreateDeterministicEventId(
            proposalEventId,
            "Conflict"));
        json.Should().Contain("\"entityName\":\"BusinessPartnerProposalResult\"")
            .And.Contain("\"globalId\":\"10000000-0000-0000-0000-000000000008\"")
            .And.Contain("\"proposalEventId\":\"80000000-0000-0000-0000-000000000008\"")
            .And.Contain("\"originCompanyId\":21")
            .And.Contain("\"status\":\"Rejected\"");
    }

    [Fact]
    public void ProposalRepositoryAndWorkerApplier_AreRegisteredInProductionComposition()
    {
        var root = Root();
        var persistence = File.ReadAllText(Path.Combine(
            root,
            "src", "Backend", "NuanSystem.Persistence", "DependencyInjection",
            "PersistenceServiceRegistration.cs"));
        var worker = File.ReadAllText(Path.Combine(
            root,
            "src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Program.cs"));

        persistence.Should().Contain(
            "AddScoped<IBusinessPartnerProposalApplyRepository, BusinessPartnerProposalApplyRepository>");
        worker.Should().Contain(
            "AddScoped<ISyncEntityEventApplier, BusinessPartnerProposalSyncEventApplier>");
    }

    [Fact]
    public void StableReferences_AreResolvedToLocalIdsAndSerializedInGlobalIdOrder()
    {
        var firstAddressId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var secondAddressId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var contactId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var snapshot = Snapshot() with
        {
            Addresses =
            [
                new(secondAddressId, "ShipTo", "Second", null, null, null, null, null, null, null, false, true),
                new(firstAddressId, "BillTo", "First", null, "EC", "P", "C", null, null, null, true, true)
            ],
            Contacts =
            [
                new(contactId, "ADMIN", "EMAIL", "Contact", null, null, null, null, null, "a@b.test", null, true, true, true, null)
            ]
        };

        var resolution = BusinessPartnerProposalApplyRepository.ResolveStableReferences(
            snapshot,
            new BusinessPartnerProposalApplyRepository.IdentificationReferenceRow(7, 1),
            [
                new(firstAddressId, 11, 1, 12, 1, 13, 1),
                new(secondAddressId, null, 0, null, 0, null, 0)
            ],
            [new(contactId, 21, 1, 22, 1)]);

        resolution.IsComplete.Should().BeTrue();
        resolution.IdentificationTypeId.Should().Be(7);
        using var addresses = JsonDocument.Parse(resolution.AddressesJson!);
        addresses.RootElement[0].GetProperty("globalId").GetGuid().Should().Be(firstAddressId);
        addresses.RootElement[0].GetProperty("countryId").GetInt32().Should().Be(11);
        addresses.RootElement[0].GetProperty("provinceId").GetInt32().Should().Be(12);
        addresses.RootElement[0].GetProperty("cityId").GetInt32().Should().Be(13);
        using var contacts = JsonDocument.Parse(resolution.ContactsJson!);
        contacts.RootElement[0].GetProperty("contactTypeId").GetInt32().Should().Be(21);
        contacts.RootElement[0].GetProperty("contactChannelId").GetInt32().Should().Be(22);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public void StableReferences_MissingOrDuplicateRequiredCodeFailsClosed(int matchCount)
    {
        var addressId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var snapshot = Snapshot() with
        {
            Addresses = [new(addressId, "BillTo", "First", null, "EC", null, null, null, null, null, true, true)]
        };

        var resolution = BusinessPartnerProposalApplyRepository.ResolveStableReferences(
            snapshot,
            new BusinessPartnerProposalApplyRepository.IdentificationReferenceRow(7, 1),
            [new(addressId, 11, matchCount, null, 0, null, 0)],
            []);

        resolution.IsComplete.Should().BeFalse();
        resolution.AddressesJson.Should().BeNull();
        resolution.ContactsJson.Should().BeNull();
    }

    [Fact]
    public void TerminalProcedureParameters_PreserveEnvelopeSnapshotsOriginAndCausation()
    {
        var proposalEventId = Guid.Parse("80000000-0000-0000-0000-000000000008");
        var context = new NuanSystem.Application.Features.Sync.Dtos.SyncEventApplyContext(
            proposalEventId,
            21,
            "BusinessPartnerProposal",
            PartnerId,
            "Updated",
            "{\"originalEnvelope\":true}",
            10);
        var @base = Snapshot() with { Name = "Base" };
        var proposed = @base with { Name = "Branch" };
        var currentSnapshot = @base with { Phone = "222", SapCardCode = "CN0999999999001" };
        var proposal = Proposal() with
        {
            BaseCanonicalVersion = 4,
            Base = @base,
            Proposed = proposed,
            ChangedFields = ["Name"]
        };
        var current = new BusinessPartnerProposalCentralState(80, 6, currentSnapshot);
        var resolution = new BusinessPartnerProposalApplyRepository.StableReferenceResolution(true, 7, "[]", "[]");
        var factory = new SyncEventPayloadFactory();

        var accepted = BusinessPartnerProposalApplyRepository.CreateAcceptParameters(
            factory,
            10,
            context,
            proposal,
            new BusinessPartnerProposalDecision(
                BusinessPartnerProposalApplyOutcome.Accepted,
                7,
                proposed with { SapCardCode = "CN0999999999001" },
                []),
            current,
            resolution);
        var conflict = BusinessPartnerProposalApplyRepository.CreateConflictParameters(
            factory,
            10,
            context,
            proposal,
            new BusinessPartnerProposalDecision(
                BusinessPartnerProposalApplyOutcome.Conflict,
                6,
                currentSnapshot,
                ["Name"],
                "BP_SYNC_CONFLICT"),
            current);
        var rejected = BusinessPartnerProposalApplyRepository.CreateRejectParameters(
            factory,
            10,
            context,
            proposal,
            new BusinessPartnerProposalDecision(
                BusinessPartnerProposalApplyOutcome.Rejected,
                6,
                currentSnapshot,
                [],
                "BP_IDENTIFICATION_DUPLICATE",
                "Duplicado."));

        accepted.Operation.Should().Be("Updated");
        accepted.ProposalPayloadJson.Should().Be(context.PayloadJson);
        accepted.BaseSnapshotJson.Should().Contain("\"name\":\"Base\"").And.NotContain("entityName");
        accepted.ProposedSnapshotJson.Should().Contain("\"name\":\"Branch\"").And.NotContain("entityName");
        accepted.CurrentCanonicalSnapshotJson.Should().Contain("\"phone\":\"222\"").And.NotContain("entityName");
        accepted.CanonicalPayloadJson.Should().Contain("\"originCompanyId\":21")
            .And.Contain($"\"causationEventId\":\"{proposalEventId:D}\"");
        accepted.ResultPayloadJson.Should().Contain("\"originCompanyId\":21")
            .And.Contain($"\"proposalEventId\":\"{proposalEventId:D}\"");
        accepted.CanonicalEventId.Should().Be(
            BusinessPartnerProposalApplyRepository.CreateDeterministicEventId(proposalEventId, "Accepted"));

        conflict.Operation.Should().Be(context.Operation);
        conflict.ProposalPayloadJson.Should().Be(context.PayloadJson);
        conflict.ProposedSnapshotJson.Should().NotContain("entityName");
        conflict.CanonicalSnapshotJson.Should().Contain("\"phone\":\"222\"");
        conflict.ResultPayloadJson.Should().Contain("\"status\":\"Conflict\"");

        rejected.Operation.Should().Be(context.Operation);
        rejected.ProposalPayloadJson.Should().Be(context.PayloadJson);
        rejected.ResultPayloadJson.Should().Contain("\"status\":\"Rejected\"")
            .And.Contain("\"originCompanyId\":21");
    }

    [Theory]
    [InlineData(1, BusinessPartnerProposalApplyOutcome.Accepted, null)]
    [InlineData(2, BusinessPartnerProposalApplyOutcome.Duplicate, null)]
    [InlineData(4, BusinessPartnerProposalApplyOutcome.TerminalFailure, "BP_SYNC_EVENT_ID_COLLISION")]
    [InlineData(5, BusinessPartnerProposalApplyOutcome.Conflict, "BP_SYNC_CONFLICT")]
    public void TerminalProcedureResults_MapDurableReplayCollisionAndDefensiveConflict(
        int resultCode,
        BusinessPartnerProposalApplyOutcome expectedOutcome,
        string? expectedErrorCode)
    {
        var decision = new BusinessPartnerProposalDecision(
            BusinessPartnerProposalApplyOutcome.Accepted,
            7,
            Snapshot(),
            []);

        var result = BusinessPartnerProposalApplyRepository.MapTerminalResult(
            new BusinessPartnerProposalApplyRepository.TerminalProcedureResultRow
            {
                ResultCode = resultCode,
                CanonicalVersion = 8
            },
            decision,
            BusinessPartnerProposalApplyOutcome.Accepted);

        result.Outcome.Should().Be(expectedOutcome);
        result.CanonicalVersion.Should().Be(8);
        result.ErrorCode.Should().Be(expectedErrorCode);
    }

    [Fact]
    public void Create_ValidProposalBecomesCanonicalVersionOne()
    {
        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            Proposal(),
            current: null,
            sameRoleIdentificationExists: false,
            SapPolicy,
            stableReferencesAvailable: true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Accepted);
        decision.CanonicalVersion.Should().Be(1);
        decision.Canonical!.SapCardCode.Should().Be("CN0999999999001");
    }

    [Fact]
    public void Create_DuplicateIdentificationInSameRoleIsRejected()
    {
        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            Proposal(), null, true, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Rejected);
        decision.ErrorCode.Should().Be("BP_IDENTIFICATION_DUPLICATE");
        decision.Canonical.Should().BeNull();
    }

    [Fact]
    public void Create_SameIdentificationInDifferentRoleIsAllowed()
    {
        var supplier = Proposal() with
        {
            PartnerType = "Supplier",
            Proposed = Snapshot() with { PartnerType = "Supplier" }
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            supplier, null, false, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Accepted);
        decision.Canonical!.PartnerType.Should().Be("Supplier");
        decision.Canonical.SapCardCode.Should().Be("PL0999999999001");
    }

    [Fact]
    public void Create_LegacyBothRoleIsRejected()
    {
        var proposal = Proposal() with
        {
            PartnerType = "Both",
            Proposed = Snapshot() with { PartnerType = "Both" }
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal, null, false, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Rejected);
        decision.ErrorCode.Should().Be("BP_ROLE_INVALID");
    }

    [Fact]
    public void Create_ManipulatedNormalizedIdentificationIsRejected()
    {
        var proposal = Proposal() with
        {
            NormalizedIdentificationNumber = "MANIPULATED",
            Proposed = Snapshot() with { NormalizedIdentificationNumber = "MANIPULATED" }
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal, null, false, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Rejected);
        decision.ErrorCode.Should().Be("BP_NORMALIZED_IDENTIFICATION_MISMATCH");
    }

    [Fact]
    public void Create_ManipulatedInternalCodeIsRejected()
    {
        var proposal = Proposal() with
        {
            Code = "C-USER",
            Proposed = Snapshot() with { Code = "C-USER" }
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal, null, false, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Rejected);
        decision.ErrorCode.Should().Be("BP_INTERNAL_CODE_MISMATCH");
    }

    [Fact]
    public void Create_NonNullBaseSnapshotIsRejectedAsMalformedProposal()
    {
        var proposal = Proposal() with { Base = Snapshot() };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal, null, false, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Rejected);
        decision.ErrorCode.Should().Be("BP_SYNC_CREATE_BASE_NOT_ALLOWED");
    }

    [Theory]
    [InlineData(false, null)]
    [InlineData(true, "USER-SAP-CODE")]
    public void Create_ProtectedActivationOrSapCardCodeIsRejected(bool isActive, string? sapCardCode)
    {
        var proposal = Proposal() with
        {
            Proposed = Snapshot() with { IsActive = isActive, SapCardCode = sapCardCode }
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal, null, false, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Rejected);
        decision.ErrorCode.Should().Be("BP_PROTECTED_FIELD");
    }

    [Fact]
    public void Create_SapCardCodeOverFifteenCharactersIsRejected()
    {
        const string identification = "123456789012345";
        var proposal = Proposal() with
        {
            IdentificationNumber = identification,
            NormalizedIdentificationNumber = identification,
            Proposed = Snapshot() with
            {
                IdentificationNumber = identification,
                NormalizedIdentificationNumber = identification
            }
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal, null, false, SapPolicy, true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Rejected);
        decision.ErrorCode.Should().Be("BP_SAP_CARD_CODE_TOO_LONG");
    }

    [Fact]
    public void MissingStableReferenceIsRetryableAndDoesNotProduceCanonicalState()
    {
        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            Proposal(), null, false, SapPolicy, stableReferencesAvailable: false);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.RetryableFailure);
        decision.ErrorCode.Should().Be("BP_SYNC_REFERENCE_NOT_FOUND");
        decision.Canonical.Should().BeNull();
    }

    [Fact]
    public void MissingEnabledCentralSapPolicyIsRetryableAndUnconsumed()
    {
        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            Proposal(), null, false, sapPolicy: null, stableReferencesAvailable: true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.RetryableFailure);
        decision.ErrorCode.Should().Be("BP_SAP_CODE_POLICY_REQUIRED");
        decision.Canonical.Should().BeNull();
    }

    [Fact]
    public void Update_UsesThreeWayMergeAndIncrementsCurrentVersion()
    {
        var @base = Snapshot() with { Name = "Base", Phone = "111" };
        var proposed = @base with { Name = "Branch" };
        var current = @base with { Phone = "222", SapCardCode = "CN0999999999001" };
        var proposal = Proposal() with
        {
            BaseCanonicalVersion = 4,
            Base = @base,
            Proposed = proposed,
            ChangedFields = ["Name"]
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal,
            new BusinessPartnerProposalCentralState(80, 6, current),
            false,
            SapPolicy,
            true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Accepted);
        decision.CanonicalVersion.Should().Be(7);
        decision.Canonical!.Name.Should().Be("Branch");
        decision.Canonical.Phone.Should().Be("222");
        decision.Canonical.SapCardCode.Should().Be("CN0999999999001");
    }

    [Fact]
    public void Update_CalculatesSapCardCodeOnlyWhenCurrentMappingIsMissing()
    {
        var @base = Snapshot() with { Name = "Base", SapCardCode = null };
        var proposal = Proposal() with
        {
            BaseCanonicalVersion = 4,
            Base = @base,
            Proposed = @base with { Name = "Branch" },
            ChangedFields = ["Name"]
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal,
            new BusinessPartnerProposalCentralState(80, 6, @base),
            false,
            SapPolicy,
            true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Accepted);
        decision.Canonical!.SapCardCode.Should().Be("CN0999999999001");
    }

    [Fact]
    public void Update_PreservesConfirmedSapCardCodeWithoutRecalculatingAgainstCurrentPolicy()
    {
        const string identification = "123456789012345";
        const string confirmedSapCardCode = "LEGACY-CONFIRMED-CARD-CODE";
        var @base = Snapshot() with
        {
            IdentificationNumber = identification,
            NormalizedIdentificationNumber = identification,
            SapCardCode = confirmedSapCardCode
        };
        var proposal = Proposal() with
        {
            IdentificationNumber = identification,
            NormalizedIdentificationNumber = identification,
            BaseCanonicalVersion = 4,
            Base = @base,
            Proposed = @base with { Name = "Branch" },
            ChangedFields = ["Name"]
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal,
            new BusinessPartnerProposalCentralState(80, 6, @base),
            false,
            SapPolicy,
            true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Accepted);
        decision.Canonical!.SapCardCode.Should().Be(confirmedSapCardCode);
    }

    [Fact]
    public void Update_SameFieldDivergenceCreatesStableConflict()
    {
        var @base = Snapshot() with { Name = "Base" };
        var proposal = Proposal() with
        {
            BaseCanonicalVersion = 4,
            Base = @base,
            Proposed = @base with { Name = "Branch" },
            ChangedFields = ["Name"]
        };

        var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
            proposal,
            new BusinessPartnerProposalCentralState(80, 6, @base with { Name = "Central" }),
            false,
            SapPolicy,
            true);

        decision.Outcome.Should().Be(BusinessPartnerProposalApplyOutcome.Conflict);
        decision.ConflictFields.Should().Equal("Name");
        decision.Canonical!.Name.Should().Be("Central");
        decision.CanonicalVersion.Should().Be(6);
    }

    private static BusinessPartnerProposalPayloadV1 Proposal() =>
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

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
