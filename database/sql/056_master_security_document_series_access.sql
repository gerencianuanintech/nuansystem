IF OBJECT_ID(N'dbo.SecurityRoleDocumentSeries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityRoleDocumentSeries
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityRoleDocumentSeries PRIMARY KEY,
        RoleId int NOT NULL,
        CompanyCode nvarchar(50) NOT NULL,
        FormKey nvarchar(120) NOT NULL,
        SecurityDocumentSeriesId int NOT NULL,
        DocumentType nvarchar(50) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeries_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeries_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeries_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityRoleDocumentSeries_Role FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityRoleDocumentSeries_Key' AND object_id = OBJECT_ID(N'dbo.SecurityRoleDocumentSeries'))
BEGIN
    CREATE UNIQUE INDEX UX_SecurityRoleDocumentSeries_Key
    ON dbo.SecurityRoleDocumentSeries (RoleId, CompanyCode, FormKey, DocumentType, SecurityDocumentSeriesId);
END;
GO

IF OBJECT_ID(N'dbo.SecurityRoleDocumentSeriesOperation', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SecurityRoleDocumentSeriesOperation
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SecurityRoleDocumentSeriesOperation PRIMARY KEY,
        SecurityRoleDocumentSeriesId int NOT NULL,
        OperationId int NULL,
        ActionKey nvarchar(120) NOT NULL,
        IsAllowed bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesOperation_IsAllowed DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesOperation_IsActive DEFAULT 1,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesOperation_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_SecurityRoleDocumentSeriesOperation_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_SecurityRoleDocumentSeriesOperation_Header FOREIGN KEY (SecurityRoleDocumentSeriesId) REFERENCES dbo.SecurityRoleDocumentSeries(Id),
        CONSTRAINT FK_SecurityRoleDocumentSeriesOperation_Operation FOREIGN KEY (OperationId) REFERENCES dbo.SecurityOperations(Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SecurityRoleDocumentSeriesOperation_Key' AND object_id = OBJECT_ID(N'dbo.SecurityRoleDocumentSeriesOperation'))
BEGIN
    CREATE UNIQUE INDEX UX_SecurityRoleDocumentSeriesOperation_Key
    ON dbo.SecurityRoleDocumentSeriesOperation (SecurityRoleDocumentSeriesId, ActionKey);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEDOCUMENTSERIES_SELECCIONADAS
    @RoleId int,
    @CompanyCode nvarchar(50),
    @FormKey nvarchar(120),
    @DocumentType nvarchar(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT SecurityDocumentSeriesId
    FROM dbo.SecurityRoleDocumentSeries
    WHERE RoleId = @RoleId
      AND CompanyCode = @CompanyCode
      AND FormKey = @FormKey
      AND (@DocumentType IS NULL OR DocumentType = @DocumentType)
      AND IsDeleted = 0
      AND IsActive = 1
    ORDER BY SecurityDocumentSeriesId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEDOCUMENTSERIES_OPERACIONES
    @RoleId int,
    @CompanyCode nvarchar(50),
    @FormKey nvarchar(120),
    @DocumentType nvarchar(50),
    @SecurityDocumentSeriesId int,
    @OnlyActive bit = 1,
    @Search nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HeaderId int =
    (
        SELECT TOP (1) Id
        FROM dbo.SecurityRoleDocumentSeries
        WHERE RoleId = @RoleId
          AND CompanyCode = @CompanyCode
          AND FormKey = @FormKey
          AND DocumentType = @DocumentType
          AND SecurityDocumentSeriesId = @SecurityDocumentSeriesId
          AND IsDeleted = 0
    );

    SELECT
        @HeaderId AS SecurityRoleDocumentSeriesId,
        operation.Id AS OperationId,
        operation.Code AS OperationCode,
        operation.Name AS OperationName,
        operation.Description AS OperationDescription,
        operation.ActionKey,
        operation.IconLarge,
        operation.IconSmall,
        operation.DisplayOrder,
        CAST(CASE WHEN accessOperation.IsAllowed = 1 THEN 1 ELSE 0 END AS bit) AS IsAllowed,
        accessOperation.UpdatedByUserId,
        accessOperation.UpdatedByUserName,
        accessOperation.UpdatedAt,
        accessOperation.CreatedByUserId,
        accessOperation.CreatedByUserName,
        accessOperation.CreatedAt
    FROM dbo.SecurityOperations operation
    LEFT JOIN dbo.SecurityRoleDocumentSeriesOperation accessOperation
        ON accessOperation.SecurityRoleDocumentSeriesId = @HeaderId
       AND accessOperation.ActionKey = operation.ActionKey
       AND accessOperation.IsDeleted = 0
    WHERE operation.IsDeleted = 0
      AND (@OnlyActive = 0 OR operation.IsActive = 1)
      AND operation.ActionKey IS NOT NULL
      AND
      (
          @Search IS NULL
          OR @Search = N''
          OR operation.Code LIKE N'%' + @Search + N'%'
          OR operation.Name LIKE N'%' + @Search + N'%'
          OR operation.ActionKey LIKE N'%' + @Search + N'%'
      )
    ORDER BY operation.DisplayOrder, operation.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SECURITYROLEDOCUMENTSERIES_GUARDAR
    @RoleId int,
    @CompanyCode nvarchar(50),
    @FormKey nvarchar(120),
    @DocumentType nvarchar(50),
    @SecurityDocumentSeriesId int,
    @IsSelected bit,
    @OperationsJson nvarchar(max),
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @HeaderId int;

    SELECT @HeaderId = Id
    FROM dbo.SecurityRoleDocumentSeries WITH (UPDLOCK, HOLDLOCK)
    WHERE RoleId = @RoleId
      AND CompanyCode = @CompanyCode
      AND FormKey = @FormKey
      AND DocumentType = @DocumentType
      AND SecurityDocumentSeriesId = @SecurityDocumentSeriesId;

    IF @HeaderId IS NULL
    BEGIN
        INSERT INTO dbo.SecurityRoleDocumentSeries
        (
            RoleId, CompanyCode, FormKey, SecurityDocumentSeriesId, DocumentType,
            IsActive, CreatedByUserId, CreatedByUserName, CreatedAt, IsDeleted
        )
        VALUES
        (
            @RoleId, @CompanyCode, @FormKey, @SecurityDocumentSeriesId, @DocumentType,
            @IsSelected, @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME(), 0
        );

        SET @HeaderId = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.SecurityRoleDocumentSeries
        SET IsActive = @IsSelected,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @HeaderId;
    END;

    DECLARE @Operations table
    (
        OperationId int NULL,
        ActionKey nvarchar(120) NOT NULL PRIMARY KEY,
        IsAllowed bit NOT NULL
    );

    INSERT INTO @Operations (OperationId, ActionKey, IsAllowed)
    SELECT OperationId, ActionKey, IsAllowed
    FROM OPENJSON(@OperationsJson)
    WITH
    (
        OperationId int '$.operationId',
        ActionKey nvarchar(120) '$.actionKey',
        IsAllowed bit '$.isAllowed'
    )
    WHERE ActionKey IS NOT NULL;

    MERGE dbo.SecurityRoleDocumentSeriesOperation AS target
    USING @Operations AS source
        ON target.SecurityRoleDocumentSeriesId = @HeaderId
       AND target.ActionKey = source.ActionKey
    WHEN MATCHED THEN
        UPDATE SET
            OperationId = source.OperationId,
            IsAllowed = source.IsAllowed,
            IsActive = 1,
            IsDeleted = 0,
            DeletedByUserId = NULL,
            DeletedByUserName = NULL,
            DeletedAt = NULL,
            UpdatedByUserId = @UpdatedByUserId,
            UpdatedByUserName = @UpdatedByUserName,
            UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT
        (
            SecurityRoleDocumentSeriesId, OperationId, ActionKey, IsAllowed, IsActive,
            CreatedByUserId, CreatedByUserName, CreatedAt
        )
        VALUES
        (
            @HeaderId, source.OperationId, source.ActionKey, source.IsAllowed, 1,
            @UpdatedByUserId, @UpdatedByUserName, SYSUTCDATETIME()
        );

    COMMIT TRANSACTION;

    SELECT CAST(1 AS bit);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_VALIDAROPERACION
    @RoleId int,
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
        FROM dbo.SecurityRoleDocumentSeries header
        INNER JOIN dbo.SecurityRoleDocumentSeriesOperation operationAccess
            ON operationAccess.SecurityRoleDocumentSeriesId = header.Id
           AND operationAccess.IsDeleted = 0
           AND operationAccess.IsActive = 1
           AND operationAccess.IsAllowed = 1
           AND operationAccess.ActionKey = @ActionKey
        WHERE header.RoleId = @RoleId
          AND header.CompanyCode = @CompanyCode
          AND header.FormKey = @FormKey
          AND header.DocumentType = @DocumentType
          AND header.SecurityDocumentSeriesId = @SecurityDocumentSeriesId
          AND header.IsDeleted = 0
          AND header.IsActive = 1
    ) THEN 1 ELSE 0 END AS bit) AS IsAllowed;
END;
GO
