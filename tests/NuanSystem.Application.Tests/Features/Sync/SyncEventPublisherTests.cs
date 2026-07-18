using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncEventPublisherTests
{
    private readonly IReplicableEntityMetadataProvider _metadataProvider = Substitute.For<IReplicableEntityMetadataProvider>();
    private readonly ISyncEventPayloadFactory _payloadFactory = Substitute.For<ISyncEventPayloadFactory>();
    private readonly ISyncRoutingService _routingService = Substitute.For<ISyncRoutingService>();
    private readonly ISyncOutboxRepository _outboxRepository = Substitute.For<ISyncOutboxRepository>();

    [Fact]
    public async Task PublishAsync_DoesNotPublish_WhenSyncIsDisabledForCompany()
    {
        ConfigureMetadata(syncEnabled: false, isMaster: true, configured: true, enabled: true, SyncDirection.MasterToBranch);
        var publisher = CreatePublisher();

        var result = await publisher.PublishAsync(CreateRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Published.Should().BeFalse();
        await _outboxRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default);
    }

    [Fact]
    public async Task PublishAsync_DoesNotPublish_WhenCompanyIsNotMaster()
    {
        ConfigureMetadata(syncEnabled: true, isMaster: false, configured: true, enabled: true, SyncDirection.MasterToBranch);
        var publisher = CreatePublisher();

        var result = await publisher.PublishAsync(CreateRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Published.Should().BeFalse();
        result.Value.Reason.Should().Contain("no es Master");
        await _outboxRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default);
    }

    [Fact]
    public async Task PublishAsync_DoesNotPublish_WhenEntityIsNotConfigured()
    {
        ConfigureMetadata(syncEnabled: true, isMaster: true, configured: false, enabled: false, direction: null);
        var publisher = CreatePublisher();

        var result = await publisher.PublishAsync(CreateRequest(), CancellationToken.None);

        result.Value!.Published.Should().BeFalse();
        result.Value.Reason.Should().Contain("no tiene configuracion");
        await _outboxRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default);
    }

    [Fact]
    public async Task PublishAsync_DoesNotPublish_WhenEntityConfigurationIsDisabled()
    {
        ConfigureMetadata(syncEnabled: true, isMaster: true, configured: true, enabled: false, SyncDirection.MasterToBranch);
        var publisher = CreatePublisher();

        var result = await publisher.PublishAsync(CreateRequest(), CancellationToken.None);

        result.Value!.Published.Should().BeFalse();
        result.Value.Reason.Should().Contain("deshabilitada");
        await _outboxRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default);
    }

    [Fact]
    public async Task PublishAsync_DoesNotPublish_WhenDirectionDoesNotAllowMasterToBranch()
    {
        ConfigureMetadata(syncEnabled: true, isMaster: true, configured: true, enabled: true, SyncDirection.BranchToMaster);
        var publisher = CreatePublisher();

        var result = await publisher.PublishAsync(CreateRequest(), CancellationToken.None);

        result.Value!.Published.Should().BeFalse();
        result.Value.Reason.Should().Contain("no permite");
        await _outboxRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default);
    }

    [Fact]
    public async Task PublishAsync_Publishes_WhenCompanyIsMasterAndDirectionAllowsMasterToBranch()
    {
        CreateSyncOutboxEventData? captured = null;
        ConfigureMetadata(syncEnabled: true, isMaster: true, configured: true, enabled: true, SyncDirection.MasterToBranch);
        _payloadFactory.CreatePayloadJson(Arg.Any<SyncPublishRequest>()).Returns("""{"globalId":"value"}""");
        _outboxRepository.CreateAsync(Arg.Do<CreateSyncOutboxEventData>(data => captured = data), Arg.Any<CancellationToken>())
            .Returns(77);
        _routingService.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(false, Array.Empty<SyncRoutingTargetDto>(), "Sin perfiles activos."));
        var publisher = CreatePublisher();
        var request = CreateRequest();

        var result = await publisher.PublishAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Published.Should().BeTrue();
        result.Value.OutboxId.Should().Be(77);
        captured.Should().NotBeNull();
        captured!.CompanyId.Should().Be(request.CompanyId);
        captured.EntityGlobalId.Should().Be(request.EntityGlobalId);
        captured.PayloadJson.Should().Contain("globalId");
    }

    [Fact]
    public async Task PublishAsync_CreatesTargets_WhenRoutingReturnsBranches()
    {
        SyncRoutingContext? capturedContext = null;
        ConfigureMetadata(syncEnabled: true, isMaster: true, configured: true, enabled: true, SyncDirection.MasterToBranch);
        _outboxRepository.CreateAsync(Arg.Any<CreateSyncOutboxEventData>(), Arg.Any<CancellationToken>())
            .Returns(77);
        _routingService.ResolveTargetsAsync(Arg.Do<SyncRoutingContext>(context => capturedContext = context), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(true, [Target(2, maxRetries: 3), Target(3, maxRetries: 4)]));
        var publisher = CreatePublisher();

        var result = await publisher.PublishAsync(
            CreateRequest(entityName: "Warehouse", targetBranchCode: "REMIGIO", requireTargetBranchMatch: true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Published.Should().BeTrue();
        result.Value.Reason.Should().Contain("2 target");
        capturedContext.Should().NotBeNull();
        capturedContext!.SourceCompanyId.Should().Be(1);
        capturedContext.EntityCode.Should().Be("Warehouse");
        capturedContext.TargetBranchCode.Should().Be("REMIGIO");
        capturedContext.RequireTargetBranchMatch.Should().BeTrue();
        await _outboxRepository.Received(1).CreateTargetAsync(
            Arg.Is<CreateSyncOutboxTargetData>(target => target.OutboxId == 77 && target.BranchCompanyId == 2 && target.MaxAttempts == 4),
            Arg.Any<CancellationToken>());
        await _outboxRepository.Received(1).CreateTargetAsync(
            Arg.Is<CreateSyncOutboxTargetData>(target => target.OutboxId == 77 && target.BranchCompanyId == 3 && target.MaxAttempts == 5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishAsync_DoesNotDuplicateTargets_WhenRoutingReturnsRepeatedBranches()
    {
        ConfigureMetadata(syncEnabled: true, isMaster: true, configured: true, enabled: true, SyncDirection.MasterToBranch);
        _outboxRepository.CreateAsync(Arg.Any<CreateSyncOutboxEventData>(), Arg.Any<CancellationToken>())
            .Returns(77);
        _routingService.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(true, [Target(2), Target(2)]));
        var publisher = CreatePublisher();

        await publisher.PublishAsync(CreateRequest(entityName: "Warehouse"), CancellationToken.None);

        await _outboxRepository.Received(1).CreateAsync(Arg.Any<CreateSyncOutboxEventData>(), Arg.Any<CancellationToken>());
        await _outboxRepository.Received(1).CreateTargetAsync(Arg.Any<CreateSyncOutboxTargetData>(), Arg.Any<CancellationToken>());
        await _outboxRepository.Received(1).CreateTargetAsync(
            Arg.Is<CreateSyncOutboxTargetData>(target => target.OutboxId == 77 && target.BranchCompanyId == 2),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishAsync_DoesNotCreateTargets_WhenNoDistributionRulesApply()
    {
        ConfigureMetadata(syncEnabled: true, isMaster: true, configured: true, enabled: true, SyncDirection.MasterToBranch);
        _outboxRepository.CreateAsync(Arg.Any<CreateSyncOutboxEventData>(), Arg.Any<CancellationToken>())
            .Returns(77);
        _routingService.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(false, Array.Empty<SyncRoutingTargetDto>(), "Sin perfiles activos."));
        var publisher = CreatePublisher();

        var result = await publisher.PublishAsync(CreateRequest(entityName: "Warehouse"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Published.Should().BeTrue();
        result.Value.Reason.Should().Contain("sin targets");
        await _outboxRepository.DidNotReceiveWithAnyArgs().CreateTargetAsync(default!, default);
    }

    [Fact]
    public void PayloadFactory_IncludesGlobalIdAndCode()
    {
        var factory = new SyncEventPayloadFactory();
        var globalId = Guid.NewGuid();

        var json = factory.CreatePayloadJson(CreateRequest(globalId: globalId, entityCode: "ART-001"));

        json.Should().Contain(globalId.ToString());
        json.Should().Contain("ART-001");
        json.Should().Contain("globalId");
    }

    [Fact]
    public void PayloadFactory_RemovesSensitiveValuesAndDoesNotRequireSap()
    {
        var factory = new SyncEventPayloadFactory();
        var request = CreateRequest(payload: new
        {
            GlobalId = Guid.NewGuid(),
            Code = "CLI-001",
            Password = "secret",
            ApiToken = "token",
            SapCode = (string?)null
        });

        var json = factory.CreatePayloadJson(request);

        json.Should().NotContain("secret");
        json.Should().NotContain("ApiToken");
        json.Should().NotContain("Password");
        json.Should().Contain("globalId");
    }

    private SyncEventPublisher CreatePublisher()
    {
        _payloadFactory.CreatePayloadJson(Arg.Any<SyncPublishRequest>()).Returns("""{"globalId":"value"}""");
        return new SyncEventPublisher(_metadataProvider, _payloadFactory, _routingService, _outboxRepository);
    }

    private static SyncRoutingTargetDto Target(int branchCompanyId, int maxRetries = 3)
    {
        return new SyncRoutingTargetDto(
            10,
            20,
            "PROFILE",
            1,
            branchCompanyId,
            "Warehouse",
            500,
            maxRetries,
            30,
            30,
            true,
            true,
            true,
            false);
    }

    private void ConfigureMetadata(
        bool syncEnabled,
        bool isMaster,
        bool configured,
        bool enabled,
        SyncDirection? direction)
    {
        _metadataProvider.GetAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ReplicableEntityMetadata(
                CompanyId: 1,
                IsMaster: isMaster,
                SyncEnabled: syncEnabled,
                EntityName: "Items",
                IsConfigured: configured,
                IsEnabled: enabled,
                Direction: direction));
    }

    private static SyncPublishRequest CreateRequest(
        Guid? globalId = null,
        string? entityCode = "ART-001",
        string entityName = "Items",
        object? payload = null,
        string? targetBranchCode = null,
        bool requireTargetBranchMatch = false)
    {
        var resolvedGlobalId = globalId ?? Guid.NewGuid();
        return new SyncPublishRequest(
            CompanyId: 1,
            EntityName: entityName,
            EntityGlobalId: resolvedGlobalId,
            EntityCode: entityCode,
            Operation: SyncOperation.Updated,
            Payload: payload ?? new { GlobalId = resolvedGlobalId, Code = entityCode, Name = "Articulo" },
            SourceSystem: null,
            SourceReference: null,
            TargetBranchCode: targetBranchCode,
            RequireTargetBranchMatch: requireTargetBranchMatch);
    }
}
