using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace NuanSystem.WinForms.Forms.Common;

partial class GridColumnSettingsForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        grcColumnas = new GridControl();
        grvColumnas = new GridView();
        btnCancelar = new SimpleButton();
        btnGuardar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)grcColumnas).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvColumnas).BeginInit();
        SuspendLayout();
        // 
        // grcColumnas
        // 
        grcColumnas.Dock = DockStyle.Top;
        grcColumnas.Location = new Point(0, 0);
        grcColumnas.MainView = grvColumnas;
        grcColumnas.Name = "grcColumnas";
        grcColumnas.Size = new Size(650, 390);
        grcColumnas.TabIndex = 0;
        grcColumnas.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvColumnas });
        // 
        // grvColumnas
        // 
        grvColumnas.GridControl = grcColumnas;
        grvColumnas.Name = "grvColumnas";
        grvColumnas.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
        grvColumnas.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
        grvColumnas.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvColumnas.OptionsView.ShowGroupPanel = false;
        grvColumnas.OptionsView.ShowIndicator = false;
        grvColumnas.RowHeight = 22;
        grvColumnas.Columns.AddVisible("FieldName", "Campo").OptionsColumn.AllowEdit = false;
        grvColumnas.Columns.AddVisible("DefaultCaption", "Titulo original").OptionsColumn.AllowEdit = false;
        grvColumnas.Columns.AddVisible("Caption", "Titulo");
        grvColumnas.Columns.AddVisible("IsVisible", "Visible");
        grvColumnas.Columns.AddVisible("VisibleIndex", "Orden");
        grvColumnas.Columns.AddVisible("Width", "Ancho");
        // 
        // btnCancelar
        // 
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(438, 406);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 1;
        btnCancelar.Text = "Cancelar";
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.Location = new Point(544, 406);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 2;
        btnGuardar.Text = "Guardar";
        // 
        // GridColumnSettingsForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(650, 454);
        Controls.Add(btnGuardar);
        Controls.Add(btnCancelar);
        Controls.Add(grcColumnas);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "GridColumnSettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Personalizar columnas";
        ((System.ComponentModel.ISupportInitialize)grcColumnas).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvColumnas).EndInit();
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private GridControl grcColumnas;
    private GridView grvColumnas;
    private SimpleButton btnCancelar;
    private SimpleButton btnGuardar;
}
