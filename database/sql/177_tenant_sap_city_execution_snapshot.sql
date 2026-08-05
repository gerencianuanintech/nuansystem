/* Adds CityV1 to the typed SAP execution snapshot contract. Tenant only; requires 173. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
IF OBJECT_ID(N'dbo.SapSyncExecutionDetails',N'U') IS NULL
    OR OBJECT_ID(N'dbo.SchemaVersions',N'U') IS NULL
    THROW 51177,'Migration 173 is required before 177.',1;

IF EXISTS(SELECT 1 FROM dbo.SchemaVersions WHERE Version=N'20260805.177')
    RETURN;

BEGIN TRY
BEGIN TRANSACTION;

IF EXISTS
(
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id=OBJECT_ID(N'dbo.SapSyncExecutionDetails')
      AND name=N'CK_SapSyncExecutionDetails_ApprovedSnapshotType'
)
    ALTER TABLE dbo.SapSyncExecutionDetails DROP CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType;

ALTER TABLE dbo.SapSyncExecutionDetails WITH CHECK
ADD CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType CHECK
(
    ApprovedSnapshotType IS NULL
    OR ApprovedSnapshotType IN
       ('SupplierV1','ItemV1','PaymentTermV1','WarehouseV1','CountryV1','ProvinceV1','CityV1')
);
ALTER TABLE dbo.SapSyncExecutionDetails CHECK CONSTRAINT CK_SapSyncExecutionDetails_ApprovedSnapshotType;

DECLARE @Definition nvarchar(max)=OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLEGUARDAR'));
IF @Definition IS NULL
    THROW 51177,'SAP execution detail save procedure is required before 177.',1;

IF @Definition NOT LIKE N'%CityV1%'
BEGIN
    DECLARE @HeaderStart int = PATINDEX(N'%[^' + NCHAR(9) + NCHAR(10) + NCHAR(13) + NCHAR(32) + N']%', @Definition);
    DECLARE @ProcedureToken int = CHARINDEX(N'PROCEDURE', UPPER(@Definition), @HeaderStart);
    IF @HeaderStart = 0 OR @ProcedureToken = 0
        THROW 51177,'SAP execution snapshot procedure header is invalid.',1;
    IF UPPER(SUBSTRING(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart)) LIKE N'CREATE%'
        SET @Definition=STUFF(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart,N'CREATE OR ALTER ');
    ELSE IF UPPER(SUBSTRING(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart)) LIKE N'ALTER%'
        SET @Definition=STUFF(@Definition,@HeaderStart,@ProcedureToken-@HeaderStart,N'CREATE OR ALTER ');
    ELSE
        THROW 51177,'Unexpected SAP execution snapshot procedure header.',1;

    IF @Definition NOT LIKE N'%ProvinceV1%'
        OR @Definition NOT LIKE N'%countryCode%provinceCode%provinceName%'
        THROW 51177,'Unexpected ProvinceV1 snapshot procedure contract.',1;

    DECLARE @Before nvarchar(max) = @Definition;
    SET @Definition=REPLACE(
        @Definition,
        N'''CountryV1'', ''ProvinceV1''',
        N'''CountryV1'', ''ProvinceV1'', ''CityV1''');
    IF @Definition = @Before
        THROW 51177,'CityV1 snapshot type could not be applied.',1;

    SET @Before = @Definition;
    SET @Definition=REPLACE(
        @Definition,
        N'(@ApprovedSnapshotType = ''ProvinceV1''
                       AND property.[key] NOT IN (''countryCode'', ''provinceCode'', ''provinceName''))',
        N'(@ApprovedSnapshotType = ''CityV1''
                       AND property.[key] NOT IN (''countryCode'', ''provinceCode'', ''cityCode'', ''cityName''))
                      OR
                      (@ApprovedSnapshotType = ''ProvinceV1''
                       AND property.[key] NOT IN (''countryCode'', ''provinceCode'', ''provinceName''))');
    IF @Definition = @Before
        THROW 51177,'CityV1 field allowlist could not be applied.',1;

    IF @Definition NOT LIKE N'%@ApprovedSnapshotType = ''CityV1''%'
        OR @Definition NOT LIKE N'%''cityCode''%''cityName''%'
        THROW 51177,'CityV1 allowlist could not be applied.',1;
    EXEC sys.sp_executesql @Definition;
END;

INSERT dbo.SchemaVersions(Version,Description)
VALUES(N'20260805.177',N'Agrega snapshot CityV1 para ejecuciones SAP de Ciudades');
COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
END CATCH;
GO
