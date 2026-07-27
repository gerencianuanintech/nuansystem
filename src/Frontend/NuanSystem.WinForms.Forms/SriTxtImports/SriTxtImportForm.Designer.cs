using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Controls.Kpi;

namespace NuanSystem.WinForms.Forms.SriTxtImports;

#nullable enable
partial class SriTxtImportForm
{
    private System.ComponentModel.IContainer? components;
    private PanelControl filterPanel = null!;
    private DateEdit dateFrom = null!;
    private DateEdit dateTo = null!;
    private ComboBoxEdit cmbStatus = null!;
    private ComboBoxEdit cmbEnvironment = null!;
    private TextEdit txtFileName = null!;
    private NuanActionButton btnRefresh = null!;
    private NuanActionButton btnClear = null!;
    private NuanActionButton btnEnqueue = null!;
    private NuanActionButton btnOpenQueue = null!;
    private FlowLayoutPanel kpiPanel = null!;
    private NuanKpiCardControl cardTotal = null!;
    private NuanKpiCardControl cardValid = null!;
    private NuanKpiCardControl cardInvalid = null!;
    private NuanKpiCardControl cardLinked = null!;
    private NuanKpiCardControl cardStaged = null!;
    private NuanKpiCardControl cardPending = null!;
    private SplitContainerControl split = null!;
    private NuanDataGridControl importGrid = null!;
    private PanelControl importPagePanel = null!;
    private NuanActionButton btnImportPrevious = null!;
    private LabelControl lblImportPage = null!;
    private NuanActionButton btnImportNext = null!;
    private PanelControl rowHeaderPanel = null!;
    private LabelControl lblDetail = null!;
    private ComboBoxEdit cmbValidity = null!;
    private NuanDataGridControl rowGrid = null!;
    private PanelControl rowPagePanel = null!;
    private NuanActionButton btnRowPrevious = null!;
    private LabelControl lblRowPage = null!;
    private NuanActionButton btnRowNext = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        filterPanel = new PanelControl();
        dateFrom = new DateEdit();
        dateTo = new DateEdit();
        cmbStatus = new ComboBoxEdit();
        cmbEnvironment = new ComboBoxEdit();
        txtFileName = new TextEdit();
        btnRefresh = new NuanActionButton();
        btnClear = new NuanActionButton();
        btnEnqueue = new NuanActionButton();
        btnOpenQueue = new NuanActionButton();
        kpiPanel = new FlowLayoutPanel();
        cardTotal = new NuanKpiCardControl();
        cardValid = new NuanKpiCardControl();
        cardInvalid = new NuanKpiCardControl();
        cardLinked = new NuanKpiCardControl();
        cardStaged = new NuanKpiCardControl();
        cardPending = new NuanKpiCardControl();
        split = new SplitContainerControl();
        importGrid = new NuanDataGridControl();
        importPagePanel = new PanelControl();
        btnImportPrevious = new NuanActionButton();
        lblImportPage = new LabelControl();
        btnImportNext = new NuanActionButton();
        rowHeaderPanel = new PanelControl();
        lblDetail = new LabelControl();
        cmbValidity = new ComboBoxEdit();
        rowGrid = new NuanDataGridControl();
        rowPagePanel = new PanelControl();
        btnRowPrevious = new NuanActionButton();
        lblRowPage = new LabelControl();
        btnRowNext = new NuanActionButton();
        ((System.ComponentModel.ISupportInitialize)filterPanel).BeginInit();
        filterPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbEnvironment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        ((System.ComponentModel.ISupportInitialize)split.Panel1).BeginInit();
        split.Panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split.Panel2).BeginInit();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)importPagePanel).BeginInit();
        importPagePanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)rowHeaderPanel).BeginInit();
        rowHeaderPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)cmbValidity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)rowPagePanel).BeginInit();
        rowPagePanel.SuspendLayout();
        SuspendLayout();

        filterPanel.Dock = DockStyle.Top;
        filterPanel.Height = 88;
        filterPanel.Padding = new Padding(12);
        dateFrom.Location = new Point(12, 14);
        dateFrom.Size = new Size(120, 22);
        dateFrom.Properties.NullText = "Desde";
        dateTo.Location = new Point(140, 14);
        dateTo.Size = new Size(120, 22);
        dateTo.Properties.NullText = "Hasta";
        cmbStatus.Location = new Point(268, 14);
        cmbStatus.Size = new Size(178, 22);
        cmbStatus.Properties.NullText = "Estado";
        cmbStatus.Properties.Items.AddRange(new object[] { "Validated", "ValidatedWithErrors", "Completed", "CompletedWithErrors" });
        cmbEnvironment.Location = new Point(454, 14);
        cmbEnvironment.Size = new Size(130, 22);
        cmbEnvironment.Properties.NullText = "Ambiente";
        cmbEnvironment.Properties.Items.AddRange(new object[] { "Test", "Production" });
        txtFileName.Location = new Point(12, 50);
        txtFileName.Size = new Size(432, 22);
        txtFileName.Properties.NullValuePrompt = "Nombre de archivo";
        btnRefresh.Location = new Point(604, 12);
        btnRefresh.Size = new Size(108, 36);
        btnRefresh.ButtonText = "Actualizar";
        btnClear.Location = new Point(720, 12);
        btnClear.Size = new Size(92, 36);
        btnClear.ButtonText = "Limpiar";
        btnEnqueue.Location = new Point(820, 12);
        btnEnqueue.Size = new Size(104, 36);
        btnEnqueue.ButtonText = "Encolar";
        btnOpenQueue.Location = new Point(932, 12);
        btnOpenQueue.Size = new Size(120, 36);
        btnOpenQueue.ButtonText = "Abrir cola";
        filterPanel.Controls.AddRange(new Control[] { dateFrom, dateTo, cmbStatus, cmbEnvironment, txtFileName, btnRefresh, btnClear, btnEnqueue, btnOpenQueue });

        kpiPanel.Dock = DockStyle.Top;
        kpiPanel.Height = 104;
        kpiPanel.Padding = new Padding(8, 5, 0, 3);
        kpiPanel.WrapContents = false;
        ConfigureCard(cardTotal, "FILAS", "Total filtrado");
        ConfigureCard(cardValid, "VÁLIDAS", "Filas válidas");
        ConfigureCard(cardInvalid, "INVÁLIDAS", "Incluye duplicadas");
        ConfigureCard(cardLinked, "VINCULADAS", "Colas preexistentes");
        ConfigureCard(cardStaged, "PREPARADAS", "Staged");
        ConfigureCard(cardPending, "PENDIENTES", "Pending");
        kpiPanel.Controls.AddRange(new Control[] { cardTotal, cardValid, cardInvalid, cardLinked, cardStaged, cardPending });

        split.Dock = DockStyle.Fill;
        split.Horizontal = false;
        split.SplitterPosition = 280;
        importGrid.Dock = DockStyle.Fill;
        importPagePanel.Dock = DockStyle.Bottom;
        importPagePanel.Height = 42;
        btnImportPrevious.Location = new Point(10, 4);
        btnImportPrevious.Size = new Size(90, 34);
        btnImportPrevious.ButtonText = "Anterior";
        lblImportPage.Location = new Point(112, 14);
        lblImportPage.Text = "Página 1 de 1";
        btnImportNext.Location = new Point(360, 4);
        btnImportNext.Size = new Size(90, 34);
        btnImportNext.ButtonText = "Siguiente";
        importPagePanel.Controls.AddRange(new Control[] { btnImportPrevious, lblImportPage, btnImportNext });
        split.Panel1.Controls.Add(importGrid);
        split.Panel1.Controls.Add(importPagePanel);

        rowHeaderPanel.Dock = DockStyle.Top;
        rowHeaderPanel.Height = 58;
        lblDetail.Location = new Point(12, 10);
        lblDetail.Size = new Size(880, 38);
        lblDetail.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetail.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        lblDetail.Text = "Seleccione una importación.";
        cmbValidity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cmbValidity.Location = new Point(1014, 18);
        cmbValidity.Size = new Size(130, 22);
        cmbValidity.Properties.Items.AddRange(new object[] { "All", "Valid", "Invalid" });
        cmbValidity.EditValue = "All";
        rowHeaderPanel.Controls.AddRange(new Control[] { lblDetail, cmbValidity });
        rowGrid.Dock = DockStyle.Fill;
        rowPagePanel.Dock = DockStyle.Bottom;
        rowPagePanel.Height = 42;
        btnRowPrevious.Location = new Point(10, 4);
        btnRowPrevious.Size = new Size(90, 34);
        btnRowPrevious.ButtonText = "Anterior";
        lblRowPage.Location = new Point(112, 14);
        lblRowPage.Text = "Página 1 de 1";
        btnRowNext.Location = new Point(360, 4);
        btnRowNext.Size = new Size(90, 34);
        btnRowNext.ButtonText = "Siguiente";
        rowPagePanel.Controls.AddRange(new Control[] { btnRowPrevious, lblRowPage, btnRowNext });
        split.Panel2.Controls.Add(rowGrid);
        split.Panel2.Controls.Add(rowHeaderPanel);
        split.Panel2.Controls.Add(rowPagePanel);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 780);
        Controls.Add(split);
        Controls.Add(kpiPanel);
        Controls.Add(filterPanel);
        MinimumSize = new Size(1040, 700);
        Name = "SriTxtImportForm";
        Text = "Importaciones TXT SRI";

        ((System.ComponentModel.ISupportInitialize)filterPanel).EndInit();
        filterPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbEnvironment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)split.Panel1).EndInit();
        split.Panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split.Panel2).EndInit();
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)importPagePanel).EndInit();
        importPagePanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)rowHeaderPanel).EndInit();
        rowHeaderPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)cmbValidity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)rowPagePanel).EndInit();
        rowPagePanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private static void ConfigureCard(NuanKpiCardControl card, string title, string description)
    {
        card.Size = new Size(183, 92);
        card.Title = title;
        card.Description = description;
    }
}
