using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category","OfflineSafety")]
public sealed class Phase7SqlExecutableSuiteTests
{
    [Theory]
    [InlineData(false,false)][InlineData(true,false)][InlineData(false,true)]
    public async Task Disabled_gates_never_construct_the_runtime_or_read_secrets(bool standard,bool isolated)
    {
        var created=0;
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>Phase7SqlSuiteEntry.RunAsync("S01",new(standard,isolated),()=>{created++;throw new Exception();},default));
        Assert.Equal(0,created);
    }

    public static IEnumerable<object[]> Cases()=>Phase7SqlExecutionCatalog.Cases.Select(c=>new object[]{c.Id});
    [Theory][MemberData(nameof(Cases))]
    public async Task Every_catalog_case_is_dispatched_once_and_runtime_is_disposed(string id)
    {
        var run=new FakeRun();
        await Phase7SqlSuiteEntry.RunAsync(id,new(true,true),()=>run,default);
        Assert.Equal(new[]{id},run.Calls);Assert.True(run.Disposed);
    }

    [Fact]
    public async Task Unknown_cases_and_cancellation_do_not_create_a_runtime()
    {
        var created=0;IPhase7SqlExecutableRun Create(){created++;return new FakeRun();}
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>Phase7SqlSuiteEntry.RunAsync("S99",new(true,true),Create,default));
        using var cancel=new CancellationTokenSource();cancel.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>Phase7SqlSuiteEntry.RunAsync("S01",new(true,true),Create,cancel.Token));
        Assert.Equal(0,created);
    }

    [Theory][InlineData("missing")][InlineData("failed")][InlineData("wrongId")]
    public async Task Incomplete_evidence_never_passes_a_SQL_fact(string change)
    {
        var run=new FakeRun { Change=change };
        await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>Phase7SqlSuiteEntry.RunAsync("S01",new(true,true),()=>run,default));
        Assert.True(run.Disposed);
    }

    [Fact]
    public void Executable_methods_match_the_approved_catalog_and_are_SQL_only()
    {
        var type=typeof(BusinessPartnerTransactionSqlTests);
        var methods=type.GetMethods().Where(m=>m.GetCustomAttributes(typeof(Phase7IsolatedSqlFactAttribute),false).Length>0).ToArray();
        Assert.Equal(21,methods.Length);
        Assert.Equal(Phase7SqlExecutionCatalog.Cases.Select(c=>c.Method).Order(),methods.Select(m=>m.Name).Order());
        Assert.Equal(Phase7SqlExecutionCatalog.ExpectedTestClass,type.FullName);
        Assert.NotNull(new Phase7IsolatedSqlFactAttribute().Skip); // actual task process gates are0
    }

    [Fact]
    public async Task Unexpected_runtime_errors_are_sanitized_and_disposed()
    {
        var run=new FakeRun { Failure=new Exception("Password=secret") };
        var error=await Assert.ThrowsAsync<Phase7SqlGuardException>(()=>Phase7SqlSuiteEntry.RunAsync("S01",new(true,true),()=>run,default));
        Assert.DoesNotContain("secret",error.ToString());Assert.Null(error.InnerException);Assert.True(run.Disposed);
    }

    private sealed class FakeRun:IPhase7SqlExecutableRun
    {
        public List<string> Calls=[];public bool Disposed;public string? Change;public Exception? Failure;
        public Task<Phase7SqlCaseEvidence> ExecuteCaseAsync(string id,CancellationToken token)
        {
            Calls.Add(id);if(Failure is not null)throw Failure;
            var checks=Phase7SqlExecutionCatalog.Cases.Single(c=>c.Id==id).Checks.ToDictionary(c=>c,_=>Phase7SqlEvidenceStatus.Passed);
            if(Change=="missing")checks.Remove(checks.Keys.First());
            if(Change=="failed")checks[checks.Keys.First()]=Phase7SqlEvidenceStatus.Failed;
            return Task.FromResult(new Phase7SqlCaseEvidence(Change=="wrongId"?"S02":id,checks));
        }
        public void Dispose()=>Disposed=true;
    }
}
