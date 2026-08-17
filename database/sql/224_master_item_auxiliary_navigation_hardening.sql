/*
    Migration 224 - Forward hardening for auxiliary item-master navigation.

    Target: NuanSystem_Master only.
    Prerequisites: 179, 209, 218, 219 and 220.
    Reactivates existing physical form, menu and ADMIN role-menu rows for
    ItemOrigins and ItemCommercialSegments, then reasserts their operations.
*/
USE [NuanSystem_Master];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_NAME()<>N'NuanSystem_Master'
    THROW 51224,'Migration 224 must run only in NuanSystem_Master.',1;
IF OBJECT_ID(N'dbo.MasterSchemaHistory',N'U') IS NULL
    THROW 51224,'MasterSchemaHistory is required.',1;
IF OBJECT_ID(N'dbo.SecurityForms',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SecurityMenus',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SecurityRoleMenus',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SecurityOperations',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SecurityFormOperations',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SecurityRoleFormOperations',N'U') IS NULL
    THROW 51224,'Security navigation and operation tables are required.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.209')
    OR NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260813.218')
    THROW 51224,'Migrations 209 and 218 are required before migration 224.',1;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @AdminRoleId int=(SELECT TOP(1) Id FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0);
    IF @AdminRoleId IS NULL THROW 51224,'The active ADMIN role is required.',1;

    DECLARE @ApplicableOperations table(OperationId int PRIMARY KEY);
    INSERT @ApplicableOperations(OperationId)
    SELECT Id
    FROM dbo.SecurityOperations
    WHERE IsDeleted=0 AND IsActive=1 AND Code IN(
        N'ACTION.REFRESH',N'ACTION.CONSULT',N'ACTION.CREATE',N'ACTION.UPDATE',N'ACTION.DELETE',N'ACTION.COPY',
        N'ACTION.HISTORY',N'ACTION.CUSTOMIZE_COLUMNS',N'ACTION.EXPORT_EXCEL',N'ACTION.EXPORT_PDF',
        N'ACTION.EXPORT_JSON',N'ACTION.EXPORT_XML');

    IF (SELECT COUNT(*) FROM @ApplicableOperations)<>12
        THROW 51224,'The twelve canonical CRUD operations are required.',1;

    DECLARE @OriginFormId int=(
        SELECT TOP(1) Id FROM dbo.SecurityForms
        WHERE FormKey=N'item-origins'
           OR Code IN(N'FORM.GENERALINVENTORY.ITEMORIGINS',N'FORM.DEFINITIONS.INVENTORY.ITEMORIGINS')
        ORDER BY IsDeleted,Id);
    DECLARE @OriginMenuId int=(
        SELECT TOP(1) Id FROM dbo.SecurityMenus
        WHERE FormKey=N'item-origins'
           OR Code IN(N'MENU.GENERALINVENTORY.ITEMORIGINS',N'MENU.DEFINITIONS.INVENTORY.ITEMORIGINS')
        ORDER BY IsDeleted,Id);
    DECLARE @SegmentFormId int=(
        SELECT TOP(1) Id FROM dbo.SecurityForms
        WHERE FormKey=N'item-commercial-segments'
           OR Code=N'FORM.DEFINITIONS.INVENTORY.ItemCommercialSegments'
        ORDER BY IsDeleted,Id);
    DECLARE @SegmentMenuId int=(
        SELECT TOP(1) Id FROM dbo.SecurityMenus
        WHERE FormKey=N'item-commercial-segments'
           OR Code=N'MENU.DEFINITIONS.INVENTORY.ITEMCOMMERCIALSEGMENTS'
        ORDER BY IsDeleted,Id);

    IF @OriginFormId IS NULL OR @OriginMenuId IS NULL OR @SegmentFormId IS NULL OR @SegmentMenuId IS NULL
        THROW 51224,'ItemOrigins and ItemCommercialSegments navigation rows are required.',1;

    UPDATE dbo.SecurityForms
    SET IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE Id IN(@OriginFormId,@SegmentFormId);

    UPDATE dbo.SecurityMenus
    SET IsVisible=1,IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE Id IN(@OriginMenuId,@SegmentMenuId);

    UPDATE dbo.SecurityRoleMenus
    SET IsAllowed=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    WHERE RoleId=@AdminRoleId AND MenuId IN(@OriginMenuId,@SegmentMenuId);

    INSERT dbo.SecurityRoleMenus(RoleId,MenuId,IsAllowed,CreatedByUserName,CreatedAt)
    SELECT @AdminRoleId,source.MenuId,1,N'Sistema',SYSUTCDATETIME()
    FROM (VALUES(@OriginMenuId),(@SegmentMenuId)) source(MenuId)
    WHERE NOT EXISTS(
        SELECT 1 FROM dbo.SecurityRoleMenus target
        WHERE target.RoleId=@AdminRoleId AND target.MenuId=source.MenuId);

    /*
        Remove stale grants left by older navigation seeds. Auxiliary CRUD forms
        expose only the twelve operations declared in @ApplicableOperations.
    */
    UPDATE target
    SET IsAllowed=0,IsDeleted=1,DeletedByUserName=N'Sistema',DeletedAt=SYSUTCDATETIME(),
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    FROM dbo.SecurityRoleFormOperations target
    WHERE target.RoleId=@AdminRoleId
      AND target.FormId IN(@OriginFormId,@SegmentFormId)
      AND NOT EXISTS(
          SELECT 1 FROM @ApplicableOperations source
          WHERE source.OperationId=target.OperationId);

    UPDATE target
    SET IsActive=0,IsDeleted=1,DeletedByUserName=N'Sistema',DeletedAt=SYSUTCDATETIME(),
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    FROM dbo.SecurityFormOperations target
    WHERE target.FormId IN(@OriginFormId,@SegmentFormId)
      AND NOT EXISTS(
          SELECT 1 FROM @ApplicableOperations source
          WHERE source.OperationId=target.OperationId);

    UPDATE target
    SET IsActive=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    FROM dbo.SecurityFormOperations target
    JOIN @ApplicableOperations source ON source.OperationId=target.OperationId
    WHERE target.FormId IN(@OriginFormId,@SegmentFormId);

    INSERT dbo.SecurityFormOperations(FormId,OperationId,IsActive,CreatedByUserName,CreatedAt)
    SELECT forms.FormId,operations.OperationId,1,N'Sistema',SYSUTCDATETIME()
    FROM (VALUES(@OriginFormId),(@SegmentFormId)) forms(FormId)
    CROSS JOIN @ApplicableOperations operations
    WHERE NOT EXISTS(
        SELECT 1 FROM dbo.SecurityFormOperations target
        WHERE target.FormId=forms.FormId AND target.OperationId=operations.OperationId);

    UPDATE target
    SET IsAllowed=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,
        UpdatedByUserName=N'Sistema',UpdatedAt=SYSUTCDATETIME()
    FROM dbo.SecurityRoleFormOperations target
    JOIN @ApplicableOperations source ON source.OperationId=target.OperationId
    WHERE target.RoleId=@AdminRoleId AND target.FormId IN(@OriginFormId,@SegmentFormId);

    INSERT dbo.SecurityRoleFormOperations(RoleId,FormId,OperationId,IsAllowed,CreatedByUserName,CreatedAt)
    SELECT @AdminRoleId,forms.FormId,operations.OperationId,1,N'Sistema',SYSUTCDATETIME()
    FROM (VALUES(@OriginFormId),(@SegmentFormId)) forms(FormId)
    CROSS JOIN @ApplicableOperations operations
    WHERE NOT EXISTS(
        SELECT 1 FROM dbo.SecurityRoleFormOperations target
        WHERE target.RoleId=@AdminRoleId AND target.FormId=forms.FormId AND target.OperationId=operations.OperationId);

    IF NOT EXISTS(SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version=N'20260815.224')
        INSERT dbo.MasterSchemaHistory(Version,Description)
        VALUES(N'20260815.224',N'Hardens ItemOrigins and ItemCommercialSegments navigation reactivation');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
