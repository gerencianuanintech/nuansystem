using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Forms.Definitions.General.Cities;

partial class CityEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCountry = new LabelControl();
        lueCountry = new LookUpEdit();
        lblProvince = new LabelControl();
        lueProvince = new LookUpEdit();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        chkIsActive = new CheckEdit();
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProvince.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
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
        lueCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCountry.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80), new LookUpColumnInfo("Name", "Nombre", 180) });
        lueCountry.Properties.NullText = "";
        lueCountry.Size = new Size(330, 22);
        lueCountry.TabIndex = 0;
        lblProvince.Appearance.Font = new Font("Segoe UI", 9F);
        lblProvince.Appearance.Options.UseFont = true;
        lblProvince.Location = new Point(28, 60);
        lblProvince.Name = "lblProvince";
        lblProvince.Text = "Provincia";
        lueProvince.Location = new Point(170, 57);
        lueProvince.Name = "lueProvince";
        lueProvince.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueProvince.Properties.Appearance.Options.UseFont = true;
        lueProvince.Properties.AutoHeight = false;
        lueProvince.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueProvince.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80), new LookUpColumnInfo("Name", "Nombre", 180) });
        lueProvince.Properties.NullText = "";
        lueProvince.Size = new Size(330, 22);
        lueProvince.TabIndex = 1;
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Location = new Point(28, 91);
        lblCode.Name = "lblCode";
        lblCode.Text = "Código";
        txtCode.Location = new Point(170, 88);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 20;
        txtCode.Size = new Size(330, 22);
        txtCode.TabIndex = 2;
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.Options.UseFont = true;
        lblName.Location = new Point(28, 122);
        lblName.Name = "lblName";
        lblName.Text = "Nombre";
        txtName.Location = new Point(170, 119);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 120;
        txtName.Size = new Size(330, 22);
        txtName.TabIndex = 3;
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(166, 153);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 4;
        btnCancel.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.White;
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(294, 201);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 5;
        btnCancel.Text = "Cancelar";
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.Location = new Point(400, 201);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 6;
        btnSave.Text = "Guardar";
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(528, 259);
        Controls.Add(lblCountry);
        Controls.Add(lueCountry);
        Controls.Add(lblProvince);
        Controls.Add(lueProvince);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(chkIsActive);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
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
    private LookUpEdit lueCountry;
    private LabelControl lblProvince;
    private LookUpEdit lueProvince;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private CheckEdit chkIsActive;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;
}
