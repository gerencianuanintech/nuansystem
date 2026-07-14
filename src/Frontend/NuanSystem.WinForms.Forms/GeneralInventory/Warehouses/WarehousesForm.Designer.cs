namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

partial class WarehousesForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1120, 640);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(920, 520);
        Name = "WarehousesForm";
        Text = "Bodegas";
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
