using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;

partial class ItemGroupEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCodigo = new LabelControl();
        txtCodigo = new TextEdit();
        lblNombre = new LabelControl();
        txtNombre = new TextEdit();
        lblDescripcion = new LabelControl();
        memDescripcion = new MemoEdit();
        lblCuentaInventario = new LabelControl();
        txtCuentaInventario = new TextEdit();
        lblCuentaCostoVentas = new LabelControl();
        txtCuentaCostoVentas = new TextEdit();
        lblCuentaVentas = new LabelControl();
        txtCuentaVentas = new TextEdit();
        lblCuentaCompras = new LabelControl();
        txtCuentaCompras = new TextEdit();
        lblGrupoSap = new LabelControl();
        txtGrupoSap = new TextEdit();
        lblCodigoSap = new LabelControl();
        txtCodigoSap = new TextEdit();
        chkActivo = new CheckEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaInventario.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaCostoVentas.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaVentas.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaCompras.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtGrupoSap.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCodigoSap.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblCodigo
        // 
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = Color.Black;
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblCodigo.Location = new Point(29, 26);
        lblCodigo.Name = "lblCodigo";
        lblCodigo.Size = new Size(94, 15);
        lblCodigo.TabIndex = 0;
        lblCodigo.Text = "Código del grupo";
        // 
        // txtCodigo
        // 
        txtCodigo.Location = new Point(190, 24);
        txtCodigo.Name = "txtCodigo";
        txtCodigo.Properties.MaxLength = 50;
        txtCodigo.Size = new Size(350, 20);
        txtCodigo.TabIndex = 1;
        // 
        // lblNombre
        // 
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = Color.Black;
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblNombre.Location = new Point(29, 52);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(96, 15);
        lblNombre.TabIndex = 2;
        lblNombre.Text = "Nombre del grupo";
        // 
        // txtNombre
        // 
        txtNombre.Location = new Point(190, 50);
        txtNombre.Name = "txtNombre";
        txtNombre.Properties.MaxLength = 150;
        txtNombre.Size = new Size(350, 20);
        txtNombre.TabIndex = 3;
        // 
        // lblDescripcion
        // 
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = Color.Black;
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblDescripcion.Location = new Point(29, 78);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(63, 15);
        lblDescripcion.TabIndex = 4;
        lblDescripcion.Text = "Descripción";
        // 
        // memDescripcion
        // 
        memDescripcion.Location = new Point(190, 76);
        memDescripcion.Name = "memDescripcion";
        memDescripcion.Properties.MaxLength = 500;
        memDescripcion.Size = new Size(350, 58);
        memDescripcion.TabIndex = 5;
        // 
        // lblCuentaInventario
        // 
        lblCuentaInventario.Appearance.Font = new Font("Segoe UI", 9F);
        lblCuentaInventario.Appearance.ForeColor = Color.Black;
        lblCuentaInventario.Appearance.Options.UseFont = true;
        lblCuentaInventario.Appearance.Options.UseForeColor = true;
        lblCuentaInventario.Location = new Point(29, 146);
        lblCuentaInventario.Name = "lblCuentaInventario";
        lblCuentaInventario.Size = new Size(145, 15);
        lblCuentaInventario.TabIndex = 6;
        lblCuentaInventario.Text = "Cuenta contable inventario";
        // 
        // txtCuentaInventario
        // 
        txtCuentaInventario.Location = new Point(190, 144);
        txtCuentaInventario.Name = "txtCuentaInventario";
        txtCuentaInventario.Properties.MaxLength = 120;
        txtCuentaInventario.Size = new Size(350, 20);
        txtCuentaInventario.TabIndex = 7;
        // 
        // lblCuentaCostoVentas
        // 
        lblCuentaCostoVentas.Appearance.Font = new Font("Segoe UI", 9F);
        lblCuentaCostoVentas.Appearance.ForeColor = Color.Black;
        lblCuentaCostoVentas.Appearance.Options.UseFont = true;
        lblCuentaCostoVentas.Appearance.Options.UseForeColor = true;
        lblCuentaCostoVentas.Location = new Point(29, 172);
        lblCuentaCostoVentas.Name = "lblCuentaCostoVentas";
        lblCuentaCostoVentas.Size = new Size(156, 15);
        lblCuentaCostoVentas.TabIndex = 8;
        lblCuentaCostoVentas.Text = "Cuenta contable costo ventas";
        // 
        // txtCuentaCostoVentas
        // 
        txtCuentaCostoVentas.Location = new Point(190, 170);
        txtCuentaCostoVentas.Name = "txtCuentaCostoVentas";
        txtCuentaCostoVentas.Properties.MaxLength = 120;
        txtCuentaCostoVentas.Size = new Size(350, 20);
        txtCuentaCostoVentas.TabIndex = 9;
        // 
        // lblCuentaVentas
        // 
        lblCuentaVentas.Appearance.Font = new Font("Segoe UI", 9F);
        lblCuentaVentas.Appearance.ForeColor = Color.Black;
        lblCuentaVentas.Appearance.Options.UseFont = true;
        lblCuentaVentas.Appearance.Options.UseForeColor = true;
        lblCuentaVentas.Location = new Point(29, 198);
        lblCuentaVentas.Name = "lblCuentaVentas";
        lblCuentaVentas.Size = new Size(119, 15);
        lblCuentaVentas.TabIndex = 10;
        lblCuentaVentas.Text = "Cuenta contable ventas";
        // 
        // txtCuentaVentas
        // 
        txtCuentaVentas.Location = new Point(190, 196);
        txtCuentaVentas.Name = "txtCuentaVentas";
        txtCuentaVentas.Properties.MaxLength = 120;
        txtCuentaVentas.Size = new Size(350, 20);
        txtCuentaVentas.TabIndex = 11;
        // 
        // lblCuentaCompras
        // 
        lblCuentaCompras.Appearance.Font = new Font("Segoe UI", 9F);
        lblCuentaCompras.Appearance.ForeColor = Color.Black;
        lblCuentaCompras.Appearance.Options.UseFont = true;
        lblCuentaCompras.Appearance.Options.UseForeColor = true;
        lblCuentaCompras.Location = new Point(29, 224);
        lblCuentaCompras.Name = "lblCuentaCompras";
        lblCuentaCompras.Size = new Size(128, 15);
        lblCuentaCompras.TabIndex = 12;
        lblCuentaCompras.Text = "Cuenta contable compras";
        // 
        // txtCuentaCompras
        // 
        txtCuentaCompras.Location = new Point(190, 222);
        txtCuentaCompras.Name = "txtCuentaCompras";
        txtCuentaCompras.Properties.MaxLength = 120;
        txtCuentaCompras.Size = new Size(350, 20);
        txtCuentaCompras.TabIndex = 13;
        // 
        // lblGrupoSap
        // 
        lblGrupoSap.Appearance.Font = new Font("Segoe UI", 9F);
        lblGrupoSap.Appearance.ForeColor = Color.Black;
        lblGrupoSap.Appearance.Options.UseFont = true;
        lblGrupoSap.Appearance.Options.UseForeColor = true;
        lblGrupoSap.Location = new Point(29, 250);
        lblGrupoSap.Name = "lblGrupoSap";
        lblGrupoSap.Size = new Size(125, 15);
        lblGrupoSap.TabIndex = 14;
        lblGrupoSap.Text = "Grupo SAP Business One";
        // 
        // txtGrupoSap
        // 
        txtGrupoSap.Location = new Point(190, 248);
        txtGrupoSap.Name = "txtGrupoSap";
        txtGrupoSap.Properties.MaxLength = 100;
        txtGrupoSap.Size = new Size(350, 20);
        txtGrupoSap.TabIndex = 15;
        // 
        // lblCodigoSap
        // 
        lblCodigoSap.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigoSap.Appearance.ForeColor = Color.Black;
        lblCodigoSap.Appearance.Options.UseFont = true;
        lblCodigoSap.Appearance.Options.UseForeColor = true;
        lblCodigoSap.Location = new Point(29, 276);
        lblCodigoSap.Name = "lblCodigoSap";
        lblCodigoSap.Size = new Size(60, 15);
        lblCodigoSap.TabIndex = 16;
        lblCodigoSap.Text = "Código SAP";
        // 
        // txtCodigoSap
        // 
        txtCodigoSap.Location = new Point(190, 274);
        txtCodigoSap.Name = "txtCodigoSap";
        txtCodigoSap.Properties.MaxLength = 50;
        txtCodigoSap.Size = new Size(350, 20);
        txtCodigoSap.TabIndex = 17;
        // 
        // chkActivo
        // 
        chkActivo.EditValue = true;
        chkActivo.Location = new Point(187, 304);
        chkActivo.Name = "chkActivo";
        chkActivo.Properties.Caption = "Activo";
        chkActivo.Size = new Size(75, 20);
        chkActivo.TabIndex = 18;
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.Location = new Point(440, 342);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 32);
        btnGuardar.TabIndex = 20;
        btnGuardar.Text = "Guardar";
        // 
        // btnCancelar
        // 
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(334, 342);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 32);
        btnCancelar.TabIndex = 19;
        btnCancelar.Text = "Cancelar";
        // 
        // ItemGroupEditForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(580, 397);
        Controls.Add(lblCodigo);
        Controls.Add(txtCodigo);
        Controls.Add(lblNombre);
        Controls.Add(txtNombre);
        Controls.Add(lblDescripcion);
        Controls.Add(memDescripcion);
        Controls.Add(lblCuentaInventario);
        Controls.Add(txtCuentaInventario);
        Controls.Add(lblCuentaCostoVentas);
        Controls.Add(txtCuentaCostoVentas);
        Controls.Add(lblCuentaVentas);
        Controls.Add(txtCuentaVentas);
        Controls.Add(lblCuentaCompras);
        Controls.Add(txtCuentaCompras);
        Controls.Add(lblGrupoSap);
        Controls.Add(txtGrupoSap);
        Controls.Add(lblCodigoSap);
        Controls.Add(txtCodigoSap);
        Controls.Add(chkActivo);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ItemGroupEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nuevo grupo de artículos";
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaInventario.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaCostoVentas.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaVentas.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCuentaCompras.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtGrupoSap.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCodigoSap.Properties).EndInit();
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

    private LabelControl lblCodigo;
    private TextEdit txtCodigo;
    private LabelControl lblNombre;
    private TextEdit txtNombre;
    private LabelControl lblDescripcion;
    private MemoEdit memDescripcion;
    private LabelControl lblCuentaInventario;
    private TextEdit txtCuentaInventario;
    private LabelControl lblCuentaCostoVentas;
    private TextEdit txtCuentaCostoVentas;
    private LabelControl lblCuentaVentas;
    private TextEdit txtCuentaVentas;
    private LabelControl lblCuentaCompras;
    private TextEdit txtCuentaCompras;
    private LabelControl lblGrupoSap;
    private TextEdit txtGrupoSap;
    private LabelControl lblCodigoSap;
    private TextEdit txtCodigoSap;
    private CheckEdit chkActivo;
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}
