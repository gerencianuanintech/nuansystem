using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;

namespace NuanSystem.WinForms.Forms.Definitions.General.Provinces;

partial class ProvinceEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCountry = new LabelControl();
        lueCountry = new NuanLookupEdit();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        btnCancelar.Location = new Point(294, 152);
        btnCancelar.TabIndex = 4;
        btnGuardar.Location = new Point(400, 152);
        btnGuardar.TabIndex = 5;
        lblCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblCountry.Appearance.Options.UseFont = true;
        lblCountry.Location = new Point(28, 29);
        lblCountry.Name = "lblCountry";
        lblCountry.Size = new Size(21, 15);
        lblCountry.Text = "País";
        lueCountry.Location = new Point(170, 26);
        lueCountry.Name = "lueCountry";
        lueCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCountry.Properties.Appearance.Options.UseFont = true;
        lueCountry.Properties.AutoHeight = false;
        lueCountry.Properties.Buttons.Clear();
        lueCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueCountry.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80), new LookUpColumnInfo("Name", "Nombre", 180) });
        lueCountry.Properties.NullText = "";
        lueCountry.Size = new Size(330, 22);
        lueCountry.TabIndex = 0;
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Location = new Point(28, 57);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(40, 15);
        lblCode.Text = "Código";
        txtCode.Location = new Point(170, 54);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 20;
        txtCode.Size = new Size(330, 22);
        txtCode.TabIndex = 1;
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.Options.UseFont = true;
        lblName.Location = new Point(28, 85);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.Text = "Nombre";
        txtName.Location = new Point(170, 82);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 120;
        txtName.Size = new Size(330, 22);
        txtName.TabIndex = 2;
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(166, 112);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 3;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(528, 210);
        Controls.Add(lblCountry);
        Controls.Add(lueCountry);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(chkIsActive);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProvinceEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Provincia";
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private LabelControl lblCountry;
    private NuanLookupEdit lueCountry;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private CheckEdit chkIsActive;
}
