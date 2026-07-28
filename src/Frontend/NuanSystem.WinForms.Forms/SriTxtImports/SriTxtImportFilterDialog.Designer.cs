using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.SriTxtImports;

partial class SriTxtImportFilterDialog
{
    private System.ComponentModel.IContainer components = null!;
    private LabelControl lblDateFrom = null!;
    private DateEdit dateFrom = null!;
    private LabelControl lblDateTo = null!;
    private DateEdit dateTo = null!;
    private LabelControl lblStatus = null!;
    private ComboBoxEdit cmbStatus = null!;
    private LabelControl lblEnvironment = null!;
    private ComboBoxEdit cmbEnvironment = null!;
    private LabelControl lblFileName = null!;
    private TextEdit txtFileName = null!;
    private LabelControl lblValidity = null!;
    private ComboBoxEdit cmbValidity = null!;
    private NuanActionButton btnApply = null!;
    private NuanActionButton btnClear = null!;
    private NuanActionButton btnCancel = null!;

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
        lblDateFrom = new LabelControl();
        dateFrom = new DateEdit();
        lblDateTo = new LabelControl();
        dateTo = new DateEdit();
        lblStatus = new LabelControl();
        cmbStatus = new ComboBoxEdit();
        lblEnvironment = new LabelControl();
        cmbEnvironment = new ComboBoxEdit();
        lblFileName = new LabelControl();
        txtFileName = new TextEdit();
        lblValidity = new LabelControl();
        cmbValidity = new ComboBoxEdit();
        btnApply = new NuanActionButton();
        btnClear = new NuanActionButton();
        btnCancel = new NuanActionButton();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbEnvironment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbValidity.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblDateFrom
        // 
        lblDateFrom.Appearance.Font = AppTypography.LabelFont;
        lblDateFrom.Appearance.Options.UseFont = true;
        lblDateFrom.Location = new Point(16, 17);
        lblDateFrom.Name = "lblDateFrom";
        lblDateFrom.Size = new Size(62, 15);
        lblDateFrom.TabIndex = 0;
        lblDateFrom.Text = "Fecha desde";
        // 
        // dateFrom
        // 
        dateFrom.EditValue = null;
        dateFrom.Location = new Point(142, 14);
        dateFrom.Name = "dateFrom";
        dateFrom.Properties.Appearance.Font = AppTypography.InputFont;
        dateFrom.Properties.Appearance.Options.UseFont = true;
        dateFrom.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dateFrom.Properties.NullText = "Todas";
        dateFrom.Size = new Size(264, 22);
        dateFrom.TabIndex = 1;
        // 
        // lblDateTo
        // 
        lblDateTo.Appearance.Font = AppTypography.LabelFont;
        lblDateTo.Appearance.Options.UseFont = true;
        lblDateTo.Location = new Point(16, 45);
        lblDateTo.Name = "lblDateTo";
        lblDateTo.Size = new Size(59, 15);
        lblDateTo.TabIndex = 2;
        lblDateTo.Text = "Fecha hasta";
        // 
        // dateTo
        // 
        dateTo.EditValue = null;
        dateTo.Location = new Point(142, 42);
        dateTo.Name = "dateTo";
        dateTo.Properties.Appearance.Font = AppTypography.InputFont;
        dateTo.Properties.Appearance.Options.UseFont = true;
        dateTo.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dateTo.Properties.NullText = "Todas";
        dateTo.Size = new Size(264, 22);
        dateTo.TabIndex = 3;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.Font = AppTypography.LabelFont;
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Location = new Point(16, 73);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(105, 15);
        lblStatus.TabIndex = 4;
        lblStatus.Text = "Estado importación";
        // 
        // cmbStatus
        // 
        cmbStatus.Location = new Point(142, 70);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Properties.Appearance.Font = AppTypography.InputFont;
        cmbStatus.Properties.Appearance.Options.UseFont = true;
        cmbStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cmbStatus.Properties.Items.AddRange(new object[] { "Todos", "Validada", "Validada con errores", "Completada", "Completada con errores" });
        cmbStatus.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbStatus.SelectedIndex = 0;
        cmbStatus.Size = new Size(264, 22);
        cmbStatus.TabIndex = 5;
        // 
        // lblEnvironment
        // 
        lblEnvironment.Appearance.Font = AppTypography.LabelFont;
        lblEnvironment.Appearance.Options.UseFont = true;
        lblEnvironment.Location = new Point(16, 101);
        lblEnvironment.Name = "lblEnvironment";
        lblEnvironment.Size = new Size(49, 15);
        lblEnvironment.TabIndex = 6;
        lblEnvironment.Text = "Ambiente";
        // 
        // cmbEnvironment
        // 
        cmbEnvironment.Location = new Point(142, 98);
        cmbEnvironment.Name = "cmbEnvironment";
        cmbEnvironment.Properties.Appearance.Font = AppTypography.InputFont;
        cmbEnvironment.Properties.Appearance.Options.UseFont = true;
        cmbEnvironment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cmbEnvironment.Properties.Items.AddRange(new object[] { "Todos", "Pruebas", "Producción" });
        cmbEnvironment.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbEnvironment.SelectedIndex = 0;
        cmbEnvironment.Size = new Size(264, 22);
        cmbEnvironment.TabIndex = 7;
        // 
        // lblFileName
        // 
        lblFileName.Appearance.Font = AppTypography.LabelFont;
        lblFileName.Appearance.Options.UseFont = true;
        lblFileName.Location = new Point(16, 129);
        lblFileName.Name = "lblFileName";
        lblFileName.Size = new Size(91, 15);
        lblFileName.TabIndex = 8;
        lblFileName.Text = "Nombre archivo";
        // 
        // txtFileName
        // 
        txtFileName.Location = new Point(142, 126);
        txtFileName.Name = "txtFileName";
        txtFileName.Properties.Appearance.Font = AppTypography.InputFont;
        txtFileName.Properties.Appearance.Options.UseFont = true;
        txtFileName.Properties.MaxLength = 260;
        txtFileName.Properties.NullValuePrompt = "Contiene...";
        txtFileName.Size = new Size(264, 22);
        txtFileName.TabIndex = 9;
        // 
        // lblValidity
        // 
        lblValidity.Appearance.Font = AppTypography.LabelFont;
        lblValidity.Appearance.Options.UseFont = true;
        lblValidity.Location = new Point(16, 157);
        lblValidity.Name = "lblValidity";
        lblValidity.Size = new Size(70, 15);
        lblValidity.TabIndex = 10;
        lblValidity.Text = "Filas del TXT";
        // 
        // cmbValidity
        // 
        cmbValidity.Location = new Point(142, 154);
        cmbValidity.Name = "cmbValidity";
        cmbValidity.Properties.Appearance.Font = AppTypography.InputFont;
        cmbValidity.Properties.Appearance.Options.UseFont = true;
        cmbValidity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cmbValidity.Properties.Items.AddRange(new object[] { "Todas", "Válidas", "Inválidas" });
        cmbValidity.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbValidity.SelectedIndex = 0;
        cmbValidity.Size = new Size(264, 22);
        cmbValidity.TabIndex = 11;
        // 
        // btnApply
        // 
        btnApply.ButtonKind = NuanActionButtonKind.Save;
        btnApply.ButtonText = "Aplicar";
        btnApply.Location = new Point(318, 194);
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(88, 34);
        btnApply.TabIndex = 12;
        btnApply.Text = "Aplicar";
        btnApply.Click += ApplyButton_Click;
        // 
        // btnClear
        // 
        btnClear.ButtonKind = NuanActionButtonKind.Cancel;
        btnClear.ButtonText = "Limpiar";
        btnClear.Location = new Point(224, 194);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(88, 34);
        btnClear.TabIndex = 13;
        btnClear.Text = "Limpiar";
        btnClear.Click += ClearButton_Click;
        // 
        // btnCancel
        // 
        btnCancel.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancel.ButtonText = "Cancelar";
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(130, 194);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(88, 34);
        btnCancel.TabIndex = 14;
        btnCancel.Text = "Cancelar";
        // 
        // SriTxtImportFilterDialog
        // 
        AcceptButton = btnApply;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(422, 242);
        Controls.Add(btnCancel);
        Controls.Add(btnClear);
        Controls.Add(btnApply);
        Controls.Add(cmbValidity);
        Controls.Add(lblValidity);
        Controls.Add(txtFileName);
        Controls.Add(lblFileName);
        Controls.Add(cmbEnvironment);
        Controls.Add(lblEnvironment);
        Controls.Add(cmbStatus);
        Controls.Add(lblStatus);
        Controls.Add(dateTo);
        Controls.Add(lblDateTo);
        Controls.Add(dateFrom);
        Controls.Add(lblDateFrom);
        Font = AppTypography.BaseFont;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SriTxtImportFilterDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Filtros de importaciones TXT SRI";
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateFrom.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dateTo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbEnvironment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbValidity.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
