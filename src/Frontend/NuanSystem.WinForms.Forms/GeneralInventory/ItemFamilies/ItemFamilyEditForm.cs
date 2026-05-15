using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemFamilies;

public sealed partial class ItemFamilyEditForm : BaseEditForm
{
    public ItemFamilyEditForm(IReadOnlyCollection<ItemGroupLookupItem> itemGroups, int? selectedItemGroupId = null)
    {
        InitializeComponent();
        ConfigureForm();
        LoadItemGroups(itemGroups);
        sleItemGroup.EditValue = selectedItemGroupId;
    }

    public ItemFamilyEditForm(IReadOnlyCollection<ItemGroupLookupItem> itemGroups, ItemFamilyItem itemFamily, bool copyMode = false)
        : this(itemGroups, itemFamily.ItemGroupId)
    {
        LoadItemFamily(itemFamily, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemFamilyRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= sleItemGroup.EditValue is not null || SetItemGroupError();
        isValid &= Validator.RequireText(txtCodigo, "Ingrese el codigo de la linea/familia.");
        isValid &= Validator.RequireText(txtNombre, "Ingrese el nombre de la linea/familia.");

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemFamilyRequest(
            Convert.ToInt32(sleItemGroup.EditValue),
            txtCodigo.Text.Trim(),
            txtNombre.Text.Trim(),
            NormalizeText(memDescripcion.Text),
            chkActivo.Checked,
            NormalizeText(txtGrupoSap.Text),
            NormalizeText(txtCodigoSap.Text));
    }

    private void LoadItemGroups(IReadOnlyCollection<ItemGroupLookupItem> itemGroups)
    {
        sleItemGroup.Properties.DataSource = itemGroups;
        sleItemGroup.Properties.DisplayMember = nameof(ItemGroupLookupItem.DisplayText);
        sleItemGroup.Properties.ValueMember = nameof(ItemGroupLookupItem.Id);
        grvItemGroupLookup.PopulateColumns(itemGroups);
        ConfigureLookupColumn("Id", visible: false);
        ConfigureLookupColumn("DisplayText", visible: false);
        ConfigureLookupColumn("Code", "Codigo", 0, 110);
        ConfigureLookupColumn("Name", "Nombre", 1, 220);
    }

    private void LoadItemFamily(ItemFamilyItem itemFamily, bool copyMode)
    {
        Text = copyMode ? "Copiar linea/familia" : "Editar linea/familia";
        sleItemGroup.EditValue = itemFamily.ItemGroupId;
        txtCodigo.Text = copyMode ? string.Empty : itemFamily.Code;
        txtNombre.Text = itemFamily.Name;
        memDescripcion.Text = itemFamily.Description;
        chkActivo.Checked = itemFamily.IsActive;
        txtGrupoSap.Text = itemFamily.SapFamilyCode;
        txtCodigoSap.Text = itemFamily.SapCode;
    }

    private void ConfigureForm()
    {
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();
        chkActivo.Checked = true;
    }

    private bool SetItemGroupError()
    {
        Validator.SetError(sleItemGroup, "Seleccione el grupo de articulos.");
        return false;
    }

    private void ConfigureLookupColumn(string fieldName, string? caption = null, int? visibleIndex = null, int? width = null, bool visible = true)
    {
        if (grvItemGroupLookup.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Visible = visible;
        if (!visible)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(caption))
        {
            column.Caption = caption;
        }

        if (visibleIndex.HasValue)
        {
            column.VisibleIndex = visibleIndex.Value;
        }

        if (width.HasValue)
        {
            column.Width = width.Value;
        }
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static SaveItemFamilyRequest EmptyRequest()
    {
        return new SaveItemFamilyRequest(0, string.Empty, string.Empty, null, true, null, null);
    }
}
