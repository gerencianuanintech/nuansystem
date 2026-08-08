using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Lookups;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

partial class WarehouseEditForm
{
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private CheckEdit chkIsActive;
    private LabelControl lblGeneral;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblBranchCode;
    private TextEdit txtBranchCode;
    private LabelControl lblAddress;
    private TextEdit txtAddress;
    private LabelControl lblCity;
    private NuanLookupEdit lueCity;
    private LabelControl lblProvince;
    private NuanLookupEdit lueProvince;
    private LabelControl lblCountry;
    private NuanLookupEdit lueCountry;
    private LabelControl lblPhone;
    private TextEdit txtPhone;
    private LabelControl lblEmail;
    private TextEdit txtEmail;
    private LabelControl lblManagerName;
    private TextEdit txtManagerName;
    private LabelControl lblOperation;
    private CheckEdit chkAllowsSales;
    private CheckEdit chkAllowsPurchases;
    private CheckEdit chkAllowsTransfers;
    private CheckEdit chkAllowsProduction;
    private CheckEdit chkIsDefault;
    private LabelControl lblIntegration;
    private LabelControl lblExternalSystem;
    private TextEdit txtExternalSystem;
    private LabelControl lblExternalCode;
    private TextEdit txtExternalCode;
    private LabelControl lblSapCode;
    private TextEdit txtSapCode;
    private LabelControl lblGlobalId;
    private TextEdit txtGlobalId;

    private void InitializeComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        chkIsActive = new CheckEdit();
        lblGeneral = new LabelControl();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblBranchCode = new LabelControl();
        txtBranchCode = new TextEdit();
        lblAddress = new LabelControl();
        txtAddress = new TextEdit();
        lblCity = new LabelControl();
        lueCity = new NuanLookupEdit();
        lblProvince = new LabelControl();
        lueProvince = new NuanLookupEdit();
        lblCountry = new LabelControl();
        lueCountry = new NuanLookupEdit();
        lblPhone = new LabelControl();
        txtPhone = new TextEdit();
        lblEmail = new LabelControl();
        txtEmail = new TextEdit();
        lblManagerName = new LabelControl();
        txtManagerName = new TextEdit();
        lblOperation = new LabelControl();
        chkAllowsSales = new CheckEdit();
        chkAllowsPurchases = new CheckEdit();
        chkAllowsTransfers = new CheckEdit();
        chkAllowsProduction = new CheckEdit();
        chkIsDefault = new CheckEdit();
        lblIntegration = new LabelControl();
        lblExternalSystem = new LabelControl();
        txtExternalSystem = new TextEdit();
        lblExternalCode = new LabelControl();
        txtExternalCode = new TextEdit();
        lblSapCode = new LabelControl();
        txtSapCode = new TextEdit();
        lblGlobalId = new LabelControl();
        txtGlobalId = new TextEdit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBranchCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAddress.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProvince.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtManagerName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsPurchases.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsTransfers.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsProduction.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtGlobalId.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseBorderColor = true;
        btnCancelar.Appearance.Options.UseFont = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseBorderColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.ForeColor = Color.White;
        btnCancelar.AppearancePressed.Options.UseBackColor = true;
        btnCancelar.AppearancePressed.Options.UseBorderColor = true;
        btnCancelar.AppearancePressed.Options.UseForeColor = true;
        btnCancelar.ImageOptions.ImageToTextIndent = 0;
        btnCancelar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnCancelar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnCancelar.Location = new Point(518, 521);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseBorderColor = true;
        btnGuardar.Appearance.Options.UseFont = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseBorderColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.ForeColor = Color.White;
        btnGuardar.AppearancePressed.Options.UseBackColor = true;
        btnGuardar.AppearancePressed.Options.UseBorderColor = true;
        btnGuardar.AppearancePressed.Options.UseForeColor = true;
        btnGuardar.ImageOptions.ImageToTextIndent = 0;
        btnGuardar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnGuardar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnGuardar.Location = new Point(624, 521);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // lblCode
        // 
        lblCode.Location = new Point(24, 24);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(33, 13);
        lblCode.TabIndex = 0;
        lblCode.Text = "Codigo";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(132, 21);
        txtCode.Name = "txtCode";
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(180, 20);
        txtCode.TabIndex = 1;
        // 
        // lblName
        // 
        lblName.Location = new Point(334, 24);
        lblName.Name = "lblName";
        lblName.Size = new Size(37, 13);
        lblName.TabIndex = 2;
        lblName.Text = "Nombre";
        // 
        // txtName
        // 
        txtName.Location = new Point(414, 21);
        txtName.Name = "txtName";
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(310, 20);
        txtName.TabIndex = 3;
        // 
        // chkIsActive
        // 
        chkIsActive.Location = new Point(130, 47);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(90, 20);
        chkIsActive.TabIndex = 4;
        // 
        // lblGeneral
        // 
        lblGeneral.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneral.Appearance.Options.UseFont = true;
        lblGeneral.Location = new Point(24, 87);
        lblGeneral.Name = "lblGeneral";
        lblGeneral.Size = new Size(139, 20);
        lblGeneral.TabIndex = 5;
        lblGeneral.Text = "Informacion general";
        // 
        // lblDescription
        // 
        lblDescription.Location = new Point(24, 128);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(54, 13);
        lblDescription.TabIndex = 6;
        lblDescription.Text = "Descripcion";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(132, 125);
        memDescription.Name = "memDescription";
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(592, 52);
        memDescription.TabIndex = 7;
        // 
        // lblBranchCode
        // 
        lblBranchCode.Location = new Point(24, 186);
        lblBranchCode.Name = "lblBranchCode";
        lblBranchCode.Size = new Size(40, 13);
        lblBranchCode.TabIndex = 8;
        lblBranchCode.Text = "Sucursal";
        // 
        // txtBranchCode
        // 
        txtBranchCode.Location = new Point(132, 183);
        txtBranchCode.Name = "txtBranchCode";
        txtBranchCode.Properties.MaxLength = 50;
        txtBranchCode.Size = new Size(180, 20);
        txtBranchCode.TabIndex = 9;
        // 
        // lblAddress
        // 
        lblAddress.Location = new Point(334, 186);
        lblAddress.Name = "lblAddress";
        lblAddress.Size = new Size(43, 13);
        lblAddress.TabIndex = 10;
        lblAddress.Text = "Direccion";
        // 
        // txtAddress
        // 
        txtAddress.Location = new Point(414, 183);
        txtAddress.Name = "txtAddress";
        txtAddress.Properties.MaxLength = 250;
        txtAddress.Size = new Size(310, 20);
        txtAddress.TabIndex = 11;
        // 
        // lblCity
        // 
        lblCity.Location = new Point(568, 212);
        lblCity.Name = "lblCity";
        lblCity.Size = new Size(33, 13);
        lblCity.TabIndex = 12;
        lblCity.Text = "Ciudad";
        // 
        // lueCity
        // 
        lueCity.Location = new Point(612, 209);
        lueCity.Name = "lueCity";
        lueCity.Size = new Size(112, 20);
        lueCity.TabIndex = 13;
        // 
        // lblProvince
        // 
        lblProvince.Location = new Point(334, 212);
        lblProvince.Name = "lblProvince";
        lblProvince.Size = new Size(43, 13);
        lblProvince.TabIndex = 14;
        lblProvince.Text = "Provincia";
        // 
        // lueProvince
        // 
        lueProvince.Location = new Point(414, 209);
        lueProvince.Name = "lueProvince";
        lueProvince.Size = new Size(135, 20);
        lueProvince.TabIndex = 15;
        // 
        // lblCountry
        // 
        lblCountry.Location = new Point(24, 212);
        lblCountry.Name = "lblCountry";
        lblCountry.Size = new Size(19, 13);
        lblCountry.TabIndex = 16;
        lblCountry.Text = "Pais";
        // 
        // lueCountry
        // 
        lueCountry.Location = new Point(132, 209);
        lueCountry.Name = "lueCountry";
        lueCountry.Size = new Size(180, 20);
        lueCountry.TabIndex = 17;
        // 
        // lblPhone
        // 
        lblPhone.Location = new Point(24, 238);
        lblPhone.Name = "lblPhone";
        lblPhone.Size = new Size(42, 13);
        lblPhone.TabIndex = 18;
        lblPhone.Text = "Telefono";
        // 
        // txtPhone
        // 
        txtPhone.Location = new Point(132, 235);
        txtPhone.Name = "txtPhone";
        txtPhone.Properties.MaxLength = 50;
        txtPhone.Size = new Size(180, 20);
        txtPhone.TabIndex = 19;
        // 
        // lblEmail
        // 
        lblEmail.Location = new Point(334, 238);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(24, 13);
        lblEmail.TabIndex = 20;
        lblEmail.Text = "Email";
        // 
        // txtEmail
        // 
        txtEmail.Location = new Point(414, 235);
        txtEmail.Name = "txtEmail";
        txtEmail.Properties.MaxLength = 150;
        txtEmail.Size = new Size(310, 20);
        txtEmail.TabIndex = 21;
        // 
        // lblManagerName
        // 
        lblManagerName.Location = new Point(24, 264);
        lblManagerName.Name = "lblManagerName";
        lblManagerName.Size = new Size(61, 13);
        lblManagerName.TabIndex = 22;
        lblManagerName.Text = "Responsable";
        // 
        // txtManagerName
        // 
        txtManagerName.Location = new Point(132, 261);
        txtManagerName.Name = "txtManagerName";
        txtManagerName.Properties.MaxLength = 150;
        txtManagerName.Size = new Size(592, 20);
        txtManagerName.TabIndex = 23;
        // 
        // lblOperation
        // 
        lblOperation.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblOperation.Appearance.Options.UseFont = true;
        lblOperation.Location = new Point(24, 304);
        lblOperation.Name = "lblOperation";
        lblOperation.Size = new Size(71, 20);
        lblOperation.TabIndex = 24;
        lblOperation.Text = "Operacion";
        // 
        // chkAllowsSales
        // 
        chkAllowsSales.Location = new Point(132, 337);
        chkAllowsSales.Name = "chkAllowsSales";
        chkAllowsSales.Properties.Caption = "Permite ventas";
        chkAllowsSales.Size = new Size(130, 20);
        chkAllowsSales.TabIndex = 25;
        // 
        // chkAllowsPurchases
        // 
        chkAllowsPurchases.Location = new Point(282, 337);
        chkAllowsPurchases.Name = "chkAllowsPurchases";
        chkAllowsPurchases.Properties.Caption = "Permite compras";
        chkAllowsPurchases.Size = new Size(140, 20);
        chkAllowsPurchases.TabIndex = 26;
        // 
        // chkAllowsTransfers
        // 
        chkAllowsTransfers.Location = new Point(442, 337);
        chkAllowsTransfers.Name = "chkAllowsTransfers";
        chkAllowsTransfers.Properties.Caption = "Permite transferencias";
        chkAllowsTransfers.Size = new Size(160, 20);
        chkAllowsTransfers.TabIndex = 27;
        // 
        // chkAllowsProduction
        // 
        chkAllowsProduction.Location = new Point(132, 363);
        chkAllowsProduction.Name = "chkAllowsProduction";
        chkAllowsProduction.Properties.Caption = "Permite produccion";
        chkAllowsProduction.Size = new Size(150, 20);
        chkAllowsProduction.TabIndex = 28;
        // 
        // chkIsDefault
        // 
        chkIsDefault.Location = new Point(282, 363);
        chkIsDefault.Name = "chkIsDefault";
        chkIsDefault.Properties.Caption = "Predeterminada";
        chkIsDefault.Size = new Size(130, 20);
        chkIsDefault.TabIndex = 29;
        // 
        // lblIntegration
        // 
        lblIntegration.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblIntegration.Appearance.Options.UseFont = true;
        lblIntegration.Location = new Point(24, 409);
        lblIntegration.Name = "lblIntegration";
        lblIntegration.Size = new Size(134, 20);
        lblIntegration.TabIndex = 30;
        lblIntegration.Text = "Integracion externa";
        // 
        // lblExternalSystem
        // 
        lblExternalSystem.Location = new Point(24, 452);
        lblExternalSystem.Name = "lblExternalSystem";
        lblExternalSystem.Size = new Size(37, 13);
        lblExternalSystem.TabIndex = 31;
        lblExternalSystem.Text = "Sistema";
        // 
        // txtExternalSystem
        // 
        txtExternalSystem.Location = new Point(132, 449);
        txtExternalSystem.Name = "txtExternalSystem";
        txtExternalSystem.Properties.MaxLength = 50;
        txtExternalSystem.Size = new Size(180, 20);
        txtExternalSystem.TabIndex = 32;
        // 
        // lblExternalCode
        // 
        lblExternalCode.Location = new Point(334, 452);
        lblExternalCode.Name = "lblExternalCode";
        lblExternalCode.Size = new Size(74, 13);
        lblExternalCode.TabIndex = 33;
        lblExternalCode.Text = "Codigo externo";
        // 
        // txtExternalCode
        // 
        txtExternalCode.Location = new Point(414, 449);
        txtExternalCode.Name = "txtExternalCode";
        txtExternalCode.Properties.MaxLength = 100;
        txtExternalCode.Size = new Size(135, 20);
        txtExternalCode.TabIndex = 34;
        // 
        // lblSapCode
        // 
        lblSapCode.Location = new Point(568, 452);
        lblSapCode.Name = "lblSapCode";
        lblSapCode.Size = new Size(19, 13);
        lblSapCode.TabIndex = 35;
        lblSapCode.Text = "SAP";
        // 
        // txtSapCode
        // 
        txtSapCode.Location = new Point(612, 449);
        txtSapCode.Name = "txtSapCode";
        txtSapCode.Properties.MaxLength = 100;
        txtSapCode.Size = new Size(112, 20);
        txtSapCode.TabIndex = 36;
        // 
        // lblGlobalId
        // 
        lblGlobalId.Location = new Point(24, 478);
        lblGlobalId.Name = "lblGlobalId";
        lblGlobalId.Size = new Size(39, 13);
        lblGlobalId.TabIndex = 37;
        lblGlobalId.Text = "GlobalId";
        // 
        // txtGlobalId
        // 
        txtGlobalId.Location = new Point(132, 475);
        txtGlobalId.Name = "txtGlobalId";
        txtGlobalId.Properties.ReadOnly = true;
        txtGlobalId.Size = new Size(592, 20);
        txtGlobalId.TabIndex = 38;
        // 
        // WarehouseEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(760, 578);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(chkIsActive);
        Controls.Add(lblGeneral);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblBranchCode);
        Controls.Add(txtBranchCode);
        Controls.Add(lblAddress);
        Controls.Add(txtAddress);
        Controls.Add(lblCity);
        Controls.Add(lueCity);
        Controls.Add(lblProvince);
        Controls.Add(lueProvince);
        Controls.Add(lblCountry);
        Controls.Add(lueCountry);
        Controls.Add(lblPhone);
        Controls.Add(txtPhone);
        Controls.Add(lblEmail);
        Controls.Add(txtEmail);
        Controls.Add(lblManagerName);
        Controls.Add(txtManagerName);
        Controls.Add(lblOperation);
        Controls.Add(chkAllowsSales);
        Controls.Add(chkAllowsPurchases);
        Controls.Add(chkAllowsTransfers);
        Controls.Add(chkAllowsProduction);
        Controls.Add(chkIsDefault);
        Controls.Add(lblIntegration);
        Controls.Add(lblExternalSystem);
        Controls.Add(txtExternalSystem);
        Controls.Add(lblExternalCode);
        Controls.Add(txtExternalCode);
        Controls.Add(lblSapCode);
        Controls.Add(txtSapCode);
        Controls.Add(lblGlobalId);
        Controls.Add(txtGlobalId);
        MinimumSize = new Size(760, 610);
        Name = "WarehouseEditForm";
        Text = "Nueva bodega";
        Controls.SetChildIndex(txtGlobalId, 0);
        Controls.SetChildIndex(lblGlobalId, 0);
        Controls.SetChildIndex(txtSapCode, 0);
        Controls.SetChildIndex(lblSapCode, 0);
        Controls.SetChildIndex(txtExternalCode, 0);
        Controls.SetChildIndex(lblExternalCode, 0);
        Controls.SetChildIndex(txtExternalSystem, 0);
        Controls.SetChildIndex(lblExternalSystem, 0);
        Controls.SetChildIndex(lblIntegration, 0);
        Controls.SetChildIndex(chkIsDefault, 0);
        Controls.SetChildIndex(chkAllowsProduction, 0);
        Controls.SetChildIndex(chkAllowsTransfers, 0);
        Controls.SetChildIndex(chkAllowsPurchases, 0);
        Controls.SetChildIndex(chkAllowsSales, 0);
        Controls.SetChildIndex(lblOperation, 0);
        Controls.SetChildIndex(txtManagerName, 0);
        Controls.SetChildIndex(lblManagerName, 0);
        Controls.SetChildIndex(txtEmail, 0);
        Controls.SetChildIndex(lblEmail, 0);
        Controls.SetChildIndex(txtPhone, 0);
        Controls.SetChildIndex(lblPhone, 0);
        Controls.SetChildIndex(lueCountry, 0);
        Controls.SetChildIndex(lblCountry, 0);
        Controls.SetChildIndex(lueProvince, 0);
        Controls.SetChildIndex(lblProvince, 0);
        Controls.SetChildIndex(lueCity, 0);
        Controls.SetChildIndex(lblCity, 0);
        Controls.SetChildIndex(txtAddress, 0);
        Controls.SetChildIndex(lblAddress, 0);
        Controls.SetChildIndex(txtBranchCode, 0);
        Controls.SetChildIndex(lblBranchCode, 0);
        Controls.SetChildIndex(memDescription, 0);
        Controls.SetChildIndex(lblDescription, 0);
        Controls.SetChildIndex(lblGeneral, 0);
        Controls.SetChildIndex(chkIsActive, 0);
        Controls.SetChildIndex(txtName, 0);
        Controls.SetChildIndex(lblName, 0);
        Controls.SetChildIndex(txtCode, 0);
        Controls.SetChildIndex(lblCode, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBranchCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAddress.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueProvince.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtManagerName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsPurchases.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsTransfers.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAllowsProduction.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtGlobalId.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
