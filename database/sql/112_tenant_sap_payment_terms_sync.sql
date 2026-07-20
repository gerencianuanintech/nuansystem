/* Importacion Full SAP B1 y aplicacion idempotente de Condiciones de Pago. */
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.BusinessPartnerPaymentTerms', N'U') IS NULL
    THROW 51112, 'No existe BusinessPartnerPaymentTerms. Ejecute primero 024 y 063.', 1;
GO

IF COL_LENGTH(N'dbo.BusinessPartnerPaymentTerms', N'GlobalId') IS NULL
    ALTER TABLE dbo.BusinessPartnerPaymentTerms ADD GlobalId uniqueidentifier NOT NULL
        CONSTRAINT DF_BusinessPartnerPaymentTerms_GlobalId_112 DEFAULT NEWID();
IF COL_LENGTH(N'dbo.BusinessPartnerPaymentTerms', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.BusinessPartnerPaymentTerms ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.BusinessPartnerPaymentTerms', N'ExternalCode') IS NULL
    ALTER TABLE dbo.BusinessPartnerPaymentTerms ADD ExternalCode nvarchar(100) NULL;
GO

IF EXISTS
(
    SELECT ExternalSystem, ExternalCode
    FROM dbo.BusinessPartnerPaymentTerms
    WHERE IsDeleted=0 AND ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL
    GROUP BY ExternalSystem, ExternalCode HAVING COUNT(*) > 1
)
    THROW 51112, 'Existen referencias externas duplicadas en BusinessPartnerPaymentTerms.', 1;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'UX_BusinessPartnerPaymentTerms_GlobalId' AND object_id=OBJECT_ID(N'dbo.BusinessPartnerPaymentTerms'))
    CREATE UNIQUE INDEX UX_BusinessPartnerPaymentTerms_GlobalId ON dbo.BusinessPartnerPaymentTerms(GlobalId);
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_BusinessPartnerPaymentTerms_ExternalRef' AND object_id=OBJECT_ID(N'dbo.BusinessPartnerPaymentTerms'))
    DROP INDEX IX_BusinessPartnerPaymentTerms_ExternalRef ON dbo.BusinessPartnerPaymentTerms;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'UX_BusinessPartnerPaymentTerms_ExternalRef' AND object_id=OBJECT_ID(N'dbo.BusinessPartnerPaymentTerms'))
    CREATE UNIQUE INDEX UX_BusinessPartnerPaymentTerms_ExternalRef
        ON dbo.BusinessPartnerPaymentTerms(ExternalSystem,ExternalCode)
        WHERE IsDeleted=0 AND ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_BUSINESSPARTNERPAYMENTTERMS_IMPORTARSAP
    @ProposedGlobalId uniqueidentifier,
    @Code nvarchar(30),
    @Name nvarchar(120),
    @Days int,
    @IsCredit bit,
    @ExternalSystem nvarchar(50),
    @ExternalCode nvarchar(100),
    @AuditUserId int = NULL,
    @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Code=LTRIM(RTRIM(@Code));
    SET @Name=LTRIM(RTRIM(@Name));
    SET @ExternalSystem=LTRIM(RTRIM(@ExternalSystem));
    SET @ExternalCode=LTRIM(RTRIM(@ExternalCode));
    IF @Days < 0 THROW 51112, 'Days no puede ser negativo.', 1;
    IF NULLIF(@Code,N'') IS NULL OR NULLIF(@Name,N'') IS NULL
        THROW 51112, 'Code y Name son obligatorios.', 1;
    IF NULLIF(@ExternalSystem,N'') IS NULL OR NULLIF(@ExternalCode,N'') IS NULL
        THROW 51112, 'La referencia SAP B1 es obligatoria.', 1;

    BEGIN TRANSACTION;
    DECLARE @Id int, @Status nvarchar(20), @Message nvarchar(300);
    SELECT @Id=Id FROM dbo.BusinessPartnerPaymentTerms WITH(UPDLOCK,HOLDLOCK)
    WHERE IsDeleted=0 AND ExternalSystem=@ExternalSystem AND ExternalCode=@ExternalCode;

    IF @Id IS NOT NULL AND EXISTS
    (
        SELECT 1 FROM dbo.BusinessPartnerPaymentTerms WITH(UPDLOCK,HOLDLOCK)
        WHERE IsDeleted=0 AND Code=@Code AND Id<>@Id
    )
    BEGIN
        SELECT N'Conflict' Status, Id, GlobalId, Code, Name, Days, IsCredit, IsActive,
            ExternalSystem, ExternalCode, CreatedAt, UpdatedAt,
            N'El codigo SAP B1 colisiona con otra condicion local y no se reasigna automaticamente.' Message
        FROM dbo.BusinessPartnerPaymentTerms WHERE Id=@Id;
        COMMIT;
        RETURN;
    END;

    IF @Id IS NULL AND EXISTS(SELECT 1 FROM dbo.BusinessPartnerPaymentTerms WITH(UPDLOCK,HOLDLOCK) WHERE IsDeleted=0 AND Code=@Code)
    BEGIN
        SELECT TOP(1) N'Conflict' Status, Id, GlobalId, Code, Name, Days, IsCredit, IsActive,
            ExternalSystem, ExternalCode, CreatedAt, UpdatedAt,
            N'El codigo local ya existe y no se adopta automaticamente como registro SAP B1.' Message
        FROM dbo.BusinessPartnerPaymentTerms WHERE IsDeleted=0 AND Code=@Code;
        COMMIT;
        RETURN;
    END;

    IF @Id IS NULL
    BEGIN
        INSERT dbo.BusinessPartnerPaymentTerms
            (GlobalId,Code,Name,Days,IsCredit,IsActive,ExternalSystem,ExternalCode,CreatedByUserId,CreatedByUserName)
        VALUES(@ProposedGlobalId,@Code,@Name,@Days,@IsCredit,1,@ExternalSystem,@ExternalCode,@AuditUserId,@AuditUserName);
        SET @Id=CONVERT(int,SCOPE_IDENTITY()); SET @Status=N'Created'; SET @Message=N'Condicion SAP B1 creada.';
    END
    ELSE IF EXISTS
    (
        SELECT 1 FROM dbo.BusinessPartnerPaymentTerms
        WHERE Id=@Id AND (Code<>@Code OR Name<>@Name OR Days<>@Days OR IsCredit<>@IsCredit OR IsActive=0)
    )
    BEGIN
        UPDATE dbo.BusinessPartnerPaymentTerms
        SET Code=@Code,Name=@Name,Days=@Days,IsCredit=@IsCredit,IsActive=1,
            UpdatedByUserId=@AuditUserId,UpdatedByUserName=@AuditUserName,UpdatedAt=SYSUTCDATETIME()
        WHERE Id=@Id;
        SET @Status=N'Updated'; SET @Message=N'Condicion SAP B1 actualizada.';
    END
    ELSE BEGIN SET @Status=N'Unchanged'; SET @Message=N'Condicion SAP B1 sin cambios.'; END;

    SELECT @Status Status, Id, GlobalId, Code, Name, Days, IsCredit, IsActive, ExternalSystem, ExternalCode,
        CreatedAt, UpdatedAt, @Message Message
    FROM dbo.BusinessPartnerPaymentTerms WHERE Id=@Id;
    COMMIT;
END;
GO

IF OBJECT_ID(N'dbo.SchemaHistory',N'U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260719.112')
    INSERT dbo.SchemaHistory(Version,Description) VALUES(N'20260719.112',N'Condiciones de pago SAP B1 y referencia externa unica');
GO
