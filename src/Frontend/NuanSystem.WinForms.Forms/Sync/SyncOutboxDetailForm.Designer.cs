using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync;

partial class SyncOutboxDetailForm
{
    private System.ComponentModel.IContainer components = null;

    private PanelControl headerPanel;
    private LabelControl lblHeaderTitle;
    private LabelControl lblBreadcrumb;
    private LabelControl lblHeaderEvent;
    private LabelControl lblSummaryStatusCaption;
    private LabelControl lblSummaryStatusValue;
    private LabelControl lblSummaryAttemptsCaption;
    private LabelControl lblSummaryAttemptsValue;
    private LabelControl lblSummaryCreatedCaption;
    private LabelControl lblSummaryCreatedValue;
    private LabelControl lblSummaryProcessedCaption;
    private LabelControl lblSummaryProcessedValue;
    private PanelControl generalPanel;
    private LabelControl lblEventId;
    private TextEdit txtEventId;
    private LabelControl lblEntityName;
    private TextEdit txtEntityName;
    private LabelControl lblEntityGlobalId;
    private TextEdit txtEntityGlobalId;
    private SimpleButton btnCopyGlobalId;
    private LabelControl lblEntityCode;
    private TextEdit txtEntityCode;
    private LabelControl lblOperation;
    private TextEdit txtOperation;
    private LabelControl lblStatus;
    private TextEdit txtStatus;
    private LabelControl lblAttemptCount;
    private TextEdit txtAttemptCount;
    private LabelControl lblNextRetryAt;
    private TextEdit txtNextRetryAt;
    private LabelControl lblLockedBy;
    private TextEdit txtLockedBy;
    private LabelControl lblLockExpiresAt;
    private TextEdit txtLockExpiresAt;
    private LabelControl lblLastError;
    private MemoEdit memoLastError;
    private XtraTabControl detailTabs;
    private XtraTabPage tabPayload;
    private XtraTabPage tabTargets;
    private XtraTabPage tabAudit;
    private XtraTabPage tabError;
    private MemoEdit memoPayload;
    private NuanDataGridControl grdTargets;
    private NuanDataGridControl grdAudit;
    private MemoEdit memoErrorDetail;
    private PanelControl actionPanel;
    private SimpleButton btnRetry;
    private SimpleButton btnRetryDeadLetter;
    private SimpleButton btnReleaseLock;
    private SimpleButton btnClose;

    private void InitializeComponent()
    {
        headerPanel = new PanelControl();
        lblHeaderTitle = new LabelControl();
        lblBreadcrumb = new LabelControl();
        lblHeaderEvent = new LabelControl();
        lblSummaryStatusCaption = new LabelControl();
        lblSummaryStatusValue = new LabelControl();
        lblSummaryAttemptsCaption = new LabelControl();
        lblSummaryAttemptsValue = new LabelControl();
        lblSummaryCreatedCaption = new LabelControl();
        lblSummaryCreatedValue = new LabelControl();
        lblSummaryProcessedCaption = new LabelControl();
        lblSummaryProcessedValue = new LabelControl();
        generalPanel = new PanelControl();
        lblEventId = new LabelControl();
        txtEventId = new TextEdit();
        lblEntityName = new LabelControl();
        txtEntityName = new TextEdit();
        lblEntityGlobalId = new LabelControl();
        txtEntityGlobalId = new TextEdit();
        btnCopyGlobalId = new SimpleButton();
        lblEntityCode = new LabelControl();
        txtEntityCode = new TextEdit();
        lblOperation = new LabelControl();
        txtOperation = new TextEdit();
        lblStatus = new LabelControl();
        txtStatus = new TextEdit();
        lblAttemptCount = new LabelControl();
        txtAttemptCount = new TextEdit();
        lblNextRetryAt = new LabelControl();
        txtNextRetryAt = new TextEdit();
        lblLockedBy = new LabelControl();
        txtLockedBy = new TextEdit();
        lblLockExpiresAt = new LabelControl();
        txtLockExpiresAt = new TextEdit();
        lblLastError = new LabelControl();
        memoLastError = new MemoEdit();
        detailTabs = new XtraTabControl();
        tabPayload = new XtraTabPage();
        memoPayload = new MemoEdit();
        tabTargets = new XtraTabPage();
        grdTargets = new NuanDataGridControl();
        tabAudit = new XtraTabPage();
        grdAudit = new NuanDataGridControl();
        tabError = new XtraTabPage();
        memoErrorDetail = new MemoEdit();
        actionPanel = new PanelControl();
        btnRetry = new SimpleButton();
        btnRetryDeadLetter = new SimpleButton();
        btnReleaseLock = new SimpleButton();
        btnClose = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)headerPanel).BeginInit();
        headerPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)generalPanel).BeginInit();
        generalPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtEventId.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityGlobalId.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtOperation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttemptCount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNextRetryAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtLockedBy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtLockExpiresAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memoLastError.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)detailTabs).BeginInit();
        detailTabs.SuspendLayout();
        tabPayload.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memoPayload.Properties).BeginInit();
        tabTargets.SuspendLayout();
        tabAudit.SuspendLayout();
        tabError.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memoErrorDetail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)actionPanel).BeginInit();
        actionPanel.SuspendLayout();
        SuspendLayout();
        // 
        // headerPanel
        // 
        headerPanel.Appearance.BackColor = Color.White;
        headerPanel.Appearance.Options.UseBackColor = true;
        headerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        headerPanel.Controls.Add(lblHeaderTitle);
        headerPanel.Controls.Add(lblBreadcrumb);
        headerPanel.Controls.Add(lblHeaderEvent);
        headerPanel.Controls.Add(lblSummaryStatusCaption);
        headerPanel.Controls.Add(lblSummaryStatusValue);
        headerPanel.Controls.Add(lblSummaryAttemptsCaption);
        headerPanel.Controls.Add(lblSummaryAttemptsValue);
        headerPanel.Controls.Add(lblSummaryCreatedCaption);
        headerPanel.Controls.Add(lblSummaryCreatedValue);
        headerPanel.Controls.Add(lblSummaryProcessedCaption);
        headerPanel.Controls.Add(lblSummaryProcessedValue);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 0);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(1080, 104);
        headerPanel.TabIndex = 0;
        // 
        // lblHeaderTitle
        // 
        lblHeaderTitle.Appearance.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        lblHeaderTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblHeaderTitle.Appearance.Options.UseFont = true;
        lblHeaderTitle.Appearance.Options.UseForeColor = true;
        lblHeaderTitle.Location = new Point(18, 14);
        lblHeaderTitle.Name = "lblHeaderTitle";
        lblHeaderTitle.Size = new Size(325, 28);
        lblHeaderTitle.TabIndex = 0;
        lblHeaderTitle.Text = "Detalle de Evento de Sincronizacion";
        // 
        // lblBreadcrumb
        // 
        lblBreadcrumb.Appearance.Font = new Font("Segoe UI", 9F);
        lblBreadcrumb.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblBreadcrumb.Appearance.Options.UseFont = true;
        lblBreadcrumb.Appearance.Options.UseForeColor = true;
        lblBreadcrumb.Location = new Point(20, 48);
        lblBreadcrumb.Name = "lblBreadcrumb";
        lblBreadcrumb.Size = new Size(311, 15);
        lblBreadcrumb.TabIndex = 1;
        lblBreadcrumb.Text = "Monitor de Sincronizacion > Eventos SyncOutbox > Evento";
        // 
        // lblHeaderEvent
        // 
        lblHeaderEvent.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblHeaderEvent.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblHeaderEvent.Appearance.Options.UseFont = true;
        lblHeaderEvent.Appearance.Options.UseForeColor = true;
        lblHeaderEvent.Location = new Point(20, 72);
        lblHeaderEvent.Name = "lblHeaderEvent";
        lblHeaderEvent.Size = new Size(42, 17);
        lblHeaderEvent.TabIndex = 2;
        lblHeaderEvent.Text = "Evento";
        // 
        // lblSummaryStatusCaption
        // 
        lblSummaryStatusCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSummaryStatusCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblSummaryStatusCaption.Appearance.Options.UseFont = true;
        lblSummaryStatusCaption.Appearance.Options.UseForeColor = true;
        lblSummaryStatusCaption.Location = new Point(520, 18);
        lblSummaryStatusCaption.Name = "lblSummaryStatusCaption";
        lblSummaryStatusCaption.Size = new Size(35, 15);
        lblSummaryStatusCaption.TabIndex = 3;
        lblSummaryStatusCaption.Text = "Estado";
        // 
        // lblSummaryStatusValue
        // 
        lblSummaryStatusValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblSummaryStatusValue.Appearance.ForeColor = Color.FromArgb(185, 28, 28);
        lblSummaryStatusValue.Appearance.Options.UseFont = true;
        lblSummaryStatusValue.Appearance.Options.UseForeColor = true;
        lblSummaryStatusValue.Location = new Point(520, 39);
        lblSummaryStatusValue.Name = "lblSummaryStatusValue";
        lblSummaryStatusValue.Size = new Size(30, 17);
        lblSummaryStatusValue.TabIndex = 4;
        lblSummaryStatusValue.Text = "Error";
        // 
        // lblSummaryAttemptsCaption
        // 
        lblSummaryAttemptsCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSummaryAttemptsCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblSummaryAttemptsCaption.Appearance.Options.UseFont = true;
        lblSummaryAttemptsCaption.Appearance.Options.UseForeColor = true;
        lblSummaryAttemptsCaption.Location = new Point(650, 18);
        lblSummaryAttemptsCaption.Name = "lblSummaryAttemptsCaption";
        lblSummaryAttemptsCaption.Size = new Size(43, 15);
        lblSummaryAttemptsCaption.TabIndex = 5;
        lblSummaryAttemptsCaption.Text = "Intentos";
        // 
        // lblSummaryAttemptsValue
        // 
        lblSummaryAttemptsValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblSummaryAttemptsValue.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblSummaryAttemptsValue.Appearance.Options.UseFont = true;
        lblSummaryAttemptsValue.Appearance.Options.UseForeColor = true;
        lblSummaryAttemptsValue.Location = new Point(650, 39);
        lblSummaryAttemptsValue.Name = "lblSummaryAttemptsValue";
        lblSummaryAttemptsValue.Size = new Size(27, 17);
        lblSummaryAttemptsValue.TabIndex = 6;
        lblSummaryAttemptsValue.Text = "0 / 0";
        // 
        // lblSummaryCreatedCaption
        // 
        lblSummaryCreatedCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSummaryCreatedCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblSummaryCreatedCaption.Appearance.Options.UseFont = true;
        lblSummaryCreatedCaption.Appearance.Options.UseForeColor = true;
        lblSummaryCreatedCaption.Location = new Point(780, 18);
        lblSummaryCreatedCaption.Name = "lblSummaryCreatedCaption";
        lblSummaryCreatedCaption.Size = new Size(38, 15);
        lblSummaryCreatedCaption.TabIndex = 7;
        lblSummaryCreatedCaption.Text = "Creado";
        // 
        // lblSummaryCreatedValue
        // 
        lblSummaryCreatedValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblSummaryCreatedValue.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblSummaryCreatedValue.Appearance.Options.UseFont = true;
        lblSummaryCreatedValue.Appearance.Options.UseForeColor = true;
        lblSummaryCreatedValue.Location = new Point(860, 16);
        lblSummaryCreatedValue.Name = "lblSummaryCreatedValue";
        lblSummaryCreatedValue.Size = new Size(5, 17);
        lblSummaryCreatedValue.TabIndex = 8;
        lblSummaryCreatedValue.Text = "-";
        // 
        // lblSummaryProcessedCaption
        // 
        lblSummaryProcessedCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSummaryProcessedCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblSummaryProcessedCaption.Appearance.Options.UseFont = true;
        lblSummaryProcessedCaption.Appearance.Options.UseForeColor = true;
        lblSummaryProcessedCaption.Location = new Point(763, 41);
        lblSummaryProcessedCaption.Name = "lblSummaryProcessedCaption";
        lblSummaryProcessedCaption.Size = new Size(55, 15);
        lblSummaryProcessedCaption.TabIndex = 9;
        lblSummaryProcessedCaption.Text = "Procesado";
        // 
        // lblSummaryProcessedValue
        // 
        lblSummaryProcessedValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblSummaryProcessedValue.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblSummaryProcessedValue.Appearance.Options.UseFont = true;
        lblSummaryProcessedValue.Appearance.Options.UseForeColor = true;
        lblSummaryProcessedValue.Location = new Point(860, 39);
        lblSummaryProcessedValue.Name = "lblSummaryProcessedValue";
        lblSummaryProcessedValue.Size = new Size(5, 17);
        lblSummaryProcessedValue.TabIndex = 10;
        lblSummaryProcessedValue.Text = "-";
        // 
        // generalPanel
        // 
        generalPanel.Appearance.BackColor = Color.White;
        generalPanel.Appearance.Options.UseBackColor = true;
        generalPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        generalPanel.Controls.Add(lblEventId);
        generalPanel.Controls.Add(txtEventId);
        generalPanel.Controls.Add(lblEntityName);
        generalPanel.Controls.Add(txtEntityName);
        generalPanel.Controls.Add(lblEntityGlobalId);
        generalPanel.Controls.Add(txtEntityGlobalId);
        generalPanel.Controls.Add(btnCopyGlobalId);
        generalPanel.Controls.Add(lblEntityCode);
        generalPanel.Controls.Add(txtEntityCode);
        generalPanel.Controls.Add(lblOperation);
        generalPanel.Controls.Add(txtOperation);
        generalPanel.Controls.Add(lblStatus);
        generalPanel.Controls.Add(txtStatus);
        generalPanel.Controls.Add(lblAttemptCount);
        generalPanel.Controls.Add(txtAttemptCount);
        generalPanel.Controls.Add(lblNextRetryAt);
        generalPanel.Controls.Add(txtNextRetryAt);
        generalPanel.Controls.Add(lblLockedBy);
        generalPanel.Controls.Add(txtLockedBy);
        generalPanel.Controls.Add(lblLockExpiresAt);
        generalPanel.Controls.Add(txtLockExpiresAt);
        generalPanel.Controls.Add(lblLastError);
        generalPanel.Controls.Add(memoLastError);
        generalPanel.Dock = DockStyle.Top;
        generalPanel.Location = new Point(0, 104);
        generalPanel.Name = "generalPanel";
        generalPanel.Size = new Size(1080, 230);
        generalPanel.TabIndex = 0;
        // 
        // lblEventId
        // 
        lblEventId.Appearance.Font = new Font("Segoe UI", 9F);
        lblEventId.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblEventId.Appearance.Options.UseFont = true;
        lblEventId.Appearance.Options.UseForeColor = true;
        lblEventId.Location = new Point(24, 20);
        lblEventId.Name = "lblEventId";
        lblEventId.Size = new Size(39, 15);
        lblEventId.TabIndex = 2;
        lblEventId.Text = "EventId";
        // 
        // txtEventId
        // 
        txtEventId.Location = new Point(112, 16);
        txtEventId.Name = "txtEventId";
        txtEventId.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtEventId.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEventId.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtEventId.Properties.Appearance.Options.UseBackColor = true;
        txtEventId.Properties.Appearance.Options.UseFont = true;
        txtEventId.Properties.Appearance.Options.UseForeColor = true;
        txtEventId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtEventId.Properties.ReadOnly = true;
        txtEventId.Size = new Size(360, 22);
        txtEventId.TabIndex = 3;
        // 
        // lblEntityName
        // 
        lblEntityName.Appearance.Font = new Font("Segoe UI", 9F);
        lblEntityName.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblEntityName.Appearance.Options.UseFont = true;
        lblEntityName.Appearance.Options.UseForeColor = true;
        lblEntityName.Location = new Point(24, 76);
        lblEntityName.Name = "lblEntityName";
        lblEntityName.Size = new Size(40, 15);
        lblEntityName.TabIndex = 4;
        lblEntityName.Text = "Entidad";
        // 
        // txtEntityName
        // 
        txtEntityName.Location = new Point(112, 72);
        txtEntityName.Name = "txtEntityName";
        txtEntityName.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtEntityName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEntityName.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtEntityName.Properties.Appearance.Options.UseBackColor = true;
        txtEntityName.Properties.Appearance.Options.UseFont = true;
        txtEntityName.Properties.Appearance.Options.UseForeColor = true;
        txtEntityName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtEntityName.Properties.ReadOnly = true;
        txtEntityName.Size = new Size(360, 22);
        txtEntityName.TabIndex = 5;
        // 
        // lblEntityGlobalId
        // 
        lblEntityGlobalId.Appearance.Font = new Font("Segoe UI", 9F);
        lblEntityGlobalId.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblEntityGlobalId.Appearance.Options.UseFont = true;
        lblEntityGlobalId.Appearance.Options.UseForeColor = true;
        lblEntityGlobalId.Location = new Point(24, 104);
        lblEntityGlobalId.Name = "lblEntityGlobalId";
        lblEntityGlobalId.Size = new Size(44, 15);
        lblEntityGlobalId.TabIndex = 6;
        lblEntityGlobalId.Text = "GlobalId";
        // 
        // txtEntityGlobalId
        // 
        txtEntityGlobalId.Location = new Point(112, 100);
        txtEntityGlobalId.Name = "txtEntityGlobalId";
        txtEntityGlobalId.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtEntityGlobalId.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEntityGlobalId.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtEntityGlobalId.Properties.Appearance.Options.UseBackColor = true;
        txtEntityGlobalId.Properties.Appearance.Options.UseFont = true;
        txtEntityGlobalId.Properties.Appearance.Options.UseForeColor = true;
        txtEntityGlobalId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtEntityGlobalId.Properties.ReadOnly = true;
        txtEntityGlobalId.Size = new Size(324, 22);
        txtEntityGlobalId.TabIndex = 7;
        // 
        // btnCopyGlobalId
        // 
        btnCopyGlobalId.AllowFocus = false;
        btnCopyGlobalId.Appearance.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold);
        btnCopyGlobalId.Appearance.ForeColor = Color.FromArgb(0, 102, 204);
        btnCopyGlobalId.Appearance.Options.UseFont = true;
        btnCopyGlobalId.Appearance.Options.UseForeColor = true;
        btnCopyGlobalId.Location = new Point(442, 100);
        btnCopyGlobalId.Name = "btnCopyGlobalId";
        btnCopyGlobalId.Size = new Size(30, 22);
        btnCopyGlobalId.TabIndex = 8;
        btnCopyGlobalId.ToolTip = "Copiar GlobalId";
        // 
        // lblEntityCode
        // 
        lblEntityCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblEntityCode.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblEntityCode.Appearance.Options.UseFont = true;
        lblEntityCode.Appearance.Options.UseForeColor = true;
        lblEntityCode.Location = new Point(24, 132);
        lblEntityCode.Name = "lblEntityCode";
        lblEntityCode.Size = new Size(39, 15);
        lblEntityCode.TabIndex = 9;
        lblEntityCode.Text = "Codigo";
        // 
        // txtEntityCode
        // 
        txtEntityCode.Location = new Point(112, 128);
        txtEntityCode.Name = "txtEntityCode";
        txtEntityCode.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtEntityCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEntityCode.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtEntityCode.Properties.Appearance.Options.UseBackColor = true;
        txtEntityCode.Properties.Appearance.Options.UseFont = true;
        txtEntityCode.Properties.Appearance.Options.UseForeColor = true;
        txtEntityCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtEntityCode.Properties.ReadOnly = true;
        txtEntityCode.Size = new Size(360, 22);
        txtEntityCode.TabIndex = 10;
        // 
        // lblOperation
        // 
        lblOperation.Appearance.Font = new Font("Segoe UI", 9F);
        lblOperation.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblOperation.Appearance.Options.UseFont = true;
        lblOperation.Appearance.Options.UseForeColor = true;
        lblOperation.Location = new Point(542, 20);
        lblOperation.Name = "lblOperation";
        lblOperation.Size = new Size(55, 15);
        lblOperation.TabIndex = 11;
        lblOperation.Text = "Operacion";
        // 
        // txtOperation
        // 
        txtOperation.Location = new Point(650, 16);
        txtOperation.Name = "txtOperation";
        txtOperation.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtOperation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtOperation.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtOperation.Properties.Appearance.Options.UseBackColor = true;
        txtOperation.Properties.Appearance.Options.UseFont = true;
        txtOperation.Properties.Appearance.Options.UseForeColor = true;
        txtOperation.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtOperation.Properties.ReadOnly = true;
        txtOperation.Size = new Size(360, 22);
        txtOperation.TabIndex = 12;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblStatus.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.Location = new Point(24, 48);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(35, 15);
        lblStatus.TabIndex = 13;
        lblStatus.Text = "Estado";
        // 
        // txtStatus
        // 
        txtStatus.Location = new Point(112, 44);
        txtStatus.Name = "txtStatus";
        txtStatus.Properties.Appearance.BackColor = Color.FromArgb(254, 226, 226);
        txtStatus.Properties.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        txtStatus.Properties.Appearance.ForeColor = Color.FromArgb(185, 28, 28);
        txtStatus.Properties.Appearance.Options.UseBackColor = true;
        txtStatus.Properties.Appearance.Options.UseFont = true;
        txtStatus.Properties.Appearance.Options.UseForeColor = true;
        txtStatus.Properties.Appearance.Options.UseTextOptions = true;
        txtStatus.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        txtStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtStatus.Properties.ReadOnly = true;
        txtStatus.Size = new Size(360, 22);
        txtStatus.TabIndex = 14;
        // 
        // lblAttemptCount
        // 
        lblAttemptCount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttemptCount.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblAttemptCount.Appearance.Options.UseFont = true;
        lblAttemptCount.Appearance.Options.UseForeColor = true;
        lblAttemptCount.Location = new Point(542, 48);
        lblAttemptCount.Name = "lblAttemptCount";
        lblAttemptCount.Size = new Size(43, 15);
        lblAttemptCount.TabIndex = 19;
        lblAttemptCount.Text = "Intentos";
        // 
        // txtAttemptCount
        // 
        txtAttemptCount.Location = new Point(650, 44);
        txtAttemptCount.Name = "txtAttemptCount";
        txtAttemptCount.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtAttemptCount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttemptCount.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtAttemptCount.Properties.Appearance.Options.UseBackColor = true;
        txtAttemptCount.Properties.Appearance.Options.UseFont = true;
        txtAttemptCount.Properties.Appearance.Options.UseForeColor = true;
        txtAttemptCount.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtAttemptCount.Properties.ReadOnly = true;
        txtAttemptCount.Size = new Size(360, 22);
        txtAttemptCount.TabIndex = 20;
        // 
        // lblNextRetryAt
        // 
        lblNextRetryAt.Appearance.Font = new Font("Segoe UI", 9F);
        lblNextRetryAt.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblNextRetryAt.Appearance.Options.UseFont = true;
        lblNextRetryAt.Appearance.Options.UseForeColor = true;
        lblNextRetryAt.Location = new Point(542, 76);
        lblNextRetryAt.Name = "lblNextRetryAt";
        lblNextRetryAt.Size = new Size(71, 15);
        lblNextRetryAt.TabIndex = 23;
        lblNextRetryAt.Text = "Proximo retry";
        // 
        // txtNextRetryAt
        // 
        txtNextRetryAt.Location = new Point(650, 72);
        txtNextRetryAt.Name = "txtNextRetryAt";
        txtNextRetryAt.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtNextRetryAt.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtNextRetryAt.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtNextRetryAt.Properties.Appearance.Options.UseBackColor = true;
        txtNextRetryAt.Properties.Appearance.Options.UseFont = true;
        txtNextRetryAt.Properties.Appearance.Options.UseForeColor = true;
        txtNextRetryAt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtNextRetryAt.Properties.ReadOnly = true;
        txtNextRetryAt.Size = new Size(360, 22);
        txtNextRetryAt.TabIndex = 24;
        // 
        // lblLockedBy
        // 
        lblLockedBy.Appearance.Font = new Font("Segoe UI", 9F);
        lblLockedBy.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblLockedBy.Appearance.Options.UseFont = true;
        lblLockedBy.Appearance.Options.UseForeColor = true;
        lblLockedBy.Location = new Point(542, 104);
        lblLockedBy.Name = "lblLockedBy";
        lblLockedBy.Size = new Size(51, 15);
        lblLockedBy.TabIndex = 25;
        lblLockedBy.Text = "LockedBy";
        // 
        // txtLockedBy
        // 
        txtLockedBy.Location = new Point(650, 100);
        txtLockedBy.Name = "txtLockedBy";
        txtLockedBy.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtLockedBy.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtLockedBy.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtLockedBy.Properties.Appearance.Options.UseBackColor = true;
        txtLockedBy.Properties.Appearance.Options.UseFont = true;
        txtLockedBy.Properties.Appearance.Options.UseForeColor = true;
        txtLockedBy.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtLockedBy.Properties.ReadOnly = true;
        txtLockedBy.Size = new Size(360, 22);
        txtLockedBy.TabIndex = 26;
        // 
        // lblLockExpiresAt
        // 
        lblLockExpiresAt.Appearance.Font = new Font("Segoe UI", 9F);
        lblLockExpiresAt.Appearance.ForeColor = Color.FromArgb(71, 85, 105);
        lblLockExpiresAt.Appearance.Options.UseFont = true;
        lblLockExpiresAt.Appearance.Options.UseForeColor = true;
        lblLockExpiresAt.Location = new Point(542, 132);
        lblLockExpiresAt.Name = "lblLockExpiresAt";
        lblLockExpiresAt.Size = new Size(73, 15);
        lblLockExpiresAt.TabIndex = 29;
        lblLockExpiresAt.Text = "LockExpiresAt";
        // 
        // txtLockExpiresAt
        // 
        txtLockExpiresAt.Location = new Point(650, 128);
        txtLockExpiresAt.Name = "txtLockExpiresAt";
        txtLockExpiresAt.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        txtLockExpiresAt.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtLockExpiresAt.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtLockExpiresAt.Properties.Appearance.Options.UseBackColor = true;
        txtLockExpiresAt.Properties.Appearance.Options.UseFont = true;
        txtLockExpiresAt.Properties.Appearance.Options.UseForeColor = true;
        txtLockExpiresAt.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        txtLockExpiresAt.Properties.ReadOnly = true;
        txtLockExpiresAt.Size = new Size(360, 22);
        txtLockExpiresAt.TabIndex = 30;
        // 
        // lblLastError
        // 
        lblLastError.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblLastError.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblLastError.Appearance.Options.UseFont = true;
        lblLastError.Appearance.Options.UseForeColor = true;
        lblLastError.Location = new Point(24, 168);
        lblLastError.Name = "lblLastError";
        lblLastError.Size = new Size(64, 15);
        lblLastError.TabIndex = 31;
        lblLastError.Text = "Ultimo error";
        // 
        // memoLastError
        // 
        memoLastError.Location = new Point(112, 156);
        memoLastError.Name = "memoLastError";
        memoLastError.Properties.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        memoLastError.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memoLastError.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memoLastError.Properties.Appearance.Options.UseBackColor = true;
        memoLastError.Properties.Appearance.Options.UseFont = true;
        memoLastError.Properties.Appearance.Options.UseForeColor = true;
        memoLastError.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        memoLastError.Properties.ReadOnly = true;
        memoLastError.Size = new Size(898, 64);
        memoLastError.TabIndex = 32;
        // 
        // detailTabs
        // 
        detailTabs.Dock = DockStyle.Fill;
        detailTabs.Location = new Point(0, 334);
        detailTabs.Name = "detailTabs";
        detailTabs.SelectedTabPage = tabPayload;
        detailTabs.Size = new Size(1080, 324);
        detailTabs.TabIndex = 1;
        detailTabs.TabPages.AddRange(new XtraTabPage[] { tabPayload, tabTargets, tabAudit, tabError });
        // 
        // tabPayload
        // 
        tabPayload.Controls.Add(memoPayload);
        tabPayload.Name = "tabPayload";
        tabPayload.Size = new Size(1078, 301);
        tabPayload.Text = "Payload";
        // 
        // memoPayload
        // 
        memoPayload.Dock = DockStyle.Fill;
        memoPayload.Location = new Point(0, 0);
        memoPayload.Name = "memoPayload";
        memoPayload.Properties.Appearance.Font = new Font("Consolas", 9F);
        memoPayload.Properties.Appearance.Options.UseFont = true;
        memoPayload.Properties.ReadOnly = true;
        memoPayload.Size = new Size(1078, 301);
        memoPayload.TabIndex = 0;
        // 
        // tabTargets
        // 
        tabTargets.Controls.Add(grdTargets);
        tabTargets.Name = "tabTargets";
        tabTargets.Size = new Size(1078, 301);
        tabTargets.Text = "Targets";
        // 
        // grdTargets
        // 
        grdTargets.Dock = DockStyle.Fill;
        grdTargets.GridName = "Targets";
        grdTargets.Location = new Point(0, 0);
        grdTargets.Name = "grdTargets";
        grdTargets.ShowFindPanel = false;
        grdTargets.Size = new Size(1078, 301);
        grdTargets.TabIndex = 0;
        // 
        // tabAudit
        // 
        tabAudit.Controls.Add(grdAudit);
        tabAudit.Name = "tabAudit";
        tabAudit.Size = new Size(1078, 301);
        tabAudit.Text = "Auditoria";
        // 
        // grdAudit
        // 
        grdAudit.Dock = DockStyle.Fill;
        grdAudit.GridName = "Audit";
        grdAudit.Location = new Point(0, 0);
        grdAudit.Name = "grdAudit";
        grdAudit.Size = new Size(1078, 301);
        grdAudit.TabIndex = 0;
        // 
        // tabError
        // 
        tabError.Controls.Add(memoErrorDetail);
        tabError.Name = "tabError";
        tabError.Size = new Size(1078, 301);
        tabError.Text = "Error";
        // 
        // memoErrorDetail
        // 
        memoErrorDetail.Dock = DockStyle.Fill;
        memoErrorDetail.Location = new Point(0, 0);
        memoErrorDetail.Name = "memoErrorDetail";
        memoErrorDetail.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memoErrorDetail.Properties.Appearance.Options.UseFont = true;
        memoErrorDetail.Properties.ReadOnly = true;
        memoErrorDetail.Size = new Size(1078, 301);
        memoErrorDetail.TabIndex = 0;
        // 
        // actionPanel
        // 
        actionPanel.Appearance.BackColor = Color.White;
        actionPanel.Appearance.Options.UseBackColor = true;
        actionPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        actionPanel.Controls.Add(btnRetry);
        actionPanel.Controls.Add(btnRetryDeadLetter);
        actionPanel.Controls.Add(btnReleaseLock);
        actionPanel.Controls.Add(btnClose);
        actionPanel.Dock = DockStyle.Bottom;
        actionPanel.Location = new Point(0, 658);
        actionPanel.Name = "actionPanel";
        actionPanel.Size = new Size(1080, 54);
        actionPanel.TabIndex = 2;
        // 
        // btnRetry
        // 
        btnRetry.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRetry.Appearance.Options.UseFont = true;
        btnRetry.Location = new Point(18, 11);
        btnRetry.Name = "btnRetry";
        btnRetry.Size = new Size(118, 32);
        btnRetry.TabIndex = 0;
        btnRetry.Text = "Reintentar";
        // 
        // btnRetryDeadLetter
        // 
        btnRetryDeadLetter.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRetryDeadLetter.Appearance.Options.UseFont = true;
        btnRetryDeadLetter.Location = new Point(144, 11);
        btnRetryDeadLetter.Name = "btnRetryDeadLetter";
        btnRetryDeadLetter.Size = new Size(154, 32);
        btnRetryDeadLetter.TabIndex = 1;
        btnRetryDeadLetter.Text = "Reintentar DeadLetter";
        // 
        // btnReleaseLock
        // 
        btnReleaseLock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnReleaseLock.Appearance.Options.UseFont = true;
        btnReleaseLock.Location = new Point(306, 11);
        btnReleaseLock.Name = "btnReleaseLock";
        btnReleaseLock.Size = new Size(148, 32);
        btnReleaseLock.TabIndex = 2;
        btnReleaseLock.Text = "Liberar lock vencido";
        // 
        // btnClose
        // 
        btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClose.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnClose.Appearance.Options.UseFont = true;
        btnClose.DialogResult = DialogResult.OK;
        btnClose.Location = new Point(968, 11);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(100, 32);
        btnClose.TabIndex = 3;
        btnClose.Text = "Cerrar";
        // 
        // SyncOutboxDetailForm
        // 
        AcceptButton = btnClose;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1080, 712);
        Controls.Add(detailTabs);
        Controls.Add(actionPanel);
        Controls.Add(generalPanel);
        Controls.Add(headerPanel);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SyncOutboxDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Detalle SyncOutbox";
        ((System.ComponentModel.ISupportInitialize)headerPanel).EndInit();
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)generalPanel).EndInit();
        generalPanel.ResumeLayout(false);
        generalPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtEventId.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityGlobalId.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtOperation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttemptCount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNextRetryAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtLockedBy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtLockExpiresAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memoLastError.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)detailTabs).EndInit();
        detailTabs.ResumeLayout(false);
        tabPayload.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)memoPayload.Properties).EndInit();
        tabTargets.ResumeLayout(false);
        tabAudit.ResumeLayout(false);
        tabError.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)memoErrorDetail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)actionPanel).EndInit();
        actionPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
