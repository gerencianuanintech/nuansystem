namespace NuanSystem.WinForms.Forms.ConfigurationCompanies;

partial class ConfigurationCompaniesForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // ConfigurationCompaniesForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(980, 560);
        Name = "ConfigurationCompaniesForm";
        Text = "Companias";
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
