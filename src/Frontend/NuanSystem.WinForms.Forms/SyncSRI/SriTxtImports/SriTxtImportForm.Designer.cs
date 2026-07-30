using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Controls.Kpi;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.SriTxtImports;

#nullable enable
partial class SriTxtImportForm
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel kpiPanel = null!;
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
        kpiPanel = new TableLayoutPanel();
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

        kpiPanel.ColumnCount = 6;
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        kpiPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
        kpiPanel.RowCount = 1;
        kpiPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        kpiPanel.Controls.Add(cardTotal, 0, 0);
        kpiPanel.Controls.Add(cardValid, 1, 0);
        kpiPanel.Controls.Add(cardInvalid, 2, 0);
        kpiPanel.Controls.Add(cardLinked, 3, 0);
        kpiPanel.Controls.Add(cardStaged, 4, 0);
        kpiPanel.Controls.Add(cardPending, 5, 0);
        kpiPanel.Dock = DockStyle.Top;
        kpiPanel.Height = 100;
        kpiPanel.Name = "kpiPanel";
        kpiPanel.Padding = new Padding(8, 4, 8, 4);

        cardTotal.Description = "Total filtrado";
        cardTotal.Dock = DockStyle.Fill;
        cardTotal.HeaderColor = BrandResources.Primary;
        cardTotal.HeaderHeight = 58;
        cardTotal.Margin = new Padding(4);
        cardTotal.MinimumSize = Size.Empty;
        cardTotal.Name = "cardTotal";
        cardTotal.Size = new Size(130, 92);
        cardTotal.Title = "FILAS";

        cardValid.Description = "Filas válidas";
        cardValid.Dock = DockStyle.Fill;
        cardValid.HeaderColor = BrandResources.Primary;
        cardValid.HeaderHeight = 58;
        cardValid.Margin = new Padding(4);
        cardValid.MinimumSize = Size.Empty;
        cardValid.Name = "cardValid";
        cardValid.Size = new Size(130, 92);
        cardValid.Title = "VÁLIDAS";

        cardInvalid.Description = "Incluye duplicadas";
        cardInvalid.Dock = DockStyle.Fill;
        cardInvalid.HeaderColor = BrandResources.Primary;
        cardInvalid.HeaderHeight = 58;
        cardInvalid.Margin = new Padding(4);
        cardInvalid.MinimumSize = Size.Empty;
        cardInvalid.Name = "cardInvalid";
        cardInvalid.Size = new Size(130, 92);
        cardInvalid.Title = "INVÁLIDAS";

        cardLinked.Description = "Colas preexistentes";
        cardLinked.Dock = DockStyle.Fill;
        cardLinked.HeaderColor = BrandResources.Primary;
        cardLinked.HeaderHeight = 58;
        cardLinked.Margin = new Padding(4);
        cardLinked.MinimumSize = Size.Empty;
        cardLinked.Name = "cardLinked";
        cardLinked.Size = new Size(130, 92);
        cardLinked.Title = "VINCULADAS";

        cardStaged.Description = "Staged";
        cardStaged.Dock = DockStyle.Fill;
        cardStaged.HeaderColor = BrandResources.Primary;
        cardStaged.HeaderHeight = 58;
        cardStaged.Margin = new Padding(4);
        cardStaged.MinimumSize = Size.Empty;
        cardStaged.Name = "cardStaged";
        cardStaged.Size = new Size(130, 92);
        cardStaged.Title = "PREPARADAS";

        cardPending.Description = "Pending";
        cardPending.Dock = DockStyle.Fill;
        cardPending.HeaderColor = BrandResources.Primary;
        cardPending.HeaderHeight = 58;
        cardPending.Margin = new Padding(4);
        cardPending.MinimumSize = Size.Empty;
        cardPending.Name = "cardPending";
        cardPending.Size = new Size(130, 92);
        cardPending.Title = "PENDIENTES";

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
        MinimumSize = new Size(860, 700);
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

}
