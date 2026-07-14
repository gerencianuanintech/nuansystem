using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class SyncExecutionDetailForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayout;
    private FlowLayoutPanel actionsPanel;
    private SimpleButton refreshButton;
    private SimpleButton cancelButton;
    private SimpleButton retryButton;
    private MemoEdit summaryEdit;
    private GridControl detailGrid;
    private GridView detailGridView;
    private GridColumn colEntityCode;
    private GridColumn colStatus;
    private GridColumn colStartedAt;
    private GridColumn colFinishedAt;
    private GridColumn colTotalRecordsRead;
    private GridColumn colTotalEventsPublished;
    private GridColumn colTotalSkipped;
    private GridColumn colTotalErrors;
    private GridColumn colLastProcessedKey;
    private GridColumn colMessage;
    private System.Windows.Forms.Timer pollingTimer;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        rootLayout = new TableLayoutPanel();
        actionsPanel = new FlowLayoutPanel();
        refreshButton = new SimpleButton();
        cancelButton = new SimpleButton();
        retryButton = new SimpleButton();
        summaryEdit = new MemoEdit();
        detailGrid = new GridControl();
        detailGridView = new GridView();
        colEntityCode = new GridColumn();
        colStatus = new GridColumn();
        colStartedAt = new GridColumn();
        colFinishedAt = new GridColumn();
        colTotalRecordsRead = new GridColumn();
        colTotalEventsPublished = new GridColumn();
        colTotalSkipped = new GridColumn();
        colTotalErrors = new GridColumn();
        colLastProcessedKey = new GridColumn();
        colMessage = new GridColumn();
        pollingTimer = new System.Windows.Forms.Timer(components);
        rootLayout.SuspendLayout();
        actionsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)detailGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)detailGridView).BeginInit();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(actionsPanel, 0, 0);
        rootLayout.Controls.Add(summaryEdit, 0, 1);
        rootLayout.Controls.Add(detailGrid, 0, 2);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(12);
        rootLayout.RowCount = 3;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.Size = new Size(980, 640);
        rootLayout.TabIndex = 0;
        // 
        // actionsPanel
        // 
        actionsPanel.Controls.Add(refreshButton);
        actionsPanel.Controls.Add(cancelButton);
        actionsPanel.Controls.Add(retryButton);
        actionsPanel.Dock = DockStyle.Fill;
        actionsPanel.FlowDirection = FlowDirection.LeftToRight;
        actionsPanel.Location = new Point(15, 15);
        actionsPanel.Name = "actionsPanel";
        actionsPanel.Size = new Size(950, 36);
        actionsPanel.TabIndex = 0;
        // 
        // buttons
        // 
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(92, 28);
        refreshButton.Text = "Actualizar";
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(92, 28);
        cancelButton.Text = "Cancelar";
        retryButton.Name = "retryButton";
        retryButton.Size = new Size(92, 28);
        retryButton.Text = "Reintentar";
        // 
        // summaryEdit
        // 
        summaryEdit.Dock = DockStyle.Fill;
        summaryEdit.Location = new Point(15, 57);
        summaryEdit.Name = "summaryEdit";
        summaryEdit.Properties.ReadOnly = true;
        summaryEdit.Size = new Size(950, 144);
        summaryEdit.TabIndex = 1;
        // 
        // detailGrid
        // 
        detailGrid.Dock = DockStyle.Fill;
        detailGrid.Location = new Point(15, 207);
        detailGrid.MainView = detailGridView;
        detailGrid.Name = "detailGrid";
        detailGrid.Size = new Size(950, 418);
        detailGrid.TabIndex = 2;
        detailGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { detailGridView });
        // 
        // detailGridView
        // 
        detailGridView.Columns.AddRange(new GridColumn[] { colEntityCode, colStatus, colStartedAt, colFinishedAt, colTotalRecordsRead, colTotalEventsPublished, colTotalSkipped, colTotalErrors, colLastProcessedKey, colMessage });
        detailGridView.GridControl = detailGrid;
        detailGridView.Name = "detailGridView";
        detailGridView.OptionsBehavior.Editable = false;
        detailGridView.OptionsView.ShowGroupPanel = false;
        // 
        // columns
        // 
        colEntityCode.Caption = "Entidad";
        colEntityCode.FieldName = "EntityCode";
        colEntityCode.Visible = true;
        colEntityCode.VisibleIndex = 0;
        colEntityCode.Width = 160;
        colStatus.Caption = "Estado";
        colStatus.FieldName = "Status";
        colStatus.Visible = true;
        colStatus.VisibleIndex = 1;
        colStatus.Width = 110;
        colStartedAt.Caption = "Inicio";
        colStartedAt.FieldName = "StartedAt";
        colStartedAt.Visible = true;
        colStartedAt.VisibleIndex = 2;
        colStartedAt.Width = 150;
        colFinishedAt.Caption = "Fin";
        colFinishedAt.FieldName = "FinishedAt";
        colFinishedAt.Visible = true;
        colFinishedAt.VisibleIndex = 3;
        colFinishedAt.Width = 150;
        colTotalRecordsRead.Caption = "Leidos";
        colTotalRecordsRead.FieldName = "TotalRecordsRead";
        colTotalRecordsRead.Visible = true;
        colTotalRecordsRead.VisibleIndex = 4;
        colTotalRecordsRead.Width = 90;
        colTotalEventsPublished.Caption = "Publicados";
        colTotalEventsPublished.FieldName = "TotalEventsPublished";
        colTotalEventsPublished.Visible = true;
        colTotalEventsPublished.VisibleIndex = 5;
        colTotalEventsPublished.Width = 90;
        colTotalSkipped.Caption = "Omitidos";
        colTotalSkipped.FieldName = "TotalSkipped";
        colTotalSkipped.Visible = true;
        colTotalSkipped.VisibleIndex = 6;
        colTotalSkipped.Width = 90;
        colTotalErrors.Caption = "Errores";
        colTotalErrors.FieldName = "TotalErrors";
        colTotalErrors.Visible = true;
        colTotalErrors.VisibleIndex = 7;
        colTotalErrors.Width = 90;
        colLastProcessedKey.Caption = "Ultima clave";
        colLastProcessedKey.FieldName = "LastProcessedKey";
        colLastProcessedKey.Visible = true;
        colLastProcessedKey.VisibleIndex = 8;
        colLastProcessedKey.Width = 130;
        colMessage.Caption = "Mensaje";
        colMessage.FieldName = "Message";
        colMessage.Visible = true;
        colMessage.VisibleIndex = 9;
        colMessage.Width = 260;
        // 
        // pollingTimer
        // 
        pollingTimer.Enabled = false;
        pollingTimer.Interval = 7000;
        // 
        // SyncExecutionDetailForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(980, 640);
        Controls.Add(rootLayout);
        MinimumSize = new Size(860, 520);
        Name = "SyncExecutionDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Detalle de ejecucion";
        rootLayout.ResumeLayout(false);
        actionsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)detailGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)detailGridView).EndInit();
        ResumeLayout(false);
    }
}
