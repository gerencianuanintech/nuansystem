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
            && !file.Contains("ConfigureButton", StringComparison.Ordinal));
        files[0].Should().Contain("Entidades y programacion");
        files[1].Should().Contain("Detalle de ejecucion SAP");
        files[2].Should().Contain("Filtrar ejecuciones SAP");
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
