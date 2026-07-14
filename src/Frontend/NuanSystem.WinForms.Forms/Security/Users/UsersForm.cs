using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Security.Users.Models;
using NuanSystem.WinForms.ViewModels.Security.Users;
using RoleMaintenanceEditForm = NuanSystem.WinForms.Forms.Security.Roles.RoleEditForm;

namespace NuanSystem.WinForms.Forms.Security.Users;

public sealed partial class UsersForm : BaseGridCrudListForm
{
    private readonly UsersViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public UsersForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public UsersForm(UsersViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public UsersForm(UsersViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "users");
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

        await viewModel.LoadCatalogsAsync();
        using var form = CreateEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Usuario creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (viewModel is null || SelectedItem() is not { } user)
        {
            return;
        }

        await viewModel.LoadCatalogsAsync();
        using var form = CreateEditForm(user);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(user.Id, form.Request);
        ShowSuccess("Usuario actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } user)
        {
            return;
        }

        await viewModel.LoadCatalogsAsync();
        using var form = CreateEditForm(user, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Usuario copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (viewModel is null || SelectedItem() is not { } user)
        {
            return;
        }

        if (!Confirm($"Eliminar el usuario {user.UserName}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(user.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } user)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de usuario",
            $"{user.UserName} - {user.DisplayName}",
            cancellationToken => auditClient.GetSecurityChangesAsync("SecurityUsers", user.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(UserAdminItem.UserName), "Usuario", 1, 130);
        ConfigureColumn(nameof(UserAdminItem.DisplayName), "Nombre", 2, 180);
        ConfigureColumn(nameof(UserAdminItem.Email), "Correo", 3, 210);
        ConfigureColumn(nameof(UserAdminItem.IsActive), "Activo", 4, 70);
        ConfigureColumn(nameof(UserAdminItem.LastLoginAt), "Ultimo ingreso", 5, 130);
        ConfigureColumn(nameof(UserAdminItem.FailedAccessCount), "Intentos fallidos", 6, 110);
    }

    private UserAdminItem? SelectedItem()
    {
        return SelectedGridItem<UserAdminItem>();
    }

    private UserEditForm CreateEditForm(UserAdminItem? user = null, bool copyMode = false)
    {
        var form = new UserEditForm(viewModel.Roles, user, copyMode, viewModel.CanCreateRoles);
        form.CreateRoleRequested += CreateRoleFromLookupAsync;
        return form;
    }

    private async Task<RoleItem?> CreateRoleFromLookupAsync(UserEditForm owner)
    {
        if (!viewModel.CanCreateRoles)
        {
            return null;
        }

        using var form = new RoleMaintenanceEditForm();
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        var created = await viewModel.CreateRoleAsync(form.Request);
        ShowSuccess("Rol creado correctamente.");
        await viewModel.LoadCatalogsAsync();
        owner.RefreshRoleLookup(viewModel.Roles, created.Id);

        return created;
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
                PermissionCodes.UsersManage,
                PermissionCodes.UsersManage,
                PermissionCodes.UsersManage,
                PermissionCodes.UsersManage));
        }
    }
}

