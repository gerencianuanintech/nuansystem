using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Controls.Buttons;

namespace NuanSystem.WinForms.Forms.Sap;

partial class SapSyncExecutionDetailForm
{
    private System.ComponentModel.IContainer components = null; private PanelControl actionsPanel; private NuanActionButton refreshButton; private NuanActionButton retryButton; private NuanActionButton cancelButton; private NuanActionButton releaseButton; private MemoEdit summaryEdit; private GridControl detailGrid; private GridView detailView; private System.Windows.Forms.Timer pollingTimer;
    private GridColumn sourceColumn; private GridColumn actionColumn; private GridColumn statusColumn; private GridColumn attemptsColumn; private GridColumn resultColumn; private GridColumn messageColumn; private GridColumn nextAttemptColumn; private GridColumn finishedColumn;
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container(); actionsPanel = new PanelControl(); refreshButton = new NuanActionButton(); retryButton = new NuanActionButton(); cancelButton = new NuanActionButton(); releaseButton = new NuanActionButton(); summaryEdit = new MemoEdit(); detailGrid = new GridControl(); detailView = new GridView(); pollingTimer = new System.Windows.Forms.Timer(components); sourceColumn = new GridColumn(); actionColumn = new GridColumn(); statusColumn = new GridColumn(); attemptsColumn = new GridColumn(); resultColumn = new GridColumn(); messageColumn = new GridColumn(); nextAttemptColumn = new GridColumn(); finishedColumn = new GridColumn();
        ((System.ComponentModel.ISupportInitialize)actionsPanel).BeginInit(); actionsPanel.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)detailGrid).BeginInit(); ((System.ComponentModel.ISupportInitialize)detailView).BeginInit(); SuspendLayout();
        actionsPanel.Controls.AddRange(new Control[] { refreshButton, retryButton, cancelButton, releaseButton }); actionsPanel.Dock = DockStyle.Top; actionsPanel.Location = new Point(0, 0); actionsPanel.Name = "actionsPanel"; actionsPanel.Size = new Size(1080, 52); actionsPanel.TabIndex = 0;
        refreshButton.ButtonKind = NuanActionButtonKind.Refresh; refreshButton.ButtonText = "Actualizar"; refreshButton.Location = new Point(12, 8); refreshButton.Name = "refreshButton"; refreshButton.Size = new Size(104, 36); refreshButton.TabIndex = 0; refreshButton.Text = "Actualizar";
        retryButton.ButtonKind = NuanActionButtonKind.Retry; retryButton.ButtonText = "Reintentar"; retryButton.Location = new Point(124, 8); retryButton.Name = "retryButton"; retryButton.Size = new Size(104, 36); retryButton.TabIndex = 1; retryButton.Text = "Reintentar";
        cancelButton.ButtonKind = NuanActionButtonKind.Cancel; cancelButton.ButtonText = "Cancelar"; cancelButton.Location = new Point(236, 8); cancelButton.Name = "cancelButton"; cancelButton.Size = new Size(104, 36); cancelButton.TabIndex = 2; cancelButton.Text = "Cancelar";
        releaseButton.ButtonKind = NuanActionButtonKind.Warning; releaseButton.ButtonText = "Liberar lock"; releaseButton.Location = new Point(348, 8); releaseButton.Name = "releaseButton"; releaseButton.Size = new Size(104, 36); releaseButton.TabIndex = 3; releaseButton.Text = "Liberar lock";
        summaryEdit.Dock = DockStyle.Top; summaryEdit.Location = new Point(0, 52); summaryEdit.Name = "summaryEdit"; summaryEdit.Properties.ReadOnly = true; summaryEdit.Size = new Size(1080, 150); summaryEdit.TabIndex = 1;
        detailGrid.Dock = DockStyle.Fill; detailGrid.Location = new Point(0, 202); detailGrid.MainView = detailView; detailGrid.Name = "detailGrid"; detailGrid.Size = new Size(1080, 458); detailGrid.TabIndex = 2; detailGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { detailView });
        detailView.Columns.AddRange(new GridColumn[] { sourceColumn, actionColumn, statusColumn, attemptsColumn, resultColumn, messageColumn, nextAttemptColumn, finishedColumn }); detailView.GridControl = detailGrid; detailView.Name = "detailView"; detailView.OptionsBehavior.Editable = false; detailView.OptionsView.ShowGroupPanel = false; detailView.OptionsView.ColumnAutoWidth = false;
        sourceColumn.Caption = "Clave SAP"; sourceColumn.FieldName = "SourceRecordKey"; sourceColumn.Visible = true; sourceColumn.VisibleIndex = 0; sourceColumn.Width = 135;
        actionColumn.Caption = "Accion"; actionColumn.FieldName = "Action"; actionColumn.Visible = true; actionColumn.VisibleIndex = 1; actionColumn.Width = 90;
        statusColumn.Caption = "Estado"; statusColumn.FieldName = "Status"; statusColumn.Visible = true; statusColumn.VisibleIndex = 2; statusColumn.Width = 130;
        attemptsColumn.Caption = "Intentos"; attemptsColumn.FieldName = "AttemptCount"; attemptsColumn.Visible = true; attemptsColumn.VisibleIndex = 3; attemptsColumn.Width = 70;
        resultColumn.Caption = "Resultado"; resultColumn.FieldName = "ResultCode"; resultColumn.Visible = true; resultColumn.VisibleIndex = 4; resultColumn.Width = 120;
        messageColumn.Caption = "Mensaje"; messageColumn.FieldName = "SafeMessage"; messageColumn.Visible = true; messageColumn.VisibleIndex = 5; messageColumn.Width = 300;
        nextAttemptColumn.Caption = "Proximo intento"; nextAttemptColumn.FieldName = "NextAttemptAtUtc"; nextAttemptColumn.Visible = true; nextAttemptColumn.VisibleIndex = 6; nextAttemptColumn.Width = 145;
        finishedColumn.Caption = "Finalizado"; finishedColumn.FieldName = "FinishedAtUtc"; finishedColumn.Visible = true; finishedColumn.VisibleIndex = 7; finishedColumn.Width = 145;
        pollingTimer.Enabled = false; pollingTimer.Interval = 7000;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(1080, 660); Controls.Add(detailGrid); Controls.Add(summaryEdit); Controls.Add(actionsPanel); MinimumSize = new Size(900, 560); Name = "SapSyncExecutionDetailForm"; StartPosition = FormStartPosition.CenterParent; Text = "Detalle de ejecucion SAP";
        ((System.ComponentModel.ISupportInitialize)actionsPanel).EndInit(); actionsPanel.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)detailGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)detailView).EndInit(); ResumeLayout(false);
    }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
