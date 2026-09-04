using System.ComponentModel;
using System.Data;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.ViewModels.Sync;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions;
using NuanSystem.WinForms.Forms.Sync.EntityDefinitions;
using NuanSystem.WinForms.ViewModels.Sync.EntityDefinitions;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncProfileEditForm : BaseEditForm
{
    private static readonly Color NavigationSelectedBackColor = Color.FromArgb(235, 250, 246);
    private static readonly Color NavigationSelectedBorderColor = Color.FromArgb(129, 225, 207);
    private static readonly Color NavigationSelectedForeColor = Color.FromArgb(0, 160, 128);
    private static readonly Color NavigationNormalBackColor = Color.White;
    private static readonly Color NavigationNormalBorderColor = Color.White;
    private static readonly Color NavigationNormalForeColor = Color.FromArgb(23, 32, 51);

    private readonly List<BranchPreviewRow> branchPreviewRows = BranchPreviewRows().ToList();
    private readonly List<EntityPreviewRow> entityPreviewRows = EntityPreviewRows().ToList();
    private readonly List<ValidationResultRow> validationRows = new();
    private readonly List<ExecutionGridRow> executionRows = new();
    private readonly System.Windows.Forms.Timer executionsPollingTimer = new() { Interval = 7000 };
    private readonly Dictionary<string, DistributionPreviewState> distributionPreviewStates = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> distributionBranchColumns = new(StringComparer.OrdinalIgnoreCase);
    private RepositoryItemCheckEdit? distributionCheckEditor;
    private DataTable? distributionTable;
    private bool refreshingDistribution;
    private bool loadingSchedule;
    private bool scheduleDirty;
    private bool refreshingScheduleSummary;
    private bool hasValidationRun;
    private SyncProfileEditViewModel? viewModel;
    private ISyncConfigurationClient? executionClient;
    private ISyncEntityDefinitionClient? entityDefinitionClient;
    private ApiSession? session;
    private int? profileId;
    private bool canCreateBranchCompany;
    private bool canValidateProfile = true;
    private bool refreshingExecutions;
    private bool executionActionInProgress;
    private bool executionsLoaded;
    private int executionsPageNumber = 1;
    private int executionsPageSize = 25;
    private int executionsTotalCount;

    public SyncProfileEditForm()
    {
        InitializeComponent();
        ConfigureTabNavigation();
        ConfigureBranchesPage();
        ConfigureEntitiesPage();
        ConfigureDistributionPage();
        ConfigureSchedulePage();
        ConfigureValidationPage();
        ConfigureExecutionsPage();
        WireBranchActions();
        WireEntityActions();
        WireDistributionActions();
        WireValidationInvalidation();
    }

    public SyncProfileEditForm(
        SyncProfileEditViewModel viewModel,
        int? profileId,
        bool canCreateBranchCompany = false,
        bool canValidateProfile = true,
        ISyncConfigurationClient? executionClient = null,
        ApiSession? session = null,
        ISyncEntityDefinitionClient? entityDefinitionClient = null)
        : this()
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        this.viewModel = viewModel;
        this.profileId = profileId;
        this.executionClient = executionClient;
        this.entityDefinitionClient = entityDefinitionClient;
        this.session = session;
        this.canCreateBranchCompany = canCreateBranchCompany;
        this.canValidateProfile = canValidateProfile;
        btnValidateProfile.Enabled = canValidateProfile;
        Text = profileId.HasValue ? "Editar perfil de sincronizacion" : "Nuevo perfil de sincronizacion";
        LoadGeneralOptions();
        LoadGeneralFromState();
        ConfigureGeneralEditability();
        aceExecutions.Enabled = profileId.HasValue && CanViewExecutions;
        LoadBranchesFromState();
        RefreshBranchesGrid();
        LoadEntitiesFromState();
        RefreshEntitiesGrid();
        RefreshBusinessPartnerCodePolicyPanel();
        RefreshDistributionGrid();
        LoadScheduleOptions();
        LoadScheduleFromState();
        UpdateExecutionActionState();
    }

    public event Func<SyncProfileEditForm, Task<CompanyLookupItem?>>? CreateBranchCompanyRequested;

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (viewModel is null || !viewModel.RequiresBusinessPartnerSapCodePolicy || !CanManageBusinessPartnerSapCodePolicy)
        {
            return;
        }

        await RunWithUiExceptionHandlingAsync(async () =>
        {
            if (viewModel.BusinessPartnerSapCodePolicy is null)
            {
                await viewModel.LoadBusinessPartnerSapCodePolicyAsync();
            }

            RefreshBusinessPartnerCodePolicyPanel();
        });
    }

    protected override void BuildRequest()
    {
        base.BuildRequest();
        CopyGeneralToState();
        CopyScheduleToState();
    }

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "El codigo es obligatorio.");
        isValid &= Validator.RequireText(txtName, "El nombre es obligatorio.");
        isValid &= Validator.RequireCombo(cboDirection, "Seleccione la dirección del perfil.");
        isValid &= Validator.RequireCombo(cboExecutionMode, "Seleccione el modo de ejecucion.");

        if (viewModel?.State.CompanyId is null or <= 0)
        {
            Validator.SetError(txtMasterCompany, "Seleccione una empresa maestra valida.");
            isValid = false;
        }

        if (txtCode.Text.Trim().Length > 50)
        {
            Validator.SetError(txtCode, "El codigo no puede superar 50 caracteres.");
            isValid = false;
        }

        if (txtName.Text.Trim().Length > 150)
        {
            Validator.SetError(txtName, "El nombre no puede superar 150 caracteres.");
            isValid = false;
        }

        if (memDescription.Text.Trim().Length > 500)
        {
            Validator.SetError(memDescription, "La descripcion no puede superar 500 caracteres.");
            isValid = false;
        }

        isValid &= ValidateRange(spnBatchSize, 1, 10000, "El Batch debe estar entre 1 y 10000.");
        isValid &= ValidateRange(spnMaxRetries, 0, 10, "Los reintentos deben estar entre 0 y 10.");
        isValid &= ValidateRange(spnRetryDelaySeconds, 0, 3600, "La espera debe estar entre 0 y 3600 segundos.");
        isValid &= ValidateRange(spnTimeoutMinutes, 1, 1440, "El tiempo maximo debe estar entre 1 y 1440 minutos.");

        var generalIsValid = isValid;
        var scheduleIsValid = ValidateScheduleControls();
        isValid &= scheduleIsValid;

        if (!generalIsValid)
        {
            SelectNavigationElement(aceGeneral);
        }
        else if (!scheduleIsValid)
        {
            SelectNavigationElement(aceSchedule);
        }

        return isValid;
    }

    protected override async Task<bool> PersistAsync()
    {
        if (viewModel is null)
        {
            return false;
        }

        await SaveBusinessPartnerSapCodePolicyAsync();

        if (canValidateProfile)
        {
            var validation = await viewModel.ValidateAsync();
            ApplyValidationResult(validation);
            if (!validation.IsValid)
            {
                SelectNavigationElement(aceValidation);
                ShowWarning("Corrija los errores de configuracion antes de guardar el perfil.");
                return false;
            }

        }

        var saved = await viewModel.SaveAsync();
        profileId = saved.Id;
        scheduleDirty = false;
        return true;
    }

    protected override void ApplyReadOnlyMode()
    {
        base.ApplyReadOnlyMode();
        canCreateBranchCompany = false;
        grdDistribution.GridView.OptionsBehavior.Editable = false;
        btnValidateProfile.Enabled = false;
        UpdateScheduleEditorState();
        UpdateDistributionActionState();
        UpdateExecutionActionState();
    }

    private void LoadGeneralOptions()
    {
        if (viewModel is null)
        {
            return;
        }

        cboExecutionMode.Properties.Items.Clear();
        cboExecutionMode.Properties.Items.AddRange(viewModel.Catalog.ExecutionModes
            .Select(item => item.Code)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Cast<object>()
            .ToArray());

        cboDirection.Properties.Items.Clear();
        cboDirection.Properties.Items.AddRange(SyncProfileDirectionPolicy.Build(viewModel.Catalog.Directions)
            .Cast<object>()
            .ToArray());

        cboSapPrefixMode.Properties.Items.Clear();
        cboSapPrefixMode.Properties.Items.AddRange(new object[] { "NationalForeign", "RoleOnly" });
        txtPassportIdentificationTypeCode.Properties.MaxLength = 30;
    }

    private void LoadGeneralFromState()
    {
        if (viewModel is null)
        {
            return;
        }

        var state = viewModel.State;
        var masterCompany = viewModel.Catalog.MasterCompanies.FirstOrDefault(company => company.Id == state.CompanyId);
        if (masterCompany is null)
        {
            masterCompany = viewModel.Catalog.MasterCompanies.FirstOrDefault(company => company.IsActive);
            if (masterCompany is not null)
            {
                state.CompanyId = masterCompany.Id;
            }
        }

        txtMasterCompany.EditValue = masterCompany?.Name ?? string.Empty;
        txtCode.EditValue = state.Code;
        txtName.EditValue = state.Name;
        memDescription.EditValue = state.Description ?? string.Empty;
        cboDirection.EditValue = cboDirection.Properties.Items
            .OfType<SyncProfileDirectionOption>()
            .FirstOrDefault(option => string.Equals(option.Code, state.Direction, StringComparison.OrdinalIgnoreCase));
        cboExecutionMode.EditValue = state.ExecutionMode;
        txtConflictStrategy.EditValue = state.ConflictStrategy;
        spnBatchSize.EditValue = state.BatchSize;
        spnMaxRetries.EditValue = state.MaxRetries;
        spnRetryDelaySeconds.EditValue = state.RetryDelaySeconds;
        spnTimeoutMinutes.EditValue = state.TimeoutMinutes;
        swIsActive.IsOn = state.IsActive;
    }

    private void RefreshBusinessPartnerCodePolicyPanel()
    {
        if (viewModel is null)
        {
            pnlBusinessPartnerCodePolicy.Visible = false;
            return;
        }

        pnlBusinessPartnerCodePolicy.Visible =
            viewModel.RequiresBusinessPartnerSapCodePolicy && CanManageBusinessPartnerSapCodePolicy;
        if (!pnlBusinessPartnerCodePolicy.Visible || viewModel.BusinessPartnerSapCodePolicy is not { } policy)
        {
            return;
        }

        swSapCodePolicyEnabled.IsOn = policy.IsEnabled;
        cboSapPrefixMode.EditValue = policy.PrefixMode;
        txtPassportIdentificationTypeCode.Text = policy.PassportIdentificationTypeCode;
        lblCustomerNationalExample.Text = $"Cliente nacional: {policy.CustomerNationalExample}";
        lblCustomerForeignExample.Text = $"Cliente extranjero: {policy.CustomerForeignExample}";
        lblSupplierNationalExample.Text = $"Proveedor nacional: {policy.SupplierNationalExample}";
        lblSupplierForeignExample.Text = $"Proveedor extranjero: {policy.SupplierForeignExample}";
    }

    private async Task RefreshBusinessPartnerCodePolicyAsync()
    {
        if (viewModel is null || !viewModel.RequiresBusinessPartnerSapCodePolicy || !CanManageBusinessPartnerSapCodePolicy)
        {
            RefreshBusinessPartnerCodePolicyPanel();
            return;
        }

        await viewModel.LoadBusinessPartnerSapCodePolicyAsync();
        RefreshBusinessPartnerCodePolicyPanel();
    }

    private async Task SaveBusinessPartnerSapCodePolicyAsync()
    {
        if (viewModel is null || !viewModel.RequiresBusinessPartnerSapCodePolicy || !CanManageBusinessPartnerSapCodePolicy)
        {
            return;
        }

        await viewModel.SaveBusinessPartnerSapCodePolicyAsync(
            new SaveBusinessPartnerSapCodePolicyRequest(
                swSapCodePolicyEnabled.IsOn,
                Convert.ToString(cboSapPrefixMode.EditValue)?.Trim() ?? string.Empty,
                txtPassportIdentificationTypeCode.Text.Trim(),
                string.IsNullOrWhiteSpace(viewModel.BusinessPartnerSapCodePolicy?.RowVersion)
                    ? null
                    : viewModel.BusinessPartnerSapCodePolicy.RowVersion));

        RefreshBusinessPartnerCodePolicyPanel();
    }

    private void ConfigureGeneralEditability()
    {
        if (viewModel is null)
        {
            return;
        }

        txtMasterCompany.Properties.ReadOnly = true;
        txtCode.Properties.ReadOnly = viewModel.State.Id > 0;
        cboDirection.Properties.ReadOnly = IsReadOnlyMode;
        txtConflictStrategy.Properties.ReadOnly = true;
    }

    private void CopyGeneralToState()
    {
        if (viewModel is null)
        {
            return;
        }

        var state = viewModel.State;
        state.Code = txtCode.Text.Trim();
        state.Name = txtName.Text.Trim();
        state.Description = string.IsNullOrWhiteSpace(memDescription.Text) ? null : memDescription.Text.Trim();
        state.Direction = cboDirection.SelectedItem is SyncProfileDirectionOption direction
            ? direction.Code
            : string.Empty;
        state.ExecutionMode = Convert.ToString(cboExecutionMode.EditValue)?.Trim() ?? string.Empty;
        state.ConflictStrategy = txtConflictStrategy.Text.Trim();
        state.BatchSize = Convert.ToInt32(spnBatchSize.Value);
        state.MaxRetries = Convert.ToInt32(spnMaxRetries.Value);
        state.RetryDelaySeconds = Convert.ToInt32(spnRetryDelaySeconds.Value);
        state.TimeoutMinutes = Convert.ToInt32(spnTimeoutMinutes.Value);
        state.IsActive = swIsActive.IsOn;
    }

    private bool ValidateRange(SpinEdit control, int minimum, int maximum, string message)
    {
        var value = Convert.ToInt32(control.Value);
        if (value >= minimum && value <= maximum)
        {
            return true;
        }

        Validator.SetError(control, message);
        return false;
    }

    private void ConfigureTabNavigation()
    {
        foreach (var element in NavigationElements())
        {
            element.ImageOptions.SvgImageSize = new Size(18, 18);
        }

        accordionNavigation.ElementClick += AccordionNavigation_ElementClick;
        SelectNavigationElement(aceGeneral);
    }

    private void AccordionNavigation_ElementClick(object? sender, ElementClickEventArgs e)
    {
        SelectNavigationElement(e.Element);
    }

    private void SelectNavigationElement(AccordionControlElement selectedElement)
    {
        accordionNavigation.SelectedElement = selectedElement;

        navigationFrame.SelectedPage = selectedElement switch
        {
            var element when ReferenceEquals(element, aceGeneral) => pageGeneral,
            var element when ReferenceEquals(element, aceBranches) => pageBranches,
            var element when ReferenceEquals(element, aceEntities) => pageEntities,
            var element when ReferenceEquals(element, aceDistribution) => pageDistribution,
            var element when ReferenceEquals(element, aceSchedule) => pageSchedule,
            var element when ReferenceEquals(element, aceValidation) => pageValidation,
            var element when ReferenceEquals(element, aceExecutions) => pageExecutions,
            _ => navigationFrame.SelectedPage
        };

        ApplyNavigationSelection(selectedElement);

        if (ReferenceEquals(selectedElement, aceExecutions))
        {
            _ = RefreshExecutionsAsync();
        }
        else if (ReferenceEquals(selectedElement, aceSchedule))
        {
            _ = RefreshScheduleSummaryAsync();
            executionsPollingTimer.Stop();
        }
        else
        {
            executionsPollingTimer.Stop();
        }
    }

    private void ApplyNavigationSelection(AccordionControlElement selectedElement)
    {
        foreach (var element in NavigationElements())
        {
            ApplyNavigationElementStyle(element, ReferenceEquals(element, selectedElement));
        }
    }

    private static void ApplyNavigationElementStyle(AccordionControlElement element, bool selected)
    {
        element.Appearance.Normal.BackColor = selected ? NavigationSelectedBackColor : NavigationNormalBackColor;
        element.Appearance.Normal.BorderColor = selected ? NavigationSelectedBorderColor : NavigationNormalBorderColor;
        element.Appearance.Normal.ForeColor = selected ? NavigationSelectedForeColor : NavigationNormalForeColor;
        element.Appearance.Normal.Font = AppTypography.ButtonFont;
        element.Appearance.Normal.Options.UseBackColor = true;
        element.Appearance.Normal.Options.UseBorderColor = true;
        element.Appearance.Normal.Options.UseForeColor = true;
        element.Appearance.Normal.Options.UseFont = true;
        element.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon(
            "diskette_16.svg",
            selected ? NavigationSelectedForeColor : NavigationNormalForeColor);
        element.ImageOptions.SvgImageSize = new Size(18, 18);
    }

    private IEnumerable<AccordionControlElement> NavigationElements()
    {
        yield return aceGeneral;
        yield return aceBranches;
        yield return aceEntities;
        yield return aceDistribution;
        yield return aceSchedule;
        yield return aceValidation;
        yield return aceExecutions;
    }

    private void ConfigureBranchesPage()
    {
        grdBranches.SetStatusBadgeProvider(value =>
            string.Equals(Convert.ToString(value), "Activo", StringComparison.OrdinalIgnoreCase)
                ? NuanGridBadgeStyle.Success
                : NuanGridBadgeStyle.Neutral);

        grdBranches.ConfigureColumns(
            new NuanGridColumnDefinition
            {
                FieldName = "BranchCompanyCode",
                Caption = "Código empresa",
                VisibleIndex = 0,
                Width = 115
            },
            new NuanGridColumnDefinition
            {
                FieldName = "BranchCode",
                Caption = "Código sucursal",
                VisibleIndex = 1,
                Width = 110
            },
            new NuanGridColumnDefinition
            {
                FieldName = "BranchCompanyName",
                Caption = "Nombre sucursal",
                VisibleIndex = 2,
                Width = 170
            },
            new NuanGridColumnDefinition
            {
                FieldName = "BranchDatabaseName",
                Caption = "Base de datos",
                VisibleIndex = 3,
                Width = 140
            },
            new NuanGridColumnDefinition
            {
                FieldName = "StatusText",
                Caption = "Activo en perfil",
                VisibleIndex = 4,
                Width = 115,
                Format = NuanGridColumnFormat.StatusBadge
            },
            new NuanGridColumnDefinition
            {
                FieldName = "BatchSize",
                Caption = "Batch",
                VisibleIndex = 5,
                Width = 70,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = "MaxRetries",
                Caption = "Reintentos",
                VisibleIndex = 6,
                Width = 86,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = "LastSynchronizationAt",
                Caption = "Última sincronización",
                VisibleIndex = 7,
                Width = 150,
                Format = NuanGridColumnFormat.DateTime
            });

        RefreshBranchesGrid();
    }

    private void ConfigureEntitiesPage()
    {
        grdEntities.SetStatusBadgeProvider(value =>
            string.Equals(Convert.ToString(value), "Activo", StringComparison.OrdinalIgnoreCase)
                ? NuanGridBadgeStyle.Success
                : NuanGridBadgeStyle.Neutral);

        grdEntities.ConfigureColumns(
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.EntityCode),
                Caption = "Código",
                VisibleIndex = 0,
                Width = 150
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.EntityName),
                Caption = "Entidad",
                VisibleIndex = 1,
                Width = 160
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.ExecutionOrder),
                Caption = "Orden",
                VisibleIndex = 2,
                Width = 70,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.SyncMode),
                Caption = "Modo",
                VisibleIndex = 3,
                Width = 95
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.KeyField),
                Caption = "KeyField",
                VisibleIndex = 4,
                Width = 110
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.ModifiedAtField),
                Caption = "ModifiedAtField",
                VisibleIndex = 5,
                Width = 130
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.VersionField),
                Caption = "VersionField",
                VisibleIndex = 6,
                Width = 110
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.ActiveField),
                Caption = "ActiveField",
                VisibleIndex = 7,
                Width = 105
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.AllowInsert),
                Caption = "Insertar",
                VisibleIndex = 8,
                Width = 78,
                Format = NuanGridColumnFormat.Boolean
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.AllowUpdate),
                Caption = "Actualizar",
                VisibleIndex = 9,
                Width = 84,
                Format = NuanGridColumnFormat.Boolean
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.AllowDeactivate),
                Caption = "Desactivar",
                VisibleIndex = 10,
                Width = 88,
                Format = NuanGridColumnFormat.Boolean
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.ContinueOnError),
                Caption = "Continuar en error",
                VisibleIndex = 11,
                Width = 125,
                Format = NuanGridColumnFormat.Boolean
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.BatchSize),
                Caption = "Batch",
                VisibleIndex = 12,
                Width = 74,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(EntityPreviewRow.StatusText),
                Caption = "Estado",
                VisibleIndex = 13,
                Width = 90,
                Format = NuanGridColumnFormat.StatusBadge
            });

        RefreshEntitiesGrid();
    }

    private void ConfigureDistributionPage()
    {
        var view = grdDistribution.GridView;
        distributionCheckEditor = new RepositoryItemCheckEdit
        {
            AllowGrayed = false,
            AutoHeight = false
        };
        distributionCheckEditor.EditValueChanged += (_, _) => view.PostEditor();
        grdDistribution.GridControl.RepositoryItems.Add(distributionCheckEditor);

        view.OptionsBehavior.Editable = true;
        view.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDown;
        view.OptionsSelection.EnableAppearanceFocusedCell = true;
        view.OptionsView.ColumnAutoWidth = false;
        view.OptionsView.ShowGroupPanel = false;
        view.OptionsView.ShowIndicator = true;
        view.IndicatorWidth = 28;
        view.Appearance.FocusedCell.BackColor = Color.FromArgb(235, 250, 246);
        view.Appearance.FocusedCell.Options.UseBackColor = true;
        AppTypography.ApplyGrid(view);

        view.CellValueChanged += DistributionView_CellValueChanged;
        view.CustomDrawCell += DistributionView_CustomDrawCell;
        view.FocusedColumnChanged += (_, _) => UpdateDistributionActionState();
        view.FocusedRowChanged += (_, _) => UpdateDistributionActionState();

        RefreshDistributionGrid();
    }

    private void ConfigureSchedulePage()
    {
        cboScheduleType.SelectedIndexChanged += (_, _) => ScheduleEditorValueChanged();
        spnScheduleIntervalMinutes.EditValueChanged += (_, _) => ScheduleEditorValueChanged();
        timScheduleExecutionTime.EditValueChanged += (_, _) => ScheduleEditorValueChanged();
        cboScheduleTimeZone.EditValueChanged += (_, _) => ScheduleEditorValueChanged();
        swPreventConcurrentExecutions.EditValueChanged += (_, _) => ScheduleEditorValueChanged();
        swScheduleIsActive.EditValueChanged += (_, _) => ScheduleEditorValueChanged();
        UpdateScheduleEditorState();
    }

    private void LoadScheduleOptions()
    {
        if (viewModel is null || viewModel.Catalog.ScheduleTypes.Count == 0)
        {
            return;
        }

        loadingSchedule = true;
        try
        {
            cboScheduleType.Properties.Items.Clear();
            cboScheduleType.Properties.Items.AddRange(
                viewModel.Catalog.ScheduleTypes.Select(item => item.Name).Cast<object>().ToArray());

            var defaultTimeZone = string.IsNullOrWhiteSpace(viewModel.Catalog.DefaultTimeZoneId)
                ? "America/Guayaquil"
                : viewModel.Catalog.DefaultTimeZoneId;
            if (!cboScheduleTimeZone.Properties.Items.Contains(defaultTimeZone))
            {
                cboScheduleTimeZone.Properties.Items.Add(defaultTimeZone);
            }
        }
        finally
        {
            loadingSchedule = false;
        }
    }

    private void LoadScheduleFromState()
    {
        if (viewModel is null)
        {
            return;
        }

        loadingSchedule = true;
        try
        {
            var schedule = viewModel.State.Schedule;
            cboScheduleType.EditValue = ScheduleTypeDisplayName(schedule.ScheduleType);
            spnScheduleIntervalMinutes.EditValue = schedule.IntervalMinutes ?? 60;
            timScheduleExecutionTime.Time = DateTime.Today.Add(schedule.ExecutionTime ?? TimeSpan.FromHours(23));
            cboScheduleTimeZone.EditValue = string.IsNullOrWhiteSpace(schedule.TimeZoneId)
                ? "America/Guayaquil"
                : schedule.TimeZoneId;
            swPreventConcurrentExecutions.IsOn = schedule.PreventConcurrentExecutions;
            swScheduleIsActive.IsOn = schedule.IsActive;
            scheduleDirty = false;
        }
        finally
        {
            loadingSchedule = false;
        }

        UpdateScheduleEditorState();
    }

    private void ScheduleEditorValueChanged()
    {
        if (loadingSchedule || IsReadOnlyMode)
        {
            return;
        }

        CopyScheduleToState();
        scheduleDirty = true;
        InvalidateValidationResult();
        UpdateScheduleEditorState();
    }

    private void CopyScheduleToState()
    {
        if (viewModel is null)
        {
            return;
        }

        var schedule = viewModel.State.Schedule;
        schedule.Configure(
            SelectedScheduleTypeCode(),
            Convert.ToInt32(spnScheduleIntervalMinutes.Value),
            timScheduleExecutionTime.Time.TimeOfDay,
            Convert.ToString(cboScheduleTimeZone.EditValue),
            swPreventConcurrentExecutions.IsOn,
            swScheduleIsActive.IsOn);
    }

    private void UpdateScheduleEditorState()
    {
        var scheduleType = SelectedScheduleTypeCode();
        var isManual = string.Equals(scheduleType, "Manual", StringComparison.OrdinalIgnoreCase);
        var isInterval = string.Equals(scheduleType, "Interval", StringComparison.OrdinalIgnoreCase);
        var isDaily = string.Equals(scheduleType, "Daily", StringComparison.OrdinalIgnoreCase);

        var canEditSchedule = !IsReadOnlyMode;
        spnScheduleIntervalMinutes.Enabled = canEditSchedule && isInterval;
        lblScheduleIntervalUnit.Enabled = canEditSchedule && isInterval;
        timScheduleExecutionTime.Enabled = canEditSchedule && isDaily;
        cboScheduleTimeZone.Enabled = canEditSchedule && !isManual;

        lblScheduleEffectiveFrequencyValue.Text = viewModel?.State.Schedule.EffectiveFrequencyText()
                                                  ?? "Ejecución manual";

        var isActive = swScheduleIsActive.IsOn;
        lblScheduleStatusValue.Text = !isActive
            ? "Inactiva"
            : isManual
                ? "Manual"
                : "Programada";
        lblScheduleStatusValue.Appearance.BackColor = isActive
            ? BrandResources.SuccessBack
            : BrandResources.Surface;
        lblScheduleStatusValue.Appearance.ForeColor = isActive
            ? BrandResources.SuccessText
            : BrandResources.MutedText;
        lblScheduleNextExecutionValue.Text = !isActive || isManual
            ? "No aplica"
            : profileId is null
                ? "Se calculará al guardar"
                : scheduleDirty
                    ? "Se recalculará al guardar"
                    : FormatScheduleDate(viewModel?.ProfileSummary?.NextExecutionAt);
        lblScheduleLastExecutionValue.Text = FormatScheduleDate(
            viewModel?.LastSuccessfulScheduledExecutionAt?.UtcDateTime);

        lblScheduleInfo.Text = swPreventConcurrentExecutions.IsOn
            ? "Cuando una ejecución anterior continúe en proceso, no se iniciará una nueva ejecución para este perfil."
            : "La programación permite iniciar una nueva ejecución aunque exista otra ejecución del perfil en proceso.";
    }

    private bool ValidateScheduleControls()
    {
        var scheduleType = SelectedScheduleTypeCode();
        var isValid = scheduleType is "Manual" or "Interval" or "Daily";
        if (!isValid)
        {
            Validator.SetError(cboScheduleType, "Seleccione un tipo de programación válido.");
        }

        if (string.Equals(scheduleType, "Interval", StringComparison.OrdinalIgnoreCase))
        {
            var interval = Convert.ToInt32(spnScheduleIntervalMinutes.Value);
            if (interval is < 1 or > 1440)
            {
                Validator.SetError(spnScheduleIntervalMinutes, "El intervalo debe estar entre 1 y 1440 minutos.");
                isValid = false;
            }
        }

        var timeZoneId = Convert.ToString(cboScheduleTimeZone.EditValue)?.Trim();
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            Validator.SetError(cboScheduleTimeZone, "La zona horaria es obligatoria.");
            return false;
        }

        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            Validator.SetError(cboScheduleTimeZone, "La zona horaria no es válida para .NET.");
            isValid = false;
        }
        catch (InvalidTimeZoneException)
        {
            Validator.SetError(cboScheduleTimeZone, "La zona horaria no es válida para .NET.");
            isValid = false;
        }

        return isValid;
    }

    private async Task RefreshScheduleSummaryAsync()
    {
        if (refreshingScheduleSummary || viewModel is null || viewModel.State.Id <= 0)
        {
            UpdateScheduleEditorState();
            return;
        }

        refreshingScheduleSummary = true;
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                await viewModel.RefreshProfileSummaryAsync(CanViewExecutions);
                if (!IsDisposed && !Disposing)
                {
                    UpdateScheduleEditorState();
                }
            });
        }
        finally
        {
            refreshingScheduleSummary = false;
        }
    }

    private string FormatScheduleDate(DateTime? utcDateTime)
    {
        if (!utcDateTime.HasValue)
        {
            return "-";
        }

        var utc = DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc);
        var timeZoneId = viewModel?.State.Schedule.TimeZoneId;
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                string.IsNullOrWhiteSpace(timeZoneId) ? "America/Guayaquil" : timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone).ToString("dd/MM/yyyy HH:mm");
        }
        catch (TimeZoneNotFoundException)
        {
            return utc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        }
        catch (InvalidTimeZoneException)
        {
            return utc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        }
    }

    private string SelectedScheduleTypeCode()
    {
        var selected = Convert.ToString(cboScheduleType.EditValue)?.Trim();
        var catalogItem = viewModel?.Catalog.ScheduleTypes.FirstOrDefault(item =>
            string.Equals(item.Name, selected, StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.Code, selected, StringComparison.OrdinalIgnoreCase));
        if (catalogItem is not null)
        {
            return catalogItem.Code;
        }

        return selected switch
        {
            "Intervalo" => "Interval",
            "Diaria" => "Daily",
            _ => "Manual"
        };
    }

    private string ScheduleTypeDisplayName(string scheduleType)
    {
        var catalogItem = viewModel?.Catalog.ScheduleTypes.FirstOrDefault(item =>
            string.Equals(item.Code, scheduleType, StringComparison.OrdinalIgnoreCase));
        if (catalogItem is not null)
        {
            return catalogItem.Name;
        }

        return scheduleType switch
        {
            "Interval" => "Intervalo",
            "Daily" => "Diaria",
            _ => "Manual"
        };
    }

    private void ConfigureValidationPage()
    {
        grdValidationResults.SetStatusBadgeProvider(value =>
            string.Equals(Convert.ToString(value), "Error", StringComparison.OrdinalIgnoreCase)
                ? NuanGridBadgeStyle.Error
                : NuanGridBadgeStyle.Warning);
        grdValidationResults.ConfigureColumns(
            new NuanGridColumnDefinition
            {
                FieldName = nameof(ValidationResultRow.Type),
                Caption = "Tipo",
                VisibleIndex = 0,
                Width = 100,
                Format = NuanGridColumnFormat.StatusBadge
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(ValidationResultRow.Code),
                Caption = "Código",
                VisibleIndex = 1,
                Width = 220
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(ValidationResultRow.FieldOrSection),
                Caption = "Campo o sección",
                VisibleIndex = 2,
                Width = 150
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(ValidationResultRow.Message),
                Caption = "Mensaje",
                VisibleIndex = 3,
                Width = 300
            });

        var view = grdValidationResults.GridView;
        view.OptionsBehavior.Editable = false;
        view.OptionsSelection.EnableAppearanceFocusedCell = false;
        view.OptionsView.ColumnAutoWidth = false;
        view.OptionsView.ShowGroupPanel = false;
        view.RowHeight = 30;
        view.Appearance.FocusedRow.BackColor = NavigationSelectedBackColor;
        view.Appearance.FocusedRow.Options.UseBackColor = true;
        view.CustomDrawEmptyForeground += ValidationView_CustomDrawEmptyForeground;
        grdValidationResults.RowDoubleClick += (_, _) => OpenSelectedValidationIssue();
        grdValidationResults.KeyDown += ValidationResults_KeyDown;
        btnValidateProfile.Click += BtnValidateProfile_Click;
        grdValidationResults.SetData(validationRows);
        SetValidationPendingState("Pendiente de validar");
    }

    private void WireValidationInvalidation()
    {
        BaseEdit[] generalEditors =
        [
            txtCode,
            txtName,
            memDescription,
            cboDirection,
            cboExecutionMode,
            spnBatchSize,
            spnMaxRetries,
            spnRetryDelaySeconds,
            spnTimeoutMinutes,
            swIsActive
        ];

        BaseEdit[] policyEditors =
        [
            swSapCodePolicyEnabled,
            cboSapPrefixMode,
            txtPassportIdentificationTypeCode
        ];

        foreach (var editor in generalEditors)
        {
            editor.EditValueChanged += (_, _) => InvalidateValidationResult();
        }

        foreach (var editor in policyEditors)
        {
            editor.EditValueChanged += (_, _) => InvalidateValidationResult();
        }
    }

    private async void BtnValidateProfile_Click(object? sender, EventArgs e)
    {
        if (viewModel is null || IsReadOnlyMode || !canValidateProfile)
        {
            return;
        }

        Validator.Clear();
        if (!ValidateForm())
        {
            Validator.FocusFirstInvalid();
            ShowWarning("Revise los campos resaltados antes de validar el perfil.");
            return;
        }

        BuildRequest();
        btnValidateProfile.Enabled = false;
        btnValidateProfile.ButtonText = "Validando";
        try
        {
            await SaveBusinessPartnerSapCodePolicyAsync();
            var result = await viewModel.ValidateAsync();
            ApplyValidationResult(result);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
        finally
        {
            btnValidateProfile.ButtonText = "Validar";
            btnValidateProfile.Enabled = !IsReadOnlyMode && canValidateProfile;
        }
    }

    private void ApplyValidationResult(SyncProfileValidationResult result)
    {
        hasValidationRun = true;
        validationRows.Clear();
        validationRows.AddRange(result.Errors.Select(message => ValidationResultRow.From("Error", message)));
        validationRows.AddRange(result.Warnings.Select(message => ValidationResultRow.From("Advertencia", message)));
        grdValidationResults.SetData(validationRows);

        var hasWarnings = result.Warnings.Count > 0;
        lblValidationResultValue.Text = !result.IsValid
            ? "Configuración inválida"
            : hasWarnings
                ? "Válida con advertencias"
                : "Configuración válida";
        lblValidationResultValue.Appearance.BackColor = !result.IsValid
            ? BrandResources.ErrorBack
            : hasWarnings
                ? BrandResources.WarningBack
                : BrandResources.SuccessBack;
        lblValidationResultValue.Appearance.ForeColor = !result.IsValid
            ? BrandResources.ErrorText
            : hasWarnings
                ? BrandResources.WarningText
                : BrandResources.SuccessText;

        SetValidationCounter(
            lblValidationErrorsValue,
            result.Errors.Count,
            result.Errors.Count == 0 ? BrandResources.SuccessBack : BrandResources.ErrorBack,
            result.Errors.Count == 0 ? BrandResources.SuccessText : BrandResources.ErrorText);
        SetValidationCounter(
            lblValidationWarningsValue,
            result.Warnings.Count,
            result.Warnings.Count == 0 ? BrandResources.SuccessBack : BrandResources.WarningBack,
            result.Warnings.Count == 0 ? BrandResources.SuccessText : BrandResources.WarningText);
        grdValidationResults.GridView.Invalidate();
    }

    private void InvalidateValidationResult()
    {
        if (!hasValidationRun)
        {
            return;
        }

        SetValidationPendingState("Requiere revalidación");
    }

    private void SetValidationPendingState(string statusText)
    {
        hasValidationRun = false;
        validationRows.Clear();
        grdValidationResults.SetData(validationRows);
        lblValidationResultValue.Text = statusText;
        lblValidationResultValue.Appearance.BackColor = BrandResources.Surface;
        lblValidationResultValue.Appearance.ForeColor = BrandResources.MutedText;
        SetValidationCounter(
            lblValidationErrorsValue,
            0,
            BrandResources.Surface,
            BrandResources.MutedText);
        SetValidationCounter(
            lblValidationWarningsValue,
            0,
            BrandResources.Surface,
            BrandResources.MutedText);
        grdValidationResults.GridView.Invalidate();
    }

    private void ValidationResults_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        OpenSelectedValidationIssue();
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private void OpenSelectedValidationIssue()
    {
        if (grdValidationResults.GetFocusedRow<ValidationResultRow>() is not { } selected)
        {
            return;
        }

        SelectNavigationElement(selected.Section switch
        {
            SyncProfileEditorSection.Branches => aceBranches,
            SyncProfileEditorSection.Entities => aceEntities,
            SyncProfileEditorSection.Distribution => aceDistribution,
            SyncProfileEditorSection.Schedule => aceSchedule,
            _ => aceGeneral
        });
        FocusValidationTarget(selected);
    }

    private void FocusValidationTarget(ValidationResultRow issue)
    {
        Control target = issue.Section switch
        {
            SyncProfileEditorSection.Branches => grdBranches,
            SyncProfileEditorSection.Entities => grdEntities,
            SyncProfileEditorSection.Distribution => grdDistribution,
            SyncProfileEditorSection.Schedule => ResolveScheduleValidationControl(issue.Field),
            _ => ResolveGeneralValidationControl(issue.Field)
        };

        target.Focus();
    }

    private Control ResolveGeneralValidationControl(string? field)
    {
        return field switch
        {
            "Code" => txtCode,
            "Name" => txtName,
            "CompanyId" => txtMasterCompany,
            "Direction" => cboDirection,
            "ExecutionMode" => cboExecutionMode,
            "ConflictStrategy" => txtConflictStrategy,
            "BatchSize" => spnBatchSize,
            "MaxRetries" => spnMaxRetries,
            "RetryDelaySeconds" => spnRetryDelaySeconds,
            "TimeoutMinutes" => spnTimeoutMinutes,
            "IsActive" => swIsActive,
            _ => txtCode
        };
    }

    private Control ResolveScheduleValidationControl(string? field)
    {
        return field switch
        {
            "IntervalMinutes" => spnScheduleIntervalMinutes,
            "ExecutionTime" => timScheduleExecutionTime,
            "TimeZoneId" => cboScheduleTimeZone,
            "PreventConcurrentExecutions" => swPreventConcurrentExecutions,
            "IsActive" => swScheduleIsActive,
            _ => cboScheduleType
        };
    }

    private static void SetValidationCounter(LabelControl label, int value, Color backColor, Color foreColor)
    {
        label.Text = value.ToString();
        label.Appearance.BackColor = backColor;
        label.Appearance.ForeColor = foreColor;
    }

    private void ValidationView_CustomDrawEmptyForeground(object? sender, CustomDrawEventArgs e)
    {
        var text = hasValidationRun
            ? "No se encontraron errores ni advertencias."
            : "Ejecute la validación para revisar la configuración del perfil.";
        using var brush = new SolidBrush(BrandResources.MutedText);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        e.Cache.Graphics.DrawString(text, AppTypography.BaseReadableFont, brush, e.Bounds, format);
        e.Handled = true;
    }

    private void ConfigureExecutionsPage()
    {
        grdExecutions.SetStatusBadgeProvider(value => Convert.ToString(value) switch
        {
            "Pendiente" => NuanGridBadgeStyle.Info,
            "En proceso" => NuanGridBadgeStyle.Info,
            "Completada" => NuanGridBadgeStyle.Success,
            "Completada con errores" => NuanGridBadgeStyle.Warning,
            "Cancelando" => NuanGridBadgeStyle.Warning,
            "Cancelada" => NuanGridBadgeStyle.Neutral,
            "Fallida" => NuanGridBadgeStyle.Error,
            _ => NuanGridBadgeStyle.Neutral
        });
        grdExecutions.ConfigureColumns(
            ExecutionColumn(nameof(ExecutionGridRow.Id), "Id", 0, 55, NuanGridColumnFormat.Number),
            ExecutionColumn(nameof(ExecutionGridRow.ExecutionTypeText), "Tipo", 1, 90),
            ExecutionColumn(nameof(ExecutionGridRow.StatusText), "Estado", 2, 145, NuanGridColumnFormat.StatusBadge),
            ExecutionColumn(nameof(ExecutionGridRow.RequestedBy), "Solicitado por", 3, 105),
            ExecutionColumn(nameof(ExecutionGridRow.RequestedAt), "Solicitado", 4, 135, NuanGridColumnFormat.DateTime),
            ExecutionColumn(nameof(ExecutionGridRow.StartedAt), "Inicio", 5, 135, NuanGridColumnFormat.DateTime),
            ExecutionColumn(nameof(ExecutionGridRow.FinishedAt), "Fin", 6, 135, NuanGridColumnFormat.DateTime),
            ExecutionColumn(nameof(ExecutionGridRow.TotalEntities), "Entidades", 7, 75, NuanGridColumnFormat.Number),
            ExecutionColumn(nameof(ExecutionGridRow.TotalRecordsRead), "Leidos", 8, 75, NuanGridColumnFormat.Number),
            ExecutionColumn(nameof(ExecutionGridRow.TotalEventsPublished), "Publicados", 9, 85, NuanGridColumnFormat.Number),
            ExecutionColumn(nameof(ExecutionGridRow.TotalSkipped), "Omitidos", 10, 75, NuanGridColumnFormat.Number),
            ExecutionColumn(nameof(ExecutionGridRow.TotalErrors), "Errores", 11, 65, NuanGridColumnFormat.Number),
            ExecutionColumn(nameof(ExecutionGridRow.Message), "Mensaje", 12, 250));

        var view = grdExecutions.GridView;
        view.OptionsBehavior.Editable = false;
        view.OptionsSelection.EnableAppearanceFocusedCell = false;
        view.OptionsView.ColumnAutoWidth = false;
        view.OptionsView.ShowGroupPanel = false;
        view.RowHeight = 30;
        view.Appearance.FocusedRow.BackColor = NavigationSelectedBackColor;
        view.Appearance.FocusedRow.Options.UseBackColor = true;
        view.CustomDrawEmptyForeground += ExecutionsView_CustomDrawEmptyForeground;

        grdExecutions.FocusedRowChanged += (_, _) => UpdateExecutionActionState();
        grdExecutions.RowDoubleClick += async (_, _) => await OpenSelectedExecutionDetailAsync();
        grdExecutions.KeyDown += ExecutionsGrid_KeyDown;
        btnViewExecutionDetail.Click += async (_, _) => await OpenSelectedExecutionDetailAsync();
        btnCancelExecution.Click += async (_, _) => await CancelSelectedExecutionAsync();
        btnRetryExecution.Click += async (_, _) => await RetrySelectedExecutionAsync();
        btnRefreshExecutions.Click += async (_, _) => await RefreshExecutionsAsync();
        btnExecutionsFirstPage.Click += async (_, _) => await GoToExecutionPageAsync(1);
        btnExecutionsPreviousPage.Click += async (_, _) => await GoToExecutionPageAsync(executionsPageNumber - 1);
        btnExecutionsNextPage.Click += async (_, _) => await GoToExecutionPageAsync(executionsPageNumber + 1);
        btnExecutionsLastPage.Click += async (_, _) => await GoToExecutionPageAsync(ExecutionsTotalPages);
        cboExecutionsPageSize.SelectedIndexChanged += async (_, _) => await ChangeExecutionsPageSizeAsync();
        executionsPollingTimer.Tick += async (_, _) => await RefreshActiveExecutionsAsync();

        grdExecutions.SetData(executionRows);
        UpdateExecutionPager();
        UpdateExecutionActionState();
        UpdateExecutionPollingState();
    }

    private static NuanGridColumnDefinition ExecutionColumn(
        string fieldName,
        string caption,
        int visibleIndex,
        int width,
        NuanGridColumnFormat format = NuanGridColumnFormat.Text)
    {
        return new NuanGridColumnDefinition
        {
            FieldName = fieldName,
            Caption = caption,
            VisibleIndex = visibleIndex,
            Width = width,
            Format = format
        };
    }

    private async Task RefreshExecutionsAsync(int? preferredExecutionId = null)
    {
        if (refreshingExecutions || profileId is null || executionClient is null || !CanViewExecutions)
        {
            UpdateExecutionActionState();
            UpdateExecutionPollingState();
            return;
        }

        refreshingExecutions = true;
        var selectionId = preferredExecutionId ?? SelectedExecution()?.Id;
        btnRefreshExecutions.Enabled = false;
        UpdateExecutionPager();
        UpdateExecutionActionState();
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                var page = await LoadExecutionPageAsync();
                var totalPages = CalculateExecutionTotalPages(page.TotalCount);
                if (executionsPageNumber > totalPages)
                {
                    executionsPageNumber = totalPages;
                    page = await LoadExecutionPageAsync();
                }

                if (IsDisposed || Disposing)
                {
                    return;
                }

                executionsLoaded = true;
                executionsTotalCount = page.TotalCount;
                executionsPageNumber = Math.Clamp(page.PageNumber, 1, CalculateExecutionTotalPages(page.TotalCount));
                executionRows.Clear();
                executionRows.AddRange(page.Items.Select(ExecutionGridRow.From));
                grdExecutions.SetData(executionRows);
                RestoreExecutionSelection(selectionId);
                UpdateExecutionPager();
                UpdateExecutionActionState();
                UpdateExecutionPollingState();
            });
        }
        finally
        {
            refreshingExecutions = false;
            UpdateExecutionActionState();
        }
    }

    private Task<PagedResult<SyncProfileExecutionListItem>> LoadExecutionPageAsync()
    {
        return executionClient!.SearchExecutionsAsync(new SyncProfileExecutionFilter
        {
            ProfileId = profileId,
            PageNumber = executionsPageNumber,
            PageSize = executionsPageSize
        });
    }

    private void RestoreExecutionSelection(int? executionId)
    {
        if (executionRows.Count == 0)
        {
            return;
        }

        var rowHandle = executionId.HasValue
            ? grdExecutions.GridView.LocateByValue(nameof(ExecutionGridRow.Id), executionId.Value)
            : -1;
        grdExecutions.GridView.FocusedRowHandle = rowHandle >= 0 ? rowHandle : 0;
    }

    private async void ExecutionsGrid_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        await OpenSelectedExecutionDetailAsync();
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private async Task RefreshActiveExecutionsAsync()
    {
        if (ReferenceEquals(navigationFrame.SelectedPage, pageExecutions)
            && executionRows.Any(row => SyncExecutionStatusPolicy.IsActive(row.Source.Status)))
        {
            await RefreshExecutionsAsync();
        }
        else
        {
            UpdateExecutionPollingState();
        }
    }

    private async Task OpenSelectedExecutionDetailAsync()
    {
        if (SelectedExecution() is not { } execution
            || executionClient is null
            || session is null
            || refreshingExecutions
            || executionActionInProgress
            || !CanViewExecutions)
        {
            return;
        }

        using var form = new SyncExecutionDetailForm(
            new SyncProfileExecutionDetailViewModel(executionClient),
            session,
            execution.Id,
            allowActions: !IsReadOnlyMode);
        form.ShowDialog(this);
        await RefreshExecutionsAsync();
    }

    private async Task CancelSelectedExecutionAsync()
    {
        if (SelectedExecution() is not { } execution
            || executionClient is null
            || IsReadOnlyMode
            || executionActionInProgress
            || !CanCancelExecutions)
        {
            return;
        }

        if (!SyncExecutionStatusPolicy.CanCancel(execution.Source.Status))
        {
            ShowWarning("La ejecución seleccionada ya no admite cancelación.");
            await RefreshExecutionsAsync(execution.Id);
            return;
        }

        if (XtraMessageBox.Show(
                this,
                $"Cancelar la ejecucion {execution.Id}?",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        executionActionInProgress = true;
        UpdateExecutionActionState();
        UpdateExecutionPollingState();
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                await executionClient.CancelExecutionAsync(execution.Id);
                await RefreshExecutionsAsync(execution.Id);
            });
        }
        finally
        {
            executionActionInProgress = false;
            UpdateExecutionActionState();
            UpdateExecutionPollingState();
        }
    }

    private async Task RetrySelectedExecutionAsync()
    {
        if (SelectedExecution() is not { } execution
            || executionClient is null
            || IsReadOnlyMode
            || executionActionInProgress
            || !CanRetryExecutions)
        {
            return;
        }

        if (!SyncExecutionStatusPolicy.CanRetry(execution.Source.Status))
        {
            ShowWarning("La ejecución seleccionada no admite reintento.");
            await RefreshExecutionsAsync(execution.Id);
            return;
        }

        if (XtraMessageBox.Show(
                this,
                $"Reintentar la ejecucion {execution.Id}?",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        executionActionInProgress = true;
        UpdateExecutionActionState();
        UpdateExecutionPollingState();
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                var retry = await executionClient.RetryExecutionAsync(execution.Id);
                executionsPageNumber = 1;
                await RefreshExecutionsAsync(retry.NewExecutionId);
            });
        }
        finally
        {
            executionActionInProgress = false;
            UpdateExecutionActionState();
            UpdateExecutionPollingState();
        }
    }

    private async Task GoToExecutionPageAsync(int pageNumber)
    {
        var targetPage = Math.Clamp(pageNumber, 1, ExecutionsTotalPages);
        if (targetPage == executionsPageNumber)
        {
            return;
        }

        executionsPageNumber = targetPage;
        await RefreshExecutionsAsync();
    }

    private async Task ChangeExecutionsPageSizeAsync()
    {
        if (!int.TryParse(Convert.ToString(cboExecutionsPageSize.EditValue), out var pageSize)
            || pageSize == executionsPageSize)
        {
            return;
        }

        executionsPageSize = pageSize;
        executionsPageNumber = 1;
        await RefreshExecutionsAsync();
    }

    private void UpdateExecutionPager()
    {
        lblExecutionsPageInfo.Text = $"Pagina {executionsPageNumber} de {ExecutionsTotalPages}";
        lblExecutionsTotal.Text = $"Total: {executionsTotalCount:N0} ejecuciones";
        var canNavigate = CanViewExecutions
                          && profileId.HasValue
                          && !refreshingExecutions
                          && !executionActionInProgress;
        btnExecutionsFirstPage.Enabled = canNavigate && executionsPageNumber > 1;
        btnExecutionsPreviousPage.Enabled = canNavigate && executionsPageNumber > 1;
        btnExecutionsNextPage.Enabled = canNavigate && executionsPageNumber < ExecutionsTotalPages;
        btnExecutionsLastPage.Enabled = canNavigate && executionsPageNumber < ExecutionsTotalPages;
        cboExecutionsPageSize.Enabled = canNavigate;
    }

    private void UpdateExecutionActionState()
    {
        var selected = SelectedExecution();
        var hasPersistedProfile = profileId.HasValue;
        var hasSelection = selected is not null;
        var actionsAvailable = !refreshingExecutions && !executionActionInProgress;
        var mutationActionsAvailable = actionsAvailable && !IsReadOnlyMode;
        btnViewExecutionDetail.Enabled = actionsAvailable && CanViewExecutions && hasSelection;
        btnCancelExecution.Enabled = mutationActionsAvailable
            && CanCancelExecutions
            && selected is not null
            && SyncExecutionStatusPolicy.CanCancel(selected.Source.Status);
        btnRetryExecution.Enabled = mutationActionsAvailable
            && CanRetryExecutions
            && selected is not null
            && SyncExecutionStatusPolicy.CanRetry(selected.Source.Status);
        btnRefreshExecutions.Enabled = actionsAvailable && CanViewExecutions && hasPersistedProfile;
        UpdateExecutionPager();
    }

    private void UpdateExecutionPollingState()
    {
        var shouldPoll = ReferenceEquals(navigationFrame.SelectedPage, pageExecutions)
            && !executionActionInProgress
            && executionRows.Any(row => SyncExecutionStatusPolicy.IsActive(row.Source.Status));
        lblExecutionsAutoRefresh.Visible = shouldPoll;
        if (shouldPoll)
        {
            executionsPollingTimer.Start();
        }
        else
        {
            executionsPollingTimer.Stop();
        }
    }

    private void ExecutionsView_CustomDrawEmptyForeground(object? sender, CustomDrawEventArgs e)
    {
        var text = profileId is null
            ? "Guarde el perfil para consultar sus ejecuciones."
            : !CanViewExecutions
                ? "No tiene acceso para consultar las ejecuciones."
                : executionsLoaded
                    ? "No existen ejecuciones registradas para este perfil."
                    : "Seleccione Actualizar para cargar las ejecuciones.";
        using var brush = new SolidBrush(BrandResources.MutedText);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        e.Cache.Graphics.DrawString(text, AppTypography.BaseReadableFont, brush, e.Bounds, format);
        e.Handled = true;
    }

    private ExecutionGridRow? SelectedExecution()
    {
        return grdExecutions.GetFocusedRow<ExecutionGridRow>();
    }

    private int ExecutionsTotalPages => CalculateExecutionTotalPages(executionsTotalCount);
    private bool CanViewExecutions => session?.HasPermission(PermissionCodes.SyncConfigurationViewExecutions) == true;
    private bool CanManageBusinessPartnerSapCodePolicy =>
        session?.HasPermission(PermissionCodes.SapManage) == true;
    private bool CanCreateEntityDefinition =>
        entityDefinitionClient is not null
        && session?.HasPermission(PermissionCodes.SyncEntitiesCreate) == true;
    private bool CanCancelExecutions => session?.HasPermission(PermissionCodes.SyncConfigurationCancel) == true;
    private bool CanRetryExecutions => session?.HasPermission(PermissionCodes.SyncConfigurationRetry) == true;

    private int CalculateExecutionTotalPages(int totalCount)
    {
        return Math.Max(1, (int)Math.Ceiling(totalCount / (double)executionsPageSize));
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        executionsPollingTimer.Stop();
        executionsPollingTimer.Dispose();
        base.OnFormClosed(e);
    }

    private void WireBranchActions()
    {
        btnAddBranch.Click += BtnAddBranch_Click;
        btnEditBranch.Click += (_, _) => EditSelectedBranch();
        btnRemoveBranch.Click += BtnRemoveBranch_Click;
        btnActivateBranch.Click += (_, _) => SetSelectedBranchActive(true);
        btnDeactivateBranch.Click += (_, _) => SetSelectedBranchActive(false);
        btnRefreshBranches.Click += async (_, _) => await RunWithUiExceptionHandlingAsync(RefreshBranchesAsync);
        grdBranches.RowDoubleClick += (_, _) => EditSelectedBranch();
    }

    private void WireEntityActions()
    {
        btnAddEntity.Click += BtnAddEntity_Click;
        btnEditEntity.Click += BtnEditEntity_Click;
        btnRemoveEntity.Click += BtnRemoveEntity_Click;
        btnMoveEntityUp.Click += (_, _) => MoveSelectedEntity(-1);
        btnMoveEntityDown.Click += (_, _) => MoveSelectedEntity(1);
        btnActivateEntity.Click += (_, _) => SetSelectedEntityActive(true);
        btnDeactivateEntity.Click += (_, _) => SetSelectedEntityActive(false);
        grdEntities.RowDoubleClick += (_, _) => EditSelectedEntity();
    }

    private void WireDistributionActions()
    {
        btnEnableDistribution.Click += (_, _) => SetSelectedDistributionEnabled(true);
        btnDisableDistribution.Click += (_, _) => SetSelectedDistributionEnabled(false);
        btnConfigureDistributionBatch.Click += async (_, _) => await OpenDistributionDialogAsync();
        btnEnableAllDistributions.Click += (_, _) => SetAllDistributionsEnabled(true);
        btnDisableAllDistributions.Click += (_, _) => SetAllDistributionsEnabled(false);
        btnRefreshDistribution.Click += (_, _) => RefreshDistributionGrid();
        grdDistribution.RowDoubleClick += async (_, _) => await OpenDistributionDialogAsync();
    }

    private void BtnAddBranch_Click(object? sender, EventArgs e)
    {
        if (viewModel is null || IsReadOnlyMode)
        {
            return;
        }

        using var dialog = new SyncProfileBranchDialog(BranchLookupOptions(), canCreateBranchCompany);
        dialog.CreateBranchCompanyRequested += DialogCreateBranchCompanyRequested;
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is not { } result)
        {
            return;
        }

        var branch = new CompanyLookupItem(
            result.BranchCompanyId,
            result.BranchCompanyCode,
            result.BranchCompanyName,
            result.IsActive,
            result.BranchCode,
            result.DatabaseName);
        if (!viewModel.State.AddBranch(branch, result.BatchSize, result.MaxRetries, result.IsActive))
        {
            ShowWarning("La sucursal seleccionada ya pertenece al perfil.");
            return;
        }

        InvalidateValidationResult();
        LoadBranchesFromState();
        RefreshBranchesGrid();
        RefreshDistributionGrid();
    }

    private void EditSelectedBranch()
    {
        if (viewModel is null || IsReadOnlyMode || SelectedBranch() is not { } selected)
        {
            return;
        }

        var initialValue = new SyncProfileBranchDialogResult(
            selected.BranchCompanyId,
            selected.BranchCompanyCode,
            selected.BranchCompanyName,
            selected.BranchCode,
            selected.BranchDatabaseName,
            selected.BatchSize,
            selected.MaxRetries,
            string.Equals(selected.StatusText, "Activo", StringComparison.OrdinalIgnoreCase),
            selected.LastSynchronizationAt);
        using var dialog = new SyncProfileBranchDialog(
            BranchLookupOptions(),
            canCreateBranchCompany,
            initialValue);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is not { } result)
        {
            return;
        }

        if (!viewModel.State.UpdateBranch(
            selected.BranchCompanyId,
            result.BatchSize,
            result.MaxRetries,
            result.IsActive))
        {
            return;
        }

        InvalidateValidationResult();
        LoadBranchesFromState();
        RefreshBranchesGrid();
        RefreshDistributionGrid();
    }

    private void BtnRemoveBranch_Click(object? sender, EventArgs e)
    {
        if (viewModel is null || IsReadOnlyMode || SelectedBranch() is not { } selected)
        {
            return;
        }

        if (XtraMessageBox.Show(this,
                $"Desea quitar la sucursal '{selected.BranchCompanyName}' del perfil?",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        if (!viewModel.State.RemoveBranch(selected.BranchCompanyId))
        {
            return;
        }

        InvalidateValidationResult();
        LoadBranchesFromState();
        RefreshBranchesGrid();
        RefreshDistributionGrid();
    }

    private void SetSelectedBranchActive(bool isActive)
    {
        if (viewModel is null || IsReadOnlyMode || SelectedBranch() is not { } selected)
        {
            return;
        }

        if (!viewModel.State.SetBranchActive(selected.BranchCompanyId, isActive))
        {
            return;
        }

        InvalidateValidationResult();
        LoadBranchesFromState();
        RefreshBranchesGrid();
        RefreshDistributionGrid();
    }

    private async Task RefreshBranchesAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        btnRefreshBranches.Enabled = false;
        try
        {
            await viewModel.RefreshCatalogAsync();
            LoadBranchesFromState();
            RefreshBranchesGrid();
            RefreshDistributionGrid();
        }
        finally
        {
            if (!IsDisposed && !Disposing)
            {
                btnRefreshBranches.Enabled = true;
            }
        }
    }

    private BranchPreviewRow? SelectedBranch()
    {
        return grdBranches.GetFocusedRow<BranchPreviewRow>();
    }

    private async void BtnAddEntity_Click(object? sender, EventArgs e)
    {
        if (viewModel is null || IsReadOnlyMode)
        {
            return;
        }

        var availableEntities = EntityCatalogOptions()
            .Where(item => entityPreviewRows.All(row =>
                !string.Equals(row.EntityCode, item.Code, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        if (availableEntities.Length == 0)
        {
            XtraMessageBox.Show(this, "No existen entidades disponibles para agregar.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SyncProfileEntityDialog(
            availableEntities,
            ExecutionModeOptions(),
            suggestedExecutionOrder: viewModel.State.Entities.Count + 1,
            canCreateEntity: CanCreateEntityDefinition);
        dialog.CreateEntityRequested += CreateEntityDefinitionFromLookupAsync;
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is not { } result)
        {
            return;
        }

        if (!viewModel.State.AddEntity(EntityPreviewRow.FromResult(result).ToEditorRow()))
        {
            ShowWarning("La entidad seleccionada ya pertenece al perfil.");
            return;
        }

        InvalidateValidationResult();
        LoadEntitiesFromState();
        RefreshEntitiesGrid();
        RefreshDistributionGrid();
        await RunWithUiExceptionHandlingAsync(RefreshBusinessPartnerCodePolicyAsync);
    }

    private async Task<SyncEntityCatalogItem?> CreateEntityDefinitionFromLookupAsync(SyncProfileEntityDialog owner)
    {
        if (!CanCreateEntityDefinition || entityDefinitionClient is null || viewModel is null)
        {
            ShowWarning("No tiene acceso para crear entidades de sincronizacion.");
            return null;
        }

        var editViewModel = new SyncEntityDefinitionEditViewModel(entityDefinitionClient);
        await editViewModel.InitializeAsync(null);
        using var form = new SyncEntityEditForm(editViewModel);
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        var createdCode = editViewModel.State.Code;
        await viewModel.RefreshCatalogAsync();
        return viewModel.Catalog.Entities.FirstOrDefault(item =>
            string.Equals(item.Code, createdCode, StringComparison.OrdinalIgnoreCase));
    }

    private void BtnEditEntity_Click(object? sender, EventArgs e)
    {
        EditSelectedEntity();
    }

    private void EditSelectedEntity()
    {
        if (viewModel is null || IsReadOnlyMode || grdEntities.GetFocusedRow<EntityPreviewRow>() is not { } selected)
        {
            return;
        }

        using var dialog = new SyncProfileEntityDialog(
            EntityCatalogOptions(),
            ExecutionModeOptions(),
            selected.ToEditorRow());
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is not { } result)
        {
            return;
        }

        if (!viewModel.State.UpdateEntity(EntityPreviewRow.FromResult(result).ToEditorRow()))
        {
            return;
        }

        InvalidateValidationResult();
        LoadEntitiesFromState();
        RefreshEntitiesGrid();
        RefreshDistributionGrid();
    }

    private async void BtnRemoveEntity_Click(object? sender, EventArgs e)
    {
        if (viewModel is null || IsReadOnlyMode || grdEntities.GetFocusedRow<EntityPreviewRow>() is not { } selected)
        {
            return;
        }

        if (XtraMessageBox.Show(this,
                $"¿Desea quitar la entidad '{selected.EntityName}' del perfil?",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        if (!viewModel.State.RemoveEntity(selected.EntityCode))
        {
            return;
        }

        InvalidateValidationResult();
        LoadEntitiesFromState();
        RefreshEntitiesGrid();
        RefreshDistributionGrid();
        await RunWithUiExceptionHandlingAsync(RefreshBusinessPartnerCodePolicyAsync);
    }

    private void MoveSelectedEntity(int offset)
    {
        if (viewModel is null || IsReadOnlyMode || grdEntities.GetFocusedRow<EntityPreviewRow>() is not { } selected)
        {
            return;
        }

        if (!viewModel.State.MoveEntity(selected.EntityCode, offset))
        {
            return;
        }

        InvalidateValidationResult();
        LoadEntitiesFromState();
        RefreshEntitiesGrid();
        RefreshDistributionGrid();
    }

    private void SetSelectedEntityActive(bool isActive)
    {
        if (viewModel is null || IsReadOnlyMode || grdEntities.GetFocusedRow<EntityPreviewRow>() is not { } selected)
        {
            return;
        }

        if (!viewModel.State.SetEntityActive(selected.EntityCode, isActive))
        {
            return;
        }

        InvalidateValidationResult();
        LoadEntitiesFromState();
        RefreshEntitiesGrid();
        RefreshDistributionGrid();
    }

    private void RefreshDistributionGrid()
    {
        if (distributionCheckEditor is null)
        {
            return;
        }

        var previousSelection = SelectedDistribution();
        refreshingDistribution = true;
        try
        {
            distributionBranchColumns.Clear();
            var table = new DataTable("SyncProfileDistribution");
            table.Columns.Add(nameof(EntityPreviewRow.EntityCode), typeof(string));
            table.Columns.Add(nameof(EntityPreviewRow.EntityName), typeof(string));
            table.Columns.Add(nameof(EntityPreviewRow.ExecutionOrder), typeof(int));
            table.Columns.Add(nameof(EntityPreviewRow.SyncMode), typeof(string));

            foreach (var branch in branchPreviewRows)
            {
                var fieldName = DistributionFieldName(branch.BranchCompanyId);
                table.Columns.Add(fieldName, typeof(bool));
                distributionBranchColumns[fieldName] = branch.BranchCompanyId;
            }

            foreach (var entity in entityPreviewRows.OrderBy(entity => entity.ExecutionOrder))
            {
                var row = table.NewRow();
                row[nameof(EntityPreviewRow.EntityCode)] = entity.EntityCode;
                row[nameof(EntityPreviewRow.EntityName)] = entity.EntityName;
                row[nameof(EntityPreviewRow.ExecutionOrder)] = entity.ExecutionOrder;
                row[nameof(EntityPreviewRow.SyncMode)] = entity.SyncMode;
                foreach (var branch in branchPreviewRows)
                {
                    row[DistributionFieldName(branch.BranchCompanyId)] =
                        GetDistributionState(entity.EntityCode, branch.BranchCompanyId).IsEnabled;
                }

                table.Rows.Add(row);
            }

            distributionTable = table;
            grdDistribution.GridControl.DataSource = table;
            var view = grdDistribution.GridView;
            view.PopulateColumns();
            ConfigureDistributionBaseColumn(view.Columns[nameof(EntityPreviewRow.EntityCode)], "Código entidad", 150, 0);
            ConfigureDistributionBaseColumn(view.Columns[nameof(EntityPreviewRow.EntityName)], "Entidad", 170, 1);
            ConfigureDistributionBaseColumn(view.Columns[nameof(EntityPreviewRow.ExecutionOrder)], "Orden", 70, 2);
            ConfigureDistributionBaseColumn(view.Columns[nameof(EntityPreviewRow.SyncMode)], "Modo", 100, 3);
            view.Columns[nameof(EntityPreviewRow.ExecutionOrder)].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;

            var visibleIndex = 4;
            foreach (var branch in branchPreviewRows)
            {
                var column = view.Columns[DistributionFieldName(branch.BranchCompanyId)];
                var branchIsActive = string.Equals(
                    branch.StatusText,
                    "Activo",
                    StringComparison.OrdinalIgnoreCase);
                column.Caption = string.IsNullOrWhiteSpace(branch.BranchCompanyName)
                    ? branch.BranchCode
                    : branch.BranchCompanyName;
                column.Visible = true;
                column.VisibleIndex = visibleIndex++;
                column.Width = 135;
                column.MinWidth = 110;
                column.ColumnEdit = distributionCheckEditor;
                column.OptionsColumn.AllowEdit = branchIsActive;
                column.OptionsColumn.ReadOnly = !branchIsActive;
                column.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                column.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            }

            RestoreDistributionSelection(previousSelection);
        }
        finally
        {
            refreshingDistribution = false;
        }

        UpdateDistributionActionState();
    }

    private static void ConfigureDistributionBaseColumn(
        GridColumn column,
        string caption,
        int width,
        int visibleIndex)
    {
        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
        column.Fixed = FixedStyle.Left;
        column.OptionsColumn.AllowEdit = false;
        column.OptionsColumn.ReadOnly = true;
    }

    private void RestoreDistributionSelection(DistributionSelection? previousSelection)
    {
        var view = grdDistribution.GridView;
        if (view.DataRowCount == 0 || distributionBranchColumns.Count == 0)
        {
            return;
        }

        var entityCode = previousSelection?.Entity.EntityCode;
        var rowHandle = 0;
        if (!string.IsNullOrWhiteSpace(entityCode))
        {
            for (var index = 0; index < view.DataRowCount; index++)
            {
                if (string.Equals(
                        Convert.ToString(view.GetRowCellValue(index, nameof(EntityPreviewRow.EntityCode))),
                        entityCode,
                        StringComparison.OrdinalIgnoreCase))
                {
                    rowHandle = index;
                    break;
                }
            }
        }

        var branchId = previousSelection?.Branch.BranchCompanyId
                       ?? distributionBranchColumns.Values.First();
        var fieldName = DistributionFieldName(branchId);
        view.FocusedRowHandle = rowHandle;
        if (view.Columns[fieldName] is { } column)
        {
            view.FocusedColumn = column;
        }
    }

    private void DistributionView_CellValueChanged(object? sender, CellValueChangedEventArgs e)
    {
        if (IsReadOnlyMode
            || refreshingDistribution
            || !distributionBranchColumns.TryGetValue(e.Column.FieldName, out var branchCompanyId))
        {
            return;
        }

        var entityCode = Convert.ToString(
            grdDistribution.GridView.GetRowCellValue(e.RowHandle, nameof(EntityPreviewRow.EntityCode)));
        if (string.IsNullOrWhiteSpace(entityCode))
        {
            return;
        }

        var current = GetDistributionState(entityCode, branchCompanyId);
        if (!SetDistributionState(
            entityCode,
            branchCompanyId,
            Convert.ToBoolean(e.Value),
            current.BatchSize,
            updateBatch: false))
        {
            RefreshDistributionGrid();
            return;
        }

        grdDistribution.GridView.RefreshRowCell(e.RowHandle, e.Column);
    }

    private void DistributionView_CustomDrawCell(object? sender, RowCellCustomDrawEventArgs e)
    {
        if (e.RowHandle < 0
            || !distributionBranchColumns.TryGetValue(e.Column.FieldName, out var branchCompanyId))
        {
            return;
        }

        var entityCode = Convert.ToString(
            grdDistribution.GridView.GetRowCellValue(e.RowHandle, nameof(EntityPreviewRow.EntityCode)));
        if (string.IsNullOrWhiteSpace(entityCode)
            || !GetDistributionState(entityCode, branchCompanyId).BatchSize.HasValue)
        {
            return;
        }

        e.DefaultDraw();
        var indicatorBounds = new Rectangle(
            e.Bounds.Right - 16,
            e.Bounds.Top + Math.Max(2, (e.Bounds.Height - 8) / 2),
            8,
            8);
        using var indicatorBrush = new SolidBrush(Color.FromArgb(0, 160, 128));
        e.Cache.FillEllipse(indicatorBrush, indicatorBounds);
        e.Handled = true;
    }

    private void SetSelectedDistributionEnabled(bool isEnabled)
    {
        if (IsReadOnlyMode || SelectedDistribution() is not { } selected)
        {
            return;
        }

        SetDistributionState(
            selected.Entity.EntityCode,
            selected.Branch.BranchCompanyId,
            isEnabled,
            selected.State.BatchSize,
            updateBatch: false);
        RefreshDistributionGrid();
    }

    private void SetAllDistributionsEnabled(bool isEnabled)
    {
        if (IsReadOnlyMode)
        {
            return;
        }

        if (viewModel is not null)
        {
            viewModel.State.SetAllDistributionsEnabled(isEnabled);
            InvalidateValidationResult();
            RefreshDistributionGrid();
            return;
        }

        foreach (var entity in entityPreviewRows)
        {
            foreach (var branch in branchPreviewRows)
            {
                var current = GetDistributionState(entity.EntityCode, branch.BranchCompanyId);
                SetDistributionState(
                    entity.EntityCode,
                    branch.BranchCompanyId,
                    isEnabled,
                    current.BatchSize,
                    updateBatch: false);
            }
        }

        RefreshDistributionGrid();
    }

    private async Task OpenDistributionDialogAsync()
    {
        if (IsReadOnlyMode || SelectedDistribution() is not { } selected)
        {
            XtraMessageBox.Show(this,
                "Seleccione una combinación entidad-sucursal.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        if (!string.Equals(selected.Branch.StatusText, "Activo", StringComparison.OrdinalIgnoreCase))
        {
            XtraMessageBox.Show(this,
                "Active la sucursal antes de configurar su distribución.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var profileBatchSize = viewModel?.State.BatchSize ?? 500;
        var entityBatchSize = selected.Entity.BatchSize;
        var branchBatchSize = viewModel is null
            ? selected.Branch.BatchSize
            : viewModel.State.Branches.FirstOrDefault(branch =>
                branch.BranchCompanyId == selected.Branch.BranchCompanyId)?.BatchSize;

        var matrixId = viewModel?.State.GetDistribution(
            selected.Entity.EntityCode,
            selected.Branch.BranchCompanyId)?.MatrixId ?? 0;
        if (matrixId <= 0 || executionClient is null)
        {
            XtraMessageBox.Show(this,
                "Guarde el perfil antes de configurar modalidades Selected o Rule. Al guardar se crean las celdas persistentes de la matriz.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        SyncDistributionPolicy policy;
        SyncDistributionPolicyCatalog policyCatalog;
        try
        {
            Cursor = Cursors.WaitCursor;
            policy = await executionClient.GetDistributionPolicyAsync(matrixId);
            policyCatalog = await executionClient.GetDistributionPolicyCatalogAsync(selected.Entity.EntityCode);
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, Text, exception);
            return;
        }
        finally
        {
            Cursor = Cursors.Default;
        }

        using var dialog = new SyncProfileDistributionDialog(new SyncProfileDistributionDialogData(
            matrixId,
            selected.Entity.EntityCode,
            selected.Entity.EntityName,
            selected.Branch.BranchCompanyId,
            selected.Branch.BranchCompanyCode,
            selected.Branch.BranchCode,
            selected.Branch.BranchCompanyName,
            selected.State.IsEnabled,
            selected.State.BatchSize,
            entityBatchSize,
            branchBatchSize,
            profileBatchSize),
            executionClient,
            policy,
            policyCatalog);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is not { } result)
        {
            return;
        }

        SetDistributionState(
            selected.Entity.EntityCode,
            selected.Branch.BranchCompanyId,
            result.IsEnabled,
            result.BatchSize,
            updateBatch: true);
        RefreshDistributionGrid();
    }

    private DistributionSelection? SelectedDistribution()
    {
        if (distributionTable is null
            || grdDistribution.GridView.FocusedColumn is not { } column
            || !distributionBranchColumns.TryGetValue(column.FieldName, out var branchCompanyId)
            || grdDistribution.GridView.GetFocusedDataRow() is not { } dataRow)
        {
            return null;
        }

        var entityCode = Convert.ToString(dataRow[nameof(EntityPreviewRow.EntityCode)]);
        var entity = entityPreviewRows.FirstOrDefault(item => string.Equals(
            item.EntityCode,
            entityCode,
            StringComparison.OrdinalIgnoreCase));
        var branch = branchPreviewRows.FirstOrDefault(item => item.BranchCompanyId == branchCompanyId);
        return entity is null || branch is null
            ? null
            : new DistributionSelection(
                entity,
                branch,
                GetDistributionState(entity.EntityCode, branch.BranchCompanyId));
    }

    private DistributionPreviewState GetDistributionState(string entityCode, int branchCompanyId)
    {
        if (viewModel is not null)
        {
            var branch = viewModel.State.Branches.FirstOrDefault(item => item.BranchCompanyId == branchCompanyId);
            if (branch is null || !branch.IsActive)
            {
                return new DistributionPreviewState(false, null);
            }

            var relation = viewModel.State.GetDistribution(entityCode, branchCompanyId);
            return relation is null
                ? new DistributionPreviewState(false, null)
                : new DistributionPreviewState(relation.IsEnabled, relation.BatchSize);
        }

        var key = DistributionPreviewKey(entityCode, branchCompanyId);
        if (distributionPreviewStates.TryGetValue(key, out var state))
        {
            return state;
        }

        var entityIndex = Math.Max(0, entityPreviewRows.FindIndex(entity => string.Equals(
            entity.EntityCode,
            entityCode,
            StringComparison.OrdinalIgnoreCase)));
        var branchIndex = Math.Max(0, branchPreviewRows.FindIndex(branch =>
            branch.BranchCompanyId == branchCompanyId));
        var isEnabled = (entityIndex + branchIndex) % 3 != 2;
        state = new DistributionPreviewState(
            isEnabled,
            isEnabled && (entityIndex + branchIndex) % 2 == 0 ? 500 : null);
        distributionPreviewStates[key] = state;
        return state;
    }

    private bool SetDistributionState(
        string entityCode,
        int branchCompanyId,
        bool isEnabled,
        int? batchSize,
        bool updateBatch)
    {
        if (IsReadOnlyMode)
        {
            return false;
        }

        if (viewModel is null)
        {
            var key = DistributionPreviewKey(entityCode, branchCompanyId);
            var current = GetDistributionState(entityCode, branchCompanyId);
            distributionPreviewStates[key] = new DistributionPreviewState(
                isEnabled,
                updateBatch ? batchSize : current.BatchSize);
            return true;
        }

        var updated = viewModel.State.SetDistribution(
            entityCode,
            branchCompanyId,
            isEnabled,
            batchSize,
            updateBatch);
        if (updated)
        {
            InvalidateValidationResult();
        }

        return updated;
    }

    private void UpdateDistributionActionState()
    {
        var selection = SelectedDistribution();
        var canEditDistribution = !IsReadOnlyMode;
        var selectedBranchIsActive = canEditDistribution
                                     && selection is not null
                                     && string.Equals(
                                         selection.Branch.StatusText,
                                         "Activo",
                                         StringComparison.OrdinalIgnoreCase);
        var hasActiveMatrix = canEditDistribution
                              && entityPreviewRows.Count > 0
                              && branchPreviewRows.Any(branch => string.Equals(
                                  branch.StatusText,
                                  "Activo",
                                  StringComparison.OrdinalIgnoreCase));
        btnEnableDistribution.Enabled = selectedBranchIsActive;
        btnDisableDistribution.Enabled = selectedBranchIsActive;
        btnConfigureDistributionBatch.Enabled = selectedBranchIsActive;
        btnEnableAllDistributions.Enabled = hasActiveMatrix;
        btnDisableAllDistributions.Enabled = hasActiveMatrix;
    }

    private static string DistributionFieldName(int branchCompanyId)
    {
        return $"Branch_{branchCompanyId}";
    }

    private static string DistributionPreviewKey(string entityCode, int branchCompanyId)
    {
        return $"{entityCode.Trim()}|{branchCompanyId}";
    }

    private async Task<CompanyLookupItem?> DialogCreateBranchCompanyRequested(SyncProfileBranchDialog owner)
    {
        if (CreateBranchCompanyRequested is null)
        {
            return null;
        }

        var created = await CreateBranchCompanyRequested(this);
        if (created is null)
        {
            return null;
        }

        owner.RefreshBranchCompanies(BranchLookupOptions(created), created.Id);
        return created;
    }

    public void RefreshBranchLookupCatalog()
    {
        RefreshBranchesGrid();
        RefreshDistributionGrid();
    }

    private IReadOnlyCollection<CompanyLookupItem> BranchLookupOptions(CompanyLookupItem? extra = null)
    {
        var branches = viewModel?.Catalog.BranchCompanies is { Count: > 0 } catalogBranches
            ? catalogBranches.ToList()
            : new List<CompanyLookupItem>();

        foreach (var configuredBranch in branchPreviewRows)
        {
            if (branches.Any(branch => branch.Id == configuredBranch.BranchCompanyId))
            {
                continue;
            }

            branches.Add(new CompanyLookupItem(
                configuredBranch.BranchCompanyId,
                configuredBranch.BranchCompanyCode,
                configuredBranch.BranchCompanyName,
                string.Equals(configuredBranch.StatusText, "Activo", StringComparison.OrdinalIgnoreCase),
                configuredBranch.BranchCode,
                configuredBranch.BranchDatabaseName));
        }

        if (extra is not null && branches.All(branch => branch.Id != extra.Id))
        {
            branches.Add(extra);
        }

        return branches;
    }

    private void RefreshBranchesGrid()
    {
        grdBranches.SetData(branchPreviewRows);
        lblBranchesTotal.Text = $"Total: {branchPreviewRows.Count} sucursales";
    }

    private void RefreshEntitiesGrid()
    {
        grdEntities.SetData(entityPreviewRows.OrderBy(row => row.ExecutionOrder).ToArray());
        lblEntitiesTotal.Text = $"Total: {entityPreviewRows.Count} entidades";
    }

    private void LoadBranchesFromState()
    {
        branchPreviewRows.Clear();
        if (viewModel is null)
        {
            return;
        }

        foreach (var branch in viewModel.State.Branches)
        {
            branchPreviewRows.Add(new BranchPreviewRow(
                branch.BranchCompanyId,
                branch.BranchCompanyCode,
                branch.BranchCode ?? string.Empty,
                branch.BranchCompanyName,
                branch.DatabaseName ?? string.Empty,
                branch.IsActive ? "Activo" : "Inactivo",
                branch.BatchSize ?? viewModel.State.BatchSize,
                branch.MaxRetries ?? viewModel.State.MaxRetries,
                branch.LastSynchronizationAt));
        }
    }

    private void LoadEntitiesFromState()
    {
        entityPreviewRows.Clear();
        if (viewModel is null)
        {
            return;
        }

        entityPreviewRows.AddRange(viewModel.State.Entities
            .OrderBy(entity => entity.ExecutionOrder)
            .Select(EntityPreviewRow.FromEditorRow));
    }

    private IReadOnlyCollection<SyncEntityCatalogItem> EntityCatalogOptions()
    {
        var entities = viewModel?.Catalog.Entities is { Count: > 0 } catalogEntities
            ? catalogEntities.ToList()
            : new List<SyncEntityCatalogItem>();

        foreach (var row in entityPreviewRows)
        {
            if (entities.Any(entity => string.Equals(entity.Code, row.EntityCode, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            entities.Add(new SyncEntityCatalogItem
            {
                Code = row.EntityCode,
                Name = row.EntityName,
                DefaultExecutionOrder = row.ExecutionOrder,
                SupportsIncremental = true,
                HasProducer = true,
                HasApplier = true,
                SupportsInsert = row.AllowInsert,
                SupportsUpdate = row.AllowUpdate,
                SupportsDeactivate = row.AllowDeactivate,
                DefaultKeyField = row.KeyField,
                DefaultModifiedAtField = row.ModifiedAtField
            });
        }

        return entities;
    }

    private IReadOnlyCollection<string> ExecutionModeOptions()
    {
        return viewModel?.Catalog.ExecutionModes
                   .Select(mode => mode.Code)
                   .Where(code => !string.IsNullOrWhiteSpace(code))
                   .Distinct(StringComparer.OrdinalIgnoreCase)
                   .ToArray()
               ?? new[] { "Full", "Incremental", "Manual" };
    }

    private static IReadOnlyCollection<BranchPreviewRow> BranchPreviewRows()
    {
        return new[]
        {
            new BranchPreviewRow(1, "NUA", "SUC-NTE", "Sucursal Norte", "NUA_NORTE", "Activo", 500, 3, new DateTime(2026, 7, 13, 22, 41, 0)),
            new BranchPreviewRow(2, "NUA", "SUC-SUR", "Sucursal Sur", "NUA_SUR", "Activo", 500, 3, new DateTime(2026, 7, 13, 22, 40, 0)),
            new BranchPreviewRow(4, "NUA", "SUC-CEN", "Sucursal Centro", "NUA_CENTRO", "Inactivo", 500, 3, null),
            new BranchPreviewRow(3, "NUA", "SUC-OES", "Sucursal Oeste", "NUA_OESTE", "Activo", 300, 2, new DateTime(2026, 7, 12, 18, 10, 0))
        };
    }

    private static IReadOnlyCollection<EntityPreviewRow> EntityPreviewRows()
    {
        return new[]
        {
            new EntityPreviewRow
            {
                EntityCode = "BusinessPartner",
                EntityName = "Socios de negocio",
                ExecutionOrder = 1,
                SyncMode = "Full",
                KeyField = "CardCode",
                ModifiedAtField = "UpdateDate",
                VersionField = "LogInstanc",
                ActiveField = "ValidFor",
                AllowInsert = true,
                AllowUpdate = true,
                AllowDeactivate = true,
                ContinueOnError = true,
                BatchSize = 500,
                IsActive = true
            },
            new EntityPreviewRow
            {
                EntityCode = "Item",
                EntityName = "Artículos",
                ExecutionOrder = 2,
                SyncMode = "Incremental",
                KeyField = "ItemCode",
                ModifiedAtField = "UpdateDate",
                VersionField = "LogInstanc",
                ActiveField = "ValidFor",
                AllowInsert = true,
                AllowUpdate = true,
                AllowDeactivate = false,
                ContinueOnError = true,
                BatchSize = 300,
                IsActive = true
            },
            new EntityPreviewRow
            {
                EntityCode = "Warehouse",
                EntityName = "Bodegas",
                ExecutionOrder = 3,
                SyncMode = "Manual",
                KeyField = "WhsCode",
                ModifiedAtField = "UpdateDate",
                VersionField = "LogInstanc",
                AllowInsert = true,
                AllowUpdate = false,
                AllowDeactivate = false,
                ContinueOnError = false,
                IsActive = false
            }
        };
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }

    private sealed class ExecutionGridRow
    {
        private ExecutionGridRow(SyncProfileExecutionListItem source)
        {
            Source = source;
        }

        public SyncProfileExecutionListItem Source { get; }
        public int Id => Source.Id;
        public string ExecutionTypeText => SyncExecutionStatusPolicy.ExecutionTypeText(Source.ExecutionType);
        public string StatusText => SyncExecutionStatusPolicy.StatusText(Source.Status);
        public string RequestedBy => string.IsNullOrWhiteSpace(Source.RequestedBy) ? "Sistema" : Source.RequestedBy;
        public DateTimeOffset RequestedAt => Source.RequestedAt;
        public DateTimeOffset? StartedAt => Source.StartedAt;
        public DateTimeOffset? FinishedAt => Source.FinishedAt;
        public int TotalEntities => Source.TotalEntities;
        public int TotalRecordsRead => Source.TotalRecordsRead;
        public int TotalEventsPublished => Source.TotalEventsPublished;
        public int TotalSkipped => Source.TotalSkipped;
        public int TotalErrors => Source.TotalErrors;
        public string Message => Source.Message ?? string.Empty;

        public static ExecutionGridRow From(SyncProfileExecutionListItem source)
        {
            return new ExecutionGridRow(source);
        }
    }

    private sealed record ValidationResultRow(
        string Type,
        string Code,
        string? Field,
        SyncProfileEditorSection Section,
        string FieldOrSection,
        string Message)
    {
        public static ValidationResultRow From(string type, SyncValidationMessage source)
        {
            var section = SyncValidationSectionResolver.Resolve(source);
            return new ValidationResultRow(
                type,
                source.Code,
                source.Field,
                section,
                SyncValidationSectionResolver.DisplayName(section),
                source.Message);
        }
    }

    private sealed record BranchPreviewRow(
        int BranchCompanyId,
        string BranchCompanyCode,
        string BranchCode,
        string BranchCompanyName,
        string BranchDatabaseName,
        string StatusText,
        int BatchSize,
        int MaxRetries,
        DateTime? LastSynchronizationAt);

    private sealed record DistributionPreviewState(bool IsEnabled, int? BatchSize);

    private sealed record DistributionSelection(
        EntityPreviewRow Entity,
        BranchPreviewRow Branch,
        DistributionPreviewState State);

    private sealed class EntityPreviewRow
    {
        public string EntityCode { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public int ExecutionOrder { get; set; }
        public string SyncMode { get; set; } = "Incremental";
        public string? KeyField { get; set; }
        public string? ModifiedAtField { get; set; }
        public string? VersionField { get; set; }
        public string? ActiveField { get; set; }
        public bool AllowInsert { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDeactivate { get; set; }
        public bool ContinueOnError { get; set; }
        public int? BatchSize { get; set; }
        public bool IsActive { get; set; }
        public string StatusText => IsActive ? "Activo" : "Inactivo";

        public static EntityPreviewRow FromEditorRow(SyncProfileEntityEditorRow source)
        {
            return new EntityPreviewRow
            {
                EntityCode = source.EntityCode,
                EntityName = source.EntityName,
                ExecutionOrder = source.ExecutionOrder,
                SyncMode = source.SyncMode,
                KeyField = source.KeyField,
                ModifiedAtField = source.ModifiedAtField,
                VersionField = source.VersionField,
                ActiveField = source.ActiveField,
                AllowInsert = source.AllowInsert,
                AllowUpdate = source.AllowUpdate,
                AllowDeactivate = source.AllowDeactivate,
                ContinueOnError = source.ContinueOnError,
                BatchSize = source.BatchSize,
                IsActive = source.IsActive
            };
        }

        public static EntityPreviewRow FromResult(SyncProfileEntityDialogResult source)
        {
            return new EntityPreviewRow
            {
                EntityCode = source.EntityCode,
                EntityName = source.EntityName,
                ExecutionOrder = source.ExecutionOrder,
                SyncMode = source.SyncMode,
                KeyField = source.KeyField,
                ModifiedAtField = source.ModifiedAtField,
                VersionField = source.VersionField,
                ActiveField = source.ActiveField,
                AllowInsert = source.AllowInsert,
                AllowUpdate = source.AllowUpdate,
                AllowDeactivate = source.AllowDeactivate,
                ContinueOnError = source.ContinueOnError,
                BatchSize = source.BatchSize,
                IsActive = source.IsActive
            };
        }

        public SyncProfileEntityEditorRow ToEditorRow()
        {
            var target = new SyncProfileEntityEditorRow { EntityCode = EntityCode };
            CopyTo(target);
            return target;
        }

        public void CopyTo(SyncProfileEntityEditorRow target)
        {
            target.EntityCode = EntityCode;
            target.EntityName = EntityName;
            target.ExecutionOrder = ExecutionOrder;
            target.SyncMode = SyncMode;
            target.KeyField = KeyField;
            target.ModifiedAtField = ModifiedAtField;
            target.VersionField = VersionField;
            target.ActiveField = ActiveField;
            target.AllowInsert = AllowInsert;
            target.AllowUpdate = AllowUpdate;
            target.AllowDeactivate = AllowDeactivate;
            target.ContinueOnError = ContinueOnError;
            target.BatchSize = BatchSize;
            target.IsActive = IsActive;
        }
    }
}
