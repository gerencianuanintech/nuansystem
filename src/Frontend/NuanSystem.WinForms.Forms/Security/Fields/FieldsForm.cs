using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Security.Fields.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Security.Fields;

namespace NuanSystem.WinForms.Forms.Security.Fields;

public sealed partial class FieldsForm : BaseGridCrudListForm
{
    private readonly FieldsViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public FieldsForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public FieldsForm(FieldsViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public FieldsForm(FieldsViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "security-fields");
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

        using var form = new FieldEditForm(viewModel.Forms);
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
        if (viewModel is null || SelectedItem() is not { } securityField)
        {
            return;
        }

        using var form = new FieldEditForm(viewModel.Forms, securityField);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(securityField.Id, form.Request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } securityField)
        {
            return;
        }

        using var form = new FieldEditForm(viewModel.Forms, securityField, copyMode: true);
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
        if (viewModel is null || SelectedItem() is not { } securityField)
        {
            return;
        }

        if (!Confirm($"Eliminar el campo {securityField.Name}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(securityField.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } securityField)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de campo",
            $"{securityField.Code} - {securityField.Name}",
            cancellationToken => auditClient.GetSecurityChangesAsync("SecurityFields", securityField.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(FieldItem.Code), "Codigo", 1, 150);
        ConfigureColumn(nameof(FieldItem.Name), "Nombre", 2, 160);
        ConfigureColumn(nameof(FieldItem.FieldKey), "Campo", 3, 160);
        ConfigureColumn(nameof(FieldItem.FormCode), "Codigo formulario", 4, 150);
        ConfigureColumn(nameof(FieldItem.FormName), "Formulario", 5, 180);
        ConfigureColumn(nameof(FieldItem.ControlType), "Tipo control", 6, 120);
        ConfigureColumn(nameof(FieldItem.DataType), "Tipo dato", 7, 100);
        ConfigureColumn(nameof(FieldItem.IsRequired), "Requerido", 8, 80);
        ConfigureColumn(nameof(FieldItem.IsReadOnly), "Lectura", 9, 70);
        ConfigureColumn(nameof(FieldItem.IsVisible), "Visible", 10, 70);
        ConfigureColumn(nameof(FieldItem.IsActive), "Activo", 11, 70);
    }

    private FieldItem? SelectedItem()
    {
        return SelectedGridItem<FieldItem>();
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
