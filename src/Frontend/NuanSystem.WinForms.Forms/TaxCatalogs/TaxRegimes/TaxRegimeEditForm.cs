using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.TaxRegimes;

public sealed partial class TaxRegimeEditForm : BaseEditForm
{
    private static readonly TaxCatalogDescriptor Descriptor = TaxCatalogDescriptors.TaxRegimes;

    public TaxRegimeEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public TaxRegimeEditForm(TaxCatalogItem item, bool copyMode = false)
        : this()
    {
        LoadCatalog(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveTaxCatalogRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveTaxCatalogRequest(txtCode.Text.Trim(), txtName.Text.Trim(), NormalizeText(memDescription.Text), chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = $"Nuevo {Descriptor.SingularTitle}";
        lblCode.Text = Descriptor.CodeLabel;
        lblName.Text = Descriptor.NameLabel;
        chkIsActive.Checked = true;
        btnSave.Click += (_, _) => Save();
    }

    private void LoadCatalog(TaxCatalogItem item, bool copyMode)
    {
        Text = copyMode ? $"Copiar {Descriptor.SingularTitle}" : $"Editar {Descriptor.SingularTitle}";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        chkIsActive.Checked = item.IsActive;
    }

    private static string? NormalizeText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SaveTaxCatalogRequest EmptyRequest() => new(string.Empty, string.Empty, null, true);
}
