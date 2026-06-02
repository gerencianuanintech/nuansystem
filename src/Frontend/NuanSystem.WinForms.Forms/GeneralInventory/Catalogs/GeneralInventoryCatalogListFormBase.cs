using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;

public abstract class GeneralInventoryCatalogListFormBase : BaseGridCrudListForm
{
    private readonly GeneralInventoryCatalogDescriptor descriptor;
    private readonly GeneralInventoryCatalogsViewModel viewModel;
    private readonly ApiSession session;

    protected GeneralInventoryCatalogListFormBase(GeneralInventoryCatalogDescriptor descriptor)
    {
        this.descriptor = descriptor;
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    protected GeneralInventoryCatalogListFormBase(
        GeneralInventoryCatalogDescriptor descriptor,
        GeneralInventoryCatalogsViewModel viewModel,
        ApiSession session,
        IGridColumnSettingsClient columnSettingsClient)
    {
        this.descriptor = descriptor;
        this.viewModel = viewModel;
        this.session = session;
        ConfigureWindow();
        ConfigureColumnPersonalization(columnSettingsClient, descriptor.FormKey);
        WirePermissions();
    }

    protected abstract Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false);

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
        using var form = CreateEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(((IGeneralInventoryCatalogEditForm)form).Request);
        ShowSuccess("Registro creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = CreateEditForm(fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, ((IGeneralInventoryCatalogEditForm)form).Request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = CreateEditForm(fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(((IGeneralInventoryCatalogEditForm)form).Request);
        ShowSuccess("Registro copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar {descriptor.SingularTitle} {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        ShowWarning("Historial preparado para integrarse con auditoria de GeneralInventory.");
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(GeneralInventoryCatalogItem.Code), "Codigo", 1, 120);
        ConfigureColumn(nameof(GeneralInventoryCatalogItem.Name), "Nombre", 2, 240);
        ConfigureColumn(nameof(GeneralInventoryCatalogItem.Description), "Descripcion", 3, 360);
        ConfigureColumn(nameof(GeneralInventoryCatalogItem.IsActive), "Activo", 4, 80);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private GeneralInventoryCatalogItem? SelectedItem()
    {
        return SelectedGridItem<GeneralInventoryCatalogItem>();
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

    private void ConfigureWindow()
    {
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 600);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(860, 500);
        Name = GetType().Name;
        Text = descriptor.Title;
    }

    private void WirePermissions()
    {
        Text = descriptor.Title;
        if (session is not null)
        {
            ConfigureCrudPermissions(session, descriptor.Permissions);
        }
    }
}
