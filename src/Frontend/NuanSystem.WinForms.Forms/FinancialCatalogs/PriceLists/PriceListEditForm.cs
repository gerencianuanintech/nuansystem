using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Catalogs;
using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.FinancialCatalogs.PriceLists;

public sealed partial class PriceListEditForm : BaseEditForm
{
    private static readonly FinancialCatalogDescriptor Descriptor = FinancialCatalogDescriptors.PriceLists;

    public PriceListEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public PriceListEditForm(FinancialCatalogItem item, bool copyMode = false)
        : this()
    {
        LoadCatalog(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveFinancialCatalogRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveFinancialCatalogRequest(txtCode.Text.Trim(), txtName.Text.Trim(), NormalizeText(memDescription.Text), chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = $"Nuevo {Descriptor.SingularTitle}";
        lblCode.Text = Descriptor.CodeLabel;
        lblName.Text = Descriptor.NameLabel;
        chkIsActive.Checked = true;
        btnSave.Click += (_, _) => Save();
    }

    private void LoadCatalog(FinancialCatalogItem item, bool copyMode)
    {
        Text = copyMode ? $"Copiar {Descriptor.SingularTitle}" : $"Editar {Descriptor.SingularTitle}";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        chkIsActive.Checked = item.IsActive;
    }

    private static string? NormalizeText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SaveFinancialCatalogRequest EmptyRequest() => new(string.Empty, string.Empty, null, true);
}
