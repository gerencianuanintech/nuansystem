using System.Data;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlDestinationObservation(
    string ServerName, string ProductVersion, string DatabaseName, int DatabaseId, int CompanyId,
    Guid RunMarker, string Purpose, int CompatibilityLevel, string Collation, bool ReadCommittedSnapshot,
    string DatabasePrincipal, string Login, string OriginalLogin, bool Privileged, bool CanViewDefinition,
    IReadOnlyList<string> DatabasePermissions, IReadOnlyList<string> ServerPermissions,
    IReadOnlyList<string> ObjectPermissions, string SchemaSha256, IReadOnlyDictionary<string, string> ProcedureSha256,
    int Spid, int TransactionCount, int TransactionState, bool HasFaultMarkers);

internal delegate Task<Phase7SqlDestinationObservation> Phase7SqlDestinationReader(
    IDbConnection connection, IDbTransaction? transaction, int timeoutSeconds, CancellationToken token);

// Explicitly constructed by the isolated test harness only. Never registered in product DI.
// Offline authorization/gates/window and connection-string restrictions remain owned by the fixture.
internal sealed class Phase7SqlDestinationVerifier(Phase7SqlDestinationReader? reader = null) : IPhase7SqlDestinationVerifier
{
    private readonly Phase7SqlDestinationReader read = reader ?? Phase7SqlDestinationSqlReader.ReadAsync;

    public async Task VerifyAsync(Phase7SqlManifest manifest, Phase7SqlTarget target, bool observer,
        IDbConnection connection, IDbTransaction? transaction, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        Require(ReferenceEquals(target, manifest.TenantA) || ReferenceEquals(target, manifest.TenantB), "TARGET");
        Require(manifest.CommandTimeoutSeconds is >= 1 and <= 120, "TIMEOUT");
        Require(connection.State == ConnectionState.Open && (observer ? transaction is null
            : transaction is not null && ReferenceEquals(transaction.Connection, connection)), "HANDLES");
        Phase7SqlDestinationObservation value;
        try { value = await read(connection, transaction, manifest.CommandTimeoutSeconds, token); }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
        catch { throw new Phase7SqlGuardException("PHASE7_SQL_PREFLIGHT_READ"); }
        token.ThrowIfCancellationRequested();
        Require(value is not null, "EMPTY");
        ValidateObservation(manifest,target,observer,value!);
        Require(connection.State == ConnectionState.Open && (transaction is null || ReferenceEquals(transaction.Connection, connection)), "HANDLES");
    }

    internal static void ValidateObservation(Phase7SqlManifest manifest,Phase7SqlTarget target,bool observer,Phase7SqlDestinationObservation value)
    {
        Require(value!.ServerName == manifest.Server.ExpectedServerName && value.ProductVersion == manifest.Server.ProductVersion, "SERVER");
        Require(value.DatabaseName == target.DatabaseName && value.DatabaseId == target.DatabaseId && value.DatabaseId > 4
            && value.CompanyId == target.CompanyId && value.RunMarker == target.RunMarker && value.RunMarker == manifest.RunId
            && value.Purpose == target.Purpose && value.Purpose == "BackendTransactionValidation", "DESTINATION");
        Require(value.CompatibilityLevel == target.CompatibilityLevel && value.Collation == target.Collation
            && value.ReadCommittedSnapshot == target.ReadCommittedSnapshot, "OPTIONS");
        var principal = observer ? target.ObserverPrincipal : target.ApplicationPrincipal;
        Require(value.DatabasePrincipal == principal && value.Login == principal && value.OriginalLogin == principal
            && !value.Privileged && value.CanViewDefinition, "IDENTITY");
        var expected = observer ? target.ObserverPermissions : target.ApplicationPermissions;
        // CONNECT and VIEW DEFINITION are mandatory harness capabilities, separate from business permissions.
        Require(DatabasePermissionsMatch(value.DatabasePermissions, expected), "PERMISSIONS");
        Require(value.ServerPermissions is not null && value.ServerPermissions.Contains("CONNECT SQL", StringComparer.Ordinal)
            && value.ServerPermissions.All(p => p is "CONNECT SQL" or "VIEW ANY DATABASE"), "SERVER_PERMISSIONS");
        Require(value.ObjectPermissions is not null && value.ObjectPermissions.All(p => expected.Contains(p, StringComparer.Ordinal)
            || p is "VIEW DEFINITION" or "VIEW SECURITY DEFINITION" or "VIEW PERFORMANCE DEFINITION"), "OBJECT_PERMISSIONS");
        Require(HashEquals(value.SchemaSha256, target.SchemaSha256), "SCHEMA");
        Require(value.ProcedureSha256 is not null && target.ProcedureSha256.All(p =>
            value.ProcedureSha256.TryGetValue(p.Key, out var actual) && HashEquals(actual, p.Value)), "PROCEDURES");
        Require(value.Spid > 0 && value.TransactionCount == (observer ? 0 : 1)
            && value.TransactionState == (observer ? 0 : 1) && !value.HasFaultMarkers, "SESSION");
    }

    private static bool DatabasePermissionsMatch(IReadOnlyList<string>? actual, IEnumerable<string> expected) => actual is not null
        && actual.Count == actual.Distinct(StringComparer.Ordinal).Count()
        // SQL Server 2016+ public metadata defaults may be explicitly denied in a hardened lab.
        // Permit either presence or absence; neither permits reading key material or writing data.
        && actual.Where(p => p is not "VIEW ANY COLUMN MASTER KEY DEFINITION" and not "VIEW ANY COLUMN ENCRYPTION KEY DEFINITION"
            and not "VIEW SECURITY DEFINITION" and not "VIEW PERFORMANCE DEFINITION")
            .ToHashSet(StringComparer.Ordinal).SetEquals(expected.Concat(["CONNECT", "VIEW DEFINITION"]));
    private static bool HashEquals(string? actual, string expected) => actual is { Length: 64 }
        && actual.All(Uri.IsHexDigit) && actual.Any(c => c != '0') && string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
    private static void Require(bool valid, string suffix)
    {
        if (!valid) throw new Phase7SqlGuardException("PHASE7_SQL_PREFLIGHT_" + suffix);
    }
}
