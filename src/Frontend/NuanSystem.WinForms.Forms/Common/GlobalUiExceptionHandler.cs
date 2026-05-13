using DevExpress.XtraEditors;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.WinForms.Forms.Common;

public static class GlobalUiExceptionHandler
{
    private static IAuditClient? auditClient;
    private static ApiSession? session;
    private static int isHandlingException;

    public static void Configure(IAuditClient client, ApiSession apiSession)
    {
        auditClient = client;
        session = apiSession;
    }

    public static void RegisterApplicationHandlers()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, args) => Handle(args.Exception, "ThreadException");
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                Handle(exception, "UnhandledException", showMessage: false);
            }
        };
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            Handle(args.Exception, "UnobservedTaskException", showMessage: false);
            args.SetObserved();
        };
    }

    public static void Handle(Exception exception, string actionName, string? formName = null, bool showMessage = true)
    {
        if (Interlocked.Exchange(ref isHandlingException, 1) == 1)
        {
            return;
        }

        try
        {
            Task.Run(() => TryRegisterErrorAsync(exception, actionName, formName)).Wait(TimeSpan.FromSeconds(6));

            if (showMessage)
            {
                XtraMessageBox.Show(
                    Form.ActiveForm,
                    "Ocurrio un error inesperado. El detalle fue registrado para revision.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        finally
        {
            Interlocked.Exchange(ref isHandlingException, 0);
        }
    }

    private static async Task TryRegisterErrorAsync(Exception exception, string actionName, string? formName)
    {
        if (auditClient is null)
        {
            return;
        }

        try
        {
            var currentUser = session?.CurrentUser;
            var currentCompany = session?.CurrentCompany;
            var request = new CreateAuditErrorLogRequest(
                "WinForms",
                currentUser?.UserId,
                currentUser?.UserName,
                currentCompany?.Code,
                null,
                formName ?? Form.ActiveForm?.Name,
                actionName,
                null,
                null,
                null,
                null,
                Trim(exception.Message, 2000) ?? "Error no controlado en cliente.",
                Trim(exception.GetType().FullName, 300),
                exception.ToString(),
                Guid.NewGuid().ToString("N"),
                null,
                Trim(Environment.MachineName, 120),
                null);

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await auditClient.RegisterErrorAsync(request, cancellationTokenSource.Token).ConfigureAwait(false);
        }
        catch
        {
            // Evita que un fallo registrando auditoria provoque otro error en cascada.
        }
    }

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
