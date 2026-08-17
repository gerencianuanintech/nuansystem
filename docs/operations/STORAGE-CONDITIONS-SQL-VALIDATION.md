# StorageConditions SQL deployment evidence

Date: 2026-08-13  
Scope: deploy migrations `214`, `215`, and `216` only.  
Runtime boundary: no synchronization profile, route, relay, worker, SAP, or SRI process was enabled or executed.

## Databases

- Master: `NuanSystem_Master`.
- Tenants: `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO`, and `NuanSystem_DEMO_CANARIS`.
- All four databases were confirmed `ONLINE` before the deployment.
- Preflight confirmed that versions `20260813.214`, `20260813.215`, and `20260813.216` were absent.

## Verified backups

The following `COPY_ONLY WITH CHECKSUM` backups passed `RESTORE VERIFYONLY ... WITH CHECKSUM` before any migration ran:

- `/var/opt/mssql/data/NuanSystem_Master_StorageConditions214216_20260813_213141.bak`
- `/var/opt/mssql/data/NuanSystem_DEMO_StorageConditions214216_20260813_213141.bak`
- `/var/opt/mssql/data/NuanSystem_DEMO_REMIGIO_StorageConditions214216_20260813_213141.bak`
- `/var/opt/mssql/data/NuanSystem_DEMO_CANARIS_StorageConditions214216_20260813_213141.bak`

## Execution

- `214_tenant_storage_conditions_master.sql` ran twice with `sqlcmd -b` in each tenant.
- `215_master_definitions_inventory_storage_conditions_navigation.sql` ran twice with `sqlcmd -b` in Master.
- `216_master_storage_conditions_sync_registration.sql` ran twice with `sqlcmd -b` in Master.
- Both passes completed successfully, proving runtime idempotency for the observed legacy state.

## Post-deployment evidence

Each tenant has:

- exactly one `SchemaHistory` row for `20260813.214`;
- two preserved functional rows, `AMBIENTE` and `REFRIGERADO`;
- no null or empty `GlobalId`, no negative `SortOrder`;
- the three required indexes;
- nine dedicated `StorageConditions` stored procedures;
- a lookup returning two rows and exactly one `REFRIGERADO` row;
- zero `StorageCondition` events in `LocalOutbox`.

Master has:

- exactly one `MasterSchemaHistory` row for `20260813.215` and one for `20260813.216`;
- one canonical form and menu with FormKey `storage-conditions`;
- zero active legacy `inventory-storage-conditions` forms or menus;
- both API permissions granted to `ADMIN` and 34 approved form-operation grants;
- one active `StorageCondition` entity definition with zero active dependencies;
- one entity configuration and one ownership configuration, both disabled;
- no worker, profile, or distribution activation performed by this deployment.

The first read-only Master postcheck referenced a nonexistent `Permissions.IsDeleted` column and failed before returning results. The corrected read-only query used `Permissions.IsActive` and passed. No migration failed and no corrective data write was required.

