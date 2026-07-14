using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Controls.Kpi;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync;

partial class SyncMonitorForm
{
    private System.ComponentModel.IContainer components = null;

    private PanelControl pnlHeader;
    private NuanKpiCardControl cardPending;
    private NuanKpiCardControl cardInProcess;
    private NuanKpiCardControl cardApplied;
    private NuanKpiCardControl cardError;
    private NuanKpiCardControl cardDeadLetter;
    private NuanKpiCardControl cardIgnored;
    private PanelControl pnlFilters;
    private LabelControl lblStatus;
    private ComboBoxEdit cmbStatus;
    private LabelControl lblEntity;
    private TextEdit txtEntity;
    private LabelControl lblBranch;
    private TextEdit txtBranch;
    private LabelControl lblFrom;
    private DateEdit dateFrom;
    private LabelControl lblTo;
    private DateEdit dateTo;
    private CheckEdit chkHasErrors;
    private CheckEdit chkDeadLetterOnly;
    private NuanActionButton btnApplyFilters;
    private NuanActionButton btnClearFilters;
    private PanelControl pnlContent;
    private PanelControl pnlGridHeader;
    private LabelControl lblOutboxTitle;
    private NuanDataGridControl outboxGrid;

    private void InitializeComponent()
    {
        pnlHeader = new PanelControl();
        cardIgnored = new NuanKpiCardControl();
        cardDeadLetter = new NuanKpiCardControl();
        cardError = new NuanKpiCardControl();
        cardApplied = new NuanKpiCardControl();
        cardInProcess = new NuanKpiCardControl();
        cardPending = new NuanKpiCardControl();
        pnlFilters = new PanelControl();
        lblStatus = new LabelControl();
        cmbStatus = new ComboBoxEdit();
        lblEntity = new LabelControl();
        txtEntity = new TextEdit();
        lblBranch = new LabelControl();
        txtBranch = new TextEdit();
        lblFrom = new LabelControl();
        dateFrom = new DateEdit();
        lblTo = new LabelControl();
        dateTo = new DateEdit();
        chkHasErrors = new CheckEdit();
        chkDeadLetterOnly = new CheckEdit();
        btnApplyFilters = new NuanActionButton();
        btnClearFilters = new NuanActionButton();
        pnlContent = new PanelControl();
        outboxGrid = new NuanDataGridControl();
        pnlGridHeader = new PanelControl();
        lblOutboxTitle = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).BeginInit();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).BeginInit();
        pnlFilters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEntity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBranch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkHasErrors.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkDeadLetterOnly.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlContent).BeginInit();
        pnlContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlGridHeader).BeginInit();
        pnlGridHeader.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.Appearance.BackColor = Color.White;
        pnlHeader.Appearance.Options.UseBackColor = true;
        pnlHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlHeader.Controls.Add(cardIgnored);
        pnlHeader.Controls.Add(cardDeadLetter);
        pnlHeader.Controls.Add(cardError);
        pnlHeader.Controls.Add(cardApplied);
        pnlHeader.Controls.Add(cardInProcess);
        pnlHeader.Controls.Add(cardPending);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1236, 143);
        pnlHeader.TabIndex = 0;
        // 
        // cardIgnored
        // 
        cardIgnored.Location = new Point(1022, 13);
        cardIgnored.MinimumSize = new Size(160, 88);
        cardIgnored.Name = "cardIgnored";
        cardIgnored.Size = new Size(196, 118);
        cardIgnored.TabIndex = 5;
        // 
        // cardDeadLetter
        // 
        cardDeadLetter.Location = new Point(820, 13);
        cardDeadLetter.MinimumSize = new Size(160, 88);
        cardDeadLetter.Name = "cardDeadLetter";
        cardDeadLetter.Size = new Size(196, 118);
        cardDeadLetter.TabIndex = 4;
        // 
        // cardError
        // 
        cardError.Location = new Point(618, 13);
        cardError.MinimumSize = new Size(160, 88);
        cardError.Name = "cardError";
        cardError.Size = new Size(196, 118);
        cardError.TabIndex = 3;
        // 
        // cardApplied
        // 
        cardApplied.Location = new Point(416, 13);
        cardApplied.MinimumSize = new Size(160, 88);
        cardApplied.Name = "cardApplied";
        cardApplied.Size = new Size(196, 118);
        cardApplied.TabIndex = 2;
        // 
        // cardInProcess
        // 
        cardInProcess.Location = new Point(214, 13);
        cardInProcess.MinimumSize = new Size(160, 88);
        cardInProcess.Name = "cardInProcess";
        cardInProcess.Size = new Size(196, 118);
        cardInProcess.TabIndex = 1;
        // 
        // cardPending
        // 
        cardPending.Location = new Point(12, 13);
        cardPending.MinimumSize = new Size(160, 88);
        cardPending.Name = "cardPending";
        cardPending.Size = new Size(196, 118);
        cardPending.TabIndex = 0;
        // 
        // pnlFilters
        // 
        pnlFilters.Appearance.BackColor = Color.White;
        pnlFilters.Appearance.Options.UseBackColor = true;
        pnlFilters.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        pnlFilters.Controls.Add(lblStatus);
        pnlFilters.Controls.Add(cmbStatus);
        pnlFilters.Controls.Add(lblEntity);
        pnlFilters.Controls.Add(txtEntity);
        pnlFilters.Controls.Add(lblBranch);
        pnlFilters.Controls.Add(txtBranch);
        pnlFilters.Controls.Add(lblFrom);
        pnlFilters.Controls.Add(dateFrom);
        pnlFilters.Controls.Add(lblTo);
        pnlFilters.Controls.Add(dateTo);
        pnlFilters.Controls.Add(chkHasErrors);
        pnlFilters.Controls.Add(chkDeadLetterOnly);
        pnlFilters.Controls.Add(btnApplyFilters);
        pnlFilters.Controls.Add(btnClearFilters);
        pnlFilters.Dock = DockStyle.Top;
        pnlFilters.Location = new Point(0, 143);
        pnlFilters.Name = "pnlFilters";
        pnlFilters.Padding = new Padding(16, 10, 16, 10);
        pnlFilters.Size = new Size(1236, 71);
        pnlFilters.TabIndex = 2;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblStatus.Appearance.ForeColor = Color.FromArgb(31, 41, 55);
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.Location = new Point(18, 14);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(35, 15);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "Estado";
        // 
        // cmbStatus
        // 
        cmbStatus.EditValue = "(Todos)";
        cmbStatus.Location = new Point(18, 35);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cmbStatus.Properties.Appearance.Options.UseFont = true;
        cmbStatus.Properties.Items.AddRange(new object[] { "(Todos)", "Pending", "InProcess", "Applied", "Error", "DeadLetter", "Ignored" });
        cmbStatus.Size = new Size(146, 22);
        cmbStatus.TabIndex = 1;
        // 
        // lblEntity
        // 
        lblEntity.Appearance.Font = new Font("Segoe UI", 9F);
        lblEntity.Appearance.ForeColor = Color.FromArgb(31, 41, 55);
        lblEntity.Appearance.Options.UseFont = true;
        lblEntity.Appearance.Options.UseForeColor = true;
        lblEntity.Location = new Point(188, 14);
        lblEntity.Name = "lblEntity";
        lblEntity.Size = new Size(40, 15);
        lblEntity.TabIndex = 2;
        lblEntity.Text = "Entidad";
        // 
        // txtEntity
        // 
        txtEntity.EditValue = "(Todas)";
        txtEntity.Location = new Point(188, 35);
        txtEntity.Name = "txtEntity";
        txtEntity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEntity.Properties.Appearance.Options.UseFont = true;
        txtEntity.Size = new Size(160, 22);
        txtEntity.TabIndex = 3;
        // 
        // lblBranch
        // 
        lblBranch.Appearance.Font = new Font("Segoe UI", 9F);
        lblBranch.Appearance.ForeColor = Color.FromArgb(31, 41, 55);
        lblBranch.Appearance.Options.UseFont = true;
        lblBranch.Appearance.Options.UseForeColor = true;
        lblBranch.Location = new Point(372, 14);
        lblBranch.Name = "lblBranch";
        lblBranch.Size = new Size(44, 15);
        lblBranch.TabIndex = 4;
        lblBranch.Text = "Sucursal";
        // 
        // txtBranch
        // 
        txtBranch.EditValue = "(Todas)";
        txtBranch.Location = new Point(372, 35);
        txtBranch.Name = "txtBranch";
        txtBranch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBranch.Properties.Appearance.Options.UseFont = true;
        txtBranch.Properties.ReadOnly = true;
        txtBranch.Size = new Size(166, 22);
        txtBranch.TabIndex = 5;
        // 
        // lblFrom
        // 
        lblFrom.Appearance.Font = new Font("Segoe UI", 9F);
        lblFrom.Appearance.ForeColor = Color.FromArgb(31, 41, 55);
        lblFrom.Appearance.Options.UseFont = true;
        lblFrom.Appearance.Options.UseForeColor = true;
        lblFrom.Location = new Point(562, 14);
        lblFrom.Name = "lblFrom";
        lblFrom.Size = new Size(32, 15);
        lblFrom.TabIndex = 6;
        lblFrom.Text = "Desde";
        // 
        // dateFrom
        // 
        dateFrom.EditValue = new DateTime(2026, 7, 9, 0, 0, 0, 0);
        dateFrom.Location = new Point(562, 35);
        dateFrom.Name = "dateFrom";
        dateFrom.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dateFrom.Properties.Appearance.Options.UseFont = true;
        dateFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dateFrom.Size = new Size(124, 22);
        dateFrom.TabIndex = 7;
        // 
        // lblTo
        // 
        lblTo.Appearance.Font = new Font("Segoe UI", 9F);
        lblTo.Appearance.ForeColor = Color.FromArgb(31, 41, 55);
        lblTo.Appearance.Options.UseFont = true;
        lblTo.Appearance.Options.UseForeColor = true;
        lblTo.Location = new Point(710, 14);
        lblTo.Name = "lblTo";
        lblTo.Size = new Size(30, 15);
        lblTo.TabIndex = 8;
        lblTo.Text = "Hasta";
        // 
        // dateTo
        // 
        dateTo.EditValue = new DateTime(2026, 7, 9, 0, 0, 0, 0);
        dateTo.Location = new Point(710, 35);
        dateTo.Name = "dateTo";
        dateTo.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dateTo.Properties.Appearance.Options.UseFont = true;
        dateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dateTo.Size = new Size(124, 22);
        dateTo.TabIndex = 9;
        // 
        // chkHasErrors
        // 
        chkHasErrors.Location = new Point(858, 20);
        chkHasErrors.Name = "chkHasErrors";
        chkHasErrors.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkHasErrors.Properties.Appearance.Options.UseFont = true;
        chkHasErrors.Properties.Caption = "Solo con errores";
        chkHasErrors.Size = new Size(136, 20);
        chkHasErrors.TabIndex = 10;
        // 
        // chkDeadLetterOnly
        // 
        chkDeadLetterOnly.Location = new Point(858, 46);
        chkDeadLetterOnly.Name = "chkDeadLetterOnly";
        chkDeadLetterOnly.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkDeadLetterOnly.Properties.Appearance.Options.UseFont = true;
        chkDeadLetterOnly.Properties.Caption = "Solo DeadLetter";
        chkDeadLetterOnly.Size = new Size(136, 20);
        chkDeadLetterOnly.TabIndex = 11;
        // 
        // btnApplyFilters
        // 
        btnApplyFilters.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnApplyFilters.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnApplyFilters.Appearance.ForeColor = Color.White;
        btnApplyFilters.Appearance.Options.UseBackColor = true;
        btnApplyFilters.Appearance.Options.UseFont = true;
        btnApplyFilters.Appearance.Options.UseForeColor = true;
        btnApplyFilters.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnApplyFilters.AppearanceHovered.ForeColor = Color.White;
        btnApplyFilters.AppearanceHovered.Options.UseBackColor = true;
        btnApplyFilters.AppearanceHovered.Options.UseForeColor = true;
        btnApplyFilters.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnApplyFilters.AppearancePressed.ForeColor = Color.White;
        btnApplyFilters.AppearancePressed.Options.UseBackColor = true;
        btnApplyFilters.AppearancePressed.Options.UseForeColor = true;
        btnApplyFilters.ButtonKind = NuanActionButtonKind.Save;
        btnApplyFilters.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        btnApplyFilters.ButtonText = "Buscar";
        btnApplyFilters.ImageOptions.ImageToTextAlignment = ImageAlignToText.None;
        btnApplyFilters.ImageOptions.ImageToTextIndent = 0;
        btnApplyFilters.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnApplyFilters.ImageOptions.SvgImageSize = new Size(24, 24);
        btnApplyFilters.Location = new Point(1000, 20);
        btnApplyFilters.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnApplyFilters.Name = "btnApplyFilters";
        btnApplyFilters.Size = new Size(100, 36);
        btnApplyFilters.TabIndex = 12;
        btnApplyFilters.Text = "Buscar";
        // 
        // btnClearFilters
        // 
        btnClearFilters.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnClearFilters.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnClearFilters.Appearance.ForeColor = Color.White;
        btnClearFilters.Appearance.Options.UseBackColor = true;
        btnClearFilters.Appearance.Options.UseFont = true;
        btnClearFilters.Appearance.Options.UseForeColor = true;
        btnClearFilters.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnClearFilters.AppearanceHovered.ForeColor = Color.White;
        btnClearFilters.AppearanceHovered.Options.UseBackColor = true;
        btnClearFilters.AppearanceHovered.Options.UseForeColor = true;
        btnClearFilters.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnClearFilters.AppearancePressed.ForeColor = Color.White;
        btnClearFilters.AppearancePressed.Options.UseBackColor = true;
        btnClearFilters.AppearancePressed.Options.UseForeColor = true;
        btnClearFilters.ButtonKind = NuanActionButtonKind.Cancel;
        btnClearFilters.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        btnClearFilters.ButtonText = "Limpiar";
        btnClearFilters.ImageOptions.ImageToTextAlignment = ImageAlignToText.None;
        btnClearFilters.ImageOptions.ImageToTextIndent = 0;
        btnClearFilters.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnClearFilters.ImageOptions.SvgImageSize = new Size(24, 24);
        btnClearFilters.Location = new Point(1106, 20);
        btnClearFilters.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnClearFilters.Name = "btnClearFilters";
        btnClearFilters.Size = new Size(100, 36);
        btnClearFilters.TabIndex = 13;
        btnClearFilters.Text = "Limpiar";
        // 
        // pnlContent
        // 
        pnlContent.Appearance.BackColor = Color.White;
        pnlContent.Appearance.Options.UseBackColor = true;
        pnlContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlContent.Controls.Add(outboxGrid);
        pnlContent.Controls.Add(pnlGridHeader);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(0, 214);
        pnlContent.Name = "pnlContent";
        pnlContent.Padding = new Padding(24, 0, 24, 18);
        pnlContent.Size = new Size(1236, 706);
        pnlContent.TabIndex = 3;
        // 
        // outboxGrid
        // 
        outboxGrid.Dock = DockStyle.Fill;
        outboxGrid.FormKey = "sync-monitor";
        outboxGrid.GridName = "OutboxGrid";
        outboxGrid.Location = new Point(24, 38);
        outboxGrid.Name = "outboxGrid";
        outboxGrid.PageSize = 50;
        outboxGrid.Size = new Size(1188, 650);
        outboxGrid.TabIndex = 1;
        // 
        // pnlGridHeader
        // 
        pnlGridHeader.Appearance.BackColor = Color.White;
        pnlGridHeader.Appearance.Options.UseBackColor = true;
        pnlGridHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlGridHeader.Controls.Add(lblOutboxTitle);
        pnlGridHeader.Dock = DockStyle.Top;
        pnlGridHeader.Location = new Point(24, 0);
        pnlGridHeader.Name = "pnlGridHeader";
        pnlGridHeader.Size = new Size(1188, 38);
        pnlGridHeader.TabIndex = 0;
        // 
        // lblOutboxTitle
        // 
        lblOutboxTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblOutboxTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblOutboxTitle.Appearance.Options.UseFont = true;
        lblOutboxTitle.Appearance.Options.UseForeColor = true;
        lblOutboxTitle.Location = new Point(0, 10);
        lblOutboxTitle.Name = "lblOutboxTitle";
        lblOutboxTitle.Size = new Size(140, 20);
        lblOutboxTitle.TabIndex = 0;
        lblOutboxTitle.Text = "Eventos SyncOutbox";
        // 
        // SyncMonitorForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1236, 920);
        Controls.Add(pnlContent);
        Controls.Add(pnlFilters);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1180, 760);
        Name = "SyncMonitorForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Monitor Sync";
        ((System.ComponentModel.ISupportInitialize)pnlHeader).EndInit();
        pnlHeader.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlFilters).EndInit();
        pnlFilters.ResumeLayout(false);
        pnlFilters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEntity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBranch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkHasErrors.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkDeadLetterOnly.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlContent).EndInit();
        pnlContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlGridHeader).EndInit();
        pnlGridHeader.ResumeLayout(false);
        pnlGridHeader.PerformLayout();
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
