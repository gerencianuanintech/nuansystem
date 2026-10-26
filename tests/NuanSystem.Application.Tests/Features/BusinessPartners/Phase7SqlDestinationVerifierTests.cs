using System.Data;
using NSubstitute;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlDestinationVerifierTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Approved_observation_uses_the_supplied_open_handles_and_timeout(bool observer)
    {
        var (manifest, target, sample) = Data(observer);
        var connection = Substitute.For<IDbConnection>();
        connection.State.Returns(ConnectionState.Open);
        var transaction = observer ? null : Substitute.For<IDbTransaction>();
        if (transaction is not null) transaction.Connection.Returns(connection);
        var calls = 0;
        var verifier = new Phase7SqlDestinationVerifier((c, t, timeout, ct) =>
        {
            Assert.Same(connection, c); Assert.Same(transaction, t);
            Assert.Equal(30, timeout); Assert.False(ct.IsCancellationRequested);
            calls++; return Task.FromResult(sample);
        });
        await verifier.VerifyAsync(manifest, target, observer, connection, transaction, default);
        Assert.Equal(1, calls);
        connection.DidNotReceive().Open(); connection.DidNotReceive().Close();
        connection.DidNotReceive().BeginTransaction(); connection.DidNotReceive().Dispose();
    }

    [Theory]
    [InlineData("server")][InlineData("version")][InlineData("database")][InlineData("id")]
    [InlineData("company")][InlineData("run")][InlineData("purpose")][InlineData("compatibility")]
    [InlineData("collation")][InlineData("rcsi")][InlineData("principal")][InlineData("login")]
    [InlineData("impersonation")][InlineData("privilege")][InlineData("metadata")]
    [InlineData("permissions")][InlineData("serverPermissions")][InlineData("objectPermissions")]
    [InlineData("schema")][InlineData("procedure")][InlineData("missingProcedure")]
    [InlineData("transaction")][InlineData("state")][InlineData("spid")][InlineData("markers")]
    public async Task Mismatches_fail_closed_without_exposing_observed_data(string mutation)
    {
        var (manifest, target, sample) = Data(false);
        sample = mutation switch
        {
            "server" => sample with { ServerName = "sensitive-wrong-server" },
            "version" => sample with { ProductVersion = "17.0.1.1" },
            "database" => sample with { DatabaseName = "production" },
            "id" => sample with { DatabaseId = 1 },
            "company" => sample with { CompanyId = 999 },
            "run" => sample with { RunMarker = Guid.NewGuid() },
            "purpose" => sample with { Purpose = "Production" },
            "compatibility" => sample with { CompatibilityLevel = 150 },
            "collation" => sample with { Collation = "different" },
            "rcsi" => sample with { ReadCommittedSnapshot = true },
            "principal" => sample with { DatabasePrincipal = "dbo" },
            "login" => sample with { Login = "sa" },
            "impersonation" => sample with { OriginalLogin = "other" },
            "privilege" => sample with { Privileged = true },
            "metadata" => sample with { CanViewDefinition = false },
            "permissions" => sample with { DatabasePermissions = ["SELECT", "INSERT", "EXECUTE", "DELETE"] },
            "serverPermissions" => sample with { ServerPermissions = ["CONTROL SERVER"] },
            "objectPermissions" => sample with { ObjectPermissions = ["ALTER"] },
            "schema" => sample with { SchemaSha256 = new('C', 64) },
            "procedure" => sample with { ProcedureSha256 = new Dictionary<string, string> { ["dbo.test"] = new('C',64) } },
            "missingProcedure" => sample with { ProcedureSha256 = new Dictionary<string, string>() },
            "transaction" => sample with { TransactionCount = 2 },
            "state" => sample with { TransactionState = -1 },
            "spid" => sample with { Spid = 0 },
            _ => sample with { HasFaultMarkers = true }
        };
        var connection = Substitute.For<IDbConnection>(); connection.State.Returns(ConnectionState.Open);
        var transaction = Substitute.For<IDbTransaction>(); transaction.Connection.Returns(connection);
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => Task.FromResult(sample));
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, false, connection, transaction, default));
        Assert.StartsWith("PHASE7_SQL_PREFLIGHT_", error.ErrorCode);
        Assert.DoesNotContain("sensitive", error.ToString()); Assert.Null(error.InnerException);
    }

    [Fact]
    public async Task Cancelled_or_closed_or_foreign_handles_never_call_the_reader()
    {
        var (manifest, target, _) = Data(false);
        var calls = 0;
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => { calls++; throw new Exception(); });
        var c = Substitute.For<IDbConnection>();
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, false, c, null, default));
        c.State.Returns(ConnectionState.Open);
        var t = Substitute.For<IDbTransaction>(); t.Connection.Returns(Substitute.For<IDbConnection>());
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, false, c, t, default));
        using var cancel = new CancellationTokenSource(); cancel.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => verifier.VerifyAsync(manifest, target, false, c, null, cancel.Token));
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task Read_errors_are_sanitized_and_cancellation_is_preserved()
    {
        var (manifest, target, _) = Data(true);
        var c = Substitute.For<IDbConnection>(); c.State.Returns(ConnectionState.Open);
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => throw new InvalidOperationException("Password=secret"));
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, true, c, null, default));
        Assert.DoesNotContain("secret", error.ToString());
        using var cancel = new CancellationTokenSource();
        verifier = new((_, _, _, ct) => { cancel.Cancel(); throw new OperationCanceledException(ct); });
        var cancelled = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => verifier.VerifyAsync(manifest, target, true, c, null, cancel.Token));
        Assert.Equal(cancel.Token, cancelled.CancellationToken);
    }

    [Fact]
    public async Task Observer_cannot_reuse_writer_observation_or_an_ambient_transaction()
    {
        var (manifest, target, sample) = Data(false);
        var c = Substitute.For<IDbConnection>(); c.State.Returns(ConnectionState.Open);
        var t = Substitute.For<IDbTransaction>(); t.Connection.Returns(c);
        var calls = 0;
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => { calls++; return Task.FromResult(sample); });
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, true, c, t, default));
        Assert.Equal(0, calls);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, true, c, null, default));
    }

    [Fact]
    public async Task Tenant_B_is_checked_against_B_and_foreign_target_objects_are_rejected()
    {
        var (manifest, target, sample) = Data(true);
        var c = Substitute.For<IDbConnection>(); c.State.Returns(ConnectionState.Open);
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => Task.FromResult(sample));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target with { }, true, c, null, default));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, manifest.TenantB, true, c, null, default));
        sample = sample with { DatabaseName = manifest.TenantB.DatabaseName, DatabaseId = 102, CompanyId = 102 };
        await verifier.VerifyAsync(manifest, manifest.TenantB, true, c, null, default);
    }

    [Fact]
    public async Task Cancellation_after_read_and_a_connection_closed_by_reader_are_rejected()
    {
        var (manifest, target, sample) = Data(true);
        var c = Substitute.For<IDbConnection>(); c.State.Returns(ConnectionState.Open);
        using var cancel = new CancellationTokenSource();
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => { cancel.Cancel(); return Task.FromResult(sample); });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => verifier.VerifyAsync(manifest, target, true, c, null, cancel.Token));
        verifier = new((_, _, _, _) => { c.State.Returns(ConnectionState.Closed); return Task.FromResult(sample); });
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, true, c, null, default));
    }

    [Theory]
    [InlineData("duplicatePermission")][InlineData("missingConnect")][InlineData("missingMetadataRight")]
    [InlineData("nullPermissions")][InlineData("nullObjects")][InlineData("nullProcedures")]
    [InlineData("emptyHash")][InlineData("zeroHash")][InlineData("invalidHash")]
    public async Task Incomplete_or_ambiguous_observations_are_rejected(string mutation)
    {
        var (manifest, target, sample) = Data(true);
        sample = mutation switch
        {
            "duplicatePermission" => sample with { DatabasePermissions = ["SELECT", "SELECT", "CONNECT", "VIEW DEFINITION"] },
            "missingConnect" => sample with { DatabasePermissions = ["SELECT", "VIEW DEFINITION"] },
            "missingMetadataRight" => sample with { DatabasePermissions = ["SELECT", "CONNECT"] },
            "nullPermissions" => sample with { DatabasePermissions = null! },
            "nullObjects" => sample with { ObjectPermissions = null! },
            "nullProcedures" => sample with { ProcedureSha256 = null! },
            "emptyHash" => sample with { SchemaSha256 = "" },
            "zeroHash" => sample with { SchemaSha256 = new('0',64) },
            _ => sample with { SchemaSha256 = new('Z',64) }
        };
        var c = Substitute.For<IDbConnection>(); c.State.Returns(ConnectionState.Open);
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => Task.FromResult(sample));
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => verifier.VerifyAsync(manifest, target, true, c, null, default));
    }

    [Fact]
    public async Task Default_reader_rejects_fake_providers_without_issuing_commands()
    {
        var (manifest, target, _) = Data(true);
        var c = Substitute.For<IDbConnection>(); c.State.Returns(ConnectionState.Open);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(() => new Phase7SqlDestinationVerifier()
            .VerifyAsync(manifest, target, true, c, null, default));
        c.DidNotReceive().CreateCommand(); c.DidNotReceive().Open();
    }

    [Fact]
    public void Fingerprints_use_original_utf16le_definition_without_bom_or_normalization()
    {
        Assert.Equal("E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855", Phase7SqlDestinationSqlReader.Fingerprint(""));
        Assert.NotEqual(Phase7SqlDestinationSqlReader.Fingerprint("a\r\nb"), Phase7SqlDestinationSqlReader.Fingerprint("a\nb"));
    }

    [Fact]
    public async Task Public_key_metadata_permissions_do_not_require_extra_business_grants()
    {
        var (manifest, target, sample) = Data(true);
        sample = sample with { DatabasePermissions = [..sample.DatabasePermissions,
            "VIEW ANY COLUMN MASTER KEY DEFINITION", "VIEW ANY COLUMN ENCRYPTION KEY DEFINITION"] };
        var c = Substitute.For<IDbConnection>(); c.State.Returns(ConnectionState.Open);
        var verifier = new Phase7SqlDestinationVerifier((_, _, _, _) => Task.FromResult(sample));
        await verifier.VerifyAsync(manifest, target, true, c, null, default);
    }

    [Fact]
    public async Task Sql2022_metadata_permissions_implied_by_view_definition_are_accepted_without_business_escalation()
    {
        var (manifest,target,sample)=Data(true);
        sample=sample with{DatabasePermissions=[..sample.DatabasePermissions,"VIEW SECURITY DEFINITION","VIEW PERFORMANCE DEFINITION"],
            ObjectPermissions=[..sample.ObjectPermissions,"VIEW SECURITY DEFINITION","VIEW PERFORMANCE DEFINITION"]};
        var c=Substitute.For<IDbConnection>();c.State.Returns(ConnectionState.Open);
        var verifier=new Phase7SqlDestinationVerifier((_,_,_,_)=>Task.FromResult(sample));
        await verifier.VerifyAsync(manifest,target,true,c,null,default);
    }

    [Fact]
    public async Task Metadata_subpermissions_do_not_authorize_database_state_or_DDL_permissions()
    {
        var (manifest,target,sample)=Data(true);
        sample=sample with{DatabasePermissions=[..sample.DatabasePermissions,"VIEW DATABASE STATE"]};
        var c=Substitute.For<IDbConnection>();c.State.Returns(ConnectionState.Open);
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>new Phase7SqlDestinationVerifier((_,_,_,_)=>Task.FromResult(sample)).VerifyAsync(manifest,target,true,c,null,default));
    }

    private static (Phase7SqlManifest, Phase7SqlTarget, Phase7SqlDestinationObservation) Data(bool observer)
    {
        var run = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        Phase7SqlTarget Target(string role, int id) => new(role, $"NuanSystem_Phase7_{run:N}_{role}", id, id, run,
            "BackendTransactionValidation", 160, "SQL_Latin1_General_CP1_CI_AS", false, new('A', 64),
            new Dictionary<string, string> { ["dbo.test"] = new('B',64) }, "p7_app", "p7_observer",
            ["SELECT", "EXECUTE", "INSERT"], ["SELECT"], "unused", "unused");
        var target = Target("A",101);
        var manifest = new Phase7SqlManifest(1, run, "operator", "exec", "prep", DateTimeOffset.UtcNow.AddHours(-1),
            DateTimeOffset.UtcNow.AddHours(1), 600, 30, new("lab", "LAB", "16.0.1000.6"), target, Target("B",102));
        var principal = observer ? target.ObserverPrincipal : target.ApplicationPrincipal;
        return (manifest, target, new("LAB", "16.0.1000.6", target.DatabaseName, 101, 101, run,
            target.Purpose, 160, target.Collation, false, principal, principal, principal, false, true,
            observer ? ["SELECT", "CONNECT", "VIEW DEFINITION"] : ["SELECT", "INSERT", "EXECUTE", "CONNECT", "VIEW DEFINITION"],
            ["CONNECT SQL", "VIEW ANY DATABASE"], ["SELECT"], target.SchemaSha256, target.ProcedureSha256,
            52, observer ? 0 : 1, observer ? 0 : 1, false));
    }
}
