using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.SecurityMenus.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.SecurityMenus;

namespace NuanSystem.WinForms.Forms.SecurityMenus;

public sealed partial class MenusForm : BaseGridCrudListForm
{
    private readonly MenusViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public MenusForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public MenusForm(MenusViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public MenusForm(MenusViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "security-menus");
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadFormsAsync();
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

        using var form = new MenuEditForm(viewModel.Items, viewModel.Forms);
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
        if (viewModel is null || SelectedItem() is not { } menu)
        {
            return;
        }

        using var form = new MenuEditForm(viewModel.Items, viewModel.Forms, menu);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(menu.Id, form.Request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } menu)
        {
            return;
        }

        using var form = new MenuEditForm(viewModel.Items, viewModel.Forms, menu, copyMode: true);
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
        if (viewModel is null || SelectedItem() is not { } menu)
        {
            return;
        }

        if (!Confirm($"Eliminar el menu {menu.Name}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(menu.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } menu)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de menu",
            $"{menu.Code} - {menu.Name}",
            cancellationToken => auditClient.GetSecurityChangesAsync("SecurityMenus", menu.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(SecurityMenuItem.Code), "Codigo", 1, 150);
        ConfigureColumn(nameof(SecurityMenuItem.Name), "Nombre", 2, 180);
        ConfigureColumn(nameof(SecurityMenuItem.ParentCode), "Codigo padre", 3, 150);
        ConfigureColumn(nameof(SecurityMenuItem.ParentName), "Menu padre", 4, 180);
        ConfigureColumn(nameof(SecurityMenuItem.MenuTypeName), "Tipo", 5, 110);
        ConfigureColumn(nameof(SecurityMenuItem.FormCode), "Codigo formulario", 6, 150);
        ConfigureColumn(nameof(SecurityMenuItem.FormName), "Formulario", 7, 180);
        ConfigureColumn(nameof(SecurityMenuItem.DisplayOrder), "Orden", 8, 70);
        ConfigureColumn(nameof(SecurityMenuItem.IsVisible), "Visible", 9, 70);
        ConfigureColumn(nameof(SecurityMenuItem.IsActive), "Activo", 10, 70);
    }

    private SecurityMenuItem? SelectedItem()
    {
        return SelectedGridItem<SecurityMenuItem>();
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
