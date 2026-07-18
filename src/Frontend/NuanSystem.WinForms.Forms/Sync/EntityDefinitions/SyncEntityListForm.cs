using DevExpress.Utils;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions.Models;
using NuanSystem.WinForms.ViewModels.Sync.EntityDefinitions;

namespace NuanSystem.WinForms.Forms.Sync.EntityDefinitions;

public sealed partial class SyncEntityListForm : BaseGridCrudListForm
{
    public const string FormKey = "sync-entities";

    private SyncEntityDefinitionsViewModel? viewModel;
    private ISyncEntityDefinitionClient? client;

    public SyncEntityListForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SyncEntityListForm(
        SyncEntityDefinitionsViewModel viewModel,
        ISyncEntityDefinitionClient client,
        ApiSession session,
        IGridColumnSettingsClient gridColumnSettingsClient)
        : this()
    {
        this.viewModel = viewModel;
        this.client = client;
        ViewModel.Filter.PageSize = 500;

        ConfigureCrudPermissions(session, new CrudOperationPermissions(
            PermissionCodes.SyncEntitiesView,
            PermissionCodes.SyncEntitiesCreate,
            PermissionCodes.SyncEntitiesEdit,
            PermissionCodes.SyncEntitiesDelete));
        ConfigureColumnPersonalization(gridColumnSettingsClient, FormKey);
        GridView.DoubleClick += GridViewDoubleClick;
    }

    private SyncEntityDefinitionsViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    private ISyncEntityDefinitionClient Client =>
        client ?? throw new InvalidOperationException("El cliente de entidades Sync no esta configurado.");

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null || IsInDesignMode())
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await ViewModel.LoadAsync();
            SetGridData(ViewModel.Definitions);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override Task CreateAsync() => OpenEditorAsync(null);

    protected override async Task EditAsync()
    {
        if (SelectedDefinition() is not { } definition)
        {
            ShowWarning("Seleccione una entidad de sincronizacion.");
            return;
        }

        await OpenEditorAsync(definition.Id);
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedDefinition() is not { } definition)
        {
            ShowWarning("Seleccione una entidad de sincronizacion.");
            return;
        }

        if (definition.IsSystem)
        {
            ShowWarning("Las entidades del sistema no se pueden eliminar.");
            return;
        }

        if (definition.IsInUse)
        {
            ShowWarning("La entidad esta utilizada por al menos un perfil de sincronizacion.");
            return;
        }

        if (!Confirm($"Eliminar la entidad {definition.Code}?"))
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await ViewModel.DeleteAsync(definition.Id);
            await LoadDataAsync();
        });
    }

    protected override Task HistoryAsync()
    {
        if (SelectedDefinition() is not { } definition)
        {
            ShowWarning("Seleccione una entidad de sincronizacion.");
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de entidad de sincronizacion",
            $"{definition.Code} - {definition.Name}",
            cancellationToken => Client.GetHistoryAsync(definition.Id, cancellationToken));
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

        ConfigureColumn(nameof(SyncEntityDefinitionListItem.Code), "Codigo", 0, 125);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.Name), "Entidad", 1, 220);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.DefaultExecutionOrder), "Orden", 2, 75, HorzAlignment.Far);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.SupportsIncremental), "Incremental", 3, 90, HorzAlignment.Center);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.SupportsInsert), "Insertar", 4, 75, HorzAlignment.Center);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.SupportsUpdate), "Actualizar", 5, 85, HorzAlignment.Center);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.SupportsDeactivate), "Desactivar", 6, 90, HorzAlignment.Center);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.DefaultKeyField), "Campo clave", 7, 120);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.DefaultModifiedAtField), "Campo modificacion", 8, 140);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.DependencyCount), "Dependencias", 9, 100, HorzAlignment.Far);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.IsOperative), "Operativa", 10, 80, HorzAlignment.Center);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.IsSystem), "Sistema", 11, 80, HorzAlignment.Center);
        ConfigureColumn(nameof(SyncEntityDefinitionListItem.StatusText), "Estado", 12, 85, HorzAlignment.Center);
    }

    private async Task OpenEditorAsync(int? id)
    {
        var editViewModel = new SyncEntityDefinitionEditViewModel(Client);
        await editViewModel.InitializeAsync(id);

        using var form = new SyncEntityEditForm(editViewModel);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void GridViewDoubleClick(object? sender, EventArgs e)
    {
        if (CanUpdate)
        {
            await ExecuteEditAsync();
        }
        else
        {
            await ExecuteConsultAsync();
        }
    }

    private SyncEntityDefinitionListItem? SelectedDefinition() =>
        SelectedGridItem<SyncEntityDefinitionListItem>();

    private void ConfigureColumn(
        string fieldName,
        string caption,
        int visibleIndex,
        int width,
        HorzAlignment alignment = HorzAlignment.Near)
    {
        if (GridView.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
        column.AppearanceCell.TextOptions.HAlignment = alignment;
        column.AppearanceHeader.TextOptions.HAlignment = alignment == HorzAlignment.Far
            ? HorzAlignment.Far
            : HorzAlignment.Near;
    }

    private bool IsInDesignMode()
    {
        return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
