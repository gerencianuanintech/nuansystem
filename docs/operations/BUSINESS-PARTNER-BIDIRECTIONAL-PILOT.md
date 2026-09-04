# Piloto de socios de negocio bidireccionales

## Estado y límites

Este runbook prepara un piloto controlado de `Sucursal -> tenant central -> sucursales` para clientes y proveedores. Los bloques 1 (identidad/configuración) y 2 (propuesta, reconciliación y redistribución interna) están completos a nivel de código y pruebas automatizadas. No están activados operativamente.

Validado en código:

- doce escenarios de flujo en memoria con writer, payload, routing, promoción, relay, worker, aplicadores, política de reconciliación y merge de producción;
- contratos estáticos de las migraciones 228, 229 y 230;
- pruebas SQL de integración compilables, serializadas y opt-in;
- ausencia de llamadas SAP y de mutaciones de stock, costos, precios o documentos en este incremento.

No validado todavía:

- ejecución runtime de las pruebas SQL opt-in;
- readiness de datos de cada tenant real;
- instalación de migraciones en bases piloto;
- despliegue o activación del worker/perfiles;
- piloto funcional/UAT y rollback operativo;
- conectividad, escritura o reconciliación con SAP Business One.

No continuar si no existen respaldos restaurables, instancia SQL de pruebas separada, responsables de datos y operación, ventana aprobada y evidencia saneada. Este runbook no autoriza usar credenciales ni bases de producción en pruebas automatizadas.

## 1. Inventario, respaldo y readiness

1. Identificar una sola empresa central y todas sus sucursales piloto. Registrar `CompanyId`, `ParentCompanyId`, `IsMaster`, `SyncEnabled`, base tenant y responsable; no copiar secretos al acta.
2. Confirmar que SAP permanece fuera del alcance: no habilitar `SapSyncOutbox`, `NuanSystem.SyncWorker`, Service Layer, DI API ni HANA.
3. Crear respaldos completos de `NuanSystem_Master`, tenant central y cada tenant sucursal. Restaurar una muestra en infraestructura no productiva y registrar hora, operador, hash/nombre del backup y resultado.
4. Ejecutar de forma solo lectura `database/sql/manual/check_business_partner_bidirectional_readiness.sql` en el tenant central y en cada sucursal.
5. Archivar el resultado por tenant. Deben revisarse al menos: `GlobalId` faltantes, códigos o `SapCardCode` duplicados/largos, identificación normalizada duplicada por rol, registros `Both`, identidades de direcciones/contactos y colas BusinessPartner pendientes.

Gate: no instalar migraciones si el reporte no fue revisado y firmado por el responsable de datos de cada tenant.

## 2. Limpieza humana de datos

Resolver manualmente duplicados y registros históricos `Both`. No renumerar códigos masivamente, no dividir roles automáticamente y no borrar trazabilidad.

- Para conservar un solo rol, documentar el registro sobreviviente y la razón.
- Para conservar cliente y proveedor, crear explícitamente dos identidades: distinto `GlobalId`, `Code` y `SapCardCode`.
- Mantener el valor original de identificación y corregir la normalizada según la política aprobada.
- Repetir el readiness hasta obtener cero bloqueos o una excepción formal documentada que no impida las restricciones de las migraciones.

## 3. Instalación controlada

Aplicar siempre en este orden y registrar inicio, fin, operador y resultado por base:

1. `228_tenant_business_partner_bidirectional_foundation.sql` en tenant central y todas las sucursales.
2. `229_master_business_partner_bidirectional_governance.sql` exclusivamente en `NuanSystem_Master`.
3. `230_tenant_business_partner_bidirectional_operations.sql` en tenant central y todas las sucursales.

Reejecutar una vez cada script en la misma ventana para comprobar idempotencia. Verificar una sola fila de historia por versión `20260903.228`, `20260903.229` y `20260903.230`. No editar los guards ni usar `USE` para redirigir scripts.

La excepción de 229 para nombres `NuanSystem_Test_Master_<32hex>` existe solo para el fixture automatizado: requiere `SESSION_CONTEXT('NUANSYSTEM_INTEGRATION_TEST_MASTER_DATABASE')` igual a `DB_NAME()` y de solo lectura en la misma conexión. No es un mecanismo de despliegue.

## 4. Política central de códigos SAP

Configurar la política únicamente para la empresa central y mantenerla inicialmente deshabilitada. Elegir una opción aprobada:

- `NationalForeign`: cliente `CN`/`CE`, proveedor `PL`/`PE`;
- `RoleOnly`: cliente `C`, proveedor `P`.

Configurar el código estable que representa pasaporte. Probar en preview cliente/proveedor nacional y extranjero; comprobar normalización, longitud máxima y que cliente y proveedor con la misma identificación producen identidades separadas. Habilitar la política solo tras aprobación. Esto calcula/reserva `SapCardCode`; no autoriza un envío SAP.

## 5. Habilitación auditada de definiciones

Antes de crear o habilitar perfiles, verificar técnicamente en la versión desplegada los appliers, esquemas de payload, idempotencia, rutas cerradas y compatibilidad de las migraciones del piloto. Con esa verificación aprobada, usar exclusivamente el CRUD administrativo de definiciones de sincronización para habilitar `BusinessPartnerProposal` y `BusinessPartnerProposalResult`; no modificar los defaults SQL inactivos ni habilitar otras definiciones como parte de este piloto.

La operación debe registrar usuario, fecha UTC, valores anterior/nuevo y motivo aprobado. Conservar como evidencia saneada la consulta posterior del CRUD que muestre exactamente ambas definiciones activas, junto con el identificador del registro de auditoría y la versión desplegada. No editar tablas directamente.

## 5.1. Perfiles cerrados e inactivos

Crear perfiles separados, inicialmente inactivos:

- `BranchToMaster`: `BusinessPartnerProposal`, cada sucursal con destino exclusivo a su empresa central padre;
- `MasterToBranch`: `BusinessPartner` para todas las sucursales de la central y `BusinessPartnerProposalResult` únicamente para la sucursal origen.

Verificar que no existen destinos fuera de la familia central/sucursales, perfiles activos duplicados ni rutas ambiguas. No crear rutas hacia SAP.

## 6. Despliegue apagado

Desplegar `NuanSystem.MasterBranchSyncWorker` con:

```json
{
  "Enabled": false,
  "SkeletonMode": true,
  "SkeletonModeBehavior": "ObserveOnly",
  "EnabledEntityAppliers": [],
  "LocalOutboxRelay": { "Enabled": false }
}
```

Comprobar salud del proceso y configuración efectiva saneada. Con worker, relay y perfiles apagados, dos ciclos de observación deben dejar idénticos estados, intentos, locks y contadores de `LocalOutbox`/`SyncOutbox`.

## 7. ObserveOnly y preflight

Mantener perfiles y relay apagados, habilitar solamente el proceso en `ObserveOnly` y verificar:

- cero eventos legacy pendientes para `BusinessPartner`, `BusinessPartnerProposal` y `BusinessPartnerProposalResult`;
- cero locks nuevos y cero mutaciones de socios;
- cero `DeadLetter` no explicados;
- cero filas nuevas en `SapSyncOutbox`.

Si existe trabajo legacy, detener y clasificarlo antes de activar el relay.

## 8. Activación del piloto

Activar exclusivamente una central y sus sucursales registradas:

1. habilitar los perfiles cerrados;
2. configurar allowlist exacta con `BusinessPartnerProposal`, `BusinessPartner` y `BusinessPartnerProposalResult`;
3. cambiar `SkeletonMode=false`;
4. habilitar relay;
5. habilitar worker;
6. observar primero un solo alta de cliente y luego un solo alta de proveedor.

No ampliar empresas, entidades ni lotes durante la misma observación.

## 9. Evidencia de los doce casos

Ejecutar y conservar evidencia saneada de:

1. una transacción de sucursal crea exactamente una propuesta durable;
2. replay del mismo `EventId` no duplica socio, inbox, outbox ni distribución;
3. central no disponible deja retry durable sin perder el alta local;
4. aceptación llega a origen y todas las sucursales hermanas;
5. aplicar la réplica no publica otra propuesta;
6. duplicado activo del mismo rol se rechaza;
7. igual identificación en rol diferente se acepta con identidad y código distintos;
8. cambios concurrentes al mismo campo crean conflicto visible;
9. cambios concurrentes disjuntos se fusionan y elevan versión;
10. payloads/logs no incluyen secretos ni campos comerciales excluidos;
11. worker, relay y perfiles deshabilitados no mutan por background;
12. no se toca `SapSyncOutbox`, stock, costos, precios ni documentos.

Para cada caso registrar `GlobalId`, `EventId`, versión, estados de inbox/outbox/targets, timestamps UTC y resultado, sin cadenas de conexión, contraseñas, tokens ni payloads sensibles.

## 10. Parada y rollback operativo

Ante duplicado, loop, conflicto silencioso, ruta incorrecta, `DeadLetter` no explicado o cualquier fila SAP/comercial inesperada:

1. desactivar perfiles;
2. desactivar relay;
3. desactivar worker;
4. confirmar que no aparecen nuevos claims, locks o intentos;
5. consultar y preservar eventos `Pending`, `Error`, `DeadLetter`, inbox, conflictos y auditoría;
6. no borrar, truncar ni reescribir eventos para “limpiar” el tablero;
7. solo cuando ningún perfil activo ni otro flujo use las definiciones, deshabilitar mediante el CRUD administrativo únicamente `BusinessPartnerProposal` y `BusinessPartnerProposalResult`, registrando auditoría y verificación posterior; si siguen en uso, mantenerlas activas y escalar;
8. restaurar base solo mediante el procedimiento aprobado si la corrección forward-only no es segura.

Consultas de control deben filtrar por la empresa piloto y los tres nombres de entidad. El rollback operativo detiene procesamiento; no revierte automáticamente datos aceptados.

## 11. Criterio de salida

El piloto puede cerrarse únicamente con:

- cero `DeadLetter` no explicados;
- cero duplicados por rol y cero identidades hijas duplicadas;
- cero bucles de propuesta/canónico;
- mismo `GlobalId`, `Code`, `SapCardCode` y `CanonicalVersion` en central y sucursales después de aceptación;
- conflictos visibles y resolución solo con permiso y motivo;
- reinicio del worker sin duplicar socios, hijos, inbox, outbox ni conflictos;
- desactivación que conserva eventos recuperables;
- cero filas nuevas en `SapSyncOutbox` y cero mutaciones de stock, costos, precios o documentos.

Las pruebas SQL opt-in se habilitan únicamente en la instancia aislada con:

```text
NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS=1
NUANSYSTEM_SQL_INTEGRATION_ADMIN_CONNECTION=<conexión administrativa con Initial Catalog=master>
```

Sin `NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS=1`, se omiten con la razón: `Requiere SQL Server de integracion. Establezca NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS=1 para ejecutar.` La conexión no se guarda en `appsettings`, logs ni evidencia.
