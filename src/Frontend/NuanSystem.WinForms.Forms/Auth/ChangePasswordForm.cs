using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Auth;

public sealed partial class ChangePasswordForm : XtraForm
{
    public ChangePasswordForm()
    {
        InitializeComponent();
        ApplyButtonIcons();
        btnGuardar.Click += BtnGuardar_Click;
    }

    public string CurrentPassword => txtClaveActual.Text;

    public string NewPassword => txtNuevaClave.Text;

    private void ApplyButtonIcons()
    {
        btnGuardar.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon("diskette_32.svg", Color.White);
        btnGuardar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnCancelar.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon("cancelar_32.svg", Color.White);
        btnCancelar.ImageOptions.SvgImageSize = new Size(32, 32);
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword))
        {
            XtraMessageBox.Show(this, "Ingrese la clave actual.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtClaveActual.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 8)
        {
            XtraMessageBox.Show(this, "La nueva clave debe tener al menos 8 caracteres.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNuevaClave.Focus();
            return;
        }

        if (!string.Equals(NewPassword, txtConfirmarClave.Text, StringComparison.Ordinal))
        {
            XtraMessageBox.Show(this, "La confirmacion no coincide.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtConfirmarClave.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
