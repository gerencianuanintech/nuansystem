using System.Text.Json;
using FluentAssertions;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncWinFormsContractTests
{
    [Fact]
    public void Models_DeserializeSafeProfileAndExecutionContracts()
    {
        const string profileJson = """{"items":[{"id":7,"companyCode":"DEMO","code":"SAP-WH","name":"Bodegas","isActive":false,"activeEntityCount":1,"rowVersion":"AQIDBA=="}],"totalCount":1,"pageNumber":1,"pageSize":50}""";
        const string executionJson = """{"id":9,"executionUid":"11111111-1111-1111-1111-111111111111","profileCode":"SAP-WH","profileName":"Bodegas","companyCode":"DEMO","entityCode":"Warehouses","direction":"SapToErp","triggerType":"Scheduled","status":"Completed","requestedAtUtc":"2026-07-31T12:00:00Z","totalRecords":2,"rowVersion":"AQIDBA=="}""";
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        JsonSerializer.Deserialize<SapPagedResult<SapSyncProfileListItem>>(profileJson, options)!.Items.Single().Code.Should().Be("SAP-WH");
        JsonSerializer.Deserialize<SapSyncExecutionDetail>(executionJson, options)!.EntityCode.Should().Be("Warehouses");
        typeof(SapSyncExecutionDetail).GetProperties().Select(property => property.Name)
            .Should().NotContain(["ProfileSnapshotJson", "EffectiveParametersJson", "ApprovedSnapshotJson", "SnapshotHash", "Password", "ConnectionString"]);
    }

    [Fact]
    public void EditorState_UsesOnlyImplementedCapabilitiesAndPersistsSelectedEntities()
    {
        var catalog = new SapSyncProfileCatalog
        {
            Companies = [new(1, "DEMO", "Empresa Demo")],
            Entities =
            [
                new() { EntityCode = "Warehouses", DisplayName = "Bodegas", IsActive = true, IsImplemented = true, SupportsSapToErp = true, SupportsFull = true },
                new() { EntityCode = "PurchaseOrders", DisplayName = "Ordenes", IsActive = true, IsImplemented = false, SupportsSapToErp = true, SupportsFull = true }
            ]
        };

        var state = SapSyncProfileEditorState.Create(catalog);
        state.Code = "SAP-WH";
        state.Name = "Bodegas";
        state.Entities.Single().IsActive = true;
        state.Entities.Single().ScheduleIsActive = true;

        state.Entities.Should().ContainSingle(item => item.EntityCode == "Warehouses");
        state.ToRequest().Entities.Should().ContainSingle(item => item.EntityCode == "Warehouses" && item.Schedule.IsActive);
    }

    [Fact]
    public async Task Client_UsesIndependentSapRoutesAndDeleteRowVersionBody()
    {
        var api = new CapturingApiClient();
        var client = new SapSyncManagementClient(api);

        await client.SearchProfilesAsync(new SapSyncProfileListFilter { Search = "Bodegas Demo", EntityCode = "Warehouses" });
        api.LastPath.Should().StartWith("/api/sap/sync-profiles?").And.Contain("search=Bodegas%20Demo").And.Contain("entityCode=Warehouses");

        await client.DeleteProfileAsync(42, [1, 2, 3, 4, 5, 6, 7, 8]);
        api.LastPath.Should().Be("/api/sap/sync-profiles/42");
        api.LastRequest.Should().BeOfType<SapSyncProfileVersionRequest>();
    }

    [Fact]
    public void Navigation_UsesIndependentFormKeysAndDoesNotExposeUnimplementedExecuteAction()
    {
        var main = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var program = Read("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var profiles = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileListForm.cs");

        main.Should().Contain("\"sap-sync-profiles\" => sapSyncProfileListFormFactory()")
            .And.Contain("\"sap-sync-executions\" => sapSyncExecutionListFormFactory()");
        program.Should().Contain("SapSyncManagementClient").And.Contain("CreateSapSyncProfileListForm").And.Contain("CreateSapSyncExecutionListForm");
        profiles.Should().Contain("\"execute\" => false");
    }

    [Fact]
    public void RibbonBuiltInOperationResolution_PrefersCanonicalUiAliasEvenWhenDenied()
    {
        var main = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var resolveOperation = main[main.IndexOf("private static FormOperationAccessItem? ResolveOperation", StringComparison.Ordinal)..];
        resolveOperation = resolveOperation[..resolveOperation.IndexOf("private void MoveRibbonButton", StringComparison.Ordinal)];

        resolveOperation.Should().NotContain(".Where(operation => operation.IsAllowed)")
            .And.Contain("foreach (var key in keys)")
            .And.Contain(".OrderBy(operation => operation.DisplayOrder)")
            .And.Contain("return operation;");
    }

    [Fact]
    public void Migration160_RegistersMaintenanceMonitorMenusAndAdminOperationsIdempotently()
    {
        var sql = Read("database", "sql", "160_master_sap_sync_winforms_navigation.sql");
        sql.Should().Contain("N'20260731.160'")
            .And.Contain("N'sap-sync-profiles',1,1")
            .And.Contain("N'sap-sync-executions',3,0")
            .And.Contain("N'MENU.ADMINISTRATION.INTEGRATIONS.SAP.PROFILES'")
            .And.Contain("N'MENU.ADMINISTRATION.INTEGRATIONS.SAP.EXECUTIONS'")
            .And.Contain("N'ACTION.SAP_SYNC_PROFILES.VALIDATE'")
            .And.Contain("N'ACTION.SAP_SYNC_EXECUTIONS.RELEASE_EXPIRED_LOCK'")
            .And.NotContain("@ProfilesFormId,N'ACTION.SAP_SYNC_PROFILES.EXECUTE'");
    }

    [Fact]
    public void Migration161_MapsSpecializedExecutionActionsToCorporateRibbonIcons()
    {
        var sql = Read("database", "sql", "161_master_sap_sync_execution_ribbon_icons.sql");
        var initializer = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        var expected = new[]
        {
            (Operation: "ACTION.SAP_SYNC_EXECUTIONS.RETRY", Icon: "reintentar_ejecucion"),
            (Operation: "ACTION.SAP_SYNC_EXECUTIONS.CANCEL", Icon: "cancelar_ejecucion"),
            (Operation: "ACTION.SAP_SYNC_EXECUTIONS.RELEASE_EXPIRED_LOCK", Icon: "liberar_lock_vencido")
        };

        sql.Should().Contain("N'20260801.161'")
            .And.Contain("N'20260731.160'")
            .And.NotContain("SecurityRoleFormOperations")
            .And.NotContain("RolePermissions");
        initializer.Should().Contain("161_master_sap_sync_execution_ribbon_icons.sql");

        foreach (var item in expected)
        {
            sql.Should().Contain($"N'{item.Operation}'")
                .And.Contain($"N'Ribbon/{item.Icon}_32.svg'")
                .And.Contain($"N'Ribbon/{item.Icon}_16.svg'");

            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Assets", "Icons", "Ribbon", $"{item.Icon}_32.svg")
                .Should().Contain("width=\"32\"").And.Contain("stroke=\"#00B894\"");
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Assets", "Icons", "Ribbon", $"{item.Icon}_16.svg")
                .Should().Contain("width=\"16\"").And.Contain("stroke=\"#00B894\"");
        }
    }

    [Fact]
    public void Designers_AreExplicitAndKeepSapCaptions()
    {
        var files = new[]
        {
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileEditForm.Designer.cs"),
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionDetailForm.Designer.cs"),
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionFilterDialog.Designer.cs")
        };
        files.Should().OnlyContain(file => !file.Contains("foreach", StringComparison.Ordinal)
            && !file.Contains("BuildLayout", StringComparison.Ordinal)
            && !file.Contains("ConfigureColumn", StringComparison.Ordinal)
            && !file.Contains("ConfigureButton", StringComparison.Ordinal)
            && !file.Contains("AddRange(new[]", StringComparison.Ordinal));
        files[0].Should().Contain("Entidades y programacion");
        files[0].Should().Contain("Datos generales")
            .And.Contain("Entidades y programacion")
            .And.NotContain("XtraTabControl")
            .And.NotContain("XtraTabPage")
            .And.Contain("btnCancelar.Location = new Point(936, 672)")
            .And.Contain("btnGuardar.Location = new Point(1042, 672)");
        files[1].Should().Contain("Detalle de ejecucion SAP");
        files[2].Should().Contain("Filtrar ejecuciones SAP");
    }

    [Fact]
    public void SapForms_UseCorporatePagingOperationalAndLookupContracts()
    {
        var profileList = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileListForm.cs");
        var profileEdit = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileEditForm.Designer.cs");
        var profileFilter = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileFilterDialog.Designer.cs");
        var executionList = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionListForm.cs");
        var executionListDesigner = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionListForm.Designer.cs");
        var executionDetail = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionDetailForm.cs");
        var executionDetailDesigner = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionDetailForm.Designer.cs");

        profileList.Should().Contain("EnableServerPaging(50)")
            .And.Contain("NuanGrid.PageRequested")
            .And.Contain("SetPagedGridData(");
        profileEdit.Should().Contain("NuanLookupEdit companyEdit")
            .And.Contain("companyEdit.Location = new Point(160, 50)")
            .And.Contain("codeEdit.Location = new Point(700, 50)")
            .And.Contain("nameEdit.Location = new Point(160, 78)")
            .And.Contain("descriptionEdit.Location = new Point(700, 78)");
        profileFilter.Should().Contain("AutoScaleMode = AutoScaleMode.Font");

        executionList.Should().Contain("SapSyncExecutionListForm : BaseCrudListForm")
            .And.NotContain("SapSyncExecutionListForm : BaseGridCrudListForm")
            .And.Contain("executionGrid.PageRequested")
            .And.Contain("executionGrid.SetPagedData(")
            .And.Contain("RunWithBusyStateAsync");
        executionListDesigner.Should().Contain("NuanDataGridControl executionGrid")
            .And.Contain("ShowPagination = true")
            .And.NotContain("new GridControl");

        executionDetail.Should().Contain("detailGrid.PageRequested")
            .And.Contain("detailGrid.SetPagedData(")
            .And.Contain("GoToDetailPageAsync");
        executionDetailDesigner.Should().Contain("NuanDataGridControl detailGrid")
            .And.Contain("GridName = \"Details\"")
            .And.NotContain("new GridControl")
            .And.NotContain("GridView detailView");
    }

    [Fact]
    public void SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing()
    {
        var actionButton = Read("src", "Frontend", "NuanSystem.WinForms.Controls", "Buttons", "NuanActionButton.cs");
        var baseEdit = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Common", "BaseEditForm.Designer.cs");
        var profileEdit = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileEditForm.Designer.cs");

        actionButton.Should().Contain("private const int DefaultWidth = 100;")
            .And.Contain("private const int DefaultHeight = 36;");
        baseEdit.Should().Contain("btnCancelar.Size = new Size(100, 36)")
            .And.Contain("btnGuardar.Size = new Size(100, 36)")
            .And.Contain("btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right")
            .And.Contain("btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right");
        profileEdit.Should().Contain("btnCancelar.Location = new Point(936, 672)")
            .And.Contain("btnGuardar.Location = new Point(1042, 672)")
            .And.NotContain("btnCancelar.Size =")
            .And.NotContain("btnGuardar.Size =");
    }

    [Fact]
    public void SapExecutionFilter_UsesSriDialogActionContract()
    {
        var sriFilter = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "SyncSRI", "SriTxtImports", "SriTxtImportFilterDialog.Designer.cs");
        var sriMonitorFilter = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "SyncSRI", "SriDocuments", "SriDocumentMonitorFilterDialog.Designer.cs");
        var sapFilter = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionFilterDialog.Designer.cs");
        var sapFilterCode = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionFilterDialog.cs");

        sriFilter.Should().Contain("btnApply.Size = new Size(100, 36)")
            .And.Contain("btnClear.Size = new Size(100, 36)")
            .And.Contain("btnCancel.Size = new Size(100, 36)");
        sriMonitorFilter.Should().Contain("btnApply.Size=new Size(100,36)")
            .And.Contain("btnClear.Size=new Size(100,36)")
            .And.Contain("btnCancel.Size=new Size(100,36)");
        sapFilter.Should().Contain("acceptButton.ButtonKind = NuanActionButtonKind.Save")
            .And.Contain("acceptButton.Size = new Size(100, 36)")
            .And.Contain("clearButton.Size = new Size(100, 36)")
            .And.Contain("cancelActionButton.Size = new Size(100, 36)")
            .And.Contain("cancelActionButton.Location = new Point(93, 196)")
            .And.Contain("clearButton.Location = new Point(199, 196)")
            .And.Contain("acceptButton.Location = new Point(305, 196)");
        sapFilterCode.Should().Contain("ClearButton_Click")
            .And.Contain("directionEdit.SelectedIndex = 0")
            .And.Contain("statusEdit.SelectedIndex = 0")
            .And.Contain("triggerEdit.SelectedIndex = 0")
            .And.Contain("DialogResult = DialogResult.OK");
    }

    [Fact]
    public void SapProfileFilter_UsesCompleteDialogActionContract()
    {
        var profileFilter = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileFilterDialog.Designer.cs");
        var profileFilterCode = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileFilterDialog.cs");

        profileFilter.Should().Contain("acceptButton.ButtonKind = NuanActionButtonKind.Save")
            .And.Contain("acceptButton.Size = new Size(100, 36)")
            .And.Contain("clearButton.Size = new Size(100, 36)")
            .And.Contain("cancelActionButton.Size = new Size(100, 36)")
            .And.Contain("cancelActionButton.Location = new Point(128, 116)")
            .And.Contain("clearButton.Location = new Point(234, 116)")
            .And.Contain("acceptButton.Location = new Point(340, 116)");
        profileFilterCode.Should().Contain("ClearButton_Click")
            .And.Contain("searchEdit.Text = string.Empty")
            .And.Contain("entityEdit.Text = string.Empty")
            .And.Contain("statusEdit.SelectedIndex = 0")
            .And.Contain("DialogResult = DialogResult.OK");
    }

    [Fact]
    public void AllFilterDialogs_UseCompleteCorporateActionContract()
    {
        var designers = new[]
        {
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncProfileFilterDialog.Designer.cs"),
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sap", "SapSyncExecutionFilterDialog.Designer.cs"),
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileFilterDialog.Designer.cs"),
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "SyncSRI", "SriDocuments", "SriDocumentMonitorFilterDialog.Designer.cs"),
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "SyncSRI", "SriTxtImports", "SriTxtImportFilterDialog.Designer.cs")
        };

        foreach (var designer in designers)
        {
            var compact = string.Concat(designer.Where(character => !char.IsWhiteSpace(character)));
            compact.Should().Contain("NuanActionButton")
                .And.Contain("ButtonText=\"Cancelar\"")
                .And.Contain("ButtonText=\"Limpiar\"")
                .And.Contain("ButtonText=\"Aplicar\"");
            (compact.Split("Size=newSize(100,36)", StringSplitOptions.None).Length - 1)
                .Should().BeGreaterThanOrEqualTo(3);
        }
    }

    private static string Read(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine([directory.FullName, "NuanSystem.sln"]))) directory = directory.Parent;
        directory.Should().NotBeNull();
        return File.ReadAllText(Path.Combine([directory!.FullName, .. parts]));
    }

    private sealed class CapturingApiClient : INuanApiClient
    {
        public string? LastPath { get; private set; }
        public object? LastRequest { get; private set; }
        public Task<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken = default)
        {
            LastPath = path;
            object response = typeof(TResponse).IsGenericType ? new SapPagedResult<SapSyncProfileListItem>([], 0, 1, 200) : new object();
            return Task.FromResult((TResponse)response);
        }
        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<TResponse> DeleteAsync<TResponse>(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<TResponse> DeleteAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default)
        {
            LastPath = path; LastRequest = request; return Task.FromResult((TResponse)(object)new SapSyncProfileWriteResult(42, false, []));
        }
        public Task<TResponse> PostFileAsync<TResponse>(string path, Stream content, string fileName, string formFieldName = "file", string contentType = "application/octet-stream", CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<ApiFileResponse> GetFileAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> IsAvailableAsync(string path = "/health", CancellationToken cancellationToken = default) => Task.FromResult(true);
    }
}
