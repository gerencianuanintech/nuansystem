using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;

public sealed partial class ItemGroupEditForm : BaseEditForm
{
    private readonly IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups;

    public ItemGroupEditForm()
        : this(Array.Empty<ChartOfAccountLookupItem>())
    {
    }

    public ItemGroupEditForm(IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups)
    {
        this.accountLookups = accountLookups;
        InitializeComponent();
        ConfigureForm();
    }

    public ItemGroupEditForm(ItemGroupItem itemGroup, bool copyMode = false)
        : this(itemGroup, Array.Empty<ChartOfAccountLookupItem>(), copyMode)
    {
    }

    public ItemGroupEditForm(ItemGroupItem itemGroup, IReadOnlyCollection<ChartOfAccountLookupItem> accountLookups, bool copyMode = false)
    {
        this.accountLookups = accountLookups;
        InitializeComponent();
        ConfigureForm();
        LoadItemGroup(itemGroup, copyMode);
    }

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
        btnGuardar.Click += (_, _) => Save();
        chkActivo.Checked = true;
        ConfigureAccountLookup(lueCuentaInventario);
        ConfigureAccountLookup(lueCuentaCostoVentas);
        ConfigureAccountLookup(lueCuentaVentas);
        ConfigureAccountLookup(lueCuentaCompras);
    }

    private void ConfigureAccountLookup(LookUpEdit lookup)
    {
        if (lookup.Properties.Buttons.Count == 1)
        {
            lookup.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
        }

        lookup.Properties.DataSource = accountLookups;
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
        lookup.ButtonClick += (_, e) =>
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                lookup.EditValue = null;
            }
        };
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
