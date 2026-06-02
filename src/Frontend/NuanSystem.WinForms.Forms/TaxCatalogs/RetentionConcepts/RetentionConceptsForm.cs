using DevExpress.Utils;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;
using NuanSystem.WinForms.ViewModels.TaxCatalogs.Catalogs;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.RetentionConcepts;

public sealed partial class RetentionConceptsForm : BaseGridCrudListForm
{
    private static readonly TaxCatalogDescriptor Descriptor = TaxCatalogDescriptors.RetentionConcepts;
    private readonly RetentionConceptsViewModel viewModel;
    private readonly ApiSession session;

    public RetentionConceptsForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WirePermissions();
    }

    public RetentionConceptsForm(RetentionConceptsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, Descriptor.FormKey);
        WirePermissions();
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
        using var form = new RetentionConceptEditForm(await viewModel.GetRetentionTypesLookupAsync());
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
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
        using var form = new RetentionConceptEditForm(await viewModel.GetRetentionTypesLookupAsync(), fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
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
        using var form = new RetentionConceptEditForm(await viewModel.GetRetentionTypesLookupAsync(), fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Registro copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar {Descriptor.SingularTitle} {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        ShowWarning("Historial preparado para integrarse con auditoria de catalogos tributarios.");
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(RetentionConceptItem.Code), "Codigo", 1, 90);
        ConfigureColumn(nameof(RetentionConceptItem.SriCode), "Codigo SRI", 2, 90);
        ConfigureColumn(nameof(RetentionConceptItem.Name), "Concepto", 3, 240);
        ConfigureColumn(nameof(RetentionConceptItem.RetentionTypeName), "Tipo retencion", 4, 160);
        ConfigureColumn(nameof(RetentionConceptItem.Percent), "%", 5, 80);
        ConfigureColumn(nameof(RetentionConceptItem.AppliesIva), "Aplica IVA", 6, 90);
        ConfigureColumn(nameof(RetentionConceptItem.AppliesIncome), "Aplica renta", 7, 100);
        ConfigureColumn(nameof(RetentionConceptItem.IsActive), "Activo", 8, 80);

        if (GridView.Columns[nameof(RetentionConceptItem.Percent)] is { } percentColumn)
        {
            percentColumn.DisplayFormat.FormatType = FormatType.Numeric;
            percentColumn.DisplayFormat.FormatString = "n2";
            percentColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        }

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private RetentionConceptItem? SelectedItem() => SelectedGridItem<RetentionConceptItem>();

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

    private void WirePermissions()
    {
        Text = Descriptor.Title;
        if (session is not null)
        {
            ConfigureCrudPermissions(session, Descriptor.Permissions);
        }
    }
}
