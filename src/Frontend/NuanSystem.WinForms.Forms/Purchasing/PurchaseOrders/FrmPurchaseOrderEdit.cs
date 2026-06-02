using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders.Models;

namespace NuanSystem.WinForms.Forms.Purchasing.PurchaseOrders;

public sealed partial class FrmPurchaseOrderEdit : BaseEditForm
{
    private readonly PurchaseOrderDetail? order;
    private readonly PurchaseOrderLookups lookups;
    private readonly bool useReferencePreviewData;
    private readonly BindingList<PurchaseOrderLineItem> lines = new();
    private readonly BindingList<PurchaseOrderApprovalItem> approvals = new();
    private readonly BindingList<PurchaseOrderApprovalFlowItem> approvalFlow = new();
    private readonly BindingList<PurchaseOrderRelatedDocumentItem> relatedDocuments = new();
    private readonly BindingList<PurchaseOrderAttachmentItem> attachments = new();
    private readonly BindingList<PurchaseOrderSapSyncLogItem> sapLogs = new();

    public FrmPurchaseOrderEdit()
        : this(null, CreateDesignLookups(), true)
    {
    }

    public FrmPurchaseOrderEdit(PurchaseOrderDetail? order, PurchaseOrderLookups lookups)
        : this(order, lookups, false)
    {
    }

    private FrmPurchaseOrderEdit(PurchaseOrderDetail? order, PurchaseOrderLookups lookups, bool useReferencePreviewData)
    {
        this.order = order;
        this.lookups = lookups;
        this.useReferencePreviewData = useReferencePreviewData;
        InitializeComponent();
        BindData();
        WireEvents();
        LoadOrder();
        RefreshAttachmentPreview();
        RefreshTotals();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SavePurchaseOrderRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        return Validator.RequireText(slueSupplier, "Proveedor es obligatorio.")
            & RequireValue(lueCurrency, "Moneda es obligatoria.")
            & RequireValue(luePaymentTerm, "Condición de pago es obligatoria.")
            & RequireValue(lueBuyer, "Comprador es obligatorio.")
            & RequireValue(lueMainWarehouse, "Bodega principal es obligatoria.")
            & ValidateLines();
    }

    protected override void BuildRequest()
    {
        CommitLines();
        RefreshTotals();
        var supplier = FindLookup(lookups.Suppliers, ToNullableInt(slueSupplier.EditValue));

        Request = new SavePurchaseOrderRequest(
            null,
            ToNullableInt(FirstActive(lookups.DocumentSeries)?.Id),
            FirstActive(lookups.DocumentSeries)?.Code ?? lblSeriesValue.Text,
            lblNumberValue.Text,
            ToNullableInt(slueSupplier.EditValue) ?? 0,
            supplier?.Code ?? string.Empty,
            supplier?.Name ?? slueSupplier.Text,
            NullIfEmpty(txtSupplierTaxId.Text),
            NullIfEmpty(txtSupplierContact.Text),
            NullIfEmpty(txtSupplierPhone.Text),
            NullIfEmpty(txtSupplierEmail.Text),
            deDocumentDate.DateTime.Date,
            deDeliveryDate.DateTime.Date,
            FindLookup(lookups.Currencies, ToNullableInt(lueCurrency.EditValue))?.Code ?? lueCurrency.Text,
            1m,
            ToNullableInt(luePaymentTerm.EditValue),
            ToNullableInt(luePriceList.EditValue),
            ToNullableInt(lueBuyer.EditValue),
            ToNullableInt(lueMainWarehouse.EditValue),
            ToNullableInt(lueProject.EditValue),
            ToNullableInt(lueCostCenter.EditValue),
            ToNullableInt(luePurchaseType.EditValue),
            NullIfEmpty(memoComments.Text),
            spnGlobalDiscountPercent.Value,
            lines.Where(IsPersistableLine).ToArray(),
            BuildAddresses(),
            relatedDocuments.ToArray(),
            attachments.ToArray());
    }

    private void BindData()
    {
        slueSupplier.Properties.DataSource = lookups.Suppliers;
        lueCurrency.Properties.DataSource = lookups.Currencies;
        luePaymentTerm.Properties.DataSource = lookups.PaymentTerms;
        luePriceList.Properties.DataSource = lookups.PriceLists;
        lueBuyer.Properties.DataSource = lookups.Buyers;
        lueMainWarehouse.Properties.DataSource = lookups.Warehouses;
        lueProject.Properties.DataSource = lookups.Projects;
        lueCostCenter.Properties.DataSource = lookups.CostCenters;
        luePurchaseType.Properties.DataSource = lookups.PurchaseTypes;
        lueDeliveryAddress.Properties.DataSource = Array.Empty<PurchaseOrderLookupOption>();
        lueBillingAddress.Properties.DataSource = Array.Empty<PurchaseOrderLookupOption>();
        repoItem.DataSource = lookups.Items;
        repoUnit.DataSource = lookups.Units;
        repoTax.DataSource = lookups.Taxes;
        repoWarehouse.DataSource = lookups.Warehouses;
        repoCostCenter.DataSource = lookups.CostCenters;
        repoProject.DataSource = lookups.Projects;
        gridLines.DataSource = lines;
        gridApprovals.DataSource = approvals;
        gridApprovalFlow.DataSource = approvalFlow;
        gridRelatedDocuments.DataSource = relatedDocuments;
        gridAttachments.DataSource = attachments;
        gridSapLogs.DataSource = sapLogs;
    }

    private void WireEvents()
    {
        btnSave.Click += (_, _) => Save();
        btnCancel.Click += (_, _) => Close();
        slueSupplier.EditValueChanged += (_, _) => ApplySupplierDefaults();
        spnGlobalDiscountPercent.EditValueChanged += (_, _) => RefreshTotals();
        viewLines.CellValueChanged += (_, _) => RefreshCurrentLine();
        viewLines.RowUpdated += (_, _) => RefreshTotals();
        viewAttachments.FocusedRowChanged += (_, _) => RefreshAttachmentPreview();
        viewLines.InitNewRow += (_, e) =>
        {
            if (viewLines.GetRow(e.RowHandle) is not PurchaseOrderLineItem line)
            {
                return;
            }

            line.LineNumber = lines.Count;
            line.DeliveryDate = deDeliveryDate.DateTime == default ? DateTime.Today : deDeliveryDate.DateTime.Date;
            line.WarehouseId = ToNullableInt(lueMainWarehouse.EditValue) ?? 0;
        };
        btnSyncSap.Click += (_, _) => XtraMessageBox.Show(this, "La sincronizacion SAP se ejecuta desde backend.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        btnRefreshSapStatus.Click += (_, _) => XtraMessageBox.Show(this, "Estado SAP actualizado desde la orden cargada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        btnCancelSapSync.Enabled = false;
        viewRelatedDocuments.RowCellStyle += ApplyRelatedDocumentStatusStyle;
        ApplyRelatedDocumentButtonIcons();
        RefreshAttachmentPreview();
    }

    private void LoadOrder()
    {
        deDocumentDate.DateTime = order?.DocumentDate == default ? DateTime.Today : order?.DocumentDate ?? DateTime.Today;
        deDeliveryDate.DateTime = order?.DeliveryDate == default ? DateTime.Today : order?.DeliveryDate ?? DateTime.Today;
        lblSeriesValue.Text = order?.SeriesCode ?? FirstActive(lookups.DocumentSeries)?.Code ?? "OC-2026";
        lblNumberValue.Text = order?.DocumentNumber ?? "OC-000001";
        lblDocumentNumber.Text = lblNumberValue.Text;
        lblStatus.Text = DisplayStatus(order?.Status ?? "Draft").ToUpperInvariant();
        txtSapStatus.Text = DisplaySapStatus(order?.SapStatus ?? "Pending");
        txtSapObjectType.Text = "22 (Purchase Order)";
        txtSapSyncDocEntry.Text = order?.SapDocEntry?.ToString() ?? "0";
        txtSapSyncDocNum.Text = order?.SapDocNum?.ToString() ?? "0";
        txtSapDocEntry.Text = order?.SapDocEntry?.ToString() ?? "0";
        txtSapDocNum.Text = order?.SapDocNum?.ToString() ?? "0";
        txtSapCurrency.Text = order?.CurrencyCode ?? "USD";
        txtSapTotal.Text = order?.TotalAmount.ToString("N2") ?? "0.00";
        txtSapSyncDate.Text = order?.SapSyncDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        txtSapUser.Text = "-";
        txtSapLastError.Text = order?.SapMessage ?? "-";
        memoSapMessage.Text = string.IsNullOrWhiteSpace(order?.SapMessage) ? "Aun no se ha realizado ninguna sincronizacion con SAP." : order.SapMessage;

        if (order is null)
        {
            SetDefaultLookups();
            EnsureAddressDefaults();
            if (useReferencePreviewData)
            {
                LoadReferencePreviewData();
            }

            return;
        }

        slueSupplier.EditValue = order.SupplierId;
        txtSupplierTaxId.Text = order.SupplierTaxId;
        txtSupplierContact.Text = order.ContactName;
        txtSupplierPhone.Text = order.Phone;
        txtSupplierEmail.Text = order.Email;
        lueCurrency.EditValue = lookups.Currencies.FirstOrDefault(item => item.Code == order.CurrencyCode)?.Id;
        luePaymentTerm.EditValue = order.PaymentTermId;
        luePriceList.EditValue = order.PriceListId;
        lueBuyer.EditValue = order.BuyerId;
        lueMainWarehouse.EditValue = order.MainWarehouseId;
        lueProject.EditValue = order.ProjectId;
        lueCostCenter.EditValue = order.CostCenterId;
        luePurchaseType.EditValue = order.PurchaseTypeId;
        memoComments.Text = order.Comments;
        spnGlobalDiscountPercent.Value = order.DiscountPercent;

        foreach (var line in order.Lines.OrderBy(item => item.LineNumber))
        {
            lines.Add(line);
        }

        foreach (var approval in order.Approvals)
        {
            approvals.Add(approval);
        }

        foreach (var approval in order.Approvals.OrderBy(item => item.ApprovalLevel))
        {
            approvalFlow.Add(new PurchaseOrderApprovalFlowItem
            {
                Step = approval.ApprovalLevel,
                Role = approval.RoleName ?? string.Empty,
                User = approval.UserName ?? string.Empty,
                Status = DisplayStatus(approval.Status),
                DateText = approval.RespondedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-"
            });
        }

        foreach (var document in order.RelatedDocuments)
        {
            relatedDocuments.Add(document);
        }

        foreach (var attachment in order.Attachments)
        {
            attachments.Add(attachment);
        }

        foreach (var log in order.SapLogs)
        {
            sapLogs.Add(log);
        }

        LoadAddresses(order.Addresses);
    }

    private void SetDefaultLookups()
    {
        slueSupplier.EditValue = FirstActive(lookups.Suppliers)?.Id;
        lueCurrency.EditValue = FirstActive(lookups.Currencies)?.Id;
        luePaymentTerm.EditValue = FirstActive(lookups.PaymentTerms)?.Id;
        luePriceList.EditValue = FirstActive(lookups.PriceLists)?.Id;
        lueBuyer.EditValue = FirstActive(lookups.Buyers)?.Id;
        lueMainWarehouse.EditValue = FirstActive(lookups.Warehouses)?.Id;
        lueProject.EditValue = FirstActive(lookups.Projects)?.Id;
        lueCostCenter.EditValue = FirstActive(lookups.CostCenters)?.Id;
        luePurchaseType.EditValue = FirstActive(lookups.PurchaseTypes)?.Id;
    }

    private void LoadReferencePreviewData()
    {
        txtSupplierTaxId.Text = "890.123.456-7";
        txtSupplierContact.Text = "María Fernanda Gómez";
        txtSupplierPhone.Text = "(02) 299 4500";
        txtSupplierEmail.Text = "mfgomez@suministros.com";
        deDocumentDate.DateTime = new DateTime(2025, 5, 16);
        deDeliveryDate.DateTime = new DateTime(2025, 5, 30);
        memoComments.Text = "Compra de materiales para fase 2 del proyecto.";
        txtDeliveryAddressName.Text = "Bodega Central";
        memoDeliveryStreet.Text = "Av. de los Industriales 1234";
        txtDeliveryReference.Text = "Frente al Parque Industrial";
        txtDeliveryCity.Text = "Quito";
        txtDeliveryState.Text = "Pichincha";
        txtDeliveryZipCode.Text = "170135";
        txtDeliveryCountry.Text = "Ecuador";
        txtDeliveryPhone.Text = "(02) 299 4500";
        txtBillingAddressName.Text = "Oficina Matriz";
        memoBillingStreet.Text = "Calle 10 de Agosto N35-45 y Naciones Unidas";
        txtBillingReference.Text = "Edificio Suministros Industriales";
        txtBillingCity.Text = "Quito";
        txtBillingState.Text = "Pichincha";
        txtBillingZipCode.Text = "170507";
        txtBillingCountry.Text = "Ecuador";
        txtBillingPhone.Text = "(02) 299 4500";
        txtBillingEmail.Text = "facturacion@suministros.com";
        txtApprovalPolicy.Text = "COM-ORD-001";
        txtApprovalLevel.Text = "2 de 3";
        approvalFlow.Add(new PurchaseOrderApprovalFlowItem { Step = 1, Role = "Solicitante", User = "Juan Pérez", Status = "Completado", DateText = "16/05/2025 09:10" });
        approvalFlow.Add(new PurchaseOrderApprovalFlowItem { Step = 2, Role = "Aprobador Nivel 1", User = "Carla Méndez", Status = "Aprobado", DateText = "16/05/2025 10:02" });
        approvalFlow.Add(new PurchaseOrderApprovalFlowItem { Step = 3, Role = "Aprobador Nivel 2", User = "Carlos Ramírez", Status = "Aprobado", DateText = "16/05/2025 11:20" });
        approvalFlow.Add(new PurchaseOrderApprovalFlowItem { Step = 4, Role = "Aprobador Nivel 3", User = "Gerente General", Status = "Pendiente", DateText = "-" });
        txtApprovalStatus.Text = "En Proceso";
        memoApprovalObservation.Text = "Pendiente aprobación del Gerente General.";

        lines.Add(new PurchaseOrderLineItem { LineNumber = 1, ItemId = 1, ItemCode = "MAT-0001", ItemName = "Tubería de Acero Inox. 304 Ø 2\" SCH 10", UnitId = 1, UnitCode = "MTR", Quantity = 100m, OpenQuantity = 100m, UnitPrice = 12.50m, TaxId = 1, TaxCode = "IVA 12%", TaxRate = 0.12m, WarehouseId = 1, WarehouseCode = "BODEGA CENTRAL", DeliveryDate = new DateTime(2025, 5, 30), CostCenterId = 1, ProjectId = 1 });
        lines.Add(new PurchaseOrderLineItem { LineNumber = 2, ItemId = 2, ItemCode = "MAT-0002", ItemName = "Válvula de Bola 2\" Acero Inox. 304", UnitId = 2, UnitCode = "UND", Quantity = 10m, OpenQuantity = 10m, UnitPrice = 85.00m, TaxId = 1, TaxCode = "IVA 12%", TaxRate = 0.12m, WarehouseId = 1, WarehouseCode = "BODEGA CENTRAL", DeliveryDate = new DateTime(2025, 5, 30), CostCenterId = 1, ProjectId = 1 });
        lines.Add(new PurchaseOrderLineItem { LineNumber = 3, ItemId = 3, ItemCode = "MAT-0003", ItemName = "Brida Slip-On 2\" 150# Acero Inox. 304", UnitId = 2, UnitCode = "UND", Quantity = 20m, OpenQuantity = 20m, UnitPrice = 8.75m, TaxId = 1, TaxCode = "IVA 12%", TaxRate = 0.12m, WarehouseId = 1, WarehouseCode = "BODEGA CENTRAL", DeliveryDate = new DateTime(2025, 5, 30), CostCenterId = 1, ProjectId = 1 });
        lines.Add(new PurchaseOrderLineItem { LineNumber = 4, ItemId = 4, ItemCode = "MAT-0004", ItemName = "Perno Hex. 1/2\" x 2\" Acero Inox. 304", UnitId = 2, UnitCode = "UND", Quantity = 200m, OpenQuantity = 200m, UnitPrice = 0.35m, TaxId = 1, TaxCode = "IVA 12%", TaxRate = 0.12m, WarehouseId = 1, WarehouseCode = "BODEGA CENTRAL", DeliveryDate = new DateTime(2025, 5, 30), CostCenterId = 1, ProjectId = 1 });
        lines.Add(new PurchaseOrderLineItem { LineNumber = 5, ItemId = 5, ItemCode = "MAT-0005", ItemName = "Empaque de PTFE 2\"", UnitId = 2, UnitCode = "UND", Quantity = 50m, OpenQuantity = 50m, UnitPrice = 1.20m, TaxId = 1, TaxCode = "IVA 12%", TaxRate = 0.12m, WarehouseId = 1, WarehouseCode = "BODEGA CENTRAL", DeliveryDate = new DateTime(2025, 5, 30), CostCenterId = 1, ProjectId = 1 });
        lines.Add(new PurchaseOrderLineItem { LineNumber = 6, ItemId = 6, ItemCode = "SERV-0001", ItemName = "Transporte y Flete", UnitId = 3, UnitCode = "SERV", Quantity = 1m, OpenQuantity = 1m, UnitPrice = 120.00m, TaxId = 1, TaxCode = "IVA 12%", TaxRate = 0.12m, WarehouseId = 1, WarehouseCode = "BODEGA CENTRAL", DeliveryDate = new DateTime(2025, 5, 30), CostCenterId = 1, ProjectId = 1 });
        approvals.Add(new PurchaseOrderApprovalItem { ApprovalLevel = 1, RoleName = "Aprobador Nivel 1", UserName = "Carla Méndez", RequestedAt = new DateTime(2025, 5, 16, 9, 15, 0), RespondedAt = new DateTime(2025, 5, 16, 10, 2, 0), Status = "Aprobado", Observation = "Revisado y aprobado." });
        approvals.Add(new PurchaseOrderApprovalItem { ApprovalLevel = 2, RoleName = "Aprobador Nivel 2", UserName = "Carlos Ramírez", RequestedAt = new DateTime(2025, 5, 16, 10, 2, 0), RespondedAt = new DateTime(2025, 5, 16, 11, 20, 0), Status = "Aprobado", Observation = "Aceptado." });
        approvals.Add(new PurchaseOrderApprovalItem { ApprovalLevel = 3, RoleName = "Aprobador Nivel 3", UserName = "Gerente General", RequestedAt = new DateTime(2025, 5, 16, 11, 20, 0), Status = "Pendiente", Observation = "Pendiente de aprobación." });
        attachments.Add(new PurchaseOrderAttachmentItem { FileName = "cotizacion_suministros_16052025.pdf", OriginalFileName = "Cotizacion_Suministros_16052025.pdf", FileExtension = ".pdf", MimeType = "PDF Document", FileSize = 1240 * 1024, Status = "Activo", Comment = "Cotización recibida del proveedor.", CreatedByUserName = "Juan Pérez", CreatedAt = new DateTime(2025, 5, 16, 10, 15, 0) });
        attachments.Add(new PurchaseOrderAttachmentItem { FileName = "detalle_tecnico_materiales.xlsx", OriginalFileName = "Detalle_Tecnico_Materiales.xlsx", FileExtension = ".xlsx", MimeType = "Microsoft Excel", FileSize = 512 * 1024, Status = "Activo", Comment = "Especificaciones técnicas.", CreatedByUserName = "Juan Pérez", CreatedAt = new DateTime(2025, 5, 16, 10, 22, 0) });
        attachments.Add(new PurchaseOrderAttachmentItem { FileName = "terminos_condiciones.docx", OriginalFileName = "Terminos_y_Condiciones.docx", FileExtension = ".docx", MimeType = "Microsoft Word", FileSize = 243 * 1024, Status = "Activo", Comment = "Términos acordados.", CreatedByUserName = "María Gómez", CreatedAt = new DateTime(2025, 5, 16, 10, 28, 0) });
        attachments.Add(new PurchaseOrderAttachmentItem { FileName = "plano_layout_nueva_planta.pdf", OriginalFileName = "Plano_Layout_Nueva_Planta.pdf", FileExtension = ".pdf", MimeType = "PDF Document", FileSize = 2850 * 1024, Status = "Activo", Comment = "Plano de referencia general.", CreatedByUserName = "Juan Pérez", CreatedAt = new DateTime(2025, 5, 16, 10, 35, 0) });
        attachments.Add(new PurchaseOrderAttachmentItem { FileName = "certificaciones_proveedor.zip", OriginalFileName = "Certificaciones_Proveedor.zip", FileExtension = ".zip", MimeType = "Compressed Folder", FileSize = 3120 * 1024, Status = "Activo", Comment = "Certificaciones vigentes.", CreatedByUserName = "María Gómez", CreatedAt = new DateTime(2025, 5, 16, 10, 40, 0) });
        attachments.Add(new PurchaseOrderAttachmentItem { FileName = "politica_calidad_proveedor.pdf", OriginalFileName = "Politica_Calidad_Proveedor.pdf", FileExtension = ".pdf", MimeType = "PDF Document", FileSize = 798 * 1024, Status = "Activo", Comment = "Política de calidad del proveedor.", CreatedByUserName = "María Gómez", CreatedAt = new DateTime(2025, 5, 16, 10, 47, 0) });
        attachments.Add(new PurchaseOrderAttachmentItem { FileName = "historial_entregas_proveedor.xlsx", OriginalFileName = "Historial_Entregas_Proveedor.xlsx", FileExtension = ".xlsx", MimeType = "Microsoft Excel", FileSize = 356 * 1024, Status = "Activo", Comment = "Historial de entregas 2024-2025.", CreatedByUserName = "Juan Pérez", CreatedAt = new DateTime(2025, 5, 16, 10, 55, 0) });
        sapLogs.Add(new PurchaseOrderSapSyncLogItem { CreatedAt = new DateTime(2025, 5, 16, 9, 15, 23), Process = "Validación", Status = "Exitoso", Message = "Validación de datos correcta. La orden está lista para ser enviada.", UserName = "Juan Pérez", AttemptNumber = 1 });
        sapLogs.Add(new PurchaseOrderSapSyncLogItem { CreatedAt = new DateTime(2025, 5, 16, 9, 16, 2), Process = "Envío a SAP", Status = "Pendiente", Message = "La orden ha sido enviada a SAP y está en proceso de creación.", UserName = "Juan Pérez", AttemptNumber = 1 });
        sapLogs.Add(new PurchaseOrderSapSyncLogItem { CreatedAt = new DateTime(2025, 5, 16, 9, 17, 18), Process = "Envío a SAP", Status = "Exitoso", Message = "Orden de compra creada exitosamente en SAP.", UserName = "Juan Pérez", AttemptNumber = 1 });
        sapLogs.Add(new PurchaseOrderSapSyncLogItem { CreatedAt = new DateTime(2025, 5, 16, 9, 20, 45), Process = "Validación", Status = "Fallido", Message = "Error de validación: El centro de costo no existe en SAP.", UserName = "Juan Pérez", AttemptNumber = 2 });
        sapLogs.Add(new PurchaseOrderSapSyncLogItem { CreatedAt = new DateTime(2025, 5, 16, 9, 25, 9), Process = "Envío a SAP", Status = "Exitoso", Message = "Orden de compra creada exitosamente en SAP.", UserName = "Juan Pérez", AttemptNumber = 2 });
        relatedDocuments.Add(new PurchaseOrderRelatedDocumentItem { RelatedDocumentType = "Solicitud de Compra", Series = "SC-2026", Number = "SC-000245", Date = new DateTime(2025, 5, 10), Status = "Aprobada", Reference = "Dep. Mantenimiento", Comment = "Solicitud de tuberia y valvulas para nueva planta.", Total = 2650m });
        relatedDocuments.Add(new PurchaseOrderRelatedDocumentItem { RelatedDocumentType = "Cotizacion Proveedor", Series = "COT-2026", Number = "COT-00321", Date = new DateTime(2025, 5, 12), Status = "Aceptada", Reference = "Oferta valida hasta 20/05/2025", Comment = "Cotizacion con mejores condiciones.", Total = 2675m });
        relatedDocuments.Add(new PurchaseOrderRelatedDocumentItem { RelatedDocumentType = "Orden de Venta Interna", Series = "OVI-2026", Number = "OVI-000112", Date = new DateTime(2025, 5, 13), Status = "Aprobada", Reference = "Venta Interna de Activos", Comment = "Traslado a proyecto Nueva Planta.", Total = 2675m });
        relatedDocuments.Add(new PurchaseOrderRelatedDocumentItem { RelatedDocumentType = "Entrada de Mercancia", Series = "EM-2026", Number = "EM-000178", Date = new DateTime(2025, 5, 30), Status = "Pendiente", Reference = "Recepcion programada", Comment = "Recepcion planificada en almacen.", Total = 2675m });
        relatedDocuments.Add(new PurchaseOrderRelatedDocumentItem { RelatedDocumentType = "Factura Proveedor", Series = "FAC-2026", Number = "FAC-000321", Status = "Pendiente", Reference = "-", Comment = "Factura pendiente de recepcion.", Total = 2675m });
        memoRelatedDocumentNotes.Text = "Ingrese observaciones adicionales relacionadas con los documentos.";
    }

    private void ApplySupplierDefaults()
    {
        if (FindLookup(lookups.Suppliers, ToNullableInt(slueSupplier.EditValue)) is not { } supplier)
        {
            return;
        }

        txtSupplierTaxId.Text = supplier.Code;
        txtSupplierContact.Text = supplier.Name;
        EnsureAddressDefaults();
    }

    private void RefreshAttachmentPreview()
    {
        lblAttachmentFooterCount.Text = $"Total de archivos: {attachments.Count:N0}";
        lblAttachmentFooterSize.Text = $"Tamaño total: {FormatFileSize(attachments.Sum(item => item.FileSize))}";

        PurchaseOrderAttachmentItem? attachment = viewAttachments.GetFocusedRow() as PurchaseOrderAttachmentItem;
        if (attachment is null && attachments.Count > 0)
        {
            attachment = attachments[0];
        }

        if (attachment is null)
        {
            lblAttachmentPreviewTitle.Text = "Archivo seleccionado";
            lblAttachmentTypeValue.Text = "-";
            lblAttachmentSizeValue.Text = "-";
            lblAttachmentDateValue.Text = "-";
            lblAttachmentUserValue.Text = "-";
            lblAttachmentStatusValue.Text = "-";
            lblAttachmentCommentValue.Text = "-";
            return;
        }

        lblAttachmentPreviewTitle.Text = attachment.OriginalFileName;
        lblAttachmentTypeValue.Text = string.IsNullOrWhiteSpace(attachment.MimeType) ? attachment.FileExtension ?? "-" : attachment.MimeType;
        lblAttachmentSizeValue.Text = FormatFileSize(attachment.FileSize);
        lblAttachmentDateValue.Text = attachment.CreatedAt == default ? "-" : attachment.CreatedAt.ToString("dd/MM/yyyy HH:mm");
        lblAttachmentUserValue.Text = string.IsNullOrWhiteSpace(attachment.CreatedByUserName) ? "-" : attachment.CreatedByUserName;
        lblAttachmentStatusValue.Text = attachment.Status;
        lblAttachmentCommentValue.Text = string.IsNullOrWhiteSpace(attachment.Comment) ? "-" : attachment.Comment;
    }

    private void RefreshCurrentLine()
    {
        CommitLines();
        if (viewLines.GetFocusedRow() is PurchaseOrderLineItem line)
        {
            ApplyLineLookups(line);
            CalculateLine(line);
            viewLines.RefreshRow(viewLines.FocusedRowHandle);
        }

        RefreshTotals();
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes >= 1024L * 1024L)
        {
            return $"{bytes / 1024m / 1024m:N2} MB";
        }

        return $"{bytes / 1024m:N0} KB";
    }

    private void RefreshTotals()
    {
        CommitLines();
        foreach (var line in lines)
        {
            ApplyLineLookups(line);
            CalculateLine(line);
        }

        var validLines = lines.Where(IsPersistableLine).ToArray();
        var subtotal = validLines.Sum(item => item.LineSubtotal);
        var discountPercent = Math.Clamp(spnGlobalDiscountPercent.Value, 0, 100);
        var discount = Math.Round(subtotal * discountPercent / 100m, 6);
        var taxableBase = Math.Max(0, subtotal - discount);
        var taxRatio = subtotal == 0 ? 0 : taxableBase / subtotal;
        var tax = Math.Round(validLines.Sum(item => item.TaxAmount * taxRatio), 6);
        var total = taxableBase + tax;
        SetMoney(lblDetailSubtotal, subtotal);
        SetMoney(lblDetailDiscount, discount);
        SetMoney(lblDetailBase, taxableBase);
        SetMoney(lblDetailTax, tax);
        SetMoney(lblDetailTotal, total);
        SetMoney(lblSummarySubtotal, subtotal);
        SetMoney(lblSummaryDiscount, discount);
        SetMoney(lblSummaryBase, taxableBase);
        SetMoney(lblSummaryTax, tax);
        SetMoney(lblSummaryTotal, total);
        lblSummaryItems.Text = validLines.Length.ToString("N0");
        lblSummaryQuantity.Text = validLines.Sum(item => item.Quantity).ToString("N2");
        lblSummaryWeight.Text = "0.00";
        txtApprovalAmount.Text = total.ToString("N2");
        txtSapTotal.Text = total.ToString("N2");
        if (useReferencePreviewData && order is null)
        {
            ApplyReferencePreviewTotals();
        }

        viewLines.RefreshData();
    }

    private void ApplyReferencePreviewTotals()
    {
        var previewLineTotals = new[] { 1400m, 850m, 175m, 70m, 60m, 120m };
        for (var index = 0; index < lines.Count && index < previewLineTotals.Length; index++)
        {
            lines[index].LineTotal = previewLineTotals[index];
        }

        SetMoney(lblDetailSubtotal, 2675m);
        SetMoney(lblDetailDiscount, 0m);
        SetMoney(lblDetailBase, 2675m);
        SetMoney(lblDetailTax, 321m);
        SetMoney(lblDetailTotal, 2996m);
        SetMoney(lblSummarySubtotal, 2675m);
        SetMoney(lblSummaryDiscount, 0m);
        SetMoney(lblSummaryBase, 2675m);
        SetMoney(lblSummaryTax, 321m);
        SetMoney(lblSummaryTotal, 2996m);
        lblSummaryItems.Text = "6";
        lblSummaryQuantity.Text = "381.00";
        lblSummaryWeight.Text = "738.00";
        txtApprovalAmount.Text = "2,996.00";
        txtSapTotal.Text = "2,996.00";
    }

    private void ApplyRelatedDocumentButtonIcons()
    {
        SetButtonIcon(btnAddRelatedDocument, "agregar_16.svg", Color.FromArgb(16, 185, 80));
        SetButtonIcon(btnViewRelatedDocument, "busqueda_filtro_16.svg", Color.FromArgb(0, 92, 255));
        SetButtonIcon(btnUnlinkRelatedDocument, "quitar_16.svg", Color.FromArgb(230, 35, 45));
        SetButtonIcon(btnRefreshRelatedDocuments, "actualizar_16.svg", Color.FromArgb(0, 92, 255));
    }

    private static void SetButtonIcon(SimpleButton button, string iconName, Color color)
    {
        var icon = OperationButtonIcons.LoadOperationIcon(iconName, color);
        if (icon is null)
        {
            return;
        }

        button.ImageOptions.SvgImage = icon;
        button.ImageOptions.SvgImageSize = new Size(16, 16);
    }

    private void ApplyRelatedDocumentStatusStyle(object? sender, RowCellStyleEventArgs e)
    {
        if (e.Column != colRelatedDocumentStatus || viewRelatedDocuments.GetRow(e.RowHandle) is not PurchaseOrderRelatedDocumentItem document)
        {
            return;
        }

        e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        e.Appearance.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);

        switch (document.Status)
        {
            case "Aprobada":
            case "Aceptada":
            case "Recibida":
                e.Appearance.BackColor = Color.FromArgb(220, 248, 228);
                e.Appearance.ForeColor = Color.FromArgb(10, 125, 55);
                break;
            case "Confirmada":
                e.Appearance.BackColor = Color.FromArgb(220, 236, 255);
                e.Appearance.ForeColor = Color.FromArgb(0, 92, 190);
                break;
            case "Pendiente":
                e.Appearance.BackColor = Color.FromArgb(255, 241, 214);
                e.Appearance.ForeColor = Color.FromArgb(184, 103, 0);
                break;
        }
    }

    private void ApplyLineLookups(PurchaseOrderLineItem line)
    {
        if (FindLookup(lookups.Items, line.ItemId) is { } item)
        {
            line.ItemCode = item.Code;
            if (string.IsNullOrWhiteSpace(line.ItemName))
            {
                line.ItemName = item.Name;
            }
        }

        if (FindLookup(lookups.Units, line.UnitId) is { } unit)
        {
            line.UnitCode = unit.Code;
        }

        if (lookups.Taxes.FirstOrDefault(item => item.Id == line.TaxId) is { } tax)
        {
            line.TaxCode = tax.Code;
            line.TaxRate = tax.Rate;
        }

        if (lookups.Warehouses.FirstOrDefault(item => item.Id == line.WarehouseId) is { } warehouse)
        {
            line.WarehouseCode = warehouse.Code;
        }
    }

    private static void CalculateLine(PurchaseOrderLineItem line)
    {
        line.Quantity = Math.Max(0, line.Quantity);
        line.OpenQuantity = line.Quantity;
        line.UnitPrice = Math.Max(0, line.UnitPrice);
        line.DiscountPercent = Math.Clamp(line.DiscountPercent, 0, 100);
        var gross = line.Quantity * line.UnitPrice;
        line.DiscountAmount = Math.Round(gross * line.DiscountPercent / 100m, 6);
        line.LineSubtotal = Math.Max(0, gross - line.DiscountAmount);
        line.TaxAmount = Math.Round(line.LineSubtotal * Math.Max(0, line.TaxRate), 6);
        line.LineTotal = line.LineSubtotal + line.TaxAmount;
    }

    private bool ValidateLines()
    {
        CommitLines();
        var validLines = lines.Where(IsPersistableLine).ToArray();
        if (validLines.Length == 0)
        {
            ShowWarning("Debe registrar al menos una linea valida.");
            return false;
        }

        foreach (var line in validLines)
        {
            if (line.Quantity <= 0 || line.UnitPrice < 0 || line.UnitId is null || line.TaxId is null || line.WarehouseId <= 0 || line.DeliveryDate == default)
            {
                ShowWarning("Revise articulo, unidad, cantidad, impuesto, bodega y fecha de entrega en el detalle.");
                return false;
            }
        }

        return true;
    }

    private IReadOnlyCollection<PurchaseOrderAddressItem> BuildAddresses()
    {
        return new[]
        {
            new PurchaseOrderAddressItem
            {
                AddressType = "Delivery",
                AddressName = NullIfEmpty(txtDeliveryAddressName.Text),
                Street = NullIfEmpty(memoDeliveryStreet.Text),
                Reference = NullIfEmpty(txtDeliveryReference.Text),
                City = NullIfEmpty(txtDeliveryCity.Text),
                State = NullIfEmpty(txtDeliveryState.Text),
                ZipCode = NullIfEmpty(txtDeliveryZipCode.Text),
                Country = NullIfEmpty(txtDeliveryCountry.Text),
                Phone = NullIfEmpty(txtDeliveryPhone.Text),
                IsModified = true
            },
            new PurchaseOrderAddressItem
            {
                AddressType = "Billing",
                AddressName = NullIfEmpty(txtBillingAddressName.Text),
                Street = NullIfEmpty(memoBillingStreet.Text),
                Reference = NullIfEmpty(txtBillingReference.Text),
                City = NullIfEmpty(txtBillingCity.Text),
                State = NullIfEmpty(txtBillingState.Text),
                ZipCode = NullIfEmpty(txtBillingZipCode.Text),
                Country = NullIfEmpty(txtBillingCountry.Text),
                Phone = NullIfEmpty(txtBillingPhone.Text),
                Email = NullIfEmpty(txtBillingEmail.Text),
                IsModified = true
            }
        };
    }

    private void LoadAddresses(IReadOnlyCollection<PurchaseOrderAddressItem> addresses)
    {
        if (addresses.FirstOrDefault(item => item.AddressType == "Delivery") is { } delivery)
        {
            txtDeliveryAddressName.Text = delivery.AddressName;
            memoDeliveryStreet.Text = delivery.Street;
            txtDeliveryReference.Text = delivery.Reference;
            txtDeliveryCity.Text = delivery.City;
            txtDeliveryState.Text = delivery.State;
            txtDeliveryZipCode.Text = delivery.ZipCode;
            txtDeliveryCountry.Text = delivery.Country;
            txtDeliveryPhone.Text = delivery.Phone;
        }

        if (addresses.FirstOrDefault(item => item.AddressType == "Billing") is { } billing)
        {
            txtBillingAddressName.Text = billing.AddressName;
            memoBillingStreet.Text = billing.Street;
            txtBillingReference.Text = billing.Reference;
            txtBillingCity.Text = billing.City;
            txtBillingState.Text = billing.State;
            txtBillingZipCode.Text = billing.ZipCode;
            txtBillingCountry.Text = billing.Country;
            txtBillingPhone.Text = billing.Phone;
            txtBillingEmail.Text = billing.Email;
        }
    }

    private void EnsureAddressDefaults()
    {
        if (!string.IsNullOrWhiteSpace(txtDeliveryAddressName.Text))
        {
            return;
        }

        txtDeliveryAddressName.Text = slueSupplier.Text;
        txtBillingAddressName.Text = slueSupplier.Text;
        txtDeliveryCountry.Text = "Ecuador";
        txtBillingCountry.Text = "Ecuador";
        txtBillingEmail.Text = txtSupplierEmail.Text;
    }

    private void CommitLines()
    {
        viewLines.PostEditor();
        viewLines.UpdateCurrentRow();
    }

    private bool RequireValue(BaseEdit control, string message)
    {
        if (control.EditValue is null || control.EditValue == DBNull.Value)
        {
            Validator.SetError(control, message);
            return false;
        }

        return true;
    }

    private static bool IsPersistableLine(PurchaseOrderLineItem line)
    {
        return line.ItemId > 0 || !string.IsNullOrWhiteSpace(line.ItemCode) || !string.IsNullOrWhiteSpace(line.ItemName);
    }

    private static PurchaseOrderLookupOption? FindLookup(IEnumerable<PurchaseOrderLookupOption> items, int? id)
    {
        return id.HasValue ? items.FirstOrDefault(item => item.Id == id.Value) : null;
    }

    private static PurchaseOrderLookupOption? FirstActive(IEnumerable<PurchaseOrderLookupOption> items)
    {
        return items.FirstOrDefault(item => item.IsActive);
    }

    private static PurchaseOrderWarehouseLookup? FirstActive(IEnumerable<PurchaseOrderWarehouseLookup> items)
    {
        return items.FirstOrDefault(item => item.IsActive);
    }

    private static int? ToNullableInt(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        if (value is int intValue)
        {
            return intValue;
        }

        return int.TryParse(Convert.ToString(value), out var parsed) ? parsed : null;
    }

    private static void SetMoney(LabelControl label, decimal value)
    {
        label.Text = value.ToString("N2");
    }

    private static string? NullIfEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string DisplayStatus(string status)
    {
        return status switch
        {
            "Draft" => "Borrador",
            "PendingApproval" => "En Proceso",
            "Approved" => "Aprobada",
            "Rejected" => "Rechazada",
            "SapPending" => "Pendiente SAP",
            "SapSynced" => "Sincronizada SAP",
            "SapError" => "Error SAP",
            "Closed" => "Cerrada",
            "Cancelled" => "Cancelada",
            _ => status
        };
    }

    private static string DisplaySapStatus(string status)
    {
        return status switch
        {
            "Pending" => "Pendiente de envio",
            "Synced" => "Sincronizado correctamente",
            "Error" => "Error SAP",
            "Cancelled" => "Cancelado",
            _ => status
        };
    }

    private static SavePurchaseOrderRequest EmptyRequest()
    {
        return new SavePurchaseOrderRequest(null, null, string.Empty, string.Empty, 0, string.Empty, string.Empty, null, null, null, null, DateTime.Today, DateTime.Today, "USD", 1m, null, null, null, null, null, null, null, null, 0, [], [], [], []);
    }

    private sealed class PurchaseOrderApprovalFlowItem
    {
        public int Step { get; set; }

        public string Role { get; set; } = string.Empty;

        public string User { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string DateText { get; set; } = string.Empty;
    }

    private static PurchaseOrderLookups CreateDesignLookups()
    {
        return new PurchaseOrderLookups(
            [new PurchaseOrderLookupOption(1, "SUMINISTROS", "SUMINISTROS INDUSTRIALES S.A.")],
            [
                new PurchaseOrderLookupOption(1, "MAT-0001", "Tubería de Acero Inox. 304 Ø 2\" SCH 10"),
                new PurchaseOrderLookupOption(2, "MAT-0002", "Válvula de Bola 2\" Acero Inox. 304"),
                new PurchaseOrderLookupOption(3, "MAT-0003", "Brida Slip-On 2\" 150# Acero Inox. 304"),
                new PurchaseOrderLookupOption(4, "MAT-0004", "Perno Hex. 1/2\" x 2\" Acero Inox. 304"),
                new PurchaseOrderLookupOption(5, "MAT-0005", "Empaque de PTFE 2\""),
                new PurchaseOrderLookupOption(6, "SERV-0001", "Transporte y Flete")
            ],
            [
                new PurchaseOrderLookupOption(1, "MTR", "Metro"),
                new PurchaseOrderLookupOption(2, "UND", "Unidad"),
                new PurchaseOrderLookupOption(3, "SERV", "Servicio")
            ],
            [new PurchaseOrderWarehouseLookup(1, "BODEGA CENTRAL", "Bodega Central")],
            [new PurchaseOrderTaxLookup(1, "IVA 12%", "IVA 12%", 0.12m)],
            [new PurchaseOrderLookupOption(1, "USD", "Dólares")],
            [new PurchaseOrderLookupOption(1, "30D", "30 Días")],
            [new PurchaseOrderLookupOption(1, "LISTA USD 2025", "Lista USD 2025")],
            [new PurchaseOrderLookupOption(1, "JP", "Juan Pérez")],
            [new PurchaseOrderLookupOption(1, "CC-01", "Administración")],
            [new PurchaseOrderLookupOption(1, "PRY-001", "Nueva Planta")],
            [new PurchaseOrderLookupOption(1, "NAC", "Nacional")],
            [new PurchaseOrderLookupOption(1, "OC-2026", "Órdenes 2026")]);
    }
}
