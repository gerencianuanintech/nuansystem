using FluentAssertions;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncConfigurationContractTests
{
    [Fact]
    public void SyncConfigurationScript_DefinesOnlyAdministrativeConfigurationTables()
    {
        var script = ReadDatabaseScript("069_sync_master_branch_configuration.sql");

        script.Should().Contain("CREATE TABLE dbo.SyncProfiles");
        script.Should().Contain("CREATE TABLE dbo.SyncProfileBranches");
        script.Should().Contain("CREATE TABLE dbo.SyncProfileEntities");
        script.Should().Contain("CREATE TABLE dbo.SyncProfileEntityBranches");
        script.Should().Contain("CREATE TABLE dbo.SyncSchedules");

        script.Should().NotContain("CREATE TABLE dbo.SyncExecutions");
        script.Should().NotContain("CREATE TABLE dbo.SyncExecutionDetails");
        script.Should().NotContain("CREATE TABLE dbo.SyncErrors");
        script.Should().NotContain("CREATE TABLE dbo.SyncCheckpoints");
        script.Should().NotContain("CREATE TABLE dbo.SyncOutbox");
        script.Should().NotContain("CREATE TABLE dbo.SyncInbox");
        script.Should().NotContain("CREATE TABLE dbo.SyncAudit");
    }

    [Fact]
    public void SyncConfigurationScript_UsesRealCompanyIdentifiersAndSupportedValues()
    {
        var script = ReadDatabaseScript("069_sync_master_branch_configuration.sql");

        script.Should().Contain("CompanyId int NOT NULL");
        script.Should().Contain("BranchCompanyId int NOT NULL");
        script.Should().Contain("FK_SyncProfiles_Companies");
        script.Should().Contain("FK_SyncProfileBranches_Companies");
        script.Should().Contain("Direction IN (N'MasterToBranch')");
        script.Should().Contain("ConflictStrategy IN (N'MasterWins')");
        script.Should().Contain("ExecutionMode IN (N'Incremental', N'Full', N'Manual')");
        script.Should().Contain("ScheduleType IN (N'Manual', N'Interval', N'Daily')");
        script.Should().Contain("America/Guayaquil");

        script.Should().NotContain("TenantId");
        script.Should().NotContain("MasterTenantId");
        script.Should().NotContain("BranchTenantId");
        script.Should().NotContain("BranchId uniqueidentifier");
        script.Should().NotContain("ConnectionProfileId");
    }

    [Fact]
    public void SyncConfigurationScript_DefinesAggregateUniquenessAndMatrixConsistency()
    {
        var script = ReadDatabaseScript("069_sync_master_branch_configuration.sql");

        script.Should().Contain("UX_SyncProfiles_Company_Code_Active");
        script.Should().Contain("UX_SyncProfileBranches_Profile_Branch_Active");
        script.Should().Contain("UX_SyncProfileEntities_Profile_Code_Active");
        script.Should().Contain("UX_SyncProfileEntityBranches_Entity_Branch_Active");
        script.Should().Contain("UX_SyncSchedules_Profile_Active");
        script.Should().Contain("FK_SyncProfileEntityBranches_Entities_Profile");
        script.Should().Contain("FK_SyncProfileEntityBranches_Branches_Profile");
        script.Should().Contain("BatchSize BETWEEN 1 AND 10000");
        script.Should().Contain("MaxRetries BETWEEN 0 AND 10");
        script.Should().Contain("RetryDelaySeconds BETWEEN 0 AND 3600");
        script.Should().Contain("TimeoutMinutes BETWEEN 1 AND 1440");
    }

    [Fact]
    public void SyncConfigurationRepository_UsesStoredProceduresForAggregatePersistence()
    {
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncProfileRepository.cs");
        var registration = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "DependencyInjection",
            "PersistenceServiceRegistration.cs");

        repository.Should().Contain("CommandType.StoredProcedure");
        repository.Should().Contain("QueryMultipleAsync");
        repository.Should().Contain("SP_NA_GET_SYNCPROFILELISTAR");
        repository.Should().Contain("SP_NA_GET_SYNCPROFILEBUSCARPORID");
        repository.Should().Contain("SP_NA_POST_SYNCPROFILECREAR");
        repository.Should().Contain("SP_NA_PUT_SYNCPROFILEACTUALIZAR");
        repository.Should().Contain("SP_NA_PATCH_SYNCPROFILEACTIVAR");
        repository.Should().Contain("SP_NA_GET_SYNCPROFILETIENEHISTORIAL");
        registration.Should().Contain("services.AddScoped<ISyncProfileRepository, SyncProfileRepository>();");
    }

    [Fact]
    public void SyncConfigurationContracts_AreCompanyScopedAndUseInitialCatalog()
    {
        typeof(ISyncProfileRepository).GetMethods()
            .Select(method => method.Name)
            .Should()
            .BeEquivalentTo(
                "SearchAsync",
                "ListAsync",
                "GetByIdAsync",
                "GetByCodeAsync",
                "CreateAsync",
                "UpdateAsync",
                "SetActiveAsync",
                "DeleteAsync",
                "HasOperationalHistoryAsync",
                "GetCompanyLookupsAsync",
                "RecordAuditAsync");

        var aggregate = new SyncProfileAggregate(
            Id: 0,
            CompanyId: 1,
            Code: "MASTER-CATALOGS",
            Name: "Catalogos maestros",
            Description: null,
            Direction: "MasterToBranch",
            ExecutionMode: "Incremental",
            ConflictStrategy: "MasterWins",
            BatchSize: 500,
            MaxRetries: 3,
            RetryDelaySeconds: 60,
            TimeoutMinutes: 30,
            IsActive: true,
            AuditUserId: 1,
            AuditUserName: "Sistema",
            Branches: [],
            Entities: [],
            EntityBranches: [],
            Schedule: null);

        aggregate.CompanyId.Should().Be(1);
        SyncMasterBranchEntityCodes.IsKnown(SyncMasterBranchEntityCodes.Countries).Should().BeTrue();
        SyncMasterBranchEntityCodes.InitialCatalog.Select(item => item.EntityCode).Should().BeEquivalentTo(
            "Countries",
            "Provinces",
            "Cities",
            "Currencies",
            "BusinessPartnerPaymentTerms",
            "SupplierGroups",
            "SupplierClasses",
            "EconomicActivities",
            "Zones",
            "SupplyMethods",
            "BusinessPartner",
            "Item",
            "Warehouse");
        SyncMasterBranchEntityCodes.InitialCatalog.Should().OnlyContain(item => item.ExistsInModel);
        SyncMasterBranchEntityCodes.InitialCatalog
            .Where(item => item.EntityCode is "BusinessPartner" or "Item" or "Warehouse")
            .Should()
            .OnlyContain(item => item.HasProducer && item.HasApplier && item.SupportsInsert && item.SupportsUpdate && item.SupportsDeactivate);
        SyncMasterBranchEntityCodes.InitialCatalog
            .Where(item => item.EntityCode is not "BusinessPartner" and not "Item" and not "Warehouse")
            .Should()
            .OnlyContain(item => !item.HasProducer && !item.HasApplier);
    }

    [Fact]
    public void SyncConfigurationApi_ExposesConfigurationEndpointsOnly()
    {
        var endpoints = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Api",
            "Endpoints",
            "SyncConfigurationEndpoints.cs");

        endpoints.Should().Contain("/api/sync/configuration/profiles");
        endpoints.Should().Contain("/api/sync/configuration/catalog");
        endpoints.Should().Contain("/api/sync/configuration/profiles/{id:int}/activate");
        endpoints.Should().Contain("/api/sync/configuration/profiles/{id:int}/deactivate");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationView");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationCreate");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationEdit");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationDelete");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationActivate");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationValidate");

        endpoints.Should().NotContain("/api/sync/configuration/profiles/{id:int}/run");
        endpoints.Should().NotContain("/api/sync/configuration/profiles/{id:int}/dispatch");
        endpoints.Should().NotContain("/api/sync/configuration/profiles/{id:int}/process");
    }

    [Fact]
    public void SyncConfigurationScript_AddsPaginationDeleteCatalogAndPermissions()
    {
        var script = ReadDatabaseScript("069_sync_master_branch_configuration.sql");
        var permissions = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Shared",
            "Constants",
            "PermissionCodes.cs");

        script.Should().Contain("SP_NA_GET_SYNCPROFILEPAGINAR");
        script.Should().Contain("SP_NA_GET_SYNCCONFIGURATIONCOMPANYLOOKUPS");
        script.Should().Contain("SP_NA_DELETE_SYNCPROFILEELIMINAR");
        script.Should().Contain("SP_NA_POST_SYNCPROFILEAUDITREGISTRAR");
        script.Should().Contain("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
        script.Should().Contain("SYNC.CONFIGURATION.VIEW");
        script.Should().Contain("RolePermissions");

        permissions.Should().Contain("SyncConfigurationView");
        permissions.Should().Contain("SyncConfigurationCreate");
        permissions.Should().Contain("SyncConfigurationEdit");
        permissions.Should().Contain("SyncConfigurationDelete");
        permissions.Should().Contain("SyncConfigurationActivate");
        permissions.Should().Contain("SyncConfigurationValidate");
    }

    [Fact]
    public void SyncRoutingScript_DefinesConfigurableRoutingOverExistingOutboxTargets()
    {
        var script = ReadDatabaseScript("070_sync_master_branch_routing.sql");

        script.Should().Contain("SP_NA_GET_SYNCROUTINGTARGETS");
        script.Should().Contain("SP_NA_GET_SYNCPROFILEACTIVECONFLICTS");
        script.Should().Contain("IX_SyncProfiles_Routing");
        script.Should().Contain("IX_SyncProfileEntities_Routing");
        script.Should().Contain("IX_SyncProfileBranches_Routing");
        script.Should().Contain("IX_SyncProfileEntityBranches_Routing");
        script.Should().Contain("profile.IsActive = 1");
        script.Should().Contain("profile.Direction = N'MasterToBranch'");
        script.Should().Contain("profile.ExecutionMode = N'Incremental'");
        script.Should().Contain("profile.ConflictStrategy = N'MasterWins'");
        script.Should().Contain("COALESCE(matrix.BatchSize, entity.BatchSize, profileBranch.BatchSize, profile.BatchSize)");
        script.Should().Contain("COALESCE(profileBranch.MaxRetries, profile.MaxRetries)");
        script.Should().Contain("OPENJSON(@CombinationsJson)");
        script.Should().Contain("N'BusinessPartner', N'Item', N'Warehouse'");

        script.Should().NotContain("CREATE TABLE dbo.SyncOutbox");
        script.Should().NotContain("CREATE TABLE dbo.SyncOutboxTargets");
        script.Should().NotContain("ALTER TABLE dbo.SyncOutboxTargets ADD");
        script.Should().NotContain("EXEC(@");
        script.Should().NotContain("sp_executesql");
        script.Should().NotContain("ConnectionString");
        script.Should().NotContain("Password");
    }

    [Fact]
    public void SyncRoutingRepository_UsesStoredProceduresAndDoesNotLoadAggregates()
    {
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncRoutingRepository.cs");
        var registration = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "DependencyInjection",
            "PersistenceServiceRegistration.cs");
        var publisher = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Application",
            "Features",
            "Sync",
            "Services",
            "SyncEventPublisher.cs");

        repository.Should().Contain("SP_NA_GET_SYNCROUTINGTARGETS");
        repository.Should().Contain("SP_NA_GET_SYNCPROFILEACTIVECONFLICTS");
        repository.Should().Contain("CommandType.StoredProcedure");
        repository.Should().NotContain("GetByIdAsync");
        registration.Should().Contain("services.AddScoped<ISyncRoutingRepository, SyncRoutingRepository>();");
        publisher.Should().Contain("ISyncRoutingService");
        publisher.Should().Contain("CreateTargetAsync");
        publisher.Should().Contain("MaxAttemptsFromRetries");
    }

    [Fact]
    public void SyncProfileExecutionScript_DefinesAdministrativeExecutionOnly()
    {
        var script = ReadDatabaseScript("071_sync_profile_execution.sql");
        var permissions = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Shared",
            "Constants",
            "PermissionCodes.cs");

        script.Should().Contain("CREATE TABLE dbo.SyncProfileExecutions");
        script.Should().Contain("CREATE TABLE dbo.SyncProfileExecutionDetails");
        script.Should().Contain("SP_NA_CREATE_SYNCPROFILEEXECUTION");
        script.Should().Contain("SP_NA_GET_PENDING_SYNCPROFILEEXECUTIONS");
        script.Should().Contain("SP_NA_GET_DUE_SYNCPROFILES");
        script.Should().Contain("SP_NA_GET_SYNCROUTINGTARGETS");
        script.Should().Contain("@SyncProfileId int = NULL");
        script.Should().Contain("profile.ExecutionMode IN (N'Incremental', N'Full', N'Manual')");
        script.Should().Contain("SYNC.CONFIGURATION.EXECUTE");
        script.Should().Contain("SYNC.CONFIGURATION.VIEWEXECUTIONS");
        script.Should().Contain("SYNC.CONFIGURATION.CANCEL");
        script.Should().Contain("SYNC.CONFIGURATION.RETRY");

        script.Should().NotContain("CREATE TABLE dbo.SyncOutbox");
        script.Should().NotContain("CREATE TABLE dbo.SyncInbox");
        script.Should().NotContain("CREATE TABLE dbo.SyncProfileExecutionErrors");
        script.Should().NotContain("ConnectionString");
        script.Should().NotContain("Password");

        permissions.Should().Contain("SyncConfigurationExecute");
        permissions.Should().Contain("SyncConfigurationViewExecutions");
        permissions.Should().Contain("SyncConfigurationCancel");
        permissions.Should().Contain("SyncConfigurationRetry");
    }

    [Fact]
    public void SyncProfileExecutionApi_ExposesManualExecutionEndpoints()
    {
        var endpoints = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Api",
            "Endpoints",
            "SyncConfigurationEndpoints.cs");
        var hostedService = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Api",
            "Services",
            "SyncProfileExecutionHostedService.cs");

        endpoints.Should().Contain("/api/sync/configuration/profiles/{id:int}/execute");
        endpoints.Should().Contain("/api/sync/configuration/executions");
        endpoints.Should().Contain("/api/sync/configuration/executions/{id:int}/cancel");
        endpoints.Should().Contain("/api/sync/configuration/executions/{id:int}/retry");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationExecute");
        endpoints.Should().Contain("PermissionCodes.SyncConfigurationViewExecutions");
        hostedService.Should().Contain("ISyncProfileExecutionService");
        hostedService.Should().Contain("ISyncScheduleCalculator");
        hostedService.Should().NotContain("SyncInbox");
        hostedService.Should().NotContain("CreateTargetAsync");
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
