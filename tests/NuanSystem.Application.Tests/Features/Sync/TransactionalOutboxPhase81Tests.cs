using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class TransactionalOutboxPhase81Tests
{
    [Fact]
    public void Relay_IsDisabledByDefault()
    {
        var options = new MasterBranchSyncWorkerOptions();

        options.Enabled.Should().BeFalse();
        options.LocalOutboxRelay.Enabled.Should().BeFalse();
        options.LocalOutboxRelay.NormalizedBatchSize.Should().Be(25);
    }

    [Fact]
    public async Task PromotionService_PreservesEventIdentityAndRoutingEvidence()
    {
        var routing = Substitute.For<ISyncRoutingService>();
        var repository = Substitute.For<ISyncOutboxPromotionRepository>();
        var syncEvent = Event();
        var target = new SyncRoutingTargetDto(
            1, 2, "DEFAULT", syncEvent.CompanyId, 20, syncEvent.EntityName,
            100, 4, 15, 5, true, true, true, false);
        var decision = new SyncDistributionDecisionDto(3, 20, "All", true, "Matched", 1);
        routing.ResolveTargetsAsync(Arg.Any<SyncRoutingContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncRoutingEvaluationResult(true, [target], Decisions: [decision]));
        repository.PromoteAsync(Arg.Any<SyncOutboxPromotionData>(), Arg.Any<CancellationToken>())
            .Returns(new SyncOutboxPromotionResult(SyncOutboxPromotionStatus.Created, 99, "Created"));

        var service = new LocalSyncOutboxPromotionService(routing, repository);
        var result = await service.PromoteAsync(syncEvent, "relay-a");

        result.OutboxId.Should().Be(99);
        await repository.Received(1).PromoteAsync(
            Arg.Is<SyncOutboxPromotionData>(data =>
                data.Event.EventId == syncEvent.EventId
                && data.Event.Id == syncEvent.Id
                && data.Targets.Single().BranchCompanyId == 20
                && data.Decisions.Single().SyncProfileEntityBranchId == 3
                && data.WorkerInstance == "relay-a"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void TenantMigration_DefinesExclusiveLeasesAndForwardOnlyState()
    {
        var sql = Read("database", "sql", "124_tenant_local_outbox_relay.sql");

        sql.Should().Contain("LockedBy")
            .And.Contain("LockExpiresAt")
            .And.Contain("WITH (UPDLOCK,READPAST,ROWLOCK)")
            .And.Contain("SP_NA_POST_LOCALOUTBOX_RECLAMAR")
            .And.Contain("SP_NA_POST_LOCALOUTBOX_COMPLETARPROMOCION")
            .And.Contain("SP_NA_POST_LOCALOUTBOX_PROGRAMARREINTENTO")
            .And.Contain("SP_NA_POST_LOCALOUTBOX_COMPLETARCONFLICTO")
            .And.Contain("Version=N'20260725.124'")
            .And.NotContain("DELETE FROM dbo.SchemaHistory")
            .And.NotContain("DROP TABLE");
    }

    [Fact]
    public void MasterMigration_ProtectsSourceIdentityAndIsForwardOnly()
    {
        var sql = Read("database", "sql", "125_master_sync_outbox_promotion.sql");

        sql.Should().Contain("IX_SyncOutbox_SourceReference")
            .And.Contain("Version=N'20260725.125'")
            .And.NotContain("DELETE FROM dbo.MasterSchemaHistory")
            .And.NotContain("DROP TABLE");
    }

    [Fact]
    public void PromotionRepository_UsesOneMasterTransactionAndLocksEventId()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync",
            "SyncOutboxPromotionRepository.cs");

        source.Should().Contain("BeginTransactionAsync")
            .And.Contain("WITH (UPDLOCK,HOLDLOCK)")
            .And.Contain("data.Event.EventId")
            .And.Contain("SyncDistributionDecisionLog")
            .And.Contain("SyncOutboxTargets")
            .And.Contain("CommitAsync")
            .And.Contain("RollbackAsync");
    }

    [Fact]
    public void LocalWriter_RequiresCallersTransaction()
    {
        var contract = typeof(ILocalSyncOutboxRepository)
            .GetMethod(nameof(ILocalSyncOutboxRepository.CreateAsync))!;
        var parameterTypes = contract.GetParameters().Select(parameter => parameter.ParameterType).ToArray();

        parameterTypes.Should().Contain(typeof(System.Data.IDbConnection));
        parameterTypes.Should().Contain(typeof(System.Data.IDbTransaction));
    }

    [Fact]
    public void RelayDiscoversOnlyEnabledMasterCompanies()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync",
            "LocalSyncOutboxRepository.cs");

        source.Should().Contain("IsMaster=1")
            .And.Contain("SyncEnabled=1")
            .And.NotContain("IsMaster=0 AND SyncEnabled=1");
    }

    private static LocalSyncOutboxDto Event() => new(
        7,
        Guid.NewGuid(),
        10,
        "BusinessPartner",
        Guid.NewGuid(),
        "C001",
        SyncOperation.Created,
        """{"code":"C001"}""",
        SyncEventStatus.InProcess,
        1,
        3,
        null,
        "relay-a",
        DateTime.UtcNow,
        DateTime.UtcNow.AddMinutes(5),
        DateTime.UtcNow,
        null,
        null);

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
