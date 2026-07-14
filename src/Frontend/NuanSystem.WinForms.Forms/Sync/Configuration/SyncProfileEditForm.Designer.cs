using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class SyncProfileEditForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayout;
    private XtraTabControl tabs;
    private XtraTabPage tabGeneral;
    private XtraTabPage tabBranches;
    private XtraTabPage tabEntities;
    private XtraTabPage tabMatrix;
    private XtraTabPage tabSchedule;
    private TableLayoutPanel generalLayout;
    private LabelControl lblCode;
    private TextEdit codeEdit;
    private LabelControl lblName;
    private TextEdit nameEdit;
    private LabelControl lblCompany;
    private LookUpEdit companyEdit;
    private LabelControl lblDirection;
    private ComboBoxEdit directionEdit;
    private LabelControl lblExecutionMode;
    private ComboBoxEdit executionModeEdit;
    private LabelControl lblConflictStrategy;
    private ComboBoxEdit conflictStrategyEdit;
    private LabelControl lblBatchSize;
    private SpinEdit batchSizeEdit;
    private LabelControl lblMaxRetries;
    private SpinEdit maxRetriesEdit;
    private LabelControl lblRetryDelay;
    private SpinEdit retryDelayEdit;
    private LabelControl lblTimeout;
    private SpinEdit timeoutEdit;
    private CheckEdit activeEdit;
    private LabelControl lblDescription;
    private MemoEdit descriptionEdit;
    private TableLayoutPanel branchesLayout;
    private FlowLayoutPanel branchesActionsPanel;
    private LookUpEdit branchLookup;
    private SimpleButton btnAddBranch;
    private SimpleButton btnRemoveBranch;
    private GridControl branchesGrid;
    private GridView branchesView;
    private GridColumn colBranchCompanyId;
    private GridColumn colBranchDisplay;
    private GridColumn colBranchBatchSize;
    private GridColumn colBranchMaxRetries;
    private GridColumn colBranchIsActive;
    private TableLayoutPanel entitiesLayout;
    private FlowLayoutPanel entitiesActionsPanel;
    private LookUpEdit entityLookup;
    private SimpleButton btnAddEntity;
    private SimpleButton btnRemoveEntity;
    private GridControl entitiesGrid;
    private GridView entitiesView;
    private GridColumn colEntityCode;
    private GridColumn colEntityName;
    private GridColumn colEntityExecutionOrder;
    private GridColumn colEntitySyncMode;
    private GridColumn colEntityAllowInsert;
    private GridColumn colEntityAllowUpdate;
    private GridColumn colEntityAllowDeactivate;
    private GridColumn colEntityIsActive;
    private GridControl matrixGrid;
    private GridView matrixView;
    private GridColumn colMatrixEntityCode;
    private GridColumn colMatrixBranchCompanyId;
    private GridColumn colMatrixIsEnabled;
    private GridColumn colMatrixBatchSize;
    private TableLayoutPanel scheduleLayout;
    private LabelControl lblScheduleType;
    private ComboBoxEdit scheduleTypeEdit;
    private LabelControl lblInterval;
    private SpinEdit intervalEdit;
    private LabelControl lblExecutionTime;
    private TimeEdit executionTimeEdit;
    private LabelControl lblTimeZone;
    private TextEdit timeZoneEdit;
    private CheckEdit preventConcurrentEdit;
    private CheckEdit scheduleActiveEdit;
    private FlowLayoutPanel footerPanel;
    private SimpleButton btnSave;
    private SimpleButton btnCancel;
    private SimpleButton btnValidate;

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
        rootLayout = new TableLayoutPanel();
        tabs = new XtraTabControl();
        tabGeneral = new XtraTabPage();
        generalLayout = new TableLayoutPanel();
        lblCode = new LabelControl();
        codeEdit = new TextEdit();
        lblName = new LabelControl();
        nameEdit = new TextEdit();
        lblCompany = new LabelControl();
        companyEdit = new LookUpEdit();
        lblDirection = new LabelControl();
        directionEdit = new ComboBoxEdit();
        lblExecutionMode = new LabelControl();
        executionModeEdit = new ComboBoxEdit();
        lblConflictStrategy = new LabelControl();
        conflictStrategyEdit = new ComboBoxEdit();
        lblBatchSize = new LabelControl();
        batchSizeEdit = new SpinEdit();
        lblMaxRetries = new LabelControl();
        maxRetriesEdit = new SpinEdit();
        lblRetryDelay = new LabelControl();
        retryDelayEdit = new SpinEdit();
        lblTimeout = new LabelControl();
        timeoutEdit = new SpinEdit();
        activeEdit = new CheckEdit();
        lblDescription = new LabelControl();
        descriptionEdit = new MemoEdit();
        tabBranches = new XtraTabPage();
        branchesLayout = new TableLayoutPanel();
        branchesActionsPanel = new FlowLayoutPanel();
        branchLookup = new LookUpEdit();
        btnAddBranch = new SimpleButton();
        btnRemoveBranch = new SimpleButton();
        branchesGrid = new GridControl();
        branchesView = new GridView();
        colBranchCompanyId = new GridColumn();
        colBranchDisplay = new GridColumn();
        colBranchBatchSize = new GridColumn();
        colBranchMaxRetries = new GridColumn();
        colBranchIsActive = new GridColumn();
        tabEntities = new XtraTabPage();
        entitiesLayout = new TableLayoutPanel();
        entitiesActionsPanel = new FlowLayoutPanel();
        entityLookup = new LookUpEdit();
        btnAddEntity = new SimpleButton();
        btnRemoveEntity = new SimpleButton();
        entitiesGrid = new GridControl();
        entitiesView = new GridView();
        colEntityCode = new GridColumn();
        colEntityName = new GridColumn();
        colEntityExecutionOrder = new GridColumn();
        colEntitySyncMode = new GridColumn();
        colEntityAllowInsert = new GridColumn();
        colEntityAllowUpdate = new GridColumn();
        colEntityAllowDeactivate = new GridColumn();
        colEntityIsActive = new GridColumn();
        tabMatrix = new XtraTabPage();
        matrixGrid = new GridControl();
        matrixView = new GridView();
        colMatrixEntityCode = new GridColumn();
        colMatrixBranchCompanyId = new GridColumn();
        colMatrixIsEnabled = new GridColumn();
        colMatrixBatchSize = new GridColumn();
        tabSchedule = new XtraTabPage();
        scheduleLayout = new TableLayoutPanel();
        lblScheduleType = new LabelControl();
        scheduleTypeEdit = new ComboBoxEdit();
        lblInterval = new LabelControl();
        intervalEdit = new SpinEdit();
        lblExecutionTime = new LabelControl();
        executionTimeEdit = new TimeEdit();
        lblTimeZone = new LabelControl();
        timeZoneEdit = new TextEdit();
        preventConcurrentEdit = new CheckEdit();
        scheduleActiveEdit = new CheckEdit();
        footerPanel = new FlowLayoutPanel();
        btnSave = new SimpleButton();
        btnCancel = new SimpleButton();
        btnValidate = new SimpleButton();
        rootLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabs).BeginInit();
        tabs.SuspendLayout();
        tabGeneral.SuspendLayout();
        generalLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)codeEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nameEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)companyEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)directionEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)executionModeEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)conflictStrategyEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)batchSizeEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)maxRetriesEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)retryDelayEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)timeoutEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)activeEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)descriptionEdit.Properties).BeginInit();
        tabBranches.SuspendLayout();
        branchesLayout.SuspendLayout();
        branchesActionsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)branchLookup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)branchesGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)branchesView).BeginInit();
        tabEntities.SuspendLayout();
        entitiesLayout.SuspendLayout();
        entitiesActionsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)entityLookup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)entitiesGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)entitiesView).BeginInit();
        tabMatrix.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)matrixGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)matrixView).BeginInit();
        tabSchedule.SuspendLayout();
        scheduleLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)scheduleTypeEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)intervalEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)executionTimeEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)timeZoneEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)preventConcurrentEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)scheduleActiveEdit.Properties).BeginInit();
        footerPanel.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(tabs, 0, 0);
        rootLayout.Controls.Add(footerPanel, 0, 1);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(10);
        rootLayout.RowCount = 2;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        rootLayout.Size = new Size(891, 624);
        rootLayout.TabIndex = 0;
        // 
        // tabs
        // 
        tabs.Dock = DockStyle.Fill;
        tabs.Location = new Point(13, 13);
        tabs.Name = "tabs";
        tabs.SelectedTabPage = tabGeneral;
        tabs.Size = new Size(865, 560);
        tabs.TabIndex = 0;
        tabs.TabPages.AddRange(new XtraTabPage[] { tabGeneral, tabBranches, tabEntities, tabMatrix, tabSchedule });
        // 
        // tabGeneral
        // 
        tabGeneral.Controls.Add(generalLayout);
        tabGeneral.Name = "tabGeneral";
        tabGeneral.Size = new Size(863, 535);
        tabGeneral.Text = "General";
        // 
        // generalLayout
        // 
        generalLayout.ColumnCount = 4;
        generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 111F));
        generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 111F));
        generalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        generalLayout.Controls.Add(lblCode, 0, 0);
        generalLayout.Controls.Add(codeEdit, 1, 0);
        generalLayout.Controls.Add(lblName, 2, 0);
        generalLayout.Controls.Add(nameEdit, 3, 0);
        generalLayout.Controls.Add(lblCompany, 0, 1);
        generalLayout.Controls.Add(companyEdit, 1, 1);
        generalLayout.Controls.Add(lblDirection, 2, 1);
        generalLayout.Controls.Add(directionEdit, 3, 1);
        generalLayout.Controls.Add(lblExecutionMode, 0, 2);
        generalLayout.Controls.Add(executionModeEdit, 1, 2);
        generalLayout.Controls.Add(lblConflictStrategy, 2, 2);
        generalLayout.Controls.Add(conflictStrategyEdit, 3, 2);
        generalLayout.Controls.Add(lblBatchSize, 0, 3);
        generalLayout.Controls.Add(batchSizeEdit, 1, 3);
        generalLayout.Controls.Add(lblMaxRetries, 2, 3);
        generalLayout.Controls.Add(maxRetriesEdit, 3, 3);
        generalLayout.Controls.Add(lblRetryDelay, 0, 4);
        generalLayout.Controls.Add(retryDelayEdit, 1, 4);
        generalLayout.Controls.Add(lblTimeout, 2, 4);
        generalLayout.Controls.Add(timeoutEdit, 3, 4);
        generalLayout.Controls.Add(activeEdit, 1, 5);
        generalLayout.Controls.Add(lblDescription, 0, 6);
        generalLayout.Controls.Add(descriptionEdit, 1, 6);
        generalLayout.Dock = DockStyle.Fill;
        generalLayout.Location = new Point(0, 0);
        generalLayout.Name = "generalLayout";
        generalLayout.Padding = new Padding(10);
        generalLayout.RowCount = 8;
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        generalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
        generalLayout.Size = new Size(863, 535);
        generalLayout.TabIndex = 0;
        // 
        // lblCode
        // 
        lblCode.Dock = DockStyle.Fill;
        lblCode.Location = new Point(13, 13);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(105, 25);
        lblCode.TabIndex = 0;
        lblCode.Text = "Codigo";
        // 
        // codeEdit
        // 
        codeEdit.Dock = DockStyle.Fill;
        codeEdit.Location = new Point(124, 13);
        codeEdit.Name = "codeEdit";
        codeEdit.Size = new Size(304, 20);
        codeEdit.TabIndex = 1;
        // 
        // lblName
        // 
        lblName.Dock = DockStyle.Fill;
        lblName.Location = new Point(434, 13);
        lblName.Name = "lblName";
        lblName.Size = new Size(105, 25);
        lblName.TabIndex = 2;
        lblName.Text = "Nombre";
        // 
        // nameEdit
        // 
        nameEdit.Dock = DockStyle.Fill;
        nameEdit.Location = new Point(545, 13);
        nameEdit.Name = "nameEdit";
        nameEdit.Size = new Size(305, 20);
        nameEdit.TabIndex = 3;
        // 
        // lblCompany
        // 
        lblCompany.Dock = DockStyle.Fill;
        lblCompany.Location = new Point(13, 44);
        lblCompany.Name = "lblCompany";
        lblCompany.Size = new Size(105, 25);
        lblCompany.TabIndex = 4;
        lblCompany.Text = "Empresa maestra";
        // 
        // companyEdit
        // 
        companyEdit.Dock = DockStyle.Fill;
        companyEdit.Location = new Point(124, 44);
        companyEdit.Name = "companyEdit";
        companyEdit.Size = new Size(304, 20);
        companyEdit.TabIndex = 5;
        // 
        // lblDirection
        // 
        lblDirection.Dock = DockStyle.Fill;
        lblDirection.Location = new Point(434, 44);
        lblDirection.Name = "lblDirection";
        lblDirection.Size = new Size(105, 25);
        lblDirection.TabIndex = 6;
        lblDirection.Text = "Direccion";
        // 
        // directionEdit
        // 
        directionEdit.Dock = DockStyle.Fill;
        directionEdit.Location = new Point(545, 44);
        directionEdit.Name = "directionEdit";
        directionEdit.Size = new Size(305, 20);
        directionEdit.TabIndex = 7;
        // 
        // lblExecutionMode
        // 
        lblExecutionMode.Dock = DockStyle.Fill;
        lblExecutionMode.Location = new Point(13, 75);
        lblExecutionMode.Name = "lblExecutionMode";
        lblExecutionMode.Size = new Size(105, 25);
        lblExecutionMode.TabIndex = 8;
        lblExecutionMode.Text = "Modo";
        // 
        // executionModeEdit
        // 
        executionModeEdit.Dock = DockStyle.Fill;
        executionModeEdit.Location = new Point(124, 75);
        executionModeEdit.Name = "executionModeEdit";
        executionModeEdit.Size = new Size(304, 20);
        executionModeEdit.TabIndex = 9;
        // 
        // lblConflictStrategy
        // 
        lblConflictStrategy.Dock = DockStyle.Fill;
        lblConflictStrategy.Location = new Point(434, 75);
        lblConflictStrategy.Name = "lblConflictStrategy";
        lblConflictStrategy.Size = new Size(105, 25);
        lblConflictStrategy.TabIndex = 10;
        lblConflictStrategy.Text = "Conflicto";
        // 
        // conflictStrategyEdit
        // 
        conflictStrategyEdit.Dock = DockStyle.Fill;
        conflictStrategyEdit.Location = new Point(545, 75);
        conflictStrategyEdit.Name = "conflictStrategyEdit";
        conflictStrategyEdit.Size = new Size(305, 20);
        conflictStrategyEdit.TabIndex = 11;
        // 
        // lblBatchSize
        // 
        lblBatchSize.Dock = DockStyle.Fill;
        lblBatchSize.Location = new Point(13, 106);
        lblBatchSize.Name = "lblBatchSize";
        lblBatchSize.Size = new Size(105, 25);
        lblBatchSize.TabIndex = 12;
        lblBatchSize.Text = "Lote";
        // 
        // batchSizeEdit
        // 
        batchSizeEdit.Dock = DockStyle.Fill;
        batchSizeEdit.EditValue = new decimal(new int[] { 500, 0, 0, 0 });
        batchSizeEdit.Location = new Point(124, 106);
        batchSizeEdit.Name = "batchSizeEdit";
        batchSizeEdit.Properties.IsFloatValue = false;
        batchSizeEdit.Properties.MaxValue = new decimal(new int[] { 100000, 0, 0, 0 });
        batchSizeEdit.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        batchSizeEdit.Size = new Size(304, 20);
        batchSizeEdit.TabIndex = 13;
        // 
        // lblMaxRetries
        // 
        lblMaxRetries.Dock = DockStyle.Fill;
        lblMaxRetries.Location = new Point(434, 106);
        lblMaxRetries.Name = "lblMaxRetries";
        lblMaxRetries.Size = new Size(105, 25);
        lblMaxRetries.TabIndex = 14;
        lblMaxRetries.Text = "Reintentos";
        // 
        // maxRetriesEdit
        // 
        maxRetriesEdit.Dock = DockStyle.Fill;
        maxRetriesEdit.EditValue = new decimal(new int[] { 3, 0, 0, 0 });
        maxRetriesEdit.Location = new Point(545, 106);
        maxRetriesEdit.Name = "maxRetriesEdit";
        maxRetriesEdit.Properties.IsFloatValue = false;
        maxRetriesEdit.Properties.MaxValue = new decimal(new int[] { 100, 0, 0, 0 });
        maxRetriesEdit.Size = new Size(305, 20);
        maxRetriesEdit.TabIndex = 15;
        // 
        // lblRetryDelay
        // 
        lblRetryDelay.Dock = DockStyle.Fill;
        lblRetryDelay.Location = new Point(13, 137);
        lblRetryDelay.Name = "lblRetryDelay";
        lblRetryDelay.Size = new Size(105, 25);
        lblRetryDelay.TabIndex = 16;
        lblRetryDelay.Text = "Espera retry";
        // 
        // retryDelayEdit
        // 
        retryDelayEdit.Dock = DockStyle.Fill;
        retryDelayEdit.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        retryDelayEdit.Location = new Point(124, 137);
        retryDelayEdit.Name = "retryDelayEdit";
        retryDelayEdit.Properties.IsFloatValue = false;
        retryDelayEdit.Properties.MaxValue = new decimal(new int[] { 86400, 0, 0, 0 });
        retryDelayEdit.Size = new Size(304, 20);
        retryDelayEdit.TabIndex = 17;
        // 
        // lblTimeout
        // 
        lblTimeout.Dock = DockStyle.Fill;
        lblTimeout.Location = new Point(434, 137);
        lblTimeout.Name = "lblTimeout";
        lblTimeout.Size = new Size(105, 25);
        lblTimeout.TabIndex = 18;
        lblTimeout.Text = "Timeout min.";
        // 
        // timeoutEdit
        // 
        timeoutEdit.Dock = DockStyle.Fill;
        timeoutEdit.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        timeoutEdit.Location = new Point(545, 137);
        timeoutEdit.Name = "timeoutEdit";
        timeoutEdit.Properties.IsFloatValue = false;
        timeoutEdit.Properties.MaxValue = new decimal(new int[] { 1440, 0, 0, 0 });
        timeoutEdit.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        timeoutEdit.Size = new Size(305, 20);
        timeoutEdit.TabIndex = 19;
        // 
        // activeEdit
        // 
        activeEdit.Dock = DockStyle.Fill;
        activeEdit.Location = new Point(124, 168);
        activeEdit.Name = "activeEdit";
        activeEdit.Properties.Caption = "Activo";
        activeEdit.Size = new Size(304, 25);
        activeEdit.TabIndex = 20;
        // 
        // lblDescription
        // 
        lblDescription.Dock = DockStyle.Fill;
        lblDescription.Location = new Point(13, 199);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(105, 313);
        lblDescription.TabIndex = 21;
        lblDescription.Text = "Descripcion";
        // 
        // descriptionEdit
        // 
        generalLayout.SetColumnSpan(descriptionEdit, 3);
        descriptionEdit.Dock = DockStyle.Fill;
        descriptionEdit.Location = new Point(124, 199);
        descriptionEdit.Name = "descriptionEdit";
        descriptionEdit.Size = new Size(726, 313);
        descriptionEdit.TabIndex = 22;
        // 
        // tabBranches
        // 
        tabBranches.Controls.Add(branchesLayout);
        tabBranches.Name = "tabBranches";
        tabBranches.Size = new Size(863, 535);
        tabBranches.Text = "Sucursales";
        // 
        // branchesLayout
        // 
        branchesLayout.ColumnCount = 1;
        branchesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        branchesLayout.Controls.Add(branchesActionsPanel, 0, 0);
        branchesLayout.Controls.Add(branchesGrid, 0, 1);
        branchesLayout.Dock = DockStyle.Fill;
        branchesLayout.Location = new Point(0, 0);
        branchesLayout.Name = "branchesLayout";
        branchesLayout.Padding = new Padding(10);
        branchesLayout.RowCount = 2;
        branchesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
        branchesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        branchesLayout.Size = new Size(863, 535);
        branchesLayout.TabIndex = 0;
        // 
        // branchesActionsPanel
        // 
        branchesActionsPanel.Controls.Add(branchLookup);
        branchesActionsPanel.Controls.Add(btnAddBranch);
        branchesActionsPanel.Controls.Add(btnRemoveBranch);
        branchesActionsPanel.Dock = DockStyle.Fill;
        branchesActionsPanel.Location = new Point(13, 13);
        branchesActionsPanel.Name = "branchesActionsPanel";
        branchesActionsPanel.Size = new Size(837, 29);
        branchesActionsPanel.TabIndex = 0;
        // 
        // branchLookup
        // 
        branchLookup.Location = new Point(3, 3);
        branchLookup.Name = "branchLookup";
        branchLookup.Size = new Size(309, 20);
        branchLookup.TabIndex = 0;
        // 
        // btnAddBranch
        // 
        btnAddBranch.Location = new Point(318, 3);
        btnAddBranch.Name = "btnAddBranch";
        btnAddBranch.Size = new Size(103, 24);
        btnAddBranch.TabIndex = 1;
        btnAddBranch.Text = "Agregar sucursal";
        // 
        // btnRemoveBranch
        // 
        btnRemoveBranch.Location = new Point(427, 3);
        btnRemoveBranch.Name = "btnRemoveBranch";
        btnRemoveBranch.Size = new Size(111, 24);
        btnRemoveBranch.TabIndex = 2;
        btnRemoveBranch.Text = "Quitar seleccionado";
        // 
        // branchesGrid
        // 
        branchesGrid.Dock = DockStyle.Fill;
        branchesGrid.Location = new Point(13, 48);
        branchesGrid.MainView = branchesView;
        branchesGrid.Name = "branchesGrid";
        branchesGrid.Size = new Size(837, 474);
        branchesGrid.TabIndex = 1;
        branchesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { branchesView });
        // 
        // branchesView
        // 
        branchesView.Columns.AddRange(new GridColumn[] { colBranchCompanyId, colBranchDisplay, colBranchBatchSize, colBranchMaxRetries, colBranchIsActive });
        branchesView.DetailHeight = 303;
        branchesView.GridControl = branchesGrid;
        branchesView.Name = "branchesView";
        branchesView.OptionsEditForm.PopupEditFormWidth = 686;
        branchesView.OptionsView.ShowAutoFilterRow = true;
        branchesView.OptionsView.ShowGroupPanel = false;
        // 
        // colBranchCompanyId
        // 
        colBranchCompanyId.Caption = "Id";
        colBranchCompanyId.FieldName = "BranchCompanyId";
        colBranchCompanyId.MinWidth = 17;
        colBranchCompanyId.Name = "colBranchCompanyId";
        colBranchCompanyId.Visible = true;
        colBranchCompanyId.VisibleIndex = 0;
        colBranchCompanyId.Width = 69;
        // 
        // colBranchDisplay
        // 
        colBranchDisplay.Caption = "Sucursal";
        colBranchDisplay.FieldName = "BranchDisplay";
        colBranchDisplay.MinWidth = 17;
        colBranchDisplay.Name = "colBranchDisplay";
        colBranchDisplay.Visible = true;
        colBranchDisplay.VisibleIndex = 1;
        colBranchDisplay.Width = 223;
        // 
        // colBranchBatchSize
        // 
        colBranchBatchSize.Caption = "Lote";
        colBranchBatchSize.FieldName = "BatchSize";
        colBranchBatchSize.MinWidth = 17;
        colBranchBatchSize.Name = "colBranchBatchSize";
        colBranchBatchSize.Visible = true;
        colBranchBatchSize.VisibleIndex = 2;
        colBranchBatchSize.Width = 69;
        // 
        // colBranchMaxRetries
        // 
        colBranchMaxRetries.Caption = "Reintentos";
        colBranchMaxRetries.FieldName = "MaxRetries";
        colBranchMaxRetries.MinWidth = 17;
        colBranchMaxRetries.Name = "colBranchMaxRetries";
        colBranchMaxRetries.Visible = true;
        colBranchMaxRetries.VisibleIndex = 3;
        colBranchMaxRetries.Width = 77;
        // 
        // colBranchIsActive
        // 
        colBranchIsActive.Caption = "Activa";
        colBranchIsActive.FieldName = "IsActive";
        colBranchIsActive.MinWidth = 17;
        colBranchIsActive.Name = "colBranchIsActive";
        colBranchIsActive.Visible = true;
        colBranchIsActive.VisibleIndex = 4;
        colBranchIsActive.Width = 69;
        // 
        // tabEntities
        // 
        tabEntities.Controls.Add(entitiesLayout);
        tabEntities.Name = "tabEntities";
        tabEntities.Size = new Size(863, 535);
        tabEntities.Text = "Entidades";
        // 
        // entitiesLayout
        // 
        entitiesLayout.ColumnCount = 1;
        entitiesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        entitiesLayout.Controls.Add(entitiesActionsPanel, 0, 0);
        entitiesLayout.Controls.Add(entitiesGrid, 0, 1);
        entitiesLayout.Dock = DockStyle.Fill;
        entitiesLayout.Location = new Point(0, 0);
        entitiesLayout.Name = "entitiesLayout";
        entitiesLayout.Padding = new Padding(10);
        entitiesLayout.RowCount = 2;
        entitiesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
        entitiesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        entitiesLayout.Size = new Size(863, 535);
        entitiesLayout.TabIndex = 0;
        // 
        // entitiesActionsPanel
        // 
        entitiesActionsPanel.Controls.Add(entityLookup);
        entitiesActionsPanel.Controls.Add(btnAddEntity);
        entitiesActionsPanel.Controls.Add(btnRemoveEntity);
        entitiesActionsPanel.Dock = DockStyle.Fill;
        entitiesActionsPanel.Location = new Point(13, 13);
        entitiesActionsPanel.Name = "entitiesActionsPanel";
        entitiesActionsPanel.Size = new Size(837, 29);
        entitiesActionsPanel.TabIndex = 0;
        // 
        // entityLookup
        // 
        entityLookup.Location = new Point(3, 3);
        entityLookup.Name = "entityLookup";
        entityLookup.Size = new Size(309, 20);
        entityLookup.TabIndex = 0;
        // 
        // btnAddEntity
        // 
        btnAddEntity.Location = new Point(318, 3);
        btnAddEntity.Name = "btnAddEntity";
        btnAddEntity.Size = new Size(103, 24);
        btnAddEntity.TabIndex = 1;
        btnAddEntity.Text = "Agregar entidad";
        // 
        // btnRemoveEntity
        // 
        btnRemoveEntity.Location = new Point(427, 3);
        btnRemoveEntity.Name = "btnRemoveEntity";
        btnRemoveEntity.Size = new Size(111, 24);
        btnRemoveEntity.TabIndex = 2;
        btnRemoveEntity.Text = "Quitar seleccionado";
        // 
        // entitiesGrid
        // 
        entitiesGrid.Dock = DockStyle.Fill;
        entitiesGrid.Location = new Point(13, 48);
        entitiesGrid.MainView = entitiesView;
        entitiesGrid.Name = "entitiesGrid";
        entitiesGrid.Size = new Size(837, 474);
        entitiesGrid.TabIndex = 1;
        entitiesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { entitiesView });
        // 
        // entitiesView
        // 
        entitiesView.Columns.AddRange(new GridColumn[] { colEntityCode, colEntityName, colEntityExecutionOrder, colEntitySyncMode, colEntityAllowInsert, colEntityAllowUpdate, colEntityAllowDeactivate, colEntityIsActive });
        entitiesView.DetailHeight = 303;
        entitiesView.GridControl = entitiesGrid;
        entitiesView.Name = "entitiesView";
        entitiesView.OptionsEditForm.PopupEditFormWidth = 686;
        entitiesView.OptionsView.ShowAutoFilterRow = true;
        entitiesView.OptionsView.ShowGroupPanel = false;
        // 
        // colEntityCode
        // 
        colEntityCode.Caption = "Codigo";
        colEntityCode.FieldName = "EntityCode";
        colEntityCode.MinWidth = 17;
        colEntityCode.Name = "colEntityCode";
        colEntityCode.Visible = true;
        colEntityCode.VisibleIndex = 0;
        colEntityCode.Width = 129;
        // 
        // colEntityName
        // 
        colEntityName.Caption = "Entidad";
        colEntityName.FieldName = "EntityName";
        colEntityName.MinWidth = 17;
        colEntityName.Name = "colEntityName";
        colEntityName.Visible = true;
        colEntityName.VisibleIndex = 1;
        colEntityName.Width = 189;
        // 
        // colEntityExecutionOrder
        // 
        colEntityExecutionOrder.Caption = "Orden";
        colEntityExecutionOrder.FieldName = "ExecutionOrder";
        colEntityExecutionOrder.MinWidth = 17;
        colEntityExecutionOrder.Name = "colEntityExecutionOrder";
        colEntityExecutionOrder.Visible = true;
        colEntityExecutionOrder.VisibleIndex = 2;
        colEntityExecutionOrder.Width = 69;
        // 
        // colEntitySyncMode
        // 
        colEntitySyncMode.Caption = "Modo";
        colEntitySyncMode.FieldName = "SyncMode";
        colEntitySyncMode.MinWidth = 17;
        colEntitySyncMode.Name = "colEntitySyncMode";
        colEntitySyncMode.Visible = true;
        colEntitySyncMode.VisibleIndex = 3;
        colEntitySyncMode.Width = 86;
        // 
        // colEntityAllowInsert
        // 
        colEntityAllowInsert.Caption = "Insert";
        colEntityAllowInsert.FieldName = "AllowInsert";
        colEntityAllowInsert.MinWidth = 17;
        colEntityAllowInsert.Name = "colEntityAllowInsert";
        colEntityAllowInsert.Visible = true;
        colEntityAllowInsert.VisibleIndex = 4;
        colEntityAllowInsert.Width = 60;
        // 
        // colEntityAllowUpdate
        // 
        colEntityAllowUpdate.Caption = "Update";
        colEntityAllowUpdate.FieldName = "AllowUpdate";
        colEntityAllowUpdate.MinWidth = 17;
        colEntityAllowUpdate.Name = "colEntityAllowUpdate";
        colEntityAllowUpdate.Visible = true;
        colEntityAllowUpdate.VisibleIndex = 5;
        colEntityAllowUpdate.Width = 60;
        // 
        // colEntityAllowDeactivate
        // 
        colEntityAllowDeactivate.Caption = "Desact.";
        colEntityAllowDeactivate.FieldName = "AllowDeactivate";
        colEntityAllowDeactivate.MinWidth = 17;
        colEntityAllowDeactivate.Name = "colEntityAllowDeactivate";
        colEntityAllowDeactivate.Visible = true;
        colEntityAllowDeactivate.VisibleIndex = 6;
        colEntityAllowDeactivate.Width = 60;
        // 
        // colEntityIsActive
        // 
        colEntityIsActive.Caption = "Activa";
        colEntityIsActive.FieldName = "IsActive";
        colEntityIsActive.MinWidth = 17;
        colEntityIsActive.Name = "colEntityIsActive";
        colEntityIsActive.Visible = true;
        colEntityIsActive.VisibleIndex = 7;
        colEntityIsActive.Width = 60;
        // 
        // tabMatrix
        // 
        tabMatrix.Controls.Add(matrixGrid);
        tabMatrix.Name = "tabMatrix";
        tabMatrix.Size = new Size(863, 535);
        tabMatrix.Text = "Matriz entidad-sucursal";
        // 
        // matrixGrid
        // 
        matrixGrid.Dock = DockStyle.Fill;
        matrixGrid.Location = new Point(0, 0);
        matrixGrid.MainView = matrixView;
        matrixGrid.Name = "matrixGrid";
        matrixGrid.Size = new Size(863, 535);
        matrixGrid.TabIndex = 0;
        matrixGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { matrixView });
        // 
        // matrixView
        // 
        matrixView.Columns.AddRange(new GridColumn[] { colMatrixEntityCode, colMatrixBranchCompanyId, colMatrixIsEnabled, colMatrixBatchSize });
        matrixView.DetailHeight = 303;
        matrixView.GridControl = matrixGrid;
        matrixView.Name = "matrixView";
        matrixView.OptionsEditForm.PopupEditFormWidth = 686;
        matrixView.OptionsView.ShowAutoFilterRow = true;
        matrixView.OptionsView.ShowGroupPanel = false;
        // 
        // colMatrixEntityCode
        // 
        colMatrixEntityCode.Caption = "Entidad";
        colMatrixEntityCode.FieldName = "EntityCode";
        colMatrixEntityCode.MinWidth = 17;
        colMatrixEntityCode.Name = "colMatrixEntityCode";
        colMatrixEntityCode.Visible = true;
        colMatrixEntityCode.VisibleIndex = 0;
        colMatrixEntityCode.Width = 154;
        // 
        // colMatrixBranchCompanyId
        // 
        colMatrixBranchCompanyId.Caption = "Sucursal Id";
        colMatrixBranchCompanyId.FieldName = "BranchCompanyId";
        colMatrixBranchCompanyId.MinWidth = 17;
        colMatrixBranchCompanyId.Name = "colMatrixBranchCompanyId";
        colMatrixBranchCompanyId.Visible = true;
        colMatrixBranchCompanyId.VisibleIndex = 1;
        colMatrixBranchCompanyId.Width = 94;
        // 
        // colMatrixIsEnabled
        // 
        colMatrixIsEnabled.Caption = "Habilitada";
        colMatrixIsEnabled.FieldName = "IsEnabled";
        colMatrixIsEnabled.MinWidth = 17;
        colMatrixIsEnabled.Name = "colMatrixIsEnabled";
        colMatrixIsEnabled.Visible = true;
        colMatrixIsEnabled.VisibleIndex = 2;
        colMatrixIsEnabled.Width = 86;
        // 
        // colMatrixBatchSize
        // 
        colMatrixBatchSize.Caption = "Lote";
        colMatrixBatchSize.FieldName = "BatchSize";
        colMatrixBatchSize.MinWidth = 17;
        colMatrixBatchSize.Name = "colMatrixBatchSize";
        colMatrixBatchSize.Visible = true;
        colMatrixBatchSize.VisibleIndex = 3;
        colMatrixBatchSize.Width = 77;
        // 
        // tabSchedule
        // 
        tabSchedule.Controls.Add(scheduleLayout);
        tabSchedule.Name = "tabSchedule";
        tabSchedule.Size = new Size(863, 535);
        tabSchedule.Text = "Programacion";
        // 
        // scheduleLayout
        // 
        scheduleLayout.ColumnCount = 4;
        scheduleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 137F));
        scheduleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        scheduleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 137F));
        scheduleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        scheduleLayout.Controls.Add(lblScheduleType, 0, 0);
        scheduleLayout.Controls.Add(scheduleTypeEdit, 1, 0);
        scheduleLayout.Controls.Add(lblInterval, 2, 0);
        scheduleLayout.Controls.Add(intervalEdit, 3, 0);
        scheduleLayout.Controls.Add(lblExecutionTime, 0, 1);
        scheduleLayout.Controls.Add(executionTimeEdit, 1, 1);
        scheduleLayout.Controls.Add(lblTimeZone, 2, 1);
        scheduleLayout.Controls.Add(timeZoneEdit, 3, 1);
        scheduleLayout.Controls.Add(preventConcurrentEdit, 1, 2);
        scheduleLayout.Controls.Add(scheduleActiveEdit, 3, 2);
        scheduleLayout.Dock = DockStyle.Fill;
        scheduleLayout.Location = new Point(0, 0);
        scheduleLayout.Name = "scheduleLayout";
        scheduleLayout.Padding = new Padding(10);
        scheduleLayout.RowCount = 5;
        scheduleLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        scheduleLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        scheduleLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
        scheduleLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        scheduleLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
        scheduleLayout.Size = new Size(863, 535);
        scheduleLayout.TabIndex = 0;
        // 
        // lblScheduleType
        // 
        lblScheduleType.Dock = DockStyle.Fill;
        lblScheduleType.Location = new Point(13, 13);
        lblScheduleType.Name = "lblScheduleType";
        lblScheduleType.Size = new Size(131, 25);
        lblScheduleType.TabIndex = 0;
        lblScheduleType.Text = "Tipo";
        // 
        // scheduleTypeEdit
        // 
        scheduleTypeEdit.Dock = DockStyle.Fill;
        scheduleTypeEdit.Location = new Point(150, 13);
        scheduleTypeEdit.Name = "scheduleTypeEdit";
        scheduleTypeEdit.Size = new Size(278, 20);
        scheduleTypeEdit.TabIndex = 1;
        // 
        // lblInterval
        // 
        lblInterval.Dock = DockStyle.Fill;
        lblInterval.Location = new Point(434, 13);
        lblInterval.Name = "lblInterval";
        lblInterval.Size = new Size(131, 25);
        lblInterval.TabIndex = 2;
        lblInterval.Text = "Intervalo min.";
        // 
        // intervalEdit
        // 
        intervalEdit.Dock = DockStyle.Fill;
        intervalEdit.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        intervalEdit.Location = new Point(571, 13);
        intervalEdit.Name = "intervalEdit";
        intervalEdit.Properties.IsFloatValue = false;
        intervalEdit.Properties.MaxValue = new decimal(new int[] { 1440, 0, 0, 0 });
        intervalEdit.Size = new Size(279, 20);
        intervalEdit.TabIndex = 3;
        // 
        // lblExecutionTime
        // 
        lblExecutionTime.Dock = DockStyle.Fill;
        lblExecutionTime.Location = new Point(13, 44);
        lblExecutionTime.Name = "lblExecutionTime";
        lblExecutionTime.Size = new Size(131, 25);
        lblExecutionTime.TabIndex = 4;
        lblExecutionTime.Text = "Hora";
        // 
        // executionTimeEdit
        // 
        executionTimeEdit.Dock = DockStyle.Fill;
        executionTimeEdit.EditValue = new DateTime(2026, 7, 11, 0, 0, 0, 0);
        executionTimeEdit.Location = new Point(150, 44);
        executionTimeEdit.Name = "executionTimeEdit";
        executionTimeEdit.Properties.MaskSettings.Set("mask", "HH:mm");
        executionTimeEdit.Size = new Size(278, 20);
        executionTimeEdit.TabIndex = 5;
        // 
        // lblTimeZone
        // 
        lblTimeZone.Dock = DockStyle.Fill;
        lblTimeZone.Location = new Point(434, 44);
        lblTimeZone.Name = "lblTimeZone";
        lblTimeZone.Size = new Size(131, 25);
        lblTimeZone.TabIndex = 6;
        lblTimeZone.Text = "Zona horaria";
        // 
        // timeZoneEdit
        // 
        timeZoneEdit.Dock = DockStyle.Fill;
        timeZoneEdit.Location = new Point(571, 44);
        timeZoneEdit.Name = "timeZoneEdit";
        timeZoneEdit.Size = new Size(279, 20);
        timeZoneEdit.TabIndex = 7;
        // 
        // preventConcurrentEdit
        // 
        preventConcurrentEdit.Dock = DockStyle.Fill;
        preventConcurrentEdit.Location = new Point(150, 75);
        preventConcurrentEdit.Name = "preventConcurrentEdit";
        preventConcurrentEdit.Properties.Caption = "Evitar ejecuciones concurrentes";
        preventConcurrentEdit.Size = new Size(278, 25);
        preventConcurrentEdit.TabIndex = 8;
        // 
        // scheduleActiveEdit
        // 
        scheduleActiveEdit.Dock = DockStyle.Fill;
        scheduleActiveEdit.Location = new Point(571, 75);
        scheduleActiveEdit.Name = "scheduleActiveEdit";
        scheduleActiveEdit.Properties.Caption = "Programacion activa";
        scheduleActiveEdit.Size = new Size(279, 25);
        scheduleActiveEdit.TabIndex = 9;
        // 
        // footerPanel
        // 
        footerPanel.Controls.Add(btnSave);
        footerPanel.Controls.Add(btnCancel);
        footerPanel.Controls.Add(btnValidate);
        footerPanel.Dock = DockStyle.Fill;
        footerPanel.FlowDirection = FlowDirection.RightToLeft;
        footerPanel.Location = new Point(13, 579);
        footerPanel.Name = "footerPanel";
        footerPanel.Size = new Size(865, 32);
        footerPanel.TabIndex = 1;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(776, 3);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(86, 26);
        btnSave.TabIndex = 0;
        btnSave.Text = "Guardar";
        // 
        // btnCancel
        // 
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(684, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(86, 26);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancelar";
        // 
        // btnValidate
        // 
        btnValidate.Location = new Point(592, 3);
        btnValidate.Name = "btnValidate";
        btnValidate.Size = new Size(86, 26);
        btnValidate.TabIndex = 2;
        btnValidate.Text = "Validar";
        // 
        // SyncProfileEditForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(891, 624);
        Controls.Add(rootLayout);
        MinimumSize = new Size(823, 537);
        Name = "SyncProfileEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Perfil de sincronizacion";
        rootLayout.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabs).EndInit();
        tabs.ResumeLayout(false);
        tabGeneral.ResumeLayout(false);
        generalLayout.ResumeLayout(false);
        generalLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)codeEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)nameEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)companyEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)directionEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)executionModeEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)conflictStrategyEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)batchSizeEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)maxRetriesEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)retryDelayEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)timeoutEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)activeEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)descriptionEdit.Properties).EndInit();
        tabBranches.ResumeLayout(false);
        branchesLayout.ResumeLayout(false);
        branchesActionsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)branchLookup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)branchesGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)branchesView).EndInit();
        tabEntities.ResumeLayout(false);
        entitiesLayout.ResumeLayout(false);
        entitiesActionsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)entityLookup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)entitiesGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)entitiesView).EndInit();
        tabMatrix.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)matrixGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)matrixView).EndInit();
        tabSchedule.ResumeLayout(false);
        scheduleLayout.ResumeLayout(false);
        scheduleLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)scheduleTypeEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)intervalEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)executionTimeEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)timeZoneEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)preventConcurrentEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)scheduleActiveEdit.Properties).EndInit();
        footerPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

}
