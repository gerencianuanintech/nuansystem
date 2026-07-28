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
        components=new System.ComponentModel.Container();
        kpiPanel=new TableLayoutPanel(); cardTotal=new NuanKpiCardControl(); cardPending=new NuanKpiCardControl(); cardAuthorized=new NuanKpiCardControl(); cardErrors=new NuanKpiCardControl();
        split=new SplitContainerControl(); documentGrid=new NuanDataGridControl(); tabs=new XtraTabControl(); detailTab=new XtraTabPage(); attemptsTab=new XtraTabPage(); auditTab=new XtraTabPage(); workerTab=new XtraTabPage(); lblDetail=new LabelControl(); attemptGrid=new NuanDataGridControl(); auditGrid=new NuanDataGridControl(); lblWorkerHealth=new LabelControl();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit(); ((System.ComponentModel.ISupportInitialize)split.Panel1).BeginInit(); split.Panel1.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)split.Panel2).BeginInit(); split.Panel2.SuspendLayout(); split.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)tabs).BeginInit(); tabs.SuspendLayout(); detailTab.SuspendLayout(); attemptsTab.SuspendLayout(); auditTab.SuspendLayout(); workerTab.SuspendLayout(); SuspendLayout();
        kpiPanel.ColumnCount=4;
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F));
        kpiPanel.GrowStyle=TableLayoutPanelGrowStyle.FixedSize;
        kpiPanel.RowCount=1;
        kpiPanel.RowStyles.Add(new RowStyle(SizeType.Percent,100F));
        kpiPanel.Controls.Add(cardTotal,0,0);
        kpiPanel.Controls.Add(cardPending,1,0);
        kpiPanel.Controls.Add(cardAuthorized,2,0);
        kpiPanel.Controls.Add(cardErrors,3,0);
        kpiPanel.Dock=DockStyle.Top;
        kpiPanel.Height=100;
        kpiPanel.Name="kpiPanel";
        kpiPanel.Padding=new Padding(8,4,8,4);
        cardTotal.Description="Documentos registrados"; cardTotal.Dock=DockStyle.Fill; cardTotal.HeaderColor=BrandResources.Primary; cardTotal.HeaderHeight=58; cardTotal.Margin=new Padding(4); cardTotal.MinimumSize=Size.Empty; cardTotal.Name="cardTotal"; cardTotal.Title="TOTAL";
        cardPending.Description="En espera"; cardPending.Dock=DockStyle.Fill; cardPending.HeaderColor=BrandResources.Primary; cardPending.HeaderHeight=58; cardPending.Margin=new Padding(4); cardPending.MinimumSize=Size.Empty; cardPending.Name="cardPending"; cardPending.Title="PENDIENTES";
        cardAuthorized.Description="XML disponible"; cardAuthorized.Dock=DockStyle.Fill; cardAuthorized.HeaderColor=BrandResources.Primary; cardAuthorized.HeaderHeight=58; cardAuthorized.Margin=new Padding(4); cardAuthorized.MinimumSize=Size.Empty; cardAuthorized.Name="cardAuthorized"; cardAuthorized.Title="AUTORIZADOS";
        cardErrors.Description="Fallidos o DeadLetter"; cardErrors.Dock=DockStyle.Fill; cardErrors.HeaderColor=BrandResources.Primary; cardErrors.HeaderHeight=58; cardErrors.Margin=new Padding(4); cardErrors.MinimumSize=Size.Empty; cardErrors.Name="cardErrors"; cardErrors.Title="ERRORES";
        split.Dock=DockStyle.Fill; split.Horizontal=false; split.SplitterPosition=350;
        documentGrid.Dock=DockStyle.Fill; split.Panel1.Controls.Add(documentGrid);
        tabs.Dock=DockStyle.Fill; tabs.TabPages.AddRange(new XtraTabPage[] { detailTab,attemptsTab,auditTab,workerTab });
        detailTab.Text="Detalle"; attemptsTab.Text="Intentos"; auditTab.Text="Auditoria"; workerTab.Text="Salud del worker";
        lblDetail.Dock=DockStyle.Fill; lblDetail.Appearance.TextOptions.WordWrap=DevExpress.Utils.WordWrap.Wrap; lblDetail.AutoSizeMode=LabelAutoSizeMode.None; lblDetail.Padding=new Padding(16); detailTab.Controls.Add(lblDetail);
        attemptGrid.Dock=DockStyle.Fill; attemptsTab.Controls.Add(attemptGrid); auditGrid.Dock=DockStyle.Fill; auditTab.Controls.Add(auditGrid); lblWorkerHealth.Dock=DockStyle.Fill; lblWorkerHealth.AutoSizeMode=LabelAutoSizeMode.None; lblWorkerHealth.Padding=new Padding(16); lblWorkerHealth.Appearance.TextOptions.WordWrap=DevExpress.Utils.WordWrap.Wrap; workerTab.Controls.Add(lblWorkerHealth); split.Panel2.Controls.Add(tabs);
        AutoScaleDimensions=new SizeF(7F,15F); AutoScaleMode=AutoScaleMode.Font; ClientSize=new Size(1180,760); Controls.Add(split); Controls.Add(kpiPanel); MinimumSize=new Size(980,650); Name="SriDocumentMonitorForm"; Text="Monitor de documentos SRI";
        ((System.ComponentModel.ISupportInitialize)split.Panel1).EndInit(); split.Panel1.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)split.Panel2).EndInit(); split.Panel2.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)split).EndInit(); split.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)tabs).EndInit(); tabs.ResumeLayout(false); detailTab.ResumeLayout(false); attemptsTab.ResumeLayout(false); auditTab.ResumeLayout(false); workerTab.ResumeLayout(false); ResumeLayout(false);
    }
}
