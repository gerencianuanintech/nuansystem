using FluentAssertions;
using NSubstitute;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sync;
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
        source.Should().Contain("PostAsync<SaveSyncProfileRequest, int>");
        source.Should().Contain("PutAsync<SaveSyncProfileRequest, bool>");
        source.Should().Contain("GetAsync<SyncProfileDetail>");
        source.Should().Contain("distribution-policies/{matrixId}/candidates");
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
    public async Task SyncConfigurationClient_ShouldFollowProfileWriteResponseContracts()
    {
        var apiClient = Substitute.For<INuanApiClient>();
        var request = new SaveSyncProfileRequest { Code = "SYNC-NEW", Name = "Nuevo", CompanyId = 1 };
        var created = new SyncProfileDetail { Id = 27, Code = request.Code, Name = request.Name, CompanyId = 1 };
        apiClient.PostAsync<SaveSyncProfileRequest, int>(
                "/api/sync/configuration/profiles",
                request,
                Arg.Any<CancellationToken>())
            .Returns(27);
        apiClient.PutAsync<SaveSyncProfileRequest, bool>(
                "/api/sync/configuration/profiles/27",
                request,
                Arg.Any<CancellationToken>())
            .Returns(true);
        apiClient.GetAsync<SyncProfileDetail>(
                "/api/sync/configuration/profiles/27",
                Arg.Any<CancellationToken>())
            .Returns(created);
        var client = new SyncConfigurationClient(apiClient);

        var createResult = await client.CreateProfileAsync(request);
        var updateResult = await client.UpdateProfileAsync(27, request);

        createResult.Should().BeSameAs(created);
        updateResult.Should().BeSameAs(created);
        await apiClient.Received(1).PostAsync<SaveSyncProfileRequest, int>(
            "/api/sync/configuration/profiles", request, Arg.Any<CancellationToken>());
        await apiClient.Received(1).PutAsync<SaveSyncProfileRequest, bool>(
            "/api/sync/configuration/profiles/27", request, Arg.Any<CancellationToken>());
        await apiClient.Received(2).GetAsync<SyncProfileDetail>(
            "/api/sync/configuration/profiles/27", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncConfigurationClient_ShouldEncodeDistributionCandidateSearch()
    {
        var apiClient = Substitute.For<INuanApiClient>();
        apiClient.GetAsync<IReadOnlyCollection<SyncDistributionCandidate>>(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<SyncDistributionCandidate>());
        var client = new SyncConfigurationClient(apiClient);

        await client.SearchDistributionCandidatesAsync(7, "INV PAP", 50);

        await apiClient.Received(1).GetAsync<IReadOnlyCollection<SyncDistributionCandidate>>(
            "/api/sync/configuration/distribution-policies/7/candidates?search=INV%20PAP&take=50",
            Arg.Any<CancellationToken>());
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
        state.Description = "Sincronizacion de articulos";
        state.ExecutionMode = "Full";
        state.BatchSize = 750;
        state.MaxRetries = 4;
        state.RetryDelaySeconds = 45;
        state.TimeoutMinutes = 90;
        state.IsActive = false;
        state.AddBranch(catalog.BranchCompanies.Single());
        state.AddEntityFromCatalog(catalog.Entities.Single());
        state.Schedule.ScheduleType = "Interval";
        state.Schedule.IntervalMinutes = 10;

        var request = state.ToRequest();

        request.Code.Should().Be("ITEMS");
        request.Name.Should().Be("Items");
        request.Description.Should().Be("Sincronizacion de articulos");
        request.CompanyId.Should().Be(1);
        request.Direction.Should().Be("MasterToBranch");
        request.ExecutionMode.Should().Be("Full");
        request.ConflictStrategy.Should().Be("MasterWins");
        request.BatchSize.Should().Be(750);
        request.MaxRetries.Should().Be(4);
        request.RetryDelaySeconds.Should().Be(45);
        request.TimeoutMinutes.Should().Be(90);
        request.IsActive.Should().BeFalse();
        request.Branches.Should().ContainSingle(branch => branch.BranchCompanyId == 2);
        request.Entities.Should().ContainSingle(entity => entity.EntityCode == "Item" && entity.Branches.Count == 1);
        request.Schedule.Should().NotBeNull();
        request.Schedule!.ScheduleType.Should().Be("Interval");
        request.Schedule.IntervalMinutes.Should().Be(10);
    }

    [Fact]
    public void SyncProfileEditorState_ShouldKeepBranchAndEntityMatrixConsistent()
    {
        var state = SyncProfileEditorState.CreateNew();
        state.Entities.Add(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Articulos",
            ExecutionOrder = 1
        });
        var branch = new CompanyLookupItem(20, "SUC-01", "Sucursal Norte", true, "NORTE", "NUA_NORTE");

        state.AddBranch(branch, 300, 2, true).Should().BeTrue();
        state.AddBranch(branch, 500, 3, true).Should().BeFalse();
        state.Branches.Should().ContainSingle(item =>
            item.BranchCompanyId == 20
            && item.BranchCode == "NORTE"
            && item.DatabaseName == "NUA_NORTE"
            && item.BatchSize == 300
            && item.MaxRetries == 2);
        state.EntityBranches.Should().ContainSingle(item =>
            item.BranchCompanyId == 20 && item.EntityCode == "Item" && item.IsEnabled);

        state.UpdateBranch(20, 450, 4, false).Should().BeTrue();
        state.Branches.Single().BatchSize.Should().Be(450);
        state.Branches.Single().MaxRetries.Should().Be(4);
        state.Branches.Single().IsActive.Should().BeFalse();

        state.SetBranchActive(20, true).Should().BeTrue();
        state.Branches.Single().IsActive.Should().BeTrue();

        state.RemoveBranch(20).Should().BeTrue();
        state.Branches.Should().BeEmpty();
        state.EntityBranches.Should().BeEmpty();
    }

    [Fact]
    public async Task SyncProfileEditViewModel_ShouldRefreshBranchCatalogWithoutLosingConfiguration()
    {
        var initialCatalog = CreateCatalog() with
        {
            BranchCompanies = [new CompanyLookupItem(20, "OLD", "Sucursal anterior", true, "NORTE", "DB_OLD")]
        };
        var refreshedCatalog = initialCatalog with
        {
            BranchCompanies = [new CompanyLookupItem(20, "NEW", "Sucursal Norte", true, "NORTE-01", "DB_NEW")]
        };
        var client = Substitute.For<ISyncConfigurationClient>();
        client.GetCatalogAsync(Arg.Any<CancellationToken>()).Returns(initialCatalog, refreshedCatalog);
        var viewModel = new SyncProfileEditViewModel(client);
        await viewModel.InitializeAsync(null);
        viewModel.State.AddBranch(initialCatalog.BranchCompanies.Single(), 650, 5, false);

        await viewModel.RefreshCatalogAsync();

        viewModel.State.Branches.Should().ContainSingle(branch =>
            branch.BranchCompanyCode == "NEW"
            && branch.BranchCompanyName == "Sucursal Norte"
            && branch.BranchCode == "NORTE-01"
            && branch.DatabaseName == "DB_NEW"
            && branch.BatchSize == 650
            && branch.MaxRetries == 5
            && !branch.IsActive);
        await client.Received(2).GetCatalogAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SyncProfileBranchActions_ShouldBeWiredToEditorState()
    {
        var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");
        var dialog = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileBranchDialog.cs");

        form.Should().Contain("btnEditBranch.Click");
        form.Should().Contain("btnRemoveBranch.Click");
        form.Should().Contain("btnActivateBranch.Click");
        form.Should().Contain("btnDeactivateBranch.Click");
        form.Should().Contain("btnRefreshBranches.Click");
        form.Should().Contain("viewModel.State.RemoveBranch");
        form.Should().Contain("viewModel.State.SetBranchActive");
        dialog.Should().Contain("LoadInitialValue(initialValue)");
        dialog.Should().Contain("lueBranchCompany.Enabled = false;");
    }

    [Fact]
    public void SyncProfileEditorState_ShouldManageEntitiesAndTheirMatrixFromOneState()
    {
        var state = SyncProfileEditorState.CreateNew();
        state.AddBranch(new CompanyLookupItem(10, "ACT", "Sucursal activa", true), 500, 3, true);
        state.AddBranch(new CompanyLookupItem(20, "INA", "Sucursal inactiva", true), 500, 3, false);

        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Articulos",
            ExecutionOrder = 1,
            SyncMode = "Incremental"
        }).Should().BeTrue();
        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Warehouse",
            EntityName = "Almacenes",
            ExecutionOrder = 1,
            SyncMode = "Full"
        }).Should().BeTrue();
        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Duplicado",
            ExecutionOrder = 3
        }).Should().BeFalse();

        state.Entities.OrderBy(entity => entity.ExecutionOrder)
            .Select(entity => entity.EntityCode)
            .Should().Equal("Warehouse", "Item");
        state.EntityBranches.Should().HaveCount(2);
        state.EntityBranches.Should().OnlyContain(link => link.BranchCompanyId == 10);

        state.UpdateEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Articulos actualizados",
            ExecutionOrder = 1,
            SyncMode = "Full",
            BatchSize = 250,
            IsActive = false
        }).Should().BeTrue();
        state.Entities.OrderBy(entity => entity.ExecutionOrder)
            .Select(entity => entity.EntityCode)
            .Should().Equal("Item", "Warehouse");
        state.Entities.Single(entity => entity.EntityCode == "Item").EntityName.Should().Be("Articulos actualizados");
        state.Entities.Single(entity => entity.EntityCode == "Item").BatchSize.Should().Be(250);

        state.MoveEntity("Item", 1).Should().BeTrue();
        state.Entities.OrderBy(entity => entity.ExecutionOrder)
            .Select(entity => entity.EntityCode)
            .Should().Equal("Warehouse", "Item");

        state.RemoveEntity("Warehouse").Should().BeTrue();
        state.Entities.Should().ContainSingle(entity => entity.EntityCode == "Item" && entity.ExecutionOrder == 1);
        state.EntityBranches.Should().ContainSingle(link => link.EntityCode == "Item");
    }

    [Fact]
    public void SyncProfileEditorState_ShouldReconcileMatrixWhenBranchActivationChanges()
    {
        var state = SyncProfileEditorState.CreateNew();
        state.AddBranch(new CompanyLookupItem(10, "SUC", "Sucursal", true), 500, 3, true);
        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Articulos",
            ExecutionOrder = 1
        });
        state.EntityBranches.Should().ContainSingle();

        state.SetBranchActive(10, false).Should().BeTrue();
        state.EntityBranches.Should().BeEmpty();
        state.ToRequest().Entities.Single().Branches.Should().BeEmpty();

        state.SetBranchActive(10, true).Should().BeTrue();
        state.EntityBranches.Should().ContainSingle(link =>
            link.EntityCode == "Item" && link.BranchCompanyId == 10 && link.IsEnabled);
        state.ToRequest().Entities.Single().Branches.Should().ContainSingle();
    }

    [Fact]
    public void SyncProfileEntityDialog_ShouldEnforceCatalogAndPersistenceConstraints()
    {
        var dialog = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEntityDialog.cs");
        var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");

        dialog.Should().Contain("EnforceSelectedEntityCapabilities");
        dialog.Should().Contain("ValidateTechnicalField");
        dialog.Should().Contain("sedBatchSize.Value > 10000");
        dialog.Should().Contain("suggestedExecutionOrder");
        form.Should().Contain("viewModel.State.AddEntity");
        form.Should().Contain("viewModel.State.UpdateEntity");
        form.Should().Contain("viewModel.State.RemoveEntity");
        form.Should().Contain("viewModel.State.MoveEntity");
        form.Should().Contain("viewModel.State.SetEntityActive");
    }

    [Fact]
    public void SyncProfileEditorState_ShouldApplyDistributionBatchPrecedence()
    {
        var state = SyncProfileEditorState.CreateNew();
        state.BatchSize = 1000;
        state.AddBranch(new CompanyLookupItem(10, "SUC", "Sucursal", true), 800, 3, true);
        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Articulos",
            ExecutionOrder = 1,
            BatchSize = 600
        });

        state.SetDistribution("Item", 10, true, 400, updateBatch: true).Should().BeTrue();
        state.EffectiveBatchSize("Item", 10).Should().Be(400);

        state.SetDistribution("Item", 10, true, null, updateBatch: true).Should().BeTrue();
        state.EffectiveBatchSize("Item", 10).Should().Be(600);

        state.Entities.Single().BatchSize = null;
        state.EffectiveBatchSize("Item", 10).Should().Be(800);

        state.Branches.Single().BatchSize = null;
        state.EffectiveBatchSize("Item", 10).Should().Be(1000);
    }

    [Fact]
    public void SyncProfileEditorState_ShouldManageIndividualAndBulkDistributionActions()
    {
        var state = SyncProfileEditorState.CreateNew();
        state.AddBranch(new CompanyLookupItem(10, "ACT", "Sucursal activa", true), 500, 3, true);
        state.AddBranch(new CompanyLookupItem(20, "INA", "Sucursal inactiva", true), 500, 3, false);
        state.AddEntity(new SyncProfileEntityEditorRow
        {
            EntityCode = "Item",
            EntityName = "Articulos",
            ExecutionOrder = 1
        });
        state.SetDistribution("Item", 10, true, 250, updateBatch: true).Should().BeTrue();

        state.SetAllDistributionsEnabled(false);
        state.GetDistribution("Item", 10).Should().Match<SyncEntityBranchEditorRow>(link =>
            !link.IsEnabled && link.BatchSize == 250);

        state.SetAllDistributionsEnabled(true);
        state.GetDistribution("Item", 10).Should().Match<SyncEntityBranchEditorRow>(link =>
            link.IsEnabled && link.BatchSize == 250);
        state.SetDistribution("Item", 20, true, 300, updateBatch: true).Should().BeFalse();
        state.SetDistribution("Item", 10, true, 10001, updateBatch: true).Should().BeFalse();

        var request = state.ToRequest();
        request.Entities.Single().Branches.Should().ContainSingle(link =>
            link.BranchCompanyId == 10 && link.IsEnabled && link.BatchSize == 250);
    }

    [Fact]
    public void SyncProfileDistributionForm_ShouldUseCentralStateAndLockInactiveBranches()
    {
        var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");
        var dialog = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileDistributionDialog.cs");

        form.Should().Contain("viewModel.State.GetDistribution");
        form.Should().Contain("viewModel.State.SetDistribution");
        form.Should().Contain("viewModel.State.SetAllDistributionsEnabled");
        form.Should().Contain("column.OptionsColumn.ReadOnly = !branchIsActive;");
        form.Should().Contain("Active la sucursal antes de configurar su distribución.");
        dialog.Should().Contain("SpecificBatchSize()");
        dialog.Should().Contain("data.EntityBatchSize");
        dialog.Should().Contain("data.BranchBatchSize");
        dialog.Should().Contain("data.ProfileBatchSize");
        dialog.Should().Contain("SearchDistributionCandidatesAsync");
        dialog.Should().Contain("UpdateDistributionPolicyAsync");
        dialog.Should().Contain("BuildRuleJson");
        dialog.Should().Contain("selectedIds");
        dialog.Should().Contain("\"Selected\"");
        dialog.Should().Contain("\"Rule\"");
    }

    [Fact]
    public void SyncScheduleEditorState_ShouldNormalizeEachScheduleType()
    {
        var schedule = new SyncScheduleEditorState();

        schedule.Configure(
            "Manual",
            30,
            new TimeSpan(23, 0, 0),
            null,
            preventConcurrentExecutions: true,
            isActive: true);
        schedule.ToRequest().Should().Match<SaveSyncScheduleRequest>(request =>
            request.ScheduleType == "Manual"
            && request.IntervalMinutes == null
            && request.ExecutionTime == null
            && request.TimeZoneId == "America/Guayaquil");
        schedule.EffectiveFrequencyText().Should().Be("Ejecución manual");

        schedule.Configure(
            "Interval",
            45,
            new TimeSpan(23, 0, 0),
            "America/Guayaquil",
            preventConcurrentExecutions: false,
            isActive: true);
        schedule.ToRequest().Should().Match<SaveSyncScheduleRequest>(request =>
            request.IntervalMinutes == 45 && request.ExecutionTime == null);
        schedule.EffectiveFrequencyText().Should().Be("Cada 45 minutos");

        schedule.Configure(
            "Daily",
            45,
            new TimeSpan(22, 30, 0),
            "America/Guayaquil",
            preventConcurrentExecutions: true,
            isActive: true);
        schedule.ToRequest().Should().Match<SaveSyncScheduleRequest>(request =>
            request.IntervalMinutes == null && request.ExecutionTime == new TimeSpan(22, 30, 0));
        schedule.EffectiveFrequencyText().Should().Be("Diaria a las 22:30");
    }

    [Fact]
    public async Task SyncProfileEditViewModel_ShouldLoadAuthoritativeScheduleSummary()
    {
        var client = Substitute.For<ISyncConfigurationClient>();
        var catalog = CreateCatalog();
        var detail = new SyncProfileDetail { Id = 12, Code = "SYNC-12", Name = "Perfil", CompanyId = 1 };
        var nextExecution = new DateTime(2026, 7, 16, 15, 0, 0, DateTimeKind.Utc);
        var finishedAt = new DateTimeOffset(2026, 7, 16, 14, 0, 0, TimeSpan.Zero);
        client.GetCatalogAsync(Arg.Any<CancellationToken>()).Returns(catalog);
        client.GetProfileAsync(12, Arg.Any<CancellationToken>()).Returns(detail);
        client.SearchProfilesAsync(Arg.Any<SyncProfileListFilter>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<SyncProfileListItem>(
                [new SyncProfileListItem { Id = 12, Code = "SYNC-12", NextExecutionAt = nextExecution }],
                1,
                1,
                50));
        client.SearchExecutionsAsync(Arg.Any<SyncProfileExecutionFilter>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<SyncProfileExecutionListItem>(
                [new SyncProfileExecutionListItem(
                    100,
                    12,
                    "SYNC-12",
                    "Perfil",
                    1,
                    "Matriz",
                    "Scheduled",
                    "Completed",
                    "corr-100",
                    "System",
                    finishedAt.AddMinutes(-5),
                    finishedAt.AddMinutes(-5),
                    finishedAt,
                    1,
                    10,
                    10,
                    0,
                    0,
                    null)],
                1,
                1,
                1));

        var viewModel = new SyncProfileEditViewModel(client);
        await viewModel.InitializeAsync(12);
        await viewModel.RefreshProfileSummaryAsync(includeExecutionSummary: true);

        viewModel.ProfileSummary.Should().NotBeNull();
        viewModel.ProfileSummary!.NextExecutionAt.Should().Be(nextExecution);
        viewModel.LastSuccessfulScheduledExecutionAt.Should().Be(finishedAt);
        await client.Received(1).SearchProfilesAsync(
            Arg.Is<SyncProfileListFilter>(filter => filter.Search == "SYNC-12"),
            Arg.Any<CancellationToken>());
        await client.Received(1).SearchExecutionsAsync(
            Arg.Is<SyncProfileExecutionFilter>(filter =>
                filter.ProfileId == 12
                && filter.Status == "Completed"
                && filter.ExecutionType == "Scheduled"
                && filter.PageSize == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SyncProfileScheduleForm_ShouldValidateAndShowAuthoritativeState()
    {
        var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");

        form.Should().Contain("ValidateScheduleControls");
        form.Should().Contain("TimeZoneInfo.FindSystemTimeZoneById");
        form.Should().Contain("RefreshProfileSummaryAsync(CanViewExecutions)");
        form.Should().Contain("Se calculará al guardar");
        form.Should().Contain("Se recalculará al guardar");
        form.Should().Contain("LastSuccessfulScheduledExecutionAt");
        form.Should().NotContain("CalculateNextExecution(");
    }

    [Fact]
    public void SyncValidationSectionResolver_ShouldClassifyBackendMessagesByFunctionalSection()
    {
        SyncValidationSectionResolver.Resolve(new SyncValidationMessage(
                "SyncProfileCodeRequired", "Code", "Código requerido"))
            .Should().Be(SyncProfileEditorSection.General);
        SyncValidationSectionResolver.Resolve(new SyncValidationMessage(
                "SyncBranchInactive", "BranchCompanyId", "Sucursal inactiva"))
            .Should().Be(SyncProfileEditorSection.Branches);
        SyncValidationSectionResolver.Resolve(new SyncValidationMessage(
                "SyncEntityUnknown", "EntityCode", "Entidad desconocida"))
            .Should().Be(SyncProfileEditorSection.Entities);
        SyncValidationSectionResolver.Resolve(new SyncValidationMessage(
                "SyncTechnicalFieldInvalid", "KeyField", "Campo técnico inválido"))
            .Should().Be(SyncProfileEditorSection.Entities);
        SyncValidationSectionResolver.Resolve(new SyncValidationMessage(
                "SyncMatrixEnabledRequired", "Entities", "Distribución requerida"))
            .Should().Be(SyncProfileEditorSection.Distribution);
        SyncValidationSectionResolver.Resolve(new SyncValidationMessage(
                "SyncBranchWithoutEnabledEntity", "Branches", "Distribución incompleta"))
            .Should().Be(SyncProfileEditorSection.Distribution);
        SyncValidationSectionResolver.Resolve(new SyncValidationMessage(
                "SyncScheduleDailyTimeRequired", "ExecutionTime", "Hora requerida"))
            .Should().Be(SyncProfileEditorSection.Schedule);
    }

    [Fact]
    public async Task SyncProfileEditViewModel_ShouldValidateCurrentEditorRequestThroughClient()
    {
        var client = Substitute.For<ISyncConfigurationClient>();
        client.GetCatalogAsync(Arg.Any<CancellationToken>()).Returns(CreateCatalog());
        client.ValidateProfileAsync(Arg.Any<SaveSyncProfileRequest>(), Arg.Any<CancellationToken>())
            .Returns(new SyncProfileValidationResult { IsValid = true });

        var viewModel = new SyncProfileEditViewModel(client);
        await viewModel.InitializeAsync(null);
        viewModel.State.Code = "SYNC-VALIDATE";
        viewModel.State.Name = "Perfil por validar";
        viewModel.State.Schedule.Configure(
            "Interval",
            30,
            null,
            "America/Guayaquil",
            preventConcurrentExecutions: true,
            isActive: true);

        var result = await viewModel.ValidateAsync();

        result.IsValid.Should().BeTrue();
        await client.Received(1).ValidateProfileAsync(
            Arg.Is<SaveSyncProfileRequest>(request =>
                request.Code == "SYNC-VALIDATE"
                && request.Name == "Perfil por validar"
                && request.Schedule != null
                && request.Schedule.ScheduleType == "Interval"
                && request.Schedule.IntervalMinutes == 30),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SyncProfileValidationForm_ShouldInvalidateAndNavigateActionableResults()
    {
        var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");

        form.Should().Contain("WireValidationInvalidation");
        form.Should().Contain("InvalidateValidationResult");
        form.Should().Contain("Requiere revalidación");
        form.Should().Contain("Validator.Clear();");
        form.Should().Contain("if (!ValidateForm())");
        form.Should().Contain("OpenSelectedValidationIssue");
        form.Should().Contain("grdValidationResults.RowDoubleClick");
        form.Should().Contain("grdValidationResults.KeyDown");
        form.Should().Contain("SyncValidationSectionResolver.Resolve(source)");
    }

    [Fact]
    public void SyncExecutionStatusPolicy_ShouldControlActionsAndLocalizedText()
    {
        SyncExecutionStatusPolicy.IsActive("pending").Should().BeTrue();
        SyncExecutionStatusPolicy.IsActive("RUNNING").Should().BeTrue();
        SyncExecutionStatusPolicy.IsActive("Completed").Should().BeFalse();

        SyncExecutionStatusPolicy.CanCancel("Pending").Should().BeTrue();
        SyncExecutionStatusPolicy.CanCancel("Running").Should().BeTrue();
        SyncExecutionStatusPolicy.CanCancel("Cancelling").Should().BeFalse();

        SyncExecutionStatusPolicy.CanRetry("Cancelled").Should().BeTrue();
        SyncExecutionStatusPolicy.CanRetry("completedwitherrors").Should().BeTrue();
        SyncExecutionStatusPolicy.CanRetry("Failed").Should().BeTrue();
        SyncExecutionStatusPolicy.CanRetry("Completed").Should().BeFalse();

        SyncExecutionStatusPolicy.StatusText("CompletedWithErrors").Should().Be("Completada con errores");
        SyncExecutionStatusPolicy.ExecutionTypeText("scheduled").Should().Be("Programada");
        SyncExecutionStatusPolicy.ExecutionTypeText("Retry").Should().Be("Reintento");
    }

    [Fact]
    public async Task SyncExecutionsViewModel_ShouldLoadRequestedProfilePage()
    {
        var client = Substitute.For<ISyncConfigurationClient>();
        client.SearchExecutionsAsync(Arg.Any<SyncProfileExecutionFilter>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<SyncProfileExecutionListItem>([], 37, 2, 25));
        var viewModel = new SyncExecutionsViewModel(client);
        viewModel.Filter.ProfileId = 12;
        viewModel.Filter.PageNumber = 2;
        viewModel.Filter.PageSize = 25;

        await viewModel.LoadAsync();

        viewModel.Executions.Should().BeEmpty();
        viewModel.TotalCount.Should().Be(37);
        await client.Received(1).SearchExecutionsAsync(
            Arg.Is<SyncProfileExecutionFilter>(filter =>
                filter.ProfileId == 12
                && filter.PageNumber == 2
                && filter.PageSize == 25),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SyncProfileExecutionsPage_ShouldPreserveSelectionAndGuardActions()
    {
        var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");

        form.Should().Contain("aceExecutions.Enabled = profileId.HasValue && CanViewExecutions");
        form.Should().Contain("executionActionInProgress");
        form.Should().Contain("RestoreExecutionSelection");
        form.Should().Contain("retry.NewExecutionId");
        form.Should().Contain("SyncExecutionStatusPolicy.CanCancel");
        form.Should().Contain("SyncExecutionStatusPolicy.CanRetry");
        form.Should().Contain("SyncExecutionStatusPolicy.IsActive");
        form.Should().Contain("grdExecutions.KeyDown");
        form.Should().Contain("CalculateExecutionTotalPages");
    }

    [Fact]
    public async Task SyncProfileEditViewModel_ShouldCreateNewProfileThroughClient()
    {
        var client = Substitute.For<ISyncConfigurationClient>();
        var catalog = CreateCatalog();
        client.GetCatalogAsync(Arg.Any<CancellationToken>()).Returns(catalog);
        client.CreateProfileAsync(Arg.Any<SaveSyncProfileRequest>(), Arg.Any<CancellationToken>())
            .Returns(new SyncProfileDetail { Id = 11, Code = "SYNC-NEW", Name = "Nuevo", CompanyId = 1 });

        var viewModel = new SyncProfileEditViewModel(client);
        await viewModel.InitializeAsync(null);
        viewModel.State.Code = "SYNC-NEW";
        viewModel.State.Name = "Nuevo";

        var saved = await viewModel.SaveAsync();

        saved.Id.Should().Be(11);
        viewModel.State.Id.Should().Be(11);
        viewModel.State.Code.Should().Be("SYNC-NEW");
        await client.Received(1).CreateProfileAsync(
            Arg.Is<SaveSyncProfileRequest>(request => request.Code == "SYNC-NEW" && request.CompanyId == 1),
            Arg.Any<CancellationToken>());
        await client.DidNotReceive().UpdateProfileAsync(
            Arg.Any<int>(),
            Arg.Any<SaveSyncProfileRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncProfileEditViewModel_ShouldUpdateExistingProfileThroughClient()
    {
        var client = Substitute.For<ISyncConfigurationClient>();
        var catalog = CreateCatalog();
        var detail = new SyncProfileDetail
        {
            Id = 12,
            Code = "SYNC-EDIT",
            Name = "Anterior",
            CompanyId = 1,
            IsActive = false
        };
        client.GetCatalogAsync(Arg.Any<CancellationToken>()).Returns(catalog);
        client.GetProfileAsync(12, Arg.Any<CancellationToken>()).Returns(detail);
        client.UpdateProfileAsync(12, Arg.Any<SaveSyncProfileRequest>(), Arg.Any<CancellationToken>())
            .Returns(detail with { Name = "Actualizado" });

        var viewModel = new SyncProfileEditViewModel(client);
        await viewModel.InitializeAsync(12);
        viewModel.State.Name = "Actualizado";

        var saved = await viewModel.SaveAsync();

        saved.Name.Should().Be("Actualizado");
        viewModel.State.Id.Should().Be(12);
        viewModel.State.Name.Should().Be("Actualizado");
        await client.Received(1).UpdateProfileAsync(
            12,
            Arg.Is<SaveSyncProfileRequest>(request => request.Code == "SYNC-EDIT" && request.Name == "Actualizado"),
            Arg.Any<CancellationToken>());
        await client.DidNotReceive().CreateProfileAsync(
            Arg.Any<SaveSyncProfileRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SyncProfileEditForm_ShouldPersistOnlyAfterAsyncValidation()
    {
        var baseEditForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Common", "BaseEditForm.cs");
        var syncEditForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");

        baseEditForm.Should().Contain("protected virtual Task<bool> PersistAsync()");
        baseEditForm.Should().Contain("if (!await PersistAsync())");
        syncEditForm.Should().Contain("protected override async Task<bool> PersistAsync()");
        syncEditForm.Should().Contain("var validation = await viewModel.ValidateAsync();");
        syncEditForm.Should().Contain("await viewModel.SaveAsync();");
        syncEditForm.Should().Contain("LoadGeneralFromState();");
        syncEditForm.Should().Contain("CopyGeneralToState();");
    }

    [Fact]
    public void SyncProfileForms_ShouldIntegrateConsultPermissionsAndWarningValidation()
    {
        var editForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");
        var listForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileListForm.cs");

        editForm.Should().Contain("protected override void ApplyReadOnlyMode()");
        editForm.Should().Contain("grdDistribution.GridView.OptionsBehavior.Editable = false;");
        editForm.Should().Contain("allowActions: !IsReadOnlyMode");
        editForm.Should().NotContain("validation.Warnings.Count > 0 && viewModel.State.IsActive");

        listForm.Should().Contain("if (CanUpdate)");
        listForm.Should().Contain("await ExecuteConsultAsync();");
        listForm.Should().Contain("Seleccione un perfil para consultar sus ejecuciones.");
        listForm.Should().Contain("new SyncExecutionListForm(");
        listForm.Should().Contain("profile.Id");
    }

    [Fact]
    public void SyncExecutionDetailForm_ShouldGateMutationsAndFollowRetryExecution()
    {
        var detailForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncExecutionDetailForm.cs");

        detailForm.Should().Contain("bool allowActions = true");
        detailForm.Should().Contain("SyncExecutionStatusPolicy.CanCancel");
        detailForm.Should().Contain("SyncExecutionStatusPolicy.CanRetry");
        detailForm.Should().Contain("executionId = retry.NewExecutionId;");
        detailForm.Should().Contain("isActionInProgress");
        detailForm.Should().Contain("UpdatePollingState();");
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
        var profileEditForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.cs");
        var viewModels = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Sync", "SyncConfigurationViewModels.cs");
        var combined = string.Join(Environment.NewLine, listForm, listDesigner, detailForm, profileEditForm, viewModels);

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
    public void SyncProfileEditorDialogs_ShouldRemainDesignerBacked()
    {
        var formNames = new[]
        {
            "SyncProfileBranchDialog",
            "SyncProfileEntityDialog",
            "SyncProfileDistributionDialog"
        };

        foreach (var formName in formNames)
        {
            var form = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", $"{formName}.cs");
            var designer = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", $"{formName}.Designer.cs");
            var resxPath = Path.Combine(FindWorkspaceRoot(), "src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", $"{formName}.resx");

            form.Should().Contain($"partial class {formName}");
            form.Should().Contain($"public {formName}()");
            form.Should().Contain("InitializeComponent();");
            form.Should().NotContain("BuildLayout");
            form.Should().NotContain("BuildUi");
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
        master.Should().Contain(":r 079_sync_profile_entity_catalog_alignment.sql");
        master.Should().Contain(":r 080_sync_entity_definitions.sql");
        master.Should().Contain(":r 081_sync_entity_definition_api_security.sql");
        master.Should().Contain("NuanSystem_Master");
        master.Should().Contain("SyncEntityDefinitions");
        master.Should().Contain("FK_SyncProfileEntities_EntityDefinition");
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
        check.Should().Contain("SP_NA_PUT_SYNCPROFILEACTUALIZAR.EntityCatalog");
        check.Should().Contain("FK_SyncProfileEntities_EntityDefinition");
        check.Should().Contain("Outdated");
        check.Should().NotContain("SyncCheckpoints");
    }

    private static string ReadWorkspaceFile(params string[] segments)
    {
        return File.ReadAllText(Path.Combine(FindWorkspaceRoot(), Path.Combine(segments)));
    }

    private static SyncConfigurationCatalog CreateCatalog()
    {
        return new SyncConfigurationCatalog
        {
            MasterCompanies = [new CompanyLookupItem(1, "MST", "Matriz", true)],
            Directions = [new LookupItem("MasterToBranch", "MasterToBranch")],
            ExecutionModes = [new LookupItem("Incremental", "Incremental")],
            ConflictStrategies = [new LookupItem("MasterWins", "MasterWins")],
            ScheduleTypes = [new LookupItem("Manual", "Manual")]
        };
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
