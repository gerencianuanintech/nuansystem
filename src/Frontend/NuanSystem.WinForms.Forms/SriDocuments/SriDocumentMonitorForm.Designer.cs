using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Controls.Kpi;

namespace NuanSystem.WinForms.Forms.SriDocuments;

#nullable enable
partial class SriDocumentMonitorForm
{
    private System.ComponentModel.IContainer? components;
    private PanelControl filterPanel=null!;
    private ComboBoxEdit cmbEnvironment=null!;
    private ComboBoxEdit cmbStatus=null!;
    private ComboBoxEdit cmbDocumentType=null!;
    private ComboBoxEdit cmbSourceType=null!;
    private TextEdit txtSearch=null!;
    private DateEdit dateFrom=null!;
    private DateEdit dateTo=null!;
    private NuanActionButton btnRefresh=null!;
    private NuanActionButton btnClear=null!;
    private NuanActionButton btnDownload=null!;
    private FlowLayoutPanel kpiPanel=null!;
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
        filterPanel=new PanelControl(); cmbEnvironment=new ComboBoxEdit(); cmbStatus=new ComboBoxEdit(); cmbDocumentType=new ComboBoxEdit(); cmbSourceType=new ComboBoxEdit(); txtSearch=new TextEdit(); dateFrom=new DateEdit(); dateTo=new DateEdit(); btnRefresh=new NuanActionButton(); btnClear=new NuanActionButton(); btnDownload=new NuanActionButton();
        kpiPanel=new FlowLayoutPanel(); cardTotal=new NuanKpiCardControl(); cardPending=new NuanKpiCardControl(); cardAuthorized=new NuanKpiCardControl(); cardErrors=new NuanKpiCardControl();
        split=new SplitContainerControl(); documentGrid=new NuanDataGridControl(); tabs=new XtraTabControl(); detailTab=new XtraTabPage(); attemptsTab=new XtraTabPage(); auditTab=new XtraTabPage(); workerTab=new XtraTabPage(); lblDetail=new LabelControl(); attemptGrid=new NuanDataGridControl(); auditGrid=new NuanDataGridControl(); lblWorkerHealth=new LabelControl();
        ((System.ComponentModel.ISupportInitialize)cmbDocumentType.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)cmbSourceType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)filterPanel).BeginInit(); filterPanel.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)cmbEnvironment.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)dateTo.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)split).BeginInit(); ((System.ComponentModel.ISupportInitialize)split.Panel1).BeginInit(); split.Panel1.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)split.Panel2).BeginInit(); split.Panel2.SuspendLayout(); split.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)tabs).BeginInit(); tabs.SuspendLayout(); detailTab.SuspendLayout(); attemptsTab.SuspendLayout(); auditTab.SuspendLayout(); workerTab.SuspendLayout(); SuspendLayout();
        filterPanel.Dock=DockStyle.Top; filterPanel.Height=88;
        cmbEnvironment.Location=new Point(12,14); cmbEnvironment.Size=new Size(120,22); cmbEnvironment.Properties.Items.AddRange(new object[] { "Test","Production" }); cmbEnvironment.Properties.NullText="Ambiente";
        cmbStatus.Location=new Point(140,14); cmbStatus.Size=new Size(140,22); cmbStatus.Properties.Items.AddRange(new object[] { "Pending","Querying","RetryScheduled","Authorized","NotFound","Failed","DeadLetter","Cancelled" }); cmbStatus.Properties.NullText="Estado";
        cmbDocumentType.Location=new Point(288,14); cmbDocumentType.Size=new Size(115,22); cmbDocumentType.Properties.Items.AddRange(new object[] { "01","04","07" }); cmbDocumentType.Properties.NullText="Tipo";
        cmbSourceType.Location=new Point(411,14); cmbSourceType.Size=new Size(140,22); cmbSourceType.Properties.Items.AddRange(new object[] { "NuanSystem","Txt","SapAddOn","Manual","ExternalApi" }); cmbSourceType.Properties.NullText="Origen";
        dateFrom.Location=new Point(12,50); dateFrom.Size=new Size(120,22); dateFrom.Properties.NullText="Desde";
        dateTo.Location=new Point(140,50); dateTo.Size=new Size(140,22); dateTo.Properties.NullText="Hasta";
        txtSearch.Location=new Point(288,50); txtSearch.Size=new Size(263,22); txtSearch.Properties.NullValuePrompt="Referencia segura";
        btnRefresh.Location=new Point(570,26); btnRefresh.Size=new Size(110,36); btnRefresh.ButtonText="Actualizar";
        btnClear.Location=new Point(688,26); btnClear.Size=new Size(100,36); btnClear.ButtonText="Limpiar";
        btnDownload.Location=new Point(796,26); btnDownload.Size=new Size(130,36); btnDownload.ButtonText="Descargar XML"; btnDownload.Enabled=false;
        filterPanel.Controls.AddRange(new Control[] { cmbEnvironment,cmbStatus,cmbDocumentType,cmbSourceType,dateFrom,dateTo,txtSearch,btnRefresh,btnClear,btnDownload });
        kpiPanel.Dock=DockStyle.Top; kpiPanel.Height=112; kpiPanel.Padding=new Padding(8,6,0,4); kpiPanel.WrapContents=false;
        cardTotal.Size=new Size(210,98); cardTotal.Title="TOTAL"; cardTotal.Description="Documentos registrados";
        cardPending.Size=new Size(210,98); cardPending.Title="PENDIENTES"; cardPending.Description="En espera";
        cardAuthorized.Size=new Size(210,98); cardAuthorized.Title="AUTORIZADOS"; cardAuthorized.Description="XML disponible";
        cardErrors.Size=new Size(210,98); cardErrors.Title="ERRORES"; cardErrors.Description="Fallidos o DeadLetter";
        kpiPanel.Controls.AddRange(new Control[] { cardTotal,cardPending,cardAuthorized,cardErrors });
        split.Dock=DockStyle.Fill; split.Horizontal=false; split.SplitterPosition=350;
        documentGrid.Dock=DockStyle.Fill; split.Panel1.Controls.Add(documentGrid);
        tabs.Dock=DockStyle.Fill; tabs.TabPages.AddRange(new XtraTabPage[] { detailTab,attemptsTab,auditTab,workerTab });
        detailTab.Text="Detalle"; attemptsTab.Text="Intentos"; auditTab.Text="Auditoria"; workerTab.Text="Salud del worker";
        lblDetail.Dock=DockStyle.Fill; lblDetail.Appearance.TextOptions.WordWrap=DevExpress.Utils.WordWrap.Wrap; lblDetail.AutoSizeMode=LabelAutoSizeMode.None; lblDetail.Padding=new Padding(16); detailTab.Controls.Add(lblDetail);
        attemptGrid.Dock=DockStyle.Fill; attemptsTab.Controls.Add(attemptGrid); auditGrid.Dock=DockStyle.Fill; auditTab.Controls.Add(auditGrid); lblWorkerHealth.Dock=DockStyle.Fill; lblWorkerHealth.AutoSizeMode=LabelAutoSizeMode.None; lblWorkerHealth.Padding=new Padding(16); lblWorkerHealth.Appearance.TextOptions.WordWrap=DevExpress.Utils.WordWrap.Wrap; workerTab.Controls.Add(lblWorkerHealth); split.Panel2.Controls.Add(tabs);
        AutoScaleDimensions=new SizeF(7F,15F); AutoScaleMode=AutoScaleMode.Font; ClientSize=new Size(1180,760); Controls.Add(split); Controls.Add(kpiPanel); Controls.Add(filterPanel); MinimumSize=new Size(980,650); Name="SriDocumentMonitorForm"; Text="Monitor de documentos SRI";
        ((System.ComponentModel.ISupportInitialize)cmbDocumentType.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)cmbSourceType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)filterPanel).EndInit(); filterPanel.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)cmbEnvironment.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)dateTo.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)split.Panel1).EndInit(); split.Panel1.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)split.Panel2).EndInit(); split.Panel2.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)split).EndInit(); split.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)tabs).EndInit(); tabs.ResumeLayout(false); detailTab.ResumeLayout(false); attemptsTab.ResumeLayout(false); auditTab.ResumeLayout(false); workerTab.ResumeLayout(false); ResumeLayout(false);
    }
}
