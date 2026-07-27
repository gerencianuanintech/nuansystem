namespace NuanSystem.WinForms.Forms.TaxCatalogs.Taxes;

partial class TaxesForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 600);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(860, 500);
        Name = "TaxesForm";
        Text = "Impuestos";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
}
