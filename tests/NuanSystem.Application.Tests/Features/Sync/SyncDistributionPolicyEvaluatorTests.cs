using System.Text.Json;
using FluentAssertions;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncDistributionPolicyEvaluatorTests
{
    private readonly SyncDistributionPolicyEvaluator _sut = new();

    [Fact]
    public void All_MatchesAnyRecord()
    {
        var decision = _sut.Evaluate(Target("All"), Context("20"));

        decision.Matched.Should().BeTrue();
    }

    [Fact]
    public void Selected_OnlyMatchesSelectedGlobalId()
    {
        _sut.Evaluate(Target("Selected", isSelected: true), Context("20")).Matched.Should().BeTrue();
        _sut.Evaluate(Target("Selected", isSelected: false), Context("20")).Matched.Should().BeFalse();
    }

    [Fact]
    public void Rule_MatchesWarehouseCode()
    {
        const string rule = """
        {"match":"All","conditions":[{"field":"code","operator":"Equals","value":"20"}]}
        """;

        var decision = _sut.Evaluate(Target("Rule", rule), Context("20"));

        decision.Matched.Should().BeTrue();
    }

    [Fact]
    public void Rule_RejectsUnknownField()
    {
        const string rule = """
        {"match":"All","conditions":[{"field":"sqlTable","operator":"Equals","value":"OWHS"}]}
        """;

        var decision = _sut.Evaluate(Target("Rule", rule), Context("20"));

        decision.Matched.Should().BeFalse();
        decision.Reason.Should().Contain("no autorizado");
    }

    private static SyncRoutingTargetDto Target(string mode, string? rule = null, bool isSelected = false) => new(
        1, 2, "PILOT", 1, 10, "Warehouse", 100, 3, 30, 5,
        true, true, true, false, 99, mode, "KeepInMaster", rule, 1, isSelected);

    private static SyncRoutingContext Context(string code)
    {
        var payload = JsonSerializer.Serialize(new { payload = new { code, isActive = true } });
        return new SyncRoutingContext(1, "Warehouse", EntityGlobalId: Guid.NewGuid(), PayloadJson: payload);
    }
}
