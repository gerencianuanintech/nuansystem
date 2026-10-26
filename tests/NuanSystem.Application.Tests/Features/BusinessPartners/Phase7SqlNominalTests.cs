using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlNominalTests
{
    [Fact]
    public async Task S01_S03_pilot_and_legacy_have_equivalent_nominal_evidence()
    {
        var legacy = new Phase7NominalOfflineHarness(legacy: true);
        var pilot = new Phase7NominalOfflineHarness(legacy: false);
        var reference = await legacy.RunAsync();
        var actual = await pilot.RunAsync();
        Phase7SqlNominalScenarios.AssertEquivalent(reference, actual);
        Assert.Equal(3, actual.Count);
        Assert.NotEqual(reference[0].Partner.Id, actual[0].Partner.Id);
        Assert.NotEqual(reference[0].Partner.GlobalId, actual[0].Partner.GlobalId);
        Assert.Equal(actual[0].Partner.GlobalId, actual[2].Partner.GlobalId);
        Assert.True(actual[2].IsDeleted);
        Assert.False(actual[2].Partner.IsActive);
        Assert.Equal(3, pilot.Commits);
        Assert.Equal(new[] { "AfterCreate", "AfterEnqueue", "AfterUpdate", "AfterEnqueue", "AfterDelete", "AfterEnqueue" }, pilot.Checkpoints.Select(c => c.Label));
        Assert.Equal(new[] { 0, 0, 1, 1, 2, 2 }, pilot.Checkpoints.Select(c => c.CompletedCommits));
        Assert.NotNull(actual[0].Partner.Email);
        Assert.Null(actual[1].Partner.Email);
        Assert.Equal("Direccion editada", Assert.Single(actual[1].Partner.Addresses).Line1);
    }

    [Fact]
    public async Task Requested_clear_is_rejected_when_persistence_keeps_the_previous_value()
    {
        var harness = new Phase7NominalOfflineHarness(false, retainEmailOnUpdate: true);
        var error = await Assert.ThrowsAsync<Phase7SqlGuardException>(() => harness.RunAsync());
        Assert.Equal("PHASE7_SQL_COMMERCIAL_FIELDS_MISMATCH", error.ErrorCode);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task S04_business_writes_succeed_without_local_intention(bool isMaster, bool syncEnabled)
    {
        var harness = new Phase7NominalOfflineHarness(false, isMaster, syncEnabled);
        var snapshots = await harness.RunAsync();
        Assert.All(snapshots, snapshot => { Assert.Empty(snapshot.Events); Assert.Empty(snapshot.SyncAudits); });
        Assert.Equal(3, harness.Commits);
    }

    [Theory]
    [InlineData("commercial")]
    [InlineData("relationship")]
    [InlineData("audit")]
    [InlineData("event")]
    [InlineData("syncAudit")]
    [InlineData("identityDrift")]
    public async Task Equivalence_rejects_meaningful_differences(string change)
    {
        var reference = await new Phase7NominalOfflineHarness(true).RunAsync();
        var actual = (await new Phase7NominalOfflineHarness(false).RunAsync()).ToArray();
        switch (change)
        {
            case "commercial": actual[1].Partner.CreditLimit = 900; break;
            case "relationship": actual[1].Partner.Addresses = []; break;
            case "audit": actual[1] = actual[1] with { Audits = [] }; break;
            case "event": actual[1] = actual[1] with { Events = [] }; break;
            case "syncAudit": actual[1] = actual[1] with { SyncAudits = [] }; break;
            case "identityDrift": actual[1].Partner.GlobalId = Guid.NewGuid(); break;
        }
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlNominalScenarios.AssertEquivalent(reference, actual));
    }

    [Fact]
    public async Task Independent_generated_timestamps_do_not_break_business_equivalence()
    {
        var reference = await new Phase7NominalOfflineHarness(true).RunAsync();
        var actual = await new Phase7NominalOfflineHarness(false).RunAsync();
        foreach (var snapshot in actual) snapshot.Partner.CreatedAt = new DateTime(2026, 10, 24);
        Phase7SqlNominalScenarios.AssertEquivalent(reference, actual);
    }

    [Theory]
    [InlineData("sapCardCode")]
    [InlineData("sapCompanyCode")]
    [InlineData("connectionString")]
    [InlineData("password")]
    [InlineData("accessToken")]
    public void Nested_sensitive_payload_fields_are_rejected(string field)
    {
        var node = new System.Text.Json.Nodes.JsonObject { ["payload"] = new System.Text.Json.Nodes.JsonArray(new System.Text.Json.Nodes.JsonObject { [field] = "excluded" }) };
        Assert.Throws<Phase7SqlGuardException>(() => Phase7SqlNominalScenarios.AssertSafePayload(node));
    }
}
