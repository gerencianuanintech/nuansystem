/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Crea el plan de cuentas contable con estructura jerarquica y SPs CRUD.
*/

IF OBJECT_ID(N'dbo.ChartOfAccounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChartOfAccounts
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChartOfAccounts PRIMARY KEY,
        CompanyId int NOT NULL,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(500) NULL,
        ExternalCode nvarchar(50) NULL,
        AccountType nvarchar(30) NOT NULL,
        AccountClass nvarchar(30) NULL,
        ParentAccountId int NULL,
        [Level] int NOT NULL CONSTRAINT DF_ChartOfAccounts_Level DEFAULT 1,
        IsTitle bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsTitle DEFAULT 0,
        AllowsMovement bit NOT NULL CONSTRAINT DF_ChartOfAccounts_AllowsMovement DEFAULT 1,
        IsActive bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsActive DEFAULT 1,
        CurrencyCode nvarchar(3) NULL,
        Balance decimal(19,6) NOT NULL CONSTRAINT DF_ChartOfAccounts_Balance DEFAULT 0,
        IsConfidential bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsConfidential DEFAULT 0,
        IsMonetaryAccount bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsMonetaryAccount DEFAULT 0,
        IsAssociatedAccount bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsAssociatedAccount DEFAULT 0,
        RevalueByIndex bit NOT NULL CONSTRAINT DF_ChartOfAccounts_RevalueByIndex DEFAULT 0,
        BlockManualPosting bit NOT NULL CONSTRAINT DF_ChartOfAccounts_BlockManualPosting DEFAULT 0,
        RelevantForCashFlow bit NOT NULL CONSTRAINT DF_ChartOfAccounts_RelevantForCashFlow DEFAULT 0,
        RequiresCostCenter bit NOT NULL CONSTRAINT DF_ChartOfAccounts_RequiresCostCenter DEFAULT 0,
        RequiresThirdParty bit NOT NULL CONSTRAINT DF_ChartOfAccounts_RequiresThirdParty DEFAULT 0,
        RequiresProject bit NOT NULL CONSTRAINT DF_ChartOfAccounts_RequiresProject DEFAULT 0,
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(120) NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_ChartOfAccounts_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(120) NULL,
        UpdatedAt datetime2(0) NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsDeleted DEFAULT 0,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(120) NULL,
        DeletedAt datetime2(0) NULL,
        CONSTRAINT FK_ChartOfAccounts_Parent FOREIGN KEY (ParentAccountId) REFERENCES dbo.ChartOfAccounts(Id),
        CONSTRAINT CK_ChartOfAccounts_AccountType CHECK (AccountType IN (N'ASSET', N'LIABILITY', N'EQUITY', N'INCOME', N'EXPENSE', N'COST', N'ORDER')),
        CONSTRAINT CK_ChartOfAccounts_NoSelfParent CHECK (ParentAccountId IS NULL OR ParentAccountId <> Id)
    );
END;
GO

IF COL_LENGTH(N'dbo.ChartOfAccounts', N'ExternalCode') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD ExternalCode nvarchar(50) NULL;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'AccountClass') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD AccountClass nvarchar(30) NULL;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'IsTitle') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD IsTitle bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsTitle DEFAULT 0;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'Balance') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD Balance decimal(19,6) NOT NULL CONSTRAINT DF_ChartOfAccounts_Balance DEFAULT 0;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'IsConfidential') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD IsConfidential bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsConfidential DEFAULT 0;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'IsMonetaryAccount') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD IsMonetaryAccount bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsMonetaryAccount DEFAULT 0;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'IsAssociatedAccount') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD IsAssociatedAccount bit NOT NULL CONSTRAINT DF_ChartOfAccounts_IsAssociatedAccount DEFAULT 0;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'RevalueByIndex') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD RevalueByIndex bit NOT NULL CONSTRAINT DF_ChartOfAccounts_RevalueByIndex DEFAULT 0;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'BlockManualPosting') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD BlockManualPosting bit NOT NULL CONSTRAINT DF_ChartOfAccounts_BlockManualPosting DEFAULT 0;
IF COL_LENGTH(N'dbo.ChartOfAccounts', N'RelevantForCashFlow') IS NULL ALTER TABLE dbo.ChartOfAccounts ADD RelevantForCashFlow bit NOT NULL CONSTRAINT DF_ChartOfAccounts_RelevantForCashFlow DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ChartOfAccounts_Company_Code_Active' AND object_id = OBJECT_ID(N'dbo.ChartOfAccounts'))
BEGIN
    CREATE UNIQUE INDEX UX_ChartOfAccounts_Company_Code_Active ON dbo.ChartOfAccounts (CompanyId, Code) WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ChartOfAccounts_Parent' AND object_id = OBJECT_ID(N'dbo.ChartOfAccounts'))
BEGIN
    CREATE INDEX IX_ChartOfAccounts_Parent ON dbo.ChartOfAccounts (ParentAccountId) WHERE IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CHART_OF_ACCOUNTS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        account.Id,
        account.CompanyId,
        account.Code,
        account.Name,
        account.Description,
        account.ExternalCode,
        account.AccountType,
        account.AccountClass,
        account.ParentAccountId,
        parent.Code AS ParentAccountCode,
        parent.Name AS ParentAccountName,
        account.[Level],
        account.IsTitle,
        account.AllowsMovement,
        account.IsActive,
        account.CurrencyCode,
        account.Balance,
        account.IsConfidential,
        account.IsMonetaryAccount,
        account.IsAssociatedAccount,
        account.RevalueByIndex,
        account.BlockManualPosting,
        account.RelevantForCashFlow,
        account.RequiresCostCenter,
        account.RequiresThirdParty,
        account.RequiresProject,
        account.CreatedByUserId,
        account.CreatedByUserName,
        account.CreatedAt,
        account.UpdatedByUserId,
        account.UpdatedByUserName,
        account.UpdatedAt,
        account.DeletedByUserId,
        account.DeletedByUserName,
        account.DeletedAt
    FROM dbo.ChartOfAccounts account
    LEFT JOIN dbo.ChartOfAccounts parent ON parent.Id = account.ParentAccountId
    WHERE account.IsDeleted = 0
    ORDER BY account.Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CHART_OF_ACCOUNTS_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Code,
        Name,
        AccountType,
        ParentAccountId,
        [Level],
        IsActive
    FROM dbo.ChartOfAccounts
    WHERE IsDeleted = 0
    ORDER BY Code;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CHART_OF_ACCOUNTS_BUSCARPORID
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        account.Id,
        account.CompanyId,
        account.Code,
        account.Name,
        account.Description,
        account.ExternalCode,
        account.AccountType,
        account.AccountClass,
        account.ParentAccountId,
        parent.Code AS ParentAccountCode,
        parent.Name AS ParentAccountName,
        account.[Level],
        account.IsTitle,
        account.AllowsMovement,
        account.IsActive,
        account.CurrencyCode,
        account.Balance,
        account.IsConfidential,
        account.IsMonetaryAccount,
        account.IsAssociatedAccount,
        account.RevalueByIndex,
        account.BlockManualPosting,
        account.RelevantForCashFlow,
        account.RequiresCostCenter,
        account.RequiresThirdParty,
        account.RequiresProject,
        account.CreatedByUserId,
        account.CreatedByUserName,
        account.CreatedAt,
        account.UpdatedByUserId,
        account.UpdatedByUserName,
        account.UpdatedAt,
        account.DeletedByUserId,
        account.DeletedByUserName,
        account.DeletedAt
    FROM dbo.ChartOfAccounts account
    LEFT JOIN dbo.ChartOfAccounts parent ON parent.Id = account.ParentAccountId
    WHERE account.Id = @Id
      AND account.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CHART_OF_ACCOUNTSBUSCARPORCODIGO
    @CompanyId int,
    @Code nvarchar(50),
    @ExcluirId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.ChartOfAccounts
    WHERE CompanyId = @CompanyId
      AND Code = @Code
      AND IsDeleted = 0
      AND (@ExcluirId IS NULL OR Id <> @ExcluirId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_CHART_OF_ACCOUNTS_TIENEHIJAS
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.ChartOfAccounts
    WHERE ParentAccountId = @Id
      AND IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_CHART_OF_ACCOUNTS_CREAR
    @CompanyId int,
    @Code nvarchar(50),
    @Name nvarchar(200),
    @Description nvarchar(500) = NULL,
    @ExternalCode nvarchar(50) = NULL,
    @AccountType nvarchar(30),
    @AccountClass nvarchar(30) = NULL,
    @ParentAccountId int = NULL,
    @IsTitle bit,
    @AllowsMovement bit,
    @IsActive bit,
    @CurrencyCode nvarchar(3) = NULL,
    @Balance decimal(19,6),
    @IsConfidential bit,
    @IsMonetaryAccount bit,
    @IsAssociatedAccount bit,
    @RevalueByIndex bit,
    @BlockManualPosting bit,
    @RelevantForCashFlow bit,
    @RequiresCostCenter bit,
    @RequiresThirdParty bit,
    @RequiresProject bit,
    @CreatedByUserId int = NULL,
    @CreatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Level int = 1;
    IF @ParentAccountId IS NOT NULL
    BEGIN
        SELECT @Level = [Level] + 1
        FROM dbo.ChartOfAccounts
        WHERE Id = @ParentAccountId AND CompanyId = @CompanyId AND IsDeleted = 0;
    END;

    INSERT INTO dbo.ChartOfAccounts
    (
        CompanyId,
        Code,
        Name,
        Description,
        ExternalCode,
        AccountType,
        AccountClass,
        ParentAccountId,
        [Level],
        IsTitle,
        AllowsMovement,
        IsActive,
        CurrencyCode,
        Balance,
        IsConfidential,
        IsMonetaryAccount,
        IsAssociatedAccount,
        RevalueByIndex,
        BlockManualPosting,
        RelevantForCashFlow,
        RequiresCostCenter,
        RequiresThirdParty,
        RequiresProject,
        CreatedByUserId,
        CreatedByUserName
    )
    VALUES
    (
        @CompanyId,
        @Code,
        @Name,
        @Description,
        @ExternalCode,
        @AccountType,
        @AccountClass,
        @ParentAccountId,
        @Level,
        @IsTitle,
        @AllowsMovement,
        @IsActive,
        @CurrencyCode,
        @Balance,
        @IsConfidential,
        @IsMonetaryAccount,
        @IsAssociatedAccount,
        @RevalueByIndex,
        @BlockManualPosting,
        @RelevantForCashFlow,
        @RequiresCostCenter,
        @RequiresThirdParty,
        @RequiresProject,
        @CreatedByUserId,
        @CreatedByUserName
    );

    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_CHART_OF_ACCOUNTS_ACTUALIZAR
    @Id int,
    @CompanyId int,
    @Code nvarchar(50),
    @Name nvarchar(200),
    @Description nvarchar(500) = NULL,
    @ExternalCode nvarchar(50) = NULL,
    @AccountType nvarchar(30),
    @AccountClass nvarchar(30) = NULL,
    @ParentAccountId int = NULL,
    @IsTitle bit,
    @AllowsMovement bit,
    @IsActive bit,
    @CurrencyCode nvarchar(3) = NULL,
    @Balance decimal(19,6),
    @IsConfidential bit,
    @IsMonetaryAccount bit,
    @IsAssociatedAccount bit,
    @RevalueByIndex bit,
    @BlockManualPosting bit,
    @RelevantForCashFlow bit,
    @RequiresCostCenter bit,
    @RequiresThirdParty bit,
    @RequiresProject bit,
    @UpdatedByUserId int = NULL,
    @UpdatedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Level int = 1;
    IF @ParentAccountId IS NOT NULL
    BEGIN
        SELECT @Level = [Level] + 1
        FROM dbo.ChartOfAccounts
        WHERE Id = @ParentAccountId AND CompanyId = @CompanyId AND IsDeleted = 0;
    END;

    UPDATE dbo.ChartOfAccounts
    SET
        CompanyId = @CompanyId,
        Code = @Code,
        Name = @Name,
        Description = @Description,
        ExternalCode = @ExternalCode,
        AccountType = @AccountType,
        AccountClass = @AccountClass,
        ParentAccountId = @ParentAccountId,
        [Level] = @Level,
        IsTitle = @IsTitle,
        AllowsMovement = @AllowsMovement,
        IsActive = @IsActive,
        CurrencyCode = @CurrencyCode,
        Balance = @Balance,
        IsConfidential = @IsConfidential,
        IsMonetaryAccount = @IsMonetaryAccount,
        IsAssociatedAccount = @IsAssociatedAccount,
        RevalueByIndex = @RevalueByIndex,
        BlockManualPosting = @BlockManualPosting,
        RelevantForCashFlow = @RelevantForCashFlow,
        RequiresCostCenter = @RequiresCostCenter,
        RequiresThirdParty = @RequiresThirdParty,
        RequiresProject = @RequiresProject,
        UpdatedByUserId = @UpdatedByUserId,
        UpdatedByUserName = @UpdatedByUserName,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_CHART_OF_ACCOUNTS_ELIMINAR
    @Id int,
    @DeletedByUserId int = NULL,
    @DeletedByUserName nvarchar(120) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ChartOfAccounts
    SET
        IsDeleted = 1,
        DeletedByUserId = @DeletedByUserId,
        DeletedByUserName = @DeletedByUserName,
        DeletedAt = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO
