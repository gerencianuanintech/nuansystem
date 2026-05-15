using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Accounting.ChartOfAccounts;

namespace NuanSystem.WinForms.Forms.Accounting.ChartOfAccounts;

public sealed class ChartOfAccountsForm : BaseGridCrudListForm
{
    private readonly ChartOfAccountsViewModel viewModel;
    private readonly ApiSession session;

    public ChartOfAccountsForm()
    {
        viewModel = null!;
        session = null!;
    }

    public ChartOfAccountsForm(ChartOfAccountsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        Text = "Plan de cuentas";
        ConfigureColumnPersonalization(columnSettingsClient, "chart-of-accounts");
        ConfigureCrudPermissions(session, CrudOperationPermissions.ChartOfAccounts);
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
        var lookup = await viewModel.GetLookupAsync();
        using var form = CreateEditForm(lookup);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Cuenta contable creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        var lookup = await viewModel.GetLookupAsync();
        using var form = CreateEditForm(lookup, fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Cuenta contable actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        var lookup = await viewModel.GetLookupAsync();
        using var form = CreateEditForm(lookup, fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Cuenta contable copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar la cuenta contable {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(ChartOfAccountItem.Code), "Codigo", 1, 110);
        ConfigureColumn(nameof(ChartOfAccountItem.Name), "Nombre", 2, 220);
        ConfigureColumn(nameof(ChartOfAccountItem.ExternalCode), "Cod. externo", 3, 110);
        ConfigureColumn(nameof(ChartOfAccountItem.AccountType), "Tipo", 4, 100);
        ConfigureColumn(nameof(ChartOfAccountItem.AccountClass), "Clase", 5, 100);
        ConfigureColumn(nameof(ChartOfAccountItem.ParentDisplay), "Cuenta padre", 6, 220);
        ConfigureColumn(nameof(ChartOfAccountItem.Level), "Nivel", 7, 70);
        ConfigureColumn(nameof(ChartOfAccountItem.IsTitle), "Titulo", 8, 70);
        ConfigureColumn(nameof(ChartOfAccountItem.AllowsMovement), "Mov.", 9, 70);
        ConfigureColumn(nameof(ChartOfAccountItem.CurrencyCode), "Moneda", 10, 80);
        ConfigureColumn(nameof(ChartOfAccountItem.Balance), "Saldo", 11, 100);
        ConfigureColumn(nameof(ChartOfAccountItem.BlockManualPosting), "Bloq. manual", 12, 100);
        ConfigureColumn(nameof(ChartOfAccountItem.RelevantForCashFlow), "Flujo caja", 13, 90);
        ConfigureColumn(nameof(ChartOfAccountItem.RequiresCostCenter), "Centro costo", 14, 110);
        ConfigureColumn(nameof(ChartOfAccountItem.RequiresThirdParty), "Tercero", 15, 90);
        ConfigureColumn(nameof(ChartOfAccountItem.RequiresProject), "Proyecto", 16, 90);
        ConfigureColumn(nameof(ChartOfAccountItem.IsActive), "Activo", 17, 70);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ChartOfAccountEditForm CreateEditForm(
        IReadOnlyCollection<ChartOfAccountLookupItem> lookup,
        ChartOfAccountItem? account = null,
        bool copyMode = false)
    {
        var form = new ChartOfAccountEditForm(
            session.CurrentCompany?.Id ?? 0,
            lookup,
            CanCreate,
            account,
            copyMode);

        form.CreateParentRequested += CreateParentFromLookupAsync;
        return form;
    }

    private async Task<ChartOfAccountLookupItem?> CreateParentFromLookupAsync(ChartOfAccountEditForm owner)
    {
        var lookup = await viewModel.GetLookupAsync();
        using var form = new ChartOfAccountEditForm(session.CurrentCompany?.Id ?? 0, lookup, false);
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        var created = await viewModel.CreateAndReturnAsync(form.Request);
        var refreshedLookup = await viewModel.GetLookupAsync();
        owner.RefreshParentAccounts(refreshedLookup, created.Id);
        return new ChartOfAccountLookupItem(
            created.Id,
            created.Code,
            created.Name,
            created.AccountType,
            created.ParentAccountId,
            created.Level,
            created.IsActive);
    }

    private ChartOfAccountItem? SelectedItem()
    {
        return SelectedGridItem<ChartOfAccountItem>();
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
}
