using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
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

public sealed class BusinessPartnerSyncEventApplierTests
{
    private static readonly Guid PartnerId = Guid.Parse("10000000-0000-0000-0000-000000000009");

    [Fact]
    public void CanApply_AcceptsOnlyCanonicalBusinessPartnerEntity()
    {
        var applier = CreateApplier(out _, out _);
        applier.CanApply("BusinessPartner").Should().BeTrue();
        applier.CanApply("businesspartner").Should().BeTrue();
        applier.CanApply("BusinessPartnerProposalResult").Should().BeFalse();
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("null")]
    [InlineData("42")]
    [InlineData("{invalid")]
    public async Task Apply_InvalidEnvelopeIsTerminal(string json)
    {
        var applier = CreateApplier(out var repository, out _);
        var context = Context(Canonical()) with { PayloadJson = json };
        var result = await applier.ApplyAsync(context);
        result.Should().BeEquivalentTo(new SyncEventApplyResult(false,
            "Payload canonico de socio no es JSON valido.", "SYNC_PAYLOAD_INVALID", false, true));
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_LegacyOrUnsupportedSchemaIsTerminal()
    {
        var applier = CreateApplier(out var repository, out _);
        var result = await applier.ApplyAsync(Context(Canonical() with { SchemaVersion = 1 }));
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_LEGACY_PAYLOAD_UNSUPPORTED");
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task Apply_EntityAndPayloadGlobalIdsMustMatch()
    {
        var applier = CreateApplier(out var repository, out _);
        var result = await applier.ApplyAsync(Context(Canonical()) with { EntityGlobalId = Guid.NewGuid() });
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_GLOBAL_ID_MISMATCH");
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Apply_DuplicateChildGlobalIdsAreTerminal(bool addresses)
    {
        var childId = Guid.Parse("20000000-0000-0000-0000-000000000099");
        var snapshot = addresses
            ? Snapshot() with
            {
                Addresses =
                [
                    new(childId, "Billing", "One", null, "EC", null, null, null, null, null, true, true),
                    new(childId, "Shipping", "Two", null, "EC", null, null, null, null, null, false, true)
                ]
            }
            : Snapshot() with
            {
                Contacts =
                [
                    new(childId, "OWNER", "EMAIL", "One", null, null, null, null, null, null, null, true, true, true, null),
                    new(childId, "BUYER", "MOBILE", "Two", null, null, null, null, null, null, null, false, false, true, null)
                ]
            };
        var applier = CreateApplier(out var repository, out _);

        var result = await applier.ApplyAsync(Context(Canonical() with { Partner = snapshot }));

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData("extra-root")]
    [InlineData("missing-root-code")]
    [InlineData("numeric-root-operation")]
    [InlineData("root-entity-mismatch")]
    [InlineData("root-global-mismatch")]
    [InlineData("root-code-mismatch")]
    [InlineData("root-operation-mismatch")]
    [InlineData("missing-origin")]
    [InlineData("missing-is-active")]
    [InlineData("addresses-not-array")]
    [InlineData("blank-address-type")]
    [InlineData("missing-address-line")]
    [InlineData("missing-address-global-id")]
    [InlineData("blank-contact-name")]
    [InlineData("missing-contact-global-id")]
    [InlineData("missing-contact-code")]
    [InlineData("string-contact-bool")]
    [InlineData("numeric-contact-code")]
    public async Task Apply_ValidJsonWithInvalidCanonicalWireShapeIsTerminal(string mutation)
    {
        var applier = CreateApplier(out var repository, out _);
        repository.ApplyCanonicalAsync(default, default!, default!, default)
            .ReturnsForAnyArgs(new BusinessPartnerSyncApplyResult(true, false, 80, "Applied."));

        var result = await applier.ApplyAsync(MutatedCanonicalContext(mutation));

        result.Should().BeEquivalentTo(new SyncEventApplyResult(
            false, "Payload canonico de socio no es JSON valido.", "SYNC_PAYLOAD_INVALID", false, true));
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("4")]
    [InlineData("-1")]
    [InlineData(" 1 ")]
    public async Task Apply_NumericOrUndefinedOperationIsTerminal(string operation)
    {
        var applier = CreateApplier(out var repository, out _);
        repository.ApplyCanonicalAsync(default, default!, default!, default)
            .ReturnsForAnyArgs(new BusinessPartnerSyncApplyResult(true, false, 80, "Applied."));

        var result = await applier.ApplyAsync(Context(Canonical()) with { Operation = operation });

        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_OPERATION_UNSUPPORTED");
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData(21)]
    [InlineData(22)]
    public async Task Apply_CentralCanonicalFansOutToOriginAndSiblingBranch(int targetCompanyId)
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(targetCompanyId, Arg.Any<CancellationToken>())
            .Returns(Branch(targetCompanyId, 10));
        var context = Context(Canonical(), targetCompanyId);
        repository.ApplyCanonicalAsync(targetCompanyId, context, Arg.Any<BusinessPartnerCanonicalPayloadV2>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncApplyResult(true, false, 80, "Aplicado."));
        var result = await applier.ApplyAsync(context);
        result.Applied.Should().BeTrue();
        await repository.Received(1).ApplyCanonicalAsync(targetCompanyId, context,
            Arg.Is<BusinessPartnerCanonicalPayloadV2>(value => value.SchemaVersion == 2 &&
                value.CanonicalVersion == 7 && value.Partner.GlobalId == PartnerId),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false, true, true, "BP_SYNC_SOURCE_CENTRAL_REQUIRED")]
    [InlineData(true, false, true, "BP_SYNC_TARGET_BRANCH_REQUIRED")]
    [InlineData(true, true, false, "BP_SYNC_PARENT_MISMATCH")]
    public async Task Apply_RequiresCentralToDirectChildTopology(
        bool sourceIsCentral, bool targetIsBranch, bool parentMatches, string expectedCode)
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>())
            .Returns(sourceIsCentral ? Central(10) : Branch(10, 99));
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>())
            .Returns(targetIsBranch ? Branch(21, parentMatches ? 10 : 99) : Central(21));
        var result = await applier.ApplyAsync(Context(Canonical()));
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be(expectedCode);
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task Apply_RequiresSyncEnabledAtBothEnds(bool sourceEnabled, bool targetEnabled)
    {
        var applier = CreateApplier(out var repository, out var companies);
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>()).Returns(Central(10, sourceEnabled));
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>()).Returns(Branch(21, 10, targetEnabled));
        var result = await applier.ApplyAsync(Context(Canonical()));
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("BP_SYNC_DISABLED");
        await repository.DidNotReceiveWithAnyArgs().ApplyCanonicalAsync(default, default!, default!, default);
    }

    [Theory]
    [InlineData(1, true, false, false)]
    [InlineData(2, true, true, false)]
    [InlineData(3, true, true, true)]
    [InlineData(4, false, false, false)]
    [InlineData(5, false, false, false)]
    public void CanonicalProcedureResult_ClosesApplyEqualStaleAndTerminalOutcomes(
        int resultCode, bool applied, bool alreadyApplied, bool ignored)
    {
        var result = BusinessPartnerSyncApplyRepository.MapCanonicalResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow
            {
                ResultCode = resultCode,
                BusinessPartnerId = 80
            });
        result.Applied.Should().Be(applied);
        result.AlreadyApplied.Should().Be(alreadyApplied);
        result.Ignored.Should().Be(ignored);
        result.Terminal.Should().Be(resultCode is 4 or 5);
    }

    [Fact]
    public void CanonicalParameters_KeepSapCodeReadOnlyAndReplaceChildrenByGlobalId()
    {
        var addressId = Guid.Parse("20000000-0000-0000-0000-000000000009");
        var contactId = Guid.Parse("30000000-0000-0000-0000-000000000009");
        var payload = Canonical() with { Partner = Snapshot() with
        {
            SapCardCode = "CN0999999999001",
            Addresses = [new(addressId, "Billing", "Street", null, "EC", "P01", "C01", null, null, null, true, true)],
            Contacts = [new(contactId, "SALE", "EMAIL", "Ana", null, null, null, null, null, "a@b.ec", null, true, true, true, null)]
        }};
        var references = new BusinessPartnerSyncApplyRepository.StableReferenceResolution(true, 7,
            JsonSerializer.Serialize(new[] { new { globalId = addressId, countryId = 1, provinceId = 2, cityId = 3 } }),
            JsonSerializer.Serialize(new[] { new { globalId = contactId, contactTypeId = 4, contactChannelId = 5 } }));
        var parameters = BusinessPartnerSyncApplyRepository.CreateCanonicalParameters(Context(payload), payload, references);
        parameters.SapCardCode.Should().Be("CN0999999999001");
        parameters.AddressesJson.Should().Contain(addressId.ToString()).And.Contain("countryId");
        parameters.ContactsJson.Should().Contain(contactId.ToString()).And.Contain("contactTypeId");
        typeof(BusinessPartnerSyncApplyRepository).GetConstructors().Single().GetParameters()
            .Should().ContainSingle(parameter => parameter.ParameterType == typeof(ICompanyResolver));
    }

    [Fact]
    public void CanonicalPreflight_UsesExactEnvelopeAndClosesOnlyEarlyOutcomes()
    {
        var payload = Canonical();
        var context = Context(payload);

        var parameters = BusinessPartnerSyncApplyRepository.CreateCanonicalPreflightParameters(context, payload);

        parameters.Should().Be(new BusinessPartnerSyncApplyRepository.PreflightParameters(
            context.EventId,
            context.SourceCompanyId,
            "BusinessPartner",
            context.EntityGlobalId,
            "Updated",
            context.PayloadJson,
            7,
            CompareCanonicalVersion: true,
            EqualVersionIsReplay: true));
        BusinessPartnerSyncApplyRepository.MapCanonicalPreflightResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow { ResultCode = 0 }).Should().BeNull();
        BusinessPartnerSyncApplyRepository.MapCanonicalPreflightResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow { ResultCode = 2 }).Should().Match<BusinessPartnerSyncApplyResult>(
                result => result.Applied && result.AlreadyApplied);
        BusinessPartnerSyncApplyRepository.MapCanonicalPreflightResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow { ResultCode = 3 }).Should().Match<BusinessPartnerSyncApplyResult>(
                result => result.Applied && result.Ignored);
        BusinessPartnerSyncApplyRepository.MapCanonicalPreflightResult(
            new BusinessPartnerSyncApplyRepository.ApplyResultRow { ResultCode = 4 }).Should().Match<BusinessPartnerSyncApplyResult>(
                result => result.Terminal && result.ErrorCode == "BP_SYNC_EVENT_ID_COLLISION");
    }

    private static BusinessPartnerSyncEventApplier CreateApplier(
        out IBusinessPartnerSyncApplyRepository repository, out ICompanyResolver companies)
    {
        repository = Substitute.For<IBusinessPartnerSyncApplyRepository>();
        companies = Substitute.For<ICompanyResolver>();
        companies.ResolveByIdAsync(10, Arg.Any<CancellationToken>()).Returns(Central(10));
        companies.ResolveByIdAsync(21, Arg.Any<CancellationToken>()).Returns(Branch(21, 10));
        companies.ResolveByIdAsync(22, Arg.Any<CancellationToken>()).Returns(Branch(22, 10));
        return new BusinessPartnerSyncEventApplier(repository, companies);
    }

    private static SyncEventApplyContext Context(BusinessPartnerCanonicalPayloadV2 payload, int target = 21)
    {
        var json = new SyncEventPayloadFactory().CreatePayloadJson(new SyncPublishRequest(10, "BusinessPartner",
            payload.Partner.GlobalId, payload.Partner.Code, SyncOperation.Updated, payload, null, null));
        return new SyncEventApplyContext(Guid.Parse("90000000-0000-0000-0000-000000000009"), 10,
            "BusinessPartner", payload.Partner.GlobalId, "Updated", json, target, 81);
    }

    private static BusinessPartnerCanonicalPayloadV2 Canonical() =>
        new(2, 7, 21, Guid.Parse("80000000-0000-0000-0000-000000000009"), Snapshot());

    private static SyncEventApplyContext MutatedCanonicalContext(string mutation)
    {
        var childId = Guid.Parse("20000000-0000-0000-0000-000000000009");
        var payload = Canonical() with
        {
            Partner = Snapshot() with
            {
                Addresses = [new(childId, "Billing", "Street", null, "EC", null, null, null, null, null, true, true)],
                Contacts = [new(childId, "OWNER", "EMAIL", "Ana", null, null, null, null, null, null, null, true, true, true, null)]
            }
        };
        var context = Context(payload);
        var root = JsonNode.Parse(context.PayloadJson)!.AsObject();
        var wirePayload = root["payload"]!.AsObject();
        var partner = wirePayload["partner"]!.AsObject();
        switch (mutation)
        {
            case "extra-root": root["unexpected"] = true; break;
            case "missing-root-code": root.Remove("code"); break;
            case "numeric-root-operation": root["operation"] = 1; break;
            case "root-entity-mismatch": root["entityName"] = "BusinessPartnerProposalResult"; break;
            case "root-global-mismatch": root["globalId"] = JsonValue.Create(Guid.NewGuid()); break;
            case "root-code-mismatch": root["code"] = "BP-OTHER"; break;
            case "root-operation-mismatch": root["operation"] = "Created"; break;
            case "missing-origin": wirePayload.Remove("originCompanyId"); break;
            case "missing-is-active": partner.Remove("isActive"); break;
            case "addresses-not-array": partner["addresses"] = new JsonObject(); break;
            case "blank-address-type": partner["addresses"]![0]!["addressType"] = " "; break;
            case "missing-address-line": partner["addresses"]![0]!.AsObject().Remove("line1"); break;
            case "missing-address-global-id": partner["addresses"]![0]!.AsObject().Remove("globalId"); break;
            case "blank-contact-name": partner["contacts"]![0]!["name"] = ""; break;
            case "missing-contact-global-id": partner["contacts"]![0]!.AsObject().Remove("globalId"); break;
            case "missing-contact-code": partner["contacts"]![0]!.AsObject().Remove("contactTypeCode"); break;
            case "string-contact-bool": partner["contacts"]![0]!["isPrimary"] = "true"; break;
            case "numeric-contact-code": partner["contacts"]![0]!["contactTypeCode"] = 7; break;
            default: throw new ArgumentOutOfRangeException(nameof(mutation), mutation, null);
        }

        return context with { PayloadJson = root.ToJsonString() };
    }

    private static BusinessPartnerCanonicalSnapshot Snapshot() =>
        new(PartnerId, "BP-10000000000000000000000000000009", "Partner", null, "Customer", "RUC",
            "09.999-999 99001", "0999999999001", null, null, null, true, [], []);

    private static CompanyConnectionInfo Central(int id, bool enabled = true) =>
        new(id, $"C{id}", "Central", DatabaseEngine.SqlServer, "Server=central;", SapIntegrationMode.None,
            CompanyOperationMode.Standalone, true, null, null, enabled);

    private static CompanyConnectionInfo Branch(int id, int parentId, bool enabled = true) =>
        new(id, $"B{id}", "Branch", DatabaseEngine.SqlServer, "Server=branch;", SapIntegrationMode.None,
            CompanyOperationMode.Standalone, false, parentId, $"B{id}", enabled);
}
