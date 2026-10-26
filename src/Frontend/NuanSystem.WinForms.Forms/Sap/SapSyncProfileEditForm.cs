using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncProfileEditForm : BaseEditForm
{
    private readonly SapSyncProfileEditViewModel? viewModel;
    private readonly ISapMasterDataMappingClient? mappingClient;
    private SapSyncProfileEntityEditorRow? selectedEntity;
    private bool bindingEntity;

    public SapSyncProfileEditForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
        suppliersCard.Click += (_, _) => SelectEntity(0);
        customersCard.Click += (_, _) => SelectEntity(1);
        itemsCard.Click += (_, _) => SelectEntity(2);
        warehousesCard.Click += (_, _) => SelectEntity(3);
        purchaseOrdersCard.Click += (_, _) => SelectEntity(4);
        paymentTermsCard.Click += (_, _) => SelectEntity(5);
        btnSaveSchedule.Click += (_, _) => SaveSelectedSchedule();
        btnConfigureSupplierMapping.Click += async (_, _) => await ConfigureSelectedMappingAsync();
        btnValidateEntity.Click += (_, _) => ValidateSelectedEntity();
        btnLastExecution.Click += (_, _) => ShowInfo("La consulta de última ejecución se habilitará al completar el monitor de ejecuciones SAP.");
    }

    public SapSyncProfileEditForm(SapSyncProfileEditViewModel viewModel, ISapMasterDataMappingClient mappingClient) : this()
    {
        this.viewModel = viewModel;
        this.mappingClient = mappingClient;
        Bind();
    }

    private SapSyncProfileEditViewModel ViewModel => viewModel ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");

    private void Bind()
    {
        var state = ViewModel.State;
        companyEdit.Properties.DataSource = ViewModel.Catalog.Companies;
        companyEdit.Properties.DisplayMember = "DisplayName";
        companyEdit.Properties.ValueMember = "Id";
        companyEdit.EditValue = state.CompanyId;
        companyEdit.Properties.ReadOnly = state.Id.HasValue;
        codeEdit.Text = state.Code;
        nameEdit.Text = state.Name;
        descriptionEdit.Text = state.Description;
        statusLabel.Text = state.Id.HasValue ? (state.IsActive ? "Estado: Activo" : "Estado: Inactivo") : "Estado: Nuevo (inactivo al guardar)";
        directionEdit.Properties.Items.Clear(); directionEdit.Properties.Items.AddRange(ViewModel.Catalog.Directions.Select(item => item.Code).ToArray());
        modeEdit.Properties.Items.Clear(); modeEdit.Properties.Items.AddRange(ViewModel.Catalog.SyncModes.Select(item => item.Code).ToArray());
        scheduleEdit.Properties.Items.Clear(); scheduleEdit.Properties.Items.AddRange(ViewModel.Catalog.ScheduleTypes.Select(item => item.Code).ToArray());
        timezoneEdit.Properties.Items.Clear(); timezoneEdit.Properties.Items.Add(ViewModel.Catalog.DefaultTimeZoneId);
        BindEntityCard(suppliersCard, state.Entities, 0, "Proveedores");
        BindEntityCard(customersCard, state.Entities, 1, "Clientes");
        BindEntityCard(itemsCard, state.Entities, 2, "Ítems");
        BindEntityCard(warehousesCard, state.Entities, 3, "Bodegas");
        BindEntityCard(purchaseOrdersCard, state.Entities, 4, "Órdenes de compra");
        BindEntityCard(paymentTermsCard, state.Entities, 5, "Condiciones de pago");
        var firstEntity = state.Entities.Count > 0 ? 0 : -1;
        if (firstEntity >= 0) SelectEntity(firstEntity);
    }

    private static void BindEntityCard(SimpleButton card, IReadOnlyList<SapSyncProfileEntityEditorRow> entities, int index, string fallbackName)
    {
        if (index >= entities.Count)
        {
            card.Visible = false;
            return;
        }

        var entity = entities[index];
        card.Visible = true;
        card.Text = $"{entity.EntityName ?? fallbackName}\r\n{(entity.IsActive ? "Activa" : "Inactiva")}  |  {entity.Direction}";
        card.Tag = index;
    }

    private void SelectEntity(int index)
    {
        if (bindingEntity || viewModel is null || index < 0 || index >= ViewModel.State.Entities.Count) return;
        SaveSelectedSchedule();
        selectedEntity = ViewModel.State.Entities[index];
        bindingEntity = true;
        try
        {
            entityDetailLabel.Text = $"Configuración de entidad: {selectedEntity.EntityName}";
            directionEdit.EditValue = selectedEntity.Direction;
            modeEdit.EditValue = selectedEntity.SyncMode;
            batchEdit.Value = selectedEntity.BatchSize;
            attemptsEdit.Value = selectedEntity.MaxAttempts;
            timeoutEdit.Value = selectedEntity.ExecutionTimeoutMinutes;
            scheduleEdit.EditValue = selectedEntity.ScheduleType;
            intervalEdit.Value = selectedEntity.IntervalMinutes ?? 15;
            executionTimeEdit.Time = DateTime.Today.Add(selectedEntity.ExecutionTime ?? new TimeSpan(8, 0, 0));
            timezoneEdit.EditValue = selectedEntity.TimeZoneId;
            continueOnErrorEdit.Checked = selectedEntity.ContinueOnError;
            preventConcurrentEdit.Checked = selectedEntity.PreventConcurrentExecutions;
            scheduleActiveEdit.Checked = selectedEntity.ScheduleIsActive;
            btnConfigureSupplierMapping.Enabled = string.Equals(selectedEntity.EntityCode, "Suppliers", StringComparison.OrdinalIgnoreCase);
            HighlightEntityCards(index);
        }
        finally { bindingEntity = false; }
    }

    private void HighlightEntityCards(int selectedIndex)
    {
        var cards = new[] { suppliersCard, customersCard, itemsCard, warehousesCard, purchaseOrdersCard, paymentTermsCard };
        for (var index = 0; index < cards.Length; index++)
        {
            cards[index].Appearance.BackColor = index == selectedIndex ? BrandResources.Primary : Color.White;
            cards[index].Appearance.ForeColor = index == selectedIndex ? Color.White : Color.FromArgb(23, 32, 51);
            cards[index].Appearance.Options.UseBackColor = true;
            cards[index].Appearance.Options.UseForeColor = true;
        }
    }

    private void SaveSelectedSchedule()
    {
        if (bindingEntity || selectedEntity is null) return;
        selectedEntity.Direction = Convert.ToString(directionEdit.EditValue) ?? selectedEntity.Direction;
        selectedEntity.SyncMode = Convert.ToString(modeEdit.EditValue) ?? selectedEntity.SyncMode;
        selectedEntity.BatchSize = Convert.ToInt32(batchEdit.Value);
        selectedEntity.MaxAttempts = Convert.ToInt32(attemptsEdit.Value);
        selectedEntity.ExecutionTimeoutMinutes = Convert.ToInt32(timeoutEdit.Value);
        selectedEntity.ScheduleType = Convert.ToString(scheduleEdit.EditValue) ?? selectedEntity.ScheduleType;
        selectedEntity.IntervalMinutes = Convert.ToInt32(intervalEdit.Value);
        selectedEntity.ExecutionTime = executionTimeEdit.Time.TimeOfDay;
        selectedEntity.TimeZoneId = Convert.ToString(timezoneEdit.EditValue) ?? selectedEntity.TimeZoneId;
        selectedEntity.ContinueOnError = continueOnErrorEdit.Checked;
        selectedEntity.PreventConcurrentExecutions = preventConcurrentEdit.Checked;
        selectedEntity.ScheduleIsActive = scheduleActiveEdit.Checked;
        informationLabel.Text = "Programación actualizada en el perfil. Presione Guardar perfil para persistir los cambios.";
    }

    private async Task ConfigureSelectedMappingAsync()
    {
        SaveSelectedSchedule();
        if (selectedEntity?.Id is not { } entityId || ViewModel.State.Id is not { } profileId)
        {
            ShowWarning("Guarde primero el perfil SAP y la entidad antes de configurar sus mapeos.");
            return;
        }
        if (!string.Equals(selectedEntity.EntityCode, "Suppliers", StringComparison.OrdinalIgnoreCase))
        {
            ShowInfo("El formulario de mapeo para esta entidad se habilitará en la siguiente etapa.");
            return;
        }
        using var form = new SapSupplierMappingForm(new SapSupplierMappingViewModel(profileId, entityId, mappingClient ?? throw new InvalidOperationException("El cliente de mapeo SAP no esta configurado.")));
        form.ShowDialog(this);
        await Task.CompletedTask;
    }

    private void ValidateSelectedEntity()
    {
        SaveSelectedSchedule();
        if (selectedEntity is null) { ShowWarning("Seleccione una entidad."); return; }
        if (!selectedEntity.IsActive) { ShowWarning("La entidad está inactiva."); return; }
        ShowInfo($"La entidad {selectedEntity.EntityName} está lista para validarse con la configuración actual del perfil.");
    }

    protected override bool ValidateForm()
    {
        SaveSelectedSchedule();
        var valid = true;
        if (companyEdit.EditValue is null) { Validator.SetError(companyEdit, "Seleccione una empresa."); valid = false; }
        valid &= Validator.RequireText(codeEdit, "Ingrese el código.");
        valid &= Validator.RequireText(nameEdit, "Ingrese el nombre.");
        if (!ViewModel.State.Entities.Any(item => item.IsActive)) { ShowWarning("Active al menos una entidad SAP."); valid = false; }
        return valid;
    }

    protected override void BuildRequest()
    {
        SaveSelectedSchedule();
        ViewModel.State.CompanyId = Convert.ToInt32(companyEdit.EditValue);
        ViewModel.State.Code = codeEdit.Text;
        ViewModel.State.Name = nameEdit.Text;
        ViewModel.State.Description = descriptionEdit.Text;
    }

    protected override async Task<bool> PersistAsync() { await ViewModel.SaveAsync(); ShowSuccess("El perfil SAP se guardó correctamente."); return true; }

    private void ShowInfo(string message) => XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
