/* Adds ProvinceV1 to the typed SAP execution snapshot contract. Tenant only; requires 169. */
SET NOCOUNT ON; SET XACT_ABORT ON;
GO
IF OBJECT_ID(N'dbo.SapSyncExecutionDetails',N'U') IS NULL OR OBJECT_ID(N'dbo.SchemaVersions',N'U') IS NULL THROW 51173,'Migration 169 is required before 173.',1;
GO
IF EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.SapSyncExecutionDetails') AND name=N'CK_SapSyncExecutionDetails_ApprovedSnapshotType')
    ALTER TABLE dbo.SapSyncExecutionDetails DROP CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType;
ALTER TABLE dbo.SapSyncExecutionDetails WITH CHECK ADD CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType CHECK
(
 ApprovedSnapshotType IS NULL OR ApprovedSnapshotType IN('SupplierV1','ItemV1','PaymentTermV1','WarehouseV1','CountryV1','ProvinceV1')
);
ALTER TABLE dbo.SapSyncExecutionDetails CHECK CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType;
GO
DECLARE @Definition nvarchar(max)=OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLEGUARDAR'));
IF @Definition IS NULL THROW 51173,'SAP execution detail save procedure is required before 173.',1;
IF @Definition NOT LIKE N'%ProvinceV1%'
BEGIN
    DECLARE @HeaderStart int = PATINDEX(N'%[^' + NCHAR(9) + NCHAR(10) + NCHAR(13) + NCHAR(32) + N']%', @Definition);
    DECLARE @ProcedureToken int = CHARINDEX(N'PROCEDURE', UPPER(@Definition), @HeaderStart);
    IF @HeaderStart = 0 OR @ProcedureToken = 0
        THROW 51173,'SAP execution snapshot procedure header is invalid.',1;
    IF UPPER(SUBSTRING(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart)) LIKE N'CREATE%'
        SET @Definition=STUFF(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart,N'CREATE OR ALTER ');
    ELSE IF UPPER(SUBSTRING(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart)) LIKE N'ALTER%'
        SET @Definition=STUFF(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart,N'CREATE OR ALTER ');
    ELSE
        THROW 51173,'Unexpected SAP execution snapshot procedure header.',1;
    IF @Definition NOT LIKE N'%CountryV1%' OR @Definition NOT LIKE N'%countryCode%countryName%iso2%iso3%'
        THROW 51173,'Unexpected SAP execution snapshot procedure contract.',1;
    SET @Definition=REPLACE(@Definition,N'''WarehouseV1'', ''CountryV1''',N'''WarehouseV1'', ''CountryV1'', ''ProvinceV1''');
    SET @Definition=REPLACE(@Definition,
        N'(@ApprovedSnapshotType = ''CountryV1''
                       AND property.[key] NOT IN (''countryCode'', ''countryName'', ''iso2'', ''iso3''))',
        N'(@ApprovedSnapshotType = ''ProvinceV1''
                       AND property.[key] NOT IN (''countryCode'', ''provinceCode'', ''provinceName''))
                      OR
                      (@ApprovedSnapshotType = ''CountryV1''
                       AND property.[key] NOT IN (''countryCode'', ''countryName'', ''iso2'', ''iso3''))');
    IF @Definition NOT LIKE N'%@ApprovedSnapshotType = ''ProvinceV1''%' OR @Definition NOT LIKE N'%''provinceCode''%''provinceName''%'
        THROW 51173,'ProvinceV1 allowlist could not be applied.',1;
    EXEC sys.sp_executesql @Definition;
END;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaVersions WHERE Version=N'20260804.173')
    INSERT dbo.SchemaVersions(Version,Description) VALUES(N'20260804.173',N'Agrega snapshot ProvinceV1 para ejecuciones SAP de Provincias');
GO
