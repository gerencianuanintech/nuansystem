using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;

public sealed partial class ItemGroupEditForm : BaseEditForm
{
    private readonly IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups;
    private readonly bool canCreateAccount;
    private NuanLookupEdit? pendingCreateLookup;

    public ItemGroupEditForm()
        : this(Array.Empty<ChartOfAccountLookupItem>(), canCreateAccount: false)
    {
    }

    public ItemGroupEditForm(IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups, bool canCreateAccount = false)
    {
        this.accountLookups = accountLookups;
        this.canCreateAccount = canCreateAccount;
        InitializeComponent();
        ConfigureForm();
    }

    public ItemGroupEditForm(ItemGroupItem itemGroup, bool copyMode = false)
        : this(itemGroup, Array.Empty<ChartOfAccountLookupItem>(), copyMode)
    {
    }

    public ItemGroupEditForm(ItemGroupItem itemGroup, IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups, bool copyMode = false)
        : this(itemGroup, accountLookups, canCreateAccount: false, copyMode)
    {
    }

    public ItemGroupEditForm(
        ItemGroupItem itemGroup,
        IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups,
        bool canCreateAccount,
        bool copyMode = false)
    {
        this.accountLookups = accountLookups;
        this.canCreateAccount = canCreateAccount;
        InitializeComponent();
        ConfigureForm();
        LoadItemGroup(itemGroup, copyMode);
    }

    public event Func<ItemGroupEditForm, Task<ChartOfAccountLookupItem?>>? CreateAccountRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemGroupRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCodigo, "Ingrese el código del grupo.");
        isValid &= Validator.RequireText(txtNombre, "Ingrese el nombre del grupo.");

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemGroupRequest(
            txtCodigo.Text.Trim(),
            txtNombre.Text.Trim(),
            NormalizeText(memDescripcion.Text),
            chkActivo.Checked,
            NormalizeLookupValue(lueCuentaInventario.EditValue),
            NormalizeLookupValue(lueCuentaCostoVentas.EditValue),
            NormalizeLookupValue(lueCuentaVentas.EditValue),
            NormalizeLookupValue(lueCuentaCompras.EditValue),
            NormalizeText(txtGrupoSap.Text),
            NormalizeText(txtCodigoSap.Text));
    }

    public void RefreshAccountLookups(IReadOnlyCollection<ChartOfAccountLookupItem> accounts, string? selectedCode = null)
    {
        ConfigureAccountLookup(lueCuentaInventario, accounts);
        ConfigureAccountLookup(lueCuentaCostoVentas, accounts);
        ConfigureAccountLookup(lueCuentaVentas, accounts);
        ConfigureAccountLookup(lueCuentaCompras, accounts);

        if (!string.IsNullOrWhiteSpace(selectedCode) && pendingCreateLookup is not null)
        {
            pendingCreateLookup.EditValue = selectedCode;
        }
    }

    private void LoadItemGroup(ItemGroupItem itemGroup, bool copyMode)
    {
        Text = copyMode ? "Copiar grupo de artículos" : "Editar grupo de artículos";

        txtCodigo.Text = copyMode ? string.Empty : itemGroup.Code;
        txtNombre.Text = itemGroup.Name;
        memDescripcion.Text = itemGroup.Description;
        chkActivo.Checked = itemGroup.IsActive;
        lueCuentaInventario.EditValue = itemGroup.InventoryAccountCode;
        lueCuentaCostoVentas.EditValue = itemGroup.CostOfSalesAccountCode;
        lueCuentaVentas.EditValue = itemGroup.SalesAccountCode;
        lueCuentaCompras.EditValue = itemGroup.PurchaseAccountCode;
        txtGrupoSap.Text = itemGroup.SapGroupCode;
        txtCodigoSap.Text = itemGroup.SapCode;
    }

    private void ConfigureForm()
    {
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        chkActivo.Checked = true;
        ConfigureAccountLookup(lueCuentaInventario, accountLookups);
        ConfigureAccountLookup(lueCuentaCostoVentas, accountLookups);
        ConfigureAccountLookup(lueCuentaVentas, accountLookups);
        ConfigureAccountLookup(lueCuentaCompras, accountLookups);
    }

    private void ConfigureAccountLookup(NuanLookupEdit lookup, IReadOnlyCollection<ChartOfAccountLookupItem> accounts)
    {
        lookup.RefreshButtons();
        lookup.CreateButtonEnabled = canCreateAccount;
        lookup.Properties.DataSource = accounts;
        lookup.Properties.DisplayMember = nameof(ChartOfAccountLookupItem.DisplayText);
        lookup.Properties.ValueMember = nameof(ChartOfAccountLookupItem.Code);
        lookup.Properties.NullText = string.Empty;
        lookup.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lookup.Properties.SearchMode = SearchMode.AutoSearch;
        lookup.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.Code), "Codigo", 90));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.Name), "Nombre", 220));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.AccountType), "Tipo", 90));
        lookup.CreateButtonClick -= AccountLookupCreateButtonClick;
        lookup.CreateButtonClick += AccountLookupCreateButtonClick;
    }

    private async void AccountLookupCreateButtonClick(object? sender, EventArgs e)
    {
        if (sender is not NuanLookupEdit lookup || CreateAccountRequested is null || !canCreateAccount)
        {
            return;
        }

        pendingCreateLookup = lookup;
        try
        {
            var created = await CreateAccountRequested(this);
            if (created is not null)
            {
                lookup.EditValue = created.Code;
            }
        }
        finally
        {
            pendingCreateLookup = null;
        }
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeLookupValue(object? value)
    {
        return value is null ? null : NormalizeText(value.ToString());
    }

    private static SaveItemGroupRequest EmptyRequest()
    {
        return new SaveItemGroupRequest(string.Empty, string.Empty, null, true, null, null, null, null, null, null);
    }
}
