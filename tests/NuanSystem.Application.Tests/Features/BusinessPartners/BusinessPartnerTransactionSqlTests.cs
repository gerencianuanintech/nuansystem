using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[CollectionDefinition("Phase7IsolatedSql",DisableParallelization=true)]
public sealed class Phase7IsolatedSqlCollection;

[Collection("Phase7IsolatedSql")]
[Trait("Category","SqlServerIntegration")]
[Trait("Scope","Phase7Isolated")]
public sealed class BusinessPartnerTransactionSqlTests
{
    // Discovery never reads a manifest or a credential. Each body independently rechecks authorization.
    private static Task Run(string id)=>Phase7SqlSuiteEntry.RunAsync(id,Phase7SqlGates.ReadEnvironment(),
        ()=>Phase7SqlSuiteLoader.FromEnvironment(id),default);
    [Phase7IsolatedSqlFact] public Task S01()=>Run("S01");
    [Phase7IsolatedSqlFact] public Task S02()=>Run("S02");
    [Phase7IsolatedSqlFact] public Task S03()=>Run("S03");
    [Phase7IsolatedSqlFact] public Task S04()=>Run("S04");
    [Phase7IsolatedSqlFact] public Task S05()=>Run("S05");
    [Phase7IsolatedSqlFact] public Task S06()=>Run("S06");
    [Phase7IsolatedSqlFact] public Task S07()=>Run("S07");
    [Phase7IsolatedSqlFact] public Task S08()=>Run("S08");
    [Phase7IsolatedSqlFact] public Task S09()=>Run("S09");
    [Phase7IsolatedSqlFact] public Task S10()=>Run("S10");
    [Phase7IsolatedSqlFact] public Task S11()=>Run("S11");
    [Phase7IsolatedSqlFact] public Task S12()=>Run("S12");
    [Phase7IsolatedSqlFact] public Task S13()=>Run("S13");
    [Phase7IsolatedSqlFact] public Task S14()=>Run("S14");
    [Phase7IsolatedSqlFact] public Task S15()=>Run("S15");
    [Phase7IsolatedSqlFact] public Task S16()=>Run("S16");
    [Phase7IsolatedSqlFact] public Task S17()=>Run("S17");
    [Phase7IsolatedSqlFact] public Task S18()=>Run("S18");
    [Phase7IsolatedSqlFact] public Task Equivalence()=>Run("Equivalence");
    [Phase7IsolatedSqlFact] public Task R01_Legacy()=>Run("R01.Legacy");
    [Phase7IsolatedSqlFact] public Task R01_Pilot()=>Run("R01.Pilot");
}
