using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sap;

partial class SapSyncProfileEditForm
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl generalSectionLabel;
    private LabelControl companyLabel;
    private NuanLookupEdit companyEdit;
    private LabelControl codeLabel;
    private TextEdit codeEdit;
    private LabelControl nameLabel;
    private TextEdit nameEdit;
    private LabelControl descriptionLabel;
    private MemoEdit descriptionEdit;
    private LabelControl statusLabel;
    private LabelControl entitiesSectionLabel;
    private LabelControl informationLabel;
    private GridControl entitiesGrid;
    private GridView entitiesView;
    private RepositoryItemComboBox directionRepository;
    private RepositoryItemComboBox modeRepository;
    private RepositoryItemComboBox scheduleRepository;
    private GridColumn entityCodeColumn;
    private GridColumn entityNameColumn;
    private GridColumn directionColumn;
    private GridColumn modeColumn;
    private GridColumn batchColumn;
    private GridColumn attemptsColumn;
    private GridColumn orderColumn;
    private GridColumn activeColumn;
    private GridColumn scheduleTypeColumn;
    private GridColumn intervalColumn;
    private GridColumn continueOnErrorColumn;
    private GridColumn timeoutColumn;
    private GridColumn executionTimeColumn;
    private GridColumn timeZoneColumn;
    private GridColumn preventConcurrentColumn;
    private GridColumn scheduleActiveColumn;
    private GridColumn nextExecutionColumn;
    private GridColumn lastExecutionColumn;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        generalSectionLabel = new LabelControl();
        companyLabel = new LabelControl();
        companyEdit = new NuanLookupEdit();
        codeLabel = new LabelControl();
        codeEdit = new TextEdit();
        nameLabel = new LabelControl();
        nameEdit = new TextEdit();
        descriptionLabel = new LabelControl();
        descriptionEdit = new MemoEdit();
        statusLabel = new LabelControl();
        entitiesSectionLabel = new LabelControl();
        informationLabel = new LabelControl();
        entitiesGrid = new GridControl();
        entitiesView = new GridView();
        directionRepository = new RepositoryItemComboBox();
        modeRepository = new RepositoryItemComboBox();
        scheduleRepository = new RepositoryItemComboBox();
        entityCodeColumn = new GridColumn();
        entityNameColumn = new GridColumn();
        directionColumn = new GridColumn();
        modeColumn = new GridColumn();
        batchColumn = new GridColumn();
        attemptsColumn = new GridColumn();
        orderColumn = new GridColumn();
        activeColumn = new GridColumn();
        continueOnErrorColumn = new GridColumn();
        timeoutColumn = new GridColumn();
        scheduleTypeColumn = new GridColumn();
        intervalColumn = new GridColumn();
        executionTimeColumn = new GridColumn();
        timeZoneColumn = new GridColumn();
        preventConcurrentColumn = new GridColumn();
        scheduleActiveColumn = new GridColumn();
        nextExecutionColumn = new GridColumn();
        lastExecutionColumn = new GridColumn();
        ((System.ComponentModel.ISupportInitialize)companyEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)codeEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nameEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)descriptionEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)entitiesGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)entitiesView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)directionRepository).BeginInit();
        ((System.ComponentModel.ISupportInitialize)modeRepository).BeginInit();
        ((System.ComponentModel.ISupportInitialize)scheduleRepository).BeginInit();
        SuspendLayout();
        //
        // generalSectionLabel
        //
        generalSectionLabel.Appearance.Font = AppTypography.SectionFont;
        generalSectionLabel.Appearance.ForeColor = BrandResources.Primary;
        generalSectionLabel.Appearance.Options.UseFont = true;
        generalSectionLabel.Appearance.Options.UseForeColor = true;
        generalSectionLabel.Location = new Point(24, 18);
        generalSectionLabel.Name = "generalSectionLabel";
        generalSectionLabel.Size = new Size(106, 20);
        generalSectionLabel.TabIndex = 4;
        generalSectionLabel.Text = "Datos generales";
        //
        // companyLabel
        //
        companyLabel.Appearance.Font = AppTypography.LabelFont;
        companyLabel.Appearance.Options.UseFont = true;
        companyLabel.Location = new Point(24, 53);
        companyLabel.Name = "companyLabel";
        companyLabel.Size = new Size(75, 15);
        companyLabel.TabIndex = 5;
        companyLabel.Text = "Empresa SAP";
        //
        // companyEdit
        //
        companyEdit.ClearButtonEnabled = false;
        companyEdit.CreateButtonEnabled = false;
        companyEdit.Location = new Point(160, 50);
        companyEdit.Name = "companyEdit";
        companyEdit.Properties.Appearance.Font = AppTypography.InputFont;
        companyEdit.Properties.Appearance.Options.UseFont = true;
        companyEdit.Properties.AutoHeight = false;
        companyEdit.Properties.NullText = "Seleccione...";
        companyEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        companyEdit.Size = new Size(360, 22);
        companyEdit.TabIndex = 0;
        //
        // codeLabel
        //
        codeLabel.Appearance.Font = AppTypography.LabelFont;
        codeLabel.Appearance.Options.UseFont = true;
        codeLabel.Location = new Point(590, 53);
        codeLabel.Name = "codeLabel";
        codeLabel.Size = new Size(39, 15);
        codeLabel.TabIndex = 6;
        codeLabel.Text = "Codigo";
        //
        // codeEdit
        //
        codeEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        codeEdit.Location = new Point(700, 50);
        codeEdit.Name = "codeEdit";
        codeEdit.Properties.Appearance.Font = AppTypography.InputFont;
        codeEdit.Properties.Appearance.Options.UseFont = true;
        codeEdit.Properties.AutoHeight = false;
        codeEdit.Properties.MaxLength = 50;
        codeEdit.Size = new Size(436, 22);
        codeEdit.TabIndex = 1;
        //
        // nameLabel
        //
        nameLabel.Appearance.Font = AppTypography.LabelFont;
        nameLabel.Appearance.Options.UseFont = true;
        nameLabel.Location = new Point(24, 81);
        nameLabel.Name = "nameLabel";
        nameLabel.Size = new Size(44, 15);
        nameLabel.TabIndex = 7;
        nameLabel.Text = "Nombre";
        //
        // nameEdit
        //
        nameEdit.Location = new Point(160, 78);
        nameEdit.Name = "nameEdit";
        nameEdit.Properties.Appearance.Font = AppTypography.InputFont;
        nameEdit.Properties.Appearance.Options.UseFont = true;
        nameEdit.Properties.AutoHeight = false;
        nameEdit.Properties.MaxLength = 150;
        nameEdit.Size = new Size(360, 22);
        nameEdit.TabIndex = 2;
        //
        // descriptionLabel
        //
        descriptionLabel.Appearance.Font = AppTypography.LabelFont;
        descriptionLabel.Appearance.Options.UseFont = true;
        descriptionLabel.Location = new Point(590, 81);
        descriptionLabel.Name = "descriptionLabel";
        descriptionLabel.Size = new Size(62, 15);
        descriptionLabel.TabIndex = 8;
        descriptionLabel.Text = "Descripcion";
        //
        // descriptionEdit
        //
        descriptionEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        descriptionEdit.Location = new Point(700, 78);
        descriptionEdit.Name = "descriptionEdit";
        descriptionEdit.Properties.Appearance.Font = AppTypography.InputFont;
        descriptionEdit.Properties.Appearance.Options.UseFont = true;
        descriptionEdit.Properties.MaxLength = 500;
        descriptionEdit.Size = new Size(436, 58);
        descriptionEdit.TabIndex = 3;
        //
        // statusLabel
        //
        statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        statusLabel.Appearance.Font = AppTypography.SmallFont;
        statusLabel.Appearance.ForeColor = BrandResources.MutedText;
        statusLabel.Appearance.Options.UseFont = true;
        statusLabel.Appearance.Options.UseForeColor = true;
        statusLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        statusLabel.AutoSizeMode = LabelAutoSizeMode.None;
        statusLabel.Location = new Point(866, 18);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(270, 20);
        statusLabel.TabIndex = 9;
        statusLabel.Text = "Estado";
        //
        // entitiesSectionLabel
        //
        entitiesSectionLabel.Appearance.Font = AppTypography.SectionFont;
        entitiesSectionLabel.Appearance.ForeColor = BrandResources.Primary;
        entitiesSectionLabel.Appearance.Options.UseFont = true;
        entitiesSectionLabel.Appearance.Options.UseForeColor = true;
        entitiesSectionLabel.Location = new Point(24, 158);
        entitiesSectionLabel.Name = "entitiesSectionLabel";
        entitiesSectionLabel.Size = new Size(181, 20);
        entitiesSectionLabel.TabIndex = 10;
        entitiesSectionLabel.Text = "Entidades y programacion";
        //
        // informationLabel
        //
        informationLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        informationLabel.Appearance.Font = AppTypography.SmallFont;
        informationLabel.Appearance.ForeColor = BrandResources.MutedText;
        informationLabel.Appearance.Options.UseFont = true;
        informationLabel.Appearance.Options.UseForeColor = true;
        informationLabel.AutoSizeMode = LabelAutoSizeMode.None;
        informationLabel.Location = new Point(24, 681);
        informationLabel.Name = "informationLabel";
        informationLabel.Size = new Size(860, 18);
        informationLabel.TabIndex = 11;
        informationLabel.Text = "El perfil se guarda inactivo hasta completar su validacion y activacion.";
        //
        // entitiesGrid
        //
        entitiesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        entitiesGrid.Location = new Point(24, 188);
        entitiesGrid.MainView = entitiesView;
        entitiesGrid.Name = "entitiesGrid";
        entitiesGrid.RepositoryItems.AddRange(new RepositoryItem[] { directionRepository, modeRepository, scheduleRepository });
        entitiesGrid.Size = new Size(1112, 470);
        entitiesGrid.TabIndex = 4;
        entitiesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { entitiesView });
        //
        // entitiesView
        //
        entitiesView.Appearance.HeaderPanel.Font = AppTypography.GridHeaderFont;
        entitiesView.Appearance.HeaderPanel.Options.UseFont = true;
        entitiesView.Appearance.Row.Font = AppTypography.GridRowFont;
        entitiesView.Appearance.Row.Options.UseFont = true;
        entitiesView.Columns.AddRange(new GridColumn[] { entityCodeColumn, entityNameColumn, directionColumn, modeColumn, batchColumn, attemptsColumn, orderColumn, activeColumn, continueOnErrorColumn, timeoutColumn, scheduleTypeColumn, intervalColumn, executionTimeColumn, timeZoneColumn, preventConcurrentColumn, scheduleActiveColumn, nextExecutionColumn, lastExecutionColumn });
        entitiesView.GridControl = entitiesGrid;
        entitiesView.Name = "entitiesView";
        entitiesView.OptionsView.ColumnAutoWidth = false;
        entitiesView.OptionsView.ShowGroupPanel = false;
        entitiesView.OptionsView.ShowIndicator = false;
        //
        // repositories
        //
        directionRepository.AutoHeight = false;
        directionRepository.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
        directionRepository.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        modeRepository.AutoHeight = false;
        modeRepository.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
        modeRepository.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        scheduleRepository.AutoHeight = false;
        scheduleRepository.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
        scheduleRepository.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        //
        // columns
        //
        entityCodeColumn.Caption = "Entidad"; entityCodeColumn.FieldName = "EntityCode"; entityCodeColumn.OptionsColumn.ReadOnly = true; entityCodeColumn.Visible = true; entityCodeColumn.VisibleIndex = 0; entityCodeColumn.Width = 115;
        entityNameColumn.Caption = "Nombre"; entityNameColumn.FieldName = "EntityName"; entityNameColumn.OptionsColumn.ReadOnly = true; entityNameColumn.Visible = true; entityNameColumn.VisibleIndex = 1; entityNameColumn.Width = 170;
        directionColumn.Caption = "Direccion"; directionColumn.ColumnEdit = directionRepository; directionColumn.FieldName = "Direction"; directionColumn.Visible = true; directionColumn.VisibleIndex = 2; directionColumn.Width = 110;
        modeColumn.Caption = "Modo"; modeColumn.ColumnEdit = modeRepository; modeColumn.FieldName = "SyncMode"; modeColumn.Visible = true; modeColumn.VisibleIndex = 3; modeColumn.Width = 90;
        batchColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far; batchColumn.Caption = "Lote"; batchColumn.FieldName = "BatchSize"; batchColumn.Visible = true; batchColumn.VisibleIndex = 4; batchColumn.Width = 70;
        attemptsColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far; attemptsColumn.Caption = "Intentos"; attemptsColumn.FieldName = "MaxAttempts"; attemptsColumn.Visible = true; attemptsColumn.VisibleIndex = 5; attemptsColumn.Width = 70;
        orderColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far; orderColumn.Caption = "Orden"; orderColumn.FieldName = "ExecutionOrder"; orderColumn.Visible = true; orderColumn.VisibleIndex = 6; orderColumn.Width = 65;
        activeColumn.Caption = "Activa"; activeColumn.FieldName = "IsActive"; activeColumn.Visible = true; activeColumn.VisibleIndex = 7; activeColumn.Width = 65;
        continueOnErrorColumn.Caption = "Continuar con error"; continueOnErrorColumn.FieldName = "ContinueOnError"; continueOnErrorColumn.Visible = true; continueOnErrorColumn.VisibleIndex = 8; continueOnErrorColumn.Width = 115;
        timeoutColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far; timeoutColumn.Caption = "Timeout (min)"; timeoutColumn.FieldName = "ExecutionTimeoutMinutes"; timeoutColumn.Visible = true; timeoutColumn.VisibleIndex = 9; timeoutColumn.Width = 90;
        scheduleTypeColumn.Caption = "Agenda"; scheduleTypeColumn.ColumnEdit = scheduleRepository; scheduleTypeColumn.FieldName = "ScheduleType"; scheduleTypeColumn.Visible = true; scheduleTypeColumn.VisibleIndex = 10; scheduleTypeColumn.Width = 90;
        intervalColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far; intervalColumn.Caption = "Minutos"; intervalColumn.FieldName = "IntervalMinutes"; intervalColumn.Visible = true; intervalColumn.VisibleIndex = 11; intervalColumn.Width = 70;
        executionTimeColumn.Caption = "Hora"; executionTimeColumn.FieldName = "ExecutionTime"; executionTimeColumn.Visible = true; executionTimeColumn.VisibleIndex = 12; executionTimeColumn.Width = 80;
        timeZoneColumn.Caption = "Zona horaria"; timeZoneColumn.FieldName = "TimeZoneId"; timeZoneColumn.Visible = true; timeZoneColumn.VisibleIndex = 13; timeZoneColumn.Width = 150;
        preventConcurrentColumn.Caption = "Evitar simultaneas"; preventConcurrentColumn.FieldName = "PreventConcurrentExecutions"; preventConcurrentColumn.Visible = true; preventConcurrentColumn.VisibleIndex = 14; preventConcurrentColumn.Width = 115;
        scheduleActiveColumn.Caption = "Agenda activa"; scheduleActiveColumn.FieldName = "ScheduleIsActive"; scheduleActiveColumn.Visible = true; scheduleActiveColumn.VisibleIndex = 15; scheduleActiveColumn.Width = 90;
        nextExecutionColumn.Caption = "Proxima ejecucion"; nextExecutionColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm"; nextExecutionColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime; nextExecutionColumn.FieldName = "NextExecutionAtUtc"; nextExecutionColumn.OptionsColumn.ReadOnly = true; nextExecutionColumn.Visible = true; nextExecutionColumn.VisibleIndex = 16; nextExecutionColumn.Width = 145;
        lastExecutionColumn.Caption = "Ultima ejecucion"; lastExecutionColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm"; lastExecutionColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime; lastExecutionColumn.FieldName = "LastExecutionAtUtc"; lastExecutionColumn.OptionsColumn.ReadOnly = true; lastExecutionColumn.Visible = true; lastExecutionColumn.VisibleIndex = 17; lastExecutionColumn.Width = 145;
        //
        // inherited actions
        //
        btnCancelar.Location = new Point(936, 672);
        btnCancelar.TabIndex = 5;
        btnGuardar.Location = new Point(1042, 672);
        btnGuardar.TabIndex = 6;
        //
        // SapSyncProfileEditForm
        //
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1160, 720);
        Controls.Add(generalSectionLabel);
        Controls.Add(companyLabel);
        Controls.Add(companyEdit);
        Controls.Add(codeLabel);
        Controls.Add(codeEdit);
        Controls.Add(nameLabel);
        Controls.Add(nameEdit);
        Controls.Add(descriptionLabel);
        Controls.Add(descriptionEdit);
        Controls.Add(statusLabel);
        Controls.Add(entitiesSectionLabel);
        Controls.Add(entitiesGrid);
        Controls.Add(informationLabel);
        Font = AppTypography.BaseFont;
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimumSize = new Size(980, 650);
        Name = "SapSyncProfileEditForm";
        Text = "Perfil SAP";
        Controls.SetChildIndex(informationLabel, 0);
        Controls.SetChildIndex(entitiesGrid, 0);
        Controls.SetChildIndex(entitiesSectionLabel, 0);
        Controls.SetChildIndex(statusLabel, 0);
        Controls.SetChildIndex(descriptionEdit, 0);
        Controls.SetChildIndex(descriptionLabel, 0);
        Controls.SetChildIndex(nameEdit, 0);
        Controls.SetChildIndex(nameLabel, 0);
        Controls.SetChildIndex(codeEdit, 0);
        Controls.SetChildIndex(codeLabel, 0);
        Controls.SetChildIndex(companyEdit, 0);
        Controls.SetChildIndex(companyLabel, 0);
        Controls.SetChildIndex(generalSectionLabel, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)companyEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)codeEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)nameEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)descriptionEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)entitiesGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)entitiesView).EndInit();
        ((System.ComponentModel.ISupportInitialize)directionRepository).EndInit();
        ((System.ComponentModel.ISupportInitialize)modeRepository).EndInit();
        ((System.ComponentModel.ISupportInitialize)scheduleRepository).EndInit();
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
}
