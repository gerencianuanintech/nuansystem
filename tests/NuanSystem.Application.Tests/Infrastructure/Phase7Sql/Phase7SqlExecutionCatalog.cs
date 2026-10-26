using System.Security.Cryptography;
using System.Text;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal enum Phase7SqlEvidenceStatus { NotRun, Passed, Failed, Skipped, Inconclusive }
internal enum Phase7SqlEvidenceOrigin { Offline, SqlServerLaboratory }
internal sealed record Phase7SqlCaseSpec(string Id, IReadOnlyList<string> Checks)
{
    public string Method => Id.Replace('.', '_');
    public string TestName => $"{Phase7SqlExecutionCatalog.ExpectedTestClass}.{Method}";
}

// Proposed executable-suite contract. No SQL Fact methods or launcher are created here.
internal static class Phase7SqlExecutionCatalog
{
    public const string ExpectedTestClass = "NuanSystem.Application.Tests.Features.BusinessPartners.BusinessPartnerTransactionSqlTests";
    public const string RestrictedFilter = "Category=SqlServerIntegration&Scope=Phase7Isolated&FullyQualifiedName~BusinessPartnerTransactionSqlTests";
    public static IReadOnlyList<Phase7SqlCaseSpec> Cases { get; } = Build();
    public static IReadOnlyList<string> RunChecks { get; } = Array.AsReadOnly(new[]
    {
        "ApprovedManifest", "DestinationPreflight", "EffectiveSchemaAndProcedures", "FaultTriggerDefinitions", "LookupSeeds",
        "OfflineBaseline", "ArtifactHashes", "DurableSnapshots", "SessionTraces", "ConnectionCounts", "ErrorDiagnostics",
        "NoOrphanSessions", "NoExternalCalls", "SapDisabled", "WorkersAndRelayDisabled", "GatesDisabledAtEnd", "DataPreserved"
    });
    public static IReadOnlyList<string> ArtifactKinds { get; } = Array.AsReadOnly(new[]
    {
        "Manifest", "SourceInventory", "Binaries", "CaseCatalog", "Schema", "Procedures", "Trx", "Snapshots", "Sessions",
        "Connections", "Errors", "Boundaries", "Environment", "SessionTermination"
    });
    public static string Sha256 { get; } = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n",
        Cases.Select(c => $"{c.Id}|{c.TestName}|{string.Join(',', c.Checks)}")))));

    private static IReadOnlyList<Phase7SqlCaseSpec> Build()
    {
        var cases = new List<Phase7SqlCaseSpec>();
        void Add(string id, params string[] checks) => cases.Add(new(id, Array.AsReadOnly(checks)));
        Add("S01", "Legacy", "Pilot", "BusinessAndChildren", "AuditAndIntention");
        Add("S02", "Legacy", "Pilot", "StableGlobalId", "CommercialChanges");
        Add("S03", "Legacy", "Pilot", "LogicalDelete", "StableGlobalId");
        Add("S04", "NotMaster", "SyncDisabled", "BothDisabled", "SapNoneSyncEnabled");
        foreach (var id in Enumerable.Range(5, 7)) Add($"S{id:00}", "Create", "Update", "Delete", "ExactRollback", "PrimaryAndSecondaryErrors");
        Add("S12", "TenantA", "TenantB", "SameBusinessKey", "DistinctDataAndPayloads");
        Add("S13", "PoolingEnabled", "ObservedSpidReuse", "NoResidualTransaction", "NoFaultMarkers");
        Add("S14", "DuplicateRejected", "OneIdentityAndEvent", "UnitReusable");
        Add("S15", "IndependentUnits", "InsertBarrier", "OneWinner", "ConflictRecorded", "NoRetry");
        Add("S16", "TwoSuccessfulUpdates", "EventsPerOperation", "ObservedAuditDelta", "HistoryPreserved", "ChildReplacementSemantics");
        Add("S17", "OwnerOnlyClose", "BeforeCommit", "ExactRollback", "OneCallbackAndConnection", "ErrorsPreserved");
        Add("S18", "EscapedSuccess", "EscapedFailure", "CompanyChanged", "Nested", "Overlap", "Reuse");
        Add("Equivalence", "Commercial", "Relations", "Audit", "Events", "IndependentGeneratedIdentity");
        Add("R01.Legacy", "InternalRollbackNotFound", "NoPartialCommit", "EffectiveProcedureOutcomeRecorded");
        Add("R01.Pilot", "InternalRollbackNotFound", "NoPartialCommit", "EffectiveProcedureOutcomeRecorded");
        return cases.AsReadOnly();
    }
}

internal sealed record Phase7SqlCaseEvidence(string Id, IReadOnlyDictionary<string, Phase7SqlEvidenceStatus> Checks);
internal sealed record Phase7SqlArtifactEvidence(string Kind, string RelativePath, string Sha256);
internal sealed record Phase7SqlRunEvidence(Guid RunId, string TenantADatabase, string TenantBDatabase,
    string ManifestSha256, string SourceSha256, string BinarySha256, string CatalogSha256,
    DateTimeOffset StartedAtUtc, DateTimeOffset FinishedAtUtc, Phase7SqlEvidenceOrigin Origin,
    IReadOnlyDictionary<string, Phase7SqlEvidenceStatus> RunChecks, IReadOnlyList<Phase7SqlCaseEvidence> Cases,
    IReadOnlyList<Phase7SqlArtifactEvidence> Artifacts, byte[] TrxBytes, int ExternalBoundaryAttempts, bool SapExecuted, bool WorkersExecuted);

internal sealed class Phase7SqlClosureAssessment(IEnumerable<string> reasons)
{
    public IReadOnlyList<string> Reasons { get; } = Array.AsReadOnly(reasons.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray());
    public bool ReadyForIndependentPilotReview => Reasons.Count == 0;
    public bool GlobalPhase7Closed => false;
    public bool EvidenceAuthenticityVerified => false;
}
