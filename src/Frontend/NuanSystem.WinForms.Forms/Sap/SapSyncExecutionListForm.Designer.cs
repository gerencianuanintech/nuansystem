namespace NuanSystem.WinForms.Forms.Sap;

partial class SapSyncExecutionListForm
{
    private System.ComponentModel.IContainer components = null; private System.Windows.Forms.Timer pollingTimer;
    private void InitializeComponent() { components = new System.ComponentModel.Container(); pollingTimer = new System.Windows.Forms.Timer(components); SuspendLayout(); pollingTimer.Enabled = false; pollingTimer.Interval = 7000; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(1180, 720); MinimumSize = new Size(960, 560); Name = "SapSyncExecutionListForm"; Text = "Ejecuciones SAP"; ResumeLayout(false); }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
