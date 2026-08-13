/*
    Funcionaliza Unidades de medida como maestro independiente de tenant.

    Evoluciona la tabla existente sin recrearla ni fusionar codigos historicos.
    CAJ y CAJA se conservan como identidades diferentes y quedan auditadas.
    ExternalSystem/ExternalCode pertenecen a la empresa local y no deben viajar
    en la sincronizacion Matriz-Sucursal.

    Los SP de escritura respetan una transaccion externa para que el backend
    confirme maestro + LocalOutbox en una sola unidad atomica.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.UnitOfMeasures', N'U') IS NULL
    THROW 51194, 'UnitOfMeasures from inventory baseline is required.', 1;
IF OBJECT_ID(N'dbo.AuditInventoryChanges', N'U') IS NULL
    THROW 51194, 'AuditInventoryChanges is required.', 1;
IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NULL
    THROW 51194, 'LocalOutbox is required for atomic CRUD publishing.', 1;
IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
    THROW 51194, 'SchemaHistory is required.', 1;
GO

IF COL_LENGTH(N'dbo.UnitOfMeasures', N'GlobalId') IS NULL
    ALTER TABLE dbo.UnitOfMeasures ADD GlobalId uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.UnitOfMeasures', N'Description') IS NULL
    ALTER TABLE dbo.UnitOfMeasures ADD Description nvarchar(500) NULL;
IF COL_LENGTH(N'dbo.UnitOfMeasures', N'Symbol') IS NULL
    ALTER TABLE dbo.UnitOfMeasures ADD Symbol nvarchar(20) NULL;
IF COL_LENGTH(N'dbo.UnitOfMeasures', N'MagnitudeCode') IS NULL
    ALTER TABLE dbo.UnitOfMeasures ADD MagnitudeCode nvarchar(20) NULL;
IF COL_LENGTH(N'dbo.UnitOfMeasures', N'SortOrder') IS NULL
    ALTER TABLE dbo.UnitOfMeasures ADD SortOrder int NULL;
IF COL_LENGTH(N'dbo.UnitOfMeasures', N'ExternalSystem') IS NULL
    ALTER TABLE dbo.UnitOfMeasures ADD ExternalSystem nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.UnitOfMeasures', N'ExternalCode') IS NULL
    ALTER TABLE dbo.UnitOfMeasures ADD ExternalCode nvarchar(100) NULL;
GO

IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WHERE LEN(Code)>50)
    THROW 51194, 'UnitOfMeasures contains Code values longer than 50.', 1;
IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WHERE LEN(Name)>150)
    THROW 51194, 'UnitOfMeasures contains Name values longer than 150.', 1;
IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WHERE LEN(Description)>500)
    THROW 51194, 'UnitOfMeasures contains Description values longer than 500.', 1;
GO

/* 044 puede haber creado este indice sobre Name(120); se reemplaza por el
   indice compuesto moderno despues de ampliar Name a 150. */
IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'IX_UnitOfMeasures_Name_Active')
    DROP INDEX IX_UnitOfMeasures_Name_Active ON dbo.UnitOfMeasures;

IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'Code' AND max_length<>100)
    ALTER TABLE dbo.UnitOfMeasures ALTER COLUMN Code nvarchar(50) NOT NULL;
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'Name' AND max_length<>300)
    ALTER TABLE dbo.UnitOfMeasures ALTER COLUMN Name nvarchar(150) NOT NULL;
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'Description' AND max_length<>1000)
    ALTER TABLE dbo.UnitOfMeasures ALTER COLUMN Description nvarchar(500) NULL;
GO

/* Captura el estado anterior solo para registrar el backfill una vez. */
DECLARE @Backfill table
(
    Id int PRIMARY KEY,
    OldSymbol nvarchar(20) NULL,
    OldMagnitudeCode nvarchar(20) NULL,
    OldSortOrder int NULL
);

INSERT @Backfill(Id,OldSymbol,OldMagnitudeCode,OldSortOrder)
SELECT Id,Symbol,MagnitudeCode,SortOrder
FROM dbo.UnitOfMeasures
WHERE GlobalId IS NULL OR MagnitudeCode IS NULL OR SortOrder IS NULL
   OR (Symbol IS NULL AND UPPER(LTRIM(RTRIM(Code))) IN(N'UND',N'CAJ',N'CAJA',N'PAQ',N'BUL',N'KG',N'G',N'L'));

UPDATE dbo.UnitOfMeasures SET GlobalId=NEWID() WHERE GlobalId IS NULL;
UPDATE dbo.UnitOfMeasures SET SortOrder=0 WHERE SortOrder IS NULL;

UPDATE dbo.UnitOfMeasures
SET MagnitudeCode=CASE UPPER(LTRIM(RTRIM(Code)))
        WHEN N'UND' THEN N'Quantity'
        WHEN N'CAJ' THEN N'Packaging'
        WHEN N'CAJA' THEN N'Packaging'
        WHEN N'PAQ' THEN N'Packaging'
        WHEN N'BUL' THEN N'Packaging'
        WHEN N'KG' THEN N'Mass'
        WHEN N'G' THEN N'Mass'
        WHEN N'L' THEN N'Volume'
        ELSE N'Other' END
WHERE MagnitudeCode IS NULL;

UPDATE dbo.UnitOfMeasures
SET Symbol=CASE UPPER(LTRIM(RTRIM(Code)))
        WHEN N'UND' THEN N'und'
        WHEN N'CAJ' THEN N'caj'
        WHEN N'CAJA' THEN N'caj'
        WHEN N'PAQ' THEN N'paq'
        WHEN N'BUL' THEN N'bul'
        WHEN N'KG' THEN N'kg'
        WHEN N'G' THEN N'g'
        WHEN N'L' THEN N'L' END
WHERE Symbol IS NULL
  AND UPPER(LTRIM(RTRIM(Code))) IN(N'UND',N'CAJ',N'CAJA',N'PAQ',N'BUL',N'KG',N'G',N'L');

INSERT dbo.AuditInventoryChanges
(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
SELECT N'UnitOfMeasures',CONVERT(nvarchar(80),u.Id),N'UPDATE',valuesToAudit.FieldName,
       valuesToAudit.OldValue,valuesToAudit.NewValue,N'Sistema',N'Migration194'
FROM dbo.UnitOfMeasures u
JOIN @Backfill b ON b.Id=u.Id
CROSS APPLY(VALUES
    (N'Symbol',CONVERT(nvarchar(max),b.OldSymbol),CONVERT(nvarchar(max),u.Symbol)),
    (N'MagnitudeCode',CONVERT(nvarchar(max),b.OldMagnitudeCode),CONVERT(nvarchar(max),u.MagnitudeCode)),
    (N'SortOrder',CONVERT(nvarchar(max),b.OldSortOrder),CONVERT(nvarchar(max),u.SortOrder))
) valuesToAudit(FieldName,OldValue,NewValue)
WHERE ISNULL(valuesToAudit.OldValue,N'')<>ISNULL(valuesToAudit.NewValue,N'')
  AND NOT EXISTS
  (
      SELECT 1 FROM dbo.AuditInventoryChanges existing
      WHERE existing.EntityName=N'UnitOfMeasures'
        AND existing.RecordId=CONVERT(nvarchar(80),u.Id)
        AND existing.FieldName=valuesToAudit.FieldName
        AND existing.[Source]=N'Migration194'
  );

/* CAJ y CAJA no se fusionan: cada Code conserva Id/GlobalId propios. */
INSERT dbo.AuditInventoryChanges
(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserName,[Source])
SELECT N'UnitOfMeasures',CONVERT(nvarchar(80),u.Id),N'UPDATE',N'LegacyIdentityPreserved',
       NULL,u.Code,N'Sistema',N'Migration194'
FROM dbo.UnitOfMeasures u
WHERE UPPER(LTRIM(RTRIM(u.Code))) IN(N'CAJ',N'CAJA')
  AND NOT EXISTS
  (
      SELECT 1 FROM dbo.AuditInventoryChanges existing
      WHERE existing.EntityName=N'UnitOfMeasures'
        AND existing.RecordId=CONVERT(nvarchar(80),u.Id)
        AND existing.FieldName=N'LegacyIdentityPreserved'
        AND existing.[Source]=N'Migration194'
  );
GO

IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WHERE NULLIF(LTRIM(RTRIM(Code)),N'') IS NULL)
    THROW 51194, 'UnitOfMeasures contains blank codes.', 1;
IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WHERE NULLIF(LTRIM(RTRIM(Name)),N'') IS NULL)
    THROW 51194, 'UnitOfMeasures contains blank names.', 1;
IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WHERE MagnitudeCode NOT IN
    (N'Quantity',N'Packaging',N'Mass',N'Volume',N'Length',N'Area',N'Time',N'Other'))
    THROW 51194, 'UnitOfMeasures contains an unsupported MagnitudeCode.', 1;
IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WHERE SortOrder<0)
    THROW 51194, 'UnitOfMeasures contains a negative SortOrder.', 1;
IF EXISTS(SELECT Code FROM dbo.UnitOfMeasures GROUP BY Code HAVING COUNT_BIG(*)>1)
    THROW 51194, 'UnitOfMeasures contains duplicate codes including tombstones; reconcile before deployment.', 1;
IF EXISTS(SELECT GlobalId FROM dbo.UnitOfMeasures GROUP BY GlobalId HAVING COUNT_BIG(*)>1)
    THROW 51194, 'UnitOfMeasures contains duplicate GlobalId values.', 1;
IF EXISTS
(
    SELECT 1 FROM dbo.UnitOfMeasures
    WHERE (NULLIF(LTRIM(RTRIM(ExternalSystem)),N'') IS NULL AND NULLIF(LTRIM(RTRIM(ExternalCode)),N'') IS NOT NULL)
       OR (NULLIF(LTRIM(RTRIM(ExternalSystem)),N'') IS NOT NULL AND NULLIF(LTRIM(RTRIM(ExternalCode)),N'') IS NULL)
)
    THROW 51194, 'External system and code must be informed together.', 1;
GO

IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'GlobalId' AND is_nullable=1)
    ALTER TABLE dbo.UnitOfMeasures ALTER COLUMN GlobalId uniqueidentifier NOT NULL;
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'MagnitudeCode' AND is_nullable=1)
    ALTER TABLE dbo.UnitOfMeasures ALTER COLUMN MagnitudeCode nvarchar(20) NOT NULL;
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'SortOrder' AND is_nullable=1)
    ALTER TABLE dbo.UnitOfMeasures ALTER COLUMN SortOrder int NOT NULL;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints d
    JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id
    WHERE d.parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND c.name=N'GlobalId'
)
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT DF_UnitOfMeasures_GlobalId_194 DEFAULT NEWID() FOR GlobalId;
IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints d
    JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id
    WHERE d.parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND c.name=N'MagnitudeCode'
)
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT DF_UnitOfMeasures_MagnitudeCode DEFAULT N'Other' FOR MagnitudeCode;
IF NOT EXISTS
(
    SELECT 1 FROM sys.default_constraints d
    JOIN sys.columns c ON c.object_id=d.parent_object_id AND c.column_id=d.parent_column_id
    WHERE d.parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND c.name=N'SortOrder'
)
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT DF_UnitOfMeasures_SortOrder DEFAULT (0) FOR SortOrder;
GO

IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'CK_UnitOfMeasures_Code_NotBlank')
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT CK_UnitOfMeasures_Code_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Code)),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'CK_UnitOfMeasures_Name_NotBlank')
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT CK_UnitOfMeasures_Name_NotBlank CHECK(NULLIF(LTRIM(RTRIM(Name)),N'') IS NOT NULL);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'CK_UnitOfMeasures_MagnitudeCode')
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT CK_UnitOfMeasures_MagnitudeCode CHECK
    (MagnitudeCode IN(N'Quantity',N'Packaging',N'Mass',N'Volume',N'Length',N'Area',N'Time',N'Other'));
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'CK_UnitOfMeasures_SortOrder')
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT CK_UnitOfMeasures_SortOrder CHECK(SortOrder>=0);
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'CK_UnitOfMeasures_ExternalReferencePair')
    ALTER TABLE dbo.UnitOfMeasures ADD CONSTRAINT CK_UnitOfMeasures_ExternalReferencePair CHECK
    ((NULLIF(LTRIM(RTRIM(ExternalSystem)),N'') IS NULL AND NULLIF(LTRIM(RTRIM(ExternalCode)),N'') IS NULL)
     OR (NULLIF(LTRIM(RTRIM(ExternalSystem)),N'') IS NOT NULL AND NULLIF(LTRIM(RTRIM(ExternalCode)),N'') IS NOT NULL));
GO

IF EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'UX_UnitOfMeasures_Code_Active')
    DROP INDEX UX_UnitOfMeasures_Code_Active ON dbo.UnitOfMeasures;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'UX_UnitOfMeasures_Code')
    CREATE UNIQUE INDEX UX_UnitOfMeasures_Code ON dbo.UnitOfMeasures(Code);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'UX_UnitOfMeasures_GlobalId')
    CREATE UNIQUE INDEX UX_UnitOfMeasures_GlobalId ON dbo.UnitOfMeasures(GlobalId);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'IX_UnitOfMeasures_Active_SortOrder_Name')
    CREATE INDEX IX_UnitOfMeasures_Active_SortOrder_Name
    ON dbo.UnitOfMeasures(IsActive,SortOrder,Name) INCLUDE(Code,GlobalId,Symbol,MagnitudeCode) WHERE IsDeleted=0;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.UnitOfMeasures') AND name=N'IX_UnitOfMeasures_ExternalRef')
    CREATE INDEX IX_UnitOfMeasures_ExternalRef ON dbo.UnitOfMeasures(ExternalSystem,ExternalCode)
    WHERE ExternalSystem IS NOT NULL AND ExternalCode IS NOT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_UNIT_OF_MEASURES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,GlobalId,Code,Name,Description,Symbol,MagnitudeCode,SortOrder,IsActive,
           ExternalSystem,ExternalCode,
           CreatedByUserId,CreatedByUserName,CreatedAt,
           UpdatedByUserId,UpdatedByUserName,UpdatedAt,
           DeletedByUserId,DeletedByUserName,DeletedAt
    FROM dbo.UnitOfMeasures WHERE IsDeleted=0
    ORDER BY SortOrder,Name,Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_UNIT_OF_MEASURES_BUSCARPORID @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP(1) Id,GlobalId,Code,Name,Description,Symbol,MagnitudeCode,SortOrder,IsActive,
           ExternalSystem,ExternalCode,
           CreatedByUserId,CreatedByUserName,CreatedAt,
           UpdatedByUserId,UpdatedByUserName,UpdatedAt,
           DeletedByUserId,DeletedByUserName,DeletedAt
    FROM dbo.UnitOfMeasures WHERE Id=@Id AND IsDeleted=0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_UNIT_OF_MEASURES_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,GlobalId,Code,Name,Symbol,MagnitudeCode,SortOrder,IsActive
    FROM dbo.UnitOfMeasures WHERE IsDeleted=0 AND IsActive=1
    ORDER BY SortOrder,Name,Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_UNIT_OF_MEASURES_BUSCARPORCODIGO
    @Code nvarchar(50), @ExcluirId int=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @Code=LTRIM(RTRIM(@Code));
    SELECT COUNT(1) FROM dbo.UnitOfMeasures
    WHERE Code=@Code AND (@ExcluirId IS NULL OR Id<>@ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_UNIT_OF_MEASURES_HISTORIAL @Id int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,EntityName,RecordId,[Action],FieldName,OldValue,NewValue,
           UserId,UserName,[Source],CreatedAt
    FROM dbo.AuditInventoryChanges
    WHERE EntityName=N'UnitOfMeasures' AND RecordId=CONVERT(nvarchar(80),@Id)
    ORDER BY CreatedAt DESC,Id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_UNIT_OF_MEASURES_CREAR
    @GlobalId uniqueidentifier,
    @Code nvarchar(50), @Name nvarchar(150), @Description nvarchar(500)=NULL,
    @Symbol nvarchar(20)=NULL, @MagnitudeCode nvarchar(20), @SortOrder int=0, @IsActive bit,
    @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @CreatedByUserId int=NULL, @CreatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
    SET @Symbol=NULLIF(LTRIM(RTRIM(@Symbol)),N'');
    SET @MagnitudeCode=LTRIM(RTRIM(@MagnitudeCode));
    SET @ExternalSystem=NULLIF(LTRIM(RTRIM(@ExternalSystem)),N'');
    SET @ExternalCode=NULLIF(LTRIM(RTRIM(@ExternalCode)),N'');
    IF @GlobalId IS NULL OR @GlobalId='00000000-0000-0000-0000-000000000000' THROW 51194,'UnitOfMeasure GlobalId is required.',1;
    IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1;
    IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
    IF @MagnitudeCode NOT IN(N'Quantity',N'Packaging',N'Mass',N'Volume',N'Length',N'Area',N'Time',N'Other') THROW 51194,'MagnitudeCode is invalid.',1;
    IF @SortOrder<0 THROW 51194,'SortOrder cannot be negative.',1;
    IF (@ExternalSystem IS NULL AND @ExternalCode IS NOT NULL) OR (@ExternalSystem IS NOT NULL AND @ExternalCode IS NULL)
        THROW 51194,'External system and code must be informed together.',1;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code OR GlobalId=@GlobalId)
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
        INSERT dbo.UnitOfMeasures
        (GlobalId,Code,Name,Description,Symbol,MagnitudeCode,SortOrder,IsActive,IsDeleted,
         ExternalSystem,ExternalCode,CreatedByUserId,CreatedByUserName,CreatedAt)
        VALUES(@GlobalId,@Code,@Name,@Description,@Symbol,@MagnitudeCode,@SortOrder,@IsActive,0,
               @ExternalSystem,@ExternalCode,@CreatedByUserId,@CreatedByUserName,SYSUTCDATETIME());
        DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        SELECT N'UnitOfMeasures',CONVERT(nvarchar(80),@Id),N'INSERT',FieldName,NULL,NewValue,@CreatedByUserId,@CreatedByUserName
        FROM(VALUES
            (N'Code',CONVERT(nvarchar(max),@Code)),(N'Name',CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@Description)),(N'Symbol',CONVERT(nvarchar(max),@Symbol)),
            (N'MagnitudeCode',CONVERT(nvarchar(max),@MagnitudeCode)),(N'SortOrder',CONVERT(nvarchar(max),@SortOrder)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
            (N'ExternalSystem',CONVERT(nvarchar(max),@ExternalSystem)),(N'ExternalCode',CONVERT(nvarchar(max),@ExternalCode))
        ) valuesToAudit(FieldName,NewValue);
        IF @OwnTransaction=1 COMMIT; SELECT @Id;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_UNIT_OF_MEASURES_ACTUALIZAR
    @Id int,
    @Code nvarchar(50), @Name nvarchar(150), @Description nvarchar(500)=NULL,
    @Symbol nvarchar(20)=NULL, @MagnitudeCode nvarchar(20), @SortOrder int=0, @IsActive bit,
    @ExternalSystem nvarchar(50)=NULL, @ExternalCode nvarchar(100)=NULL,
    @UpdatedByUserId int=NULL, @UpdatedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    SET @Code=LTRIM(RTRIM(@Code)); SET @Name=LTRIM(RTRIM(@Name));
    SET @Symbol=NULLIF(LTRIM(RTRIM(@Symbol)),N''); SET @MagnitudeCode=LTRIM(RTRIM(@MagnitudeCode));
    SET @ExternalSystem=NULLIF(LTRIM(RTRIM(@ExternalSystem)),N''); SET @ExternalCode=NULLIF(LTRIM(RTRIM(@ExternalCode)),N'');
    IF NULLIF(@Code,N'') IS NULL THROW 51002,'El codigo es obligatorio.',1;
    IF NULLIF(@Name,N'') IS NULL THROW 51003,'El nombre es obligatorio.',1;
    IF @MagnitudeCode NOT IN(N'Quantity',N'Packaging',N'Mass',N'Volume',N'Length',N'Area',N'Time',N'Other') THROW 51194,'MagnitudeCode is invalid.',1;
    IF @SortOrder<0 THROW 51194,'SortOrder cannot be negative.',1;
    IF (@ExternalSystem IS NULL AND @ExternalCode IS NOT NULL) OR (@ExternalSystem IS NOT NULL AND @ExternalCode IS NULL)
        THROW 51194,'External system and code must be informed together.',1;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        DECLARE @OldCode nvarchar(50),@OldName nvarchar(150),@OldDescription nvarchar(500),@OldSymbol nvarchar(20),
                @OldMagnitudeCode nvarchar(20),@OldSortOrder int,@OldIsActive bit,
                @OldExternalSystem nvarchar(50),@OldExternalCode nvarchar(100);
        SELECT @OldCode=Code,@OldName=Name,@OldDescription=Description,@OldSymbol=Symbol,
               @OldMagnitudeCode=MagnitudeCode,@OldSortOrder=SortOrder,@OldIsActive=IsActive,
               @OldExternalSystem=ExternalSystem,@OldExternalCode=ExternalCode
        FROM dbo.UnitOfMeasures WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @OldCode IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;
        IF EXISTS(SELECT 1 FROM dbo.UnitOfMeasures WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND Id<>@Id)
        BEGIN IF @OwnTransaction=1 COMMIT; SELECT -1; RETURN; END;
        UPDATE dbo.UnitOfMeasures
        SET Code=@Code,Name=@Name,Description=@Description,Symbol=@Symbol,MagnitudeCode=@MagnitudeCode,
            SortOrder=@SortOrder,IsActive=@IsActive,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,
            UpdatedByUserId=@UpdatedByUserId,UpdatedByUserName=@UpdatedByUserName,UpdatedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;
        INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        SELECT N'UnitOfMeasures',CONVERT(nvarchar(80),@Id),N'UPDATE',FieldName,OldValue,NewValue,@UpdatedByUserId,@UpdatedByUserName
        FROM(VALUES
            (N'Code',CONVERT(nvarchar(max),@OldCode),CONVERT(nvarchar(max),@Code)),
            (N'Name',CONVERT(nvarchar(max),@OldName),CONVERT(nvarchar(max),@Name)),
            (N'Description',CONVERT(nvarchar(max),@OldDescription),CONVERT(nvarchar(max),@Description)),
            (N'Symbol',CONVERT(nvarchar(max),@OldSymbol),CONVERT(nvarchar(max),@Symbol)),
            (N'MagnitudeCode',CONVERT(nvarchar(max),@OldMagnitudeCode),CONVERT(nvarchar(max),@MagnitudeCode)),
            (N'SortOrder',CONVERT(nvarchar(max),@OldSortOrder),CONVERT(nvarchar(max),@SortOrder)),
            (N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),CONVERT(nvarchar(max),CONVERT(int,@IsActive))),
            (N'ExternalSystem',CONVERT(nvarchar(max),@OldExternalSystem),CONVERT(nvarchar(max),@ExternalSystem)),
            (N'ExternalCode',CONVERT(nvarchar(max),@OldExternalCode),CONVERT(nvarchar(max),@ExternalCode))
        ) valuesToAudit(FieldName,OldValue,NewValue)
        WHERE ISNULL(OldValue,N'')<>ISNULL(NewValue,N'');
        IF @OwnTransaction=1 COMMIT; SELECT 1;
    END TRY
    BEGIN CATCH
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        IF ERROR_NUMBER() IN(2601,2627) BEGIN SELECT -1; RETURN; END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_UNIT_OF_MEASURES_ELIMINAR
    @Id int, @DeletedByUserId int=NULL, @DeletedByUserName nvarchar(120)=NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRY
        DECLARE @OwnTransaction bit=CASE WHEN @@TRANCOUNT=0 THEN 1 ELSE 0 END;
        IF @OwnTransaction=1 BEGIN TRANSACTION;
        DECLARE @OldIsActive bit,@IsInUse bit=0;
        SELECT @OldIsActive=IsActive FROM dbo.UnitOfMeasures WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND IsDeleted=0;
        IF @OldIsActive IS NULL BEGIN IF @OwnTransaction=1 COMMIT; SELECT 0; RETURN; END;

        IF OBJECT_ID(N'dbo.Items',N'U') IS NOT NULL AND EXISTS
        (
            SELECT 1 FROM dbo.Items
            WHERE InventoryUnitOfMeasureId=@Id OR PurchaseUnitOfMeasureId=@Id OR SalesUnitOfMeasureId=@Id
        ) SET @IsInUse=1;
        IF OBJECT_ID(N'dbo.ItemBarcodes',N'U') IS NOT NULL AND EXISTS
        (SELECT 1 FROM dbo.ItemBarcodes WHERE UnitOfMeasureId=@Id) SET @IsInUse=1;

        /* Protege tambien referencias simples agregadas por modulos posteriores. */
        DECLARE @SchemaName sysname,@TableName sysname,@ColumnName sysname,@Sql nvarchar(max),@Referenced bit;
        DECLARE reference_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT OBJECT_SCHEMA_NAME(fk.parent_object_id),OBJECT_NAME(fk.parent_object_id),parentColumn.name
        FROM sys.foreign_keys fk
        JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id=fk.object_id
        JOIN sys.columns parentColumn ON parentColumn.object_id=fk.parent_object_id AND parentColumn.column_id=fkc.parent_column_id
        WHERE fk.referenced_object_id=OBJECT_ID(N'dbo.UnitOfMeasures')
          AND fk.parent_object_id NOT IN(OBJECT_ID(N'dbo.Items'),OBJECT_ID(N'dbo.ItemBarcodes'))
          AND 1=(SELECT COUNT(1) FROM sys.foreign_key_columns x WHERE x.constraint_object_id=fk.object_id);
        OPEN reference_cursor;
        FETCH NEXT FROM reference_cursor INTO @SchemaName,@TableName,@ColumnName;
        WHILE @@FETCH_STATUS=0 AND @IsInUse=0
        BEGIN
            SET @Referenced=0;
            SET @Sql=N'SELECT @Found=CASE WHEN EXISTS(SELECT 1 FROM '+QUOTENAME(@SchemaName)+N'.'+QUOTENAME(@TableName)+
                     N' WHERE '+QUOTENAME(@ColumnName)+N'=@UnitId) THEN 1 ELSE 0 END;';
            EXEC sys.sp_executesql @Sql,N'@UnitId int,@Found bit OUTPUT',@UnitId=@Id,@Found=@Referenced OUTPUT;
            IF @Referenced=1 SET @IsInUse=1;
            FETCH NEXT FROM reference_cursor INTO @SchemaName,@TableName,@ColumnName;
        END;
        CLOSE reference_cursor; DEALLOCATE reference_cursor;

        IF @IsInUse=1 BEGIN IF @OwnTransaction=1 COMMIT; SELECT -2; RETURN; END;

        UPDATE dbo.UnitOfMeasures
        SET IsActive=0,IsDeleted=1,DeletedByUserId=@DeletedByUserId,
            DeletedByUserName=@DeletedByUserName,DeletedAt=SYSUTCDATETIME()
        WHERE Id=@Id AND IsDeleted=0;
        INSERT dbo.AuditInventoryChanges(EntityName,RecordId,[Action],FieldName,OldValue,NewValue,UserId,UserName)
        VALUES
        (N'UnitOfMeasures',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsActive',CONVERT(nvarchar(max),CONVERT(int,@OldIsActive)),N'0',@DeletedByUserId,@DeletedByUserName),
        (N'UnitOfMeasures',CONVERT(nvarchar(80),@Id),N'DELETE',N'IsDeleted',N'0',N'1',@DeletedByUserId,@DeletedByUserName);
        IF @OwnTransaction=1 COMMIT; SELECT 1;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local','reference_cursor')>=0 CLOSE reference_cursor;
        IF CURSOR_STATUS('local','reference_cursor')>-3 DEALLOCATE reference_cursor;
        IF @OwnTransaction=1 AND XACT_STATE()<>0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.SchemaHistory WHERE Version=N'20260812.194')
    INSERT dbo.SchemaHistory(Version,Description)
    VALUES(N'20260812.194',N'Funcionaliza Unidades de medida con magnitud, simbolo, orden, CRUD y auditoria');
GO
