/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    No debe ejecutarse en NuanSystem_Master.
*/

IF OBJECT_ID(N'dbo.SchemaHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SchemaHistory
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SchemaHistory PRIMARY KEY,
        Version nvarchar(50) NOT NULL,
        Description nvarchar(300) NOT NULL,
        AppliedAt datetime2(0) NOT NULL CONSTRAINT DF_SchemaHistory_AppliedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_SchemaHistory_Version UNIQUE (Version)
    );
END;
GO

IF OBJECT_ID(N'dbo.Items', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Items
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Items PRIMARY KEY,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(500) NULL,
        UnitOfMeasure nvarchar(20) NOT NULL CONSTRAINT DF_Items_UnitOfMeasure DEFAULT N'UND',
        IsInventoryItem bit NOT NULL CONSTRAINT DF_Items_IsInventoryItem DEFAULT 1,
        IsActive bit NOT NULL CONSTRAINT DF_Items_IsActive DEFAULT 1,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_Items_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt datetime2(0) NULL,
        CONSTRAINT UQ_Items_Code UNIQUE (Code)
    );
END;
GO

IF OBJECT_ID(N'dbo.SapSyncLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapSyncLog
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncLog PRIMARY KEY,
        CompanyId int NOT NULL,
        EntityType nvarchar(80) NOT NULL,
        EntityId nvarchar(80) NOT NULL,
        SapObjectType nvarchar(80) NOT NULL,
        RequestJson nvarchar(max) NULL,
        ResponseJson nvarchar(max) NULL,
        Status nvarchar(30) NOT NULL,
        ErrorMessage nvarchar(max) NULL,
        SapDocEntry int NULL,
        SapDocNum int NULL,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_SapSyncLog_CreatedAt DEFAULT SYSUTCDATETIME(),
        SyncedAt datetime2(0) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaHistory WHERE Version = N'20260427.04')
BEGIN
    INSERT INTO dbo.SchemaHistory (Version, Description)
    VALUES (N'20260427.04', N'Fase 4: esquema inicial tenant');
END;
GO
