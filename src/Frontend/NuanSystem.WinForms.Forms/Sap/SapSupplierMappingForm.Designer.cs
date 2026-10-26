using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sap;

partial class SapSupplierMappingForm
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl titleLabel;
    private LabelControl sourceLabel;
    private LabelControl sourceValueLabel;
    private LabelControl destinationLabel;
    private LabelControl destinationValueLabel;
    private LabelControl informationLabel;
    private LabelControl statusLabel;
    private GridControl entitiesGrid;
    private GridView entitiesView;
    private GridColumn sapFieldColumn;
    private GridColumn nuanFieldColumn;
    private GridColumn directionColumn;
    private GridColumn dataTypeColumn;
    private GridColumn requiredColumn;
    private GridColumn readOnlyColumn;
    private GridColumn transformationColumn;
    private GridColumn activeColumn;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        titleLabel = new LabelControl(); sourceLabel = new LabelControl(); sourceValueLabel = new LabelControl();
        destinationLabel = new LabelControl(); destinationValueLabel = new LabelControl(); informationLabel = new LabelControl(); statusLabel = new LabelControl();
        entitiesGrid = new GridControl(); entitiesView = new GridView(); sapFieldColumn = new GridColumn(); nuanFieldColumn = new GridColumn();
        directionColumn = new GridColumn(); dataTypeColumn = new GridColumn(); requiredColumn = new GridColumn(); readOnlyColumn = new GridColumn(); transformationColumn = new GridColumn(); activeColumn = new GridColumn();
        ((System.ComponentModel.ISupportInitialize)entitiesGrid).BeginInit(); ((System.ComponentModel.ISupportInitialize)entitiesView).BeginInit(); SuspendLayout();
        titleLabel.Appearance.Font = AppTypography.SectionFont; titleLabel.Appearance.ForeColor = BrandResources.Primary; titleLabel.Appearance.Options.UseFont = true; titleLabel.Appearance.Options.UseForeColor = true; titleLabel.Location = new Point(24, 18); titleLabel.Name = "titleLabel"; titleLabel.Size = new Size(230, 20); titleLabel.Text = "1. Campos y mapeo";
        sourceLabel.Appearance.Font = AppTypography.LabelFont; sourceLabel.Appearance.Options.UseFont = true; sourceLabel.Location = new Point(24, 57); sourceLabel.Size = new Size(80, 15); sourceLabel.Text = "Origen SAP:";
        sourceValueLabel.Appearance.Font = AppTypography.InputFont; sourceValueLabel.Appearance.Options.UseFont = true; sourceValueLabel.Location = new Point(120, 57); sourceValueLabel.Size = new Size(240, 15); sourceValueLabel.Text = "OCRD - Socios de negocio";
        destinationLabel.Appearance.Font = AppTypography.LabelFont; destinationLabel.Appearance.Options.UseFont = true; destinationLabel.Location = new Point(500, 57); destinationLabel.Size = new Size(100, 15); destinationLabel.Text = "Destino:";
        destinationValueLabel.Appearance.Font = AppTypography.InputFont; destinationValueLabel.Appearance.Options.UseFont = true; destinationValueLabel.Location = new Point(610, 57); destinationValueLabel.Size = new Size(220, 15); destinationValueLabel.Text = "Proveedor NuanSystem";
        informationLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; informationLabel.Appearance.Font = AppTypography.SmallFont; informationLabel.Appearance.ForeColor = BrandResources.MutedText; informationLabel.Appearance.Options.UseFont = true; informationLabel.Appearance.Options.UseForeColor = true; informationLabel.Location = new Point(24, 604); informationLabel.Size = new Size(850, 18); informationLabel.Text = "Los campos SAP de solo lectura no se modifican desde NuanSystem.";
        statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; statusLabel.Appearance.Font = AppTypography.SmallFont; statusLabel.Appearance.ForeColor = BrandResources.MutedText; statusLabel.Appearance.Options.UseFont = true; statusLabel.Appearance.Options.UseForeColor = true; statusLabel.Location = new Point(24, 632); statusLabel.Size = new Size(220, 18); statusLabel.Text = "Cargando...";
        entitiesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; entitiesGrid.Location = new Point(24, 92); entitiesGrid.MainView = entitiesView; entitiesGrid.Name = "entitiesGrid"; entitiesGrid.Size = new Size(1072, 492); entitiesGrid.TabIndex = 0; entitiesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { entitiesView });
        entitiesView.Appearance.HeaderPanel.Font = AppTypography.GridHeaderFont; entitiesView.Appearance.HeaderPanel.Options.UseFont = true; entitiesView.Appearance.Row.Font = AppTypography.GridRowFont; entitiesView.Appearance.Row.Options.UseFont = true; entitiesView.Columns.AddRange(new GridColumn[] { sapFieldColumn, nuanFieldColumn, directionColumn, dataTypeColumn, requiredColumn, readOnlyColumn, transformationColumn, activeColumn }); entitiesView.GridControl = entitiesGrid; entitiesView.Name = "entitiesView"; entitiesView.OptionsView.ColumnAutoWidth = false; entitiesView.OptionsView.ShowGroupPanel = false; entitiesView.OptionsView.ShowIndicator = false; entitiesView.OptionsBehavior.Editable = true;
        sapFieldColumn.Caption = "Campo SAP"; sapFieldColumn.FieldName = "SapField"; sapFieldColumn.Visible = true; sapFieldColumn.VisibleIndex = 0; sapFieldColumn.Width = 150;
        nuanFieldColumn.Caption = "Campo NuanSystem"; nuanFieldColumn.FieldName = "NuanField"; nuanFieldColumn.Visible = true; nuanFieldColumn.VisibleIndex = 1; nuanFieldColumn.Width = 220;
        directionColumn.Caption = "Dirección"; directionColumn.FieldName = "Direction"; directionColumn.Visible = true; directionColumn.VisibleIndex = 2; directionColumn.Width = 100;
        dataTypeColumn.Caption = "Tipo"; dataTypeColumn.FieldName = "DataType"; dataTypeColumn.Visible = true; dataTypeColumn.VisibleIndex = 3; dataTypeColumn.Width = 90;
        requiredColumn.Caption = "Requerido"; requiredColumn.FieldName = "IsRequired"; requiredColumn.Visible = true; requiredColumn.VisibleIndex = 4; requiredColumn.Width = 80;
        readOnlyColumn.Caption = "Solo lectura"; readOnlyColumn.FieldName = "IsReadOnly"; readOnlyColumn.Visible = true; readOnlyColumn.VisibleIndex = 5; readOnlyColumn.Width = 90;
        transformationColumn.Caption = "Transformación"; transformationColumn.FieldName = "TransformationKey"; transformationColumn.Visible = true; transformationColumn.VisibleIndex = 6; transformationColumn.Width = 170;
        activeColumn.Caption = "Activo"; activeColumn.FieldName = "IsActive"; activeColumn.Visible = true; activeColumn.VisibleIndex = 7; activeColumn.Width = 65;
        btnCancelar.Location = new Point(884, 628); btnGuardar.Location = new Point(990, 628);
        Appearance.BackColor = BrandResources.Background; Appearance.Options.UseBackColor = true; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(1120, 680); Controls.Add(titleLabel); Controls.Add(sourceLabel); Controls.Add(sourceValueLabel); Controls.Add(destinationLabel); Controls.Add(destinationValueLabel); Controls.Add(entitiesGrid); Controls.Add(informationLabel); Controls.Add(statusLabel); Font = AppTypography.BaseFont; FormBorderStyle = FormBorderStyle.Sizable; MinimumSize = new Size(960, 560); Name = "SapSupplierMappingForm"; Text = "Mapeo de proveedores SAP";
        ((System.ComponentModel.ISupportInitialize)entitiesGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)entitiesView).EndInit(); ResumeLayout(false); PerformLayout();
    }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
