using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.Documents.SecurityDocumentSeries;

partial class SecurityDocumentSeriesEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDocumentType = new LabelControl();
        lueDocumentType = new LookUpEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblPrefix = new LabelControl();
        txtPrefix = new TextEdit();
        lblEstablishment = new LabelControl();
        lueEstablishment = new LookUpEdit();
        lblEmissionPoint = new LabelControl();
        lueEmissionPoint = new LookUpEdit();
        lblInitialNumber = new LabelControl();
        sedInitialNumber = new SpinEdit();
        lblCurrentNumber = new LabelControl();
        sedCurrentNumber = new SpinEdit();
        lblNextNumber = new LabelControl();
        sedNextNumber = new SpinEdit();
        lblNumberLength = new LabelControl();
        sedNumberLength = new SpinEdit();
        chkIsDefault = new CheckEdit();
        chkIsActive = new CheckEdit();
        chkIsSapIntegrationActive = new CheckEdit();
        lblSapObjectType = new LabelControl();
        lueSapObjectType = new LookUpEdit();
        lblSapSeriesId = new LabelControl();
        sedSapSeriesId = new SpinEdit();
        lblSapSeriesName = new LabelControl();
        txtSapSeriesName = new TextEdit();
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPrefix.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueEstablishment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueEmissionPoint.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedInitialNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedCurrentNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedNextNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedNumberLength.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSapIntegrationActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapObjectType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedSapSeriesId.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSeriesName.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblCode
        // 
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = Color.Black;
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(28, 34);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(38, 15);
        lblCode.TabIndex = 0;
        lblCode.Text = "Codigo";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(145, 31);
        txtCode.Name = "txtCode";
        txtCode.Properties.MaxLength = 40;
        txtCode.Size = new Size(190, 22);
        txtCode.TabIndex = 1;
        // 
        // lblName
        // 
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = Color.Black;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(28, 60);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 2;
        lblName.Text = "Nombre";
        // 
        // txtName
        // 
        txtName.Location = new Point(145, 57);
        txtName.Name = "txtName";
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(190, 22);
        txtName.TabIndex = 3;
        // 
        // lblDocumentType
        // 
        lblDocumentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblDocumentType.Appearance.ForeColor = Color.Black;
        lblDocumentType.Appearance.Options.UseFont = true;
        lblDocumentType.Appearance.Options.UseForeColor = true;
        lblDocumentType.Location = new Point(28, 86);
        lblDocumentType.Name = "lblDocumentType";
        lblDocumentType.Size = new Size(101, 15);
        lblDocumentType.TabIndex = 4;
        lblDocumentType.Text = "Tipo de documento";
        // 
        // lueDocumentType
        // 
        lueDocumentType.Location = new Point(145, 83);
        lueDocumentType.Name = "lueDocumentType";
        lueDocumentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueDocumentType.Size = new Size(190, 22);
        lueDocumentType.TabIndex = 5;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = Color.Black;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(28, 112);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(61, 15);
        lblDescription.TabIndex = 6;
        lblDescription.Text = "Descripcion";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(145, 109);
        memDescription.Name = "memDescription";
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(190, 48);
        memDescription.TabIndex = 7;
        // 
        // lblPrefix
        // 
        lblPrefix.Appearance.Font = new Font("Segoe UI", 9F);
        lblPrefix.Appearance.ForeColor = Color.Black;
        lblPrefix.Appearance.Options.UseFont = true;
        lblPrefix.Appearance.Options.UseForeColor = true;
        lblPrefix.Location = new Point(28, 164);
        lblPrefix.Name = "lblPrefix";
        lblPrefix.Size = new Size(34, 15);
        lblPrefix.TabIndex = 8;
        lblPrefix.Text = "Prefijo";
        // 
        // txtPrefix
        // 
        txtPrefix.Location = new Point(145, 161);
        txtPrefix.Name = "txtPrefix";
        txtPrefix.Properties.MaxLength = 20;
        txtPrefix.Size = new Size(190, 22);
        txtPrefix.TabIndex = 9;
        // 
        // lblEstablishment
        // 
        lblEstablishment.Appearance.Font = new Font("Segoe UI", 9F);
        lblEstablishment.Appearance.ForeColor = Color.Black;
        lblEstablishment.Appearance.Options.UseFont = true;
        lblEstablishment.Appearance.Options.UseForeColor = true;
        lblEstablishment.Location = new Point(28, 190);
        lblEstablishment.Name = "lblEstablishment";
        lblEstablishment.Size = new Size(83, 15);
        lblEstablishment.TabIndex = 10;
        lblEstablishment.Text = "Establecimiento";
        // 
        // lueEstablishment
        // 
        lueEstablishment.Location = new Point(145, 187);
        lueEstablishment.Name = "lueEstablishment";
        lueEstablishment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueEstablishment.Size = new Size(190, 22);
        lueEstablishment.TabIndex = 11;
        // 
        // lblEmissionPoint
        // 
        lblEmissionPoint.Appearance.Font = new Font("Segoe UI", 9F);
        lblEmissionPoint.Appearance.ForeColor = Color.Black;
        lblEmissionPoint.Appearance.Options.UseFont = true;
        lblEmissionPoint.Appearance.Options.UseForeColor = true;
        lblEmissionPoint.Location = new Point(28, 216);
        lblEmissionPoint.Name = "lblEmissionPoint";
        lblEmissionPoint.Size = new Size(89, 15);
        lblEmissionPoint.TabIndex = 12;
        lblEmissionPoint.Text = "Punto de emision";
        // 
        // lueEmissionPoint
        // 
        lueEmissionPoint.Location = new Point(145, 213);
        lueEmissionPoint.Name = "lueEmissionPoint";
        lueEmissionPoint.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueEmissionPoint.Size = new Size(190, 22);
        lueEmissionPoint.TabIndex = 13;
        // 
        // lblInitialNumber
        // 
        lblInitialNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblInitialNumber.Appearance.ForeColor = Color.Black;
        lblInitialNumber.Appearance.Options.UseFont = true;
        lblInitialNumber.Appearance.Options.UseForeColor = true;
        lblInitialNumber.Location = new Point(375, 34);
        lblInitialNumber.Name = "lblInitialNumber";
        lblInitialNumber.Size = new Size(76, 15);
        lblInitialNumber.TabIndex = 14;
        lblInitialNumber.Text = "Numero inicial";
        // 
        // sedInitialNumber
        // 
        sedInitialNumber.EditValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedInitialNumber.Location = new Point(505, 31);
        sedInitialNumber.Name = "sedInitialNumber";
        sedInitialNumber.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedInitialNumber.Properties.Appearance.Options.UseTextOptions = true;
        sedInitialNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        sedInitialNumber.Properties.IsFloatValue = false;
        sedInitialNumber.Properties.MaskSettings.Set("mask", "N0");
        sedInitialNumber.Properties.MaxValue = new decimal(new int[] { 999999999, 0, 0, 0 });
        sedInitialNumber.Size = new Size(130, 22);
        sedInitialNumber.TabIndex = 15;
        // 
        // lblCurrentNumber
        // 
        lblCurrentNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblCurrentNumber.Appearance.ForeColor = Color.Black;
        lblCurrentNumber.Appearance.Options.UseFont = true;
        lblCurrentNumber.Appearance.Options.UseForeColor = true;
        lblCurrentNumber.Location = new Point(375, 60);
        lblCurrentNumber.Name = "lblCurrentNumber";
        lblCurrentNumber.Size = new Size(75, 15);
        lblCurrentNumber.TabIndex = 16;
        lblCurrentNumber.Text = "Numero actual";
        // 
        // sedCurrentNumber
        // 
        sedCurrentNumber.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedCurrentNumber.Location = new Point(505, 57);
        sedCurrentNumber.Name = "sedCurrentNumber";
        sedCurrentNumber.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedCurrentNumber.Properties.Appearance.Options.UseTextOptions = true;
        sedCurrentNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        sedCurrentNumber.Properties.IsFloatValue = false;
        sedCurrentNumber.Properties.MaskSettings.Set("mask", "N0");
        sedCurrentNumber.Properties.MaxValue = new decimal(new int[] { 999999999, 0, 0, 0 });
        sedCurrentNumber.Size = new Size(130, 22);
        sedCurrentNumber.TabIndex = 17;
        // 
        // lblNextNumber
        // 
        lblNextNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblNextNumber.Appearance.ForeColor = Color.Black;
        lblNextNumber.Appearance.Options.UseFont = true;
        lblNextNumber.Appearance.Options.UseForeColor = true;
        lblNextNumber.Location = new Point(375, 86);
        lblNextNumber.Name = "lblNextNumber";
        lblNextNumber.Size = new Size(88, 15);
        lblNextNumber.TabIndex = 18;
        lblNextNumber.Text = "Siguiente numero";
        // 
        // sedNextNumber
        // 
        sedNextNumber.EditValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedNextNumber.Location = new Point(505, 83);
        sedNextNumber.Name = "sedNextNumber";
        sedNextNumber.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedNextNumber.Properties.Appearance.Options.UseTextOptions = true;
        sedNextNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        sedNextNumber.Properties.IsFloatValue = false;
        sedNextNumber.Properties.MaskSettings.Set("mask", "N0");
        sedNextNumber.Properties.MaxValue = new decimal(new int[] { 999999999, 0, 0, 0 });
        sedNextNumber.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedNextNumber.Size = new Size(130, 22);
        sedNextNumber.TabIndex = 19;
        // 
        // lblNumberLength
        // 
        lblNumberLength.Appearance.Font = new Font("Segoe UI", 9F);
        lblNumberLength.Appearance.ForeColor = Color.Black;
        lblNumberLength.Appearance.Options.UseFont = true;
        lblNumberLength.Appearance.Options.UseForeColor = true;
        lblNumberLength.Location = new Point(375, 112);
        lblNumberLength.Name = "lblNumberLength";
        lblNumberLength.Size = new Size(100, 15);
        lblNumberLength.TabIndex = 20;
        lblNumberLength.Text = "Longitud del numero";
        // 
        // sedNumberLength
        // 
        sedNumberLength.EditValue = new decimal(new int[] { 8, 0, 0, 0 });
        sedNumberLength.Location = new Point(505, 109);
        sedNumberLength.Name = "sedNumberLength";
        sedNumberLength.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedNumberLength.Properties.Appearance.Options.UseTextOptions = true;
        sedNumberLength.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        sedNumberLength.Properties.IsFloatValue = false;
        sedNumberLength.Properties.MaskSettings.Set("mask", "N0");
        sedNumberLength.Properties.MaxValue = new decimal(new int[] { 18, 0, 0, 0 });
        sedNumberLength.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedNumberLength.Size = new Size(130, 22);
        sedNumberLength.TabIndex = 21;
        // 
        // chkIsDefault
        // 
        chkIsDefault.Location = new Point(372, 161);
        chkIsDefault.Name = "chkIsDefault";
        chkIsDefault.Properties.Caption = "Serie por defecto";
        chkIsDefault.Size = new Size(160, 20);
        chkIsDefault.TabIndex = 22;
        // 
        // chkIsActive
        // 
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(372, 187);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(160, 20);
        chkIsActive.TabIndex = 23;
        // 
        // chkIsSapIntegrationActive
        // 
        chkIsSapIntegrationActive.EditValue = true;
        chkIsSapIntegrationActive.Location = new Point(372, 213);
        chkIsSapIntegrationActive.Name = "chkIsSapIntegrationActive";
        chkIsSapIntegrationActive.Properties.Caption = "Activo para integracion SAP";
        chkIsSapIntegrationActive.Size = new Size(190, 20);
        chkIsSapIntegrationActive.TabIndex = 24;
        // 
        // lblSapObjectType
        // 
        lblSapObjectType.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapObjectType.Appearance.ForeColor = Color.Black;
        lblSapObjectType.Appearance.Options.UseFont = true;
        lblSapObjectType.Appearance.Options.UseForeColor = true;
        lblSapObjectType.Location = new Point(675, 34);
        lblSapObjectType.Name = "lblSapObjectType";
        lblSapObjectType.Size = new Size(56, 15);
        lblSapObjectType.TabIndex = 25;
        lblSapObjectType.Text = "Objeto SAP";
        // 
        // lueSapObjectType
        // 
        lueSapObjectType.Location = new Point(795, 31);
        lueSapObjectType.Name = "lueSapObjectType";
        lueSapObjectType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueSapObjectType.Size = new Size(170, 22);
        lueSapObjectType.TabIndex = 26;
        // 
        // lblSapSeriesId
        // 
        lblSapSeriesId.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSeriesId.Appearance.ForeColor = Color.Black;
        lblSapSeriesId.Appearance.Options.UseFont = true;
        lblSapSeriesId.Appearance.Options.UseForeColor = true;
        lblSapSeriesId.Location = new Point(675, 60);
        lblSapSeriesId.Name = "lblSapSeriesId";
        lblSapSeriesId.Size = new Size(48, 15);
        lblSapSeriesId.TabIndex = 27;
        lblSapSeriesId.Text = "Serie SAP";
        // 
        // sedSapSeriesId
        // 
        sedSapSeriesId.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedSapSeriesId.Location = new Point(795, 57);
        sedSapSeriesId.Name = "sedSapSeriesId";
        sedSapSeriesId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedSapSeriesId.Properties.Appearance.Options.UseTextOptions = true;
        sedSapSeriesId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        sedSapSeriesId.Properties.IsFloatValue = false;
        sedSapSeriesId.Properties.MaskSettings.Set("mask", "N0");
        sedSapSeriesId.Properties.MaxValue = new decimal(new int[] { 999999999, 0, 0, 0 });
        sedSapSeriesId.Size = new Size(170, 22);
        sedSapSeriesId.TabIndex = 28;
        // 
        // lblSapSeriesName
        // 
        lblSapSeriesName.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSeriesName.Appearance.ForeColor = Color.Black;
        lblSapSeriesName.Appearance.Options.UseFont = true;
        lblSapSeriesName.Appearance.Options.UseForeColor = true;
        lblSapSeriesName.Location = new Point(675, 86);
        lblSapSeriesName.Name = "lblSapSeriesName";
        lblSapSeriesName.Size = new Size(93, 15);
        lblSapSeriesName.TabIndex = 29;
        lblSapSeriesName.Text = "Nombre serie SAP";
        // 
        // txtSapSeriesName
        // 
        txtSapSeriesName.Location = new Point(795, 83);
        txtSapSeriesName.Name = "txtSapSeriesName";
        txtSapSeriesName.Properties.MaxLength = 150;
        txtSapSeriesName.Size = new Size(170, 22);
        txtSapSeriesName.TabIndex = 30;
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.ForeColor = Color.White;
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancel.AppearanceHovered.ForeColor = Color.White;
        btnCancel.AppearanceHovered.Options.UseBackColor = true;
        btnCancel.AppearanceHovered.Options.UseForeColor = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(755, 272);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 31;
        btnCancel.Text = "Cancelar";
        // 
        // btnSave
        // 
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnSave.AppearanceHovered.ForeColor = Color.White;
        btnSave.AppearanceHovered.Options.UseBackColor = true;
        btnSave.AppearanceHovered.Options.UseForeColor = true;
        btnSave.Location = new Point(870, 272);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 32;
        btnSave.Text = "Guardar";
        // 
        // SecurityDocumentSeriesEditForm
        // 
        AcceptButton = btnSave;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(1000, 330);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblDocumentType);
        Controls.Add(lueDocumentType);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblPrefix);
        Controls.Add(txtPrefix);
        Controls.Add(lblEstablishment);
        Controls.Add(lueEstablishment);
        Controls.Add(lblEmissionPoint);
        Controls.Add(lueEmissionPoint);
        Controls.Add(lblInitialNumber);
        Controls.Add(sedInitialNumber);
        Controls.Add(lblCurrentNumber);
        Controls.Add(sedCurrentNumber);
        Controls.Add(lblNextNumber);
        Controls.Add(sedNextNumber);
        Controls.Add(lblNumberLength);
        Controls.Add(sedNumberLength);
        Controls.Add(chkIsDefault);
        Controls.Add(chkIsActive);
        Controls.Add(chkIsSapIntegrationActive);
        Controls.Add(lblSapObjectType);
        Controls.Add(lueSapObjectType);
        Controls.Add(lblSapSeriesId);
        Controls.Add(sedSapSeriesId);
        Controls.Add(lblSapSeriesName);
        Controls.Add(txtSapSeriesName);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SecurityDocumentSeriesEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nueva serie de documento";
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPrefix.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueEstablishment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueEmissionPoint.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedInitialNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedCurrentNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedNextNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedNumberLength.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSapIntegrationActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapObjectType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedSapSeriesId.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSeriesName.Properties).EndInit();
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

    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDocumentType;
    private LookUpEdit lueDocumentType;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblPrefix;
    private TextEdit txtPrefix;
    private LabelControl lblEstablishment;
    private LookUpEdit lueEstablishment;
    private LabelControl lblEmissionPoint;
    private LookUpEdit lueEmissionPoint;
    private LabelControl lblInitialNumber;
    private SpinEdit sedInitialNumber;
    private LabelControl lblCurrentNumber;
    private SpinEdit sedCurrentNumber;
    private LabelControl lblNextNumber;
    private SpinEdit sedNextNumber;
    private LabelControl lblNumberLength;
    private SpinEdit sedNumberLength;
    private CheckEdit chkIsDefault;
    private CheckEdit chkIsActive;
    private CheckEdit chkIsSapIntegrationActive;
    private LabelControl lblSapObjectType;
    private LookUpEdit lueSapObjectType;
    private LabelControl lblSapSeriesId;
    private SpinEdit sedSapSeriesId;
    private LabelControl lblSapSeriesName;
    private TextEdit txtSapSeriesName;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;
}
