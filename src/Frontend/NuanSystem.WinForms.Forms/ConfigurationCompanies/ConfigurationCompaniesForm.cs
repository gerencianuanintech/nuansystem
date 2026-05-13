using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.ConfigurationCompanies.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.ConfigurationCompanies;

namespace NuanSystem.WinForms.Forms.ConfigurationCompanies;

public sealed partial class ConfigurationCompaniesForm : BaseGridCrudListForm
{
    private readonly ConfigurationCompaniesViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public ConfigurationCompaniesForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public ConfigurationCompaniesForm(ConfigurationCompaniesViewModel viewModel, ApiSession session, IAuditClient auditClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        WireEvents();
    }

    public ConfigurationCompaniesForm(ConfigurationCompaniesViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
        : this(viewModel, session, auditClient)
    {
        ConfigureColumnPersonalization(columnSettingsClient, "configuration-companies");
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

        using var form = new ConfigurationCompanyEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Compania creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (viewModel is null || SelectedItem() is not { } company)
        {
            return;
        }

        using var form = new ConfigurationCompanyEditForm(company);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(company.Id, form.Request);
        ShowSuccess("Compania actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } company)
        {
            return;
        }

        using var form = new ConfigurationCompanyEditForm(company, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Compania copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (viewModel is null || SelectedItem() is not { } company)
        {
            return;
        }

        if (!Confirm($"Eliminar la compania {company.CommercialName}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(company.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } company)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de compania",
            $"{company.Code} - {company.CommercialName}",
            cancellationToken => auditClient.GetSecurityChangesAsync("ConfigurationCompanies", company.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(ConfigurationCompanyItem.Code), "Codigo", 1, 90);
        ConfigureColumn(nameof(ConfigurationCompanyItem.CommercialName), "Nombre comercial", 2, 180);
        ConfigureColumn(nameof(ConfigurationCompanyItem.Email), "Correo", 3, 160);
        ConfigureColumn(nameof(ConfigurationCompanyItem.Phone), "Telefono", 4, 100);
        ConfigureColumn(nameof(ConfigurationCompanyItem.DatabaseName), "Base de datos", 5, 130);
        ConfigureColumn(nameof(ConfigurationCompanyItem.IsDefault), "Pred.", 6, 60);
        ConfigureColumn(nameof(ConfigurationCompanyItem.IsActive), "Activo", 7, 60);
    }

    private ConfigurationCompanyItem? SelectedItem()
    {
        return SelectedGridItem<ConfigurationCompanyItem>();
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
                PermissionCodes.CompaniesManage,
                PermissionCodes.CompaniesManage,
                PermissionCodes.CompaniesManage,
                PermissionCodes.CompaniesManage));
        }
    }
}
