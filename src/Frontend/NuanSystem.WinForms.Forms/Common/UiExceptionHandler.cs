using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.Common;

public static class UiExceptionHandler
{
    public static async Task RunAsync(IWin32Window owner, string title, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception exception)
        {
            GlobalUiExceptionHandler.Handle(exception, title, owner is Control control ? control.Name : null, showMessage: false);
            XtraMessageBox.Show(owner, exception.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
