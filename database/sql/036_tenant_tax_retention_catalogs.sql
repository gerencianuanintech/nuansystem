SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.TaxRegimes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaxRegimes
    (
        TaxRegimeId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TaxRegimes PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_TaxRegimes_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_TaxRegimes_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_TaxRegimes_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.TaxpayerTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaxpayerTypes
    (
        TaxpayerTypeId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TaxpayerTypes PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_TaxpayerTypes_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_TaxpayerTypes_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_TaxpayerTypes_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.RetentionTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RetentionTypes
    (
        RetentionTypeId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RetentionTypes PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_RetentionTypes_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_RetentionTypes_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_RetentionTypes_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.TaxSupports', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaxSupports
    (
        TaxSupportId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TaxSupports PRIMARY KEY,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        IsActive bit NOT NULL CONSTRAINT DF_TaxSupports_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_TaxSupports_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_TaxSupports_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.RetentionConcepts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RetentionConcepts
    (
        RetentionConceptId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RetentionConcepts PRIMARY KEY,
        RetentionTypeId int NULL,
        Code nvarchar(30) NOT NULL,
        Name nvarchar(160) NOT NULL,
        Description nvarchar(300) NULL,
        SriCode nvarchar(30) NULL,
        [Percent] decimal(9,4) NOT NULL CONSTRAINT DF_RetentionConcepts_Percent DEFAULT 0,
        AppliesIva bit NOT NULL CONSTRAINT DF_RetentionConcepts_AppliesIva DEFAULT 0,
        AppliesIncome bit NOT NULL CONSTRAINT DF_RetentionConcepts_AppliesIncome DEFAULT 0,
        IsActive bit NOT NULL CONSTRAINT DF_RetentionConcepts_IsActive DEFAULT 1,
        IsDeleted bit NOT NULL CONSTRAINT DF_RetentionConcepts_IsDeleted DEFAULT 0,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_RetentionConcepts_CreatedAt DEFAULT SYSUTCDATETIME(),
        CreatedByUserId int NULL,
        CreatedByUserName nvarchar(100) NULL,
        UpdatedAt datetime2(0) NULL,
        UpdatedByUserId int NULL,
        UpdatedByUserName nvarchar(100) NULL,
        DeletedAt datetime2(0) NULL,
        DeletedByUserId int NULL,
        DeletedByUserName nvarchar(100) NULL,
        CONSTRAINT FK_RetentionConcepts_RetentionTypes FOREIGN KEY (RetentionTypeId) REFERENCES dbo.RetentionTypes(RetentionTypeId)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_TaxRegimes_Code' AND object_id = OBJECT_ID(N'dbo.TaxRegimes'))
    CREATE UNIQUE INDEX UX_TaxRegimes_Code ON dbo.TaxRegimes(Code) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_TaxpayerTypes_Code' AND object_id = OBJECT_ID(N'dbo.TaxpayerTypes'))
    CREATE UNIQUE INDEX UX_TaxpayerTypes_Code ON dbo.TaxpayerTypes(Code) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RetentionTypes_Code' AND object_id = OBJECT_ID(N'dbo.RetentionTypes'))
    CREATE UNIQUE INDEX UX_RetentionTypes_Code ON dbo.RetentionTypes(Code) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_TaxSupports_Code' AND object_id = OBJECT_ID(N'dbo.TaxSupports'))
    CREATE UNIQUE INDEX UX_TaxSupports_Code ON dbo.TaxSupports(Code) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RetentionConcepts_Code' AND object_id = OBJECT_ID(N'dbo.RetentionConcepts'))
    CREATE UNIQUE INDEX UX_RetentionConcepts_Code ON dbo.RetentionConcepts(Code) WHERE IsDeleted = 0;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.TaxRegimes WHERE Code = N'GENERAL')
    INSERT INTO dbo.TaxRegimes (Code, Name, Description, CreatedByUserName) VALUES (N'GENERAL', N'Regimen general', N'Regimen tributario general.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.TaxRegimes WHERE Code = N'ESPECIAL')
    INSERT INTO dbo.TaxRegimes (Code, Name, Description, CreatedByUserName) VALUES (N'ESPECIAL', N'Regimen especial', N'Regimen especial de contribuyente.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.TaxpayerTypes WHERE Code = N'SOCIEDAD')
    INSERT INTO dbo.TaxpayerTypes (Code, Name, Description, CreatedByUserName) VALUES (N'SOCIEDAD', N'Sociedad', N'Persona juridica o sociedad.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.TaxpayerTypes WHERE Code = N'NATURAL')
    INSERT INTO dbo.TaxpayerTypes (Code, Name, Description, CreatedByUserName) VALUES (N'NATURAL', N'Persona natural', N'Persona natural.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.RetentionTypes WHERE Code = N'FUENTE')
    INSERT INTO dbo.RetentionTypes (Code, Name, Description, CreatedByUserName) VALUES (N'FUENTE', N'Retencion Fuente', N'Retencion de impuesto a la renta.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.RetentionTypes WHERE Code = N'IVA')
    INSERT INTO dbo.RetentionTypes (Code, Name, Description, CreatedByUserName) VALUES (N'IVA', N'Retencion IVA', N'Retencion de IVA.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.TaxSupports WHERE Code = N'FACTURA')
    INSERT INTO dbo.TaxSupports (Code, Name, Description, CreatedByUserName) VALUES (N'FACTURA', N'Factura', N'Sustento tributario factura.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.TaxSupports WHERE Code = N'LIQCOMPRA')
    INSERT INTO dbo.TaxSupports (Code, Name, Description, CreatedByUserName) VALUES (N'LIQCOMPRA', N'Liquidacion de compra', N'Sustento tributario liquidacion de compra.', N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.TaxSupports WHERE Code = N'NOTACRED')
    INSERT INTO dbo.TaxSupports (Code, Name, Description, CreatedByUserName) VALUES (N'NOTACRED', N'Nota de credito', N'Sustento tributario nota de credito.', N'System');
GO

DECLARE @RetentionFuenteId int = (SELECT TOP (1) RetentionTypeId FROM dbo.RetentionTypes WHERE Code = N'FUENTE' AND IsDeleted = 0);
DECLARE @RetentionIvaId int = (SELECT TOP (1) RetentionTypeId FROM dbo.RetentionTypes WHERE Code = N'IVA' AND IsDeleted = 0);

IF NOT EXISTS (SELECT 1 FROM dbo.RetentionConcepts WHERE Code = N'312')
    INSERT INTO dbo.RetentionConcepts (RetentionTypeId, Code, Name, SriCode, [Percent], AppliesIncome, CreatedByUserName)
    VALUES (@RetentionFuenteId, N'312', N'Retencion Fuente 1.75%', N'312', 1.7500, 1, N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.RetentionConcepts WHERE Code = N'723')
    INSERT INTO dbo.RetentionConcepts (RetentionTypeId, Code, Name, SriCode, [Percent], AppliesIva, CreatedByUserName)
    VALUES (@RetentionIvaId, N'723', N'Retencion IVA 30%', N'723', 30.0000, 1, N'System');
IF NOT EXISTS (SELECT 1 FROM dbo.RetentionConcepts WHERE Code = N'724')
    INSERT INTO dbo.RetentionConcepts (RetentionTypeId, Code, Name, SriCode, [Percent], AppliesIva, CreatedByUserName)
    VALUES (@RetentionIvaId, N'724', N'Retencion IVA 70%', N'724', 70.0000, 1, N'System');
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXREGIMES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TaxRegimeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.TaxRegimes WHERE IsDeleted = 0 ORDER BY Name;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXPAYERTYPES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TaxpayerTypeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.TaxpayerTypes WHERE IsDeleted = 0 ORDER BY Name;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONTYPES_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RetentionTypeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.RetentionTypes WHERE IsDeleted = 0 ORDER BY Name;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXSUPPORTS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TaxSupportId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.TaxSupports WHERE IsDeleted = 0 ORDER BY Name;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONCONCEPTS_LISTAR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT rc.RetentionConceptId AS Id, rc.RetentionTypeId, rt.Name AS RetentionTypeName, rc.Code, rc.Name, rc.Description, rc.SriCode,
           rc.[Percent], rc.AppliesIva, rc.AppliesIncome, rc.IsActive, rc.CreatedAt, rc.CreatedByUserId, rc.CreatedByUserName,
           rc.UpdatedAt, rc.UpdatedByUserId, rc.UpdatedByUserName
    FROM dbo.RetentionConcepts rc
    LEFT JOIN dbo.RetentionTypes rt ON rt.RetentionTypeId = rc.RetentionTypeId
    WHERE rc.IsDeleted = 0
    ORDER BY rc.SriCode, rc.Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXREGIMES_BUSCARPORID @Id int AS
BEGIN
    SET NOCOUNT ON;
    SELECT TaxRegimeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.TaxRegimes WHERE TaxRegimeId = @Id AND IsDeleted = 0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXPAYERTYPES_BUSCARPORID @Id int AS
BEGIN
    SET NOCOUNT ON;
    SELECT TaxpayerTypeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.TaxpayerTypes WHERE TaxpayerTypeId = @Id AND IsDeleted = 0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONTYPES_BUSCARPORID @Id int AS
BEGIN
    SET NOCOUNT ON;
    SELECT RetentionTypeId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.RetentionTypes WHERE RetentionTypeId = @Id AND IsDeleted = 0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXSUPPORTS_BUSCARPORID @Id int AS
BEGIN
    SET NOCOUNT ON;
    SELECT TaxSupportId AS Id, Code, Name, Description, IsActive, CreatedAt, CreatedByUserId, CreatedByUserName, UpdatedAt, UpdatedByUserId, UpdatedByUserName
    FROM dbo.TaxSupports WHERE TaxSupportId = @Id AND IsDeleted = 0;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONCONCEPTS_BUSCARPORID @Id int AS
BEGIN
    SET NOCOUNT ON;
    SELECT rc.RetentionConceptId AS Id, rc.RetentionTypeId, rt.Name AS RetentionTypeName, rc.Code, rc.Name, rc.Description, rc.SriCode,
           rc.[Percent], rc.AppliesIva, rc.AppliesIncome, rc.IsActive, rc.CreatedAt, rc.CreatedByUserId, rc.CreatedByUserName,
           rc.UpdatedAt, rc.UpdatedByUserId, rc.UpdatedByUserName
    FROM dbo.RetentionConcepts rc
    LEFT JOIN dbo.RetentionTypes rt ON rt.RetentionTypeId = rc.RetentionTypeId
    WHERE rc.RetentionConceptId = @Id AND rc.IsDeleted = 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXREGIMES_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT TaxRegimeId AS Id, Code, Name, IsActive FROM dbo.TaxRegimes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXPAYERTYPES_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT TaxpayerTypeId AS Id, Code, Name, IsActive FROM dbo.TaxpayerTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONTYPES_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT RetentionTypeId AS Id, Code, Name, IsActive FROM dbo.RetentionTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXSUPPORTS_LOOKUP AS BEGIN SET NOCOUNT ON; SELECT TaxSupportId AS Id, Code, Name, IsActive FROM dbo.TaxSupports WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONCONCEPTS_LOOKUP
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RetentionConceptId AS Id, Code, Name, IsActive, SriCode, [Percent], AppliesIva, AppliesIncome
    FROM dbo.RetentionConcepts
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY SriCode, Name;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXREGIMES_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.TaxRegimes WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR TaxRegimeId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXPAYERTYPES_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.TaxpayerTypes WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR TaxpayerTypeId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONTYPES_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.RetentionTypes WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR RetentionTypeId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_TAXSUPPORTS_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.TaxSupports WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR TaxSupportId <> @ExcluirId); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_RETENTIONCONCEPTS_BUSCARPORCODIGO @Code nvarchar(30), @ExcluirId int = NULL AS BEGIN SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.RetentionConcepts WHERE IsDeleted = 0 AND Code = @Code AND (@ExcluirId IS NULL OR RetentionConceptId <> @ExcluirId); END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_TAXREGIMES_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; INSERT INTO dbo.TaxRegimes (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_TAXPAYERTYPES_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; INSERT INTO dbo.TaxpayerTypes (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_RETENTIONTYPES_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; INSERT INTO dbo.RetentionTypes (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_TAXSUPPORTS_CREAR @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; INSERT INTO dbo.TaxSupports (Code, Name, Description, IsActive, CreatedByUserId, CreatedByUserName) VALUES (@Code, @Name, @Description, @IsActive, @CreatedByUserId, @CreatedByUserName); SELECT CONVERT(int, SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_RETENTIONCONCEPTS_CREAR
    @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @RetentionTypeId int = NULL,
    @SriCode nvarchar(30) = NULL, @Percent decimal(9,4) = 0, @AppliesIva bit = 0, @AppliesIncome bit = 0,
    @IsActive bit = 1, @CreatedByUserId int = NULL, @CreatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.RetentionConcepts (Code, Name, Description, RetentionTypeId, SriCode, [Percent], AppliesIva, AppliesIncome, IsActive, CreatedByUserId, CreatedByUserName)
    VALUES (@Code, @Name, @Description, @RetentionTypeId, @SriCode, @Percent, @AppliesIva, @AppliesIncome, @IsActive, @CreatedByUserId, @CreatedByUserName);
    SELECT CONVERT(int, SCOPE_IDENTITY());
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_TAXREGIMES_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.TaxRegimes SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE TaxRegimeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_TAXPAYERTYPES_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.TaxpayerTypes SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE TaxpayerTypeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_RETENTIONTYPES_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.RetentionTypes SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE RetentionTypeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_TAXSUPPORTS_ACTUALIZAR @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.TaxSupports SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName WHERE TaxSupportId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_RETENTIONCONCEPTS_ACTUALIZAR
    @Id int, @Code nvarchar(30), @Name nvarchar(160), @Description nvarchar(300) = NULL, @RetentionTypeId int = NULL,
    @SriCode nvarchar(30) = NULL, @Percent decimal(9,4) = 0, @AppliesIva bit = 0, @AppliesIncome bit = 0,
    @IsActive bit = 1, @UpdatedByUserId int = NULL, @UpdatedByUserName nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.RetentionConcepts
    SET Code = @Code, Name = @Name, Description = @Description, RetentionTypeId = @RetentionTypeId, SriCode = @SriCode,
        [Percent] = @Percent, AppliesIva = @AppliesIva, AppliesIncome = @AppliesIncome, IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME(), UpdatedByUserId = @UpdatedByUserId, UpdatedByUserName = @UpdatedByUserName
    WHERE RetentionConceptId = @Id AND IsDeleted = 0;
    SELECT @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_TAXREGIMES_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.TaxRegimes SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE TaxRegimeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_TAXPAYERTYPES_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.TaxpayerTypes SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE TaxpayerTypeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_RETENTIONTYPES_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.RetentionTypes SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE RetentionTypeId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_TAXSUPPORTS_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.TaxSupports SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE TaxSupportId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_NA_DELETE_RETENTIONCONCEPTS_ELIMINAR @Id int, @DeletedByUserId int = NULL, @DeletedByUserName nvarchar(100) = NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.RetentionConcepts SET IsDeleted = 1, IsActive = 0, DeletedAt = SYSUTCDATETIME(), DeletedByUserId = @DeletedByUserId, DeletedByUserName = @DeletedByUserName WHERE RetentionConceptId = @Id AND IsDeleted = 0; SELECT @@ROWCOUNT; END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_BUSINESSPARTNERS_LOOKUPS
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Code, Name, CountryCode FROM dbo.BusinessPartnerIdentificationTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT Id, Code, Name, Days, IsCredit FROM dbo.BusinessPartnerPaymentTerms WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Days, Name;
    SELECT Id, Code, Name, IsActive FROM dbo.ChartOfAccounts WHERE IsDeleted = 0 AND IsActive = 1 AND AllowsMovement = 1 ORDER BY Code;
    SELECT N'Customer' AS Code, N'Cliente' AS Name UNION ALL SELECT N'Supplier', N'Proveedor' UNION ALL SELECT N'Both', N'Cliente y proveedor';
    SELECT N'Active' AS Code, N'Activo' AS Name UNION ALL SELECT N'Inactive', N'Inactivo';
    SELECT N'Pending' AS Code, N'Pendiente' AS Name UNION ALL SELECT N'Synced', N'Sincronizado' UNION ALL SELECT N'Error', N'Error';
    SELECT SupplierGroupId AS Id, Code, Name, IsActive FROM dbo.SupplierGroups WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT SupplierClassId AS Id, Code, Name, IsActive FROM dbo.SupplierClasses WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT EconomicActivityId AS Id, Code, Name, IsActive FROM dbo.EconomicActivities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ZoneId AS Id, Code, Name, IsActive FROM dbo.Zones WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT SupplyMethodId AS Id, Code, Name, IsActive FROM dbo.SupplyMethods WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ContactTypeId AS Id, Code, Name, IsActive FROM dbo.ContactTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ContactChannelId AS Id, Code, Name, IsActive FROM dbo.ContactChannels WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CountryId AS Id, Code, Name, IsActive FROM dbo.Countries WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT ProvinceId AS Id, Code, Name, IsActive FROM dbo.Provinces WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CityId AS Id, Code, Name, IsActive FROM dbo.Cities WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT BankId AS Id, Code, Name, IsActive FROM dbo.Banks WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT BankAccountTypeId AS Id, Code, Name, IsActive FROM dbo.BankAccountTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT CurrencyId AS Id, Code, Name, IsActive FROM dbo.Currencies WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Code;
    SELECT PriceListId AS Id, Code, Name, IsActive FROM dbo.PriceLists WHERE IsDeleted = 0 AND IsActive = 1 AND AppliesTo IN (N'Purchasing', N'Both') ORDER BY IsDefault DESC, Name;
    SELECT PurchasingAgentId AS Id, Code, Name, IsActive FROM dbo.PurchasingAgents WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT TaxRegimeId AS Id, Code, Name, IsActive FROM dbo.TaxRegimes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT TaxpayerTypeId AS Id, Code, Name, IsActive FROM dbo.TaxpayerTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT RetentionTypeId AS Id, Code, Name, IsActive FROM dbo.RetentionTypes WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
    SELECT
        RetentionConceptId AS Id,
        Code,
        Name,
        IsActive,
        SriCode,
        [Percent],
        AppliesIva,
        AppliesIncome,
        RetentionTypeId
    FROM dbo.RetentionConcepts
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY SriCode, Name;
    SELECT TaxSupportId AS Id, Code, Name, IsActive FROM dbo.TaxSupports WHERE IsDeleted = 0 AND IsActive = 1 ORDER BY Name;
END;
GO
