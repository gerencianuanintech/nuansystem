using NuanSystem.WinForms.Forms.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemGroups;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemGroups;

public sealed class ItemGroupsForm : BaseGridCrudListForm
{
    public const string FormKey = "item-groups";
    private readonly ItemGroupsViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public ItemGroupsForm()
    {
        viewModel = null!; session = null!; auditClient = null!;
        ConfigureWindow(); WirePermissions();
    }

    public ItemGroupsForm(ItemGroupsViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel; this.session = session; this.auditClient = auditClient;
        ConfigureWindow(); ConfigureColumnPersonalization(columnSettingsClient, FormKey); WirePermissions();
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null) return;
        await RunWithBusyStateAsync(async () => { await viewModel.LoadAsync(); SetGridData(viewModel.Items); await ApplyColumnSettingsAsync(); });
    }

    protected override async Task CreateAsync()
    {
        await viewModel.LoadEditorContextAsync();
        using var form = CreateEditor();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request); ShowSuccess("Grupo de artículos creado correctamente."); await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id); await viewModel.LoadEditorContextAsync();
        using var form = CreateEditor(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request); ShowSuccess("Grupo de artículos actualizado correctamente."); await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id); await viewModel.LoadEditorContextAsync();
        using var form = CreateEditor(item, true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request); ShowSuccess("Grupo de artículos copiado correctamente."); await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (item.IsSystem) { ShowWarning("Los grupos del sistema no se pueden eliminar."); return; }
        if (!Confirm($"¿Eliminar el grupo de artículos {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id); await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm("Historial de grupo de artículos", $"{item.Code} - {item.Name}", token => auditClient.GetInventoryChangesAsync("ItemGroups", item.Id.ToString(), 200, token));
        form.ShowDialog(this); return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(ItemGroupItem.Code), "Código", 1, 105); Column(nameof(ItemGroupItem.Name), "Nombre", 2, 220);
        Column(nameof(ItemGroupItem.InventoryAccountCode), "Cuenta inventario", 3, 140); Column(nameof(ItemGroupItem.IncomeAccountCode), "Cuenta ingresos", 4, 140);
        Column(nameof(ItemGroupItem.CostOfSalesAccountCode), "Costo de ventas", 5, 140); Column(nameof(ItemGroupItem.SortOrder), "Orden", 6, 70);
        Column(nameof(ItemGroupItem.ExternalSystem), "Sistema externo", 7, 110); Column(nameof(ItemGroupItem.ExternalCode), "Código externo", 8, 110);
        Column(nameof(ItemGroupItem.IsSystem), "Sistema", 9, 75); Column(nameof(ItemGroupItem.IsActive), "Activo", 10, 70);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemGroupEditForm CreateEditor(ItemGroupItem? item = null, bool copy = false)
    {
        var form = new ItemGroupEditForm(viewModel.AccountLookups, viewModel.CanCreateAccounts, viewModel.CanEditAccounts, item, copy);
        form.CreateAccountRequested += CreateAccountAsync; form.EditAccountRequested += EditAccountAsync; return form;
    }

    private async Task<ChartOfAccountLookupItem?> CreateAccountAsync(ItemGroupEditForm owner)
    {
        if (!viewModel.CanCreateAccounts) return null;
        using var form = new ChartOfAccountEditForm(session.CurrentCompany?.Id ?? 0, viewModel.AccountLookups, viewModel.CanCreateAccounts);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.CreateAccountAsync(form.Request); await viewModel.LoadEditorContextAsync();
        owner.RefreshAccountLookups(viewModel.AccountLookups, saved.Code); ShowSuccess("Cuenta contable creada correctamente.");
        return viewModel.AccountLookups.FirstOrDefault(account => account.Id == saved.Id);
    }

    private async Task<ChartOfAccountLookupItem?> EditAccountAsync(ItemGroupEditForm owner, string code)
    {
        if (!viewModel.CanEditAccounts) return null;
        var lookup = viewModel.AccountLookups.FirstOrDefault(account => string.Equals(account.Code, code, StringComparison.OrdinalIgnoreCase));
        if (lookup is null) return null;
        var account = await viewModel.GetAccountByIdAsync(lookup.Id);
        using var form = new ChartOfAccountEditForm(session.CurrentCompany?.Id ?? 0, viewModel.AccountLookups, viewModel.CanCreateAccounts, account);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.UpdateAccountAsync(account.Id, form.Request); await viewModel.LoadEditorContextAsync();
        owner.RefreshAccountLookups(viewModel.AccountLookups, saved.Code); ShowSuccess("Cuenta contable actualizada correctamente.");
        return viewModel.AccountLookups.FirstOrDefault(item => item.Id == saved.Id);
    }

    private ItemGroupItem? SelectedItem() => SelectedGridItem<ItemGroupItem>();
    private void Column(string field, string caption, int index, int width) { if (GridView.Columns[field] is not { } column) return; column.Caption = caption; column.Visible = true; column.VisibleIndex = index; column.Width = width; column.OptionsColumn.AllowEdit = false; }
    private void ConfigureWindow() { ClientSize = new Size(1100, 640); MinimumSize = new Size(900, 520); Name = nameof(ItemGroupsForm); Text = "Grupos de artículos"; }
    private void WirePermissions() { if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.ItemGroups); }
}
