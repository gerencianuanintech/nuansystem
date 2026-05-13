using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.SecurityForms.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.SecurityForms;

namespace NuanSystem.WinForms.Forms.SecurityForms;

public sealed partial class FormsForm : BaseGridCrudListForm
{
    private readonly FormsViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public FormsForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public FormsForm(FormsViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public FormsForm(FormsViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "security-forms");
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

        using var form = new FormEditForm();
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
        if (viewModel is null || SelectedItem() is not { } securityForm)
        {
            return;
        }

        using var form = new FormEditForm(securityForm);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(securityForm.Id, form.Request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } securityForm)
        {
            return;
        }

        using var form = new FormEditForm(securityForm, copyMode: true);
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
        if (viewModel is null || SelectedItem() is not { } securityForm)
        {
            return;
        }

        if (!Confirm($"Eliminar el formulario {securityForm.Name}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(securityForm.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } securityForm)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de formulario",
            $"{securityForm.Code} - {securityForm.Name}",
            cancellationToken => auditClient.GetSecurityChangesAsync("SecurityForms", securityForm.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(SecurityFormItem.Code), "Codigo", 1, 150);
        ConfigureColumn(nameof(SecurityFormItem.Name), "Nombre", 2, 180);
        ConfigureColumn(nameof(SecurityFormItem.Description), "Descripcion", 3, 240);
        ConfigureColumn(nameof(SecurityFormItem.FormKey), "Clave", 4, 160);
        ConfigureColumn(nameof(SecurityFormItem.FormTypeName), "Tipo", 5, 110);
        ConfigureColumn(nameof(SecurityFormItem.IsVisible), "Visible", 6, 70);
        ConfigureColumn(nameof(SecurityFormItem.IsActive), "Activo", 7, 70);
    }

    private SecurityFormItem? SelectedItem()
    {
        return SelectedGridItem<SecurityFormItem>();
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
