using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Features.Sync.Commands;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Configuration.Services;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Services;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncManualActionTests
{
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();
    private readonly ISyncOutboxRepository _repository = Substitute.For<ISyncOutboxRepository>();

    public SyncManualActionTests()
    {
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            CompanyId: 1,
            CompanyCode: "MASTER",
            CommercialName: "Master",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Master;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: true));
    }

    [Fact]
    public async Task RetryError_MarksPending_WhenCurrentStatusIsError()
    {
        var current = CreateDetail(SyncEventStatus.Error);
        var expected = CreateActionResult(current, SyncEventStatus.Pending);
        _repository.GetOutboxDetailAsync(1, current.Id, Arg.Any<CancellationToken>()).Returns(current);
        _repository.RetryErrorAsync(1, current.Id, "manual", "admin", Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new RetrySyncOutboxCommandHandler(_companyContext, _repository);

        var result = await handler.Handle(new RetrySyncOutboxCommand(current.Id, "manual", "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PreviousStatus.Should().Be(SyncEventStatus.Error);
        result.Value.NewStatus.Should().Be(SyncEventStatus.Pending);
        await _repository.Received(1).RetryErrorAsync(1, current.Id, "manual", "admin", Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(SyncEventStatus.Applied)]
    [InlineData(SyncEventStatus.Pending)]
    [InlineData(SyncEventStatus.InProcess)]
    [InlineData(SyncEventStatus.Ignored)]
    public async Task RetryError_RejectsStatusesThatAreNotError(SyncEventStatus status)
    {
        var current = CreateDetail(status);
        _repository.GetOutboxDetailAsync(1, current.Id, Arg.Any<CancellationToken>()).Returns(current);
        var handler = new RetrySyncOutboxCommandHandler(_companyContext, _repository);

        var result = await handler.Handle(new RetrySyncOutboxCommand(current.Id, null, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await _repository.DidNotReceiveWithAnyArgs().RetryErrorAsync(default, default, default, default, default);
    }

    [Fact]
    public async Task RetryDeadLetter_RequiresReason()
    {
        var handler = new RetryDeadLetterSyncOutboxCommandHandler(_companyContext, _repository);

        var result = await handler.Handle(new RetryDeadLetterSyncOutboxCommand(100, " ", true, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("motivo");
        await _repository.DidNotReceiveWithAnyArgs().RetryDeadLetterAsync(default, default, default!, default, default, default);
    }

    [Fact]
    public async Task RetryDeadLetter_RejectsAppliedEvent()
    {
        var current = CreateDetail(SyncEventStatus.Applied);
        var originalPayload = current.PayloadJson;
        var originalGlobalId = current.EntityGlobalId;
        _repository.GetOutboxDetailAsync(1, current.Id, Arg.Any<CancellationToken>()).Returns(current);
        var handler = new RetryDeadLetterSyncOutboxCommandHandler(_companyContext, _repository);

        var result = await handler.Handle(new RetryDeadLetterSyncOutboxCommand(current.Id, "revisado", true, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Message.Should().Contain("DeadLetter");
        current.PayloadJson.Should().Be(originalPayload);
        current.EntityGlobalId.Should().Be(originalGlobalId);
        await _repository.DidNotReceiveWithAnyArgs().RetryDeadLetterAsync(default, default, default!, default, default, default);
    }

    [Fact]
    public async Task RetryDeadLetter_MarksPendingAndCanResetAttempts()
    {
        var current = CreateDetail(SyncEventStatus.DeadLetter, attemptCount: 3);
        var expected = CreateActionResult(current, SyncEventStatus.Pending, attemptCount: 0);
        _repository.GetOutboxDetailAsync(1, current.Id, Arg.Any<CancellationToken>()).Returns(current);
        _repository.RetryDeadLetterAsync(1, current.Id, "soporte", true, "admin", Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new RetryDeadLetterSyncOutboxCommandHandler(_companyContext, _repository);

        var result = await handler.Handle(new RetryDeadLetterSyncOutboxCommand(current.Id, "soporte", true, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PreviousStatus.Should().Be(SyncEventStatus.DeadLetter);
        result.Value.NewStatus.Should().Be(SyncEventStatus.Pending);
        result.Value.AttemptCount.Should().Be(0);
        await _repository.Received(1).RetryDeadLetterAsync(1, current.Id, "soporte", true, "admin", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReleaseExpiredLock_MarksPending_WhenInProcessLockExpired()
    {
        var current = CreateDetail(SyncEventStatus.InProcess, lockExpiresAt: DateTime.UtcNow.AddMinutes(-5));
        var expected = CreateActionResult(current, SyncEventStatus.Pending);
        _repository.GetOutboxDetailAsync(1, current.Id, Arg.Any<CancellationToken>()).Returns(current);
        _repository.ReleaseExpiredLockAsync(1, current.Id, "expired", "admin", Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new ReleaseExpiredSyncLockCommandHandler(_companyContext, _repository);

        var result = await handler.Handle(new ReleaseExpiredSyncLockCommand(current.Id, "expired", "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PreviousStatus.Should().Be(SyncEventStatus.InProcess);
        result.Value.NewStatus.Should().Be(SyncEventStatus.Pending);
        await _repository.Received(1).ReleaseExpiredLockAsync(1, current.Id, "expired", "admin", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReleaseExpiredLock_RejectsCurrentLock()
    {
        var current = CreateDetail(SyncEventStatus.InProcess, lockExpiresAt: DateTime.UtcNow.AddMinutes(5));
        _repository.GetOutboxDetailAsync(1, current.Id, Arg.Any<CancellationToken>()).Returns(current);
        var handler = new ReleaseExpiredSyncLockCommandHandler(_companyContext, _repository);

        var result = await handler.Handle(new ReleaseExpiredSyncLockCommand(current.Id, null, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("vigente");
        await _repository.DidNotReceiveWithAnyArgs().ReleaseExpiredLockAsync(default, default, default, default, default);
    }

    [Fact]
    public void ManualActionRepository_DoesNotModifyPayloadOrEntityIdentity()
    {
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncOutboxRepository.cs");

        var retryStart = repository.IndexOf("public async Task<SyncOutboxActionResultDto?> RetryErrorAsync", StringComparison.Ordinal);
        var targetsStart = repository.IndexOf("public async Task<IReadOnlyCollection<SyncOutboxTargetDto>> GetTargetsAsync", StringComparison.Ordinal);
        var actionMethods = repository[retryStart..targetsStart];

        actionMethods.Should().NotContain("PayloadJson =");
        actionMethods.Should().NotContain("EntityGlobalId =");
        actionMethods.Should().NotContain("EntityName =");
        actionMethods.Should().Contain("SyncAuditAction.Retried");
        actionMethods.Should().Contain("SyncAuditAction.RetriedFromDeadLetter");
        actionMethods.Should().Contain("SyncAuditAction.LockReleased");
    }

    [Fact]
    public async Task ProfileExecution_BlocksBranchToMasterAdministrativeExecutionWithStableCode()
    {
        var profiles = Substitute.For<ISyncProfileRepository>();
        var executions = Substitute.For<ISyncProfileExecutionRepository>();
        profiles.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns(new SyncProfileDetailDto(
            42, 1, "MST", "Matriz", "BP-PROPOSALS", "BP proposals", null,
            "BranchToMaster", "Incremental", "CentralReview", 100, 3, 30, 30, true,
            null, null, DateTime.UtcNow, null, null, null, [], [], [], null));
        var service = new SyncProfileExecutionService(
            profiles,
            Substitute.For<ISyncProfileValidationService>(),
            executions,
            Array.Empty<ISyncFullEntitySource>(),
            Substitute.For<ISyncEventPublisher>(),
            Substitute.For<ISyncEntityCatalogService>(),
            Substitute.For<ISystemClock>());

        var result = await service.RequestExecutionAsync(
            42,
            new SyncProfileExecutionRequest { ExecutionType = "Manual" });

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "SYNC_BRANCH_TO_MASTER_INCREMENTAL_ONLY");
        await executions.DidNotReceiveWithAnyArgs().CreateAsync(default!, default);
    }

    [Fact]
    public void ManualActionRepository_RegistersAuditForRetryError()
    {
        var actionMethod = ReadActionMethod("RetryErrorAsync", "RetryDeadLetterAsync");

        actionMethod.Should().Contain("SyncAuditAction.Retried");
        actionMethod.Should().Contain("SyncEventStatus.Pending");
        actionMethod.Should().Contain("BuildMessage(\"Reintento manual de evento Error.\", reason)");
        actionMethod.Should().Contain("createdBy");
        AssertAuditContractFields();
    }

    [Fact]
    public void ManualActionRepository_RegistersAuditForRetryDeadLetter()
    {
        var actionMethod = ReadActionMethod("RetryDeadLetterAsync", "ReleaseExpiredLockAsync");

        actionMethod.Should().Contain("SyncAuditAction.RetriedFromDeadLetter");
        actionMethod.Should().Contain("SyncEventStatus.Pending");
        actionMethod.Should().Contain("reason)");
        actionMethod.Should().Contain("createdBy");
        actionMethod.Should().Contain("AttemptCount = CASE WHEN @ResetAttemptCount = 1 THEN 0 ELSE AttemptCount END");
        AssertAuditContractFields();
    }

    [Fact]
    public void ManualActionRepository_RegistersAuditForReleaseExpiredLock()
    {
        var actionMethod = ReadActionMethod("ReleaseExpiredLockAsync", "GetTargetsAsync");

        actionMethod.Should().Contain("SyncAuditAction.LockReleased");
        actionMethod.Should().Contain("newStatus");
        actionMethod.Should().Contain("BuildMessage(\"Lock vencido liberado manualmente.\", reason)");
        actionMethod.Should().Contain("createdBy");
        AssertAuditContractFields();
    }

    private static SyncOutboxDetailDto CreateDetail(
        SyncEventStatus status,
        int attemptCount = 1,
        DateTime? lockExpiresAt = null)
    {
        return new SyncOutboxDetailDto(
            Id: 100,
            EventId: Guid.NewGuid(),
            CompanyId: 1,
            EntityName: "BusinessPartner",
            EntityGlobalId: Guid.NewGuid(),
            EntityCode: "CLI-001",
            Operation: SyncOperation.Updated,
            PayloadJson: """{"payload":{"code":"CLI-001"}}""",
            SourceSystem: null,
            SourceReference: "10",
            Status: status,
            AttemptCount: attemptCount,
            MaxAttempts: 3,
            NextRetryAt: DateTime.UtcNow.AddMinutes(10),
            LockedBy: lockExpiresAt is null ? null : "worker-a",
            LockedAt: lockExpiresAt is null ? null : DateTime.UtcNow.AddMinutes(-10),
            LockExpiresAt: lockExpiresAt,
            CreatedAt: DateTime.UtcNow.AddHours(-1),
            ProcessedAt: status is SyncEventStatus.Applied or SyncEventStatus.Ignored or SyncEventStatus.DeadLetter ? DateTime.UtcNow : null,
            LastErrorMessage: status is SyncEventStatus.Error or SyncEventStatus.DeadLetter ? "Error previo." : null);
    }

    private static SyncOutboxActionResultDto CreateActionResult(
        SyncOutboxDetailDto current,
        SyncEventStatus newStatus,
        int? attemptCount = null)
    {
        return new SyncOutboxActionResultDto(
            current.Id,
            current.EventId,
            current.CompanyId,
            current.EntityName,
            current.EntityGlobalId,
            current.Status,
            newStatus,
            attemptCount ?? current.AttemptCount,
            current.MaxAttempts,
            current.LockExpiresAt,
            "ok");
    }

    private static string ReadActionMethod(string startMethodName, string endMethodName)
    {
        var repository = ReadSyncOutboxRepository();
        var start = repository.IndexOf($"public async Task<SyncOutboxActionResultDto?> {startMethodName}", StringComparison.Ordinal);
        var end = repository.IndexOf($"public async Task", start + 1, StringComparison.Ordinal);

        while (end >= 0 && !repository[end..].StartsWith($"public async Task", StringComparison.Ordinal))
        {
            end = repository.IndexOf("public async Task", end + 1, StringComparison.Ordinal);
        }

        var expectedEnd = repository.IndexOf($"public async Task", repository.IndexOf(endMethodName, StringComparison.Ordinal) - 80, StringComparison.Ordinal);
        if (expectedEnd > start)
        {
            end = expectedEnd;
        }

        end = end > start ? end : repository.Length;
        return repository[start..end];
    }

    private static void AssertAuditContractFields()
    {
        var repository = ReadSyncOutboxRepository();
        var auditStart = repository.IndexOf("private static async Task AddAuditAsync", StringComparison.Ordinal);
        var auditMethod = repository[auditStart..];

        auditMethod.Should().Contain("INSERT INTO dbo.SyncAudit");
        auditMethod.Should().Contain("EventId");
        auditMethod.Should().Contain("EntityName");
        auditMethod.Should().Contain("EntityGlobalId");
        auditMethod.Should().Contain("PreviousStatus");
        auditMethod.Should().Contain("NewStatus");
        auditMethod.Should().Contain("Action");
        auditMethod.Should().Contain("Message");
        auditMethod.Should().Contain("CreatedBy");
        auditMethod.Should().Contain("current.EventId");
        auditMethod.Should().Contain("current.EntityName");
        auditMethod.Should().Contain("current.EntityGlobalId");
        auditMethod.Should().Contain("current.Status.ToString()");
        auditMethod.Should().Contain("newStatus.ToString()");
        auditMethod.Should().Contain("NormalizeCreatedBy(createdBy)");
    }

    private static string ReadSyncOutboxRepository()
    {
        return ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncOutboxRepository.cs");
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var scriptPath = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(scriptPath))
            {
                return File.ReadAllText(scriptPath);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro el archivo {Path.Combine(pathParts)}.");
    }
}
