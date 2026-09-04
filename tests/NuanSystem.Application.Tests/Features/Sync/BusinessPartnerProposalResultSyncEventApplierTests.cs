using FluentAssertions;
using System.Text.Json.Nodes;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Persistence.Repositories.Sync;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class BusinessPartnerProposalResultSyncEventApplierTests
{
    private static readonly Guid PartnerId = Guid.Parse("10000000-0000-0000-0000-000000000019");

    [Fact]
    public void CanApply_AcceptsOnlyProposalResultEntity()
    {
        var applier = CreateApplier(out _, out _);
        applier.CanApply("BusinessPartnerProposalResult").Should().BeTrue();
        applier.CanApply("businesspartnerproposalresult").Should().BeTrue();
        applier.CanApply("BusinessPartner").Should().BeFalse();
    }

    [Fact]
    public void WorkerComposition_RegistersProposalResultApplier()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        directory.Should().NotBeNull();
        var program = File.ReadAllText(Path.Combine(directory!.FullName,
            "src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Program.cs"));

        program.Should().Contain(
            "AddScoped<ISyncEntityEventApplier, BusinessPartnerProposalResultSyncEventApplier>()");
    }

    [Theory]
    [InlineData(2, "Rejected", "BP_SYNC_RESULT_SCHEMA_UNSUPPORTED")]
    [InlineData(1, "Unknown", "BP_SYNC_RESULT_STATUS_UNSUPPORTED")]
    public async Task Apply_RejectsUnsupportedSchemaAndStatus(int schema, string status, string expectedCode)
    {
        var applier = CreateApplier(out var repository, out _);
        var payload = Result(status) with { SchemaVersion = schema };
        var result = await applier.ApplyAsync(Context(payload));
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be(expectedCode);
        await repository.DidNotReceiveWithAnyArgs().ApplyProposalResultAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_ResultCanOnlyReachItsOriginBranch()
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(22, Arg.Any<CancellationToken>()).Returns(Branch(22, 10));
        var result = await applier.ApplyAsync(Context(Result("Rejected"), 22));
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_RESULT_TARGET_MISMATCH");
        await repository.DidNotReceiveWithAnyArgs().ApplyProposalResultAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_DuplicateCanonicalChildGlobalIdsAreTerminal()
    {
        var childId = Guid.Parse("20000000-0000-0000-0000-000000000199");
        var payload = Result("Rejected") with
        {
            Canonical = Snapshot() with
            {
                Addresses =
                [
                    new(childId, "Billing", "One", null, "EC", null, null, null, null, null, true, true),
                    new(childId, "Shipping", "Two", null, "EC", null, null, null, null, null, false, true)
                ]
            }
        };
        var applier = CreateApplier(out var repository, out _);

        var result = await applier.ApplyAsync(Context(payload));

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyProposalResultAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData("extra-root")]
    [InlineData("missing-root-code")]
    [InlineData("result-code-string")]
    [InlineData("numeric-root-operation")]
    [InlineData("root-entity-mismatch")]
    [InlineData("root-global-mismatch")]
    [InlineData("root-operation-mismatch")]
    [InlineData("missing-message")]
    [InlineData("missing-canonical")]
    [InlineData("missing-is-active")]
    [InlineData("addresses-not-array")]
    [InlineData("blank-address-line")]
    [InlineData("missing-address-global-id")]
    [InlineData("blank-contact-name")]
    [InlineData("missing-contact-global-id")]
    [InlineData("missing-contact-code")]
    [InlineData("string-contact-bool")]
    [InlineData("numeric-address-code")]
    public async Task Apply_ValidJsonWithInvalidResultWireShapeIsTerminal(string mutation)
    {
        var applier = CreateApplier(out var repository, out _);
        repository.ApplyProposalResultAsync(default, default!, default!, default)
            .ReturnsForAnyArgs(new BusinessPartnerSyncApplyResult(true, false, 80, "Applied."));

        var result = await applier.ApplyAsync(MutatedResultContext(mutation));

        result.Should().BeEquivalentTo(new SyncEventApplyResult(
            false, "Payload de resultado de socio no es JSON valido.", "SYNC_PAYLOAD_INVALID", false, true));
        await repository.DidNotReceiveWithAnyArgs().ApplyProposalResultAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_ConflictWithoutCanonicalIsInvalidBeforeRepository()
    {
        var applier = CreateApplier(out var repository, out _);
        var payload = Result("Conflict") with { Canonical = null };

        var result = await applier.ApplyAsync(Context(payload));

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyProposalResultAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    public async Task Apply_RejectedWithoutCanonicalRequiresCreationVersionZero(long canonicalVersion)
    {
        var applier = CreateApplier(out var repository, out _);
        var payload = Result("Rejected") with { Canonical = null, CanonicalVersion = canonicalVersion };

        var result = await applier.ApplyAsync(Context(payload));

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyProposalResultAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_RejectedWithoutCanonicalAtCreationVersionCanReachRepository()
    {
        var applier = CreateApplier(out var repository, out _);
        var payload = Result("Rejected") with { Canonical = null, CanonicalVersion = 0 };
        var context = Context(payload);
        repository.ApplyProposalResultAsync(21, context, payload, Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncApplyResult(true, false, 80, "Consumed."));

        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeTrue();
        await repository.Received(1).ApplyProposalResultAsync(21, context, payload, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("Conflict")]
    [InlineData("Rejected")]
    [InlineData("Accepted")]
    public async Task Apply_ValidResultUsesCentralToOriginTopology(string status)
    {
        var applier = CreateApplier(out var repository, out _);
        var payload = Result(status);
        var context = Context(payload);
        repository.ApplyProposalResultAsync(21, context, Arg.Any<BusinessPartnerProposalResultPayloadV1>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncApplyResult(true, false, 80, "Consumido."));
        var result = await applier.ApplyAsync(context);
        result.Applied.Should().BeTrue();
        await repository.Received(1).ApplyProposalResultAsync(21, context,
            Arg.Is<BusinessPartnerProposalResultPayloadV1>(value => value.OriginCompanyId == 21 && value.Status == status),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false, true, true, "BP_SYNC_SOURCE_CENTRAL_REQUIRED")]
    [InlineData(true, false, true, "BP_SYNC_TARGET_BRANCH_REQUIRED")]
    [InlineData(true, true, false, "BP_SYNC_PARENT_MISMATCH")]
    public async Task Apply_RequiresCentralToOriginChildTopology(
        bool sourceIsCentral, bool targetIsBranch, bool parentMatches, string expectedCode)
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>())
            .Returns(sourceIsCentral ? Central(10) : Branch(10, 99));
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>())
            .Returns(targetIsBranch ? Branch(21, parentMatches ? 10 : 99) : Central(21));
        var result = await applier.ApplyAsync(Context(Result("Rejected")));
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be(expectedCode);
        await repository.DidNotReceiveWithAnyArgs().ApplyProposalResultAsync(default, default!, default!, default);
    }

    [Fact]
    public void ResultParameters_PreserveOrRestoreAccordingToStatusWithoutPublishing()
    {
        var rejected = Result("Rejected");
        var noCanonical = rejected with { Canonical = null, CanonicalVersion = 0 };
        var conflict = rejected with { Status = "Conflict" };
        var accepted = rejected with { Status = "Accepted" };
        var references = new BusinessPartnerSyncApplyRepository.StableReferenceResolution(true, 7, "[]", "[]");
        var restore = BusinessPartnerSyncApplyRepository.CreateProposalResultParameters(
            Context(rejected), rejected, references);
        var preserve = BusinessPartnerSyncApplyRepository.CreateProposalResultParameters(
            Context(noCanonical), noCanonical, null);
        var conflictParameters = BusinessPartnerSyncApplyRepository.CreateProposalResultParameters(
            Context(conflict), conflict, null);
        var acceptedParameters = BusinessPartnerSyncApplyRepository.CreateProposalResultParameters(
            Context(accepted), accepted, null);
        restore.Status.Should().Be("Rejected");
        restore.HasCanonical.Should().BeTrue();
        restore.Name.Should().Be("Central");
        preserve.HasCanonical.Should().BeFalse();
        preserve.Name.Should().BeNull();
        conflictParameters.HasCanonical.Should().BeTrue("the procedure needs its version to ignore stale conflicts");
        conflictParameters.Name.Should().BeNull("conflicts must never canonical-upsert");
        acceptedParameters.HasCanonical.Should().BeFalse("Accepted depends on the separate canonical event");
        typeof(BusinessPartnerSyncApplyRepository).GetConstructors().Single().GetParameters()
            .Should().ContainSingle(parameter => parameter.ParameterType == typeof(ICompanyResolver));
    }

    [Theory]
    [InlineData(1, true, false, false)]
    [InlineData(2, true, true, false)]
    [InlineData(3, true, true, true)]
    [InlineData(4, false, false, false)]
    [InlineData(5, false, false, false)]
    [InlineData(6, false, false, false)]
    public void ResultProcedureResult_ClosesAppliedReplayStaleAndTerminalOutcomes(
        int resultCode, bool applied, bool alreadyApplied, bool ignored)
    {
        var result = BusinessPartnerSyncApplyRepository.MapProposalResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow
            {
                ResultCode = resultCode,
                BusinessPartnerId = 80
            });
        result.Applied.Should().Be(applied);
        result.AlreadyApplied.Should().Be(alreadyApplied);
        result.Ignored.Should().Be(ignored);
        result.Terminal.Should().Be(resultCode is 4 or 5 or 6);
        if (resultCode == 6)
            result.ErrorCode.Should().Be("BP_SYNC_EVENT_ID_COLLISION");
        if (resultCode == 4)
            result.ErrorCode.Should().Be("BP_SYNC_RESULT_INVALID");
    }

    [Theory]
    [InlineData("Accepted", false)]
    [InlineData("Conflict", true)]
    [InlineData("Rejected", true)]
    public void ResultPreflight_DedupesEveryStatusAndComparesEveryNonAcceptedResult(
        string status,
        bool compareCanonicalVersion)
    {
        var payload = Result(status);
        var context = Context(payload);

        var parameters = BusinessPartnerSyncApplyRepository.CreateProposalResultPreflightParameters(context, payload);

        parameters.EntityName.Should().Be("BusinessPartnerProposalResult");
        parameters.Operation.Should().Be("Updated");
        parameters.CanonicalVersion.Should().Be(payload.CanonicalVersion);
        parameters.CompareCanonicalVersion.Should().Be(compareCanonicalVersion);
        parameters.EqualVersionIsReplay.Should().BeFalse();
        BusinessPartnerSyncApplyRepository.MapProposalResultPreflightResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow { ResultCode = 4 }).Should().Match<BusinessPartnerSyncApplyResult>(
                result => result.Terminal && result.ErrorCode == "BP_SYNC_EVENT_ID_COLLISION");
    }

    [Fact]
    public void ResultPreflight_DeadLetterReplayIsAStableTerminalShortCircuit()
    {
        var result = BusinessPartnerSyncApplyRepository.MapProposalResultPreflightResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow { ResultCode = 5 });

        result.Should().NotBeNull();
        result!.Applied.Should().BeFalse();
        result.AlreadyApplied.Should().BeFalse();
        result.Ignored.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.ErrorCode.Should().Be("BP_SYNC_EVENT_ALREADY_TERMINAL");
    }

    private static BusinessPartnerProposalResultSyncEventApplier CreateApplier(
        out IBusinessPartnerSyncApplyRepository repository, out ICompanyResolver companies)
    {
        repository = Substitute.For<IBusinessPartnerSyncApplyRepository>();
        companies = Substitute.For<ICompanyResolver>();
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>()).Returns(Central(10));
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>()).Returns(Branch(21, 10));
        return new BusinessPartnerProposalResultSyncEventApplier(repository, companies);
    }

    private static SyncEventApplyContext Context(BusinessPartnerProposalResultPayloadV1 payload, int target = 21)
    {
        var json = new SyncEventPayloadFactory().CreatePayloadJson(new SyncPublishRequest(10,
            "BusinessPartnerProposalResult", payload.GlobalId, null, SyncOperation.Updated, payload, null, null));
        return new SyncEventApplyContext(Guid.Parse("90000000-0000-0000-0000-000000000019"), 10,
            "BusinessPartnerProposalResult", payload.GlobalId, "Updated", json, target, 91);
    }

    private static BusinessPartnerProposalResultPayloadV1 Result(string status) =>
        new(1, PartnerId, Guid.Parse("80000000-0000-0000-0000-000000000019"), 21,
            status, "Outcome.", 7, Snapshot());

    private static SyncEventApplyContext MutatedResultContext(string mutation)
    {
        var childId = Guid.Parse("20000000-0000-0000-0000-000000000019");
        var payload = Result("Rejected") with
        {
            Canonical = Snapshot() with
            {
                Addresses = [new(childId, "Billing", "Street", null, "EC", null, null, null, null, null, true, true)],
                Contacts = [new(childId, "OWNER", "EMAIL", "Ana", null, null, null, null, null, null, null, true, true, true, null)]
            }
        };
        var context = Context(payload);
        var root = JsonNode.Parse(context.PayloadJson)!.AsObject();
        var wirePayload = root["payload"]!.AsObject();
        var canonical = wirePayload["canonical"]!.AsObject();
        switch (mutation)
        {
            case "extra-root": root["unexpected"] = true; break;
            case "missing-root-code": root.Remove("code"); break;
            case "result-code-string": root["code"] = "BP"; break;
            case "numeric-root-operation": root["operation"] = 1; break;
            case "root-entity-mismatch": root["entityName"] = "BusinessPartner"; break;
            case "root-global-mismatch": root["globalId"] = JsonValue.Create(Guid.NewGuid()); break;
            case "root-operation-mismatch": root["operation"] = "Created"; break;
            case "missing-message": wirePayload.Remove("message"); break;
            case "missing-canonical": wirePayload.Remove("canonical"); break;
            case "missing-is-active": canonical.Remove("isActive"); break;
            case "addresses-not-array": canonical["addresses"] = new JsonObject(); break;
            case "blank-address-line": canonical["addresses"]![0]!["line1"] = " "; break;
            case "missing-address-global-id": canonical["addresses"]![0]!.AsObject().Remove("globalId"); break;
            case "blank-contact-name": canonical["contacts"]![0]!["name"] = ""; break;
            case "missing-contact-global-id": canonical["contacts"]![0]!.AsObject().Remove("globalId"); break;
            case "missing-contact-code": canonical["contacts"]![0]!.AsObject().Remove("contactTypeCode"); break;
            case "string-contact-bool": canonical["contacts"]![0]!["receivesNotifications"] = "true"; break;
            case "numeric-address-code": canonical["addresses"]![0]!["countryCode"] = 7; break;
            default: throw new ArgumentOutOfRangeException(nameof(mutation), mutation, null);
        }

        return context with { PayloadJson = root.ToJsonString() };
    }

    private static BusinessPartnerCanonicalSnapshot Snapshot() =>
        new(PartnerId, "BP-10000000000000000000000000000019", "Central", null, "Customer", "RUC",
            "09.999-999 99001", "0999999999001", null, null, "CN0999999999001", true, [], []);

    private static CompanyConnectionInfo Central(int id, bool enabled = true) =>
        new(id, $"C{id}", "Central", DatabaseEngine.SqlServer, "Server=central;", SapIntegrationMode.None,
            CompanyOperationMode.Standalone, true, null, null, enabled);

    private static CompanyConnectionInfo Branch(int id, int parentId, bool enabled = true) =>
        new(id, $"B{id}", "Branch", DatabaseEngine.SqlServer, "Server=branch;", SapIntegrationMode.None,
            CompanyOperationMode.Standalone, false, parentId, $"B{id}", enabled);
}
