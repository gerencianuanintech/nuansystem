/*
    Ejecutar en cada base tenant despues de 106 y 107.
    Endurece Transportistas para instalaciones donde 107 ya fue aplicado:
    - evita exito/auditoria falsos en update/delete concurrentes;
    - convierte la colision concurrente de Code en resultado -1;
    - agrega checks de Code/Name no vacios.
*/
IF OBJECT_ID(N'dbo.Carriers', N'U') IS NULL
    THROW 51010, 'Debe ejecutar primero 107_tenant_carriers.sql.', 1;
GO

IF EXISTS
(
    SELECT 1
    FROM dbo.Carriers
    WHERE LEN(LTRIM(RTRIM(Code))) = 0
       OR LEN(LTRIM(RTRIM(Name))) = 0
)
    THROW 51011, 'Existen transportistas con codigo o nombre vacio. Corrija esos registros antes de continuar.', 1;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.Carriers')
      AND name = N'CK_Carriers_Code_NotBlank'
)
    ALTER TABLE dbo.Carriers WITH CHECK
        ADD CONSTRAINT CK_Carriers_Code_NotBlank CHECK (LEN(LTRIM(RTRIM(Code))) > 0);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.Carriers')
      AND name = N'CK_Carriers_Name_NotBlank'
)
    ALTER TABLE dbo.Carriers WITH CHECK
        ADD CONSTRAINT CK_Carriers_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0);
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CARRIERS_CREAR
    @Code nvarchar(50), @Name nvarchar(150), @IdentificationTypeCode nvarchar(2),
    @IdentificationNumber nvarchar(30), @Description nvarchar(500) = NULL, @IsActive bit,
    @AuditUserId int = NULL, @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NULLIF(LTRIM(RTRIM(@Code)), N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @IdentificationTypeCode NOT IN (N'04', N'05', N'06') THROW 51000, 'Tipo de identificacion no permitido.', 1;
    IF NULLIF(LTRIM(RTRIM(@IdentificationNumber)), N'') IS NULL THROW 51001, 'La identificacion es obligatoria.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
            WHERE Code = @Code AND IsDeleted = 0
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1;
            RETURN;
        END;

        INSERT dbo.Carriers (Code, Name, IdentificationTypeCode, IdentificationNumber, Description, IsActive, CreatedByUserId, CreatedByUserName, CreatedAt)
        VALUES (@Code, @Name, @IdentificationTypeCode, @IdentificationNumber, @Description, @IsActive, @AuditUserId, @AuditUserName, SYSUTCDATETIME());

        DECLARE @Id int = CONVERT(int, SCOPE_IDENTITY());
        INSERT dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
        SELECT N'Carrier', CONVERT(nvarchar(80), @Id), N'INSERT', FieldName, NULL, NewValue, @AuditUserId, @AuditUserName
        FROM (VALUES
            (N'Code', CONVERT(nvarchar(max), @Code)),
            (N'Name', CONVERT(nvarchar(max), @Name)),
            (N'IdentificationTypeCode', CONVERT(nvarchar(max), @IdentificationTypeCode)),
            (N'IdentificationNumber', CONVERT(nvarchar(max), @IdentificationNumber)),
            (N'Description', CONVERT(nvarchar(max), @Description)),
            (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @IsActive)))) changes(FieldName, NewValue);

        COMMIT TRANSACTION;
        SELECT @Id;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CARRIERS_ACTUALIZAR
    @Id int, @Code nvarchar(50), @Name nvarchar(150), @IdentificationTypeCode nvarchar(2),
    @IdentificationNumber nvarchar(30), @Description nvarchar(500) = NULL, @IsActive bit,
    @AuditUserId int = NULL, @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NULLIF(LTRIM(RTRIM(@Code)), N'') IS NULL THROW 51002, 'El codigo es obligatorio.', 1;
    IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL THROW 51003, 'El nombre es obligatorio.', 1;
    IF @IdentificationTypeCode NOT IN (N'04', N'05', N'06') THROW 51000, 'Tipo de identificacion no permitido.', 1;
    IF NULLIF(LTRIM(RTRIM(@IdentificationNumber)), N'') IS NULL THROW 51001, 'La identificacion es obligatoria.', 1;

    BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @OldCode nvarchar(50), @OldName nvarchar(150), @OldType nvarchar(2), @OldNumber nvarchar(30), @OldDescription nvarchar(500), @OldIsActive bit;
    SELECT @OldCode = Code, @OldName = Name, @OldType = IdentificationTypeCode, @OldNumber = IdentificationNumber, @OldDescription = Description, @OldIsActive = IsActive
    FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
    WHERE Id = @Id AND IsDeleted = 0;
    IF @OldCode IS NULL BEGIN ROLLBACK TRANSACTION; SELECT 0; RETURN; END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
        WHERE Code = @Code AND Id <> @Id AND IsDeleted = 0
    )
    BEGIN
        ROLLBACK TRANSACTION;
        SELECT -1;
        RETURN;
    END;

    UPDATE dbo.Carriers
    SET Code = @Code, Name = @Name, IdentificationTypeCode = @IdentificationTypeCode,
        IdentificationNumber = @IdentificationNumber, Description = @Description, IsActive = @IsActive,
        UpdatedByUserId = @AuditUserId, UpdatedByUserName = @AuditUserName, UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;
    IF @@ROWCOUNT = 0 BEGIN ROLLBACK TRANSACTION; SELECT 0; RETURN; END;

    INSERT dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    SELECT N'Carrier', CONVERT(nvarchar(80), @Id), N'UPDATE', FieldName, OldValue, NewValue, @AuditUserId, @AuditUserName
    FROM (VALUES
        (N'Code', CONVERT(nvarchar(max), @OldCode), CONVERT(nvarchar(max), @Code)),
        (N'Name', CONVERT(nvarchar(max), @OldName), CONVERT(nvarchar(max), @Name)),
        (N'IdentificationTypeCode', CONVERT(nvarchar(max), @OldType), CONVERT(nvarchar(max), @IdentificationTypeCode)),
        (N'IdentificationNumber', CONVERT(nvarchar(max), @OldNumber), CONVERT(nvarchar(max), @IdentificationNumber)),
        (N'Description', CONVERT(nvarchar(max), @OldDescription), CONVERT(nvarchar(max), @Description)),
        (N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), CONVERT(nvarchar(max), CONVERT(int, @IsActive)))) changes(FieldName, OldValue, NewValue)
    WHERE ISNULL(OldValue, N'') <> ISNULL(NewValue, N'');

    COMMIT TRANSACTION;
    SELECT 1;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SELECT -1;
            RETURN;
        END;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_CARRIERS_ELIMINAR
    @Id int, @AuditUserId int = NULL, @AuditUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    DECLARE @OldIsActive bit;
    SELECT @OldIsActive = IsActive
    FROM dbo.Carriers WITH (UPDLOCK, HOLDLOCK)
    WHERE Id = @Id AND IsDeleted = 0;
    IF @OldIsActive IS NULL BEGIN ROLLBACK TRANSACTION; SELECT 0; RETURN; END;

    UPDATE dbo.Carriers
    SET IsActive = 0, IsDeleted = 1, DeletedByUserId = @AuditUserId,
        DeletedByUserName = @AuditUserName, DeletedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND IsDeleted = 0;
    IF @@ROWCOUNT = 0 BEGIN ROLLBACK TRANSACTION; SELECT 0; RETURN; END;

    INSERT dbo.AuditCatalogChanges (EntityName, RecordId, [Action], FieldName, OldValue, NewValue, UserId, UserName)
    VALUES
        (N'Carrier', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsActive', CONVERT(nvarchar(max), CONVERT(int, @OldIsActive)), N'0', @AuditUserId, @AuditUserName),
        (N'Carrier', CONVERT(nvarchar(80), @Id), N'DELETE', N'IsDeleted', N'0', N'1', @AuditUserId, @AuditUserName);

    COMMIT TRANSACTION;
    SELECT 1;
END;
GO
