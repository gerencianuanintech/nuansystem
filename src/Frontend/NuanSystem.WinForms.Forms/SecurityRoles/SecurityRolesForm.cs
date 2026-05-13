using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.SecurityRoles.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.SecurityRoles;

namespace NuanSystem.WinForms.Forms.SecurityRoles;

public sealed partial class SecurityRolesForm : BaseGridCrudListForm
{
    private readonly SecurityRolesViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public SecurityRolesForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public SecurityRolesForm(SecurityRolesViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public SecurityRolesForm(SecurityRolesViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "security-roles");
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

        using var form = new SecurityRoleEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Rol creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (viewModel is null || SelectedItem() is not { } role)
        {
            return;
        }

        using var form = new SecurityRoleEditForm(role);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(role.Id, form.Request);
        ShowSuccess("Rol actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } role)
        {
            return;
        }

        using var form = new SecurityRoleEditForm(role, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Rol copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (viewModel is null || SelectedItem() is not { } role)
        {
            return;
        }

        if (!Confirm($"Eliminar el rol {role.Name}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(role.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } role)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de rol",
            $"{role.Code} - {role.Name}",
            cancellationToken => auditClient.GetSecurityChangesAsync("SecurityRoles", role.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(SecurityRoleItem.Code), "Codigo", 1, 120);
        ConfigureColumn(nameof(SecurityRoleItem.Name), "Nombre", 2, 180);
        ConfigureColumn(nameof(SecurityRoleItem.Description), "Descripcion", 3, 260);
        ConfigureColumn(nameof(SecurityRoleItem.DisplayOrder), "Orden", 4, 70);
        ConfigureColumn(nameof(SecurityRoleItem.IsSystemRole), "Sistema", 5, 70);
        ConfigureColumn(nameof(SecurityRoleItem.IsAssignable), "Asignable", 6, 80);
        ConfigureColumn(nameof(SecurityRoleItem.IsActive), "Activo", 7, 70);
    }

    private SecurityRoleItem? SelectedItem()
    {
        return SelectedGridItem<SecurityRoleItem>();
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
