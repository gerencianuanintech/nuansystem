using System.Text;
using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Carriers.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Carriers;

namespace NuanSystem.WinForms.Forms.Carriers;

public sealed partial class CarriersForm : BaseGridCrudListForm
{
    public const string FormKey = "carriers";
    private static readonly CrudOperationPermissions Permissions = new(PermissionCodes.CarriersRead, PermissionCodes.CarriersManage, PermissionCodes.CarriersManage, PermissionCodes.CarriersManage);
    private readonly CarriersViewModel viewModel;
    private readonly ApiSession session;

    public CarriersForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WirePermissions();
    }

    public CarriersForm(CarriersViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, FormKey);
        WirePermissions();
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null) return;
        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        using var form = new CarrierEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Transportista creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item) return;
        var detail = await viewModel.GetByIdAsync(item.Id);
        using var form = new CarrierEditForm(detail);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Transportista actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item) return;
        var detail = await viewModel.GetByIdAsync(item.Id);
        using var form = new CarrierEditForm(detail, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Transportista copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item || !Confirm($"Eliminar transportista {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override async Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return;
        var changes = await viewModel.GetHistoryAsync(item.Id);
        if (changes.Count == 0)
        {
            ShowWarning("No existen cambios registrados para el transportista seleccionado.");
            return;
        }

        var text = new StringBuilder();
        foreach (var change in changes.Take(20))
        {
            text.AppendLine($"{change.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm} | {change.UserName ?? "Sistema"} | {change.Action}");
            text.AppendLine($"{change.FieldName}: {change.OldValue ?? "-"} -> {change.NewValue ?? "-"}");
            text.AppendLine();
        }
        XtraMessageBox.Show(this, text.ToString(), $"Historial - {item.Code}", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        ConfigureColumn(nameof(CarrierItem.Code), "Código", 1, 100);
        ConfigureColumn(nameof(CarrierItem.Name), "Nombre", 2, 220);
        ConfigureColumn(nameof(CarrierItem.IdentificationTypeDisplay), "Tipo de identificacion", 3, 150);
        ConfigureColumn(nameof(CarrierItem.IdentificationNumber), "Identificación", 4, 140);
        ConfigureColumn(nameof(CarrierItem.Description), "Descripcion", 5, 260);
        ConfigureColumn(nameof(CarrierItem.IsActive), "Activo", 6, 80);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private CarrierItem? SelectedItem() => SelectedGridItem<CarrierItem>();

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column) return;
        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
        column.OptionsColumn.AllowEdit = false;
    }

    private void WirePermissions()
    {
        Text = "Transportistas";
        if (session is not null) ConfigureCrudPermissions(session, Permissions);
    }
}
