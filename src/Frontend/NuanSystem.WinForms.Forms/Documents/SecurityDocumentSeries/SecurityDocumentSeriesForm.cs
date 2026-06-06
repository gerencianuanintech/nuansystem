using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Documents.SecurityDocumentSeries;

namespace NuanSystem.WinForms.Forms.Documents.SecurityDocumentSeries;

public sealed partial class SecurityDocumentSeriesForm : BaseGridCrudListForm
{
    private readonly SecurityDocumentSeriesViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public SecurityDocumentSeriesForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public SecurityDocumentSeriesForm(
        SecurityDocumentSeriesViewModel viewModel,
        ApiSession session,
        IAuditClient auditClient,
        IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, "security-document-series");
        WireEvents();
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
        await viewModel.LoadLookupsAsync();
        using var form = new SecurityDocumentSeriesEditForm(viewModel.Lookups);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Serie de documento creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        await viewModel.LoadLookupsAsync();
        using var form = new SecurityDocumentSeriesEditForm(viewModel.Lookups, fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Serie de documento actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        await viewModel.LoadLookupsAsync();
        using var form = new SecurityDocumentSeriesEditForm(viewModel.Lookups, fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Serie de documento copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar la serie de documento {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de serie de documento",
            $"{item.Code} - {item.Name}",
            cancellationToken => auditClient.GetSecurityChangesAsync("SecurityDocumentSeries", item.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(SecurityDocumentSeriesItem.Code), "Codigo", 1, 100);
        ConfigureColumn(nameof(SecurityDocumentSeriesItem.Name), "Nombre", 2, 220);
        ConfigureColumn(nameof(SecurityDocumentSeriesItem.DocumentTypeName), "Tipo Documento", 3, 170);
        ConfigureColumn(nameof(SecurityDocumentSeriesItem.Prefix), "Prefijo", 4, 90);
        ConfigureColumn(nameof(SecurityDocumentSeriesItem.Establishment), "Establecimiento", 5, 130);
        ConfigureColumn(nameof(SecurityDocumentSeriesItem.EmissionPoint), "Pto. Emision", 6, 120);
        ConfigureColumn(nameof(SecurityDocumentSeriesItem.DisplayNumber), "Siguiente Nro.", 7, 120);
        ConfigureColumn(nameof(SecurityDocumentSeriesItem.IsActive), "Activo", 8, 70);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private SecurityDocumentSeriesItem? SelectedItem()
    {
        return SelectedGridItem<SecurityDocumentSeriesItem>();
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
        Text = "Series de Documentos";

        if (session is not null)
        {
            ConfigureCrudPermissions(session, CrudOperationPermissions.SecurityDocumentSeries);
        }
    }
}
