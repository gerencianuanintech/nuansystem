using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.ConfigurationSettings.Models;

namespace NuanSystem.WinForms.Forms.ConfigurationSettings;

public sealed partial class SettingsEditForm : BaseEditForm
{
    public SettingsEditForm(ConfigurationSettingItem? parameter = null, bool copyMode = false)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();

        if (parameter is not null)
        {
            Text = copyMode ? "Copiar parametro" : "Editar parametro";
            txtClave.Text = copyMode ? string.Empty : parameter.Key;
            txtClave.Properties.ReadOnly = !copyMode;
            memValor.Text = parameter.Value;
            memDescripcion.Text = parameter.Description;
            txtTipoDato.Text = parameter.DataType;
            txtCategoria.Text = parameter.Category;
            chkEncriptado.Checked = parameter.IsEncrypted;
            chkSistema.Checked = copyMode ? false : parameter.IsSystemParameter;
            chkEditable.Checked = parameter.IsEditable;
            sedOrden.Value = parameter.DisplayOrder;
            memValorDefecto.Text = parameter.DefaultValue;
            txtValidacion.Text = parameter.ValidationExpression;
            chkActivo.Checked = parameter.IsActive;
        }
        else
        {
            txtTipoDato.Text = "Text";
            chkEditable.Checked = true;
            chkActivo.Checked = true;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveConfigurationSettingRequest Request { get; private set; } = new(string.Empty, null, null, "Text", null, false, false, true, 0, null, null, true);

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtClave, "Clave es requerida.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveConfigurationSettingRequest(
            txtClave.Text.Trim(),
            string.IsNullOrWhiteSpace(memValor.Text) ? null : memValor.Text.Trim(),
            string.IsNullOrWhiteSpace(memDescripcion.Text) ? null : memDescripcion.Text.Trim(),
            string.IsNullOrWhiteSpace(txtTipoDato.Text) ? "Text" : txtTipoDato.Text.Trim(),
            string.IsNullOrWhiteSpace(txtCategoria.Text) ? null : txtCategoria.Text.Trim(),
            chkEncriptado.Checked,
            chkSistema.Checked,
            chkEditable.Checked,
            Convert.ToInt32(sedOrden.Value),
            string.IsNullOrWhiteSpace(memValorDefecto.Text) ? null : memValorDefecto.Text.Trim(),
            string.IsNullOrWhiteSpace(txtValidacion.Text) ? null : txtValidacion.Text.Trim(),
            chkActivo.Checked);
    }
}
