using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.SecurityForms.Models;

namespace NuanSystem.WinForms.Forms.SecurityForms;

public sealed partial class FormEditForm : BaseEditForm
{
    public FormEditForm(SecurityFormItem? securityForm = null, bool copyMode = false)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();

        if (securityForm is not null)
        {
            Text = copyMode ? "Copiar formulario" : "Editar formulario";
            codeTextEdit.Text = copyMode ? string.Empty : securityForm.Code;
            nameTextEdit.Text = securityForm.Name;
            descriptionMemoEdit.Text = securityForm.Description;
            formKeyTextEdit.Text = copyMode ? string.Empty : securityForm.FormKey;
            formTypeComboBoxEdit.SelectedIndex = Math.Max(0, securityForm.FormType - 1);
            hasListViewCheckEdit.Checked = securityForm.HasListView;
            hasEditViewCheckEdit.Checked = securityForm.HasEditView;
            visibleCheckEdit.Checked = securityForm.IsVisible;
            activeCheckEdit.Checked = securityForm.IsActive;
        }
        else
        {
            formTypeComboBoxEdit.SelectedIndex = 0;
            hasListViewCheckEdit.Checked = true;
            hasEditViewCheckEdit.Checked = true;
            visibleCheckEdit.Checked = true;
            activeCheckEdit.Checked = true;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveSecurityFormRequest Request { get; private set; } = new(
        string.Empty,
        string.Empty,
        null,
        string.Empty,
        1,
        true,
        true,
        true,
        true);

    protected override bool ValidateForm()
    {
        return Validator.RequireText(codeTextEdit, "Codigo es requerido.")
            & Validator.RequireText(nameTextEdit, "Nombre es requerido.")
            & Validator.RequireText(formKeyTextEdit, "Clave de formulario es requerida.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveSecurityFormRequest(
            codeTextEdit.Text.Trim(),
            nameTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(descriptionMemoEdit.Text) ? null : descriptionMemoEdit.Text.Trim(),
            formKeyTextEdit.Text.Trim(),
            formTypeComboBoxEdit.SelectedIndex + 1,
            hasListViewCheckEdit.Checked,
            hasEditViewCheckEdit.Checked,
            visibleCheckEdit.Checked,
            activeCheckEdit.Checked);
    }
}
