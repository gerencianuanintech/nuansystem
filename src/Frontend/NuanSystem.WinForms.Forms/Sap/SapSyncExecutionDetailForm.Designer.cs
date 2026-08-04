using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Grids;

namespace NuanSystem.WinForms.Forms.Sap;

#nullable enable
partial class SapSyncExecutionDetailForm
{
    private System.ComponentModel.IContainer? components;
    private PanelControl actionsPanel = null!;
    private NuanActionButton refreshButton = null!;
    private NuanActionButton retryButton = null!;
    private NuanActionButton cancelButton = null!;
    private NuanActionButton releaseButton = null!;
    private MemoEdit summaryEdit = null!;
    private NuanDataGridControl detailGrid = null!;
    private System.Windows.Forms.Timer pollingTimer = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        actionsPanel = new PanelControl();
        refreshButton = new NuanActionButton();
        retryButton = new NuanActionButton();
        cancelButton = new NuanActionButton();
        releaseButton = new NuanActionButton();
        summaryEdit = new MemoEdit();
        detailGrid = new NuanDataGridControl();
        pollingTimer = new System.Windows.Forms.Timer(components);
        ((System.ComponentModel.ISupportInitialize)actionsPanel).BeginInit();
        actionsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).BeginInit();
        SuspendLayout();
        actionsPanel.Controls.AddRange(new Control[] { refreshButton, retryButton, cancelButton, releaseButton });
        actionsPanel.Dock = DockStyle.Top;
        actionsPanel.Location = new Point(0, 0);
        actionsPanel.Name = "actionsPanel";
        actionsPanel.Size = new Size(1080, 52);
        actionsPanel.TabIndex = 0;
        refreshButton.ButtonKind = NuanActionButtonKind.Save;
        refreshButton.ButtonText = "Actualizar";
        refreshButton.IconNameOverride = "actualizar_16.svg";
        refreshButton.IconSize = 16;
        refreshButton.Location = new Point(12, 8);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(100, 36);
        refreshButton.TabIndex = 0;
        retryButton.ButtonKind = NuanActionButtonKind.Save;
        retryButton.ButtonText = "Reintentar";
        retryButton.IconNameOverride = "reintentar_ejecucion_16.svg";
        retryButton.IconSize = 16;
        retryButton.Location = new Point(120, 8);
        retryButton.Name = "retryButton";
        retryButton.Size = new Size(100, 36);
        retryButton.TabIndex = 1;
        cancelButton.ButtonKind = NuanActionButtonKind.Cancel;
        cancelButton.ButtonText = "Cancelar";
        cancelButton.IconNameOverride = "cancelar_16.svg";
        cancelButton.IconSize = 16;
        cancelButton.Location = new Point(228, 8);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(100, 36);
        cancelButton.TabIndex = 2;
        releaseButton.ButtonKind = NuanActionButtonKind.Save;
        releaseButton.ButtonText = "Liberar lock";
        releaseButton.IconNameOverride = "liberar_lock_vencido_16.svg";
        releaseButton.IconSize = 16;
        releaseButton.Location = new Point(336, 8);
        releaseButton.Name = "releaseButton";
        releaseButton.Size = new Size(100, 36);
        releaseButton.TabIndex = 3;
        summaryEdit.Dock = DockStyle.Top;
        summaryEdit.Location = new Point(0, 52);
        summaryEdit.Name = "summaryEdit";
        summaryEdit.Properties.ReadOnly = true;
        summaryEdit.Size = new Size(1080, 150);
        summaryEdit.TabIndex = 1;
        detailGrid.Dock = DockStyle.Fill;
        detailGrid.EnableColumnCustomization = true;
        detailGrid.FormKey = "sap-sync-executions";
        detailGrid.GridName = "Details";
        detailGrid.Location = new Point(0, 202);
        detailGrid.MultiSelect = false;
        detailGrid.Name = "detailGrid";
        detailGrid.PageSize = 100;
        detailGrid.ShowFindPanel = true;
        detailGrid.ShowPagination = true;
        detailGrid.ShowSelectionCheckBox = false;
        detailGrid.Size = new Size(1080, 458);
        detailGrid.TabIndex = 2;
        pollingTimer.Enabled = false;
        pollingTimer.Interval = 7000;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1080, 660);
        Controls.Add(detailGrid);
        Controls.Add(summaryEdit);
        Controls.Add(actionsPanel);
        MinimumSize = new Size(900, 560);
        Name = "SapSyncExecutionDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Detalle de ejecucion SAP";
        ((System.ComponentModel.ISupportInitialize)actionsPanel).EndInit();
        actionsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).EndInit();
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
