using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncEntityDefinitionApiContractTests
{
    [Fact]
    public void Api_ShouldExposeThinMediatREndpointsWithIndependentPermissions()
    {
        var endpoints = ReadWorkspaceFile(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "SyncEntityDefinitionEndpoints.cs");
        var program = ReadWorkspaceFile("src", "Backend", "NuanSystem.Api", "Program.cs");
        var tags = ReadWorkspaceFile("src", "Backend", "NuanSystem.Api", "OpenApi", "SwaggerTags.cs");

        endpoints.Should().Contain("/api/sync/configuration/entities");
        endpoints.Should().Contain("{BaseRoute}/lookup");
        endpoints.Should().Contain("{BaseRoute}/{{id:int}}");
        endpoints.Should().Contain("GetSyncEntityDefinitionsQuery");
        endpoints.Should().Contain("GetSyncEntityDefinitionByIdQuery");
        endpoints.Should().Contain("GetSyncEntityDefinitionLookupQuery");
        endpoints.Should().Contain("GetSyncEntityDefinitionHistoryQuery");
        endpoints.Should().Contain("{BaseRoute}/{{id:int}}/history");
        endpoints.Should().Contain("CreateSyncEntityDefinitionCommand");
        endpoints.Should().Contain("UpdateSyncEntityDefinitionCommand");
        endpoints.Should().Contain("DeleteSyncEntityDefinitionCommand");
        endpoints.Should().Contain("PermissionCodes.SyncEntitiesView");
        endpoints.Should().Contain("PermissionCodes.SyncEntitiesCreate");
        endpoints.Should().Contain("PermissionCodes.SyncEntitiesEdit");
        endpoints.Should().Contain("PermissionCodes.SyncEntitiesDelete");
        endpoints.Should().Contain("SwaggerTags.SyncEntityDefinitions");
        endpoints.Should().NotContain("ISyncEntityDefinitionRepository");

        program.Should().Contain("app.MapSyncEntityDefinitionEndpoints();");
        tags.Should().Contain("Synchronization - Entity Definitions");
    }

    [Fact]
    public void SecurityScript_ShouldRegisterPermissionsAndFormWithoutPublishingMenu()
    {
        var script = ReadWorkspaceFile("database", "sql", "081_sync_entity_definition_api_security.sql");
        var permissions = ReadWorkspaceFile(
            "src", "Backend", "NuanSystem.Shared", "Constants", "PermissionCodes.cs");

        script.Should().Contain("SYNC.ENTITIES.VIEW");
        script.Should().Contain("SYNC.ENTITIES.CREATE");
        script.Should().Contain("SYNC.ENTITIES.EDIT");
        script.Should().Contain("SYNC.ENTITIES.DELETE");
        script.Should().Contain("FORM.ADMINISTRATION.SYNC.ENTITIES");
        script.Should().Contain("sync-entities");
        script.Should().Contain("SecurityRoleFormOperations");
        script.Should().Contain("RolePermissions");
        script.Should().Contain("ACTION.CUSTOMIZE_COLUMNS");
        script.Should().Contain("20260715.081");
        script.Should().NotContain("INSERT INTO dbo.SecurityMenus");
        script.Should().NotContain("MERGE dbo.SecurityMenus");

        permissions.Should().Contain("SyncEntitiesView");
        permissions.Should().Contain("SyncEntitiesCreate");
        permissions.Should().Contain("SyncEntitiesEdit");
        permissions.Should().Contain("SyncEntitiesDelete");
    }

    [Fact]
    public void DeploymentScripts_ShouldApplyAndDiagnoseEntitySecurity()
    {
        var installer = ReadWorkspaceFile("database", "sql", "074_apply_master_branch_sync_master.sql");
        var diagnostic = ReadWorkspaceFile("database", "sql", "076_check_master_branch_sync_installation.sql");

        installer.Should().Contain(":r 081_sync_entity_definition_api_security.sql");
        installer.Should().Contain(":r 082_sync_entity_definition_winforms.sql");
        diagnostic.Should().Contain("SYNC.ENTITIES.VIEW");
        diagnostic.Should().Contain("SYNC.ENTITIES.CREATE");
        diagnostic.Should().Contain("SYNC.ENTITIES.EDIT");
        diagnostic.Should().Contain("SYNC.ENTITIES.DELETE");
        diagnostic.Should().Contain("sync-entities");
        diagnostic.Should().Contain("SP_NA_GET_SYNCENTITYDEFINITIONHISTORIAL");
    }

    [Fact]
    public void WinFormsSecurityScript_ShouldPublishEntityMenuAndHistoryProcedure()
    {
        var script = ReadWorkspaceFile("database", "sql", "082_sync_entity_definition_winforms.sql");

        script.Should().Contain("SP_NA_GET_SYNCENTITYDEFINITIONHISTORIAL");
        script.Should().Contain("AuditSyncConfigurationChanges");
        script.Should().Contain("MENU.ADMINISTRATION.INTEGRATIONS.SYNC.ENTITIES");
        script.Should().Contain("sync-entities");
        script.Should().Contain("SecurityRoleMenus");
        script.Should().Contain("20260716.082");
    }

    private static string ReadWorkspaceFile(params string[] segments)
    {
        return File.ReadAllText(Path.Combine(FindWorkspaceRoot(), Path.Combine(segments)));
    }

    private static string FindWorkspaceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("No se encontro la raiz del workspace NuanSystem.");
    }
}
