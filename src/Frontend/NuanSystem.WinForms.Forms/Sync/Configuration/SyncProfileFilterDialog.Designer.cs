using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class SyncProfileFilterDialog
{
    private System.ComponentModel.IContainer components = null!;
    private LabelControl lblSearch;
    private TextEdit txtSearch;
    private LabelControl lblStatus;
    private ComboBoxEdit cboStatus;
    private LabelControl lblExecutionMode;
    private ComboBoxEdit cboExecutionMode;
    private NuanActionButton btnApply;
    private NuanActionButton btnClear;
    private NuanActionButton btnCancel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblSearch = new LabelControl();
        txtSearch = new TextEdit();
        lblStatus = new LabelControl();
        cboStatus = new ComboBoxEdit();
        lblExecutionMode = new LabelControl();
        cboExecutionMode = new ComboBoxEdit();
        btnApply = new NuanActionButton();
        btnClear = new NuanActionButton();
        btnCancel = new NuanActionButton();
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboExecutionMode.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblSearch
        // 
        lblSearch.Appearance.Font = AppTypography.LabelFont;
        lblSearch.Appearance.Options.UseFont = true;
        lblSearch.Location = new Point(14, 17);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(35, 15);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Buscar";
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(118, 14);
        txtSearch.Name = "txtSearch";
        txtSearch.Properties.Appearance.Font = AppTypography.InputFont;
        txtSearch.Properties.Appearance.Options.UseFont = true;
        txtSearch.Properties.NullValuePrompt = "Codigo, perfil o empresa";
        txtSearch.Size = new Size(264, 22);
        txtSearch.TabIndex = 1;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.Font = AppTypography.LabelFont;
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Location = new Point(14, 43);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(35, 15);
        lblStatus.TabIndex = 2;
        lblStatus.Text = "Estado";
        // 
        // cboStatus
        // 
        cboStatus.Location = new Point(118, 40);
        cboStatus.Name = "cboStatus";
        cboStatus.Properties.Appearance.Font = AppTypography.InputFont;
        cboStatus.Properties.Appearance.Options.UseFont = true;
        cboStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboStatus.Properties.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
        cboStatus.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboStatus.Size = new Size(264, 22);
        cboStatus.TabIndex = 3;
        // 
        // lblExecutionMode
        // 
        lblExecutionMode.Appearance.Font = AppTypography.LabelFont;
        lblExecutionMode.Appearance.Options.UseFont = true;
        lblExecutionMode.Location = new Point(14, 69);
        lblExecutionMode.Name = "lblExecutionMode";
        lblExecutionMode.Size = new Size(32, 15);
        lblExecutionMode.TabIndex = 4;
        lblExecutionMode.Text = "Modo";
        // 
        // cboExecutionMode
        // 
        cboExecutionMode.Location = new Point(118, 66);
        cboExecutionMode.Name = "cboExecutionMode";
        cboExecutionMode.Properties.Appearance.Font = AppTypography.InputFont;
        cboExecutionMode.Properties.Appearance.Options.UseFont = true;
        cboExecutionMode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cboExecutionMode.Properties.Items.AddRange(new object[] { "Todos", "Incremental", "Full", "Manual" });
        cboExecutionMode.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboExecutionMode.Size = new Size(264, 22);
        cboExecutionMode.TabIndex = 5;
        // 
        // btnApply
        // 
        btnApply.ButtonKind = NuanActionButtonKind.Save;
        btnApply.ButtonText = "Aplicar";
        btnApply.Location = new Point(282, 98);
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(100, 36);
        btnApply.TabIndex = 6;
        btnApply.Text = "Aplicar";
        btnApply.Click += ApplyButton_Click;
        // 
        // btnClear
        // 
        btnClear.ButtonKind = NuanActionButtonKind.Cancel;
        btnClear.ButtonText = "Limpiar";
        btnClear.Location = new Point(176, 98);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(100, 36);
        btnClear.TabIndex = 7;
        btnClear.Text = "Limpiar";
        btnClear.Click += ClearButton_Click;
        // 
        // btnCancel
        // 
        btnCancel.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancel.ButtonText = "Cancelar";
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(70, 98);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 8;
        btnCancel.Text = "Cancelar";
        // 
        // SyncProfileFilterDialog
        // 
        AcceptButton = btnApply;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(394, 142);
        Controls.Add(btnCancel);
        Controls.Add(btnClear);
        Controls.Add(btnApply);
        Controls.Add(cboExecutionMode);
        Controls.Add(lblExecutionMode);
        Controls.Add(cboStatus);
        Controls.Add(lblStatus);
        Controls.Add(txtSearch);
        Controls.Add(lblSearch);
        Font = AppTypography.BaseFont;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SyncProfileFilterDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Filtros de perfiles";
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboExecutionMode.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
