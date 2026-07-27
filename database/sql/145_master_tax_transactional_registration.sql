/*
    Iteracion 8.7 - Registro Master de Tax.
    No habilita perfiles, rutas, ownership ni workers.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.SyncEntityDefinitions',N'U') IS NULL THROW 51145,'SyncEntityDefinitions is required.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL THROW 51145,'MasterSchemaHistory is required.',1;
GO

IF EXISTS(SELECT 1 FROM dbo.SyncEntityDefinitions WHERE Code=N'Tax')
    UPDATE dbo.SyncEntityDefinitions SET Name=N'Impuestos',
        Description=N'Catalogo tributario con LocalOutbox transaccional, tasa decimal y conflicto terminal sin adopcion.',
        DefaultExecutionOrder=45,SupportsIncremental=1,SupportsInsert=1,SupportsUpdate=1,SupportsDeactivate=1,
        DefaultKeyField=N'Code',DefaultModifiedAtField=N'UpdatedAt',IsSystem=1,IsActive=1,IsDeleted=0,
        UpdatedAt=SYSUTCDATETIME(),UpdatedByUserName=N'Sistema' WHERE Code=N'Tax';
ELSE
    INSERT dbo.SyncEntityDefinitions(Code,Name,Description,DefaultExecutionOrder,SupportsIncremental,
        SupportsInsert,SupportsUpdate,SupportsDeactivate,DefaultKeyField,DefaultModifiedAtField,
        IsSystem,IsActive,CreatedByUserName)
    VALUES(N'Tax',N'Impuestos',N'Catalogo tributario con LocalOutbox transaccional, tasa decimal y conflicto terminal sin adopcion.',
        45,1,1,1,1,N'Code',N'UpdatedAt',1,1,N'Sistema');
GO

IF OBJECT_ID(N'dbo.SyncEntityConfigurations',N'U') IS NOT NULL
    INSERT dbo.SyncEntityConfigurations(CompanyId,EntityName,IsEnabled,Direction,ConflictPolicy,BatchSize,MaxAttempts)
    SELECT Id,N'Tax',CONVERT(bit,0),N'MasterToBranch',N'MasterWins',100,3 FROM dbo.Companies c
    WHERE c.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.SyncEntityConfigurations x WHERE x.CompanyId=c.Id AND x.EntityName=N'Tax');
GO

IF OBJECT_ID(N'dbo.EntityOwnershipConfigurations',N'U') IS NOT NULL
    INSERT dbo.EntityOwnershipConfigurations(CompanyId,EntityName,SourceOfTruth,SyncDirection,IsEnabled)
    SELECT Id,N'Tax',0,4,CONVERT(bit,0) FROM dbo.Companies c
    WHERE c.IsMaster=1 AND NOT EXISTS(SELECT 1 FROM dbo.EntityOwnershipConfigurations x WHERE x.CompanyId=c.Id AND x.EntityName=N'Tax');
GO

IF OBJECT_ID(N'dbo.Permissions',N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Modules',N'U') IS NOT NULL
BEGIN
    DECLARE @ModuleId int=(SELECT TOP(1) Id FROM dbo.Modules WHERE Code=N'TAXCATALOGS');
    IF @ModuleId IS NULL THROW 51145,'TAXCATALOGS module is required.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.Permissions WHERE Code=N'TAX.RATES.READ')
        INSERT dbo.Permissions(ModuleId,Code,Name,Description)
        VALUES(@ModuleId,N'TAX.RATES.READ',N'Consultar impuestos',N'Permite consultar el catalogo independiente de impuestos.');
    IF NOT EXISTS(SELECT 1 FROM dbo.Permissions WHERE Code=N'TAX.RATES.MANAGE')
        INSERT dbo.Permissions(ModuleId,Code,Name,Description)
        VALUES(@ModuleId,N'TAX.RATES.MANAGE',N'Administrar impuestos',N'Permite crear, actualizar, desactivar y eliminar impuestos.');
END;
GO

IF OBJECT_ID(N'dbo.SecurityForms',N'U') IS NOT NULL AND OBJECT_ID(N'dbo.SecurityMenus',N'U') IS NOT NULL
BEGIN
    DECLARE @ParentMenuId int=(SELECT TOP(1) Id FROM dbo.SecurityMenus WHERE Code=N'MENU.TAXCATALOGS' AND IsDeleted=0);
    IF @ParentMenuId IS NULL THROW 51145,'MENU.TAXCATALOGS is required.',1;

    IF NOT EXISTS(SELECT 1 FROM dbo.SecurityForms WHERE Code=N'FORM.TAXCATALOGS.TAXES' AND IsDeleted=0)
        INSERT dbo.SecurityForms(Code,Name,Description,FormKey,FormType,IsVisible,IsActive,CreatedByUserName,CreatedAt)
        VALUES(N'FORM.TAXCATALOGS.TAXES',N'Impuestos',N'Mantenimiento independiente de impuestos',N'taxes',1,1,1,N'Sistema',SYSUTCDATETIME());
    UPDATE dbo.SecurityForms SET Name=N'Impuestos',Description=N'Mantenimiento independiente de impuestos',
        FormKey=N'taxes',FormType=1,IsVisible=1,IsActive=1
    WHERE Code=N'FORM.TAXCATALOGS.TAXES' AND IsDeleted=0;

    DECLARE @FormId int=(SELECT TOP(1) Id FROM dbo.SecurityForms WHERE Code=N'FORM.TAXCATALOGS.TAXES' AND IsDeleted=0);
    IF NOT EXISTS(SELECT 1 FROM dbo.SecurityMenus WHERE Code=N'MENU.TAXCATALOGS.TAXES' AND IsDeleted=0)
        INSERT dbo.SecurityMenus(ParentId,Code,Name,Description,MenuType,FormId,FormKey,
            IconLarge,IconSmall,DisplayOrder,IsVisible,IsActive,CreatedByUserName,CreatedAt)
        VALUES(@ParentMenuId,N'MENU.TAXCATALOGS.TAXES',N'Impuestos',N'Mantenimiento independiente de impuestos',
            3,@FormId,N'taxes',N'Accordion/catalogos_32.svg',N'Accordion/catalogos_16.svg',5,1,1,N'Sistema',SYSUTCDATETIME());
    UPDATE dbo.SecurityMenus SET ParentId=@ParentMenuId,Name=N'Impuestos',
        Description=N'Mantenimiento independiente de impuestos',MenuType=3,FormId=@FormId,FormKey=N'taxes',
        DisplayOrder=5,IsVisible=1,IsActive=1
    WHERE Code=N'MENU.TAXCATALOGS.TAXES' AND IsDeleted=0;
END;
GO

IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260727.145')
    INSERT dbo.MasterSchemaHistory(Version,Description)
    VALUES(N'20260727.145',N'Registra Tax transaccional y permisos sin concesiones automaticas');
GO
