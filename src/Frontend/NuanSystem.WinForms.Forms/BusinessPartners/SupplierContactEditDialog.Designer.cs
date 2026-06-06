using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

partial class SupplierContactEditDialog
{
    private System.ComponentModel.IContainer components = null;

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
        pnlMain = new PanelControl();
        lblContactTreatment = new LabelControl();
        lueContactTreatment = new LookUpEdit();
        lblContactFirstName = new LabelControl();
        txtContactFirstName = new TextEdit();
        lblContactLastName = new LabelControl();
        txtContactLastName = new TextEdit();
        lblContactPosition = new LabelControl();
        txtContactPosition = new TextEdit();
        lblContactDepartment = new LabelControl();
        txtContactDepartment = new TextEdit();
        lblContactPhone = new LabelControl();
        txtContactPhone = new TextEdit();
        lblContactExtension = new LabelControl();
        txtContactExtension = new TextEdit();
        lblContactMobile = new LabelControl();
        txtContactMobile = new TextEdit();
        lblContactEmail = new LabelControl();
        txtContactEmail = new TextEdit();
        lblContactBirthday = new LabelControl();
        dteContactBirthday = new DateEdit();
        lblContactPrimary = new LabelControl();
        tglContactPrimary = new ToggleSwitch();
        lblContactPrimaryValue = new LabelControl();
        lblContactActive = new LabelControl();
        tglContactActive = new ToggleSwitch();
        lblContactActiveValue = new LabelControl();
        lblContactNotes = new LabelControl();
        memContactNotes = new MemoEdit();
        pnlFooter = new PanelControl();
        btnSaveContact = new SimpleButton();
        btnCancelContact = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
        pnlMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueContactTreatment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactFirstName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactLastName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPosition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactDepartment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactExtension.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactMobile.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteContactBirthday.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteContactBirthday.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglContactPrimary.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglContactActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memContactNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlFooter).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();
        // 
        // pnlMain
        // 
        pnlMain.BorderStyle = BorderStyles.NoBorder;
        pnlMain.Controls.Add(lblContactTreatment);
        pnlMain.Controls.Add(lueContactTreatment);
        pnlMain.Controls.Add(lblContactFirstName);
        pnlMain.Controls.Add(txtContactFirstName);
        pnlMain.Controls.Add(lblContactLastName);
        pnlMain.Controls.Add(txtContactLastName);
        pnlMain.Controls.Add(lblContactPosition);
        pnlMain.Controls.Add(txtContactPosition);
        pnlMain.Controls.Add(lblContactDepartment);
        pnlMain.Controls.Add(txtContactDepartment);
        pnlMain.Controls.Add(lblContactPhone);
        pnlMain.Controls.Add(txtContactPhone);
        pnlMain.Controls.Add(lblContactExtension);
        pnlMain.Controls.Add(txtContactExtension);
        pnlMain.Controls.Add(lblContactMobile);
        pnlMain.Controls.Add(txtContactMobile);
        pnlMain.Controls.Add(lblContactEmail);
        pnlMain.Controls.Add(txtContactEmail);
        pnlMain.Controls.Add(lblContactBirthday);
        pnlMain.Controls.Add(dteContactBirthday);
        pnlMain.Controls.Add(lblContactPrimary);
        pnlMain.Controls.Add(tglContactPrimary);
        pnlMain.Controls.Add(lblContactPrimaryValue);
        pnlMain.Controls.Add(lblContactActive);
        pnlMain.Controls.Add(tglContactActive);
        pnlMain.Controls.Add(lblContactActiveValue);
        pnlMain.Controls.Add(lblContactNotes);
        pnlMain.Controls.Add(memContactNotes);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(0, 0);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(458, 378);
        pnlMain.TabIndex = 0;
        // 
        // lblContactTreatment
        // 
        lblContactTreatment.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactTreatment.Appearance.Options.UseFont = true;
        lblContactTreatment.Location = new Point(24, 26);
        lblContactTreatment.Name = "lblContactTreatment";
        lblContactTreatment.Size = new Size(67, 15);
        lblContactTreatment.TabIndex = 0;
        lblContactTreatment.Text = "Tratamiento:";
        // 
        // lueContactTreatment
        // 
        lueContactTreatment.Location = new Point(142, 23);
        lueContactTreatment.Name = "lueContactTreatment";
        lueContactTreatment.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lueContactTreatment.Properties.Appearance.Options.UseFont = true;
        lueContactTreatment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueContactTreatment.Properties.NullText = string.Empty;
        lueContactTreatment.Size = new Size(230, 22);
        lueContactTreatment.TabIndex = 1;
        // 
        // lblContactFirstName
        // 
        lblContactFirstName.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactFirstName.Appearance.Options.UseFont = true;
        lblContactFirstName.Location = new Point(24, 58);
        lblContactFirstName.Name = "lblContactFirstName";
        lblContactFirstName.Size = new Size(51, 15);
        lblContactFirstName.TabIndex = 2;
        lblContactFirstName.Text = "Nombres:";
        // 
        // txtContactFirstName
        // 
        txtContactFirstName.Location = new Point(142, 55);
        txtContactFirstName.Name = "txtContactFirstName";
        txtContactFirstName.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactFirstName.Properties.Appearance.Options.UseFont = true;
        txtContactFirstName.Size = new Size(280, 22);
        txtContactFirstName.TabIndex = 3;
        // 
        // lblContactLastName
        // 
        lblContactLastName.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactLastName.Appearance.Options.UseFont = true;
        lblContactLastName.Location = new Point(24, 90);
        lblContactLastName.Name = "lblContactLastName";
        lblContactLastName.Size = new Size(49, 15);
        lblContactLastName.TabIndex = 4;
        lblContactLastName.Text = "Apellidos:";
        // 
        // txtContactLastName
        // 
        txtContactLastName.Location = new Point(142, 87);
        txtContactLastName.Name = "txtContactLastName";
        txtContactLastName.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactLastName.Properties.Appearance.Options.UseFont = true;
        txtContactLastName.Size = new Size(280, 22);
        txtContactLastName.TabIndex = 5;
        // 
        // lblContactPosition
        // 
        lblContactPosition.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactPosition.Appearance.Options.UseFont = true;
        lblContactPosition.Location = new Point(24, 122);
        lblContactPosition.Name = "lblContactPosition";
        lblContactPosition.Size = new Size(35, 15);
        lblContactPosition.TabIndex = 6;
        lblContactPosition.Text = "Cargo:";
        // 
        // txtContactPosition
        // 
        txtContactPosition.Location = new Point(142, 119);
        txtContactPosition.Name = "txtContactPosition";
        txtContactPosition.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactPosition.Properties.Appearance.Options.UseFont = true;
        txtContactPosition.Size = new Size(280, 22);
        txtContactPosition.TabIndex = 7;
        // 
        // lblContactDepartment
        // 
        lblContactDepartment.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactDepartment.Appearance.Options.UseFont = true;
        lblContactDepartment.Location = new Point(24, 154);
        lblContactDepartment.Name = "lblContactDepartment";
        lblContactDepartment.Size = new Size(111, 15);
        lblContactDepartment.TabIndex = 8;
        lblContactDepartment.Text = "Departamento / Área:";
        // 
        // txtContactDepartment
        // 
        txtContactDepartment.Location = new Point(142, 151);
        txtContactDepartment.Name = "txtContactDepartment";
        txtContactDepartment.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactDepartment.Properties.Appearance.Options.UseFont = true;
        txtContactDepartment.Size = new Size(280, 22);
        txtContactDepartment.TabIndex = 9;
        // 
        // lblContactPhone
        // 
        lblContactPhone.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactPhone.Appearance.Options.UseFont = true;
        lblContactPhone.Location = new Point(24, 186);
        lblContactPhone.Name = "lblContactPhone";
        lblContactPhone.Size = new Size(48, 15);
        lblContactPhone.TabIndex = 10;
        lblContactPhone.Text = "Teléfono:";
        // 
        // txtContactPhone
        // 
        txtContactPhone.Location = new Point(142, 183);
        txtContactPhone.Name = "txtContactPhone";
        txtContactPhone.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactPhone.Properties.Appearance.Options.UseFont = true;
        txtContactPhone.Size = new Size(136, 22);
        txtContactPhone.TabIndex = 11;
        // 
        // lblContactExtension
        // 
        lblContactExtension.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactExtension.Appearance.Options.UseFont = true;
        lblContactExtension.Location = new Point(296, 186);
        lblContactExtension.Name = "lblContactExtension";
        lblContactExtension.Size = new Size(64, 15);
        lblContactExtension.TabIndex = 12;
        lblContactExtension.Text = "Anexo / Ext.:";
        // 
        // txtContactExtension
        // 
        txtContactExtension.Location = new Point(366, 183);
        txtContactExtension.Name = "txtContactExtension";
        txtContactExtension.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactExtension.Properties.Appearance.Options.UseFont = true;
        txtContactExtension.Size = new Size(56, 22);
        txtContactExtension.TabIndex = 13;
        // 
        // lblContactMobile
        // 
        lblContactMobile.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactMobile.Appearance.Options.UseFont = true;
        lblContactMobile.Location = new Point(24, 218);
        lblContactMobile.Name = "lblContactMobile";
        lblContactMobile.Size = new Size(37, 15);
        lblContactMobile.TabIndex = 14;
        lblContactMobile.Text = "Celular:";
        // 
        // txtContactMobile
        // 
        txtContactMobile.Location = new Point(142, 215);
        txtContactMobile.Name = "txtContactMobile";
        txtContactMobile.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactMobile.Properties.Appearance.Options.UseFont = true;
        txtContactMobile.Size = new Size(136, 22);
        txtContactMobile.TabIndex = 15;
        // 
        // lblContactEmail
        // 
        lblContactEmail.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactEmail.Appearance.Options.UseFont = true;
        lblContactEmail.Location = new Point(24, 250);
        lblContactEmail.Name = "lblContactEmail";
        lblContactEmail.Size = new Size(103, 15);
        lblContactEmail.TabIndex = 16;
        lblContactEmail.Text = "Correo Electrónico:";
        // 
        // txtContactEmail
        // 
        txtContactEmail.Location = new Point(142, 247);
        txtContactEmail.Name = "txtContactEmail";
        txtContactEmail.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtContactEmail.Properties.Appearance.Options.UseFont = true;
        txtContactEmail.Size = new Size(280, 22);
        txtContactEmail.TabIndex = 17;
        // 
        // lblContactBirthday
        // 
        lblContactBirthday.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactBirthday.Appearance.Options.UseFont = true;
        lblContactBirthday.Location = new Point(24, 282);
        lblContactBirthday.Name = "lblContactBirthday";
        lblContactBirthday.Size = new Size(64, 15);
        lblContactBirthday.TabIndex = 18;
        lblContactBirthday.Text = "Cumpleaños:";
        // 
        // dteContactBirthday
        // 
        dteContactBirthday.EditValue = null;
        dteContactBirthday.Location = new Point(142, 279);
        dteContactBirthday.Name = "dteContactBirthday";
        dteContactBirthday.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        dteContactBirthday.Properties.Appearance.Options.UseFont = true;
        dteContactBirthday.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteContactBirthday.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteContactBirthday.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
        dteContactBirthday.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dteContactBirthday.Properties.EditFormat.FormatString = "dd/MM/yyyy";
        dteContactBirthday.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dteContactBirthday.Properties.MaskSettings.Set("mask", "dd/MM/yyyy");
        dteContactBirthday.Size = new Size(136, 22);
        dteContactBirthday.TabIndex = 19;
        // 
        // lblContactPrimary
        // 
        lblContactPrimary.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactPrimary.Appearance.Options.UseFont = true;
        lblContactPrimary.Location = new Point(296, 282);
        lblContactPrimary.Name = "lblContactPrimary";
        lblContactPrimary.Size = new Size(63, 15);
        lblContactPrimary.TabIndex = 20;
        lblContactPrimary.Text = "Es Principal:";
        // 
        // tglContactPrimary
        // 
        tglContactPrimary.Location = new Point(366, 277);
        tglContactPrimary.Name = "tglContactPrimary";
        tglContactPrimary.Properties.OffText = string.Empty;
        tglContactPrimary.Properties.OnText = string.Empty;
        tglContactPrimary.Size = new Size(48, 24);
        tglContactPrimary.TabIndex = 21;
        // 
        // lblContactPrimaryValue
        // 
        lblContactPrimaryValue.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactPrimaryValue.Appearance.Options.UseFont = true;
        lblContactPrimaryValue.Location = new Point(418, 282);
        lblContactPrimaryValue.Name = "lblContactPrimaryValue";
        lblContactPrimaryValue.Size = new Size(15, 15);
        lblContactPrimaryValue.TabIndex = 22;
        lblContactPrimaryValue.Text = "No";
        // 
        // lblContactActive
        // 
        lblContactActive.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactActive.Appearance.Options.UseFont = true;
        lblContactActive.Location = new Point(24, 314);
        lblContactActive.Name = "lblContactActive";
        lblContactActive.Size = new Size(36, 15);
        lblContactActive.TabIndex = 23;
        lblContactActive.Text = "Activo:";
        // 
        // tglContactActive
        // 
        tglContactActive.Location = new Point(142, 309);
        tglContactActive.Name = "tglContactActive";
        tglContactActive.Properties.OffText = string.Empty;
        tglContactActive.Properties.OnText = string.Empty;
        tglContactActive.Size = new Size(48, 24);
        tglContactActive.TabIndex = 24;
        // 
        // lblContactActiveValue
        // 
        lblContactActiveValue.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactActiveValue.Appearance.Options.UseFont = true;
        lblContactActiveValue.Location = new Point(194, 314);
        lblContactActiveValue.Name = "lblContactActiveValue";
        lblContactActiveValue.Size = new Size(11, 15);
        lblContactActiveValue.TabIndex = 25;
        lblContactActiveValue.Text = "Sí";
        // 
        // lblContactNotes
        // 
        lblContactNotes.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblContactNotes.Appearance.Options.UseFont = true;
        lblContactNotes.Location = new Point(24, 346);
        lblContactNotes.Name = "lblContactNotes";
        lblContactNotes.Size = new Size(79, 15);
        lblContactNotes.TabIndex = 26;
        lblContactNotes.Text = "Observaciones:";
        // 
        // memContactNotes
        // 
        memContactNotes.Location = new Point(142, 343);
        memContactNotes.Name = "memContactNotes";
        memContactNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        memContactNotes.Properties.Appearance.Options.UseFont = true;
        memContactNotes.Size = new Size(280, 66);
        memContactNotes.TabIndex = 27;
        // 
        // pnlFooter
        // 
        pnlFooter.BorderStyle = BorderStyles.NoBorder;
        pnlFooter.Controls.Add(btnSaveContact);
        pnlFooter.Controls.Add(btnCancelContact);
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Location = new Point(0, 378);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Size = new Size(458, 64);
        pnlFooter.TabIndex = 1;
        // 
        // btnSaveContact
        // 
        btnSaveContact.Appearance.BackColor = Color.FromArgb(0, 102, 204);
        btnSaveContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnSaveContact.Appearance.ForeColor = Color.White;
        btnSaveContact.Appearance.Options.UseBackColor = true;
        btnSaveContact.Appearance.Options.UseFont = true;
        btnSaveContact.Appearance.Options.UseForeColor = true;
        btnSaveContact.Location = new Point(168, 16);
        btnSaveContact.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSaveContact.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSaveContact.Name = "btnSaveContact";
        btnSaveContact.Size = new Size(100, 32);
        btnSaveContact.TabIndex = 0;
        btnSaveContact.Text = "Guardar";
        // 
        // btnCancelContact
        // 
        btnCancelContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnCancelContact.Appearance.Options.UseFont = true;
        btnCancelContact.DialogResult = DialogResult.Cancel;
        btnCancelContact.Location = new Point(282, 16);
        btnCancelContact.Name = "btnCancelContact";
        btnCancelContact.Size = new Size(100, 32);
        btnCancelContact.TabIndex = 1;
        btnCancelContact.Text = "Cancelar";
        // 
        // SupplierContactEditDialog
        // 
        AcceptButton = btnSaveContact;
        Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelContact;
        ClientSize = new Size(458, 442);
        Controls.Add(pnlMain);
        Controls.Add(pnlFooter);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SupplierContactEditDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nuevo Contacto";
        ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
        pnlMain.ResumeLayout(false);
        pnlMain.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueContactTreatment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactFirstName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactLastName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPosition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactDepartment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactExtension.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactMobile.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteContactBirthday.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteContactBirthday.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglContactPrimary.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglContactActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memContactNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlFooter).EndInit();
        pnlFooter.ResumeLayout(false);
        ResumeLayout(false);
    }

    private PanelControl pnlMain;
    private LabelControl lblContactTreatment;
    private LookUpEdit lueContactTreatment;
    private LabelControl lblContactFirstName;
    private TextEdit txtContactFirstName;
    private LabelControl lblContactLastName;
    private TextEdit txtContactLastName;
    private LabelControl lblContactPosition;
    private TextEdit txtContactPosition;
    private LabelControl lblContactDepartment;
    private TextEdit txtContactDepartment;
    private LabelControl lblContactPhone;
    private TextEdit txtContactPhone;
    private LabelControl lblContactExtension;
    private TextEdit txtContactExtension;
    private LabelControl lblContactMobile;
    private TextEdit txtContactMobile;
    private LabelControl lblContactEmail;
    private TextEdit txtContactEmail;
    private LabelControl lblContactBirthday;
    private DateEdit dteContactBirthday;
    private LabelControl lblContactPrimary;
    private ToggleSwitch tglContactPrimary;
    private LabelControl lblContactPrimaryValue;
    private LabelControl lblContactActive;
    private ToggleSwitch tglContactActive;
    private LabelControl lblContactActiveValue;
    private LabelControl lblContactNotes;
    private MemoEdit memContactNotes;
    private PanelControl pnlFooter;
    private SimpleButton btnSaveContact;
    private SimpleButton btnCancelContact;
}
