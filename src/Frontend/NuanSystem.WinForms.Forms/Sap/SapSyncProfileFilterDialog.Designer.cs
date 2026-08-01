using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Buttons;

namespace NuanSystem.WinForms.Forms.Sap;

partial class SapSyncProfileFilterDialog
{
    private System.ComponentModel.IContainer components = null; private LabelControl searchLabel; private TextEdit searchEdit; private LabelControl entityLabel; private TextEdit entityEdit; private LabelControl statusLabel; private ComboBoxEdit statusEdit; private NuanActionButton acceptButton; private NuanActionButton clearButton; private NuanActionButton cancelActionButton;
    private void InitializeComponent()
    {
        searchLabel = new LabelControl(); searchEdit = new TextEdit(); entityLabel = new LabelControl(); entityEdit = new TextEdit(); statusLabel = new LabelControl(); statusEdit = new ComboBoxEdit(); acceptButton = new NuanActionButton(); clearButton = new NuanActionButton(); cancelActionButton = new NuanActionButton(); ((System.ComponentModel.ISupportInitialize)searchEdit.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)entityEdit.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)statusEdit.Properties).BeginInit(); SuspendLayout();
        searchLabel.Location = new Point(20, 23); searchLabel.Text = "Buscar"; searchEdit.Location = new Point(130, 20); searchEdit.Size = new Size(310, 22); searchEdit.TabIndex = 0;
        entityLabel.Location = new Point(20, 51); entityLabel.Text = "Entidad"; entityEdit.Location = new Point(130, 48); entityEdit.Size = new Size(310, 22); entityEdit.TabIndex = 1;
        statusLabel.Location = new Point(20, 79); statusLabel.Text = "Estado"; statusEdit.Location = new Point(130, 76); statusEdit.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)); statusEdit.Properties.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" }); statusEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor; statusEdit.SelectedIndex = 0; statusEdit.Size = new Size(180, 22); statusEdit.TabIndex = 2;
        acceptButton.ButtonKind = NuanActionButtonKind.Save; acceptButton.ButtonText = "Aplicar"; acceptButton.Location = new Point(340, 116); acceptButton.Name = "acceptButton"; acceptButton.Size = new Size(100, 36); acceptButton.TabIndex = 3; acceptButton.Text = "Aplicar";
        clearButton.ButtonKind = NuanActionButtonKind.Cancel; clearButton.ButtonText = "Limpiar"; clearButton.Location = new Point(234, 116); clearButton.Name = "clearButton"; clearButton.Size = new Size(100, 36); clearButton.TabIndex = 4; clearButton.Text = "Limpiar"; clearButton.Click += ClearButton_Click;
        cancelActionButton.ButtonKind = NuanActionButtonKind.Cancel; cancelActionButton.ButtonText = "Cancelar"; cancelActionButton.DialogResult = DialogResult.Cancel; cancelActionButton.Location = new Point(128, 116); cancelActionButton.Name = "cancelActionButton"; cancelActionButton.Size = new Size(100, 36); cancelActionButton.TabIndex = 5; cancelActionButton.Text = "Cancelar";
        AcceptButton = acceptButton; CancelButton = cancelActionButton; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(460, 171); Controls.AddRange(new Control[] { searchLabel, searchEdit, entityLabel, entityEdit, statusLabel, statusEdit, cancelActionButton, clearButton, acceptButton }); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Name = "SapSyncProfileFilterDialog"; StartPosition = FormStartPosition.CenterParent; Text = "Filtrar perfiles SAP";
        ((System.ComponentModel.ISupportInitialize)searchEdit.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)entityEdit.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)statusEdit.Properties).EndInit(); ResumeLayout(false); PerformLayout();
    }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
