using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncProfileListForm : BaseGridCrudListForm
{
    public const string FormKey = "sync-profiles";

    private SyncProfilesViewModel? viewModel;
    private ISyncConfigurationClient? client;
    private ApiSession? session;

    public SyncProfileListForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SyncProfileListForm(SyncProfilesViewModel viewModel, ISyncConfigurationClient client, ApiSession session)
        : this()
    {
        this.viewModel = viewModel;
        this.client = client;
        this.session = session;

        ConfigureCrudPermissions(session, new CrudOperationPermissions(
            PermissionCodes.SyncConfigurationView,
            PermissionCodes.SyncConfigurationCreate,
            PermissionCodes.SyncConfigurationEdit,
            PermissionCodes.SyncConfigurationDelete));
        WireEvents();
    }

    private SyncProfilesViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    private ISyncConfigurationClient Client =>
        client ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    private ApiSession Session =>
        session ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    protected override async Task LoadDataAsync()
    {
        if (IsInDesignMode() || viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await ViewModel.LoadAsync();
            SetGridData(ViewModel.Profiles);
        });
    }

    protected override async Task CreateAsync()
    {
        await OpenEditorAsync(null);
    }

    protected override async Task EditAsync()
    {
        if (GetSelectedProfile() is not { } profile)
        {
            ShowWarning("Seleccione un perfil.");
            return;
        }

        await OpenEditorAsync(profile.Id);
    }

    protected override async Task DeleteAsync()
    {
        if (GetSelectedProfile() is not { } profile)
        {
            ShowWarning("Seleccione un perfil.");
            return;
        }

        if (!Confirm($"Eliminar el perfil {profile.Code}?"))
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await ViewModel.DeleteAsync(profile.Id);
            await LoadDataAsync();
        });
    }

    private void WireEvents()
    {
        GridView.DoubleClick += async (_, _) => await ExecuteEditAsync();
    }

    private async Task OpenEditorAsync(int? id)
    {
        using var form = new SyncProfileEditForm(new SyncProfileEditViewModel(Client), id);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async Task ChangeActivationAsync(bool active)
    {
        if (GetSelectedProfile() is not { } profile)
        {
            ShowWarning("Seleccione un perfil.");
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            if (active)
            {
                await ViewModel.ActivateAsync(profile.Id);
            }
            else
            {
                await ViewModel.DeactivateAsync(profile.Id);
            }

            await LoadDataAsync();
        });
    }

    private async Task ValidateSelectedAsync()
    {
        if (GetSelectedProfile() is not { } profile)
        {
            ShowWarning("Seleccione un perfil.");
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            var result = await ViewModel.ValidatePersistedAsync(profile.Id);
            ShowValidationResult(result);
        });
    }

    private async Task ExecuteSelectedAsync()
    {
        if (GetSelectedProfile() is not { } profile)
        {
            ShowWarning("Seleccione un perfil.");
            return;
        }

        using var dialog = new ExecuteSyncProfileDialog(profile.Name);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            var result = await ViewModel.ExecuteAsync(profile.Id, dialog.Request);
            ShowSuccess($"Ejecucion {result.ExecutionId} creada en estado {result.Status}.");
            await LoadDataAsync();
        });
    }

    private void OpenExecutions()
    {
        var profileId = GetSelectedProfile()?.Id;
        using var form = new SyncExecutionListForm(
            new SyncExecutionsViewModel(Client),
            new SyncProfileExecutionDetailViewModel(Client),
            Session,
            profileId);
        form.ShowDialog(this);
    }

    private async Task OpenFiltersAsync()
    {
        using var dialog = new SyncProfileFilterDialog(ViewModel.Filter);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        ViewModel.Filter.Search = dialog.Search;
        ViewModel.Filter.IsActive = dialog.SelectedIsActive;
        ViewModel.Filter.ExecutionMode = dialog.ExecutionMode;
        ViewModel.Filter.PageNumber = 1;
        await LoadDataAsync();
    }

    private SyncProfileListItem? GetSelectedProfile()
    {
        return SelectedGridItem<SyncProfileListItem>();
    }

    public override bool CanExecuteCustomOperation(string operationKey)
    {
        return IsCustomOperation(operationKey, "activate")
            ? Session.HasPermission(PermissionCodes.SyncConfigurationActivate)
            : IsCustomOperation(operationKey, "deactivate")
                ? Session.HasPermission(PermissionCodes.SyncConfigurationActivate)
                : IsCustomOperation(operationKey, "filter", "filters", "filtro", "filtros")
                    ? Session.HasPermission(PermissionCodes.SyncConfigurationView)
                    : IsCustomOperation(operationKey, "validate")
                        ? Session.HasPermission(PermissionCodes.SyncConfigurationValidate)
                        : IsCustomOperation(operationKey, "execute")
                            ? Session.HasPermission(PermissionCodes.SyncConfigurationExecute)
                            : IsCustomOperation(operationKey, "executions", "viewexecutions", "view-executions")
                                ? Session.HasPermission(PermissionCodes.SyncConfigurationViewExecutions)
                                : base.CanExecuteCustomOperation(operationKey);
    }

    public override Task ExecuteCustomOperationAsync(string operationKey)
    {
        if (IsCustomOperation(operationKey, "activate"))
        {
            return ChangeActivationAsync(true);
        }

        if (IsCustomOperation(operationKey, "deactivate"))
        {
            return ChangeActivationAsync(false);
        }

        if (IsCustomOperation(operationKey, "validate"))
        {
            return ValidateSelectedAsync();
        }

        if (IsCustomOperation(operationKey, "filter", "filters", "filtro", "filtros"))
        {
            return OpenFiltersAsync();
        }

        if (IsCustomOperation(operationKey, "execute"))
        {
            return ExecuteSelectedAsync();
        }

        if (IsCustomOperation(operationKey, "executions", "viewexecutions", "view-executions"))
        {
            OpenExecutions();
            return Task.CompletedTask;
        }

        return base.ExecuteCustomOperationAsync(operationKey);
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(SyncProfileListItem.Code), "Codigo", 1, 110);
        ConfigureColumn(nameof(SyncProfileListItem.Name), "Perfil", 2, 240);
        ConfigureColumn(nameof(SyncProfileListItem.CompanyName), "Empresa maestra", 3, 220);
        ConfigureColumn(nameof(SyncProfileListItem.Direction), "Direccion", 4, 130);
        ConfigureColumn(nameof(SyncProfileListItem.ExecutionMode), "Modo", 5, 110);
        ConfigureColumn(nameof(SyncProfileListItem.BatchSize), "Batch", 6, 90);
        ConfigureColumn(nameof(SyncProfileListItem.MaxRetries), "Reintentos", 7, 90);
        ConfigureColumn(nameof(SyncProfileListItem.StatusText), "Estado", 8, 90);
        ConfigureColumn(nameof(SyncProfileListItem.NextExecutionAt), "Proxima ejecucion", 9, 150);
        ConfigureColumn(nameof(SyncProfileListItem.LastExecutionAt), "Ultima ejecucion", 10, 150);
        ConfigureColumn(nameof(SyncProfileListItem.BranchCount), "Sucursales", 11, 90);
        ConfigureColumn(nameof(SyncProfileListItem.EntityCount), "Entidades", 12, 90);
    }

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
    }

    private static bool IsCustomOperation(string operationKey, params string[] aliases)
    {
        var normalized = NormalizeOperation(operationKey);
        return aliases.Select(NormalizeOperation).Any(alias => string.Equals(normalized, alias, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeOperation(string operationKey)
    {
        return operationKey
            .Replace("ACTION.", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private void ShowValidationResult(SyncProfileValidationResult result)
    {
        var messages = result.Errors.Concat(result.Warnings)
            .Select(message => $"{message.Code}: {message.Message}")
            .ToArray();

        var text = messages.Length == 0
            ? "Validacion completada sin observaciones."
            : string.Join(Environment.NewLine, messages);

        XtraMessageBox.Show(this, text, result.IsValid ? "Validacion correcta" : "Validacion con errores", MessageBoxButtons.OK,
            result.IsValid ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
