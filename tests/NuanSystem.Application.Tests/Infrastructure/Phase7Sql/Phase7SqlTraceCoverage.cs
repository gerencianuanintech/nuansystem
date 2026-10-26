using System.Text.Json.Nodes;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlTraceRequirement(int WritesA,int WritesB,int StatesA,int StatesB,int NewScopesA,int NewScopesB);

internal static class Phase7SqlTraceCoverage
{
    public static Phase7SqlTraceRequirement Required(string id)=>id switch
    {
        "S01" or "S02" or "S03" or "Equivalence"=>new(3,3,0,0,0,0),
        "S04"=>new(12,0,0,0,0,0),
        "S05" or "S06" or "S07" or "S08" or "S09" or "S10" or "S11"=>new(5,0,0,0,0,0),
        "S12"=>new(1,1,1,1,0,0),"S13"=>new(2,0,2,0,2,0),"S14"=>new(3,0,3,0,0,0),
        "S15"=>new(2,0,2,0,2,0),"S16"=>new(3,0,3,0,0,0),"S17"=>new(1,0,1,0,1,0),
        "S18"=>new(6,0,6,0,1,0),"R01.Legacy" or "R01.Pilot"=>new(1,0,0,0,0,0),
        _=>throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_TRACE_CASE")
    };
    public static void Validate(IReadOnlyDictionary<string,Phase7SqlTracePair> after,IReadOnlyList<Phase7SqlCaseTiming> timings,
        IReadOnlyList<Phase7SqlEvidenceBundleFile> snapshots,Phase7SqlManifest manifest,Phase7SqlTrxInfo trx,bool finalAvailable)
    {
        void Require(bool value)=>Phase7SqlEvidenceFiles.Require(value,"TRACE_COVERAGE");
        Require(timings.Count==21 && timings.Select(t=>t.Id).Distinct().Count()==21 && timings.Select(t=>t.Sequence).Order().SequenceEqual(Enumerable.Range(1,21)));
        Phase7SqlTracePair? previous=null;DateTimeOffset previousFinish=trx.Start;
        foreach(var time in timings.OrderBy(t=>t.Sequence))
        {
            Require(time.StartedAtUtc.Offset==TimeSpan.Zero && time.FinishedAtUtc.Offset==TimeSpan.Zero
                && time.StartedAtUtc>=previousFinish && time.FinishedAtUtc>=time.StartedAtUtc && time.FinishedAtUtc<=trx.Finish
                && time.FinalSnapshotsCaptured==(time.Sequence==21) && (!time.FinalSnapshotsCaptured || finalAvailable)
                && after.ContainsKey(time.Id));
            var current=after[time.Id];var rule=Required(time.Id);
            if(previous is not null)Require(Equal(previous,time.Before));
            else Require(Empty(time.Before.A) && Empty(time.Before.B));
            var ids=new HashSet<Guid>();
            void Ids(Phase7SqlTraceNode node){Require(node.EvidenceScopeId!=Guid.Empty && ids.Add(node.EvidenceScopeId));foreach(var child in node.Independent)Ids(child);}
            Ids(current.A);Ids(current.B);
            foreach(var (role,beforeNode,afterNode,writes,states,scopes,target) in new[]{
                ("A",time.Before.A,current.A,rule.WritesA,rule.StatesA,rule.NewScopesA,manifest.TenantA),
                ("B",time.Before.B,current.B,rule.WritesB,rule.StatesB,rule.NewScopesB,manifest.TenantB)})
            {
                Phase7SqlGlobalControls.ValidateTrace(beforeNode,target);Phase7SqlGlobalControls.ValidateTrace(afterNode,target);
                Preserve(beforeNode,afterNode);
                Require(Writes(afterNode)-Writes(beforeNode)==writes && States(afterNode)-States(beforeNode)==states
                    && Scopes(afterNode)-Scopes(beforeNode)==scopes);
                var expectedObservers=1+snapshots.Count(s=>s.RelativePath.StartsWith(time.Id.Replace('.','_')+"/snapshot-"+role+"-",StringComparison.Ordinal))
                    +(time.FinalSnapshotsCaptured?1:0);
                Require(afterNode.ObserverConnectionCount-beforeNode.ObserverConnectionCount==expectedObservers);
                Require(Observers(afterNode)-Observers(beforeNode)==expectedObservers);
                if(scopes>0)
                {
                    var fresh=afterNode.Independent.Skip(beforeNode.Independent.Count).ToArray();
                    Require(fresh.Length==scopes && afterNode.TotalWriteConnections==beforeNode.TotalWriteConnections
                        && afterNode.ScopeStates.Count==beforeNode.ScopeStates.Count && fresh.All(n=>n.TotalWriteConnections==writes/scopes
                            && n.ScopeStates.Count==writes/scopes && n.Independent.Count==0 && n.ObserverConnectionCount==0));
                }
                else Require(afterNode.TotalWriteConnections-beforeNode.TotalWriteConnections==writes
                    && afterNode.ScopeStates.Count-beforeNode.ScopeStates.Count==states);
                if(time.Id=="S13" && role=="A")
                {
                    var fresh=afterNode.Independent.Skip(beforeNode.Independent.Count).ToArray();
                    Require(fresh.Length==2 && fresh.All(n=>n.ScopeStates.Count==1)
                        && fresh.SelectMany(n=>n.ScopeStates).Select(s=>s.Spid).Distinct().Count()==1);
                }
            }
            previous=current;previousFinish=time.FinishedAtUtc;
        }
    }
    private static int Writes(Phase7SqlTraceNode n)=>n.TotalWriteConnections+n.Independent.Sum(Writes);
    private static int States(Phase7SqlTraceNode n)=>n.ScopeStates.Count+n.Independent.Sum(States);
    private static int Observers(Phase7SqlTraceNode n)=>n.ObserverConnectionCount+n.Independent.Sum(Observers);
    private static int Scopes(Phase7SqlTraceNode n)=>1+n.Independent.Sum(Scopes);
    private static bool Empty(Phase7SqlTraceNode n)=>Writes(n)==0 && States(n)==0 && n.ObserverConnectionCount==0 && n.Independent.Count==0;
    private static bool Equal(object a,object b)=>JsonNode.DeepEquals(System.Text.Json.JsonSerializer.SerializeToNode(a),System.Text.Json.JsonSerializer.SerializeToNode(b));
    private static void Preserve(Phase7SqlTraceNode before,Phase7SqlTraceNode after)
    {
        Phase7SqlEvidenceFiles.Require(before.EvidenceScopeId==after.EvidenceScopeId && after.TotalWriteConnections>=before.TotalWriteConnections
            && after.ObserverConnectionCount>=before.ObserverConnectionCount && after.Samples.Take(before.Samples.Count).SequenceEqual(before.Samples)
            && after.ScopeStates.Take(before.ScopeStates.Count).SequenceEqual(before.ScopeStates) && after.Independent.Count>=before.Independent.Count,"TRACE_HISTORY_LOST");
        for(var i=0;i<before.Independent.Count;i++)Preserve(before.Independent[i],after.Independent[i]);
    }
}
