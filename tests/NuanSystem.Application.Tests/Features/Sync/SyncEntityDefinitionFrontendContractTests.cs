using FluentAssertions;
using NSubstitute;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions.Models;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;
using NuanSystem.WinForms.ViewModels.Sync.EntityDefinitions;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncEntityDefinitionFrontendContractTests
{
    [Fact]
    public async Task Client_ShouldUseCentralApiClientAndEncodeListFilters()
    {
        var apiClient = Substitute.For<INuanApiClient>();
        apiClient
            .GetAsync<PagedResult<SyncEntityDefinitionListItem>>(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<SyncEntityDefinitionListItem>([], 0, 2, 25));
        var client = new SyncEntityDefinitionClient(apiClient);

        var page = await client.SearchAsync(new SyncEntityDefinitionListFilter
        {
            Search = "Item master",
            IsActive = false,
            PageNumber = 2,
            PageSize = 25
        });

        page.PageNumber.Should().Be(2);
        await apiClient.Received(1).GetAsync<PagedResult<SyncEntityDefinitionListItem>>(
            Arg.Is<string>(path =>
                path.StartsWith("/api/sync/configuration/entities?", StringComparison.Ordinal)
                && path.Contains("search=Item%20master", StringComparison.Ordinal)
                && path.Contains("isActive=false", StringComparison.Ordinal)
                && path.Contains("pageNumber=2", StringComparison.Ordinal)
                && path.Contains("pageSize=25", StringComparison.Ordinal)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditViewModel_ShouldPreserveUnavailableDependenciesAndExcludeCurrentDefinition()
    {
        var client = Substitute.For<ISyncEntityDefinitionClient>();
        var lookup = new[]
        {
            Lookup(1, "Current", true),
            Lookup(2, "BusinessPartner", true)
        };
        var detail = Detail(
            1,
            "Current",
            [
                new SyncEntityDefinitionDependency(10, 2, "BusinessPartner", "Socios de negocio"),
                new SyncEntityDefinitionDependency(11, 3, "Legacy", "Dependencia historica")
            ]);
        client.GetLookupAsync(1, Arg.Any<CancellationToken>()).Returns(lookup);
        client.GetAsync(1, Arg.Any<CancellationToken>()).Returns(detail);
        client.UpdateAsync(1, Arg.Any<UpdateSyncEntityDefinitionRequest>(), Arg.Any<CancellationToken>())
            .Returns(detail with { Name = "Actualizada" });
        var viewModel = new SyncEntityDefinitionEditViewModel(client);

        await viewModel.InitializeAsync(1);

        viewModel.State.IsCodeReadOnly.Should().BeTrue();
        viewModel.State.Dependencies.Should().NotContain(option => option.DefinitionId == 1);
        viewModel.State.Dependencies.Should().ContainSingle(option =>
            option.DefinitionId == 2 && option.IsAvailable && option.IsSelected);
        viewModel.State.Dependencies.Should().ContainSingle(option =>
            option.DefinitionId == 3 && !option.IsAvailable && option.IsSelected);

        viewModel.State.Name = "Actualizada";
        viewModel.State.Dependencies.Single(option => option.DefinitionId == 3).IsSelected = false;
        await viewModel.SaveAsync();

        await client.Received(1).UpdateAsync(
            1,
            Arg.Is<UpdateSyncEntityDefinitionRequest>(request =>
                request.Name == "Actualizada"
                && request.DependencyDefinitionIds.SequenceEqual(new[] { 2 })),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void FrontendLayer_ShouldRemainApiOnlyAndBePreparedInComposition()
    {
        var client = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "EntityDefinitions", "SyncEntityDefinitionClient.cs");
        var viewModels = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.ViewModels", "Sync", "EntityDefinitions", "SyncEntityDefinitionViewModels.cs");
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var combined = string.Join(Environment.NewLine, client, viewModels);

        client.Should().Contain("INuanApiClient");
        client.Should().Contain("/api/sync/configuration/entities");
        client.Should().Contain("/lookup");
        program.Should().Contain("SyncEntityDefinitionClient");

        combined.Should().NotContain("HttpClient");
        combined.Should().NotContain("Authorization");
        combined.Should().NotContain("X-Company-Code");
        combined.Should().NotContain("SqlConnection");
        combined.Should().NotContain("Dapper");
        combined.Should().NotContain("ConnectionString");
        combined.Should().NotContain("SyncEntityListForm");
        combined.Should().NotContain("SyncEntityEditForm");
    }

    [Fact]
    public void WinForms_ShouldUseProjectBasesDesignerAndDynamicNavigation()
    {
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "EntityDefinitions", "SyncEntityListForm.cs");
        var editForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "EntityDefinitions", "SyncEntityEditForm.cs");
        var editDesigner = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "EntityDefinitions", "SyncEntityEditForm.Designer.cs");
        var mainForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var shellViewModel = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");

        listForm.Should().Contain("SyncEntityListForm : BaseGridCrudListForm");
        listForm.Should().Contain("PermissionCodes.SyncEntitiesView");
        listForm.Should().Contain("ConfigureColumnPersonalization(gridColumnSettingsClient, FormKey)");
        listForm.Should().Contain("RecordHistoryForm");
        editForm.Should().Contain("SyncEntityEditForm : BaseEditForm");
        editDesigner.Should().Contain("NuanDataGridControl grdDependencies");
        editDesigner.Should().NotContain("PanelControl");
        editDesigner.Should().Contain("btnGuardar.Size = new Size(100, 36)");
        editDesigner.Should().Contain("btnCancelar.Size = new Size(100, 36)");
        mainForm.Should().Contain("\"sync-entities\" => syncEntityListFormFactory()");
        shellViewModel.Should().Contain("\"sync-entities\"");
    }

    [Fact]
    public async Task Client_ShouldExposeEntityHistoryThroughCentralApiClient()
    {
        var apiClient = Substitute.For<INuanApiClient>();
        apiClient
            .GetAsync<IReadOnlyCollection<NuanSystem.WinForms.Services.Audit.Models.SecurityChangeItem>>(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<NuanSystem.WinForms.Services.Audit.Models.SecurityChangeItem>());
        var client = new SyncEntityDefinitionClient(apiClient);

        await client.GetHistoryAsync(15);

        await apiClient.Received(1)
            .GetAsync<IReadOnlyCollection<NuanSystem.WinForms.Services.Audit.Models.SecurityChangeItem>>(
                "/api/sync/configuration/entities/15/history",
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public void ProfileEntityLookup_ShouldUseDatabaseCatalogAndCreateWithEntityPermission()
    {
        var dialog = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEntityDialog.cs");
        var profileForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");
        var catalogHandler = ReadWorkspaceFile(
            "src", "Backend", "NuanSystem.Application", "Features", "Sync", "Configuration", "Queries", "SyncConfigurationQueryHandlers.cs");
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var lookup = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Controls", "Lookups", "NuanLookupEdit.cs");
        var configurationModels = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "Models", "SyncConfigurationModels.cs");

        dialog.Should().Contain("lueEntity.CreateButtonEnabled = canCreateEntity");
        dialog.Should().Contain("CreateEntityRequested");
        dialog.Should().Contain("lueEntity.CreateButtonClick += EntityLookupCreateButtonClick");
        dialog.Should().Contain("nameof(SyncEntityCatalogItem.IsOperative), \"Operativa\"");
        profileForm.Should().Contain("PermissionCodes.SyncEntitiesCreate");
        profileForm.Should().Contain("SyncEntityEditForm");
        profileForm.Should().Contain("await viewModel.RefreshCatalogAsync()");
        catalogHandler.Should().Contain("ISyncEntityCatalogService entityCatalogService");
        catalogHandler.Should().Contain("Entities = entityDefinitions.Select(SyncEntityDefinitionMapper.ToProfileCatalogItem)");
        program.Should().Contain("configurationCompanyClient,");
        program.Should().Contain("syncEntityDefinitionClient);");
        lookup.Should().Contain("Properties.Buttons.Remove(duplicate)");
        lookup.Should().Contain("e.Button.Kind == ButtonPredefines.Plus && e.Button.Enabled");
        configurationModels.Should().Contain("public bool IsOperative => HasProducer && HasApplier");
    }

    [Fact]
    public void ProfileEditor_ShouldOrderConfiguredDependenciesBeforeDependentEntity()
    {
        var state = SyncProfileEditorState.CreateNew();
        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Articulos",
            ExecutionOrder = 1
        });
        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "ItemGroups",
            EntityName = "Grupos",
            ExecutionOrder = 2
        });

        state.ApplyDependencyOrder(
        [
            new SyncEntityCatalogItem { Code = "Item", Dependencies = ["ItemGroups"] },
            new SyncEntityCatalogItem { Code = "ItemGroups" }
        ]);

        state.Entities.OrderBy(entity => entity.ExecutionOrder)
            .Select(entity => entity.EntityCode)
            .Should().Equal("ItemGroups", "Item");
    }

    private static SyncEntityDefinitionLookupItem Lookup(int id, string code, bool isActive)
    {
        return new SyncEntityDefinitionLookupItem
        {
            Id = id,
            Code = code,
            Name = code,
            IsActive = isActive
        };
    }

    private static SyncEntityDefinitionDetail Detail(
        int id,
        string code,
        IReadOnlyCollection<SyncEntityDefinitionDependency> dependencies)
    {
        return new SyncEntityDefinitionDetail
        {
            Id = id,
            Code = code,
            Name = code,
            DefaultExecutionOrder = 100,
            SupportsInsert = true,
            SupportsUpdate = true,
            SupportsDeactivate = true,
            IsActive = true,
            Dependencies = dependencies,
            CreatedAt = DateTime.UtcNow
        };
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
