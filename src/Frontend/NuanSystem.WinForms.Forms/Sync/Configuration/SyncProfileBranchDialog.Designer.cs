using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class SyncProfileBranchDialog
{
    private System.ComponentModel.IContainer components;
    private LabelControl lblBranchSectionTitle;
    private SeparatorControl sepBranchSection;
    private LabelControl lblBranchCompany;
    private NuanLookupEdit lueBranchCompany;
    private LabelControl lblBranchCode;
    private TextEdit txtBranchCode;
    private LabelControl lblBranchName;
    private TextEdit txtBranchName;
    private LabelControl lblDatabaseName;
    private TextEdit txtDatabaseName;
    private LabelControl lblExecutionSectionTitle;
    private SeparatorControl sepExecutionSection;
    private LabelControl lblIsActive;
    private ToggleSwitch swIsActive;
    private LabelControl lblBatchSize;
    private SpinEdit sedBatchSize;
    private LabelControl lblMaxRetries;
    private SpinEdit sedMaxRetries;
    private LabelControl lblLastSynchronization;
    private TextEdit txtLastSynchronization;
    private NuanActionButton btnCancel;
    private NuanActionButton btnAdd;

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
        components = new System.ComponentModel.Container();
        lblBranchSectionTitle = new LabelControl();
        sepBranchSection = new SeparatorControl();
        lblBranchCompany = new LabelControl();
        lueBranchCompany = new NuanLookupEdit();
        lblBranchCode = new LabelControl();
        txtBranchCode = new TextEdit();
        lblBranchName = new LabelControl();
        txtBranchName = new TextEdit();
        lblDatabaseName = new LabelControl();
        txtDatabaseName = new TextEdit();
        lblExecutionSectionTitle = new LabelControl();
        sepExecutionSection = new SeparatorControl();
        lblIsActive = new LabelControl();
        swIsActive = new ToggleSwitch();
        lblBatchSize = new LabelControl();
        sedBatchSize = new SpinEdit();
        lblMaxRetries = new LabelControl();
        sedMaxRetries = new SpinEdit();
        lblLastSynchronization = new LabelControl();
        txtLastSynchronization = new TextEdit();
        btnCancel = new NuanActionButton();
        btnAdd = new NuanActionButton();
        ((System.ComponentModel.ISupportInitialize)sepBranchSection).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBranchCompany.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBranchCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBranchName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDatabaseName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sepExecutionSection).BeginInit();
        ((System.ComponentModel.ISupportInitialize)swIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedBatchSize.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedMaxRetries.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtLastSynchronization.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblBranchSectionTitle
        // 
        lblBranchSectionTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblBranchSectionTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblBranchSectionTitle.Appearance.Options.UseFont = true;
        lblBranchSectionTitle.Appearance.Options.UseForeColor = true;
        lblBranchSectionTitle.Location = new Point(12, 12);
        lblBranchSectionTitle.Name = "lblBranchSectionTitle";
        lblBranchSectionTitle.Size = new Size(135, 20);
        lblBranchSectionTitle.TabIndex = 2;
        lblBranchSectionTitle.Text = "Datos de la sucursal";
        // 
        // sepBranchSection
        // 
        sepBranchSection.LineColor = Color.FromArgb(0, 184, 148);
        sepBranchSection.Location = new Point(12, 36);
        sepBranchSection.Name = "sepBranchSection";
        sepBranchSection.Size = new Size(596, 12);
        sepBranchSection.TabIndex = 3;
        // 
        // lblBranchCompany
        // 
        lblBranchCompany.Appearance.Font = new Font("Segoe UI", 9F);
        lblBranchCompany.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBranchCompany.Appearance.Options.UseFont = true;
        lblBranchCompany.Appearance.Options.UseForeColor = true;
        lblBranchCompany.Location = new Point(12, 66);
        lblBranchCompany.Name = "lblBranchCompany";
        lblBranchCompany.Size = new Size(91, 15);
        lblBranchCompany.TabIndex = 4;
        lblBranchCompany.Text = "Empresa sucursal";
        // 
        // lueBranchCompany
        // 
        lueBranchCompany.Location = new Point(189, 63);
        lueBranchCompany.Name = "lueBranchCompany";
        lueBranchCompany.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBranchCompany.Properties.Appearance.Options.UseFont = true;
        lueBranchCompany.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lueBranchCompany.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus) });
        lueBranchCompany.Properties.NullText = "";
        lueBranchCompany.Properties.SearchMode = SearchMode.AutoSearch;
        lueBranchCompany.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueBranchCompany.Size = new Size(346, 22);
        lueBranchCompany.TabIndex = 5;
        // 
        // lblBranchCode
        // 
        lblBranchCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblBranchCode.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBranchCode.Appearance.Options.UseFont = true;
        lblBranchCode.Appearance.Options.UseForeColor = true;
        lblBranchCode.Location = new Point(12, 94);
        lblBranchCode.Name = "lblBranchCode";
        lblBranchCode.Size = new Size(85, 15);
        lblBranchCode.TabIndex = 6;
        lblBranchCode.Text = "Codigo sucursal";
        // 
        // txtBranchCode
        // 
        txtBranchCode.EditValue = "SUC-NTE";
        txtBranchCode.Location = new Point(189, 91);
        txtBranchCode.Name = "txtBranchCode";
        txtBranchCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBranchCode.Properties.Appearance.Options.UseFont = true;
        txtBranchCode.Properties.ReadOnly = true;
        txtBranchCode.Size = new Size(346, 22);
        txtBranchCode.TabIndex = 7;
        // 
        // lblBranchName
        // 
        lblBranchName.Appearance.Font = new Font("Segoe UI", 9F);
        lblBranchName.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBranchName.Appearance.Options.UseFont = true;
        lblBranchName.Appearance.Options.UseForeColor = true;
        lblBranchName.Location = new Point(12, 122);
        lblBranchName.Name = "lblBranchName";
        lblBranchName.Size = new Size(90, 15);
        lblBranchName.TabIndex = 8;
        lblBranchName.Text = "Nombre sucursal";
        // 
        // txtBranchName
        // 
        txtBranchName.EditValue = "Sucursal Norte";
        txtBranchName.Location = new Point(189, 119);
        txtBranchName.Name = "txtBranchName";
        txtBranchName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBranchName.Properties.Appearance.Options.UseFont = true;
        txtBranchName.Properties.ReadOnly = true;
        txtBranchName.Size = new Size(346, 22);
        txtBranchName.TabIndex = 9;
        // 
        // lblDatabaseName
        // 
        lblDatabaseName.Appearance.Font = new Font("Segoe UI", 9F);
        lblDatabaseName.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDatabaseName.Appearance.Options.UseFont = true;
        lblDatabaseName.Appearance.Options.UseForeColor = true;
        lblDatabaseName.Location = new Point(12, 150);
        lblDatabaseName.Name = "lblDatabaseName";
        lblDatabaseName.Size = new Size(72, 15);
        lblDatabaseName.TabIndex = 10;
        lblDatabaseName.Text = "Base de datos";
        // 
        // txtDatabaseName
        // 
        txtDatabaseName.EditValue = "NUA_NORTE";
        txtDatabaseName.Location = new Point(189, 147);
        txtDatabaseName.Name = "txtDatabaseName";
        txtDatabaseName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDatabaseName.Properties.Appearance.Options.UseFont = true;
        txtDatabaseName.Properties.ReadOnly = true;
        txtDatabaseName.Size = new Size(346, 22);
        txtDatabaseName.TabIndex = 11;
        // 
        // lblExecutionSectionTitle
        // 
        lblExecutionSectionTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblExecutionSectionTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblExecutionSectionTitle.Appearance.Options.UseFont = true;
        lblExecutionSectionTitle.Appearance.Options.UseForeColor = true;
        lblExecutionSectionTitle.Location = new Point(12, 201);
        lblExecutionSectionTitle.Name = "lblExecutionSectionTitle";
        lblExecutionSectionTitle.Size = new Size(168, 20);
        lblExecutionSectionTitle.TabIndex = 12;
        lblExecutionSectionTitle.Text = "Parametros de ejecucion";
        // 
        // sepExecutionSection
        // 
        sepExecutionSection.LineColor = Color.FromArgb(0, 184, 148);
        sepExecutionSection.Location = new Point(12, 225);
        sepExecutionSection.Name = "sepExecutionSection";
        sepExecutionSection.Size = new Size(596, 12);
        sepExecutionSection.TabIndex = 13;
        // 
        // lblIsActive
        // 
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(12, 255);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Size = new Size(80, 15);
        lblIsActive.TabIndex = 14;
        lblIsActive.Text = "Activo en perfil";
        // 
        // swIsActive
        // 
        swIsActive.EditValue = true;
        swIsActive.Location = new Point(189, 253);
        swIsActive.Name = "swIsActive";
        swIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        swIsActive.Properties.Appearance.Options.UseFont = true;
        swIsActive.Properties.OffText = "Inactivo";
        swIsActive.Properties.OnText = "Activo";
        swIsActive.Size = new Size(134, 20);
        swIsActive.TabIndex = 15;
        // 
        // lblBatchSize
        // 
        lblBatchSize.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchSize.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBatchSize.Appearance.Options.UseFont = true;
        lblBatchSize.Appearance.Options.UseForeColor = true;
        lblBatchSize.Location = new Point(12, 282);
        lblBatchSize.Name = "lblBatchSize";
        lblBatchSize.Size = new Size(30, 15);
        lblBatchSize.TabIndex = 16;
        lblBatchSize.Text = "Batch";
        // 
        // sedBatchSize
        // 
        sedBatchSize.EditValue = new decimal(new int[] { 500, 0, 0, 0 });
        sedBatchSize.Location = new Point(189, 279);
        sedBatchSize.Name = "sedBatchSize";
        sedBatchSize.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sedBatchSize.Properties.Appearance.Options.UseFont = true;
        sedBatchSize.Properties.Appearance.Options.UseTextOptions = true;
        sedBatchSize.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedBatchSize.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sedBatchSize.Properties.IsFloatValue = false;
        sedBatchSize.Properties.MaskSettings.Set("mask", "N00");
        sedBatchSize.Properties.MaxValue = new decimal(new int[] { 10000, 0, 0, 0 });
        sedBatchSize.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedBatchSize.Size = new Size(118, 22);
        sedBatchSize.TabIndex = 17;
        // 
        // lblMaxRetries
        // 
        lblMaxRetries.Appearance.Font = new Font("Segoe UI", 9F);
        lblMaxRetries.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblMaxRetries.Appearance.Options.UseFont = true;
        lblMaxRetries.Appearance.Options.UseForeColor = true;
        lblMaxRetries.Location = new Point(12, 310);
        lblMaxRetries.Name = "lblMaxRetries";
        lblMaxRetries.Size = new Size(56, 15);
        lblMaxRetries.TabIndex = 18;
        lblMaxRetries.Text = "Reintentos";
        // 
        // sedMaxRetries
        // 
        sedMaxRetries.EditValue = new decimal(new int[] { 3, 0, 0, 0 });
        sedMaxRetries.Location = new Point(189, 307);
        sedMaxRetries.Name = "sedMaxRetries";
        sedMaxRetries.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sedMaxRetries.Properties.Appearance.Options.UseFont = true;
        sedMaxRetries.Properties.Appearance.Options.UseTextOptions = true;
        sedMaxRetries.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedMaxRetries.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sedMaxRetries.Properties.IsFloatValue = false;
        sedMaxRetries.Properties.MaskSettings.Set("mask", "N00");
        sedMaxRetries.Properties.MaxValue = new decimal(new int[] { 10, 0, 0, 0 });
        sedMaxRetries.Size = new Size(118, 22);
        sedMaxRetries.TabIndex = 19;
        // 
        // lblLastSynchronization
        // 
        lblLastSynchronization.Appearance.Font = new Font("Segoe UI", 9F);
        lblLastSynchronization.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblLastSynchronization.Appearance.Options.UseFont = true;
        lblLastSynchronization.Appearance.Options.UseForeColor = true;
        lblLastSynchronization.Location = new Point(12, 338);
        lblLastSynchronization.Name = "lblLastSynchronization";
        lblLastSynchronization.Size = new Size(114, 15);
        lblLastSynchronization.TabIndex = 20;
        lblLastSynchronization.Text = "Ultima sincronizacion";
        // 
        // txtLastSynchronization
        // 
        txtLastSynchronization.EditValue = "2026-07-13 22:41";
        txtLastSynchronization.Location = new Point(189, 335);
        txtLastSynchronization.Name = "txtLastSynchronization";
        txtLastSynchronization.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtLastSynchronization.Properties.Appearance.Options.UseFont = true;
        txtLastSynchronization.Properties.ReadOnly = true;
        txtLastSynchronization.Size = new Size(160, 22);
        txtLastSynchronization.TabIndex = 21;
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
        btnCancel.Location = new Point(329, 365);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 24;
        btnCancel.Text = "Cancelar";
        btnCancel.UseDefaultSize = true;
        // 
        // btnAdd
        // 
        btnAdd.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnAdd.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnAdd.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAdd.Appearance.ForeColor = Color.White;
        btnAdd.Appearance.Options.UseBackColor = true;
        btnAdd.Appearance.Options.UseBorderColor = true;
        btnAdd.Appearance.Options.UseFont = true;
        btnAdd.Appearance.Options.UseForeColor = true;
        btnAdd.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnAdd.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnAdd.AppearanceHovered.ForeColor = Color.White;
        btnAdd.AppearanceHovered.Options.UseBackColor = true;
        btnAdd.AppearanceHovered.Options.UseBorderColor = true;
        btnAdd.AppearanceHovered.Options.UseForeColor = true;
        btnAdd.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnAdd.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnAdd.AppearancePressed.ForeColor = Color.White;
        btnAdd.AppearancePressed.Options.UseBackColor = true;
        btnAdd.AppearancePressed.Options.UseBorderColor = true;
        btnAdd.AppearancePressed.Options.UseForeColor = true;
        btnAdd.ButtonKind = NuanActionButtonKind.Save;
        btnAdd.ButtonStyle = BorderStyles.UltraFlat;
        btnAdd.ButtonText = "Agregar";
        btnAdd.DialogResult = DialogResult.OK;
        btnAdd.IconNameOverride = "agregar_16.svg";
        btnAdd.IconSize = 16;
        btnAdd.ImageOptions.ImageToTextIndent = 0;
        btnAdd.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnAdd.ImageOptions.SvgImageSize = new Size(16, 16);
        btnAdd.Location = new Point(435, 365);
        btnAdd.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAdd.LookAndFeel.UseDefaultLookAndFeel = false;
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(100, 36);
        btnAdd.TabIndex = 25;
        btnAdd.Text = "Agregar";
        btnAdd.UseDefaultSize = true;
        // 
        // SyncProfileBranchDialog
        // 
        AcceptButton = btnAdd;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(554, 412);
        Controls.Add(lblBranchSectionTitle);
        Controls.Add(sepBranchSection);
        Controls.Add(lblBranchCompany);
        Controls.Add(lueBranchCompany);
        Controls.Add(lblBranchCode);
        Controls.Add(txtBranchCode);
        Controls.Add(lblBranchName);
        Controls.Add(txtBranchName);
        Controls.Add(lblDatabaseName);
        Controls.Add(txtDatabaseName);
        Controls.Add(lblExecutionSectionTitle);
        Controls.Add(sepExecutionSection);
        Controls.Add(lblIsActive);
        Controls.Add(swIsActive);
        Controls.Add(lblBatchSize);
        Controls.Add(sedBatchSize);
        Controls.Add(lblMaxRetries);
        Controls.Add(sedMaxRetries);
        Controls.Add(lblLastSynchronization);
        Controls.Add(txtLastSynchronization);
        Controls.Add(btnCancel);
        Controls.Add(btnAdd);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SyncProfileBranchDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Sucursal del perfil";
        ((System.ComponentModel.ISupportInitialize)sepBranchSection).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBranchCompany.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBranchCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBranchName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDatabaseName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sepExecutionSection).EndInit();
        ((System.ComponentModel.ISupportInitialize)swIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedBatchSize.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedMaxRetries.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtLastSynchronization.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
