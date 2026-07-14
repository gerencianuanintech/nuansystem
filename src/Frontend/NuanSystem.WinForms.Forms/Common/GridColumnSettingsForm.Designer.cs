using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Controls.Buttons;

namespace NuanSystem.WinForms.Forms.Common;

partial class GridColumnSettingsForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        grcColumnas = new GridControl();
        grvColumnas = new GridView();
        btnCancelar = new NuanActionButton();
        btnGuardar = new NuanActionButton();
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
        btnCancelar.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancelar.ButtonText = "Cancelar";
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(438, 406);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 1;
        btnCancelar.Text = "Cancelar";
        // 
        // btnGuardar
        // 
        btnGuardar.ButtonKind = NuanActionButtonKind.Save;
        btnGuardar.ButtonText = "Guardar";
        btnGuardar.Location = new Point(544, 406);
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

        // Tipografia estandar de GridView
        grvColumnas.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvColumnas.Appearance.HeaderPanel.Options.UseFont = true;
        grvColumnas.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvColumnas.Appearance.Row.Options.UseFont = true;
        grvColumnas.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvColumnas.Appearance.FooterPanel.Options.UseFont = true;
        grvColumnas.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvColumnas.Appearance.FilterPanel.Options.UseFont = true;
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
    private NuanActionButton btnCancelar;
    private NuanActionButton btnGuardar;
}
