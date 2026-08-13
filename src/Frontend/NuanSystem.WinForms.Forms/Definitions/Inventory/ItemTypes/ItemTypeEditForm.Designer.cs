using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemTypes;

partial class ItemTypeEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblBehavior = new LabelControl();
        lueBehavior = new LookUpEdit();
        chkDefaultPurchase = new CheckEdit();
        chkDefaultSales = new CheckEdit();
        chkDefaultInventory = new CheckEdit();
        lblSortOrder = new LabelControl();
        spnSortOrder = new SpinEdit();
        chkIsSystem = new CheckEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBehavior.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultPurchase.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultInventory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(382, 326);
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(488, 326);
        // 
        // lblCode
        // 
        lblCode.Location = new Point(28, 29);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(39, 15);
        lblCode.TabIndex = 20;
        lblCode.Text = "Código";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(205, 26);
        txtCode.Name = "txtCode";
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 30;
        txtCode.Size = new Size(383, 22);
        txtCode.TabIndex = 0;
        // 
        // lblName
        // 
        lblName.Location = new Point(28, 57);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 21;
        lblName.Text = "Nombre";
        // 
        // txtName
        // 
        txtName.Location = new Point(205, 54);
        txtName.Name = "txtName";
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 100;
        txtName.Size = new Size(383, 22);
        txtName.TabIndex = 1;
        // 
        // lblDescription
        // 
        lblDescription.Location = new Point(28, 85);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(62, 15);
        lblDescription.TabIndex = 22;
        lblDescription.Text = "Descripción";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(205, 82);
        memDescription.Name = "memDescription";
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(383, 62);
        memDescription.TabIndex = 2;
        // 
        // lblBehavior
        // 
        lblBehavior.Location = new Point(28, 157);
        lblBehavior.Name = "lblBehavior";
        lblBehavior.Size = new Size(91, 15);
        lblBehavior.TabIndex = 23;
        lblBehavior.Text = "Comportamiento";
        // 
        // lueBehavior
        // 
        lueBehavior.Location = new Point(205, 154);
        lueBehavior.Name = "lueBehavior";
        lueBehavior.Properties.AutoHeight = false;
        lueBehavior.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBehavior.Properties.NullText = "";
        lueBehavior.Properties.ShowFooter = false;
        lueBehavior.Properties.ShowHeader = false;
        lueBehavior.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueBehavior.Size = new Size(383, 22);
        lueBehavior.TabIndex = 3;
        // 
        // chkDefaultPurchase
        // 
        chkDefaultPurchase.Location = new Point(201, 185);
        chkDefaultPurchase.Name = "chkDefaultPurchase";
        chkDefaultPurchase.Properties.Caption = "Ítem de compra predeterminado";
        chkDefaultPurchase.Size = new Size(220, 20);
        chkDefaultPurchase.TabIndex = 4;
        // 
        // chkDefaultSales
        // 
        chkDefaultSales.Location = new Point(201, 213);
        chkDefaultSales.Name = "chkDefaultSales";
        chkDefaultSales.Properties.Caption = "Ítem de venta predeterminado";
        chkDefaultSales.Size = new Size(220, 20);
        chkDefaultSales.TabIndex = 5;
        // 
        // chkDefaultInventory
        // 
        chkDefaultInventory.Location = new Point(201, 241);
        chkDefaultInventory.Name = "chkDefaultInventory";
        chkDefaultInventory.Properties.Caption = "Ítem de inventario predeterminado";
        chkDefaultInventory.Size = new Size(230, 20);
        chkDefaultInventory.TabIndex = 6;
        // 
        // lblSortOrder
        // 
        lblSortOrder.Location = new Point(28, 275);
        lblSortOrder.Name = "lblSortOrder";
        lblSortOrder.Size = new Size(108, 15);
        lblSortOrder.TabIndex = 24;
        lblSortOrder.Text = "Orden de visualización";
        // 
        // spnSortOrder
        // 
        spnSortOrder.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnSortOrder.Location = new Point(205, 272);
        spnSortOrder.Name = "spnSortOrder";
        spnSortOrder.Properties.AutoHeight = false;
        spnSortOrder.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSortOrder.Properties.IsFloatValue = false;
        spnSortOrder.Properties.MaskSettings.Set("mask", "d");
        spnSortOrder.Properties.MaxValue = new decimal(new int[] { 9999, 0, 0, 0 });
        spnSortOrder.Size = new Size(100, 22);
        spnSortOrder.TabIndex = 7;
        // 
        // chkIsSystem
        // 
        chkIsSystem.Enabled = false;
        chkIsSystem.Location = new Point(321, 273);
        chkIsSystem.Name = "chkIsSystem";
        chkIsSystem.Properties.Caption = "Tipo del sistema";
        chkIsSystem.Size = new Size(120, 20);
        chkIsSystem.TabIndex = 8;
        // 
        // chkIsActive
        // 
        chkIsActive.Location = new Point(463, 273);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 9;
        // 
        // ItemTypeEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(616, 407);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblBehavior);
        Controls.Add(lueBehavior);
        Controls.Add(chkDefaultPurchase);
        Controls.Add(chkDefaultSales);
        Controls.Add(chkDefaultInventory);
        Controls.Add(lblSortOrder);
        Controls.Add(spnSortOrder);
        Controls.Add(chkIsSystem);
        Controls.Add(chkIsActive);
        MinimumSize = new Size(618, 439);
        Name = "ItemTypeEditForm";
        Text = "Nuevo tipo de ítem";
        Controls.SetChildIndex(chkIsActive, 0);
        Controls.SetChildIndex(chkIsSystem, 0);
        Controls.SetChildIndex(spnSortOrder, 0);
        Controls.SetChildIndex(lblSortOrder, 0);
        Controls.SetChildIndex(chkDefaultInventory, 0);
        Controls.SetChildIndex(chkDefaultSales, 0);
        Controls.SetChildIndex(chkDefaultPurchase, 0);
        Controls.SetChildIndex(lueBehavior, 0);
        Controls.SetChildIndex(lblBehavior, 0);
        Controls.SetChildIndex(memDescription, 0);
        Controls.SetChildIndex(lblDescription, 0);
        Controls.SetChildIndex(txtName, 0);
        Controls.SetChildIndex(lblName, 0);
        Controls.SetChildIndex(txtCode, 0);
        Controls.SetChildIndex(lblCode, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBehavior.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultPurchase.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkDefaultInventory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblBehavior;
    private LookUpEdit lueBehavior;
    private CheckEdit chkDefaultPurchase;
    private CheckEdit chkDefaultSales;
    private CheckEdit chkDefaultInventory;
    private LabelControl lblSortOrder;
    private SpinEdit spnSortOrder;
    private CheckEdit chkIsSystem;
    private CheckEdit chkIsActive;
}
