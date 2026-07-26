using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class MasterBranchSyncWorkerSkeletonTests
{
    [Fact]
    public async Task Processor_DoesNotClaim_WhenWorkerIsDisabled()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions { Enabled = false },
            outboxRepository);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(0);
        await outboxRepository.DidNotReceiveWithAnyArgs().ReleaseExpiredLocksAsync(default);
        await outboxRepository.DidNotReceiveWithAnyArgs().ClaimPendingAsync(default!, default, default, default);
    }

    [Fact]
    public void MasterBranchSyncWorkerOptions_DefaultSkeletonModeBehavior_IsObserveOnly()
    {
        var options = new MasterBranchSyncWorkerOptions();

        options.SkeletonMode.Should().BeTrue();
        options.SkeletonModeBehavior.Should().Be(SkeletonModeBehavior.ObserveOnly);
    }

    [Fact]
    public async Task Processor_DoesNotClaimOrChangeState_WhenSkeletonModeObserveOnly()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var auditRepository = Substitute.For<ISyncAuditRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions
            {
                Enabled = true,
                WorkerInstance = "worker-a",
                SkeletonMode = true,
                SkeletonModeBehavior = SkeletonModeBehavior.ObserveOnly
            },
            outboxRepository,
            auditRepository,
            applier);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(0);
        await outboxRepository.DidNotReceiveWithAnyArgs().ReleaseExpiredLocksAsync(default);
        await outboxRepository.DidNotReceiveWithAnyArgs().ClaimPendingAsync(default!, default, default, default);
        await outboxRepository.DidNotReceiveWithAnyArgs().UpdateStatusAsync(default, default, default, default);
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkIgnoredAsync(default, default, default);
        await auditRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        await applier.DidNotReceiveWithAnyArgs().ApplyAsync(default!, default);
    }

    [Fact]
    public async Task Processor_ClaimsWithLockedByAndLockExpiresAt_WhenEnabled()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<SyncOutboxDto>());
        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions
            {
                Enabled = true,
                WorkerInstance = "worker-a",
                BatchSize = 25,
                LockMinutes = 7,
                SkeletonMode = false
            },
            outboxRepository);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(0);
        await outboxRepository.Received(1).ReleaseExpiredLocksAsync(Arg.Any<CancellationToken>());
        await outboxRepository.Received(1).ClaimPendingAsync(
            "worker-a",
            25,
            TimeSpan.FromMinutes(7),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task NoOpSyncEventApplier_DoesNotApplyBusinessEntities_WhenSkeletonModeIsEnabled()
    {
        var applier = new NoOpSyncEventApplier(Options.Create(new MasterBranchSyncWorkerOptions
        {
            SkeletonMode = true
        }));

        var result = await applier.ApplyAsync(CreateApplyContext(), CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Message.Should().Contain("SkeletonMode");
        result.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void WorkerProject_DoesNotReferenceSapOrCrudBusinessRepositories()
    {
        var source = ReadWorkerProjectSource();

        source.Should().NotContain("Sap", "Master/Branch worker must remain separate from SAP integration.");
        source.Should().NotContain("BusinessPartnerRepository");
        source.Should().NotContain("IBusinessPartnerRepository");
        source.Should().NotContain("IItemRepository");
        source.Should().NotContain("ItemRepository");
        source.Should().NotContain("IWarehouseRepository");
        source.Should().NotContain("WarehouseRepository");
        source.Should().NotContain("IPriceListRepository");
        source.Should().NotContain("PriceListRepository");
        source.Should().Contain("BusinessPartnerSyncEventApplier");
        source.Should().Contain("ItemSyncEventApplier");
        source.Should().Contain("WarehouseSyncEventApplier");
        source.Should().Contain("ReferenceCatalogSyncEventApplier");
        source.Should().Contain("PurchaseOrderSyncEventApplier");
    }

    [Fact]
    public void SyncOutboxRepository_ClaimUsesTechnicalLockAndRetryGuards()
    {
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncOutboxRepository.cs");

        repository.Should().Contain("ClaimPendingAsync");
        repository.Should().Contain("READPAST");
        repository.Should().Contain("UPDLOCK");
        repository.Should().Contain("LockedBy = @LockedBy");
        repository.Should().Contain("LockExpiresAt = DATEADD(minute, @LockMinutes, SYSUTCDATETIME())");
        repository.Should().Contain("AttemptCount < MaxAttempts");
        repository.Should().Contain("NextRetryAt IS NULL OR NextRetryAt <= SYSUTCDATETIME()");
        repository.Should().Contain("Status IN (N'Pending', N'Error')");
        repository.Should().NotContain("Status IN (N'Pending', N'Error', N'DeadLetter')");
    }

    [Fact]
    public void SyncOutboxRepository_MarkDeadLetterClearsTechnicalLockAndClosesEvent()
    {
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncOutboxRepository.cs");

        repository.Should().Contain("MarkDeadLetterAsync");
        repository.Should().Contain("Status = N'DeadLetter'");
        repository.Should().Contain("NextRetryAt = NULL");
        repository.Should().Contain("ProcessedAt = SYSUTCDATETIME()");
        repository.Should().Contain("LockedBy = NULL");
        repository.Should().Contain("LockedAt = NULL");
        repository.Should().Contain("LockExpiresAt = NULL");
    }

    [Fact]
    public async Task Processor_MarksError_WhenAttemptsRemain()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var auditRepository = Substitute.For<ISyncAuditRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var syncEvent = CreateOutboxEvent(attemptCount: 1, maxAttempts: 3);

        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>())
            .Returns(new[] { syncEvent });
        outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, Arg.Any<CancellationToken>())
            .Returns(new[] { CreateTarget(syncEvent.Id) });
        outboxRepository.TryMarkTargetInProcessAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(true);
        applier.ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>())
            .Returns<Task<SyncEventApplyResult>>(_ => throw new InvalidOperationException("transient failure"));

        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions { Enabled = true, WorkerInstance = "worker-a", SkeletonMode = false },
            outboxRepository,
            auditRepository,
            applier);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(1);
        await outboxRepository.Received(1).MarkTargetErrorAsync(
            20,
            "transient failure",
            Arg.Any<TimeSpan>(),
            Arg.Any<CancellationToken>());
        await outboxRepository.Received(1).MarkErrorAsync(
            syncEvent.Id,
            "Uno o mas targets quedaron pendientes o con error reprocesable.",
            Arg.Any<TimeSpan>(),
            Arg.Any<CancellationToken>());
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkDeadLetterAsync(default, default!, default);
        await auditRepository.Received(1).AddAsync(
            Arg.Is<CreateSyncAuditData>(data =>
                data.EventId == syncEvent.EventId &&
                data.BranchCompanyId == 2 &&
                data.Action == SyncAuditAction.Failed &&
                data.NewStatus == SyncEventStatus.Error),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Processor_KeepsMissingDependencyRetryableInsteadOfIgnoringTarget()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var auditRepository = Substitute.For<ISyncAuditRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var syncEvent = CreateOutboxEvent(attemptCount: 1, maxAttempts: 3);
        var target = CreateTarget(syncEvent.Id);

        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(new[] { syncEvent });
        outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, Arg.Any<CancellationToken>())
            .Returns(new[] { target });
        outboxRepository.TryMarkTargetInProcessAsync(target.Id, Arg.Any<CancellationToken>()).Returns(true);
        applier.ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncEventApplyResult(
                false,
                "El grupo INV-PAP aun no existe en la sucursal.",
                "SYNC_DEPENDENCY_PENDING",
                Retryable: true));

        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions { Enabled = true, WorkerInstance = "worker-a", SkeletonMode = false },
            outboxRepository,
            auditRepository,
            applier);

        await processor.ProcessOnceAsync(CancellationToken.None);

        await outboxRepository.Received(1).MarkTargetErrorAsync(
            target.Id,
            "El grupo INV-PAP aun no existe en la sucursal.",
            Arg.Any<TimeSpan>(),
            Arg.Any<CancellationToken>());
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkTargetIgnoredAsync(default, default, default);
        await auditRepository.Received(1).AddAsync(
            Arg.Is<CreateSyncAuditData>(data =>
                data.BranchCompanyId == target.BranchCompanyId &&
                data.Action == SyncAuditAction.Failed &&
                data.ErrorCode == "SYNC_DEPENDENCY_PENDING"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Processor_MarksTerminalApplyConflictAsDeadLetterWithoutRetry()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var auditRepository = Substitute.For<ISyncAuditRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var syncEvent = CreateOutboxEvent(attemptCount: 1, maxAttempts: 3);
        var target = CreateTarget(syncEvent.Id);

        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(new[] { syncEvent });
        outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, Arg.Any<CancellationToken>())
            .Returns(new[] { target });
        outboxRepository.TryMarkTargetInProcessAsync(target.Id, Arg.Any<CancellationToken>()).Returns(true);
        applier.ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncEventApplyResult(
                false,
                "El codigo ya pertenece a otro GlobalId.",
                "SYNC_ITEM_FAMILY_CODE_CONFLICT",
                Terminal: true));

        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions { Enabled = true, WorkerInstance = "worker-a", SkeletonMode = false },
            outboxRepository,
            auditRepository,
            applier);

        await processor.ProcessOnceAsync(CancellationToken.None);

        await outboxRepository.Received(1).MarkTargetDeadLetterAsync(
            target.Id,
            "El codigo ya pertenece a otro GlobalId.",
            Arg.Any<CancellationToken>());
        await outboxRepository.DidNotReceiveWithAnyArgs()
            .MarkTargetErrorAsync(default, default!, default, default);
        await outboxRepository.DidNotReceiveWithAnyArgs()
            .MarkTargetIgnoredAsync(default, default!, default);
        await auditRepository.Received(1).AddAsync(
            Arg.Is<CreateSyncAuditData>(data =>
                data.BranchCompanyId == target.BranchCompanyId &&
                data.Action == SyncAuditAction.DeadLetter &&
                data.ErrorCode == "SYNC_ITEM_FAMILY_CODE_CONFLICT"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Processor_MarksDeadLetter_WhenMaxAttemptsReached()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var auditRepository = Substitute.For<ISyncAuditRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var syncEvent = CreateOutboxEvent(attemptCount: 3, maxAttempts: 3);
        var target = CreateTarget(syncEvent.Id, attemptCount: 2, maxAttempts: 3);

        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>())
            .Returns(new[] { syncEvent });
        outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, Arg.Any<CancellationToken>())
            .Returns(new[] { target });
        outboxRepository.TryMarkTargetInProcessAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(true);
        applier.ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>())
            .Returns<Task<SyncEventApplyResult>>(_ => throw new InvalidOperationException("permanent failure"));

        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions { Enabled = true, WorkerInstance = "worker-a", SkeletonMode = false },
            outboxRepository,
            auditRepository,
            applier);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(1);
        await outboxRepository.Received(1).MarkTargetDeadLetterAsync(
            target.Id,
            "permanent failure",
            Arg.Any<CancellationToken>());
        await outboxRepository.Received(1).MarkDeadLetterAsync(
            syncEvent.Id,
            "Uno o mas targets quedaron en DeadLetter.",
            Arg.Any<CancellationToken>());
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkErrorAsync(default, default!, default, default);
        await auditRepository.Received(1).AddAsync(
            Arg.Is<CreateSyncAuditData>(data =>
                data.EventId == syncEvent.EventId &&
                data.BranchCompanyId == target.BranchCompanyId &&
                data.Action == SyncAuditAction.DeadLetter &&
                data.NewStatus == SyncEventStatus.DeadLetter),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Processor_ClaimsAndReleases_WhenSkeletonModeClaimAndRelease()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var auditRepository = Substitute.For<ISyncAuditRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var syncEvent = CreateOutboxEvent(attemptCount: 1, maxAttempts: 3);

        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>())
            .Returns(new[] { syncEvent });
        outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, Arg.Any<CancellationToken>())
            .Returns(new[] { CreateTarget(syncEvent.Id) });

        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions
            {
                Enabled = true,
                WorkerInstance = "worker-a",
                SkeletonMode = true,
                SkeletonModeBehavior = SkeletonModeBehavior.ClaimAndRelease
            },
            outboxRepository,
            auditRepository,
            applier);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(1);
        await outboxRepository.Received(1).ReleaseExpiredLocksAsync(Arg.Any<CancellationToken>());
        await outboxRepository.Received(1).ClaimPendingAsync(
            "worker-a",
            Arg.Any<int>(),
            Arg.Any<TimeSpan>(),
            Arg.Any<CancellationToken>());
        await outboxRepository.Received(1).UpdateStatusAsync(
            syncEvent.Id,
            SyncEventStatus.Pending,
            Arg.Is<string>(message => message.Contains("ClaimAndRelease")),
            Arg.Any<CancellationToken>());
        await outboxRepository.DidNotReceiveWithAnyArgs().TryMarkTargetInProcessAsync(default, default);
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkTargetAppliedAsync(default, default);
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkIgnoredAsync(default, default, default);
        await applier.DidNotReceiveWithAnyArgs().ApplyAsync(default!, default);
        await auditRepository.Received(1).AddAsync(
            Arg.Is<CreateSyncAuditData>(data =>
                data.EventId == syncEvent.EventId &&
                data.Action == SyncAuditAction.DryRun &&
                data.NewStatus == SyncEventStatus.Pending),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Processor_ClaimsAndIgnores_WhenSkeletonModeClaimAndIgnore()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var syncEvent = CreateOutboxEvent(attemptCount: 1, maxAttempts: 3);

        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>())
            .Returns(new[] { syncEvent });
        outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, Arg.Any<CancellationToken>())
            .Returns(new[] { CreateTarget(syncEvent.Id) });

        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions
            {
                Enabled = true,
                WorkerInstance = "worker-a",
                SkeletonMode = true,
                SkeletonModeBehavior = SkeletonModeBehavior.ClaimAndIgnore
            },
            outboxRepository,
            Substitute.For<ISyncAuditRepository>(),
            applier);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(1);
        await outboxRepository.DidNotReceiveWithAnyArgs().TryMarkTargetInProcessAsync(default, default);
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkTargetAppliedAsync(default, default);
        await outboxRepository.Received(1).MarkIgnoredAsync(
            syncEvent.Id,
            Arg.Is<string>(message => message.Contains("ClaimAndIgnore")),
            Arg.Any<CancellationToken>());
        await applier.DidNotReceiveWithAnyArgs().ApplyAsync(default!, default);
    }

    [Fact]
    public async Task Processor_MarksOutboxAppliedOnlyAfterAllTargetsAreAppliedOrIgnored()
    {
        var outboxRepository = Substitute.For<ISyncOutboxRepository>();
        var applier = Substitute.For<ISyncEventApplier>();
        var syncEvent = CreateOutboxEvent(attemptCount: 1, maxAttempts: 3);
        var firstTarget = CreateTarget(syncEvent.Id, targetId: 20, branchCompanyId: 2);
        var secondTarget = CreateTarget(syncEvent.Id, targetId: 21, branchCompanyId: 3);

        outboxRepository.ClaimPendingAsync(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<TimeSpan>(),
                Arg.Any<CancellationToken>())
            .Returns(new[] { syncEvent });
        outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, Arg.Any<CancellationToken>())
            .Returns(new[] { firstTarget, secondTarget });
        outboxRepository.TryMarkTargetInProcessAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(true);
        applier.ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>())
            .Returns(
                new SyncEventApplyResult(true, "Aplicado."),
                new SyncEventApplyResult(false, "Ignorado por regla."));

        var processor = CreateProcessor(
            new MasterBranchSyncWorkerOptions { Enabled = true, WorkerInstance = "worker-a", SkeletonMode = false },
            outboxRepository,
            Substitute.For<ISyncAuditRepository>(),
            applier);

        var processed = await processor.ProcessOnceAsync(CancellationToken.None);

        processed.Should().Be(1);
        await outboxRepository.Received(1).MarkTargetAppliedAsync(firstTarget.Id, Arg.Any<CancellationToken>());
        await outboxRepository.Received(1).MarkTargetIgnoredAsync(secondTarget.Id, "Ignorado por regla.", Arg.Any<CancellationToken>());
        await outboxRepository.Received(1).MarkAppliedAsync(syncEvent.Id, Arg.Any<CancellationToken>());
        await outboxRepository.DidNotReceiveWithAnyArgs().MarkErrorAsync(default, default!, default, default);
    }

    private static MasterBranchSyncWorkerProcessor CreateProcessor(
        MasterBranchSyncWorkerOptions options,
        ISyncOutboxRepository outboxRepository)
    {
        return new MasterBranchSyncWorkerProcessor(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(options),
            outboxRepository,
            Substitute.For<ISyncAuditRepository>(),
            Substitute.For<ISyncEventApplier>(),
            Substitute.For<ILocalSyncOutboxRelay>(),
            NullLogger<MasterBranchSyncWorkerProcessor>.Instance);
    }

    private static MasterBranchSyncWorkerProcessor CreateProcessor(
        MasterBranchSyncWorkerOptions options,
        ISyncOutboxRepository outboxRepository,
        ISyncAuditRepository auditRepository,
        ISyncEventApplier eventApplier)
    {
        return new MasterBranchSyncWorkerProcessor(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(options),
            outboxRepository,
            auditRepository,
            eventApplier,
            Substitute.For<ILocalSyncOutboxRelay>(),
            NullLogger<MasterBranchSyncWorkerProcessor>.Instance);
    }

    private static SyncEventApplyContext CreateApplyContext()
    {
        return new SyncEventApplyContext(
            Guid.NewGuid(),
            SourceCompanyId: 1,
            EntityName: "BusinessPartner",
            EntityGlobalId: Guid.NewGuid(),
            Operation: SyncOperation.Created.ToString(),
            PayloadJson: """{"code":"CLI-001"}""");
    }

    private static SyncOutboxDto CreateOutboxEvent(int attemptCount, int maxAttempts)
    {
        return new SyncOutboxDto(
            Id: 10,
            EventId: Guid.NewGuid(),
            CompanyId: 1,
            EntityName: "BusinessPartner",
            EntityGlobalId: Guid.NewGuid(),
            EntityCode: "CLI-001",
            Operation: SyncOperation.Updated,
            PayloadJson: """{"code":"CLI-001"}""",
            SourceSystem: null,
            SourceReference: null,
            Status: SyncEventStatus.InProcess,
            AttemptCount: attemptCount,
            MaxAttempts: maxAttempts,
            NextRetryAt: null,
            LockedBy: "worker-a",
            LockedAt: DateTime.UtcNow,
            LockExpiresAt: DateTime.UtcNow.AddMinutes(5),
            CreatedAt: DateTime.UtcNow.AddMinutes(-1),
            ProcessedAt: null,
            LastErrorMessage: null);
    }

    private static SyncOutboxTargetDto CreateTarget(
        long outboxId,
        long targetId = 20,
        int branchCompanyId = 2,
        int attemptCount = 0,
        int maxAttempts = 3)
    {
        return new SyncOutboxTargetDto(
            Id: targetId,
            OutboxId: outboxId,
            BranchCompanyId: branchCompanyId,
            Status: SyncEventStatus.Pending,
            AttemptCount: attemptCount,
            MaxAttempts: maxAttempts,
            NextRetryAt: null,
            AppliedAt: null,
            LastErrorMessage: null,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: null);
    }

    private static string ReadWorkerProjectSource()
    {
        var directory = FindRepositoryRoot();
        var workerDirectory = Path.Combine(directory.FullName, "src", "Backend", "NuanSystem.MasterBranchSyncWorker");
        var files = Directory.GetFiles(workerDirectory, "*.cs", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(workerDirectory, "*.csproj", SearchOption.TopDirectoryOnly))
            .Concat(Directory.GetFiles(workerDirectory, "appsettings*.json", SearchOption.TopDirectoryOnly));

        return string.Join(Environment.NewLine, files.Select(File.ReadAllText));
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = FindRepositoryRoot();
        var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
        return File.ReadAllText(path);
    }

    private static DirectoryInfo FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            {
                return directory;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }

    private sealed class StaticOptionsMonitor<T>(T currentValue) : IOptionsMonitor<T>
    {
        public T CurrentValue { get; } = currentValue;

        public T Get(string? name) => CurrentValue;

        public IDisposable? OnChange(Action<T, string?> listener) => null;
    }
}
