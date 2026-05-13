using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Auth;

partial class ChangePasswordForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblClaveActual = new LabelControl();
        txtClaveActual = new TextEdit();
        lblNuevaClave = new LabelControl();
        txtNuevaClave = new TextEdit();
        lblConfirmarClave = new LabelControl();
        txtConfirmarClave = new TextEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtClaveActual.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNuevaClave.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtConfirmarClave.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblClaveActual
        // 
        lblClaveActual.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblClaveActual.Appearance.Options.UseForeColor = true;
        lblClaveActual.Location = new Point(25, 23);
        lblClaveActual.Name = "lblClaveActual";
        lblClaveActual.Size = new Size(59, 13);
        lblClaveActual.TabIndex = 0;
        lblClaveActual.Text = "Clave actual";
        // 
        // txtClaveActual
        // 
        txtClaveActual.Location = new Point(129, 21);
        txtClaveActual.Name = "txtClaveActual";
        txtClaveActual.Properties.PasswordChar = '*';
        txtClaveActual.Size = new Size(214, 20);
        txtClaveActual.TabIndex = 1;
        // 
        // lblNuevaClave
        // 
        lblNuevaClave.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNuevaClave.Appearance.Options.UseForeColor = true;
        lblNuevaClave.Location = new Point(25, 45);
        lblNuevaClave.Name = "lblNuevaClave";
        lblNuevaClave.Size = new Size(59, 13);
        lblNuevaClave.TabIndex = 2;
        lblNuevaClave.Text = "Nueva clave";
        // 
        // txtNuevaClave
        // 
        txtNuevaClave.Location = new Point(129, 43);
        txtNuevaClave.Name = "txtNuevaClave";
        txtNuevaClave.Properties.PasswordChar = '*';
        txtNuevaClave.Size = new Size(214, 20);
        txtNuevaClave.TabIndex = 3;
        // 
        // lblConfirmarClave
        // 
        lblConfirmarClave.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblConfirmarClave.Appearance.Options.UseForeColor = true;
        lblConfirmarClave.Location = new Point(25, 68);
        lblConfirmarClave.Name = "lblConfirmarClave";
        lblConfirmarClave.Size = new Size(75, 13);
        lblConfirmarClave.TabIndex = 4;
        lblConfirmarClave.Text = "Confirmar clave";
        // 
        // txtConfirmarClave
        // 
        txtConfirmarClave.Location = new Point(129, 66);
        txtConfirmarClave.Name = "txtConfirmarClave";
        txtConfirmarClave.Properties.PasswordChar = '*';
        txtConfirmarClave.Size = new Size(214, 20);
        txtConfirmarClave.TabIndex = 5;
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnGuardar.Location = new Point(243, 105);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 7;
        btnGuardar.Text = "Guardar";
        // 
        // btnCancelar
        // 
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnCancelar.Location = new Point(137, 105);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 6;
        btnCancelar.Text = "Cancelar";
        // 
        // ChangePasswordForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(369, 157);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        Controls.Add(txtConfirmarClave);
        Controls.Add(lblConfirmarClave);
        Controls.Add(txtNuevaClave);
        Controls.Add(lblNuevaClave);
        Controls.Add(txtClaveActual);
        Controls.Add(lblClaveActual);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ChangePasswordForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Cambiar clave";
        ((System.ComponentModel.ISupportInitialize)txtClaveActual.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNuevaClave.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtConfirmarClave.Properties).EndInit();
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

    private LabelControl lblClaveActual;
    private TextEdit txtClaveActual;
    private LabelControl lblNuevaClave;
    private TextEdit txtNuevaClave;
    private LabelControl lblConfirmarClave;
    private TextEdit txtConfirmarClave;
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}
