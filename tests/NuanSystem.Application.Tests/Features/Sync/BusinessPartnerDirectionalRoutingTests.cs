using System.Data;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class BusinessPartnerDirectionalRoutingTests
{
    [Fact]
    public async Task BranchWriter_CreatesProposalOnlyForItsParent_WithNullBaseOnCreate()
    {
        var repository = Substitute.For<ILocalSyncOutboxRepository>();
        CreateLocalSyncOutboxData? captured = null;
        repository.CreateAsync(
                Arg.Do<CreateLocalSyncOutboxData>(data => captured = data),
                Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns(1L);
        var writer = new BusinessPartnerLocalOutboxWriter(
            Context(Branch(21, 10)), new SyncEventPayloadFactory(), repository);
        var current = Partner(canonicalVersion: 0);

        await writer.EnqueueAsync(
            new BusinessPartnerOutboxWriteRequest(current, null, SyncOperation.Created, 7, "branch-user", null),
            Substitute.For<IDbConnection>(), Substitute.For<IDbTransaction>());

        captured.Should().NotBeNull();
        captured!.CompanyId.Should().Be(21);
        captured.TargetCompanyId.Should().Be(10);
        captured.EntityName.Should().Be("BusinessPartnerProposal");
        captured.MaxAttempts.Should().Be(3);
        using var document = JsonDocument.Parse(captured.PayloadJson);
        var payload = document.RootElement.GetProperty("payload");
        payload.GetProperty("schemaVersion").GetInt32().Should().Be(1);
        payload.GetProperty("base").ValueKind.Should().Be(JsonValueKind.Null);
        payload.GetProperty("baseCanonicalVersion").GetInt64().Should().Be(0);
        payload.GetProperty("originUserId").GetInt32().Should().Be(7);
    }

    [Fact]
    public async Task BranchWriter_UpdateCarriesBaseAndDeterministicChangedFields()
    {
        var repository = Substitute.For<ILocalSyncOutboxRepository>();
        CreateLocalSyncOutboxData? captured = null;
        repository.CreateAsync(
                Arg.Do<CreateLocalSyncOutboxData>(data => captured = data),
                Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns(1L);
        var writer = new BusinessPartnerLocalOutboxWriter(
            Context(Branch(21, 10)), new SyncEventPayloadFactory(), repository);
        var before = Partner(name: "Before", canonicalVersion: 4);
        var after = Partner(name: "After", canonicalVersion: 4, globalId: before.GlobalId);
        var addressId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        before.Addresses = [Address(addressId, "Old street")];
        after.Addresses = [Address(addressId, "New street")];
        var causation = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        await writer.EnqueueAsync(
            new BusinessPartnerOutboxWriteRequest(after, before, SyncOperation.Updated, null, null, causation),
            Substitute.For<IDbConnection>(), Substitute.For<IDbTransaction>());

        captured!.CausationEventId.Should().Be(causation);
        using var document = JsonDocument.Parse(captured.PayloadJson);
        var payload = document.RootElement.GetProperty("payload");
        payload.GetProperty("base").GetProperty("name").GetString().Should().Be("Before");
        payload.GetProperty("proposed").GetProperty("name").GetString().Should().Be("After");
        payload.GetProperty("baseCanonicalVersion").GetInt64().Should().Be(4);
        payload.GetProperty("changedFields").EnumerateArray().Select(item => item.GetString())
            .Should().Equal($"Addresses/{addressId:N}/Line1", "Name");
    }

    [Fact]
    public async Task CentralWriter_CreatesCanonicalV2WithNullTargetAndPreservedCausation()
    {
        var repository = Substitute.For<ILocalSyncOutboxRepository>();
        CreateLocalSyncOutboxData? captured = null;
        repository.CreateAsync(
                Arg.Do<CreateLocalSyncOutboxData>(data => captured = data),
                Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns(1L);
        var writer = new BusinessPartnerLocalOutboxWriter(
            Context(Central(10)), new SyncEventPayloadFactory(), repository);
        var current = Partner(canonicalVersion: 7);
        var causation = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        await writer.EnqueueAsync(
            new BusinessPartnerOutboxWriteRequest(current, null, SyncOperation.Updated, 5, "central-user", causation),
            Substitute.For<IDbConnection>(), Substitute.For<IDbTransaction>());

        captured!.EntityName.Should().Be("BusinessPartner");
        captured.TargetCompanyId.Should().BeNull();
        captured.CausationEventId.Should().Be(causation);
        using var document = JsonDocument.Parse(captured.PayloadJson);
        var payload = document.RootElement.GetProperty("payload");
        payload.GetProperty("schemaVersion").GetInt32().Should().Be(2);
        payload.GetProperty("canonicalVersion").GetInt64().Should().Be(7);
        payload.GetProperty("causationEventId").GetGuid().Should().Be(causation);
    }

    [Fact]
    public async Task BranchWriter_LegacyReview_DoesNotPublish()
    {
        var repository = Substitute.For<ILocalSyncOutboxRepository>();
        var writer = new BusinessPartnerLocalOutboxWriter(
            Context(Branch(21, 10)), new SyncEventPayloadFactory(), repository);
        var current = Partner();
        current.MasterSyncStatus = "LegacyReview";

        var eventId = await writer.EnqueueAsync(
            new BusinessPartnerOutboxWriteRequest(current, null, SyncOperation.Updated, null, null, null),
            Substitute.For<IDbConnection>(), Substitute.For<IDbTransaction>());

        eventId.Should().BeNull();
        await repository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Promotion_NoActiveRoute_IsDeferredWithoutCreatingMasterOutbox()
    {
        var routing = Substitute.For<ISyncRoutingService>();
        var repository = Substitute.For<ISyncOutboxPromotionRepository>();
        routing.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(false, [], "No active route"));
        var service = new LocalSyncOutboxPromotionService(routing, repository);

        var result = await service.PromoteAsync(LocalEvent(targetCompanyId: 10), "relay-1");

        result.Status.Should().Be(SyncOutboxPromotionStatus.Deferred);
        result.Reason.Should().Be("No active route");
        await repository.DidNotReceiveWithAnyArgs().PromoteAsync(default!, default);
        await routing.Received(1).ResolveTargetsAsync(
            Arg.Is<SyncRoutingContext>(context => context.SourceCompanyId == 21 && context.TargetCompanyId == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Promotion_NonDirectedEventWithoutTargets_UsesEstablishedRepositoryPath()
    {
        var routing = Substitute.For<ISyncRoutingService>();
        var repository = Substitute.For<ISyncOutboxPromotionRepository>();
        routing.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(false, [], "No matching distribution"));
        SyncOutboxPromotionData? captured = null;
        repository.PromoteAsync(
                Arg.Do<SyncOutboxPromotionData>(data => captured = data),
                Arg.Any<CancellationToken>())
            .Returns(new SyncOutboxPromotionResult(SyncOutboxPromotionStatus.Created, 88, "established path"));
        var service = new LocalSyncOutboxPromotionService(routing, repository);

        var result = await service.PromoteAsync(
            LocalEvent(targetCompanyId: null, entityName: "Warehouse"),
            "relay-1");

        result.Status.Should().Be(SyncOutboxPromotionStatus.Created);
        result.OutboxId.Should().Be(88);
        captured.Should().NotBeNull();
        captured!.Event.EntityName.Should().Be("Warehouse");
        captured.Targets.Should().BeEmpty();
        captured.Decisions.Should().BeEmpty();
    }

    [Fact]
    public async Task Promotion_ActiveDirectedRoute_PreservesEventAndCausationIdentity()
    {
        var routing = Substitute.For<ISyncRoutingService>();
        var repository = Substitute.For<ISyncOutboxPromotionRepository>();
        var target = Target(10);
        routing.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(true, [target]));
        SyncOutboxPromotionData? captured = null;
        repository.PromoteAsync(
                Arg.Do<SyncOutboxPromotionData>(data => captured = data),
                Arg.Any<CancellationToken>())
            .Returns(new SyncOutboxPromotionResult(SyncOutboxPromotionStatus.Created, 99, "created"));
        var local = LocalEvent(targetCompanyId: 10);
        local.CausationEventId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var service = new LocalSyncOutboxPromotionService(routing, repository);

        var result = await service.PromoteAsync(local, "relay-1");

        result.Status.Should().Be(SyncOutboxPromotionStatus.Created);
        captured!.Event.Should().BeSameAs(local);
        captured.Event.EventId.Should().Be(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        captured.Event.CausationEventId.Should().Be(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"));
        captured.Targets.Should().ContainSingle(item => item.BranchCompanyId == 10);
    }

    [Theory]
    [InlineData("BusinessPartnerProposal")]
    [InlineData("BusinessPartnerProposalResult")]
    public async Task Promotion_DirectionalEventWithoutExplicitTarget_FailsClosed(string entityName)
    {
        var routing = Substitute.For<ISyncRoutingService>();
        var repository = Substitute.For<ISyncOutboxPromotionRepository>();
        var service = new LocalSyncOutboxPromotionService(routing, repository);

        var result = await service.PromoteAsync(
            LocalEvent(targetCompanyId: null, entityName),
            "relay-1");

        result.Status.Should().Be(SyncOutboxPromotionStatus.Deferred);
        await routing.DidNotReceiveWithAnyArgs().ResolveTargetsAsync(default!, default);
        await repository.DidNotReceiveWithAnyArgs().PromoteAsync(default!, default);
    }

    [Fact]
    public async Task Promotion_RouteToSiblingInsteadOfExplicitParent_FailsClosed()
    {
        var routing = Substitute.For<ISyncRoutingService>();
        var repository = Substitute.For<ISyncOutboxPromotionRepository>();
        routing.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(true, [Target(22)]));
        var service = new LocalSyncOutboxPromotionService(routing, repository);

        var result = await service.PromoteAsync(LocalEvent(targetCompanyId: 10), "relay-1");

        result.Status.Should().Be(SyncOutboxPromotionStatus.Deferred);
        result.Reason.Should().Contain("no coincide");
        await repository.DidNotReceiveWithAnyArgs().PromoteAsync(default!, default);
    }

    private static ICompanyContext Context(CompanyConnectionInfo company)
    {
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(true);
        context.CurrentCompany.Returns(company);
        return context;
    }

    private static CompanyConnectionInfo Branch(int companyId, int? parentCompanyId) => new(
        companyId, $"B{companyId}", "Branch", DatabaseEngine.SqlServer, "tenant",
        SapIntegrationMode.None, CompanyOperationMode.Standalone, false,
        ParentCompanyId: parentCompanyId, BranchCode: $"B{companyId}", SyncEnabled: true);

    private static CompanyConnectionInfo Central(int companyId) => new(
        companyId, $"M{companyId}", "Central", DatabaseEngine.SqlServer, "tenant",
        SapIntegrationMode.None, CompanyOperationMode.Standalone, true,
        SyncEnabled: true);

    private static BusinessPartnerDto Partner(
        string name = "Partner",
        long canonicalVersion = 0,
        Guid? globalId = null) => new()
    {
        Id = 5,
        GlobalId = globalId ?? Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Code = "BP-11111111111111111111111111111111",
        Name = name,
        PartnerType = "Customer",
        IdentificationTypeCode = "RUC",
        IdentificationNumber = "0999999999001",
        NormalizedIdentificationNumber = "0999999999001",
        CanonicalVersion = canonicalVersion,
        MasterSyncStatus = "Accepted",
        IsActive = true
    };

    private static BusinessPartnerAddressDto Address(Guid globalId, string line1) => new()
    {
        GlobalId = globalId,
        AddressType = "Main",
        Line1 = line1,
        IsPrimary = true,
        IsActive = true
    };

    private static LocalSyncOutboxDto LocalEvent(
        int? targetCompanyId,
        string entityName = "BusinessPartnerProposal") => new()
    {
        Id = 8,
        EventId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        CompanyId = 21,
        TargetCompanyId = targetCompanyId,
        EntityName = entityName,
        EntityGlobalId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        EntityCode = "BP-11111111111111111111111111111111",
        Operation = SyncOperation.Created,
        PayloadJson = "{}",
        MaxAttempts = 3
    };

    private static SyncRoutingTargetDto Target(int companyId) => new(
        SyncProfileId: 1,
        SyncProfileEntityId: 2,
        SyncProfileCode: "BP-PROP",
        SourceCompanyId: 21,
        BranchCompanyId: companyId,
        EntityCode: "BusinessPartnerProposal",
        BatchSize: 100,
        MaxRetries: 3,
        RetryDelaySeconds: 30,
        TimeoutMinutes: 5,
        AllowInsert: true,
        AllowUpdate: true,
        AllowDeactivate: false,
        ContinueOnError: false);
}
