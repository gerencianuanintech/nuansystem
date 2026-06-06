using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.OperationalCatalogs.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.OperationalCatalogs;

namespace NuanSystem.WinForms.Forms.OperationalCatalogs;

public sealed partial class OperationalCatalogsForm : BaseGridCrudListForm
{
    private readonly OperationalCatalogsViewModel viewModel;
    private readonly ApiSession session;

    public OperationalCatalogsForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        ConfigureForm();
    }

    public OperationalCatalogsForm(
        OperationalCatalogsViewModel viewModel,
        ApiSession session,
        IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, "operational-catalogs");
        ConfigureForm();
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
        await viewModel.LoadParentValuesAsync();
        using var form = new OperationalCatalogEditForm(viewModel.CatalogKey, viewModel.ParentValues);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Valor de catalogo creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        await viewModel.LoadParentValuesAsync();
        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new OperationalCatalogEditForm(viewModel.CatalogKey, viewModel.ParentValues, fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Valor de catalogo actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        await viewModel.LoadParentValuesAsync();
        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new OperationalCatalogEditForm(viewModel.CatalogKey, viewModel.ParentValues, fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Valor de catalogo copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar el valor {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        ShowWarning("Historial preparado para integrarse con auditoria de catalogos operativos.");
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(OperationalCatalogItem.Code), "Codigo", 1, 110);
        ConfigureColumn(nameof(OperationalCatalogItem.Name), "Nombre", 2, 220);
        ConfigureColumn(nameof(OperationalCatalogItem.Description), "Descripcion", 3, 280);
        ConfigureColumn(nameof(OperationalCatalogItem.ParentCode), "Padre", 4, 100);
        ConfigureColumn(nameof(OperationalCatalogItem.DisplayOrder), "Orden", 5, 80);
        ConfigureColumn(nameof(OperationalCatalogItem.IsDefault), "Defecto", 6, 80);
        ConfigureColumn(nameof(OperationalCatalogItem.IsActive), "Activo", 7, 70);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private OperationalCatalogItem? SelectedItem()
    {
        return SelectedGridItem<OperationalCatalogItem>();
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

    private void ConfigureForm()
    {
        Text = "Catalogos operativos";

        lueCatalogKey.Properties.DataSource = OperationalCatalogDescriptors.All;
        lueCatalogKey.Properties.DisplayMember = nameof(OperationalCatalogDescriptor.Name);
        lueCatalogKey.Properties.ValueMember = nameof(OperationalCatalogDescriptor.CatalogKey);
        lueCatalogKey.Properties.NullText = string.Empty;
        lueCatalogKey.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueCatalogKey.Properties.Columns.Clear();
        lueCatalogKey.Properties.Columns.Add(new LookUpColumnInfo(nameof(OperationalCatalogDescriptor.Name), "Catalogo", 220));
        lueCatalogKey.EditValue = OperationalCatalogDescriptors.DocumentEstablishment;
        lueCatalogKey.EditValueChanged += async (_, _) => await ChangeCatalogAsync();

        if (session is not null)
        {
            ConfigureCrudPermissions(session, CrudOperationPermissions.OperationalCatalogs);
        }
    }

    private async Task ChangeCatalogAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        viewModel.SetCatalogKey(Convert.ToString(lueCatalogKey.EditValue) ?? OperationalCatalogDescriptors.DocumentEstablishment);
        await LoadDataAsync();
    }
}
