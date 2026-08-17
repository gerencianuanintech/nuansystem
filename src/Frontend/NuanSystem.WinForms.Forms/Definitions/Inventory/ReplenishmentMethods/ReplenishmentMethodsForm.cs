using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ReplenishmentMethods;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ReplenishmentMethods;

public sealed class ReplenishmentMethodsForm : BaseGridCrudListForm
{
    public const string FormKey = "replenishment-methods";
    private readonly ReplenishmentMethodsViewModel viewModel;
    private readonly ApiSession session;

    public ReplenishmentMethodsForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public ReplenishmentMethodsForm(
        ReplenishmentMethodsViewModel viewModel,
        ApiSession session,
        IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        ConfigureWindow();
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
        using var form = new ReplenishmentMethodEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Método de reposición creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ReplenishmentMethodEditForm(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Método de reposición actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ReplenishmentMethodEditForm(item, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Método de reposición copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item || !Confirm($"¿Eliminar el método de reposición {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de método de reposición",
            $"{item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0, "ReplenishmentMethod", item.Id.ToString(), change.Action, change.FieldName,
                    change.OldValue, change.NewValue, change.UserId, change.UserName,
                    "API", change.CreatedAt)).ToList();
            });
        form.ShowDialog(this);
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(ReplenishmentMethodItem.Code), "Código", 1, 130);
        Column(nameof(ReplenishmentMethodItem.Name), "Nombre", 2, 280);
        Column(nameof(ReplenishmentMethodItem.Description), "Descripción", 3, 420);
        Column(nameof(ReplenishmentMethodItem.SortOrder), "Orden", 4, 80, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ReplenishmentMethodItem.IsActive), "Activo", 5, 80, DevExpress.Utils.HorzAlignment.Center);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private void Column(string field, string caption, int index, int width, DevExpress.Utils.HorzAlignment? alignment = null)
    {
        if (GridView.Columns[field] is not { } column) return;
        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = index;
        column.Width = width;
        column.OptionsColumn.AllowEdit = false;
        if (alignment.HasValue) column.AppearanceCell.TextOptions.HAlignment = alignment.Value;
    }

    private void ConfigureWindow()
    {
        ClientSize = new Size(1100, 620);
        MinimumSize = new Size(900, 520);
        Name = nameof(ReplenishmentMethodsForm);
        Text = "Métodos de reposición";
    }

    private ReplenishmentMethodItem? SelectedItem() => SelectedGridItem<ReplenishmentMethodItem>();

    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.ReplenishmentMethods);
    }
}
