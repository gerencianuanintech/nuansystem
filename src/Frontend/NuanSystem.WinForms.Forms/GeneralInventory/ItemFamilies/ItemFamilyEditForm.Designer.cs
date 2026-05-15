using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemFamilies;

partial class ItemFamilyEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblItemGroup = new LabelControl();
        sleItemGroup = new SearchLookUpEdit();
        grvItemGroupLookup = new GridView();
        lblCodigo = new LabelControl();
        txtCodigo = new TextEdit();
        lblNombre = new LabelControl();
        txtNombre = new TextEdit();
        lblDescripcion = new LabelControl();
        memDescripcion = new MemoEdit();
        lblGrupoSap = new LabelControl();
        txtGrupoSap = new TextEdit();
        lblCodigoSap = new LabelControl();
        txtCodigoSap = new TextEdit();
        chkActivo = new CheckEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)sleItemGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvItemGroupLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtGrupoSap.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCodigoSap.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        SuspendLayout();
        lblItemGroup.Location = new Point(29, 26);
        lblItemGroup.Text = "Grupo de articulos";
        sleItemGroup.Location = new Point(190, 24);
        sleItemGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sleItemGroup.Properties.DisplayMember = "DisplayText";
        sleItemGroup.Properties.NullText = "";
        sleItemGroup.Properties.PopupView = grvItemGroupLookup;
        sleItemGroup.Properties.ValueMember = "Id";
        sleItemGroup.Size = new Size(350, 20);
        grvItemGroupLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvItemGroupLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvItemGroupLookup.OptionsView.ShowGroupPanel = false;
        lblCodigo.Location = new Point(29, 52);
        lblCodigo.Text = "Codigo";
        txtCodigo.Location = new Point(190, 50);
        txtCodigo.Properties.MaxLength = 50;
        txtCodigo.Size = new Size(350, 20);
        lblNombre.Location = new Point(29, 78);
        lblNombre.Text = "Nombre";
        txtNombre.Location = new Point(190, 76);
        txtNombre.Properties.MaxLength = 150;
        txtNombre.Size = new Size(350, 20);
        lblDescripcion.Location = new Point(29, 104);
        lblDescripcion.Text = "Descripcion";
        memDescripcion.Location = new Point(190, 102);
        memDescripcion.Properties.MaxLength = 500;
        memDescripcion.Size = new Size(350, 58);
        lblGrupoSap.Location = new Point(29, 174);
        lblGrupoSap.Text = "Grupo SAP Business One";
        txtGrupoSap.Location = new Point(190, 172);
        txtGrupoSap.Properties.MaxLength = 100;
        txtGrupoSap.Size = new Size(350, 20);
        lblCodigoSap.Location = new Point(29, 200);
        lblCodigoSap.Text = "Codigo SAP";
        txtCodigoSap.Location = new Point(190, 198);
        txtCodigoSap.Properties.MaxLength = 50;
        txtCodigoSap.Size = new Size(350, 20);
        chkActivo.EditValue = true;
        chkActivo.Location = new Point(187, 226);
        chkActivo.Properties.Caption = "Activo";
        chkActivo.Size = new Size(75, 20);
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.Location = new Point(440, 266);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Size = new Size(100, 32);
        btnGuardar.Text = "Guardar";
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(334, 266);
        btnCancelar.Size = new Size(100, 32);
        btnCancelar.Text = "Cancelar";
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(580, 321);
        Controls.AddRange(new Control[] { lblItemGroup, sleItemGroup, lblCodigo, txtCodigo, lblNombre, txtNombre, lblDescripcion, memDescripcion, lblGrupoSap, txtGrupoSap, lblCodigoSap, txtCodigoSap, chkActivo, btnCancelar, btnGuardar });
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nueva linea/familia";
        ((System.ComponentModel.ISupportInitialize)sleItemGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvItemGroupLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).EndInit();
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

    private LabelControl lblItemGroup;
    private SearchLookUpEdit sleItemGroup;
    private GridView grvItemGroupLookup;
    private LabelControl lblCodigo;
    private TextEdit txtCodigo;
    private LabelControl lblNombre;
    private TextEdit txtNombre;
    private LabelControl lblDescripcion;
    private MemoEdit memDescripcion;
    private LabelControl lblGrupoSap;
    private TextEdit txtGrupoSap;
    private LabelControl lblCodigoSap;
    private TextEdit txtCodigoSap;
    private CheckEdit chkActivo;
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}
