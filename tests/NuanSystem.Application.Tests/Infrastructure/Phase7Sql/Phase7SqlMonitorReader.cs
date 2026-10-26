using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.Data.SqlClient;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlMonitorApproval(Guid RunId,string AuthorizationReference,string Principal,
    DateTimeOffset NotBeforeUtc,DateTimeOffset ExpiresAtUtc);

// Independent parent-side monitor, separate permission/approval. No opener, credential resolver, kill or cleanup API.
internal static class Phase7SqlMonitorReader
{
    public static string ApplicationName(Guid run,string role,bool observer)=>$"NuanSystem.Phase7.{run:N}.{role}.{(observer?"O":"W")}";
    public static async Task<Phase7SqlMonitorObservation> ReadAsync(IDbConnection connection,Phase7SqlManifest manifest,
        Phase7SqlMonitorApproval approval,DateTimeOffset now,int timeout,CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var actualStart=DateTimeOffset.UtcNow;
        Phase7SqlEvidenceFiles.Require(connection is SqlConnection && connection.State==ConnectionState.Open && connection.Database=="master"
            && timeout is >=1 and <=120 && approval.RunId==manifest.RunId && now.Offset==TimeSpan.Zero
            && approval.NotBeforeUtc.Offset==TimeSpan.Zero && approval.ExpiresAtUtc.Offset==TimeSpan.Zero
            && now>=approval.NotBeforeUtc && now<approval.ExpiresAtUtc && !string.IsNullOrWhiteSpace(approval.AuthorizationReference)
            && actualStart>=approval.NotBeforeUtc && actualStart<approval.ExpiresAtUtc
            && !new[]{"sa","dbo",manifest.TenantA.ApplicationPrincipal,manifest.TenantB.ApplicationPrincipal,
                manifest.TenantA.ObserverPrincipal,manifest.TenantB.ObserverPrincipal}.Contains(approval.Principal,StringComparer.OrdinalIgnoreCase),"MONITOR_SCOPE");
        foreach(var target in new[]{manifest.TenantA,manifest.TenantB})
            Phase7SqlEvidenceFiles.Require(target.DatabaseName==$"NuanSystem_Phase7_{manifest.RunId:N}_{target.Role}"
                && target.Role is "A" or "B" && target.DatabaseId>4,"MONITOR_TARGET");
        try
        {
            var identity=await connection.QuerySingleAsync<Identity>(new CommandDefinition("""
                SELECT CONVERT(nvarchar(128),SERVERPROPERTY('ServerName')) ServerName,
                  CONVERT(nvarchar(128),SERVERPROPERTY('ProductVersion')) ProductVersion,
                  SUSER_SNAME() Login,ORIGINAL_LOGIN() OriginalLogin,@@TRANCOUNT Transactions,XACT_STATE() State,
                  CONVERT(int,COALESCE(IS_SRVROLEMEMBER('sysadmin'),1)) IsSysadmin,
                  CONVERT(int,CASE WHEN COALESCE(HAS_PERMS_BY_NAME(NULL,'SERVER','VIEW SERVER STATE'),0)=1
                    OR COALESCE(HAS_PERMS_BY_NAME(NULL,'SERVER','VIEW SERVER PERFORMANCE STATE'),0)=1 THEN 1 ELSE 0 END) HasVisibility;
                """,commandTimeout:timeout,cancellationToken:token));
            Phase7SqlEvidenceFiles.Require(identity.ServerName==manifest.Server.ExpectedServerName && identity.ProductVersion==manifest.Server.ProductVersion
                && identity.Login==approval.Principal && identity.OriginalLogin==approval.Principal && identity.IsSysadmin==0
                && identity.Transactions==0 && identity.State==0 && identity.HasVisibility==1,"MONITOR_IDENTITY");
            var names=new[]{"A","B"}.SelectMany(role=>new[]{ApplicationName(manifest.RunId,role,false),ApplicationName(manifest.RunId,role,true)}).ToArray();
            var sessions=await connection.QuerySingleAsync<int>(new CommandDefinition("SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE is_user_process=1 AND program_name IN @Names",new{Names=names},commandTimeout:timeout,cancellationToken:token));
            var retained=new List<Phase7SqlRetainedDatabase>();var transactions=new List<long>();
            foreach(var target in new[]{manifest.TenantA,manifest.TenantB})
            {
                var tx=await connection.QuerySingleAsync<long>(new CommandDefinition("SELECT COUNT_BIG(*) FROM sys.dm_tran_database_transactions WHERE database_id=@Id AND database_transaction_type IN (1,2)",new{Id=target.DatabaseId},commandTimeout:timeout,cancellationToken:token));
                transactions.Add(tx);
                var prefix=$"[{target.DatabaseName}]"; // derived only from validated full RunId + closed role whitelist
                var markers=await connection.QuerySingleAsync<Markers>(new CommandDefinition($"""
                    SELECT name DatabaseName,database_id DatabaseId,
                      (SELECT TRY_CONVERT(uniqueidentifier,CONVERT(nvarchar(128),value)) FROM {prefix}.sys.extended_properties WHERE class=0 AND name=N'Phase7.RunId') RunMarker,
                      (SELECT CONVERT(nvarchar(128),value) FROM {prefix}.sys.extended_properties WHERE class=0 AND name=N'Phase7.Purpose') Purpose
                    FROM sys.databases WHERE name=@Name AND database_id=@Id AND state=0;
                    """,new{Name=target.DatabaseName,Id=target.DatabaseId},commandTimeout:timeout,cancellationToken:token));
                var query="SELECT (SELECT "+string.Join(",",Phase7SqlRollbackSnapshot.RequiredTables.Select(table=>
                    $"JSON_QUERY((SELECT * FROM {prefix}.dbo.[{table}] ORDER BY [{Phase7SqlSnapshotSqlReader.PrimaryKey(table)}] FOR JSON PATH,INCLUDE_NULL_VALUES)) [{table}]"))+" FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)";
                var raw=await connection.QuerySingleAsync<string>(new CommandDefinition(query,commandTimeout:timeout,cancellationToken:token));
                Phase7SqlEvidenceFiles.Require(raw.Length<=8*1024*1024,"MONITOR_SNAPSHOT_SIZE");
                var bytes=System.Text.Encoding.UTF8.GetBytes(raw);Phase7SqlGlobalControls.ValidatePhysicalSnapshot(bytes);
                var snapshot=System.Text.Json.Nodes.JsonNode.Parse(bytes)!.AsObject();
                var counts=Phase7SqlRollbackSnapshot.RequiredTables.ToDictionary(table=>table,table=>(long)snapshot[table]!.AsArray().Count);
                retained.Add(new(target.Role,markers.DatabaseName,markers.DatabaseId,markers.RunMarker,markers.Purpose,counts,Phase7SqlEvidenceFiles.Hash(bytes)));
            }
            token.ThrowIfCancellationRequested();
            var observed=DateTimeOffset.UtcNow;
            Phase7SqlEvidenceFiles.Require(observed>=approval.NotBeforeUtc && observed<approval.ExpiresAtUtc,"MONITOR_WINDOW_EXPIRED");
            return new(manifest.RunId,identity.ServerName,identity.ProductVersion,approval.Principal,approval.AuthorizationReference,
                observed,sessions,transactions[0],transactions[1],retained);
        }
        catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}
        catch(Phase7SqlGuardException){throw;}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_MONITOR_READ");}
    }
    private sealed record Identity(string ServerName,string ProductVersion,string Login,string OriginalLogin,int Transactions,int State,int IsSysadmin,int HasVisibility);
    private sealed record Markers(string DatabaseName,int DatabaseId,Guid RunMarker,string Purpose);

    public static Phase7SqlIndependentWitness Witness(Process child,Guid run,string inspector,string authorization,
        DateTimeOffset workersOffFrom,DateTimeOffset workersOffThrough,IReadOnlyList<string> components)
    {
        // Does not launch, wait for or terminate a process. Parent must already own its controlled test child.
        var exited=child.HasExited;
        return new(run,child.Id,exited,exited?child.ExitCode:null,exited?DateTimeOffset.UtcNow:null,inspector,authorization,
            workersOffFrom,workersOffThrough,components.ToArray(),Phase7SqlEvidenceOrigin.SqlServerLaboratory);
    }
}
