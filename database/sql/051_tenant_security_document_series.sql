SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.SecurityDocumentSeries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityDocumentSeries
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityDocumentSeries PRIMARY KEY,
        DocumentType nvarchar(50) NOT NULL,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        Prefix nvarchar(20) NOT NULL,
        Establishment nvarchar(20) NOT NULL,
        EmissionPoint nvarchar(20) NOT NULL,
        InitialNumber int NOT NULL CONSTRAINT DF_SecurityDocumentSeries_InitialNumber DEFAULT 1,
        CurrentNumber int NOT NULL CONSTRAINT DF_SecurityDocumentSeries_CurrentNumber DEFAULT 0,
        NextNumber int NOT NULL CONSTRAINT DF_SecurityDocumentSeries_NextNumber DEFAULT 1,
        NumberLength int NOT NULL CONSTRAINT DF_SecurityDocumentSeries_NumberLength DEFAULT 8,
        SapObjectType nvarchar(20) NULL,
        SapSeriesId int NULL,
        SapSeriesName nvarchar(120) NULL,
        IsDefault bit NOT NULL CONSTRAINT DF_SecurityDocumentSeries_IsDefault DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityDocumentSeries_IsActive DEFAULT 1,
        IsSapIntegrationActive bit NOT NULL CONSTRAINT DF_SecurityDocumentSeries_IsSapIntegrationActive DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityDocumentSeries_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityDocumentSeries_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT CK_SecurityDocumentSeries_InitialNumber CHECK (InitialNumber >= 0),
        CONSTRAINT CK_SecurityDocumentSeries_CurrentNumber CHECK (CurrentNumber >= 0),
        CONSTRAINT CK_SecurityDocumentSeries_NextNumber CHECK (NextNumber > 0),
        CONSTRAINT CK_SecurityDocumentSeries_NumberLength CHECK (NumberLength > 0 AND NumberLength <= 18)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityDocumentSeries_Code' AND object_id = OBJECT_ID(N'dbo.SecurityDocumentSeries'))
    CREATE UNIQUE INDEX UX_SecurityDocumentSeries_Code ON dbo.SecurityDocumentSeries(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityDocumentSeries_Type_Prefix_Est_Emission' AND object_id = OBJECT_ID(N'dbo.SecurityDocumentSeries'))
    CREATE UNIQUE INDEX UX_SecurityDocumentSeries_Type_Prefix_Est_Emission ON dbo.SecurityDocumentSeries(DocumentType, Prefix, Establishment, EmissionPoint) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityDocumentSeries_DefaultByType' AND object_id = OBJECT_ID(N'dbo.SecurityDocumentSeries'))
    CREATE UNIQUE INDEX UX_SecurityDocumentSeries_DefaultByType ON dbo.SecurityDocumentSeries(DocumentType) WHERE IsDeleted = 0 AND IsDefault = 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_LISTAR
    @Search nvarchar(120) = NULL,
    @DocumentType nvarchar(50) = NULL,
    @IsActive bit = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint,
        InitialNumber, CurrentNumber, NextNumber, NumberLength,
        RIGHT(REPLICATE(N'0', NumberLength) + CONVERT(nvarchar(30), NextNumber), NumberLength) AS NextNumberFormatted,
        SapObjectType, SapSeriesId, SapSeriesName,
        IsDefault, IsActive, IsSapIntegrationActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.SecurityDocumentSeries
    WHERE IsDeleted = 0
      AND (@IsActive IS NULL OR IsActive = @IsActive)
      AND (@DocumentType IS NULL OR DocumentType = @DocumentType)
      AND (
            @Search IS NULL OR
            Code LIKE N'%' + @Search + N'%' OR
            Name LIKE N'%' + @Search + N'%' OR
            Prefix LIKE N'%' + @Search + N'%' OR
            DocumentType LIKE N'%' + @Search + N'%'
      )
    ORDER BY DocumentType, IsDefault DESC, Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint,
        InitialNumber, CurrentNumber, NextNumber, NumberLength,
        RIGHT(REPLICATE(N'0', NumberLength) + CONVERT(nvarchar(30), NextNumber), NumberLength) AS NextNumberFormatted,
        SapObjectType, SapSeriesId, SapSeriesName,
        IsDefault, IsActive, IsSapIntegrationActive,
        CreatedByUserId, CreatedByUserName, CreatedAt,
        UpdatedByUserId, UpdatedByUserName, UpdatedAt
    FROM dbo.SecurityDocumentSeries
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_LOOKUP
    @DocumentType nvarchar(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, Code, Name, DocumentType, Prefix, Establishment, EmissionPoint,
        NextNumber, NumberLength,
        RIGHT(REPLICATE(N'0', NumberLength) + CONVERT(nvarchar(30), NextNumber), NumberLength) AS NextNumberFormatted,
        IsDefault, IsActive
    FROM dbo.SecurityDocumentSeries
    WHERE IsDeleted = 0
      AND IsActive = 1
      AND (@DocumentType IS NULL OR DocumentType = @DocumentType)
    ORDER BY DocumentType, IsDefault DESC, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_BUSCARPORCODIGO
    @Code nvarchar(30),
    @ExcludeId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.SecurityDocumentSeries
    WHERE Code = @Code
      AND IsDeleted = 0
      AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_BUSCARPORCLAVE
    @DocumentType nvarchar(50),
    @Prefix nvarchar(20),
    @Establishment nvarchar(20),
    @EmissionPoint nvarchar(20),
    @ExcludeId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.SecurityDocumentSeries
    WHERE DocumentType = @DocumentType
      AND Prefix = @Prefix
      AND Establishment = @Establishment
      AND EmissionPoint = @EmissionPoint
      AND IsDeleted = 0
      AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SECURITYDOCUMENTSERIES_CREAR
    @DocumentType nvarchar(50),
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @Prefix nvarchar(20),
    @Establishment nvarchar(20),
    @EmissionPoint nvarchar(20),
    @InitialNumber int,
    @CurrentNumber int,
    @NextNumber int,
    @NumberLength int,
    @SapObjectType nvarchar(20) = NULL,
    @SapSeriesId int = NULL,
    @SapSeriesName nvarchar(120) = NULL,
    @IsDefault bit = 0,
    @IsActive bit = 1,
    @IsSapIntegrationActive bit = 0,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    IF @IsDefault = 1
    BEGIN
        UPDATE dbo.SecurityDocumentSeries
        SET IsDefault = 0,
            UpdatedByUserId = @CreatedByUserId,
            UpdatedByUserName = @CreatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE DocumentType = @DocumentType
          AND IsDeleted = 0
          AND IsDefault = 1;
    END;

    INSERT INTO dbo.SecurityDocumentSeries
    (
        DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint,
        InitialNumber, CurrentNumber, NextNumber, NumberLength,
        SapObjectType, SapSeriesId, SapSeriesName,
        IsDefault, IsActive, IsSapIntegrationActive,
        CreatedByUserId, CreatedByUserName
    )
    VALUES
    (
        @DocumentType, @Code, @Name, @Description, @Prefix, @Establishment, @EmissionPoint,
        @InitialNumber, @CurrentNumber, @NextNumber, @NumberLength,
        @SapObjectType, @SapSeriesId, @SapSeriesName,
        @IsDefault, @IsActive, @IsSapIntegrationActive,
        @CreatedByUserId, @CreatedByUserName
    );

    DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());

    COMMIT TRANSACTION;

    SELECT @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SECURITYDOCUMENTSERIES_ACTUALIZAR
    @Id int,
    @DocumentType nvarchar(50),
    @Code nvarchar(30),
    @Name nvarchar(160),
    @Description nvarchar(300) = NULL,
    @Prefix nvarchar(20),
    @Establishment nvarchar(20),
    @EmissionPoint nvarchar(20),
    @InitialNumber int,
    @CurrentNumber int,
    @NextNumber int,
    @NumberLength int,
    @SapObjectType nvarchar(20) = NULL,
    @SapSeriesId int = NULL,
    @SapSeriesName nvarchar(120) = NULL,
    @IsDefault bit = 0,
    @IsActive bit = 1,
    @IsSapIntegrationActive bit = 0,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    IF @IsDefault = 1
    BEGIN
        UPDATE dbo.SecurityDocumentSeries
        SET IsDefault = 0,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id <> @Id
          AND DocumentType = @DocumentType
          AND IsDeleted = 0
          AND IsDefault = 1;
    END;

    UPDATE dbo.SecurityDocumentSeries
    SET DocumentType = @DocumentType,
        Code = @Code,
        Name = @Name,
        Description = @Description,
        Prefix = @Prefix,
        Establishment = @Establishment,
        EmissionPoint = @EmissionPoint,
        InitialNumber = @InitialNumber,
        CurrentNumber = @CurrentNumber,
        NextNumber = @NextNumber,
        NumberLength = @NumberLength,
        SapObjectType = @SapObjectType,
        SapSeriesId = @SapSeriesId,
        SapSeriesName = @SapSeriesName,
        IsDefault = @IsDefault,
        IsActive = @IsActive,
        IsSapIntegrationActive = @IsSapIntegrationActive,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    DECLARE @AffectedRows int = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @AffectedRows;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_SECURITYDOCUMENTSERIES_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SecurityDocumentSeries
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedByUserId = @DeletedByUserId,
        UpdatedByUserName = @DeletedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SECURITYDOCUMENTSERIES_RESERVARNUMERO
    @Id int,
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE
        @Prefix nvarchar(20),
        @ReservedNumber int,
        @NumberLength int;

    SELECT
        @Prefix = Prefix,
        @ReservedNumber = NextNumber,
        @NumberLength = NumberLength
    FROM dbo.SecurityDocumentSeries WITH (UPDLOCK, HOLDLOCK)
    WHERE Id = @Id
      AND IsDeleted = 0
      AND IsActive = 1;

    IF @ReservedNumber IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        SELECT CAST(0 AS bit) AS Success,
               CAST(NULL AS int) AS ReservedNumber,
               CAST(NULL AS nvarchar(30)) AS FormattedNumber,
               CAST(NULL AS nvarchar(60)) AS DisplayNumber,
               N'La serie no existe o no esta activa.' AS Message;
        RETURN;
    END;

    UPDATE dbo.SecurityDocumentSeries
    SET CurrentNumber = @ReservedNumber,
        NextNumber = @ReservedNumber + 1,
        UpdatedByUserId = @AuditUserId,
        UpdatedByUserName = @AuditUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id;

    COMMIT TRANSACTION;

    DECLARE @FormattedNumber nvarchar(30) = RIGHT(REPLICATE(N'0', @NumberLength) + CONVERT(nvarchar(30), @ReservedNumber), @NumberLength);

    SELECT CAST(1 AS bit) AS Success,
           @ReservedNumber AS ReservedNumber,
           @FormattedNumber AS FormattedNumber,
           CONCAT(@Prefix, N'-', @FormattedNumber) AS DisplayNumber,
           N'Numero reservado correctamente.' AS Message;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'OC001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, SapSeriesId, SapSeriesName, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'PURCHASE_ORDER', N'OC001', N'Orden de compra', N'Serie base para ordenes de compra.', N'OC', N'001', N'001', 1, 122, 123, 8, N'22', NULL, NULL, 1, 1, 1, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'FAC001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'SALES_INVOICE', N'FAC001', N'Factura de venta', N'Serie base para facturas de venta.', N'FV', N'001', N'001', 1, 1566, 1567, 8, N'13', 1, 1, 1, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'BOL001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'SALES_RECEIPT', N'BOL001', N'Boleta de venta', N'Serie base para boletas de venta.', N'BV', N'001', N'001', 1, 3240, 3241, 8, NULL, 1, 1, 0, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'GUI001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'DELIVERY_NOTE', N'GUI001', N'Guia de remision', N'Serie base para guias de remision.', N'GR', N'001', N'001', 1, 2457, 2458, 8, NULL, 1, 1, 0, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'ING001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'PURCHASE_RECEIPT', N'ING001', N'Ingreso por compras', N'Serie base para ingresos por compras.', N'IC', N'001', N'001', 1, 4121, 4122, 8, N'20', 1, 1, 1, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'TRA001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'WAREHOUSE_TRANSFER', N'TRA001', N'Transferencia entre almacenes', N'Serie base para transferencias.', N'TA', N'001', N'001', 1, 1088, 1089, 8, N'67', 1, 1, 1, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'NDE001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'DEBIT_NOTE', N'NDE001', N'Nota de debito', N'Serie base para notas de debito.', N'ND', N'001', N'001', 1, 764, 765, 8, NULL, 1, 1, 0, N'Sistema');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SecurityDocumentSeries WHERE Code = N'NCR001' AND IsDeleted = 0)
    INSERT INTO dbo.SecurityDocumentSeries (DocumentType, Code, Name, Description, Prefix, Establishment, EmissionPoint, InitialNumber, CurrentNumber, NextNumber, NumberLength, SapObjectType, IsDefault, IsActive, IsSapIntegrationActive, CreatedByUserName)
    VALUES (N'CREDIT_NOTE', N'NCR001', N'Nota de credito', N'Serie base para notas de credito.', N'NC', N'001', N'001', 1, 653, 654, 8, N'14', 1, 1, 1, N'Sistema');
GO
