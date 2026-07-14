using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Security.Operations.Models;

namespace NuanSystem.WinForms.Forms.Security.Operations;

public sealed partial class OperationEditForm : BaseEditForm
{
    public OperationEditForm(OperationItem? operation = null, bool copyMode = false)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();

        if (operation is not null)
        {
            Text = copyMode ? "Copiar operacion" : "Editar operacion";
            codeTextEdit.Text = copyMode ? string.Empty : operation.Code;
            nameTextEdit.Text = operation.Name;
            descriptionMemoEdit.Text = operation.Description;
            ribbonPageTextEdit.Text = operation.RibbonPageName;
            ribbonGroupTextEdit.Text = operation.RibbonGroupName;
            actionKeyTextEdit.Text = copyMode ? string.Empty : operation.ActionKey;
            iconLargeTextEdit.Text = operation.IconLarge;
            iconSmallTextEdit.Text = operation.IconSmall;
            displayOrderSpinEdit.Value = operation.DisplayOrder;
            activeCheckEdit.Checked = operation.IsActive;
        }
        else
        {
            ribbonPageTextEdit.Text = "Inicio";
            ribbonGroupTextEdit.Text = "Acciones";
            activeCheckEdit.Checked = true;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveOperationRequest Request { get; private set; } = new(
        string.Empty,
        string.Empty,
        null,
        "Inicio",
        "Acciones",
        string.Empty,
        null,
        null,
        0,
        true);

    protected override bool ValidateForm()
    {
        return Validator.RequireText(codeTextEdit, "Codigo es requerido.")
            & Validator.RequireText(nameTextEdit, "Nombre es requerido.")
            & Validator.RequireText(ribbonPageTextEdit, "Menu es requerido.")
            & Validator.RequireText(ribbonGroupTextEdit, "Agrupado es requerido.")
            & Validator.RequireText(actionKeyTextEdit, "Accion es requerida.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveOperationRequest(
            codeTextEdit.Text.Trim(),
            nameTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(descriptionMemoEdit.Text) ? null : descriptionMemoEdit.Text.Trim(),
            ribbonPageTextEdit.Text.Trim(),
            ribbonGroupTextEdit.Text.Trim(),
            actionKeyTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(iconLargeTextEdit.Text) ? null : iconLargeTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(iconSmallTextEdit.Text) ? null : iconSmallTextEdit.Text.Trim(),
            Convert.ToInt32(displayOrderSpinEdit.Value),
            activeCheckEdit.Checked);
    }
}
