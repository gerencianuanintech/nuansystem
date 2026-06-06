using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.OperationalCatalogs;

partial class OperationalCatalogsForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        pnlFilters = new PanelControl();
        lblCatalogKey = new LabelControl();
        lueCatalogKey = new LookUpEdit();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).BeginInit();
        pnlFilters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueCatalogKey.Properties).BeginInit();
        SuspendLayout();
        pnlFilters.Appearance.BackColor = Color.White;
        pnlFilters.Appearance.Options.UseBackColor = true;
        pnlFilters.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlFilters.Controls.Add(lblCatalogKey);
        pnlFilters.Controls.Add(lueCatalogKey);
        pnlFilters.Dock = DockStyle.Top;
        pnlFilters.Location = new Point(0, 0);
        pnlFilters.Name = "pnlFilters";
        pnlFilters.Size = new Size(900, 42);
        pnlFilters.TabIndex = 0;
        lblCatalogKey.Appearance.Font = new Font("Segoe UI", 9F);
        lblCatalogKey.Appearance.ForeColor = Color.Black;
        lblCatalogKey.Appearance.Options.UseFont = true;
        lblCatalogKey.Appearance.Options.UseForeColor = true;
        lblCatalogKey.Location = new Point(12, 14);
        lblCatalogKey.Name = "lblCatalogKey";
        lblCatalogKey.Size = new Size(50, 15);
        lblCatalogKey.TabIndex = 0;
        lblCatalogKey.Text = "Catalogo";
        lueCatalogKey.Location = new Point(78, 10);
        lueCatalogKey.Name = "lueCatalogKey";
        lueCatalogKey.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCatalogKey.Properties.Appearance.Options.UseFont = true;
        lueCatalogKey.Properties.AutoHeight = false;
        lueCatalogKey.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueCatalogKey.Size = new Size(260, 22);
        lueCatalogKey.TabIndex = 1;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(pnlFilters);
        Name = "OperationalCatalogsForm";
        Text = "Catalogos operativos";
        ((System.ComponentModel.ISupportInitialize)pnlFilters).EndInit();
        pnlFilters.ResumeLayout(false);
        pnlFilters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueCatalogKey.Properties).EndInit();
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

    private PanelControl pnlFilters;
    private LabelControl lblCatalogKey;
    private LookUpEdit lueCatalogKey;
}
