using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.ConfigurationSettings.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.ConfigurationSettings;

namespace NuanSystem.WinForms.Forms.ConfigurationSettings;

public sealed partial class SettingsForm : BaseGridCrudListForm
{
    private readonly ConfigurationSettingsViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public SettingsForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public SettingsForm(ConfigurationSettingsViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public SettingsForm(ConfigurationSettingsViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "configuration-settings");
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        using var form = new SettingsEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Parametro guardado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (viewModel is null || SelectedItem() is not { } parameter)
        {
            return;
        }

        using var form = new SettingsEditForm(parameter);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(parameter.Id, form.Request);
        ShowSuccess("Parametro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } parameter)
        {
            return;
        }

        using var form = new SettingsEditForm(parameter, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Parametro copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (viewModel is null || SelectedItem() is not { } parameter)
        {
            return;
        }

        if (!Confirm($"Eliminar el parametro {parameter.Key}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(parameter.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } parameter)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de parametro",
            parameter.Key,
            cancellationToken => auditClient.GetSecurityChangesAsync("ConfigurationSettings", parameter.Id.ToString(), 200, cancellationToken));

        form.ShowDialog(this);
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(ConfigurationSettingItem.Key), "Clave", 1, 180);
        ConfigureColumn(nameof(ConfigurationSettingItem.Category), "Categoria", 2, 110);
        ConfigureColumn(nameof(ConfigurationSettingItem.DataType), "Tipo", 3, 80);
        ConfigureColumn(nameof(ConfigurationSettingItem.Value), "Valor", 4, 220);
        ConfigureColumn(nameof(ConfigurationSettingItem.IsSystemParameter), "Sistema", 5, 70);
        ConfigureColumn(nameof(ConfigurationSettingItem.IsEditable), "Editable", 6, 70);
        ConfigureColumn(nameof(ConfigurationSettingItem.IsActive), "Activo", 7, 60);
    }

    private ConfigurationSettingItem? SelectedItem()
    {
        return SelectedGridItem<ConfigurationSettingItem>();
    }

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
    }

    private void WireEvents()
    {
        if (session is not null)
        {
            ConfigureCrudPermissions(session, new CrudOperationPermissions(
                NuanSystem.Shared.Constants.PermissionCodes.SettingsManage,
                NuanSystem.Shared.Constants.PermissionCodes.SettingsManage,
                NuanSystem.Shared.Constants.PermissionCodes.SettingsManage,
                NuanSystem.Shared.Constants.PermissionCodes.SettingsManage));
        }
    }
}
