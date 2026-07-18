using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync.EntityDefinitions;

partial class SyncEntityEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblGeneralTitle = new LabelControl();
        sepGeneral = new SeparatorControl();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblExecutionOrder = new LabelControl();
        spnExecutionOrder = new SpinEdit();
        chkActive = new CheckEdit();
        lblCapabilitiesTitle = new LabelControl();
        sepCapabilities = new SeparatorControl();
        lblKeyField = new LabelControl();
        txtKeyField = new TextEdit();
        lblModifiedAtField = new LabelControl();
        txtModifiedAtField = new TextEdit();
        chkIncremental = new CheckEdit();
        chkInsert = new CheckEdit();
        chkUpdate = new CheckEdit();
        chkDeactivate = new CheckEdit();
        chkSystem = new CheckEdit();
        chkProducer = new CheckEdit();
        chkApplier = new CheckEdit();
        chkOperative = new CheckEdit();
        lblDependenciesTitle = new LabelControl();
        sepDependencies = new SeparatorControl();
        grdDependencies = new NuanDataGridControl();
        ((System.ComponentModel.ISupportInitialize)sepGeneral).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnExecutionOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepCapabilities).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtKeyField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtModifiedAtField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIncremental.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkInsert.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkUpdate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkDeactivate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkProducer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkApplier.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkOperative.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepDependencies).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseBorderColor = true;
        btnCancelar.Appearance.Options.UseFont = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseBorderColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.ForeColor = Color.White;
        btnCancelar.AppearancePressed.Options.UseBackColor = true;
        btnCancelar.AppearancePressed.Options.UseBorderColor = true;
        btnCancelar.AppearancePressed.Options.UseForeColor = true;
        btnCancelar.ImageOptions.ImageToTextIndent = 0;
        btnCancelar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnCancelar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnCancelar.Location = new Point(598, 594);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 28;
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseBorderColor = true;
        btnGuardar.Appearance.Options.UseFont = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseBorderColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.ForeColor = Color.White;
        btnGuardar.AppearancePressed.Options.UseBackColor = true;
        btnGuardar.AppearancePressed.Options.UseBorderColor = true;
        btnGuardar.AppearancePressed.Options.UseForeColor = true;
        btnGuardar.ImageOptions.ImageToTextIndent = 0;
        btnGuardar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnGuardar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnGuardar.Location = new Point(704, 594);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 29;
        // 
        // lblGeneralTitle
        // 
        lblGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblGeneralTitle.Appearance.Options.UseFont = true;
        lblGeneralTitle.Appearance.Options.UseForeColor = true;
        lblGeneralTitle.Location = new Point(26, 20);
        lblGeneralTitle.Name = "lblGeneralTitle";
        lblGeneralTitle.Size = new Size(109, 20);
        lblGeneralTitle.TabIndex = 0;
        lblGeneralTitle.Text = "Datos generales";
        // 
        // sepGeneral
        // 
        sepGeneral.Location = new Point(26, 40);
        sepGeneral.Name = "sepGeneral";
        sepGeneral.Size = new Size(374, 18);
        sepGeneral.TabIndex = 1;
        // 
        // lblCode
        // 
        lblCode.Location = new Point(26, 66);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(33, 13);
        lblCode.TabIndex = 2;
        lblCode.Text = "Codigo";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(148, 63);
        txtCode.Name = "txtCode";
        txtCode.Properties.MaxLength = 80;
        txtCode.Size = new Size(252, 20);
        txtCode.TabIndex = 3;
        // 
        // lblName
        // 
        lblName.Location = new Point(26, 94);
        lblName.Name = "lblName";
        lblName.Size = new Size(37, 13);
        lblName.TabIndex = 4;
        lblName.Text = "Nombre";
        // 
        // txtName
        // 
        txtName.Location = new Point(148, 91);
        txtName.Name = "txtName";
        txtName.Properties.MaxLength = 120;
        txtName.Size = new Size(252, 20);
        txtName.TabIndex = 5;
        // 
        // lblDescription
        // 
        lblDescription.Location = new Point(26, 122);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(54, 13);
        lblDescription.TabIndex = 6;
        lblDescription.Text = "Descripcion";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(148, 119);
        memDescription.Name = "memDescription";
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(252, 68);
        memDescription.TabIndex = 7;
        // 
        // lblExecutionOrder
        // 
        lblExecutionOrder.Location = new Point(26, 200);
        lblExecutionOrder.Name = "lblExecutionOrder";
        lblExecutionOrder.Size = new Size(93, 13);
        lblExecutionOrder.TabIndex = 8;
        lblExecutionOrder.Text = "Orden de ejecucion";
        // 
        // spnExecutionOrder
        // 
        spnExecutionOrder.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnExecutionOrder.Location = new Point(148, 197);
        spnExecutionOrder.Name = "spnExecutionOrder";
        spnExecutionOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        spnExecutionOrder.Properties.IsFloatValue = false;
        spnExecutionOrder.Properties.MaskSettings.Set("mask", "N00");
        spnExecutionOrder.Properties.MaxValue = new decimal(new int[] { 9999, 0, 0, 0 });
        spnExecutionOrder.Size = new Size(120, 20);
        spnExecutionOrder.TabIndex = 9;
        // 
        // chkActive
        // 
        chkActive.EditValue = true;
        chkActive.Location = new Point(145, 228);
        chkActive.Name = "chkActive";
        chkActive.Properties.Caption = "Activo";
        chkActive.Size = new Size(96, 20);
        chkActive.TabIndex = 10;
        // 
        // lblCapabilitiesTitle
        // 
        lblCapabilitiesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCapabilitiesTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblCapabilitiesTitle.Appearance.Options.UseFont = true;
        lblCapabilitiesTitle.Appearance.Options.UseForeColor = true;
        lblCapabilitiesTitle.Location = new Point(430, 20);
        lblCapabilitiesTitle.Name = "lblCapabilitiesTitle";
        lblCapabilitiesTitle.Size = new Size(143, 20);
        lblCapabilitiesTitle.TabIndex = 11;
        lblCapabilitiesTitle.Text = "Capacidades tecnicas";
        // 
        // sepCapabilities
        // 
        sepCapabilities.Location = new Point(430, 40);
        sepCapabilities.Name = "sepCapabilities";
        sepCapabilities.Size = new Size(374, 18);
        sepCapabilities.TabIndex = 12;
        // 
        // lblKeyField
        // 
        lblKeyField.Location = new Point(430, 66);
        lblKeyField.Name = "lblKeyField";
        lblKeyField.Size = new Size(61, 13);
        lblKeyField.TabIndex = 13;
        lblKeyField.Text = "Campo clave";
        // 
        // txtKeyField
        // 
        txtKeyField.Location = new Point(564, 63);
        txtKeyField.Name = "txtKeyField";
        txtKeyField.Properties.MaxLength = 100;
        txtKeyField.Size = new Size(240, 20);
        txtKeyField.TabIndex = 14;
        // 
        // lblModifiedAtField
        // 
        lblModifiedAtField.Location = new Point(430, 94);
        lblModifiedAtField.Name = "lblModifiedAtField";
        lblModifiedAtField.Size = new Size(94, 13);
        lblModifiedAtField.TabIndex = 15;
        lblModifiedAtField.Text = "Campo modificacion";
        // 
        // txtModifiedAtField
        // 
        txtModifiedAtField.Location = new Point(564, 91);
        txtModifiedAtField.Name = "txtModifiedAtField";
        txtModifiedAtField.Properties.MaxLength = 100;
        txtModifiedAtField.Size = new Size(240, 20);
        txtModifiedAtField.TabIndex = 16;
        // 
        // chkIncremental
        // 
        chkIncremental.Location = new Point(427, 124);
        chkIncremental.Name = "chkIncremental";
        chkIncremental.Properties.Caption = "Soporta sincronizacion incremental";
        chkIncremental.Size = new Size(207, 20);
        chkIncremental.TabIndex = 17;
        // 
        // chkInsert
        // 
        chkInsert.Location = new Point(427, 150);
        chkInsert.Name = "chkInsert";
        chkInsert.Properties.Caption = "Permite insertar";
        chkInsert.Size = new Size(140, 20);
        chkInsert.TabIndex = 18;
        // 
        // chkUpdate
        // 
        chkUpdate.Location = new Point(427, 176);
        chkUpdate.Name = "chkUpdate";
        chkUpdate.Properties.Caption = "Permite actualizar";
        chkUpdate.Size = new Size(140, 20);
        chkUpdate.TabIndex = 19;
        // 
        // chkDeactivate
        // 
        chkDeactivate.Location = new Point(427, 202);
        chkDeactivate.Name = "chkDeactivate";
        chkDeactivate.Properties.Caption = "Permite desactivar";
        chkDeactivate.Size = new Size(140, 20);
        chkDeactivate.TabIndex = 20;
        // 
        // chkSystem
        // 
        chkSystem.Enabled = false;
        chkSystem.Location = new Point(640, 124);
        chkSystem.Name = "chkSystem";
        chkSystem.Properties.Caption = "Definicion del sistema";
        chkSystem.Size = new Size(164, 20);
        chkSystem.TabIndex = 21;
        // 
        // chkProducer
        // 
        chkProducer.Enabled = false;
        chkProducer.Location = new Point(640, 150);
        chkProducer.Name = "chkProducer";
        chkProducer.Properties.Caption = "Productor disponible";
        chkProducer.Size = new Size(164, 20);
        chkProducer.TabIndex = 22;
        // 
        // chkApplier
        // 
        chkApplier.Enabled = false;
        chkApplier.Location = new Point(640, 176);
        chkApplier.Name = "chkApplier";
        chkApplier.Properties.Caption = "Aplicador disponible";
        chkApplier.Size = new Size(164, 20);
        chkApplier.TabIndex = 23;
        // 
        // chkOperative
        // 
        chkOperative.Enabled = false;
        chkOperative.Location = new Point(640, 202);
        chkOperative.Name = "chkOperative";
        chkOperative.Properties.Caption = "Entidad operativa";
        chkOperative.Size = new Size(164, 20);
        chkOperative.TabIndex = 24;
        // 
        // lblDependenciesTitle
        // 
        lblDependenciesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblDependenciesTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblDependenciesTitle.Appearance.Options.UseFont = true;
        lblDependenciesTitle.Appearance.Options.UseForeColor = true;
        lblDependenciesTitle.Location = new Point(26, 276);
        lblDependenciesTitle.Name = "lblDependenciesTitle";
        lblDependenciesTitle.Size = new Size(96, 20);
        lblDependenciesTitle.TabIndex = 25;
        lblDependenciesTitle.Text = "Dependencias";
        // 
        // sepDependencies
        // 
        sepDependencies.Location = new Point(26, 296);
        sepDependencies.Name = "sepDependencies";
        sepDependencies.Size = new Size(778, 18);
        sepDependencies.TabIndex = 26;
        // 
        // grdDependencies
        // 
        grdDependencies.FormKey = "sync-entities";
        grdDependencies.GridName = "DependenciesGrid";
        grdDependencies.Location = new Point(26, 318);
        grdDependencies.Name = "grdDependencies";
        grdDependencies.ShowPagination = false;
        grdDependencies.Size = new Size(778, 254);
        grdDependencies.TabIndex = 27;
        // 
        // SyncEntityEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(830, 650);
        Controls.Add(lblGeneralTitle);
        Controls.Add(sepGeneral);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblExecutionOrder);
        Controls.Add(spnExecutionOrder);
        Controls.Add(chkActive);
        Controls.Add(lblCapabilitiesTitle);
        Controls.Add(sepCapabilities);
        Controls.Add(lblKeyField);
        Controls.Add(txtKeyField);
        Controls.Add(lblModifiedAtField);
        Controls.Add(txtModifiedAtField);
        Controls.Add(chkIncremental);
        Controls.Add(chkInsert);
        Controls.Add(chkUpdate);
        Controls.Add(chkDeactivate);
        Controls.Add(chkSystem);
        Controls.Add(chkProducer);
        Controls.Add(chkApplier);
        Controls.Add(chkOperative);
        Controls.Add(lblDependenciesTitle);
        Controls.Add(sepDependencies);
        Controls.Add(grdDependencies);
        MinimumSize = new Size(832, 682);
        Name = "SyncEntityEditForm";
        Text = "Nueva entidad de sincronizacion";
        Controls.SetChildIndex(grdDependencies, 0);
        Controls.SetChildIndex(sepDependencies, 0);
        Controls.SetChildIndex(lblDependenciesTitle, 0);
        Controls.SetChildIndex(chkOperative, 0);
        Controls.SetChildIndex(chkApplier, 0);
        Controls.SetChildIndex(chkProducer, 0);
        Controls.SetChildIndex(chkSystem, 0);
        Controls.SetChildIndex(chkDeactivate, 0);
        Controls.SetChildIndex(chkUpdate, 0);
        Controls.SetChildIndex(chkInsert, 0);
        Controls.SetChildIndex(chkIncremental, 0);
        Controls.SetChildIndex(txtModifiedAtField, 0);
        Controls.SetChildIndex(lblModifiedAtField, 0);
        Controls.SetChildIndex(txtKeyField, 0);
        Controls.SetChildIndex(lblKeyField, 0);
        Controls.SetChildIndex(sepCapabilities, 0);
        Controls.SetChildIndex(lblCapabilitiesTitle, 0);
        Controls.SetChildIndex(chkActive, 0);
        Controls.SetChildIndex(spnExecutionOrder, 0);
        Controls.SetChildIndex(lblExecutionOrder, 0);
        Controls.SetChildIndex(memDescription, 0);
        Controls.SetChildIndex(lblDescription, 0);
        Controls.SetChildIndex(txtName, 0);
        Controls.SetChildIndex(lblName, 0);
        Controls.SetChildIndex(txtCode, 0);
        Controls.SetChildIndex(lblCode, 0);
        Controls.SetChildIndex(sepGeneral, 0);
        Controls.SetChildIndex(lblGeneralTitle, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)sepGeneral).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnExecutionOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepCapabilities).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtKeyField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtModifiedAtField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIncremental.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkInsert.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkUpdate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkDeactivate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkProducer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkApplier.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkOperative.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepDependencies).EndInit();
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

    private LabelControl lblGeneralTitle;
    private SeparatorControl sepGeneral;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblExecutionOrder;
    private SpinEdit spnExecutionOrder;
    private CheckEdit chkActive;
    private LabelControl lblCapabilitiesTitle;
    private SeparatorControl sepCapabilities;
    private LabelControl lblKeyField;
    private TextEdit txtKeyField;
    private LabelControl lblModifiedAtField;
    private TextEdit txtModifiedAtField;
    private CheckEdit chkIncremental;
    private CheckEdit chkInsert;
    private CheckEdit chkUpdate;
    private CheckEdit chkDeactivate;
    private CheckEdit chkSystem;
    private CheckEdit chkProducer;
    private CheckEdit chkApplier;
    private CheckEdit chkOperative;
    private LabelControl lblDependenciesTitle;
    private SeparatorControl sepDependencies;
    private NuanDataGridControl grdDependencies;
}
