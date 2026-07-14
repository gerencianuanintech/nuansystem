using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncProfileEditForm : XtraForm
{
    private SyncProfileEditViewModel? viewModel;
    private int? profileId;

    public SyncProfileEditForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
        ConfigureDesignerSafeRuntime();
    }

    public SyncProfileEditForm(SyncProfileEditViewModel viewModel, int? profileId)
        : this()
    {
        this.viewModel = viewModel;
        this.profileId = profileId;
        Text = profileId.HasValue ? "Editar perfil de sincronizacion" : "Nuevo perfil de sincronizacion";
        WireEvents();
    }

    private SyncProfileEditViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (IsInDesignMode() || viewModel is null)
        {
            return;
        }

        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            await ViewModel.InitializeAsync(profileId);
            BindCatalog();
            BindState();
        });
    }

    private void ConfigureDesignerSafeRuntime()
    {
        ApplyScheduleFieldState();
    }

    private void WireEvents()
    {
        btnSave.Click += async (_, _) => await SaveAsync();
        btnValidate.Click += async (_, _) => await ValidateAsync();
        btnAddBranch.Click += (_, _) => AddSelectedBranch();
        btnRemoveBranch.Click += (_, _) => RemoveSelectedBranch();
        btnAddEntity.Click += (_, _) => AddSelectedEntity();
        btnRemoveEntity.Click += (_, _) => RemoveSelectedEntity();
        branchesView.CellValueChanged += (_, _) => matrixView.RefreshData();
        entitiesView.CellValueChanged += (_, _) => matrixView.RefreshData();
        scheduleTypeEdit.SelectedValueChanged += (_, _) => ApplyScheduleFieldState();
        scheduleTypeEdit.TextChanged += (_, _) => ApplyScheduleFieldState();
    }

    private void BindCatalog()
    {
        companyEdit.Properties.DataSource = ViewModel.Catalog.MasterCompanies;
        companyEdit.Properties.DisplayMember = nameof(CompanyLookupItem.DisplayName);
        companyEdit.Properties.ValueMember = nameof(CompanyLookupItem.Id);

        branchLookup.Properties.DataSource = ViewModel.Catalog.BranchCompanies;
        branchLookup.Properties.DisplayMember = nameof(CompanyLookupItem.DisplayName);
        branchLookup.Properties.ValueMember = nameof(CompanyLookupItem.Id);

        entityLookup.Properties.DataSource = ViewModel.Catalog.Entities;
        entityLookup.Properties.DisplayMember = nameof(SyncEntityCatalogItem.Name);
        entityLookup.Properties.ValueMember = nameof(SyncEntityCatalogItem.Code);

        FillCombo(directionEdit, ViewModel.Catalog.Directions.Select(item => item.Code));
        FillCombo(executionModeEdit, ViewModel.Catalog.ExecutionModes.Select(item => item.Code));
        FillCombo(conflictStrategyEdit, ViewModel.Catalog.ConflictStrategies.Select(item => item.Code));
        FillCombo(scheduleTypeEdit, ViewModel.Catalog.ScheduleTypes.Select(item => item.Code));
    }

    private void BindState()
    {
        var state = ViewModel.State;
        codeEdit.Text = state.Code;
        nameEdit.Text = state.Name;
        descriptionEdit.Text = state.Description;
        companyEdit.EditValue = state.CompanyId;
        directionEdit.Text = state.Direction;
        executionModeEdit.Text = state.ExecutionMode;
        conflictStrategyEdit.Text = state.ConflictStrategy;
        batchSizeEdit.Value = state.BatchSize;
        maxRetriesEdit.Value = state.MaxRetries;
        retryDelayEdit.Value = state.RetryDelaySeconds;
        timeoutEdit.Value = state.TimeoutMinutes;
        activeEdit.Checked = state.IsActive;
        scheduleTypeEdit.Text = state.Schedule.ScheduleType;
        intervalEdit.Value = state.Schedule.IntervalMinutes ?? 0;
        executionTimeEdit.EditValue = state.Schedule.ExecutionTime.HasValue ? DateTime.Today.Add(state.Schedule.ExecutionTime.Value) : null;
        timeZoneEdit.Text = state.Schedule.TimeZoneId;
        preventConcurrentEdit.Checked = state.Schedule.PreventConcurrentExecutions;
        scheduleActiveEdit.Checked = state.Schedule.IsActive;
        branchesGrid.DataSource = state.Branches;
        entitiesGrid.DataSource = state.Entities;
        matrixGrid.DataSource = state.EntityBranches;
        ApplyScheduleFieldState();
    }

    private void PullState()
    {
        var state = ViewModel.State;
        state.Code = codeEdit.Text;
        state.Name = nameEdit.Text;
        state.Description = descriptionEdit.Text;
        state.CompanyId = companyEdit.EditValue is int companyId ? companyId : 0;
        state.Direction = directionEdit.Text;
        state.ExecutionMode = executionModeEdit.Text;
        state.ConflictStrategy = conflictStrategyEdit.Text;
        state.BatchSize = Convert.ToInt32(batchSizeEdit.Value);
        state.MaxRetries = Convert.ToInt32(maxRetriesEdit.Value);
        state.RetryDelaySeconds = Convert.ToInt32(retryDelayEdit.Value);
        state.TimeoutMinutes = Convert.ToInt32(timeoutEdit.Value);
        state.IsActive = activeEdit.Checked;
        state.Schedule.ScheduleType = scheduleTypeEdit.Text;
        state.Schedule.IntervalMinutes = string.Equals(state.Schedule.ScheduleType, "Interval", StringComparison.OrdinalIgnoreCase) && intervalEdit.Value > 0
            ? Convert.ToInt32(intervalEdit.Value)
            : null;
        state.Schedule.ExecutionTime = string.Equals(state.Schedule.ScheduleType, "Daily", StringComparison.OrdinalIgnoreCase) && executionTimeEdit.EditValue is DateTime time
            ? time.TimeOfDay
            : null;
        state.Schedule.TimeZoneId = string.IsNullOrWhiteSpace(timeZoneEdit.Text) ? "America/Guayaquil" : timeZoneEdit.Text.Trim();
        state.Schedule.PreventConcurrentExecutions = preventConcurrentEdit.Checked;
        state.Schedule.IsActive = scheduleActiveEdit.Checked;
        branchesView.CloseEditor();
        entitiesView.CloseEditor();
        matrixView.CloseEditor();
        branchesView.UpdateCurrentRow();
        entitiesView.UpdateCurrentRow();
        matrixView.UpdateCurrentRow();
    }

    private async Task SaveAsync()
    {
        PullState();
        if (!ValidateLocal())
        {
            return;
        }

        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            await ViewModel.SaveAsync();
            DialogResult = DialogResult.OK;
            Close();
        });
    }

    private async Task ValidateAsync()
    {
        PullState();
        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            var result = await ViewModel.ValidateAsync();
            var messages = result.Errors.Concat(result.Warnings).Select(message => $"{message.Code}: {message.Message}").ToArray();
            XtraMessageBox.Show(
                this,
                messages.Length == 0 ? "Validacion completada sin observaciones." : string.Join(Environment.NewLine, messages),
                result.IsValid ? "Validacion correcta" : "Validacion con observaciones",
                MessageBoxButtons.OK,
                result.IsValid ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        });
    }

    private bool ValidateLocal()
    {
        if (string.IsNullOrWhiteSpace(codeEdit.Text) || string.IsNullOrWhiteSpace(nameEdit.Text))
        {
            XtraMessageBox.Show(this, "Codigo y nombre son requeridos.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (ViewModel.State.CompanyId <= 0)
        {
            XtraMessageBox.Show(this, "Seleccione empresa maestra.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (ViewModel.State.Branches.Count == 0 || ViewModel.State.Entities.Count == 0)
        {
            XtraMessageBox.Show(this, "Configure al menos una sucursal y una entidad.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private void ApplyScheduleFieldState()
    {
        var scheduleType = scheduleTypeEdit.Text;
        var isInterval = string.Equals(scheduleType, "Interval", StringComparison.OrdinalIgnoreCase);
        var isDaily = string.Equals(scheduleType, "Daily", StringComparison.OrdinalIgnoreCase);

        intervalEdit.Enabled = isInterval;
        executionTimeEdit.Enabled = isDaily;
        if (!isInterval)
        {
            intervalEdit.Value = 0;
        }

        if (!isDaily)
        {
            executionTimeEdit.EditValue = null;
        }
    }

    private void AddSelectedBranch()
    {
        if (branchLookup.GetSelectedDataRow() is CompanyLookupItem branch)
        {
            ViewModel.State.AddBranch(branch);
            branchesView.RefreshData();
            matrixView.RefreshData();
        }
    }

    private void AddSelectedEntity()
    {
        if (entityLookup.GetSelectedDataRow() is SyncEntityCatalogItem entity)
        {
            ViewModel.State.AddEntityFromCatalog(entity);
            entitiesView.RefreshData();
            matrixView.RefreshData();
        }
    }

    private void RemoveSelectedBranch()
    {
        if (branchesView.GetFocusedRow() is not SyncProfileBranchEditorRow branch)
        {
            return;
        }

        ViewModel.State.Branches.Remove(branch);
        foreach (var matrix in ViewModel.State.EntityBranches.Where(row => row.BranchCompanyId == branch.BranchCompanyId).ToArray())
        {
            ViewModel.State.EntityBranches.Remove(matrix);
        }
    }

    private void RemoveSelectedEntity()
    {
        if (entitiesView.GetFocusedRow() is not SyncProfileEntityEditorRow entity)
        {
            return;
        }

        ViewModel.State.Entities.Remove(entity);
        foreach (var matrix in ViewModel.State.EntityBranches.Where(row => string.Equals(row.EntityCode, entity.EntityCode, StringComparison.OrdinalIgnoreCase)).ToArray())
        {
            ViewModel.State.EntityBranches.Remove(matrix);
        }
    }

    private static void FillCombo(ComboBoxEdit combo, IEnumerable<string> values)
    {
        combo.Properties.Items.Clear();
        combo.Properties.Items.AddRange(values.Where(value => !string.IsNullOrWhiteSpace(value)).Cast<object>().ToArray());
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
