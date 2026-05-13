using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;

public sealed partial class ItemGroupEditForm : BaseEditForm
{
    public ItemGroupEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public ItemGroupEditForm(ItemGroupItem itemGroup, bool copyMode = false)
    {
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
            NormalizeText(txtCuentaInventario.Text),
            NormalizeText(txtCuentaCostoVentas.Text),
            NormalizeText(txtCuentaVentas.Text),
            NormalizeText(txtCuentaCompras.Text),
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
        txtCuentaInventario.Text = itemGroup.InventoryAccountCode;
        txtCuentaCostoVentas.Text = itemGroup.CostOfSalesAccountCode;
        txtCuentaVentas.Text = itemGroup.SalesAccountCode;
        txtCuentaCompras.Text = itemGroup.PurchaseAccountCode;
        txtGrupoSap.Text = itemGroup.SapGroupCode;
        txtCodigoSap.Text = itemGroup.SapCode;
    }

    private void ConfigureForm()
    {
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();
        chkActivo.Checked = true;
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static SaveItemGroupRequest EmptyRequest()
    {
        return new SaveItemGroupRequest(string.Empty, string.Empty, null, true, null, null, null, null, null, null);
    }
}
