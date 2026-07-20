# SAP worker contract map

- orchestration: `Application/Features/SapSync/Services/SapSyncOrchestrator.cs`
- job runner/retry: sibling `SapSyncJobRunner.cs` and `SapSyncRetryPolicy.cs`
- lock/log/watermark/heartbeat: sibling services
- handlers: `Application/Features/SapSync/Handlers`
- workers/options: `NuanSystem.SyncWorker/Workers` and `Options`
- storage: SQL `049_master_sap_sync_worker.sql` and `050_tenant_sap_sync_worker.sql`
- operative payment-terms handler: `SapPaymentTermSyncHandler` (`PaymentTerms`, Full `SapToErp`), with tenant/master registration in SQL `112`/`113`/`114`
- Service Layer TLS: `ServiceLayer:IgnoreSslErrors` is wired only with explicit Development approval; production requires a trusted certificate chain

Explicit `NotImplemented` results are architectural evidence, not ignorable TODOs.
