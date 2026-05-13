namespace NuanSystem.WinForms.Forms.ConfigurationSettings;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // SettingsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 600);
        Name = "SettingsForm";
        Text = "Parametros";
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
