/*
    Migration 220 - Forward repair for ItemOrigins operation applicability.

    Target: NuanSystem_Master only.
    Prerequisites: migrations 179 and 209.
    Adds the twelve canonical CRUD/grid operations to SecurityFormOperations
    and completes the approved ADMIN grants without changing other roles.
*/
USE [NuanSystem_Master];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master'
    THROW 51220,'Migration 220 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL
    THROW 51220,'MasterSchemaHistory is required.',1;
IF OBJECT_ID(N'dbo.SecurityFormOperations',N'U') IS NULL
    THROW 51220,'Migration 179 is required before migration 220.',1;
IF OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL
    THROW 51220,'SecurityRoleFormOperations is required before migration 220.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.209')
    THROW 51220,'Migration 209 is required before migration 220.',1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @FormId int=(
        SELECT TOP(1) Id FROM dbo.SecurityForms
        WHERE FormKey=N'item-origins'
           OR Code IN(N'FORM.GENERALINVENTORY.ITEMORIGINS',N'FORM.DEFINITIONS.INVENTORY.ITEMORIGINS')
        ORDER BY IsDeleted,Id
    );
    DECLARE @AdminRoleId int=(
        SELECT TOP(1) Id FROM dbo.Roles
        WHERE Code=N'ADMIN' AND IsDeleted=0
    );

    IF @FormId IS NULL OR @AdminRoleId IS NULL
        THROW 51220,'The active ItemOrigins form and ADMIN role are required.',1;

    UPDATE dbo.SecurityForms
    SET FormType=1,HasListView=1,HasEditView=1,IsVisible=1,IsActive=1,
        IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE Id=@FormId;

    DECLARE @ApplicableOperations table(OperationId int PRIMARY KEY);
    INSERT @ApplicableOperations(OperationId)
    SELECT operation.Id
    FROM dbo.SecurityOperations operation
    WHERE operation.IsDeleted=0
      AND operation.IsActive=1
      AND operation.Code IN(
          N'ACTION.REFRESH',N'ACTION.CONSULT',N'ACTION.CREATE',N'ACTION.UPDATE',
          N'ACTION.DELETE',N'ACTION.COPY',N'ACTION.HISTORY',N'ACTION.CUSTOMIZE_COLUMNS',
          N'ACTION.EXPORT_EXCEL',N'ACTION.EXPORT_PDF',N'ACTION.EXPORT_JSON',N'ACTION.EXPORT_XML'
      );

    IF (SELECT COUNT(*) FROM @ApplicableOperations)<>12
        THROW 51220,'The twelve canonical CRUD operations are required.',1;

    UPDATE target
    SET IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    FROM dbo.SecurityFormOperations target
    JOIN @ApplicableOperations source ON source.OperationId=target.OperationId
    WHERE target.FormId=@FormId;

    INSERT dbo.SecurityFormOperations(FormId,OperationId,IsActive,CreatedByUserName,CreatedAt)
    SELECT @FormId,source.OperationId,1,N'Sistema',SYSUTCDATETIME()
    FROM @ApplicableOperations source
    WHERE NOT EXISTS(
        SELECT 1 FROM dbo.SecurityFormOperations target
        WHERE target.FormId=@FormId AND target.OperationId=source.OperationId
    );

    UPDATE target
    SET IsAllowed=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    FROM dbo.SecurityRoleFormOperations target
    JOIN @ApplicableOperations source ON source.OperationId=target.OperationId
    WHERE target.RoleId=@AdminRoleId AND target.FormId=@FormId;

    INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt)
    SELECT @AdminRoleId,@FormId,source.OperationId,1,N'Sistema',SYSUTCDATETIME()
    FROM @ApplicableOperations source
    WHERE NOT EXISTS(
        SELECT 1 FROM dbo.SecurityRoleFormOperations target
        WHERE target.RoleId=@AdminRoleId AND target.FormId=@FormId AND target.OperationId=source.OperationId
    );

    IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260814.220')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260814.220',N'Repairs ItemOrigins form operation applicability');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
