using DevExpress.XtraEditors;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.WinForms.Forms.Common;

public class BaseCrudListForm : XtraForm
{
    private SimpleButton? refreshButton;
    private SimpleButton? createButton;
    private SimpleButton? editButton;
    private SimpleButton? deleteButton;
    private bool canRefresh = true;
    private bool canCreate = true;
    private bool canCopy = true;
    private bool canUpdate = true;
    private bool canDelete = true;
    private bool canConsult = true;
    private bool canHistory = true;
    private bool canCustomizeColumns = true;
    private bool canExportExcel = true;
    private bool canExportPdf = true;
    private bool canExportJson = true;
    private bool canExportXml = true;
    private bool actionsEnabled = true;

    public event EventHandler? ActionStateChanged;

    public bool CanRefresh => actionsEnabled && canRefresh;

    public bool CanCreate => actionsEnabled && canCreate;

    public bool CanCopy => actionsEnabled && canCopy;

    public bool CanUpdate => actionsEnabled && canUpdate;

    public bool CanDelete => actionsEnabled && canDelete;

    public bool CanConsult => actionsEnabled && canConsult;

    public bool CanHistory => actionsEnabled && canHistory;

    public bool CanCustomizeColumns => actionsEnabled && canCustomizeColumns;

    public bool CanExportExcel => actionsEnabled && canExportExcel;

    public bool CanExportPdf => actionsEnabled && canExportPdf;

    public bool CanExportJson => actionsEnabled && canExportJson;

    public bool CanExportXml => actionsEnabled && canExportXml;

    protected void ConfigureCrudButtons(
        SimpleButton refreshButton,
        SimpleButton createButton,
        SimpleButton editButton,
        SimpleButton deleteButton)
    {
        this.refreshButton = refreshButton;
        this.createButton = createButton;
        this.editButton = editButton;
        this.deleteButton = deleteButton;

        refreshButton.Click += async (_, _) => await ExecuteRefreshAsync();
        createButton.Click += async (_, _) => await ExecuteCreateAsync();
        editButton.Click += async (_, _) => await ExecuteEditAsync();
        deleteButton.Click += async (_, _) => await ExecuteDeleteAsync();
    }

    protected void ConfigureCrudPermissions(ApiSession session, CrudOperationPermissions permissions)
    {
        canRefresh = session.HasPermission(permissions.Read);
        canCreate = session.HasPermission(permissions.Create);
        canUpdate = session.HasPermission(permissions.Update);
        canDelete = session.HasPermission(permissions.Delete);

        ApplyButtonPermissions();
    }

    public void ConfigureCrudOperationAccess(IEnumerable<string> allowedOperations)
    {
        var operations = new HashSet<string>(
            allowedOperations
                .Where(operation => !string.IsNullOrWhiteSpace(operation))
                .Select(NormalizeOperation),
            StringComparer.OrdinalIgnoreCase);

        if (operations.Count == 0)
        {
            canRefresh = false;
            canCreate = false;
            canCopy = false;
            canUpdate = false;
            canDelete = false;
            canConsult = false;
            canHistory = false;
            canCustomizeColumns = false;
            canExportExcel = false;
            canExportPdf = false;
            canExportJson = false;
            canExportXml = false;
            ApplyButtonPermissions();
            return;
        }

        canRefresh = HasAny(operations, "refresh", "actionrefresh", "actualizar", "actionactualizar", "reload", "actionreload");
        canCreate = HasAny(operations, "create", "actioncreate", "new", "actionnew", "nuevo", "actionnuevo", "crear");
        canCopy = HasAny(operations, "copy", "actioncopy", "copiar", "actioncopiar", "duplicate", "actionduplicate", "duplicar", "actionduplicar");
        canUpdate = HasAny(operations, "update", "actionupdate", "edit", "actionedit", "editar", "actioneditar", "modificar");
        canDelete = HasAny(operations, "delete", "actiondelete", "eliminar", "actioneliminar", "borrar");
        canConsult = HasAny(operations, "consult", "actionconsult", "consultar", "actionconsultar", "view", "actionview", "ver");
        canHistory = HasAny(operations, "history", "actionhistory", "historial", "actionhistorial", "audit", "actionaudit", "auditoria");
        canCustomizeColumns = HasAny(operations, "customizecolumns", "actioncustomizecolumns", "columns", "actioncolumns", "columnas", "personalizarcolumnas", "configurarcolumnas");
        canExportExcel = HasAny(operations, "exportexcel", "actionexportexcel", "excel", "actionexcel");
        canExportPdf = HasAny(operations, "exportpdf", "actionexportpdf", "pdf", "actionpdf");
        canExportJson = HasAny(operations, "exportjson", "actionexportjson", "json", "actionjson");
        canExportXml = HasAny(operations, "exportxml", "actionexportxml", "xml", "actionxml");

        ApplyButtonPermissions();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadDataAsync();
    }

    public Task ExecuteRefreshAsync()
    {
        return CanRefresh ? LoadDataAsync() : Task.CompletedTask;
    }

    public Task ExecuteCreateAsync()
    {
        return CanCreate ? CreateAsync() : Task.CompletedTask;
    }

    public Task ExecuteCopyAsync()
    {
        return CanCopy ? ExecuteCopyCoreAsync() : Task.CompletedTask;
    }

    public Task ExecuteEditAsync()
    {
        return CanUpdate ? ExecuteEditCoreAsync() : Task.CompletedTask;
    }

    public Task ExecuteDeleteAsync()
    {
        return CanDelete ? ExecuteDeleteCoreAsync() : Task.CompletedTask;
    }

    public Task ExecuteConsultAsync()
    {
        return CanConsult ? ExecuteConsultCoreAsync() : Task.CompletedTask;
    }

    public Task ExecuteHistoryAsync()
    {
        return CanHistory ? ExecuteHistoryCoreAsync() : Task.CompletedTask;
    }

    public Task ExecuteCustomizeColumnsAsync()
    {
        return CanCustomizeColumns ? ExecuteCustomizeColumnsCoreAsync() : Task.CompletedTask;
    }

    protected virtual Task LoadDataAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task CreateAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task CopyAsync()
    {
        ShowWarning("La copia aun no esta configurada para este mantenimiento.");
        return Task.CompletedTask;
    }

    protected virtual Task EditAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task DeleteAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task HistoryAsync()
    {
        ShowWarning("El historial aun no esta configurado para este mantenimiento.");
        return Task.CompletedTask;
    }

    protected virtual Task CustomizeColumnsAsync()
    {
        ShowWarning("La personalizacion de columnas aun no esta configurada para este listado.");
        return Task.CompletedTask;
    }

    protected virtual async Task ConsultAsync()
    {
        using (BaseEditForm.BeginReadOnlyMode())
        {
            await EditAsync();
        }
    }

    protected virtual Task ExecuteEditCoreAsync()
    {
        return EditAsync();
    }

    protected virtual Task ExecuteCopyCoreAsync()
    {
        return CopyAsync();
    }

    protected virtual Task ExecuteDeleteCoreAsync()
    {
        return DeleteAsync();
    }

    protected virtual Task ExecuteConsultCoreAsync()
    {
        return ConsultAsync();
    }

    protected virtual Task ExecuteHistoryCoreAsync()
    {
        return HistoryAsync();
    }

    protected virtual Task ExecuteCustomizeColumnsCoreAsync()
    {
        return CustomizeColumnsAsync();
    }

    protected async Task RunWithBusyStateAsync(Func<Task> action)
    {
        ToggleButtons(false);
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, action);
        }
        finally
        {
            ToggleButtons(true);
        }
    }

    protected bool Confirm(string message)
    {
        return XtraMessageBox.Show(this, message, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    protected void ShowWarning(string message)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    protected void ShowSuccess(string message)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    protected void ShowError(string message)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void ToggleButtons(bool enabled)
    {
        actionsEnabled = enabled;

        if (refreshButton is not null)
        {
            refreshButton.Enabled = CanRefresh;
        }

        if (createButton is not null)
        {
            createButton.Enabled = CanCreate;
        }

        if (editButton is not null)
        {
            editButton.Enabled = CanUpdate;
        }

        if (deleteButton is not null)
        {
            deleteButton.Enabled = CanDelete;
        }

        ActionStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyButtonPermissions()
    {
        ToggleButtons(actionsEnabled);
    }

    private static bool HasAny(HashSet<string> operations, params string[] aliases)
    {
        return aliases.Select(NormalizeOperation).Any(operations.Contains);
    }

    private static string NormalizeOperation(string operation)
    {
        return operation.Trim().Replace("_", string.Empty).Replace("-", string.Empty).Replace(".", string.Empty).ToLowerInvariant();
    }
}
