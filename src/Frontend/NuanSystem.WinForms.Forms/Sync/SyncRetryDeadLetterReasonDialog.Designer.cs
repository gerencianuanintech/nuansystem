using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sync;

partial class SyncRetryDeadLetterReasonDialog
{
    private System.ComponentModel.IContainer components = null;

    private LabelControl lblTitle;
    private LabelControl lblDescription;
    private PanelControl summaryPanel;
    private LabelControl lblEventIdCaption;
    private LabelControl lblEventIdValue;
    private LabelControl lblEntityCaption;
    private LabelControl lblEntityValue;
    private LabelControl lblCodeCaption;
    private LabelControl lblCodeValue;
    private LabelControl lblStatusCaption;
    private LabelControl lblStatusValue;
    private LabelControl lblOperationCaption;
    private LabelControl lblOperationValue;
    private LabelControl lblReason;
    private LabelControl lblRequiredNote;
    private LabelControl lblCharacterCount;
    private MemoEdit memoReason;
    private SimpleButton btnAccept;
    private SimpleButton btnCancel;

    private void InitializeComponent()
    {
        lblTitle = new LabelControl();
        lblDescription = new LabelControl();
        summaryPanel = new PanelControl();
        lblEventIdCaption = new LabelControl();
        lblEventIdValue = new LabelControl();
        lblEntityCaption = new LabelControl();
        lblEntityValue = new LabelControl();
        lblCodeCaption = new LabelControl();
        lblCodeValue = new LabelControl();
        lblStatusCaption = new LabelControl();
        lblStatusValue = new LabelControl();
        lblOperationCaption = new LabelControl();
        lblOperationValue = new LabelControl();
        lblReason = new LabelControl();
        lblRequiredNote = new LabelControl();
        lblCharacterCount = new LabelControl();
        memoReason = new MemoEdit();
        btnAccept = new SimpleButton();
        btnCancel = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)summaryPanel).BeginInit();
        summaryPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memoReason.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.Appearance.ForeColor = BrandResources.Text;
        lblTitle.Appearance.Options.UseFont = true;
        lblTitle.Appearance.Options.UseForeColor = true;
        lblTitle.Location = new Point(18, 16);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(206, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Reintentar DeadLetter";
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblDescription.Appearance.ForeColor = BrandResources.MutedText;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.LineVisible = true;
        lblDescription.Location = new Point(18, 52);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(532, 45);
        lblDescription.TabIndex = 1;
        lblDescription.Text = "El evento seleccionado fue cerrado como DeadLetter debido a un error que no pudo resolverse automaticamente.\r\nAl reintentar, el evento sera vuelto a procesar y la accion quedara registrada en auditoria.";
        // 
        // summaryPanel
        // 
        summaryPanel.Appearance.BackColor = Color.FromArgb(255, 247, 237);
        summaryPanel.Appearance.Options.UseBackColor = true;
        summaryPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        summaryPanel.Controls.Add(lblEventIdCaption);
        summaryPanel.Controls.Add(lblEventIdValue);
        summaryPanel.Controls.Add(lblEntityCaption);
        summaryPanel.Controls.Add(lblEntityValue);
        summaryPanel.Controls.Add(lblCodeCaption);
        summaryPanel.Controls.Add(lblCodeValue);
        summaryPanel.Controls.Add(lblStatusCaption);
        summaryPanel.Controls.Add(lblStatusValue);
        summaryPanel.Controls.Add(lblOperationCaption);
        summaryPanel.Controls.Add(lblOperationValue);
        summaryPanel.Location = new Point(18, 112);
        summaryPanel.Name = "summaryPanel";
        summaryPanel.Size = new Size(584, 118);
        summaryPanel.TabIndex = 2;
        // 
        // lblEventIdCaption
        // 
        lblEventIdCaption.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblEventIdCaption.Appearance.ForeColor = BrandResources.MutedText;
        lblEventIdCaption.Appearance.Options.UseFont = true;
        lblEventIdCaption.Appearance.Options.UseForeColor = true;
        lblEventIdCaption.Location = new Point(18, 18);
        lblEventIdCaption.Name = "lblEventIdCaption";
        lblEventIdCaption.Size = new Size(42, 15);
        lblEventIdCaption.TabIndex = 0;
        lblEventIdCaption.Text = "EventId";
        // 
        // lblEventIdValue
        // 
        lblEventIdValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblEventIdValue.Appearance.ForeColor = BrandResources.Text;
        lblEventIdValue.Appearance.Options.UseFont = true;
        lblEventIdValue.Appearance.Options.UseForeColor = true;
        lblEventIdValue.Location = new Point(18, 38);
        lblEventIdValue.Name = "lblEventIdValue";
        lblEventIdValue.Size = new Size(7, 15);
        lblEventIdValue.TabIndex = 1;
        lblEventIdValue.Text = "-";
        // 
        // lblEntityCaption
        // 
        lblEntityCaption.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblEntityCaption.Appearance.ForeColor = BrandResources.MutedText;
        lblEntityCaption.Appearance.Options.UseFont = true;
        lblEntityCaption.Appearance.Options.UseForeColor = true;
        lblEntityCaption.Location = new Point(170, 18);
        lblEntityCaption.Name = "lblEntityCaption";
        lblEntityCaption.Size = new Size(41, 15);
        lblEntityCaption.TabIndex = 2;
        lblEntityCaption.Text = "Entidad";
        // 
        // lblEntityValue
        // 
        lblEntityValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblEntityValue.Appearance.ForeColor = BrandResources.Text;
        lblEntityValue.Appearance.Options.UseFont = true;
        lblEntityValue.Appearance.Options.UseForeColor = true;
        lblEntityValue.Location = new Point(170, 38);
        lblEntityValue.Name = "lblEntityValue";
        lblEntityValue.Size = new Size(7, 15);
        lblEntityValue.TabIndex = 3;
        lblEntityValue.Text = "-";
        // 
        // lblCodeCaption
        // 
        lblCodeCaption.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblCodeCaption.Appearance.ForeColor = BrandResources.MutedText;
        lblCodeCaption.Appearance.Options.UseFont = true;
        lblCodeCaption.Appearance.Options.UseForeColor = true;
        lblCodeCaption.Location = new Point(392, 18);
        lblCodeCaption.Name = "lblCodeCaption";
        lblCodeCaption.Size = new Size(39, 15);
        lblCodeCaption.TabIndex = 4;
        lblCodeCaption.Text = "Codigo";
        // 
        // lblCodeValue
        // 
        lblCodeValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblCodeValue.Appearance.ForeColor = BrandResources.Text;
        lblCodeValue.Appearance.Options.UseFont = true;
        lblCodeValue.Appearance.Options.UseForeColor = true;
        lblCodeValue.Location = new Point(392, 38);
        lblCodeValue.Name = "lblCodeValue";
        lblCodeValue.Size = new Size(7, 15);
        lblCodeValue.TabIndex = 5;
        lblCodeValue.Text = "-";
        // 
        // lblStatusCaption
        // 
        lblStatusCaption.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblStatusCaption.Appearance.ForeColor = BrandResources.MutedText;
        lblStatusCaption.Appearance.Options.UseFont = true;
        lblStatusCaption.Appearance.Options.UseForeColor = true;
        lblStatusCaption.Location = new Point(18, 68);
        lblStatusCaption.Name = "lblStatusCaption";
        lblStatusCaption.Size = new Size(36, 15);
        lblStatusCaption.TabIndex = 6;
        lblStatusCaption.Text = "Estado";
        // 
        // lblStatusValue
        // 
        lblStatusValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblStatusValue.Appearance.ForeColor = Color.FromArgb(190, 18, 60);
        lblStatusValue.Appearance.Options.UseFont = true;
        lblStatusValue.Appearance.Options.UseForeColor = true;
        lblStatusValue.Location = new Point(18, 88);
        lblStatusValue.Name = "lblStatusValue";
        lblStatusValue.Size = new Size(7, 15);
        lblStatusValue.TabIndex = 7;
        lblStatusValue.Text = "-";
        // 
        // lblOperationCaption
        // 
        lblOperationCaption.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblOperationCaption.Appearance.ForeColor = BrandResources.MutedText;
        lblOperationCaption.Appearance.Options.UseFont = true;
        lblOperationCaption.Appearance.Options.UseForeColor = true;
        lblOperationCaption.Location = new Point(170, 68);
        lblOperationCaption.Name = "lblOperationCaption";
        lblOperationCaption.Size = new Size(53, 15);
        lblOperationCaption.TabIndex = 8;
        lblOperationCaption.Text = "Operacion";
        // 
        // lblOperationValue
        // 
        lblOperationValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblOperationValue.Appearance.ForeColor = BrandResources.Text;
        lblOperationValue.Appearance.Options.UseFont = true;
        lblOperationValue.Appearance.Options.UseForeColor = true;
        lblOperationValue.Location = new Point(170, 88);
        lblOperationValue.Name = "lblOperationValue";
        lblOperationValue.Size = new Size(7, 15);
        lblOperationValue.TabIndex = 9;
        lblOperationValue.Text = "-";
        // 
        // lblReason
        // 
        lblReason.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblReason.Appearance.ForeColor = BrandResources.Text;
        lblReason.Appearance.Options.UseFont = true;
        lblReason.Appearance.Options.UseForeColor = true;
        lblReason.Location = new Point(18, 248);
        lblReason.Name = "lblReason";
        lblReason.Size = new Size(38, 15);
        lblReason.TabIndex = 3;
        lblReason.Text = "Motivo del reproceso *";
        // 
        // memoReason
        // 
        memoReason.Location = new Point(18, 270);
        memoReason.Name = "memoReason";
        memoReason.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        memoReason.Properties.Appearance.Options.UseFont = true;
        memoReason.Properties.MaxLength = 500;
        memoReason.Size = new Size(584, 118);
        memoReason.TabIndex = 4;
        // 
        // lblRequiredNote
        // 
        lblRequiredNote.Appearance.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblRequiredNote.Appearance.ForeColor = BrandResources.MutedText;
        lblRequiredNote.Appearance.Options.UseFont = true;
        lblRequiredNote.Appearance.Options.UseForeColor = true;
        lblRequiredNote.Location = new Point(18, 400);
        lblRequiredNote.Name = "lblRequiredNote";
        lblRequiredNote.Size = new Size(314, 15);
        lblRequiredNote.TabIndex = 5;
        lblRequiredNote.Text = "El motivo es obligatorio y quedara registrado en auditoria.";
        // 
        // lblCharacterCount
        // 
        lblCharacterCount.Appearance.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblCharacterCount.Appearance.ForeColor = BrandResources.MutedText;
        lblCharacterCount.Appearance.Options.UseFont = true;
        lblCharacterCount.Appearance.Options.UseForeColor = true;
        lblCharacterCount.Location = new Point(540, 400);
        lblCharacterCount.Name = "lblCharacterCount";
        lblCharacterCount.Size = new Size(38, 15);
        lblCharacterCount.TabIndex = 6;
        lblCharacterCount.Text = "0 / 500";
        // 
        // btnAccept
        // 
        btnAccept.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnAccept.Appearance.BackColor = BrandResources.Primary;
        btnAccept.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnAccept.Appearance.ForeColor = Color.White;
        btnAccept.Appearance.Options.UseBackColor = true;
        btnAccept.Appearance.Options.UseFont = true;
        btnAccept.Appearance.Options.UseForeColor = true;
        btnAccept.Location = new Point(446, 436);
        btnAccept.Name = "btnAccept";
        btnAccept.Size = new Size(156, 34);
        btnAccept.TabIndex = 7;
        btnAccept.Text = "Reintentar DeadLetter";
        // 
        // btnCancel
        // 
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(334, 436);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(102, 32);
        btnCancel.TabIndex = 8;
        btnCancel.Text = "Cancelar";
        // 
        // SyncRetryDeadLetterReasonDialog
        // 
        AcceptButton = btnAccept;
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(620, 492);
        Controls.Add(btnCancel);
        Controls.Add(btnAccept);
        Controls.Add(lblCharacterCount);
        Controls.Add(lblRequiredNote);
        Controls.Add(memoReason);
        Controls.Add(lblReason);
        Controls.Add(summaryPanel);
        Controls.Add(lblDescription);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SyncRetryDeadLetterReasonDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Reintentar evento DeadLetter";
        ((System.ComponentModel.ISupportInitialize)summaryPanel).EndInit();
        summaryPanel.ResumeLayout(false);
        summaryPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memoReason.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
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
