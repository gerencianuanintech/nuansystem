using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemPresentationEditDialog
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        chkActive = new CheckEdit();
        chkPrincipal = new CheckEdit();
        tglAppliesInventory = new ToggleSwitch();
        lblAppliesInventory = new LabelControl();
        tglAppliesSale = new ToggleSwitch();
        lblAppliesSale = new LabelControl();
        tglAppliesPurchase = new ToggleSwitch();
        lblAppliesPurchase = new LabelControl();
        txtBarcode = new TextEdit();
        lblBarcode = new LabelControl();
        spnFactor = new SpinEdit();
        lblFactor = new LabelControl();
        lueUnit = new LookUpEdit();
        lblUnit = new LabelControl();
        txtPresentation = new TextEdit();
        lblPresentation = new LabelControl();
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkPrincipal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAppliesInventory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAppliesSale.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAppliesPurchase.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBarcode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnFactor.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPresentation.Properties).BeginInit();
        SuspendLayout();
        // 
        // chkActive
        // 
        chkActive.EditValue = true;
        chkActive.Location = new Point(360, 152);
        chkActive.Name = "chkActive";
        chkActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkActive.Properties.Appearance.Options.UseFont = true;
        chkActive.Properties.Caption = "Activa";
        chkActive.Size = new Size(74, 20);
        chkActive.TabIndex = 33;
        // 
        // chkPrincipal
        // 
        chkPrincipal.Location = new Point(234, 152);
        chkPrincipal.Name = "chkPrincipal";
        chkPrincipal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkPrincipal.Properties.Appearance.Options.UseFont = true;
        chkPrincipal.Properties.Caption = "Principal";
        chkPrincipal.Size = new Size(84, 20);
        chkPrincipal.TabIndex = 32;
        // 
        // tglAppliesInventory
        // 
        tglAppliesInventory.EditValue = true;
        tglAppliesInventory.Location = new Point(120, 153);
        tglAppliesInventory.Name = "tglAppliesInventory";
        tglAppliesInventory.Properties.OffText = "No";
        tglAppliesInventory.Properties.OnText = "Si";
        tglAppliesInventory.Size = new Size(90, 18);
        tglAppliesInventory.TabIndex = 31;
        // 
        // lblAppliesInventory
        // 
        lblAppliesInventory.Appearance.Font = new Font("Segoe UI", 9F);
        lblAppliesInventory.Appearance.Options.UseFont = true;
        lblAppliesInventory.Location = new Point(18, 154);
        lblAppliesInventory.Name = "lblAppliesInventory";
        lblAppliesInventory.Size = new Size(92, 15);
        lblAppliesInventory.TabIndex = 30;
        lblAppliesInventory.Text = "Aplica inventario:";
        // 
        // tglAppliesSale
        // 
        tglAppliesSale.EditValue = true;
        tglAppliesSale.Location = new Point(360, 125);
        tglAppliesSale.Name = "tglAppliesSale";
        tglAppliesSale.Properties.OffText = "No";
        tglAppliesSale.Properties.OnText = "Si";
        tglAppliesSale.Size = new Size(90, 18);
        tglAppliesSale.TabIndex = 29;
        // 
        // lblAppliesSale
        // 
        lblAppliesSale.Appearance.Font = new Font("Segoe UI", 9F);
        lblAppliesSale.Appearance.Options.UseFont = true;
        lblAppliesSale.Location = new Point(234, 126);
        lblAppliesSale.Name = "lblAppliesSale";
        lblAppliesSale.Size = new Size(68, 15);
        lblAppliesSale.TabIndex = 28;
        lblAppliesSale.Text = "Aplica venta:";
        // 
        // tglAppliesPurchase
        // 
        tglAppliesPurchase.EditValue = true;
        tglAppliesPurchase.Location = new Point(120, 125);
        tglAppliesPurchase.Name = "tglAppliesPurchase";
        tglAppliesPurchase.Properties.OffText = "No";
        tglAppliesPurchase.Properties.OnText = "Si";
        tglAppliesPurchase.Size = new Size(90, 18);
        tglAppliesPurchase.TabIndex = 27;
        // 
        // lblAppliesPurchase
        // 
        lblAppliesPurchase.Appearance.Font = new Font("Segoe UI", 9F);
        lblAppliesPurchase.Appearance.Options.UseFont = true;
        lblAppliesPurchase.Location = new Point(18, 126);
        lblAppliesPurchase.Name = "lblAppliesPurchase";
        lblAppliesPurchase.Size = new Size(80, 15);
        lblAppliesPurchase.TabIndex = 26;
        lblAppliesPurchase.Text = "Aplica compra:";
        // 
        // txtBarcode
        // 
        txtBarcode.Location = new Point(120, 97);
        txtBarcode.Name = "txtBarcode";
        txtBarcode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBarcode.Properties.Appearance.Options.UseFont = true;
        txtBarcode.Size = new Size(330, 22);
        txtBarcode.TabIndex = 25;
        // 
        // lblBarcode
        // 
        lblBarcode.Appearance.Font = new Font("Segoe UI", 9F);
        lblBarcode.Appearance.Options.UseFont = true;
        lblBarcode.Location = new Point(18, 100);
        lblBarcode.Name = "lblBarcode";
        lblBarcode.Size = new Size(88, 15);
        lblBarcode.TabIndex = 24;
        lblBarcode.Text = "Codigo de barra:";
        // 
        // spnFactor
        // 
        spnFactor.EditValue = new decimal(new int[] { 1, 0, 0, 0 });
        spnFactor.Location = new Point(120, 69);
        spnFactor.Name = "spnFactor";
        spnFactor.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnFactor.Properties.Appearance.Options.UseFont = true;
        spnFactor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        spnFactor.Properties.DisplayFormat.FormatString = "n2";
        spnFactor.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnFactor.Properties.EditFormat.FormatString = "n2";
        spnFactor.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnFactor.Properties.MaskSettings.Set("mask", "n2");
        spnFactor.Size = new Size(130, 22);
        spnFactor.TabIndex = 23;
        // 
        // lblFactor
        // 
        lblFactor.Appearance.Font = new Font("Segoe UI", 9F);
        lblFactor.Appearance.Options.UseFont = true;
        lblFactor.Location = new Point(18, 72);
        lblFactor.Name = "lblFactor";
        lblFactor.Size = new Size(92, 15);
        lblFactor.TabIndex = 22;
        lblFactor.Text = "Factor inventario:";
        // 
        // lueUnit
        // 
        lueUnit.Location = new Point(120, 41);
        lueUnit.Name = "lueUnit";
        lueUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueUnit.Properties.Appearance.Options.UseFont = true;
        lueUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueUnit.Size = new Size(330, 22);
        lueUnit.TabIndex = 21;
        // 
        // lblUnit
        // 
        lblUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblUnit.Appearance.Options.UseFont = true;
        lblUnit.Location = new Point(18, 44);
        lblUnit.Name = "lblUnit";
        lblUnit.Size = new Size(41, 15);
        lblUnit.TabIndex = 20;
        lblUnit.Text = "Unidad:";
        // 
        // txtPresentation
        // 
        txtPresentation.Location = new Point(120, 13);
        txtPresentation.Name = "txtPresentation";
        txtPresentation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPresentation.Properties.Appearance.Options.UseFont = true;
        txtPresentation.Size = new Size(330, 22);
        txtPresentation.TabIndex = 19;
        // 
        // lblPresentation
        // 
        lblPresentation.Appearance.Font = new Font("Segoe UI", 9F);
        lblPresentation.Appearance.Options.UseFont = true;
        lblPresentation.Location = new Point(18, 16);
        lblPresentation.Name = "lblPresentation";
        lblPresentation.Size = new Size(71, 15);
        lblPresentation.TabIndex = 18;
        lblPresentation.Text = "Presentacion:";
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.White;
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(244, 212);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 34;
        btnCancel.Text = "Cancelar";
        // 
        // btnSave
        // 
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.Location = new Point(350, 212);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 35;
        btnSave.Text = "Guardar";
        // 
        // ItemPresentationEditDialog
        // 
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(475, 274);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkActive);
        Controls.Add(chkPrincipal);
        Controls.Add(tglAppliesInventory);
        Controls.Add(lblAppliesInventory);
        Controls.Add(tglAppliesSale);
        Controls.Add(lblAppliesSale);
        Controls.Add(tglAppliesPurchase);
        Controls.Add(lblAppliesPurchase);
        Controls.Add(txtBarcode);
        Controls.Add(lblBarcode);
        Controls.Add(spnFactor);
        Controls.Add(lblFactor);
        Controls.Add(lueUnit);
        Controls.Add(lblUnit);
        Controls.Add(txtPresentation);
        Controls.Add(lblPresentation);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ItemPresentationEditDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Presentacion";
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkPrincipal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAppliesInventory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAppliesSale.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAppliesPurchase.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBarcode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnFactor.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPresentation.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    private CheckEdit chkActive;
    private CheckEdit chkPrincipal;
    private ToggleSwitch tglAppliesInventory;
    private LabelControl lblAppliesInventory;
    private ToggleSwitch tglAppliesSale;
    private LabelControl lblAppliesSale;
    private ToggleSwitch tglAppliesPurchase;
    private LabelControl lblAppliesPurchase;
    private TextEdit txtBarcode;
    private LabelControl lblBarcode;
    private SpinEdit spnFactor;
    private LabelControl lblFactor;
    private LookUpEdit lueUnit;
    private LabelControl lblUnit;
    private TextEdit txtPresentation;
    private LabelControl lblPresentation;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;
}
