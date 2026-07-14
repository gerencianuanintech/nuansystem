using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.OperationalCatalogs.Models;

namespace NuanSystem.WinForms.Forms.OperationalCatalogs;

public sealed partial class OperationalCatalogEditForm : BaseEditForm
{
    private readonly string catalogKey;
    private readonly IReadOnlyCollection<OperationalCatalogLookupItem> parentValues;

    public OperationalCatalogEditForm(
        string catalogKey,
        IReadOnlyCollection<OperationalCatalogLookupItem> parentValues)
    {
        this.catalogKey = catalogKey;
        this.parentValues = parentValues;
        InitializeComponent();
        ConfigureForm();
    }

    public OperationalCatalogEditForm(
        string catalogKey,
        IReadOnlyCollection<OperationalCatalogLookupItem> parentValues,
        OperationalCatalogItem item,
        bool copyMode = false)
        : this(catalogKey, parentValues)
    {
        LoadItem(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveOperationalCatalogRequest Request { get; private set; } =
        new(string.Empty, string.Empty, null, null, null, 0, false, true);

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");

        if (lueParentCode.Enabled && lueParentCode.EditValue is null)
        {
            isValid = false;
            Validator.SetError(lueParentCode, "Seleccione el valor padre.");
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        var descriptor = OperationalCatalogDescriptors.All
            .FirstOrDefault(item => string.Equals(item.CatalogKey, catalogKey, StringComparison.OrdinalIgnoreCase));

        Request = new SaveOperationalCatalogRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NormalizeText(memDescription.Text),
            descriptor?.ParentCatalogKey,
            NormalizeText(Convert.ToString(lueParentCode.EditValue)),
            Convert.ToInt32(sedDisplayOrder.Value),
            chkIsDefault.Checked,
            chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();

        var descriptor = OperationalCatalogDescriptors.All
            .FirstOrDefault(item => string.Equals(item.CatalogKey, catalogKey, StringComparison.OrdinalIgnoreCase));

        Text = "Nuevo valor de catalogo";
        txtCatalogKey.Text = descriptor?.Name ?? catalogKey;
        chkIsActive.Checked = true;

        lueParentCode.Properties.DataSource = parentValues;
        lueParentCode.Properties.DisplayMember = nameof(OperationalCatalogLookupItem.DisplayText);
        lueParentCode.Properties.ValueMember = nameof(OperationalCatalogLookupItem.Code);
        lueParentCode.Properties.NullText = string.Empty;
        lueParentCode.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueParentCode.Properties.Columns.Clear();
        lueParentCode.Properties.Columns.Add(new LookUpColumnInfo(nameof(OperationalCatalogLookupItem.Code), "Codigo", 70));
        lueParentCode.Properties.Columns.Add(new LookUpColumnInfo(nameof(OperationalCatalogLookupItem.Name), "Nombre", 180));

        var hasParent = !string.IsNullOrWhiteSpace(descriptor?.ParentCatalogKey);
        lblParentCode.Enabled = hasParent;
        lueParentCode.Enabled = hasParent;
    }

    private void LoadItem(OperationalCatalogItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar valor de catalogo" : "Editar valor de catalogo";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        lueParentCode.EditValue = item.ParentCode;
        sedDisplayOrder.Value = item.DisplayOrder;
        chkIsDefault.Checked = item.IsDefault;
        chkIsActive.Checked = item.IsActive;
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
