using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal static class Phase7SqlManifestValidator
{
    public const int MaximumManifestBytes = 65_536;
    public const int MaximumRunSeconds = 3_600;
    public const int MaximumCommandSeconds = 120;
    public const string LaboratoryPurpose = "BackendTransactionValidation";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        AllowDuplicateProperties = false,
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true,
        MaxDepth = 12
    };

    private static readonly string[] RequiredProcedures =
    [
        "dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR",
        "dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR",
        "dbo.SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR",
        "dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID",
        "dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORCODIGO",
        "dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION"
    ];

    public static Phase7SqlOfflineValidation Validate(
        string? json, Phase7SqlApproval? approval, Phase7SqlGates gates, DateTimeOffset utcNow)
    {
        Require(gates.StandardEnabled && gates.IsolatedEnabled, "PHASE7_SQL_GATES_DISABLED");
        Require(!string.IsNullOrWhiteSpace(json), "PHASE7_SQL_MANIFEST_REQUIRED");
        Require(json!.Length <= MaximumManifestBytes && Encoding.UTF8.GetByteCount(json) <= MaximumManifestBytes,
            "PHASE7_SQL_MANIFEST_INVALID");

        Phase7SqlManifest manifest;
        try
        {
            manifest = JsonSerializer.Deserialize<Phase7SqlManifest>(json, JsonOptions)
                ?? throw new JsonException();
        }
        catch (JsonException)
        {
            // Do not propagate parser text, inner exceptions or input containing credentials.
            throw new Phase7SqlGuardException("PHASE7_SQL_MANIFEST_INVALID");
        }

        Require(approval is not null, "PHASE7_SQL_APPROVAL_REQUIRED");
        var trusted = approval!;
        Require(Token(trusted.AuthorizationId) && Token(trusted.ProvisioningAuthorizationId)
            && trusted.AuthorizationId != trusted.ProvisioningAuthorizationId && Token(trusted.Operator)
            && trusted.RunId != Guid.Empty && Hash(trusted.ManifestSha256)
            && trusted.TenantA is not null && trusted.TenantB is not null
            && UtcWindow(trusted.NotBeforeUtc, trusted.ExpiresAtUtc), "PHASE7_SQL_APPROVAL_INVALID");

        var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
        Require(string.Equals(fingerprint, trusted.ManifestSha256, StringComparison.OrdinalIgnoreCase), "PHASE7_SQL_APPROVAL_MISMATCH");
        Require(manifest.SchemaVersion == 1 && manifest.RunId != Guid.Empty && manifest.RunId == trusted.RunId
            && Token(manifest.Operator) && manifest.Operator == trusted.Operator
            && manifest.ExecutionAuthorizationId == trusted.AuthorizationId
            && manifest.ProvisioningAuthorizationId == trusted.ProvisioningAuthorizationId,
            "PHASE7_SQL_APPROVAL_MISMATCH");

        Require(utcNow.Offset == TimeSpan.Zero && UtcWindow(manifest.NotBeforeUtc, manifest.ExpiresAtUtc)
            && manifest.NotBeforeUtc >= trusted.NotBeforeUtc && manifest.ExpiresAtUtc <= trusted.ExpiresAtUtc
            && utcNow >= manifest.NotBeforeUtc && utcNow < manifest.ExpiresAtUtc, "PHASE7_SQL_WINDOW_INVALID");
        Require(manifest.RunTimeoutSeconds is > 0 and <= MaximumRunSeconds
            && manifest.CommandTimeoutSeconds is > 0 and <= MaximumCommandSeconds
            && manifest.CommandTimeoutSeconds <= manifest.RunTimeoutSeconds
            && (manifest.ExpiresAtUtc - utcNow).TotalSeconds >= manifest.RunTimeoutSeconds,
            "PHASE7_SQL_LIMITS_INVALID");

        Require(manifest.Server is not null && manifest.TenantA is not null && manifest.TenantB is not null,
            "PHASE7_SQL_MANIFEST_INVALID");
        ValidateServer(manifest.Server!, trusted);
        ValidateTarget(manifest.TenantA!, trusted.TenantA, "A", manifest.RunId);
        ValidateTarget(manifest.TenantB!, trusted.TenantB, "B", manifest.RunId);
        Require(manifest.TenantA!.DatabaseId != manifest.TenantB!.DatabaseId
            && manifest.TenantA.CompanyId != manifest.TenantB.CompanyId
            && !string.Equals(manifest.TenantA.DatabaseName, manifest.TenantB.DatabaseName, StringComparison.OrdinalIgnoreCase),
            "PHASE7_SQL_TARGET_COLLISION");

        return new Phase7SqlOfflineValidation(manifest with
        {
            TenantA = Freeze(manifest.TenantA),
            TenantB = Freeze(manifest.TenantB)
        }, fingerprint);
    }

    private static void ValidateServer(Phase7SqlServer server, Phase7SqlApproval approval)
    {
        Require(Matches(server.DataSource, @"^(?:tcp:)?[A-Za-z0-9][A-Za-z0-9.-]{0,252}(?:\\[A-Za-z0-9_][A-Za-z0-9_-]{0,127}|,(?<port>[1-9][0-9]{0,4}))?$")
            && Matches(server.ExpectedServerName, @"^[A-Za-z0-9][A-Za-z0-9_.-]{0,127}(?:\\[A-Za-z0-9_-]{1,128})?$")
            && Matches(server.ProductVersion, @"^[0-9]{1,3}\.[0-9]{1,5}\.[0-9]{1,6}\.[0-9]{1,6}$")
            && Version.TryParse(server.ProductVersion, out var version) && version.Major > 0,
            "PHASE7_SQL_SERVER_INVALID");
        var comma = server.DataSource.LastIndexOf(',');
        Require(comma < 0 || int.TryParse(server.DataSource[(comma + 1)..], out var port) && port <= 65_535,
            "PHASE7_SQL_SERVER_INVALID");
        Require(string.Equals(server.DataSource, approval.DataSource, StringComparison.OrdinalIgnoreCase)
            && string.Equals(server.ExpectedServerName, approval.ExpectedServerName, StringComparison.OrdinalIgnoreCase),
            "PHASE7_SQL_SERVER_NOT_APPROVED");
    }

    private static void ValidateTarget(Phase7SqlTarget target, Phase7SqlApprovedDestination approved, string role, Guid runId)
    {
        Require(target.Role == role && approved.Role == role, "PHASE7_SQL_TARGET_ROLE_INVALID");
        var expectedName = $"NuanSystem_Phase7_{runId:N}_{role}";
        Require(target.DatabaseName == expectedName && target.DatabaseId > 4 && target.CompanyId > 0
            && target.DatabaseName == approved.DatabaseName && target.DatabaseId == approved.DatabaseId
            && target.CompanyId == approved.CompanyId, "PHASE7_SQL_TARGET_NOT_APPROVED");
        Require(target.RunMarker == runId && target.Purpose == LaboratoryPurpose,
            "PHASE7_SQL_MARKER_INVALID");
        Require(target.CompatibilityLevel is 130 or 140 or 150 or 160 or 170
            && Matches(target.Collation, @"^[A-Za-z0-9_]{1,128}$") && Hash(target.SchemaSha256),
            "PHASE7_SQL_SCHEMA_INVALID");

        Require(target.ProcedureSha256 is not null && target.ProcedureSha256.Count is >= 6 and <= 64,
            "PHASE7_SQL_SCHEMA_INVALID");
        var procedures = target.ProcedureSha256!;
        Require(procedures.Keys.Distinct(StringComparer.OrdinalIgnoreCase).Count() == procedures.Count
            && procedures.All(entry => Matches(entry.Key, @"^dbo\.[A-Za-z_][A-Za-z0-9_]{0,127}$") && Hash(entry.Value))
            && RequiredProcedures.All(required => procedures.Keys.Contains(required, StringComparer.OrdinalIgnoreCase)),
            "PHASE7_SQL_SCHEMA_INVALID");

        Require(Principal(target.ApplicationPrincipal) && Principal(target.ObserverPrincipal)
            && !string.Equals(target.ApplicationPrincipal, target.ObserverPrincipal, StringComparison.OrdinalIgnoreCase),
            "PHASE7_SQL_PRINCIPAL_INVALID");
        Require(Permissions(target.ApplicationPermissions, ["SELECT", "EXECUTE", "INSERT", "UPDATE"], ["SELECT", "EXECUTE", "INSERT"])
            && Permissions(target.ObserverPermissions, ["SELECT"], ["SELECT"]), "PHASE7_SQL_PERMISSIONS_INVALID");
        Require(target.ApplicationSecretReference == $"env:NUANSYSTEM_PHASE7_TENANT_{role}_CONNECTION_STRING"
            && target.ObserverSecretReference == $"env:NUANSYSTEM_PHASE7_OBSERVER_{role}_CONNECTION_STRING",
            "PHASE7_SQL_SECRET_REFERENCE_INVALID");
    }

    private static bool Permissions(IReadOnlyList<string>? permissions, string[] allowed, string[] required) =>
        permissions is not null && permissions.Count == permissions.Distinct(StringComparer.Ordinal).Count()
        && permissions.All(permission => allowed.Contains(permission, StringComparer.Ordinal))
        && required.All(permission => permissions.Contains(permission, StringComparer.Ordinal));

    private static Phase7SqlTarget Freeze(Phase7SqlTarget target) => target with
    {
        ApplicationPermissions = Array.AsReadOnly(target.ApplicationPermissions.ToArray()),
        ObserverPermissions = Array.AsReadOnly(target.ObserverPermissions.ToArray()),
        ProcedureSha256 = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(target.ProcedureSha256, StringComparer.OrdinalIgnoreCase))
    };

    private static bool Principal(string? value) => Matches(value, @"^[A-Za-z_][A-Za-z0-9_.@\-\\]{0,127}$")
        && !new[] { "sa", "dbo", "guest", "public", "sys", "INFORMATION_SCHEMA", "sysadmin", "serveradmin", "securityadmin",
            "setupadmin", "processadmin", "diskadmin", "dbcreator", "bulkadmin", "db_owner", "db_securityadmin", "db_accessadmin",
            "db_ddladmin", "db_backupoperator", "db_datareader", "db_datawriter", "db_denydatareader", "db_denydatawriter" }.Contains(value, StringComparer.OrdinalIgnoreCase);
    private static bool Token(string? value) => Matches(value, @"^[A-Za-z0-9][A-Za-z0-9_.@-]{0,127}$");
    private static bool Hash(string? value) => Matches(value, @"^[A-Fa-f0-9]{64}$") && value!.Any(character => character != '0');
    private static bool UtcWindow(DateTimeOffset start, DateTimeOffset end) => start.Offset == TimeSpan.Zero && end.Offset == TimeSpan.Zero && start < end;
    private static bool Matches(string? value, string pattern)
    {
        if (value is null) return false;
        var match = Regex.Match(value, pattern, RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
        // '$' alone can match before a final newline. Require consumption of the entire value.
        return match.Success && match.Index == 0 && match.Length == value.Length;
    }
    private static void Require([DoesNotReturnIf(false)] bool condition, string code)
    {
        if (!condition) throw new Phase7SqlGuardException(code);
    }
}
