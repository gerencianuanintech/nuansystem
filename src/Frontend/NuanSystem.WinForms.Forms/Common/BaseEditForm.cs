using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using System.ComponentModel;

namespace NuanSystem.WinForms.Forms.Common;

public class BaseEditForm : XtraForm
{
    private static readonly AsyncLocal<bool> ReadOnlyModeScope = new();
    private readonly DXErrorProvider errorProvider;

    public BaseEditForm()
    {
        AppTypography.ApplyToForm(this);

        errorProvider = new DXErrorProvider
        {
            ContainerControl = this
        };
        Validator = new FormValidator(errorProvider);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsReadOnlyMode { get; private set; }

    protected FormValidator Validator { get; }

    public static IDisposable BeginReadOnlyMode()
    {
        var previousValue = ReadOnlyModeScope.Value;
        ReadOnlyModeScope.Value = true;
        return new ReadOnlyModeToken(previousValue);
    }

    protected virtual bool ValidateForm()
    {
        return true;
    }

    protected virtual void BuildRequest()
    {
    }

    protected void Save()
    {
        if (IsReadOnlyMode)
        {
            return;
        }

        Validator.Clear();

        if (!ValidateForm())
        {
            Validator.FocusFirstInvalid();
            ShowWarning("Revise los campos resaltados.");
            return;
        }

        BuildRequest();
        DialogResult = DialogResult.OK;
        Close();
    }

    protected void ShowWarning(string message)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    protected void ShowError(Exception exception)
    {
        UiExceptionHandler.ShowError(this, Text, exception);
    }

    protected Task RunWithUiExceptionHandlingAsync(Func<Task> action)
    {
        return UiExceptionHandler.RunAsync(this, Text, action);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (ReadOnlyModeScope.Value)
        {
            ApplyReadOnlyMode();
        }
    }

    protected virtual void ApplyReadOnlyMode()
    {
        IsReadOnlyMode = true;
        Text = $"Consultar - {Text}";
        SetControlsReadOnly(this);
    }

    private static void SetControlsReadOnly(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            SetControlReadOnly(control);
            if (control.HasChildren)
            {
                SetControlsReadOnly(control);
            }
        }
    }

    private static void SetControlReadOnly(Control control)
    {
        if (control is SimpleButton button)
        {
            if (IsCancelButton(button))
            {
                button.Enabled = true;
                return;
            }

            button.Enabled = false;
            return;
        }

        if (control is BaseEdit edit)
        {
            edit.ReadOnly = true;
            edit.Properties.ReadOnly = true;
            edit.Enabled = false;
            edit.TabStop = false;
            return;
        }

        if (control is TextBoxBase textBox)
        {
            textBox.ReadOnly = true;
            textBox.Enabled = false;
            textBox.TabStop = false;
            return;
        }

        if (control is CheckBox checkBox)
        {
            checkBox.Enabled = false;
            checkBox.TabStop = false;
            return;
        }

        if (control is System.Windows.Forms.ComboBox comboBox)
        {
            comboBox.Enabled = false;
            comboBox.TabStop = false;
            return;
        }

        if (control is NumericUpDown numericUpDown)
        {
            numericUpDown.Enabled = false;
            numericUpDown.TabStop = false;
            return;
        }

        if (control is DataGridView dataGridView)
        {
            dataGridView.ReadOnly = true;
            dataGridView.Enabled = false;
            dataGridView.TabStop = false;
            return;
        }

        if (control is Button buttonControl)
        {
            if (IsCancelButton(buttonControl))
            {
                buttonControl.Enabled = true;
                return;
            }

            buttonControl.Enabled = false;
        }
    }

    private static bool IsCancelButton(Control control)
    {
        return control is IButtonControl { DialogResult: DialogResult.Cancel }
            || string.Equals(control.Text, "Cancelar", StringComparison.OrdinalIgnoreCase);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            errorProvider.Dispose();
        }

        base.Dispose(disposing);
    }

    private sealed class ReadOnlyModeToken(bool previousValue) : IDisposable
    {
        public void Dispose()
        {
            ReadOnlyModeScope.Value = previousValue;
        }
    }
}
