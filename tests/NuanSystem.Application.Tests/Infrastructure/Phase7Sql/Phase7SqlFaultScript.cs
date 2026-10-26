using System.Security.Cryptography;
using System.Text;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlFaultInstallation(string Sql, string Sha256);

// Pure renderer, not a provisioner. Output is never sent to a connection by this test harness.
internal static class Phase7SqlFaultScript
{
    public static Phase7SqlFaultInstallation Render(Guid runId, string role, int companyId)
    {
        if (runId == Guid.Empty || role is not ("A" or "B") || companyId <= 0)
            throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SCRIPT_TARGET");
        var database = $"NuanSystem_Phase7_{runId:N}_{role}";
        var prefix = ($"P7_{runId:N}"[..11] + "_").ToUpperInvariant();
        var identity = $"DB_NAME()=N'{database}' AND DB_ID()>4 AND " +
            $"TRY_CONVERT(uniqueidentifier,(SELECT TRY_CONVERT(nvarchar(128),value) FROM sys.extended_properties WHERE class=0 AND name=N'Phase7.RunId'))='{runId:D}' AND " +
            "(SELECT TRY_CONVERT(nvarchar(128),value) FROM sys.extended_properties WHERE class=0 AND name=N'Phase7.Purpose')=N'BackendTransactionValidation'";
        var definitions = new List<(string Name, string Table, string Sql)>();
        void Add(string suffix, string table, string point, int number, string rowQuery)
        {
            var name = $"TR_P7_{runId:N}_{suffix}";
            var sql = $"""
CREATE TRIGGER dbo.[{name}] ON dbo.[{table}] AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS(SELECT 1 WHERE {identity}
        AND TRY_CONVERT(uniqueidentifier,TRY_CONVERT(nvarchar(128),SESSION_CONTEXT(N'P7_RunId')))='{runId:D}'
        AND TRY_CONVERT(int,TRY_CONVERT(nvarchar(32),SESSION_CONTEXT(N'P7_CompanyId')))={companyId}
        AND TRY_CONVERT(nvarchar(40),SESSION_CONTEXT(N'P7_FaultPoint'))=N'{point}') RETURN;
    IF EXISTS({rowQuery})
        THROW {number}, N'PHASE7_SQL_{point.ToUpperInvariant()}', 1;
END;
""";
            definitions.Add((name, table, sql));
        }
        Add("Audit", "AuditCatalogChanges", "AuditInsert", 51505,
            $"SELECT 1 FROM inserted i INNER JOIN dbo.BusinessPartners b ON i.RecordId=CONVERT(nvarchar(80),b.Id) WHERE i.EntityName=N'BusinessPartners' AND LEFT(b.Code,{prefix.Length})=N'{prefix}' AND b.Code=TRY_CONVERT(nvarchar(50),SESSION_CONTEXT(N'P7_CaseCode'))");
        Add("Outbox", "LocalOutbox", "OutboxInsert", 51506,
            $"SELECT 1 FROM inserted i WHERE i.CompanyId={companyId} AND i.EntityName=N'BusinessPartner' AND LEFT(i.EntityCode,{prefix.Length})=N'{prefix}' AND i.EntityCode=TRY_CONVERT(nvarchar(50),SESSION_CONTEXT(N'P7_CaseCode'))");
        Add("SyncAudit", "SyncAudit", "SyncAuditInsert", 51507,
            $"SELECT 1 FROM inserted i INNER JOIN dbo.LocalOutbox o ON o.EventId=i.EventId AND o.CompanyId=i.CompanyId AND o.EntityGlobalId=i.EntityGlobalId WHERE i.CompanyId={companyId} AND i.EntityName=N'BusinessPartner' AND i.[Action]=N'Created' AND LEFT(o.EntityCode,{prefix.Length})=N'{prefix}' AND o.EntityCode=TRY_CONVERT(nvarchar(50),SESSION_CONTEXT(N'P7_CaseCode'))");

        var sql = new StringBuilder($"""
-- LAB ONLY. Requires separate provisioning authorization and a verified dedicated destination.
-- Tenant prerequisite objects: effective 024/028/042/065/124/126 and their dependencies.
-- Never register this file in application initialization or database/sql deployment history.
SET NOCOUNT ON;
SET XACT_ABORT ON;
IF @@TRANCOUNT<>0 THROW 51500, N'PHASE7_SQL_INSTALL_AMBIENT_TRANSACTION', 1;
IF NOT EXISTS(SELECT 1 WHERE {identity}) THROW 51500, N'PHASE7_SQL_INSTALL_TARGET_REJECTED', 1;

""");
        foreach (var item in definitions)
        {
            sql.AppendLine($"IF OBJECT_ID(N'dbo.{item.Table}',N'U') IS NULL THROW 51500,N'PHASE7_SQL_PREREQUISITE_MISSING',1;");
            sql.AppendLine($"IF OBJECT_ID(N'dbo.{item.Name}') IS NOT NULL THROW 51500,N'PHASE7_SQL_TRIGGER_ALREADY_EXISTS',1;");
        }
        sql.AppendLine("BEGIN TRY\nBEGIN TRANSACTION;");
        foreach (var item in definitions) sql.AppendLine($"EXEC(N'{item.Sql.Replace("'", "''")}');");
        sql.AppendLine("COMMIT TRANSACTION;\nEND TRY\nBEGIN CATCH\nIF XACT_STATE()<>0 ROLLBACK TRANSACTION;\nTHROW;\nEND CATCH;");
        var rendered = sql.ToString();
        return new(rendered, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rendered))));
    }
}
