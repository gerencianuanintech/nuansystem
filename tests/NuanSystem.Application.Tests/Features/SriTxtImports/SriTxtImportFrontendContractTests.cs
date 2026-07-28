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
        await viewModel.MoveImportPageAsync(1);
        await viewModel.SelectAsync(viewModel.Page.Items.Single());
        await viewModel.MoveRowPageAsync(1);

        viewModel.Filter.Page.Should().Be(2);
        viewModel.RowPage.Should().Be(2);
        await client.Received().GetRowsAsync(59, "All", 2, 100, Arg.Any<CancellationToken>());
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

        form.Should().Contain("public const string FormKey = \"sri-txt-imports\"");
        form.Should().Contain("NuanGridColumnDefinition");
        form.Should().Contain("PermissionCodes.SriTxtImportsEnqueue");
        form.Should().Contain("PermissionCodes.SriDocumentsView");
        form.Should().Contain("openQueue(queueId)");
        form.Should().NotContain("HttpClient");
        form.Should().NotContain("SqlConnection");
        form.Should().NotContain("AccessKey,");
        form.Should().NotContain("Sap");
        form.Should().NotContain("SriProvider");
        designer.Should().Contain("NuanDataGridControl");
        designer.Should().Contain("NuanKpiCardControl");
        designer.Should().Contain("NuanActionButton");
        designer.Should().Contain("AutoScaleMode = AutoScaleMode.Font");
        shell.Should().Contain("\"sri-txt-imports\" => sriTxtImportFormFactory()");
    }

    [Fact]
    public void Form_MinimumWidthPreservesTheSixKpiCardsInOneRow()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "SriTxtImports", "SriTxtImportForm.Designer.cs");

        designer.Should().Contain("MinimumSize = new Size(1151, 700)");
        designer.Should().Contain("kpiPanel.WrapContents = false");
        designer.Should().Contain(
            "kpiPanel.Controls.AddRange(new Control[] { cardTotal, cardValid, cardInvalid, cardLinked, cardStaged, cardPending })");
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
