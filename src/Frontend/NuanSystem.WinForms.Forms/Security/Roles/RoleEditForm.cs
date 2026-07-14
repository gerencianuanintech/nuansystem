using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Security.Roles.Models;

namespace NuanSystem.WinForms.Forms.Security.Roles;

public sealed partial class RoleEditForm : BaseEditForm
{
    public RoleEditForm(RoleItem? role = null, bool copyMode = false)
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
    public SaveRoleRequest Request { get; private set; } = new(string.Empty, string.Empty, null, 0, false, true, true);

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtCodigo, "Codigo es requerido.")
            & Validator.RequireText(txtNombre, "Nombre es requerido.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveRoleRequest(
            txtCodigo.Text.Trim().ToUpperInvariant(),
            txtNombre.Text.Trim(),
            string.IsNullOrWhiteSpace(memDescripcion.Text) ? null : memDescripcion.Text.Trim(),
            Convert.ToInt32(sedOrden.Value),
            chkSistema.Checked,
            chkAsignable.Checked,
            chkActivo.Checked);
    }
}
