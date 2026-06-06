/*
    Ejecutar en NuanSystem_Master.
    Seed de seguridad para Orden de Compra:
    - Formularios transaccionales de listado y edicion.
    - Operaciones dinamicas requeridas.
    - Campos minimos reales de FrmPurchaseOrderEdit.
    - Acceso inicial completo para rol ADMIN.
*/

SET NOCOUNT ON;
GO

DECLARE @AdminRoleId int = (SELECT TOP (1) Id FROM dbo.Roles WHERE Code = N'ADMIN' AND IsDeleted = 0);
DECLARE @PurchasingMenuId int;
DECLARE @PurchaseOrdersMenuId int;
DECLARE @ListFormId int;
DECLARE @EditFormId int;

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING' AND IsDeleted = 0)
BEGIN
    INSERT INTO dbo.SecurityMenus
    (
        ParentId, Code, Name, Description, MenuType, FormId, FormKey,
        IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
        CreatedByUserName, CreatedAt
    )
    VALUES
    (
        NULL, N'MENU.PURCHASING', N'Compras', N'Procesos de abastecimiento y compras',
        1, NULL, NULL, N'Accordion/compras_32.svg', N'Accordion/compras_16.svg',
        35, 1, 1, N'Sistema', SYSUTCDATETIME()
    );
END;

UPDATE dbo.SecurityMenus
SET Name = N'Compras',
    Description = N'Procesos de abastecimiento y compras',
    MenuType = 1,
    FormId = NULL,
    FormKey = NULL,
    DisplayOrder = 35,
    IsVisible = 1,
    IsActive = 1,
    IsDeleted = 0,
    DeletedByUserId = NULL,
    DeletedByUserName = NULL,
    DeletedAt = NULL,
    UpdatedByUserName = N'Sistema',
    UpdatedAt = SYSUTCDATETIME()
WHERE Code = N'MENU.PURCHASING';

SET @PurchasingMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING' AND IsDeleted = 0);

SET @ListFormId =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE IsDeleted = 0
      AND (FormKey = N'purchase-orders' OR Code IN (N'FORM.PURCHASING.PURCHASEORDERS', N'FORM.PURCHASING.PURCHASEORDERS.LIST'))
    ORDER BY CASE WHEN FormKey = N'purchase-orders' THEN 0 ELSE 1 END, Id
);

IF @ListFormId IS NULL
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, HasListView, HasEditView,
        IsVisible, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.PURCHASING.PURCHASEORDERS.LIST', N'Ordenes de Compra',
        N'Listado de ordenes de compra.', N'purchase-orders', 2, 1, 0,
        1, 1, N'Sistema', SYSUTCDATETIME()
    );

    SET @ListFormId = CONVERT(int, SCOPE_IDENTITY());
END;
ELSE
BEGIN
    UPDATE dbo.SecurityForms
    SET Code = CASE
            WHEN NOT EXISTS
            (
                SELECT 1
                FROM dbo.SecurityForms other
                WHERE other.Code = N'FORM.PURCHASING.PURCHASEORDERS.LIST'
                  AND other.Id <> @ListFormId
            )
            THEN N'FORM.PURCHASING.PURCHASEORDERS.LIST'
            ELSE Code
        END,
        Name = N'Ordenes de Compra',
        Description = N'Listado de ordenes de compra.',
        FormKey = N'purchase-orders',
        FormType = 2,
        HasListView = 1,
        HasEditView = 0,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @ListFormId;
END;

SET @EditFormId =
(
    SELECT TOP (1) Id
    FROM dbo.SecurityForms
    WHERE IsDeleted = 0
      AND (FormKey = N'purchase-orders-edit' OR Code = N'FORM.PURCHASING.PURCHASEORDERS.EDIT')
    ORDER BY CASE WHEN FormKey = N'purchase-orders-edit' THEN 0 ELSE 1 END, Id
);

IF @EditFormId IS NULL
BEGIN
    INSERT INTO dbo.SecurityForms
    (
        Code, Name, Description, FormKey, FormType, HasListView, HasEditView,
        IsVisible, IsActive, CreatedByUserName, CreatedAt
    )
    VALUES
    (
        N'FORM.PURCHASING.PURCHASEORDERS.EDIT', N'Orden de Compra',
        N'Formulario de edicion de orden de compra.', N'purchase-orders-edit', 2, 0, 1,
        1, 1, N'Sistema', SYSUTCDATETIME()
    );

    SET @EditFormId = CONVERT(int, SCOPE_IDENTITY());
END;
ELSE
BEGIN
    UPDATE dbo.SecurityForms
    SET Code = N'FORM.PURCHASING.PURCHASEORDERS.EDIT',
        Name = N'Orden de Compra',
        Description = N'Formulario de edicion de orden de compra.',
        FormKey = N'purchase-orders-edit',
        FormType = 2,
        HasListView = 0,
        HasEditView = 1,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @EditFormId;
END;

IF @PurchasingMenuId IS NOT NULL AND @ListFormId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING.PURCHASEORDERS' AND IsDeleted = 0)
    BEGIN
        INSERT INTO dbo.SecurityMenus
        (
            ParentId, Code, Name, Description, MenuType, FormId, FormKey,
            IconLarge, IconSmall, DisplayOrder, IsVisible, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @PurchasingMenuId, N'MENU.PURCHASING.PURCHASEORDERS', N'Ordenes de Compra',
            N'Listado de ordenes de compra.', 3, @ListFormId, N'purchase-orders',
            N'Accordion/compras_32.svg', N'Accordion/compras_16.svg',
            10, 1, 1, N'Sistema', SYSUTCDATETIME()
        );
    END;

    UPDATE dbo.SecurityMenus
    SET ParentId = @PurchasingMenuId,
        Name = N'Ordenes de Compra',
        Description = N'Listado de ordenes de compra.',
        MenuType = 3,
        FormId = @ListFormId,
        FormKey = N'purchase-orders',
        DisplayOrder = 10,
        IsVisible = 1,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = N'MENU.PURCHASING.PURCHASEORDERS';
END;

SET @PurchaseOrdersMenuId = (SELECT TOP (1) Id FROM dbo.SecurityMenus WHERE Code = N'MENU.PURCHASING.PURCHASEORDERS' AND IsDeleted = 0);

DECLARE @Operations table
(
    Code nvarchar(80) NOT NULL PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(300) NOT NULL,
    RibbonPageName nvarchar(80) NULL,
    RibbonGroupName nvarchar(80) NULL,
    ActionKey nvarchar(120) NOT NULL,
    DisplayOrder int NOT NULL
);

INSERT INTO @Operations (Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey, DisplayOrder)
VALUES
    (N'ACTION.REFRESH', N'Actualizar', N'Recargar informacion del formulario.', N'Inicio', N'Datos', N'refresh', 10),
    (N'ACTION.CONSULT', N'Consultar', N'Consultar registros del formulario.', N'Inicio', N'Acciones', N'consult', 20),
    (N'ACTION.NEW', N'Nuevo', N'Crear un nuevo registro.', N'Inicio', N'Mantenimiento', N'new', 30),
    (N'ACTION.CREATE', N'Crear', N'Crear registros desde API o procesos.', N'Inicio', N'Mantenimiento', N'create', 31),
    (N'ACTION.EDIT', N'Editar', N'Editar registros existentes.', N'Inicio', N'Mantenimiento', N'edit', 40),
    (N'ACTION.UPDATE', N'Actualizar registro', N'Actualizar registros desde API o procesos.', N'Inicio', N'Mantenimiento', N'update', 41),
    (N'ACTION.DELETE', N'Eliminar', N'Eliminar registros.', N'Inicio', N'Mantenimiento', N'delete', 50),
    (N'ACTION.APPROVE', N'Aprobar', N'Aprobar documentos.', N'Inicio', N'Flujo', N'approve', 60),
    (N'ACTION.REJECT', N'Rechazar', N'Rechazar documentos.', N'Inicio', N'Flujo', N'reject', 61),
    (N'ACTION.SYNC_SAP', N'Sincronizar SAP', N'Sincronizar documentos con SAP Business One.', N'Inicio', N'SAP', N'syncsap', 70),
    (N'ACTION.CANCEL', N'Anular', N'Anular documentos.', N'Inicio', N'Flujo', N'cancel', 80),
    (N'ACTION.REOPEN', N'Reabrir', N'Reabrir documentos.', N'Inicio', N'Flujo', N'reopen', 81),
    (N'ACTION.PRINT', N'Imprimir', N'Imprimir documentos o listados.', N'Inicio', N'Acciones', N'print', 90),
    (N'ACTION.COPY', N'Copiar', N'Copiar o duplicar registros.', N'Inicio', N'Acciones', N'copy', 100),
    (N'ACTION.EXPORT', N'Exportar', N'Exportar informacion.', N'Inicio', N'Datos', N'export', 110),
    (N'ACTION.HISTORY', N'Historial', N'Consultar historial y auditoria.', N'Inicio', N'Seguimiento', N'history', 120);

MERGE dbo.SecurityOperations AS target
USING @Operations AS source
    ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        Name = source.Name,
        Description = source.Description,
        RibbonPageName = source.RibbonPageName,
        RibbonGroupName = source.RibbonGroupName,
        ActionKey = source.ActionKey,
        DisplayOrder = source.DisplayOrder,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        Code, Name, Description, RibbonPageName, RibbonGroupName, ActionKey,
        DisplayOrder, IsActive, CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.Code, source.Name, source.Description, source.RibbonPageName, source.RibbonGroupName, source.ActionKey,
        source.DisplayOrder, 1, N'Sistema', SYSUTCDATETIME(), 0
    );

-- Compatibilidad con pantallas existentes: export-excel/pdf/json/xml siguen existiendo.
-- Para Orden de Compra se agrega ACTION.EXPORT como accion funcional generica sin eliminar exportaciones especificas.

DECLARE @Fields table
(
    Code nvarchar(80) NOT NULL PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    FieldKey nvarchar(120) NOT NULL,
    Description nvarchar(300) NULL,
    ControlType nvarchar(60) NOT NULL,
    DataType nvarchar(40) NOT NULL,
    IsRequired bit NOT NULL,
    IsReadOnly bit NOT NULL,
    IsVisible bit NOT NULL,
    DisplayOrder int NOT NULL
);

INSERT INTO @Fields (Code, Name, FieldKey, Description, ControlType, DataType, IsRequired, IsReadOnly, IsVisible, DisplayOrder)
VALUES
    (N'FIELD.PO.SUPPLIER', N'Proveedor', N'slueSupplier', N'Proveedor de la orden de compra.', N'SearchLookUpEdit', N'int', 1, 0, 1, 10),
    (N'FIELD.PO.SUPPLIER.TAXID', N'RUC o Identificacion proveedor', N'txtSupplierTaxId', N'Identificacion tributaria del proveedor.', N'TextEdit', N'string', 0, 1, 1, 20),
    (N'FIELD.PO.SUPPLIER.CONTACT', N'Contacto proveedor', N'txtSupplierContact', N'Contacto principal del proveedor.', N'TextEdit', N'string', 0, 0, 1, 30),
    (N'FIELD.PO.SUPPLIER.PHONE', N'Telefono', N'txtSupplierPhone', N'Telefono del proveedor.', N'TextEdit', N'string', 0, 0, 1, 40),
    (N'FIELD.PO.SUPPLIER.EMAIL', N'Email', N'txtSupplierEmail', N'Email del proveedor.', N'TextEdit', N'string', 0, 0, 1, 50),
    (N'FIELD.PO.DOCUMENT.DATE', N'Fecha documento', N'deDocumentDate', N'Fecha del documento.', N'DateEdit', N'date', 1, 0, 1, 60),
    (N'FIELD.PO.DELIVERY.DATE', N'Fecha entrega', N'deDeliveryDate', N'Fecha requerida de entrega.', N'DateEdit', N'date', 1, 0, 1, 70),
    (N'FIELD.PO.CURRENCY', N'Moneda', N'lueCurrency', N'Moneda de la orden.', N'LookUpEdit', N'string', 1, 0, 1, 80),
    (N'FIELD.PO.PAYMENT.TERM', N'Condicion de pago', N'luePaymentTerm', N'Condicion de pago.', N'LookUpEdit', N'int', 1, 0, 1, 90),
    (N'FIELD.PO.PRICE.LIST', N'Lista de precios', N'luePriceList', N'Lista de precios de compra.', N'LookUpEdit', N'int', 0, 0, 1, 100),
    (N'FIELD.PO.BUYER', N'Comprador', N'lueBuyer', N'Comprador responsable.', N'LookUpEdit', N'int', 1, 0, 1, 110),
    (N'FIELD.PO.MAIN.WAREHOUSE', N'Bodega principal', N'lueMainWarehouse', N'Bodega principal de recepcion.', N'LookUpEdit', N'int', 1, 0, 1, 120),
    (N'FIELD.PO.PROJECT', N'Proyecto', N'lueProject', N'Proyecto asociado.', N'LookUpEdit', N'int', 0, 0, 1, 130),
    (N'FIELD.PO.COST.CENTER', N'Centro de costo', N'lueCostCenter', N'Centro de costo asociado.', N'LookUpEdit', N'int', 0, 0, 1, 140),
    (N'FIELD.PO.PURCHASE.TYPE', N'Tipo de compra', N'luePurchaseType', N'Tipo de compra.', N'LookUpEdit', N'int', 0, 0, 1, 150),
    (N'FIELD.PO.COMMENTS', N'Comentarios', N'memoComments', N'Comentarios de la orden.', N'MemoEdit', N'string', 0, 0, 1, 160),
    (N'FIELD.PO.SERIES', N'Serie', N'lblSeriesValue', N'Serie asignada.', N'LabelControl', N'string', 1, 1, 1, 170),
    (N'FIELD.PO.NUMBER', N'Numero documento', N'lblNumberValue', N'Numero reservado del documento.', N'LabelControl', N'string', 1, 1, 1, 180),
    (N'FIELD.PO.STATUS', N'Estado', N'lblStatus', N'Estado de la orden.', N'LabelControl', N'string', 0, 1, 1, 190),
    (N'FIELD.PO.LINE.ITEMCODE', N'Codigo de item', N'colLineItemCode', N'Codigo del item en detalle.', N'GridColumn', N'int', 1, 0, 1, 300),
    (N'FIELD.PO.LINE.DESCRIPTION', N'Descripcion', N'colLineDescription', N'Descripcion del item.', N'GridColumn', N'string', 1, 0, 1, 310),
    (N'FIELD.PO.LINE.UNIT', N'Unidad', N'colLineUnit', N'Unidad de medida.', N'GridColumn', N'int', 1, 0, 1, 320),
    (N'FIELD.PO.LINE.QUANTITY', N'Cantidad', N'colLineQuantity', N'Cantidad solicitada.', N'GridColumn', N'decimal', 1, 0, 1, 330),
    (N'FIELD.PO.LINE.OPENQTY', N'Cantidad abierta', N'colLineOpenQuantity', N'Cantidad abierta.', N'GridColumn', N'decimal', 0, 1, 1, 340),
    (N'FIELD.PO.LINE.UNITPRICE', N'Precio unitario', N'colLineUnitPrice', N'Precio unitario.', N'GridColumn', N'decimal', 1, 0, 1, 350),
    (N'FIELD.PO.LINE.DISCOUNT', N'Descuento', N'colLineDiscountPercent', N'Porcentaje de descuento.', N'GridColumn', N'decimal', 0, 0, 1, 360),
    (N'FIELD.PO.LINE.TAX', N'Impuesto', N'colLineTax', N'Impuesto aplicado.', N'GridColumn', N'int', 1, 0, 1, 370),
    (N'FIELD.PO.LINE.WAREHOUSE', N'Bodega', N'colLineWarehouse', N'Bodega del detalle.', N'GridColumn', N'int', 1, 0, 1, 380),
    (N'FIELD.PO.LINE.DELIVERYDATE', N'Fecha entrega', N'colLineDeliveryDate', N'Fecha de entrega de la linea.', N'GridColumn', N'date', 1, 0, 1, 390),
    (N'FIELD.PO.LINE.COSTCENTER', N'Centro de costo', N'colLineCostCenter', N'Centro de costo de la linea.', N'GridColumn', N'int', 0, 0, 1, 400),
    (N'FIELD.PO.LINE.PROJECT', N'Proyecto', N'colLineProject', N'Proyecto de la linea.', N'GridColumn', N'int', 0, 0, 1, 410),
    (N'FIELD.PO.LINE.TOTAL', N'Total linea', N'colLineTotal', N'Total de la linea.', N'GridColumn', N'decimal', 0, 1, 1, 420);

MERGE dbo.SecurityFields AS target
USING
(
    SELECT
        @EditFormId AS FormId,
        Code,
        Name,
        FieldKey,
        Description,
        ControlType,
        DataType,
        IsRequired,
        CASE WHEN IsRequired = 1 THEN CONCAT(Name, N' es obligatorio.') ELSE NULL END AS ValidationMessage,
        IsReadOnly,
        IsVisible,
        DisplayOrder
    FROM @Fields
    WHERE @EditFormId IS NOT NULL
) AS source
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET
        FormId = source.FormId,
        Name = source.Name,
        FieldKey = source.FieldKey,
        Description = source.Description,
        ControlType = source.ControlType,
        DataType = source.DataType,
        IsRequired = source.IsRequired,
        ValidationMessage = source.ValidationMessage,
        IsReadOnly = source.IsReadOnly,
        IsVisible = source.IsVisible,
        IsCustom = 0,
        DisplayOrder = source.DisplayOrder,
        IsActive = 1,
        IsDeleted = 0,
        DeletedByUserId = NULL,
        DeletedByUserName = NULL,
        DeletedAt = NULL,
        UpdatedByUserName = N'Sistema',
        UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT
    (
        FormId, Code, Name, FieldKey, Description, ControlType, DataType,
        IsRequired, ValidationMessage, IsReadOnly, IsVisible, IsCustom,
        DisplayOrder, IsActive, CreatedByUserName, CreatedAt, IsDeleted
    )
    VALUES
    (
        source.FormId, source.Code, source.Name, source.FieldKey, source.Description, source.ControlType, source.DataType,
        source.IsRequired, source.ValidationMessage, source.IsReadOnly, source.IsVisible, 0,
        source.DisplayOrder, 1, N'Sistema', SYSUTCDATETIME(), 0
    );

IF @AdminRoleId IS NOT NULL
BEGIN
    MERGE dbo.SecurityRoleMenus AS target
    USING
    (
        SELECT Id AS MenuId
        FROM dbo.SecurityMenus
        WHERE Id IN (@PurchasingMenuId, @PurchaseOrdersMenuId)
          AND Id IS NOT NULL
          AND IsDeleted = 0
    ) AS source
    ON target.RoleId = @AdminRoleId
       AND target.MenuId = source.MenuId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, MenuId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.MenuId, 1, N'Sistema', SYSUTCDATETIME());

    MERGE dbo.SecurityRoleFormOperations AS target
    USING
    (
        SELECT form.Id AS FormId, operation.Id AS OperationId
        FROM dbo.SecurityForms form
        CROSS JOIN dbo.SecurityOperations operation
        WHERE form.Id IN (@ListFormId, @EditFormId)
          AND form.IsDeleted = 0
          AND operation.IsDeleted = 0
          AND operation.IsActive = 1
          AND operation.ActionKey IN
          (
              N'refresh',
              N'consult',
              N'new',
              N'create',
              N'edit',
              N'update',
              N'delete',
              N'approve',
              N'reject',
              N'syncsap',
              N'cancel',
              N'reopen',
              N'print',
              N'copy',
              N'export',
              N'history'
          )
    ) AS source
    ON target.RoleId = @AdminRoleId
       AND target.FormId = source.FormId
       AND target.OperationId = source.OperationId
    WHEN MATCHED THEN
        UPDATE SET
            IsAllowed = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (RoleId, FormId, OperationId, IsAllowed, CreatedByUserName, CreatedAt)
        VALUES (@AdminRoleId, source.FormId, source.OperationId, 1, N'Sistema', SYSUTCDATETIME());

    MERGE dbo.SecurityRoleFormFields AS target
    USING
    (
        SELECT field.FormId, field.Id AS FieldId
        FROM dbo.SecurityFields field
        WHERE field.FormId = @EditFormId
          AND field.IsDeleted = 0
          AND field.IsActive = 1
    ) AS source
    ON target.RoleId = @AdminRoleId
       AND target.FormId = source.FormId
       AND target.FieldId = source.FieldId
    WHEN MATCHED THEN
        UPDATE SET
            IsVisible = 1,
            IsEditable = 1,
            IsRequired = (SELECT IsRequired FROM dbo.SecurityFields f WHERE f.Id = source.FieldId),
            IsReadOnly = (SELECT IsReadOnly FROM dbo.SecurityFields f WHERE f.Id = source.FieldId),
            IsActive = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT
        (
            RoleId, FormId, FieldId, IsVisible, IsEditable, IsRequired, IsReadOnly, IsActive,
            CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @AdminRoleId, source.FormId, source.FieldId, 1, 1,
            (SELECT IsRequired FROM dbo.SecurityFields f WHERE f.Id = source.FieldId),
            (SELECT IsReadOnly FROM dbo.SecurityFields f WHERE f.Id = source.FieldId),
            1, N'Sistema', SYSUTCDATETIME()
        );

    DECLARE @InitialDocumentSeriesAccess table
    (
        CompanyCode nvarchar(50) NOT NULL,
        SecurityDocumentSeriesId int NOT NULL
    );

    INSERT INTO @InitialDocumentSeriesAccess (CompanyCode, SecurityDocumentSeriesId)
    SELECT company.Code, 1
    FROM dbo.Companies company
    WHERE company.Code = N'DEMO'
      AND company.IsDeleted = 0
      AND company.IsActive = 1;

    MERGE dbo.SecurityRoleDocumentSeries AS target
    USING
    (
        SELECT
            @AdminRoleId AS RoleId,
            access.CompanyCode,
            N'purchase-orders-edit' AS FormKey,
            access.SecurityDocumentSeriesId,
            N'PURCHASE_ORDER' AS DocumentType
        FROM @InitialDocumentSeriesAccess access
    ) AS source
    ON target.RoleId = source.RoleId
       AND target.CompanyCode = source.CompanyCode
       AND target.FormKey = source.FormKey
       AND target.SecurityDocumentSeriesId = source.SecurityDocumentSeriesId
       AND target.DocumentType = source.DocumentType
    WHEN MATCHED THEN
        UPDATE SET
            IsActive = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT
        (
            RoleId, CompanyCode, FormKey, SecurityDocumentSeriesId, DocumentType,
            IsActive, CreatedByUserName, CreatedAt, IsDeleted
        )
        VALUES
        (
            source.RoleId, source.CompanyCode, source.FormKey, source.SecurityDocumentSeriesId, source.DocumentType,
            1, N'Sistema', SYSUTCDATETIME(), 0
        );

    MERGE dbo.SecurityRoleDocumentSeriesOperation AS target
    USING
    (
        SELECT
            seriesAccess.Id AS SecurityRoleDocumentSeriesId,
            operation.Id AS OperationId,
            operation.ActionKey
        FROM dbo.SecurityRoleDocumentSeries seriesAccess
        INNER JOIN @InitialDocumentSeriesAccess access ON access.CompanyCode = seriesAccess.CompanyCode
            AND access.SecurityDocumentSeriesId = seriesAccess.SecurityDocumentSeriesId
        INNER JOIN dbo.SecurityOperations operation ON operation.IsDeleted = 0
            AND operation.IsActive = 1
            AND operation.ActionKey IN
            (
                N'refresh',
                N'consult',
                N'new',
                N'create',
                N'edit',
                N'update',
                N'delete',
                N'approve',
                N'reject',
                N'syncsap',
                N'cancel',
                N'reopen',
                N'print',
                N'copy',
                N'export',
                N'history'
            )
        WHERE seriesAccess.RoleId = @AdminRoleId
          AND seriesAccess.FormKey = N'purchase-orders-edit'
          AND seriesAccess.DocumentType = N'PURCHASE_ORDER'
          AND seriesAccess.IsDeleted = 0
    ) AS source
    ON target.SecurityRoleDocumentSeriesId = source.SecurityRoleDocumentSeriesId
       AND target.ActionKey = source.ActionKey
    WHEN MATCHED THEN
        UPDATE SET
            OperationId = source.OperationId,
            IsAllowed = 1,
            IsActive = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserName = N'Sistema',
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT
        (
            SecurityRoleDocumentSeriesId, OperationId, ActionKey, IsAllowed, IsActive,
            CreatedByUserName, CreatedAt, IsDeleted
        )
        VALUES
        (
            source.SecurityRoleDocumentSeriesId, source.OperationId, source.ActionKey, 1, 1,
            N'Sistema', SYSUTCDATETIME(), 0
        );
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_USUARIO_AUTORIZADAS
    @UserId int,
    @CompanyCode nvarchar(50),
    @FormKey nvarchar(120),
    @DocumentType nvarchar(50),
    @ActionKey nvarchar(120)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT header.SecurityDocumentSeriesId
    FROM dbo.UserRoles userRole
    INNER JOIN dbo.Roles role ON role.Id = userRole.RoleId
        AND role.IsDeleted = 0
        AND role.IsActive = 1
    INNER JOIN dbo.SecurityRoleDocumentSeries header ON header.RoleId = userRole.RoleId
        AND header.CompanyCode = @CompanyCode
        AND header.FormKey = @FormKey
        AND header.DocumentType = @DocumentType
        AND header.IsDeleted = 0
        AND header.IsActive = 1
    INNER JOIN dbo.SecurityRoleDocumentSeriesOperation operationAccess ON operationAccess.SecurityRoleDocumentSeriesId = header.Id
        AND operationAccess.IsDeleted = 0
        AND operationAccess.IsActive = 1
        AND operationAccess.IsAllowed = 1
        AND operationAccess.ActionKey = @ActionKey
    WHERE userRole.UserId = @UserId
    ORDER BY header.SecurityDocumentSeriesId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_VALIDAROPERACIONUSUARIO
    @UserId int,
    @CompanyCode nvarchar(50),
    @FormKey nvarchar(120),
    @DocumentType nvarchar(50),
    @SecurityDocumentSeriesId int,
    @ActionKey nvarchar(120)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.UserRoles userRole
        INNER JOIN dbo.Roles role ON role.Id = userRole.RoleId
            AND role.IsDeleted = 0
            AND role.IsActive = 1
        INNER JOIN dbo.SecurityRoleDocumentSeries header ON header.RoleId = userRole.RoleId
            AND header.CompanyCode = @CompanyCode
            AND header.FormKey = @FormKey
            AND header.DocumentType = @DocumentType
            AND header.SecurityDocumentSeriesId = @SecurityDocumentSeriesId
            AND header.IsDeleted = 0
            AND header.IsActive = 1
        INNER JOIN dbo.SecurityRoleDocumentSeriesOperation operationAccess ON operationAccess.SecurityRoleDocumentSeriesId = header.Id
            AND operationAccess.IsDeleted = 0
            AND operationAccess.IsActive = 1
            AND operationAccess.IsAllowed = 1
            AND operationAccess.ActionKey = @ActionKey
        WHERE userRole.UserId = @UserId
    ) THEN 1 ELSE 0 END AS bit) AS IsAllowed;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_FIELDS_USUARIO
    @UserId int,
    @CompanyCode nvarchar(50),
    @FormKey nvarchar(120),
    @DocumentType nvarchar(50),
    @SecurityDocumentSeriesId int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FormId int =
    (
        SELECT TOP (1) Id
        FROM dbo.SecurityForms
        WHERE FormKey = @FormKey
          AND IsDeleted = 0
    );

    ;WITH UserFieldAccess AS
    (
        SELECT
            fieldAccess.FieldId,
            MAX(CONVERT(int, fieldAccess.IsVisible)) AS IsVisible,
            MAX(CONVERT(int, fieldAccess.IsEditable)) AS IsEditable,
            MAX(CONVERT(int, fieldAccess.IsRequired)) AS IsRequired,
            MAX(CONVERT(int, fieldAccess.IsReadOnly)) AS IsReadOnly,
            MAX(CONVERT(int, fieldAccess.IsActive)) AS IsActive
        FROM dbo.UserRoles userRole
        INNER JOIN dbo.Roles role ON role.Id = userRole.RoleId
            AND role.IsDeleted = 0
            AND role.IsActive = 1
        INNER JOIN dbo.SecurityRoleDocumentSeries header ON header.RoleId = userRole.RoleId
            AND header.CompanyCode = @CompanyCode
            AND header.FormKey = @FormKey
            AND header.DocumentType = @DocumentType
            AND header.SecurityDocumentSeriesId = @SecurityDocumentSeriesId
            AND header.IsDeleted = 0
            AND header.IsActive = 1
        INNER JOIN dbo.SecurityRoleDocumentSeriesField fieldAccess ON fieldAccess.SecurityRoleDocumentSeriesId = header.Id
            AND fieldAccess.IsDeleted = 0
        WHERE userRole.UserId = @UserId
        GROUP BY fieldAccess.FieldId
    )
    SELECT
        field.Id AS FieldId,
        field.FormId,
        form.Code AS FormCode,
        form.Name AS FormName,
        form.FormKey,
        field.Code AS FieldCode,
        field.Name AS FieldName,
        field.FieldKey,
        field.Description,
        field.ControlType,
        field.DataType,
        field.IsVisible AS DefaultVisible,
        CASE WHEN field.IsReadOnly = 1 THEN CONVERT(bit, 0) ELSE CONVERT(bit, 1) END AS DefaultEditable,
        field.IsRequired AS DefaultRequired,
        field.IsReadOnly AS DefaultReadOnly,
        field.DisplayOrder,
        CONVERT(bit, COALESCE(access.IsVisible, CONVERT(int, field.IsVisible))) AS IsVisible,
        CONVERT(bit, COALESCE(access.IsEditable, CASE WHEN field.IsReadOnly = 1 THEN 0 ELSE 1 END)) AS IsEditable,
        CONVERT(bit, COALESCE(access.IsRequired, CONVERT(int, field.IsRequired))) AS IsRequired,
        CONVERT(bit, CASE
            WHEN COALESCE(access.IsEditable, CASE WHEN field.IsReadOnly = 1 THEN 0 ELSE 1 END) = 1 THEN 0
            ELSE COALESCE(access.IsReadOnly, CONVERT(int, field.IsReadOnly))
        END) AS IsReadOnly,
        CONVERT(bit, COALESCE(access.IsActive, 1)) AS IsActive,
        CONVERT(int, NULL) AS UpdatedByUserId,
        CONVERT(nvarchar(120), NULL) AS UpdatedByUserName,
        CONVERT(datetime2(0), NULL) AS UpdatedAt,
        CONVERT(int, NULL) AS CreatedByUserId,
        CONVERT(nvarchar(120), NULL) AS CreatedByUserName,
        CONVERT(datetime2(0), NULL) AS CreatedAt
    FROM dbo.SecurityFields field
    INNER JOIN dbo.SecurityForms form ON form.Id = field.FormId
        AND form.IsDeleted = 0
    LEFT JOIN UserFieldAccess access ON access.FieldId = field.Id
    WHERE field.FormId = @FormId
      AND field.IsDeleted = 0
      AND field.IsActive = 1
    ORDER BY field.DisplayOrder, field.Name;
END;
GO
