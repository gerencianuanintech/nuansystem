using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Controls.Kpi;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.SriDocuments;

#nullable enable
partial class SriDocumentMonitorForm
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel kpiPanel=null!;
    private NuanKpiCardControl cardTotal=null!;
    private NuanKpiCardControl cardPending=null!;
    private NuanKpiCardControl cardAuthorized=null!;
    private NuanKpiCardControl cardErrors=null!;
    private SplitContainerControl split=null!;
    private NuanDataGridControl documentGrid=null!;
    private XtraTabControl tabs=null!;
    private XtraTabPage detailTab=null!;
    private XtraTabPage attemptsTab=null!;
    private XtraTabPage auditTab=null!;
    private XtraTabPage workerTab=null!;
    private LabelControl lblDetail=null!;
    private NuanDataGridControl attemptGrid=null!;
    private NuanDataGridControl auditGrid=null!;
    private LabelControl lblWorkerHealth=null!;

    protected override void Dispose(bool disposing) { if(disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        kpiPanel = new TableLayoutPanel();
        cardTotal = new NuanKpiCardControl();
        cardPending = new NuanKpiCardControl();
        cardAuthorized = new NuanKpiCardControl();
        cardErrors = new NuanKpiCardControl();
        split = new SplitContainerControl();
        documentGrid = new NuanDataGridControl();
        tabs = new XtraTabControl();
        detailTab = new XtraTabPage();
        lblDetail = new LabelControl();
        attemptsTab = new XtraTabPage();
        attemptGrid = new NuanDataGridControl();
        auditTab = new XtraTabPage();
        auditGrid = new NuanDataGridControl();
        workerTab = new XtraTabPage();
        lblWorkerHealth = new LabelControl();
        kpiPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        ((System.ComponentModel.ISupportInitialize)split.Panel1).BeginInit();
        split.Panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split.Panel2).BeginInit();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabs).BeginInit();
        tabs.SuspendLayout();
        detailTab.SuspendLayout();
        attemptsTab.SuspendLayout();
        auditTab.SuspendLayout();
        workerTab.SuspendLayout();
        SuspendLayout();
        //
        // kpiPanel
        //
        kpiPanel.ColumnCount = 4;
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        kpiPanel.Controls.Add(cardTotal, 0, 0);
        kpiPanel.Controls.Add(cardPending, 1, 0);
        kpiPanel.Controls.Add(cardAuthorized, 2, 0);
        kpiPanel.Controls.Add(cardErrors, 3, 0);
        kpiPanel.Dock = DockStyle.Top;
        kpiPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
        kpiPanel.Location = new Point(0, 0);
        kpiPanel.Name = "kpiPanel";
        kpiPanel.Padding = new Padding(8, 4, 8, 4);
        kpiPanel.RowCount = 1;
        kpiPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        kpiPanel.Size = new Size(1180, 100);
        kpiPanel.TabIndex = 1;
        //
        // cardTotal
        //
        cardTotal.Description = "Documentos registrados";
        cardTotal.Dock = DockStyle.Fill;
        cardTotal.HeaderColor = Color.FromArgb(0, 184, 148);
        cardTotal.HeaderHeight = 58;
        cardTotal.Location = new Point(12, 8);
        cardTotal.Margin = new Padding(4);
        cardTotal.Name = "cardTotal";
        cardTotal.Size = new Size(283, 84);
        cardTotal.TabIndex = 0;
        cardTotal.Title = "TOTAL";
        //
        // cardPending
        //
        cardPending.Description = "En espera";
        cardPending.Dock = DockStyle.Fill;
        cardPending.HeaderColor = Color.FromArgb(0, 184, 148);
        cardPending.HeaderHeight = 58;
        cardPending.Location = new Point(303, 8);
        cardPending.Margin = new Padding(4);
        cardPending.Name = "cardPending";
        cardPending.Size = new Size(283, 84);
        cardPending.TabIndex = 1;
        cardPending.Title = "PENDIENTES";
        //
        // cardAuthorized
        //
        cardAuthorized.Description = "XML disponible";
        cardAuthorized.Dock = DockStyle.Fill;
        cardAuthorized.HeaderColor = Color.FromArgb(0, 184, 148);
        cardAuthorized.HeaderHeight = 58;
        cardAuthorized.Location = new Point(594, 8);
        cardAuthorized.Margin = new Padding(4);
        cardAuthorized.Name = "cardAuthorized";
        cardAuthorized.Size = new Size(283, 84);
        cardAuthorized.TabIndex = 2;
        cardAuthorized.Title = "AUTORIZADOS";
        //
        // cardErrors
        //
        cardErrors.Description = "Fallidos o DeadLetter";
        cardErrors.Dock = DockStyle.Fill;
        cardErrors.HeaderColor = Color.FromArgb(0, 184, 148);
        cardErrors.HeaderHeight = 58;
        cardErrors.Location = new Point(885, 8);
        cardErrors.Margin = new Padding(4);
        cardErrors.Name = "cardErrors";
        cardErrors.Size = new Size(283, 84);
        cardErrors.TabIndex = 3;
        cardErrors.Title = "ERRORES";
        //
        // split
        //
        split.Dock = DockStyle.Fill;
        split.Horizontal = false;
        split.Location = new Point(0, 100);
        split.Name = "split";
        //
        // split.Panel1
        //
        split.Panel1.Controls.Add(documentGrid);
        //
        // split.Panel2
        //
        split.Panel2.Controls.Add(tabs);
        split.Size = new Size(1180, 660);
        split.SplitterPosition = 444;
        split.TabIndex = 0;
        //
        // documentGrid
        //
        documentGrid.Dock = DockStyle.Fill;
        documentGrid.Location = new Point(0, 0);
        documentGrid.Name = "documentGrid";
        documentGrid.Size = new Size(1180, 444);
        documentGrid.TabIndex = 0;
        //
        // tabs
        //
        tabs.Dock = DockStyle.Fill;
        tabs.Location = new Point(0, 0);
        tabs.Name = "tabs";
        tabs.Size = new Size(1180, 206);
        tabs.TabIndex = 0;
        tabs.TabPages.AddRange(new XtraTabPage[] { detailTab, attemptsTab, auditTab, workerTab });
        //
        // detailTab
        //
        detailTab.Controls.Add(lblDetail);
        detailTab.Name = "detailTab";
        detailTab.Size = new Size(0, 0);
        detailTab.Text = "Detalle";
        //
        // lblDetail
        //
        lblDetail.Appearance.Options.UseTextOptions = true;
        lblDetail.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        lblDetail.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetail.Dock = DockStyle.Fill;
        lblDetail.Location = new Point(0, 0);
        lblDetail.Name = "lblDetail";
        lblDetail.Padding = new Padding(16);
        lblDetail.Size = new Size(0, 0);
        lblDetail.TabIndex = 0;
        //
        // attemptsTab
        //
        attemptsTab.Controls.Add(attemptGrid);
        attemptsTab.Name = "attemptsTab";
        attemptsTab.Size = new Size(0, 0);
        attemptsTab.Text = "Intentos";
        //
        // attemptGrid
        //
        attemptGrid.Dock = DockStyle.Fill;
        attemptGrid.Location = new Point(0, 0);
        attemptGrid.Name = "attemptGrid";
        attemptGrid.Size = new Size(0, 0);
        attemptGrid.TabIndex = 0;
        //
        // auditTab
        //
        auditTab.Controls.Add(auditGrid);
        auditTab.Name = "auditTab";
        auditTab.Size = new Size(0, 0);
        auditTab.Text = "Auditoria";
        //
        // auditGrid
        //
        auditGrid.Dock = DockStyle.Fill;
        auditGrid.Location = new Point(0, 0);
        auditGrid.Name = "auditGrid";
        auditGrid.Size = new Size(0, 0);
        auditGrid.TabIndex = 0;
        //
        // workerTab
        //
        workerTab.Controls.Add(lblWorkerHealth);
        workerTab.Name = "workerTab";
        workerTab.Size = new Size(0, 0);
        workerTab.Text = "Salud del worker";
        //
        // lblWorkerHealth
        //
        lblWorkerHealth.Appearance.Options.UseTextOptions = true;
        lblWorkerHealth.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        lblWorkerHealth.AutoSizeMode = LabelAutoSizeMode.None;
        lblWorkerHealth.Dock = DockStyle.Fill;
        lblWorkerHealth.Location = new Point(0, 0);
        lblWorkerHealth.Name = "lblWorkerHealth";
        lblWorkerHealth.Padding = new Padding(16);
        lblWorkerHealth.Size = new Size(0, 0);
        lblWorkerHealth.TabIndex = 0;
        //
        // SriDocumentMonitorForm
        //
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 760);
        Controls.Add(split);
        Controls.Add(kpiPanel);
        MinimumSize = new Size(980, 650);
        Name = "SriDocumentMonitorForm";
        Text = "Monitor de documentos SRI";
        kpiPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split.Panel1).EndInit();
        split.Panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split.Panel2).EndInit();
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabs).EndInit();
        tabs.ResumeLayout(false);
        detailTab.ResumeLayout(false);
        attemptsTab.ResumeLayout(false);
        auditTab.ResumeLayout(false);
        workerTab.ResumeLayout(false);
        ResumeLayout(false);
    }
}
