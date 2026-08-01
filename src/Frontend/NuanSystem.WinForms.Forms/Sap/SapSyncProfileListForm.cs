using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Sap;
using NuanSystem.WinForms.Controls.Grids;
using DevExpress.Utils;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncProfileListForm : BaseGridCrudListForm
{
    public const string FormKey = "sap-sync-profiles";
    private SapSyncProfilesViewModel? viewModel;
    private ISapSyncManagementClient? client;
    private ApiSession? session;
    private IGridColumnSettingsClient? columnSettingsClient;

    public SapSyncProfileListForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SapSyncProfileListForm(SapSyncProfilesViewModel viewModel, ISapSyncManagementClient client, ApiSession session, IGridColumnSettingsClient columnSettingsClient) : this()
    {
        this.viewModel = viewModel;
        this.client = client;
        this.session = session;
        this.columnSettingsClient = columnSettingsClient;
        ConfigureCrudPermissions(session, new(PermissionCodes.SapSyncProfilesView, PermissionCodes.SapSyncProfilesCreate, PermissionCodes.SapSyncProfilesEdit, PermissionCodes.SapSyncProfilesDelete));
        ConfigureColumnPersonalization(columnSettingsClient, FormKey);
        EnableServerPaging(50);
        NuanGrid.FormKey = FormKey;
        NuanGrid.PageRequested += async (_, args) => await GoToPageAsync(args);
        GridView.DoubleClick += async (_, _) => await ExecuteConsultAsync();
    }

    private SapSyncProfilesViewModel ViewModel => viewModel ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");
    private ISapSyncManagementClient Client => client ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");
    private ApiSession Session => session ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");

    protected override async Task LoadDataAsync()
    {
        if (IsInDesignMode() || viewModel is null) return;
        await RunWithBusyStateAsync(async () =>
        {
            await ViewModel.LoadAsync();
            SetPagedGridData(ViewModel.Profiles, ViewModel.Filter.PageNumber, ViewModel.Filter.PageSize, ViewModel.TotalCount);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override Task CreateAsync() => OpenEditorAsync(null, false);
    protected override async Task EditAsync()
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione un perfil SAP."); return; }
        await OpenEditorAsync(item.Id, false);
    }
    protected override async Task ConsultAsync()
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione un perfil SAP."); return; }
        await OpenEditorAsync(item.Id, true);
    }
    protected override async Task DeleteAsync()
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione un perfil SAP."); return; }
        if (!Confirm($"¿Eliminar el perfil SAP {item.Code}?")) return;
        await RunWithBusyStateAsync(async () => { await ViewModel.DeleteAsync(item); await LoadDataAsync(); });
    }

    public override bool CanExecuteCustomOperation(string operationKey) => Normalize(operationKey) switch
    {
        "validate" => Session.HasPermission(PermissionCodes.SapSyncProfilesValidate),
        "activate" or "deactivate" => Session.HasPermission(PermissionCodes.SapSyncProfilesActivate),
        "viewexecutions" or "executions" => Session.HasPermission(PermissionCodes.SapSyncExecutionsView),
        "execute" => false,
        "filter" => Session.HasPermission(PermissionCodes.SapSyncProfilesView),
        _ => base.CanExecuteCustomOperation(operationKey)
    };

    public override Task ExecuteCustomOperationAsync(string operationKey) => Normalize(operationKey) switch
    {
        "validate" => ValidateSelectedAsync(),
        "activate" => ChangeActivationAsync(true),
        "deactivate" => ChangeActivationAsync(false),
        "viewexecutions" or "executions" => OpenExecutionsAsync(),
        "filter" => OpenFilterAsync(),
        _ => base.ExecuteCustomOperationAsync(operationKey)
    };

    private async Task OpenEditorAsync(long? id, bool readOnly)
    {
        var editor = new SapSyncProfileEditViewModel(Client);
        await editor.InitializeAsync(id);
        using var scope = readOnly ? BaseEditForm.BeginReadOnlyMode() : null;
        using var form = new SapSyncProfileEditForm(editor);
        if (form.ShowDialog(this) == DialogResult.OK) await LoadDataAsync();
    }

    private async Task ValidateSelectedAsync()
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione un perfil SAP."); return; }
        await RunWithBusyStateAsync(async () =>
        {
            var result = await ViewModel.ValidateAsync(item.Id);
            var message = result.IsValid ? "El perfil SAP es valido." : string.Join(Environment.NewLine, result.Errors.Select(error => $"{error.Code}: {error.Message}"));
            XtraMessageBox.Show(this, message, "Validacion SAP", MessageBoxButtons.OK, result.IsValid ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        });
    }

    private async Task ChangeActivationAsync(bool active)
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione un perfil SAP."); return; }
        await RunWithBusyStateAsync(async () => { if (active) await ViewModel.ActivateAsync(item); else await ViewModel.DeactivateAsync(item); await LoadDataAsync(); });
    }

    private Task OpenExecutionsAsync()
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione un perfil SAP."); return Task.CompletedTask; }
        using var form = new SapSyncExecutionListForm(new SapSyncExecutionsViewModel(Client), Client, Session, columnSettingsClient, item.Id);
        form.ShowDialog(this);
        return Task.CompletedTask;
    }

    private async Task OpenFilterAsync()
    {
        using var dialog = new SapSyncProfileFilterDialog(ViewModel.Filter);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        ViewModel.Filter.Search = dialog.Search;
        ViewModel.Filter.IsActive = dialog.SelectedIsActive;
        ViewModel.Filter.EntityCode = dialog.EntityCode;
        ViewModel.Filter.PageNumber = 1;
        await LoadDataAsync();
    }

    private async Task GoToPageAsync(NuanGridPageRequestEventArgs args)
    {
        ViewModel.Filter.PageNumber = args.Page;
        ViewModel.Filter.PageSize = args.PageSize;
        await LoadDataAsync();
    }

    private SapSyncProfileListItem? Selected() => SelectedGridItem<SapSyncProfileListItem>();
    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(SapSyncProfileListItem.Code), "Codigo", 0, 110);
        Column(nameof(SapSyncProfileListItem.Name), "Perfil SAP", 1, 230);
        Column(nameof(SapSyncProfileListItem.CompanyName), "Empresa", 2, 220);
        Column(nameof(SapSyncProfileListItem.ActiveEntityCount), "Entidades", 3, 90, alignment: HorzAlignment.Far);
        Column(nameof(SapSyncProfileListItem.StatusText), "Estado", 4, 90);
        Column(nameof(SapSyncProfileListItem.UpdatedAtUtc), "Ultima modificacion", 5, 155, "yyyy-MM-dd HH:mm");
    }
    private void Column(string field, string caption, int index, int width, string? format = null, HorzAlignment? alignment = null)
    {
        if (GridView.Columns[field] is not { } column) return;
        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = index;
        column.Width = width;
        if (!string.IsNullOrWhiteSpace(format))
        {
            column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            column.DisplayFormat.FormatString = format;
        }
        if (alignment.HasValue)
        {
            column.AppearanceCell.TextOptions.HAlignment = alignment.Value;
        }
    }
    private static string Normalize(string value) => value.Replace("ACTION.", "", StringComparison.OrdinalIgnoreCase).Replace("SAP_SYNC_PROFILES.", "", StringComparison.OrdinalIgnoreCase).Replace("-", "").Replace("_", "").ToLowerInvariant();
    private bool IsInDesignMode() => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || Site?.DesignMode == true;
}
