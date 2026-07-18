SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/*
    Permite que un evento maestro limite su distribucion a una sucursal tenant.
    Cuando @RequireTargetBranchMatch = 1 y no existe codigo destino, el evento
    permanece en Master sin crear SyncOutboxTargets.
*/
CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SYNCROUTINGTARGETS
    @SourceCompanyId int,
    @EntityCode nvarchar(80),
    @SyncProfileId int = NULL,
    @TargetBranchCode nvarchar(50) = NULL,
    @RequireTargetBranchMatch bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedEntityCode nvarchar(80) = LTRIM(RTRIM(@EntityCode));
    DECLARE @NormalizedTargetBranchCode nvarchar(50) = NULLIF(LTRIM(RTRIM(@TargetBranchCode)), N'');

    SELECT DISTINCT
        profile.Id AS SyncProfileId,
        entity.Id AS SyncProfileEntityId,
        profile.Code AS SyncProfileCode,
        profile.CompanyId AS SourceCompanyId,
        profileBranch.BranchCompanyId,
        entity.EntityCode,
        COALESCE(matrix.BatchSize, entity.BatchSize, profileBranch.BatchSize, profile.BatchSize) AS BatchSize,
        COALESCE(profileBranch.MaxRetries, profile.MaxRetries) AS MaxRetries,
        profile.RetryDelaySeconds,
        profile.TimeoutMinutes,
        entity.AllowInsert,
        entity.AllowUpdate,
        entity.AllowDeactivate,
        entity.ContinueOnError
    FROM dbo.SyncProfiles AS profile
    INNER JOIN dbo.Companies AS sourceCompany
        ON sourceCompany.Id = profile.CompanyId
       AND sourceCompany.IsActive = 1
       AND sourceCompany.IsMaster = 1
       AND sourceCompany.SyncEnabled = 1
       AND sourceCompany.IsDeleted = 0
    INNER JOIN dbo.SyncProfileEntities AS entity
        ON entity.SyncProfileId = profile.Id
       AND entity.IsDeleted = 0
       AND entity.IsActive = 1
       AND entity.EntityCode = @NormalizedEntityCode
       AND (@SyncProfileId IS NOT NULL OR entity.SyncMode = N'Incremental')
    INNER JOIN dbo.SyncProfileEntityBranches AS matrix
        ON matrix.SyncProfileId = profile.Id
       AND matrix.SyncProfileEntityId = entity.Id
       AND matrix.IsDeleted = 0
       AND matrix.IsEnabled = 1
    INNER JOIN dbo.SyncProfileBranches AS profileBranch
        ON profileBranch.Id = matrix.SyncProfileBranchId
       AND profileBranch.SyncProfileId = profile.Id
       AND profileBranch.IsDeleted = 0
       AND profileBranch.IsActive = 1
    INNER JOIN dbo.Companies AS branchCompany
        ON branchCompany.Id = profileBranch.BranchCompanyId
       AND branchCompany.IsActive = 1
       AND branchCompany.IsMaster = 0
       AND branchCompany.SyncEnabled = 1
       AND branchCompany.ParentCompanyId = profile.CompanyId
       AND branchCompany.IsDeleted = 0
    WHERE profile.CompanyId = @SourceCompanyId
      AND profile.IsDeleted = 0
      AND profile.IsActive = 1
      AND profile.Direction = N'MasterToBranch'
      AND profile.ConflictStrategy = N'MasterWins'
      AND
      (
          @RequireTargetBranchMatch = 0
          OR
          (
              @NormalizedTargetBranchCode IS NOT NULL
              AND branchCompany.BranchCode = @NormalizedTargetBranchCode
          )
      )
      AND
      (
            (@SyncProfileId IS NULL AND profile.ExecutionMode = N'Incremental')
         OR (@SyncProfileId IS NOT NULL AND profile.Id = @SyncProfileId AND profile.ExecutionMode IN (N'Incremental', N'Full', N'Manual'))
      )
    ORDER BY profileBranch.BranchCompanyId, profile.Id, entity.Id;
END;
GO

IF OBJECT_ID(N'dbo.MasterSchemaHistory', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.MasterSchemaHistory WHERE Version = N'20260716.092')
BEGIN
    INSERT INTO dbo.MasterSchemaHistory (Version, Description)
    VALUES (N'20260716.092', N'Routing Maestro-Sucursal filtrado por codigo de sucursal destino');
END;
GO
