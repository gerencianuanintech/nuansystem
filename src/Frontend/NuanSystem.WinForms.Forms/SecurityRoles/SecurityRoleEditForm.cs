using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.SecurityRoles.Models;

namespace NuanSystem.WinForms.Forms.SecurityRoles;

public sealed partial class SecurityRoleEditForm : BaseEditForm
{
    public SecurityRoleEditForm(SecurityRoleItem? role = null, bool copyMode = false)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();

        if (role is not null)
        {
            Text = copyMode ? "Copiar rol" : "Editar rol";
            txtCodigo.Text = copyMode ? string.Empty : role.Code;
            txtNombre.Text = role.Name;
            memDescripcion.Text = role.Description;
            sedOrden.Value = role.DisplayOrder;
            chkActivo.Checked = role.IsActive;
            chkSistema.Checked = copyMode ? false : role.IsSystemRole;
            chkAsignable.Checked = role.IsAssignable;
        }
        else
        {
            chkActivo.Checked = true;
            chkAsignable.Checked = true;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveSecurityRoleRequest Request { get; private set; } = new(string.Empty, string.Empty, null, 0, false, true, true);

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtCodigo, "Codigo es requerido.")
            & Validator.RequireText(txtNombre, "Nombre es requerido.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveSecurityRoleRequest(
            txtCodigo.Text.Trim().ToUpperInvariant(),
            txtNombre.Text.Trim(),
            string.IsNullOrWhiteSpace(memDescripcion.Text) ? null : memDescripcion.Text.Trim(),
            Convert.ToInt32(sedOrden.Value),
            chkSistema.Checked,
            chkAsignable.Checked,
            chkActivo.Checked);
    }
}
