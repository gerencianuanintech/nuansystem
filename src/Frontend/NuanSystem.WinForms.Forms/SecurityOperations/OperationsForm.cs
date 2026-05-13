using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.SecurityOperations.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.SecurityOperations;

namespace NuanSystem.WinForms.Forms.SecurityOperations;

public sealed partial class OperationsForm : BaseGridCrudListForm
{
    private readonly OperationsViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public OperationsForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public OperationsForm(OperationsViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public OperationsForm(OperationsViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "security-operations");
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        using var form = new OperationEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Registro creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (viewModel is null || SelectedItem() is not { } operation)
        {
            return;
        }

        using var form = new OperationEditForm(operation);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(operation.Id, form.Request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } operation)
        {
            return;
        }

        using var form = new OperationEditForm(operation, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Registro copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (viewModel is null || SelectedItem() is not { } operation)
        {
            return;
        }

        if (!Confirm($"Eliminar la operacion {operation.Name}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(operation.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } operation)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de operacion",
            $"{operation.Code} - {operation.Name}",
            cancellationToken => auditClient.GetSecurityChangesAsync("SecurityOperations", operation.Id.ToString(), 200, cancellationToken));

        form.ShowDialog(this);
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(SecurityOperationItem.Code), "Codigo", 1, 130);
        ConfigureColumn(nameof(SecurityOperationItem.Name), "Nombre", 2, 160);
        ConfigureColumn(nameof(SecurityOperationItem.Description), "Descripcion", 3, 240);
        ConfigureColumn(nameof(SecurityOperationItem.RibbonPageName), "Pagina ribbon", 4, 130);
        ConfigureColumn(nameof(SecurityOperationItem.RibbonGroupName), "Grupo ribbon", 5, 130);
        ConfigureColumn(nameof(SecurityOperationItem.ActionKey), "Accion", 6, 130);
        ConfigureColumn(nameof(SecurityOperationItem.DisplayOrder), "Orden", 7, 80);
    }

    private SecurityOperationItem? SelectedItem()
    {
        return SelectedGridItem<SecurityOperationItem>();
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

    private void WireEvents()
    {
        if (session is not null)
        {
            ConfigureCrudPermissions(session, new CrudOperationPermissions(
                PermissionCodes.RolesManage,
                PermissionCodes.RolesManage,
                PermissionCodes.RolesManage,
                PermissionCodes.RolesManage));
        }
    }
}
