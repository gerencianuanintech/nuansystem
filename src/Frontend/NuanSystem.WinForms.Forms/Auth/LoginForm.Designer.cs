using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Auth;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
        behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(components);
        picLogo = new PictureEdit();
        btnEstadoApi = new SimpleButton();
        lblSeparator = new LabelControl();
        lblStep = new LabelControl();
        lblTitle = new LabelControl();
        lblSubtitle = new LabelControl();
        lblUsuario = new LabelControl();
        txtUsuario = new TextEdit();
        lblPassword = new LabelControl();
        txtPassword = new TextEdit();
        lblEmpresa = new LabelControl();
        lueEmpresa = new LookUpEdit();
        btnContinuar = new SimpleButton();
        btnCambiarUsuario = new SimpleButton();
        lblStatus = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)behaviorManager1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picLogo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtUsuario.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPassword.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueEmpresa.Properties).BeginInit();
        SuspendLayout();
        // 
        // picLogo
        // 
        picLogo.EditValue = resources.GetObject("picLogo.EditValue");
        picLogo.Location = new Point(36, 12);
        picLogo.Name = "picLogo";
        picLogo.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        picLogo.Properties.SizeMode = PictureSizeMode.Stretch;
        picLogo.Size = new Size(328, 70);
        picLogo.TabIndex = 24;
        // 
        // btnEstadoApi
        // 
        btnEstadoApi.Appearance.BackColor = Color.FromArgb(235, 235, 225);
        btnEstadoApi.Appearance.Font = new Font("Segoe UI", 8.5F);
        btnEstadoApi.Appearance.ForeColor = Color.FromArgb(110, 108, 100);
        btnEstadoApi.Appearance.Options.UseBackColor = true;
        btnEstadoApi.Appearance.Options.UseFont = true;
        btnEstadoApi.Appearance.Options.UseForeColor = true;
        btnEstadoApi.Location = new Point(266, 106);
        btnEstadoApi.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnEstadoApi.LookAndFeel.UseDefaultLookAndFeel = false;
        btnEstadoApi.Name = "btnEstadoApi";
        btnEstadoApi.Size = new Size(98, 25);
        btnEstadoApi.TabIndex = 13;
        btnEstadoApi.Text = "Diseno";
        // 
        // lblSeparator
        // 
        lblSeparator.AutoSizeMode = LabelAutoSizeMode.None;
        lblSeparator.LineColor = Color.FromArgb(220, 220, 210);
        lblSeparator.LineOrientation = LabelLineOrientation.Horizontal;
        lblSeparator.LineVisible = true;
        lblSeparator.Location = new Point(35, 81);
        lblSeparator.Name = "lblSeparator";
        lblSeparator.Size = new Size(318, 1);
        lblSeparator.TabIndex = 14;
        // 
        // lblStep
        // 
        lblStep.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblStep.Appearance.ForeColor = Color.FromArgb(150, 148, 140);
        lblStep.Appearance.Options.UseFont = true;
        lblStep.Appearance.Options.UseForeColor = true;
        lblStep.AutoSizeMode = LabelAutoSizeMode.None;
        lblStep.Location = new Point(35, 96);
        lblStep.Name = "lblStep";
        lblStep.Size = new Size(318, 18);
        lblStep.TabIndex = 15;
        lblStep.Text = "PASO 1 DE 2 - AUTENTICACION";
        // 
        // lblTitle
        // 
        lblTitle.Appearance.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.Appearance.ForeColor = Color.FromArgb(30, 30, 28);
        lblTitle.Appearance.Options.UseFont = true;
        lblTitle.Appearance.Options.UseForeColor = true;
        lblTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lblTitle.Location = new Point(35, 123);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(318, 30);
        lblTitle.TabIndex = 16;
        lblTitle.Text = "Bienvenido";
        // 
        // lblSubtitle
        // 
        lblSubtitle.Appearance.Font = new Font("Segoe UI", 10F);
        lblSubtitle.Appearance.ForeColor = Color.FromArgb(110, 108, 100);
        lblSubtitle.Appearance.Options.UseFont = true;
        lblSubtitle.Appearance.Options.UseForeColor = true;
        lblSubtitle.AutoSizeMode = LabelAutoSizeMode.None;
        lblSubtitle.Location = new Point(37, 155);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(318, 22);
        lblSubtitle.TabIndex = 17;
        lblSubtitle.Text = "Ingresa tus credenciales para continuar.";
        // 
        // lblUsuario
        // 
        lblUsuario.Appearance.Font = new Font("Segoe UI", 9F);
        lblUsuario.Appearance.ForeColor = Color.FromArgb(110, 108, 100);
        lblUsuario.Appearance.Options.UseFont = true;
        lblUsuario.Appearance.Options.UseForeColor = true;
        lblUsuario.AutoSizeMode = LabelAutoSizeMode.None;
        lblUsuario.Location = new Point(37, 195);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Size = new Size(318, 18);
        lblUsuario.TabIndex = 18;
        lblUsuario.Text = "Usuario o correo";
        // 
        // txtUsuario
        // 
        txtUsuario.Location = new Point(37, 216);
        txtUsuario.Name = "txtUsuario";
        txtUsuario.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtUsuario.Properties.Appearance.Options.UseFont = true;
        txtUsuario.Properties.BorderStyle = BorderStyles.HotFlat;
        txtUsuario.Properties.NullValuePrompt = "admin";
        txtUsuario.Size = new Size(318, 24);
        txtUsuario.TabIndex = 19;
        // 
        // lblPassword
        // 
        lblPassword.Appearance.Font = new Font("Segoe UI", 9F);
        lblPassword.Appearance.ForeColor = Color.FromArgb(110, 108, 100);
        lblPassword.Appearance.Options.UseFont = true;
        lblPassword.Appearance.Options.UseForeColor = true;
        lblPassword.AutoSizeMode = LabelAutoSizeMode.None;
        lblPassword.Location = new Point(37, 257);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(318, 18);
        lblPassword.TabIndex = 20;
        lblPassword.Text = "Contrasena";
        // 
        // txtPassword
        // 
        txtPassword.Location = new Point(37, 278);
        txtPassword.Name = "txtPassword";
        txtPassword.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPassword.Properties.Appearance.Options.UseFont = true;
        txtPassword.Properties.BorderStyle = BorderStyles.HotFlat;
        txtPassword.Properties.NullValuePrompt = "********";
        txtPassword.Properties.PasswordChar = '*';
        txtPassword.Size = new Size(318, 24);
        txtPassword.TabIndex = 21;
        // 
        // lblEmpresa
        // 
        lblEmpresa.Appearance.Font = new Font("Segoe UI", 9F);
        lblEmpresa.Appearance.ForeColor = Color.FromArgb(110, 108, 100);
        lblEmpresa.Appearance.Options.UseFont = true;
        lblEmpresa.Appearance.Options.UseForeColor = true;
        lblEmpresa.AutoSizeMode = LabelAutoSizeMode.None;
        lblEmpresa.Location = new Point(37, 318);
        lblEmpresa.Name = "lblEmpresa";
        lblEmpresa.Size = new Size(318, 18);
        lblEmpresa.TabIndex = 25;
        lblEmpresa.Text = "Empresa";
        lblEmpresa.Visible = false;
        // 
        // lueEmpresa
        // 
        lueEmpresa.Location = new Point(37, 339);
        lueEmpresa.Name = "lueEmpresa";
        lueEmpresa.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueEmpresa.Properties.Appearance.Options.UseFont = true;
        lueEmpresa.Properties.BorderStyle = BorderStyles.HotFlat;
        lueEmpresa.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueEmpresa.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("CommercialName", "Nombre comercial") });
        lueEmpresa.Properties.DisplayMember = "CommercialName";
        lueEmpresa.Properties.NullText = "Seleccione una empresa";
        lueEmpresa.Properties.ShowHeader = false;
        lueEmpresa.Properties.ValueMember = "Id";
        lueEmpresa.Size = new Size(318, 24);
        lueEmpresa.TabIndex = 26;
        lueEmpresa.Visible = false;
        // 
        // btnContinuar
        // 
        btnContinuar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnContinuar.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnContinuar.Appearance.ForeColor = Color.White;
        btnContinuar.Appearance.Options.UseBackColor = true;
        btnContinuar.Appearance.Options.UseFont = true;
        btnContinuar.Appearance.Options.UseForeColor = true;
        btnContinuar.AppearanceHovered.BackColor = Color.FromArgb(0, 161, 132);
        btnContinuar.AppearanceHovered.ForeColor = Color.White;
        btnContinuar.AppearanceHovered.Options.UseBackColor = true;
        btnContinuar.AppearanceHovered.Options.UseForeColor = true;
        btnContinuar.AppearancePressed.BackColor = Color.FromArgb(0, 141, 116);
        btnContinuar.AppearancePressed.ForeColor = Color.White;
        btnContinuar.AppearancePressed.Options.UseBackColor = true;
        btnContinuar.AppearancePressed.Options.UseForeColor = true;
        btnContinuar.Location = new Point(37, 335);
        btnContinuar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnContinuar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnContinuar.Name = "btnContinuar";
        btnContinuar.Size = new Size(318, 40);
        btnContinuar.TabIndex = 22;
        btnContinuar.Text = "Continuar";
        // 
        // btnCambiarUsuario
        // 
        btnCambiarUsuario.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        btnCambiarUsuario.Appearance.Options.UseForeColor = true;
        btnCambiarUsuario.Location = new Point(37, 431);
        btnCambiarUsuario.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCambiarUsuario.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCambiarUsuario.Name = "btnCambiarUsuario";
        btnCambiarUsuario.Size = new Size(318, 28);
        btnCambiarUsuario.TabIndex = 27;
        btnCambiarUsuario.Text = "Cambiar usuario";
        btnCambiarUsuario.Visible = false;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.ForeColor = Color.FromArgb(110, 108, 100);
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
        lblStatus.Location = new Point(37, 391);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(318, 42);
        lblStatus.TabIndex = 23;
        // 
        // LoginForm
        // 
        Appearance.Options.UseFont = true;
        ClientSize = new Size(401, 524);
        Controls.Add(picLogo);
        Controls.Add(btnEstadoApi);
        Controls.Add(lblSeparator);
        Controls.Add(lblStep);
        Controls.Add(lblTitle);
        Controls.Add(lblSubtitle);
        Controls.Add(lblUsuario);
        Controls.Add(txtUsuario);
        Controls.Add(lblPassword);
        Controls.Add(txtPassword);
        Controls.Add(lblEmpresa);
        Controls.Add(lueEmpresa);
        Controls.Add(btnContinuar);
        Controls.Add(btnCambiarUsuario);
        Controls.Add(lblStatus);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "NuanSystem - Iniciar sesion";
        ((System.ComponentModel.ISupportInitialize)behaviorManager1).EndInit();
        ((System.ComponentModel.ISupportInitialize)picLogo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtUsuario.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPassword.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueEmpresa.Properties).EndInit();
        ResumeLayout(false);
    }
    private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
    private PictureEdit picLogo;
    private SimpleButton btnEstadoApi;
    private LabelControl lblSeparator;
    private LabelControl lblStep;
    private LabelControl lblTitle;
    private LabelControl lblSubtitle;
    private LabelControl lblUsuario;
    private TextEdit txtUsuario;
    private LabelControl lblPassword;
    private TextEdit txtPassword;
    private LabelControl lblEmpresa;
    private LookUpEdit lueEmpresa;
    private SimpleButton btnContinuar;
    private SimpleButton btnCambiarUsuario;
    private LabelControl lblStatus;
}



