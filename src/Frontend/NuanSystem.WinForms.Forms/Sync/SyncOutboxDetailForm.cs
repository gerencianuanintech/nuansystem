using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using NuanSystem.Shared.Sync;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.WinForms.Forms.Sync;

public sealed partial class SyncOutboxDetailForm : XtraForm
{
    private readonly SyncOutboxDetailViewModel viewModel;
    private readonly bool canViewAudit;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ManualActionCompleted { get; private set; }

    public SyncOutboxDetailForm()
    {
        viewModel = null!;
        InitializeComponent();
    }

    public SyncOutboxDetailForm(SyncOutboxDetailViewModel viewModel, bool canViewAudit)
    {
        this.viewModel = viewModel;
        this.canViewAudit = canViewAudit;
        InitializeComponent();
        WireEvents();
        WireVisualEvents();
        BindData();
        if (!canViewAudit)
        {
            tabAudit.PageVisible = false;
        }
    }

    private void WireEvents()
    {
        btnCopyGlobalId.Click += (_, _) => CopyGlobalId();
        btnRetry.Click += async (_, _) => await RetryAsync();
        btnRetryDeadLetter.Click += async (_, _) => await RetryDeadLetterAsync();
        btnReleaseLock.Click += async (_, _) => await ReleaseExpiredLockAsync();
    }

    private void WireVisualEvents()
    {
        ConfigureCopyGlobalIdButtonIcon();
        ApplyTargetColumns();
        ApplyAuditColumns();
        grdTargets.SetStatusBadgeProvider(NuanGridStatusBadges.DefaultProvider);
        grdAudit.SetStatusBadgeProvider(NuanGridStatusBadges.DefaultProvider);
    }

    private void BindData()
    {
        if (viewModel.Detail is not { } detail)
        {
            return;
        }

        Text = $"Detalle SyncOutbox #{detail.Id}";
        lblHeaderEvent.Text = $"Evento {detail.Id:N0}";
        lblBreadcrumb.Text = $"Monitor de Sincronizacion > Eventos SyncOutbox > Evento {detail.Id:N0}";
        lblSummaryStatusValue.Text = detail.Status.ToString();
        lblSummaryAttemptsValue.Text = $"{detail.AttemptCount:N0} / {detail.MaxAttempts:N0}";
        lblSummaryCreatedValue.Text = FormatDate(detail.CreatedAt);
        lblSummaryProcessedValue.Text = FormatDate(detail.ProcessedAt);
        txtEventId.Text = detail.EventId.ToString();
        txtEntityName.Text = detail.EntityName;
        txtEntityGlobalId.Text = detail.EntityGlobalId.ToString();
        txtEntityCode.Text = detail.EntityCode ?? string.Empty;
        txtOperation.Text = detail.Operation.ToString();
        txtStatus.Text = detail.Status.ToString();
        ApplyStatusFieldStyle(detail.Status.ToString());
        txtAttemptCount.Text = detail.AttemptCount.ToString("N0");
        txtNextRetryAt.Text = FormatDate(detail.NextRetryAt);
        txtLockedBy.Text = detail.LockedBy ?? string.Empty;
        txtLockExpiresAt.Text = FormatDate(detail.LockExpiresAt);
        memoLastError.Text = detail.LastErrorMessage ?? string.Empty;
        memoErrorDetail.Text = detail.LastErrorMessage ?? string.Empty;
        memoPayload.Text = "Payload retenido por seguridad. Use auditoria, entidad, GlobalId, targets y logs de backend para diagnostico operativo.";
        grdTargets.SetData(viewModel.Targets);
        grdAudit.SetData(canViewAudit ? viewModel.AuditItems : Array.Empty<SyncAuditItem>());
        UpdateManualActionState();
    }

    private void ApplyTargetColumns()
    {
        grdTargets.ConfigureColumns(
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxTarget.BranchDisplay),
                Caption = "Sucursal",
                VisibleIndex = 0,
                Width = 150,
                Format = NuanGridColumnFormat.Text
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxTarget.Status),
                Caption = "Estado",
                VisibleIndex = 1,
                Width = 120,
                Format = NuanGridColumnFormat.StatusBadge
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxTarget.AttemptCount),
                Caption = "Intentos",
                VisibleIndex = 2,
                Width = 80,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxTarget.MaxAttempts),
                Caption = "Max",
                VisibleIndex = 3,
                Width = 70,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxTarget.AppliedAt),
                Caption = "AppliedAt",
                VisibleIndex = 4,
                Width = 150,
                Format = NuanGridColumnFormat.DateTime
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxTarget.NextRetryAt),
                Caption = "NextRetryAt",
                VisibleIndex = 5,
                Width = 150,
                Format = NuanGridColumnFormat.DateTime
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxTarget.LastErrorMessage),
                Caption = "Ultimo error",
                VisibleIndex = 6,
                Width = 260,
                Format = NuanGridColumnFormat.Text
            });
    }

    private void ApplyAuditColumns()
    {
        grdAudit.ConfigureColumns(
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncAuditItem.CreatedAt),
                Caption = "Fecha",
                VisibleIndex = 0,
                Width = 150,
                Format = NuanGridColumnFormat.DateTime
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncAuditItem.Action),
                Caption = "Accion",
                VisibleIndex = 1,
                Width = 140,
                Format = NuanGridColumnFormat.Text
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncAuditItem.PreviousStatus),
                Caption = "Estado anterior",
                VisibleIndex = 2,
                Width = 130,
                Format = NuanGridColumnFormat.StatusBadge
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncAuditItem.NewStatus),
                Caption = "Estado nuevo",
                VisibleIndex = 3,
                Width = 130,
                Format = NuanGridColumnFormat.StatusBadge
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncAuditItem.CreatedBy),
                Caption = "Usuario",
                VisibleIndex = 4,
                Width = 140,
                Format = NuanGridColumnFormat.Text
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncAuditItem.Message),
                Caption = "Mensaje",
                VisibleIndex = 5,
                Width = 300,
                Format = NuanGridColumnFormat.Text
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncAuditItem.ErrorCode),
                Caption = "ErrorCode",
                VisibleIndex = 6,
                Width = 120,
                Format = NuanGridColumnFormat.Text
            });
    }

    private void CopyGlobalId()
    {
        if (string.IsNullOrWhiteSpace(txtEntityGlobalId.Text))
        {
            return;
        }

        Clipboard.SetText(txtEntityGlobalId.Text);
        XtraMessageBox.Show(
            this,
            "GlobalId copiado al portapapeles.",
            Text,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ConfigureCopyGlobalIdButtonIcon()
    {
        btnCopyGlobalId.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon("copiar_16.svg", BrandResources.Primary);
        btnCopyGlobalId.ImageOptions.SvgImageSize = new Size(16, 16);
    }

    private void ApplyStatusFieldStyle(string? status)
    {
        ApplyStatusColors(status, txtStatus.Properties.Appearance);
    }

    private void UpdateManualActionState()
    {
        btnRetry.Visible = viewModel.HasRetryPermission;
        btnRetryDeadLetter.Visible = viewModel.HasRetryDeadLetterPermission;
        btnReleaseLock.Visible = viewModel.HasReleaseLockPermission;

        btnRetry.Enabled = viewModel.CanRetry;
        btnRetryDeadLetter.Enabled = viewModel.CanRetryDeadLetter;
        btnReleaseLock.Enabled = viewModel.CanReleaseExpiredLock;

        btnRetry.ToolTip = btnRetry.Enabled
            ? "Reintentar evento en estado Error."
            : "Disponible solo para eventos en estado Error.";
        btnRetryDeadLetter.ToolTip = btnRetryDeadLetter.Enabled
            ? "Reintentar evento DeadLetter con motivo obligatorio."
            : "Disponible solo para eventos en estado DeadLetter.";
        btnReleaseLock.ToolTip = btnReleaseLock.Enabled
            ? "Liberar lock tecnico vencido."
            : ResolveReleaseLockTooltip();
    }

    private string ResolveReleaseLockTooltip()
    {
        if (!viewModel.HasLock)
        {
            return "El evento no tiene lock tecnico.";
        }

        if (!viewModel.IsLockExpired)
        {
            return "El lock todavia esta vigente.";
        }

        return "Disponible solo para eventos InProcess o Error con lock vencido.";
    }

    private async Task RetryAsync()
    {
        if (Confirm("El evento en estado Error sera marcado para reintento. Desea continuar?") != DialogResult.Yes)
        {
            return;
        }

        await ExecuteManualActionAsync(() => viewModel.RetryAsync(canViewAudit));
    }

    private async Task RetryDeadLetterAsync()
    {
        using var dialog = new SyncRetryDeadLetterReasonDialog(viewModel.Detail);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (Confirm("Se reintentara un evento cerrado como DeadLetter. Desea continuar?") != DialogResult.Yes)
        {
            return;
        }

        await ExecuteManualActionAsync(() => viewModel.RetryDeadLetterAsync(dialog.Reason, canViewAudit));
    }

    private async Task ReleaseExpiredLockAsync()
    {
        if (Confirm("Se liberara el lock tecnico vencido del evento. Desea continuar?") != DialogResult.Yes)
        {
            return;
        }

        await ExecuteManualActionAsync(() => viewModel.ReleaseExpiredLockAsync(canViewAudit));
    }

    private async Task ExecuteManualActionAsync(Func<Task<SyncManualActionResult>> action)
    {
        ToggleManualActions(false);
        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            var result = await action();
            ManualActionCompleted = true;
            BindData();
            XtraMessageBox.Show(
                this,
                string.IsNullOrWhiteSpace(result.Message) ? "La accion se completo correctamente." : result.Message,
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        });
        ToggleManualActions(true);
    }

    private void ToggleManualActions(bool enabled)
    {
        btnRetry.Enabled = enabled && viewModel.CanRetry;
        btnRetryDeadLetter.Enabled = enabled && viewModel.CanRetryDeadLetter;
        btnReleaseLock.Enabled = enabled && viewModel.CanReleaseExpiredLock;
        Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
    }

    private DialogResult Confirm(string message)
    {
        return XtraMessageBox.Show(
            this,
            message,
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
    }

    private static string FormatDate(DateTime? value)
    {
        return value.HasValue ? value.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm") : string.Empty;
    }

    private static void ApplyStatusColors(string? status, DevExpress.Utils.AppearanceObject appearance)
    {
        var (foreColor, backColor) = status switch
        {
            nameof(SyncEventStatus.Pending) => (Color.FromArgb(55, 65, 81), Color.FromArgb(243, 244, 246)),
            nameof(SyncEventStatus.InProcess) => (Color.FromArgb(29, 78, 216), Color.FromArgb(219, 234, 254)),
            nameof(SyncEventStatus.Applied) => (Color.FromArgb(21, 128, 61), Color.FromArgb(220, 252, 231)),
            nameof(SyncEventStatus.Error) => (Color.FromArgb(185, 28, 28), Color.FromArgb(254, 226, 226)),
            nameof(SyncEventStatus.DeadLetter) => (Color.FromArgb(190, 18, 60), Color.FromArgb(255, 228, 230)),
            nameof(SyncEventStatus.Ignored) => (Color.FromArgb(75, 85, 99), Color.FromArgb(229, 231, 235)),
            _ => (BrandResources.Text, Color.White)
        };

        appearance.ForeColor = foreColor;
        appearance.BackColor = backColor;
        appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        appearance.Options.UseForeColor = true;
        appearance.Options.UseBackColor = true;
        appearance.Options.UseFont = true;
        appearance.TextOptions.HAlignment = HorzAlignment.Center;
    }
}
