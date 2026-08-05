using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;

namespace NuanSystem.WinForms.Forms.Definitions.General.Cities;

partial class CityEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCountry = new LabelControl();
        lueCountry = new NuanLookupEdit();
        lblProvince = new LabelControl();
        lueProvince = new NuanLookupEdit();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProvince.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        btnCancelar.Location = new Point(294, 180);
        btnCancelar.TabIndex = 5;
        btnGuardar.Location = new Point(400, 180);
        btnGuardar.TabIndex = 6;
        lblCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblCountry.Appearance.Options.UseFont = true;
        lblCountry.Location = new Point(28, 29);
        lblCountry.Name = "lblCountry";
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
        lblProvince.Appearance.Font = new Font("Segoe UI", 9F);
        lblProvince.Appearance.Options.UseFont = true;
        lblProvince.Location = new Point(28, 57);
        lblProvince.Name = "lblProvince";
        lblProvince.Text = "Provincia";
        lueProvince.Location = new Point(170, 54);
        lueProvince.Name = "lueProvince";
        lueProvince.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueProvince.Properties.Appearance.Options.UseFont = true;
        lueProvince.Properties.AutoHeight = false;
        lueProvince.Properties.Buttons.Clear();
        lueProvince.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueProvince.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80), new LookUpColumnInfo("Name", "Nombre", 180) });
        lueProvince.Properties.NullText = "";
        lueProvince.Size = new Size(330, 22);
        lueProvince.TabIndex = 1;
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Location = new Point(28, 85);
        lblCode.Name = "lblCode";
        lblCode.Text = "Código";
        txtCode.Location = new Point(170, 82);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 20;
        txtCode.Size = new Size(330, 22);
        txtCode.TabIndex = 2;
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.Options.UseFont = true;
        lblName.Location = new Point(28, 113);
        lblName.Name = "lblName";
        lblName.Text = "Nombre";
        txtName.Location = new Point(170, 110);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 120;
        txtName.Size = new Size(330, 22);
        txtName.TabIndex = 3;
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(166, 140);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 4;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(528, 238);
        Controls.Add(lblCountry);
        Controls.Add(lueCountry);
        Controls.Add(lblProvince);
        Controls.Add(lueProvince);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(chkIsActive);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CityEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Ciudad";
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueProvince.Properties).EndInit();
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
    private LabelControl lblProvince;
    private NuanLookupEdit lueProvince;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private CheckEdit chkIsActive;
}
