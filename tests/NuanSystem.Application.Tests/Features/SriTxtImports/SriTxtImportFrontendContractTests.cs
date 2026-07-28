using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SriTxtImports;
using NuanSystem.WinForms.Services.SriTxtImports.Models;
using NuanSystem.WinForms.ViewModels.SriTxtImports;

namespace NuanSystem.Application.Tests.Features.SriTxtImports;

public sealed class SriTxtImportFrontendContractTests
{
    [Fact]
    public void Models_DeserializePagedContractsWithoutSensitiveMembers()
    {
        const string json =
            """
            {
              "items": [{ "id": 7, "originalFileName": "fixture.txt", "status": "Validated", "rowVersion": "AQIDBA==" }],
              "totalCount": 1,
              "page": 1,
              "pageSize": 50,
              "summary": { "totalRows": 10, "validRows": 8, "invalidRows": 2, "linkedRows": 1, "stagedRows": 4, "pendingRows": 3 }
            }
            """;

        var page = JsonSerializer.Deserialize<SriTxtImportPage>(
            json,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        page.Should().NotBeNull();
        page!.Items.Single().OriginalFileName.Should().Be("fixture.txt");
        page.Summary.StagedRows.Should().Be(4);
        typeof(SriTxtImportDetail).GetProperties().Select(property => property.Name)
            .Should().NotContain(["AccessKey", "HeaderLine", "OriginalLine", "Xml", "Jwt", "ConnectionString"]);
        typeof(SriTxtImportRow).GetProperties().Select(property => property.Name)
            .Should().NotContain(["AccessKey", "HeaderLine", "OriginalLine", "Xml", "Jwt", "ConnectionString"]);
    }

    [Fact]
    public async Task Client_EscapesFiltersAndUsesServerPaging()
    {
        var api = new CapturingApiClient();
        var client = new SriTxtImportClient(api);

        await client.SearchAsync(
            new SriTxtImportFilter
            {
                FileName = "recibidos julio.txt",
                Environment = "Production",
                Status = "Validated",
                Page = 3,
                PageSize = 25
            });

        api.LastPath.Should().Contain("fileName=recibidos%20julio.txt");
        api.LastPath.Should().Contain("environment=Production");
        api.LastPath.Should().Contain("status=Validated");
        api.LastPath.Should().Contain("page=3");
        api.LastPath.Should().Contain("pageSize=25");
    }

    [Fact]
    public async Task Client_UploadsTxtThroughCorporateMultipartTransport()
    {
        var api = new CapturingApiClient
        {
            FileResponse = new SriTxtImportDetail { Id = 31, OriginalFileName = "julio.txt" }
        };
        var client = new SriTxtImportClient(api);
        await using var content = new MemoryStream([1, 2, 3]);

        var result = await client.UploadAsync(content, @"C:\temporal\julio.txt");

        result.Id.Should().Be(31);
        api.LastPath.Should().Be("/api/sri/txt-imports/upload");
        api.LastFileName.Should().Be("julio.txt");
        api.LastFormFieldName.Should().Be("file");
        api.LastContentType.Should().Be("text/plain");
    }

    [Fact]
    public async Task ViewModel_PagesImportsAndRowsIndependently()
    {
        var client = Substitute.For<ISriTxtImportClient>();
        client.SearchAsync(Arg.Any<SriTxtImportFilter>(), Arg.Any<CancellationToken>())
            .Returns(
                new SriTxtImportPage
                {
                    Items = [new SriTxtImportListItem { Id = 9 }],
                    TotalCount = 120,
                    Page = 1,
                    PageSize = 50
                },
                new SriTxtImportPage
                {
                    Items = [new SriTxtImportListItem { Id = 59 }],
                    TotalCount = 120,
                    Page = 2,
                    PageSize = 50
                });
        client.GetDetailAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new SriTxtImportDetail { Id = 59 });
        client.GetRowsAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new SriTxtImportRowPage { TotalCount = 201, Page = 1, PageSize = 100 });
        var viewModel = new SriTxtImportViewModel(client);

        await viewModel.LoadAsync();
        await viewModel.GoToImportPageAsync(2, 50);
        await viewModel.SelectAsync(viewModel.Page.Items.Single());
        await viewModel.GoToRowPageAsync(2, 100);

        viewModel.Filter.Page.Should().Be(2);
        viewModel.RowPage.Should().Be(2);
        await client.Received().GetRowsAsync(59, "All", 2, 100, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ViewModel_UploadRefreshesAndSelectsTheCreatedImport()
    {
        var client = Substitute.For<ISriTxtImportClient>();
        client.UploadAsync(Arg.Any<Stream>(), "fixture.txt", Arg.Any<CancellationToken>())
            .Returns(new SriTxtImportDetail { Id = 14, OriginalFileName = "fixture.txt" });
        client.SearchAsync(Arg.Any<SriTxtImportFilter>(), Arg.Any<CancellationToken>())
            .Returns(
                new SriTxtImportPage
                {
                    Items = [new SriTxtImportListItem { Id = 14, OriginalFileName = "fixture.txt" }],
                    TotalCount = 1,
                    Page = 1,
                    PageSize = 50
                });
        client.GetDetailAsync(14, Arg.Any<CancellationToken>())
            .Returns(new SriTxtImportDetail { Id = 14, OriginalFileName = "fixture.txt" });
        client.GetRowsAsync(14, "All", 1, 100, Arg.Any<CancellationToken>())
            .Returns(new SriTxtImportRowPage());
        var viewModel = new SriTxtImportViewModel(client);
        await using var content = new MemoryStream([1]);

        await viewModel.UploadAsync(content, "fixture.txt");

        viewModel.SelectedImport.Should().NotBeNull();
        viewModel.SelectedImport!.Id.Should().Be(14);
        viewModel.Detail!.Id.Should().Be(14);
    }

    [Fact]
    public void Form_UsesCorporateDesignerAndSafeQueueNavigationContract()
    {
        var form = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "SriTxtImports", "SriTxtImportForm.cs");
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "SriTxtImports", "SriTxtImportForm.Designer.cs");
        var shell = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "Shell", "MainForm.cs");
        var filterDialog = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "SriTxtImports", "SriTxtImportFilterDialog.cs");
        var filterDesigner = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "SriTxtImports", "SriTxtImportFilterDialog.Designer.cs");
        var formsProject = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "NuanSystem.WinForms.Forms.csproj");

        form.Should().Contain("public const string FormKey = \"sri-txt-imports\"");
        form.Should().Contain("SriTxtImportForm : BaseCrudListForm");
        form.Should().Contain("NuanGridColumnDefinition");
        form.Should().Contain("PermissionCodes.SriTxtImportsUpload");
        form.Should().Contain("PermissionCodes.SriTxtImportsEnqueue");
        form.Should().Contain("PermissionCodes.SriDocumentsView");
        form.Should().Contain("OpenFileDialog");
        form.Should().Contain("SriTxtImportFilterDialog");
        form.Should().Contain("ExecuteCustomOperationAsync");
        form.Should().Contain("OpenMonitorRequested?.Invoke")
            .And.Contain("SriTxtImportMonitorRequestedEventArgs(detail.Id)");
        shell.Should().Contain("SriTxtImportForm_OpenMonitorRequested")
            .And.Contain("ApplyImportScopeAsync(e.ImportId)");
        form.Should().NotContain("HttpClient");
        form.Should().NotContain("SqlConnection");
        form.Should().NotContain("AccessKey,");
        form.Should().NotContain("Sap");
        form.Should().NotContain("SriProvider");
        designer.Should().Contain("NuanDataGridControl");
        designer.Should().Contain("NuanKpiCardControl");
        designer.Should().Contain("AutoScaleMode = AutoScaleMode.Font");
        designer.Should().NotContain("btnImportPrevious");
        designer.Should().NotContain("btnImportNext");
        designer.Should().NotContain("btnRowPrevious");
        designer.Should().NotContain("btnRowNext");
        designer.Should().NotContain("importPagePanel");
        designer.Should().NotContain("rowPagePanel");
        designer.Should().NotContain("filterPanel");
        designer.Should().NotContain("btnRefresh");
        designer.Should().NotContain("btnEnqueue");
        designer.Should().NotContain("btnOpenQueue");
        filterDialog.Should().Contain("DialogResult = DialogResult.OK");
        filterDialog.Should().Contain("RowValidity");
        filterDesigner.Should().Contain("NuanActionButton");
        filterDesigner.Should().Contain("FormBorderStyle.FixedDialog");
        filterDesigner.Should().Contain("AutoScaleMode = AutoScaleMode.Font");
        formsProject.Should().Contain(
            "<Compile Update=\"SriTxtImports\\SriTxtImportForm.cs\">");
        formsProject.Should().Contain(
            "<Compile Update=\"SriTxtImports\\SriTxtImportFilterDialog.cs\">");
        formsProject.Should().Contain("<SubType>Form</SubType>");
        shell.Should().Contain("\"sri-txt-imports\" => sriTxtImportFormFactory()");
    }

    [Fact]
    public void Shell_LoadsFormOperationsAfterTheNewTabBecomesActive()
    {
        var shell = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "Shell", "MainForm.cs");

        var selectTab = shell.IndexOf(
            "tabControl.SelectedTabPage = page;",
            StringComparison.Ordinal);
        var showForm = shell.IndexOf(
            "form.Show();",
            selectTab,
            StringComparison.Ordinal);
        var loadOperations = shell.IndexOf(
            "_ = ApplyOperationAccessAsync(module, activeCrudForm);",
            showForm,
            StringComparison.Ordinal);

        selectTab.Should().BeGreaterThanOrEqualTo(0);
        showForm.Should().BeGreaterThan(selectTab);
        loadOperations.Should().BeGreaterThan(showForm);
    }

    [Fact]
    public void Shell_CreatesAllowedCustomOperationsWhileTheFormIsLoading()
    {
        var shell = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "Shell", "MainForm.cs");

        shell.Should().Contain(
            ".Where(operation => operation.IsAllowed)\n" +
            "            .Where(operation => !IsBuiltInOperation(operation))");
        shell.Should().NotContain(
            ".Where(operation => crudForm.CanExecuteCustomOperation(OperationKey(operation)))");
        shell.Should().Contain(
            "customButton.Button.Visibility = canExecuteCustomOperation ? BarItemVisibility.Always : BarItemVisibility.Never;");
    }

    [Fact]
    public void Form_DelegatesServerPagingToTheCorporateGrid()
    {
        var form = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "SriTxtImports", "SriTxtImportForm.cs");
        var grid = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Controls",
            "Grids", "NuanDataGridControl.cs");

        form.Should().Contain("importGrid.PageRequested");
        form.Should().Contain("rowGrid.PageRequested");
        form.Should().Contain("importGrid.SetPagedData(");
        form.Should().Contain("rowGrid.SetPagedData(");
        form.Should().NotContain("PageText(");
        form.Should().NotContain("MoveImportPageAsync(");
        form.Should().NotContain("MoveRowPageAsync(");
        grid.Should().Contain("event EventHandler<NuanGridPageRequestEventArgs>? PageRequested");
        grid.Should().Contain("public void SetPagedData<T>(");
        grid.Should().Contain("RequestServerPage(");
    }

    [Fact]
    public void Form_MinimumWidthPreservesTheSixKpiCardsInOneRow()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "SriTxtImports", "SriTxtImportForm.Designer.cs");
        var kpiControl = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Controls",
            "Kpi", "NuanKpiCardControl.cs");

        designer.Should().Contain("MinimumSize = new Size(1151, 700)");
        designer.Should().Contain("kpiPanel = new TableLayoutPanel()");
        designer.Should().Contain("kpiPanel.ColumnCount = 6");
        designer.Should().Contain("cardPending.Dock = DockStyle.Fill");
        designer.Should().Contain("cardPending.HeaderColor = BrandResources.Primary");
        designer.Should().Contain("cardPending.MinimumSize = Size.Empty");
        designer.Should().NotContain("ConfigureCard(");
        kpiControl.Should().Contain("CreateFittedValueFont");
        kpiControl.Should().Contain("Trimming = StringTrimming.None");
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(path))
                return File.ReadAllText(path);
            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontró {Path.Combine(pathParts)}.");
    }

    private sealed class CapturingApiClient : INuanApiClient
    {
        public string LastPath { get; private set; } = string.Empty;
        public string LastFileName { get; private set; } = string.Empty;
        public string LastFormFieldName { get; private set; } = string.Empty;
        public string LastContentType { get; private set; } = string.Empty;
        public object? FileResponse { get; init; }

        public Task<TResponse> GetAsync<TResponse>(
            string path,
            CancellationToken cancellationToken = default)
        {
            LastPath = path;
            return Task.FromResult((TResponse)(object)new SriTxtImportPage());
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(
            string path,
            TRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<TResponse> PutAsync<TRequest, TResponse>(
            string path,
            TRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<TResponse> DeleteAsync<TResponse>(
            string path,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<TResponse> PostFileAsync<TResponse>(
            string path,
            Stream content,
            string fileName,
            string formFieldName = "file",
            string contentType = "application/octet-stream",
            CancellationToken cancellationToken = default)
        {
            LastPath = path;
            LastFileName = fileName;
            LastFormFieldName = formFieldName;
            LastContentType = contentType;
            return Task.FromResult((TResponse)FileResponse!);
        }

        public Task<ApiFileResponse> GetFileAsync(
            string path,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<bool> IsAvailableAsync(
            string path = "/health",
            CancellationToken cancellationToken = default) =>
            Task.FromResult(true);
    }
}
