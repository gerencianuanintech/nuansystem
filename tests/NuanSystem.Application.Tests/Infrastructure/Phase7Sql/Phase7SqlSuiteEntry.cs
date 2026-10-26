namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal interface IPhase7SqlExecutableRun:IDisposable
{
    Task<Phase7SqlCaseEvidence> ExecuteCaseAsync(string id,CancellationToken token);
}

internal static class Phase7SqlSuiteEntry
{
    public static async Task RunAsync(string id,Phase7SqlGates gates,Func<IPhase7SqlExecutableRun> create,CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if(!gates.StandardEnabled || !gates.IsolatedEnabled)throw new Phase7SqlGuardException("PHASE7_SQL_GATES_DISABLED");
        var spec=Phase7SqlExecutionCatalog.Cases.SingleOrDefault(c=>c.Id==id)
            ?? throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_CASE");
        try
        {
            using var runtime=create();
            var evidence=await runtime.ExecuteCaseAsync(id,token);
            token.ThrowIfCancellationRequested();
            if(evidence.Id!=id || evidence.Checks.Count!=spec.Checks.Count
                || spec.Checks.Any(c=>!evidence.Checks.TryGetValue(c,out var state) || state!=Phase7SqlEvidenceStatus.Passed))
                throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_EVIDENCE_INCOMPLETE");
        }
        catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}
        catch(Phase7SqlGuardException){throw;}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_SUITE_FAILED");}
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class Phase7IsolatedSqlFactAttribute:FactAttribute
{
    public Phase7IsolatedSqlFactAttribute()
    {
        var gates=Phase7SqlGates.ReadEnvironment();
        if(!gates.StandardEnabled || !gates.IsolatedEnabled)Skip="Requiere autorización y ambos gates SQL aislados. No ejecutar en regresión offline.";
    }
}
