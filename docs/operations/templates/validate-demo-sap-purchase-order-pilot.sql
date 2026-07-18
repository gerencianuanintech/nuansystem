/* Consulta exclusivamente de lectura. Ejecutar conectado a NuanSystem_Master. */
SET NOCOUNT ON;

SELECT profile.Id,profile.Code,profile.IsActive,branch.BranchCompanyId,company.Code BranchCode,branch.IsActive BranchProfileActive
FROM dbo.SyncProfiles profile
INNER JOIN dbo.SyncProfileBranches branch ON branch.SyncProfileId=profile.Id AND branch.IsDeleted=0
INNER JOIN dbo.Companies company ON company.Id=branch.BranchCompanyId
WHERE profile.CompanyId=1 AND profile.Code=N'DEMO-ITEMS-PILOT' AND profile.IsDeleted=0
ORDER BY branch.BranchCompanyId;

SELECT route.SourceCompanyId,route.WarehouseCode,route.BranchCompanyId,company.Code BranchCode,route.IsActive
FROM dbo.PurchaseOrderWarehouseRoutes route
INNER JOIN dbo.Companies company ON company.Id=route.BranchCompanyId
WHERE route.SourceCompanyId=1
ORDER BY route.WarehouseCode;

SELECT EntityName,Status,COUNT_BIG(*) Total
FROM dbo.SyncOutbox
WHERE CompanyId=1
GROUP BY EntityName,Status
ORDER BY EntityName,Status;

SELECT TOP(100) outbox.Id,outbox.EventId,outbox.EntityName,outbox.EntityCode,outbox.Status,
       target.BranchCompanyId,target.Status TargetStatus,target.AttemptCount,target.LastErrorMessage
FROM dbo.SyncOutbox outbox
LEFT JOIN dbo.SyncOutboxTargets target ON target.OutboxId=outbox.Id
WHERE outbox.CompanyId=1 AND (outbox.Status IN(N'Error',N'DeadLetter') OR target.Status IN(N'Error',N'DeadLetter'))
ORDER BY outbox.Id DESC;

SELECT N'DEMO' DatabaseCode,COUNT_BIG(*) Orders,COUNT_BIG(DISTINCT SapDocEntry) DistinctSapDocEntries
FROM NuanSystem_DEMO.dbo.PurchaseOrderHeaders WHERE IsDeleted=0 AND SapDocEntry>0
UNION ALL
SELECT N'DEMO-REMIGIO',COUNT_BIG(*),COUNT_BIG(DISTINCT SapDocEntry)
FROM NuanSystem_DEMO_REMIGIO.dbo.PurchaseOrderHeaders WHERE IsDeleted=0 AND SapDocEntry>0
UNION ALL
SELECT N'DEMO-CANARIS',COUNT_BIG(*),COUNT_BIG(DISTINCT SapDocEntry)
FROM NuanSystem_DEMO_CANARIS.dbo.PurchaseOrderHeaders WHERE IsDeleted=0 AND SapDocEntry>0;

SELECT SapDocEntry,COUNT_BIG(*) Duplicates
FROM NuanSystem_DEMO.dbo.PurchaseOrderHeaders
WHERE IsDeleted=0 AND SapDocEntry>0
GROUP BY SapDocEntry HAVING COUNT_BIG(*)>1;
