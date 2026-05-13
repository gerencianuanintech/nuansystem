using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.SecurityRoles;

partial class SecurityRoleEditForm
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
        lblOrden = new LabelControl();
        sedOrden = new SpinEdit();
        chkActivo = new CheckEdit();
        chkSistema = new CheckEdit();
        chkAsignable = new CheckEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedOrden.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkSistema.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAsignable.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblCodigo
        // 
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = BrandResources.Text;
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblCodigo.Location = new Point(29, 26);
        lblCodigo.Name = "lblCodigo";
        lblCodigo.Size = new Size(39, 15);
        lblCodigo.TabIndex = 0;
        lblCodigo.Text = "Codigo";
        // 
        // txtCodigo
        // 
        txtCodigo.Location = new Point(140, 24);
        txtCodigo.Name = "txtCodigo";
        txtCodigo.Size = new Size(360, 20);
        txtCodigo.TabIndex = 1;
        // 
        // lblNombre
        // 
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = BrandResources.Text;
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblNombre.Location = new Point(29, 52);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(44, 15);
        lblNombre.TabIndex = 2;
        lblNombre.Text = "Nombre";
        // 
        // txtNombre
        // 
        txtNombre.Location = new Point(140, 50);
        txtNombre.Name = "txtNombre";
        txtNombre.Size = new Size(360, 20);
        txtNombre.TabIndex = 3;
        // 
        // lblDescripcion
        // 
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = BrandResources.Text;
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblDescripcion.Location = new Point(29, 78);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(62, 15);
        lblDescripcion.TabIndex = 4;
        lblDescripcion.Text = "Descripcion";
        // 
        // memDescripcion
        // 
        memDescripcion.Location = new Point(140, 76);
        memDescripcion.Name = "memDescripcion";
        memDescripcion.Size = new Size(360, 58);
        memDescripcion.TabIndex = 5;
        // 
        // lblOrden
        // 
        lblOrden.Appearance.Font = new Font("Segoe UI", 9F);
        lblOrden.Appearance.ForeColor = BrandResources.Text;
        lblOrden.Appearance.Options.UseFont = true;
        lblOrden.Appearance.Options.UseForeColor = true;
        lblOrden.Location = new Point(29, 146);
        lblOrden.Name = "lblOrden";
        lblOrden.Size = new Size(33, 15);
        lblOrden.TabIndex = 6;
        lblOrden.Text = "Orden";
        // 
        // sedOrden
        // 
        sedOrden.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedOrden.Location = new Point(140, 144);
        sedOrden.Name = "sedOrden";
        sedOrden.Properties.IsFloatValue = false;
        sedOrden.Properties.MaskSettings.Set("mask", "N00");
        sedOrden.Properties.MaxValue = new decimal(new int[] { 100000, 0, 0, 0 });
        sedOrden.Size = new Size(100, 20);
        sedOrden.TabIndex = 7;
        // 
        // chkActivo
        // 
        chkActivo.EditValue = true;
        chkActivo.Location = new Point(137, 174);
        chkActivo.Name = "chkActivo";
        chkActivo.Properties.Caption = "Activo";
        chkActivo.Size = new Size(75, 20);
        chkActivo.TabIndex = 8;
        // 
        // chkSistema
        // 
        chkSistema.Location = new Point(218, 174);
        chkSistema.Name = "chkSistema";
        chkSistema.Properties.Caption = "Rol del sistema";
        chkSistema.Size = new Size(120, 20);
        chkSistema.TabIndex = 9;
        // 
        // chkAsignable
        // 
        chkAsignable.EditValue = true;
        chkAsignable.Location = new Point(344, 174);
        chkAsignable.Name = "chkAsignable";
        chkAsignable.Properties.Caption = "Asignable";
        chkAsignable.Size = new Size(90, 20);
        chkAsignable.TabIndex = 10;
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.Location = new Point(400, 218);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 12;
        btnGuardar.Text = "Guardar";
        // 
        // btnCancelar
        // 
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(294, 218);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 11;
        btnCancelar.Text = "Cancelar";
        // 
        // SecurityRoleEditForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(540, 281);
        Controls.Add(lblCodigo);
        Controls.Add(txtCodigo);
        Controls.Add(lblNombre);
        Controls.Add(txtNombre);
        Controls.Add(lblDescripcion);
        Controls.Add(memDescripcion);
        Controls.Add(lblOrden);
        Controls.Add(sedOrden);
        Controls.Add(chkActivo);
        Controls.Add(chkSistema);
        Controls.Add(chkAsignable);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "SecurityRoleEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nuevo rol";
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedOrden.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkSistema.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAsignable.Properties).EndInit();
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
    private LabelControl lblOrden;
    private SpinEdit sedOrden;
    private CheckEdit chkActivo;
    private CheckEdit chkSistema;
    private CheckEdit chkAsignable;
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}
