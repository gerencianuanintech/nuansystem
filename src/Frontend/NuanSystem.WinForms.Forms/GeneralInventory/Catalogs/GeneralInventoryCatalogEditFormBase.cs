using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Buttons;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;

public class GeneralInventoryCatalogEditFormBase : BaseEditForm, IGeneralInventoryCatalogEditForm
{
    private readonly GeneralInventoryCatalogDescriptor descriptor;

    protected LabelControl lblCode = null!;
    protected TextEdit txtCode = null!;
    protected LabelControl lblName = null!;
    protected TextEdit txtName = null!;
    protected LabelControl lblDescription = null!;
    protected MemoEdit memDescription = null!;
    protected CheckEdit chkIsActive = null!;
    protected NuanActionButton btnCancel = null!;
    protected NuanActionButton btnSave = null!;

    protected GeneralInventoryCatalogEditFormBase()
        : this(GeneralInventoryCatalogDescriptors.Warehouses)
    {
    }

    protected GeneralInventoryCatalogEditFormBase(GeneralInventoryCatalogDescriptor descriptor)
    {
        this.descriptor = descriptor;
        InitializeCatalogEditorComponent();
        ConfigureForm();
    }

    protected GeneralInventoryCatalogEditFormBase(
        GeneralInventoryCatalogDescriptor descriptor,
        GeneralInventoryCatalogItem item,
        bool copyMode = false)
        : this(descriptor)
    {
        LoadCatalog(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveGeneralInventoryCatalogRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveGeneralInventoryCatalogRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NormalizeText(memDescription.Text),
            chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = $"Nuevo {descriptor.SingularTitle}";
        lblCode.Text = descriptor.CodeLabel;
        lblName.Text = descriptor.NameLabel;
        chkIsActive.Checked = true;
        btnSave.Click += (_, _) => Save();
    }

    private void LoadCatalog(GeneralInventoryCatalogItem item, bool copyMode)
    {
        Text = copyMode ? $"Copiar {descriptor.SingularTitle}" : $"Editar {descriptor.SingularTitle}";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        chkIsActive.Checked = item.IsActive;
    }

    private void InitializeCatalogEditorComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        chkIsActive = new CheckEdit();
        btnCancel = new NuanActionButton();
        btnSave = new NuanActionButton();

        ((ISupportInitialize)txtCode.Properties).BeginInit();
        ((ISupportInitialize)txtName.Properties).BeginInit();
        ((ISupportInitialize)memDescription.Properties).BeginInit();
        ((ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();

        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = Color.Black;
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(28, 29);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(39, 15);
        lblCode.TabIndex = 0;
        lblCode.Text = "Codigo";

        txtCode.Location = new Point(170, 26);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(330, 22);
        txtCode.TabIndex = 1;

        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = Color.Black;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(28, 60);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 2;
        lblName.Text = "Nombre";

        txtName.Location = new Point(170, 57);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(330, 22);
        txtName.TabIndex = 3;

        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = Color.Black;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(28, 91);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(62, 15);
        lblDescription.TabIndex = 4;
        lblDescription.Text = "Descripcion";

        memDescription.Location = new Point(170, 88);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(330, 74);
        memDescription.TabIndex = 5;

        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(166, 173);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 6;

        btnCancel.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancel.ButtonText = "Cancelar";
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(294, 217);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 7;
        btnCancel.Text = "Cancelar";

        btnSave.ButtonKind = NuanActionButtonKind.Save;
        btnSave.ButtonText = "Guardar";
        btnSave.Location = new Point(400, 217);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 8;
        btnSave.Text = "Guardar";

        AcceptButton = btnSave;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(528, 275);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(chkIsActive);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(528, 275);
        Name = GetType().Name;
        StartPosition = FormStartPosition.CenterParent;
        Text = descriptor.SingularTitle;

        ((ISupportInitialize)txtCode.Properties).EndInit();
        ((ISupportInitialize)txtName.Properties).EndInit();
        ((ISupportInitialize)memDescription.Properties).EndInit();
        ((ISupportInitialize)chkIsActive.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static SaveGeneralInventoryCatalogRequest EmptyRequest()
    {
        return new SaveGeneralInventoryCatalogRequest(string.Empty, string.Empty, null, true);
    }
}
