namespace NuanSystem.WinForms.Forms.Audit;

partial class RecordHistoryForm
{
    private System.ComponentModel.IContainer components = null;
    private BindingSource bdsHistorial;
    private DevExpress.XtraEditors.PanelControl pnlEncabezado;
    private DevExpress.XtraEditors.LabelControl lblTitulo;
    private DevExpress.XtraEditors.LabelControl lblSubtitulo;
    private DevExpress.XtraEditors.SimpleButton btnActualizar;
    private DevExpress.XtraEditors.PanelControl pnlFiltros;
    private DevExpress.XtraEditors.ComboBoxEdit cmbAccion;
    private DevExpress.XtraEditors.ComboBoxEdit cmbUsuario;
    private DevExpress.XtraEditors.LabelControl lblTotalRegistros;
    private DevExpress.XtraGrid.GridControl grcHistorial;
    private DevExpress.XtraGrid.Views.Grid.GridView grvHistorial;
    private DevExpress.XtraGrid.Columns.GridColumn colFecha;
    private DevExpress.XtraGrid.Columns.GridColumn colUsuario;
    private DevExpress.XtraGrid.Columns.GridColumn colAccion;
    private DevExpress.XtraGrid.Columns.GridColumn colCampo;
    private DevExpress.XtraGrid.Columns.GridColumn colValorAnterior;
    private DevExpress.XtraGrid.Columns.GridColumn colValorNuevo;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        bdsHistorial = new BindingSource(components);
        pnlEncabezado = new DevExpress.XtraEditors.PanelControl();
        lblTitulo = new DevExpress.XtraEditors.LabelControl();
        lblSubtitulo = new DevExpress.XtraEditors.LabelControl();
        btnActualizar = new DevExpress.XtraEditors.SimpleButton();
        pnlFiltros = new DevExpress.XtraEditors.PanelControl();
        cmbAccion = new DevExpress.XtraEditors.ComboBoxEdit();
        cmbUsuario = new DevExpress.XtraEditors.ComboBoxEdit();
        lblTotalRegistros = new DevExpress.XtraEditors.LabelControl();
        grcHistorial = new DevExpress.XtraGrid.GridControl();
        grvHistorial = new DevExpress.XtraGrid.Views.Grid.GridView();
        colFecha = new DevExpress.XtraGrid.Columns.GridColumn();
        colUsuario = new DevExpress.XtraGrid.Columns.GridColumn();
        colAccion = new DevExpress.XtraGrid.Columns.GridColumn();
        colCampo = new DevExpress.XtraGrid.Columns.GridColumn();
        colValorAnterior = new DevExpress.XtraGrid.Columns.GridColumn();
        colValorNuevo = new DevExpress.XtraGrid.Columns.GridColumn();
        ((System.ComponentModel.ISupportInitialize)bdsHistorial).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlEncabezado).BeginInit();
        pnlEncabezado.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFiltros).BeginInit();
        pnlFiltros.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)cmbAccion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbUsuario.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grcHistorial).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvHistorial).BeginInit();
        SuspendLayout();
        // 
        // pnlEncabezado
        // 
        pnlEncabezado.Appearance.BackColor = Color.White;
        pnlEncabezado.Appearance.Options.UseBackColor = true;
        pnlEncabezado.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlEncabezado.Controls.Add(lblTitulo);
        pnlEncabezado.Controls.Add(lblSubtitulo);
        pnlEncabezado.Controls.Add(btnActualizar);
        pnlEncabezado.Dock = DockStyle.Top;
        pnlEncabezado.Location = new Point(0, 0);
        pnlEncabezado.Name = "pnlEncabezado";
        pnlEncabezado.Padding = new Padding(24, 16, 24, 10);
        pnlEncabezado.Size = new Size(1040, 82);
        pnlEncabezado.TabIndex = 0;
        // 
        // lblTitulo
        // 
        lblTitulo.Appearance.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitulo.Appearance.ForeColor = NuanSystem.WinForms.Forms.Common.BrandResources.Text;
        lblTitulo.Appearance.Options.UseFont = true;
        lblTitulo.Appearance.Options.UseForeColor = true;
        lblTitulo.Location = new Point(24, 18);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new Size(216, 28);
        lblTitulo.TabIndex = 0;
        lblTitulo.Text = "Historial de operacion";
        // 
        // lblSubtitulo
        // 
        lblSubtitulo.Appearance.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblSubtitulo.Appearance.ForeColor = NuanSystem.WinForms.Forms.Common.BrandResources.Text;
        lblSubtitulo.Appearance.Options.UseFont = true;
        lblSubtitulo.Appearance.Options.UseForeColor = true;
        lblSubtitulo.Location = new Point(26, 50);
        lblSubtitulo.Name = "lblSubtitulo";
        lblSubtitulo.Size = new Size(127, 17);
        lblSubtitulo.TabIndex = 1;
        lblSubtitulo.Text = "ACTION.NEW - Nuevo";
        // 
        // btnActualizar
        // 
        btnActualizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnActualizar.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        btnActualizar.Appearance.Options.UseFont = true;
        btnActualizar.Location = new Point(902, 26);
        btnActualizar.Name = "btnActualizar";
        btnActualizar.Size = new Size(108, 30);
        btnActualizar.TabIndex = 2;
        btnActualizar.Text = "Actualizar";
        // 
        // pnlFiltros
        // 
        pnlFiltros.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        pnlFiltros.Appearance.Options.UseBackColor = true;
        pnlFiltros.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlFiltros.Controls.Add(cmbAccion);
        pnlFiltros.Controls.Add(cmbUsuario);
        pnlFiltros.Controls.Add(lblTotalRegistros);
        pnlFiltros.Dock = DockStyle.Top;
        pnlFiltros.Location = new Point(0, 82);
        pnlFiltros.Name = "pnlFiltros";
        pnlFiltros.Padding = new Padding(20, 10, 20, 10);
        pnlFiltros.Size = new Size(1040, 66);
        pnlFiltros.TabIndex = 1;
        // 
        // cmbAccion
        // 
        cmbAccion.Location = new Point(20, 20);
        cmbAccion.Name = "cmbAccion";
        cmbAccion.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        cmbAccion.Properties.Appearance.Options.UseFont = true;
        cmbAccion.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cmbAccion.Properties.Items.AddRange(new object[] { "Todas las acciones" });
        cmbAccion.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cmbAccion.Size = new Size(410, 20);
        cmbAccion.TabIndex = 0;
        cmbAccion.SelectedIndex = 0;
        // 
        // cmbUsuario
        // 
        cmbUsuario.Location = new Point(442, 20);
        cmbUsuario.Name = "cmbUsuario";
        cmbUsuario.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        cmbUsuario.Properties.Appearance.Options.UseFont = true;
        cmbUsuario.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cmbUsuario.Properties.Items.AddRange(new object[] { "Todos los usuarios" });
        cmbUsuario.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cmbUsuario.Size = new Size(410, 20);
        cmbUsuario.TabIndex = 1;
        cmbUsuario.SelectedIndex = 0;
        // 
        // lblTotalRegistros
        // 
        lblTotalRegistros.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblTotalRegistros.Appearance.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblTotalRegistros.Appearance.ForeColor = NuanSystem.WinForms.Forms.Common.BrandResources.Text;
        lblTotalRegistros.Appearance.Options.UseFont = true;
        lblTotalRegistros.Appearance.Options.UseForeColor = true;
        lblTotalRegistros.Appearance.Options.UseTextOptions = true;
        lblTotalRegistros.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblTotalRegistros.Location = new Point(890, 23);
        lblTotalRegistros.Name = "lblTotalRegistros";
        lblTotalRegistros.Size = new Size(74, 15);
        lblTotalRegistros.TabIndex = 2;
        lblTotalRegistros.Text = "0 registros";
        // 
        // grcHistorial
        // 
        grcHistorial.DataSource = bdsHistorial;
        grcHistorial.Dock = DockStyle.Fill;
        grcHistorial.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grcHistorial.Location = new Point(0, 148);
        grcHistorial.MainView = grvHistorial;
        grcHistorial.Name = "grcHistorial";
        grcHistorial.Size = new Size(1040, 412);
        grcHistorial.TabIndex = 2;
        grcHistorial.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvHistorial });
        // 
        // grvHistorial
        // 
        grvHistorial.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvHistorial.Appearance.HeaderPanel.ForeColor = Color.Black;
        grvHistorial.Appearance.HeaderPanel.Options.UseFont = true;
        grvHistorial.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvHistorial.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvHistorial.Appearance.Row.ForeColor = NuanSystem.WinForms.Forms.Common.BrandResources.Text;
        grvHistorial.Appearance.Row.Options.UseFont = true;
        grvHistorial.Appearance.Row.Options.UseForeColor = true;
        grvHistorial.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colFecha, colUsuario, colAccion, colCampo, colValorAnterior, colValorNuevo });
        grvHistorial.GridControl = grcHistorial;
        grvHistorial.Name = "grvHistorial";
        grvHistorial.OptionsBehavior.Editable = false;
        grvHistorial.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvHistorial.OptionsView.ColumnAutoWidth = false;
        grvHistorial.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
        grvHistorial.OptionsView.RowAutoHeight = false;
        grvHistorial.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Auto;
        grvHistorial.OptionsView.ShowGroupPanel = false;
        grvHistorial.RowHeight = 22;
        // 
        // colFecha
        // 
        colFecha.Caption = "Fecha";
        colFecha.FieldName = "CreatedAtText";
        colFecha.Name = "colFecha";
        colFecha.OptionsColumn.FixedWidth = true;
        colFecha.Visible = true;
        colFecha.VisibleIndex = 0;
        colFecha.Width = 122;
        // 
        // colUsuario
        // 
        colUsuario.Caption = "Usuario";
        colUsuario.FieldName = "UserName";
        colUsuario.Name = "colUsuario";
        colUsuario.OptionsColumn.FixedWidth = true;
        colUsuario.Visible = true;
        colUsuario.VisibleIndex = 1;
        colUsuario.Width = 105;
        // 
        // colAccion
        // 
        colAccion.Caption = "Accion";
        colAccion.FieldName = "Action";
        colAccion.Name = "colAccion";
        colAccion.OptionsColumn.FixedWidth = true;
        colAccion.Visible = true;
        colAccion.VisibleIndex = 2;
        colAccion.Width = 95;
        // 
        // colCampo
        // 
        colCampo.Caption = "Campo";
        colCampo.FieldName = "FieldName";
        colCampo.Name = "colCampo";
        colCampo.OptionsColumn.FixedWidth = true;
        colCampo.Visible = true;
        colCampo.VisibleIndex = 3;
        colCampo.Width = 120;
        // 
        // colValorAnterior
        // 
        colValorAnterior.Caption = "Valor anterior";
        colValorAnterior.FieldName = "OldValue";
        colValorAnterior.Name = "colValorAnterior";
        colValorAnterior.OptionsColumn.FixedWidth = true;
        colValorAnterior.Visible = true;
        colValorAnterior.VisibleIndex = 4;
        colValorAnterior.Width = 190;
        // 
        // colValorNuevo
        // 
        colValorNuevo.Caption = "Valor nuevo";
        colValorNuevo.FieldName = "NewValue";
        colValorNuevo.Name = "colValorNuevo";
        colValorNuevo.OptionsColumn.FixedWidth = true;
        colValorNuevo.Visible = true;
        colValorNuevo.VisibleIndex = 5;
        colValorNuevo.Width = 200;
        // 
        // RecordHistoryForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1040, 560);
        Controls.Add(grcHistorial);
        Controls.Add(pnlFiltros);
        Controls.Add(pnlEncabezado);
        MinimumSize = new Size(900, 460);
        Name = "RecordHistoryForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Historial de operacion";
        ((System.ComponentModel.ISupportInitialize)bdsHistorial).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlEncabezado).EndInit();
        pnlEncabezado.ResumeLayout(false);
        pnlEncabezado.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFiltros).EndInit();
        pnlFiltros.ResumeLayout(false);
        pnlFiltros.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)cmbAccion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbUsuario.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grcHistorial).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvHistorial).EndInit();
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
}
