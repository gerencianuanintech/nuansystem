using FluentAssertions;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncConfigurationContractTests
{
    [Fact]
    public void CountrySyncScripts_DefineGlobalIdentityWithoutEnablingProfiles()
    {
        var tenantScript = ReadDatabaseScript("083_tenant_country_master_branch_sync.sql");
        var masterScript = ReadDatabaseScript("084_master_country_sync_registration.sql");

        tenantScript.Should().Contain("GlobalId uniqueidentifier");
        tenantScript.Should().Contain("UX_Countries_GlobalId");
        tenantScript.Should().Contain("SP_NA_GET_COUNTRIES_LISTAR");
        tenantScript.Should().Contain("SP_NA_POST_COUNTRIES_CREAR");
        tenantScript.Should().Contain("SP_NA_PUT_COUNTRIES_ACTUALIZAR");
        tenantScript.Should().Contain("SP_NA_DELETE_COUNTRIES_ELIMINAR");
        tenantScript.Should().NotContain("Provinces");
        tenantScript.Should().NotContain("Cities");

        masterScript.Should().Contain("WHERE Code = N'Countries'");
        masterScript.Should().Contain("SupportsInsert = 1");
        masterScript.Should().Contain("SupportsUpdate = 1");
        masterScript.Should().Contain("SupportsDeactivate = 1");
        masterScript.Should().Contain("CONVERT(bit, 0)");
        masterScript.Should().NotContain("UPDATE dbo.SyncProfiles");
    }

    [Fact]
    public void ProvinceSyncScripts_DefineParentIdentityWithoutEnablingProfiles()
    {
        var tenantScript = ReadDatabaseScript("085_tenant_province_master_branch_sync.sql");
        var masterScript = ReadDatabaseScript("086_master_province_sync_registration.sql");

        tenantScript.Should().Contain("GlobalId uniqueidentifier");
        tenantScript.Should().Contain("UX_Provinces_GlobalId");
        tenantScript.Should().Contain("country.GlobalId AS CountryGlobalId");
        tenantScript.Should().Contain("SP_NA_POST_PROVINCES_CREAR");
        tenantScript.Should().Contain("SP_NA_PUT_PROVINCES_ACTUALIZAR");
        tenantScript.Should().Contain("SP_NA_DELETE_PROVINCES_ELIMINAR");
        tenantScript.Should().NotContain("dbo.Cities");

        masterScript.Should().Contain("WHERE Code = N'Provinces'");
        masterScript.Should().Contain("SupportsInsert = 1");
        masterScript.Should().Contain("SupportsUpdate = 1");
        masterScript.Should().Contain("SupportsDeactivate = 1");
        masterScript.Should().Contain("CONVERT(bit, 0)");
        masterScript.Should().NotContain("UPDATE dbo.SyncProfiles");
    }

    [Fact]
    public void CitySyncScripts_DefineBothParentIdentitiesWithoutEnablingProfiles()
    {
        var tenantScript = ReadDatabaseScript("087_tenant_city_master_branch_sync.sql");
        var masterScript = ReadDatabaseScript("088_master_city_sync_registration.sql");

        tenantScript.Should().Contain("UX_Cities_GlobalId");
        tenantScript.Should().Contain("country.GlobalId AS CountryGlobalId");
        tenantScript.Should().Contain("province.GlobalId AS ProvinceGlobalId");
        tenantScript.Should().Contain("province.CountryId = country.CountryId");
        tenantScript.Should().Contain("SP_NA_POST_CITIES_CREAR");
        tenantScript.Should().Contain("SP_NA_PUT_CITIES_ACTUALIZAR");
        tenantScript.Should().Contain("SP_NA_DELETE_CITIES_ELIMINAR");

        masterScript.Should().Contain("WHERE Code = N'Cities'");
        masterScript.Should().Contain("SupportsInsert = 1");
        masterScript.Should().Contain("SupportsUpdate = 1");
        masterScript.Should().Contain("SupportsDeactivate = 1");
        masterScript.Should().Contain("CONVERT(bit, 0)");
        masterScript.Should().NotContain("UPDATE dbo.SyncProfiles");
    }

    [Fact]
    public void CurrencySyncScripts_DefineGlobalIdentityWithoutEnablingProfiles()
    {
        var tenantScript = ReadDatabaseScript("090_tenant_currency_master_branch_sync.sql");
        var masterScript = ReadDatabaseScript("091_master_currency_sync_registration.sql");

        tenantScript.Should().Contain("GlobalId uniqueidentifier");
        tenantScript.Should().Contain("UX_Currencies_GlobalId");
        tenantScript.Should().Contain("SP_NA_GET_CURRENCIES_LISTAR");
        tenantScript.Should().Contain("SP_NA_POST_CURRENCIES_CREAR");
        tenantScript.Should().Contain("SP_NA_PUT_CURRENCIES_ACTUALIZAR");
        tenantScript.Should().Contain("SP_NA_DELETE_CURRENCIES_ELIMINAR");

        masterScript.Should().Contain("WHERE Code = N'Currencies'");
        masterScript.Should().Contain("SupportsInsert = 1");
        masterScript.Should().Contain("SupportsUpdate = 1");
        masterScript.Should().Contain("SupportsDeactivate = 1");
        masterScript.Should().Contain("CONVERT(bit, 0)");
        masterScript.Should().NotContain("UPDATE dbo.SyncProfiles");
    }

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
    public void SyncProfilePersistenceScripts_ShouldAcceptTheOperativeEntityCatalog()
    {
        var baseScript = ReadDatabaseScript("069_sync_master_branch_configuration.sql");
        var alignmentScript = ReadDatabaseScript("079_sync_profile_entity_catalog_alignment.sql");
        const string operativeCatalog = "N'SupplyMethods', N'BusinessPartner', N'ItemGroups', N'Item', N'Warehouse'";

        baseScript.Should().Contain(operativeCatalog);
        alignmentScript.Should().Contain("SP_NA_PUT_SYNCPROFILEACTUALIZAR");
        alignmentScript.Should().Contain(operativeCatalog);
        alignmentScript.Should().Contain("20260715.079");
    }

    [Fact]
    public void SyncEntityDefinitionScript_ShouldProvideAnAuditableMasterCatalog()
    {
        var script = ReadDatabaseScript("080_sync_entity_definitions.sql");

        script.Should().Contain("CREATE TABLE dbo.SyncEntityDefinitions");
        script.Should().Contain("CREATE TABLE dbo.SyncEntityDefinitionDependencies");
        script.Should().Contain("CREATE TABLE dbo.AuditSyncConfigurationChanges");
        script.Should().Contain("SP_NA_GET_SYNCENTITYDEFINITIONPAGINAR");
        script.Should().Contain("SP_NA_GET_SYNCENTITYDEFINITIONBUSCARPORID");
        script.Should().Contain("SP_NA_GET_SYNCENTITYDEFINITIONLOOKUP");
        script.Should().Contain("@IncludeInactive bit = 0");
        script.Should().Contain("dependency.EntityDefinitionId");
        script.Should().Contain("SP_NA_POST_SYNCENTITYDEFINITIONCREAR");
        script.Should().Contain("SP_NA_PUT_SYNCENTITYDEFINITIONACTUALIZAR");
        script.Should().Contain("SP_NA_DELETE_SYNCENTITYDEFINITIONELIMINAR");
        script.Should().Contain("FK_SyncProfileEntities_EntityDefinition");
        script.Should().Contain("definition.IsDeleted = 0");
        script.Should().Contain("definition.IsActive = 1 OR entity.IsActive = 0");
        script.Should().Contain("IsSystem = 1");
        script.Should().Contain("20260715.080");

        script.Should().NotContain("TenantId");
        script.Should().NotContain("SELECT *");
        script.Should().NotContain("CustomSql");
    }

    [Fact]
    public void SyncEntityDefinitionRepository_ShouldUseMasterStoredProceduresAndTypedErrors()
    {
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "SyncEntityDefinitionRepository.cs");
        var registration = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "DependencyInjection",
            "PersistenceServiceRegistration.cs");

        repository.Should().Contain("IMasterConnectionFactory");
        repository.Should().Contain("CommandType.StoredProcedure");
        repository.Should().Contain("SP_NA_GET_SYNCENTITYDEFINITIONPAGINAR");
        repository.Should().Contain("SP_NA_GET_SYNCENTITYDEFINITIONLOOKUP");
        repository.Should().Contain("SP_NA_POST_SYNCENTITYDEFINITIONCREAR");
        repository.Should().Contain("SP_NA_PUT_SYNCENTITYDEFINITIONACTUALIZAR");
        repository.Should().Contain("SP_NA_DELETE_SYNCENTITYDEFINITIONELIMINAR");
        repository.Should().Contain("TryMapSqlError");
        repository.Should().NotContain("CommandType.Text");
        registration.Should().Contain("services.AddScoped<ISyncEntityDefinitionRepository, SyncEntityDefinitionRepository>();");
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
        SyncMasterBranchEntityCodes.Find(" warehouse ")?.EntityCode.Should().Be(SyncMasterBranchEntityCodes.Warehouse);
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.BusinessPartner).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.Countries).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.Provinces).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.Cities).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.Currencies).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.ItemGroups).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.ItemFamilies).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative("CustomCatalog").Should().BeFalse();
        SyncMasterBranchEntityCodes.InitialCatalog.Select(item => item.EntityCode).Should().BeEquivalentTo(
            "Countries",
            "Provinces",
            "Cities",
            "Currencies",
            "Tax",
            "UnitOfMeasure",
            "BusinessPartnerPaymentTerms",
            "SupplierGroups",
            "SupplierClasses",
            "EconomicActivities",
            "Zones",
            "SupplyMethods",
            "BusinessPartner",
            "ItemGroups",
            "ItemFamilies",
            "Item",
            "Warehouse",
            "PriceList",
            "PurchaseOrder");
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.PriceLists).Should().BeTrue();
        SyncMasterBranchEntityCodes.IsOperative(SyncMasterBranchEntityCodes.PurchaseOrder).Should().BeTrue();
        SyncMasterBranchEntityCodes.InitialCatalog.Should().OnlyContain(item => item.ExistsInModel);
        SyncMasterBranchEntityCodes.InitialCatalog
            .Where(item => item.IsOperative)
            .Should()
            .OnlyContain(item => item.HasProducer && item.HasApplier && item.SupportsInsert && item.SupportsUpdate && item.SupportsDeactivate);
        SyncMasterBranchEntityCodes.InitialCatalog
            .Where(item => !item.IsOperative)
            .Should()
            .OnlyContain(item => !item.HasProducer && !item.HasApplier);
    }

    [Fact]
    public void OperationalEntityRegistrations_ShouldUseTheSharedManifest()
    {
        var executionService = ReadSourceFile(
            "src", "Backend", "NuanSystem.Application", "Features", "Sync", "Execution", "Services", "SyncProfileExecutionService.cs");
        var publishers = string.Join(Environment.NewLine,
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "Geography", "Commands", "CountrySyncPublisher.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "Geography", "Commands", "ProvinceSyncPublisher.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "Geography", "Commands", "CitySyncPublisher.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "FinancialCatalogs", "Catalogs", "Commands", "CurrencySyncPublisher.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "BusinessPartners", "Commands", "BusinessPartnerSyncEventFactory.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "GeneralInventory", "ItemGroups", "Commands", "ItemGroupSyncEventFactory.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "Items", "Commands", "ItemSyncEventFactory.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.Application", "Features", "GeneralInventory", "Warehouses", "Commands", "WarehouseSyncEventFactory.cs"));
        var fullSources = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "SyncFullEntitySources.cs");
        var appliers = string.Join(Environment.NewLine,
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "CountrySyncEventApplier.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "ProvinceSyncEventApplier.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "CitySyncEventApplier.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "CurrencySyncEventApplier.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "BusinessPartnerSyncEventApplier.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "ItemGroupSyncEventApplier.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "ItemSyncEventApplier.cs"),
            ReadSourceFile("src", "Backend", "NuanSystem.MasterBranchSyncWorker", "Services", "WarehouseSyncEventApplier.cs"));

        executionService.Should().NotContain("SupportedFullSources");
        executionService.Should().Contain("entitySourcesByCode.Keys");
        executionService.Should().Contain("SyncMasterBranchEntityCodes.IsOperative(entity.EntityCode)");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.Countries");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.Provinces");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.Cities");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.Currencies");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.BusinessPartner");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.ItemGroups");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.Item");
        publishers.Should().Contain("SyncMasterBranchEntityCodes.Warehouse");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.BusinessPartner");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.ItemGroups");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.Item");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.Warehouse");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.Countries");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.Provinces");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.Cities");
        fullSources.Should().Contain("SyncMasterBranchEntityCodes.Currencies");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.Countries");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.Provinces");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.Cities");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.Currencies");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.BusinessPartner");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.ItemGroups");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.Item");
        appliers.Should().Contain("SyncMasterBranchEntityCodes.Warehouse");
    }

    [Fact]
    public void IterationEightCatalogIntegration_ShouldRegisterWritersMigrationsDependenciesAndDisabledAppliers()
    {
        var applicationRegistration = ReadSourceFile(
            "src", "Backend", "NuanSystem.Application", "DependencyInjection", "ApplicationServiceRegistration.cs");
        var tenantInitializer = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");
        var masterInitializer = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        var developmentSettings = ReadSourceFile(
            "src", "Backend", "NuanSystem.MasterBranchSyncWorker", "appsettings.Development.json");

        applicationRegistration.Should().Contain("AddScoped<IItemGroupLocalOutboxWriter, ItemGroupLocalOutboxWriter>()")
            .And.Contain("AddScoped<IWarehouseLocalOutboxWriter, WarehouseLocalOutboxWriter>()");
        tenantInitializer.Should().Contain("129_tenant_item_group_transactional_outbox.sql")
            .And.Contain("131_tenant_item_sync_payload_v2.sql")
            .And.Contain("133_tenant_warehouse_transactional_outbox.sql");
        masterInitializer.Should().Contain("130_master_item_group_sync_registration.sql")
            .And.Contain("132_master_item_unit_of_measure_dependency.sql")
            .And.Contain("134_master_warehouse_sync_registration.sql");
        developmentSettings.Should().Contain("\"Enabled\": false")
            .And.Contain("\"SkeletonMode\": true")
            .And.Contain("\"UnitOfMeasure\"")
            .And.Contain("\"ItemGroups\"")
            .And.Contain("\"ItemFamilies\"")
            .And.Contain("\"Item\"")
            .And.Contain("\"Warehouse\"");

        var unitOfMeasure = SyncMasterBranchEntityCodes.Find(SyncMasterBranchEntityCodes.UnitOfMeasures);
        unitOfMeasure.Should().NotBeNull();
        unitOfMeasure!.SupportsIncremental.Should().BeFalse();

        var item = SyncMasterBranchEntityCodes.Find(SyncMasterBranchEntityCodes.Item);
        item.Should().NotBeNull();
        item!.Dependencies.Should().Equal(
            SyncMasterBranchEntityCodes.ItemGroups,
            SyncMasterBranchEntityCodes.ItemFamilies,
            SyncMasterBranchEntityCodes.UnitOfMeasures);
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

    [Fact]
    public void DependencyEngineScript_SeedsFutureDefinitionsWithoutActivatingImplementations()
    {
        var script = ReadDatabaseScript("099_master_sync_dependency_engine.sql");

        script.Should().Contain("IF OBJECT_ID(N'dbo.SyncEntityDefinitions'");
        script.Should().Contain("WHERE NOT EXISTS");
        script.Should().Contain("N'Tax'");
        script.Should().Contain("N'UnitOfMeasure'");
        script.Should().Contain("N'PriceList'");
        script.Should().Contain("N'PurchaseOrder'");
        script.Should().Contain("(N'PriceList', N'Item')");
        script.Should().Contain("(N'PurchaseOrder', N'BusinessPartner')");
        script.Should().Contain("(N'PurchaseOrder', N'Warehouse')");
        script.Should().Contain("20260718.099");
        script.Should().NotContain("SyncEntityConfigurations");
        script.Should().NotContain("EntityOwnershipConfigurations");
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
