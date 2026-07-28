using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.Taxes;

partial class TaxEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblRate = new LabelControl();
        spnRate = new SpinEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnRate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        lblCode.Location = new Point(28, 29); lblCode.Name = "lblCode"; lblCode.Text = "Código";
        txtCode.Location = new Point(170, 26); txtCode.Name = "txtCode"; txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50; txtCode.Size = new Size(330, 22); txtCode.TabIndex = 1;
        lblName.Location = new Point(28, 60); lblName.Name = "lblName"; lblName.Text = "Nombre";
        txtName.Location = new Point(170, 57); txtName.Name = "txtName"; txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150; txtName.Size = new Size(330, 22); txtName.TabIndex = 3;
        lblDescription.Location = new Point(28, 91); lblDescription.Name = "lblDescription"; lblDescription.Text = "Descripción";
        memDescription.Location = new Point(170, 88); memDescription.Name = "memDescription";
        memDescription.Properties.MaxLength = 500; memDescription.Size = new Size(330, 74); memDescription.TabIndex = 5;
        lblRate.Location = new Point(28, 176); lblRate.Name = "lblRate"; lblRate.Text = "Porcentaje";
        spnRate.Location = new Point(170, 173); spnRate.Name = "spnRate"; spnRate.Properties.AutoHeight = false;
        spnRate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[]
        {
            new(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
        });
        spnRate.Properties.DisplayFormat.FormatString = "n2 ' %'"; spnRate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnRate.Properties.EditFormat.FormatString = "n2"; spnRate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnRate.Properties.MaskSettings.Set("mask", "n2"); spnRate.Properties.MaxValue = 100m;
        spnRate.Size = new Size(120, 22); spnRate.TabIndex = 7;
        chkIsActive.Location = new Point(166, 207); chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Caption = "Activo"; chkIsActive.Size = new Size(75, 20); chkIsActive.TabIndex = 8;
        btnCancelar.Location = new Point(294, 249); btnCancelar.TabIndex = 9;
        btnGuardar.Location = new Point(400, 249); btnGuardar.TabIndex = 10;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(528, 307);
        Controls.AddRange(new Control[] { lblCode, txtCode, lblName, txtName, lblDescription, memDescription,
            lblRate, spnRate, chkIsActive });
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        Font = new Font("Segoe UI", 9F); FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false; Name = "TaxEditForm"; StartPosition = FormStartPosition.CenterParent; Text = "Nuevo impuesto";
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnRate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ResumeLayout(false); PerformLayout();
    }

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblRate;
    private SpinEdit spnRate;
    private CheckEdit chkIsActive;
}
