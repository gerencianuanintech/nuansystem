using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Scheduling;
using NuanSystem.Application.Features.SapSync.Warehouses.Services;

namespace NuanSystem.Application.Tests.Features.SapSync.Warehouses;

public sealed class SapWarehouseExecutionProcessorTests
{
    [Fact]
    public async Task W8_TransientFailure_SchedulesOnlyFailedRowAndPreservesOtherSuccess()
    {
        var reader = Substitute.For<ISapWarehouseReader>();
        var warehouseRepository = Substitute.For<IWarehouseRepository>();
        var executionRepository = Substitute.For<ISapSyncExecutionRepository>();
        var retryPolicy = Substitute.For<ISapSyncRetryPolicy>();
        var context = Context();
        var running = Execution(context, SapSyncExecutionStatuses.Running);
        reader.GetWarehousesAsync(context.CompanyId, Arg.Any<CancellationToken>()).Returns([
            Record("WH-FAIL"),
            Record("WH-OK")
        ]);
        executionRepository.GetByExecutionUidAsync(context.ExecutionUid, Arg.Any<CancellationToken>())
            .Returns(running);
        var calls = 0;
        warehouseRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            calls++;
            if (calls == 1)
            {
                throw new TimeoutException("database connection detail");
            }

            return new[] { Local("WH-OK") };
        });
        executionRepository.UpsertDetailAsync(Arg.Any<SapSyncExecutionDetailData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [1]));
        executionRepository.TransitionAsync(Arg.Any<SapSyncExecutionStateData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [2]));
        retryPolicy.Evaluate(
                Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<Exception?>(),
                Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTime>())
            .Returns(new SapSyncRetryDecision(true, false, DateTime.UtcNow.AddMinutes(1), "transient"));
        var processor = new SapWarehouseExecutionProcessor(
            reader,
            new SapWarehouseRecordProcessor(warehouseRepository, Substitute.For<ISender>()),
            executionRepository,
            retryPolicy);

        await processor.ProcessAsync(context);

        await executionRepository.Received(1).UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail =>
                detail.SourceRecordKey == "WH-FAIL"
                && detail.Status == SapSyncExecutionDetailStatuses.RetryScheduled
                && detail.ErrorClass == SapSyncSafeErrorClasses.Transient
                && detail.ApprovedSnapshotType == SapSyncApprovedSnapshotTypes.WarehouseV1
                && detail.SnapshotHash != null
                && detail.SnapshotHash.Length == 32
                && detail.SafeMessage != null
                && !detail.SafeMessage.Contains("database connection detail")),
            Arg.Any<CancellationToken>());
        await executionRepository.Received(1).UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail =>
                detail.SourceRecordKey == "WH-OK"
                && detail.Status == SapSyncExecutionDetailStatuses.Unchanged),
            Arg.Any<CancellationToken>());
        await executionRepository.Received(1).TransitionAsync(
            Arg.Is<SapSyncExecutionStateData>(state =>
                state.NewStatus == SapSyncExecutionStatuses.RetryScheduled
                && state.TotalRecords == 2
                && state.UnchangedRecords == 1
                && state.RetryScheduledRecords == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancellationAfterCurrentRow_DoesNotStartNextRowAndClosesCancelled()
    {
        var reader = Substitute.For<ISapWarehouseReader>();
        var warehouseRepository = Substitute.For<IWarehouseRepository>();
        var executionRepository = Substitute.For<ISapSyncExecutionRepository>();
        var context = Context();
        var running = Execution(context, SapSyncExecutionStatuses.Running);
        var cancelling = Execution(context, SapSyncExecutionStatuses.Cancelling);
        reader.GetWarehousesAsync(context.CompanyId, Arg.Any<CancellationToken>()).Returns([
            Record("WH-01"),
            Record("WH-02")
        ]);
        executionRepository.GetByExecutionUidAsync(context.ExecutionUid, Arg.Any<CancellationToken>())
            .Returns(running, running, cancelling);
        warehouseRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([Local("WH-01")]);
        executionRepository.UpsertDetailAsync(Arg.Any<SapSyncExecutionDetailData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [1]));
        executionRepository.TransitionAsync(Arg.Any<SapSyncExecutionStateData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [2]));
        var processor = new SapWarehouseExecutionProcessor(
            reader,
            new SapWarehouseRecordProcessor(warehouseRepository, Substitute.For<ISender>()),
            executionRepository,
            Substitute.For<ISapSyncRetryPolicy>());

        await processor.ProcessAsync(context);

        await executionRepository.Received(1).UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail => detail.SourceRecordKey == "WH-01"),
            Arg.Any<CancellationToken>());
        await executionRepository.DidNotReceive().UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail => detail.SourceRecordKey == "WH-02"),
            Arg.Any<CancellationToken>());
        await executionRepository.Received(1).TransitionAsync(
            Arg.Is<SapSyncExecutionStateData>(state =>
                state.ExpectedStatus == SapSyncExecutionStatuses.Cancelling
                && state.NewStatus == SapSyncExecutionStatuses.Cancelled
                && state.TotalRecords == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancellationDuringExecutionInitialization_ClosesRunningExecutionAsCancelled()
    {
        var reader = Substitute.For<ISapWarehouseReader>();
        var warehouseRepository = Substitute.For<IWarehouseRepository>();
        var executionRepository = Substitute.For<ISapSyncExecutionRepository>();
        var context = Context();
        var running = Execution(context, SapSyncExecutionStatuses.Running) with { RowVersion = [2] };
        var cancelling = Execution(context, SapSyncExecutionStatuses.Cancelling) with { RowVersion = [3] };
        using var cancellation = new CancellationTokenSource();
        var reads = 0;
        executionRepository.GetByExecutionUidAsync(context.ExecutionUid, Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                reads++;
                if (reads == 1)
                {
                    return null;
                }

                if (reads == 2)
                {
                    throw new OperationCanceledException(cancellation.Token);
                }

                return reads == 3 ? running : cancelling;
            });
        executionRepository.CreateAsync(Arg.Any<SapSyncExecutionCreateData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "Created", [1]));
        executionRepository.TransitionAsync(Arg.Any<SapSyncExecutionStateData>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var state = call.Arg<SapSyncExecutionStateData>();
                if (state.ExpectedStatus == SapSyncExecutionStatuses.Pending
                    && state.NewStatus == SapSyncExecutionStatuses.Running)
                {
                    cancellation.Cancel();
                }

                return new SapSyncExecutionWriteResult(1, "Updated", [2]);
            });
        var processor = new SapWarehouseExecutionProcessor(
            reader,
            new SapWarehouseRecordProcessor(warehouseRepository, Substitute.For<ISender>()),
            executionRepository,
            Substitute.For<ISapSyncRetryPolicy>());

        var action = () => processor.ProcessAsync(context, cancellation.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
        await executionRepository.Received(1).TransitionAsync(
            Arg.Is<SapSyncExecutionStateData>(state =>
                state.ExpectedStatus == SapSyncExecutionStatuses.Running
                && state.NewStatus == SapSyncExecutionStatuses.Cancelling
                && state.LastSafeErrorCode == "SAP_WAREHOUSE_EXECUTION_INTERRUPTED"),
            Arg.Is<CancellationToken>(token => !token.IsCancellationRequested));
        await executionRepository.Received(1).TransitionAsync(
            Arg.Is<SapSyncExecutionStateData>(state =>
                state.ExpectedStatus == SapSyncExecutionStatuses.Cancelling
                && state.NewStatus == SapSyncExecutionStatuses.Cancelled
                && state.LastSafeErrorCode == "SAP_WAREHOUSE_EXECUTION_INTERRUPTED"),
            Arg.Is<CancellationToken>(token => !token.IsCancellationRequested));
        await reader.DidNotReceive().GetWarehousesAsync(
            Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProfileWarehouseFilter_ProcessesContainsOrExactNameOnly()
    {
        var reader = Substitute.For<ISapWarehouseReader>();
        var warehouseRepository = Substitute.For<IWarehouseRepository>();
        var executionRepository = Substitute.For<ISapSyncExecutionRepository>();
        var context = Context() with
        {
            WarehouseNameContains = " mega ",
            WarehouseExactName = " feria libre "
        };
        var running = Execution(context, SapSyncExecutionStatuses.Running);
        reader.GetWarehousesAsync(context.CompanyId, Arg.Any<CancellationToken>()).Returns([
            Record("02", "MEGA AMERICAS"),
            Record("18", "FERIA LIBRE"),
            Record("99", "BODEGA CENTRAL")
        ]);
        executionRepository.GetByExecutionUidAsync(context.ExecutionUid, Arg.Any<CancellationToken>())
            .Returns(running);
        warehouseRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([
            Local("02", "MEGA AMERICAS"),
            Local("18", "FERIA LIBRE")
        ]);
        executionRepository.UpsertDetailAsync(Arg.Any<SapSyncExecutionDetailData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [1]));
        executionRepository.TransitionAsync(Arg.Any<SapSyncExecutionStateData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [2]));
        var processor = new SapWarehouseExecutionProcessor(
            reader,
            new SapWarehouseRecordProcessor(warehouseRepository, Substitute.For<ISender>()),
            executionRepository,
            Substitute.For<ISapSyncRetryPolicy>());

        await processor.ProcessAsync(context);

        await executionRepository.Received(1).UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail => detail.SourceRecordKey == "02"),
            Arg.Any<CancellationToken>());
        await executionRepository.Received(1).UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail => detail.SourceRecordKey == "18"),
            Arg.Any<CancellationToken>());
        await executionRepository.DidNotReceive().UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail => detail.SourceRecordKey == "99"),
            Arg.Any<CancellationToken>());
        await executionRepository.Received(1).TransitionAsync(
            Arg.Is<SapSyncExecutionStateData>(state =>
                state.NewStatus == SapSyncExecutionStatuses.Completed
                && state.TotalRecords == 2
                && state.UnchangedRecords == 2),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ContinueOnErrorFalse_StopsAfterFirstFailedRecord()
    {
        var reader = Substitute.For<ISapWarehouseReader>();
        var warehouseRepository = Substitute.For<IWarehouseRepository>();
        var executionRepository = Substitute.For<ISapSyncExecutionRepository>();
        var retryPolicy = Substitute.For<ISapSyncRetryPolicy>();
        var context = Context() with { ContinueOnError = false, BatchSize = 1 };
        var running = Execution(context, SapSyncExecutionStatuses.Running);
        reader.GetWarehousesAsync(context.CompanyId, Arg.Any<CancellationToken>()).Returns([
            Record("WH-FAIL"),
            Record("WH-NOT-STARTED")
        ]);
        executionRepository.GetByExecutionUidAsync(context.ExecutionUid, Arg.Any<CancellationToken>())
            .Returns(running);
        warehouseRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns<IReadOnlyCollection<WarehouseDto>>(_ => throw new TimeoutException());
        executionRepository.UpsertDetailAsync(Arg.Any<SapSyncExecutionDetailData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [1]));
        executionRepository.TransitionAsync(Arg.Any<SapSyncExecutionStateData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [2]));
        retryPolicy.Evaluate(
                Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<Exception?>(),
                Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTime>())
            .Returns(new SapSyncRetryDecision(true, false, DateTime.UtcNow.AddMinutes(1), "transient"));
        var processor = new SapWarehouseExecutionProcessor(
            reader,
            new SapWarehouseRecordProcessor(warehouseRepository, Substitute.For<ISender>()),
            executionRepository,
            retryPolicy);

        await processor.ProcessAsync(context);

        await executionRepository.Received(1).UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail => detail.SourceRecordKey == "WH-FAIL"),
            Arg.Any<CancellationToken>());
        await executionRepository.DidNotReceive().UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail => detail.SourceRecordKey == "WH-NOT-STARTED"),
            Arg.Any<CancellationToken>());
    }

    private static SapSyncScheduledExecutionContext Context() => new(
        ExecutionUid: Guid.NewGuid(),
        CorrelationId: Guid.NewGuid().ToString(),
        CandidateSource: SapSyncScheduleCandidateSources.Profile,
        CompanyId: 10,
        CompanyCode: "DEMO",
        ProfileId: 5,
        ProfileCode: "SAP-DEMO",
        ProfileName: "SAP DEMO",
        ProfileEntityId: 8,
        EntityCode: SapSyncEntityCode.Warehouses,
        Direction: SapSyncDirection.SapToErp,
        SyncMode: "Full",
        BatchSize: 50,
        MaxAttempts: 3,
        ExecutionOrder: 1,
        ContinueOnError: true,
        ExecutionTimeoutMinutes: 10,
        ScheduleId: 20,
        ScheduleType: "Interval",
        TimeZoneId: "America/Guayaquil",
        ScheduledForAtUtc: DateTime.UtcNow,
        WorkerInstance: "worker-01",
        CompatibilityVersion: null,
        RequiredSuccessfulCycles: 0);

    private static SapSyncExecutionDto Execution(
        SapSyncScheduledExecutionContext context,
        string status) => new(
        Id: 1,
        ExecutionUid: context.ExecutionUid,
        RunGroupId: context.ExecutionUid,
        CorrelationId: Guid.Parse(context.CorrelationId),
        SapSyncProfileId: context.ProfileId,
        SapSyncProfileEntityId: context.ProfileEntityId,
        ProfileCode: context.ProfileCode,
        ProfileName: context.ProfileName,
        CompanyId: context.CompanyId,
        CompanyCode: context.CompanyCode,
        EntityCode: context.EntityCode,
        Direction: context.Direction.ToString(),
        TriggerType: SapSyncTriggerTypes.Scheduled,
        ParentExecutionId: null,
        Status: status,
        BatchSize: context.BatchSize,
        MaxAttempts: context.MaxAttempts,
        ExecutionOrder: context.ExecutionOrder,
        TimeoutMinutes: context.ExecutionTimeoutMinutes,
        ScheduleType: context.ScheduleType,
        TimeZoneId: context.TimeZoneId,
        ProfileSnapshotJson: "{}",
        EffectiveParametersJson: "{}",
        RequestedByUserId: null,
        RequestedByUserName: null,
        RequestedAtUtc: DateTime.UtcNow,
        WorkerInstance: context.WorkerInstance,
        StartedAtUtc: DateTime.UtcNow,
        LastProgressAtUtc: DateTime.UtcNow,
        FinishedAtUtc: null,
        CancellationRequestedAtUtc: status == SapSyncExecutionStatuses.Cancelling ? DateTime.UtcNow : null,
        TotalRecords: 0,
        CreatedRecords: 0,
        UpdatedRecords: 0,
        UnchangedRecords: 0,
        ApprovalRequiredRecords: 0,
        ConflictRecords: 0,
        SkippedRecords: 0,
        RetryScheduledRecords: 0,
        FailedRecords: 0,
        DeadLetterRecords: 0,
        LastSafeErrorCode: null,
        LastSafeErrorMessage: null,
        RowVersion: [1]);

    private static SapWarehouseRecord Record(string code) =>
        Record(code, $"Bodega {code}");

    private static SapWarehouseRecord Record(string code, string name) =>
        new(code, name, "Direccion", "Cuenca", "Azuay", "EC", true);

    private static WarehouseDto Local(string code) => Local(code, $"Bodega {code}");

    private static WarehouseDto Local(string code, string name) => new()
    {
        Id = 10,
        GlobalId = Guid.NewGuid(),
        Code = code,
        Name = name,
        Address = "Direccion",
        City = "Cuenca",
        Province = "Azuay",
        Country = "EC",
        ExternalSystem = "SAP_B1",
        ExternalCode = code,
        SapCode = code,
        IsActive = true
    };
}
