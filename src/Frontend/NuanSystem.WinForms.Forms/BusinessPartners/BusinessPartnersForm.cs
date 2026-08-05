using DevExpress.Utils;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.BusinessPartners;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class BusinessPartnersForm : BaseGridCrudListForm
{
    private readonly BusinessPartnersViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient? auditClient;
    private readonly string partnerType;
    private readonly string formKey;
    private readonly string entityName;
    private readonly Func<string, Form?>? relatedMaintenanceFormFactory;
    private readonly IGeographyClient? geographyClient;

    public BusinessPartnersForm()
    {
        viewModel = null!;
        session = null!;
        partnerType = "Customer";
        formKey = "customers";
        entityName = "BusinessPartners.Customers";
        InitializeComponent();
        WireEvents();
    }

    public BusinessPartnersForm(
        BusinessPartnersViewModel viewModel,
        ApiSession session,
        IAuditClient? auditClient,
        IGridColumnSettingsClient? gridColumnSettingsClient,
        string partnerType,
        string formKey,
        string title,
        Func<string, Form?>? relatedMaintenanceFormFactory = null,
        IGeographyClient? geographyClient = null)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        this.partnerType = partnerType;
        this.formKey = formKey;
        entityName = partnerType == "Supplier" ? "BusinessPartners.Suppliers" : "BusinessPartners.Customers";
        this.relatedMaintenanceFormFactory = relatedMaintenanceFormFactory;
        this.geographyClient = geographyClient;

        InitializeComponent();
        Text = title;
        ConfigureColumnPersonalization(gridColumnSettingsClient, formKey);
        WireEvents();
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
            SetGridData(viewModel.Partners);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        var lookups = await viewModel.LoadLookupsAsync();
        if (ShowEditForm(null, lookups) is not { } request)
        {
            return;
        }

        await viewModel.CreateAsync(request);
        ShowSuccess("Registro creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected)
        {
            return;
        }

        var lookups = await viewModel.LoadLookupsAsync();
        var partner = await viewModel.GetByIdAsync(selected.Id);
        if (ShowEditForm(partner, lookups) is not { } request)
        {
            return;
        }

        await viewModel.UpdateAsync(partner.Id, request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected)
        {
            return;
        }

        var lookups = await viewModel.LoadLookupsAsync();
        var partner = await viewModel.GetByIdAsync(selected.Id);
        partner.Code = string.Empty;
        partner.SapCardCode = null;
        if (ShowEditForm(partner, lookups) is not { } request)
        {
            return;
        }

        await viewModel.CreateAsync(request);
        ShowSuccess("Registro copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } selected)
        {
            return;
        }

        if (!Confirm($"Eliminar {selected.Code} - {selected.Name}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(selected.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (auditClient is null || SelectedItem() is not { } item)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            $"Historial de {Text.ToLowerInvariant()}",
            $"{item.Code} - {item.Name}",
            cancellationToken => auditClient.GetInventoryChangesAsync(entityName, item.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(BusinessPartnerItem.Code), "Codigo", 1, 100);
        ConfigureColumn(nameof(BusinessPartnerItem.Name), "Razon social", 2, 230);
        ConfigureColumn(nameof(BusinessPartnerItem.CommercialName), "Nombre comercial", 3, 180);
        ConfigureColumn(nameof(BusinessPartnerItem.IdentificationTypeCode), "Tipo ID", 4, 80);
        ConfigureColumn(nameof(BusinessPartnerItem.IdentificationNumber), "Identificacion", 5, 140);
        ConfigureColumn(nameof(BusinessPartnerItem.Phone), "Telefono", 6, 115);
        ConfigureColumn(nameof(BusinessPartnerItem.Email), "Correo", 7, 190);
        ConfigureColumn(nameof(BusinessPartnerItem.PaymentTermName), "Condicion", 8, 150);
        ConfigureColumn(nameof(BusinessPartnerItem.CreditLimit), "Limite credito", 9, 120);
        ConfigureColumn(nameof(BusinessPartnerItem.CreditDays), "Plazo dias", 10, 90);
        ConfigureColumn(nameof(BusinessPartnerItem.CustomerAccountCode), "Cuenta cliente", 11, 130);
        ConfigureColumn(nameof(BusinessPartnerItem.SupplierAccountCode), "Cuenta proveedor", 12, 140);
        ConfigureColumn(nameof(BusinessPartnerItem.SapSyncStatus), "SAP", 13, 100);
        ConfigureColumn(nameof(BusinessPartnerItem.IsActive), "Activo", 14, 70);

        if (GridView.Columns[nameof(BusinessPartnerItem.CreditLimit)] is { } creditColumn)
        {
            creditColumn.DisplayFormat.FormatType = FormatType.Numeric;
            creditColumn.DisplayFormat.FormatString = "n2";
            creditColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        }

        if (GridView.Columns[nameof(BusinessPartnerItem.CreditDays)] is { } daysColumn)
        {
            daysColumn.DisplayFormat.FormatType = FormatType.Numeric;
            daysColumn.DisplayFormat.FormatString = "n0";
            daysColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        }

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private BusinessPartnerItem? SelectedItem()
    {
        return SelectedGridItem<BusinessPartnerItem>();
    }

    private SaveBusinessPartnerRequest? ShowEditForm(BusinessPartnerItem? partner, BusinessPartnerLookups lookups)
    {
        if (string.Equals(partnerType, "Supplier", StringComparison.OrdinalIgnoreCase))
        {
            using var supplierForm = new SupplierEditForm(
                partner,
                lookups,
                canCreateRelatedMasters: false,
                session,
                relatedMaintenanceFormFactory,
                viewModel.LoadLookupsAsync,
                geographyClient);
            return supplierForm.ShowDialog(this) == DialogResult.OK ? supplierForm.Request : null;
        }

        using var customerForm = new CustomerEditForm(partner, lookups);
        return customerForm.ShowDialog(this) == DialogResult.OK ? customerForm.Request : null;
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
            ConfigureCrudPermissions(session, CrudOperationPermissions.BusinessPartners);
        }
    }
}
