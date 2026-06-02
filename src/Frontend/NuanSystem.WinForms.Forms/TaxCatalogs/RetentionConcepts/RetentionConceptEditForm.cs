using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.RetentionConcepts;

public sealed partial class RetentionConceptEditForm : BaseEditForm
{
    private IReadOnlyCollection<TaxCatalogLookupItem> retentionTypes = Array.Empty<TaxCatalogLookupItem>();

    public RetentionConceptEditForm()
        : this(Array.Empty<TaxCatalogLookupItem>())
    {
    }

    public RetentionConceptEditForm(IReadOnlyCollection<TaxCatalogLookupItem> retentionTypes)
    {
        this.retentionTypes = retentionTypes;
        InitializeComponent();
        ConfigureForm();
        BindRetentionTypes();
    }

    public RetentionConceptEditForm(
        IReadOnlyCollection<TaxCatalogLookupItem> retentionTypes,
        RetentionConceptItem item,
        bool copyMode = false)
        : this(retentionTypes)
    {
        LoadConcept(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveRetentionConceptRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el concepto.");
        if (spnPercent.Value < 0)
        {
            Validator.SetError(spnPercent, "El porcentaje no puede ser negativo.");
            isValid = false;
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveRetentionConceptRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NormalizeText(memDescription.Text),
            ToNullableInt(lueRetentionType.EditValue),
            NormalizeText(txtSriCode.Text),
            spnPercent.Value,
            chkAppliesIva.Checked,
            chkAppliesIncome.Checked,
            chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = "Nuevo concepto de retencion";
        chkIsActive.Checked = true;
        btnSave.Click += (_, _) => Save();
    }

    private void BindRetentionTypes()
    {
        lueRetentionType.Properties.DataSource = retentionTypes.Where(item => item.IsActive).ToList();
        lueRetentionType.Properties.DisplayMember = nameof(TaxCatalogLookupItem.Name);
        lueRetentionType.Properties.ValueMember = nameof(TaxCatalogLookupItem.Id);
        lueRetentionType.Properties.Columns.Clear();
        lueRetentionType.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(TaxCatalogLookupItem.Code), "Codigo", 90));
        lueRetentionType.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(TaxCatalogLookupItem.Name), "Nombre", 180));
    }

    private void LoadConcept(RetentionConceptItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar concepto de retencion" : "Editar concepto de retencion";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        lueRetentionType.EditValue = item.RetentionTypeId;
        txtSriCode.Text = item.SriCode;
        spnPercent.Value = item.Percent;
        chkAppliesIva.Checked = item.AppliesIva;
        chkAppliesIncome.Checked = item.AppliesIncome;
        chkIsActive.Checked = item.IsActive;
    }

    private static int? ToNullableInt(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        if (value is int intValue)
        {
            return intValue;
        }

        return int.TryParse(Convert.ToString(value), out var parsedValue) ? parsedValue : null;
    }

    private static string? NormalizeText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SaveRetentionConceptRequest EmptyRequest()
        => new(string.Empty, string.Empty, null, null, null, 0, false, false, true);
}
