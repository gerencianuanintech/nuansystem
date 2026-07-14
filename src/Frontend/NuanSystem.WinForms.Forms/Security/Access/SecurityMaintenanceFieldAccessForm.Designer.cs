using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Security.Access;

partial class SecurityMaintenanceFieldAccessForm
{
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
    protected PanelControl pnlForms;
    protected LabelControl lblFormsTitle;
    protected GridControl grcForms;
    protected GridView grvForms;
    private GridColumn colFormCode;
    private GridColumn colFormName;
    protected PanelControl pnlFields;
    protected LabelControl lblFieldsTitle;
    protected GridControl grcFields;
    protected GridView grvFields;
    private GridColumn colFieldName;
    private GridColumn colFieldKey;
    private GridColumn colControlType;
    private GridColumn colVisible;
    private GridColumn colEditable;
    private GridColumn colRequired;
    private GridColumn colReadOnly;
    protected PanelControl pnlRight;
    protected PanelControl pnlDetail;
    protected LabelControl lblDetailTitle;
    protected LabelControl lblDetailRoleCaption;
    protected LabelControl lblDetailRole;
    protected LabelControl lblDetailFormCaption;
    protected LabelControl lblDetailForm;
    protected LabelControl lblDetailKeyCaption;
    protected LabelControl lblDetailKey;
    protected LabelControl lblDetailFieldCaption;
    protected LabelControl lblDetailField;
    protected LabelControl lblDetailFieldKeyCaption;
    protected LabelControl lblDetailFieldKey;
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
        pnlRoot = new PanelControl();
        pnlFields = new PanelControl();
        grcFields = new GridControl();
        grvFields = new GridView();
        colFieldName = new GridColumn();
        colFieldKey = new GridColumn();
        colControlType = new GridColumn();
        colVisible = new GridColumn();
        colEditable = new GridColumn();
        colRequired = new GridColumn();
        colReadOnly = new GridColumn();
        lblFieldsTitle = new LabelControl();
        pnlForms = new PanelControl();
        grcForms = new GridControl();
        grvForms = new GridView();
        colFormCode = new GridColumn();
        colFormName = new GridColumn();
        lblFormsTitle = new LabelControl();
        pnlRight = new PanelControl();
        pnlAudit = new PanelControl();
        memAuditObservations = new MemoEdit();
        lblAuditObservationsCaption = new LabelControl();
        lblAuditUser = new LabelControl();
        lblAuditUserCaption = new LabelControl();
        lblAuditUpdated = new LabelControl();
        lblAuditUpdatedCaption = new LabelControl();
        lblAuditTitle = new LabelControl();
        pnlDetail = new PanelControl();
        lblDetailStatus = new LabelControl();
        lblDetailStatusCaption = new LabelControl();
        lblDetailFieldKey = new LabelControl();
        lblDetailFieldKeyCaption = new LabelControl();
        lblDetailField = new LabelControl();
        lblDetailFieldCaption = new LabelControl();
        lblDetailKey = new LabelControl();
        lblDetailKeyCaption = new LabelControl();
        lblDetailForm = new LabelControl();
        lblDetailFormCaption = new LabelControl();
        lblDetailRole = new LabelControl();
        lblDetailRoleCaption = new LabelControl();
        lblDetailTitle = new LabelControl();
        pnlRoles = new PanelControl();
        lstRoles = new ListBoxControl();
        lblRolesTitle = new LabelControl();
        pnlFilters = new PanelControl();
        chkOnlyActive = new CheckEdit();
        txtSearch = new TextEdit();
        lblSearch = new LabelControl();
        lueFormFilter = new LookUpEdit();
        lblFormFilter = new LabelControl();
        pnlHeader = new PanelControl();
        btnSave = new SimpleButton();
        lblTitle = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)pnlRoot).BeginInit();
        pnlRoot.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFields).BeginInit();
        pnlFields.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcFields).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvFields).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlForms).BeginInit();
        pnlForms.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcForms).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvForms).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlRight).BeginInit();
        pnlRight.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAudit).BeginInit();
        pnlAudit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memAuditObservations.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlDetail).BeginInit();
        pnlDetail.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlRoles).BeginInit();
        pnlRoles.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lstRoles).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).BeginInit();
        pnlFilters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)chkOnlyActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFormFilter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).BeginInit();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // pnlRoot
        // 
        pnlRoot.BorderStyle = BorderStyles.NoBorder;
        pnlRoot.Controls.Add(pnlFields);
        pnlRoot.Controls.Add(pnlForms);
        pnlRoot.Controls.Add(pnlRight);
        pnlRoot.Controls.Add(pnlRoles);
        pnlRoot.Controls.Add(pnlFilters);
        pnlRoot.Controls.Add(pnlHeader);
        pnlRoot.Dock = DockStyle.Fill;
        pnlRoot.Location = new Point(0, 0);
        pnlRoot.Name = "pnlRoot";
        pnlRoot.Size = new Size(1280, 720);
        pnlRoot.TabIndex = 0;
        // 
        // pnlFields
        // 
        pnlFields.BorderStyle = BorderStyles.Simple;
        pnlFields.Controls.Add(grcFields);
        pnlFields.Controls.Add(lblFieldsTitle);
        pnlFields.Dock = DockStyle.Fill;
        pnlFields.Location = new Point(468, 102);
        pnlFields.Name = "pnlFields";
        pnlFields.Size = new Size(512, 618);
        pnlFields.TabIndex = 5;
        // 
        // grcFields
        // 
        grcFields.Dock = DockStyle.Fill;
        grcFields.Location = new Point(2, 24);
        grcFields.MainView = grvFields;
        grcFields.Name = "grcFields";
        grcFields.Size = new Size(508, 592);
        grcFields.TabIndex = 1;
        grcFields.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvFields });
        // 
        // grvFields
        // 
        grvFields.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvFields.Appearance.HeaderPanel.Options.UseFont = true;
        grvFields.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvFields.Appearance.Row.Options.UseFont = true;
        grvFields.Columns.AddRange(new GridColumn[] { colFieldName, colFieldKey, colControlType, colVisible, colEditable, colRequired, colReadOnly });
        grvFields.GridControl = grcFields;
        grvFields.Name = "grvFields";
        grvFields.OptionsView.ShowGroupPanel = false;
        // 
        // colFieldName
        // 
        colFieldName.Caption = "Etiqueta visible";
        colFieldName.FieldName = "FieldName";
        colFieldName.Name = "colFieldName";
        colFieldName.Visible = true;
        colFieldName.VisibleIndex = 0;
        colFieldName.Width = 120;
        // 
        // colFieldKey
        // 
        colFieldKey.Caption = "Campo";
        colFieldKey.FieldName = "FieldKey";
        colFieldKey.Name = "colFieldKey";
        colFieldKey.Visible = true;
        colFieldKey.VisibleIndex = 1;
        colFieldKey.Width = 110;
        // 
        // colControlType
        // 
        colControlType.Caption = "Control";
        colControlType.FieldName = "ControlType";
        colControlType.Name = "colControlType";
        colControlType.Visible = true;
        colControlType.VisibleIndex = 2;
        colControlType.Width = 90;
        // 
        // colVisible
        // 
        colVisible.Caption = "Visible";
        colVisible.FieldName = "IsVisible";
        colVisible.Name = "colVisible";
        colVisible.Visible = true;
        colVisible.VisibleIndex = 3;
        colVisible.Width = 60;
        // 
        // colEditable
        // 
        colEditable.Caption = "Editable";
        colEditable.FieldName = "IsEditable";
        colEditable.Name = "colEditable";
        colEditable.Visible = true;
        colEditable.VisibleIndex = 4;
        colEditable.Width = 65;
        // 
        // colRequired
        // 
        colRequired.Caption = "Requerido";
        colRequired.FieldName = "IsRequired";
        colRequired.Name = "colRequired";
        colRequired.Visible = true;
        colRequired.VisibleIndex = 5;
        // 
        // colReadOnly
        // 
        colReadOnly.Caption = "Solo lectura";
        colReadOnly.FieldName = "IsReadOnly";
        colReadOnly.Name = "colReadOnly";
        colReadOnly.Visible = true;
        colReadOnly.VisibleIndex = 6;
        colReadOnly.Width = 80;
        // 
        // lblFieldsTitle
        // 
        lblFieldsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblFieldsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblFieldsTitle.Appearance.Options.UseFont = true;
        lblFieldsTitle.Appearance.Options.UseForeColor = true;
        lblFieldsTitle.Dock = DockStyle.Top;
        lblFieldsTitle.Location = new Point(2, 2);
        lblFieldsTitle.Name = "lblFieldsTitle";
        lblFieldsTitle.Padding = new Padding(8, 7, 0, 0);
        lblFieldsTitle.Size = new Size(126, 22);
        lblFieldsTitle.TabIndex = 0;
        lblFieldsTitle.Text = "Campos configurables";
        // 
        // pnlForms
        // 
        pnlForms.BorderStyle = BorderStyles.Simple;
        pnlForms.Controls.Add(grcForms);
        pnlForms.Controls.Add(lblFormsTitle);
        pnlForms.Dock = DockStyle.Left;
        pnlForms.Location = new Point(218, 102);
        pnlForms.Name = "pnlForms";
        pnlForms.Size = new Size(250, 618);
        pnlForms.TabIndex = 4;
        // 
        // grcForms
        // 
        grcForms.Dock = DockStyle.Fill;
        grcForms.Location = new Point(2, 24);
        grcForms.MainView = grvForms;
        grcForms.Name = "grcForms";
        grcForms.Size = new Size(246, 592);
        grcForms.TabIndex = 1;
        grcForms.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvForms });
        // 
        // grvForms
        // 
        grvForms.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvForms.Appearance.HeaderPanel.Options.UseFont = true;
        grvForms.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvForms.Appearance.Row.Options.UseFont = true;
        grvForms.Columns.AddRange(new GridColumn[] { colFormCode, colFormName });
        grvForms.GridControl = grcForms;
        grvForms.Name = "grvForms";
        grvForms.OptionsBehavior.Editable = false;
        grvForms.OptionsView.ShowGroupPanel = false;
        // 
        // colFormCode
        // 
        colFormCode.Caption = "Codigo";
        colFormCode.FieldName = "Code";
        colFormCode.Name = "colFormCode";
        colFormCode.Visible = true;
        colFormCode.VisibleIndex = 0;
        colFormCode.Width = 85;
        // 
        // colFormName
        // 
        colFormName.Caption = "Nombre";
        colFormName.FieldName = "Name";
        colFormName.Name = "colFormName";
        colFormName.Visible = true;
        colFormName.VisibleIndex = 1;
        colFormName.Width = 140;
        // 
        // lblFormsTitle
        // 
        lblFormsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblFormsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblFormsTitle.Appearance.Options.UseFont = true;
        lblFormsTitle.Appearance.Options.UseForeColor = true;
        lblFormsTitle.Dock = DockStyle.Top;
        lblFormsTitle.Location = new Point(2, 2);
        lblFormsTitle.Name = "lblFormsTitle";
        lblFormsTitle.Padding = new Padding(8, 7, 0, 0);
        lblFormsTitle.Size = new Size(129, 22);
        lblFormsTitle.TabIndex = 0;
        lblFormsTitle.Text = "Formularios / Modulos";
        // 
        // pnlRight
        // 
        pnlRight.BorderStyle = BorderStyles.NoBorder;
        pnlRight.Controls.Add(pnlAudit);
        pnlRight.Controls.Add(pnlDetail);
        pnlRight.Dock = DockStyle.Right;
        pnlRight.Location = new Point(980, 102);
        pnlRight.Name = "pnlRight";
        pnlRight.Padding = new Padding(8, 0, 0, 0);
        pnlRight.Size = new Size(300, 618);
        pnlRight.TabIndex = 3;
        // 
        // pnlAudit
        // 
        pnlAudit.BorderStyle = BorderStyles.Simple;
        pnlAudit.Controls.Add(memAuditObservations);
        pnlAudit.Controls.Add(lblAuditObservationsCaption);
        pnlAudit.Controls.Add(lblAuditUser);
        pnlAudit.Controls.Add(lblAuditUserCaption);
        pnlAudit.Controls.Add(lblAuditUpdated);
        pnlAudit.Controls.Add(lblAuditUpdatedCaption);
        pnlAudit.Controls.Add(lblAuditTitle);
        pnlAudit.Dock = DockStyle.Fill;
        pnlAudit.Location = new Point(8, 250);
        pnlAudit.Name = "pnlAudit";
        pnlAudit.Size = new Size(292, 368);
        pnlAudit.TabIndex = 1;
        // 
        // memAuditObservations
        // 
        memAuditObservations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        memAuditObservations.Location = new Point(12, 126);
        memAuditObservations.Name = "memAuditObservations";
        memAuditObservations.Properties.NullValuePrompt = "Ingrese observaciones (opcional)...";
        memAuditObservations.Size = new Size(266, 224);
        memAuditObservations.TabIndex = 6;
        // 
        // lblAuditObservationsCaption
        // 
        lblAuditObservationsCaption.Location = new Point(12, 102);
        lblAuditObservationsCaption.Name = "lblAuditObservationsCaption";
        lblAuditObservationsCaption.Size = new Size(75, 13);
        lblAuditObservationsCaption.TabIndex = 5;
        lblAuditObservationsCaption.Text = "Observaciones:";
        // 
        // lblAuditUser
        // 
        lblAuditUser.Location = new Point(130, 72);
        lblAuditUser.Name = "lblAuditUser";
        lblAuditUser.Size = new Size(4, 13);
        lblAuditUser.TabIndex = 4;
        lblAuditUser.Text = "-";
        // 
        // lblAuditUserCaption
        // 
        lblAuditUserCaption.Location = new Point(12, 72);
        lblAuditUserCaption.Name = "lblAuditUserCaption";
        lblAuditUserCaption.Size = new Size(40, 13);
        lblAuditUserCaption.TabIndex = 3;
        lblAuditUserCaption.Text = "Usuario:";
        // 
        // lblAuditUpdated
        // 
        lblAuditUpdated.Location = new Point(130, 45);
        lblAuditUpdated.Name = "lblAuditUpdated";
        lblAuditUpdated.Size = new Size(4, 13);
        lblAuditUpdated.TabIndex = 2;
        lblAuditUpdated.Text = "-";
        // 
        // lblAuditUpdatedCaption
        // 
        lblAuditUpdatedCaption.Location = new Point(12, 45);
        lblAuditUpdatedCaption.Name = "lblAuditUpdatedCaption";
        lblAuditUpdatedCaption.Size = new Size(94, 13);
        lblAuditUpdatedCaption.TabIndex = 1;
        lblAuditUpdatedCaption.Text = "Ultima modificacion:";
        // 
        // lblAuditTitle
        // 
        lblAuditTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAuditTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAuditTitle.Appearance.Options.UseFont = true;
        lblAuditTitle.Appearance.Options.UseForeColor = true;
        lblAuditTitle.Location = new Point(12, 12);
        lblAuditTitle.Name = "lblAuditTitle";
        lblAuditTitle.Size = new Size(220, 15);
        lblAuditTitle.TabIndex = 0;
        lblAuditTitle.Text = "Informacion de auditoria / Observaciones";
        // 
        // pnlDetail
        // 
        pnlDetail.BorderStyle = BorderStyles.Simple;
        pnlDetail.Controls.Add(lblDetailStatus);
        pnlDetail.Controls.Add(lblDetailStatusCaption);
        pnlDetail.Controls.Add(lblDetailFieldKey);
        pnlDetail.Controls.Add(lblDetailFieldKeyCaption);
        pnlDetail.Controls.Add(lblDetailField);
        pnlDetail.Controls.Add(lblDetailFieldCaption);
        pnlDetail.Controls.Add(lblDetailKey);
        pnlDetail.Controls.Add(lblDetailKeyCaption);
        pnlDetail.Controls.Add(lblDetailForm);
        pnlDetail.Controls.Add(lblDetailFormCaption);
        pnlDetail.Controls.Add(lblDetailRole);
        pnlDetail.Controls.Add(lblDetailRoleCaption);
        pnlDetail.Controls.Add(lblDetailTitle);
        pnlDetail.Dock = DockStyle.Top;
        pnlDetail.Location = new Point(8, 0);
        pnlDetail.Name = "pnlDetail";
        pnlDetail.Size = new Size(292, 250);
        pnlDetail.TabIndex = 0;
        // 
        // lblDetailStatus
        // 
        lblDetailStatus.Location = new Point(112, 188);
        lblDetailStatus.Name = "lblDetailStatus";
        lblDetailStatus.Size = new Size(4, 13);
        lblDetailStatus.TabIndex = 12;
        lblDetailStatus.Text = "-";
        // 
        // lblDetailStatusCaption
        // 
        lblDetailStatusCaption.Location = new Point(12, 188);
        lblDetailStatusCaption.Name = "lblDetailStatusCaption";
        lblDetailStatusCaption.Size = new Size(37, 13);
        lblDetailStatusCaption.TabIndex = 11;
        lblDetailStatusCaption.Text = "Estado:";
        // 
        // lblDetailFieldKey
        // 
        lblDetailFieldKey.Location = new Point(112, 158);
        lblDetailFieldKey.Name = "lblDetailFieldKey";
        lblDetailFieldKey.Size = new Size(4, 13);
        lblDetailFieldKey.TabIndex = 10;
        lblDetailFieldKey.Text = "-";
        // 
        // lblDetailFieldKeyCaption
        // 
        lblDetailFieldKeyCaption.Location = new Point(12, 158);
        lblDetailFieldKeyCaption.Name = "lblDetailFieldKeyCaption";
        lblDetailFieldKeyCaption.Size = new Size(44, 13);
        lblDetailFieldKeyCaption.TabIndex = 9;
        lblDetailFieldKeyCaption.Text = "FieldKey:";
        // 
        // lblDetailField
        // 
        lblDetailField.Location = new Point(112, 128);
        lblDetailField.Name = "lblDetailField";
        lblDetailField.Size = new Size(4, 13);
        lblDetailField.TabIndex = 8;
        lblDetailField.Text = "-";
        // 
        // lblDetailFieldCaption
        // 
        lblDetailFieldCaption.Location = new Point(12, 128);
        lblDetailFieldCaption.Name = "lblDetailFieldCaption";
        lblDetailFieldCaption.Size = new Size(37, 13);
        lblDetailFieldCaption.TabIndex = 7;
        lblDetailFieldCaption.Text = "Campo:";
        // 
        // lblDetailKey
        // 
        lblDetailKey.Location = new Point(112, 98);
        lblDetailKey.Name = "lblDetailKey";
        lblDetailKey.Size = new Size(4, 13);
        lblDetailKey.TabIndex = 6;
        lblDetailKey.Text = "-";
        // 
        // lblDetailKeyCaption
        // 
        lblDetailKeyCaption.Location = new Point(12, 98);
        lblDetailKeyCaption.Name = "lblDetailKeyCaption";
        lblDetailKeyCaption.Size = new Size(83, 13);
        lblDetailKeyCaption.TabIndex = 5;
        lblDetailKeyCaption.Text = "Codigo/FormKey:";
        // 
        // lblDetailForm
        // 
        lblDetailForm.Location = new Point(112, 68);
        lblDetailForm.Name = "lblDetailForm";
        lblDetailForm.Size = new Size(4, 13);
        lblDetailForm.TabIndex = 4;
        lblDetailForm.Text = "-";
        // 
        // lblDetailFormCaption
        // 
        lblDetailFormCaption.Location = new Point(12, 68);
        lblDetailFormCaption.Name = "lblDetailFormCaption";
        lblDetailFormCaption.Size = new Size(54, 13);
        lblDetailFormCaption.TabIndex = 3;
        lblDetailFormCaption.Text = "Formulario:";
        // 
        // lblDetailRole
        // 
        lblDetailRole.Location = new Point(112, 38);
        lblDetailRole.Name = "lblDetailRole";
        lblDetailRole.Size = new Size(4, 13);
        lblDetailRole.TabIndex = 2;
        lblDetailRole.Text = "-";
        // 
        // lblDetailRoleCaption
        // 
        lblDetailRoleCaption.Location = new Point(12, 38);
        lblDetailRoleCaption.Name = "lblDetailRoleCaption";
        lblDetailRoleCaption.Size = new Size(19, 13);
        lblDetailRoleCaption.TabIndex = 1;
        lblDetailRoleCaption.Text = "Rol:";
        // 
        // lblDetailTitle
        // 
        lblDetailTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblDetailTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblDetailTitle.Appearance.Options.UseFont = true;
        lblDetailTitle.Appearance.Options.UseForeColor = true;
        lblDetailTitle.Location = new Point(12, 12);
        lblDetailTitle.Name = "lblDetailTitle";
        lblDetailTitle.Size = new Size(167, 15);
        lblDetailTitle.TabIndex = 0;
        lblDetailTitle.Text = "Detalle del acceso seleccionado";
        // 
        // pnlRoles
        // 
        pnlRoles.BorderStyle = BorderStyles.Simple;
        pnlRoles.Controls.Add(lstRoles);
        pnlRoles.Controls.Add(lblRolesTitle);
        pnlRoles.Dock = DockStyle.Left;
        pnlRoles.Location = new Point(0, 102);
        pnlRoles.Name = "pnlRoles";
        pnlRoles.Size = new Size(218, 618);
        pnlRoles.TabIndex = 2;
        // 
        // lstRoles
        // 
        lstRoles.Dock = DockStyle.Fill;
        lstRoles.Location = new Point(2, 24);
        lstRoles.Name = "lstRoles";
        lstRoles.Size = new Size(214, 592);
        lstRoles.TabIndex = 1;
        // 
        // lblRolesTitle
        // 
        lblRolesTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblRolesTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblRolesTitle.Appearance.Options.UseFont = true;
        lblRolesTitle.Appearance.Options.UseForeColor = true;
        lblRolesTitle.Dock = DockStyle.Top;
        lblRolesTitle.Location = new Point(2, 2);
        lblRolesTitle.Name = "lblRolesTitle";
        lblRolesTitle.Padding = new Padding(8, 7, 0, 0);
        lblRolesTitle.Size = new Size(43, 22);
        lblRolesTitle.TabIndex = 0;
        lblRolesTitle.Text = "ROLES";
        // 
        // pnlFilters
        // 
        pnlFilters.BorderStyle = BorderStyles.NoBorder;
        pnlFilters.Controls.Add(chkOnlyActive);
        pnlFilters.Controls.Add(txtSearch);
        pnlFilters.Controls.Add(lblSearch);
        pnlFilters.Controls.Add(lueFormFilter);
        pnlFilters.Controls.Add(lblFormFilter);
        pnlFilters.Dock = DockStyle.Top;
        pnlFilters.Location = new Point(0, 54);
        pnlFilters.Name = "pnlFilters";
        pnlFilters.Size = new Size(1280, 48);
        pnlFilters.TabIndex = 1;
        // 
        // chkOnlyActive
        // 
        chkOnlyActive.EditValue = true;
        chkOnlyActive.Location = new Point(952, 14);
        chkOnlyActive.Name = "chkOnlyActive";
        chkOnlyActive.Properties.Caption = "Mostrar solo activos";
        chkOnlyActive.Size = new Size(150, 20);
        chkOnlyActive.TabIndex = 4;
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(660, 12);
        txtSearch.Name = "txtSearch";
        txtSearch.Properties.NullValuePrompt = "Buscar campo o etiqueta...";
        txtSearch.Size = new Size(260, 20);
        txtSearch.TabIndex = 3;
        // 
        // lblSearch
        // 
        lblSearch.Location = new Point(610, 15);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(36, 13);
        lblSearch.TabIndex = 2;
        lblSearch.Text = "Buscar:";
        // 
        // lueFormFilter
        // 
        lueFormFilter.Location = new Point(350, 12);
        lueFormFilter.Name = "lueFormFilter";
        lueFormFilter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFormFilter.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Codigo", 90, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 160, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        lueFormFilter.Properties.DisplayMember = "Name";
        lueFormFilter.Properties.NullText = "";
        lueFormFilter.Properties.ValueMember = "Id";
        lueFormFilter.Size = new Size(230, 20);
        lueFormFilter.TabIndex = 1;
        // 
        // lblFormFilter
        // 
        lblFormFilter.Location = new Point(238, 15);
        lblFormFilter.Name = "lblFormFilter";
        lblFormFilter.Size = new Size(92, 13);
        lblFormFilter.TabIndex = 0;
        lblFormFilter.Text = "Formulario/Modulo:";
        // 
        // pnlHeader
        // 
        pnlHeader.BorderStyle = BorderStyles.NoBorder;
        pnlHeader.Controls.Add(btnSave);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1280, 54);
        pnlHeader.TabIndex = 0;
        // 
        // btnSave
        // 
        btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.Location = new Point(1128, 10);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(134, 34);
        btnSave.TabIndex = 1;
        btnSave.Text = "Guardar cambios";
        // 
        // lblTitle
        // 
        lblTitle.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTitle.Appearance.Options.UseFont = true;
        lblTitle.Appearance.Options.UseForeColor = true;
        lblTitle.Location = new Point(18, 17);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(454, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Accesos a Campos de Formularios de Mantenimiento";
        // 
        // SecurityMaintenanceFieldAccessForm
        // 
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1280, 720);
        Controls.Add(pnlRoot);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1100, 650);
        Name = "SecurityMaintenanceFieldAccessForm";
        Text = "Accesos a Campos de Formularios de Mantenimiento";
        ((System.ComponentModel.ISupportInitialize)pnlRoot).EndInit();
        pnlRoot.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlFields).EndInit();
        pnlFields.ResumeLayout(false);
        pnlFields.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grcFields).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvFields).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlForms).EndInit();
        pnlForms.ResumeLayout(false);
        pnlForms.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grcForms).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvForms).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlRight).EndInit();
        pnlRight.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAudit).EndInit();
        pnlAudit.ResumeLayout(false);
        pnlAudit.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memAuditObservations.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlDetail).EndInit();
        pnlDetail.ResumeLayout(false);
        pnlDetail.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlRoles).EndInit();
        pnlRoles.ResumeLayout(false);
        pnlRoles.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lstRoles).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).EndInit();
        pnlFilters.ResumeLayout(false);
        pnlFilters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)chkOnlyActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFormFilter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).EndInit();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
    }
}
