using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Customers;

partial class CustomerEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblCodigo = new LabelControl();
        txtCodigo = new TextEdit();
        lblNombre = new LabelControl();
        txtNombre = new TextEdit();
        lblIdentificacion = new LabelControl();
        txtIdentificacion = new TextEdit();
        lblCorreo = new LabelControl();
        txtCorreo = new TextEdit();
        lblTelefono = new LabelControl();
        txtTelefono = new TextEdit();
        lblDireccion = new LabelControl();
        memDireccion = new MemoEdit();
        chkActivo = new CheckEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificacion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCorreo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTelefono.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDireccion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        SuspendLayout();
        lblCodigo.Location = new Point(24, 28); lblCodigo.Name = "lblCodigo"; lblCodigo.Size = new Size(36, 13); lblCodigo.Text = "Codigo";
        txtCodigo.Location = new Point(154, 24); txtCodigo.Name = "txtCodigo"; txtCodigo.Size = new Size(330, 22);
        lblNombre.Location = new Point(24, 70); lblNombre.Name = "lblNombre"; lblNombre.Size = new Size(37, 13); lblNombre.Text = "Nombre";
        txtNombre.Location = new Point(154, 66); txtNombre.Name = "txtNombre"; txtNombre.Size = new Size(330, 22);
        lblIdentificacion.Location = new Point(24, 112); lblIdentificacion.Name = "lblIdentificacion"; lblIdentificacion.Size = new Size(65, 13); lblIdentificacion.Text = "Identificacion";
        txtIdentificacion.Location = new Point(154, 108); txtIdentificacion.Name = "txtIdentificacion"; txtIdentificacion.Size = new Size(330, 22);
        lblCorreo.Location = new Point(24, 154); lblCorreo.Name = "lblCorreo"; lblCorreo.Size = new Size(34, 13); lblCorreo.Text = "Correo";
        txtCorreo.Location = new Point(154, 150); txtCorreo.Name = "txtCorreo"; txtCorreo.Size = new Size(330, 22);
        lblTelefono.Location = new Point(24, 196); lblTelefono.Name = "lblTelefono"; lblTelefono.Size = new Size(43, 13); lblTelefono.Text = "Telefono";
        txtTelefono.Location = new Point(154, 192); txtTelefono.Name = "txtTelefono"; txtTelefono.Size = new Size(330, 22);
        lblDireccion.Location = new Point(24, 238); lblDireccion.Name = "lblDireccion"; lblDireccion.Size = new Size(47, 13); lblDireccion.Text = "Direccion";
        memDireccion.Location = new Point(154, 234); memDireccion.Name = "memDireccion"; memDireccion.Size = new Size(330, 56);
        chkActivo.Location = new Point(151, 304); chkActivo.Name = "chkActivo"; chkActivo.Properties.Caption = "Activo"; chkActivo.Size = new Size(75, 20);
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148); btnGuardar.Appearance.ForeColor = Color.White; btnGuardar.Appearance.Options.UseBackColor = true; btnGuardar.Appearance.Options.UseForeColor = true; btnGuardar.Location = new Point(278, 336); btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat; btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false; btnGuardar.Name = "btnGuardar"; btnGuardar.Size = new Size(100, 32); btnGuardar.Text = "Guardar";
        btnCancelar.DialogResult = DialogResult.Cancel; btnCancelar.Location = new Point(384, 336); btnCancelar.Name = "btnCancelar"; btnCancelar.Size = new Size(100, 32); btnCancelar.Text = "Cancelar";
        AcceptButton = btnGuardar;
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(520, 390);
        Controls.AddRange(new Control[] { lblCodigo, txtCodigo, lblNombre, txtNombre, lblIdentificacion, txtIdentificacion, lblCorreo, txtCorreo, lblTelefono, txtTelefono, lblDireccion, memDireccion, chkActivo, btnGuardar, btnCancelar });
        FormBorderStyle = FormBorderStyle.FixedDialog;
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        MaximizeBox = false;
        Name = "CustomerEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nuevo cliente";
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificacion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCorreo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTelefono.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDireccion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { components?.Dispose(); }
        base.Dispose(disposing);
    }

    private LabelControl lblCodigo; private TextEdit txtCodigo;
    private LabelControl lblNombre; private TextEdit txtNombre;
    private LabelControl lblIdentificacion; private TextEdit txtIdentificacion;
    private LabelControl lblCorreo; private TextEdit txtCorreo;
    private LabelControl lblTelefono; private TextEdit txtTelefono;
    private LabelControl lblDireccion; private MemoEdit memDireccion;
    private CheckEdit chkActivo;
    private SimpleButton btnGuardar; private SimpleButton btnCancelar;
}
