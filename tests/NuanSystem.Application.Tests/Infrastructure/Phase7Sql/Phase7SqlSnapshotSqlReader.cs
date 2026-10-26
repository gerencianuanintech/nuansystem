using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal static class Phase7SqlSnapshotSqlReader
{
    // Names are a closed internal whitelist. All destination/marker values are parameters.
    internal static string Query { get; } = "SELECT (SELECT " + string.Join(",\n",Phase7SqlRollbackSnapshot.RequiredTables.Select(table=>
        $"JSON_QUERY((SELECT * FROM dbo.[{table}] ORDER BY [{PrimaryKey(table)}] FOR JSON PATH,INCLUDE_NULL_VALUES)) [{table}]")) + """

        FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES)
        WHERE DB_NAME()=@DatabaseName AND DB_ID()=@DatabaseId AND DB_ID()>4 AND @@TRANCOUNT=0 AND XACT_STATE()=0
          AND (SELECT TRY_CONVERT(uniqueidentifier,CONVERT(nvarchar(128),value)) FROM sys.extended_properties WHERE class=0 AND name=N'Phase7.RunId')=@RunId
          AND (SELECT CONVERT(nvarchar(128),value) FROM sys.extended_properties WHERE class=0 AND name=N'Phase7.Purpose')=@Purpose
          AND (SELECT TRY_CONVERT(int,value) FROM sys.extended_properties WHERE class=0 AND name=N'Phase7.CompanyId')=@CompanyId;
        """;

    public static async Task<string> ReadAsync(IDbConnection connection,Phase7SqlManifest manifest,Phase7SqlTarget target,
        int timeoutSeconds,CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if(connection is not SqlConnection || connection.State!=ConnectionState.Open)
            throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_PROVIDER");
        return await connection.QuerySingleAsync<string>(new CommandDefinition(Query,new {
            target.DatabaseName,target.DatabaseId,RunId=manifest.RunId,target.Purpose,target.CompanyId },
            commandTimeout:timeoutSeconds,cancellationToken:token));
    }

    internal static string PrimaryKey(string table)=>table switch
    {
        "BusinessPartnerBankAccounts"=>"BusinessPartnerBankAccountId",
        "BusinessPartnerRetentionSettings"=>"BusinessPartnerRetentionSettingId",
        "BusinessPartnerSapFieldMappings"=>"BusinessPartnerSapFieldMappingId",
        "BusinessPartnerAttachmentsMetadata"=>"BusinessPartnerAttachmentId",
        "BusinessPartnerFiscalData" or "BusinessPartnerAccountingSettings" or "BusinessPartnerCreditSettings"
            or "BusinessPartnerPurchaseSettings" or "BusinessPartnerSapMapping" or "BusinessPartnerNotes"=>"BusinessPartnerId",
        "BusinessPartners" or "BusinessPartnerAddresses" or "BusinessPartnerContacts" or "AuditCatalogChanges" or "LocalOutbox" or "SyncAudit"=>"Id",
        _=>throw new Phase7SqlGuardException("PHASE7_SQL_SNAPSHOT_TABLE")
    };
}
