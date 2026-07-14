using FluentAssertions;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncConfigurationFrontendContractTests
{
    [Fact]
    public void SyncConfigurationClient_ShouldUseOnlyConfigurationApiEndpoints()
    {
        var source = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "SyncConfigurationClient.cs");

        source.Should().Contain("/api/sync/configuration");
        source.Should().Contain("GetAsync<PagedResult<SyncProfileListItem>>");
        source.Should().Contain("GetAsync<SyncConfigurationCatalog>");
        source.Should().Contain("PostAsync<SaveSyncProfileRequest, SyncProfileDetail>");
        source.Should().Contain("PutAsync<SaveSyncProfileRequest, SyncProfileDetail>");
        source.Should().Contain("DeleteAsync<object>");
        source.Should().Contain("/profiles/{id}/execute");
        source.Should().Contain("/executions/{id}/cancel");
        source.Should().Contain("/executions/{id}/retry");

        source.Should().NotContain("SqlConnection");
        source.Should().NotContain("Dapper");
        source.Should().NotContain("SyncOutbox");
        source.Should().NotContain("SyncInbox");
        source.Should().NotContain("MasterBranchSyncWorker");
        source.Should().NotContain("/dispatch");
        source.Should().NotContain("/process");
        source.Should().NotContain("/claim");
        source.Should().NotContain("/apply");
    }

    [Fact]
    public void SyncConfigurationFrontend_ShouldBeRegisteredInShellProgramAndSecuritySeed()
    {
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var mainForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var shellViewModel = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");
        var securitySeed = ReadWorkspaceFile("database", "sql", "072_sync_configuration_winforms_security.sql");

        program.Should().Contain("SyncConfigurationClient");
        program.Should().Contain("CreateSyncProfileListForm");
        program.Should().Contain("CreateSyncExecutionListForm");

        mainForm.Should().Contain("\"sync-profiles\" => syncProfileListFormFactory()");
        mainForm.Should().Contain("\"sync-executions\" => syncExecutionListFormFactory()");

        shellViewModel.Should().Contain("\"sync-profiles\"");
        shellViewModel.Should().Contain("\"sync-executions\"");
        shellViewModel.Should().Contain("PermissionCodes.SyncConfigurationView");
        shellViewModel.Should().Contain("PermissionCodes.SyncConfigurationViewExecutions");

        securitySeed.Should().Contain("FORM.ADMINISTRATION.SYNC.PROFILES");
        securitySeed.Should().Contain("FORM.ADMINISTRATION.SYNC.EXECUTIONS");
        securitySeed.Should().Contain("MENU.ADMINISTRATION.INTEGRATIONS.SYNC.PROFILES");
        securitySeed.Should().Contain("MENU.ADMINISTRATION.INTEGRATIONS.SYNC.EXECUTIONS");
        securitySeed.Should().Contain("N'sync-profiles'");
        securitySeed.Should().Contain("N'sync-executions'");
        securitySeed.Should().Contain("SYNC.CONFIGURATION.EXECUTE");
        securitySeed.Should().Contain("SYNC.CONFIGURATION.RETRY");
        securitySeed.Should().Contain("INSERT INTO dbo.RolePermissions (RoleId, PermissionId)");
        securitySeed.Should().NotContain("MERGE dbo.RolePermissions");
    }

    [Fact]
    public void SyncConfigurationFrontend_ShouldNotExposePayloadsSecretsOrWorkerOperations()
    {
        var files = new[]
        {
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "Models", "SyncConfigurationModels.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "SyncConfigurationClient.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Sync", "SyncConfigurationViewModels.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileListForm.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionListForm.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionDetailForm.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "ExecuteSyncProfileDialog.cs")
        };

        var combined = string.Join(Environment.NewLine, files);

        combined.Should().NotContain("Password");
        combined.Should().NotContain("ConnectionString");
        combined.Should().NotContain("PayloadJson");
        combined.Should().NotContain("SqlConnection");
        combined.Should().NotContain("CommandType.StoredProcedure");
        combined.Should().NotContain("MasterBranchSyncWorker");
        combined.Should().NotContain("SyncProfileExecutionHostedService");
        combined.Should().NotContain("SyncOutbox");
        combined.Should().NotContain("SyncInbox");
        combined.Should().NotContain("/dispatch");
        combined.Should().NotContain("/process");
        combined.Should().NotContain("/claim");
        combined.Should().NotContain("/apply");
    }

    [Fact]
    public void SyncProfileEditorState_ShouldBuildSaveRequestWithBranchesEntitiesMatrixAndSchedule()
    {
        var catalog = new SyncConfigurationCatalog
        {
            MasterCompanies = [new CompanyLookupItem(1, "MST", "Matriz", true)],
            BranchCompanies = [new CompanyLookupItem(2, "BR1", "Sucursal 1", true)],
            Entities =
            [
                new SyncEntityCatalogItem
                {
                    Code = "Item",
                    Name = "Articulos",
                    DefaultExecutionOrder = 210,
                    SupportsIncremental = true,
                    SupportsInsert = true,
                    SupportsUpdate = true,
                    SupportsDeactivate = true,
                    DefaultKeyField = "Code",
                    DefaultModifiedAtField = "UpdatedAt"
                }
            ],
            Directions = [new LookupItem("MasterToBranch", "MasterToBranch")],
            ExecutionModes = [new LookupItem("Incremental", "Incremental")],
            ConflictStrategies = [new LookupItem("MasterWins", "MasterWins")],
            ScheduleTypes = [new LookupItem("Interval", "Interval")]
        };

        var state = SyncProfileEditorState.CreateNew(catalog);
        state.Code = "ITEMS";
        state.Name = "Items";
        state.AddBranch(catalog.BranchCompanies.Single());
        state.AddEntityFromCatalog(catalog.Entities.Single());
        state.Schedule.ScheduleType = "Interval";
        state.Schedule.IntervalMinutes = 10;

        var request = state.ToRequest();

        request.Code.Should().Be("ITEMS");
        request.CompanyId.Should().Be(1);
        request.Branches.Should().ContainSingle(branch => branch.BranchCompanyId == 2);
        request.Entities.Should().ContainSingle(entity => entity.EntityCode == "Item" && entity.Branches.Count == 1);
        request.Schedule.Should().NotBeNull();
        request.Schedule!.ScheduleType.Should().Be("Interval");
        request.Schedule.IntervalMinutes.Should().Be(10);
    }

    [Fact]
    public void SyncProfileEditorState_ShouldNotSendManualScheduleTimeOrInterval()
    {
        var state = SyncProfileEditorState.CreateNew();
        state.Schedule.ScheduleType = "Manual";
        state.Schedule.IntervalMinutes = 15;
        state.Schedule.ExecutionTime = new TimeSpan(8, 30, 0);

        var request = state.ToRequest();

        request.Schedule.Should().NotBeNull();
        request.Schedule!.ScheduleType.Should().Be("Manual");
        request.Schedule.IntervalMinutes.Should().BeNull();
        request.Schedule.ExecutionTime.Should().BeNull();
    }

    [Fact]
    public void SyncExecutionForms_ShouldUseBackendExecutionStatuses()
    {
        var listForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionListForm.cs");
        var listDesigner = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionListForm.Designer.cs");
        var detailForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionDetailForm.cs");
        var combined = string.Join(Environment.NewLine, listForm, listDesigner, detailForm);

        combined.Should().Contain("\"Pending\"");
        combined.Should().Contain("\"Running\"");
        combined.Should().Contain("\"Cancelling\"");
        combined.Should().Contain("\"CompletedWithErrors\"");
        combined.Should().NotContain("\"Queued\"");
        combined.Should().Contain("IsDisposed || Disposing");
    }

    [Fact]
    public void SyncConfigurationForms_ShouldBeDesignerBacked()
    {
        var formNames = new[]
        {
            "SyncProfileListForm",
            "SyncProfileEditForm",
            "SyncExecutionListForm",
            "SyncExecutionDetailForm",
            "ExecuteSyncProfileDialog"
        };

        foreach (var formName in formNames)
        {
            var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", $"{formName}.cs");
            var designer = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", $"{formName}.Designer.cs");
            var resxPath = Path.Combine(FindWorkspaceRoot(), "src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", $"{formName}.resx");

            form.Should().Contain($"partial class {formName}");
            form.Should().Contain($"public {formName}()");
            form.Should().Contain("InitializeComponent();");
            form.Should().Contain("IsInDesignMode()");
            form.Should().NotContain("BuildLayout");
            form.Should().NotContain("BuildUi");
            form.Should().NotContain("CreateControls");
            form.Should().NotContain("CreateTabs");
            form.Should().NotContain("new GridControl()");
            form.Should().NotContain("new GridView()");
            form.Should().NotContain("new XtraTabControl");

            designer.Should().Contain($"partial class {formName}");
            designer.Should().Contain("private void InitializeComponent()");
            designer.Should().Contain("components?.Dispose();");
            designer.Should().NotContain("AddRange([");
            designer.Should().NotContain("HttpClient");
            designer.Should().NotContain("SqlConnection");
            designer.Should().NotContain("Dapper");
            File.Exists(resxPath).Should().BeTrue($"{formName} debe tener .resx para Visual Studio Designer");
        }
    }

    [Fact]
    public void SyncConfigurationPollingForms_ShouldNotStartTimersInDesigner()
    {
        var listForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionListForm.cs");
        var listDesigner = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionListForm.Designer.cs");
        var detailForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionDetailForm.cs");
        var detailDesigner = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionDetailForm.Designer.cs");

        listForm.Should().Contain("if (IsInDesignMode() || viewModel is null)");
        detailForm.Should().Contain("if (IsInDesignMode() || viewModel is null)");
        listForm.Should().Contain("pollingTimer.Start();");
        detailForm.Should().Contain("pollingTimer.Start();");
        listDesigner.Should().Contain("pollingTimer.Enabled = false;");
        detailDesigner.Should().Contain("pollingTimer.Enabled = false;");
        listDesigner.Should().Contain("pollingTimer.Interval = 7000;");
        detailDesigner.Should().Contain("pollingTimer.Interval = 7000;");
    }

    [Fact]
    public void SyncHardeningScript_ShouldReserveProfileExecutionAtomically()
    {
        var script = ReadWorkspaceFile("database", "sql", "073_sync_master_branch_hardening.sql");

        script.Should().Contain("SP_NA_CREATE_SYNCPROFILEEXECUTION");
        script.Should().Contain("BEGIN TRANSACTION");
        script.Should().Contain("WITH (UPDLOCK, HOLDLOCK)");
        script.Should().Contain("Status IN (N'Pending', N'Running', N'Cancelling')");
        script.Should().Contain("ROLLBACK TRANSACTION");
        script.Should().Contain("COMMIT TRANSACTION");
        script.Should().Contain("20260711.073");

        script.Should().NotContain("CREATE TABLE dbo.SyncOutbox");
        script.Should().NotContain("CREATE TABLE dbo.SyncInbox");
        script.Should().NotContain("CREATE TABLE dbo.SyncOutboxTargets");
    }

    [Fact]
    public void SyncDeploymentScripts_ShouldApplyMasterAndTenantBatches()
    {
        var master = ReadWorkspaceFile("database", "sql", "074_apply_master_branch_sync_master.sql");
        var tenant = ReadWorkspaceFile("database", "sql", "075_apply_master_branch_sync_tenant.sql");
        var check = ReadWorkspaceFile("database", "sql", "076_check_master_branch_sync_installation.sql");

        master.Should().Contain(":ON ERROR EXIT");
        master.Should().Contain(":r 069_sync_master_branch_configuration.sql");
        master.Should().Contain(":r 070_sync_master_branch_routing.sql");
        master.Should().Contain(":r 071_sync_profile_execution.sql");
        master.Should().Contain(":r 072_sync_configuration_winforms_security.sql");
        master.Should().Contain(":r 073_sync_master_branch_hardening.sql");
        master.Should().Contain("NuanSystem_Master");
        master.Should().Contain("SP_NA_CREATE_SYNCPROFILEEXECUTION");
        master.Should().NotContain("SyncCheckpoints");

        tenant.Should().Contain(":ON ERROR EXIT");
        tenant.Should().Contain(":r 065_tenant_sync_inbox_local_outbox.sql");
        tenant.Should().Contain("SyncInbox");
        tenant.Should().Contain("LocalOutbox");
        tenant.Should().Contain("SyncAudit");

        check.Should().Contain("No modifica datos");
        check.Should().Contain("SyncProfiles");
        check.Should().Contain("SyncInbox");
        check.Should().NotContain("SyncCheckpoints");
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

        throw new DirectoryNotFoundException("No se encontro NuanSystem.sln desde el directorio de pruebas.");
    }
}
