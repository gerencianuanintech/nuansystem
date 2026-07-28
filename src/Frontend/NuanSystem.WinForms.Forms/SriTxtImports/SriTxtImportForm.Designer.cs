using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Controls.Kpi;

namespace NuanSystem.WinForms.Forms.SriTxtImports;

#nullable enable
partial class SriTxtImportForm
{
    private System.ComponentModel.IContainer? components;
    private FlowLayoutPanel kpiPanel = null!;
    private NuanKpiCardControl cardTotal = null!;
    private NuanKpiCardControl cardValid = null!;
    private NuanKpiCardControl cardInvalid = null!;
    private NuanKpiCardControl cardLinked = null!;
    private NuanKpiCardControl cardStaged = null!;
    private NuanKpiCardControl cardPending = null!;
    private SplitContainerControl split = null!;
    private NuanDataGridControl importGrid = null!;
    private PanelControl rowHeaderPanel = null!;
    private LabelControl lblDetail = null!;
    private NuanDataGridControl rowGrid = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        kpiPanel = new FlowLayoutPanel();
        cardTotal = new NuanKpiCardControl();
        cardValid = new NuanKpiCardControl();
        cardInvalid = new NuanKpiCardControl();
        cardLinked = new NuanKpiCardControl();
        cardStaged = new NuanKpiCardControl();
        cardPending = new NuanKpiCardControl();
        split = new SplitContainerControl();
        importGrid = new NuanDataGridControl();
        rowHeaderPanel = new PanelControl();
        lblDetail = new LabelControl();
        rowGrid = new NuanDataGridControl();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        ((System.ComponentModel.ISupportInitialize)split.Panel1).BeginInit();
        split.Panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split.Panel2).BeginInit();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)rowHeaderPanel).BeginInit();
        rowHeaderPanel.SuspendLayout();
        SuspendLayout();

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
        split.Panel1.Controls.Add(importGrid);

        rowHeaderPanel.Dock = DockStyle.Top;
        rowHeaderPanel.Height = 58;
        lblDetail.Location = new Point(12, 10);
        lblDetail.Size = new Size(880, 38);
        lblDetail.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetail.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        lblDetail.Text = "Seleccione una importación.";
        rowHeaderPanel.Controls.Add(lblDetail);
        rowGrid.Dock = DockStyle.Fill;
        split.Panel2.Controls.Add(rowGrid);
        split.Panel2.Controls.Add(rowHeaderPanel);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 780);
        Controls.Add(split);
        Controls.Add(kpiPanel);
        MinimumSize = new Size(1151, 700);
        Name = "SriTxtImportForm";
        Text = "Importaciones TXT SRI";

        ((System.ComponentModel.ISupportInitialize)split.Panel1).EndInit();
        split.Panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split.Panel2).EndInit();
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)rowHeaderPanel).EndInit();
        rowHeaderPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private static void ConfigureCard(NuanKpiCardControl card, string title, string description)
    {
        card.Size = new Size(183, 92);
        card.Title = title;
        card.Description = description;
    }
}
