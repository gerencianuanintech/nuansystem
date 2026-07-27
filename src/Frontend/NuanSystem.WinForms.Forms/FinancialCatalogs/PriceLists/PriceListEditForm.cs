using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Catalogs;
using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;
using NuanSystem.WinForms.Services.FinancialCatalogs.PriceLists;

namespace NuanSystem.WinForms.Forms.FinancialCatalogs.PriceLists;

public sealed partial class PriceListEditForm : BaseEditForm
{
    private static readonly FinancialCatalogDescriptor Descriptor = FinancialCatalogDescriptors.PriceLists;

    public PriceListEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public PriceListEditForm(IReadOnlyCollection<FinancialCatalogLookupItem> currencies)
        : this()
    {
        BindLookups(currencies);
    }

    public PriceListEditForm(
        IReadOnlyCollection<FinancialCatalogLookupItem> currencies,
        PriceListItem item,
        bool copyMode = false)
        : this(currencies)
    {
        LoadCatalog(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SavePriceListRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        if (lueCurrency.EditValue is null)
        {
            Validator.SetError(lueCurrency, "Seleccione la moneda.");
            isValid = false;
        }

        if (lueAppliesTo.EditValue is null)
        {
            Validator.SetError(lueAppliesTo, "Seleccione el ámbito.");
            isValid = false;
        }
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SavePriceListRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NormalizeText(memDescription.Text),
            Convert.ToString(lueCurrency.EditValue) ?? string.Empty,
            Convert.ToString(lueAppliesTo.EditValue) ?? string.Empty,
            chkIsDefault.Checked,
            chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = $"Nuevo {Descriptor.SingularTitle}";
        lblCode.Text = Descriptor.CodeLabel;
        lblName.Text = Descriptor.NameLabel;
        chkIsActive.Checked = true;
        lueCurrency.CreateButtonEnabled = false;
        lueCurrency.ClearButtonEnabled = false;
        lueAppliesTo.Properties.DataSource = new[]
        {
            new PriceListScopeOption("Sales", "Ventas"),
            new PriceListScopeOption("Purchasing", "Compras"),
            new PriceListScopeOption("Both", "Ventas y compras")
        };
        lueAppliesTo.Properties.ValueMember = nameof(PriceListScopeOption.Code);
        lueAppliesTo.Properties.DisplayMember = nameof(PriceListScopeOption.Name);
        lueAppliesTo.EditValue = "Both";
    }

    private void BindLookups(IReadOnlyCollection<FinancialCatalogLookupItem> currencies)
    {
        lueCurrency.Properties.DataSource = currencies;
        lueCurrency.Properties.ValueMember = nameof(FinancialCatalogLookupItem.Code);
        lueCurrency.Properties.DisplayMember = nameof(FinancialCatalogLookupItem.DisplayName);
        lueCurrency.EditValue = currencies.FirstOrDefault()?.Code;
    }

    private void LoadCatalog(PriceListItem item, bool copyMode)
    {
        Text = copyMode ? $"Copiar {Descriptor.SingularTitle}" : $"Editar {Descriptor.SingularTitle}";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        lueCurrency.EditValue = item.CurrencyCode;
        lueAppliesTo.EditValue = item.AppliesTo;
        chkIsDefault.Checked = item.IsDefault;
        chkIsActive.Checked = item.IsActive;
    }

    private static string? NormalizeText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SavePriceListRequest EmptyRequest() =>
        new(string.Empty, string.Empty, null, string.Empty, "Both", false, true);
}
