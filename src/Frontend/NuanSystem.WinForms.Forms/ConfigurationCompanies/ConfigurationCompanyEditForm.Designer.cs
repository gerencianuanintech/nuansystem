using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.ConfigurationCompanies;

partial class ConfigurationCompanyEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCodigo = new LabelControl();
        txtCodigo = new TextEdit();
        lblNombreComercial = new LabelControl();
        txtNombreComercial = new TextEdit();
        lblRazonSocial = new LabelControl();
        txtRazonSocial = new TextEdit();
        lblIdentificacion = new LabelControl();
        txtIdentificacion = new TextEdit();
        lblDireccion = new LabelControl();
        memDireccion = new MemoEdit();
        lblTelefono = new LabelControl();
        txtTelefono = new TextEdit();
        lblCorreo = new LabelControl();
        txtCorreo = new TextEdit();
        lblMotor = new LabelControl();
        cmbMotor = new ComboBoxEdit();
        lblServidor = new LabelControl();
        txtServidor = new TextEdit();
        lblPuerto = new LabelControl();
        sedPuerto = new SpinEdit();
        lblBaseDatos = new LabelControl();
        txtBaseDatos = new TextEdit();
        lblUsuarioDb = new LabelControl();
        txtUsuarioDb = new TextEdit();
        lblClaveDb = new LabelControl();
        txtClaveDb = new TextEdit();
        lblSap = new LabelControl();
        cmbSap = new ComboBoxEdit();
        lblOrden = new LabelControl();
        sedOrden = new SpinEdit();
        lblZonaHoraria = new LabelControl();
        txtZonaHoraria = new TextEdit();
        lblCultura = new LabelControl();
        txtCultura = new TextEdit();
        lblMoneda = new LabelControl();
        txtMoneda = new TextEdit();
        chkActivo = new CheckEdit();
        chkPredeterminada = new CheckEdit();
        chkValidarConexion = new CheckEdit();
        lblLogo = new LabelControl();
        picLogo = new PictureEdit();
        btnCargarLogo = new SimpleButton();
        btnQuitarLogo = new SimpleButton();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombreComercial.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtRazonSocial.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificacion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDireccion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTelefono.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCorreo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbMotor.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtServidor.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedPuerto.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBaseDatos.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtUsuarioDb.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtClaveDb.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbSap.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedOrden.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtZonaHoraria.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCultura.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtMoneda.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkPredeterminada.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkValidarConexion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picLogo.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblCodigo
        // 
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = BrandResources.Text;
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblCodigo.Location = new Point(24, 24);
        lblCodigo.Name = "lblCodigo";
        lblCodigo.Size = new Size(39, 15);
        lblCodigo.TabIndex = 0;
        lblCodigo.Text = "Codigo";
        // 
        // txtCodigo
        // 
        txtCodigo.Location = new Point(150, 22);
        txtCodigo.Name = "txtCodigo";
        txtCodigo.Size = new Size(160, 20);
        txtCodigo.TabIndex = 1;
        // 
        // lblNombreComercial
        // 
        lblNombreComercial.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombreComercial.Appearance.ForeColor = BrandResources.Text;
        lblNombreComercial.Appearance.Options.UseFont = true;
        lblNombreComercial.Appearance.Options.UseForeColor = true;
        lblNombreComercial.Location = new Point(24, 52);
        lblNombreComercial.Name = "lblNombreComercial";
        lblNombreComercial.Size = new Size(101, 15);
        lblNombreComercial.TabIndex = 2;
        lblNombreComercial.Text = "Nombre comercial";
        // 
        // txtNombreComercial
        // 
        txtNombreComercial.Location = new Point(150, 50);
        txtNombreComercial.Name = "txtNombreComercial";
        txtNombreComercial.Size = new Size(410, 20);
        txtNombreComercial.TabIndex = 3;
        // 
        // lblRazonSocial
        // 
        lblRazonSocial.Appearance.Font = new Font("Segoe UI", 9F);
        lblRazonSocial.Appearance.ForeColor = BrandResources.Text;
        lblRazonSocial.Appearance.Options.UseFont = true;
        lblRazonSocial.Appearance.Options.UseForeColor = true;
        lblRazonSocial.Location = new Point(24, 80);
        lblRazonSocial.Name = "lblRazonSocial";
        lblRazonSocial.Size = new Size(67, 15);
        lblRazonSocial.TabIndex = 4;
        lblRazonSocial.Text = "Razon social";
        // 
        // txtRazonSocial
        // 
        txtRazonSocial.Location = new Point(150, 78);
        txtRazonSocial.Name = "txtRazonSocial";
        txtRazonSocial.Size = new Size(410, 20);
        txtRazonSocial.TabIndex = 5;
        // 
        // lblIdentificacion
        // 
        lblIdentificacion.Appearance.Font = new Font("Segoe UI", 9F);
        lblIdentificacion.Appearance.ForeColor = BrandResources.Text;
        lblIdentificacion.Appearance.Options.UseFont = true;
        lblIdentificacion.Appearance.Options.UseForeColor = true;
        lblIdentificacion.Location = new Point(24, 108);
        lblIdentificacion.Name = "lblIdentificacion";
        lblIdentificacion.Size = new Size(73, 15);
        lblIdentificacion.TabIndex = 6;
        lblIdentificacion.Text = "Identificacion";
        // 
        // txtIdentificacion
        // 
        txtIdentificacion.Location = new Point(150, 106);
        txtIdentificacion.Name = "txtIdentificacion";
        txtIdentificacion.Size = new Size(180, 20);
        txtIdentificacion.TabIndex = 7;
        // 
        // lblTelefono
        // 
        lblTelefono.Appearance.Font = new Font("Segoe UI", 9F);
        lblTelefono.Appearance.ForeColor = BrandResources.Text;
        lblTelefono.Appearance.Options.UseFont = true;
        lblTelefono.Appearance.Options.UseForeColor = true;
        lblTelefono.Location = new Point(348, 108);
        lblTelefono.Name = "lblTelefono";
        lblTelefono.Size = new Size(48, 15);
        lblTelefono.TabIndex = 8;
        lblTelefono.Text = "Telefono";
        // 
        // txtTelefono
        // 
        txtTelefono.Location = new Point(420, 106);
        txtTelefono.Name = "txtTelefono";
        txtTelefono.Size = new Size(140, 20);
        txtTelefono.TabIndex = 9;
        // 
        // lblCorreo
        // 
        lblCorreo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCorreo.Appearance.ForeColor = BrandResources.Text;
        lblCorreo.Appearance.Options.UseFont = true;
        lblCorreo.Appearance.Options.UseForeColor = true;
        lblCorreo.Location = new Point(24, 136);
        lblCorreo.Name = "lblCorreo";
        lblCorreo.Size = new Size(37, 15);
        lblCorreo.TabIndex = 10;
        lblCorreo.Text = "Correo";
        // 
        // txtCorreo
        // 
        txtCorreo.Location = new Point(150, 134);
        txtCorreo.Name = "txtCorreo";
        txtCorreo.Size = new Size(410, 20);
        txtCorreo.TabIndex = 11;
        // 
        // lblDireccion
        // 
        lblDireccion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDireccion.Appearance.ForeColor = BrandResources.Text;
        lblDireccion.Appearance.Options.UseFont = true;
        lblDireccion.Appearance.Options.UseForeColor = true;
        lblDireccion.Location = new Point(24, 164);
        lblDireccion.Name = "lblDireccion";
        lblDireccion.Size = new Size(50, 15);
        lblDireccion.TabIndex = 12;
        lblDireccion.Text = "Direccion";
        // 
        // memDireccion
        // 
        memDireccion.Location = new Point(150, 162);
        memDireccion.Name = "memDireccion";
        memDireccion.Size = new Size(410, 52);
        memDireccion.TabIndex = 13;
        // 
        // lblMotor
        // 
        lblMotor.Appearance.Font = new Font("Segoe UI", 9F);
        lblMotor.Appearance.ForeColor = BrandResources.Text;
        lblMotor.Appearance.Options.UseFont = true;
        lblMotor.Appearance.Options.UseForeColor = true;
        lblMotor.Location = new Point(24, 228);
        lblMotor.Name = "lblMotor";
        lblMotor.Size = new Size(34, 15);
        lblMotor.TabIndex = 14;
        lblMotor.Text = "Motor";
        // 
        // cmbMotor
        // 
        cmbMotor.Location = new Point(150, 226);
        cmbMotor.Name = "cmbMotor";
        cmbMotor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cmbMotor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cmbMotor.Size = new Size(160, 20);
        cmbMotor.TabIndex = 15;
        // 
        // lblServidor
        // 
        lblServidor.Appearance.Font = new Font("Segoe UI", 9F);
        lblServidor.Appearance.ForeColor = BrandResources.Text;
        lblServidor.Appearance.Options.UseFont = true;
        lblServidor.Appearance.Options.UseForeColor = true;
        lblServidor.Location = new Point(24, 256);
        lblServidor.Name = "lblServidor";
        lblServidor.Size = new Size(46, 15);
        lblServidor.TabIndex = 16;
        lblServidor.Text = "Servidor";
        // 
        // txtServidor
        // 
        txtServidor.Location = new Point(150, 254);
        txtServidor.Name = "txtServidor";
        txtServidor.Size = new Size(410, 20);
        txtServidor.TabIndex = 17;
        // 
        // lblPuerto
        // 
        lblPuerto.Appearance.Font = new Font("Segoe UI", 9F);
        lblPuerto.Appearance.ForeColor = BrandResources.Text;
        lblPuerto.Appearance.Options.UseFont = true;
        lblPuerto.Appearance.Options.UseForeColor = true;
        lblPuerto.Location = new Point(24, 284);
        lblPuerto.Name = "lblPuerto";
        lblPuerto.Size = new Size(37, 15);
        lblPuerto.TabIndex = 18;
        lblPuerto.Text = "Puerto";
        // 
        // sedPuerto
        // 
        sedPuerto.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedPuerto.Location = new Point(150, 282);
        sedPuerto.Name = "sedPuerto";
        sedPuerto.Properties.IsFloatValue = false;
        sedPuerto.Properties.MaskSettings.Set("mask", "N00");
        sedPuerto.Properties.MaxValue = new decimal(new int[] { 65535, 0, 0, 0 });
        sedPuerto.Size = new Size(100, 20);
        sedPuerto.TabIndex = 19;
        // 
        // lblBaseDatos
        // 
        lblBaseDatos.Appearance.Font = new Font("Segoe UI", 9F);
        lblBaseDatos.Appearance.ForeColor = BrandResources.Text;
        lblBaseDatos.Appearance.Options.UseFont = true;
        lblBaseDatos.Appearance.Options.UseForeColor = true;
        lblBaseDatos.Location = new Point(24, 312);
        lblBaseDatos.Name = "lblBaseDatos";
        lblBaseDatos.Size = new Size(73, 15);
        lblBaseDatos.TabIndex = 20;
        lblBaseDatos.Text = "Base de datos";
        // 
        // txtBaseDatos
        // 
        txtBaseDatos.Location = new Point(150, 310);
        txtBaseDatos.Name = "txtBaseDatos";
        txtBaseDatos.Size = new Size(410, 20);
        txtBaseDatos.TabIndex = 21;
        // 
        // lblUsuarioDb
        // 
        lblUsuarioDb.Appearance.Font = new Font("Segoe UI", 9F);
        lblUsuarioDb.Appearance.ForeColor = BrandResources.Text;
        lblUsuarioDb.Appearance.Options.UseFont = true;
        lblUsuarioDb.Appearance.Options.UseForeColor = true;
        lblUsuarioDb.Location = new Point(24, 340);
        lblUsuarioDb.Name = "lblUsuarioDb";
        lblUsuarioDb.Size = new Size(60, 15);
        lblUsuarioDb.TabIndex = 22;
        lblUsuarioDb.Text = "Usuario DB";
        // 
        // txtUsuarioDb
        // 
        txtUsuarioDb.Location = new Point(150, 338);
        txtUsuarioDb.Name = "txtUsuarioDb";
        txtUsuarioDb.Size = new Size(180, 20);
        txtUsuarioDb.TabIndex = 23;
        // 
        // lblClaveDb
        // 
        lblClaveDb.Appearance.Font = new Font("Segoe UI", 9F);
        lblClaveDb.Appearance.ForeColor = BrandResources.Text;
        lblClaveDb.Appearance.Options.UseFont = true;
        lblClaveDb.Appearance.Options.UseForeColor = true;
        lblClaveDb.Location = new Point(348, 340);
        lblClaveDb.Name = "lblClaveDb";
        lblClaveDb.Size = new Size(48, 15);
        lblClaveDb.TabIndex = 24;
        lblClaveDb.Text = "Clave DB";
        // 
        // txtClaveDb
        // 
        txtClaveDb.Location = new Point(420, 338);
        txtClaveDb.Name = "txtClaveDb";
        txtClaveDb.Properties.PasswordChar = '*';
        txtClaveDb.Size = new Size(140, 20);
        txtClaveDb.TabIndex = 25;
        // 
        // lblSap
        // 
        lblSap.Appearance.Font = new Font("Segoe UI", 9F);
        lblSap.Appearance.ForeColor = BrandResources.Text;
        lblSap.Appearance.Options.UseFont = true;
        lblSap.Appearance.Options.UseForeColor = true;
        lblSap.Location = new Point(24, 368);
        lblSap.Name = "lblSap";
        lblSap.Size = new Size(20, 15);
        lblSap.TabIndex = 26;
        lblSap.Text = "SAP";
        // 
        // cmbSap
        // 
        cmbSap.Location = new Point(150, 366);
        cmbSap.Name = "cmbSap";
        cmbSap.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cmbSap.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cmbSap.Size = new Size(180, 20);
        cmbSap.TabIndex = 27;
        // 
        // lblOrden
        // 
        lblOrden.Appearance.Font = new Font("Segoe UI", 9F);
        lblOrden.Appearance.ForeColor = BrandResources.Text;
        lblOrden.Appearance.Options.UseFont = true;
        lblOrden.Appearance.Options.UseForeColor = true;
        lblOrden.Location = new Point(348, 368);
        lblOrden.Name = "lblOrden";
        lblOrden.Size = new Size(33, 15);
        lblOrden.TabIndex = 28;
        lblOrden.Text = "Orden";
        // 
        // sedOrden
        // 
        sedOrden.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedOrden.Location = new Point(420, 366);
        sedOrden.Name = "sedOrden";
        sedOrden.Properties.IsFloatValue = false;
        sedOrden.Properties.MaskSettings.Set("mask", "N00");
        sedOrden.Properties.MaxValue = new decimal(new int[] { 100000, 0, 0, 0 });
        sedOrden.Size = new Size(100, 20);
        sedOrden.TabIndex = 29;
        // 
        // lblZonaHoraria
        // 
        lblZonaHoraria.Appearance.Font = new Font("Segoe UI", 9F);
        lblZonaHoraria.Appearance.ForeColor = BrandResources.Text;
        lblZonaHoraria.Appearance.Options.UseFont = true;
        lblZonaHoraria.Appearance.Options.UseForeColor = true;
        lblZonaHoraria.Location = new Point(24, 396);
        lblZonaHoraria.Name = "lblZonaHoraria";
        lblZonaHoraria.Size = new Size(71, 15);
        lblZonaHoraria.TabIndex = 30;
        lblZonaHoraria.Text = "Zona horaria";
        // 
        // txtZonaHoraria
        // 
        txtZonaHoraria.Location = new Point(150, 394);
        txtZonaHoraria.Name = "txtZonaHoraria";
        txtZonaHoraria.Size = new Size(180, 20);
        txtZonaHoraria.TabIndex = 31;
        // 
        // lblCultura
        // 
        lblCultura.Appearance.Font = new Font("Segoe UI", 9F);
        lblCultura.Appearance.ForeColor = BrandResources.Text;
        lblCultura.Appearance.Options.UseFont = true;
        lblCultura.Appearance.Options.UseForeColor = true;
        lblCultura.Location = new Point(348, 396);
        lblCultura.Name = "lblCultura";
        lblCultura.Size = new Size(39, 15);
        lblCultura.TabIndex = 32;
        lblCultura.Text = "Cultura";
        // 
        // txtCultura
        // 
        txtCultura.Location = new Point(420, 394);
        txtCultura.Name = "txtCultura";
        txtCultura.Size = new Size(140, 20);
        txtCultura.TabIndex = 33;
        // 
        // lblMoneda
        // 
        lblMoneda.Appearance.Font = new Font("Segoe UI", 9F);
        lblMoneda.Appearance.ForeColor = BrandResources.Text;
        lblMoneda.Appearance.Options.UseFont = true;
        lblMoneda.Appearance.Options.UseForeColor = true;
        lblMoneda.Location = new Point(24, 424);
        lblMoneda.Name = "lblMoneda";
        lblMoneda.Size = new Size(45, 15);
        lblMoneda.TabIndex = 34;
        lblMoneda.Text = "Moneda";
        // 
        // txtMoneda
        // 
        txtMoneda.Location = new Point(150, 422);
        txtMoneda.Name = "txtMoneda";
        txtMoneda.Size = new Size(80, 20);
        txtMoneda.TabIndex = 35;
        chkActivo.Location = new Point(150, 454);
        chkActivo.Name = "chkActivo";
        chkActivo.Properties.Caption = "Activo";
        chkActivo.Size = new Size(75, 20);
        chkActivo.TabIndex = 18;
        chkPredeterminada.Location = new Point(230, 454);
        chkPredeterminada.Name = "chkPredeterminada";
        chkPredeterminada.Properties.Caption = "Predeterminada";
        chkPredeterminada.Size = new Size(120, 20);
        chkPredeterminada.TabIndex = 19;
        chkValidarConexion.Location = new Point(356, 454);
        chkValidarConexion.Name = "chkValidarConexion";
        chkValidarConexion.Properties.Caption = "Validar conexion";
        chkValidarConexion.Size = new Size(130, 20);
        chkValidarConexion.TabIndex = 20;
        // 
        // lblLogo
        // 
        lblLogo.Appearance.Font = new Font("Segoe UI", 9F);
        lblLogo.Appearance.ForeColor = BrandResources.Text;
        lblLogo.Appearance.Options.UseFont = true;
        lblLogo.Appearance.Options.UseForeColor = true;
        lblLogo.Location = new Point(586, 26);
        lblLogo.Name = "lblLogo";
        lblLogo.Size = new Size(28, 15);
        lblLogo.TabIndex = 36;
        lblLogo.Text = "Logo";
        // 
        // picLogo
        // 
        picLogo.Location = new Point(586, 50);
        picLogo.Name = "picLogo";
        picLogo.Properties.AllowFocused = false;
        picLogo.Properties.NullText = "Sin logo";
        picLogo.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
        picLogo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
        picLogo.Size = new Size(140, 140);
        picLogo.TabIndex = 37;
        // 
        // btnCargarLogo
        // 
        btnCargarLogo.Location = new Point(586, 196);
        btnCargarLogo.Name = "btnCargarLogo";
        btnCargarLogo.Size = new Size(68, 28);
        btnCargarLogo.TabIndex = 38;
        btnCargarLogo.Text = "Cargar";
        // 
        // btnQuitarLogo
        // 
        btnQuitarLogo.Location = new Point(658, 196);
        btnQuitarLogo.Name = "btnQuitarLogo";
        btnQuitarLogo.Size = new Size(68, 28);
        btnQuitarLogo.TabIndex = 39;
        btnQuitarLogo.Text = "Quitar";
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(492, 496);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 40;
        btnCancelar.Text = "Cancelar";
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.ImageOptions.SvgImageSize = new Size(32, 32);        btnGuardar.Location = new Point(598, 496);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 41;
        btnGuardar.Text = "Guardar";
        // 
        // ConfigurationCompanyEditForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(752, 560);
        Controls.AddRange(new Control[] {
            lblCodigo, txtCodigo, lblNombreComercial, txtNombreComercial, lblRazonSocial, txtRazonSocial,
            lblIdentificacion, txtIdentificacion, lblTelefono, txtTelefono, lblCorreo, txtCorreo,
            lblDireccion, memDireccion, lblMotor, cmbMotor, lblServidor, txtServidor, lblPuerto, sedPuerto,
            lblBaseDatos, txtBaseDatos, lblUsuarioDb, txtUsuarioDb, lblClaveDb, txtClaveDb, lblSap, cmbSap,
            lblOrden, sedOrden, lblZonaHoraria, txtZonaHoraria, lblCultura, txtCultura, lblMoneda, txtMoneda,
            chkActivo, chkPredeterminada, chkValidarConexion, lblLogo, picLogo, btnCargarLogo, btnQuitarLogo,
            btnCancelar, btnGuardar
        });
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "ConfigurationCompanyEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nueva compania";
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombreComercial.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtRazonSocial.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificacion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDireccion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTelefono.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCorreo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbMotor.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtServidor.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedPuerto.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBaseDatos.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtUsuarioDb.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtClaveDb.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbSap.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedOrden.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtZonaHoraria.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCultura.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtMoneda.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkPredeterminada.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkValidarConexion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)picLogo.Properties).EndInit();
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

    private LabelControl lblCodigo;
    private TextEdit txtCodigo;
    private LabelControl lblNombreComercial;
    private TextEdit txtNombreComercial;
    private LabelControl lblRazonSocial;
    private TextEdit txtRazonSocial;
    private LabelControl lblIdentificacion;
    private TextEdit txtIdentificacion;
    private LabelControl lblDireccion;
    private MemoEdit memDireccion;
    private LabelControl lblTelefono;
    private TextEdit txtTelefono;
    private LabelControl lblCorreo;
    private TextEdit txtCorreo;
    private LabelControl lblMotor;
    private ComboBoxEdit cmbMotor;
    private LabelControl lblServidor;
    private TextEdit txtServidor;
    private LabelControl lblPuerto;
    private SpinEdit sedPuerto;
    private LabelControl lblBaseDatos;
    private TextEdit txtBaseDatos;
    private LabelControl lblUsuarioDb;
    private TextEdit txtUsuarioDb;
    private LabelControl lblClaveDb;
    private TextEdit txtClaveDb;
    private LabelControl lblSap;
    private ComboBoxEdit cmbSap;
    private LabelControl lblOrden;
    private SpinEdit sedOrden;
    private LabelControl lblZonaHoraria;
    private TextEdit txtZonaHoraria;
    private LabelControl lblCultura;
    private TextEdit txtCultura;
    private LabelControl lblMoneda;
    private TextEdit txtMoneda;
    private CheckEdit chkActivo;
    private CheckEdit chkPredeterminada;
    private CheckEdit chkValidarConexion;
    private LabelControl lblLogo;
    private PictureEdit picLogo;
    private SimpleButton btnCargarLogo;
    private SimpleButton btnQuitarLogo;
    private new SimpleButton btnGuardar;
    private new SimpleButton btnCancelar;
}

