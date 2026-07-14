using FluentAssertions;
using NuanSystem.Shared.Sync;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncFrontendMonitorContractTests
{
    [Fact]
    public void SyncMonitorClient_ShouldUseOnlyAllowedManualPostEndpoints()
    {
        var source = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "SyncMonitorClient.cs");

        source.Should().Contain("GetAsync<SyncDashboard>");
        source.Should().Contain("GetAsync<SyncSummary>");
        source.Should().Contain("GetAsync<IReadOnlyCollection<SyncOutboxListItem>>");
        source.Should().Contain("GetAsync<SyncOutboxDetail>");
        source.Should().Contain("GetAsync<IReadOnlyCollection<SyncOutboxTarget>>");
        source.Should().Contain("GetAsync<IReadOnlyCollection<SyncAuditItem>>");

        source.Should().Contain("PostAsync<RetrySyncOutboxRequest, SyncManualActionResult>");
        source.Should().Contain("PostAsync<RetryDeadLetterRequest, SyncManualActionResult>");
        source.Should().Contain("PostAsync<ReleaseExpiredLockRequest, SyncManualActionResult>");
        source.Should().Contain("/api/sync/outbox/{id}/retry");
        source.Should().Contain("/api/sync/outbox/{id}/retry-deadletter");
        source.Should().Contain("/api/sync/outbox/{id}/release-expired-lock");
        source.Should().NotContain("PutAsync");
        source.Should().NotContain("DeleteAsync");
        source.Should().NotContain("/apply");
        source.Should().NotContain("/run");
        source.Should().NotContain("/process");
        source.Should().NotContain("/dispatch");
        source.Should().NotContain("/claim");
        source.Should().NotContain("/sync-now");
        source.Should().NotContain("/reprocess");
        source.Should().NotContain("PayloadJson =");
        source.Should().NotContain("EntityGlobalId =");
        source.Should().NotContain("EntityName =");
    }

    [Fact]
    public void SyncOutboxListItem_ShouldNotExposePayloadJson()
    {
        var source = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "Models", "SyncMonitorModels.cs");
        var listItemStart = source.IndexOf("public sealed record SyncOutboxListItem", StringComparison.Ordinal);
        var detailStart = source.IndexOf("public sealed record SyncOutboxDetail", StringComparison.Ordinal);

        listItemStart.Should().BeGreaterOrEqualTo(0);
        detailStart.Should().BeGreaterThan(listItemStart);

        var listItemSource = source[listItemStart..detailStart];
        var detailSource = source[detailStart..];

        listItemSource.Should().NotContain("PayloadJson");
        detailSource.Should().Contain("string PayloadJson");
    }

    [Fact]
    public void SyncMonitor_ShouldBeRegisteredInShellAndDynamicMenuSeed()
    {
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var mainForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var shellViewModel = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");
        var securitySeed = ReadWorkspaceFile("database", "sql", "066_master_sync_monitor_security.sql");

        program.Should().Contain("CreateSyncMonitorForm");
        mainForm.Should().Contain("\"sync-monitor\" => syncMonitorFormFactory()");
        shellViewModel.Should().Contain("\"sync-monitor\"");
        shellViewModel.Should().Contain("PermissionCodes.SyncOutboxView");
        securitySeed.Should().Contain("FORM.ADMINISTRATION.SYNC.MONITOR");
        securitySeed.Should().Contain("MENU.ADMINISTRATION.SYNC.MONITOR");
        securitySeed.Should().Contain("N'sync-monitor'");
        securitySeed.Should().Contain("N'SYNC.OUTBOX.VIEW'");
        securitySeed.Should().Contain("N'SYNC.AUDIT.VIEW'");
    }

    [Fact]
    public void SyncMonitorUi_ShouldExposeOnlyControlledManualActions()
    {
        var files = new[]
        {
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Services", "Sync", "SyncMonitorClient.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Sync", "SyncMonitorViewModel.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Sync", "SyncOutboxListViewModel.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Sync", "SyncOutboxDetailViewModel.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Sync", "SyncAuditViewModel.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncMonitorForm.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncMonitorForm.Designer.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncOutboxDetailForm.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncOutboxDetailForm.Designer.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncRetryDeadLetterReasonDialog.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncRetryDeadLetterReasonDialog.Designer.cs")
        };

        var combined = string.Join(Environment.NewLine, files);
        combined.Should().NotContain("PutAsync");
        combined.Should().NotContain("DeleteAsync");
        combined.Should().NotContain("Reprocesar");
        combined.Should().NotContain("/apply");
        combined.Should().NotContain("/run");
        combined.Should().NotContain("/process");
        combined.Should().NotContain("/dispatch");
        combined.Should().NotContain("/claim");
        combined.Should().NotContain("/sync-now");
        combined.Should().NotContain("/reprocess");
        combined.Should().NotContain("PayloadJson =");
    }

    [Fact]
    public void SyncOutboxDetailForm_ShouldUseNuanDataGridControlForTargetsAndAudit()
    {
        var source = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncOutboxDetailForm.cs");
        var designer = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncOutboxDetailForm.Designer.cs");
        var targetsColumns = ExtractSection(source, "private void ApplyTargetColumns()", "private void ApplyAuditColumns()");
        var auditColumns = ExtractSection(source, "private void ApplyAuditColumns()", "private void CopyGlobalId()");

        designer.Should().Contain("private NuanDataGridControl grdTargets;");
        designer.Should().Contain("private NuanDataGridControl grdAudit;");
        designer.Should().Contain("grdTargets = new NuanDataGridControl();");
        designer.Should().Contain("grdAudit = new NuanDataGridControl();");
        designer.Should().Contain("tabTargets.Controls.Add(grdTargets);");
        designer.Should().Contain("tabAudit.Controls.Add(grdAudit);");
        designer.Should().NotContain("private GridControl grcTargets");
        designer.Should().NotContain("private GridView grvTargets");
        designer.Should().NotContain("private GridControl grcAudit");
        designer.Should().NotContain("private GridView grvAudit");

        source.Should().Contain("grdTargets.SetData(viewModel.Targets);");
        source.Should().Contain("grdAudit.SetData(canViewAudit ? viewModel.AuditItems : Array.Empty<SyncAuditItem>());");
        source.Should().Contain("grdTargets.SetStatusBadgeProvider(NuanGridStatusBadges.DefaultProvider);");
        source.Should().Contain("grdAudit.SetStatusBadgeProvider(NuanGridStatusBadges.DefaultProvider);");

        targetsColumns.Should().Contain("FieldName = nameof(SyncOutboxTarget.Status)");
        targetsColumns.Should().Contain("Format = NuanGridColumnFormat.StatusBadge");
        targetsColumns.Should().Contain("FieldName = nameof(SyncOutboxTarget.BranchDisplay)");
        targetsColumns.Should().Contain("FieldName = nameof(SyncOutboxTarget.LastErrorMessage)");
        targetsColumns.Should().NotContain("PayloadJson");
        targetsColumns.Should().NotContain("EntityGlobalId");

        auditColumns.Should().Contain("FieldName = nameof(SyncAuditItem.PreviousStatus)");
        auditColumns.Should().Contain("FieldName = nameof(SyncAuditItem.NewStatus)");
        auditColumns.Should().Contain("Format = NuanGridColumnFormat.StatusBadge");
        auditColumns.Should().Contain("FieldName = nameof(SyncAuditItem.CreatedBy)");
        auditColumns.Should().Contain("FieldName = nameof(SyncAuditItem.Message)");
        auditColumns.Should().NotContain("PayloadJson");
        auditColumns.Should().NotContain("ErrorDetailSummary");
    }

    [Fact]
    public void SyncOutboxDetailForm_ShouldNotRenderFullPayloadJson()
    {
        var source = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncOutboxDetailForm.cs");

        source.Should().Contain("Payload retenido por seguridad");
        source.Should().NotContain("FormatPayloadJson");
        source.Should().NotContain("memoPayload.Text = detail.PayloadJson");
        source.Should().NotContain("memoPayload.Text = FormatPayloadJson(detail.PayloadJson)");
        source.Should().NotContain("JsonDocument.Parse");
    }

    [Fact]
    public void SyncOutboxDetailForm_ShouldNotContainDesignerPlaceholdersOrUnsupportedGridSyntax()
    {
        var combined = string.Join(
            Environment.NewLine,
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncOutboxDetailForm.cs"),
            ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncOutboxDetailForm.Designer.cs"));

        combined.Should().NotContain("Retrieve Details");
        combined.Should().NotContain("Run Designer");
        combined.Should().NotContain("Change view");
        combined.Should().NotContain("Add level");
        combined.Should().NotContain("(MainView)");
        combined.Should().NotContain("AddRange([");
        combined.Should().NotContain("DataGridView");
        combined.Should().NotContain("ConfigureGrid");
        combined.Should().NotContain("ConfigureTargetsGrid");
        combined.Should().NotContain("ConfigureAuditGrid");
        combined.Should().NotContain("BuildTargets");
        combined.Should().NotContain("BuildAudit");
        combined.Should().NotContain("AddGrid");
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldEnableRetryOnlyForErrorWithPermission()
    {
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.Error) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanRetry.Should().BeTrue();
        viewModel.CanRetryDeadLetter.Should().BeFalse();
        viewModel.CanReleaseExpiredLock.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldNotEnableRetryWithoutPermission()
    {
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.Error) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: false, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanRetry.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldEnableRetryDeadLetterOnlyForDeadLetterWithPermission()
    {
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.DeadLetter) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanRetry.Should().BeFalse();
        viewModel.CanRetryDeadLetter.Should().BeTrue();
        viewModel.CanReleaseExpiredLock.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldNotEnableRetryDeadLetterWithoutPermission()
    {
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.DeadLetter) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: false, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanRetryDeadLetter.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldEnableReleaseOnlyForExpiredLockWithPermission()
    {
        var expiredLock = DateTime.UtcNow.AddMinutes(-5);
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.InProcess, expiredLock) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanRetry.Should().BeFalse();
        viewModel.CanRetryDeadLetter.Should().BeFalse();
        viewModel.CanReleaseExpiredLock.Should().BeTrue();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldEnableReleaseForErrorOnlyWhenLockIsExpired()
    {
        var expiredLock = DateTime.UtcNow.AddMinutes(-5);
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.Error, expiredLock) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanRetry.Should().BeTrue();
        viewModel.CanReleaseExpiredLock.Should().BeTrue();
        viewModel.CanRetryDeadLetter.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldNotEnableReleaseWithoutPermission()
    {
        var expiredLock = DateTime.UtcNow.AddMinutes(-5);
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.InProcess, expiredLock) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: false);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanReleaseExpiredLock.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldNotEnableReleaseForDeadLetterEvenWithExpiredLock()
    {
        var expiredLock = DateTime.UtcNow.AddMinutes(-5);
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.DeadLetter, expiredLock) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanReleaseExpiredLock.Should().BeFalse();
        viewModel.CanRetryDeadLetter.Should().BeTrue();
        viewModel.CanRetry.Should().BeFalse();
    }

    [Theory]
    [InlineData(SyncEventStatus.Applied)]
    [InlineData(SyncEventStatus.Pending)]
    [InlineData(SyncEventStatus.Ignored)]
    public async Task SyncOutboxDetailViewModel_ShouldNotEnableActionsForClosedOrPendingStates(SyncEventStatus status)
    {
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(status, DateTime.UtcNow.AddMinutes(-5)) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanRetry.Should().BeFalse();
        viewModel.CanRetryDeadLetter.Should().BeFalse();
        viewModel.CanReleaseExpiredLock.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldNotReleaseActiveLock()
    {
        var activeLock = DateTime.UtcNow.AddMinutes(5);
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.InProcess, activeLock) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        viewModel.CanReleaseExpiredLock.Should().BeFalse();
    }

    [Fact]
    public async Task SyncOutboxDetailViewModel_ShouldRequireDeadLetterReason()
    {
        var client = new FakeSyncMonitorClient { Detail = CreateDetail(SyncEventStatus.DeadLetter) };
        var viewModel = new SyncOutboxDetailViewModel(client, hasRetryPermission: true, hasRetryDeadLetterPermission: true, hasReleaseLockPermission: true);

        await viewModel.LoadAsync(10, includeAudit: true);

        var action = () => viewModel.RetryDeadLetterAsync("   ", includeAudit: true);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Debe ingresar un motivo para reintentar DeadLetter.");
    }

    private static SyncOutboxDetail CreateDetail(SyncEventStatus status, DateTime? lockExpiresAt = null)
    {
        return new SyncOutboxDetail(
            10,
            Guid.NewGuid(),
            1,
            "Item",
            Guid.NewGuid(),
            "ITEM-001",
            SyncOperation.Updated,
            "{}",
            "Master",
            null,
            status,
            1,
            3,
            null,
            lockExpiresAt.HasValue ? "worker" : null,
            lockExpiresAt?.AddMinutes(-10),
            lockExpiresAt,
            DateTime.UtcNow.AddMinutes(-30),
            null,
            status is SyncEventStatus.Error or SyncEventStatus.DeadLetter ? "Error de prueba" : null);
    }

    private static string ReadWorkspaceFile(params string[] pathParts)
    {
        var root = FindWorkspaceRoot();
        return File.ReadAllText(Path.Combine(new[] { root }.Concat(pathParts).ToArray()));
    }

    private static string ExtractSection(string source, string startToken, string endToken)
    {
        var start = source.IndexOf(startToken, StringComparison.Ordinal);
        var end = source.IndexOf(endToken, StringComparison.Ordinal);

        start.Should().BeGreaterOrEqualTo(0);
        end.Should().BeGreaterThan(start);

        return source[start..end];
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

    private sealed class FakeSyncMonitorClient : ISyncMonitorClient
    {
        public SyncOutboxDetail Detail { get; set; } = CreateDetail(SyncEventStatus.Pending);

        public Task<SyncDashboard> GetDashboardAsync(int take = 10, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<SyncSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyCollection<SyncOutboxListItem>> SearchOutboxAsync(SyncOutboxFilter filter, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<SyncOutboxDetail> GetOutboxDetailAsync(long id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Detail);
        }

        public Task<IReadOnlyCollection<SyncOutboxTarget>> GetOutboxTargetsAsync(long id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<SyncOutboxTarget>>(Array.Empty<SyncOutboxTarget>());
        }

        public Task<IReadOnlyCollection<SyncAuditItem>> SearchAuditAsync(SyncAuditFilter filter, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<SyncAuditItem>>(Array.Empty<SyncAuditItem>());
        }

        public Task<SyncManualActionResult> RetryAsync(long id, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<SyncManualActionResult> RetryDeadLetterAsync(long id, RetryDeadLetterRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<SyncManualActionResult> ReleaseExpiredLockAsync(long id, ReleaseExpiredLockRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
