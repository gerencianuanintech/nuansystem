using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class SyncProfileListForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // SyncProfileListForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 720);
        MinimumSize = new Size(960, 560);
        Name = "SyncProfileListForm";
        Text = "Perfiles de sincronizacion";
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
