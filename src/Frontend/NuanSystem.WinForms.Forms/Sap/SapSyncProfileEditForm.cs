using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncProfileEditForm : BaseEditForm
{
    // The entity/schedule matrix intentionally uses GridControl because it is an
    // inline editable master-detail editor; NuanDataGridControl is read/paging oriented.
    private readonly SapSyncProfileEditViewModel? viewModel;
    public SapSyncProfileEditForm() { InitializeComponent(); FormStyler.ApplyBase(this); }
    public SapSyncProfileEditForm(SapSyncProfileEditViewModel viewModel) : this() { this.viewModel = viewModel; Bind(); }
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
        directionRepository.Items.Clear();
        directionRepository.Items.AddRange(ViewModel.Catalog.Directions.Select(item => item.Code).ToArray());
        modeRepository.Items.Clear();
        modeRepository.Items.AddRange(ViewModel.Catalog.SyncModes.Select(item => item.Code).ToArray());
        scheduleRepository.Items.Clear();
        scheduleRepository.Items.AddRange(ViewModel.Catalog.ScheduleTypes.Select(item => item.Code).ToArray());
        entitiesGrid.DataSource = state.Entities;
    }

    protected override bool ValidateForm()
    {
        var valid = true;
        if (companyEdit.EditValue is null) { Validator.SetError(companyEdit, "Seleccione una empresa."); valid = false; }
        valid &= Validator.RequireText(codeEdit, "Ingrese el codigo.");
        valid &= Validator.RequireText(nameEdit, "Ingrese el nombre.");
        if (!ViewModel.State.Entities.Any(item => item.IsActive)) { ShowWarning("Active al menos una entidad SAP."); valid = false; }
        return valid;
    }

    protected override void BuildRequest()
    {
        ViewModel.State.CompanyId = Convert.ToInt32(companyEdit.EditValue);
        ViewModel.State.Code = codeEdit.Text;
        ViewModel.State.Name = nameEdit.Text;
        ViewModel.State.Description = descriptionEdit.Text;
        entitiesView.CloseEditor();
        entitiesView.UpdateCurrentRow();
    }

    protected override async Task<bool> PersistAsync() { await ViewModel.SaveAsync(); return true; }
}
