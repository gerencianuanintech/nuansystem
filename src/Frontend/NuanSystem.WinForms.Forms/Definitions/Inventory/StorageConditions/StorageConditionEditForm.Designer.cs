using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.StorageConditions;

partial class StorageConditionEditForm
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl lblGeneralTitle, lineGeneralTitle, lblCode, lblName, lblDescription, lblSortOrder, lblIsActive;
    private TextEdit txtCode, txtName;
    private MemoEdit memDescription;
    private SpinEdit spnSortOrder;
    private NuanToggleSwitch tglIsActive;

    private void InitializeComponent()
    {
        lblGeneralTitle = new LabelControl(); lineGeneralTitle = new LabelControl(); lblCode = new LabelControl(); txtCode = new TextEdit();
        lblName = new LabelControl(); txtName = new TextEdit(); lblDescription = new LabelControl(); memDescription = new MemoEdit();
        lblSortOrder = new LabelControl(); spnSortOrder = new SpinEdit(); lblIsActive = new LabelControl(); tglIsActive = new NuanToggleSwitch();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglIsActive.Properties).BeginInit(); SuspendLayout();
        lblGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold); lblGeneralTitle.Appearance.ForeColor = BrandResources.Primary;
        lblGeneralTitle.Appearance.Options.UseFont = true; lblGeneralTitle.Appearance.Options.UseForeColor = true; lblGeneralTitle.Location = new Point(32, 22); lblGeneralTitle.Name = "lblGeneralTitle"; lblGeneralTitle.Text = "1. Información general";
        lineGeneralTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; lineGeneralTitle.Appearance.BackColor = BrandResources.Border;
        lineGeneralTitle.Appearance.Options.UseBackColor = true; lineGeneralTitle.AutoSizeMode = LabelAutoSizeMode.None; lineGeneralTitle.Location = new Point(218, 34); lineGeneralTitle.Name = "lineGeneralTitle"; lineGeneralTitle.Size = new Size(620, 1);
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = BrandResources.Text;
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(32, 63);
        lblCode.Name = "lblCode";
        lblCode.Text = "Código:";
        txtCode.Location = new Point(154, 60);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(180, 22);
        txtCode.TabIndex = 0;
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = BrandResources.Text;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(32, 91);
        lblName.Name = "lblName";
        lblName.Text = "Nombre:";
        txtName.Location = new Point(154, 88);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(436, 22);
        txtName.TabIndex = 2;
        lblSortOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblSortOrder.Appearance.ForeColor = BrandResources.Text;
        lblSortOrder.Appearance.Options.UseFont = true;
        lblSortOrder.Appearance.Options.UseForeColor = true;
        lblSortOrder.Location = new Point(632, 63);
        lblSortOrder.Name = "lblSortOrder";
        lblSortOrder.Text = "Orden:";
        spnSortOrder.Location = new Point(680, 60);
        spnSortOrder.Name = "spnSortOrder";
        spnSortOrder.Properties.Appearance.Font = new Font("Segoe UI", 9F); spnSortOrder.Properties.Appearance.Options.UseFont = true; spnSortOrder.Properties.AutoHeight = false;
        spnSortOrder.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSortOrder.Properties.IsFloatValue = false;
        spnSortOrder.Properties.MaskSettings.Set("mask", "d");
        spnSortOrder.Properties.MaxValue = 9999;
        spnSortOrder.Size = new Size(150, 22);
        spnSortOrder.TabIndex = 1;
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = BrandResources.Text;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(32, 119);
        lblDescription.Name = "lblDescription";
        lblDescription.Text = "Descripción:";
        memDescription.Location = new Point(154, 116);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(436, 64);
        memDescription.TabIndex = 4;
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = BrandResources.Text;
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(632, 91);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Text = "Activo:";
        tglIsActive.ActiveColor = BrandResources.Primary;
        tglIsActive.InactiveColor = BrandResources.Border;
        tglIsActive.Location = new Point(684, 88);
        tglIsActive.Name = "tglIsActive";
        tglIsActive.Properties.OffText = "No";
        tglIsActive.Properties.OnText = "Sí";
        tglIsActive.Size = new Size(120, 20);
        tglIsActive.StateTextColor = BrandResources.Text;
        tglIsActive.TabIndex = 3;
        Appearance.BackColor = Color.White; Appearance.Options.UseBackColor = true; AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        CancelButtonLocation = new Point(628, 190); ClientSize = new Size(870, 236); SaveButtonLocation = new Point(734, 190); MinimumSize = new Size(886, 275);
        Controls.AddRange(new Control[] { lblGeneralTitle, lineGeneralTitle, lblCode, txtCode, lblName, txtName, lblSortOrder, spnSortOrder, lblDescription, memDescription, lblIsActive, tglIsActive });
        Font = new Font("Segoe UI", 9F); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Name = "StorageConditionEditForm"; StartPosition = FormStartPosition.CenterParent; Text = "Condición de almacenamiento";
        Controls.SetChildIndex(btnGuardar, 0); Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)tglIsActive.Properties).EndInit(); ResumeLayout(false); PerformLayout();
    }

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
