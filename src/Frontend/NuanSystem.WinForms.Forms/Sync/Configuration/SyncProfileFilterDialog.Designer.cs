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
        btnApply.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnApply.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnApply.Appearance.Font = AppTypography.ButtonFont;
        btnApply.Appearance.ForeColor = Color.White;
        btnApply.Appearance.Options.UseBackColor = true;
        btnApply.Appearance.Options.UseBorderColor = true;
        btnApply.Appearance.Options.UseFont = true;
        btnApply.Appearance.Options.UseForeColor = true;
        btnApply.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnApply.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnApply.AppearanceHovered.ForeColor = Color.White;
        btnApply.AppearanceHovered.Options.UseBackColor = true;
        btnApply.AppearanceHovered.Options.UseBorderColor = true;
        btnApply.AppearanceHovered.Options.UseForeColor = true;
        btnApply.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnApply.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnApply.AppearancePressed.ForeColor = Color.White;
        btnApply.AppearancePressed.Options.UseBackColor = true;
        btnApply.AppearancePressed.Options.UseBorderColor = true;
        btnApply.AppearancePressed.Options.UseForeColor = true;
        btnApply.ButtonKind = NuanActionButtonKind.Save;
        btnApply.ButtonStyle = BorderStyles.UltraFlat;
        btnApply.ButtonText = "Filtro";
        btnApply.IconNameOverride = "filtro_32.svg";
        btnApply.ImageOptions.ImageToTextIndent = 0;
        btnApply.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnApply.ImageOptions.SvgImageSize = new Size(24, 24);
        btnApply.Location = new Point(296, 98);
        btnApply.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnApply.LookAndFeel.UseDefaultLookAndFeel = false;
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(86, 31);
        btnApply.TabIndex = 6;
        btnApply.Text = "Filtro";
        btnApply.Click += ApplyButton_Click;
        // 
        // btnClear
        // 
        btnClear.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnClear.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnClear.Appearance.Font = AppTypography.ButtonFont;
        btnClear.Appearance.ForeColor = Color.White;
        btnClear.Appearance.Options.UseBackColor = true;
        btnClear.Appearance.Options.UseBorderColor = true;
        btnClear.Appearance.Options.UseFont = true;
        btnClear.Appearance.Options.UseForeColor = true;
        btnClear.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnClear.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnClear.AppearanceHovered.ForeColor = Color.White;
        btnClear.AppearanceHovered.Options.UseBackColor = true;
        btnClear.AppearanceHovered.Options.UseBorderColor = true;
        btnClear.AppearanceHovered.Options.UseForeColor = true;
        btnClear.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnClear.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnClear.AppearancePressed.ForeColor = Color.White;
        btnClear.AppearancePressed.Options.UseBackColor = true;
        btnClear.AppearancePressed.Options.UseBorderColor = true;
        btnClear.AppearancePressed.Options.UseForeColor = true;
        btnClear.ButtonKind = NuanActionButtonKind.Cancel;
        btnClear.ButtonStyle = BorderStyles.UltraFlat;
        btnClear.ButtonText = "Limpiar";
        btnClear.IconNameOverride = "limpiar_filtros_32.svg";
        btnClear.ImageOptions.ImageToTextIndent = 0;
        btnClear.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnClear.ImageOptions.SvgImageSize = new Size(24, 24);
        btnClear.Location = new Point(205, 98);
        btnClear.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnClear.LookAndFeel.UseDefaultLookAndFeel = false;
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(86, 31);
        btnClear.TabIndex = 7;
        btnClear.Text = "Limpiar";
        btnClear.Click += ClearButton_Click;
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.Font = AppTypography.ButtonFont;
        btnCancel.Appearance.ForeColor = Color.White;
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseBorderColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancel.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnCancel.AppearanceHovered.ForeColor = Color.White;
        btnCancel.AppearanceHovered.Options.UseBackColor = true;
        btnCancel.AppearanceHovered.Options.UseBorderColor = true;
        btnCancel.AppearanceHovered.Options.UseForeColor = true;
        btnCancel.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnCancel.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnCancel.AppearancePressed.ForeColor = Color.White;
        btnCancel.AppearancePressed.Options.UseBackColor = true;
        btnCancel.AppearancePressed.Options.UseBorderColor = true;
        btnCancel.AppearancePressed.Options.UseForeColor = true;
        btnCancel.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancel.ButtonStyle = BorderStyles.UltraFlat;
        btnCancel.ButtonText = "Cancelar";
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.ImageOptions.ImageToTextIndent = 0;
        btnCancel.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnCancel.ImageOptions.SvgImageSize = new Size(24, 24);
        btnCancel.Location = new Point(114, 98);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(86, 31);
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
