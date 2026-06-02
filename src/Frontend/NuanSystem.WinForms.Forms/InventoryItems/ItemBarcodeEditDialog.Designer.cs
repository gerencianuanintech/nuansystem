using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemBarcodeEditDialog
{
    private System.ComponentModel.IContainer components = null;
    private PanelControl pnlMain;
    private LabelControl lblBarcode;
    private TextEdit txtBarcode;
    private LabelControl lblScope;
    private LookUpEdit lueScope;
    private LabelControl lblPresentation;
    private TextEdit txtPresentation;
    private LabelControl lblUnit;
    private LookUpEdit lueUnit;
    private LabelControl lblFactor;
    private SpinEdit spnFactor;
    private CheckEdit chkPrincipal;
    private CheckEdit chkActive;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.pnlMain = new DevExpress.XtraEditors.PanelControl();
        this.lblBarcode = new DevExpress.XtraEditors.LabelControl();
        this.txtBarcode = new DevExpress.XtraEditors.TextEdit();
        this.lblScope = new DevExpress.XtraEditors.LabelControl();
        this.lueScope = new DevExpress.XtraEditors.LookUpEdit();
        this.lblPresentation = new DevExpress.XtraEditors.LabelControl();
        this.txtPresentation = new DevExpress.XtraEditors.TextEdit();
        this.lblUnit = new DevExpress.XtraEditors.LabelControl();
        this.lueUnit = new DevExpress.XtraEditors.LookUpEdit();
        this.lblFactor = new DevExpress.XtraEditors.LabelControl();
        this.spnFactor = new DevExpress.XtraEditors.SpinEdit();
        this.chkPrincipal = new DevExpress.XtraEditors.CheckEdit();
        this.chkActive = new DevExpress.XtraEditors.CheckEdit();
        this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
        this.btnSave = new DevExpress.XtraEditors.SimpleButton();
        ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
        this.pnlMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lueScope.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.txtPresentation.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lueUnit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.spnFactor.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.chkPrincipal.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).BeginInit();
        this.SuspendLayout();
        // 
        // pnlMain
        // 
        this.pnlMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        this.pnlMain.Controls.Add(this.lblBarcode);
        this.pnlMain.Controls.Add(this.txtBarcode);
        this.pnlMain.Controls.Add(this.lblScope);
        this.pnlMain.Controls.Add(this.lueScope);
        this.pnlMain.Controls.Add(this.lblPresentation);
        this.pnlMain.Controls.Add(this.txtPresentation);
        this.pnlMain.Controls.Add(this.lblUnit);
        this.pnlMain.Controls.Add(this.lueUnit);
        this.pnlMain.Controls.Add(this.lblFactor);
        this.pnlMain.Controls.Add(this.spnFactor);
        this.pnlMain.Controls.Add(this.chkPrincipal);
        this.pnlMain.Controls.Add(this.chkActive);
        this.pnlMain.Location = new System.Drawing.Point(16, 16);
        this.pnlMain.Name = "pnlMain";
        this.pnlMain.Size = new System.Drawing.Size(484, 190);
        this.pnlMain.TabIndex = 0;
        // 
        // lblBarcode
        // 
        this.lblBarcode.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblBarcode.Appearance.Options.UseFont = true;
        this.lblBarcode.Location = new System.Drawing.Point(20, 22);
        this.lblBarcode.Name = "lblBarcode";
        this.lblBarcode.Size = new System.Drawing.Size(93, 15);
        this.lblBarcode.TabIndex = 0;
        this.lblBarcode.Text = "Codigo de barras:";
        // 
        // txtBarcode
        // 
        this.txtBarcode.Location = new System.Drawing.Point(156, 19);
        this.txtBarcode.Name = "txtBarcode";
        this.txtBarcode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.txtBarcode.Properties.Appearance.Options.UseFont = true;
        this.txtBarcode.Size = new System.Drawing.Size(292, 22);
        this.txtBarcode.TabIndex = 1;
        // 
        // lblScope
        // 
        this.lblScope.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblScope.Appearance.Options.UseFont = true;
        this.lblScope.Location = new System.Drawing.Point(20, 55);
        this.lblScope.Name = "lblScope";
        this.lblScope.Size = new System.Drawing.Size(42, 15);
        this.lblScope.TabIndex = 2;
        this.lblScope.Text = "Alcance:";
        // 
        // lueScope
        // 
        this.lueScope.Location = new System.Drawing.Point(156, 52);
        this.lueScope.Name = "lueScope";
        this.lueScope.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lueScope.Properties.Appearance.Options.UseFont = true;
        this.lueScope.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
        this.lueScope.Size = new System.Drawing.Size(180, 22);
        this.lueScope.TabIndex = 3;
        // 
        // lblPresentation
        // 
        this.lblPresentation.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblPresentation.Appearance.Options.UseFont = true;
        this.lblPresentation.Location = new System.Drawing.Point(20, 88);
        this.lblPresentation.Name = "lblPresentation";
        this.lblPresentation.Size = new System.Drawing.Size(70, 15);
        this.lblPresentation.TabIndex = 4;
        this.lblPresentation.Text = "Presentacion:";
        // 
        // txtPresentation
        // 
        this.txtPresentation.Location = new System.Drawing.Point(156, 85);
        this.txtPresentation.Name = "txtPresentation";
        this.txtPresentation.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.txtPresentation.Properties.Appearance.Options.UseFont = true;
        this.txtPresentation.Size = new System.Drawing.Size(292, 22);
        this.txtPresentation.TabIndex = 5;
        // 
        // lblUnit
        // 
        this.lblUnit.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblUnit.Appearance.Options.UseFont = true;
        this.lblUnit.Location = new System.Drawing.Point(20, 121);
        this.lblUnit.Name = "lblUnit";
        this.lblUnit.Size = new System.Drawing.Size(41, 15);
        this.lblUnit.TabIndex = 6;
        this.lblUnit.Text = "Unidad:";
        // 
        // lueUnit
        // 
        this.lueUnit.Location = new System.Drawing.Point(156, 118);
        this.lueUnit.Name = "lueUnit";
        this.lueUnit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lueUnit.Properties.Appearance.Options.UseFont = true;
        this.lueUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
        this.lueUnit.Size = new System.Drawing.Size(292, 22);
        this.lueUnit.TabIndex = 7;
        // 
        // lblFactor
        // 
        this.lblFactor.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblFactor.Appearance.Options.UseFont = true;
        this.lblFactor.Location = new System.Drawing.Point(20, 154);
        this.lblFactor.Name = "lblFactor";
        this.lblFactor.Size = new System.Drawing.Size(96, 15);
        this.lblFactor.TabIndex = 8;
        this.lblFactor.Text = "Factor inventario:";
        // 
        // spnFactor
        // 
        this.spnFactor.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
        this.spnFactor.Location = new System.Drawing.Point(156, 151);
        this.spnFactor.Name = "spnFactor";
        this.spnFactor.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.spnFactor.Properties.Appearance.Options.UseFont = true;
        this.spnFactor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
        this.spnFactor.Properties.DisplayFormat.FormatString = "n2";
        this.spnFactor.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        this.spnFactor.Properties.EditFormat.FormatString = "n2";
        this.spnFactor.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        this.spnFactor.Properties.MaskSettings.Set("mask", "n2");
        this.spnFactor.Size = new System.Drawing.Size(100, 22);
        this.spnFactor.TabIndex = 9;
        // 
        // chkPrincipal
        // 
        this.chkPrincipal.Location = new System.Drawing.Point(290, 151);
        this.chkPrincipal.Name = "chkPrincipal";
        this.chkPrincipal.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkPrincipal.Properties.Appearance.Options.UseFont = true;
        this.chkPrincipal.Properties.Caption = "Principal";
        this.chkPrincipal.Size = new System.Drawing.Size(76, 22);
        this.chkPrincipal.TabIndex = 10;
        // 
        // chkActive
        // 
        this.chkActive.EditValue = true;
        this.chkActive.Location = new System.Drawing.Point(372, 151);
        this.chkActive.Name = "chkActive";
        this.chkActive.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkActive.Properties.Appearance.Options.UseFont = true;
        this.chkActive.Properties.Caption = "Activo";
        this.chkActive.Size = new System.Drawing.Size(76, 22);
        this.chkActive.TabIndex = 11;
        // 
        // btnCancel
        // 
        this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(110)))), ((int)(((byte)(114)))));
        this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.btnCancel.Appearance.ForeColor = System.Drawing.Color.White;
        this.btnCancel.Appearance.Options.UseBackColor = true;
        this.btnCancel.Appearance.Options.UseFont = true;
        this.btnCancel.Appearance.Options.UseForeColor = true;
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnCancel.Location = new System.Drawing.Point(294, 218);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(100, 36);
        this.btnCancel.TabIndex = 1;
        this.btnCancel.Text = "Cancelar";
        // 
        // btnSave
        // 
        this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(184)))), ((int)(((byte)(148)))));
        this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
        this.btnSave.Appearance.ForeColor = System.Drawing.Color.White;
        this.btnSave.Appearance.Options.UseBackColor = true;
        this.btnSave.Appearance.Options.UseFont = true;
        this.btnSave.Appearance.Options.UseForeColor = true;
        this.btnSave.Location = new System.Drawing.Point(400, 218);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(100, 36);
        this.btnSave.TabIndex = 2;
        this.btnSave.Text = "Guardar";
        // 
        // ItemBarcodeEditDialog
        // 
        this.AcceptButton = this.btnSave;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(516, 270);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.pnlMain);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "ItemBarcodeEditDialog";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Codigo de barras por presentacion";
        ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
        this.pnlMain.ResumeLayout(false);
        this.pnlMain.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lueScope.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.txtPresentation.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lueUnit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.spnFactor.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.chkPrincipal.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).EndInit();
        this.ResumeLayout(false);
    }
}
