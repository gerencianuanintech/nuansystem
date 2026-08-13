using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemGroups;

public sealed partial class ItemGroupEditForm : BaseEditForm
{
    private IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups;
    private readonly bool canCreateAccount;
    private readonly bool canEditAccount;
    private bool managingAccount;
    private NuanLookupEdit? activeAccountLookup;

    public ItemGroupEditForm() : this([], false, false) { }

    public ItemGroupEditForm(
        IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups,
        bool canCreateAccount,
        bool canEditAccount,
        ItemGroupItem? item = null,
        bool copyMode = false)
    {
        this.accountLookups = accountLookups;
        this.canCreateAccount = canCreateAccount;
        this.canEditAccount = canEditAccount;
        InitializeComponent();
        ConfigureForm();
        if (item is not null) LoadItem(item, copyMode);
    }

    public event Func<ItemGroupEditForm, Task<ChartOfAccountLookupItem?>>? CreateAccountRequested;
    public event Func<ItemGroupEditForm, string, Task<ChartOfAccountLookupItem?>>? EditAccountRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemGroupRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el código del grupo.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre del grupo.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemGroupRequest(
            txtCode.Text.Trim(), txtName.Text.Trim(), Optional(memDescription.Text),
            Value(lueInventory), Value(lueIncome), Value(lueCostOfSales), Value(lueSalesReturn),
            Value(luePurchaseReturn), Value(lueCostVariance), Value(lueInventoryAdjustment), Value(luePurchaseExpense),
            Convert.ToInt32(spnSortOrder.Value), chkIsActive.Checked,
            Optional(txtExternalSystem.Text), Optional(txtExternalCode.Text), Optional(txtSapGroupCode.Text), Optional(txtSapCode.Text));
    }

    public void RefreshAccountLookups(IReadOnlyCollection<ChartOfAccountLookupItem> accounts, string? selectedCode = null)
    {
        accountLookups = accounts;
        foreach (var lookup in AccountEditors()) BindAccountLookup(lookup);
        if (activeAccountLookup is not null && !string.IsNullOrWhiteSpace(selectedCode)) activeAccountLookup.EditValue = selectedCode;
        UpdateAccountButtons();
    }

    private void ConfigureForm()
    {
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        chkIsActive.Checked = true;
        foreach (var lookup in AccountEditors())
        {
            BindAccountLookup(lookup);
            lookup.CreateButtonClick += AccountCreateButtonClick;
            lookup.EditButtonClick += AccountEditButtonClick;
            lookup.EditValueChanged += (_, _) => UpdateAccountButtons();
        }
        UpdateAccountButtons();
    }

    private void BindAccountLookup(NuanLookupEdit lookup)
    {
        var selected = Value(lookup);
        var options = accountLookups.Where(account => account.IsActive || account.Code == selected).OrderBy(account => account.Code).ToArray();
        lookup.Properties.DataSource = options;
        lookup.Properties.DisplayMember = nameof(ChartOfAccountLookupItem.DisplayText);
        lookup.Properties.ValueMember = nameof(ChartOfAccountLookupItem.Code);
        lookup.Properties.NullText = string.Empty;
        lookup.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lookup.Properties.SearchMode = SearchMode.AutoSearch;
        lookup.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.Code), "Código", 100));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.Name), "Nombre", 220));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.AccountType), "Tipo", 90));
        lookup.RefreshButtons();
    }

    private async void AccountCreateButtonClick(object? sender, EventArgs e)
    {
        if (sender is not NuanLookupEdit lookup || CreateAccountRequested is null || !canCreateAccount || managingAccount) return;
        activeAccountLookup = lookup;
        await ManageAccountAsync(() => CreateAccountRequested(this));
    }

    private async void AccountEditButtonClick(object? sender, EventArgs e)
    {
        if (sender is not NuanLookupEdit lookup || EditAccountRequested is null || !canEditAccount || managingAccount) return;
        var code = Value(lookup);
        if (string.IsNullOrWhiteSpace(code)) return;
        activeAccountLookup = lookup;
        await ManageAccountAsync(() => EditAccountRequested(this, code));
    }

    private async Task ManageAccountAsync(Func<Task<ChartOfAccountLookupItem?>> operation)
    {
        managingAccount = true;
        UpdateAccountButtons();
        try
        {
            var account = await operation();
            if (account is not null) activeAccountLookup!.EditValue = account.Code;
        }
        finally
        {
            managingAccount = false;
            activeAccountLookup = null;
            UpdateAccountButtons();
        }
    }

    private void UpdateAccountButtons()
    {
        foreach (var lookup in AccountEditors())
        {
            lookup.CreateButtonEnabled = canCreateAccount && !managingAccount;
            lookup.EditButtonEnabled = canEditAccount && !managingAccount && !string.IsNullOrWhiteSpace(Value(lookup));
            lookup.ClearButtonEnabled = !managingAccount;
        }
    }

    private void LoadItem(ItemGroupItem item, bool copyMode)
    {
        Text = "Grupo de artículos";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        lueInventory.EditValue = item.InventoryAccountCode;
        lueIncome.EditValue = item.IncomeAccountCode;
        lueCostOfSales.EditValue = item.CostOfSalesAccountCode;
        lueSalesReturn.EditValue = item.SalesReturnAccountCode;
        luePurchaseReturn.EditValue = item.PurchaseReturnAccountCode;
        lueCostVariance.EditValue = item.CostVarianceAccountCode;
        lueInventoryAdjustment.EditValue = item.InventoryAdjustmentAccountCode;
        luePurchaseExpense.EditValue = item.PurchaseExpenseAccountCode;
        spnSortOrder.Value = item.SortOrder;
        chkIsSystem.Checked = item.IsSystem && !copyMode;
        chkIsActive.Checked = item.IsActive;
        txtExternalSystem.Text = item.ExternalSystem;
        txtExternalCode.Text = item.ExternalCode;
        txtSapGroupCode.Text = item.SapGroupCode;
        txtSapCode.Text = item.SapCode;
        foreach (var lookup in AccountEditors()) BindAccountLookup(lookup);
        if (item.IsSystem && !copyMode) txtCode.ReadOnly = true;
        UpdateAccountButtons();
    }

    private IEnumerable<NuanLookupEdit> AccountEditors()
    {
        yield return lueInventory; yield return lueIncome; yield return lueCostOfSales; yield return lueSalesReturn;
        yield return luePurchaseReturn; yield return lueCostVariance; yield return lueInventoryAdjustment; yield return luePurchaseExpense;
    }

    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? Value(NuanLookupEdit lookup) => Optional(Convert.ToString(lookup.EditValue));
    private static SaveItemGroupRequest EmptyRequest() => new(string.Empty, string.Empty, null, null, null, null, null, null, null, null, null, 0, true, null, null, null, null);
}
