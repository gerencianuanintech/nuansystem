using System.Net.Mail;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;

namespace NuanSystem.WinForms.Forms.Common;

public sealed class FormValidator
{
    private readonly DXErrorProvider errorProvider;
    private Control? firstInvalidControl;

    public FormValidator(DXErrorProvider errorProvider)
    {
        this.errorProvider = errorProvider;
    }

    public void Clear()
    {
        firstInvalidControl = null;
        errorProvider.ClearErrors();
    }

    public void FocusFirstInvalid()
    {
        firstInvalidControl?.Focus();
    }

    public bool RequireText(TextEdit control, string message)
    {
        return RequireValue(control, control.Text, message);
    }

    public bool RequireMemo(MemoEdit control, string message)
    {
        return RequireValue(control, control.Text, message);
    }

    public bool RequireCombo(ComboBoxEdit control, string message)
    {
        return RequireValue(control, control.Text, message);
    }

    public bool RequireDecimal(TextEdit control, string message)
    {
        if (string.IsNullOrWhiteSpace(control.Text) || !decimal.TryParse(control.Text, out _))
        {
            SetError(control, message);
            return false;
        }

        ClearError(control);
        return true;
    }

    public bool RequireEmail(TextEdit control, string message)
    {
        if (string.IsNullOrWhiteSpace(control.Text) || !IsValidEmail(control.Text))
        {
            SetError(control, message);
            return false;
        }

        ClearError(control);
        return true;
    }

    public bool EmailIfPresent(TextEdit control, string message)
    {
        if (string.IsNullOrWhiteSpace(control.Text))
        {
            ClearError(control);
            return true;
        }

        if (!IsValidEmail(control.Text))
        {
            SetError(control, message);
            return false;
        }

        ClearError(control);
        return true;
    }

    private bool RequireValue(BaseEdit control, string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SetError(control, message);
            return false;
        }

        ClearError(control);
        return true;
    }

    public void SetError(Control control, string message)
    {
        firstInvalidControl ??= control;
        errorProvider.SetError(control, message, ErrorType.Critical);
    }

    private void ClearError(Control control)
    {
        errorProvider.SetError(control, string.Empty);
    }

    private static bool IsValidEmail(string value)
    {
        try
        {
            var email = new MailAddress(value.Trim());
            return email.Address.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
