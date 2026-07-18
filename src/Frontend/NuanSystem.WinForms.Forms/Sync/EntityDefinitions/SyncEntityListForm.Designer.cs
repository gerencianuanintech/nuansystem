namespace NuanSystem.WinForms.Forms.Sync.EntityDefinitions;

partial class SyncEntityListForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        SuspendLayout();
        ClientSize = new Size(1180, 720);
        // 
        // SyncEntityListForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        MinimumSize = new Size(960, 560);
        Name = "SyncEntityListForm";
        Text = "Entidades de sincronizacion";
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
