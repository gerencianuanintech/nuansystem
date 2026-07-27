using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;

namespace NuanSystem.WinForms.Forms.FinancialCatalogs.PriceLists;

partial class PriceListEditForm
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblCurrency;
    private NuanLookupEdit lueCurrency;
    private LabelControl lblAppliesTo;
    private LookUpEdit lueAppliesTo;
    private CheckEdit chkIsDefault;
    private CheckEdit chkIsActive;

    private void InitializeComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblCurrency = new LabelControl();
        lueCurrency = new NuanLookupEdit();
        lblAppliesTo = new LabelControl();
        lueAppliesTo = new LookUpEdit();
        chkIsDefault = new CheckEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAppliesTo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        //
        // lblCode
        //
        lblCode.Location = new Point(28, 29);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(40, 15);
        lblCode.TabIndex = 0;
        lblCode.Text = "Código";
        //
        // txtCode
        //
        txtCode.Location = new Point(170, 26);
        txtCode.Name = "txtCode";
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 30;
        txtCode.Size = new Size(330, 22);
        txtCode.TabIndex = 1;
        //
        // lblName
        //
        lblName.Location = new Point(28, 57);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 2;
        lblName.Text = "Nombre";
        //
        // txtName
        //
        txtName.Location = new Point(170, 54);
        txtName.Name = "txtName";
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 120;
        txtName.Size = new Size(330, 22);
        txtName.TabIndex = 3;
        //
        // lblDescription
        //
        lblDescription.Location = new Point(28, 85);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(63, 15);
        lblDescription.TabIndex = 4;
        lblDescription.Text = "Descripción";
        //
        // memDescription
        //
        memDescription.Location = new Point(170, 82);
        memDescription.Name = "memDescription";
        memDescription.Properties.MaxLength = 300;
        memDescription.Size = new Size(330, 58);
        memDescription.TabIndex = 5;
        //
        // lblCurrency
        //
        lblCurrency.Location = new Point(28, 153);
        lblCurrency.Name = "lblCurrency";
        lblCurrency.Size = new Size(43, 15);
        lblCurrency.TabIndex = 6;
        lblCurrency.Text = "Moneda";
        //
        // lueCurrency
        //
        lueCurrency.Location = new Point(170, 150);
        lueCurrency.Name = "lueCurrency";
        lueCurrency.Properties.AutoHeight = false;
        lueCurrency.Properties.Buttons.Clear();
        lueCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus) });
        lueCurrency.Properties.NullText = "";
        lueCurrency.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueCurrency.Size = new Size(330, 22);
        lueCurrency.TabIndex = 7;
        //
        // lblAppliesTo
        //
        lblAppliesTo.Location = new Point(28, 181);
        lblAppliesTo.Name = "lblAppliesTo";
        lblAppliesTo.Size = new Size(42, 15);
        lblAppliesTo.TabIndex = 8;
        lblAppliesTo.Text = "Aplica a";
        //
        // lueAppliesTo
        //
        lueAppliesTo.Location = new Point(170, 178);
        lueAppliesTo.Name = "lueAppliesTo";
        lueAppliesTo.Properties.AutoHeight = false;
        lueAppliesTo.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAppliesTo.Properties.NullText = "";
        lueAppliesTo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueAppliesTo.Size = new Size(330, 22);
        lueAppliesTo.TabIndex = 9;
        //
        // chkIsDefault
        //
        chkIsDefault.Location = new Point(166, 206);
        chkIsDefault.Name = "chkIsDefault";
        chkIsDefault.Properties.Caption = "Lista predeterminada";
        chkIsDefault.Size = new Size(150, 20);
        chkIsDefault.TabIndex = 10;
        //
        // chkIsActive
        //
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(166, 234);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 11;
        //
        // btnCancelar
        //
        btnCancelar.Location = new Point(294, 276);
        btnCancelar.TabIndex = 12;
        //
        // btnGuardar
        //
        btnGuardar.Location = new Point(400, 276);
        btnGuardar.TabIndex = 13;
        //
        // PriceListEditForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(528, 330);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblCurrency);
        Controls.Add(lueCurrency);
        Controls.Add(lblAppliesTo);
        Controls.Add(lueAppliesTo);
        Controls.Add(chkIsDefault);
        Controls.Add(chkIsActive);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        MinimumSize = new Size(544, 369);
        Name = "PriceListEditForm";
        Text = "Lista de precios";
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAppliesTo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).EndInit();
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
}
