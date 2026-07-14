using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Security.Access;

partial class SecurityMaintenanceFormAccessForm
{
    private System.ComponentModel.IContainer components = null;
    protected PanelControl pnlRoot;
    protected PanelControl pnlHeader;
    protected LabelControl lblTitle;
    protected SimpleButton btnSave;
    protected PanelControl pnlFilters;
    protected LabelControl lblFormFilter;
    protected LookUpEdit lueFormFilter;
    protected LabelControl lblSearch;
    protected TextEdit txtSearch;
    protected CheckEdit chkOnlyActive;
    protected PanelControl pnlRoles;
    protected LabelControl lblRolesTitle;
    protected ListBoxControl lstRoles;
    protected PanelControl pnlMain;
    protected PanelControl pnlForms;
    protected LabelControl lblFormsTitle;
    protected GridControl grcForms;
    protected GridView grvForms;
    private GridColumn colFormName;
    private GridColumn colFormKey;
    private GridColumn colFormActive;
    protected PanelControl pnlOperations;
    protected LabelControl lblOperationsTitle;
    protected GridControl grcOperations;
    protected GridView grvOperations;
    private GridColumn colOperationName;
    private GridColumn colActionKey;
    private GridColumn colRibbonGroup;
    private GridColumn colAllowed;
    protected PanelControl pnlBottom;
    protected PanelControl pnlDetail;
    protected LabelControl lblDetailTitle;
    protected LabelControl lblDetailRoleCaption;
    protected LabelControl lblDetailRole;
    protected LabelControl lblDetailFormCaption;
    protected LabelControl lblDetailForm;
    protected LabelControl lblDetailKeyCaption;
    protected LabelControl lblDetailKey;
    protected LabelControl lblDetailStatusCaption;
    protected LabelControl lblDetailStatus;
    protected PanelControl pnlAudit;
    protected LabelControl lblAuditTitle;
    protected LabelControl lblAuditUpdatedCaption;
    protected LabelControl lblAuditUpdated;
    protected LabelControl lblAuditUserCaption;
    protected LabelControl lblAuditUser;
    protected LabelControl lblAuditObservationsCaption;
    protected MemoEdit memAuditObservations;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlRoot = new PanelControl();
        pnlHeader = new PanelControl();
        lblTitle = new LabelControl();
        btnSave = new SimpleButton();
        pnlFilters = new PanelControl();
        lblFormFilter = new LabelControl();
        lueFormFilter = new LookUpEdit();
        lblSearch = new LabelControl();
        txtSearch = new TextEdit();
        chkOnlyActive = new CheckEdit();
        pnlRoles = new PanelControl();
        lblRolesTitle = new LabelControl();
        lstRoles = new ListBoxControl();
        pnlMain = new PanelControl();
        pnlForms = new PanelControl();
        lblFormsTitle = new LabelControl();
        grcForms = new GridControl();
        grvForms = new GridView();
        colFormName = new GridColumn();
        colFormKey = new GridColumn();
        colFormActive = new GridColumn();
        pnlOperations = new PanelControl();
        lblOperationsTitle = new LabelControl();
        grcOperations = new GridControl();
        grvOperations = new GridView();
        colOperationName = new GridColumn();
        colActionKey = new GridColumn();
        colRibbonGroup = new GridColumn();
        colAllowed = new GridColumn();
        pnlBottom = new PanelControl();
        pnlDetail = new PanelControl();
        lblDetailTitle = new LabelControl();
        lblDetailRoleCaption = new LabelControl();
        lblDetailRole = new LabelControl();
        lblDetailFormCaption = new LabelControl();
        lblDetailForm = new LabelControl();
        lblDetailKeyCaption = new LabelControl();
        lblDetailKey = new LabelControl();
        lblDetailStatusCaption = new LabelControl();
        lblDetailStatus = new LabelControl();
        pnlAudit = new PanelControl();
        lblAuditTitle = new LabelControl();
        lblAuditUpdatedCaption = new LabelControl();
        lblAuditUpdated = new LabelControl();
        lblAuditUserCaption = new LabelControl();
        lblAuditUser = new LabelControl();
        lblAuditObservationsCaption = new LabelControl();
        memAuditObservations = new MemoEdit();
        ((System.ComponentModel.ISupportInitialize)pnlRoot).BeginInit();
        pnlRoot.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).BeginInit();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).BeginInit();
        pnlFilters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueFormFilter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkOnlyActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlRoles).BeginInit();
        pnlRoles.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lstRoles).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
        pnlMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlForms).BeginInit();
        pnlForms.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcForms).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvForms).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlOperations).BeginInit();
        pnlOperations.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcOperations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvOperations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlBottom).BeginInit();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlDetail).BeginInit();
        pnlDetail.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAudit).BeginInit();
        pnlAudit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memAuditObservations.Properties).BeginInit();
        SuspendLayout();
        // 
        // pnlRoot
        // 
        pnlRoot.BorderStyle = BorderStyles.NoBorder;
        pnlRoot.Controls.Add(pnlMain);
        pnlRoot.Controls.Add(pnlRoles);
        pnlRoot.Controls.Add(pnlFilters);
        pnlRoot.Controls.Add(pnlHeader);
        pnlRoot.Dock = DockStyle.Fill;
        pnlRoot.Location = new Point(0, 0);
        pnlRoot.Name = "pnlRoot";
        pnlRoot.Size = new Size(1280, 720);
        pnlRoot.TabIndex = 0;
        // 
        // pnlHeader
        // 
        pnlHeader.BorderStyle = BorderStyles.NoBorder;
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(btnSave);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1280, 54);
        pnlHeader.TabIndex = 0;
        // 
        // lblTitle
        // 
        lblTitle.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.Appearance.ForeColor = BrandResources.Text;
        lblTitle.Appearance.Options.UseFont = true;
        lblTitle.Appearance.Options.UseForeColor = true;
        lblTitle.Location = new Point(18, 17);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(340, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Accesos a Formularios de Mantenimiento";
        // 
        // btnSave
        // 
        btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSave.Appearance.BackColor = BrandResources.Primary;
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.AppearanceHovered.BackColor = BrandResources.PrimaryHover;
        btnSave.AppearanceHovered.Options.UseBackColor = true;
        btnSave.Location = new Point(1128, 10);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(134, 34);
        btnSave.TabIndex = 1;
        btnSave.Text = "Guardar cambios";
        // 
        // pnlFilters
        // 
        pnlFilters.BorderStyle = BorderStyles.NoBorder;
        pnlFilters.Controls.Add(lblFormFilter);
        pnlFilters.Controls.Add(lueFormFilter);
        pnlFilters.Controls.Add(lblSearch);
        pnlFilters.Controls.Add(txtSearch);
        pnlFilters.Controls.Add(chkOnlyActive);
        pnlFilters.Dock = DockStyle.Top;
        pnlFilters.Location = new Point(0, 54);
        pnlFilters.Name = "pnlFilters";
        pnlFilters.Size = new Size(1280, 48);
        pnlFilters.TabIndex = 1;
        // 
        // lblFormFilter
        // 
        lblFormFilter.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblFormFilter.Appearance.ForeColor = BrandResources.Text;
        lblFormFilter.Appearance.Options.UseFont = true;
        lblFormFilter.Appearance.Options.UseForeColor = true;
        lblFormFilter.Location = new Point(248, 16);
        lblFormFilter.Name = "lblFormFilter";
        lblFormFilter.Size = new Size(107, 15);
        lblFormFilter.TabIndex = 0;
        lblFormFilter.Text = "Formulario/Modulo:";
        // 
        // lueFormFilter
        // 
        lueFormFilter.Location = new Point(365, 12);
        lueFormFilter.Name = "lueFormFilter";
        lueFormFilter.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lueFormFilter.Properties.Appearance.Options.UseFont = true;
        lueFormFilter.Properties.AutoHeight = false;
        lueFormFilter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFormFilter.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Codigo", 90), new LookUpColumnInfo("Name", "Nombre", 170) });
        lueFormFilter.Properties.DisplayMember = "Name";
        lueFormFilter.Properties.NullText = "";
        lueFormFilter.Properties.ValueMember = "Id";
        lueFormFilter.Size = new Size(260, 22);
        lueFormFilter.TabIndex = 1;
        // 
        // lblSearch
        // 
        lblSearch.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblSearch.Appearance.ForeColor = BrandResources.Text;
        lblSearch.Appearance.Options.UseFont = true;
        lblSearch.Appearance.Options.UseForeColor = true;
        lblSearch.Location = new Point(648, 16);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(40, 15);
        lblSearch.TabIndex = 2;
        lblSearch.Text = "Buscar:";
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(696, 12);
        txtSearch.Name = "txtSearch";
        txtSearch.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtSearch.Properties.Appearance.Options.UseFont = true;
        txtSearch.Properties.AutoHeight = false;
        txtSearch.Properties.NullValuePrompt = "Buscar operacion...";
        txtSearch.Size = new Size(260, 22);
        txtSearch.TabIndex = 3;
        // 
        // chkOnlyActive
        // 
        chkOnlyActive.EditValue = true;
        chkOnlyActive.Location = new Point(976, 11);
        chkOnlyActive.Name = "chkOnlyActive";
        chkOnlyActive.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        chkOnlyActive.Properties.Appearance.Options.UseFont = true;
        chkOnlyActive.Properties.Caption = "Mostrar solo activos";
        chkOnlyActive.Size = new Size(150, 24);
        chkOnlyActive.TabIndex = 4;
        // 
        // pnlRoles
        // 
        pnlRoles.Controls.Add(lblRolesTitle);
        pnlRoles.Controls.Add(lstRoles);
        pnlRoles.Dock = DockStyle.Left;
        pnlRoles.Location = new Point(0, 102);
        pnlRoles.Name = "pnlRoles";
        pnlRoles.Size = new Size(220, 618);
        pnlRoles.TabIndex = 2;
        // 
        // lblRolesTitle
        // 
        lblRolesTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblRolesTitle.Appearance.ForeColor = BrandResources.Text;
        lblRolesTitle.Appearance.Options.UseFont = true;
        lblRolesTitle.Appearance.Options.UseForeColor = true;
        lblRolesTitle.Location = new Point(12, 10);
        lblRolesTitle.Name = "lblRolesTitle";
        lblRolesTitle.Size = new Size(34, 15);
        lblRolesTitle.TabIndex = 0;
        lblRolesTitle.Text = "ROLES";
        // 
        // lstRoles
        // 
        lstRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstRoles.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lstRoles.Appearance.Options.UseFont = true;
        lstRoles.Location = new Point(8, 34);
        lstRoles.Name = "lstRoles";
        lstRoles.Size = new Size(204, 574);
        lstRoles.TabIndex = 1;
        // 
        // pnlMain
        // 
        pnlMain.BorderStyle = BorderStyles.NoBorder;
        pnlMain.Controls.Add(pnlOperations);
        pnlMain.Controls.Add(pnlForms);
        pnlMain.Controls.Add(pnlBottom);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(220, 102);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(1060, 618);
        pnlMain.TabIndex = 3;
        // 
        // pnlForms
        // 
        pnlForms.Controls.Add(lblFormsTitle);
        pnlForms.Controls.Add(grcForms);
        pnlForms.Dock = DockStyle.Left;
        pnlForms.Location = new Point(0, 0);
        pnlForms.Name = "pnlForms";
        pnlForms.Size = new Size(285, 418);
        pnlForms.TabIndex = 0;
        // 
        // lblFormsTitle
        // 
        lblFormsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblFormsTitle.Appearance.ForeColor = BrandResources.Text;
        lblFormsTitle.Appearance.Options.UseFont = true;
        lblFormsTitle.Appearance.Options.UseForeColor = true;
        lblFormsTitle.Location = new Point(12, 10);
        lblFormsTitle.Name = "lblFormsTitle";
        lblFormsTitle.Size = new Size(180, 15);
        lblFormsTitle.TabIndex = 0;
        lblFormsTitle.Text = "FORMULARIOS DE MANTENIMIENTO";
        // 
        // grcForms
        // 
        grcForms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grcForms.Location = new Point(8, 34);
        grcForms.MainView = grvForms;
        grcForms.Name = "grcForms";
        grcForms.Size = new Size(269, 376);
        grcForms.TabIndex = 1;
        grcForms.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvForms });
        // 
        // grvForms
        // 
        grvForms.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvForms.Appearance.HeaderPanel.Options.UseFont = true;
        grvForms.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvForms.Appearance.Row.Options.UseFont = true;
        grvForms.Columns.AddRange(new GridColumn[] { colFormName, colFormKey, colFormActive });
        grvForms.GridControl = grcForms;
        grvForms.Name = "grvForms";
        grvForms.OptionsBehavior.Editable = false;
        grvForms.OptionsView.ShowGroupPanel = false;
        grvForms.OptionsView.ShowIndicator = false;
        // 
        // colFormName
        // 
        colFormName.Caption = "Formulario";
        colFormName.FieldName = "Name";
        colFormName.Name = "colFormName";
        colFormName.Visible = true;
        colFormName.VisibleIndex = 0;
        colFormName.Width = 170;
        // 
        // colFormKey
        // 
        colFormKey.Caption = "FormKey";
        colFormKey.FieldName = "FormKey";
        colFormKey.Name = "colFormKey";
        colFormKey.Visible = true;
        colFormKey.VisibleIndex = 1;
        colFormKey.Width = 90;
        // 
        // colFormActive
        // 
        colFormActive.Caption = "Activo";
        colFormActive.FieldName = "IsActive";
        colFormActive.Name = "colFormActive";
        colFormActive.Visible = true;
        colFormActive.VisibleIndex = 2;
        colFormActive.Width = 55;
        // 
        // pnlOperations
        // 
        pnlOperations.Controls.Add(lblOperationsTitle);
        pnlOperations.Controls.Add(grcOperations);
        pnlOperations.Dock = DockStyle.Fill;
        pnlOperations.Location = new Point(285, 0);
        pnlOperations.Name = "pnlOperations";
        pnlOperations.Size = new Size(775, 418);
        pnlOperations.TabIndex = 1;
        // 
        // lblOperationsTitle
        // 
        lblOperationsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblOperationsTitle.Appearance.ForeColor = BrandResources.Text;
        lblOperationsTitle.Appearance.Options.UseFont = true;
        lblOperationsTitle.Appearance.Options.UseForeColor = true;
        lblOperationsTitle.Location = new Point(12, 10);
        lblOperationsTitle.Name = "lblOperationsTitle";
        lblOperationsTitle.Size = new Size(150, 15);
        lblOperationsTitle.TabIndex = 0;
        lblOperationsTitle.Text = "OPERACIONES CONFIGURABLES";
        // 
        // grcOperations
        // 
        grcOperations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grcOperations.Location = new Point(8, 34);
        grcOperations.MainView = grvOperations;
        grcOperations.Name = "grcOperations";
        grcOperations.Size = new Size(759, 376);
        grcOperations.TabIndex = 1;
        grcOperations.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvOperations });
        // 
        // grvOperations
        // 
        grvOperations.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvOperations.Appearance.HeaderPanel.Options.UseFont = true;
        grvOperations.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvOperations.Appearance.Row.Options.UseFont = true;
        grvOperations.Columns.AddRange(new GridColumn[] { colOperationName, colActionKey, colRibbonGroup, colAllowed });
        grvOperations.GridControl = grcOperations;
        grvOperations.Name = "grvOperations";
        grvOperations.OptionsView.ShowGroupPanel = false;
        grvOperations.OptionsView.ShowIndicator = false;
        // 
        // colOperationName
        // 
        colOperationName.Caption = "Operacion";
        colOperationName.FieldName = "OperationName";
        colOperationName.Name = "colOperationName";
        colOperationName.OptionsColumn.AllowEdit = false;
        colOperationName.Visible = true;
        colOperationName.VisibleIndex = 0;
        colOperationName.Width = 220;
        // 
        // colActionKey
        // 
        colActionKey.Caption = "ActionKey";
        colActionKey.FieldName = "ActionKey";
        colActionKey.Name = "colActionKey";
        colActionKey.OptionsColumn.AllowEdit = false;
        colActionKey.Visible = true;
        colActionKey.VisibleIndex = 1;
        colActionKey.Width = 180;
        // 
        // colRibbonGroup
        // 
        colRibbonGroup.Caption = "Grupo";
        colRibbonGroup.FieldName = "RibbonGroupName";
        colRibbonGroup.Name = "colRibbonGroup";
        colRibbonGroup.OptionsColumn.AllowEdit = false;
        colRibbonGroup.Visible = true;
        colRibbonGroup.VisibleIndex = 2;
        colRibbonGroup.Width = 160;
        // 
        // colAllowed
        // 
        colAllowed.Caption = "Permitido";
        colAllowed.FieldName = "IsAllowed";
        colAllowed.Name = "colAllowed";
        colAllowed.Visible = true;
        colAllowed.VisibleIndex = 3;
        colAllowed.Width = 90;
        // 
        // pnlBottom
        // 
        pnlBottom.BorderStyle = BorderStyles.NoBorder;
        pnlBottom.Controls.Add(pnlAudit);
        pnlBottom.Controls.Add(pnlDetail);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 418);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Size = new Size(1060, 200);
        pnlBottom.TabIndex = 2;
        // 
        // pnlDetail
        // 
        pnlDetail.Controls.Add(lblDetailTitle);
        pnlDetail.Controls.Add(lblDetailRoleCaption);
        pnlDetail.Controls.Add(lblDetailRole);
        pnlDetail.Controls.Add(lblDetailFormCaption);
        pnlDetail.Controls.Add(lblDetailForm);
        pnlDetail.Controls.Add(lblDetailKeyCaption);
        pnlDetail.Controls.Add(lblDetailKey);
        pnlDetail.Controls.Add(lblDetailStatusCaption);
        pnlDetail.Controls.Add(lblDetailStatus);
        pnlDetail.Dock = DockStyle.Left;
        pnlDetail.Location = new Point(0, 0);
        pnlDetail.Name = "pnlDetail";
        pnlDetail.Size = new Size(420, 200);
        pnlDetail.TabIndex = 0;
        // 
        // lblDetailTitle
        // 
        lblDetailTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblDetailTitle.Appearance.ForeColor = BrandResources.Text;
        lblDetailTitle.Appearance.Options.UseFont = true;
        lblDetailTitle.Appearance.Options.UseForeColor = true;
        lblDetailTitle.Location = new Point(12, 12);
        lblDetailTitle.Name = "lblDetailTitle";
        lblDetailTitle.Size = new Size(160, 15);
        lblDetailTitle.TabIndex = 0;
        lblDetailTitle.Text = "Detalle del acceso seleccionado";
        // 
        // lblDetailRoleCaption
        // 
        lblDetailRoleCaption.Location = new Point(32, 52);
        lblDetailRoleCaption.Name = "lblDetailRoleCaption";
        lblDetailRoleCaption.Size = new Size(19, 13);
        lblDetailRoleCaption.TabIndex = 1;
        lblDetailRoleCaption.Text = "Rol:";
        // 
        // lblDetailRole
        // 
        lblDetailRole.Location = new Point(134, 52);
        lblDetailRole.Name = "lblDetailRole";
        lblDetailRole.Size = new Size(4, 13);
        lblDetailRole.TabIndex = 2;
        lblDetailRole.Text = "-";
        // 
        // lblDetailFormCaption
        // 
        lblDetailFormCaption.Location = new Point(32, 80);
        lblDetailFormCaption.Name = "lblDetailFormCaption";
        lblDetailFormCaption.Size = new Size(56, 13);
        lblDetailFormCaption.TabIndex = 3;
        lblDetailFormCaption.Text = "Formulario:";
        // 
        // lblDetailForm
        // 
        lblDetailForm.Location = new Point(134, 80);
        lblDetailForm.Name = "lblDetailForm";
        lblDetailForm.Size = new Size(4, 13);
        lblDetailForm.TabIndex = 4;
        lblDetailForm.Text = "-";
        // 
        // lblDetailKeyCaption
        // 
        lblDetailKeyCaption.Location = new Point(32, 108);
        lblDetailKeyCaption.Name = "lblDetailKeyCaption";
        lblDetailKeyCaption.Size = new Size(77, 13);
        lblDetailKeyCaption.TabIndex = 5;
        lblDetailKeyCaption.Text = "Codigo/FormKey:";
        // 
        // lblDetailKey
        // 
        lblDetailKey.Location = new Point(134, 108);
        lblDetailKey.Name = "lblDetailKey";
        lblDetailKey.Size = new Size(4, 13);
        lblDetailKey.TabIndex = 6;
        lblDetailKey.Text = "-";
        // 
        // lblDetailStatusCaption
        // 
        lblDetailStatusCaption.Location = new Point(32, 136);
        lblDetailStatusCaption.Name = "lblDetailStatusCaption";
        lblDetailStatusCaption.Size = new Size(37, 13);
        lblDetailStatusCaption.TabIndex = 7;
        lblDetailStatusCaption.Text = "Estado:";
        // 
        // lblDetailStatus
        // 
        lblDetailStatus.Location = new Point(134, 136);
        lblDetailStatus.Name = "lblDetailStatus";
        lblDetailStatus.Size = new Size(4, 13);
        lblDetailStatus.TabIndex = 8;
        lblDetailStatus.Text = "-";
        // 
        // pnlAudit
        // 
        pnlAudit.Controls.Add(lblAuditTitle);
        pnlAudit.Controls.Add(lblAuditUpdatedCaption);
        pnlAudit.Controls.Add(lblAuditUpdated);
        pnlAudit.Controls.Add(lblAuditUserCaption);
        pnlAudit.Controls.Add(lblAuditUser);
        pnlAudit.Controls.Add(lblAuditObservationsCaption);
        pnlAudit.Controls.Add(memAuditObservations);
        pnlAudit.Dock = DockStyle.Fill;
        pnlAudit.Location = new Point(420, 0);
        pnlAudit.Name = "pnlAudit";
        pnlAudit.Size = new Size(640, 200);
        pnlAudit.TabIndex = 1;
        // 
        // lblAuditTitle
        // 
        lblAuditTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblAuditTitle.Appearance.ForeColor = BrandResources.Text;
        lblAuditTitle.Appearance.Options.UseFont = true;
        lblAuditTitle.Appearance.Options.UseForeColor = true;
        lblAuditTitle.Location = new Point(12, 12);
        lblAuditTitle.Name = "lblAuditTitle";
        lblAuditTitle.Size = new Size(195, 15);
        lblAuditTitle.TabIndex = 0;
        lblAuditTitle.Text = "Informacion de auditoria / Observaciones";
        // 
        // lblAuditUpdatedCaption
        // 
        lblAuditUpdatedCaption.Location = new Point(24, 52);
        lblAuditUpdatedCaption.Name = "lblAuditUpdatedCaption";
        lblAuditUpdatedCaption.Size = new Size(99, 13);
        lblAuditUpdatedCaption.TabIndex = 1;
        lblAuditUpdatedCaption.Text = "Ultima modificacion:";
        // 
        // lblAuditUpdated
        // 
        lblAuditUpdated.Location = new Point(150, 52);
        lblAuditUpdated.Name = "lblAuditUpdated";
        lblAuditUpdated.Size = new Size(4, 13);
        lblAuditUpdated.TabIndex = 2;
        lblAuditUpdated.Text = "-";
        // 
        // lblAuditUserCaption
        // 
        lblAuditUserCaption.Location = new Point(24, 80);
        lblAuditUserCaption.Name = "lblAuditUserCaption";
        lblAuditUserCaption.Size = new Size(40, 13);
        lblAuditUserCaption.TabIndex = 3;
        lblAuditUserCaption.Text = "Usuario:";
        // 
        // lblAuditUser
        // 
        lblAuditUser.Location = new Point(150, 80);
        lblAuditUser.Name = "lblAuditUser";
        lblAuditUser.Size = new Size(4, 13);
        lblAuditUser.TabIndex = 4;
        lblAuditUser.Text = "-";
        // 
        // lblAuditObservationsCaption
        // 
        lblAuditObservationsCaption.Location = new Point(24, 110);
        lblAuditObservationsCaption.Name = "lblAuditObservationsCaption";
        lblAuditObservationsCaption.Size = new Size(77, 13);
        lblAuditObservationsCaption.TabIndex = 5;
        lblAuditObservationsCaption.Text = "Observaciones:";
        // 
        // memAuditObservations
        // 
        memAuditObservations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        memAuditObservations.Location = new Point(150, 108);
        memAuditObservations.Name = "memAuditObservations";
        memAuditObservations.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        memAuditObservations.Properties.Appearance.Options.UseFont = true;
        memAuditObservations.Properties.NullValuePrompt = "Ingrese observaciones (opcional)...";
        memAuditObservations.Size = new Size(470, 74);
        memAuditObservations.TabIndex = 6;
        // 
        // SecurityMaintenanceFormAccessForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1280, 720);
        Controls.Add(pnlRoot);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Name = "SecurityMaintenanceFormAccessForm";
        Text = "Accesos a Formularios de Mantenimiento";
        ((System.ComponentModel.ISupportInitialize)pnlRoot).EndInit();
        pnlRoot.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlHeader).EndInit();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).EndInit();
        pnlFilters.ResumeLayout(false);
        pnlFilters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueFormFilter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkOnlyActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlRoles).EndInit();
        pnlRoles.ResumeLayout(false);
        pnlRoles.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lstRoles).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
        pnlMain.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlForms).EndInit();
        pnlForms.ResumeLayout(false);
        pnlForms.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grcForms).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvForms).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlOperations).EndInit();
        pnlOperations.ResumeLayout(false);
        pnlOperations.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grcOperations).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvOperations).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlBottom).EndInit();
        pnlBottom.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlDetail).EndInit();
        pnlDetail.ResumeLayout(false);
        pnlDetail.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAudit).EndInit();
        pnlAudit.ResumeLayout(false);
        pnlAudit.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memAuditObservations.Properties).EndInit();
        ResumeLayout(false);
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
