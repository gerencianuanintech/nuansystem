using FluentAssertions;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncInfrastructureContractTests
{
    [Fact]
    public void MasterSyncScript_DefinesIdempotentOutboxWithGlobalIdAndEventUniqueness()
    {
        var script = ReadDatabaseScript("064_master_sync_outbox_inbox.sql");

        script.Should().Contain("SYNC.OUTBOX.VIEW");
        script.Should().Contain("SYNC.AUDIT.VIEW");
        script.Should().Contain("SYNC.OUTBOX.RETRY");
        script.Should().Contain("SYNC.OUTBOX.RETRY_DEADLETTER");
        script.Should().Contain("SYNC.OUTBOX.RELEASE_LOCK");
        script.Should().Contain("IF OBJECT_ID(N'dbo.SyncOutbox', N'U') IS NULL");
        script.Should().Contain("EventId uniqueidentifier NOT NULL CONSTRAINT DF_SyncOutbox_EventId DEFAULT NEWID()");
        script.Should().Contain("EntityGlobalId uniqueidentifier NOT NULL");
        script.Should().Contain("CREATE UNIQUE INDEX UX_SyncOutbox_EventId ON dbo.SyncOutbox (EventId)");
        script.Should().Contain("CREATE INDEX IX_SyncOutbox_Status_NextRetryAt ON dbo.SyncOutbox (Status, NextRetryAt, CreatedAt)");
        script.Should().Contain("CREATE TABLE dbo.SyncOutboxTargets");
        script.Should().Contain("CREATE TABLE dbo.SyncAudit");
        script.Should().Contain("RetriedFromDeadLetter");
        script.Should().Contain("LockReleased");
        script.Should().Contain("RolePermissions");
    }

    [Fact]
    public void TenantSyncScript_DefinesInboxAndLocalOutboxIdempotency()
    {
        var script = ReadDatabaseScript("065_tenant_sync_inbox_local_outbox.sql");

        script.Should().Contain("IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NULL");
        script.Should().Contain("CREATE UNIQUE INDEX UX_SyncInbox_EventId ON dbo.SyncInbox (EventId)");
        script.Should().Contain("IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL");
        script.Should().Contain("EventId uniqueidentifier NOT NULL CONSTRAINT DF_LocalOutbox_EventId DEFAULT NEWID()");
        script.Should().Contain("CREATE UNIQUE INDEX UX_LocalOutbox_EventId ON dbo.LocalOutbox (EventId)");
        script.Should().Contain("EntityGlobalId uniqueidentifier NOT NULL");
    }

    [Fact]
    public void SyncContracts_AreGenericAndDoNotRequireSap()
    {
        var enumNames = new[]
        {
            nameof(SyncDirection),
            nameof(SyncConflictPolicy),
            nameof(SyncEventStatus),
            nameof(SyncOperation),
            nameof(SyncRuleType),
            nameof(SyncAuditAction)
        };

        foreach (var enumName in enumNames)
        {
            enumName.Should().NotContain("Sap");
        }

        Enum.GetNames<SyncOperation>().Should().NotContain(name => name.Contains("Sap", StringComparison.OrdinalIgnoreCase));
        Enum.GetNames<SyncEventStatus>().Should().BeEquivalentTo("Pending", "InProcess", "Applied", "Error", "Ignored", "DeadLetter");
    }

    [Fact]
    public void CreateSyncOutboxEventData_UsesEventIdAndGlobalIdWithoutReplacingLocalId()
    {
        var eventId = Guid.NewGuid();
        var globalId = Guid.NewGuid();

        var data = new CreateSyncOutboxEventData(
            eventId,
            CompanyId: 1,
            EntityName: "Items",
            EntityGlobalId: globalId,
            EntityCode: "ART-001",
            Operation: SyncOperation.Updated,
            PayloadJson: """{"id":10,"globalId":"value"}""",
            SourceSystem: null,
            SourceReference: null);

        data.EventId.Should().Be(eventId);
        data.EntityGlobalId.Should().Be(globalId);
        data.MaxAttempts.Should().Be(3);
        data.SourceSystem.Should().BeNull();
    }

    [Fact]
    public void CreateSyncInboxEventData_UsesEventIdForIdempotentReceive()
    {
        var eventId = Guid.NewGuid();
        var globalId = Guid.NewGuid();

        var data = new CreateSyncInboxEventData(
            eventId,
            SourceCompanyId: 1,
            EntityName: "BusinessPartners",
            EntityGlobalId: globalId,
            Operation: SyncOperation.Created,
            PayloadJson: """{"code":"CLI-001"}""");

        data.EventId.Should().Be(eventId);
        data.EntityGlobalId.Should().Be(globalId);
        data.Operation.Should().Be(SyncOperation.Created);
    }

    [Fact]
    public void SyncRepositories_CreateEventsIdempotentlyWithPendingStatus()
    {
        var outboxRepository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncOutboxRepository.cs");
        var inboxRepository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncInboxRepository.cs");

        outboxRepository.Should().Contain("WHERE EventId = @EventId");
        outboxRepository.Should().Contain("BEGIN TRY");
        outboxRepository.Should().Contain("ERROR_NUMBER() IN (2601, 2627)");
        outboxRepository.Should().Contain("Status = SyncEventStatus.Pending.ToString()");
        outboxRepository.Should().Contain("Operation = data.Operation.ToString()");
        outboxRepository.Should().Contain("data.EntityGlobalId");

        inboxRepository.Should().Contain("WHERE EventId = @EventId");
        inboxRepository.Should().Contain("BEGIN TRY");
        inboxRepository.Should().Contain("ERROR_NUMBER() IN (2601, 2627)");
        inboxRepository.Should().Contain("Status = SyncEventStatus.Pending.ToString()");
        inboxRepository.Should().Contain("Operation = data.Operation.ToString()");
        inboxRepository.Should().Contain("data.EntityGlobalId");
    }

    [Fact]
    public void SyncOutboxTargets_AreIdempotentByOutboxAndBranch()
    {
        var outboxRepository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncOutboxRepository.cs");

        outboxRepository.Should().Contain("WHERE OutboxId = @OutboxId AND BranchCompanyId = @BranchCompanyId");
        outboxRepository.Should().Contain("ERROR_NUMBER() IN (2601, 2627)");
        outboxRepository.Should().Contain("SELECT Id FROM dbo.SyncOutboxTargets WHERE OutboxId = @OutboxId AND BranchCompanyId = @BranchCompanyId");
    }

    [Fact]
    public void SyncRuleEvaluator_UsesActiveRulesAndEnabledBranchesOnly()
    {
        var ruleEvaluator = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncRuleEvaluator.cs");

        ruleEvaluator.Should().Contain("FROM dbo.SyncDistributionRules AS distRule");
        ruleEvaluator.Should().NotContain("AS rule");
        ruleEvaluator.Should().NotContain(" rule.");
        ruleEvaluator.Should().Contain("distRule.CompanyId = @CompanyId");
        ruleEvaluator.Should().Contain("distRule.EntityName = @EntityName");
        ruleEvaluator.Should().Contain("distRule.IsEnabled = 1");
        ruleEvaluator.Should().Contain("branch.IsActive = 1");
        ruleEvaluator.Should().Contain("branch.IsMaster = 0");
        ruleEvaluator.Should().Contain("branch.SyncEnabled = 1");
        ruleEvaluator.Should().Contain("branch.ParentCompanyId = @CompanyId");
        ruleEvaluator.Should().Contain("branch.IsDeleted = 0");
        ruleEvaluator.Should().Contain("distRule.RuleType = N'All'");
        ruleEvaluator.Should().Contain("ORDER BY distRule.BranchCompanyId");
    }

    [Fact]
    public void SqlConnectionPolicy_UsesSecureDefaultsAndConfigurableTenantConnections()
    {
        var policyOptions = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Options",
            "SqlConnectionPolicyOptions.cs");
        var serviceRegistration = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "DependencyInjection",
            "PersistenceServiceRegistration.cs");
        var companyResolver = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Tenancy",
            "SqlServerCompanyResolver.cs");

        policyOptions.Should().Contain("public const string SectionName = \"SqlConnectionPolicy\"");
        policyOptions.Should().Contain("public bool Encrypt { get; set; } = true;");
        policyOptions.Should().Contain("public bool TrustServerCertificate { get; set; }");
        serviceRegistration.Should().Contain("services.Configure<SqlConnectionPolicyOptions>");
        companyResolver.Should().Contain("IOptions<SqlConnectionPolicyOptions>");
        companyResolver.Should().Contain("Encrypt = sqlConnectionPolicy.Encrypt");
        companyResolver.Should().Contain("TrustServerCertificate = sqlConnectionPolicy.TrustServerCertificate");
        companyResolver.Should().NotContain("TrustServerCertificate = true");
        companyResolver.Should().NotContain("Encrypt = false");
    }

    [Fact]
    public void MasterBranchSyncWorker_AppsettingsExposeNonSecretConfigurationShape()
    {
        var appsettings = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.MasterBranchSyncWorker",
            "appsettings.json");
        var productionAppsettings = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.MasterBranchSyncWorker",
            "appsettings.Production.json");

        foreach (var settings in new[] { appsettings, productionAppsettings })
        {
            settings.Should().Contain("\"ConnectionStrings\"");
            settings.Should().Contain("\"SqlServerAdmin\": \"\"");
            settings.Should().Contain("\"Security\"");
            settings.Should().Contain("\"EncryptionKey\": \"\"");
            settings.Should().Contain("\"SqlConnectionPolicy\"");
            settings.Should().Contain("\"Encrypt\": true");
            settings.Should().Contain("\"TrustServerCertificate\": false");
            settings.Should().NotContain("Password=");
            settings.Should().NotContain("User Id=sa");
        }
    }

    [Fact]
    public void SyncMonitoringEndpoints_UseGranularReadPermissionsAndExposeOnlyAllowedManualActions()
    {
        var endpoints = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Api",
            "Endpoints",
            "SyncEndpoints.cs");

        endpoints.Should().Contain("PermissionCodes.SyncOutboxView");
        endpoints.Should().Contain("PermissionCodes.SyncAuditView");
        endpoints.Should().Contain("PermissionCodes.SyncOutboxRetry");
        endpoints.Should().Contain("PermissionCodes.SyncOutboxRetryDeadLetter");
        endpoints.Should().Contain("PermissionCodes.SyncOutboxReleaseLock");
        endpoints.Should().Contain("app.MapGet(\"/api/sync/dashboard\"");
        endpoints.Should().Contain("app.MapGet(\"/api/sync/summary\"");
        endpoints.Should().Contain("app.MapGet(\"/api/sync/outbox\"");
        endpoints.Should().Contain("app.MapGet(\"/api/sync/outbox/{id:long}\"");
        endpoints.Should().Contain("app.MapGet(\"/api/sync/outbox/{id:long}/targets\"");
        endpoints.Should().Contain("app.MapGet(\"/api/sync/audit\"");
        endpoints.Should().Contain("app.MapPost(\"/api/sync/outbox/{id:long}/retry\"");
        endpoints.Should().Contain("app.MapPost(\"/api/sync/outbox/{id:long}/retry-deadletter\"");
        endpoints.Should().Contain("app.MapPost(\"/api/sync/outbox/{id:long}/release-expired-lock\"");
        endpoints.Should().NotContain("MapPut(");
        endpoints.Should().NotContain("MapPatch(");
        endpoints.Should().NotContain("MapDelete(");
        endpoints.Should().NotContain("/api/sync/reprocess");
        endpoints.Should().NotContain("/api/sync/apply");
        endpoints.Should().NotContain("/api/sync/run");
        endpoints.Should().NotContain("/api/sync/dispatch");
        endpoints.Should().NotContain("/api/sync/claim");
        endpoints.Should().NotContain("/api/sync/sync-now");
    }

    [Fact]
    public void SyncMonitoringDtos_KeepPayloadJsonOnlyInOutboxDetail()
    {
        typeof(SyncDashboardDto).GetProperties()
            .Select(property => property.Name)
            .Should()
            .NotContain("PayloadJson");

        typeof(SyncOutboxListItemDto).GetProperties()
            .Select(property => property.Name)
            .Should()
            .NotContain("PayloadJson");

        typeof(SyncOutboxDetailDto).GetProperties()
            .Select(property => property.Name)
            .Should()
            .Contain("PayloadJson");
    }

    [Fact]
    public void SyncMonitoringQueries_ExposeExpectedFilters()
    {
        var outboxFilter = typeof(SyncOutboxQueryFilter).GetProperties().Select(property => property.Name);
        var auditFilter = typeof(SyncAuditQueryFilter).GetProperties().Select(property => property.Name);

        outboxFilter.Should().Contain([
            "Status",
            "EntityName",
            "EntityGlobalId",
            "EventId",
            "BranchCompanyId",
            "CreatedFrom",
            "CreatedTo",
            "HasErrors",
            "DeadLetterOnly",
            "Page",
            "PageSize"]);

        auditFilter.Should().Contain([
            "Status",
            "EntityName",
            "EntityGlobalId",
            "EventId",
            "BranchCompanyId",
            "CreatedFrom",
            "CreatedTo",
            "HasErrors",
            "DeadLetterOnly",
            "Page",
            "PageSize"]);
    }

    [Fact]
    public void SyncMonitoringRepositories_DoNotLoadPayloadJsonInDashboardOrOutboxListing()
    {
        var outboxRepository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncOutboxRepository.cs");

        outboxRepository.Should().Contain("GetDashboardAsync");
        outboxRepository.Should().Contain("SearchOutboxAsync");
        outboxRepository.Should().Contain("GetOutboxDetailAsync");
        outboxRepository.Should().Contain("BuildOutboxWhere");
        outboxRepository.Should().Contain("outbox.Status = @Status");
        outboxRepository.Should().Contain("target.BranchCompanyId = @BranchCompanyId");
        outboxRepository.Should().Contain("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

        var listMethodStart = outboxRepository.IndexOf("public async Task<IReadOnlyCollection<SyncOutboxListItemDto>> SearchOutboxAsync", StringComparison.Ordinal);
        var detailMethodStart = outboxRepository.IndexOf("public async Task<SyncOutboxDetailDto?> GetOutboxDetailAsync", StringComparison.Ordinal);
        var listMethod = outboxRepository[listMethodStart..detailMethodStart];

        listMethod.Should().NotContain("PayloadJson");
        outboxRepository[detailMethodStart..].Should().Contain("PayloadJson");
    }

    [Fact]
    public void SyncMonitoringAuditRepository_ExposesReadOnlySearchFilters()
    {
        var auditRepository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncAuditRepository.cs");

        auditRepository.Should().Contain("SearchAuditAsync");
        auditRepository.Should().Contain("BuildAuditWhere");
        auditRepository.Should().Contain("audit.NewStatus = @Status");
        auditRepository.Should().Contain("audit.BranchCompanyId = @BranchCompanyId");
        auditRepository.Should().Contain("audit.Action = N'DeadLetter'");

        var searchStart = auditRepository.IndexOf("public async Task<IReadOnlyCollection<SyncAuditDto>> SearchAuditAsync", StringComparison.Ordinal);
        var searchMethod = auditRepository[searchStart..];

        searchMethod.Should().NotContain("INSERT ");
        searchMethod.Should().NotContain("UPDATE ");
        searchMethod.Should().NotContain("DELETE ");
    }

    private static string ReadDatabaseScript(string scriptName)
    {
        return ReadSourceFile("database", "sql", scriptName);
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var scriptPath = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(scriptPath))
            {
                return File.ReadAllText(scriptPath);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro el archivo {Path.Combine(pathParts)}.");
    }
}
