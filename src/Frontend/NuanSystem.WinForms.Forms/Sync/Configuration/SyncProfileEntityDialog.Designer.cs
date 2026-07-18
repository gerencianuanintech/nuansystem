using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Lookups;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class SyncProfileEntityDialog
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl lblEntitySectionTitle;
    private SeparatorControl sepEntitySection;
    private LabelControl lblEntity;
    private NuanLookupEdit lueEntity;
    private LabelControl lblEntityName;
    private TextEdit txtEntityName;
    private LabelControl lblExecutionOrder;
    private SpinEdit sedExecutionOrder;
    private LabelControl lblSyncMode;
    private ComboBoxEdit cboSyncMode;
    private LabelControl lblIsActive;
    private ToggleSwitch swIsActive;
    private LabelControl lblTechnicalSectionTitle;
    private SeparatorControl sepTechnicalSection;
    private LabelControl lblKeyField;
    private TextEdit txtKeyField;
    private LabelControl lblModifiedAtField;
    private TextEdit txtModifiedAtField;
    private LabelControl lblVersionField;
    private TextEdit txtVersionField;
    private LabelControl lblActiveField;
    private TextEdit txtActiveField;
    private LabelControl lblCapabilitiesSectionTitle;
    private SeparatorControl sepCapabilitiesSection;
    private LabelControl lblAllowInsert;
    private ToggleSwitch swAllowInsert;
    private LabelControl lblAllowUpdate;
    private ToggleSwitch swAllowUpdate;
    private LabelControl lblAllowDeactivate;
    private ToggleSwitch swAllowDeactivate;
    private LabelControl lblContinueOnError;
    private ToggleSwitch swContinueOnError;
    private LabelControl lblBatchSize;
    private SpinEdit sedBatchSize;
    private LabelControl lblDependencies;
    private TextEdit txtDependencies;
    private NuanActionButton btnSave;
    private NuanActionButton btnCancel;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        EditorButtonImageOptions editorButtonImageOptions1 = new EditorButtonImageOptions();
        SerializableAppearanceObject serializableAppearanceObject1 = new SerializableAppearanceObject();
        SerializableAppearanceObject serializableAppearanceObject2 = new SerializableAppearanceObject();
        SerializableAppearanceObject serializableAppearanceObject3 = new SerializableAppearanceObject();
        SerializableAppearanceObject serializableAppearanceObject4 = new SerializableAppearanceObject();
        EditorButtonImageOptions editorButtonImageOptions2 = new EditorButtonImageOptions();
        SerializableAppearanceObject serializableAppearanceObject5 = new SerializableAppearanceObject();
        SerializableAppearanceObject serializableAppearanceObject6 = new SerializableAppearanceObject();
        SerializableAppearanceObject serializableAppearanceObject7 = new SerializableAppearanceObject();
        SerializableAppearanceObject serializableAppearanceObject8 = new SerializableAppearanceObject();
        lblEntitySectionTitle = new LabelControl();
        sepEntitySection = new SeparatorControl();
        lblEntity = new LabelControl();
        lueEntity = new NuanLookupEdit();
        lblEntityName = new LabelControl();
        txtEntityName = new TextEdit();
        lblExecutionOrder = new LabelControl();
        sedExecutionOrder = new SpinEdit();
        lblSyncMode = new LabelControl();
        cboSyncMode = new ComboBoxEdit();
        lblIsActive = new LabelControl();
        swIsActive = new ToggleSwitch();
        lblTechnicalSectionTitle = new LabelControl();
        sepTechnicalSection = new SeparatorControl();
        lblKeyField = new LabelControl();
        txtKeyField = new TextEdit();
        lblModifiedAtField = new LabelControl();
        txtModifiedAtField = new TextEdit();
        lblVersionField = new LabelControl();
        txtVersionField = new TextEdit();
        lblActiveField = new LabelControl();
        txtActiveField = new TextEdit();
        lblCapabilitiesSectionTitle = new LabelControl();
        sepCapabilitiesSection = new SeparatorControl();
        lblAllowInsert = new LabelControl();
        swAllowInsert = new ToggleSwitch();
        lblAllowUpdate = new LabelControl();
        swAllowUpdate = new ToggleSwitch();
        lblAllowDeactivate = new LabelControl();
        swAllowDeactivate = new ToggleSwitch();
        lblContinueOnError = new LabelControl();
        swContinueOnError = new ToggleSwitch();
        lblBatchSize = new LabelControl();
        sedBatchSize = new SpinEdit();
        lblDependencies = new LabelControl();
        txtDependencies = new TextEdit();
        btnSave = new NuanActionButton();
        btnCancel = new NuanActionButton();
        ((System.ComponentModel.ISupportInitialize)sepEntitySection).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueEntity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedExecutionOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboSyncMode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepTechnicalSection).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtKeyField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtModifiedAtField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtVersionField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtActiveField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepCapabilitiesSection).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swAllowInsert.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swAllowUpdate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swAllowDeactivate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swContinueOnError.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedBatchSize.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDependencies.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblEntitySectionTitle
        // 
        lblEntitySectionTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblEntitySectionTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblEntitySectionTitle.Appearance.Options.UseFont = true;
        lblEntitySectionTitle.Appearance.Options.UseForeColor = true;
        lblEntitySectionTitle.Location = new Point(24, 20);
        lblEntitySectionTitle.Name = "lblEntitySectionTitle";
        lblEntitySectionTitle.Size = new Size(132, 20);
        lblEntitySectionTitle.TabIndex = 0;
        lblEntitySectionTitle.Text = "Datos de la entidad";
        // 
        // sepEntitySection
        // 
        sepEntitySection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepEntitySection.LineColor = Color.FromArgb(0, 184, 148);
        sepEntitySection.Location = new Point(24, 44);
        sepEntitySection.Name = "sepEntitySection";
        sepEntitySection.Size = new Size(558, 12);
        sepEntitySection.TabIndex = 1;
        // 
        // lblEntity
        // 
        lblEntity.Appearance.Font = new Font("Segoe UI", 9F);
        lblEntity.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblEntity.Appearance.Options.UseFont = true;
        lblEntity.Appearance.Options.UseForeColor = true;
        lblEntity.Location = new Point(24, 70);
        lblEntity.Name = "lblEntity";
        lblEntity.Size = new Size(82, 15);
        lblEntity.TabIndex = 2;
        lblEntity.Text = "Código entidad";
        // 
        // lueEntity
        // 
        lueEntity.Location = new Point(205, 66);
        lueEntity.Name = "lueEntity";
        lueEntity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueEntity.Properties.Appearance.Options.UseFont = true;
        lueEntity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete, "", -1, true, true, false, editorButtonImageOptions1, new KeyShortcut(Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Limpiar seleccion", null, null, ToolTipAnchor.Default), new EditorButton(ButtonPredefines.Plus, "", -1, false, true, false, editorButtonImageOptions2, new KeyShortcut(Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Crear nuevo", null, null, ToolTipAnchor.Default) });
        lueEntity.Properties.NullText = "";
        lueEntity.Size = new Size(370, 22);
        lueEntity.TabIndex = 3;
        // 
        // lblEntityName
        // 
        lblEntityName.Appearance.Font = new Font("Segoe UI", 9F);
        lblEntityName.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblEntityName.Appearance.Options.UseFont = true;
        lblEntityName.Appearance.Options.UseForeColor = true;
        lblEntityName.Location = new Point(24, 98);
        lblEntityName.Name = "lblEntityName";
        lblEntityName.Size = new Size(87, 15);
        lblEntityName.TabIndex = 4;
        lblEntityName.Text = "Nombre entidad";
        // 
        // txtEntityName
        // 
        txtEntityName.Location = new Point(205, 94);
        txtEntityName.Name = "txtEntityName";
        txtEntityName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEntityName.Properties.Appearance.Options.UseFont = true;
        txtEntityName.Properties.ReadOnly = true;
        txtEntityName.Size = new Size(370, 22);
        txtEntityName.TabIndex = 5;
        // 
        // lblExecutionOrder
        // 
        lblExecutionOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblExecutionOrder.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblExecutionOrder.Appearance.Options.UseFont = true;
        lblExecutionOrder.Appearance.Options.UseForeColor = true;
        lblExecutionOrder.Location = new Point(24, 126);
        lblExecutionOrder.Name = "lblExecutionOrder";
        lblExecutionOrder.Size = new Size(103, 15);
        lblExecutionOrder.TabIndex = 6;
        lblExecutionOrder.Text = "Orden de ejecución";
        // 
        // sedExecutionOrder
        // 
        sedExecutionOrder.EditValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedExecutionOrder.Location = new Point(205, 122);
        sedExecutionOrder.Name = "sedExecutionOrder";
        sedExecutionOrder.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sedExecutionOrder.Properties.Appearance.Options.UseFont = true;
        sedExecutionOrder.Properties.Appearance.Options.UseTextOptions = true;
        sedExecutionOrder.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        sedExecutionOrder.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sedExecutionOrder.Properties.IsFloatValue = false;
        sedExecutionOrder.Properties.MaskSettings.Set("mask", "d");
        sedExecutionOrder.Properties.MaxValue = new decimal(new int[] { 9999, 0, 0, 0 });
        sedExecutionOrder.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedExecutionOrder.Size = new Size(150, 22);
        sedExecutionOrder.TabIndex = 7;
        // 
        // lblSyncMode
        // 
        lblSyncMode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSyncMode.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblSyncMode.Appearance.Options.UseFont = true;
        lblSyncMode.Appearance.Options.UseForeColor = true;
        lblSyncMode.Location = new Point(24, 154);
        lblSyncMode.Name = "lblSyncMode";
        lblSyncMode.Size = new Size(127, 15);
        lblSyncMode.TabIndex = 8;
        lblSyncMode.Text = "Modo de sincronización";
        // 
        // cboSyncMode
        // 
        cboSyncMode.Location = new Point(205, 150);
        cboSyncMode.Name = "cboSyncMode";
        cboSyncMode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboSyncMode.Properties.Appearance.Options.UseFont = true;
        cboSyncMode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboSyncMode.Size = new Size(150, 22);
        cboSyncMode.TabIndex = 9;
        // 
        // lblIsActive
        // 
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(390, 154);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Size = new Size(80, 15);
        lblIsActive.TabIndex = 10;
        lblIsActive.Text = "Activo en perfil";
        // 
        // swIsActive
        // 
        swIsActive.EditValue = true;
        swIsActive.Location = new Point(478, 150);
        swIsActive.Name = "swIsActive";
        swIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swIsActive.Properties.Appearance.Options.UseFont = true;
        swIsActive.Properties.OffText = "Inactivo";
        swIsActive.Properties.OnText = "Activo";
        swIsActive.Size = new Size(97, 20);
        swIsActive.TabIndex = 11;
        // 
        // lblTechnicalSectionTitle
        // 
        lblTechnicalSectionTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblTechnicalSectionTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblTechnicalSectionTitle.Appearance.Options.UseFont = true;
        lblTechnicalSectionTitle.Appearance.Options.UseForeColor = true;
        lblTechnicalSectionTitle.Location = new Point(24, 196);
        lblTechnicalSectionTitle.Name = "lblTechnicalSectionTitle";
        lblTechnicalSectionTitle.Size = new Size(113, 20);
        lblTechnicalSectionTitle.TabIndex = 12;
        lblTechnicalSectionTitle.Text = "Campos técnicos";
        // 
        // sepTechnicalSection
        // 
        sepTechnicalSection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepTechnicalSection.LineColor = Color.FromArgb(0, 184, 148);
        sepTechnicalSection.Location = new Point(24, 220);
        sepTechnicalSection.Name = "sepTechnicalSection";
        sepTechnicalSection.Size = new Size(558, 12);
        sepTechnicalSection.TabIndex = 13;
        // 
        // lblKeyField
        // 
        lblKeyField.Appearance.Font = new Font("Segoe UI", 9F);
        lblKeyField.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblKeyField.Appearance.Options.UseFont = true;
        lblKeyField.Appearance.Options.UseForeColor = true;
        lblKeyField.Location = new Point(24, 246);
        lblKeyField.Name = "lblKeyField";
        lblKeyField.Size = new Size(44, 15);
        lblKeyField.TabIndex = 14;
        lblKeyField.Text = "KeyField";
        // 
        // txtKeyField
        // 
        txtKeyField.Location = new Point(205, 242);
        txtKeyField.Name = "txtKeyField";
        txtKeyField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtKeyField.Properties.Appearance.Options.UseFont = true;
        txtKeyField.Size = new Size(370, 22);
        txtKeyField.TabIndex = 15;
        // 
        // lblModifiedAtField
        // 
        lblModifiedAtField.Appearance.Font = new Font("Segoe UI", 9F);
        lblModifiedAtField.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblModifiedAtField.Appearance.Options.UseFont = true;
        lblModifiedAtField.Appearance.Options.UseForeColor = true;
        lblModifiedAtField.Location = new Point(24, 274);
        lblModifiedAtField.Name = "lblModifiedAtField";
        lblModifiedAtField.Size = new Size(85, 15);
        lblModifiedAtField.TabIndex = 16;
        lblModifiedAtField.Text = "ModifiedAtField";
        // 
        // txtModifiedAtField
        // 
        txtModifiedAtField.Location = new Point(205, 270);
        txtModifiedAtField.Name = "txtModifiedAtField";
        txtModifiedAtField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtModifiedAtField.Properties.Appearance.Options.UseFont = true;
        txtModifiedAtField.Size = new Size(370, 22);
        txtModifiedAtField.TabIndex = 17;
        // 
        // lblVersionField
        // 
        lblVersionField.Appearance.Font = new Font("Segoe UI", 9F);
        lblVersionField.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblVersionField.Appearance.Options.UseFont = true;
        lblVersionField.Appearance.Options.UseForeColor = true;
        lblVersionField.Location = new Point(24, 302);
        lblVersionField.Name = "lblVersionField";
        lblVersionField.Size = new Size(64, 15);
        lblVersionField.TabIndex = 18;
        lblVersionField.Text = "VersionField";
        // 
        // txtVersionField
        // 
        txtVersionField.Location = new Point(205, 298);
        txtVersionField.Name = "txtVersionField";
        txtVersionField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtVersionField.Properties.Appearance.Options.UseFont = true;
        txtVersionField.Size = new Size(370, 22);
        txtVersionField.TabIndex = 19;
        // 
        // lblActiveField
        // 
        lblActiveField.Appearance.Font = new Font("Segoe UI", 9F);
        lblActiveField.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblActiveField.Appearance.Options.UseFont = true;
        lblActiveField.Appearance.Options.UseForeColor = true;
        lblActiveField.Location = new Point(24, 330);
        lblActiveField.Name = "lblActiveField";
        lblActiveField.Size = new Size(58, 15);
        lblActiveField.TabIndex = 20;
        lblActiveField.Text = "ActiveField";
        // 
        // txtActiveField
        // 
        txtActiveField.Location = new Point(205, 326);
        txtActiveField.Name = "txtActiveField";
        txtActiveField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtActiveField.Properties.Appearance.Options.UseFont = true;
        txtActiveField.Size = new Size(370, 22);
        txtActiveField.TabIndex = 21;
        // 
        // lblCapabilitiesSectionTitle
        // 
        lblCapabilitiesSectionTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCapabilitiesSectionTitle.Appearance.ForeColor = Color.FromArgb(0, 137, 111);
        lblCapabilitiesSectionTitle.Appearance.Options.UseFont = true;
        lblCapabilitiesSectionTitle.Appearance.Options.UseForeColor = true;
        lblCapabilitiesSectionTitle.Location = new Point(24, 374);
        lblCapabilitiesSectionTitle.Name = "lblCapabilitiesSectionTitle";
        lblCapabilitiesSectionTitle.Size = new Size(166, 20);
        lblCapabilitiesSectionTitle.TabIndex = 22;
        lblCapabilitiesSectionTitle.Text = "Capacidades y ejecución";
        // 
        // sepCapabilitiesSection
        // 
        sepCapabilitiesSection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sepCapabilitiesSection.LineColor = Color.FromArgb(0, 184, 148);
        sepCapabilitiesSection.Location = new Point(24, 398);
        sepCapabilitiesSection.Name = "sepCapabilitiesSection";
        sepCapabilitiesSection.Size = new Size(558, 12);
        sepCapabilitiesSection.TabIndex = 23;
        // 
        // lblAllowInsert
        // 
        lblAllowInsert.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowInsert.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblAllowInsert.Appearance.Options.UseFont = true;
        lblAllowInsert.Appearance.Options.UseForeColor = true;
        lblAllowInsert.Location = new Point(24, 424);
        lblAllowInsert.Name = "lblAllowInsert";
        lblAllowInsert.Size = new Size(84, 15);
        lblAllowInsert.TabIndex = 24;
        lblAllowInsert.Text = "Permitir insertar";
        // 
        // swAllowInsert
        // 
        swAllowInsert.EditValue = true;
        swAllowInsert.Location = new Point(205, 420);
        swAllowInsert.Name = "swAllowInsert";
        swAllowInsert.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swAllowInsert.Properties.Appearance.Options.UseFont = true;
        swAllowInsert.Properties.OffText = "No";
        swAllowInsert.Properties.OnText = "Sí";
        swAllowInsert.Size = new Size(82, 20);
        swAllowInsert.TabIndex = 25;
        // 
        // lblAllowUpdate
        // 
        lblAllowUpdate.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowUpdate.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblAllowUpdate.Appearance.Options.UseFont = true;
        lblAllowUpdate.Appearance.Options.UseForeColor = true;
        lblAllowUpdate.Location = new Point(322, 424);
        lblAllowUpdate.Name = "lblAllowUpdate";
        lblAllowUpdate.Size = new Size(95, 15);
        lblAllowUpdate.TabIndex = 26;
        lblAllowUpdate.Text = "Permitir actualizar";
        // 
        // swAllowUpdate
        // 
        swAllowUpdate.EditValue = true;
        swAllowUpdate.Location = new Point(440, 420);
        swAllowUpdate.Name = "swAllowUpdate";
        swAllowUpdate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swAllowUpdate.Properties.Appearance.Options.UseFont = true;
        swAllowUpdate.Properties.OffText = "No";
        swAllowUpdate.Properties.OnText = "Sí";
        swAllowUpdate.Size = new Size(82, 20);
        swAllowUpdate.TabIndex = 27;
        // 
        // lblAllowDeactivate
        // 
        lblAllowDeactivate.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowDeactivate.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblAllowDeactivate.Appearance.Options.UseFont = true;
        lblAllowDeactivate.Appearance.Options.UseForeColor = true;
        lblAllowDeactivate.Location = new Point(24, 450);
        lblAllowDeactivate.Name = "lblAllowDeactivate";
        lblAllowDeactivate.Size = new Size(98, 15);
        lblAllowDeactivate.TabIndex = 28;
        lblAllowDeactivate.Text = "Permitir desactivar";
        // 
        // swAllowDeactivate
        // 
        swAllowDeactivate.EditValue = true;
        swAllowDeactivate.Location = new Point(205, 446);
        swAllowDeactivate.Name = "swAllowDeactivate";
        swAllowDeactivate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swAllowDeactivate.Properties.Appearance.Options.UseFont = true;
        swAllowDeactivate.Properties.OffText = "No";
        swAllowDeactivate.Properties.OnText = "Sí";
        swAllowDeactivate.Size = new Size(82, 20);
        swAllowDeactivate.TabIndex = 29;
        // 
        // lblContinueOnError
        // 
        lblContinueOnError.Appearance.Font = new Font("Segoe UI", 9F);
        lblContinueOnError.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblContinueOnError.Appearance.Options.UseFont = true;
        lblContinueOnError.Appearance.Options.UseForeColor = true;
        lblContinueOnError.Location = new Point(322, 450);
        lblContinueOnError.Name = "lblContinueOnError";
        lblContinueOnError.Size = new Size(97, 15);
        lblContinueOnError.TabIndex = 30;
        lblContinueOnError.Text = "Continuar en error";
        // 
        // swContinueOnError
        // 
        swContinueOnError.Location = new Point(440, 446);
        swContinueOnError.Name = "swContinueOnError";
        swContinueOnError.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swContinueOnError.Properties.Appearance.Options.UseFont = true;
        swContinueOnError.Properties.OffText = "No";
        swContinueOnError.Properties.OnText = "Sí";
        swContinueOnError.Size = new Size(82, 20);
        swContinueOnError.TabIndex = 31;
        // 
        // lblBatchSize
        // 
        lblBatchSize.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchSize.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBatchSize.Appearance.Options.UseFont = true;
        lblBatchSize.Appearance.Options.UseForeColor = true;
        lblBatchSize.Location = new Point(24, 504);
        lblBatchSize.Name = "lblBatchSize";
        lblBatchSize.Size = new Size(86, 15);
        lblBatchSize.TabIndex = 32;
        lblBatchSize.Text = "Batch específico";
        // 
        // sedBatchSize
        // 
        sedBatchSize.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedBatchSize.Location = new Point(205, 500);
        sedBatchSize.Name = "sedBatchSize";
        sedBatchSize.Properties.AllowNullInput = DefaultBoolean.True;
        sedBatchSize.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sedBatchSize.Properties.Appearance.Options.UseFont = true;
        sedBatchSize.Properties.Appearance.Options.UseTextOptions = true;
        sedBatchSize.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        sedBatchSize.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sedBatchSize.Properties.IsFloatValue = false;
        sedBatchSize.Properties.MaskSettings.Set("mask", "d");
        sedBatchSize.Properties.MaxValue = new decimal(new int[] { 100000, 0, 0, 0 });
        sedBatchSize.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedBatchSize.Size = new Size(150, 22);
        sedBatchSize.TabIndex = 33;
        // 
        // btnSave
        // 
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseBorderColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnSave.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnSave.AppearanceHovered.ForeColor = Color.White;
        btnSave.AppearanceHovered.Options.UseBackColor = true;
        btnSave.AppearanceHovered.Options.UseBorderColor = true;
        btnSave.AppearanceHovered.Options.UseForeColor = true;
        btnSave.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnSave.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnSave.AppearancePressed.ForeColor = Color.White;
        btnSave.AppearancePressed.Options.UseBackColor = true;
        btnSave.AppearancePressed.Options.UseBorderColor = true;
        btnSave.AppearancePressed.Options.UseForeColor = true;
        btnSave.ButtonKind = NuanActionButtonKind.Save;
        btnSave.ButtonStyle = BorderStyles.UltraFlat;
        btnSave.ButtonText = "Guardar";
        btnSave.DialogResult = DialogResult.OK;
        btnSave.IconNameOverride = "guardar_16.svg";
        btnSave.IconSize = 16;
        btnSave.ImageOptions.ImageToTextIndent = 0;
        btnSave.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnSave.ImageOptions.SvgImageSize = new Size(16, 16);
        // 
        // lblDependencies
        // 
        lblDependencies.Appearance.Font = new Font("Segoe UI", 9F);
        lblDependencies.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDependencies.Appearance.Options.UseFont = true;
        lblDependencies.Appearance.Options.UseForeColor = true;
        lblDependencies.Location = new Point(24, 476);
        lblDependencies.Name = "lblDependencies";
        lblDependencies.Size = new Size(75, 15);
        lblDependencies.TabIndex = 34;
        lblDependencies.Text = "Dependencias";
        // 
        // txtDependencies
        // 
        txtDependencies.Location = new Point(205, 472);
        txtDependencies.Name = "txtDependencies";
        txtDependencies.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDependencies.Properties.Appearance.Options.UseFont = true;
        txtDependencies.Properties.ReadOnly = true;
        txtDependencies.Size = new Size(370, 22);
        txtDependencies.TabIndex = 35;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(369, 537);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 36;
        btnSave.Text = "Guardar";
        btnSave.UseDefaultSize = true;
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.White;
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseBorderColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancel.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnCancel.AppearanceHovered.ForeColor = Color.White;
        btnCancel.AppearanceHovered.Options.UseBackColor = true;
        btnCancel.AppearanceHovered.Options.UseBorderColor = true;
        btnCancel.AppearanceHovered.Options.UseForeColor = true;
        btnCancel.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnCancel.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnCancel.AppearancePressed.ForeColor = Color.White;
        btnCancel.AppearancePressed.Options.UseBackColor = true;
        btnCancel.AppearancePressed.Options.UseBorderColor = true;
        btnCancel.AppearancePressed.Options.UseForeColor = true;
        btnCancel.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancel.ButtonStyle = BorderStyles.UltraFlat;
        btnCancel.ButtonText = "Cancelar";
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.IconNameOverride = "cancelar_16.svg";
        btnCancel.IconSize = 16;
        btnCancel.ImageOptions.ImageToTextIndent = 0;
        btnCancel.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnCancel.ImageOptions.SvgImageSize = new Size(16, 16);
        btnCancel.Location = new Point(475, 537);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 37;
        btnCancel.Text = "Cancelar";
        btnCancel.UseDefaultSize = true;
        // 
        // SyncProfileEntityDialog
        // 
        AcceptButton = btnSave;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(606, 597);
        Controls.Add(lblEntitySectionTitle);
        Controls.Add(sepEntitySection);
        Controls.Add(lblEntity);
        Controls.Add(lueEntity);
        Controls.Add(lblEntityName);
        Controls.Add(txtEntityName);
        Controls.Add(lblExecutionOrder);
        Controls.Add(sedExecutionOrder);
        Controls.Add(lblSyncMode);
        Controls.Add(cboSyncMode);
        Controls.Add(lblIsActive);
        Controls.Add(swIsActive);
        Controls.Add(lblTechnicalSectionTitle);
        Controls.Add(sepTechnicalSection);
        Controls.Add(lblKeyField);
        Controls.Add(txtKeyField);
        Controls.Add(lblModifiedAtField);
        Controls.Add(txtModifiedAtField);
        Controls.Add(lblVersionField);
        Controls.Add(txtVersionField);
        Controls.Add(lblActiveField);
        Controls.Add(txtActiveField);
        Controls.Add(lblCapabilitiesSectionTitle);
        Controls.Add(sepCapabilitiesSection);
        Controls.Add(lblAllowInsert);
        Controls.Add(swAllowInsert);
        Controls.Add(lblAllowUpdate);
        Controls.Add(swAllowUpdate);
        Controls.Add(lblAllowDeactivate);
        Controls.Add(swAllowDeactivate);
        Controls.Add(lblContinueOnError);
        Controls.Add(swContinueOnError);
        Controls.Add(lblBatchSize);
        Controls.Add(sedBatchSize);
        Controls.Add(lblDependencies);
        Controls.Add(txtDependencies);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SyncProfileEntityDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Entidad del perfil";
        ((System.ComponentModel.ISupportInitialize)sepEntitySection).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueEntity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEntityName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedExecutionOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboSyncMode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)swIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepTechnicalSection).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtKeyField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtModifiedAtField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtVersionField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtActiveField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepCapabilitiesSection).EndInit();
        ((System.ComponentModel.ISupportInitialize)swAllowInsert.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)swAllowUpdate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)swAllowDeactivate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)swContinueOnError.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedBatchSize.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDependencies.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
