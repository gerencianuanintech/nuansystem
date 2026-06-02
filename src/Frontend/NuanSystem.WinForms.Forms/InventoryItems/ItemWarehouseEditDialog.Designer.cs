using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemWarehouseEditDialog
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
        lueWarehouse = new LookUpEdit();
        lblWarehouse = new LabelControl();
        spnMinimumStock = new SpinEdit();
        lblMinimumStock = new LabelControl();
        spnMaximumStock = new SpinEdit();
        lblMaximumStock = new LabelControl();
        spnReorderPoint = new SpinEdit();
        lblReorderPoint = new LabelControl();
        spnRequiredStock = new SpinEdit();
        lblRequiredStock = new LabelControl();
        txtDefaultLocationCode = new TextEdit();
        lblDefaultLocationCode = new LabelControl();
        spnWarehouseCost = new SpinEdit();
        lblWarehouseCost = new LabelControl();
        chkDefaultWarehouse = new CheckEdit();
        chkLocked = new CheckEdit();
        chkActive = new CheckEdit();
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)lueWarehouse.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumStock.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMaximumStock.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnReorderPoint.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnRequiredStock.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDefaultLocationCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnWarehouseCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultWarehouse.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkLocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).BeginInit();
        SuspendLayout();
        // 
        // lueWarehouse
        // 
        lueWarehouse.Location = new Point(126, 13);
        lueWarehouse.Name = "lueWarehouse";
        lueWarehouse.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueWarehouse.Properties.Appearance.Options.UseFont = true;
        lueWarehouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueWarehouse.Size = new Size(342, 22);
        lueWarehouse.TabIndex = 1;
        // 
        // lblWarehouse
        // 
        lblWarehouse.Appearance.Font = new Font("Segoe UI", 9F);
        lblWarehouse.Appearance.Options.UseFont = true;
        lblWarehouse.Location = new Point(18, 16);
        lblWarehouse.Name = "lblWarehouse";
        lblWarehouse.Size = new Size(43, 15);
        lblWarehouse.TabIndex = 0;
        lblWarehouse.Text = "Bodega:";
        // 
        // spnMinimumStock
        // 
        spnMinimumStock.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMinimumStock.Location = new Point(126, 41);
        spnMinimumStock.Name = "spnMinimumStock";
        spnMinimumStock.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumStock.Properties.Appearance.Options.UseFont = true;
        spnMinimumStock.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumStock.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnMinimumStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        spnMinimumStock.Properties.DisplayFormat.FormatString = "n2";
        spnMinimumStock.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnMinimumStock.Properties.EditFormat.FormatString = "n2";
        spnMinimumStock.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnMinimumStock.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumStock.Size = new Size(120, 22);
        spnMinimumStock.TabIndex = 3;
        // 
        // lblMinimumStock
        // 
        lblMinimumStock.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumStock.Appearance.Options.UseFont = true;
        lblMinimumStock.Location = new Point(18, 44);
        lblMinimumStock.Name = "lblMinimumStock";
        lblMinimumStock.Size = new Size(77, 15);
        lblMinimumStock.TabIndex = 2;
        lblMinimumStock.Text = "Stock minimo:";
        // 
        // spnMaximumStock
        // 
        spnMaximumStock.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMaximumStock.Location = new Point(348, 41);
        spnMaximumStock.Name = "spnMaximumStock";
        spnMaximumStock.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMaximumStock.Properties.Appearance.Options.UseFont = true;
        spnMaximumStock.Properties.Appearance.Options.UseTextOptions = true;
        spnMaximumStock.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnMaximumStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        spnMaximumStock.Properties.DisplayFormat.FormatString = "n2";
        spnMaximumStock.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnMaximumStock.Properties.EditFormat.FormatString = "n2";
        spnMaximumStock.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnMaximumStock.Properties.MaskSettings.Set("mask", "n2");
        spnMaximumStock.Size = new Size(120, 22);
        spnMaximumStock.TabIndex = 5;
        // 
        // lblMaximumStock
        // 
        lblMaximumStock.Appearance.Font = new Font("Segoe UI", 9F);
        lblMaximumStock.Appearance.Options.UseFont = true;
        lblMaximumStock.Location = new Point(264, 44);
        lblMaximumStock.Name = "lblMaximumStock";
        lblMaximumStock.Size = new Size(78, 15);
        lblMaximumStock.TabIndex = 4;
        lblMaximumStock.Text = "Stock maximo:";
        // 
        // spnReorderPoint
        // 
        spnReorderPoint.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnReorderPoint.Location = new Point(126, 69);
        spnReorderPoint.Name = "spnReorderPoint";
        spnReorderPoint.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnReorderPoint.Properties.Appearance.Options.UseFont = true;
        spnReorderPoint.Properties.Appearance.Options.UseTextOptions = true;
        spnReorderPoint.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnReorderPoint.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        spnReorderPoint.Properties.DisplayFormat.FormatString = "n2";
        spnReorderPoint.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnReorderPoint.Properties.EditFormat.FormatString = "n2";
        spnReorderPoint.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnReorderPoint.Properties.MaskSettings.Set("mask", "n2");
        spnReorderPoint.Size = new Size(120, 22);
        spnReorderPoint.TabIndex = 7;
        // 
        // lblReorderPoint
        // 
        lblReorderPoint.Appearance.Font = new Font("Segoe UI", 9F);
        lblReorderPoint.Appearance.Options.UseFont = true;
        lblReorderPoint.Location = new Point(18, 72);
        lblReorderPoint.Name = "lblReorderPoint";
        lblReorderPoint.Size = new Size(79, 15);
        lblReorderPoint.TabIndex = 6;
        lblReorderPoint.Text = "Punto reorden:";
        // 
        // spnRequiredStock
        // 
        spnRequiredStock.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnRequiredStock.Location = new Point(348, 69);
        spnRequiredStock.Name = "spnRequiredStock";
        spnRequiredStock.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnRequiredStock.Properties.Appearance.Options.UseFont = true;
        spnRequiredStock.Properties.Appearance.Options.UseTextOptions = true;
        spnRequiredStock.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnRequiredStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        spnRequiredStock.Properties.DisplayFormat.FormatString = "n2";
        spnRequiredStock.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnRequiredStock.Properties.EditFormat.FormatString = "n2";
        spnRequiredStock.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnRequiredStock.Properties.MaskSettings.Set("mask", "n2");
        spnRequiredStock.Size = new Size(120, 22);
        spnRequiredStock.TabIndex = 9;
        // 
        // lblRequiredStock
        // 
        lblRequiredStock.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiredStock.Appearance.Options.UseFont = true;
        lblRequiredStock.Location = new Point(264, 72);
        lblRequiredStock.Name = "lblRequiredStock";
        lblRequiredStock.Size = new Size(74, 15);
        lblRequiredStock.TabIndex = 8;
        lblRequiredStock.Text = "Stock optimo:";
        // 
        // txtDefaultLocationCode
        // 
        txtDefaultLocationCode.Location = new Point(126, 97);
        txtDefaultLocationCode.Name = "txtDefaultLocationCode";
        txtDefaultLocationCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDefaultLocationCode.Properties.Appearance.Options.UseFont = true;
        txtDefaultLocationCode.Size = new Size(120, 22);
        txtDefaultLocationCode.TabIndex = 11;
        // 
        // lblDefaultLocationCode
        // 
        lblDefaultLocationCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblDefaultLocationCode.Appearance.Options.UseFont = true;
        lblDefaultLocationCode.Location = new Point(18, 100);
        lblDefaultLocationCode.Name = "lblDefaultLocationCode";
        lblDefaultLocationCode.Size = new Size(79, 15);
        lblDefaultLocationCode.TabIndex = 10;
        lblDefaultLocationCode.Text = "Ubicacion def.:";
        // 
        // spnWarehouseCost
        // 
        spnWarehouseCost.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnWarehouseCost.Location = new Point(348, 97);
        spnWarehouseCost.Name = "spnWarehouseCost";
        spnWarehouseCost.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnWarehouseCost.Properties.Appearance.Options.UseFont = true;
        spnWarehouseCost.Properties.Appearance.Options.UseTextOptions = true;
        spnWarehouseCost.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnWarehouseCost.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        spnWarehouseCost.Properties.DisplayFormat.FormatString = "n2";
        spnWarehouseCost.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnWarehouseCost.Properties.EditFormat.FormatString = "n2";
        spnWarehouseCost.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnWarehouseCost.Properties.MaskSettings.Set("mask", "n2");
        spnWarehouseCost.Size = new Size(120, 22);
        spnWarehouseCost.TabIndex = 13;
        // 
        // lblWarehouseCost
        // 
        lblWarehouseCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblWarehouseCost.Appearance.Options.UseFont = true;
        lblWarehouseCost.Location = new Point(264, 100);
        lblWarehouseCost.Name = "lblWarehouseCost";
        lblWarehouseCost.Size = new Size(34, 15);
        lblWarehouseCost.TabIndex = 12;
        lblWarehouseCost.Text = "Costo:";
        // 
        // chkDefaultWarehouse
        // 
        chkDefaultWarehouse.Location = new Point(126, 125);
        chkDefaultWarehouse.Name = "chkDefaultWarehouse";
        chkDefaultWarehouse.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkDefaultWarehouse.Properties.Appearance.Options.UseFont = true;
        chkDefaultWarehouse.Properties.Caption = "Bodega principal";
        chkDefaultWarehouse.Size = new Size(130, 20);
        chkDefaultWarehouse.TabIndex = 14;
        // 
        // chkLocked
        // 
        chkLocked.Location = new Point(276, 125);
        chkLocked.Name = "chkLocked";
        chkLocked.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkLocked.Properties.Appearance.Options.UseFont = true;
        chkLocked.Properties.Caption = "Bloqueada";
        chkLocked.Size = new Size(92, 20);
        chkLocked.TabIndex = 15;
        // 
        // chkActive
        // 
        chkActive.EditValue = true;
        chkActive.Location = new Point(388, 125);
        chkActive.Name = "chkActive";
        chkActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkActive.Properties.Appearance.Options.UseFont = true;
        chkActive.Properties.Caption = "Activa";
        chkActive.Size = new Size(74, 20);
        chkActive.TabIndex = 16;
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
        btnCancel.Location = new Point(260, 163);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 18;
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
        btnSave.Location = new Point(366, 163);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 17;
        btnSave.Text = "Guardar";
        // 
        // ItemWarehouseEditDialog
        // 
        AcceptButton = btnSave;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(486, 209);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        Controls.Add(chkActive);
        Controls.Add(chkLocked);
        Controls.Add(chkDefaultWarehouse);
        Controls.Add(lblWarehouseCost);
        Controls.Add(spnWarehouseCost);
        Controls.Add(lblDefaultLocationCode);
        Controls.Add(txtDefaultLocationCode);
        Controls.Add(lblRequiredStock);
        Controls.Add(spnRequiredStock);
        Controls.Add(lblReorderPoint);
        Controls.Add(spnReorderPoint);
        Controls.Add(lblMaximumStock);
        Controls.Add(spnMaximumStock);
        Controls.Add(lblMinimumStock);
        Controls.Add(spnMinimumStock);
        Controls.Add(lblWarehouse);
        Controls.Add(lueWarehouse);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ItemWarehouseEditDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Bodega del item";
        ((System.ComponentModel.ISupportInitialize)lueWarehouse.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumStock.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMaximumStock.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnReorderPoint.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnRequiredStock.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDefaultLocationCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnWarehouseCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultWarehouse.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkLocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private LookUpEdit lueWarehouse;
    private LabelControl lblWarehouse;
    private SpinEdit spnMinimumStock;
    private LabelControl lblMinimumStock;
    private SpinEdit spnMaximumStock;
    private LabelControl lblMaximumStock;
    private SpinEdit spnReorderPoint;
    private LabelControl lblReorderPoint;
    private SpinEdit spnRequiredStock;
    private LabelControl lblRequiredStock;
    private TextEdit txtDefaultLocationCode;
    private LabelControl lblDefaultLocationCode;
    private SpinEdit spnWarehouseCost;
    private LabelControl lblWarehouseCost;
    private CheckEdit chkDefaultWarehouse;
    private CheckEdit chkLocked;
    private CheckEdit chkActive;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;
}
