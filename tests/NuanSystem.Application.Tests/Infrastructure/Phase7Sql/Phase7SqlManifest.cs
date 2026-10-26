namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Test infrastructure only. These declarations never resolve secrets or open connections.
internal sealed record Phase7SqlManifest(
    int SchemaVersion,
    Guid RunId,
    string Operator,
    string ExecutionAuthorizationId,
    string ProvisioningAuthorizationId,
    DateTimeOffset NotBeforeUtc,
    DateTimeOffset ExpiresAtUtc,
    int RunTimeoutSeconds,
    int CommandTimeoutSeconds,
    Phase7SqlServer Server,
    Phase7SqlTarget TenantA,
    Phase7SqlTarget TenantB);

internal sealed record Phase7SqlServer(string DataSource, string ExpectedServerName, string ProductVersion);

internal sealed record Phase7SqlTarget(
    string Role,
    string DatabaseName,
    int DatabaseId,
    int CompanyId,
    Guid RunMarker,
    string Purpose,
    int CompatibilityLevel,
    string Collation,
    bool ReadCommittedSnapshot,
    string SchemaSha256,
    IReadOnlyDictionary<string, string> ProcedureSha256,
    string ApplicationPrincipal,
    string ObserverPrincipal,
    IReadOnlyList<string> ApplicationPermissions,
    IReadOnlyList<string> ObserverPermissions,
    string ApplicationSecretReference,
    string ObserverSecretReference);

// Supplied separately by a trusted caller, never deserialized from the manifest.
// Consistency with this record is not verification of a human signature or live SQL identity.
internal sealed record Phase7SqlApproval(
    string AuthorizationId,
    string ProvisioningAuthorizationId,
    string Operator,
    Guid RunId,
    string ManifestSha256,
    string DataSource,
    string ExpectedServerName,
    Phase7SqlApprovedDestination TenantA,
    Phase7SqlApprovedDestination TenantB,
    DateTimeOffset NotBeforeUtc,
    DateTimeOffset ExpiresAtUtc);

internal sealed record Phase7SqlApprovedDestination(string Role, string DatabaseName, int DatabaseId, int CompanyId);

internal sealed class Phase7SqlOfflineValidation(Phase7SqlManifest manifest, string manifestSha256)
{
    public Phase7SqlManifest Manifest { get; } = manifest;
    public string ManifestSha256 { get; } = manifestSha256;
    public bool RequiresOnlineVerification => true;
}

internal sealed class Phase7SqlGuardException(string errorCode)
    : InvalidOperationException($"Validacion SQL aislada rechazada ({errorCode}).")
{
    public string ErrorCode { get; } = errorCode;
}
