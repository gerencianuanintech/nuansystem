using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Tests.Infrastructure;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Services;

namespace NuanSystem.Application.Tests.Features.Sync;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
public sealed class BusinessPartnerBidirectionalSqlIntegrationCollection
{
    public const string CollectionName = "BusinessPartner bidirectional disposable SQL databases";
}

[Collection(BusinessPartnerBidirectionalSqlIntegrationCollection.CollectionName)]
public sealed class BusinessPartnerBidirectionalSqlIntegrationTests
{
    [SqlServerIntegrationFact]
    public async Task DisposableDatabases_MigrationsAreIdempotentAndContractsRollbackCleanly()
    {
        await using var fixture = await BusinessPartnerSqlFixture.CreateAsync();

        await fixture.RerunMigrationsAsync();

        (await fixture.CountHistoryAsync(fixture.MasterDatabaseName, "20260903.229")).Should().Be(1);
        foreach (var tenant in fixture.TenantDatabaseNames)
        {
            (await fixture.CountHistoryAsync(tenant, "20260903.228")).Should().Be(1);
            (await fixture.CountHistoryAsync(tenant, "20260903.230")).Should().Be(1);
            await fixture.VerifyRoleUniquenessVersionInboxOutboxAndRollbackAsync(tenant);
        }
    }

    [SqlServerIntegrationFact]
    public async Task MasterMigration_RejectsAbsentOrMismatchedBindingAndAcceptsReadOnlyExactContext()
    {
        await using var fixture = await BusinessPartnerSqlFixture.CreateAsync();

        var absent = await fixture.ExecuteMasterMigrationWithoutBindingAsync();
        var mismatched = await fixture.ExecuteMasterMigrationWithMismatchedBindingAsync();

        absent.Number.Should().Be(52229);
        mismatched.Number.Should().Be(52229);
        await fixture.RerunMasterMigrationWithReadOnlyBindingAsync();
        (await fixture.CountHistoryAsync(fixture.MasterDatabaseName, "20260903.229")).Should().Be(1);
    }

    private sealed class BusinessPartnerSqlFixture : IAsyncDisposable
    {
        internal const string AdminConnectionEnvironmentVariable =
            "NUANSYSTEM_SQL_INTEGRATION_ADMIN_CONNECTION";
        internal const string SessionContextKey =
            "NUANSYSTEM_INTEGRATION_TEST_MASTER_DATABASE";

        private static readonly Regex ValidMasterName = new(
            "^NuanSystem_Test_Master_[0-9a-f]{32}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex ValidTenantName = new(
            "^NuanSystem_Test_Tenant_(Central|BranchA|BranchB)_[0-9a-f]{32}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex GoBatch = new(
            @"^\s*GO\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private readonly SqlConnectionStringBuilder admin;
        private readonly Guid runId;
        private readonly HashSet<string> createdDatabaseRegistry = new(StringComparer.Ordinal);
        private SqlConnection? serializationConnection;

        private BusinessPartnerSqlFixture(SqlConnectionStringBuilder admin, Guid runId)
        {
            this.admin = admin;
            this.runId = runId;
            var suffix = runId.ToString("N");
            MasterDatabaseName = $"NuanSystem_Test_Master_{suffix}";
            TenantDatabaseNames =
            [
                $"NuanSystem_Test_Tenant_Central_{suffix}",
                $"NuanSystem_Test_Tenant_BranchA_{suffix}",
                $"NuanSystem_Test_Tenant_BranchB_{suffix}"
            ];
        }

        public string MasterDatabaseName { get; }
        public IReadOnlyCollection<string> TenantDatabaseNames { get; }

        public static async Task<BusinessPartnerSqlFixture> CreateAsync()
        {
            var raw = Environment.GetEnvironmentVariable(AdminConnectionEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new InvalidOperationException(
                    $"{AdminConnectionEnvironmentVariable} is required when SQL integration tests are enabled.");
            }

            var builder = new SqlConnectionStringBuilder(raw);
            if (!string.Equals(builder.InitialCatalog, "master", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"{AdminConnectionEnvironmentVariable} must have Initial Catalog=master.");
            }

            var fixture = new BusinessPartnerSqlFixture(builder, Guid.NewGuid());
            try
            {
                await fixture.AcquireSerializationLockAsync();
                await fixture.CreateAndInitializeAsync();
                return fixture;
            }
            catch (Exception initializationException)
            {
                try
                {
                    await fixture.DisposeAsync();
                }
                catch (Exception cleanupException)
                {
                    throw new AggregateException(
                        "SQL fixture initialization and safe cleanup both failed.",
                        initializationException,
                        cleanupException);
                }

                throw;
            }
        }

        public async Task RerunMigrationsAsync()
        {
            await RerunMasterMigrationWithReadOnlyBindingAsync();
            foreach (var tenant in TenantDatabaseNames)
            {
                await using var connection = await OpenDatabaseAsync(tenant);
                await ExecuteScriptAsync(connection, "228_tenant_business_partner_bidirectional_foundation.sql");
                await ExecuteScriptAsync(connection, "230_tenant_business_partner_bidirectional_operations.sql");
            }
        }

        public async Task RerunMasterMigrationWithReadOnlyBindingAsync()
        {
            await using var connection = await OpenDatabaseAsync(MasterDatabaseName);
            await BindMasterSessionContextAsync(connection);
            await ExecuteScriptAsync(connection, "229_master_business_partner_bidirectional_governance.sql");
        }

        public async Task<SqlException> ExecuteMasterMigrationWithoutBindingAsync()
        {
            await using var connection = await OpenDatabaseAsync(MasterDatabaseName);
            return await CaptureGuardFailureAsync(connection);
        }

        public async Task<SqlException> ExecuteMasterMigrationWithMismatchedBindingAsync()
        {
            await using var connection = await OpenDatabaseAsync(MasterDatabaseName);
            await SetMasterSessionContextAsync(
                connection,
                $"NuanSystem_Test_Master_{Guid.Empty:N}");
            return await CaptureGuardFailureAsync(connection);
        }

        private static async Task<SqlException> CaptureGuardFailureAsync(SqlConnection connection)
        {
            try
            {
                await ExecuteScriptAsync(connection, "229_master_business_partner_bidirectional_governance.sql");
            }
            catch (SqlException exception)
            {
                return exception;
            }

            throw new InvalidOperationException("Migration 229 unexpectedly accepted an unbound test connection.");
        }

        public async Task<int> CountHistoryAsync(string databaseName, string version)
        {
            ValidateRegisteredDatabase(databaseName);
            var table = databaseName == MasterDatabaseName ? "dbo.MasterSchemaHistory" : "dbo.SchemaHistory";
            await using var connection = await OpenDatabaseAsync(databaseName);
            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT_BIG(1) FROM {table} WHERE Version=@Version;";
            command.Parameters.Add("@Version", SqlDbType.NVarChar, 50).Value = version;
            return checked((int)Convert.ToInt64(await command.ExecuteScalarAsync()));
        }

        public async Task VerifyRoleUniquenessVersionInboxOutboxAndRollbackAsync(string tenantDatabaseName)
        {
            ValidateRegisteredDatabase(tenantDatabaseName);
            await using var connection = await OpenDatabaseAsync(tenantDatabaseName);
            await VerifyRoleUniquenessAndVersionAsync(connection);
            await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            var eventId = Guid.NewGuid();
            var globalId = Guid.NewGuid();

            await using (var schema = connection.CreateCommand())
            {
                schema.Transaction = transaction;
                schema.CommandText = """
SELECT
    CASE WHEN EXISTS
    (
        SELECT 1 FROM sys.indexes
        WHERE object_id=OBJECT_ID(N'dbo.BusinessPartners')
          AND name=N'UX_BusinessPartners_Identification_Active' AND is_unique=1
    ) THEN 1 ELSE 0 END
    + CASE WHEN COL_LENGTH(N'dbo.BusinessPartners',N'CanonicalVersion') IS NOT NULL THEN 1 ELSE 0 END;
""";
                Convert.ToInt32(await schema.ExecuteScalarAsync()).Should().Be(2);
            }

            long inboxId;
            await using (var inbox = connection.CreateCommand())
            {
                inbox.Transaction = transaction;
                inbox.CommandText = """
DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@EnvelopeResult int;
EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
    @EventId=@EventId,@SourceCompanyId=21,@EntityName=N'BusinessPartnerProposal',
    @EntityGlobalId=@GlobalId,@Operation=N'Created',@PayloadJson=N'{"pilot":true}',
    @InboxId=@InboxId OUTPUT,@InboxStatus=@InboxStatus OUTPUT,@EnvelopeResult=@EnvelopeResult OUTPUT;
SELECT @InboxId;
""";
                inbox.Parameters.Add("@EventId", SqlDbType.UniqueIdentifier).Value = eventId;
                inbox.Parameters.Add("@GlobalId", SqlDbType.UniqueIdentifier).Value = globalId;
                inboxId = Convert.ToInt64(await inbox.ExecuteScalarAsync());
            }

            await using (var replay = connection.CreateCommand())
            {
                replay.Transaction = transaction;
                replay.CommandText = """
DECLARE @InboxId bigint,@InboxStatus nvarchar(30),@EnvelopeResult int;
EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE
    @EventId=@EventId,@SourceCompanyId=21,@EntityName=N'BusinessPartnerProposal',
    @EntityGlobalId=@GlobalId,@Operation=N'Created',@PayloadJson=N'{"pilot":true}',
    @InboxId=@InboxId OUTPUT,@InboxStatus=@InboxStatus OUTPUT,@EnvelopeResult=@EnvelopeResult OUTPUT;
SELECT CASE WHEN @InboxId=@ExpectedInboxId AND @EnvelopeResult=2 THEN 1 ELSE 0 END;
""";
                replay.Parameters.Add("@EventId", SqlDbType.UniqueIdentifier).Value = eventId;
                replay.Parameters.Add("@GlobalId", SqlDbType.UniqueIdentifier).Value = globalId;
                replay.Parameters.Add("@ExpectedInboxId", SqlDbType.BigInt).Value = inboxId;
                Convert.ToInt32(await replay.ExecuteScalarAsync()).Should().Be(1);
            }

            await using (var outbox = connection.CreateCommand())
            {
                outbox.Transaction = transaction;
                outbox.CommandText = """
DECLARE @OutboxId bigint,@EnvelopeResult int;
EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE
    @EventId=@EventId,@CompanyId=10,@TargetCompanyId=21,@CausationEventId=NULL,
    @EntityName=N'BusinessPartnerProposalResult',@EntityGlobalId=@GlobalId,@EntityCode=NULL,
    @Operation=N'Updated',@PayloadJson=N'{"pilot":true}',
    @OutboxId=@OutboxId OUTPUT,@EnvelopeResult=@EnvelopeResult OUTPUT;
SELECT CASE WHEN @OutboxId IS NOT NULL AND @EnvelopeResult=1 THEN 1 ELSE 0 END;
""";
                outbox.Parameters.Add("@EventId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                outbox.Parameters.Add("@GlobalId", SqlDbType.UniqueIdentifier).Value = globalId;
                Convert.ToInt32(await outbox.ExecuteScalarAsync()).Should().Be(1);
            }

            await transaction.RollbackAsync();

            await using var rolledBack = connection.CreateCommand();
            rolledBack.CommandText = "SELECT COUNT_BIG(1) FROM dbo.SyncInbox WHERE EventId=@EventId;";
            rolledBack.Parameters.Add("@EventId", SqlDbType.UniqueIdentifier).Value = eventId;
            Convert.ToInt64(await rolledBack.ExecuteScalarAsync()).Should().Be(0);
        }

        private static async Task VerifyRoleUniquenessAndVersionAsync(SqlConnection connection)
        {
            int identificationTypeId;
            await using (var lookup = connection.CreateCommand())
            {
                lookup.CommandText = "SELECT TOP(1) Id FROM dbo.BusinessPartnerIdentificationTypes WHERE IsActive=1 ORDER BY Id;";
                identificationTypeId = Convert.ToInt32(await lookup.ExecuteScalarAsync());
            }

            var customerId = Guid.NewGuid();
            var supplierId = Guid.NewGuid();
            const string identification = "0999999999001";
            await ExecuteCanonicalUpsertAsync(
                connection, customerId, "Customer", identificationTypeId, identification, canonicalVersion: 1);
            await ExecuteCanonicalUpsertAsync(
                connection, supplierId, "Supplier", identificationTypeId, identification, canonicalVersion: 1);
            await ExecuteCanonicalUpsertAsync(
                connection, customerId, "Customer", identificationTypeId, identification, canonicalVersion: 2);

            await using (var check = connection.CreateCommand())
            {
                check.CommandText = """
SELECT CASE WHEN COUNT_BIG(1)=2 AND MAX(CASE WHEN GlobalId=@CustomerId THEN CanonicalVersion ELSE 0 END)=2
            THEN 1 ELSE 0 END
FROM dbo.BusinessPartners
WHERE GlobalId IN (@CustomerId,@SupplierId) AND IsDeleted=0;
""";
                check.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                check.Parameters.Add("@SupplierId", SqlDbType.UniqueIdentifier).Value = supplierId;
                Convert.ToInt32(await check.ExecuteScalarAsync()).Should().Be(1);
            }

            var duplicateSameRole = async () => await ExecuteCanonicalUpsertAsync(
                connection,
                Guid.NewGuid(),
                "Customer",
                identificationTypeId,
                identification,
                canonicalVersion: 1);
            var exception = await duplicateSameRole.Should().ThrowAsync<SqlException>();
            exception.Which.Number.Should().Be(52030);
            exception.Which.Message.Should().Contain("Canonical identification belongs to another BusinessPartner");
        }

        private static async Task ExecuteCanonicalUpsertAsync(
            SqlConnection connection,
            Guid globalId,
            string partnerType,
            int identificationTypeId,
            string identification,
            long canonicalVersion)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
EXEC dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT
    @GlobalId=@GlobalId,
    @Code=@Code,
    @Name=@Name,
    @PartnerType=@PartnerType,
    @IdentificationTypeId=@IdentificationTypeId,
    @IdentificationNumber=@Identification,
    @NormalizedIdentificationNumber=@Identification,
    @SapCardCode=@SapCardCode,
    @CanonicalVersion=@CanonicalVersion,
    @IsActive=1,
    @IsDeleted=0,
    @AddressesJson=N'[]',
    @ContactsJson=N'[]',
    @AuditUserName=N'integration-test';
""";
            command.Parameters.Add("@GlobalId", SqlDbType.UniqueIdentifier).Value = globalId;
            command.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = $"BP-{globalId:N}".ToUpperInvariant();
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = $"{partnerType} integration test";
            command.Parameters.Add("@PartnerType", SqlDbType.NVarChar, 20).Value = partnerType;
            command.Parameters.Add("@IdentificationTypeId", SqlDbType.Int).Value = identificationTypeId;
            command.Parameters.Add("@Identification", SqlDbType.NVarChar, 50).Value = identification;
            command.Parameters.Add("@SapCardCode", SqlDbType.NVarChar, 50).Value =
                (partnerType == "Customer" ? "C" : "P") + identification;
            command.Parameters.Add("@CanonicalVersion", SqlDbType.BigInt).Value = canonicalVersion;
            await command.ExecuteNonQueryAsync();
        }

        public async ValueTask DisposeAsync()
        {
            var failures = new List<Exception>();
            foreach (var databaseName in createdDatabaseRegistry.Reverse().ToArray())
            {
                try
                {
                    await VerifyMarkerAsync(databaseName);
                    SqlConnection.ClearAllPools();
                    await using var connection = await OpenAdminAsync();
                    await using var command = connection.CreateCommand();
                    command.CommandText = $"DROP DATABASE {QuoteValidated(databaseName)};";
                    await command.ExecuteNonQueryAsync();
                    createdDatabaseRegistry.Remove(databaseName);
                }
                catch (Exception exception)
                {
                    failures.Add(new InvalidOperationException(
                        $"Safe cleanup could not remove registered disposable database {databaseName}.",
                        exception));
                }
            }

            if (serializationConnection is not null)
            {
                try
                {
                    await using var release = serializationConnection.CreateCommand();
                    release.CommandText = """
DECLARE @Result int;
EXEC @Result=sys.sp_releaseapplock
    @Resource=N'NuanSystem.BusinessPartnerBidirectionalSqlIntegration',
    @LockOwner=N'Session';
SELECT @Result;
""";
                    var result = Convert.ToInt32(await release.ExecuteScalarAsync());
                    if (result < 0)
                        throw new InvalidOperationException("SQL integration serialization lock was not released.");
                }
                catch (Exception exception)
                {
                    failures.Add(exception);
                }
                finally
                {
                    await serializationConnection.DisposeAsync();
                    serializationConnection = null;
                }
            }

            if (failures.Count > 0)
                throw new AggregateException(failures);
        }

        private async Task AcquireSerializationLockAsync()
        {
            serializationConnection = await OpenAdminAsync();
            await using var command = serializationConnection.CreateCommand();
            command.CommandText = """
DECLARE @Result int;
EXEC @Result=sys.sp_getapplock
    @Resource=N'NuanSystem.BusinessPartnerBidirectionalSqlIntegration',
    @LockMode=N'Exclusive',
    @LockOwner=N'Session',
    @LockTimeout=60000;
SELECT @Result;
""";
            var result = Convert.ToInt32(await command.ExecuteScalarAsync());
            if (result < 0)
                throw new InvalidOperationException("Could not acquire SQL integration serialization lock.");
        }

        private async Task CreateAndInitializeAsync()
        {
            var requested = new[] { MasterDatabaseName }.Concat(TenantDatabaseNames).ToArray();
            requested.Should().OnlyHaveUniqueItems();
            foreach (var name in requested)
            {
                ValidateDisposableName(name);
                if (await DatabaseExistsAsync(name))
                    throw new InvalidOperationException($"Disposable database name already exists: {name}.");
            }

            foreach (var name in requested)
            {
                await using var connection = await OpenAdminAsync();
                await using var create = connection.CreateCommand();
                create.CommandText = $"CREATE DATABASE {QuoteValidated(name)};";
                await create.ExecuteNonQueryAsync();
                createdDatabaseRegistry.Add(name);
                await CreateMarkerAsync(name);
            }

            await InitializeMasterAsync();
            foreach (var tenant in TenantDatabaseNames)
                await InitializeTenantAsync(tenant);
            await ConfigureCompaniesAsync();
        }

        private async Task InitializeMasterAsync()
        {
            await using var connection = await OpenDatabaseAsync(MasterDatabaseName);
            var method = typeof(SqlServerMasterDatabaseInitializer).GetMethod(
                "CreateSchemaObjectsAsync",
                BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new MissingMethodException(nameof(SqlServerMasterDatabaseInitializer), "CreateSchemaObjectsAsync");
            var task = method.Invoke(null, [connection, CancellationToken.None]) as Task
                ?? throw new InvalidOperationException("Master schema bootstrap did not return a Task.");
            await task;

            foreach (var prerequisite in new[]
                     {
                         "064_master_sync_outbox_inbox.sql",
                         "069_sync_master_branch_configuration.sql",
                         "080_sync_entity_definitions.sql",
                         "092_sync_routing_by_target_branch.sql",
                         "093_sync_distribution_policies.sql",
                         "094_master_company_branch_hierarchy.sql"
                     })
            {
                await ExecuteScriptAsync(
                    connection,
                    prerequisite,
                    skipExactMasterGuard: prerequisite.StartsWith("080_", StringComparison.Ordinal));
            }

            await BindMasterSessionContextAsync(connection);
            await ExecuteScriptAsync(connection, "229_master_business_partner_bidirectional_governance.sql");
        }

        private async Task InitializeTenantAsync(string databaseName)
        {
            var schemaField = typeof(SqlServerTenantDatabaseInitializer).GetField(
                "TenantSchemaSql",
                BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new MissingFieldException(nameof(SqlServerTenantDatabaseInitializer), "TenantSchemaSql");
            var tenantSchema = schemaField.GetRawConstantValue() as string
                ?? throw new InvalidOperationException("Tenant schema bootstrap is unavailable.");
            await using (var bootstrapConnection = await OpenDatabaseAsync(databaseName))
            {
                await using var bootstrap = bootstrapConnection.CreateCommand();
                bootstrap.CommandText = tenantSchema;
                bootstrap.CommandTimeout = 120;
                await bootstrap.ExecuteNonQueryAsync();
                await ExecuteScriptAsync(bootstrapConnection, "024_tenant_business_partners.sql");
            }

            var context = new FixtureCompanyContext();
            context.SetCurrentCompany(new CompanyConnectionInfo(
                1,
                databaseName,
                databaseName,
                DatabaseEngine.SqlServer,
                BuildDatabaseConnectionString(databaseName),
                SapIntegrationMode.None));
            var initializer = new SqlServerTenantDatabaseInitializer(
                context,
                new FixtureTenantConnectionFactory(BuildDatabaseConnectionString(databaseName)));
            await initializer.InitializeCurrentTenantAsync();
        }

        private async Task ConfigureCompaniesAsync()
        {
            await using var connection = await OpenDatabaseAsync(MasterDatabaseName);
            await using var command = connection.CreateCommand();
            command.CommandText = """
DECLARE @CentralId int;
INSERT dbo.Companies
    (Code,CommercialName,DatabaseEngine,[Server],DatabaseName,DatabaseUser,DatabasePasswordEncrypted,
     IsActive,SapIntegrationMode,IsMaster,ParentCompanyId,SyncEnabled)
VALUES
    (N'BP-TEST-CENTRAL',N'BP test central',1,N'integration-test',@CentralDb,N'integration-test',N'integration-test',1,0,1,NULL,1);
SET @CentralId=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.Companies
    (Code,CommercialName,DatabaseEngine,[Server],DatabaseName,DatabaseUser,DatabasePasswordEncrypted,
     IsActive,SapIntegrationMode,IsMaster,ParentCompanyId,SyncEnabled,BranchCode)
VALUES
    (N'BP-TEST-BRANCH-A',N'BP test branch A',1,N'integration-test',@BranchADb,N'integration-test',N'integration-test',1,0,0,@CentralId,1,N'A'),
    (N'BP-TEST-BRANCH-B',N'BP test branch B',1,N'integration-test',@BranchBDb,N'integration-test',N'integration-test',1,0,0,@CentralId,1,N'B');
""";
            var tenants = TenantDatabaseNames.ToArray();
            command.Parameters.Add("@CentralDb", SqlDbType.NVarChar, 128).Value = tenants[0];
            command.Parameters.Add("@BranchADb", SqlDbType.NVarChar, 128).Value = tenants[1];
            command.Parameters.Add("@BranchBDb", SqlDbType.NVarChar, 128).Value = tenants[2];
            await command.ExecuteNonQueryAsync();
        }

        private async Task BindMasterSessionContextAsync(SqlConnection connection)
            => await SetMasterSessionContextAsync(connection, MasterDatabaseName);

        private static async Task SetMasterSessionContextAsync(
            SqlConnection connection,
            string databaseName)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
EXEC sys.sp_set_session_context
    @key=N'NUANSYSTEM_INTEGRATION_TEST_MASTER_DATABASE',
    @value=@DatabaseName,
    @read_only=1;
""";
            command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar, 128).Value = databaseName;
            await command.ExecuteNonQueryAsync();
        }

        private async Task CreateMarkerAsync(string databaseName)
        {
            await using var connection = await OpenDatabaseAsync(databaseName);
            await using var command = connection.CreateCommand();
            command.CommandText = """
CREATE TABLE dbo.NuanSystemIntegrationTestMarker
(
    RunId uniqueidentifier NOT NULL CONSTRAINT PK_NuanSystemIntegrationTestMarker PRIMARY KEY,
    DatabaseName sysname NOT NULL,
    CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_NuanSystemIntegrationTestMarker_CreatedAt DEFAULT SYSUTCDATETIME()
);
INSERT dbo.NuanSystemIntegrationTestMarker(RunId,DatabaseName) VALUES(@RunId,@DatabaseName);
""";
            command.Parameters.Add("@RunId", SqlDbType.UniqueIdentifier).Value = runId;
            command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar, 128).Value = databaseName;
            await command.ExecuteNonQueryAsync();
        }

        private async Task VerifyMarkerAsync(string databaseName)
        {
            ValidateRegisteredDatabase(databaseName);
            await using var connection = await OpenDatabaseAsync(databaseName);
            await using var command = connection.CreateCommand();
            command.CommandText = """
SELECT COUNT_BIG(1)
FROM dbo.NuanSystemIntegrationTestMarker
WHERE RunId=@RunId AND DatabaseName=@DatabaseName;
""";
            command.Parameters.Add("@RunId", SqlDbType.UniqueIdentifier).Value = runId;
            command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar, 128).Value = databaseName;
            if (Convert.ToInt64(await command.ExecuteScalarAsync()) != 1)
                throw new InvalidOperationException("Disposable database ownership marker mismatch; cleanup refused.");
        }

        private async Task<bool> DatabaseExistsAsync(string databaseName)
        {
            ValidateDisposableName(databaseName);
            await using var connection = await OpenAdminAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT CASE WHEN DB_ID(@DatabaseName) IS NULL THEN 0 ELSE 1 END;";
            command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar, 128).Value = databaseName;
            return Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
        }

        private async Task<SqlConnection> OpenAdminAsync()
        {
            if (!string.Equals(admin.InitialCatalog, "master", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Administrative connection no longer targets master.");
            var connection = new SqlConnection(admin.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }

        private async Task<SqlConnection> OpenDatabaseAsync(string databaseName)
        {
            ValidateRegisteredDatabase(databaseName);
            var connection = new SqlConnection(BuildDatabaseConnectionString(databaseName));
            await connection.OpenAsync();
            return connection;
        }

        private string BuildDatabaseConnectionString(string databaseName)
        {
            ValidateRegisteredDatabase(databaseName);
            return new SqlConnectionStringBuilder(admin.ConnectionString)
            {
                InitialCatalog = databaseName
            }.ConnectionString;
        }

        private void ValidateRegisteredDatabase(string databaseName)
        {
            ValidateDisposableName(databaseName);
            if (!createdDatabaseRegistry.Contains(databaseName))
                throw new InvalidOperationException("Database is not in this fixture's exact creation registry.");
        }

        private static void ValidateDisposableName(string databaseName)
        {
            if (!ValidMasterName.IsMatch(databaseName) && !ValidTenantName.IsMatch(databaseName))
                throw new InvalidOperationException("Database name is outside the disposable integration-test namespace.");
        }

        private static string QuoteValidated(string databaseName)
        {
            ValidateDisposableName(databaseName);
            using var commandBuilder = new SqlCommandBuilder();
            return commandBuilder.QuoteIdentifier(databaseName);
        }

        private static async Task ExecuteScriptAsync(
            SqlConnection connection,
            string fileName,
            bool skipExactMasterGuard = false)
        {
            var script = await File.ReadAllTextAsync(Path.Combine(Root(), "database", "sql", fileName));
            foreach (var batch in GoBatch.Split(script).Where(value => !string.IsNullOrWhiteSpace(value)))
            {
                if (Regex.IsMatch(batch, @"\bUSE\s+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                    throw new InvalidOperationException($"Fixture refuses script batch with USE: {fileName}.");
                if (skipExactMasterGuard && batch.Contains("IF DB_NAME() <> N'NuanSystem_Master'", StringComparison.Ordinal))
                    continue;
                await using var command = connection.CreateCommand();
                command.CommandText = batch;
                command.CommandTimeout = 120;
                await command.ExecuteNonQueryAsync();
            }
        }

        private static string Root()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
                directory = directory.Parent;
            return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
        }
    }

    private sealed class FixtureCompanyContext : ICompanyContext
    {
        public bool HasActiveCompany => CurrentCompany is not null;
        public CompanyConnectionInfo? CurrentCompany { get; private set; }
        public void SetCurrentCompany(CompanyConnectionInfo company) => CurrentCompany = company;
    }

    private sealed class FixtureTenantConnectionFactory(string connectionString) : ITenantConnectionFactory
    {
        public IDbConnection CreateConnection() => new SqlConnection(connectionString);
    }
}
