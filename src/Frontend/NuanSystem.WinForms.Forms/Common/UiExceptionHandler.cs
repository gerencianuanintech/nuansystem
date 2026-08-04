using DevExpress.XtraEditors;
using NuanSystem.WinForms.Services.Http;

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
            ShowError(owner, title, exception, registerError: false);
        }
    }

    public static void ShowError(IWin32Window owner, string title, Exception exception, bool registerError = true)
    {
        if (registerError)
        {
            GlobalUiExceptionHandler.Handle(exception, title, owner is Control control ? control.Name : null, showMessage: false);
        }

        XtraMessageBox.Show(owner, GetUserMessage(exception), title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static string GetUserMessage(Exception exception)
    {
        if (exception is ApiClientException apiException)
        {
            return ApiClientErrorMessageFormatter.Format(apiException);
        }

        return "Ocurrio un error inesperado. Intente nuevamente o contacte soporte.";
    }
}
