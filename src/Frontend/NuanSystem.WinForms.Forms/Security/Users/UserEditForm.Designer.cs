using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Security.Users;

partial class UserEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblUsuario = new LabelControl();
        txtUsuario = new TextEdit();
        lblCorreo = new LabelControl();
        txtCorreo = new TextEdit();
        lblTelefono = new LabelControl();
        txtTelefono = new TextEdit();
        lblNombre = new LabelControl();
        txtNombre = new TextEdit();
        lblNombres = new LabelControl();
        txtNombres = new TextEdit();
        lblApellidos = new LabelControl();
        txtApellidos = new TextEdit();
        lblClave = new LabelControl();
        txtClave = new TextEdit();
        lblRol = new LabelControl();
        lueRol = new NuanLookupEdit();
        lblBloqueo = new LabelControl();
        deBloqueo = new DateEdit();
        lblFoto = new LabelControl();
        picFoto = new PictureEdit();
        btnCargarFoto = new SimpleButton();
        btnQuitarFoto = new SimpleButton();
        chkActivo = new CheckEdit();
        chkBloqueado = new CheckEdit();
        chkPuedeWeb = new CheckEdit();
        chkPuedeMovil = new CheckEdit();
        chkCorreoConfirmado = new CheckEdit();
        chkTelefonoConfirmado = new CheckEdit();
        chkCambiarClave = new CheckEdit();
        chkDobleFactor = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)txtUsuario.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCorreo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTelefono.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombres.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtApellidos.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtClave.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRol.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)deBloqueo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)deBloqueo.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picFoto.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkBloqueado.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkPuedeWeb.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkPuedeMovil.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkCorreoConfirmado.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkTelefonoConfirmado.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkCambiarClave.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkDobleFactor.Properties).BeginInit();
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
        btnCancelar.Location = new Point(464, 401);
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
        btnGuardar.Location = new Point(570, 401);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // lblUsuario
        // 
        lblUsuario.Appearance.Font = new Font("Segoe UI", 9F);
        lblUsuario.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblUsuario.Appearance.Options.UseFont = true;
        lblUsuario.Appearance.Options.UseForeColor = true;
        lblUsuario.Location = new Point(29, 26);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Size = new Size(40, 15);
        lblUsuario.TabIndex = 0;
        lblUsuario.Text = "Usuario";
        // 
        // txtUsuario
        // 
        txtUsuario.Location = new Point(140, 24);
        txtUsuario.Name = "txtUsuario";
        txtUsuario.Size = new Size(360, 20);
        txtUsuario.TabIndex = 1;
        // 
        // lblCorreo
        // 
        lblCorreo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCorreo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCorreo.Appearance.Options.UseFont = true;
        lblCorreo.Appearance.Options.UseForeColor = true;
        lblCorreo.Location = new Point(29, 52);
        lblCorreo.Name = "lblCorreo";
        lblCorreo.Size = new Size(36, 15);
        lblCorreo.TabIndex = 2;
        lblCorreo.Text = "Correo";
        // 
        // txtCorreo
        // 
        txtCorreo.Location = new Point(140, 50);
        txtCorreo.Name = "txtCorreo";
        txtCorreo.Size = new Size(360, 20);
        txtCorreo.TabIndex = 3;
        // 
        // lblTelefono
        // 
        lblTelefono.Appearance.Font = new Font("Segoe UI", 9F);
        lblTelefono.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTelefono.Appearance.Options.UseFont = true;
        lblTelefono.Appearance.Options.UseForeColor = true;
        lblTelefono.Location = new Point(29, 78);
        lblTelefono.Name = "lblTelefono";
        lblTelefono.Size = new Size(47, 15);
        lblTelefono.TabIndex = 4;
        lblTelefono.Text = "Telefono";
        // 
        // txtTelefono
        // 
        txtTelefono.Location = new Point(140, 76);
        txtTelefono.Name = "txtTelefono";
        txtTelefono.Size = new Size(360, 20);
        txtTelefono.TabIndex = 5;
        // 
        // lblNombre
        // 
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblNombre.Location = new Point(29, 104);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(44, 15);
        lblNombre.TabIndex = 6;
        lblNombre.Text = "Nombre";
        // 
        // txtNombre
        // 
        txtNombre.Location = new Point(140, 102);
        txtNombre.Name = "txtNombre";
        txtNombre.Size = new Size(360, 20);
        txtNombre.TabIndex = 7;
        // 
        // lblNombres
        // 
        lblNombres.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombres.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNombres.Appearance.Options.UseFont = true;
        lblNombres.Appearance.Options.UseForeColor = true;
        lblNombres.Location = new Point(29, 130);
        lblNombres.Name = "lblNombres";
        lblNombres.Size = new Size(49, 15);
        lblNombres.TabIndex = 8;
        lblNombres.Text = "Nombres";
        // 
        // txtNombres
        // 
        txtNombres.Location = new Point(140, 128);
        txtNombres.Name = "txtNombres";
        txtNombres.Size = new Size(360, 20);
        txtNombres.TabIndex = 9;
        // 
        // lblApellidos
        // 
        lblApellidos.Appearance.Font = new Font("Segoe UI", 9F);
        lblApellidos.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblApellidos.Appearance.Options.UseFont = true;
        lblApellidos.Appearance.Options.UseForeColor = true;
        lblApellidos.Location = new Point(29, 156);
        lblApellidos.Name = "lblApellidos";
        lblApellidos.Size = new Size(49, 15);
        lblApellidos.TabIndex = 10;
        lblApellidos.Text = "Apellidos";
        // 
        // txtApellidos
        // 
        txtApellidos.Location = new Point(140, 154);
        txtApellidos.Name = "txtApellidos";
        txtApellidos.Size = new Size(360, 20);
        txtApellidos.TabIndex = 11;
        // 
        // lblClave
        // 
        lblClave.Appearance.Font = new Font("Segoe UI", 9F);
        lblClave.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblClave.Appearance.Options.UseFont = true;
        lblClave.Appearance.Options.UseForeColor = true;
        lblClave.Location = new Point(29, 182);
        lblClave.Name = "lblClave";
        lblClave.Size = new Size(29, 15);
        lblClave.TabIndex = 12;
        lblClave.Text = "Clave";
        // 
        // txtClave
        // 
        txtClave.Location = new Point(140, 180);
        txtClave.Name = "txtClave";
        txtClave.Properties.PasswordChar = '*';
        txtClave.Size = new Size(360, 20);
        txtClave.TabIndex = 13;
        // 
        // lblRol
        // 
        lblRol.Appearance.Font = new Font("Segoe UI", 9F);
        lblRol.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblRol.Appearance.Options.UseFont = true;
        lblRol.Appearance.Options.UseForeColor = true;
        lblRol.Location = new Point(29, 208);
        lblRol.Name = "lblRol";
        lblRol.Size = new Size(17, 15);
        lblRol.TabIndex = 14;
        lblRol.Text = "Rol";
        // 
        // lueRol
        // 
        lueRol.Location = new Point(140, 206);
        lueRol.Name = "lueRol";
        lueRol.Properties.Buttons.Clear();
        lueRol.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus) });
        lueRol.Size = new Size(360, 20);
        lueRol.TabIndex = 15;
        // 
        // lblBloqueo
        // 
        lblBloqueo.Appearance.Font = new Font("Segoe UI", 9F);
        lblBloqueo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblBloqueo.Appearance.Options.UseFont = true;
        lblBloqueo.Appearance.Options.UseForeColor = true;
        lblBloqueo.Location = new Point(29, 234);
        lblBloqueo.Name = "lblBloqueo";
        lblBloqueo.Size = new Size(88, 15);
        lblBloqueo.TabIndex = 16;
        lblBloqueo.Text = "Bloqueado hasta";
        // 
        // deBloqueo
        // 
        deBloqueo.EditValue = null;
        deBloqueo.Location = new Point(140, 232);
        deBloqueo.Name = "deBloqueo";
        deBloqueo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        deBloqueo.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
        deBloqueo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        deBloqueo.Properties.DisplayFormat.FormatString = "g";
        deBloqueo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        deBloqueo.Properties.EditFormat.FormatString = "g";
        deBloqueo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        deBloqueo.Properties.MaskSettings.Set("mask", "g");
        deBloqueo.Size = new Size(360, 20);
        deBloqueo.TabIndex = 17;
        // 
        // lblFoto
        // 
        lblFoto.Appearance.Font = new Font("Segoe UI", 9F);
        lblFoto.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFoto.Appearance.Options.UseFont = true;
        lblFoto.Appearance.Options.UseForeColor = true;
        lblFoto.Location = new Point(530, 26);
        lblFoto.Name = "lblFoto";
        lblFoto.Size = new Size(24, 15);
        lblFoto.TabIndex = 18;
        lblFoto.Text = "Foto";
        // 
        // picFoto
        // 
        picFoto.Location = new Point(530, 50);
        picFoto.Name = "picFoto";
        picFoto.Properties.AllowFocused = false;
        picFoto.Properties.NullText = "Sin foto";
        picFoto.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
        picFoto.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
        picFoto.Size = new Size(140, 140);
        picFoto.TabIndex = 19;
        // 
        // btnCargarFoto
        // 
        btnCargarFoto.Location = new Point(530, 196);
        btnCargarFoto.Name = "btnCargarFoto";
        btnCargarFoto.Size = new Size(68, 28);
        btnCargarFoto.TabIndex = 20;
        btnCargarFoto.Text = "Cargar";
        // 
        // btnQuitarFoto
        // 
        btnQuitarFoto.Location = new Point(602, 196);
        btnQuitarFoto.Name = "btnQuitarFoto";
        btnQuitarFoto.Size = new Size(68, 28);
        btnQuitarFoto.TabIndex = 21;
        btnQuitarFoto.Text = "Quitar";
        // 
        // chkActivo
        // 
        chkActivo.EditValue = true;
        chkActivo.Location = new Point(137, 286);
        chkActivo.Name = "chkActivo";
        chkActivo.Properties.Caption = "Activo";
        chkActivo.Size = new Size(75, 18);
        chkActivo.TabIndex = 22;
        // 
        // chkBloqueado
        // 
        chkBloqueado.Location = new Point(218, 286);
        chkBloqueado.Name = "chkBloqueado";
        chkBloqueado.Properties.Caption = "Bloqueado";
        chkBloqueado.Size = new Size(100, 18);
        chkBloqueado.TabIndex = 23;
        // 
        // chkPuedeWeb
        // 
        chkPuedeWeb.EditValue = true;
        chkPuedeWeb.Location = new Point(324, 286);
        chkPuedeWeb.Name = "chkPuedeWeb";
        chkPuedeWeb.Properties.Caption = "Web";
        chkPuedeWeb.Size = new Size(70, 18);
        chkPuedeWeb.TabIndex = 24;
        // 
        // chkPuedeMovil
        // 
        chkPuedeMovil.EditValue = true;
        chkPuedeMovil.Location = new Point(400, 286);
        chkPuedeMovil.Name = "chkPuedeMovil";
        chkPuedeMovil.Properties.Caption = "Movil";
        chkPuedeMovil.Size = new Size(70, 18);
        chkPuedeMovil.TabIndex = 25;
        // 
        // chkCorreoConfirmado
        // 
        chkCorreoConfirmado.Location = new Point(137, 312);
        chkCorreoConfirmado.Name = "chkCorreoConfirmado";
        chkCorreoConfirmado.Properties.Caption = "Correo confirmado";
        chkCorreoConfirmado.Size = new Size(130, 18);
        chkCorreoConfirmado.TabIndex = 26;
        // 
        // chkTelefonoConfirmado
        // 
        chkTelefonoConfirmado.Location = new Point(273, 312);
        chkTelefonoConfirmado.Name = "chkTelefonoConfirmado";
        chkTelefonoConfirmado.Properties.Caption = "Telefono confirmado";
        chkTelefonoConfirmado.Size = new Size(146, 18);
        chkTelefonoConfirmado.TabIndex = 27;
        // 
        // chkCambiarClave
        // 
        chkCambiarClave.Location = new Point(137, 338);
        chkCambiarClave.Name = "chkCambiarClave";
        chkCambiarClave.Properties.Caption = "Cambiar clave al ingresar";
        chkCambiarClave.Size = new Size(170, 18);
        chkCambiarClave.TabIndex = 28;
        // 
        // chkDobleFactor
        // 
        chkDobleFactor.Location = new Point(313, 338);
        chkDobleFactor.Name = "chkDobleFactor";
        chkDobleFactor.Properties.Caption = "Doble factor";
        chkDobleFactor.Size = new Size(100, 18);
        chkDobleFactor.TabIndex = 29;
        // 
        // UserEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(700, 449);
        Controls.Add(chkDobleFactor);
        Controls.Add(chkCambiarClave);
        Controls.Add(chkTelefonoConfirmado);
        Controls.Add(chkCorreoConfirmado);
        Controls.Add(chkPuedeMovil);
        Controls.Add(chkPuedeWeb);
        Controls.Add(chkBloqueado);
        Controls.Add(chkActivo);
        Controls.Add(btnQuitarFoto);
        Controls.Add(btnCargarFoto);
        Controls.Add(picFoto);
        Controls.Add(lblFoto);
        Controls.Add(deBloqueo);
        Controls.Add(lblBloqueo);
        Controls.Add(lueRol);
        Controls.Add(lblRol);
        Controls.Add(txtClave);
        Controls.Add(lblClave);
        Controls.Add(txtApellidos);
        Controls.Add(lblApellidos);
        Controls.Add(txtNombres);
        Controls.Add(lblNombres);
        Controls.Add(txtNombre);
        Controls.Add(lblNombre);
        Controls.Add(txtTelefono);
        Controls.Add(lblTelefono);
        Controls.Add(txtCorreo);
        Controls.Add(lblCorreo);
        Controls.Add(txtUsuario);
        Controls.Add(lblUsuario);
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        Name = "UserEditForm";
        Text = "Nuevo usuario";
        Controls.SetChildIndex(lblUsuario, 0);
        Controls.SetChildIndex(txtUsuario, 0);
        Controls.SetChildIndex(lblCorreo, 0);
        Controls.SetChildIndex(txtCorreo, 0);
        Controls.SetChildIndex(lblTelefono, 0);
        Controls.SetChildIndex(txtTelefono, 0);
        Controls.SetChildIndex(lblNombre, 0);
        Controls.SetChildIndex(txtNombre, 0);
        Controls.SetChildIndex(lblNombres, 0);
        Controls.SetChildIndex(txtNombres, 0);
        Controls.SetChildIndex(lblApellidos, 0);
        Controls.SetChildIndex(txtApellidos, 0);
        Controls.SetChildIndex(lblClave, 0);
        Controls.SetChildIndex(txtClave, 0);
        Controls.SetChildIndex(lblRol, 0);
        Controls.SetChildIndex(lueRol, 0);
        Controls.SetChildIndex(lblBloqueo, 0);
        Controls.SetChildIndex(deBloqueo, 0);
        Controls.SetChildIndex(lblFoto, 0);
        Controls.SetChildIndex(picFoto, 0);
        Controls.SetChildIndex(btnCargarFoto, 0);
        Controls.SetChildIndex(btnQuitarFoto, 0);
        Controls.SetChildIndex(chkActivo, 0);
        Controls.SetChildIndex(chkBloqueado, 0);
        Controls.SetChildIndex(chkPuedeWeb, 0);
        Controls.SetChildIndex(chkPuedeMovil, 0);
        Controls.SetChildIndex(chkCorreoConfirmado, 0);
        Controls.SetChildIndex(chkTelefonoConfirmado, 0);
        Controls.SetChildIndex(chkCambiarClave, 0);
        Controls.SetChildIndex(chkDobleFactor, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtUsuario.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCorreo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTelefono.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombres.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtApellidos.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtClave.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRol.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)deBloqueo.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)deBloqueo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)picFoto.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkBloqueado.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkPuedeWeb.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkPuedeMovil.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkCorreoConfirmado.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkTelefonoConfirmado.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkCambiarClave.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkDobleFactor.Properties).EndInit();
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

    private LabelControl lblUsuario;
    private TextEdit txtUsuario;
    private LabelControl lblCorreo;
    private TextEdit txtCorreo;
    private LabelControl lblTelefono;
    private TextEdit txtTelefono;
    private LabelControl lblNombre;
    private TextEdit txtNombre;
    private LabelControl lblNombres;
    private TextEdit txtNombres;
    private LabelControl lblApellidos;
    private TextEdit txtApellidos;
    private LabelControl lblClave;
    private TextEdit txtClave;
    private LabelControl lblRol;
    private NuanLookupEdit lueRol;
    private LabelControl lblBloqueo;
    private DateEdit deBloqueo;
    private LabelControl lblFoto;
    private PictureEdit picFoto;
    private SimpleButton btnCargarFoto;
    private SimpleButton btnQuitarFoto;
    private CheckEdit chkActivo;
    private CheckEdit chkBloqueado;
    private CheckEdit chkPuedeWeb;
    private CheckEdit chkPuedeMovil;
    private CheckEdit chkCorreoConfirmado;
    private CheckEdit chkTelefonoConfirmado;
    private CheckEdit chkCambiarClave;
    private CheckEdit chkDobleFactor;
}


