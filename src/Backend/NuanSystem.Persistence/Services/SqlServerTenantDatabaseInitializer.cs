using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Domain.Tenancy;
using System.Text.RegularExpressions;

namespace NuanSystem.Persistence.Services;

public sealed class SqlServerTenantDatabaseInitializer(
    ICompanyContext companyContext,
    ITenantConnectionFactory tenantConnectionFactory) : ITenantDatabaseInitializer
{
    public async Task InitializeCurrentTenantAsync(CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany
            ?? throw new InvalidOperationException("No hay empresa activa para inicializar la base tenant.");

        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            throw new NotSupportedException($"Inicializacion tenant no implementada para {company.DatabaseEngine}.");
        }

        await using var connection = (SqlConnection)tenantConnectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = TenantSchemaSql;
        await command.ExecuteNonQueryAsync(cancellationToken);

        await ExecuteOptionalTenantScriptsAsync(connection, cancellationToken);
    }

    private static async Task ExecuteOptionalTenantScriptsAsync(
        SqlConnection connection,
        CancellationToken cancellationToken)
    {
        foreach (var fileName in new[]
                 {
                      "018_inventory_items_master.sql",
                      "021_inventory_item_families_master.sql",
                      "043_inventory_item_master_profile.sql",
                      "044_inventory_auxiliary_catalogs.sql",
                      "048_tenant_sap_supplier_import.sql",
                      "050_tenant_sap_sync_worker.sql",
                      "051_tenant_security_document_series.sql",
                      "053_tenant_operational_catalog.sql",
                      "063_tenant_global_ids_and_external_refs.sql",
                      "065_tenant_sync_inbox_local_outbox.sql",
                      "067_tenant_warehouses_master.sql",
                      "095_tenant_item_sap_import.sql",
                      "097_tenant_item_group_master_branch_sync.sql"
                      ,"100_tenant_purchase_reference_catalog_sync.sql"
                      ,"101_tenant_sap_purchase_order_import.sql"
                      ,"103_tenant_purchase_order_sync.sql"
                      ,"115_tenant_sri_document_queue.sql"
                      ,"117_tenant_sri_worker_and_document_store.sql"
                      ,"118_tenant_sri_document_monitor_and_download.sql"
                      ,"121_tenant_sri_worker_operational_summary.sql"
                      ,"123_tenant_sri_document_monitor_summary_bigint_fix.sql"
                      ,"124_tenant_local_outbox_relay.sql"
                      ,"127_tenant_item_family_master_branch_sync.sql"
                      ,"129_tenant_item_group_transactional_outbox.sql"
                      ,"131_tenant_item_sync_payload_v2.sql"
                      ,"133_tenant_warehouse_transactional_outbox.sql"
                      ,"135_tenant_warehouse_tombstone_code_reservation.sql"
                      ,"136_tenant_currency_transactional_outbox.sql"
                   })
        {
            var scriptPath = FindDatabaseScriptPath(fileName);
            if (scriptPath is null)
            {
                continue;
            }

            await ExecuteScriptFileAsync(connection, scriptPath, cancellationToken);
        }
    }

    private static async Task ExecuteScriptFileAsync(
        SqlConnection connection,
        string scriptPath,
        CancellationToken cancellationToken)
    {
        var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        foreach (var batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(batch))
            {
                continue;
            }

            await using var command = connection.CreateCommand();
            command.CommandText = batch;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static string? FindDatabaseScriptPath(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(directory.FullName, "database", "sql", fileName);
            if (File.Exists(path))
            {
                return path;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private const string TenantSchemaSql = """
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SchemaHistory
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SchemaHistory PRIMARY KEY,
        Version nvarchar(50) NOT NULL,
        Description nvarchar(300) NOT NULL,
        AppliedAt datetime2(0) NOT NULL CONSTRAINT DF_SchemaHistory_AppliedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_SchemaHistory_Version UNIQUE (Version)
    );
END;

IF OBJECT_ID(N'dbo.Items', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Items
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Items PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(500) NULL,
        UnitOfMeasure nvarchar(20) NOT NULL CONSTRAINT DF_Items_UnitOfMeasure DEFAULT N'UND',
        IsInventoryItem bit NOT NULL CONSTRAINT DF_Items_IsInventoryItem DEFAULT 1,
        IsActive bit NOT NULL CONSTRAINT DF_Items_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Items_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_Items_Code UNIQUE (Code)
    );
END;

IF OBJECT_ID(N'dbo.SapSyncLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncLog
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncLog PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityType nvarchar(80) NOT NULL,
        EntityId nvarchar(80) NOT NULL,
        SapObjectType nvarchar(80) NOT NULL,
        RequestJson nvarchar(max) NULL,
        ResponseJson nvarchar(max) NULL,
        Status nvarchar(30) NOT NULL,
        ErrorMessage nvarchar(max) NULL,
        SapDocEntry int NULL,
        SapDocNum int NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncLog_CreatedAt DEFAULT SYSUTCDATETIME(),
        SyncedAt datetime2(0) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260427.04')
BEGIN
    INSERT INTO dbo.SchemaHistory (Version, Description)
    VALUES (N'20260427.04', N'Fase 4: esquema inicial tenant');
END;

""";
}
