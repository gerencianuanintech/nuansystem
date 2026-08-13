using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemLines;

partial class ItemLineEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblGeneralTitle = new LabelControl();
        lineGeneralTitle = new LabelControl();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblSortOrder = new LabelControl();
        spnSortOrder = new SpinEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblIsActive = new LabelControl();
        chkIsActive = new NuanToggleSwitch();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(974, 224);
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(1080, 224);
        // 
        // lblGeneralTitle
        // 
        lblGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralTitle.Appearance.ForeColor = BrandResources.Primary;
        lblGeneralTitle.Appearance.Options.UseFont = true;
        lblGeneralTitle.Appearance.Options.UseForeColor = true;
        lblGeneralTitle.Location = new Point(32, 22);
        lblGeneralTitle.Name = "lblGeneralTitle";
        lblGeneralTitle.Size = new Size(164, 20);
        lblGeneralTitle.TabIndex = 20;
        lblGeneralTitle.Text = "1. Información general";
        // 
        // lineGeneralTitle
        // 
        lineGeneralTitle.Appearance.BackColor = BrandResources.Border;
        lineGeneralTitle.Appearance.Options.UseBackColor = true;
        lineGeneralTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineGeneralTitle.Location = new Point(218, 34);
        lineGeneralTitle.Name = "lineGeneralTitle";
        lineGeneralTitle.Size = new Size(950, 1);
        lineGeneralTitle.TabIndex = 21;
        // 
        // lblCode
        // 
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = BrandResources.Text;
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(32, 63);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(42, 15);
        lblCode.TabIndex = 22;
        lblCode.Text = "Código:";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(150, 60);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(180, 22);
        txtCode.TabIndex = 0;
        // 
        // lblName
        // 
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = BrandResources.Text;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(360, 63);
        lblName.Name = "lblName";
        lblName.Size = new Size(47, 15);
        lblName.TabIndex = 23;
        lblName.Text = "Nombre:";
        // 
        // txtName
        // 
        txtName.Location = new Point(455, 60);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(455, 22);
        txtName.TabIndex = 1;
        // 
        // lblSortOrder
        // 
        lblSortOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblSortOrder.Appearance.ForeColor = BrandResources.Text;
        lblSortOrder.Appearance.Options.UseFont = true;
        lblSortOrder.Appearance.Options.UseForeColor = true;
        lblSortOrder.Location = new Point(940, 63);
        lblSortOrder.Name = "lblSortOrder";
        lblSortOrder.Size = new Size(40, 15);
        lblSortOrder.TabIndex = 24;
        lblSortOrder.Text = "Orden:";
        // 
        // spnSortOrder
        // 
        spnSortOrder.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnSortOrder.Location = new Point(1018, 60);
        spnSortOrder.Name = "spnSortOrder";
        spnSortOrder.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSortOrder.Properties.Appearance.Options.UseFont = true;
        spnSortOrder.Properties.Appearance.Options.UseTextOptions = true;
        spnSortOrder.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnSortOrder.Properties.AutoHeight = false;
        spnSortOrder.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSortOrder.Properties.IsFloatValue = false;
        spnSortOrder.Properties.MaskSettings.Set("mask", "d");
        spnSortOrder.Properties.MaxValue = new decimal(new int[] { 9999, 0, 0, 0 });
        spnSortOrder.Size = new Size(150, 22);
        spnSortOrder.TabIndex = 2;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = BrandResources.Text;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(32, 91);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(65, 15);
        lblDescription.TabIndex = 25;
        lblDescription.Text = "Descripción:";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(150, 88);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(760, 80);
        memDescription.TabIndex = 3;
        // 
        // lblIsActive
        // 
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = BrandResources.Text;
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(940, 91);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Size = new Size(40, 15);
        lblIsActive.TabIndex = 26;
        lblIsActive.Text = "Activo:";
        // 
        // chkIsActive
        // 
        chkIsActive.ActiveColor = BrandResources.Primary;
        chkIsActive.InactiveColor = BrandResources.Border;
        chkIsActive.Location = new Point(1018, 89);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.OffText = "No";
        chkIsActive.Properties.OnText = "Sí";
        chkIsActive.Size = new Size(70, 20);
        chkIsActive.StateTextColor = BrandResources.Text;
        chkIsActive.TabIndex = 4;
        // 
        // ItemLineEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 280);
        Controls.Add(lblGeneralTitle);
        Controls.Add(lineGeneralTitle);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblSortOrder);
        Controls.Add(spnSortOrder);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblIsActive);
        Controls.Add(chkIsActive);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(1216, 319);
        Name = "ItemLineEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Línea de artículos";
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
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

    private LabelControl lblGeneralTitle;
    private LabelControl lineGeneralTitle;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblSortOrder;
    private SpinEdit spnSortOrder;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblIsActive;
    private NuanToggleSwitch chkIsActive;
}
