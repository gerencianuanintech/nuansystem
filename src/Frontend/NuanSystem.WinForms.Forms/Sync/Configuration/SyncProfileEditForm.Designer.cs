using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class SyncProfileEditForm
{
    private System.ComponentModel.IContainer components = null;
    private AccordionControl accordionNavigation;
    private AccordionControlElement aceGeneral;
    private AccordionControlElement aceBranches;
    private AccordionControlElement aceEntities;
    private AccordionControlElement aceDistribution;
    private AccordionControlElement aceSchedule;
    private AccordionControlElement aceValidation;
    private AccordionControlElement aceExecutions;
    private NavigationFrame navigationFrame;
    private NavigationPage pageGeneral;
    private NavigationPage pageBranches;
    private NavigationPage pageEntities;
    private NavigationPage pageDistribution;
    private NavigationPage pageSchedule;
    private NavigationPage pageValidation;
    private NavigationPage pageExecutions;
    private LabelControl lblGeneralTitle;
    private SeparatorControl sepGeneralTitle;
    private LabelControl lblMasterCompany;
    private TextEdit txtMasterCompany;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblDirection;
    private ComboBoxEdit cboDirection;
    private LabelControl lblExecutionMode;
    private ComboBoxEdit cboExecutionMode;
    private LabelControl lblConflictStrategy;
    private TextEdit txtConflictStrategy;
    private SeparatorControl sepExecutionParameters;
    private LabelControl lblBatchSize;
    private SpinEdit spnBatchSize;
    private LabelControl lblMaxRetries;
    private SpinEdit spnMaxRetries;
    private LabelControl lblRetryDelay;
    private SpinEdit spnRetryDelaySeconds;
    private LabelControl lblRetryDelayUnit;
    private LabelControl lblTimeout;
    private SpinEdit spnTimeoutMinutes;
    private LabelControl lblTimeoutUnit;
    private SeparatorControl sepStatus;
    private ToggleSwitch swIsActive;
    private PanelControl pnlBusinessPartnerCodePolicy;
    private LabelControl lblBusinessPartnerCodePolicyTitle;
    private ToggleSwitch swSapCodePolicyEnabled;
    private LabelControl lblSapCodePolicyEnabled;
    private ComboBoxEdit cboSapPrefixMode;
    private LabelControl lblSapPrefixMode;
    private TextEdit txtPassportIdentificationTypeCode;
    private LabelControl lblPassportIdentificationTypeCode;
    private LabelControl lblCustomerNationalExample;
    private LabelControl lblCustomerForeignExample;
    private LabelControl lblSupplierNationalExample;
    private LabelControl lblSupplierForeignExample;
    private LabelControl lblBranchesTitle;
    private SeparatorControl sepBranchesTitle;
    private NuanActionButton btnAddBranch;
    private NuanActionButton btnEditBranch;
    private NuanActionButton btnRemoveBranch;
    private NuanActionButton btnActivateBranch;
    private NuanActionButton btnDeactivateBranch;
    private NuanActionButton btnRefreshBranches;
    private NuanDataGridControl grdBranches;
    private GridColumn colBranchCompanyCode;
    private GridColumn colBranchCode;
    private GridColumn colBranchName;
    private GridColumn colBranchDatabaseName;
    private GridColumn colBranchStatus;
    private GridColumn colBranchBatchSize;
    private GridColumn colBranchMaxRetries;
    private GridColumn colBranchLastSynchronizationAt;
    private LabelControl lblBranchesTotal;
    private LabelControl lblEntitiesTitle;
    private SeparatorControl sepEntitiesTitle;
    private NuanActionButton btnAddEntity;
    private NuanActionButton btnEditEntity;
    private NuanActionButton btnRemoveEntity;
    private NuanActionButton btnMoveEntityUp;
    private NuanActionButton btnMoveEntityDown;
    private NuanActionButton btnActivateEntity;
    private NuanActionButton btnDeactivateEntity;
    private NuanDataGridControl grdEntities;
    private LabelControl lblEntitiesInfo;
    private LabelControl lblEntitiesTotal;
    private LabelControl lblDistributionTitle;
    private SeparatorControl sepDistributionTitle;
    private NuanActionButton btnEnableDistribution;
    private NuanActionButton btnDisableDistribution;
    private NuanActionButton btnConfigureDistributionBatch;
    private NuanActionButton btnEnableAllDistributions;
    private NuanActionButton btnDisableAllDistributions;
    private NuanActionButton btnRefreshDistribution;
    private NuanDataGridControl grdDistribution;
    private LabelControl lblDistributionInfo;
    private LabelControl lblScheduleTitle;
    private SeparatorControl sepScheduleTitle;
    private LabelControl lblScheduleConfigurationTitle;
    private SeparatorControl sepScheduleConfiguration;
    private LabelControl lblScheduleType;
    private ComboBoxEdit cboScheduleType;
    private LabelControl lblScheduleInterval;
    private SpinEdit spnScheduleIntervalMinutes;
    private LabelControl lblScheduleIntervalUnit;
    private LabelControl lblScheduleExecutionTime;
    private TimeEdit timScheduleExecutionTime;
    private LabelControl lblScheduleTimeZone;
    private ComboBoxEdit cboScheduleTimeZone;
    private LabelControl lblPreventConcurrentExecutions;
    private ToggleSwitch swPreventConcurrentExecutions;
    private LabelControl lblScheduleIsActive;
    private ToggleSwitch swScheduleIsActive;
    private LabelControl lblScheduleInfo;
    private LabelControl lblScheduleStatusTitle;
    private SeparatorControl sepScheduleStatus;
    private LabelControl lblScheduleNextExecution;
    private LabelControl lblScheduleNextExecutionValue;
    private LabelControl lblScheduleLastExecution;
    private LabelControl lblScheduleLastExecutionValue;
    private LabelControl lblScheduleEffectiveFrequency;
    private LabelControl lblScheduleEffectiveFrequencyValue;
    private LabelControl lblScheduleStatus;
    private LabelControl lblScheduleStatusValue;
    private LabelControl lblValidationTitle;
    private SeparatorControl sepValidationTitle;
    private NuanActionButton btnValidateProfile;
    private LabelControl lblValidationDescription;
    private LabelControl lblValidationSummarySurface;
    private LabelControl lblValidationResultCaption;
    private LabelControl lblValidationResultValue;
    private LabelControl lblValidationErrorsCaption;
    private LabelControl lblValidationErrorsValue;
    private LabelControl lblValidationWarningsCaption;
    private LabelControl lblValidationWarningsValue;
    private LabelControl lblValidationResultsTitle;
    private NuanDataGridControl grdValidationResults;
    private LabelControl lblValidationInfo;
    private LabelControl lblExecutionsTitle;
    private SeparatorControl sepExecutionsTitle;
    private NuanActionButton btnViewExecutionDetail;
    private NuanActionButton btnCancelExecution;
    private NuanActionButton btnRetryExecution;
    private NuanActionButton btnRefreshExecutions;
    private LabelControl lblExecutionsAutoRefresh;
    private LabelControl lblExecutionsHistoryTitle;
    private NuanDataGridControl grdExecutions;
    private SimpleButton btnExecutionsFirstPage;
    private SimpleButton btnExecutionsPreviousPage;
    private LabelControl lblExecutionsPageInfo;
    private SimpleButton btnExecutionsNextPage;
    private SimpleButton btnExecutionsLastPage;
    private LabelControl lblExecutionsTotal;
    private LabelControl lblExecutionsPageSize;
    private ComboBoxEdit cboExecutionsPageSize;
    private LabelControl lblExecutionsInfo;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        accordionNavigation = new AccordionControl();
        aceGeneral = new AccordionControlElement();
        aceBranches = new AccordionControlElement();
        aceEntities = new AccordionControlElement();
        aceDistribution = new AccordionControlElement();
        aceSchedule = new AccordionControlElement();
        aceValidation = new AccordionControlElement();
        aceExecutions = new AccordionControlElement();
        navigationFrame = new NavigationFrame();
        pageGeneral = new NavigationPage();
        lblGeneralTitle = new LabelControl();
        sepGeneralTitle = new SeparatorControl();
        lblMasterCompany = new LabelControl();
        txtMasterCompany = new TextEdit();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblDirection = new LabelControl();
        cboDirection = new ComboBoxEdit();
        lblExecutionMode = new LabelControl();
        cboExecutionMode = new ComboBoxEdit();
        lblConflictStrategy = new LabelControl();
        txtConflictStrategy = new TextEdit();
        sepExecutionParameters = new SeparatorControl();
        lblBatchSize = new LabelControl();
        spnBatchSize = new SpinEdit();
        lblMaxRetries = new LabelControl();
        spnMaxRetries = new SpinEdit();
        lblRetryDelay = new LabelControl();
        spnRetryDelaySeconds = new SpinEdit();
        lblRetryDelayUnit = new LabelControl();
        lblTimeout = new LabelControl();
        spnTimeoutMinutes = new SpinEdit();
        lblTimeoutUnit = new LabelControl();
        sepStatus = new SeparatorControl();
        swIsActive = new ToggleSwitch();
        pnlBusinessPartnerCodePolicy = new PanelControl();
        lblBusinessPartnerCodePolicyTitle = new LabelControl();
        swSapCodePolicyEnabled = new ToggleSwitch();
        lblSapCodePolicyEnabled = new LabelControl();
        cboSapPrefixMode = new ComboBoxEdit();
        lblSapPrefixMode = new LabelControl();
        txtPassportIdentificationTypeCode = new TextEdit();
        lblPassportIdentificationTypeCode = new LabelControl();
        lblCustomerNationalExample = new LabelControl();
        lblCustomerForeignExample = new LabelControl();
        lblSupplierNationalExample = new LabelControl();
        lblSupplierForeignExample = new LabelControl();
        pageBranches = new NavigationPage();
        lblBranchesTitle = new LabelControl();
        sepBranchesTitle = new SeparatorControl();
        btnAddBranch = new NuanActionButton();
        btnEditBranch = new NuanActionButton();
        btnRemoveBranch = new NuanActionButton();
        btnActivateBranch = new NuanActionButton();
        btnDeactivateBranch = new NuanActionButton();
        btnRefreshBranches = new NuanActionButton();
        grdBranches = new NuanDataGridControl();
        lblBranchesTotal = new LabelControl();
        pageEntities = new NavigationPage();
        lblEntitiesTitle = new LabelControl();
        sepEntitiesTitle = new SeparatorControl();
        btnAddEntity = new NuanActionButton();
        btnEditEntity = new NuanActionButton();
        btnRemoveEntity = new NuanActionButton();
        btnMoveEntityUp = new NuanActionButton();
        btnMoveEntityDown = new NuanActionButton();
        btnActivateEntity = new NuanActionButton();
        btnDeactivateEntity = new NuanActionButton();
        grdEntities = new NuanDataGridControl();
        lblEntitiesInfo = new LabelControl();
        lblEntitiesTotal = new LabelControl();
        pageDistribution = new NavigationPage();
        lblDistributionTitle = new LabelControl();
        sepDistributionTitle = new SeparatorControl();
        btnEnableDistribution = new NuanActionButton();
        btnDisableDistribution = new NuanActionButton();
        btnConfigureDistributionBatch = new NuanActionButton();
        btnEnableAllDistributions = new NuanActionButton();
        btnDisableAllDistributions = new NuanActionButton();
        btnRefreshDistribution = new NuanActionButton();
        grdDistribution = new NuanDataGridControl();
        lblDistributionInfo = new LabelControl();
        pageSchedule = new NavigationPage();
        lblScheduleTitle = new LabelControl();
        sepScheduleTitle = new SeparatorControl();
        lblScheduleConfigurationTitle = new LabelControl();
        sepScheduleConfiguration = new SeparatorControl();
        lblScheduleType = new LabelControl();
        cboScheduleType = new ComboBoxEdit();
        lblScheduleInterval = new LabelControl();
        spnScheduleIntervalMinutes = new SpinEdit();
        lblScheduleIntervalUnit = new LabelControl();
        lblScheduleExecutionTime = new LabelControl();
        timScheduleExecutionTime = new TimeEdit();
        lblScheduleTimeZone = new LabelControl();
        cboScheduleTimeZone = new ComboBoxEdit();
        lblPreventConcurrentExecutions = new LabelControl();
        swPreventConcurrentExecutions = new ToggleSwitch();
        lblScheduleIsActive = new LabelControl();
        swScheduleIsActive = new ToggleSwitch();
        lblScheduleInfo = new LabelControl();
        lblScheduleStatusTitle = new LabelControl();
        sepScheduleStatus = new SeparatorControl();
        lblScheduleNextExecution = new LabelControl();
        lblScheduleNextExecutionValue = new LabelControl();
        lblScheduleLastExecution = new LabelControl();
        lblScheduleLastExecutionValue = new LabelControl();
        lblScheduleEffectiveFrequency = new LabelControl();
        lblScheduleEffectiveFrequencyValue = new LabelControl();
        lblScheduleStatus = new LabelControl();
        lblScheduleStatusValue = new LabelControl();
        pageValidation = new NavigationPage();
        lblValidationTitle = new LabelControl();
        sepValidationTitle = new SeparatorControl();
        btnValidateProfile = new NuanActionButton();
        lblValidationDescription = new LabelControl();
        lblValidationResultCaption = new LabelControl();
        lblValidationResultValue = new LabelControl();
        lblValidationErrorsCaption = new LabelControl();
        lblValidationErrorsValue = new LabelControl();
        lblValidationWarningsCaption = new LabelControl();
        lblValidationWarningsValue = new LabelControl();
        lblValidationResultsTitle = new LabelControl();
        grdValidationResults = new NuanDataGridControl();
        lblValidationInfo = new LabelControl();
        lblValidationSummarySurface = new LabelControl();
        pageExecutions = new NavigationPage();
        lblExecutionsTitle = new LabelControl();
        sepExecutionsTitle = new SeparatorControl();
        btnViewExecutionDetail = new NuanActionButton();
        btnCancelExecution = new NuanActionButton();
        btnRetryExecution = new NuanActionButton();
        btnRefreshExecutions = new NuanActionButton();
        lblExecutionsAutoRefresh = new LabelControl();
        lblExecutionsHistoryTitle = new LabelControl();
        grdExecutions = new NuanDataGridControl();
        btnExecutionsFirstPage = new SimpleButton();
        btnExecutionsPreviousPage = new SimpleButton();
        lblExecutionsPageInfo = new LabelControl();
        btnExecutionsNextPage = new SimpleButton();
        btnExecutionsLastPage = new SimpleButton();
        lblExecutionsTotal = new LabelControl();
        lblExecutionsPageSize = new LabelControl();
        cboExecutionsPageSize = new ComboBoxEdit();
        lblExecutionsInfo = new LabelControl();
        colBranchCompanyCode = new GridColumn();
        colBranchCode = new GridColumn();
        colBranchName = new GridColumn();
        colBranchDatabaseName = new GridColumn();
        colBranchStatus = new GridColumn();
        colBranchBatchSize = new GridColumn();
        colBranchMaxRetries = new GridColumn();
        colBranchLastSynchronizationAt = new GridColumn();
        ((System.ComponentModel.ISupportInitialize)accordionNavigation).BeginInit();
        ((System.ComponentModel.ISupportInitialize)navigationFrame).BeginInit();
        navigationFrame.SuspendLayout();
        pageGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sepGeneralTitle).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtMasterCompany.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboDirection.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboExecutionMode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtConflictStrategy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepExecutionParameters).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnBatchSize.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxRetries.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnRetryDelaySeconds.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnTimeoutMinutes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepStatus).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlBusinessPartnerCodePolicy).BeginInit();
        pnlBusinessPartnerCodePolicy.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)swSapCodePolicyEnabled.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboSapPrefixMode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPassportIdentificationTypeCode.Properties).BeginInit();
        pageBranches.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sepBranchesTitle).BeginInit();
        pageEntities.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sepEntitiesTitle).BeginInit();
        pageDistribution.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sepDistributionTitle).BeginInit();
        pageSchedule.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sepScheduleTitle).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepScheduleConfiguration).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboScheduleType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnScheduleIntervalMinutes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)timScheduleExecutionTime.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboScheduleTimeZone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swPreventConcurrentExecutions.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swScheduleIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepScheduleStatus).BeginInit();
        pageValidation.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sepValidationTitle).BeginInit();
        pageExecutions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sepExecutionsTitle).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboExecutionsPageSize.Properties).BeginInit();
        SuspendLayout();
        //
        // btnCancelar
        //
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseBorderColor = true;
        btnCancelar.Appearance.Options.UseFont = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseBorderColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.ForeColor = Color.White;
        btnCancelar.AppearancePressed.Options.UseBackColor = true;
        btnCancelar.AppearancePressed.Options.UseBorderColor = true;
        btnCancelar.AppearancePressed.Options.UseForeColor = true;
        btnCancelar.ImageOptions.ImageToTextIndent = 0;
        btnCancelar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnCancelar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnCancelar.Location = new Point(939, 668);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancelar.TabIndex = 2;
        //
        // btnGuardar
        //
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseBorderColor = true;
        btnGuardar.Appearance.Options.UseFont = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseBorderColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.ForeColor = Color.White;
        btnGuardar.AppearancePressed.Options.UseBackColor = true;
        btnGuardar.AppearancePressed.Options.UseBorderColor = true;
        btnGuardar.AppearancePressed.Options.UseForeColor = true;
        btnGuardar.ImageOptions.ImageToTextIndent = 0;
        btnGuardar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnGuardar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnGuardar.Location = new Point(832, 668);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        //
        // accordionNavigation
        //
        accordionNavigation.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        accordionNavigation.Appearance.AccordionControl.BackColor = Color.White;
        accordionNavigation.Appearance.AccordionControl.Options.UseBackColor = true;
        accordionNavigation.Appearance.Item.Hovered.BackColor = Color.FromArgb(245, 247, 250);
        accordionNavigation.Appearance.Item.Hovered.ForeColor = Color.FromArgb(23, 32, 51);
        accordionNavigation.Appearance.Item.Hovered.Options.UseBackColor = true;
        accordionNavigation.Appearance.Item.Hovered.Options.UseForeColor = true;
        accordionNavigation.Appearance.Item.Normal.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        accordionNavigation.Appearance.Item.Normal.ForeColor = Color.FromArgb(23, 32, 51);
        accordionNavigation.Appearance.Item.Normal.Options.UseFont = true;
        accordionNavigation.Appearance.Item.Normal.Options.UseForeColor = true;
        accordionNavigation.Appearance.Item.Pressed.BackColor = Color.FromArgb(235, 250, 246);
        accordionNavigation.Appearance.Item.Pressed.ForeColor = Color.FromArgb(0, 160, 128);
        accordionNavigation.Appearance.Item.Pressed.Options.UseBackColor = true;
        accordionNavigation.Appearance.Item.Pressed.Options.UseForeColor = true;
        accordionNavigation.Elements.AddRange(new AccordionControlElement[] { aceGeneral, aceBranches, aceEntities, aceDistribution, aceSchedule, aceValidation, aceExecutions });
        accordionNavigation.Location = new Point(12, 12);
        accordionNavigation.Name = "accordionNavigation";
        accordionNavigation.ScrollBarMode = ScrollBarMode.Touch;
        accordionNavigation.Size = new Size(168, 637);
        accordionNavigation.TabIndex = 0;
        //
        // aceGeneral
        //
        aceGeneral.Name = "aceGeneral";
        aceGeneral.Style = ElementStyle.Item;
        aceGeneral.Text = "General";
        //
        // aceBranches
        //
        aceBranches.Name = "aceBranches";
        aceBranches.Style = ElementStyle.Item;
        aceBranches.Text = "Sucursales";
        //
        // aceEntities
        //
        aceEntities.Name = "aceEntities";
        aceEntities.Style = ElementStyle.Item;
        aceEntities.Text = "Entidades";
        //
        // aceDistribution
        //
        aceDistribution.Name = "aceDistribution";
        aceDistribution.Style = ElementStyle.Item;
        aceDistribution.Text = "Distribucion";
        //
        // aceSchedule
        //
        aceSchedule.Name = "aceSchedule";
        aceSchedule.Style = ElementStyle.Item;
        aceSchedule.Text = "Programacion";
        //
        // aceValidation
        //
        aceValidation.Name = "aceValidation";
        aceValidation.Style = ElementStyle.Item;
        aceValidation.Text = "Validacion";
        //
        // aceExecutions
        //
        aceExecutions.Name = "aceExecutions";
        aceExecutions.Style = ElementStyle.Item;
        aceExecutions.Text = "Ejecuciones";
        //
        // navigationFrame
        //
        navigationFrame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        navigationFrame.Appearance.BackColor = Color.White;
        navigationFrame.Appearance.Font = new Font("Segoe UI", 9F);
        navigationFrame.Appearance.Options.UseBackColor = true;
        navigationFrame.Appearance.Options.UseFont = true;
        navigationFrame.Controls.Add(pageGeneral);
        navigationFrame.Controls.Add(pageBranches);
        navigationFrame.Controls.Add(pageEntities);
        navigationFrame.Controls.Add(pageDistribution);
        navigationFrame.Controls.Add(pageSchedule);
        navigationFrame.Controls.Add(pageValidation);
        navigationFrame.Controls.Add(pageExecutions);
        navigationFrame.Location = new Point(188, 12);
        navigationFrame.Name = "navigationFrame";
        navigationFrame.Pages.AddRange(new NavigationPageBase[] { pageGeneral, pageBranches, pageEntities, pageDistribution, pageSchedule, pageValidation, pageExecutions });
        navigationFrame.SelectedPage = pageGeneral;
        navigationFrame.Size = new Size(851, 637);
        navigationFrame.TabIndex = 3;
        navigationFrame.Text = "navigationFrame";
        //
        // pageGeneral
        //
        pageGeneral.Caption = "General";
        pageGeneral.Controls.Add(lblGeneralTitle);
        pageGeneral.Controls.Add(sepGeneralTitle);
        pageGeneral.Controls.Add(lblMasterCompany);
        pageGeneral.Controls.Add(txtMasterCompany);
        pageGeneral.Controls.Add(lblCode);
        pageGeneral.Controls.Add(txtCode);
        pageGeneral.Controls.Add(lblName);
        pageGeneral.Controls.Add(txtName);
        pageGeneral.Controls.Add(lblDescription);
        pageGeneral.Controls.Add(memDescription);
        pageGeneral.Controls.Add(lblDirection);
        pageGeneral.Controls.Add(cboDirection);
        pageGeneral.Controls.Add(lblExecutionMode);
        pageGeneral.Controls.Add(cboExecutionMode);
        pageGeneral.Controls.Add(lblConflictStrategy);
        pageGeneral.Controls.Add(txtConflictStrategy);
        pageGeneral.Controls.Add(sepExecutionParameters);
        pageGeneral.Controls.Add(lblBatchSize);
        pageGeneral.Controls.Add(spnBatchSize);
        pageGeneral.Controls.Add(lblMaxRetries);
        pageGeneral.Controls.Add(spnMaxRetries);
        pageGeneral.Controls.Add(lblRetryDelay);
        pageGeneral.Controls.Add(spnRetryDelaySeconds);
        pageGeneral.Controls.Add(lblRetryDelayUnit);
        pageGeneral.Controls.Add(lblTimeout);
        pageGeneral.Controls.Add(spnTimeoutMinutes);
        pageGeneral.Controls.Add(lblTimeoutUnit);
        pageGeneral.Controls.Add(sepStatus);
        pageGeneral.Controls.Add(swIsActive);
        pageGeneral.Controls.Add(pnlBusinessPartnerCodePolicy);
        pageGeneral.Font = new Font("Segoe UI", 9F);
        pageGeneral.Name = "pageGeneral";
        pageGeneral.Size = new Size(851, 637);
        //
        // lblGeneralTitle
        //
        lblGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblGeneralTitle.Appearance.Options.UseFont = true;
        lblGeneralTitle.Appearance.Options.UseForeColor = true;
        lblGeneralTitle.Location = new Point(28, 22);
        lblGeneralTitle.Name = "lblGeneralTitle";
        lblGeneralTitle.Size = new Size(109, 20);
        lblGeneralTitle.TabIndex = 0;
        lblGeneralTitle.Text = "Datos generales";
        //
        // sepGeneralTitle
        //
        sepGeneralTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepGeneralTitle.LineColor = Color.FromArgb(0, 184, 148);
        sepGeneralTitle.Location = new Point(28, 54);
        sepGeneralTitle.Name = "sepGeneralTitle";
        sepGeneralTitle.Size = new Size(790, 18);
        sepGeneralTitle.TabIndex = 1;
        //
        // lblMasterCompany
        //
        lblMasterCompany.Appearance.Font = new Font("Segoe UI", 9F);
        lblMasterCompany.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblMasterCompany.Appearance.Options.UseFont = true;
        lblMasterCompany.Appearance.Options.UseForeColor = true;
        lblMasterCompany.Location = new Point(32, 92);
        lblMasterCompany.Name = "lblMasterCompany";
        lblMasterCompany.Size = new Size(90, 15);
        lblMasterCompany.TabIndex = 2;
        lblMasterCompany.Text = "Empresa maestra";
        //
        // txtMasterCompany
        //
        txtMasterCompany.EditValue = "NuanSystem S.A.";
        txtMasterCompany.Location = new Point(230, 88);
        txtMasterCompany.Name = "txtMasterCompany";
        txtMasterCompany.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtMasterCompany.Properties.Appearance.Options.UseFont = true;
        txtMasterCompany.Properties.ReadOnly = true;
        txtMasterCompany.Size = new Size(360, 22);
        txtMasterCompany.TabIndex = 3;
        //
        // lblCode
        //
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(32, 132);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(39, 15);
        lblCode.TabIndex = 4;
        lblCode.Text = "Codigo";
        //
        // txtCode
        //
        txtCode.EditValue = "SYNC-001";
        txtCode.Location = new Point(230, 128);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Size = new Size(360, 22);
        txtCode.TabIndex = 5;
        //
        // lblName
        //
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(32, 172);
        lblName.Name = "lblName";
        lblName.Size = new Size(27, 15);
        lblName.TabIndex = 6;
        lblName.Text = "Perfil";
        //
        // txtName
        //
        txtName.EditValue = "Clientes y Proveedores";
        txtName.Location = new Point(230, 168);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Size = new Size(360, 22);
        txtName.TabIndex = 7;
        //
        // lblDescription
        //
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(32, 212);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(62, 15);
        lblDescription.TabIndex = 8;
        lblDescription.Text = "Descripcion";
        //
        // memDescription
        //
        memDescription.EditValue = "Sincroniza informacion maestra desde la empresa principal hacia las sucursales.";
        memDescription.Location = new Point(230, 208);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Size = new Size(360, 84);
        memDescription.TabIndex = 9;
        //
        // lblDirection
        //
        lblDirection.Appearance.Font = new Font("Segoe UI", 9F);
        lblDirection.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDirection.Appearance.Options.UseFont = true;
        lblDirection.Appearance.Options.UseForeColor = true;
        lblDirection.Location = new Point(32, 306);
        lblDirection.Name = "lblDirection";
        lblDirection.Size = new Size(50, 15);
        lblDirection.TabIndex = 10;
        lblDirection.Text = "Direccion";
        //
        // cboDirection
        //
        cboDirection.EditValue = "Central origen → sucursales destino";
        cboDirection.Location = new Point(230, 302);
        cboDirection.Name = "cboDirection";
        cboDirection.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboDirection.Properties.Appearance.Options.UseFont = true;
        cboDirection.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboDirection.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboDirection.Size = new Size(360, 22);
        cboDirection.TabIndex = 11;
        //
        // lblExecutionMode
        //
        lblExecutionMode.Appearance.Font = new Font("Segoe UI", 9F);
        lblExecutionMode.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblExecutionMode.Appearance.Options.UseFont = true;
        lblExecutionMode.Appearance.Options.UseForeColor = true;
        lblExecutionMode.Location = new Point(32, 342);
        lblExecutionMode.Name = "lblExecutionMode";
        lblExecutionMode.Size = new Size(32, 15);
        lblExecutionMode.TabIndex = 12;
        lblExecutionMode.Text = "Modo";
        //
        // cboExecutionMode
        //
        cboExecutionMode.EditValue = "Full";
        cboExecutionMode.Location = new Point(230, 338);
        cboExecutionMode.Name = "cboExecutionMode";
        cboExecutionMode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboExecutionMode.Properties.Appearance.Options.UseFont = true;
        cboExecutionMode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboExecutionMode.Properties.Items.AddRange(new object[] { "Incremental", "Full", "Manual" });
        cboExecutionMode.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboExecutionMode.Size = new Size(360, 22);
        cboExecutionMode.TabIndex = 13;
        //
        // lblConflictStrategy
        //
        lblConflictStrategy.Appearance.Font = new Font("Segoe UI", 9F);
        lblConflictStrategy.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblConflictStrategy.Appearance.Options.UseFont = true;
        lblConflictStrategy.Appearance.Options.UseForeColor = true;
        lblConflictStrategy.Location = new Point(32, 378);
        lblConflictStrategy.Name = "lblConflictStrategy";
        lblConflictStrategy.Size = new Size(117, 15);
        lblConflictStrategy.TabIndex = 14;
        lblConflictStrategy.Text = "Estrategia de conflicto";
        //
        // txtConflictStrategy
        //
        txtConflictStrategy.EditValue = "MasterWins";
        txtConflictStrategy.Location = new Point(230, 374);
        txtConflictStrategy.Name = "txtConflictStrategy";
        txtConflictStrategy.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtConflictStrategy.Properties.Appearance.Options.UseFont = true;
        txtConflictStrategy.Properties.ReadOnly = true;
        txtConflictStrategy.Size = new Size(360, 22);
        txtConflictStrategy.TabIndex = 15;
        //
        // sepExecutionParameters
        //
        sepExecutionParameters.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepExecutionParameters.LineColor = Color.FromArgb(226, 232, 240);
        sepExecutionParameters.Location = new Point(28, 410);
        sepExecutionParameters.Name = "sepExecutionParameters";
        sepExecutionParameters.Size = new Size(790, 18);
        sepExecutionParameters.TabIndex = 16;
        //
        // lblBatchSize
        //
        lblBatchSize.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchSize.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBatchSize.Appearance.Options.UseFont = true;
        lblBatchSize.Appearance.Options.UseForeColor = true;
        lblBatchSize.Location = new Point(32, 438);
        lblBatchSize.Name = "lblBatchSize";
        lblBatchSize.Size = new Size(30, 15);
        lblBatchSize.TabIndex = 17;
        lblBatchSize.Text = "Batch";
        //
        // spnBatchSize
        //
        spnBatchSize.EditValue = new decimal(new int[] { 500, 0, 0, 0 });
        spnBatchSize.Location = new Point(230, 434);
        spnBatchSize.Name = "spnBatchSize";
        spnBatchSize.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnBatchSize.Properties.Appearance.Options.UseFont = true;
        spnBatchSize.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnBatchSize.Properties.IsFloatValue = false;
        spnBatchSize.Properties.MaskSettings.Set("mask", "N00");
        spnBatchSize.Properties.MaxValue = new decimal(new int[] { 10000, 0, 0, 0 });
        spnBatchSize.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        spnBatchSize.Size = new Size(120, 22);
        spnBatchSize.TabIndex = 18;
        //
        // lblMaxRetries
        //
        lblMaxRetries.Appearance.Font = new Font("Segoe UI", 9F);
        lblMaxRetries.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblMaxRetries.Appearance.Options.UseFont = true;
        lblMaxRetries.Appearance.Options.UseForeColor = true;
        lblMaxRetries.Location = new Point(32, 474);
        lblMaxRetries.Name = "lblMaxRetries";
        lblMaxRetries.Size = new Size(107, 15);
        lblMaxRetries.TabIndex = 19;
        lblMaxRetries.Text = "Reintentos maximos";
        //
        // spnMaxRetries
        //
        spnMaxRetries.EditValue = new decimal(new int[] { 3, 0, 0, 0 });
        spnMaxRetries.Location = new Point(230, 470);
        spnMaxRetries.Name = "spnMaxRetries";
        spnMaxRetries.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMaxRetries.Properties.Appearance.Options.UseFont = true;
        spnMaxRetries.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMaxRetries.Properties.IsFloatValue = false;
        spnMaxRetries.Properties.MaskSettings.Set("mask", "N00");
        spnMaxRetries.Properties.MaxValue = new decimal(new int[] { 10, 0, 0, 0 });
        spnMaxRetries.Size = new Size(120, 22);
        spnMaxRetries.TabIndex = 20;
        //
        // lblRetryDelay
        //
        lblRetryDelay.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetryDelay.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblRetryDelay.Appearance.Options.UseFont = true;
        lblRetryDelay.Appearance.Options.UseForeColor = true;
        lblRetryDelay.Location = new Point(32, 510);
        lblRetryDelay.Name = "lblRetryDelay";
        lblRetryDelay.Size = new Size(120, 15);
        lblRetryDelay.TabIndex = 21;
        lblRetryDelay.Text = "Espera entre reintentos";
        //
        // spnRetryDelaySeconds
        //
        spnRetryDelaySeconds.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        spnRetryDelaySeconds.Location = new Point(230, 506);
        spnRetryDelaySeconds.Name = "spnRetryDelaySeconds";
        spnRetryDelaySeconds.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnRetryDelaySeconds.Properties.Appearance.Options.UseFont = true;
        spnRetryDelaySeconds.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnRetryDelaySeconds.Properties.IsFloatValue = false;
        spnRetryDelaySeconds.Properties.MaskSettings.Set("mask", "N00");
        spnRetryDelaySeconds.Properties.MaxValue = new decimal(new int[] { 3600, 0, 0, 0 });
        spnRetryDelaySeconds.Size = new Size(120, 22);
        spnRetryDelaySeconds.TabIndex = 22;
        //
        // lblRetryDelayUnit
        //
        lblRetryDelayUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetryDelayUnit.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblRetryDelayUnit.Appearance.Options.UseFont = true;
        lblRetryDelayUnit.Appearance.Options.UseForeColor = true;
        lblRetryDelayUnit.Location = new Point(372, 510);
        lblRetryDelayUnit.Name = "lblRetryDelayUnit";
        lblRetryDelayUnit.Size = new Size(51, 15);
        lblRetryDelayUnit.TabIndex = 23;
        lblRetryDelayUnit.Text = "segundos";
        //
        // lblTimeout
        //
        lblTimeout.Appearance.Font = new Font("Segoe UI", 9F);
        lblTimeout.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTimeout.Appearance.Options.UseFont = true;
        lblTimeout.Appearance.Options.UseForeColor = true;
        lblTimeout.Location = new Point(32, 546);
        lblTimeout.Name = "lblTimeout";
        lblTimeout.Size = new Size(157, 15);
        lblTimeout.TabIndex = 24;
        lblTimeout.Text = "Tiempo maximo de ejecucion";
        //
        // spnTimeoutMinutes
        //
        spnTimeoutMinutes.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        spnTimeoutMinutes.Location = new Point(230, 542);
        spnTimeoutMinutes.Name = "spnTimeoutMinutes";
        spnTimeoutMinutes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnTimeoutMinutes.Properties.Appearance.Options.UseFont = true;
        spnTimeoutMinutes.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnTimeoutMinutes.Properties.IsFloatValue = false;
        spnTimeoutMinutes.Properties.MaskSettings.Set("mask", "N00");
        spnTimeoutMinutes.Properties.MaxValue = new decimal(new int[] { 1440, 0, 0, 0 });
        spnTimeoutMinutes.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        spnTimeoutMinutes.Size = new Size(120, 22);
        spnTimeoutMinutes.TabIndex = 25;
        //
        // lblTimeoutUnit
        //
        lblTimeoutUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblTimeoutUnit.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTimeoutUnit.Appearance.Options.UseFont = true;
        lblTimeoutUnit.Appearance.Options.UseForeColor = true;
        lblTimeoutUnit.Location = new Point(372, 546);
        lblTimeoutUnit.Name = "lblTimeoutUnit";
        lblTimeoutUnit.Size = new Size(44, 15);
        lblTimeoutUnit.TabIndex = 26;
        lblTimeoutUnit.Text = "minutos";
        //
        // sepStatus
        //
        sepStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepStatus.LineColor = Color.FromArgb(226, 232, 240);
        sepStatus.Location = new Point(28, 574);
        sepStatus.Name = "sepStatus";
        sepStatus.Size = new Size(790, 18);
        sepStatus.TabIndex = 27;
        //
        // swIsActive
        //
        swIsActive.EditValue = true;
        swIsActive.Location = new Point(633, 88);
        swIsActive.Name = "swIsActive";
        swIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swIsActive.Properties.Appearance.Options.UseFont = true;
        swIsActive.Properties.OffText = "Inactivo";
        swIsActive.Properties.OnText = "Activo";
        swIsActive.Size = new Size(160, 20);
        swIsActive.TabIndex = 29;
        //
        // pnlBusinessPartnerCodePolicy
        //
        pnlBusinessPartnerCodePolicy.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        pnlBusinessPartnerCodePolicy.Appearance.Options.UseBackColor = true;
        pnlBusinessPartnerCodePolicy.Controls.Add(lblBusinessPartnerCodePolicyTitle);
        pnlBusinessPartnerCodePolicy.Controls.Add(lblSapCodePolicyEnabled);
        pnlBusinessPartnerCodePolicy.Controls.Add(swSapCodePolicyEnabled);
        pnlBusinessPartnerCodePolicy.Controls.Add(lblSapPrefixMode);
        pnlBusinessPartnerCodePolicy.Controls.Add(cboSapPrefixMode);
        pnlBusinessPartnerCodePolicy.Controls.Add(lblPassportIdentificationTypeCode);
        pnlBusinessPartnerCodePolicy.Controls.Add(txtPassportIdentificationTypeCode);
        pnlBusinessPartnerCodePolicy.Controls.Add(lblCustomerNationalExample);
        pnlBusinessPartnerCodePolicy.Controls.Add(lblCustomerForeignExample);
        pnlBusinessPartnerCodePolicy.Controls.Add(lblSupplierNationalExample);
        pnlBusinessPartnerCodePolicy.Controls.Add(lblSupplierForeignExample);
        pnlBusinessPartnerCodePolicy.Location = new Point(610, 124);
        pnlBusinessPartnerCodePolicy.Name = "pnlBusinessPartnerCodePolicy";
        pnlBusinessPartnerCodePolicy.Padding = new Padding(14);
        pnlBusinessPartnerCodePolicy.Size = new Size(208, 426);
        pnlBusinessPartnerCodePolicy.TabIndex = 30;
        pnlBusinessPartnerCodePolicy.Visible = false;
        //
        // lblBusinessPartnerCodePolicyTitle
        //
        lblBusinessPartnerCodePolicyTitle.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblBusinessPartnerCodePolicyTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblBusinessPartnerCodePolicyTitle.Appearance.Options.UseFont = true;
        lblBusinessPartnerCodePolicyTitle.Appearance.Options.UseForeColor = true;
        lblBusinessPartnerCodePolicyTitle.Location = new Point(14, 14);
        lblBusinessPartnerCodePolicyTitle.Name = "lblBusinessPartnerCodePolicyTitle";
        lblBusinessPartnerCodePolicyTitle.Size = new Size(143, 17);
        lblBusinessPartnerCodePolicyTitle.TabIndex = 0;
        lblBusinessPartnerCodePolicyTitle.Text = "Códigos SAP de socios";
        //
        // lblSapCodePolicyEnabled
        //
        lblSapCodePolicyEnabled.Location = new Point(14, 48);
        lblSapCodePolicyEnabled.Name = "lblSapCodePolicyEnabled";
        lblSapCodePolicyEnabled.Size = new Size(43, 15);
        lblSapCodePolicyEnabled.TabIndex = 1;
        lblSapCodePolicyEnabled.Text = "Política";
        //
        // swSapCodePolicyEnabled
        //
        swSapCodePolicyEnabled.Location = new Point(82, 45);
        swSapCodePolicyEnabled.Name = "swSapCodePolicyEnabled";
        swSapCodePolicyEnabled.Properties.OffText = "Inactiva";
        swSapCodePolicyEnabled.Properties.OnText = "Activa";
        swSapCodePolicyEnabled.Size = new Size(110, 20);
        swSapCodePolicyEnabled.TabIndex = 2;
        //
        // lblSapPrefixMode
        //
        lblSapPrefixMode.Location = new Point(14, 82);
        lblSapPrefixMode.Name = "lblSapPrefixMode";
        lblSapPrefixMode.Size = new Size(89, 15);
        lblSapPrefixMode.TabIndex = 3;
        lblSapPrefixMode.Text = "Modo de prefijo";
        //
        // cboSapPrefixMode
        //
        cboSapPrefixMode.Location = new Point(14, 103);
        cboSapPrefixMode.Name = "cboSapPrefixMode";
        cboSapPrefixMode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboSapPrefixMode.Properties.Items.AddRange(new object[] { "NationalForeign", "RoleOnly" });
        cboSapPrefixMode.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboSapPrefixMode.Size = new Size(178, 22);
        cboSapPrefixMode.TabIndex = 4;
        //
        // lblPassportIdentificationTypeCode
        //
        lblPassportIdentificationTypeCode.Location = new Point(14, 140);
        lblPassportIdentificationTypeCode.Name = "lblPassportIdentificationTypeCode";
        lblPassportIdentificationTypeCode.Size = new Size(127, 15);
        lblPassportIdentificationTypeCode.TabIndex = 5;
        lblPassportIdentificationTypeCode.Text = "Tipo ID para extranjero";
        //
        // txtPassportIdentificationTypeCode
        //
        txtPassportIdentificationTypeCode.Location = new Point(14, 161);
        txtPassportIdentificationTypeCode.Name = "txtPassportIdentificationTypeCode";
        txtPassportIdentificationTypeCode.Size = new Size(178, 22);
        txtPassportIdentificationTypeCode.TabIndex = 6;
        //
        // lblCustomerNationalExample
        //
        lblCustomerNationalExample.AutoSizeMode = LabelAutoSizeMode.None;
        lblCustomerNationalExample.Location = new Point(14, 200);
        lblCustomerNationalExample.Name = "lblCustomerNationalExample";
        lblCustomerNationalExample.Size = new Size(178, 36);
        lblCustomerNationalExample.TabIndex = 7;
        lblCustomerNationalExample.Text = "Cliente nacional: —";
        //
        // lblCustomerForeignExample
        //
        lblCustomerForeignExample.AutoSizeMode = LabelAutoSizeMode.None;
        lblCustomerForeignExample.Location = new Point(14, 242);
        lblCustomerForeignExample.Name = "lblCustomerForeignExample";
        lblCustomerForeignExample.Size = new Size(178, 36);
        lblCustomerForeignExample.TabIndex = 8;
        lblCustomerForeignExample.Text = "Cliente extranjero: —";
        //
        // lblSupplierNationalExample
        //
        lblSupplierNationalExample.AutoSizeMode = LabelAutoSizeMode.None;
        lblSupplierNationalExample.Location = new Point(14, 284);
        lblSupplierNationalExample.Name = "lblSupplierNationalExample";
        lblSupplierNationalExample.Size = new Size(178, 36);
        lblSupplierNationalExample.TabIndex = 9;
        lblSupplierNationalExample.Text = "Proveedor nacional: —";
        //
        // lblSupplierForeignExample
        //
        lblSupplierForeignExample.AutoSizeMode = LabelAutoSizeMode.None;
        lblSupplierForeignExample.Location = new Point(14, 326);
        lblSupplierForeignExample.Name = "lblSupplierForeignExample";
        lblSupplierForeignExample.Size = new Size(178, 50);
        lblSupplierForeignExample.TabIndex = 10;
        lblSupplierForeignExample.Text = "Proveedor extranjero: —";
        //
        // pageBranches
        //
        pageBranches.Caption = "Sucursales";
        pageBranches.Controls.Add(lblBranchesTitle);
        pageBranches.Controls.Add(sepBranchesTitle);
        pageBranches.Controls.Add(btnAddBranch);
        pageBranches.Controls.Add(btnEditBranch);
        pageBranches.Controls.Add(btnRemoveBranch);
        pageBranches.Controls.Add(btnActivateBranch);
        pageBranches.Controls.Add(btnDeactivateBranch);
        pageBranches.Controls.Add(btnRefreshBranches);
        pageBranches.Controls.Add(grdBranches);
        pageBranches.Controls.Add(lblBranchesTotal);
        pageBranches.Font = new Font("Segoe UI", 9F);
        pageBranches.Name = "pageBranches";
        pageBranches.Size = new Size(851, 637);
        //
        // lblBranchesTitle
        //
        lblBranchesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblBranchesTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblBranchesTitle.Appearance.Options.UseFont = true;
        lblBranchesTitle.Appearance.Options.UseForeColor = true;
        lblBranchesTitle.Location = new Point(28, 22);
        lblBranchesTitle.Name = "lblBranchesTitle";
        lblBranchesTitle.Size = new Size(125, 20);
        lblBranchesTitle.TabIndex = 0;
        lblBranchesTitle.Text = "Sucursales destino";
        //
        // sepBranchesTitle
        //
        sepBranchesTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepBranchesTitle.LineColor = Color.FromArgb(0, 184, 148);
        sepBranchesTitle.Location = new Point(28, 54);
        sepBranchesTitle.Name = "sepBranchesTitle";
        sepBranchesTitle.Size = new Size(790, 18);
        sepBranchesTitle.TabIndex = 1;
        //
        // btnAddBranch
        //
        btnAddBranch.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnAddBranch.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnAddBranch.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddBranch.Appearance.ForeColor = Color.White;
        btnAddBranch.Appearance.Options.UseBackColor = true;
        btnAddBranch.Appearance.Options.UseBorderColor = true;
        btnAddBranch.Appearance.Options.UseFont = true;
        btnAddBranch.Appearance.Options.UseForeColor = true;
        btnAddBranch.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnAddBranch.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnAddBranch.AppearanceHovered.ForeColor = Color.White;
        btnAddBranch.AppearanceHovered.Options.UseBackColor = true;
        btnAddBranch.AppearanceHovered.Options.UseBorderColor = true;
        btnAddBranch.AppearanceHovered.Options.UseForeColor = true;
        btnAddBranch.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnAddBranch.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnAddBranch.AppearancePressed.ForeColor = Color.White;
        btnAddBranch.AppearancePressed.Options.UseBackColor = true;
        btnAddBranch.AppearancePressed.Options.UseBorderColor = true;
        btnAddBranch.AppearancePressed.Options.UseForeColor = true;
        btnAddBranch.ButtonKind = NuanActionButtonKind.Save;
        btnAddBranch.ButtonStyle = BorderStyles.UltraFlat;
        btnAddBranch.ButtonText = "Agregar";
        btnAddBranch.IconNameOverride = "agregar_16.svg";
        btnAddBranch.IconSize = 16;
        btnAddBranch.ImageOptions.ImageToTextIndent = 0;
        btnAddBranch.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnAddBranch.ImageOptions.SvgImageSize = new Size(16, 16);
        btnAddBranch.Location = new Point(28, 88);
        btnAddBranch.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddBranch.LookAndFeel.UseDefaultLookAndFeel = false;
        btnAddBranch.Name = "btnAddBranch";
        btnAddBranch.Size = new Size(100, 26);
        btnAddBranch.TabIndex = 2;
        btnAddBranch.Text = "Agregar";
        //
        // btnEditBranch
        //
        btnEditBranch.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnEditBranch.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnEditBranch.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditBranch.Appearance.ForeColor = Color.White;
        btnEditBranch.Appearance.Options.UseBackColor = true;
        btnEditBranch.Appearance.Options.UseBorderColor = true;
        btnEditBranch.Appearance.Options.UseFont = true;
        btnEditBranch.Appearance.Options.UseForeColor = true;
        btnEditBranch.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnEditBranch.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnEditBranch.AppearanceHovered.ForeColor = Color.White;
        btnEditBranch.AppearanceHovered.Options.UseBackColor = true;
        btnEditBranch.AppearanceHovered.Options.UseBorderColor = true;
        btnEditBranch.AppearanceHovered.Options.UseForeColor = true;
        btnEditBranch.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnEditBranch.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnEditBranch.AppearancePressed.ForeColor = Color.White;
        btnEditBranch.AppearancePressed.Options.UseBackColor = true;
        btnEditBranch.AppearancePressed.Options.UseBorderColor = true;
        btnEditBranch.AppearancePressed.Options.UseForeColor = true;
        btnEditBranch.ButtonKind = NuanActionButtonKind.Save;
        btnEditBranch.ButtonStyle = BorderStyles.UltraFlat;
        btnEditBranch.ButtonText = "Editar";
        btnEditBranch.IconNameOverride = "editar_16.svg";
        btnEditBranch.IconSize = 16;
        btnEditBranch.ImageOptions.ImageToTextIndent = 0;
        btnEditBranch.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnEditBranch.ImageOptions.SvgImageSize = new Size(16, 16);
        btnEditBranch.Location = new Point(134, 88);
        btnEditBranch.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnEditBranch.LookAndFeel.UseDefaultLookAndFeel = false;
        btnEditBranch.Name = "btnEditBranch";
        btnEditBranch.Size = new Size(100, 26);
        btnEditBranch.TabIndex = 3;
        btnEditBranch.Text = "Editar";
        //
        // btnRemoveBranch
        //
        btnRemoveBranch.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnRemoveBranch.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnRemoveBranch.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemoveBranch.Appearance.ForeColor = Color.White;
        btnRemoveBranch.Appearance.Options.UseBackColor = true;
        btnRemoveBranch.Appearance.Options.UseBorderColor = true;
        btnRemoveBranch.Appearance.Options.UseFont = true;
        btnRemoveBranch.Appearance.Options.UseForeColor = true;
        btnRemoveBranch.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnRemoveBranch.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnRemoveBranch.AppearanceHovered.ForeColor = Color.White;
        btnRemoveBranch.AppearanceHovered.Options.UseBackColor = true;
        btnRemoveBranch.AppearanceHovered.Options.UseBorderColor = true;
        btnRemoveBranch.AppearanceHovered.Options.UseForeColor = true;
        btnRemoveBranch.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnRemoveBranch.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnRemoveBranch.AppearancePressed.ForeColor = Color.White;
        btnRemoveBranch.AppearancePressed.Options.UseBackColor = true;
        btnRemoveBranch.AppearancePressed.Options.UseBorderColor = true;
        btnRemoveBranch.AppearancePressed.Options.UseForeColor = true;
        btnRemoveBranch.ButtonKind = NuanActionButtonKind.Save;
        btnRemoveBranch.ButtonStyle = BorderStyles.UltraFlat;
        btnRemoveBranch.ButtonText = "Quitar";
        btnRemoveBranch.IconNameOverride = "quitar_16.svg";
        btnRemoveBranch.IconSize = 16;
        btnRemoveBranch.ImageOptions.ImageToTextIndent = 0;
        btnRemoveBranch.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnRemoveBranch.ImageOptions.SvgImageSize = new Size(16, 16);
        btnRemoveBranch.Location = new Point(240, 88);
        btnRemoveBranch.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnRemoveBranch.LookAndFeel.UseDefaultLookAndFeel = false;
        btnRemoveBranch.Name = "btnRemoveBranch";
        btnRemoveBranch.Size = new Size(100, 26);
        btnRemoveBranch.TabIndex = 4;
        btnRemoveBranch.Text = "Quitar";
        //
        // btnActivateBranch
        //
        btnActivateBranch.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnActivateBranch.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnActivateBranch.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnActivateBranch.Appearance.ForeColor = Color.White;
        btnActivateBranch.Appearance.Options.UseBackColor = true;
        btnActivateBranch.Appearance.Options.UseBorderColor = true;
        btnActivateBranch.Appearance.Options.UseFont = true;
        btnActivateBranch.Appearance.Options.UseForeColor = true;
        btnActivateBranch.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnActivateBranch.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnActivateBranch.AppearanceHovered.ForeColor = Color.White;
        btnActivateBranch.AppearanceHovered.Options.UseBackColor = true;
        btnActivateBranch.AppearanceHovered.Options.UseBorderColor = true;
        btnActivateBranch.AppearanceHovered.Options.UseForeColor = true;
        btnActivateBranch.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnActivateBranch.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnActivateBranch.AppearancePressed.ForeColor = Color.White;
        btnActivateBranch.AppearancePressed.Options.UseBackColor = true;
        btnActivateBranch.AppearancePressed.Options.UseBorderColor = true;
        btnActivateBranch.AppearancePressed.Options.UseForeColor = true;
        btnActivateBranch.ButtonKind = NuanActionButtonKind.Save;
        btnActivateBranch.ButtonStyle = BorderStyles.UltraFlat;
        btnActivateBranch.ButtonText = "Activar";
        btnActivateBranch.IconNameOverride = "aprobar_16.svg";
        btnActivateBranch.IconSize = 16;
        btnActivateBranch.ImageOptions.ImageToTextIndent = 0;
        btnActivateBranch.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnActivateBranch.ImageOptions.SvgImageSize = new Size(16, 16);
        btnActivateBranch.Location = new Point(346, 88);
        btnActivateBranch.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnActivateBranch.LookAndFeel.UseDefaultLookAndFeel = false;
        btnActivateBranch.Name = "btnActivateBranch";
        btnActivateBranch.Size = new Size(100, 26);
        btnActivateBranch.TabIndex = 5;
        btnActivateBranch.Text = "Activar";
        //
        // btnDeactivateBranch
        //
        btnDeactivateBranch.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnDeactivateBranch.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnDeactivateBranch.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeactivateBranch.Appearance.ForeColor = Color.White;
        btnDeactivateBranch.Appearance.Options.UseBackColor = true;
        btnDeactivateBranch.Appearance.Options.UseBorderColor = true;
        btnDeactivateBranch.Appearance.Options.UseFont = true;
        btnDeactivateBranch.Appearance.Options.UseForeColor = true;
        btnDeactivateBranch.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnDeactivateBranch.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnDeactivateBranch.AppearanceHovered.ForeColor = Color.White;
        btnDeactivateBranch.AppearanceHovered.Options.UseBackColor = true;
        btnDeactivateBranch.AppearanceHovered.Options.UseBorderColor = true;
        btnDeactivateBranch.AppearanceHovered.Options.UseForeColor = true;
        btnDeactivateBranch.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnDeactivateBranch.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnDeactivateBranch.AppearancePressed.ForeColor = Color.White;
        btnDeactivateBranch.AppearancePressed.Options.UseBackColor = true;
        btnDeactivateBranch.AppearancePressed.Options.UseBorderColor = true;
        btnDeactivateBranch.AppearancePressed.Options.UseForeColor = true;
        btnDeactivateBranch.ButtonKind = NuanActionButtonKind.Save;
        btnDeactivateBranch.ButtonStyle = BorderStyles.UltraFlat;
        btnDeactivateBranch.ButtonText = "Desactivar";
        btnDeactivateBranch.IconNameOverride = "rechazar_16.svg";
        btnDeactivateBranch.IconSize = 16;
        btnDeactivateBranch.ImageOptions.ImageToTextIndent = 0;
        btnDeactivateBranch.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnDeactivateBranch.ImageOptions.SvgImageSize = new Size(16, 16);
        btnDeactivateBranch.Location = new Point(452, 88);
        btnDeactivateBranch.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnDeactivateBranch.LookAndFeel.UseDefaultLookAndFeel = false;
        btnDeactivateBranch.Name = "btnDeactivateBranch";
        btnDeactivateBranch.Size = new Size(100, 26);
        btnDeactivateBranch.TabIndex = 6;
        btnDeactivateBranch.Text = "Desactivar";
        //
        // btnRefreshBranches
        //
        btnRefreshBranches.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnRefreshBranches.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnRefreshBranches.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRefreshBranches.Appearance.ForeColor = Color.White;
        btnRefreshBranches.Appearance.Options.UseBackColor = true;
        btnRefreshBranches.Appearance.Options.UseBorderColor = true;
        btnRefreshBranches.Appearance.Options.UseFont = true;
        btnRefreshBranches.Appearance.Options.UseForeColor = true;
        btnRefreshBranches.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnRefreshBranches.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnRefreshBranches.AppearanceHovered.ForeColor = Color.White;
        btnRefreshBranches.AppearanceHovered.Options.UseBackColor = true;
        btnRefreshBranches.AppearanceHovered.Options.UseBorderColor = true;
        btnRefreshBranches.AppearanceHovered.Options.UseForeColor = true;
        btnRefreshBranches.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnRefreshBranches.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnRefreshBranches.AppearancePressed.ForeColor = Color.White;
        btnRefreshBranches.AppearancePressed.Options.UseBackColor = true;
        btnRefreshBranches.AppearancePressed.Options.UseBorderColor = true;
        btnRefreshBranches.AppearancePressed.Options.UseForeColor = true;
        btnRefreshBranches.ButtonKind = NuanActionButtonKind.Save;
        btnRefreshBranches.ButtonStyle = BorderStyles.UltraFlat;
        btnRefreshBranches.ButtonText = "Refrescar";
        btnRefreshBranches.IconNameOverride = "actualizar_16.svg";
        btnRefreshBranches.IconSize = 16;
        btnRefreshBranches.ImageOptions.ImageToTextIndent = 0;
        btnRefreshBranches.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnRefreshBranches.ImageOptions.SvgImageSize = new Size(16, 16);
        btnRefreshBranches.Location = new Point(558, 88);
        btnRefreshBranches.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnRefreshBranches.LookAndFeel.UseDefaultLookAndFeel = false;
        btnRefreshBranches.Name = "btnRefreshBranches";
        btnRefreshBranches.Size = new Size(100, 26);
        btnRefreshBranches.TabIndex = 7;
        btnRefreshBranches.Text = "Refrescar";
        //
        // grdBranches
        //
        grdBranches.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdBranches.FormKey = "sync-profile-branches";
        grdBranches.GridName = "BranchesGrid";
        grdBranches.Location = new Point(28, 140);
        grdBranches.Name = "grdBranches";
        grdBranches.ShowPagination = false;
        grdBranches.Size = new Size(790, 418);
        grdBranches.TabIndex = 8;
        //
        // lblBranchesTotal
        //
        lblBranchesTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblBranchesTotal.Appearance.Font = new Font("Segoe UI", 9F);
        lblBranchesTotal.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBranchesTotal.Appearance.Options.UseFont = true;
        lblBranchesTotal.Appearance.Options.UseForeColor = true;
        lblBranchesTotal.Location = new Point(28, 584);
        lblBranchesTotal.Name = "lblBranchesTotal";
        lblBranchesTotal.Size = new Size(96, 15);
        lblBranchesTotal.TabIndex = 10;
        lblBranchesTotal.Text = "Total: 0 sucursales";
        //
        // pageEntities
        //
        pageEntities.Caption = "Entidades";
        pageEntities.Controls.Add(lblEntitiesTitle);
        pageEntities.Controls.Add(sepEntitiesTitle);
        pageEntities.Controls.Add(btnAddEntity);
        pageEntities.Controls.Add(btnEditEntity);
        pageEntities.Controls.Add(btnRemoveEntity);
        pageEntities.Controls.Add(btnMoveEntityUp);
        pageEntities.Controls.Add(btnMoveEntityDown);
        pageEntities.Controls.Add(btnActivateEntity);
        pageEntities.Controls.Add(btnDeactivateEntity);
        pageEntities.Controls.Add(grdEntities);
        pageEntities.Controls.Add(lblEntitiesInfo);
        pageEntities.Controls.Add(lblEntitiesTotal);
        pageEntities.Font = new Font("Segoe UI", 9F);
        pageEntities.Name = "pageEntities";
        pageEntities.Size = new Size(851, 637);
        //
        // lblEntitiesTitle
        //
        lblEntitiesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblEntitiesTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblEntitiesTitle.Appearance.Options.UseFont = true;
        lblEntitiesTitle.Appearance.Options.UseForeColor = true;
        lblEntitiesTitle.Location = new Point(28, 22);
        lblEntitiesTitle.Name = "lblEntitiesTitle";
        lblEntitiesTitle.Size = new Size(159, 20);
        lblEntitiesTitle.TabIndex = 0;
        lblEntitiesTitle.Text = "Entidades configuradas";
        //
        // sepEntitiesTitle
        //
        sepEntitiesTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepEntitiesTitle.LineColor = Color.FromArgb(0, 184, 148);
        sepEntitiesTitle.Location = new Point(28, 54);
        sepEntitiesTitle.Name = "sepEntitiesTitle";
        sepEntitiesTitle.Size = new Size(790, 18);
        sepEntitiesTitle.TabIndex = 2;
        //
        // btnAddEntity
        //
        btnAddEntity.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnAddEntity.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnAddEntity.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddEntity.Appearance.ForeColor = Color.White;
        btnAddEntity.Appearance.Options.UseBackColor = true;
        btnAddEntity.Appearance.Options.UseBorderColor = true;
        btnAddEntity.Appearance.Options.UseFont = true;
        btnAddEntity.Appearance.Options.UseForeColor = true;
        btnAddEntity.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnAddEntity.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnAddEntity.AppearanceHovered.ForeColor = Color.White;
        btnAddEntity.AppearanceHovered.Options.UseBackColor = true;
        btnAddEntity.AppearanceHovered.Options.UseBorderColor = true;
        btnAddEntity.AppearanceHovered.Options.UseForeColor = true;
        btnAddEntity.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnAddEntity.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnAddEntity.AppearancePressed.ForeColor = Color.White;
        btnAddEntity.AppearancePressed.Options.UseBackColor = true;
        btnAddEntity.AppearancePressed.Options.UseBorderColor = true;
        btnAddEntity.AppearancePressed.Options.UseForeColor = true;
        btnAddEntity.ButtonKind = NuanActionButtonKind.Save;
        btnAddEntity.ButtonStyle = BorderStyles.UltraFlat;
        btnAddEntity.ButtonText = "Agregar";
        btnAddEntity.IconNameOverride = "agregar_16.svg";
        btnAddEntity.IconSize = 16;
        btnAddEntity.ImageOptions.ImageToTextIndent = 0;
        btnAddEntity.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnAddEntity.ImageOptions.SvgImageSize = new Size(16, 16);
        btnAddEntity.Location = new Point(28, 88);
        btnAddEntity.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddEntity.LookAndFeel.UseDefaultLookAndFeel = false;
        btnAddEntity.Name = "btnAddEntity";
        btnAddEntity.Size = new Size(100, 26);
        btnAddEntity.TabIndex = 3;
        btnAddEntity.Text = "Agregar";
        //
        // btnEditEntity
        //
        btnEditEntity.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnEditEntity.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnEditEntity.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditEntity.Appearance.ForeColor = Color.White;
        btnEditEntity.Appearance.Options.UseBackColor = true;
        btnEditEntity.Appearance.Options.UseBorderColor = true;
        btnEditEntity.Appearance.Options.UseFont = true;
        btnEditEntity.Appearance.Options.UseForeColor = true;
        btnEditEntity.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnEditEntity.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnEditEntity.AppearanceHovered.ForeColor = Color.White;
        btnEditEntity.AppearanceHovered.Options.UseBackColor = true;
        btnEditEntity.AppearanceHovered.Options.UseBorderColor = true;
        btnEditEntity.AppearanceHovered.Options.UseForeColor = true;
        btnEditEntity.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnEditEntity.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnEditEntity.AppearancePressed.ForeColor = Color.White;
        btnEditEntity.AppearancePressed.Options.UseBackColor = true;
        btnEditEntity.AppearancePressed.Options.UseBorderColor = true;
        btnEditEntity.AppearancePressed.Options.UseForeColor = true;
        btnEditEntity.ButtonKind = NuanActionButtonKind.Save;
        btnEditEntity.ButtonStyle = BorderStyles.UltraFlat;
        btnEditEntity.ButtonText = "Editar";
        btnEditEntity.IconNameOverride = "editar_16.svg";
        btnEditEntity.IconSize = 16;
        btnEditEntity.ImageOptions.ImageToTextIndent = 0;
        btnEditEntity.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnEditEntity.ImageOptions.SvgImageSize = new Size(16, 16);
        btnEditEntity.Location = new Point(134, 88);
        btnEditEntity.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnEditEntity.LookAndFeel.UseDefaultLookAndFeel = false;
        btnEditEntity.Name = "btnEditEntity";
        btnEditEntity.Size = new Size(100, 26);
        btnEditEntity.TabIndex = 4;
        btnEditEntity.Text = "Editar";
        //
        // btnRemoveEntity
        //
        btnRemoveEntity.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnRemoveEntity.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnRemoveEntity.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemoveEntity.Appearance.ForeColor = Color.White;
        btnRemoveEntity.Appearance.Options.UseBackColor = true;
        btnRemoveEntity.Appearance.Options.UseBorderColor = true;
        btnRemoveEntity.Appearance.Options.UseFont = true;
        btnRemoveEntity.Appearance.Options.UseForeColor = true;
        btnRemoveEntity.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnRemoveEntity.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnRemoveEntity.AppearanceHovered.ForeColor = Color.White;
        btnRemoveEntity.AppearanceHovered.Options.UseBackColor = true;
        btnRemoveEntity.AppearanceHovered.Options.UseBorderColor = true;
        btnRemoveEntity.AppearanceHovered.Options.UseForeColor = true;
        btnRemoveEntity.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnRemoveEntity.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnRemoveEntity.AppearancePressed.ForeColor = Color.White;
        btnRemoveEntity.AppearancePressed.Options.UseBackColor = true;
        btnRemoveEntity.AppearancePressed.Options.UseBorderColor = true;
        btnRemoveEntity.AppearancePressed.Options.UseForeColor = true;
        btnRemoveEntity.ButtonKind = NuanActionButtonKind.Save;
        btnRemoveEntity.ButtonStyle = BorderStyles.UltraFlat;
        btnRemoveEntity.ButtonText = "Quitar";
        btnRemoveEntity.IconNameOverride = "quitar_16.svg";
        btnRemoveEntity.IconSize = 16;
        btnRemoveEntity.ImageOptions.ImageToTextIndent = 0;
        btnRemoveEntity.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnRemoveEntity.ImageOptions.SvgImageSize = new Size(16, 16);
        btnRemoveEntity.Location = new Point(240, 88);
        btnRemoveEntity.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnRemoveEntity.LookAndFeel.UseDefaultLookAndFeel = false;
        btnRemoveEntity.Name = "btnRemoveEntity";
        btnRemoveEntity.Size = new Size(100, 26);
        btnRemoveEntity.TabIndex = 5;
        btnRemoveEntity.Text = "Quitar";
        //
        // btnMoveEntityUp
        //
        btnMoveEntityUp.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnMoveEntityUp.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnMoveEntityUp.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnMoveEntityUp.Appearance.ForeColor = Color.White;
        btnMoveEntityUp.Appearance.Options.UseBackColor = true;
        btnMoveEntityUp.Appearance.Options.UseBorderColor = true;
        btnMoveEntityUp.Appearance.Options.UseFont = true;
        btnMoveEntityUp.Appearance.Options.UseForeColor = true;
        btnMoveEntityUp.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnMoveEntityUp.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnMoveEntityUp.AppearanceHovered.ForeColor = Color.White;
        btnMoveEntityUp.AppearanceHovered.Options.UseBackColor = true;
        btnMoveEntityUp.AppearanceHovered.Options.UseBorderColor = true;
        btnMoveEntityUp.AppearanceHovered.Options.UseForeColor = true;
        btnMoveEntityUp.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnMoveEntityUp.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnMoveEntityUp.AppearancePressed.ForeColor = Color.White;
        btnMoveEntityUp.AppearancePressed.Options.UseBackColor = true;
        btnMoveEntityUp.AppearancePressed.Options.UseBorderColor = true;
        btnMoveEntityUp.AppearancePressed.Options.UseForeColor = true;
        btnMoveEntityUp.ButtonKind = NuanActionButtonKind.Save;
        btnMoveEntityUp.ButtonStyle = BorderStyles.UltraFlat;
        btnMoveEntityUp.ButtonText = "Subir";
        btnMoveEntityUp.IconNameOverride = "exportar_16.svg";
        btnMoveEntityUp.IconSize = 16;
        btnMoveEntityUp.ImageOptions.ImageToTextIndent = 0;
        btnMoveEntityUp.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnMoveEntityUp.ImageOptions.SvgImageSize = new Size(16, 16);
        btnMoveEntityUp.Location = new Point(346, 88);
        btnMoveEntityUp.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnMoveEntityUp.LookAndFeel.UseDefaultLookAndFeel = false;
        btnMoveEntityUp.Name = "btnMoveEntityUp";
        btnMoveEntityUp.Size = new Size(100, 26);
        btnMoveEntityUp.TabIndex = 6;
        btnMoveEntityUp.Text = "Subir";
        //
        // btnMoveEntityDown
        //
        btnMoveEntityDown.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnMoveEntityDown.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnMoveEntityDown.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnMoveEntityDown.Appearance.ForeColor = Color.White;
        btnMoveEntityDown.Appearance.Options.UseBackColor = true;
        btnMoveEntityDown.Appearance.Options.UseBorderColor = true;
        btnMoveEntityDown.Appearance.Options.UseFont = true;
        btnMoveEntityDown.Appearance.Options.UseForeColor = true;
        btnMoveEntityDown.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnMoveEntityDown.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnMoveEntityDown.AppearanceHovered.ForeColor = Color.White;
        btnMoveEntityDown.AppearanceHovered.Options.UseBackColor = true;
        btnMoveEntityDown.AppearanceHovered.Options.UseBorderColor = true;
        btnMoveEntityDown.AppearanceHovered.Options.UseForeColor = true;
        btnMoveEntityDown.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnMoveEntityDown.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnMoveEntityDown.AppearancePressed.ForeColor = Color.White;
        btnMoveEntityDown.AppearancePressed.Options.UseBackColor = true;
        btnMoveEntityDown.AppearancePressed.Options.UseBorderColor = true;
        btnMoveEntityDown.AppearancePressed.Options.UseForeColor = true;
        btnMoveEntityDown.ButtonKind = NuanActionButtonKind.Save;
        btnMoveEntityDown.ButtonStyle = BorderStyles.UltraFlat;
        btnMoveEntityDown.ButtonText = "Bajar";
        btnMoveEntityDown.IconNameOverride = "importar_16.svg";
        btnMoveEntityDown.IconSize = 16;
        btnMoveEntityDown.ImageOptions.ImageToTextIndent = 0;
        btnMoveEntityDown.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnMoveEntityDown.ImageOptions.SvgImageSize = new Size(16, 16);
        btnMoveEntityDown.Location = new Point(452, 88);
        btnMoveEntityDown.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnMoveEntityDown.LookAndFeel.UseDefaultLookAndFeel = false;
        btnMoveEntityDown.Name = "btnMoveEntityDown";
        btnMoveEntityDown.Size = new Size(100, 26);
        btnMoveEntityDown.TabIndex = 7;
        btnMoveEntityDown.Text = "Bajar";
        //
        // btnActivateEntity
        //
        btnActivateEntity.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnActivateEntity.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnActivateEntity.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnActivateEntity.Appearance.ForeColor = Color.White;
        btnActivateEntity.Appearance.Options.UseBackColor = true;
        btnActivateEntity.Appearance.Options.UseBorderColor = true;
        btnActivateEntity.Appearance.Options.UseFont = true;
        btnActivateEntity.Appearance.Options.UseForeColor = true;
        btnActivateEntity.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnActivateEntity.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnActivateEntity.AppearanceHovered.ForeColor = Color.White;
        btnActivateEntity.AppearanceHovered.Options.UseBackColor = true;
        btnActivateEntity.AppearanceHovered.Options.UseBorderColor = true;
        btnActivateEntity.AppearanceHovered.Options.UseForeColor = true;
        btnActivateEntity.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnActivateEntity.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnActivateEntity.AppearancePressed.ForeColor = Color.White;
        btnActivateEntity.AppearancePressed.Options.UseBackColor = true;
        btnActivateEntity.AppearancePressed.Options.UseBorderColor = true;
        btnActivateEntity.AppearancePressed.Options.UseForeColor = true;
        btnActivateEntity.ButtonKind = NuanActionButtonKind.Save;
        btnActivateEntity.ButtonStyle = BorderStyles.UltraFlat;
        btnActivateEntity.ButtonText = "Activar";
        btnActivateEntity.IconNameOverride = "aprobar_16.svg";
        btnActivateEntity.IconSize = 16;
        btnActivateEntity.ImageOptions.ImageToTextIndent = 0;
        btnActivateEntity.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnActivateEntity.ImageOptions.SvgImageSize = new Size(16, 16);
        btnActivateEntity.Location = new Point(558, 88);
        btnActivateEntity.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnActivateEntity.LookAndFeel.UseDefaultLookAndFeel = false;
        btnActivateEntity.Name = "btnActivateEntity";
        btnActivateEntity.Size = new Size(100, 26);
        btnActivateEntity.TabIndex = 8;
        btnActivateEntity.Text = "Activar";
        //
        // btnDeactivateEntity
        //
        btnDeactivateEntity.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnDeactivateEntity.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnDeactivateEntity.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeactivateEntity.Appearance.ForeColor = Color.White;
        btnDeactivateEntity.Appearance.Options.UseBackColor = true;
        btnDeactivateEntity.Appearance.Options.UseBorderColor = true;
        btnDeactivateEntity.Appearance.Options.UseFont = true;
        btnDeactivateEntity.Appearance.Options.UseForeColor = true;
        btnDeactivateEntity.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnDeactivateEntity.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnDeactivateEntity.AppearanceHovered.ForeColor = Color.White;
        btnDeactivateEntity.AppearanceHovered.Options.UseBackColor = true;
        btnDeactivateEntity.AppearanceHovered.Options.UseBorderColor = true;
        btnDeactivateEntity.AppearanceHovered.Options.UseForeColor = true;
        btnDeactivateEntity.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnDeactivateEntity.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnDeactivateEntity.AppearancePressed.ForeColor = Color.White;
        btnDeactivateEntity.AppearancePressed.Options.UseBackColor = true;
        btnDeactivateEntity.AppearancePressed.Options.UseBorderColor = true;
        btnDeactivateEntity.AppearancePressed.Options.UseForeColor = true;
        btnDeactivateEntity.ButtonKind = NuanActionButtonKind.Save;
        btnDeactivateEntity.ButtonStyle = BorderStyles.UltraFlat;
        btnDeactivateEntity.ButtonText = "Desactivar";
        btnDeactivateEntity.IconNameOverride = "rechazar_16.svg";
        btnDeactivateEntity.IconSize = 16;
        btnDeactivateEntity.ImageOptions.ImageToTextIndent = 0;
        btnDeactivateEntity.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnDeactivateEntity.ImageOptions.SvgImageSize = new Size(16, 16);
        btnDeactivateEntity.Location = new Point(664, 88);
        btnDeactivateEntity.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnDeactivateEntity.LookAndFeel.UseDefaultLookAndFeel = false;
        btnDeactivateEntity.Name = "btnDeactivateEntity";
        btnDeactivateEntity.Size = new Size(100, 26);
        btnDeactivateEntity.TabIndex = 9;
        btnDeactivateEntity.Text = "Desactivar";
        //
        // grdEntities
        //
        grdEntities.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdEntities.FormKey = "sync-profile-entities";
        grdEntities.GridName = "EntitiesGrid";
        grdEntities.Location = new Point(28, 132);
        grdEntities.Name = "grdEntities";
        grdEntities.ShowPagination = false;
        grdEntities.Size = new Size(790, 386);
        grdEntities.TabIndex = 10;
        //
        // lblEntitiesInfo
        //
        lblEntitiesInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblEntitiesInfo.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblEntitiesInfo.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblEntitiesInfo.Appearance.Options.UseFont = true;
        lblEntitiesInfo.Appearance.Options.UseForeColor = true;
        lblEntitiesInfo.AutoSizeMode = LabelAutoSizeMode.None;
        lblEntitiesInfo.Location = new Point(28, 540);
        lblEntitiesInfo.Name = "lblEntitiesInfo";
        lblEntitiesInfo.Size = new Size(790, 34);
        lblEntitiesInfo.TabIndex = 11;
        lblEntitiesInfo.Text = "Las entidades activas participan en la ejecución según el orden configurado. Use el diálogo para definir campos técnicos y capacidades.";
        //
        // lblEntitiesTotal
        //
        lblEntitiesTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblEntitiesTotal.Appearance.Font = new Font("Segoe UI", 9F);
        lblEntitiesTotal.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblEntitiesTotal.Appearance.Options.UseFont = true;
        lblEntitiesTotal.Appearance.Options.UseForeColor = true;
        lblEntitiesTotal.Location = new Point(28, 586);
        lblEntitiesTotal.Name = "lblEntitiesTotal";
        lblEntitiesTotal.Size = new Size(93, 15);
        lblEntitiesTotal.TabIndex = 12;
        lblEntitiesTotal.Text = "Total: 0 entidades";
        //
        // pageDistribution
        //
        pageDistribution.Caption = "Distribución";
        pageDistribution.Controls.Add(lblDistributionTitle);
        pageDistribution.Controls.Add(sepDistributionTitle);
        pageDistribution.Controls.Add(btnEnableDistribution);
        pageDistribution.Controls.Add(btnDisableDistribution);
        pageDistribution.Controls.Add(btnConfigureDistributionBatch);
        pageDistribution.Controls.Add(btnEnableAllDistributions);
        pageDistribution.Controls.Add(btnDisableAllDistributions);
        pageDistribution.Controls.Add(btnRefreshDistribution);
        pageDistribution.Controls.Add(grdDistribution);
        pageDistribution.Controls.Add(lblDistributionInfo);
        pageDistribution.Font = new Font("Segoe UI", 9F);
        pageDistribution.Name = "pageDistribution";
        pageDistribution.Size = new Size(851, 637);
        //
        // lblDistributionTitle
        //
        lblDistributionTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblDistributionTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblDistributionTitle.Appearance.Options.UseFont = true;
        lblDistributionTitle.Appearance.Options.UseForeColor = true;
        lblDistributionTitle.Location = new Point(28, 22);
        lblDistributionTitle.Name = "lblDistributionTitle";
        lblDistributionTitle.Size = new Size(174, 20);
        lblDistributionTitle.TabIndex = 0;
        lblDistributionTitle.Text = "Distribución de entidades";
        //
        // sepDistributionTitle
        //
        sepDistributionTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepDistributionTitle.LineColor = Color.FromArgb(0, 184, 148);
        sepDistributionTitle.Location = new Point(28, 54);
        sepDistributionTitle.Name = "sepDistributionTitle";
        sepDistributionTitle.Size = new Size(790, 18);
        sepDistributionTitle.TabIndex = 2;
        //
        // btnEnableDistribution
        //
        btnEnableDistribution.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnEnableDistribution.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnEnableDistribution.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEnableDistribution.Appearance.ForeColor = Color.White;
        btnEnableDistribution.Appearance.Options.UseBackColor = true;
        btnEnableDistribution.Appearance.Options.UseBorderColor = true;
        btnEnableDistribution.Appearance.Options.UseFont = true;
        btnEnableDistribution.Appearance.Options.UseForeColor = true;
        btnEnableDistribution.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnEnableDistribution.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnEnableDistribution.AppearanceHovered.ForeColor = Color.White;
        btnEnableDistribution.AppearanceHovered.Options.UseBackColor = true;
        btnEnableDistribution.AppearanceHovered.Options.UseBorderColor = true;
        btnEnableDistribution.AppearanceHovered.Options.UseForeColor = true;
        btnEnableDistribution.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnEnableDistribution.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnEnableDistribution.AppearancePressed.ForeColor = Color.White;
        btnEnableDistribution.AppearancePressed.Options.UseBackColor = true;
        btnEnableDistribution.AppearancePressed.Options.UseBorderColor = true;
        btnEnableDistribution.AppearancePressed.Options.UseForeColor = true;
        btnEnableDistribution.ButtonKind = NuanActionButtonKind.Save;
        btnEnableDistribution.ButtonStyle = BorderStyles.UltraFlat;
        btnEnableDistribution.ButtonText = "Habilitar";
        btnEnableDistribution.IconNameOverride = "aprobar_16.svg";
        btnEnableDistribution.IconSize = 16;
        btnEnableDistribution.ImageOptions.ImageToTextIndent = 0;
        btnEnableDistribution.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnEnableDistribution.ImageOptions.SvgImageSize = new Size(16, 16);
        btnEnableDistribution.Location = new Point(28, 88);
        btnEnableDistribution.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnEnableDistribution.LookAndFeel.UseDefaultLookAndFeel = false;
        btnEnableDistribution.Name = "btnEnableDistribution";
        btnEnableDistribution.Size = new Size(100, 26);
        btnEnableDistribution.TabIndex = 3;
        btnEnableDistribution.Text = "Habilitar";
        //
        // btnDisableDistribution
        //
        btnDisableDistribution.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnDisableDistribution.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnDisableDistribution.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDisableDistribution.Appearance.ForeColor = Color.White;
        btnDisableDistribution.Appearance.Options.UseBackColor = true;
        btnDisableDistribution.Appearance.Options.UseBorderColor = true;
        btnDisableDistribution.Appearance.Options.UseFont = true;
        btnDisableDistribution.Appearance.Options.UseForeColor = true;
        btnDisableDistribution.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnDisableDistribution.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnDisableDistribution.AppearanceHovered.ForeColor = Color.White;
        btnDisableDistribution.AppearanceHovered.Options.UseBackColor = true;
        btnDisableDistribution.AppearanceHovered.Options.UseBorderColor = true;
        btnDisableDistribution.AppearanceHovered.Options.UseForeColor = true;
        btnDisableDistribution.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnDisableDistribution.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnDisableDistribution.AppearancePressed.ForeColor = Color.White;
        btnDisableDistribution.AppearancePressed.Options.UseBackColor = true;
        btnDisableDistribution.AppearancePressed.Options.UseBorderColor = true;
        btnDisableDistribution.AppearancePressed.Options.UseForeColor = true;
        btnDisableDistribution.ButtonKind = NuanActionButtonKind.Save;
        btnDisableDistribution.ButtonStyle = BorderStyles.UltraFlat;
        btnDisableDistribution.ButtonText = "Deshabilitar";
        btnDisableDistribution.IconNameOverride = "rechazar_16.svg";
        btnDisableDistribution.IconSize = 16;
        btnDisableDistribution.ImageOptions.ImageToTextIndent = 0;
        btnDisableDistribution.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnDisableDistribution.ImageOptions.SvgImageSize = new Size(16, 16);
        btnDisableDistribution.Location = new Point(134, 88);
        btnDisableDistribution.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnDisableDistribution.LookAndFeel.UseDefaultLookAndFeel = false;
        btnDisableDistribution.Name = "btnDisableDistribution";
        btnDisableDistribution.Size = new Size(106, 26);
        btnDisableDistribution.TabIndex = 4;
        btnDisableDistribution.Text = "Deshabilitar";
        //
        // btnConfigureDistributionBatch
        //
        btnConfigureDistributionBatch.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnConfigureDistributionBatch.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnConfigureDistributionBatch.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnConfigureDistributionBatch.Appearance.ForeColor = Color.White;
        btnConfigureDistributionBatch.Appearance.Options.UseBackColor = true;
        btnConfigureDistributionBatch.Appearance.Options.UseBorderColor = true;
        btnConfigureDistributionBatch.Appearance.Options.UseFont = true;
        btnConfigureDistributionBatch.Appearance.Options.UseForeColor = true;
        btnConfigureDistributionBatch.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnConfigureDistributionBatch.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnConfigureDistributionBatch.AppearanceHovered.ForeColor = Color.White;
        btnConfigureDistributionBatch.AppearanceHovered.Options.UseBackColor = true;
        btnConfigureDistributionBatch.AppearanceHovered.Options.UseBorderColor = true;
        btnConfigureDistributionBatch.AppearanceHovered.Options.UseForeColor = true;
        btnConfigureDistributionBatch.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnConfigureDistributionBatch.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnConfigureDistributionBatch.AppearancePressed.ForeColor = Color.White;
        btnConfigureDistributionBatch.AppearancePressed.Options.UseBackColor = true;
        btnConfigureDistributionBatch.AppearancePressed.Options.UseBorderColor = true;
        btnConfigureDistributionBatch.AppearancePressed.Options.UseForeColor = true;
        btnConfigureDistributionBatch.ButtonKind = NuanActionButtonKind.Save;
        btnConfigureDistributionBatch.ButtonStyle = BorderStyles.UltraFlat;
        btnConfigureDistributionBatch.ButtonText = "Configurar política";
        btnConfigureDistributionBatch.IconNameOverride = "editar_cuadro_16.svg";
        btnConfigureDistributionBatch.IconSize = 16;
        btnConfigureDistributionBatch.ImageOptions.ImageToTextIndent = 0;
        btnConfigureDistributionBatch.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnConfigureDistributionBatch.ImageOptions.SvgImageSize = new Size(16, 16);
        btnConfigureDistributionBatch.Location = new Point(246, 88);
        btnConfigureDistributionBatch.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnConfigureDistributionBatch.LookAndFeel.UseDefaultLookAndFeel = false;
        btnConfigureDistributionBatch.Name = "btnConfigureDistributionBatch";
        btnConfigureDistributionBatch.Size = new Size(140, 26);
        btnConfigureDistributionBatch.TabIndex = 5;
        btnConfigureDistributionBatch.Text = "Configurar política";
        //
        // btnEnableAllDistributions
        //
        btnEnableAllDistributions.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnEnableAllDistributions.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnEnableAllDistributions.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEnableAllDistributions.Appearance.ForeColor = Color.White;
        btnEnableAllDistributions.Appearance.Options.UseBackColor = true;
        btnEnableAllDistributions.Appearance.Options.UseBorderColor = true;
        btnEnableAllDistributions.Appearance.Options.UseFont = true;
        btnEnableAllDistributions.Appearance.Options.UseForeColor = true;
        btnEnableAllDistributions.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnEnableAllDistributions.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnEnableAllDistributions.AppearanceHovered.ForeColor = Color.White;
        btnEnableAllDistributions.AppearanceHovered.Options.UseBackColor = true;
        btnEnableAllDistributions.AppearanceHovered.Options.UseBorderColor = true;
        btnEnableAllDistributions.AppearanceHovered.Options.UseForeColor = true;
        btnEnableAllDistributions.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnEnableAllDistributions.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnEnableAllDistributions.AppearancePressed.ForeColor = Color.White;
        btnEnableAllDistributions.AppearancePressed.Options.UseBackColor = true;
        btnEnableAllDistributions.AppearancePressed.Options.UseBorderColor = true;
        btnEnableAllDistributions.AppearancePressed.Options.UseForeColor = true;
        btnEnableAllDistributions.ButtonKind = NuanActionButtonKind.Save;
        btnEnableAllDistributions.ButtonStyle = BorderStyles.UltraFlat;
        btnEnableAllDistributions.ButtonText = "Habilitar todos";
        btnEnableAllDistributions.IconNameOverride = "aprobar_16.svg";
        btnEnableAllDistributions.IconSize = 16;
        btnEnableAllDistributions.ImageOptions.ImageToTextIndent = 0;
        btnEnableAllDistributions.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnEnableAllDistributions.ImageOptions.SvgImageSize = new Size(16, 16);
        btnEnableAllDistributions.Location = new Point(392, 88);
        btnEnableAllDistributions.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnEnableAllDistributions.LookAndFeel.UseDefaultLookAndFeel = false;
        btnEnableAllDistributions.Name = "btnEnableAllDistributions";
        btnEnableAllDistributions.Size = new Size(112, 26);
        btnEnableAllDistributions.TabIndex = 6;
        btnEnableAllDistributions.Text = "Habilitar todos";
        //
        // btnDisableAllDistributions
        //
        btnDisableAllDistributions.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnDisableAllDistributions.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnDisableAllDistributions.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDisableAllDistributions.Appearance.ForeColor = Color.White;
        btnDisableAllDistributions.Appearance.Options.UseBackColor = true;
        btnDisableAllDistributions.Appearance.Options.UseBorderColor = true;
        btnDisableAllDistributions.Appearance.Options.UseFont = true;
        btnDisableAllDistributions.Appearance.Options.UseForeColor = true;
        btnDisableAllDistributions.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnDisableAllDistributions.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnDisableAllDistributions.AppearanceHovered.ForeColor = Color.White;
        btnDisableAllDistributions.AppearanceHovered.Options.UseBackColor = true;
        btnDisableAllDistributions.AppearanceHovered.Options.UseBorderColor = true;
        btnDisableAllDistributions.AppearanceHovered.Options.UseForeColor = true;
        btnDisableAllDistributions.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnDisableAllDistributions.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnDisableAllDistributions.AppearancePressed.ForeColor = Color.White;
        btnDisableAllDistributions.AppearancePressed.Options.UseBackColor = true;
        btnDisableAllDistributions.AppearancePressed.Options.UseBorderColor = true;
        btnDisableAllDistributions.AppearancePressed.Options.UseForeColor = true;
        btnDisableAllDistributions.ButtonKind = NuanActionButtonKind.Save;
        btnDisableAllDistributions.ButtonStyle = BorderStyles.UltraFlat;
        btnDisableAllDistributions.ButtonText = "Deshabilitar todos";
        btnDisableAllDistributions.IconNameOverride = "rechazar_16.svg";
        btnDisableAllDistributions.IconSize = 16;
        btnDisableAllDistributions.ImageOptions.ImageToTextIndent = 0;
        btnDisableAllDistributions.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnDisableAllDistributions.ImageOptions.SvgImageSize = new Size(16, 16);
        btnDisableAllDistributions.Location = new Point(510, 88);
        btnDisableAllDistributions.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnDisableAllDistributions.LookAndFeel.UseDefaultLookAndFeel = false;
        btnDisableAllDistributions.Name = "btnDisableAllDistributions";
        btnDisableAllDistributions.Size = new Size(126, 26);
        btnDisableAllDistributions.TabIndex = 7;
        btnDisableAllDistributions.Text = "Deshabilitar todos";
        //
        // btnRefreshDistribution
        //
        btnRefreshDistribution.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnRefreshDistribution.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnRefreshDistribution.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRefreshDistribution.Appearance.ForeColor = Color.White;
        btnRefreshDistribution.Appearance.Options.UseBackColor = true;
        btnRefreshDistribution.Appearance.Options.UseBorderColor = true;
        btnRefreshDistribution.Appearance.Options.UseFont = true;
        btnRefreshDistribution.Appearance.Options.UseForeColor = true;
        btnRefreshDistribution.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnRefreshDistribution.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnRefreshDistribution.AppearanceHovered.ForeColor = Color.White;
        btnRefreshDistribution.AppearanceHovered.Options.UseBackColor = true;
        btnRefreshDistribution.AppearanceHovered.Options.UseBorderColor = true;
        btnRefreshDistribution.AppearanceHovered.Options.UseForeColor = true;
        btnRefreshDistribution.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnRefreshDistribution.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnRefreshDistribution.AppearancePressed.ForeColor = Color.White;
        btnRefreshDistribution.AppearancePressed.Options.UseBackColor = true;
        btnRefreshDistribution.AppearancePressed.Options.UseBorderColor = true;
        btnRefreshDistribution.AppearancePressed.Options.UseForeColor = true;
        btnRefreshDistribution.ButtonKind = NuanActionButtonKind.Save;
        btnRefreshDistribution.ButtonStyle = BorderStyles.UltraFlat;
        btnRefreshDistribution.ButtonText = "Refrescar";
        btnRefreshDistribution.IconNameOverride = "actualizar_16.svg";
        btnRefreshDistribution.IconSize = 16;
        btnRefreshDistribution.ImageOptions.ImageToTextIndent = 0;
        btnRefreshDistribution.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnRefreshDistribution.ImageOptions.SvgImageSize = new Size(16, 16);
        btnRefreshDistribution.Location = new Point(642, 88);
        btnRefreshDistribution.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnRefreshDistribution.LookAndFeel.UseDefaultLookAndFeel = false;
        btnRefreshDistribution.Name = "btnRefreshDistribution";
        btnRefreshDistribution.Size = new Size(100, 26);
        btnRefreshDistribution.TabIndex = 8;
        btnRefreshDistribution.Text = "Refrescar";
        //
        // grdDistribution
        //
        grdDistribution.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdDistribution.FormKey = "sync-profile-distribution";
        grdDistribution.GridName = "DistributionGrid";
        grdDistribution.Location = new Point(28, 132);
        grdDistribution.Name = "grdDistribution";
        grdDistribution.ShowFindPanel = false;
        grdDistribution.ShowPagination = false;
        grdDistribution.Size = new Size(790, 420);
        grdDistribution.TabIndex = 9;
        //
        // lblDistributionInfo
        //
        lblDistributionInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblDistributionInfo.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblDistributionInfo.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblDistributionInfo.Appearance.Options.UseFont = true;
        lblDistributionInfo.Appearance.Options.UseForeColor = true;
        lblDistributionInfo.AutoSizeMode = LabelAutoSizeMode.None;
        lblDistributionInfo.Location = new Point(28, 574);
        lblDistributionInfo.Name = "lblDistributionInfo";
        lblDistributionInfo.Size = new Size(790, 34);
        lblDistributionInfo.TabIndex = 10;
        lblDistributionInfo.Text = "Seleccione una combinación entidad-sucursal y pulse Configurar política para editar modo, selección, regla y batch.";
        //
        // pageSchedule
        //
        pageSchedule.Caption = "Programacion";
        pageSchedule.Controls.Add(lblScheduleTitle);
        pageSchedule.Controls.Add(sepScheduleTitle);
        pageSchedule.Controls.Add(lblScheduleConfigurationTitle);
        pageSchedule.Controls.Add(sepScheduleConfiguration);
        pageSchedule.Controls.Add(lblScheduleType);
        pageSchedule.Controls.Add(cboScheduleType);
        pageSchedule.Controls.Add(lblScheduleInterval);
        pageSchedule.Controls.Add(spnScheduleIntervalMinutes);
        pageSchedule.Controls.Add(lblScheduleIntervalUnit);
        pageSchedule.Controls.Add(lblScheduleExecutionTime);
        pageSchedule.Controls.Add(timScheduleExecutionTime);
        pageSchedule.Controls.Add(lblScheduleTimeZone);
        pageSchedule.Controls.Add(cboScheduleTimeZone);
        pageSchedule.Controls.Add(lblPreventConcurrentExecutions);
        pageSchedule.Controls.Add(swPreventConcurrentExecutions);
        pageSchedule.Controls.Add(lblScheduleIsActive);
        pageSchedule.Controls.Add(swScheduleIsActive);
        pageSchedule.Controls.Add(lblScheduleInfo);
        pageSchedule.Controls.Add(lblScheduleStatusTitle);
        pageSchedule.Controls.Add(sepScheduleStatus);
        pageSchedule.Controls.Add(lblScheduleNextExecution);
        pageSchedule.Controls.Add(lblScheduleNextExecutionValue);
        pageSchedule.Controls.Add(lblScheduleLastExecution);
        pageSchedule.Controls.Add(lblScheduleLastExecutionValue);
        pageSchedule.Controls.Add(lblScheduleEffectiveFrequency);
        pageSchedule.Controls.Add(lblScheduleEffectiveFrequencyValue);
        pageSchedule.Controls.Add(lblScheduleStatus);
        pageSchedule.Controls.Add(lblScheduleStatusValue);
        pageSchedule.Font = new Font("Segoe UI", 9F);
        pageSchedule.Name = "pageSchedule";
        pageSchedule.Size = new Size(851, 637);
        //
        // lblScheduleTitle
        //
        lblScheduleTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblScheduleTitle.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        lblScheduleTitle.Appearance.Options.UseFont = true;
        lblScheduleTitle.Appearance.Options.UseForeColor = true;
        lblScheduleTitle.Location = new Point(28, 22);
        lblScheduleTitle.Name = "lblScheduleTitle";
        lblScheduleTitle.Size = new Size(162, 20);
        lblScheduleTitle.TabIndex = 0;
        lblScheduleTitle.Text = "Programación del perfil";
        //
        // sepScheduleTitle
        //
        sepScheduleTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepScheduleTitle.LineColor = Color.FromArgb(0, 184, 148);
        sepScheduleTitle.Location = new Point(28, 54);
        sepScheduleTitle.Name = "sepScheduleTitle";
        sepScheduleTitle.Size = new Size(790, 18);
        sepScheduleTitle.TabIndex = 2;
        //
        // lblScheduleConfigurationTitle
        //
        lblScheduleConfigurationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblScheduleConfigurationTitle.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        lblScheduleConfigurationTitle.Appearance.Options.UseFont = true;
        lblScheduleConfigurationTitle.Appearance.Options.UseForeColor = true;
        lblScheduleConfigurationTitle.Location = new Point(44, 96);
        lblScheduleConfigurationTitle.Name = "lblScheduleConfigurationTitle";
        lblScheduleConfigurationTitle.Size = new Size(235, 20);
        lblScheduleConfigurationTitle.TabIndex = 3;
        lblScheduleConfigurationTitle.Text = "Configuración de la programación";
        //
        // sepScheduleConfiguration
        //
        sepScheduleConfiguration.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepScheduleConfiguration.LineColor = Color.FromArgb(221, 226, 240);
        sepScheduleConfiguration.Location = new Point(44, 117);
        sepScheduleConfiguration.Name = "sepScheduleConfiguration";
        sepScheduleConfiguration.Size = new Size(758, 12);
        sepScheduleConfiguration.TabIndex = 4;
        //
        // lblScheduleType
        //
        lblScheduleType.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleType.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleType.Appearance.Options.UseFont = true;
        lblScheduleType.Appearance.Options.UseForeColor = true;
        lblScheduleType.Location = new Point(48, 139);
        lblScheduleType.Name = "lblScheduleType";
        lblScheduleType.Size = new Size(118, 15);
        lblScheduleType.TabIndex = 5;
        lblScheduleType.Text = "Tipo de programación";
        //
        // cboScheduleType
        //
        cboScheduleType.EditValue = "Intervalo";
        cboScheduleType.Location = new Point(300, 135);
        cboScheduleType.Name = "cboScheduleType";
        cboScheduleType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboScheduleType.Properties.Appearance.Options.UseFont = true;
        cboScheduleType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboScheduleType.Properties.Items.AddRange(new object[] { "Manual", "Intervalo", "Diaria" });
        cboScheduleType.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboScheduleType.Size = new Size(360, 22);
        cboScheduleType.TabIndex = 6;
        //
        // lblScheduleInterval
        //
        lblScheduleInterval.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleInterval.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleInterval.Appearance.Options.UseFont = true;
        lblScheduleInterval.Appearance.Options.UseForeColor = true;
        lblScheduleInterval.Location = new Point(48, 177);
        lblScheduleInterval.Name = "lblScheduleInterval";
        lblScheduleInterval.Size = new Size(70, 15);
        lblScheduleInterval.TabIndex = 7;
        lblScheduleInterval.Text = "Ejecutar cada";
        //
        // spnScheduleIntervalMinutes
        //
        spnScheduleIntervalMinutes.EditValue = new decimal(new int[] { 60, 0, 0, 0 });
        spnScheduleIntervalMinutes.Location = new Point(300, 173);
        spnScheduleIntervalMinutes.Name = "spnScheduleIntervalMinutes";
        spnScheduleIntervalMinutes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnScheduleIntervalMinutes.Properties.Appearance.Options.UseFont = true;
        spnScheduleIntervalMinutes.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnScheduleIntervalMinutes.Properties.IsFloatValue = false;
        spnScheduleIntervalMinutes.Properties.MaskSettings.Set("mask", "N00");
        spnScheduleIntervalMinutes.Properties.MaxValue = new decimal(new int[] { 1440, 0, 0, 0 });
        spnScheduleIntervalMinutes.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        spnScheduleIntervalMinutes.Size = new Size(280, 22);
        spnScheduleIntervalMinutes.TabIndex = 8;
        //
        // lblScheduleIntervalUnit
        //
        lblScheduleIntervalUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleIntervalUnit.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleIntervalUnit.Appearance.Options.UseFont = true;
        lblScheduleIntervalUnit.Appearance.Options.UseForeColor = true;
        lblScheduleIntervalUnit.Location = new Point(590, 177);
        lblScheduleIntervalUnit.Name = "lblScheduleIntervalUnit";
        lblScheduleIntervalUnit.Size = new Size(44, 15);
        lblScheduleIntervalUnit.TabIndex = 9;
        lblScheduleIntervalUnit.Text = "minutos";
        //
        // lblScheduleExecutionTime
        //
        lblScheduleExecutionTime.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleExecutionTime.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleExecutionTime.Appearance.Options.UseFont = true;
        lblScheduleExecutionTime.Appearance.Options.UseForeColor = true;
        lblScheduleExecutionTime.Location = new Point(48, 215);
        lblScheduleExecutionTime.Name = "lblScheduleExecutionTime";
        lblScheduleExecutionTime.Size = new Size(96, 15);
        lblScheduleExecutionTime.TabIndex = 10;
        lblScheduleExecutionTime.Text = "Hora de ejecución";
        //
        // timScheduleExecutionTime
        //
        timScheduleExecutionTime.EditValue = new DateTime(2026, 7, 15, 23, 0, 0, 0);
        timScheduleExecutionTime.Enabled = false;
        timScheduleExecutionTime.Location = new Point(300, 211);
        timScheduleExecutionTime.Name = "timScheduleExecutionTime";
        timScheduleExecutionTime.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        timScheduleExecutionTime.Properties.Appearance.Options.UseFont = true;
        timScheduleExecutionTime.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        timScheduleExecutionTime.Properties.DisplayFormat.FormatString = "HH:mm";
        timScheduleExecutionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        timScheduleExecutionTime.Properties.EditFormat.FormatString = "HH:mm";
        timScheduleExecutionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        timScheduleExecutionTime.Properties.MaskSettings.Set("mask", "HH:mm");
        timScheduleExecutionTime.Size = new Size(360, 22);
        timScheduleExecutionTime.TabIndex = 11;
        //
        // lblScheduleTimeZone
        //
        lblScheduleTimeZone.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleTimeZone.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleTimeZone.Appearance.Options.UseFont = true;
        lblScheduleTimeZone.Appearance.Options.UseForeColor = true;
        lblScheduleTimeZone.Location = new Point(48, 253);
        lblScheduleTimeZone.Name = "lblScheduleTimeZone";
        lblScheduleTimeZone.Size = new Size(67, 15);
        lblScheduleTimeZone.TabIndex = 12;
        lblScheduleTimeZone.Text = "Zona horaria";
        //
        // cboScheduleTimeZone
        //
        cboScheduleTimeZone.EditValue = "America/Guayaquil";
        cboScheduleTimeZone.Location = new Point(300, 249);
        cboScheduleTimeZone.Name = "cboScheduleTimeZone";
        cboScheduleTimeZone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboScheduleTimeZone.Properties.Appearance.Options.UseFont = true;
        cboScheduleTimeZone.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboScheduleTimeZone.Properties.Items.AddRange(new object[] { "America/Guayaquil" });
        cboScheduleTimeZone.Size = new Size(360, 22);
        cboScheduleTimeZone.TabIndex = 13;
        //
        // lblPreventConcurrentExecutions
        //
        lblPreventConcurrentExecutions.Appearance.Font = new Font("Segoe UI", 9F);
        lblPreventConcurrentExecutions.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblPreventConcurrentExecutions.Appearance.Options.UseFont = true;
        lblPreventConcurrentExecutions.Appearance.Options.UseForeColor = true;
        lblPreventConcurrentExecutions.Location = new Point(48, 291);
        lblPreventConcurrentExecutions.Name = "lblPreventConcurrentExecutions";
        lblPreventConcurrentExecutions.Size = new Size(160, 15);
        lblPreventConcurrentExecutions.TabIndex = 14;
        lblPreventConcurrentExecutions.Text = "Evitar ejecuciones simultáneas";
        //
        // swPreventConcurrentExecutions
        //
        swPreventConcurrentExecutions.EditValue = true;
        swPreventConcurrentExecutions.Location = new Point(300, 285);
        swPreventConcurrentExecutions.Name = "swPreventConcurrentExecutions";
        swPreventConcurrentExecutions.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swPreventConcurrentExecutions.Properties.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        swPreventConcurrentExecutions.Properties.Appearance.Options.UseFont = true;
        swPreventConcurrentExecutions.Properties.Appearance.Options.UseForeColor = true;
        swPreventConcurrentExecutions.Properties.OffText = "Inactivo";
        swPreventConcurrentExecutions.Properties.OnText = "Activo";
        swPreventConcurrentExecutions.Size = new Size(150, 20);
        swPreventConcurrentExecutions.TabIndex = 15;
        //
        // lblScheduleIsActive
        //
        lblScheduleIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleIsActive.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleIsActive.Appearance.Options.UseFont = true;
        lblScheduleIsActive.Appearance.Options.UseForeColor = true;
        lblScheduleIsActive.Location = new Point(48, 329);
        lblScheduleIsActive.Name = "lblScheduleIsActive";
        lblScheduleIsActive.Size = new Size(109, 15);
        lblScheduleIsActive.TabIndex = 16;
        lblScheduleIsActive.Text = "Programación activa";
        //
        // swScheduleIsActive
        //
        swScheduleIsActive.EditValue = true;
        swScheduleIsActive.Location = new Point(300, 323);
        swScheduleIsActive.Name = "swScheduleIsActive";
        swScheduleIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swScheduleIsActive.Properties.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        swScheduleIsActive.Properties.Appearance.Options.UseFont = true;
        swScheduleIsActive.Properties.Appearance.Options.UseForeColor = true;
        swScheduleIsActive.Properties.OffText = "Inactivo";
        swScheduleIsActive.Properties.OnText = "Activo";
        swScheduleIsActive.Size = new Size(150, 20);
        swScheduleIsActive.TabIndex = 17;
        //
        // lblScheduleInfo
        //
        lblScheduleInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblScheduleInfo.Appearance.BackColor = Color.FromArgb(247, 248, 252);
        lblScheduleInfo.Appearance.BorderColor = Color.FromArgb(221, 226, 240);
        lblScheduleInfo.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblScheduleInfo.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblScheduleInfo.Appearance.Options.UseBackColor = true;
        lblScheduleInfo.Appearance.Options.UseBorderColor = true;
        lblScheduleInfo.Appearance.Options.UseFont = true;
        lblScheduleInfo.Appearance.Options.UseForeColor = true;
        lblScheduleInfo.AutoSizeMode = LabelAutoSizeMode.None;
        lblScheduleInfo.BorderStyle = BorderStyles.Simple;
        lblScheduleInfo.Location = new Point(44, 359);
        lblScheduleInfo.Name = "lblScheduleInfo";
        lblScheduleInfo.Padding = new Padding(12, 0, 8, 0);
        lblScheduleInfo.Size = new Size(758, 38);
        lblScheduleInfo.TabIndex = 18;
        lblScheduleInfo.Text = "Cuando una ejecución anterior continúe en proceso, no se iniciará una nueva ejecución para este perfil.";
        //
        // lblScheduleStatusTitle
        //
        lblScheduleStatusTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblScheduleStatusTitle.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        lblScheduleStatusTitle.Appearance.Options.UseFont = true;
        lblScheduleStatusTitle.Appearance.Options.UseForeColor = true;
        lblScheduleStatusTitle.Location = new Point(44, 419);
        lblScheduleStatusTitle.Name = "lblScheduleStatusTitle";
        lblScheduleStatusTitle.Size = new Size(183, 20);
        lblScheduleStatusTitle.TabIndex = 19;
        lblScheduleStatusTitle.Text = "Estado de la programación";
        //
        // sepScheduleStatus
        //
        sepScheduleStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepScheduleStatus.LineColor = Color.FromArgb(221, 226, 240);
        sepScheduleStatus.Location = new Point(44, 440);
        sepScheduleStatus.Name = "sepScheduleStatus";
        sepScheduleStatus.Size = new Size(758, 12);
        sepScheduleStatus.TabIndex = 20;
        //
        // lblScheduleNextExecution
        //
        lblScheduleNextExecution.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleNextExecution.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleNextExecution.Appearance.Options.UseFont = true;
        lblScheduleNextExecution.Appearance.Options.UseForeColor = true;
        lblScheduleNextExecution.Location = new Point(48, 465);
        lblScheduleNextExecution.Name = "lblScheduleNextExecution";
        lblScheduleNextExecution.Size = new Size(100, 15);
        lblScheduleNextExecution.TabIndex = 21;
        lblScheduleNextExecution.Text = "Próxima ejecución:";
        //
        // lblScheduleNextExecutionValue
        //
        lblScheduleNextExecutionValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleNextExecutionValue.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleNextExecutionValue.Appearance.Options.UseFont = true;
        lblScheduleNextExecutionValue.Appearance.Options.UseForeColor = true;
        lblScheduleNextExecutionValue.Location = new Point(300, 465);
        lblScheduleNextExecutionValue.Name = "lblScheduleNextExecutionValue";
        lblScheduleNextExecutionValue.Size = new Size(5, 15);
        lblScheduleNextExecutionValue.TabIndex = 22;
        lblScheduleNextExecutionValue.Text = "-";
        //
        // lblScheduleLastExecution
        //
        lblScheduleLastExecution.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleLastExecution.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleLastExecution.Appearance.Options.UseFont = true;
        lblScheduleLastExecution.Appearance.Options.UseForeColor = true;
        lblScheduleLastExecution.Location = new Point(48, 493);
        lblScheduleLastExecution.Name = "lblScheduleLastExecution";
        lblScheduleLastExecution.Size = new Size(199, 15);
        lblScheduleLastExecution.TabIndex = 23;
        lblScheduleLastExecution.Text = "Última ejecución programada exitosa:";
        //
        // lblScheduleLastExecutionValue
        //
        lblScheduleLastExecutionValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleLastExecutionValue.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleLastExecutionValue.Appearance.Options.UseFont = true;
        lblScheduleLastExecutionValue.Appearance.Options.UseForeColor = true;
        lblScheduleLastExecutionValue.Location = new Point(300, 493);
        lblScheduleLastExecutionValue.Name = "lblScheduleLastExecutionValue";
        lblScheduleLastExecutionValue.Size = new Size(5, 15);
        lblScheduleLastExecutionValue.TabIndex = 24;
        lblScheduleLastExecutionValue.Text = "-";
        //
        // lblScheduleEffectiveFrequency
        //
        lblScheduleEffectiveFrequency.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleEffectiveFrequency.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleEffectiveFrequency.Appearance.Options.UseFont = true;
        lblScheduleEffectiveFrequency.Appearance.Options.UseForeColor = true;
        lblScheduleEffectiveFrequency.Location = new Point(48, 521);
        lblScheduleEffectiveFrequency.Name = "lblScheduleEffectiveFrequency";
        lblScheduleEffectiveFrequency.Size = new Size(104, 15);
        lblScheduleEffectiveFrequency.TabIndex = 25;
        lblScheduleEffectiveFrequency.Text = "Frecuencia efectiva:";
        //
        // lblScheduleEffectiveFrequencyValue
        //
        lblScheduleEffectiveFrequencyValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleEffectiveFrequencyValue.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleEffectiveFrequencyValue.Appearance.Options.UseFont = true;
        lblScheduleEffectiveFrequencyValue.Appearance.Options.UseForeColor = true;
        lblScheduleEffectiveFrequencyValue.Location = new Point(300, 521);
        lblScheduleEffectiveFrequencyValue.Name = "lblScheduleEffectiveFrequencyValue";
        lblScheduleEffectiveFrequencyValue.Size = new Size(89, 15);
        lblScheduleEffectiveFrequencyValue.TabIndex = 26;
        lblScheduleEffectiveFrequencyValue.Text = "Cada 60 minutos";
        //
        // lblScheduleStatus
        //
        lblScheduleStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblScheduleStatus.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblScheduleStatus.Appearance.Options.UseFont = true;
        lblScheduleStatus.Appearance.Options.UseForeColor = true;
        lblScheduleStatus.Location = new Point(48, 553);
        lblScheduleStatus.Name = "lblScheduleStatus";
        lblScheduleStatus.Size = new Size(38, 15);
        lblScheduleStatus.TabIndex = 27;
        lblScheduleStatus.Text = "Estado:";
        //
        // lblScheduleStatusValue
        //
        lblScheduleStatusValue.Appearance.BackColor = Color.FromArgb(236, 253, 245);
        lblScheduleStatusValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblScheduleStatusValue.Appearance.ForeColor = Color.FromArgb(22, 163, 74);
        lblScheduleStatusValue.Appearance.Options.UseBackColor = true;
        lblScheduleStatusValue.Appearance.Options.UseFont = true;
        lblScheduleStatusValue.Appearance.Options.UseForeColor = true;
        lblScheduleStatusValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblScheduleStatusValue.Location = new Point(300, 547);
        lblScheduleStatusValue.Name = "lblScheduleStatusValue";
        lblScheduleStatusValue.Padding = new Padding(10, 0, 10, 0);
        lblScheduleStatusValue.Size = new Size(105, 27);
        lblScheduleStatusValue.TabIndex = 28;
        lblScheduleStatusValue.Text = "Programada";
        //
        // pageValidation
        //
        pageValidation.Caption = "Validacion";
        pageValidation.Controls.Add(lblValidationTitle);
        pageValidation.Controls.Add(sepValidationTitle);
        pageValidation.Controls.Add(btnValidateProfile);
        pageValidation.Controls.Add(lblValidationDescription);
        pageValidation.Controls.Add(lblValidationResultCaption);
        pageValidation.Controls.Add(lblValidationResultValue);
        pageValidation.Controls.Add(lblValidationErrorsCaption);
        pageValidation.Controls.Add(lblValidationErrorsValue);
        pageValidation.Controls.Add(lblValidationWarningsCaption);
        pageValidation.Controls.Add(lblValidationWarningsValue);
        pageValidation.Controls.Add(lblValidationResultsTitle);
        pageValidation.Controls.Add(grdValidationResults);
        pageValidation.Controls.Add(lblValidationInfo);
        pageValidation.Controls.Add(lblValidationSummarySurface);
        pageValidation.Font = new Font("Segoe UI", 9F);
        pageValidation.Name = "pageValidation";
        pageValidation.Size = new Size(851, 637);
        //
        // lblValidationTitle
        //
        lblValidationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblValidationTitle.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        lblValidationTitle.Appearance.Options.UseFont = true;
        lblValidationTitle.Appearance.Options.UseForeColor = true;
        lblValidationTitle.Location = new Point(28, 22);
        lblValidationTitle.Name = "lblValidationTitle";
        lblValidationTitle.Size = new Size(137, 20);
        lblValidationTitle.TabIndex = 0;
        lblValidationTitle.Text = "Validación del perfil";
        //
        // sepValidationTitle
        //
        sepValidationTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepValidationTitle.LineColor = Color.FromArgb(0, 184, 148);
        sepValidationTitle.Location = new Point(28, 54);
        sepValidationTitle.Name = "sepValidationTitle";
        sepValidationTitle.Size = new Size(790, 18);
        sepValidationTitle.TabIndex = 2;
        //
        // btnValidateProfile
        //
        btnValidateProfile.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnValidateProfile.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnValidateProfile.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnValidateProfile.Appearance.ForeColor = Color.White;
        btnValidateProfile.Appearance.Options.UseBackColor = true;
        btnValidateProfile.Appearance.Options.UseBorderColor = true;
        btnValidateProfile.Appearance.Options.UseFont = true;
        btnValidateProfile.Appearance.Options.UseForeColor = true;
        btnValidateProfile.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnValidateProfile.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnValidateProfile.AppearanceHovered.ForeColor = Color.White;
        btnValidateProfile.AppearanceHovered.Options.UseBackColor = true;
        btnValidateProfile.AppearanceHovered.Options.UseBorderColor = true;
        btnValidateProfile.AppearanceHovered.Options.UseForeColor = true;
        btnValidateProfile.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnValidateProfile.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnValidateProfile.AppearancePressed.ForeColor = Color.White;
        btnValidateProfile.AppearancePressed.Options.UseBackColor = true;
        btnValidateProfile.AppearancePressed.Options.UseBorderColor = true;
        btnValidateProfile.AppearancePressed.Options.UseForeColor = true;
        btnValidateProfile.ButtonKind = NuanActionButtonKind.Save;
        btnValidateProfile.ButtonStyle = BorderStyles.UltraFlat;
        btnValidateProfile.ButtonText = "Validar";
        btnValidateProfile.IconNameOverride = "aprobar_16.svg";
        btnValidateProfile.IconSize = 16;
        btnValidateProfile.ImageOptions.ImageToTextIndent = 0;
        btnValidateProfile.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnValidateProfile.ImageOptions.SvgImageSize = new Size(16, 16);
        btnValidateProfile.Location = new Point(28, 96);
        btnValidateProfile.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnValidateProfile.LookAndFeel.UseDefaultLookAndFeel = false;
        btnValidateProfile.Name = "btnValidateProfile";
        btnValidateProfile.Size = new Size(100, 26);
        btnValidateProfile.TabIndex = 3;
        btnValidateProfile.Text = "Validar";
        btnValidateProfile.UseDefaultSize = true;
        //
        // lblValidationDescription
        //
        lblValidationDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblValidationDescription.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblValidationDescription.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblValidationDescription.Appearance.Options.UseFont = true;
        lblValidationDescription.Appearance.Options.UseForeColor = true;
        lblValidationDescription.AutoSizeMode = LabelAutoSizeMode.None;
        lblValidationDescription.Location = new Point(148, 91);
        lblValidationDescription.Name = "lblValidationDescription";
        lblValidationDescription.Size = new Size(670, 36);
        lblValidationDescription.TabIndex = 4;
        lblValidationDescription.Text = "La validación revisa datos generales, sucursales, entidades, distribución, programación y conflictos de enrutamiento.";
        //
        // lblValidationResultCaption
        //
        lblValidationResultCaption.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblValidationResultCaption.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblValidationResultCaption.Appearance.Options.UseFont = true;
        lblValidationResultCaption.Appearance.Options.UseForeColor = true;
        lblValidationResultCaption.Location = new Point(48, 172);
        lblValidationResultCaption.Name = "lblValidationResultCaption";
        lblValidationResultCaption.Size = new Size(55, 15);
        lblValidationResultCaption.TabIndex = 6;
        lblValidationResultCaption.Text = "Resultado:";
        //
        // lblValidationResultValue
        //
        lblValidationResultValue.Appearance.BackColor = Color.FromArgb(247, 248, 252);
        lblValidationResultValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblValidationResultValue.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblValidationResultValue.Appearance.Options.UseBackColor = true;
        lblValidationResultValue.Appearance.Options.UseFont = true;
        lblValidationResultValue.Appearance.Options.UseForeColor = true;
        lblValidationResultValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblValidationResultValue.Location = new Point(120, 164);
        lblValidationResultValue.Name = "lblValidationResultValue";
        lblValidationResultValue.Padding = new Padding(10, 0, 10, 0);
        lblValidationResultValue.Size = new Size(152, 30);
        lblValidationResultValue.TabIndex = 7;
        lblValidationResultValue.Text = "Pendiente de validar";
        //
        // lblValidationErrorsCaption
        //
        lblValidationErrorsCaption.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblValidationErrorsCaption.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblValidationErrorsCaption.Appearance.Options.UseFont = true;
        lblValidationErrorsCaption.Appearance.Options.UseForeColor = true;
        lblValidationErrorsCaption.Location = new Point(330, 172);
        lblValidationErrorsCaption.Name = "lblValidationErrorsCaption";
        lblValidationErrorsCaption.Size = new Size(39, 15);
        lblValidationErrorsCaption.TabIndex = 8;
        lblValidationErrorsCaption.Text = "Errores:";
        //
        // lblValidationErrorsValue
        //
        lblValidationErrorsValue.Appearance.BackColor = Color.FromArgb(247, 248, 252);
        lblValidationErrorsValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblValidationErrorsValue.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblValidationErrorsValue.Appearance.Options.UseBackColor = true;
        lblValidationErrorsValue.Appearance.Options.UseFont = true;
        lblValidationErrorsValue.Appearance.Options.UseForeColor = true;
        lblValidationErrorsValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblValidationErrorsValue.Location = new Point(386, 164);
        lblValidationErrorsValue.Name = "lblValidationErrorsValue";
        lblValidationErrorsValue.Size = new Size(38, 30);
        lblValidationErrorsValue.TabIndex = 9;
        lblValidationErrorsValue.Text = "0";
        //
        // lblValidationWarningsCaption
        //
        lblValidationWarningsCaption.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblValidationWarningsCaption.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblValidationWarningsCaption.Appearance.Options.UseFont = true;
        lblValidationWarningsCaption.Appearance.Options.UseForeColor = true;
        lblValidationWarningsCaption.Location = new Point(486, 172);
        lblValidationWarningsCaption.Name = "lblValidationWarningsCaption";
        lblValidationWarningsCaption.Size = new Size(71, 15);
        lblValidationWarningsCaption.TabIndex = 10;
        lblValidationWarningsCaption.Text = "Advertencias:";
        //
        // lblValidationWarningsValue
        //
        lblValidationWarningsValue.Appearance.BackColor = Color.FromArgb(247, 248, 252);
        lblValidationWarningsValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblValidationWarningsValue.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblValidationWarningsValue.Appearance.Options.UseBackColor = true;
        lblValidationWarningsValue.Appearance.Options.UseFont = true;
        lblValidationWarningsValue.Appearance.Options.UseForeColor = true;
        lblValidationWarningsValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblValidationWarningsValue.Location = new Point(576, 164);
        lblValidationWarningsValue.Name = "lblValidationWarningsValue";
        lblValidationWarningsValue.Size = new Size(38, 30);
        lblValidationWarningsValue.TabIndex = 11;
        lblValidationWarningsValue.Text = "0";
        //
        // lblValidationResultsTitle
        //
        lblValidationResultsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblValidationResultsTitle.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        lblValidationResultsTitle.Appearance.Options.UseFont = true;
        lblValidationResultsTitle.Appearance.Options.UseForeColor = true;
        lblValidationResultsTitle.Location = new Point(28, 228);
        lblValidationResultsTitle.Name = "lblValidationResultsTitle";
        lblValidationResultsTitle.Size = new Size(184, 20);
        lblValidationResultsTitle.TabIndex = 12;
        lblValidationResultsTitle.Text = "Resultados de la validación";
        //
        // grdValidationResults
        //
        grdValidationResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdValidationResults.FormKey = "sync-profile-validation";
        grdValidationResults.GridName = "ValidationResultsGrid";
        grdValidationResults.Location = new Point(28, 254);
        grdValidationResults.Name = "grdValidationResults";
        grdValidationResults.ShowFindPanel = false;
        grdValidationResults.ShowPagination = false;
        grdValidationResults.Size = new Size(790, 282);
        grdValidationResults.TabIndex = 13;
        //
        // lblValidationInfo
        //
        lblValidationInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblValidationInfo.Appearance.BackColor = Color.FromArgb(247, 248, 252);
        lblValidationInfo.Appearance.BorderColor = Color.FromArgb(221, 226, 240);
        lblValidationInfo.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblValidationInfo.Appearance.ForeColor = Color.FromArgb(37, 99, 235);
        lblValidationInfo.Appearance.Options.UseBackColor = true;
        lblValidationInfo.Appearance.Options.UseBorderColor = true;
        lblValidationInfo.Appearance.Options.UseFont = true;
        lblValidationInfo.Appearance.Options.UseForeColor = true;
        lblValidationInfo.AutoSizeMode = LabelAutoSizeMode.None;
        lblValidationInfo.BorderStyle = BorderStyles.Simple;
        lblValidationInfo.Location = new Point(28, 554);
        lblValidationInfo.Name = "lblValidationInfo";
        lblValidationInfo.Padding = new Padding(12, 0, 8, 0);
        lblValidationInfo.Size = new Size(790, 42);
        lblValidationInfo.TabIndex = 14;
        lblValidationInfo.Text = "Los errores deben corregirse antes de activar o ejecutar el perfil. Las advertencias permiten continuar, pero deben revisarse.";
        //
        // lblValidationSummarySurface
        //
        lblValidationSummarySurface.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblValidationSummarySurface.Appearance.BackColor = Color.White;
        lblValidationSummarySurface.Appearance.BorderColor = Color.FromArgb(221, 226, 240);
        lblValidationSummarySurface.Appearance.Options.UseBackColor = true;
        lblValidationSummarySurface.Appearance.Options.UseBorderColor = true;
        lblValidationSummarySurface.AutoSizeMode = LabelAutoSizeMode.None;
        lblValidationSummarySurface.BorderStyle = BorderStyles.Simple;
        lblValidationSummarySurface.Location = new Point(28, 148);
        lblValidationSummarySurface.Name = "lblValidationSummarySurface";
        lblValidationSummarySurface.Size = new Size(790, 62);
        lblValidationSummarySurface.TabIndex = 5;
        //
        // pageExecutions
        //
        pageExecutions.Caption = "Ejecuciones";
        pageExecutions.Controls.Add(lblExecutionsTitle);
        pageExecutions.Controls.Add(sepExecutionsTitle);
        pageExecutions.Controls.Add(btnViewExecutionDetail);
        pageExecutions.Controls.Add(btnCancelExecution);
        pageExecutions.Controls.Add(btnRetryExecution);
        pageExecutions.Controls.Add(btnRefreshExecutions);
        pageExecutions.Controls.Add(lblExecutionsAutoRefresh);
        pageExecutions.Controls.Add(lblExecutionsHistoryTitle);
        pageExecutions.Controls.Add(grdExecutions);
        pageExecutions.Controls.Add(btnExecutionsFirstPage);
        pageExecutions.Controls.Add(btnExecutionsPreviousPage);
        pageExecutions.Controls.Add(lblExecutionsPageInfo);
        pageExecutions.Controls.Add(btnExecutionsNextPage);
        pageExecutions.Controls.Add(btnExecutionsLastPage);
        pageExecutions.Controls.Add(lblExecutionsTotal);
        pageExecutions.Controls.Add(lblExecutionsPageSize);
        pageExecutions.Controls.Add(cboExecutionsPageSize);
        pageExecutions.Controls.Add(lblExecutionsInfo);
        pageExecutions.Font = new Font("Segoe UI", 9F);
        pageExecutions.Name = "pageExecutions";
        pageExecutions.Size = new Size(851, 637);
        //
        // lblExecutionsTitle
        //
        lblExecutionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblExecutionsTitle.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        lblExecutionsTitle.Appearance.Options.UseFont = true;
        lblExecutionsTitle.Appearance.Options.UseForeColor = true;
        lblExecutionsTitle.Location = new Point(28, 22);
        lblExecutionsTitle.Name = "lblExecutionsTitle";
        lblExecutionsTitle.Size = new Size(144, 20);
        lblExecutionsTitle.TabIndex = 0;
        lblExecutionsTitle.Text = "Ejecuciones del perfil";
        //
        // sepExecutionsTitle
        //
        sepExecutionsTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepExecutionsTitle.LineColor = Color.FromArgb(0, 184, 148);
        sepExecutionsTitle.Location = new Point(28, 54);
        sepExecutionsTitle.Name = "sepExecutionsTitle";
        sepExecutionsTitle.Size = new Size(790, 18);
        sepExecutionsTitle.TabIndex = 2;
        //
        // btnViewExecutionDetail
        //
        btnViewExecutionDetail.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnViewExecutionDetail.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnViewExecutionDetail.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnViewExecutionDetail.Appearance.ForeColor = Color.White;
        btnViewExecutionDetail.Appearance.Options.UseBackColor = true;
        btnViewExecutionDetail.Appearance.Options.UseBorderColor = true;
        btnViewExecutionDetail.Appearance.Options.UseFont = true;
        btnViewExecutionDetail.Appearance.Options.UseForeColor = true;
        btnViewExecutionDetail.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnViewExecutionDetail.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnViewExecutionDetail.AppearanceHovered.ForeColor = Color.White;
        btnViewExecutionDetail.AppearanceHovered.Options.UseBackColor = true;
        btnViewExecutionDetail.AppearanceHovered.Options.UseBorderColor = true;
        btnViewExecutionDetail.AppearanceHovered.Options.UseForeColor = true;
        btnViewExecutionDetail.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnViewExecutionDetail.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnViewExecutionDetail.AppearancePressed.ForeColor = Color.White;
        btnViewExecutionDetail.AppearancePressed.Options.UseBackColor = true;
        btnViewExecutionDetail.AppearancePressed.Options.UseBorderColor = true;
        btnViewExecutionDetail.AppearancePressed.Options.UseForeColor = true;
        btnViewExecutionDetail.ButtonKind = NuanActionButtonKind.Save;
        btnViewExecutionDetail.ButtonStyle = BorderStyles.UltraFlat;
        btnViewExecutionDetail.ButtonText = "Ver detalle";
        btnViewExecutionDetail.IconNameOverride = "ver_detalle_16.svg";
        btnViewExecutionDetail.IconSize = 16;
        btnViewExecutionDetail.ImageOptions.ImageToTextIndent = 0;
        btnViewExecutionDetail.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnViewExecutionDetail.ImageOptions.SvgImageSize = new Size(16, 16);
        btnViewExecutionDetail.Location = new Point(28, 96);
        btnViewExecutionDetail.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnViewExecutionDetail.LookAndFeel.UseDefaultLookAndFeel = false;
        btnViewExecutionDetail.Name = "btnViewExecutionDetail";
        btnViewExecutionDetail.Size = new Size(100, 26);
        btnViewExecutionDetail.TabIndex = 3;
        btnViewExecutionDetail.Text = "Ver detalle";
        //
        // btnCancelExecution
        //
        btnCancelExecution.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelExecution.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnCancelExecution.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancelExecution.Appearance.ForeColor = Color.White;
        btnCancelExecution.Appearance.Options.UseBackColor = true;
        btnCancelExecution.Appearance.Options.UseBorderColor = true;
        btnCancelExecution.Appearance.Options.UseFont = true;
        btnCancelExecution.Appearance.Options.UseForeColor = true;
        btnCancelExecution.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelExecution.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnCancelExecution.AppearanceHovered.ForeColor = Color.White;
        btnCancelExecution.AppearanceHovered.Options.UseBackColor = true;
        btnCancelExecution.AppearanceHovered.Options.UseBorderColor = true;
        btnCancelExecution.AppearanceHovered.Options.UseForeColor = true;
        btnCancelExecution.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnCancelExecution.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnCancelExecution.AppearancePressed.ForeColor = Color.White;
        btnCancelExecution.AppearancePressed.Options.UseBackColor = true;
        btnCancelExecution.AppearancePressed.Options.UseBorderColor = true;
        btnCancelExecution.AppearancePressed.Options.UseForeColor = true;
        btnCancelExecution.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancelExecution.ButtonStyle = BorderStyles.UltraFlat;
        btnCancelExecution.ButtonText = "Cancelar";
        btnCancelExecution.IconNameOverride = "cancelar_16.svg";
        btnCancelExecution.IconSize = 16;
        btnCancelExecution.ImageOptions.ImageToTextIndent = 0;
        btnCancelExecution.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnCancelExecution.ImageOptions.SvgImageSize = new Size(16, 16);
        btnCancelExecution.Location = new Point(136, 96);
        btnCancelExecution.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelExecution.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancelExecution.Name = "btnCancelExecution";
        btnCancelExecution.Size = new Size(100, 26);
        btnCancelExecution.TabIndex = 4;
        btnCancelExecution.Text = "Cancelar";
        //
        // btnRetryExecution
        //
        btnRetryExecution.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnRetryExecution.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnRetryExecution.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRetryExecution.Appearance.ForeColor = Color.White;
        btnRetryExecution.Appearance.Options.UseBackColor = true;
        btnRetryExecution.Appearance.Options.UseBorderColor = true;
        btnRetryExecution.Appearance.Options.UseFont = true;
        btnRetryExecution.Appearance.Options.UseForeColor = true;
        btnRetryExecution.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnRetryExecution.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnRetryExecution.AppearanceHovered.ForeColor = Color.White;
        btnRetryExecution.AppearanceHovered.Options.UseBackColor = true;
        btnRetryExecution.AppearanceHovered.Options.UseBorderColor = true;
        btnRetryExecution.AppearanceHovered.Options.UseForeColor = true;
        btnRetryExecution.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnRetryExecution.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnRetryExecution.AppearancePressed.ForeColor = Color.White;
        btnRetryExecution.AppearancePressed.Options.UseBackColor = true;
        btnRetryExecution.AppearancePressed.Options.UseBorderColor = true;
        btnRetryExecution.AppearancePressed.Options.UseForeColor = true;
        btnRetryExecution.ButtonKind = NuanActionButtonKind.Save;
        btnRetryExecution.ButtonStyle = BorderStyles.UltraFlat;
        btnRetryExecution.ButtonText = "Reintentar";
        btnRetryExecution.IconNameOverride = "actualizar_16.svg";
        btnRetryExecution.IconSize = 16;
        btnRetryExecution.ImageOptions.ImageToTextIndent = 0;
        btnRetryExecution.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnRetryExecution.ImageOptions.SvgImageSize = new Size(16, 16);
        btnRetryExecution.Location = new Point(244, 96);
        btnRetryExecution.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnRetryExecution.LookAndFeel.UseDefaultLookAndFeel = false;
        btnRetryExecution.Name = "btnRetryExecution";
        btnRetryExecution.Size = new Size(100, 26);
        btnRetryExecution.TabIndex = 5;
        btnRetryExecution.Text = "Reintentar";
        //
        // btnRefreshExecutions
        //
        btnRefreshExecutions.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnRefreshExecutions.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnRefreshExecutions.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRefreshExecutions.Appearance.ForeColor = Color.White;
        btnRefreshExecutions.Appearance.Options.UseBackColor = true;
        btnRefreshExecutions.Appearance.Options.UseBorderColor = true;
        btnRefreshExecutions.Appearance.Options.UseFont = true;
        btnRefreshExecutions.Appearance.Options.UseForeColor = true;
        btnRefreshExecutions.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnRefreshExecutions.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnRefreshExecutions.AppearanceHovered.ForeColor = Color.White;
        btnRefreshExecutions.AppearanceHovered.Options.UseBackColor = true;
        btnRefreshExecutions.AppearanceHovered.Options.UseBorderColor = true;
        btnRefreshExecutions.AppearanceHovered.Options.UseForeColor = true;
        btnRefreshExecutions.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnRefreshExecutions.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnRefreshExecutions.AppearancePressed.ForeColor = Color.White;
        btnRefreshExecutions.AppearancePressed.Options.UseBackColor = true;
        btnRefreshExecutions.AppearancePressed.Options.UseBorderColor = true;
        btnRefreshExecutions.AppearancePressed.Options.UseForeColor = true;
        btnRefreshExecutions.ButtonKind = NuanActionButtonKind.Save;
        btnRefreshExecutions.ButtonStyle = BorderStyles.UltraFlat;
        btnRefreshExecutions.ButtonText = "Actualizar";
        btnRefreshExecutions.IconNameOverride = "actualizar_16.svg";
        btnRefreshExecutions.IconSize = 16;
        btnRefreshExecutions.ImageOptions.ImageToTextIndent = 0;
        btnRefreshExecutions.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnRefreshExecutions.ImageOptions.SvgImageSize = new Size(16, 16);
        btnRefreshExecutions.Location = new Point(352, 96);
        btnRefreshExecutions.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnRefreshExecutions.LookAndFeel.UseDefaultLookAndFeel = false;
        btnRefreshExecutions.Name = "btnRefreshExecutions";
        btnRefreshExecutions.Size = new Size(100, 26);
        btnRefreshExecutions.TabIndex = 6;
        btnRefreshExecutions.Text = "Actualizar";
        //
        // lblExecutionsAutoRefresh
        //
        lblExecutionsAutoRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblExecutionsAutoRefresh.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblExecutionsAutoRefresh.Appearance.ForeColor = Color.FromArgb(22, 163, 74);
        lblExecutionsAutoRefresh.Appearance.Options.UseFont = true;
        lblExecutionsAutoRefresh.Appearance.Options.UseForeColor = true;
        lblExecutionsAutoRefresh.Appearance.Options.UseTextOptions = true;
        lblExecutionsAutoRefresh.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblExecutionsAutoRefresh.AutoSizeMode = LabelAutoSizeMode.None;
        lblExecutionsAutoRefresh.Location = new Point(608, 100);
        lblExecutionsAutoRefresh.Name = "lblExecutionsAutoRefresh";
        lblExecutionsAutoRefresh.Size = new Size(210, 20);
        lblExecutionsAutoRefresh.TabIndex = 7;
        lblExecutionsAutoRefresh.Text = "Actualización automática activa";
        //
        // lblExecutionsHistoryTitle
        //
        lblExecutionsHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblExecutionsHistoryTitle.Appearance.ForeColor = Color.FromArgb(0, 161, 132);
        lblExecutionsHistoryTitle.Appearance.Options.UseFont = true;
        lblExecutionsHistoryTitle.Appearance.Options.UseForeColor = true;
        lblExecutionsHistoryTitle.Location = new Point(28, 138);
        lblExecutionsHistoryTitle.Name = "lblExecutionsHistoryTitle";
        lblExecutionsHistoryTitle.Size = new Size(161, 20);
        lblExecutionsHistoryTitle.TabIndex = 8;
        lblExecutionsHistoryTitle.Text = "Historial de ejecuciones";
        //
        // grdExecutions
        //
        grdExecutions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdExecutions.FormKey = "sync-profile-executions";
        grdExecutions.GridName = "ProfileExecutionsGrid";
        grdExecutions.Location = new Point(28, 164);
        grdExecutions.Name = "grdExecutions";
        grdExecutions.ShowFindPanel = false;
        grdExecutions.ShowPagination = false;
        grdExecutions.Size = new Size(790, 326);
        grdExecutions.TabIndex = 9;
        //
        // btnExecutionsFirstPage
        //
        btnExecutionsFirstPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnExecutionsFirstPage.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnExecutionsFirstPage.Appearance.Options.UseFont = true;
        btnExecutionsFirstPage.Location = new Point(28, 500);
        btnExecutionsFirstPage.Name = "btnExecutionsFirstPage";
        btnExecutionsFirstPage.Size = new Size(36, 28);
        btnExecutionsFirstPage.TabIndex = 10;
        btnExecutionsFirstPage.Text = "|<";
        //
        // btnExecutionsPreviousPage
        //
        btnExecutionsPreviousPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnExecutionsPreviousPage.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnExecutionsPreviousPage.Appearance.Options.UseFont = true;
        btnExecutionsPreviousPage.Location = new Point(70, 500);
        btnExecutionsPreviousPage.Name = "btnExecutionsPreviousPage";
        btnExecutionsPreviousPage.Size = new Size(36, 28);
        btnExecutionsPreviousPage.TabIndex = 11;
        btnExecutionsPreviousPage.Text = "<";
        //
        // lblExecutionsPageInfo
        //
        lblExecutionsPageInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblExecutionsPageInfo.Appearance.Font = new Font("Segoe UI", 9F);
        lblExecutionsPageInfo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblExecutionsPageInfo.Appearance.Options.UseFont = true;
        lblExecutionsPageInfo.Appearance.Options.UseForeColor = true;
        lblExecutionsPageInfo.Location = new Point(120, 507);
        lblExecutionsPageInfo.Name = "lblExecutionsPageInfo";
        lblExecutionsPageInfo.Size = new Size(70, 15);
        lblExecutionsPageInfo.TabIndex = 12;
        lblExecutionsPageInfo.Text = "Página 1 de 1";
        //
        // btnExecutionsNextPage
        //
        btnExecutionsNextPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnExecutionsNextPage.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnExecutionsNextPage.Appearance.Options.UseFont = true;
        btnExecutionsNextPage.Location = new Point(210, 500);
        btnExecutionsNextPage.Name = "btnExecutionsNextPage";
        btnExecutionsNextPage.Size = new Size(36, 28);
        btnExecutionsNextPage.TabIndex = 13;
        btnExecutionsNextPage.Text = ">";
        //
        // btnExecutionsLastPage
        //
        btnExecutionsLastPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnExecutionsLastPage.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnExecutionsLastPage.Appearance.Options.UseFont = true;
        btnExecutionsLastPage.Location = new Point(252, 500);
        btnExecutionsLastPage.Name = "btnExecutionsLastPage";
        btnExecutionsLastPage.Size = new Size(36, 28);
        btnExecutionsLastPage.TabIndex = 14;
        btnExecutionsLastPage.Text = ">|";
        //
        // lblExecutionsTotal
        //
        lblExecutionsTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblExecutionsTotal.Appearance.Font = new Font("Segoe UI", 9F);
        lblExecutionsTotal.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblExecutionsTotal.Appearance.Options.UseFont = true;
        lblExecutionsTotal.Appearance.Options.UseForeColor = true;
        lblExecutionsTotal.Location = new Point(314, 507);
        lblExecutionsTotal.Name = "lblExecutionsTotal";
        lblExecutionsTotal.Size = new Size(104, 15);
        lblExecutionsTotal.TabIndex = 15;
        lblExecutionsTotal.Text = "Total: 0 ejecuciones";
        //
        // lblExecutionsPageSize
        //
        lblExecutionsPageSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblExecutionsPageSize.Appearance.Font = new Font("Segoe UI", 9F);
        lblExecutionsPageSize.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblExecutionsPageSize.Appearance.Options.UseFont = true;
        lblExecutionsPageSize.Appearance.Options.UseForeColor = true;
        lblExecutionsPageSize.Location = new Point(646, 507);
        lblExecutionsPageSize.Name = "lblExecutionsPageSize";
        lblExecutionsPageSize.Size = new Size(111, 15);
        lblExecutionsPageSize.TabIndex = 16;
        lblExecutionsPageSize.Text = "Registros por página:";
        //
        // cboExecutionsPageSize
        //
        cboExecutionsPageSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        cboExecutionsPageSize.EditValue = "25";
        cboExecutionsPageSize.Location = new Point(764, 503);
        cboExecutionsPageSize.Name = "cboExecutionsPageSize";
        cboExecutionsPageSize.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboExecutionsPageSize.Properties.Appearance.Options.UseFont = true;
        cboExecutionsPageSize.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboExecutionsPageSize.Properties.Items.AddRange(new object[] { "10", "25", "50", "100" });
        cboExecutionsPageSize.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboExecutionsPageSize.Size = new Size(54, 22);
        cboExecutionsPageSize.TabIndex = 17;
        //
        // lblExecutionsInfo
        //
        lblExecutionsInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblExecutionsInfo.Appearance.BackColor = Color.FromArgb(247, 248, 252);
        lblExecutionsInfo.Appearance.BorderColor = Color.FromArgb(221, 226, 240);
        lblExecutionsInfo.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblExecutionsInfo.Appearance.ForeColor = Color.FromArgb(37, 99, 235);
        lblExecutionsInfo.Appearance.Options.UseBackColor = true;
        lblExecutionsInfo.Appearance.Options.UseBorderColor = true;
        lblExecutionsInfo.Appearance.Options.UseFont = true;
        lblExecutionsInfo.Appearance.Options.UseForeColor = true;
        lblExecutionsInfo.AutoSizeMode = LabelAutoSizeMode.None;
        lblExecutionsInfo.BorderStyle = BorderStyles.Simple;
        lblExecutionsInfo.Location = new Point(28, 548);
        lblExecutionsInfo.Name = "lblExecutionsInfo";
        lblExecutionsInfo.Padding = new Padding(12, 0, 8, 0);
        lblExecutionsInfo.Size = new Size(790, 42);
        lblExecutionsInfo.TabIndex = 18;
        lblExecutionsInfo.Text = "Las ejecuciones activas se actualizan automáticamente. Utilice Ver detalle para consultar el resultado por entidad.";
        //
        // colBranchCompanyCode
        //
        colBranchCompanyCode.Caption = "Código empresa";
        colBranchCompanyCode.FieldName = "BranchCompanyCode";
        colBranchCompanyCode.Name = "colBranchCompanyCode";
        colBranchCompanyCode.Visible = true;
        colBranchCompanyCode.VisibleIndex = 0;
        colBranchCompanyCode.Width = 115;
        //
        // colBranchCode
        //
        colBranchCode.Caption = "Código sucursal";
        colBranchCode.FieldName = "BranchCode";
        colBranchCode.Name = "colBranchCode";
        colBranchCode.Visible = true;
        colBranchCode.VisibleIndex = 1;
        colBranchCode.Width = 110;
        //
        // colBranchName
        //
        colBranchName.Caption = "Nombre sucursal";
        colBranchName.FieldName = "BranchCompanyName";
        colBranchName.Name = "colBranchName";
        colBranchName.Visible = true;
        colBranchName.VisibleIndex = 2;
        colBranchName.Width = 170;
        //
        // colBranchDatabaseName
        //
        colBranchDatabaseName.Caption = "Base de datos";
        colBranchDatabaseName.FieldName = "BranchDatabaseName";
        colBranchDatabaseName.Name = "colBranchDatabaseName";
        colBranchDatabaseName.Visible = true;
        colBranchDatabaseName.VisibleIndex = 3;
        colBranchDatabaseName.Width = 140;
        //
        // colBranchStatus
        //
        colBranchStatus.Caption = "Activo en perfil";
        colBranchStatus.FieldName = "StatusText";
        colBranchStatus.Name = "colBranchStatus";
        colBranchStatus.Visible = true;
        colBranchStatus.VisibleIndex = 4;
        colBranchStatus.Width = 115;
        //
        // colBranchBatchSize
        //
        colBranchBatchSize.Caption = "Batch";
        colBranchBatchSize.FieldName = "BatchSize";
        colBranchBatchSize.Name = "colBranchBatchSize";
        colBranchBatchSize.Visible = true;
        colBranchBatchSize.VisibleIndex = 5;
        colBranchBatchSize.Width = 70;
        //
        // colBranchMaxRetries
        //
        colBranchMaxRetries.Caption = "Reintentos";
        colBranchMaxRetries.FieldName = "MaxRetries";
        colBranchMaxRetries.Name = "colBranchMaxRetries";
        colBranchMaxRetries.Visible = true;
        colBranchMaxRetries.VisibleIndex = 6;
        colBranchMaxRetries.Width = 86;
        //
        // colBranchLastSynchronizationAt
        //
        colBranchLastSynchronizationAt.Caption = "Última sincronización";
        colBranchLastSynchronizationAt.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
        colBranchLastSynchronizationAt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        colBranchLastSynchronizationAt.FieldName = "LastSynchronizationAt";
        colBranchLastSynchronizationAt.Name = "colBranchLastSynchronizationAt";
        colBranchLastSynchronizationAt.Visible = true;
        colBranchLastSynchronizationAt.VisibleIndex = 7;
        colBranchLastSynchronizationAt.Width = 150;
        //
        // SyncProfileEditForm
        //
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1050, 715);
        Controls.Add(navigationFrame);
        Controls.Add(accordionNavigation);
        Margin = new Padding(4, 3, 4, 3);
        MinimumSize = new Size(957, 600);
        Name = "SyncProfileEditForm";
        Text = "Perfil de sincronizacion";
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(accordionNavigation, 0);
        Controls.SetChildIndex(navigationFrame, 0);
        ((System.ComponentModel.ISupportInitialize)accordionNavigation).EndInit();
        ((System.ComponentModel.ISupportInitialize)navigationFrame).EndInit();
        navigationFrame.ResumeLayout(false);
        pageGeneral.ResumeLayout(false);
        pageGeneral.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sepGeneralTitle).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtMasterCompany.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboDirection.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboExecutionMode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtConflictStrategy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepExecutionParameters).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnBatchSize.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxRetries.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnRetryDelaySeconds.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnTimeoutMinutes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepStatus).EndInit();
        ((System.ComponentModel.ISupportInitialize)swIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)swSapCodePolicyEnabled.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboSapPrefixMode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPassportIdentificationTypeCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlBusinessPartnerCodePolicy).EndInit();
        pnlBusinessPartnerCodePolicy.ResumeLayout(false);
        pnlBusinessPartnerCodePolicy.PerformLayout();
        pageBranches.ResumeLayout(false);
        pageBranches.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sepBranchesTitle).EndInit();
        pageEntities.ResumeLayout(false);
        pageEntities.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sepEntitiesTitle).EndInit();
        pageDistribution.ResumeLayout(false);
        pageDistribution.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sepDistributionTitle).EndInit();
        pageSchedule.ResumeLayout(false);
        pageSchedule.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sepScheduleTitle).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepScheduleConfiguration).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboScheduleType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnScheduleIntervalMinutes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)timScheduleExecutionTime.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboScheduleTimeZone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)swPreventConcurrentExecutions.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)swScheduleIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepScheduleStatus).EndInit();
        pageValidation.ResumeLayout(false);
        pageValidation.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sepValidationTitle).EndInit();
        pageExecutions.ResumeLayout(false);
        pageExecutions.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sepExecutionsTitle).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboExecutionsPageSize.Properties).EndInit();
        ResumeLayout(false);
    }
}
