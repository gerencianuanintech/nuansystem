using System.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private readonly Dictionary<XtraTabPage, ItemTabIconPair> documentTabIcons = new();
    private bool loadingAttachmentEditor;
    private DataRow? editedAttachmentRow;

    private void ConfigureDocumentsTab()
    {
        ConfigureDocumentLookups();
        ConfigureDocumentTabHeaders();
        ConfigureAttachmentsLayout();
        ConfigureRemarksLayout();

        gvAttachments.FocusedRowChanged += AttachmentsFocusedRowChanged;
        btnLoadImage.Click += AddAttachmentClick;
        btnRemoveImage.Click += RemoveAttachmentClick;
        btnPreviewImage.Click += OpenAttachmentClick;
        btnSetMainImage.Click += SetMainAttachmentClick;

        foreach (var editor in new DevExpress.XtraEditors.BaseEdit[]
                 {
                     lueAttachmentType, txtAttachmentFileName, memAttachmentDescription,
                     lueAttachmentCategory, lueAttachmentStatus, txtAttachmentReference,
                     chkAttachmentPrincipal, chkVisibleInSales, chkVisibleInPurchases,
                     chkVisibleInPortal, chkAttachmentConfidential, spnAttachmentOrder,
                     dteAttachmentValidFrom, dteAttachmentValidTo, memAttachmentAlternativeText
                 })
        {
            editor.EditValueChanged += AttachmentEditorChanged;
        }
    }

    private void ConfigureDocumentLookups()
    {
        BindFixedLookup(lueAttachmentType, new[]
        {
            new LookupOption("Imagen producto", "Imagen producto"),
            new LookupOption("Ficha técnica", "Ficha técnica"),
            new LookupOption("Certificado", "Certificado"),
            new LookupOption("Otro", "Otro")
        });
        BindFixedLookup(lueAttachmentCategory, new[]
        {
            new LookupOption("Comercial", "Comercial"),
            new LookupOption("Técnico", "Técnico"),
            new LookupOption("Calidad", "Calidad"),
            new LookupOption("Legal", "Legal")
        });
        BindFixedLookup(lueAttachmentStatus, new[]
        {
            new LookupOption("Activo", "Activo"),
            new LookupOption("Inactivo", "Inactivo")
        });
        BindFixedLookup(lueNotePriority, new[]
        {
            new LookupOption("Baja", "Baja"),
            new LookupOption("Media", "Media"),
            new LookupOption("Alta", "Alta")
        });
        BindFixedLookup(lueNoteVisibility, new[]
        {
            new LookupOption("Internal", "Uso interno"),
            new LookupOption("Operational", "Operación"),
            new LookupOption("All", "Todos")
        });
    }

    private void ConfigureDocumentTabHeaders()
    {
        tabDocumentSections.HandleCreated += DocumentSectionsHandleCreated;
        tabDocumentSections.SelectedPageChanged += DocumentSectionsSelectedPageChanged;
        tabDocumentSections.CustomDrawTabHeader += DocumentSectionsCustomDrawTabHeader;
        tabDocumentSections.Appearance.Font = new Font("Segoe UI", 9F);
        tabDocumentSections.Appearance.Options.UseFont = true;
        tabDocumentSections.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabDocumentSections.AppearancePage.Header.Options.UseFont = true;
        tabDocumentSections.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabDocumentSections.AppearancePage.HeaderActive.Options.UseFont = true;
        tabDocumentSections.AppearancePage.HeaderActive.ForeColor = BrandResources.Primary;
        tabDocumentSections.AppearancePage.HeaderActive.Options.UseForeColor = true;
        tabDocumentSections.HeaderAutoFill = DevExpress.Utils.DefaultBoolean.False;
        tabDocumentSections.TabPageWidth = 220;
    }

    private void ConfigureAttachmentsLayout()
    {
        tabDocuments.BackColor = Color.White;
        tabDocuments.Appearance.PageClient.BackColor = Color.White;
        tabDocuments.Appearance.PageClient.BackColor2 = Color.White;
        tabDocuments.Appearance.PageClient.Options.UseBackColor = true;
        tabDocumentSections.BackColor = Color.White;
        tabDocumentSections.Appearance.BackColor = Color.White;
        tabDocumentSections.Appearance.Options.UseBackColor = true;
        tabDocumentSections.AppearancePage.PageClient.BackColor = Color.White;
        tabDocumentSections.AppearancePage.PageClient.BackColor2 = Color.White;
        tabDocumentSections.AppearancePage.PageClient.Options.UseBackColor = true;
        tabAttachments.BackColor = Color.White;
        tabAttachments.Appearance.PageClient.BackColor = Color.White;
        tabAttachments.Appearance.PageClient.BackColor2 = Color.White;
        tabAttachments.Appearance.PageClient.Options.UseBackColor = true;

        lblAttachmentExtension.Visible = false;
        txtAttachmentExtension.Visible = false;
        lblAttachmentSize.Visible = false;
        txtAttachmentSize.Visible = false;
        lblAttachmentUploadedAt.Visible = false;
        dteAttachmentUploadedAt.Visible = false;
        lblAttachmentUser.Visible = false;
        txtAttachmentUser.Visible = false;
        lblAttachmentPreviewNoteIcon.Text = "▣";
        btnSetMainAttachment.Visible = true;
        gvAttachments.OptionsView.ColumnAutoWidth = true;
        gvAttachments.OptionsSelection.EnableAppearanceFocusedCell = false;

        ConfigureGridActionButton(btnLoadImage, "importar_16.svg", BrandResources.Primary);
        ConfigureGridActionButton(btnRemoveImage, "quitar_16.svg", Color.FromArgb(255, 51, 51));
        ConfigureGridActionButton(btnPreviewImage, "consultar_16.svg", Color.FromArgb(0, 102, 255));
        ConfigureGridActionButton(btnSetMainImage, "aprobar_16.svg", Color.FromArgb(0, 166, 81));
        ConfigureGridActionButton(btnAddAttachment, "agregar_16.svg", BrandResources.Primary);
        ConfigureGridActionButton(btnUpdateAttachment, "editar_16.svg", Color.FromArgb(0, 102, 255));
        ConfigureGridActionButton(btnRemoveAttachment, "quitar_16.svg", Color.FromArgb(255, 51, 51));
        ConfigureGridActionButton(btnSetMainAttachment, "aprobar_16.svg", Color.FromArgb(0, 166, 81));
        ConfigureGridActionButton(btnDownloadAttachment, "exportar_16.svg", Color.FromArgb(0, 102, 255));
        ConfigureGridActionButton(btnOpenAttachment, "ver_detalle_16.svg", Color.FromArgb(0, 102, 255));
    }

    private void ConfigureRemarksLayout()
    {
        tabRemarks.BackColor = Color.White;
        tabRemarks.Appearance.PageClient.BackColor = Color.White;
        tabRemarks.Appearance.PageClient.BackColor2 = Color.White;
        tabRemarks.Appearance.PageClient.Options.UseBackColor = true;
        gvOperationalAlerts.OptionsView.ColumnAutoWidth = true;
        gvOperationalAlerts.OptionsSelection.EnableAppearanceFocusedCell = false;

        ConfigureGridActionButton(btnAddOperationalAlert, "agregar_16.svg", BrandResources.Primary);
        ConfigureGridActionButton(btnUpdateOperationalAlert, "editar_16.svg", Color.FromArgb(0, 102, 255));
        ConfigureGridActionButton(btnRemoveOperationalAlert, "quitar_16.svg", Color.FromArgb(255, 51, 51));
        ConfigureGridActionButton(btnClearOperationalAlert, "limpiar_16.svg", Color.FromArgb(0, 102, 255));
    }

    private static void DocumentsTabPagePaint(object? sender, PaintEventArgs e)
    {
        if (sender is XtraTabPage)
        {
            e.Graphics.Clear(Color.White);
        }
    }

    private static void ConfigureLinkButton(DevExpress.XtraEditors.SimpleButton button, Color? color = null)
    {
        button.ButtonStyle = BorderStyles.NoBorder;
        button.Appearance.BackColor = Color.Transparent;
        button.Appearance.ForeColor = color ?? BrandResources.Primary;
        button.Appearance.Options.UseBackColor = true;
        button.Appearance.Options.UseForeColor = true;
    }

    private static void ConfigureGridActionButton(
        DevExpress.XtraEditors.SimpleButton button,
        string iconName,
        Color color)
    {
        ConfigureLinkButton(button, color);
        button.Appearance.Font = new Font("Segoe UI", 9F);
        button.Appearance.Options.UseFont = true;
        button.AppearanceHovered.BackColor = Color.FromArgb(238, 246, 255);
        button.AppearanceHovered.ForeColor = color;
        button.AppearanceHovered.Options.UseBackColor = true;
        button.AppearanceHovered.Options.UseForeColor = true;
        button.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon(iconName, color);
        button.ImageOptions.SvgImageSize = new Size(16, 16);
        button.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
        button.ImageOptions.ImageToTextIndent = 4;
    }

    private void AttachmentsFocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        CommitAttachmentEditor();
        LoadAttachmentEditor(gvAttachments.GetFocusedDataRow());
    }

    private void AttachmentEditorChanged(object? sender, EventArgs e)
    {
        if (!loadingAttachmentEditor)
        {
            CommitAttachmentEditor();
        }
    }

    private void LoadAttachmentEditor(DataRow? row)
    {
        editedAttachmentRow = row;
        if (row is null)
        {
            return;
        }

        loadingAttachmentEditor = true;
        try
        {
            lueAttachmentType.EditValue = Convert.ToString(row["TipoDocumento"]);
            lueAttachmentCategory.EditValue = Convert.ToString(row["Categoria"]);
            txtAttachmentFileName.Text = Convert.ToString(row["NombreArchivo"]) ?? string.Empty;
            memAttachmentDescription.Text = Convert.ToString(row["Descripcion"]) ?? string.Empty;
            txtAttachmentReference.Text = Convert.ToString(row["ReferenciaDocumental"]) ?? string.Empty;
            lueAttachmentStatus.EditValue = Convert.ToString(row["Estado"]);
            chkAttachmentPrincipal.IsOn = ToBool(row["Principal"]);
            chkVisibleInSales.IsOn = ToBool(row["VisibleVentas"]);
            chkVisibleInPurchases.IsOn = ToBool(row["VisibleCompras"]);
            chkVisibleInPortal.IsOn = ToBool(row["VisiblePortal"]);
            chkAttachmentConfidential.IsOn = ToBool(row["Confidencial"]);
            spnAttachmentOrder.Value = ToInt(row["OrdenVisual"]);
            dteAttachmentValidFrom.EditValue = ToDate(row["VigenciaDesde"]);
            dteAttachmentValidTo.EditValue = ToDate(row["VigenciaHasta"]);
            memAttachmentAlternativeText.Text = Convert.ToString(row["TextoAlternativo"]) ?? string.Empty;
            lblAttachmentPreviewNote.Text = $"{Convert.ToString(row["Extension"])}  ·  {Convert.ToString(row["Tamano"])}  ·  {ToDate(row["Fecha"]):dd/MM/yyyy HH:mm}  ·  {Convert.ToString(row["Usuario"])}";
        }
        finally
        {
            loadingAttachmentEditor = false;
        }
    }

    private void CommitAttachmentEditor()
    {
        if (loadingAttachmentEditor || editedAttachmentRow is null || editedAttachmentRow.RowState == DataRowState.Deleted)
        {
            return;
        }

        editedAttachmentRow["TipoDocumento"] = GetLookupString(lueAttachmentType) ?? string.Empty;
        editedAttachmentRow["Categoria"] = GetLookupString(lueAttachmentCategory) ?? string.Empty;
        editedAttachmentRow["NombreArchivo"] = txtAttachmentFileName.Text.Trim();
        editedAttachmentRow["Descripcion"] = memAttachmentDescription.Text.Trim();
        editedAttachmentRow["ReferenciaDocumental"] = txtAttachmentReference.Text.Trim();
        editedAttachmentRow["Estado"] = GetLookupString(lueAttachmentStatus) ?? "Activo";
        if (chkAttachmentPrincipal.IsOn)
        {
            ClearMainAttachment(editedAttachmentRow);
        }
        editedAttachmentRow["Principal"] = chkAttachmentPrincipal.IsOn;
        editedAttachmentRow["VisibleVentas"] = chkVisibleInSales.IsOn;
        editedAttachmentRow["VisibleCompras"] = chkVisibleInPurchases.IsOn;
        editedAttachmentRow["VisiblePortal"] = chkVisibleInPortal.IsOn;
        editedAttachmentRow["Confidencial"] = chkAttachmentConfidential.IsOn;
        editedAttachmentRow["OrdenVisual"] = decimal.ToInt32(spnAttachmentOrder.Value);
        editedAttachmentRow["VigenciaDesde"] = dteAttachmentValidFrom.EditValue ?? DBNull.Value;
        editedAttachmentRow["VigenciaHasta"] = dteAttachmentValidTo.EditValue ?? DBNull.Value;
        editedAttachmentRow["TextoAlternativo"] = memAttachmentAlternativeText.Text.Trim();
    }

    private void DocumentSectionsHandleCreated(object? sender, EventArgs e)
    {
        if (documentTabIcons.Count == 0)
        {
            documentTabIcons[tabAttachments] = new ItemTabIconPair(
                LoadItemTabIcon("documentos_imagenes_20.svg"),
                LoadItemTabIcon("documentos_imagenes_active_20.svg"));
            documentTabIcons[tabRemarks] = new ItemTabIconPair(
                LoadItemTabIcon("documentos_observaciones_20.svg"),
                LoadItemTabIcon("documentos_observaciones_active_20.svg"));
        }

        ApplyDocumentTabVisualState();
    }

    private void DocumentSectionsSelectedPageChanged(object? sender, TabPageChangedEventArgs e)
    {
        ApplyDocumentTabVisualState();
    }

    private void DocumentSectionsCustomDrawTabHeader(object? sender, TabHeaderCustomDrawEventArgs e)
    {
        e.DefaultDraw();
        if (ReferenceEquals(e.TabHeaderInfo.Page, tabDocumentSections.SelectedTabPage))
        {
            using var brush = new SolidBrush(BrandResources.Primary);
            e.Graphics.FillRectangle(brush, e.Bounds.Left + 8, e.Bounds.Bottom - 3, Math.Max(1, e.Bounds.Width - 16), 3);
        }
        e.Handled = true;
    }

    private void ApplyDocumentTabVisualState()
    {
        foreach (var entry in documentTabIcons)
        {
            entry.Key.ImageOptions.SvgImage = ReferenceEquals(entry.Key, tabDocumentSections.SelectedTabPage)
                ? entry.Value.Active
                : entry.Value.Inactive;
            entry.Key.ImageOptions.SvgImageSize = new Size(20, 20);
        }
        tabDocumentSections.Invalidate();
    }
}
