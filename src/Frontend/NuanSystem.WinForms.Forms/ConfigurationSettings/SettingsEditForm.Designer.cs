using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.ConfigurationSettings;

partial class SettingsEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblClave = new LabelControl();
        txtClave = new TextEdit();
        lblValor = new LabelControl();
        memValor = new MemoEdit();
        lblDescripcion = new LabelControl();
        memDescripcion = new MemoEdit();
        lblTipoDato = new LabelControl();
        txtTipoDato = new TextEdit();
        lblCategoria = new LabelControl();
        txtCategoria = new TextEdit();
        lblOrden = new LabelControl();
        sedOrden = new SpinEdit();
        lblValorDefecto = new LabelControl();
        memValorDefecto = new MemoEdit();
        lblValidacion = new LabelControl();
        txtValidacion = new TextEdit();
        chkEncriptado = new CheckEdit();
        chkSistema = new CheckEdit();
        chkEditable = new CheckEdit();
        chkActivo = new CheckEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtClave.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memValor.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTipoDato.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCategoria.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedOrden.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memValorDefecto.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtValidacion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkEncriptado.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkSistema.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkEditable.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblClave
        // 
        lblClave.Appearance.Font = new Font("Segoe UI", 9F);
        lblClave.Appearance.ForeColor = BrandResources.Text;
        lblClave.Appearance.Options.UseFont = true;
        lblClave.Appearance.Options.UseForeColor = true;
        lblClave.Location = new Point(28, 28);
        lblClave.Name = "lblClave";
        lblClave.Size = new Size(30, 15);
        lblClave.TabIndex = 0;
        lblClave.Text = "Clave";
        // 
        // txtClave
        // 
        txtClave.Location = new Point(132, 26);
        txtClave.Name = "txtClave";
        txtClave.Size = new Size(380, 20);
        txtClave.TabIndex = 1;
        // 
        // lblValor
        // 
        lblValor.Appearance.Font = new Font("Segoe UI", 9F);
        lblValor.Appearance.ForeColor = BrandResources.Text;
        lblValor.Appearance.Options.UseFont = true;
        lblValor.Appearance.Options.UseForeColor = true;
        lblValor.Location = new Point(28, 58);
        lblValor.Name = "lblValor";
        lblValor.Size = new Size(27, 15);
        lblValor.TabIndex = 2;
        lblValor.Text = "Valor";
        // 
        // memValor
        // 
        memValor.Location = new Point(132, 56);
        memValor.Name = "memValor";
        memValor.Size = new Size(380, 86);
        memValor.TabIndex = 3;
        // 
        // lblDescripcion
        // 
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = BrandResources.Text;
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblDescripcion.Location = new Point(28, 154);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(62, 15);
        lblDescripcion.TabIndex = 4;
        lblDescripcion.Text = "Descripcion";
        // 
        // memDescripcion
        // 
        memDescripcion.Location = new Point(132, 152);
        memDescripcion.Name = "memDescripcion";
        memDescripcion.Size = new Size(380, 86);
        memDescripcion.TabIndex = 5;
        // 
        // lblTipoDato
        // 
        lblTipoDato.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipoDato.Appearance.ForeColor = BrandResources.Text;
        lblTipoDato.Appearance.Options.UseFont = true;
        lblTipoDato.Appearance.Options.UseForeColor = true;
        lblTipoDato.Location = new Point(28, 252);
        lblTipoDato.Name = "lblTipoDato";
        lblTipoDato.Size = new Size(24, 15);
        lblTipoDato.TabIndex = 6;
        lblTipoDato.Text = "Tipo";
        // 
        // txtTipoDato
        // 
        txtTipoDato.Location = new Point(132, 250);
        txtTipoDato.Name = "txtTipoDato";
        txtTipoDato.Size = new Size(120, 20);
        txtTipoDato.TabIndex = 7;
        // 
        // lblCategoria
        // 
        lblCategoria.Appearance.Font = new Font("Segoe UI", 9F);
        lblCategoria.Appearance.ForeColor = BrandResources.Text;
        lblCategoria.Appearance.Options.UseFont = true;
        lblCategoria.Appearance.Options.UseForeColor = true;
        lblCategoria.Location = new Point(272, 252);
        lblCategoria.Name = "lblCategoria";
        lblCategoria.Size = new Size(52, 15);
        lblCategoria.TabIndex = 8;
        lblCategoria.Text = "Categoria";
        // 
        // txtCategoria
        // 
        txtCategoria.Location = new Point(354, 250);
        txtCategoria.Name = "txtCategoria";
        txtCategoria.Size = new Size(158, 20);
        txtCategoria.TabIndex = 9;
        // 
        // lblOrden
        // 
        lblOrden.Appearance.Font = new Font("Segoe UI", 9F);
        lblOrden.Appearance.ForeColor = BrandResources.Text;
        lblOrden.Appearance.Options.UseFont = true;
        lblOrden.Appearance.Options.UseForeColor = true;
        lblOrden.Location = new Point(28, 282);
        lblOrden.Name = "lblOrden";
        lblOrden.Size = new Size(33, 15);
        lblOrden.TabIndex = 10;
        lblOrden.Text = "Orden";
        // 
        // sedOrden
        // 
        sedOrden.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedOrden.Location = new Point(132, 280);
        sedOrden.Name = "sedOrden";
        sedOrden.Properties.IsFloatValue = false;
        sedOrden.Properties.MaskSettings.Set("mask", "N00");
        sedOrden.Properties.MaxValue = new decimal(new int[] { 100000, 0, 0, 0 });
        sedOrden.Size = new Size(120, 20);
        sedOrden.TabIndex = 11;
        // 
        // lblValidacion
        // 
        lblValidacion.Appearance.Font = new Font("Segoe UI", 9F);
        lblValidacion.Appearance.ForeColor = BrandResources.Text;
        lblValidacion.Appearance.Options.UseFont = true;
        lblValidacion.Appearance.Options.UseForeColor = true;
        lblValidacion.Location = new Point(272, 282);
        lblValidacion.Name = "lblValidacion";
        lblValidacion.Size = new Size(54, 15);
        lblValidacion.TabIndex = 12;
        lblValidacion.Text = "Validacion";
        // 
        // txtValidacion
        // 
        txtValidacion.Location = new Point(354, 280);
        txtValidacion.Name = "txtValidacion";
        txtValidacion.Size = new Size(158, 20);
        txtValidacion.TabIndex = 13;
        // 
        // lblValorDefecto
        // 
        lblValorDefecto.Appearance.Font = new Font("Segoe UI", 9F);
        lblValorDefecto.Appearance.ForeColor = BrandResources.Text;
        lblValorDefecto.Appearance.Options.UseFont = true;
        lblValorDefecto.Appearance.Options.UseForeColor = true;
        lblValorDefecto.Location = new Point(28, 314);
        lblValorDefecto.Name = "lblValorDefecto";
        lblValorDefecto.Size = new Size(90, 15);
        lblValorDefecto.TabIndex = 14;
        lblValorDefecto.Text = "Valor por defecto";
        // 
        // memValorDefecto
        // 
        memValorDefecto.Location = new Point(132, 312);
        memValorDefecto.Name = "memValorDefecto";
        memValorDefecto.Size = new Size(380, 58);
        memValorDefecto.TabIndex = 15;
        // 
        // checks
        // 
        chkEncriptado.Location = new Point(129, 382);
        chkEncriptado.Name = "chkEncriptado";
        chkEncriptado.Properties.Caption = "Encriptado";
        chkEncriptado.Size = new Size(90, 20);
        chkEncriptado.TabIndex = 16;
        chkSistema.Location = new Point(225, 382);
        chkSistema.Name = "chkSistema";
        chkSistema.Properties.Caption = "Sistema";
        chkSistema.Size = new Size(80, 20);
        chkSistema.TabIndex = 17;
        chkEditable.Location = new Point(311, 382);
        chkEditable.Name = "chkEditable";
        chkEditable.Properties.Caption = "Editable";
        chkEditable.Size = new Size(80, 20);
        chkEditable.TabIndex = 18;
        chkActivo.Location = new Point(397, 382);
        chkActivo.Name = "chkActivo";
        chkActivo.Properties.Caption = "Activo";
        chkActivo.Size = new Size(80, 20);
        chkActivo.TabIndex = 19;
        // 
        // btnCancelar
        // 
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
        btnCancelar.Location = new Point(306, 424);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 20;
        btnCancelar.Text = "Cancelar";
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
        btnGuardar.ImageOptions.SvgImageSize = new Size(32, 32);        btnGuardar.Location = new Point(412, 424);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 21;
        btnGuardar.Text = "Guardar";
        // 
        // SettingsEditForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(544, 488);
        Controls.Add(lblClave);
        Controls.Add(txtClave);
        Controls.Add(lblValor);
        Controls.Add(memValor);
        Controls.Add(lblDescripcion);
        Controls.Add(memDescripcion);
        Controls.Add(lblTipoDato);
        Controls.Add(txtTipoDato);
        Controls.Add(lblCategoria);
        Controls.Add(txtCategoria);
        Controls.Add(lblOrden);
        Controls.Add(sedOrden);
        Controls.Add(lblValidacion);
        Controls.Add(txtValidacion);
        Controls.Add(lblValorDefecto);
        Controls.Add(memValorDefecto);
        Controls.Add(chkEncriptado);
        Controls.Add(chkSistema);
        Controls.Add(chkEditable);
        Controls.Add(chkActivo);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "SettingsEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Parametro";
        ((System.ComponentModel.ISupportInitialize)txtClave.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memValor.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTipoDato.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCategoria.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedOrden.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memValorDefecto.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtValidacion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkEncriptado.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkSistema.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkEditable.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).EndInit();
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

    private LabelControl lblClave;
    private TextEdit txtClave;
    private LabelControl lblValor;
    private MemoEdit memValor;
    private LabelControl lblDescripcion;
    private MemoEdit memDescripcion;
    private LabelControl lblTipoDato;
    private TextEdit txtTipoDato;
    private LabelControl lblCategoria;
    private TextEdit txtCategoria;
    private LabelControl lblOrden;
    private SpinEdit sedOrden;
    private LabelControl lblValorDefecto;
    private MemoEdit memValorDefecto;
    private LabelControl lblValidacion;
    private TextEdit txtValidacion;
    private CheckEdit chkEncriptado;
    private CheckEdit chkSistema;
    private CheckEdit chkEditable;
    private CheckEdit chkActivo;
    private new SimpleButton btnGuardar;
    private new SimpleButton btnCancelar;
}

