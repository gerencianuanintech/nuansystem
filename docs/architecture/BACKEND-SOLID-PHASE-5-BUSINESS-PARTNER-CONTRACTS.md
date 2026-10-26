# Fase 5: contrato de importacion de proveedores SAP

Fecha: 2026-10-23. Refactor de contratos Application/Persistence, sin cambios de negocio ni SQL.

## Descubrimiento y decision

Consumidores de IBusinessPartnerRepository: tres queries (lista, detalle y lookups), tres comandos (alta, edicion y baja) y SapSupplierImportService. Solo este ultimo usa ImportSupplierFromSapAsync; tambien requiere GetAllAsync para preview, matching por identidad y evitar duplicados. No existe un contrato de persistencia de importacion equivalente. Los aplicadores Matriz/Sucursal tienen su propia persistencia y no usan este contrato.

Se extrae ISapSupplierImportRepository en Application/Abstractions/Sap, con la firma y los DTO actuales. No se divide lectura/escritura del CRUD: no aporta beneficio demostrado a esta extraccion. El importador mantiene IBusinessPartnerRepository para sus consultas y recibe el nuevo contrato solo para importar. Los handlers CRUD conservan sus dependencias y comportamiento.

BusinessPartnerRepository implementa ambos contratos. Se registra una unica instancia scoped concreta y dos aliases scoped que la resuelven. Se conserva ITenantConnectionFactory, la apertura de conexiones por metodo, los metodos con transaccion explicita y el procedimiento SP_NA_POST_BUSINESSPARTNERS_IMPORTARSAP con sus parametros y resultados intactos. No se introduce sesion transaccional nueva; eso pertenece a fase 7.

## Registro vertical y alcance

Outcome: CRUD sin metodo SAP; importacion equivalente por un puerto segregado.
Capas: Application y Persistence cambian; Domain/API/WinForms/SQL verificadas sin cambios.
Referencia: BusinessPartnerRepository, handlers BusinessPartners, SapSupplierImportService, BusinessPartnerSyncPublishingTests y composicion real con barreras de IO.
Tenant: mismo factory/contexto scoped; dos scopes no comparten repositorio.
Transaccion: CRUD usa ITransactionRunner y LocalOutbox en la misma transaccion existente; la importacion conserva la transaccion del procedimiento SQL.
Identidad/auditoria: sin cambios de DTOs, GlobalId, matching, id retornado, AuditUserId/AuditUserName ni publicacion de eventos.
Seguridad: permisos y entradas API no cambian; no se crea una nueva ruta.
Exclusiones: sin ejecucion SAP, DB, migraciones, transacciones nuevas, cambios monetarios ni fases posteriores.

Las referencias de catalogo siguen apuntando al mismo repositorio y patron de tenant/transaccion; este documento registra el puerto SAP adicional y su arista SapSupplierImportService -> ISapSupplierImportRepository -> BusinessPartnerRepository. No se modifica el nucleo de instrucciones .codex.

Se trabaja en el checkout actual para conservar los cambios previos. Snapshot y copias en artifacts/backend-solid-phase5; sin reset, stash, commits ni cambio de rama.
