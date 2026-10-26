# Fase7 SQL aislada — tarea1: manifiesto y guardas offline

Fecha: 2026-10-24. Tarea1 implementada; tareas2–6 y SQL real pendientes. No se ejecutó SAP ni se abrió conexión a ninguna base.

## Entrega

Infraestructura nueva, exclusivamente en el proyecto de pruebas:

- `Infrastructure/Phase7Sql/Phase7SqlManifest.cs`: esquema del manifiesto, aprobación independiente, destinos y resultado offline.
- `Infrastructure/Phase7Sql/Phase7SqlManifestValidator.cs`: validación pura, sin factories, drivers SQL, archivos de configuración o resolución de secretos.
- `Infrastructure/Phase7Sql/Phase7SqlGates.cs`: dos flags explícitos; ReadEnvironment lee solo sus nombres. Reutiliza el parser estándar existente.
- `Features/BusinessPartners/Phase7SqlSafetyTests.cs`:65 casos normales de categoría OfflineSafety, sin atributo de integración SQL que los omita con gates apagados.
- `Fixtures/Phase7Sql/manifest.example.json`: plantilla inerte, sin secretos y no aceptable como destino de ensayo.

El único cambio de código existente es hacer `SqlServerIntegrationFactAttribute.IsEnabled` internal en lugar de private para reutilizarlo. La comparación contra la copia previa confirma que no cambió su lógica: acepta únicamente1 o true, sin distinguir mayúsculas y sin normalizar espacios.

## Uso y frontera de confianza

La entrada es `Phase7SqlManifestValidator.Validate(json, approval, gates, utcNow)`. El llamador de confianza proporciona aprobación, hora UTC y flags por separado; el JSON no puede incluir un campo isAuthorized ni una aprobación propia. El resultado conserva una instantánea de colecciones de solo lectura y siempre devuelve RequiresOnlineVerification=true.

El objeto Phase7SqlApproval contiene referencias distintas de autorización de ejecución/preparación, operador y RunId aprobados, SHA-256 del manifiesto, endpoint/ServerName esperados, identidades A/B y ventana UTC. No se crea ningún registro de aprobación real en esta tarea. La unidad prueba esos datos con valores simulados, no con autorizaciones de laboratorio efectivas.

**Importante:** la consistencia con ese objeto no verifica una firma humana, la existencia del servidor, sus permisos reales ni la vigencia de un documento fuera del proceso. La futura fixture debe obtenerlo de un canal confiable independiente, leer los gates reales y realizar las comprobaciones online de la tarea2 antes de DML. No usar un objeto de prueba o el propio JSON para autoaprobarse. Este resultado no es una capacidad para ejecutar SQL ni un mecanismo de protección contra repetición de autorizaciones.

El hash corresponde al texto exacto recibido, codificado UTF-8 sin BOM: cualquier cambio de formato, espacios u orden invalida la aprobación anterior. No recalcularlo serializando la instantánea resultante. Guardar el manifiesto en UTF-8 sin BOM permite comparar el hash de sus bytes con la huella aprobada; la lectura acotada de archivos reales pertenece a la futura fixture.

## Contrato del manifiesto versión1

Todas las propiedades y parámetros son obligatorios. El JSON usa nombres camelCase y es sensible a mayúsculas; rechaza campos desconocidos, duplicados, null no admitidos, comentarios, trailing commas y profundidad mayor de12. Límite:65536 bytes UTF-8.

| Bloque | Validación offline |
|---|---|
| Identidad y aprobación | schemaVersion=1, RunId no vacío y coincidente, operador y referencias explícitas, hash exacto aportado independientemente. |
| Ventana | Fechas UTC; ventana del manifiesto contenida en la aprobación; hora actual dentro; presupuesto completo disponible antes de expiración. |
| Duración | Ejecución1..3600 segundos, comando1..120 y comando no mayor que ejecución. La futura fixture debe aplicar efectivamente esos límites. |
| Servidor | Endpoint canónico hostname/IPv4, tcp: opcional y puerto válido o instancia nombrada; no connection string/URI/ruta. Coincidencia con allowlist externa y ServerName esperado. Versión numérica de4 componentes. |
| Destinos | Exactamente TenantA/RoleA y TenantB/RoleB; nombres NuanSystem_Phase7_<RunIdN>_A/B, DB_ID>4, CompanyId>0 y coincidencia con identidades aprobadas. Dos bases/ids/empresas distintos. |
| Marcadores | RunMarker igual al RunId, Purpose=BackendTransactionValidation. Se verifican declaraciones, no propiedades reales de SQL todavía. |
| Esquema | Compatibilidad130/140/150/160/170; collation declarada; RCSI declarado; SHA-256 no cero y6..64 huellas de procedimientos dbo, con los seis entrypoints actuales requeridos y sin nombres ambiguos por casing. |
| Principales | Nombres válidos y distintos para ejecutor/observador; se rechazan cuentas y roles administrativos conocidos. No equivale a inspeccionar membresías reales. |
| Permisos | Ejecutor: SELECT/EXECUTE/INSERT requeridos, UPDATE opcional; sin DDL, DELETE físico o roles. Observador: solo SELECT. La limitación real por objetos se comprueba en tarea2. |
| Referencias de secretos | Solo env:NUANSYSTEM_PHASE7_TENANT_A/B_CONNECTION_STRING y env:NUANSYSTEM_PHASE7_OBSERVER_A/B_CONNECTION_STRING, según rol. Son nombres, nunca se leen sus valores. |

La allowlist no se extrae del mismo documento: declarar otro servidor o un nombre con prefijo de laboratorio no basta. La plantilla usa RunId vacío, IDs0, hashes cero, endpoint de reemplazo y ventana vencida; se comprobó que no pase validación incluso si un test aporta su huella. No es una configuración por defecto ni se carga automáticamente.

Los errores usan Phase7SqlGuardException con ErrorCode estable y mensaje genérico. No devuelven JSON, credenciales, datos del parser ni inner exceptions. Familias: gates/manifiesto, aprobación, ventana/límites, servidor/destinos, marcadores/esquema, principales/permisos y referencias a secretos.

## Pruebas y verificación

- Pruebas escritas primero: compilación RED por ausencia de los nuevos contratos.
- Primer grupo:45 casos aprobados.
- Casos límite adicionales reprodujeron9 rechazos faltantes: saltos de línea finales aceptados por el ancla regex `$` y nombres de roles administrativos. Se corrigieron con consumo de todo el valor y denylist de roles; no se relajaron aserciones.
- Resultado enfocado final:65/65 aprobadas.
- Solución Release:0 errores y0 advertencias.
- Regresión local:1328/1328 aprobadas,0 fallos,0 omitidas; se conservan las1263 pruebas anteriores, incluidos los tres contratos WinForms corregidos.
- Revisión independiente de solo lectura: sin hallazgos accionables; confirmó pureza offline, hash de aprobación y límites de las afirmaciones.

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUANSYSTEM_RUN_PHASE7_SQL_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~Phase7SqlSafetyTests' --logger 'trx;LogFileName=task1-offline-final.trx' --results-directory artifacts/backend-solid-phase7-sql-task1/offline-final --verbosity minimal
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=task1-final.trx' --results-directory artifacts/backend-solid-phase7-sql-task1/final --verbosity minimal
```

Los flags solo afectan el proceso hijo y permanecen apagados durante los comandos. Los casos positivos pasan flags/aprobaciones sintéticos como parámetros puros; no cambian el entorno del proceso. Se excluyen SQL externo y piloto SAP; los ensayos HTTP previos siguen totalmente en memoria. Build/test fuera del sandbox permiten SDK Windows y testhost local, sin nuevas dependencias, instalaciones ni configuración global.

## Preservación y estado

Rama/HEAD conservados: codex/auxiliary-master-generator, e97124a1e2863f066c1fa55ea992c78179f01623. Snapshot de183 archivos previos modificados/no rastreados:182 conservan SHA-256 exacto y ninguno falta. El único de ese conjunto editado es el plan para marcar la tarea1 y precisar su contrato; el atributo anterior se guardó aparte y su lógica se comprobó intacta. No se modificaron archivos productivos, SQL, WinForms ni catálogos internos. Sin stash/reset/commit/cambio de rama.

Evidencia local ignorada por Git, en artifacts/backend-solid-phase7-sql-task1/: baseline.json, before/, red-build.log, edges-red/task1-edges-red.trx, offline-final/task1-offline-final.trx, build-final.log, final/task1-final.trx y final/summary.json. La revisión de espacios de archivos nuevos y diff pasó.

No existen todavía Phase7SqlFixture, Phase7SqlSessionProbe ni fault-injection.sql; no se implementó la suite SQL de S01–S18, no se provisionó laboratorio y no se resolvieron secretos. El gate SQL de fase7 continúa pendiente. Siguiente tarea: preparar su fixture y trazabilidad solo dentro del alcance que se autorice; este incremento no autoriza conectarse ni escribir bases.
