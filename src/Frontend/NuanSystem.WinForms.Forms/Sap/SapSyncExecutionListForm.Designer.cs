using NuanSystem.WinForms.Controls.Grids;

namespace NuanSystem.WinForms.Forms.Sap;

#nullable enable
partial class SapSyncExecutionListForm
{
    private System.ComponentModel.IContainer? components;
    private NuanDataGridControl executionGrid = null!;
    private System.Windows.Forms.Timer pollingTimer = null!;

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
        components = new System.ComponentModel.Container();
        executionGrid = new NuanDataGridControl();
        pollingTimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        executionGrid.Dock = DockStyle.Fill;
        executionGrid.EnableColumnCustomization = true;
        executionGrid.FormKey = "sap-sync-executions";
        executionGrid.GridName = "MainGrid";
        executionGrid.Location = new Point(0, 0);
        executionGrid.MultiSelect = false;
        executionGrid.Name = "executionGrid";
        executionGrid.PageSize = 50;
        executionGrid.ShowFindPanel = true;
        executionGrid.ShowPagination = true;
        executionGrid.ShowSelectionCheckBox = false;
        executionGrid.Size = new Size(1180, 720);
        executionGrid.TabIndex = 0;
        pollingTimer.Enabled = false;
        pollingTimer.Interval = 7000;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 720);
        Controls.Add(executionGrid);
        MinimumSize = new Size(960, 560);
        Name = "SapSyncExecutionListForm";
        Text = "Ejecuciones SAP";
        ResumeLayout(false);
    }
}
