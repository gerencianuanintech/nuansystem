using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sap;

partial class SapIntegrationSettingsForm
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl lblConnectionTitle, lblOptionsTitle, lblCompany, lblCompanyValue, lblServiceLayerUrl, lblSapCompanyDb, lblSapUser, lblSapPassword, lblMaxRetryCount, statusLabel;
    private LabelControl lineConnection, lineOptions;
    private TextEdit txtServiceLayerUrl, txtSapCompanyDb, txtSapUser, txtSapPassword;
    private SpinEdit spnMaxRetryCount;
    private NuanToggleSwitch tglIsEnabled;
    private SimpleButton btnIdentityPolicy, btnTestConnection;

    private void InitializeComponent()
    {
        lblConnectionTitle = new LabelControl();
        lineConnection = new LabelControl();
        lblOptionsTitle = new LabelControl();
        lineOptions = new LabelControl();
        lblCompany = new LabelControl();
        lblCompanyValue = new LabelControl();
        lblServiceLayerUrl = new LabelControl();
        lblSapCompanyDb = new LabelControl();
        lblSapUser = new LabelControl();
        lblSapPassword = new LabelControl();
        lblMaxRetryCount = new LabelControl();
        statusLabel = new LabelControl();
        txtServiceLayerUrl = new TextEdit();
        txtSapCompanyDb = new TextEdit();
        txtSapUser = new TextEdit();
        txtSapPassword = new TextEdit();
        spnMaxRetryCount = new SpinEdit();
        tglIsEnabled = new NuanToggleSwitch();
        btnIdentityPolicy = new SimpleButton();
        btnTestConnection = new SimpleButton();
        lblEnabled = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)txtServiceLayerUrl.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCompanyDb.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapPassword.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxRetryCount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglIsEnabled.Properties).BeginInit();
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
        btnCancelar.Location = new Point(628, 300);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
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
        btnGuardar.Location = new Point(734, 300);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // lblConnectionTitle
        // 
        lblConnectionTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblConnectionTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblConnectionTitle.Appearance.Options.UseFont = true;
        lblConnectionTitle.Appearance.Options.UseForeColor = true;
        lblConnectionTitle.Location = new Point(32, 22);
        lblConnectionTitle.Name = "lblConnectionTitle";
        lblConnectionTitle.Size = new Size(110, 20);
        lblConnectionTitle.TabIndex = 2;
        lblConnectionTitle.Text = "1. Conexión SAP";
        // 
        // lineConnection
        // 
        lineConnection.Appearance.BackColor = Color.FromArgb(221, 226, 240);
        lineConnection.Appearance.Options.UseBackColor = true;
        lineConnection.AutoSizeMode = LabelAutoSizeMode.None;
        lineConnection.Location = new Point(220, 34);
        lineConnection.Name = "lineConnection";
        lineConnection.Size = new Size(618, 1);
        lineConnection.TabIndex = 3;
        // 
        // lblOptionsTitle
        // 
        lblOptionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblOptionsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblOptionsTitle.Appearance.Options.UseFont = true;
        lblOptionsTitle.Appearance.Options.UseForeColor = true;
        lblOptionsTitle.Location = new Point(32, 190);
        lblOptionsTitle.Name = "lblOptionsTitle";
        lblOptionsTitle.Size = new Size(149, 20);
        lblOptionsTitle.TabIndex = 4;
        lblOptionsTitle.Text = "2. Opciones generales";
        // 
        // lineOptions
        // 
        lineOptions.Appearance.BackColor = Color.FromArgb(221, 226, 240);
        lineOptions.Appearance.Options.UseBackColor = true;
        lineOptions.AutoSizeMode = LabelAutoSizeMode.None;
        lineOptions.Location = new Point(208, 202);
        lineOptions.Name = "lineOptions";
        lineOptions.Size = new Size(630, 1);
        lineOptions.TabIndex = 5;
        // 
        // lblCompany
        // 
        lblCompany.Appearance.Font = new Font("Segoe UI", 9F);
        lblCompany.Appearance.Options.UseFont = true;
        lblCompany.Location = new Point(32, 63);
        lblCompany.Name = "lblCompany";
        lblCompany.Size = new Size(48, 15);
        lblCompany.TabIndex = 6;
        lblCompany.Text = "Empresa:";
        // 
        // lblCompanyValue
        // 
        lblCompanyValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblCompanyValue.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblCompanyValue.Appearance.Options.UseFont = true;
        lblCompanyValue.Appearance.Options.UseForeColor = true;
        lblCompanyValue.Location = new Point(154, 63);
        lblCompanyValue.Name = "lblCompanyValue";
        lblCompanyValue.Size = new Size(79, 15);
        lblCompanyValue.TabIndex = 7;
        lblCompanyValue.Text = "Empresa activa";
        // 
        // lblServiceLayerUrl
        // 
        lblServiceLayerUrl.Appearance.Font = new Font("Segoe UI", 9F);
        lblServiceLayerUrl.Appearance.Options.UseFont = true;
        lblServiceLayerUrl.Location = new Point(32, 91);
        lblServiceLayerUrl.Name = "lblServiceLayerUrl";
        lblServiceLayerUrl.Size = new Size(95, 15);
        lblServiceLayerUrl.TabIndex = 8;
        lblServiceLayerUrl.Text = "URL Service Layer:";
        // 
        // lblSapCompanyDb
        // 
        lblSapCompanyDb.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapCompanyDb.Appearance.Options.UseFont = true;
        lblSapCompanyDb.Location = new Point(32, 119);
        lblSapCompanyDb.Name = "lblSapCompanyDb";
        lblSapCompanyDb.Size = new Size(99, 15);
        lblSapCompanyDb.TabIndex = 9;
        lblSapCompanyDb.Text = "Base de datos SAP:";
        // 
        // lblSapUser
        // 
        lblSapUser.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapUser.Appearance.Options.UseFont = true;
        lblSapUser.Location = new Point(450, 91);
        lblSapUser.Name = "lblSapUser";
        lblSapUser.Size = new Size(85, 15);
        lblSapUser.TabIndex = 10;
        lblSapUser.Text = "Usuario técnico:";
        // 
        // lblSapPassword
        // 
        lblSapPassword.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapPassword.Appearance.Options.UseFont = true;
        lblSapPassword.Location = new Point(450, 119);
        lblSapPassword.Name = "lblSapPassword";
        lblSapPassword.Size = new Size(63, 15);
        lblSapPassword.TabIndex = 11;
        lblSapPassword.Text = "Contraseña:";
        // 
        // lblMaxRetryCount
        // 
        lblMaxRetryCount.Appearance.Font = new Font("Segoe UI", 9F);
        lblMaxRetryCount.Appearance.Options.UseFont = true;
        lblMaxRetryCount.Location = new Point(32, 231);
        lblMaxRetryCount.Name = "lblMaxRetryCount";
        lblMaxRetryCount.Size = new Size(59, 15);
        lblMaxRetryCount.TabIndex = 13;
        lblMaxRetryCount.Text = "Reintentos:";
        // 
        // statusLabel
        // 
        statusLabel.Appearance.Font = new Font("Segoe UI", 8.5F);
        statusLabel.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        statusLabel.Appearance.Options.UseFont = true;
        statusLabel.Appearance.Options.UseForeColor = true;
        statusLabel.Location = new Point(300, 235);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(73, 13);
        statusLabel.TabIndex = 14;
        statusLabel.Text = "Sin configurar";
        // 
        // txtServiceLayerUrl
        // 
        txtServiceLayerUrl.Location = new Point(154, 88);
        txtServiceLayerUrl.Name = "txtServiceLayerUrl";
        txtServiceLayerUrl.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtServiceLayerUrl.Properties.Appearance.Options.UseFont = true;
        txtServiceLayerUrl.Properties.AutoHeight = false;
        txtServiceLayerUrl.Properties.MaxLength = 500;
        txtServiceLayerUrl.Size = new Size(250, 22);
        txtServiceLayerUrl.TabIndex = 0;
        // 
        // txtSapCompanyDb
        // 
        txtSapCompanyDb.Location = new Point(154, 116);
        txtSapCompanyDb.Name = "txtSapCompanyDb";
        txtSapCompanyDb.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapCompanyDb.Properties.Appearance.Options.UseFont = true;
        txtSapCompanyDb.Properties.AutoHeight = false;
        txtSapCompanyDb.Properties.MaxLength = 100;
        txtSapCompanyDb.Size = new Size(250, 22);
        txtSapCompanyDb.TabIndex = 1;
        // 
        // txtSapUser
        // 
        txtSapUser.Location = new Point(572, 88);
        txtSapUser.Name = "txtSapUser";
        txtSapUser.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapUser.Properties.Appearance.Options.UseFont = true;
        txtSapUser.Properties.AutoHeight = false;
        txtSapUser.Properties.MaxLength = 100;
        txtSapUser.Size = new Size(180, 22);
        txtSapUser.TabIndex = 2;
        // 
        // txtSapPassword
        // 
        txtSapPassword.Location = new Point(572, 116);
        txtSapPassword.Name = "txtSapPassword";
        txtSapPassword.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapPassword.Properties.Appearance.Options.UseFont = true;
        txtSapPassword.Properties.AutoHeight = false;
        txtSapPassword.Properties.PasswordChar = '●';
        txtSapPassword.Size = new Size(180, 22);
        txtSapPassword.TabIndex = 3;
        // 
        // spnMaxRetryCount
        // 
        spnMaxRetryCount.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMaxRetryCount.Location = new Point(154, 228);
        spnMaxRetryCount.Name = "spnMaxRetryCount";
        spnMaxRetryCount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMaxRetryCount.Properties.Appearance.Options.UseFont = true;
        spnMaxRetryCount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMaxRetryCount.Properties.IsFloatValue = false;
        spnMaxRetryCount.Properties.MaxValue = new decimal(new int[] { 20, 0, 0, 0 });
        spnMaxRetryCount.Size = new Size(100, 22);
        spnMaxRetryCount.TabIndex = 5;
        // 
        // tglIsEnabled
        // 
        tglIsEnabled.ActiveColor = Color.FromArgb(0, 184, 148);
        tglIsEnabled.InactiveColor = Color.FromArgb(221, 226, 240);
        tglIsEnabled.Location = new Point(154, 150);
        tglIsEnabled.MinimumSize = new Size(58, 20);
        tglIsEnabled.Name = "tglIsEnabled";
        tglIsEnabled.Properties.OffText = "No";
        tglIsEnabled.Properties.OnText = "Sí";
        tglIsEnabled.Size = new Size(120, 20);
        tglIsEnabled.StateTextColor = Color.FromArgb(23, 32, 51);
        tglIsEnabled.TabIndex = 4;
        // 
        // btnIdentityPolicy
        // 
        btnIdentityPolicy.Location = new Point(32, 270);
        btnIdentityPolicy.Name = "btnIdentityPolicy";
        btnIdentityPolicy.Size = new Size(240, 30);
        btnIdentityPolicy.TabIndex = 6;
        btnIdentityPolicy.Text = "Configurar política de identidad";
        // 
        // btnTestConnection
        // 
        btnTestConnection.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnTestConnection.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnTestConnection.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnTestConnection.Appearance.ForeColor = Color.White;
        btnTestConnection.Appearance.Options.UseBackColor = true;
        btnTestConnection.Appearance.Options.UseBorderColor = true;
        btnTestConnection.Appearance.Options.UseFont = true;
        btnTestConnection.Appearance.Options.UseForeColor = true;
        btnTestConnection.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnTestConnection.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnTestConnection.AppearanceHovered.Options.UseBackColor = true;
        btnTestConnection.AppearanceHovered.Options.UseBorderColor = true;
        btnTestConnection.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnTestConnection.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnTestConnection.AppearancePressed.Options.UseBackColor = true;
        btnTestConnection.AppearancePressed.Options.UseBorderColor = true;
        btnTestConnection.Location = new Point(647, 145);
        btnTestConnection.Name = "btnTestConnection";
        btnTestConnection.Size = new Size(105, 30);
        btnTestConnection.TabIndex = 7;
        btnTestConnection.Text = "Probar conexión";
        // 
        // lblEnabled
        // 
        lblEnabled.Appearance.Font = new Font("Segoe UI", 9F);
        lblEnabled.Appearance.Options.UseFont = true;
        lblEnabled.Location = new Point(0, 0);
        lblEnabled.Name = "lblEnabled";
        lblEnabled.Size = new Size(0, 15);
        lblEnabled.TabIndex = 12;
        // 
        // SapIntegrationSettingsForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButtonLocation = new Point(628, 300);
        ClientSize = new Size(870, 346);
        Controls.Add(lblConnectionTitle);
        Controls.Add(lineConnection);
        Controls.Add(lblOptionsTitle);
        Controls.Add(lineOptions);
        Controls.Add(lblCompany);
        Controls.Add(lblCompanyValue);
        Controls.Add(lblServiceLayerUrl);
        Controls.Add(txtServiceLayerUrl);
        Controls.Add(lblSapCompanyDb);
        Controls.Add(txtSapCompanyDb);
        Controls.Add(lblSapUser);
        Controls.Add(txtSapUser);
        Controls.Add(lblSapPassword);
        Controls.Add(txtSapPassword);
        Controls.Add(lblEnabled);
        Controls.Add(tglIsEnabled);
        Controls.Add(lblMaxRetryCount);
        Controls.Add(spnMaxRetryCount);
        Controls.Add(statusLabel);
        Controls.Add(btnIdentityPolicy);
        Controls.Add(btnTestConnection);
        MinimumSize = new Size(872, 378);
        Name = "SapIntegrationSettingsForm";
        SaveButtonLocation = new Point(734, 300);
        Text = "Configuración general SAP";
        Controls.SetChildIndex(btnTestConnection, 0);
        Controls.SetChildIndex(btnIdentityPolicy, 0);
        Controls.SetChildIndex(statusLabel, 0);
        Controls.SetChildIndex(spnMaxRetryCount, 0);
        Controls.SetChildIndex(lblMaxRetryCount, 0);
        Controls.SetChildIndex(tglIsEnabled, 0);
        Controls.SetChildIndex(lblEnabled, 0);
        Controls.SetChildIndex(txtSapPassword, 0);
        Controls.SetChildIndex(lblSapPassword, 0);
        Controls.SetChildIndex(txtSapUser, 0);
        Controls.SetChildIndex(lblSapUser, 0);
        Controls.SetChildIndex(txtSapCompanyDb, 0);
        Controls.SetChildIndex(lblSapCompanyDb, 0);
        Controls.SetChildIndex(txtServiceLayerUrl, 0);
        Controls.SetChildIndex(lblServiceLayerUrl, 0);
        Controls.SetChildIndex(lblCompanyValue, 0);
        Controls.SetChildIndex(lblCompany, 0);
        Controls.SetChildIndex(lineOptions, 0);
        Controls.SetChildIndex(lblOptionsTitle, 0);
        Controls.SetChildIndex(lineConnection, 0);
        Controls.SetChildIndex(lblConnectionTitle, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtServiceLayerUrl.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCompanyDb.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapPassword.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxRetryCount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglIsEnabled.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

    private LabelControl lblEnabled;
}
