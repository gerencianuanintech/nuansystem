using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.SecurityAccess;

partial class SecurityTransactionalFieldAccessForm
{
    private System.ComponentModel.IContainer components = null;
    private PanelControl pnlRoot;
    private PanelControl pnlHeader;
    private LabelControl lblTitle;
    private SimpleButton btnSave;
    private PanelControl pnlFilters;
    private LabelControl lblFormFilter;
    private LookUpEdit lueFormFilter;
    private LabelControl lblDocumentType;
    private LookUpEdit lueDocumentType;
    private LabelControl lblSeriesFilter;
    private LookUpEdit lueSeriesFilter;
    private LabelControl lblSearch;
    private TextEdit txtSearch;
    private CheckEdit chkOnlyActive;
    private PanelControl pnlLeft;
    private PanelControl pnlRoles;
    private LabelControl lblRolesTitle;
    private ListBoxControl lstRoles;
    private PanelControl pnlForms;
    private LabelControl lblFormsTitle;
    private GridControl grcForms;
    private GridView grvForms;
    private GridColumn colFormName;
    private PanelControl pnlRight;
    private PanelControl pnlDetail;
    private LabelControl lblDetailTitle;
    private LabelControl lblDetailRoleCaption;
    private LabelControl lblDetailRole;
    private LabelControl lblDetailFormCaption;
    private LabelControl lblDetailForm;
    private LabelControl lblDetailDocumentTypeCaption;
    private LabelControl lblDetailDocumentType;
    private LabelControl lblDetailSeriesCaption;
    private LabelControl lblDetailSeries;
    private LabelControl lblDetailCodeCaption;
    private LabelControl lblDetailCode;
    private LabelControl lblDetailEstablishmentCaption;
    private LabelControl lblDetailEstablishment;
    private LabelControl lblDetailEmissionPointCaption;
    private LabelControl lblDetailEmissionPoint;
    private LabelControl lblDetailStatusCaption;
    private LabelControl lblDetailStatus;
    private PanelControl pnlAudit;
    private LabelControl lblAuditTitle;
    private LabelControl lblAuditUpdatedCaption;
    private LabelControl lblAuditUpdated;
    private LabelControl lblAuditUserCaption;
    private LabelControl lblAuditUser;
    private LabelControl lblAuditEquipmentCaption;
    private LabelControl lblAuditEquipment;
    private LabelControl lblAuditIpCaption;
    private LabelControl lblAuditIp;
    private LabelControl lblAuditObservationsCaption;
    private MemoEdit memAuditObservations;
    private PanelControl pnlCenter;
    private PanelControl pnlSeries;
    private LabelControl lblSeriesTitle;
    private GridControl grcSeries;
    private GridView grvSeries;
    private GridColumn colSeriesCode;
    private GridColumn colSeriesName;
    private GridColumn colSeriesDocumentType;
    private GridColumn colSeriesPrefix;
    private GridColumn colSeriesEstablishment;
    private GridColumn colSeriesEmissionPoint;
    private GridColumn colSeriesActive;
    private GridColumn colSeriesSelected;
    private PanelControl pnlOperations;
    private LabelControl lblOperationsTitle;
    private GridControl grcOperations;
    private GridView grvOperations;
    private GridColumn colFieldSection;
    private GridColumn colFieldKey;
    private GridColumn colFieldName;
    private GridColumn colFieldControl;
    private GridColumn colFieldVisible;
    private GridColumn colFieldEditable;
    private GridColumn colFieldRequired;
    private GridColumn colFieldReadOnly;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlRoot = new PanelControl();
        pnlCenter = new PanelControl();
        pnlOperations = new PanelControl();
        lblOperationsTitle = new LabelControl();
        grcOperations = new GridControl();
        grvOperations = new GridView();
        colFieldSection = new GridColumn();
        colFieldKey = new GridColumn();
        colFieldName = new GridColumn();
        colFieldControl = new GridColumn();
        colFieldVisible = new GridColumn();
        colFieldEditable = new GridColumn();
        colFieldRequired = new GridColumn();
        colFieldReadOnly = new GridColumn();
        pnlSeries = new PanelControl();
        lblSeriesTitle = new LabelControl();
        grcSeries = new GridControl();
        grvSeries = new GridView();
        colSeriesCode = new GridColumn();
        colSeriesName = new GridColumn();
        colSeriesDocumentType = new GridColumn();
        colSeriesPrefix = new GridColumn();
        colSeriesEstablishment = new GridColumn();
        colSeriesEmissionPoint = new GridColumn();
        colSeriesActive = new GridColumn();
        colSeriesSelected = new GridColumn();
        pnlRight = new PanelControl();
        pnlAudit = new PanelControl();
        lblAuditTitle = new LabelControl();
        lblAuditUpdatedCaption = new LabelControl();
        lblAuditUpdated = new LabelControl();
        lblAuditUserCaption = new LabelControl();
        lblAuditUser = new LabelControl();
        lblAuditEquipmentCaption = new LabelControl();
        lblAuditEquipment = new LabelControl();
        lblAuditIpCaption = new LabelControl();
        lblAuditIp = new LabelControl();
        lblAuditObservationsCaption = new LabelControl();
        memAuditObservations = new MemoEdit();
        pnlDetail = new PanelControl();
        lblDetailTitle = new LabelControl();
        lblDetailRoleCaption = new LabelControl();
        lblDetailRole = new LabelControl();
        lblDetailFormCaption = new LabelControl();
        lblDetailForm = new LabelControl();
        lblDetailDocumentTypeCaption = new LabelControl();
        lblDetailDocumentType = new LabelControl();
        lblDetailSeriesCaption = new LabelControl();
        lblDetailSeries = new LabelControl();
        lblDetailCodeCaption = new LabelControl();
        lblDetailCode = new LabelControl();
        lblDetailEstablishmentCaption = new LabelControl();
        lblDetailEstablishment = new LabelControl();
        lblDetailEmissionPointCaption = new LabelControl();
        lblDetailEmissionPoint = new LabelControl();
        lblDetailStatusCaption = new LabelControl();
        lblDetailStatus = new LabelControl();
        pnlLeft = new PanelControl();
        pnlForms = new PanelControl();
        lblFormsTitle = new LabelControl();
        grcForms = new GridControl();
        grvForms = new GridView();
        colFormName = new GridColumn();
        pnlRoles = new PanelControl();
        lblRolesTitle = new LabelControl();
        lstRoles = new ListBoxControl();
        pnlFilters = new PanelControl();
        lblFormFilter = new LabelControl();
        lueFormFilter = new LookUpEdit();
        lblDocumentType = new LabelControl();
        lueDocumentType = new LookUpEdit();
        lblSeriesFilter = new LabelControl();
        lueSeriesFilter = new LookUpEdit();
        lblSearch = new LabelControl();
        txtSearch = new TextEdit();
        chkOnlyActive = new CheckEdit();
        pnlHeader = new PanelControl();
        lblTitle = new LabelControl();
        btnSave = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)pnlRoot).BeginInit();
        pnlRoot.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlCenter).BeginInit();
        pnlCenter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlOperations).BeginInit();
        pnlOperations.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcOperations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvOperations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlSeries).BeginInit();
        pnlSeries.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcSeries).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSeries).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlRight).BeginInit();
        pnlRight.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAudit).BeginInit();
        pnlAudit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memAuditObservations.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlDetail).BeginInit();
        pnlDetail.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlLeft).BeginInit();
        pnlLeft.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlForms).BeginInit();
        pnlForms.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcForms).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvForms).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlRoles).BeginInit();
        pnlRoles.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lstRoles).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).BeginInit();
        pnlFilters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueFormFilter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSeriesFilter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkOnlyActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).BeginInit();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // pnlRoot
        // 
        pnlRoot.BorderStyle = BorderStyles.NoBorder;
        pnlRoot.Controls.Add(pnlCenter);
        pnlRoot.Controls.Add(pnlRight);
        pnlRoot.Controls.Add(pnlLeft);
        pnlRoot.Controls.Add(pnlFilters);
        pnlRoot.Controls.Add(pnlHeader);
        pnlRoot.Dock = DockStyle.Fill;
        pnlRoot.Location = new Point(0, 0);
        pnlRoot.Name = "pnlRoot";
        pnlRoot.Size = new Size(1360, 760);
        pnlRoot.TabIndex = 0;
        // 
        // pnlCenter
        // 
        pnlCenter.BorderStyle = BorderStyles.NoBorder;
        pnlCenter.Controls.Add(pnlOperations);
        pnlCenter.Controls.Add(pnlSeries);
        pnlCenter.Dock = DockStyle.Fill;
        pnlCenter.Location = new Point(500, 102);
        pnlCenter.Name = "pnlCenter";
        pnlCenter.Size = new Size(550, 658);
        pnlCenter.TabIndex = 3;
        // 
        // pnlOperations
        // 
        pnlOperations.Controls.Add(lblOperationsTitle);
        pnlOperations.Controls.Add(grcOperations);
        pnlOperations.Dock = DockStyle.Fill;
        pnlOperations.Location = new Point(0, 270);
        pnlOperations.Name = "pnlOperations";
        pnlOperations.Size = new Size(550, 388);
        pnlOperations.TabIndex = 1;
        // 
        // lblOperationsTitle
        // 
        lblOperationsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblOperationsTitle.Appearance.ForeColor = BrandResources.Text;
        lblOperationsTitle.Appearance.Options.UseFont = true;
        lblOperationsTitle.Appearance.Options.UseForeColor = true;
        lblOperationsTitle.Location = new Point(10, 9);
        lblOperationsTitle.Name = "lblOperationsTitle";
        lblOperationsTitle.Size = new Size(352, 15);
        lblOperationsTitle.TabIndex = 0;
        lblOperationsTitle.Text = "Campos configurables para el formulario/serie seleccionada";
        // 
        // grcOperations
        // 
        grcOperations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grcOperations.Location = new Point(8, 30);
        grcOperations.MainView = grvOperations;
        grcOperations.Name = "grcOperations";
        grcOperations.Size = new Size(534, 350);
        grcOperations.TabIndex = 1;
        grcOperations.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvOperations });
        // 
        // grvOperations
        // 
        grvOperations.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvOperations.Appearance.HeaderPanel.Options.UseFont = true;
        grvOperations.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvOperations.Appearance.Row.Options.UseFont = true;
        grvOperations.Columns.AddRange(new GridColumn[] { colFieldSection, colFieldKey, colFieldName, colFieldControl, colFieldVisible, colFieldEditable, colFieldRequired, colFieldReadOnly });
        grvOperations.GridControl = grcOperations;
        grvOperations.Name = "grvOperations";
        grvOperations.OptionsView.ShowGroupPanel = false;
        grvOperations.OptionsView.ShowIndicator = false;
        // 
        // colFieldSection
        // 
        colFieldSection.Caption = "Sección";
        colFieldSection.FieldName = "Description";
        colFieldSection.Name = "colFieldSection";
        colFieldSection.OptionsColumn.AllowEdit = false;
        colFieldSection.Visible = true;
        colFieldSection.VisibleIndex = 0;
        colFieldSection.Width = 80;
        // 
        // colFieldKey
        // 
        colFieldKey.Caption = "Campo";
        colFieldKey.FieldName = "FieldKey";
        colFieldKey.Name = "colFieldKey";
        colFieldKey.OptionsColumn.AllowEdit = false;
        colFieldKey.Visible = true;
        colFieldKey.VisibleIndex = 1;
        colFieldKey.Width = 120;
        // 
        // colFieldName
        // 
        colFieldName.Caption = "Etiqueta visible";
        colFieldName.FieldName = "FieldName";
        colFieldName.Name = "colFieldName";
        colFieldName.OptionsColumn.AllowEdit = false;
        colFieldName.Visible = true;
        colFieldName.VisibleIndex = 2;
        colFieldName.Width = 135;
        // 
        // colFieldControl
        // 
        colFieldControl.Caption = "Control";
        colFieldControl.FieldName = "ControlType";
        colFieldControl.Name = "colFieldControl";
        colFieldControl.OptionsColumn.AllowEdit = false;
        colFieldControl.Visible = true;
        colFieldControl.VisibleIndex = 3;
        colFieldControl.Width = 110;
        // 
        // colFieldVisible
        // 
        colFieldVisible.Caption = "Visible";
        colFieldVisible.FieldName = "IsVisible";
        colFieldVisible.Name = "colFieldVisible";
        colFieldVisible.Visible = true;
        colFieldVisible.VisibleIndex = 4;
        colFieldVisible.Width = 60;
        // 
        // colFieldEditable
        // 
        colFieldEditable.Caption = "Editable";
        colFieldEditable.FieldName = "IsEditable";
        colFieldEditable.Name = "colFieldEditable";
        colFieldEditable.Visible = true;
        colFieldEditable.VisibleIndex = 5;
        colFieldEditable.Width = 65;
        // 
        // colFieldRequired
        // 
        colFieldRequired.Caption = "Requerido";
        colFieldRequired.FieldName = "IsRequired";
        colFieldRequired.Name = "colFieldRequired";
        colFieldRequired.Visible = true;
        colFieldRequired.VisibleIndex = 6;
        colFieldRequired.Width = 75;
        // 
        // colFieldReadOnly
        // 
        colFieldReadOnly.Caption = "Solo lectura";
        colFieldReadOnly.FieldName = "IsReadOnly";
        colFieldReadOnly.Name = "colFieldReadOnly";
        colFieldReadOnly.Visible = true;
        colFieldReadOnly.VisibleIndex = 7;
        colFieldReadOnly.Width = 85;
        // 
        // pnlSeries
        // 
        pnlSeries.Controls.Add(lblSeriesTitle);
        pnlSeries.Controls.Add(grcSeries);
        pnlSeries.Dock = DockStyle.Top;
        pnlSeries.Location = new Point(0, 0);
        pnlSeries.Name = "pnlSeries";
        pnlSeries.Size = new Size(550, 270);
        pnlSeries.TabIndex = 0;
        // 
        // lblSeriesTitle
        // 
        lblSeriesTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblSeriesTitle.Appearance.ForeColor = BrandResources.Text;
        lblSeriesTitle.Appearance.Options.UseFont = true;
        lblSeriesTitle.Appearance.Options.UseForeColor = true;
        lblSeriesTitle.Location = new Point(10, 9);
        lblSeriesTitle.Name = "lblSeriesTitle";
        lblSeriesTitle.Size = new Size(196, 15);
        lblSeriesTitle.TabIndex = 0;
        lblSeriesTitle.Text = "DOCUMENTOS / SERIES DISPONIBLES";
        // 
        // grcSeries
        // 
        grcSeries.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grcSeries.Location = new Point(8, 30);
        grcSeries.MainView = grvSeries;
        grcSeries.Name = "grcSeries";
        grcSeries.Size = new Size(534, 232);
        grcSeries.TabIndex = 1;
        grcSeries.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSeries });
        // 
        // grvSeries
        // 
        grvSeries.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvSeries.Appearance.HeaderPanel.Options.UseFont = true;
        grvSeries.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvSeries.Appearance.Row.Options.UseFont = true;
        grvSeries.Columns.AddRange(new GridColumn[] { colSeriesCode, colSeriesName, colSeriesDocumentType, colSeriesPrefix, colSeriesEstablishment, colSeriesEmissionPoint, colSeriesActive, colSeriesSelected });
        grvSeries.GridControl = grcSeries;
        grvSeries.Name = "grvSeries";
        grvSeries.OptionsView.ShowGroupPanel = false;
        grvSeries.OptionsView.ShowIndicator = false;
        // 
        // colSeriesCode
        // 
        colSeriesCode.Caption = "Código";
        colSeriesCode.FieldName = "Code";
        colSeriesCode.Name = "colSeriesCode";
        colSeriesCode.OptionsColumn.AllowEdit = false;
        colSeriesCode.Visible = true;
        colSeriesCode.VisibleIndex = 0;
        colSeriesCode.Width = 70;
        // 
        // colSeriesName
        // 
        colSeriesName.Caption = "Nombre";
        colSeriesName.FieldName = "Name";
        colSeriesName.Name = "colSeriesName";
        colSeriesName.OptionsColumn.AllowEdit = false;
        colSeriesName.Visible = true;
        colSeriesName.VisibleIndex = 1;
        colSeriesName.Width = 155;
        // 
        // colSeriesDocumentType
        // 
        colSeriesDocumentType.Caption = "Tipo Documento";
        colSeriesDocumentType.FieldName = "DocumentTypeName";
        colSeriesDocumentType.Name = "colSeriesDocumentType";
        colSeriesDocumentType.OptionsColumn.AllowEdit = false;
        colSeriesDocumentType.Visible = true;
        colSeriesDocumentType.VisibleIndex = 2;
        colSeriesDocumentType.Width = 125;
        // 
        // colSeriesPrefix
        // 
        colSeriesPrefix.Caption = "Prefijo";
        colSeriesPrefix.FieldName = "Prefix";
        colSeriesPrefix.Name = "colSeriesPrefix";
        colSeriesPrefix.OptionsColumn.AllowEdit = false;
        colSeriesPrefix.Visible = true;
        colSeriesPrefix.VisibleIndex = 3;
        colSeriesPrefix.Width = 55;
        // 
        // colSeriesEstablishment
        // 
        colSeriesEstablishment.Caption = "Establecimiento";
        colSeriesEstablishment.FieldName = "Establishment";
        colSeriesEstablishment.Name = "colSeriesEstablishment";
        colSeriesEstablishment.OptionsColumn.AllowEdit = false;
        colSeriesEstablishment.Visible = true;
        colSeriesEstablishment.VisibleIndex = 4;
        colSeriesEstablishment.Width = 100;
        // 
        // colSeriesEmissionPoint
        // 
        colSeriesEmissionPoint.Caption = "Pto. Emisión";
        colSeriesEmissionPoint.FieldName = "EmissionPoint";
        colSeriesEmissionPoint.Name = "colSeriesEmissionPoint";
        colSeriesEmissionPoint.OptionsColumn.AllowEdit = false;
        colSeriesEmissionPoint.Visible = true;
        colSeriesEmissionPoint.VisibleIndex = 5;
        colSeriesEmissionPoint.Width = 85;
        // 
        // colSeriesActive
        // 
        colSeriesActive.Caption = "Activo";
        colSeriesActive.FieldName = "IsActive";
        colSeriesActive.Name = "colSeriesActive";
        colSeriesActive.OptionsColumn.AllowEdit = false;
        colSeriesActive.Visible = true;
        colSeriesActive.VisibleIndex = 6;
        colSeriesActive.Width = 55;
        // 
        // colSeriesSelected
        // 
        colSeriesSelected.Caption = "Seleccionar";
        colSeriesSelected.FieldName = "IsSelected";
        colSeriesSelected.Name = "colSeriesSelected";
        colSeriesSelected.Visible = true;
        colSeriesSelected.VisibleIndex = 7;
        colSeriesSelected.Width = 80;
        // 
        // pnlRight
        // 
        pnlRight.BorderStyle = BorderStyles.NoBorder;
        pnlRight.Controls.Add(pnlAudit);
        pnlRight.Controls.Add(pnlDetail);
        pnlRight.Dock = DockStyle.Right;
        pnlRight.Location = new Point(1050, 102);
        pnlRight.Name = "pnlRight";
        pnlRight.Size = new Size(310, 658);
        pnlRight.TabIndex = 4;
        // 
        // pnlAudit
        // 
        pnlAudit.Controls.Add(lblAuditTitle);
        pnlAudit.Controls.Add(lblAuditUpdatedCaption);
        pnlAudit.Controls.Add(lblAuditUpdated);
        pnlAudit.Controls.Add(lblAuditUserCaption);
        pnlAudit.Controls.Add(lblAuditUser);
        pnlAudit.Controls.Add(lblAuditEquipmentCaption);
        pnlAudit.Controls.Add(lblAuditEquipment);
        pnlAudit.Controls.Add(lblAuditIpCaption);
        pnlAudit.Controls.Add(lblAuditIp);
        pnlAudit.Controls.Add(lblAuditObservationsCaption);
        pnlAudit.Controls.Add(memAuditObservations);
        pnlAudit.Dock = DockStyle.Fill;
        pnlAudit.Location = new Point(0, 272);
        pnlAudit.Name = "pnlAudit";
        pnlAudit.Size = new Size(310, 386);
        pnlAudit.TabIndex = 1;
        // 
        // lblAuditTitle
        // 
        lblAuditTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblAuditTitle.Appearance.ForeColor = BrandResources.Text;
        lblAuditTitle.Appearance.Options.UseFont = true;
        lblAuditTitle.Appearance.Options.UseForeColor = true;
        lblAuditTitle.Location = new Point(10, 10);
        lblAuditTitle.Name = "lblAuditTitle";
        lblAuditTitle.Size = new Size(230, 15);
        lblAuditTitle.TabIndex = 0;
        lblAuditTitle.Text = "Información de auditoría / Observaciones";
        // 
        // lblAuditUpdatedCaption
        // 
        lblAuditUpdatedCaption.Location = new Point(16, 48);
        lblAuditUpdatedCaption.Name = "lblAuditUpdatedCaption";
        lblAuditUpdatedCaption.Size = new Size(102, 13);
        lblAuditUpdatedCaption.TabIndex = 1;
        lblAuditUpdatedCaption.Text = "Última modificación:";
        // 
        // lblAuditUpdated
        // 
        lblAuditUpdated.Location = new Point(132, 48);
        lblAuditUpdated.Name = "lblAuditUpdated";
        lblAuditUpdated.Size = new Size(4, 13);
        lblAuditUpdated.TabIndex = 2;
        lblAuditUpdated.Text = "-";
        // 
        // lblAuditUserCaption
        // 
        lblAuditUserCaption.Location = new Point(16, 76);
        lblAuditUserCaption.Name = "lblAuditUserCaption";
        lblAuditUserCaption.Size = new Size(40, 13);
        lblAuditUserCaption.TabIndex = 3;
        lblAuditUserCaption.Text = "Usuario:";
        // 
        // lblAuditUser
        // 
        lblAuditUser.Location = new Point(132, 76);
        lblAuditUser.Name = "lblAuditUser";
        lblAuditUser.Size = new Size(4, 13);
        lblAuditUser.TabIndex = 4;
        lblAuditUser.Text = "-";
        // 
        // lblAuditEquipmentCaption
        // 
        lblAuditEquipmentCaption.Location = new Point(16, 104);
        lblAuditEquipmentCaption.Name = "lblAuditEquipmentCaption";
        lblAuditEquipmentCaption.Size = new Size(37, 13);
        lblAuditEquipmentCaption.TabIndex = 5;
        lblAuditEquipmentCaption.Text = "Equipo:";
        // 
        // lblAuditEquipment
        // 
        lblAuditEquipment.Location = new Point(132, 104);
        lblAuditEquipment.Name = "lblAuditEquipment";
        lblAuditEquipment.Size = new Size(4, 13);
        lblAuditEquipment.TabIndex = 6;
        lblAuditEquipment.Text = "-";
        // 
        // lblAuditIpCaption
        // 
        lblAuditIpCaption.Location = new Point(16, 132);
        lblAuditIpCaption.Name = "lblAuditIpCaption";
        lblAuditIpCaption.Size = new Size(12, 13);
        lblAuditIpCaption.TabIndex = 7;
        lblAuditIpCaption.Text = "IP:";
        // 
        // lblAuditIp
        // 
        lblAuditIp.Location = new Point(132, 132);
        lblAuditIp.Name = "lblAuditIp";
        lblAuditIp.Size = new Size(4, 13);
        lblAuditIp.TabIndex = 8;
        lblAuditIp.Text = "-";
        // 
        // lblAuditObservationsCaption
        // 
        lblAuditObservationsCaption.Location = new Point(16, 162);
        lblAuditObservationsCaption.Name = "lblAuditObservationsCaption";
        lblAuditObservationsCaption.Size = new Size(77, 13);
        lblAuditObservationsCaption.TabIndex = 9;
        lblAuditObservationsCaption.Text = "Observaciones:";
        // 
        // memAuditObservations
        // 
        memAuditObservations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        memAuditObservations.Location = new Point(132, 160);
        memAuditObservations.Name = "memAuditObservations";
        memAuditObservations.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        memAuditObservations.Properties.Appearance.Options.UseFont = true;
        memAuditObservations.Properties.NullValuePrompt = "Ingrese observaciones (opcional)...";
        memAuditObservations.Size = new Size(160, 200);
        memAuditObservations.TabIndex = 10;
        // 
        // pnlDetail
        // 
        pnlDetail.Controls.Add(lblDetailTitle);
        pnlDetail.Controls.Add(lblDetailRoleCaption);
        pnlDetail.Controls.Add(lblDetailRole);
        pnlDetail.Controls.Add(lblDetailFormCaption);
        pnlDetail.Controls.Add(lblDetailForm);
        pnlDetail.Controls.Add(lblDetailDocumentTypeCaption);
        pnlDetail.Controls.Add(lblDetailDocumentType);
        pnlDetail.Controls.Add(lblDetailSeriesCaption);
        pnlDetail.Controls.Add(lblDetailSeries);
        pnlDetail.Controls.Add(lblDetailCodeCaption);
        pnlDetail.Controls.Add(lblDetailCode);
        pnlDetail.Controls.Add(lblDetailEstablishmentCaption);
        pnlDetail.Controls.Add(lblDetailEstablishment);
        pnlDetail.Controls.Add(lblDetailEmissionPointCaption);
        pnlDetail.Controls.Add(lblDetailEmissionPoint);
        pnlDetail.Controls.Add(lblDetailStatusCaption);
        pnlDetail.Controls.Add(lblDetailStatus);
        pnlDetail.Dock = DockStyle.Top;
        pnlDetail.Location = new Point(0, 0);
        pnlDetail.Name = "pnlDetail";
        pnlDetail.Size = new Size(310, 272);
        pnlDetail.TabIndex = 0;
        // 
        // lblDetailTitle
        // 
        lblDetailTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblDetailTitle.Appearance.ForeColor = BrandResources.Text;
        lblDetailTitle.Appearance.Options.UseFont = true;
        lblDetailTitle.Appearance.Options.UseForeColor = true;
        lblDetailTitle.Location = new Point(10, 10);
        lblDetailTitle.Name = "lblDetailTitle";
        lblDetailTitle.Size = new Size(166, 15);
        lblDetailTitle.TabIndex = 0;
        lblDetailTitle.Text = "Detalle del acceso seleccionado";
        // 
        // lblDetailRoleCaption
        // 
        lblDetailRoleCaption.Location = new Point(16, 48);
        lblDetailRoleCaption.Name = "lblDetailRoleCaption";
        lblDetailRoleCaption.Size = new Size(19, 13);
        lblDetailRoleCaption.TabIndex = 1;
        lblDetailRoleCaption.Text = "Rol:";
        // 
        // lblDetailRole
        // 
        lblDetailRole.Location = new Point(132, 48);
        lblDetailRole.Name = "lblDetailRole";
        lblDetailRole.Size = new Size(4, 13);
        lblDetailRole.TabIndex = 2;
        lblDetailRole.Text = "-";
        // 
        // lblDetailFormCaption
        // 
        lblDetailFormCaption.Location = new Point(16, 76);
        lblDetailFormCaption.Name = "lblDetailFormCaption";
        lblDetailFormCaption.Size = new Size(56, 13);
        lblDetailFormCaption.TabIndex = 3;
        lblDetailFormCaption.Text = "Formulario:";
        // 
        // lblDetailForm
        // 
        lblDetailForm.Location = new Point(132, 76);
        lblDetailForm.Name = "lblDetailForm";
        lblDetailForm.Size = new Size(4, 13);
        lblDetailForm.TabIndex = 4;
        lblDetailForm.Text = "-";
        // 
        // lblDetailDocumentTypeCaption
        // 
        lblDetailDocumentTypeCaption.Location = new Point(16, 104);
        lblDetailDocumentTypeCaption.Name = "lblDetailDocumentTypeCaption";
        lblDetailDocumentTypeCaption.Size = new Size(97, 13);
        lblDetailDocumentTypeCaption.TabIndex = 5;
        lblDetailDocumentTypeCaption.Text = "Tipo de documento:";
        // 
        // lblDetailDocumentType
        // 
        lblDetailDocumentType.Location = new Point(132, 104);
        lblDetailDocumentType.Name = "lblDetailDocumentType";
        lblDetailDocumentType.Size = new Size(4, 13);
        lblDetailDocumentType.TabIndex = 6;
        lblDetailDocumentType.Text = "-";
        // 
        // lblDetailSeriesCaption
        // 
        lblDetailSeriesCaption.Location = new Point(16, 132);
        lblDetailSeriesCaption.Name = "lblDetailSeriesCaption";
        lblDetailSeriesCaption.Size = new Size(29, 13);
        lblDetailSeriesCaption.TabIndex = 7;
        lblDetailSeriesCaption.Text = "Serie:";
        // 
        // lblDetailSeries
        // 
        lblDetailSeries.Location = new Point(132, 132);
        lblDetailSeries.Name = "lblDetailSeries";
        lblDetailSeries.Size = new Size(4, 13);
        lblDetailSeries.TabIndex = 8;
        lblDetailSeries.Text = "-";
        // 
        // lblDetailCodeCaption
        // 
        lblDetailCodeCaption.Location = new Point(16, 160);
        lblDetailCodeCaption.Name = "lblDetailCodeCaption";
        lblDetailCodeCaption.Size = new Size(37, 13);
        lblDetailCodeCaption.TabIndex = 9;
        lblDetailCodeCaption.Text = "Código:";
        // 
        // lblDetailCode
        // 
        lblDetailCode.Location = new Point(132, 160);
        lblDetailCode.Name = "lblDetailCode";
        lblDetailCode.Size = new Size(4, 13);
        lblDetailCode.TabIndex = 10;
        lblDetailCode.Text = "-";
        // 
        // lblDetailEstablishmentCaption
        // 
        lblDetailEstablishmentCaption.Location = new Point(16, 188);
        lblDetailEstablishmentCaption.Name = "lblDetailEstablishmentCaption";
        lblDetailEstablishmentCaption.Size = new Size(80, 13);
        lblDetailEstablishmentCaption.TabIndex = 11;
        lblDetailEstablishmentCaption.Text = "Establecimiento:";
        // 
        // lblDetailEstablishment
        // 
        lblDetailEstablishment.Location = new Point(132, 188);
        lblDetailEstablishment.Name = "lblDetailEstablishment";
        lblDetailEstablishment.Size = new Size(4, 13);
        lblDetailEstablishment.TabIndex = 12;
        lblDetailEstablishment.Text = "-";
        // 
        // lblDetailEmissionPointCaption
        // 
        lblDetailEmissionPointCaption.Location = new Point(16, 216);
        lblDetailEmissionPointCaption.Name = "lblDetailEmissionPointCaption";
        lblDetailEmissionPointCaption.Size = new Size(86, 13);
        lblDetailEmissionPointCaption.TabIndex = 13;
        lblDetailEmissionPointCaption.Text = "Punto de emisión:";
        // 
        // lblDetailEmissionPoint
        // 
        lblDetailEmissionPoint.Location = new Point(132, 216);
        lblDetailEmissionPoint.Name = "lblDetailEmissionPoint";
        lblDetailEmissionPoint.Size = new Size(4, 13);
        lblDetailEmissionPoint.TabIndex = 14;
        lblDetailEmissionPoint.Text = "-";
        // 
        // lblDetailStatusCaption
        // 
        lblDetailStatusCaption.Location = new Point(16, 244);
        lblDetailStatusCaption.Name = "lblDetailStatusCaption";
        lblDetailStatusCaption.Size = new Size(37, 13);
        lblDetailStatusCaption.TabIndex = 15;
        lblDetailStatusCaption.Text = "Activo:";
        // 
        // lblDetailStatus
        // 
        lblDetailStatus.Location = new Point(132, 244);
        lblDetailStatus.Name = "lblDetailStatus";
        lblDetailStatus.Size = new Size(4, 13);
        lblDetailStatus.TabIndex = 16;
        lblDetailStatus.Text = "-";
        // 
        // pnlLeft
        // 
        pnlLeft.BorderStyle = BorderStyles.NoBorder;
        pnlLeft.Controls.Add(pnlForms);
        pnlLeft.Controls.Add(pnlRoles);
        pnlLeft.Dock = DockStyle.Left;
        pnlLeft.Location = new Point(0, 102);
        pnlLeft.Name = "pnlLeft";
        pnlLeft.Size = new Size(500, 658);
        pnlLeft.TabIndex = 2;
        // 
        // pnlForms
        // 
        pnlForms.Controls.Add(lblFormsTitle);
        pnlForms.Controls.Add(grcForms);
        pnlForms.Dock = DockStyle.Fill;
        pnlForms.Location = new Point(230, 0);
        pnlForms.Name = "pnlForms";
        pnlForms.Size = new Size(270, 658);
        pnlForms.TabIndex = 1;
        // 
        // lblFormsTitle
        // 
        lblFormsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblFormsTitle.Appearance.ForeColor = BrandResources.Text;
        lblFormsTitle.Appearance.Options.UseFont = true;
        lblFormsTitle.Appearance.Options.UseForeColor = true;
        lblFormsTitle.Location = new Point(10, 9);
        lblFormsTitle.Name = "lblFormsTitle";
        lblFormsTitle.Size = new Size(178, 15);
        lblFormsTitle.TabIndex = 0;
        lblFormsTitle.Text = "FORMULARIOS TRANSACCIONALES";
        // 
        // grcForms
        // 
        grcForms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grcForms.Location = new Point(8, 30);
        grcForms.MainView = grvForms;
        grcForms.Name = "grcForms";
        grcForms.Size = new Size(254, 620);
        grcForms.TabIndex = 1;
        grcForms.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvForms });
        // 
        // grvForms
        // 
        grvForms.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        grvForms.Appearance.HeaderPanel.Options.UseFont = true;
        grvForms.Appearance.Row.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        grvForms.Appearance.Row.Options.UseFont = true;
        grvForms.Columns.AddRange(new GridColumn[] { colFormName });
        grvForms.GridControl = grcForms;
        grvForms.Name = "grvForms";
        grvForms.OptionsBehavior.Editable = false;
        grvForms.OptionsView.ShowColumnHeaders = false;
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
        // 
        // pnlRoles
        // 
        pnlRoles.Controls.Add(lblRolesTitle);
        pnlRoles.Controls.Add(lstRoles);
        pnlRoles.Dock = DockStyle.Left;
        pnlRoles.Location = new Point(0, 0);
        pnlRoles.Name = "pnlRoles";
        pnlRoles.Size = new Size(230, 658);
        pnlRoles.TabIndex = 0;
        // 
        // lblRolesTitle
        // 
        lblRolesTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblRolesTitle.Appearance.ForeColor = BrandResources.Text;
        lblRolesTitle.Appearance.Options.UseFont = true;
        lblRolesTitle.Appearance.Options.UseForeColor = true;
        lblRolesTitle.Location = new Point(10, 9);
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
        lstRoles.Location = new Point(8, 30);
        lstRoles.Name = "lstRoles";
        lstRoles.Size = new Size(214, 620);
        lstRoles.TabIndex = 1;
        // 
        // pnlFilters
        // 
        pnlFilters.BorderStyle = BorderStyles.NoBorder;
        pnlFilters.Controls.Add(lblFormFilter);
        pnlFilters.Controls.Add(lueFormFilter);
        pnlFilters.Controls.Add(lblDocumentType);
        pnlFilters.Controls.Add(lueDocumentType);
        pnlFilters.Controls.Add(lblSeriesFilter);
        pnlFilters.Controls.Add(lueSeriesFilter);
        pnlFilters.Controls.Add(lblSearch);
        pnlFilters.Controls.Add(txtSearch);
        pnlFilters.Controls.Add(chkOnlyActive);
        pnlFilters.Dock = DockStyle.Top;
        pnlFilters.Location = new Point(0, 54);
        pnlFilters.Name = "pnlFilters";
        pnlFilters.Size = new Size(1360, 48);
        pnlFilters.TabIndex = 1;
        // 
        // lblFormFilter
        // 
        lblFormFilter.Location = new Point(12, 17);
        lblFormFilter.Name = "lblFormFilter";
        lblFormFilter.Size = new Size(55, 13);
        lblFormFilter.TabIndex = 0;
        lblFormFilter.Text = "Formulario:";
        // 
        // lueFormFilter
        // 
        lueFormFilter.Location = new Point(76, 13);
        lueFormFilter.Name = "lueFormFilter";
        lueFormFilter.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lueFormFilter.Properties.Appearance.Options.UseFont = true;
        lueFormFilter.Properties.AutoHeight = false;
        lueFormFilter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFormFilter.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 85), new LookUpColumnInfo("Name", "Nombre", 180) });
        lueFormFilter.Properties.DisplayMember = "Name";
        lueFormFilter.Properties.NullText = "";
        lueFormFilter.Properties.ValueMember = "Id";
        lueFormFilter.Size = new Size(210, 22);
        lueFormFilter.TabIndex = 1;
        // 
        // lblDocumentType
        // 
        lblDocumentType.Location = new Point(306, 17);
        lblDocumentType.Name = "lblDocumentType";
        lblDocumentType.Size = new Size(94, 13);
        lblDocumentType.TabIndex = 2;
        lblDocumentType.Text = "Tipo de documento:";
        // 
        // lueDocumentType
        // 
        lueDocumentType.Location = new Point(408, 13);
        lueDocumentType.Name = "lueDocumentType";
        lueDocumentType.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lueDocumentType.Properties.Appearance.Options.UseFont = true;
        lueDocumentType.Properties.AutoHeight = false;
        lueDocumentType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueDocumentType.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 75), new LookUpColumnInfo("Name", "Nombre", 160) });
        lueDocumentType.Properties.DisplayMember = "Name";
        lueDocumentType.Properties.NullText = "(Todos)";
        lueDocumentType.Properties.ValueMember = "Code";
        lueDocumentType.Size = new Size(190, 22);
        lueDocumentType.TabIndex = 3;
        // 
        // lblSeriesFilter
        // 
        lblSeriesFilter.Location = new Point(618, 17);
        lblSeriesFilter.Name = "lblSeriesFilter";
        lblSeriesFilter.Size = new Size(29, 13);
        lblSeriesFilter.TabIndex = 4;
        lblSeriesFilter.Text = "Serie:";
        // 
        // lueSeriesFilter
        // 
        lueSeriesFilter.Location = new Point(656, 13);
        lueSeriesFilter.Name = "lueSeriesFilter";
        lueSeriesFilter.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lueSeriesFilter.Properties.Appearance.Options.UseFont = true;
        lueSeriesFilter.Properties.AutoHeight = false;
        lueSeriesFilter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSeriesFilter.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 75), new LookUpColumnInfo("Name", "Nombre", 180) });
        lueSeriesFilter.Properties.DisplayMember = "DisplayName";
        lueSeriesFilter.Properties.NullText = "(Todas)";
        lueSeriesFilter.Properties.ValueMember = "Id";
        lueSeriesFilter.Size = new Size(210, 22);
        lueSeriesFilter.TabIndex = 5;
        // 
        // lblSearch
        // 
        lblSearch.Location = new Point(886, 17);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(40, 13);
        lblSearch.TabIndex = 6;
        lblSearch.Text = "Buscar:";
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(934, 13);
        txtSearch.Name = "txtSearch";
        txtSearch.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        txtSearch.Properties.Appearance.Options.UseFont = true;
        txtSearch.Properties.AutoHeight = false;
        txtSearch.Properties.NullValuePrompt = "Buscar por código o nombre...";
        txtSearch.Size = new Size(210, 22);
        txtSearch.TabIndex = 7;
        // 
        // chkOnlyActive
        // 
        chkOnlyActive.EditValue = true;
        chkOnlyActive.Location = new Point(1160, 12);
        chkOnlyActive.Name = "chkOnlyActive";
        chkOnlyActive.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        chkOnlyActive.Properties.Appearance.Options.UseFont = true;
        chkOnlyActive.Properties.Caption = "Mostrar solo activas";
        chkOnlyActive.Size = new Size(150, 24);
        chkOnlyActive.TabIndex = 8;
        // 
        // pnlHeader
        // 
        pnlHeader.BorderStyle = BorderStyles.NoBorder;
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(btnSave);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1360, 54);
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
        lblTitle.Size = new Size(372, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Accesos a Campos de Formularios Transaccionales";
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
        btnSave.Location = new Point(1198, 10);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(144, 34);
        btnSave.TabIndex = 1;
        btnSave.Text = "Guardar cambios";
        // 
        // SecurityTransactionalFieldAccessForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1360, 760);
        Controls.Add(pnlRoot);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Name = "SecurityTransactionalFieldAccessForm";
        Text = "Accesos a Campos de Formularios Transaccionales";
        ((System.ComponentModel.ISupportInitialize)pnlRoot).EndInit();
        pnlRoot.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlCenter).EndInit();
        pnlCenter.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlOperations).EndInit();
        pnlOperations.ResumeLayout(false);
        pnlOperations.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grcOperations).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvOperations).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlSeries).EndInit();
        pnlSeries.ResumeLayout(false);
        pnlSeries.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grcSeries).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSeries).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlRight).EndInit();
        pnlRight.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAudit).EndInit();
        pnlAudit.ResumeLayout(false);
        pnlAudit.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memAuditObservations.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlDetail).EndInit();
        pnlDetail.ResumeLayout(false);
        pnlDetail.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlLeft).EndInit();
        pnlLeft.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlForms).EndInit();
        pnlForms.ResumeLayout(false);
        pnlForms.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grcForms).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvForms).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlRoles).EndInit();
        pnlRoles.ResumeLayout(false);
        pnlRoles.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lstRoles).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlFilters).EndInit();
        pnlFilters.ResumeLayout(false);
        pnlFilters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueFormFilter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSeriesFilter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkOnlyActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).EndInit();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
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
