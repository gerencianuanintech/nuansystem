# Fase 4: contratos y capacidades SAP

Fecha: 2026-10-23. Alcance: integracion SAP opcional, Application y pruebas locales.

Se sustituyen los contratos conjuntos por ISapSyncImporter e ISapSyncExporter. Cada implementacion declara sus modos reales. No se registra exportacion ni el placeholder de PurchaseOrders. Both se rechaza explicitamente.

Un registro scoped cruza importadores, exportadores, procesadores programados y de reintento; rechaza registros duplicados o incompatibles. Las capacidades persistidas siguen siendo restricciones: el registro solo puede reducirlas. Catalogo y validacion de perfiles requieren un procesador programado real; el planificador aplica el mismo criterio antes de reservar. El preparador valida direccion/modo antes de despachar. Los importadores manuales existentes siguen disponibles por sus comandos y por la orquestacion directa.

Actualmente solo Warehouses, Countries, Provinces y Cities disponen del flujo programado Full y reintento. Suppliers, Items y PaymentTerms conservan sus importaciones existentes; no se activan agendas para ellos. No se implementa outbox ERP a SAP ni se ejecutan cambios SQL.

Se trabaja en el checkout actual para conservar las fases anteriores y los cambios del usuario. Evidencia previa y copias de archivos en artifacts/backend-solid-phase4. No se usa reset, stash ni se cambia de rama. Se coordina con 2026-10-19-sap-sync-quality-hardening sin ampliar esta fase a un nuevo dispatcher durable.

## Matriz de ejecucion efectiva

| Entidad | Importador directo | Perfil/programacion | Exportacion |
|---|---|---|---|
| Suppliers | Incremental (las opciones Full del servicio siguen disponibles por su comando) | No: falta procesador programado | No |
| Items | Full | No: falta procesador programado | No |
| PaymentTerms | Full | No: falta procesador programado | No |
| Warehouses, Countries, Provinces, Cities | Full | Full, con procesadores de reintento existentes | No |
| PurchaseOrders | Sin handler generico registrado; sus comandos especificos no cambian | No | No |

Los registros existentes de perfiles no se alteran en la base. Un perfil activo de Suppliers, Items o PaymentTerms que antes se aceptaba solo por metadata ahora se rechaza al validarlo y no se reserva en el planificador. Las importaciones por comandos siguen disponibles. Las respuestas conservan los DTO y rutas actuales de WinForms; el catalogo filtra entidades, direcciones y modos sin soporte real. Both se rechaza incluso si hay contratos direccionales separados: no se implementa un coordinador bidireccional en esta fase.

## Extension y limites

Para una entidad programable se registra un importador, modos admitidos y procesador programado; si hay reintentos, cada procesador debe declarar EntityCode y un tipo de snapshot unico. El registro valida duplicados por contrato y compatibilidad importador/programado/reintento. El despacho de reintentos existente conserva su seleccion por snapshot y su persistencia; no se reescribe. Los modos programados actuales son solo SAP a ERP. Un futuro exportador directo puede implementar ISapSyncExporter sin obligar a modificar un importador; programar exportaciones requiere implementar su flujo durable y ampliar explicitamente las capacidades.

No se mantienen adaptadores ni metodos de exportacion ficticios en los importadores. SapSyncJobRunner/Outbox sigue pendiente y fuera de esta entrega. La fase no acredita conectividad SAP, SQL, rendimiento ni produccion. Evidencia: docs/operations/BACKEND-SOLID-PHASE-4-VALIDATION.md.
