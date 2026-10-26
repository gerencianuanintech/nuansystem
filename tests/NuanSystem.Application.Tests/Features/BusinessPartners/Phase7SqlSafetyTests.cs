using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlSafetyTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 24, 12, 30, 0, TimeSpan.Zero);
    private static readonly Guid RunId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
    private static readonly Phase7SqlGates Enabled = new(true, true);

    [Fact]
    public void Valid_manifest_returns_only_an_offline_snapshot_requiring_online_verification()
    {
        var json = ValidJson();
        var result = Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now);
        Assert.True(result.RequiresOnlineVerification);
        Assert.Equal(RunId, result.Manifest.RunId);
        Assert.Equal(Database("A"), result.Manifest.TenantA.DatabaseName);
        Assert.Equal(Database("B"), result.Manifest.TenantB.DatabaseName);
        Assert.Equal(Hash(json), result.ManifestSha256);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Both_gates_are_required_even_for_an_approved_manifest(bool standard, bool isolated)
    {
        var json = ValidJson();
        Reject("PHASE7_SQL_GATES_DISABLED", () => Phase7SqlManifestValidator.Validate(json, Approval(json), new(standard, isolated), Now));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Missing_manifest_fails_closed(string? json) =>
        Reject("PHASE7_SQL_MANIFEST_REQUIRED", () => Phase7SqlManifestValidator.Validate(json, null, Enabled, Now));

    [Fact]
    public void Manifest_cannot_supply_its_own_approval()
    {
        Reject("PHASE7_SQL_APPROVAL_REQUIRED", () => Phase7SqlManifestValidator.Validate(ValidJson(), null, Enabled, Now));
        var json = Change("isAuthorized", JsonValue.Create(true));
        Reject("PHASE7_SQL_MANIFEST_INVALID", () => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Fact]
    public void Any_content_change_invalidates_the_independent_approval_hash()
    {
        var original = ValidJson();
        var changed = Change("operator", JsonValue.Create("different-operator"));
        Reject("PHASE7_SQL_APPROVAL_MISMATCH", () => Phase7SqlManifestValidator.Validate(changed, Approval(original), Enabled, Now));
    }

    [Theory]
    [InlineData("server.dataSource", "tcp:other-lab,1433")]
    [InlineData("server.expectedServerName", "OTHER-LAB")]
    [InlineData("tenantA.databaseName", "NuanSystem_Master")]
    [InlineData("tenantB.databaseName", "NuanSystem_DEMO")]
    [InlineData("tenantA.databaseName", "NuanSystem_Remigio")]
    [InlineData("tenantB.databaseName", "NuanSystem_Cañaris")]
    public void Destinations_must_match_the_approved_allowlist_not_just_a_lab_prefix(string path, string value)
    {
        var json = Change(path, JsonValue.Create(value));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Fact]
    public void Exchanging_database_roles_or_ids_is_rejected()
    {
        var root = JsonNode.Parse(ValidJson())!.AsObject();
        var a = root["tenantA"]!.DeepClone();
        root["tenantA"] = root["tenantB"]!.DeepClone();
        root["tenantB"] = a;
        var json = root.ToJsonString();
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
        json = Change("tenantA.databaseId", JsonValue.Create(102));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Theory]
    [InlineData("password", "SENSITIVE_VALUE")]
    [InlineData("connectionString", "Server=prod;Password=SENSITIVE_VALUE")]
    [InlineData("tenantA.passwordEncrypted", "SENSITIVE_VALUE")]
    [InlineData("server.dataSource", "Server=prod;User Id=sa;Password=SENSITIVE_VALUE")]
    [InlineData("tenantA.applicationSecretReference", "env:NUANSYSTEM_MASTER_CONNECTION_STRING")]
    [InlineData("tenantB.observerSecretReference", "SENSITIVE_VALUE")]
    public void Embedded_credentials_and_unapproved_secret_references_are_rejected_without_echoing_input(string path, string value)
    {
        var json = Change(path, JsonValue.Create(value));
        var error = Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
        Assert.DoesNotContain("SENSITIVE_VALUE", error.ToString());
        Assert.DoesNotContain("Server=prod", error.ToString());
        Assert.Null(error.InnerException);
    }

    [Theory]
    [InlineData("tenantA.purpose", "Production")]
    [InlineData("tenantA.runMarker", "00000000-0000-0000-0000-000000000000")]
    [InlineData("tenantB.schemaSha256", "not-a-hash")]
    [InlineData("tenantA.collation", "Latin1;DROP DATABASE")]
    [InlineData("tenantA.applicationPrincipal", "dbo")]
    [InlineData("tenantA.observerPrincipal", "p7_app")]
    public void Invalid_identity_schema_or_principal_metadata_is_rejected(string path, string value)
    {
        var json = Change(path, JsonValue.Create(value));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Fact]
    public void Manifest_is_strict_and_rejects_unknown_missing_duplicate_and_null_fields()
    {
        foreach (var json in new[] { "{}", "null", "{", Change("tenantA", null),
            ValidJson().Insert(1, "\"schemaVersion\":1,"), Change("SchemaVersion", JsonValue.Create(1)) })
            Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Theory]
    [InlineData(0, 30)]
    [InlineData(3601, 30)]
    [InlineData(60, 0)]
    [InlineData(60, 121)]
    [InlineData(10, 30)]
    public void Timeouts_must_be_positive_bounded_and_consistent(int runSeconds, int commandSeconds)
    {
        var root = JsonNode.Parse(ValidJson())!.AsObject();
        root["runTimeoutSeconds"] = runSeconds;
        root["commandTimeoutSeconds"] = commandSeconds;
        var json = root.ToJsonString();
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Theory]
    [InlineData(-61)]
    [InlineData(61)]
    [InlineData(30)]
    public void Authorization_window_must_cover_the_current_time_and_the_whole_run(int minuteOffset)
    {
        var json = ValidJson();
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now.AddMinutes(minuteOffset)));
    }

    [Fact]
    public void Expired_wrong_operator_or_missing_authorization_ids_are_rejected()
    {
        var json = ValidJson();
        foreach (var approval in new[] { Approval(json) with { Operator = "another" }, Approval(json) with { AuthorizationId = "" },
            Approval(json) with { ExpiresAtUtc = Now.AddSeconds(-1) }, Approval(json) with { ManifestSha256 = "invalid" } })
            Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, approval, Enabled, Now));
    }

    [Fact]
    public void DDL_privileges_and_writing_observers_are_not_permitted()
    {
        foreach (var path in new[] { "tenantA.applicationPermissions", "tenantB.observerPermissions" })
        {
            var json = Change(path, new JsonArray("SELECT", "EXECUTE", "INSERT", "ALTER"));
            Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
        }
    }

    [Fact]
    public void Unbounded_input_and_missing_procedure_fingerprints_are_rejected()
    {
        var json = new string(' ', 65_537);
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
        json = Change("tenantA.procedureSha256", new JsonObject());
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Theory]
    [InlineData(null, null, false, false)]
    [InlineData("0", "0", false, false)]
    [InlineData("1", "true", true, true)]
    [InlineData("TRUE", "1", true, true)]
    [InlineData("yes", " 1 ", false, false)]
    public void Gate_parsing_reuses_the_existing_explicit_enablement_contract(string? standard, string? isolated, bool a, bool b)
    {
        var gates = Phase7SqlGates.FromValues(standard, isolated);
        Assert.Equal(a, gates.StandardEnabled);
        Assert.Equal(b, gates.IsolatedEnabled);
    }

    [Theory]
    [InlineData("operator")]
    [InlineData("server.dataSource")]
    [InlineData("server.expectedServerName")]
    [InlineData("tenantA.applicationPrincipal")]
    [InlineData("tenantA.schemaSha256")]
    [InlineData("tenantA.collation")]
    public void Trailing_line_breaks_are_not_valid_identifier_or_hash_characters(string path)
    {
        var original = JsonNode.Parse(ValidJson())!;
        var parts = path.Split('.');
        var node = original;
        foreach (var part in parts) node = node[part]!;
        var value = node.GetValue<string>() + "\n";
        var json = Change(path, JsonValue.Create(value));
        var approval = Approval(json);
        if (path == "operator") approval = approval with { Operator = value };
        if (path == "server.dataSource") approval = approval with { DataSource = value };
        if (path == "server.expectedServerName") approval = approval with { ExpectedServerName = value };
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, approval, Enabled, Now));
    }

    [Theory]
    [InlineData("db_owner")]
    [InlineData("sysadmin")]
    [InlineData("db_ddladmin")]
    public void Administrative_role_names_are_not_application_principals(string role)
    {
        var json = Change("tenantA.applicationPrincipal", JsonValue.Create(role));
        Reject("PHASE7_SQL_PRINCIPAL_INVALID", () => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Theory]
    [InlineData("a", 65_537)]
    [InlineData("é", 40_000)]
    public void Payload_limit_is_enforced_in_UTF8_bytes(string character, int count)
    {
        var json = Change("operator", JsonValue.Create(string.Concat(Enumerable.Repeat(character, count))));
        Reject("PHASE7_SQL_MANIFEST_INVALID", () => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Fact]
    public void Validated_permissions_and_fingerprints_cannot_be_mutated()
    {
        var json = ValidJson();
        var result = Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now);
        Assert.Throws<NotSupportedException>(() => ((IList<string>)result.Manifest.TenantA.ApplicationPermissions).Add("ALTER"));
        Assert.Throws<NotSupportedException>(() => ((IDictionary<string, string>)result.Manifest.TenantA.ProcedureSha256).Clear());
    }

    [Fact]
    public void Example_manifest_is_inert_and_cannot_be_approved_as_a_test_destination()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln"))) directory = directory.Parent;
        Assert.NotNull(directory);
        var json = File.ReadAllText(Path.Combine(directory.FullName, "tests", "NuanSystem.Application.Tests", "Fixtures", "Phase7Sql", "manifest.example.json"));
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
        Assert.DoesNotContain("password", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Colliding_tenants_are_rejected_even_if_the_approval_repeats_the_same_database_id()
    {
        var json = Change("tenantB.databaseId", JsonValue.Create(101));
        var approval = Approval(json);
        approval = approval with { TenantB = approval.TenantB with { DatabaseId = 101 } };
        Reject("PHASE7_SQL_TARGET_COLLISION", () => Phase7SqlManifestValidator.Validate(json, approval, Enabled, Now));
    }

    [Fact]
    public void Empty_or_case_ambiguous_procedure_names_are_not_accepted()
    {
        var root = JsonNode.Parse(ValidJson())!;
        var procedures = root["tenantA"]!["procedureSha256"]!.AsObject();
        procedures["dbo.sp_na_post_businesspartners_crear"] = new string('C', 64);
        var json = root.ToJsonString();
        Reject("PHASE7_SQL_SCHEMA_INVALID", () => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Theory]
    [InlineData("server.dataSource", "tcp:phase7-lab.example.test,65536")]
    [InlineData("server.productVersion", "16.0.1000.6\n")]
    [InlineData("server.productVersion", "16.0.1000.6 ")]
    [InlineData("server.dataSource", "phase7-lab/path")]
    public void Endpoint_and_version_formats_are_not_silently_normalized(string path, string value)
    {
        var json = Change(path, JsonValue.Create(value));
        Reject("PHASE7_SQL_SERVER_INVALID", () => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    [Fact]
    public void UTC_is_required_and_approval_windows_cannot_be_extended_by_the_manifest()
    {
        var json = ValidJson();
        Reject("PHASE7_SQL_WINDOW_INVALID", () => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now.ToOffset(TimeSpan.FromHours(1))));
        json = Change("expiresAtUtc", JsonValue.Create(Now.AddHours(1)));
        Reject("PHASE7_SQL_WINDOW_INVALID", () => Phase7SqlManifestValidator.Validate(json, Approval(json), Enabled, Now));
    }

    private static void Reject(string code, Action action) => Assert.Equal(code, Assert.Throws<Phase7SqlGuardException>(action).ErrorCode);
    private static string Hash(string json) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
    private static string Database(string role) => $"NuanSystem_Phase7_{RunId:N}_{role}";
    private static Phase7SqlApproval Approval(string json) => new("AUTH-EXEC", "AUTH-PREP", "test-operator", RunId, Hash(json),
        "tcp:phase7-lab.example.test,1433", "PHASE7-LAB", new("A", Database("A"), 101, 11), new("B", Database("B"), 102, 12),
        Now.AddMinutes(-30), Now.AddMinutes(30));

    private static string Change(string path, JsonNode? value)
    {
        var root = JsonNode.Parse(ValidJson())!;
        var parts = path.Split('.');
        var parent = root;
        foreach (var part in parts[..^1]) parent = parent[part]!;
        parent[parts[^1]] = value;
        return root.ToJsonString();
    }

    private static string ValidJson()
    {
        object Target(string role, int databaseId, int companyId) => new
        {
            role, databaseName = Database(role), databaseId, companyId, runMarker = RunId, purpose = "BackendTransactionValidation",
            compatibilityLevel = 160, collation = "SQL_Latin1_General_CP1_CI_AS", readCommittedSnapshot = false,
            schemaSha256 = new string('A', 64),
            procedureSha256 = new Dictionary<string, string>
            {
                ["dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR"] = new('B', 64),
                ["dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR"] = new('B', 64),
                ["dbo.SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR"] = new('B', 64),
                ["dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID"] = new('B', 64),
                ["dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORCODIGO"] = new('B', 64),
                ["dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION"] = new('B', 64)
            },
            applicationPrincipal = "p7_app", observerPrincipal = "p7_observer",
            applicationPermissions = new[] { "SELECT", "EXECUTE", "INSERT" }, observerPermissions = new[] { "SELECT" },
            applicationSecretReference = $"env:NUANSYSTEM_PHASE7_TENANT_{role}_CONNECTION_STRING",
            observerSecretReference = $"env:NUANSYSTEM_PHASE7_OBSERVER_{role}_CONNECTION_STRING"
        };
        return JsonSerializer.Serialize(new
        {
            schemaVersion = 1, runId = RunId, @operator = "test-operator", executionAuthorizationId = "AUTH-EXEC", provisioningAuthorizationId = "AUTH-PREP",
            notBeforeUtc = Now.AddMinutes(-30), expiresAtUtc = Now.AddMinutes(30), runTimeoutSeconds = 600, commandTimeoutSeconds = 30,
            server = new { dataSource = "tcp:phase7-lab.example.test,1433", expectedServerName = "PHASE7-LAB", productVersion = "16.0.1000.6" },
            tenantA = Target("A", 101, 11), tenantB = Target("B", 102, 12)
        });
    }
}
