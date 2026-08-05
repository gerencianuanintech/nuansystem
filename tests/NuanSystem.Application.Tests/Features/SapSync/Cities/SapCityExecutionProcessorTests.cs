using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Cities.Services;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCityExecutionProcessorTests
{
    [Fact]
    public async Task FullExecution_TransientRowFailurePersistsApprovedSnapshotAndSchedulesRetry()
    {
        var reader = Substitute.For<ISapCityReader>();
        var geographyRepository = Substitute.For<IGeographyRepository>();
        var executionRepository = Substitute.For<ISapSyncExecutionRepository>();
        var retryPolicy = Substitute.For<ISapSyncRetryPolicy>();
        var context = Context();
        var running = Execution(context, SapSyncExecutionStatuses.Running);
        var country = Country();
        var province = Province(country);
        reader.GetCitiesAsync(context.CompanyId, Arg.Any<CancellationToken>())
            .Returns([new SapCityRecord("EC", "01", "0101", "Quito")]);
        executionRepository.GetByExecutionUidAsync(context.ExecutionUid, Arg.Any<CancellationToken>())
            .Returns(running);
        geographyRepository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        geographyRepository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([province]);
        geographyRepository.GetCitiesAsync(Arg.Any<CancellationToken>())
            .Returns<IReadOnlyCollection<CityDto>>(_ => throw new TimeoutException("sensitive detail"));
        executionRepository.UpsertDetailAsync(
                Arg.Any<SapSyncExecutionDetailData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [1]));
        executionRepository.TransitionAsync(
                Arg.Any<SapSyncExecutionStateData>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "OK", [2]));
        retryPolicy.Evaluate(
                Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<Exception?>(),
                Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTime>())
            .Returns(new SapSyncRetryDecision(
                true, false, DateTime.UtcNow.AddMinutes(1), "transient"));
        var processor = new SapCityExecutionProcessor(
            reader,
            new SapCityRecordProcessor(geographyRepository, Substitute.For<ISender>()),
            executionRepository,
            retryPolicy);

        await processor.ProcessAsync(context);

        await reader.Received(1).GetCitiesAsync(context.CompanyId, Arg.Any<CancellationToken>());
        await executionRepository.Received(1).UpsertDetailAsync(
            Arg.Is<SapSyncExecutionDetailData>(detail =>
                detail.SourceRecordKey == "EC|01|0101"
                && detail.Status == SapSyncExecutionDetailStatuses.RetryScheduled
                && detail.ErrorClass == SapSyncSafeErrorClasses.Transient
                && detail.ApprovedSnapshotType == SapSyncApprovedSnapshotTypes.CityV1
                && detail.SnapshotHash != null
                && detail.SnapshotHash.Length == 32
                && detail.SafeMessage != null
                && !detail.SafeMessage.Contains("sensitive detail")),
            Arg.Any<CancellationToken>());
        await executionRepository.Received(1).TransitionAsync(
            Arg.Is<SapSyncExecutionStateData>(state =>
                state.NewStatus == SapSyncExecutionStatuses.RetryScheduled
                && state.TotalRecords == 1
                && state.RetryScheduledRecords == 1),
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
        EntityCode: SapSyncEntityCode.Cities,
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
        CancellationRequestedAtUtc: null,
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

    private static CountryDto Country() => new()
    {
        Id = 1,
        GlobalId = Guid.NewGuid(),
        Code = "LOCAL-EC",
        Name = "Ecuador",
        ExternalSystem = "SAP_B1",
        ExternalCode = "EC",
        IsActive = true
    };

    private static ProvinceDto Province(CountryDto country) => new()
    {
        Id = 2,
        GlobalId = Guid.NewGuid(),
        CountryId = country.Id,
        CountryGlobalId = country.GlobalId,
        CountryCode = country.Code,
        CountryName = country.Name,
        Code = "LOCAL-01",
        Name = "Pichincha",
        ExternalSystem = "SAP_B1",
        ExternalCode = "EC|01",
        IsActive = true
    };
}
