namespace NuanSystem.WinForms.Forms.Audit;

partial class AuditLogsForm
{
    private System.ComponentModel.IContainer components = null;
    private DevExpress.XtraEditors.PanelControl pnlFiltros;
    private DevExpress.XtraEditors.LabelControl lblRegistros;
    private DevExpress.XtraEditors.SpinEdit sedRegistros;
    private DevExpress.XtraEditors.SimpleButton btnActualizar;
    private DevExpress.XtraGrid.GridControl grcAuditoria;
    private DevExpress.XtraGrid.Views.Grid.GridView grvAuditoria;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlFiltros = new DevExpress.XtraEditors.PanelControl();
        lblRegistros = new DevExpress.XtraEditors.LabelControl();
        sedRegistros = new DevExpress.XtraEditors.SpinEdit();
        btnActualizar = new DevExpress.XtraEditors.SimpleButton();
        grcAuditoria = new DevExpress.XtraGrid.GridControl();
        grvAuditoria = new DevExpress.XtraGrid.Views.Grid.GridView();
        ((System.ComponentModel.ISupportInitialize)pnlFiltros).BeginInit();
        pnlFiltros.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sedRegistros.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grcAuditoria).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAuditoria).BeginInit();
        SuspendLayout();
        // 
        // pnlFiltros
        // 
        pnlFiltros.Appearance.BackColor = Color.White;
        pnlFiltros.Appearance.Options.UseBackColor = true;
        pnlFiltros.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlFiltros.Controls.Add(lblRegistros);
        pnlFiltros.Controls.Add(sedRegistros);
        pnlFiltros.Controls.Add(btnActualizar);
        pnlFiltros.Dock = DockStyle.Top;
        pnlFiltros.Location = new Point(0, 0);
        pnlFiltros.Name = "pnlFiltros";
        pnlFiltros.Padding = new Padding(8);
        pnlFiltros.Size = new Size(1120, 42);
        pnlFiltros.TabIndex = 0;
        // 
        // lblRegistros
        // 
        lblRegistros.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblRegistros.Appearance.Options.UseFont = true;
        lblRegistros.Location = new Point(8, 13);
        lblRegistros.Name = "lblRegistros";
        lblRegistros.Size = new Size(48, 15);
        lblRegistros.TabIndex = 0;
        lblRegistros.Text = "Registros";
        // 
        // sedRegistros
        // 
        sedRegistros.EditValue = new decimal(new int[] { 200, 0, 0, 0 });
        sedRegistros.Location = new Point(82, 8);
        sedRegistros.Name = "sedRegistros";
        sedRegistros.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        sedRegistros.Properties.Appearance.Options.UseFont = true;
        sedRegistros.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        sedRegistros.Properties.IsFloatValue = false;
        sedRegistros.Properties.MaskSettings.Set("mask", "N00");
        sedRegistros.Properties.MaxValue = new decimal(new int[] { 500, 0, 0, 0 });
        sedRegistros.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
        sedRegistros.Size = new Size(80, 22);
        sedRegistros.TabIndex = 1;
        // 
        // btnActualizar
        // 
        btnActualizar.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        btnActualizar.Appearance.Options.UseFont = true;
        btnActualizar.Location = new Point(176, 7);
        btnActualizar.Name = "btnActualizar";
        btnActualizar.Size = new Size(110, 28);
        btnActualizar.TabIndex = 2;
        btnActualizar.Text = "Actualizar";
        // 
        // grcAuditoria
        // 
        grcAuditoria.Dock = DockStyle.Fill;
        grcAuditoria.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grcAuditoria.Location = new Point(0, 42);
        grcAuditoria.MainView = grvAuditoria;
        grcAuditoria.Name = "grcAuditoria";
        grcAuditoria.Size = new Size(1120, 578);
        grcAuditoria.TabIndex = 1;
        grcAuditoria.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvAuditoria });
        // 
        // grvAuditoria
        // 
        grvAuditoria.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvAuditoria.Appearance.HeaderPanel.ForeColor = Color.Black;
        grvAuditoria.Appearance.HeaderPanel.Options.UseFont = true;
        grvAuditoria.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvAuditoria.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvAuditoria.Appearance.Row.Options.UseFont = true;
        grvAuditoria.GridControl = grcAuditoria;
        grvAuditoria.Name = "grvAuditoria";
        grvAuditoria.OptionsBehavior.Editable = false;
        grvAuditoria.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAuditoria.OptionsView.ShowGroupPanel = false;
        grvAuditoria.RowHeight = 22;
        // 
        // AuditLogsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1120, 620);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Controls.Add(grcAuditoria);
        Controls.Add(pnlFiltros);
        MinimumSize = new Size(860, 460);
        Name = "AuditLogsForm";
        Text = "Auditoria operativa";
        ((System.ComponentModel.ISupportInitialize)pnlFiltros).EndInit();
        pnlFiltros.ResumeLayout(false);
        pnlFiltros.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)sedRegistros.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grcAuditoria).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAuditoria).EndInit();

        // Tipografia estandar de GridView
        grvAuditoria.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvAuditoria.Appearance.FooterPanel.Options.UseFont = true;
        grvAuditoria.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvAuditoria.Appearance.FilterPanel.Options.UseFont = true;
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

