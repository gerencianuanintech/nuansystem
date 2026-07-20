# SRI queue contract status

## Implemented evidence

- `TenantFeatureCodes.SriDocuments` defines capability `SRI_DOCUMENTS`.
- `TenantIntegrationCodes.Sri` defines integration code `SRI`.
- `database/sql/062_master_tenant_configuration.sql` seeds both disabled by default.
- `TenantIntegrationService` protects sensitive configuration values in reads.
- Existing tests verify that the SRI capability is disabled by default.
- `docs/architecture/SRI-DOCUMENTS-WORKER.md` describes the target separation between capturers, queue, worker, and storage.

## Not implemented at Iteration 5 start

- No SRI queue, attempts, XML/document store, repository, use case, endpoint, permission set, monitor form, or `NuanSystem.SriWorker` project exists.
- No production SRI provider/client, certificate flow, environment selection, or end-to-end authorization/download flow exists.
- Proposed state names and fields are architecture candidates, not database contracts.

## Evidence rule

Search the current repository before every task. If implementation advances, update this reference in the same change. A document or test double is not proof that a production path exists.
