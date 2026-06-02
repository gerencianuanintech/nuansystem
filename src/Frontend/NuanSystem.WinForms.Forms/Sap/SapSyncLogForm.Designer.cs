using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sap;

partial class SapSyncLogForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        grcSapLogs = new GridControl();
        grvSapLogs = new GridView();
        colId = new DevExpress.XtraGrid.Columns.GridColumn();
        colEntidad = new DevExpress.XtraGrid.Columns.GridColumn();
        colRegistro = new DevExpress.XtraGrid.Columns.GridColumn();
        colObjetoSap = new DevExpress.XtraGrid.Columns.GridColumn();
        colEstado = new DevExpress.XtraGrid.Columns.GridColumn();
        colMensaje = new DevExpress.XtraGrid.Columns.GridColumn();
        colDocEntry = new DevExpress.XtraGrid.Columns.GridColumn();
        colDocNum = new DevExpress.XtraGrid.Columns.GridColumn();
        colCreado = new DevExpress.XtraGrid.Columns.GridColumn();
        colSincronizado = new DevExpress.XtraGrid.Columns.GridColumn();
        ((System.ComponentModel.ISupportInitialize)grcSapLogs).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSapLogs).BeginInit();
        SuspendLayout();
        // 
        // grcSapLogs
        // 
        grcSapLogs.Dock = DockStyle.Fill;
        grcSapLogs.Location = new Point(0, 0);
        grcSapLogs.MainView = grvSapLogs;
        grcSapLogs.Name = "grcSapLogs";
        grcSapLogs.Size = new Size(980, 560);
        grcSapLogs.TabIndex = 0;
        grcSapLogs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSapLogs });
        // 
        // grvSapLogs
        // 
        grvSapLogs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colId, colEntidad, colRegistro, colObjetoSap, colEstado, colMensaje, colDocEntry, colDocNum, colCreado, colSincronizado });
        grvSapLogs.GridControl = grcSapLogs;
        grvSapLogs.Name = "grvSapLogs";
        grvSapLogs.OptionsBehavior.Editable = false;
        grvSapLogs.OptionsFind.AlwaysVisible = true;
        grvSapLogs.OptionsFind.FindNullPrompt = "Buscar...";
        grvSapLogs.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSapLogs.OptionsView.ShowGroupPanel = false;
        grvSapLogs.RowHeight = 22;
        // 
        // colId
        // 
        colId.Caption = "Id";
        colId.FieldName = "Id";
        colId.Visible = true;
        colId.VisibleIndex = 0;
        colId.Width = 70;
        // 
        // colEntidad
        // 
        colEntidad.Caption = "Entidad";
        colEntidad.FieldName = "EntityType";
        colEntidad.Visible = true;
        colEntidad.VisibleIndex = 1;
        colEntidad.Width = 110;
        // 
        // colRegistro
        // 
        colRegistro.Caption = "Registro";
        colRegistro.FieldName = "EntityId";
        colRegistro.Visible = true;
        colRegistro.VisibleIndex = 2;
        colRegistro.Width = 90;
        // 
        // colObjetoSap
        // 
        colObjetoSap.Caption = "Objeto SAP";
        colObjetoSap.FieldName = "SapObjectType";
        colObjetoSap.Visible = true;
        colObjetoSap.VisibleIndex = 3;
        colObjetoSap.Width = 110;
        // 
        // colEstado
        // 
        colEstado.Caption = "Estado";
        colEstado.FieldName = "Status";
        colEstado.Visible = true;
        colEstado.VisibleIndex = 4;
        colEstado.Width = 100;
        // 
        // colMensaje
        // 
        colMensaje.Caption = "Mensaje";
        colMensaje.FieldName = "ErrorMessage";
        colMensaje.Visible = true;
        colMensaje.VisibleIndex = 5;
        colMensaje.Width = 220;
        // 
        // colDocEntry
        // 
        colDocEntry.Caption = "DocEntry";
        colDocEntry.FieldName = "SapDocEntry";
        colDocEntry.Visible = true;
        colDocEntry.VisibleIndex = 6;
        colDocEntry.Width = 90;
        // 
        // colDocNum
        // 
        colDocNum.Caption = "DocNum";
        colDocNum.FieldName = "SapDocNum";
        colDocNum.Visible = true;
        colDocNum.VisibleIndex = 7;
        colDocNum.Width = 90;
        // 
        // colCreado
        // 
        colCreado.Caption = "Creado";
        colCreado.FieldName = "CreatedAtUtc";
        colCreado.Visible = true;
        colCreado.VisibleIndex = 8;
        colCreado.Width = 130;
        // 
        // colSincronizado
        // 
        colSincronizado.Caption = "Sincronizado";
        colSincronizado.FieldName = "SyncedAtUtc";
        colSincronizado.Visible = true;
        colSincronizado.VisibleIndex = 9;
        colSincronizado.Width = 130;
        // 
        // SapSyncLogForm
        // 
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(980, 560);
        Controls.Add(grcSapLogs);
        Font = new Font("Segoe UI", 9F);
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        MinimumSize = new Size(780, 420);
        Name = "SapSyncLogForm";
        Text = "Logs de integracion SAP";
        ((System.ComponentModel.ISupportInitialize)grcSapLogs).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSapLogs).EndInit();

        // Tipografia estandar de GridView
        grvSapLogs.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvSapLogs.Appearance.HeaderPanel.Options.UseFont = true;
        grvSapLogs.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvSapLogs.Appearance.Row.Options.UseFont = true;
        grvSapLogs.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvSapLogs.Appearance.FooterPanel.Options.UseFont = true;
        grvSapLogs.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvSapLogs.Appearance.FilterPanel.Options.UseFont = true;
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

    private GridControl grcSapLogs;
    private GridView grvSapLogs;
    private DevExpress.XtraGrid.Columns.GridColumn colId;
    private DevExpress.XtraGrid.Columns.GridColumn colEntidad;
    private DevExpress.XtraGrid.Columns.GridColumn colRegistro;
    private DevExpress.XtraGrid.Columns.GridColumn colObjetoSap;
    private DevExpress.XtraGrid.Columns.GridColumn colEstado;
    private DevExpress.XtraGrid.Columns.GridColumn colMensaje;
    private DevExpress.XtraGrid.Columns.GridColumn colDocEntry;
    private DevExpress.XtraGrid.Columns.GridColumn colDocNum;
    private DevExpress.XtraGrid.Columns.GridColumn colCreado;
    private DevExpress.XtraGrid.Columns.GridColumn colSincronizado;
}
