using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Controls.Grids;

namespace NuanSystem.WinForms.Forms.Sap;

#nullable enable
partial class SapSyncExecutionDetailForm
{
    private System.ComponentModel.IContainer? components;
    private PanelControl actionsPanel = null!;
    private NuanActionButton refreshButton = null!;
    private NuanActionButton retryButton = null!;
    private NuanActionButton cancelButton = null!;
    private NuanActionButton releaseButton = null!;
    private MemoEdit summaryEdit = null!;
    private NuanDataGridControl detailGrid = null!;
    private System.Windows.Forms.Timer pollingTimer = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        actionsPanel = new PanelControl();
        refreshButton = new NuanActionButton();
        retryButton = new NuanActionButton();
        cancelButton = new NuanActionButton();
        releaseButton = new NuanActionButton();
        summaryEdit = new MemoEdit();
        detailGrid = new NuanDataGridControl();
        pollingTimer = new System.Windows.Forms.Timer(components);
        ((System.ComponentModel.ISupportInitialize)actionsPanel).BeginInit();
        actionsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).BeginInit();
        SuspendLayout();
        //
        // actionsPanel
        //
        actionsPanel.Controls.Add(refreshButton);
        actionsPanel.Controls.Add(retryButton);
        actionsPanel.Controls.Add(cancelButton);
        actionsPanel.Controls.Add(releaseButton);
        actionsPanel.Dock = DockStyle.Top;
        actionsPanel.Location = new Point(0, 0);
        actionsPanel.Name = "actionsPanel";
        actionsPanel.Size = new Size(926, 45);
        actionsPanel.TabIndex = 0;
        //
        // refreshButton
        //
        refreshButton.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        refreshButton.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        refreshButton.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        refreshButton.Appearance.ForeColor = Color.White;
        refreshButton.Appearance.Options.UseBackColor = true;
        refreshButton.Appearance.Options.UseBorderColor = true;
        refreshButton.Appearance.Options.UseFont = true;
        refreshButton.Appearance.Options.UseForeColor = true;
        refreshButton.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        refreshButton.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        refreshButton.AppearanceHovered.ForeColor = Color.White;
        refreshButton.AppearanceHovered.Options.UseBackColor = true;
        refreshButton.AppearanceHovered.Options.UseBorderColor = true;
        refreshButton.AppearanceHovered.Options.UseForeColor = true;
        refreshButton.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        refreshButton.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        refreshButton.AppearancePressed.ForeColor = Color.White;
        refreshButton.AppearancePressed.Options.UseBackColor = true;
        refreshButton.AppearancePressed.Options.UseBorderColor = true;
        refreshButton.AppearancePressed.Options.UseForeColor = true;
        refreshButton.ButtonKind = NuanActionButtonKind.Save;
        refreshButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
        refreshButton.ButtonText = "Actualizar";
        refreshButton.IconNameOverride = "actualizar_16.svg";
        refreshButton.IconSize = 16;
        refreshButton.ImageOptions.ImageToTextIndent = 0;
        refreshButton.ImageOptions.Location = ImageLocation.MiddleLeft;
        refreshButton.ImageOptions.SvgImageSize = new Size(16, 16);
        refreshButton.Location = new Point(10, 7);
        refreshButton.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        refreshButton.LookAndFeel.UseDefaultLookAndFeel = false;
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(100, 36);
        refreshButton.TabIndex = 0;
        refreshButton.Text = "Actualizar";
        //
        // retryButton
        //
        retryButton.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        retryButton.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        retryButton.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        retryButton.Appearance.ForeColor = Color.White;
        retryButton.Appearance.Options.UseBackColor = true;
        retryButton.Appearance.Options.UseBorderColor = true;
        retryButton.Appearance.Options.UseFont = true;
        retryButton.Appearance.Options.UseForeColor = true;
        retryButton.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        retryButton.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        retryButton.AppearanceHovered.ForeColor = Color.White;
        retryButton.AppearanceHovered.Options.UseBackColor = true;
        retryButton.AppearanceHovered.Options.UseBorderColor = true;
        retryButton.AppearanceHovered.Options.UseForeColor = true;
        retryButton.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        retryButton.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        retryButton.AppearancePressed.ForeColor = Color.White;
        retryButton.AppearancePressed.Options.UseBackColor = true;
        retryButton.AppearancePressed.Options.UseBorderColor = true;
        retryButton.AppearancePressed.Options.UseForeColor = true;
        retryButton.ButtonKind = NuanActionButtonKind.Save;
        retryButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
        retryButton.ButtonText = "Reintentar";
        retryButton.IconNameOverride = "reintentar_ejecucion_16.svg";
        retryButton.IconSize = 16;
        retryButton.ImageOptions.ImageToTextIndent = 0;
        retryButton.ImageOptions.Location = ImageLocation.MiddleLeft;
        retryButton.ImageOptions.SvgImageSize = new Size(16, 16);
        retryButton.Location = new Point(118, 7);
        retryButton.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        retryButton.LookAndFeel.UseDefaultLookAndFeel = false;
        retryButton.Name = "retryButton";
        retryButton.Size = new Size(100, 36);
        retryButton.TabIndex = 1;
        retryButton.Text = "Reintentar";
        //
        // cancelButton
        //
        cancelButton.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        cancelButton.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        cancelButton.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        cancelButton.Appearance.ForeColor = Color.White;
        cancelButton.Appearance.Options.UseBackColor = true;
        cancelButton.Appearance.Options.UseBorderColor = true;
        cancelButton.Appearance.Options.UseFont = true;
        cancelButton.Appearance.Options.UseForeColor = true;
        cancelButton.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        cancelButton.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        cancelButton.AppearanceHovered.ForeColor = Color.White;
        cancelButton.AppearanceHovered.Options.UseBackColor = true;
        cancelButton.AppearanceHovered.Options.UseBorderColor = true;
        cancelButton.AppearanceHovered.Options.UseForeColor = true;
        cancelButton.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        cancelButton.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        cancelButton.AppearancePressed.ForeColor = Color.White;
        cancelButton.AppearancePressed.Options.UseBackColor = true;
        cancelButton.AppearancePressed.Options.UseBorderColor = true;
        cancelButton.AppearancePressed.Options.UseForeColor = true;
        cancelButton.ButtonKind = NuanActionButtonKind.Cancel;
        cancelButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
        cancelButton.ButtonText = "Cancelar";
        cancelButton.IconNameOverride = "cancelar_16.svg";
        cancelButton.IconSize = 16;
        cancelButton.ImageOptions.ImageToTextIndent = 0;
        cancelButton.ImageOptions.Location = ImageLocation.MiddleLeft;
        cancelButton.ImageOptions.SvgImageSize = new Size(16, 16);
        cancelButton.Location = new Point(334, 7);
        cancelButton.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        cancelButton.LookAndFeel.UseDefaultLookAndFeel = false;
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(100, 36);
        cancelButton.TabIndex = 2;
        cancelButton.Text = "Cancelar";
        //
        // releaseButton
        //
        releaseButton.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        releaseButton.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        releaseButton.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        releaseButton.Appearance.ForeColor = Color.White;
        releaseButton.Appearance.Options.UseBackColor = true;
        releaseButton.Appearance.Options.UseBorderColor = true;
        releaseButton.Appearance.Options.UseFont = true;
        releaseButton.Appearance.Options.UseForeColor = true;
        releaseButton.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        releaseButton.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        releaseButton.AppearanceHovered.ForeColor = Color.White;
        releaseButton.AppearanceHovered.Options.UseBackColor = true;
        releaseButton.AppearanceHovered.Options.UseBorderColor = true;
        releaseButton.AppearanceHovered.Options.UseForeColor = true;
        releaseButton.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        releaseButton.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        releaseButton.AppearancePressed.ForeColor = Color.White;
        releaseButton.AppearancePressed.Options.UseBackColor = true;
        releaseButton.AppearancePressed.Options.UseBorderColor = true;
        releaseButton.AppearancePressed.Options.UseForeColor = true;
        releaseButton.ButtonKind = NuanActionButtonKind.Save;
        releaseButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
        releaseButton.ButtonText = "Liberar lock";
        releaseButton.IconNameOverride = "liberar_lock_vencido_16.svg";
        releaseButton.IconSize = 16;
        releaseButton.ImageOptions.ImageToTextIndent = 0;
        releaseButton.ImageOptions.Location = ImageLocation.MiddleLeft;
        releaseButton.ImageOptions.SvgImageSize = new Size(16, 16);
        releaseButton.Location = new Point(226, 7);
        releaseButton.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        releaseButton.LookAndFeel.UseDefaultLookAndFeel = false;
        releaseButton.Name = "releaseButton";
        releaseButton.Size = new Size(100, 36);
        releaseButton.TabIndex = 3;
        releaseButton.Text = "Liberar lock";
        //
        // summaryEdit
        //
        summaryEdit.Dock = DockStyle.Top;
        summaryEdit.Location = new Point(0, 45);
        summaryEdit.Name = "summaryEdit";
        summaryEdit.Properties.ReadOnly = true;
        summaryEdit.Size = new Size(926, 130);
        summaryEdit.TabIndex = 1;
        //
        // detailGrid
        //
        detailGrid.Dock = DockStyle.Fill;
        detailGrid.EnableColumnCustomization = true;
        detailGrid.FormKey = "sap-sync-executions";
        detailGrid.GridName = "Details";
        detailGrid.Location = new Point(0, 175);
        detailGrid.Name = "detailGrid";
        detailGrid.PageSize = 100;
        detailGrid.Size = new Size(926, 397);
        detailGrid.TabIndex = 2;
        //
        // pollingTimer
        //
        pollingTimer.Interval = 7000;
        //
        // SapSyncExecutionDetailForm
        //
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(926, 572);
        Controls.Add(detailGrid);
        Controls.Add(summaryEdit);
        Controls.Add(actionsPanel);
        MinimumSize = new Size(771, 485);
        Name = "SapSyncExecutionDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Detalle de ejecucion SAP";
        ((System.ComponentModel.ISupportInitialize)actionsPanel).EndInit();
        actionsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)summaryEdit.Properties).EndInit();
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
