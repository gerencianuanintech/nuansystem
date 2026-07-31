using System.Data;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Persistence.Repositories.SapSync;

internal sealed class SapSyncScheduleCandidateRow
{
    public SapSyncScheduleCandidateRow()
    {
    }

    public string? CandidateSource { get; set; }
    public int CompanyId { get; set; }
    public string? CompanyCode { get; set; }
    public long? ProfileId { get; set; }
    public string? ProfileCode { get; set; }
    public string? ProfileName { get; set; }
    public bool ProfileIsActive { get; set; }
    public long? ProfileEntityId { get; set; }
    public string? EntityCode { get; set; }
    public string? Direction { get; set; }
    public string? SyncMode { get; set; }
    public int BatchSize { get; set; }
    public int MaxAttempts { get; set; }
    public int ExecutionOrder { get; set; }
    public bool ContinueOnError { get; set; }
    public int ExecutionTimeoutMinutes { get; set; }
    public bool EntityIsActive { get; set; }
    public long? ScheduleId { get; set; }
    public string? ScheduleType { get; set; }
    public int? IntervalMinutes { get; set; }
    public TimeSpan? ExecutionTime { get; set; }
    public string? TimeZoneId { get; set; }
    public bool PreventConcurrentExecutions { get; set; }
    public DateTime? NextExecutionAtUtc { get; set; }
    public DateTime? LastScheduledAtUtc { get; set; }
    public DateTime? LastExecutionAtUtc { get; set; }
    public bool ScheduleIsActive { get; set; }
    public byte[]? ScheduleRowVersion { get; set; }
    public bool SupportsSapToErp { get; set; }
    public bool SupportsErpToSap { get; set; }
    public bool SupportsFull { get; set; }
    public bool SupportsIncremental { get; set; }
    public bool CapabilityIsImplemented { get; set; }
    public bool CapabilityIsActive { get; set; }
    public bool LegacyFallbackEnabled { get; set; }
    public string? CompatibilityVersion { get; set; }
    public int RequiredSuccessfulCycles { get; set; }
    public long SortProfileId { get; set; }
    public long SortEntityId { get; set; }
}

internal static class SapSyncScheduleCandidateRowMapper
{
    internal const string InvalidRequiredValueCode =
        "SAP_SYNC_SCHEDULE_CANDIDATE_REQUIRED_VALUE_INVALID";
    internal const string InvalidDirectionCode =
        "SAP_SYNC_SCHEDULE_CANDIDATE_DIRECTION_INVALID";

    public static SapSyncScheduleCandidate Map(SapSyncScheduleCandidateRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        var directionText = Require(row.Direction, nameof(row.Direction));
        if (!Enum.TryParse<SapSyncDirection>(directionText, ignoreCase: true, out var direction)
            || !Enum.IsDefined(direction))
        {
            throw new DataException(InvalidDirectionCode);
        }

        return new SapSyncScheduleCandidate(
            Require(row.CandidateSource, nameof(row.CandidateSource)),
            row.CompanyId,
            Require(row.CompanyCode, nameof(row.CompanyCode)),
            row.ProfileId,
            Require(row.ProfileCode, nameof(row.ProfileCode)),
            Require(row.ProfileName, nameof(row.ProfileName)),
            row.ProfileIsActive,
            row.ProfileEntityId,
            Require(row.EntityCode, nameof(row.EntityCode)),
            direction,
            Require(row.SyncMode, nameof(row.SyncMode)),
            row.BatchSize,
            row.MaxAttempts,
            row.ExecutionOrder,
            row.ContinueOnError,
            row.ExecutionTimeoutMinutes,
            row.EntityIsActive,
            row.ScheduleId,
            Require(row.ScheduleType, nameof(row.ScheduleType)),
            row.IntervalMinutes,
            row.ExecutionTime,
            Require(row.TimeZoneId, nameof(row.TimeZoneId)),
            row.PreventConcurrentExecutions,
            row.NextExecutionAtUtc,
            row.LastScheduledAtUtc,
            row.LastExecutionAtUtc,
            row.ScheduleIsActive,
            row.ScheduleRowVersion?.ToArray(),
            row.SupportsSapToErp,
            row.SupportsErpToSap,
            row.SupportsFull,
            row.SupportsIncremental,
            row.CapabilityIsImplemented,
            row.CapabilityIsActive,
            row.LegacyFallbackEnabled,
            row.CompatibilityVersion,
            row.RequiredSuccessfulCycles,
            row.SortProfileId,
            row.SortEntityId);
    }

    private static string Require(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DataException($"{InvalidRequiredValueCode}:{fieldName}");
        }

        return value;
    }
}
