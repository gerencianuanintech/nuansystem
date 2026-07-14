using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Controls.Grids;

namespace NuanSystem.WinForms.Forms.Common;

partial class BaseGridCrudListForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        nuanGrid = new NuanDataGridControl();
        paginationPanel = new PanelControl();
        firstPageButton = new SimpleButton();
        previousPageButton = new SimpleButton();
        pageInfoLabel = new LabelControl();
        nextPageButton = new SimpleButton();
        lastPageButton = new SimpleButton();
        pageSizeLabel = new LabelControl();
        pageSizeCombo = new ComboBoxEdit();
        totalInfoLabel = new LabelControl();
        selectionInfoLabel = new LabelControl();
        auditPanel = new PanelControl();
        auditCreatedLabel = new LabelControl();
        auditUpdatedLabel = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)paginationPanel).BeginInit();
        paginationPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pageSizeCombo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)auditPanel).BeginInit();
        auditPanel.SuspendLayout();
        SuspendLayout();
        // 
        // nuanGrid
        // 
        nuanGrid.Dock = DockStyle.Fill;
        nuanGrid.EnableColumnCustomization = false;
        nuanGrid.FormKey = "";
        nuanGrid.GridName = "MainGrid";
        nuanGrid.Location = new Point(0, 0);
        nuanGrid.MultiSelect = true;
        nuanGrid.Name = "nuanGrid";
        nuanGrid.PageSize = 20;
        nuanGrid.ShowFindPanel = true;
        nuanGrid.ShowPagination = false;
        nuanGrid.ShowSelectionCheckBox = true;
        nuanGrid.Size = new Size(900, 482);
        nuanGrid.TabIndex = 0;
        nuanGrid.InnerGridControl.Font = AppTypography.BaseFont;
        nuanGrid.InnerGridControl.Name = "gridControl";
        nuanGrid.InnerGridView.Appearance.HeaderPanel.Font = AppTypography.GridHeaderFont;
        nuanGrid.InnerGridView.Appearance.HeaderPanel.ForeColor = BrandResources.Text;
        nuanGrid.InnerGridView.Appearance.HeaderPanel.Options.UseFont = true;
        nuanGrid.InnerGridView.Appearance.HeaderPanel.Options.UseForeColor = true;
        nuanGrid.InnerGridView.Appearance.Row.Font = AppTypography.GridRowFont;
        nuanGrid.InnerGridView.Appearance.Row.ForeColor = BrandResources.Text;
        nuanGrid.InnerGridView.Appearance.Row.Options.UseFont = true;
        nuanGrid.InnerGridView.Appearance.Row.Options.UseForeColor = true;
        nuanGrid.InnerGridView.Appearance.FooterPanel.Font = AppTypography.GridHeaderFont;
        nuanGrid.InnerGridView.Appearance.FooterPanel.Options.UseFont = true;
        nuanGrid.InnerGridView.Appearance.FilterPanel.Font = AppTypography.GridRowFont;
        nuanGrid.InnerGridView.Appearance.FilterPanel.Options.UseFont = true;
        nuanGrid.InnerGridView.Name = "gridView";
        nuanGrid.InnerGridView.OptionsBehavior.Editable = false;
        nuanGrid.InnerGridView.OptionsFind.AlwaysVisible = true;
        nuanGrid.InnerGridView.OptionsFind.FindNullPrompt = "Buscar...";
        nuanGrid.InnerGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
        nuanGrid.InnerGridView.OptionsSelection.MultiSelect = true;
        nuanGrid.InnerGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
        nuanGrid.InnerGridView.OptionsView.ColumnAutoWidth = false;
        nuanGrid.InnerGridView.OptionsView.ShowGroupPanel = false;
        // 
        // paginationPanel
        // 
        paginationPanel.Appearance.BackColor = Color.White;
        paginationPanel.Appearance.Options.UseBackColor = true;
        paginationPanel.BorderStyle = BorderStyles.NoBorder;
        paginationPanel.Controls.Add(firstPageButton);
        paginationPanel.Controls.Add(previousPageButton);
        paginationPanel.Controls.Add(pageInfoLabel);
        paginationPanel.Controls.Add(nextPageButton);
        paginationPanel.Controls.Add(lastPageButton);
        paginationPanel.Controls.Add(pageSizeLabel);
        paginationPanel.Controls.Add(pageSizeCombo);
        paginationPanel.Controls.Add(totalInfoLabel);
        paginationPanel.Controls.Add(selectionInfoLabel);
        paginationPanel.Dock = DockStyle.Bottom;
        paginationPanel.Location = new Point(0, 516);
        paginationPanel.Name = "paginationPanel";
        paginationPanel.Size = new Size(900, 44);
        paginationPanel.TabIndex = 1;
        // 
        // firstPageButton
        // 
        firstPageButton.Appearance.Font = AppTypography.ButtonFont;
        firstPageButton.Appearance.Options.UseFont = true;
        firstPageButton.Location = new Point(10, 8);
        firstPageButton.Name = "firstPageButton";
        firstPageButton.Size = new Size(36, 28);
        firstPageButton.TabIndex = 0;
        firstPageButton.Text = "|<";
        // 
        // previousPageButton
        // 
        previousPageButton.Appearance.Font = AppTypography.ButtonFont;
        previousPageButton.Appearance.Options.UseFont = true;
        previousPageButton.Location = new Point(50, 8);
        previousPageButton.Name = "previousPageButton";
        previousPageButton.Size = new Size(36, 28);
        previousPageButton.TabIndex = 1;
        previousPageButton.Text = "<";
        // 
        // pageInfoLabel
        // 
        pageInfoLabel.Appearance.Font = AppTypography.LabelFont;
        pageInfoLabel.Appearance.ForeColor = BrandResources.Text;
        pageInfoLabel.Appearance.Options.UseFont = true;
        pageInfoLabel.Appearance.Options.UseForeColor = true;
        pageInfoLabel.Location = new Point(96, 14);
        pageInfoLabel.Name = "pageInfoLabel";
        pageInfoLabel.Size = new Size(74, 15);
        pageInfoLabel.TabIndex = 2;
        pageInfoLabel.Text = "Pagina 1 de 1";
        // 
        // nextPageButton
        // 
        nextPageButton.Appearance.Font = AppTypography.ButtonFont;
        nextPageButton.Appearance.Options.UseFont = true;
        nextPageButton.Location = new Point(178, 8);
        nextPageButton.Name = "nextPageButton";
        nextPageButton.Size = new Size(36, 28);
        nextPageButton.TabIndex = 3;
        nextPageButton.Text = ">";
        // 
        // lastPageButton
        // 
        lastPageButton.Appearance.Font = AppTypography.ButtonFont;
        lastPageButton.Appearance.Options.UseFont = true;
        lastPageButton.Location = new Point(218, 8);
        lastPageButton.Name = "lastPageButton";
        lastPageButton.Size = new Size(36, 28);
        lastPageButton.TabIndex = 4;
        lastPageButton.Text = ">|";
        // 
        // pageSizeLabel
        // 
        pageSizeLabel.Appearance.Font = AppTypography.LabelFont;
        pageSizeLabel.Appearance.ForeColor = BrandResources.Text;
        pageSizeLabel.Appearance.Options.UseFont = true;
        pageSizeLabel.Appearance.Options.UseForeColor = true;
        pageSizeLabel.Location = new Point(276, 14);
        pageSizeLabel.Name = "pageSizeLabel";
        pageSizeLabel.Size = new Size(55, 15);
        pageSizeLabel.TabIndex = 5;
        pageSizeLabel.Text = "Registros:";
        // 
        // pageSizeCombo
        // 
        pageSizeCombo.EditValue = "20";
        pageSizeCombo.Location = new Point(344, 10);
        pageSizeCombo.Name = "pageSizeCombo";
        pageSizeCombo.Properties.Appearance.Font = AppTypography.InputFont;
        pageSizeCombo.Properties.Appearance.Options.UseFont = true;
        pageSizeCombo.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        pageSizeCombo.Properties.Items.AddRange(new object[] { "10", "20", "50", "100" });
        pageSizeCombo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        pageSizeCombo.Size = new Size(70, 22);
        pageSizeCombo.TabIndex = 6;
        // 
        // totalInfoLabel
        // 
        totalInfoLabel.Appearance.Font = AppTypography.LabelFont;
        totalInfoLabel.Appearance.ForeColor = BrandResources.Text;
        totalInfoLabel.Appearance.Options.UseFont = true;
        totalInfoLabel.Appearance.Options.UseForeColor = true;
        totalInfoLabel.Location = new Point(438, 14);
        totalInfoLabel.Name = "totalInfoLabel";
        totalInfoLabel.Size = new Size(90, 15);
        totalInfoLabel.TabIndex = 7;
        totalInfoLabel.Text = "Total: 0 registros";
        // 
        // selectionInfoLabel
        // 
        selectionInfoLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        selectionInfoLabel.Appearance.Font = AppTypography.LabelFont;
        selectionInfoLabel.Appearance.ForeColor = BrandResources.Text;
        selectionInfoLabel.Appearance.Options.UseFont = true;
        selectionInfoLabel.Appearance.Options.UseForeColor = true;
        selectionInfoLabel.Location = new Point(735, 14);
        selectionInfoLabel.Name = "selectionInfoLabel";
        selectionInfoLabel.Size = new Size(122, 15);
        selectionInfoLabel.TabIndex = 8;
        selectionInfoLabel.Text = "Seleccionados: 0 de 0";
        // 
        // auditPanel
        // 
        auditPanel.Appearance.BackColor = Color.FromArgb(248, 250, 252);
        auditPanel.Appearance.Options.UseBackColor = true;
        auditPanel.BorderStyle = BorderStyles.NoBorder;
        auditPanel.Controls.Add(auditCreatedLabel);
        auditPanel.Controls.Add(auditUpdatedLabel);
        auditPanel.Dock = DockStyle.Bottom;
        auditPanel.Location = new Point(0, 482);
        auditPanel.Name = "auditPanel";
        auditPanel.Size = new Size(900, 34);
        auditPanel.TabIndex = 2;
        // 
        // auditCreatedLabel
        // 
        auditCreatedLabel.Appearance.Font = AppTypography.LabelFont;
        auditCreatedLabel.Appearance.ForeColor = BrandResources.Text;
        auditCreatedLabel.Appearance.Options.UseFont = true;
        auditCreatedLabel.Appearance.Options.UseForeColor = true;
        auditCreatedLabel.Location = new Point(12, 10);
        auditCreatedLabel.Name = "auditCreatedLabel";
        auditCreatedLabel.Size = new Size(70, 15);
        auditCreatedLabel.TabIndex = 0;
        auditCreatedLabel.Text = "Creado por: -";
        // 
        // auditUpdatedLabel
        // 
        auditUpdatedLabel.Appearance.Font = AppTypography.LabelFont;
        auditUpdatedLabel.Appearance.ForeColor = BrandResources.Text;
        auditUpdatedLabel.Appearance.Options.UseFont = true;
        auditUpdatedLabel.Appearance.Options.UseForeColor = true;
        auditUpdatedLabel.Location = new Point(360, 10);
        auditUpdatedLabel.Name = "auditUpdatedLabel";
        auditUpdatedLabel.Size = new Size(95, 15);
        auditUpdatedLabel.TabIndex = 1;
        auditUpdatedLabel.Text = "Modificado por: -";
        // 
        // BaseGridCrudListForm
        // 
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 560);
        Controls.Add(nuanGrid);
        Controls.Add(auditPanel);
        Controls.Add(paginationPanel);
        Font = AppTypography.BaseFont;
        MinimumSize = new Size(720, 420);
        Name = "BaseGridCrudListForm";
        StartPosition = FormStartPosition.CenterScreen;
        ((System.ComponentModel.ISupportInitialize)paginationPanel).EndInit();
        paginationPanel.ResumeLayout(false);
        paginationPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pageSizeCombo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)auditPanel).EndInit();
        auditPanel.ResumeLayout(false);
        auditPanel.PerformLayout();
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
