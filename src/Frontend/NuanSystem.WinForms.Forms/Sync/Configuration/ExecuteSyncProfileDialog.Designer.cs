using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

partial class ExecuteSyncProfileDialog
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel rootLayout;
    private LabelControl lblEntityCodes;
    private TextEdit txtEntityCodes;
    private LabelControl lblFromKey;
    private TextEdit txtFromKey;
    private LabelControl lblMaxRecords;
    private SpinEdit sedMaxRecords;
    private FlowLayoutPanel buttonPanel;
    private SimpleButton btnExecute;
    private SimpleButton btnCancel;

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
        rootLayout = new TableLayoutPanel();
        lblEntityCodes = new LabelControl();
        txtEntityCodes = new TextEdit();
        lblFromKey = new LabelControl();
        txtFromKey = new TextEdit();
        lblMaxRecords = new LabelControl();
        sedMaxRecords = new SpinEdit();
        buttonPanel = new FlowLayoutPanel();
        btnExecute = new SimpleButton();
        btnCancel = new SimpleButton();
        rootLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtEntityCodes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtFromKey.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedMaxRecords.Properties).BeginInit();
        buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 2;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(lblEntityCodes, 0, 0);
        rootLayout.Controls.Add(txtEntityCodes, 1, 0);
        rootLayout.Controls.Add(lblFromKey, 0, 1);
        rootLayout.Controls.Add(txtFromKey, 1, 1);
        rootLayout.Controls.Add(lblMaxRecords, 0, 2);
        rootLayout.Controls.Add(sedMaxRecords, 1, 2);
        rootLayout.Controls.Add(buttonPanel, 0, 4);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(10);
        rootLayout.RowCount = 5;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        rootLayout.Size = new Size(448, 200);
        rootLayout.TabIndex = 0;
        // 
        // lblEntityCodes
        // 
        lblEntityCodes.Dock = DockStyle.Fill;
        lblEntityCodes.Location = new Point(13, 13);
        lblEntityCodes.Name = "lblEntityCodes";
        lblEntityCodes.Size = new Size(114, 23);
        lblEntityCodes.TabIndex = 0;
        lblEntityCodes.Text = "Entidades";
        // 
        // txtEntityCodes
        // 
        txtEntityCodes.Dock = DockStyle.Fill;
        txtEntityCodes.Location = new Point(133, 13);
        txtEntityCodes.Name = "txtEntityCodes";
        txtEntityCodes.Size = new Size(302, 20);
        txtEntityCodes.TabIndex = 1;
        // 
        // lblFromKey
        // 
        lblFromKey.Dock = DockStyle.Fill;
        lblFromKey.Location = new Point(13, 42);
        lblFromKey.Name = "lblFromKey";
        lblFromKey.Size = new Size(114, 23);
        lblFromKey.TabIndex = 2;
        lblFromKey.Text = "Desde clave";
        // 
        // txtFromKey
        // 
        txtFromKey.Dock = DockStyle.Fill;
        txtFromKey.Location = new Point(133, 42);
        txtFromKey.Name = "txtFromKey";
        txtFromKey.Size = new Size(302, 20);
        txtFromKey.TabIndex = 3;
        // 
        // lblMaxRecords
        // 
        lblMaxRecords.Dock = DockStyle.Fill;
        lblMaxRecords.Location = new Point(13, 71);
        lblMaxRecords.Name = "lblMaxRecords";
        lblMaxRecords.Size = new Size(114, 23);
        lblMaxRecords.TabIndex = 4;
        lblMaxRecords.Text = "Max. registros";
        // 
        // sedMaxRecords
        // 
        sedMaxRecords.Dock = DockStyle.Fill;
        sedMaxRecords.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedMaxRecords.Location = new Point(133, 71);
        sedMaxRecords.Name = "sedMaxRecords";
        sedMaxRecords.Properties.IsFloatValue = false;
        sedMaxRecords.Properties.MaskSettings.Set("mask", "N00");
        sedMaxRecords.Properties.MaxValue = new decimal(new int[] { 1000000, 0, 0, 0 });
        sedMaxRecords.Size = new Size(302, 20);
        sedMaxRecords.TabIndex = 5;
        // 
        // buttonPanel
        // 
        rootLayout.SetColumnSpan(buttonPanel, 2);
        buttonPanel.Controls.Add(btnExecute);
        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Location = new Point(13, 157);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(422, 30);
        buttonPanel.TabIndex = 6;
        // 
        // btnExecute
        // 
        btnExecute.DialogResult = DialogResult.OK;
        btnExecute.Location = new Point(342, 3);
        btnExecute.Name = "btnExecute";
        btnExecute.Size = new Size(77, 24);
        btnExecute.TabIndex = 0;
        btnExecute.Text = "Ejecutar";
        // 
        // btnCancel
        // 
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(259, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(77, 24);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancelar";
        // 
        // ExecuteSyncProfileDialog
        // 
        AcceptButton = btnExecute;
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(448, 200);
        Controls.Add(rootLayout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(447, 227);
        Name = "ExecuteSyncProfileDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Ejecutar perfil";
        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtEntityCodes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtFromKey.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedMaxRecords.Properties).EndInit();
        buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
