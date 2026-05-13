using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.SecurityFields.Models;
using NuanSystem.WinForms.Services.SecurityForms.Models;

namespace NuanSystem.WinForms.Forms.SecurityFields;

public sealed partial class FieldEditForm : BaseEditForm
{
    public FieldEditForm(IReadOnlyCollection<SecurityFormItem> forms, SecurityFieldItem? securityField = null, bool copyMode = false)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();
        LoadForms(forms);

        if (securityField is not null)
        {
            Text = copyMode ? "Copiar campo" : "Editar campo";
            formLookUpEdit.EditValue = securityField.FormId;
            codeTextEdit.Text = copyMode ? string.Empty : securityField.Code;
            nameTextEdit.Text = securityField.Name;
            fieldKeyTextEdit.Text = copyMode ? string.Empty : securityField.FieldKey;
            descriptionMemoEdit.Text = securityField.Description;
            controlTypeComboBoxEdit.Text = securityField.ControlType;
            dataTypeComboBoxEdit.Text = securityField.DataType;
            requiredCheckEdit.Checked = securityField.IsRequired;
            validationMessageTextEdit.Text = securityField.ValidationMessage;
            readOnlyCheckEdit.Checked = securityField.IsReadOnly;
            visibleCheckEdit.Checked = securityField.IsVisible;
            customCheckEdit.Checked = securityField.IsCustom;
            displayOrderSpinEdit.Value = securityField.DisplayOrder;
            activeCheckEdit.Checked = securityField.IsActive;
        }
        else
        {
            controlTypeComboBoxEdit.SelectedIndex = 0;
            dataTypeComboBoxEdit.SelectedIndex = 0;
            visibleCheckEdit.Checked = true;
            activeCheckEdit.Checked = true;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveSecurityFieldRequest Request { get; private set; } = new(
        0,
        string.Empty,
        string.Empty,
        string.Empty,
        null,
        "TextEdit",
        "string",
        false,
        null,
        false,
        true,
        false,
        0,
        true);

    protected override bool ValidateForm()
    {
        var hasForm = formLookUpEdit.EditValue is int formId && formId > 0;
        if (!hasForm)
        {
            Validator.SetError(formLookUpEdit, "Formulario es requerido.");
        }

        return hasForm
            & Validator.RequireText(codeTextEdit, "Codigo es requerido.")
            & Validator.RequireText(nameTextEdit, "Nombre es requerido.")
            & Validator.RequireText(fieldKeyTextEdit, "Campo es requerido.")
            & Validator.RequireText(controlTypeComboBoxEdit, "Tipo de control es requerido.")
            & Validator.RequireText(dataTypeComboBoxEdit, "Tipo de dato es requerido.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveSecurityFieldRequest(
            Convert.ToInt32(formLookUpEdit.EditValue),
            codeTextEdit.Text.Trim(),
            nameTextEdit.Text.Trim(),
            fieldKeyTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(descriptionMemoEdit.Text) ? null : descriptionMemoEdit.Text.Trim(),
            controlTypeComboBoxEdit.Text.Trim(),
            dataTypeComboBoxEdit.Text.Trim(),
            requiredCheckEdit.Checked,
            string.IsNullOrWhiteSpace(validationMessageTextEdit.Text) ? null : validationMessageTextEdit.Text.Trim(),
            readOnlyCheckEdit.Checked,
            visibleCheckEdit.Checked,
            customCheckEdit.Checked,
            Convert.ToInt32(displayOrderSpinEdit.Value),
            activeCheckEdit.Checked);
    }

    private void LoadForms(IReadOnlyCollection<SecurityFormItem> forms)
    {
        var formOptions = forms
            .OrderBy(form => form.Name)
            .Select(form => new FormOption(form.Id, form.Code, form.Name, form.FormKey))
            .ToList();

        formLookUpEdit.Properties.DataSource = formOptions;
        formLookUpEdit.Properties.DisplayMember = nameof(FormOption.DisplayText);
        formLookUpEdit.Properties.ValueMember = nameof(FormOption.Id);
        formLookUpEdit.Properties.NullText = "";
        formLookUpEdit.Properties.Columns.Clear();
        formLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(FormOption.Code), "Codigo", 160));
        formLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(FormOption.Name), "Nombre", 200));
        formLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(FormOption.FormKey), "Clave", 180));
    }

    private sealed record FormOption(int Id, string Code, string Name, string FormKey)
    {
        public string DisplayText => $"{Code} - {Name}";
    }
}
