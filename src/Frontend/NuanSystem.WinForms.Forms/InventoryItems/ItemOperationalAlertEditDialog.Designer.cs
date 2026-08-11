using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemOperationalAlertEditDialog
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
        lblAlertType = new LabelControl();
        cboAlertType = new ComboBoxEdit();
        lblProcess = new LabelControl();
        cboProcess = new ComboBoxEdit();
        lblMessage = new LabelControl();
        memMessage = new MemoEdit();
        lblValidFrom = new LabelControl();
        dtValidFrom = new DateEdit();
        lblValidTo = new LabelControl();
        dtValidTo = new DateEdit();
        chkBlocking = new CheckEdit();
        chkActive = new CheckEdit();
        lblPriority = new LabelControl();
        cboPriority = new ComboBoxEdit();
        chkRequiresConfirmation = new CheckEdit();
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)cboAlertType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboProcess.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memMessage.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtValidFrom.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtValidFrom.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtValidTo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtValidTo.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkBlocking.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboPriority.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkRequiresConfirmation.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblAlertType
        // 
        lblAlertType.Appearance.Font = new Font("Segoe UI", 9F);
        lblAlertType.Appearance.Options.UseFont = true;
        lblAlertType.Location = new Point(18, 17);
        lblAlertType.Name = "lblAlertType";
        lblAlertType.Size = new Size(59, 15);
        lblAlertType.TabIndex = 0;
        lblAlertType.Text = "Tipo alerta:";
        // 
        // cboAlertType
        // 
        cboAlertType.Location = new Point(126, 14);
        cboAlertType.Name = "cboAlertType";
        cboAlertType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboAlertType.Properties.Appearance.Options.UseFont = true;
        cboAlertType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cboAlertType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cboAlertType.Size = new Size(160, 22);
        cboAlertType.TabIndex = 1;
        // 
        // lblProcess
        // 
        lblProcess.Appearance.Font = new Font("Segoe UI", 9F);
        lblProcess.Appearance.Options.UseFont = true;
        lblProcess.Location = new Point(306, 17);
        lblProcess.Name = "lblProcess";
        lblProcess.Size = new Size(45, 15);
        lblProcess.TabIndex = 2;
        lblProcess.Text = "Proceso:";
        // 
        // cboProcess
        // 
        cboProcess.Location = new Point(366, 14);
        cboProcess.Name = "cboProcess";
        cboProcess.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboProcess.Properties.Appearance.Options.UseFont = true;
        cboProcess.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cboProcess.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cboProcess.Size = new Size(178, 22);
        cboProcess.TabIndex = 3;
        // 
        // lblMessage
        // 
        lblMessage.Appearance.Font = new Font("Segoe UI", 9F);
        lblMessage.Appearance.Options.UseFont = true;
        lblMessage.Location = new Point(18, 47);
        lblMessage.Name = "lblMessage";
        lblMessage.Size = new Size(47, 15);
        lblMessage.TabIndex = 4;
        lblMessage.Text = "Mensaje:";
        // 
        // memMessage
        // 
        memMessage.Location = new Point(126, 42);
        memMessage.Name = "memMessage";
        memMessage.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memMessage.Properties.Appearance.Options.UseFont = true;
        memMessage.Size = new Size(418, 72);
        memMessage.TabIndex = 5;
        // 
        // lblValidFrom
        // 
        lblValidFrom.Appearance.Font = new Font("Segoe UI", 9F);
        lblValidFrom.Appearance.Options.UseFont = true;
        lblValidFrom.Location = new Point(18, 123);
        lblValidFrom.Name = "lblValidFrom";
        lblValidFrom.Size = new Size(35, 15);
        lblValidFrom.TabIndex = 6;
        lblValidFrom.Text = "Desde:";
        // 
        // dtValidFrom
        // 
        dtValidFrom.EditValue = null;
        dtValidFrom.Location = new Point(126, 120);
        dtValidFrom.Name = "dtValidFrom";
        dtValidFrom.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtValidFrom.Properties.Appearance.Options.UseFont = true;
        dtValidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dtValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dtValidFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
        dtValidFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dtValidFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
        dtValidFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dtValidFrom.Properties.MaskSettings.Set("mask", "d");
        dtValidFrom.Size = new Size(160, 22);
        dtValidFrom.TabIndex = 7;
        // 
        // lblValidTo
        // 
        lblValidTo.Appearance.Font = new Font("Segoe UI", 9F);
        lblValidTo.Appearance.Options.UseFont = true;
        lblValidTo.Location = new Point(306, 123);
        lblValidTo.Name = "lblValidTo";
        lblValidTo.Size = new Size(33, 15);
        lblValidTo.TabIndex = 8;
        lblValidTo.Text = "Hasta:";
        // 
        // dtValidTo
        // 
        dtValidTo.EditValue = null;
        dtValidTo.Location = new Point(366, 120);
        dtValidTo.Name = "dtValidTo";
        dtValidTo.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtValidTo.Properties.Appearance.Options.UseFont = true;
        dtValidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dtValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dtValidTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
        dtValidTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dtValidTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
        dtValidTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dtValidTo.Properties.MaskSettings.Set("mask", "d");
        dtValidTo.Size = new Size(178, 22);
        dtValidTo.TabIndex = 9;
        // 
        // chkBlocking
        // 
        chkBlocking.Location = new Point(126, 148);
        chkBlocking.Name = "chkBlocking";
        chkBlocking.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkBlocking.Properties.Appearance.Options.UseFont = true;
        chkBlocking.Properties.Caption = "Bloqueante";
        chkBlocking.Size = new Size(96, 20);
        chkBlocking.TabIndex = 10;
        // 
        // chkActive
        // 
        chkActive.EditValue = true;
        chkActive.Location = new Point(240, 148);
        chkActive.Name = "chkActive";
        chkActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkActive.Properties.Appearance.Options.UseFont = true;
        chkActive.Properties.Caption = "Activa";
        chkActive.Size = new Size(74, 20);
        chkActive.TabIndex = 11;
        // 
        // lblPriority
        // 
        lblPriority.Appearance.Font = new Font("Segoe UI", 9F);
        lblPriority.Appearance.Options.UseFont = true;
        lblPriority.Location = new Point(322, 151);
        lblPriority.Name = "lblPriority";
        lblPriority.Size = new Size(51, 15);
        lblPriority.TabIndex = 12;
        lblPriority.Text = "Prioridad:";
        // 
        // cboPriority
        // 
        cboPriority.Location = new Point(384, 148);
        cboPriority.Name = "cboPriority";
        cboPriority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cboPriority.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cboPriority.Size = new Size(160, 22);
        cboPriority.TabIndex = 13;
        // 
        // chkRequiresConfirmation
        // 
        chkRequiresConfirmation.Location = new Point(126, 176);
        chkRequiresConfirmation.Name = "chkRequiresConfirmation";
        chkRequiresConfirmation.Properties.Caption = "Requiere confirmación";
        chkRequiresConfirmation.Size = new Size(150, 20);
        chkRequiresConfirmation.TabIndex = 14;
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
        btnCancel.Location = new Point(338, 205);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 12;
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
        btnSave.Location = new Point(444, 205);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 13;
        btnSave.Text = "Guardar";
        // 
        // ItemOperationalAlertEditDialog
        // 
        AcceptButton = btnSave;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(568, 251);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkActive);
        Controls.Add(chkBlocking);
        Controls.Add(lblPriority);
        Controls.Add(cboPriority);
        Controls.Add(chkRequiresConfirmation);
        Controls.Add(dtValidTo);
        Controls.Add(lblValidTo);
        Controls.Add(dtValidFrom);
        Controls.Add(lblValidFrom);
        Controls.Add(memMessage);
        Controls.Add(lblMessage);
        Controls.Add(cboProcess);
        Controls.Add(lblProcess);
        Controls.Add(cboAlertType);
        Controls.Add(lblAlertType);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ItemOperationalAlertEditDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Alerta operativa";
        ((System.ComponentModel.ISupportInitialize)cboAlertType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboProcess.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memMessage.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtValidFrom.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtValidFrom.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtValidTo.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtValidTo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkBlocking.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboPriority.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkRequiresConfirmation.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private LabelControl lblAlertType;
    private ComboBoxEdit cboAlertType;
    private LabelControl lblProcess;
    private ComboBoxEdit cboProcess;
    private LabelControl lblMessage;
    private MemoEdit memMessage;
    private LabelControl lblValidFrom;
    private DateEdit dtValidFrom;
    private LabelControl lblValidTo;
    private DateEdit dtValidTo;
    private CheckEdit chkBlocking;
    private CheckEdit chkActive;
    private LabelControl lblPriority;
    private ComboBoxEdit cboPriority;
    private CheckEdit chkRequiresConfirmation;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;
}
