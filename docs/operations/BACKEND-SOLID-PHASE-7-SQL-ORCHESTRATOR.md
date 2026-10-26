# Orquestación controlada de la validación SQL aislada

Implementación del coordinador del harness .NET, validada exclusivamente offline. No ejecuta SAP ni abre conexiones por sí mismo. No agrega un servicio, endpoint, instalador o lanzamiento automático de la validación real.

## Entrada y límites

`Phase7SqlValidationOrchestrator.ExecuteAsync(plan, approval)` hace Review por defecto: comprueba plan, huella externa, candidato congelado, paquete de provisión y rutas; no llama a ningún adaptador ni crea salida. Esta revisión local no sustituye la revisión nativa completa del paquete PowerShell.

`SimulateAsync` usa un puerto de pruebas y fuerza origen Offline. La rama futura `ExecuteAsync(..., bindings, execute: true)` exige adaptadores explícitos del anfitrión para coordinar PowerShell y el proceso .NET. No hay fallback de credenciales, detección de servidores, apertura de conexiones, Process.Start, Kill o cambios globales de entorno dentro del coordinador. El anfitrión autorizado conserva esas responsabilidades fuera del componente.

El plan vincula RunId, raíz nueva de salida, ruta y SHA-256 del bundle-manifest.json, autorizaciones de preparación/provisión, candidato, ventana UTC de hasta 24 horas y presupuestos total/por etapa de 1–3600 segundos. El presupuesto de etapa no excede el total. `Fingerprint(plan)` obtiene la huella para revisión externa; el cálculo no concede permiso. La aprobación separada requiere mismo RunId/huella y una referencia propia, distinta de preparación y provisión. El candidato y el paquete se verifican antes de tocar adaptadores. Sus raíces no pueden solaparse con la salida.

Se conserva la especificación de provisión aprobada y se compara con la suite: servidor, versión, empresas, compatibilidad, collation y RCSI deben coincidir. La aprobación independiente de suite no permite redirigir el flujo a otro laboratorio. Se conserva exactamente el plan aprobado, sin normalizar posteriormente los datos representados por su huella.

## Etapas y aprobaciones

| Etapa | Control para avanzar |
| --- | --- |
| ReviewProvisioning | La revisión nativa declara mismo RunId/hash, sin SQL y número de lotes esperado |
| Provision | Diario fijado por hash, Admitted/Started/Completed/Finished, nueve etapas ordenadas, todos los pares de lote y sesión terminada sin fallos |
| ApproveSuite | Manifiesto/aprobación obtenidos después de observar IDs, esquema, permisos y semillas; candidato, catálogo y baseline offline coinciden |
| Suite | Evidencias en la raíz esperada, manifiesto/aprobación/inputs exactos, PID consistente e inicio/fin dentro de esta ejecución |
| Monitor | Proceso original terminado, aprobación distinta y captura en la ruta esperada; proceso, salida, origen e índice verificados |
| Package | BuildFromMonitorCapture y VerifyZip con huella externa; ninguna promoción de capturas simuladas |
| Stop | Anfitrión confirma cancelación/cierre de sus recursos y gates desactivados; plazo independiente de cinco segundos |

No se fabrican DB_ID ni huellas antes de aprovisionar. El anfitrión puede esperar una aprobación externa con el token/plazo de etapa; si devuelve null en ApproveSuite o Monitor, el resultado es AwaitingSuiteApproval o AwaitingMonitorApproval. Se limpian recursos y no se ejecuta la siguiente etapa. El tiempo transcurrido no autoriza nada. No hay reanudación automática de esa carpeta: cualquier continuación requiere revisar las evidencias y usar los puntos de entrada previamente autorizados; no volver a aprovisionar sobre recursos existentes.

Cada excepción, cancelación o timeout detiene el avance y solicita Stop una vez. La espera acotada contempla callbacks que bloquean antes de devolver una Task. Un callback no cooperativo podría continuar fuera de la espera; no se intenta iniciar otra etapa o reutilizar la ejecución. La limpieza tiene cinco segundos adicionales, no declara recursos cerrados si no se confirma. No se elimina laboratorio, se hace rollback global ni se publica éxito al fallar la limpieza.

Una salida no cero de la suite con evidencias completas puede monitorizarse y empaquetarse para diagnóstico; se conserva CompletedWithBlockers. Una excepción, proceso no terminado, fuentes incompletas o captura incoherente impiden avanzar. En un estado pendiente de una ejecución real, SqlExecuted=true indica que la provisión ya ejecutó SQL; no significa que haya pasado la suite. Review y simulación siempre indican false.

## Contrato del anfitrión nativo

`Phase7SqlValidationHostBindings` requiere las seis funciones siguientes; no son implementaciones que el coordinador active por defecto:

- ReviewProvisioning: invocar la revisión de `Invoke-Phase7SqlProvisioning.ps1` con el bundle y hash del contexto. Devolver RunId, ManifestSha256 y BatchCount observados; SqlExecuted=false.
- Provision: invocar ese ejecutor con la autorización propia, conexión administrativa ya abierta y cinco SecureString, sin persistir secretos. Usar exactamente `context.ProvisioningJournalPath`; calcular el hash solo tras su terminación. El diario debe contener Finished, emitido después del cierre confirmado.
- ApproveSuite: recoger el inventario real, recibir aprobación del responsable y devolver `Phase7SqlApprovedSuite` o null. No aprobar automáticamente a partir de datos supuestos. El baseline debe haberse completado antes de esta etapa.
- RunSuite: utilizar el candidato aprobado, sin reconstruirlo/restaurarlo, y el filtro `Phase7SqlExecutionCatalog.RestrictedFilter`. Pasar al hijo los archivos de `context.PreparedDirectory` y las referencias/secretos por mecanismos aprobados; SAP y workers/relay permanecen desactivados. Activar los dos gates SQL solo en ese hijo. Asegurar que el directorio de trabajo efectivo del testhost produzca exactamente `context.SuiteRoot`, según el contrato actual de SuiteLoader. El TRX final se llama sql.trx en esa raíz. Esperar la salida, devolver el objeto Process original, PID y exit code; no sustituirlo por un PID buscado después.
- ApproveMonitor: recibir aprobación y conexión exclusiva ya abierta en master, con la configuración exigida por el ejecutor del monitor. Devolver recursos para las rutas SuiteRoot/MonitorDirectory del contexto, o null. El adaptador nativo llama directamente a Phase7SqlMonitorExecutor.ExecuteAsync; no reconstruye observaciones.
- Stop: cooperar con cancelación, desactivar los gates del hijo y cerrar sesiones/runspaces/proceso propios según los procedimientos autorizados. Debe ser seguro también tras éxito y ante recursos parcialmente preparados. Nunca modificar datos o configuración global como mecanismo de limpieza.

Los adaptadores de provisión y arranque son un puente de confianza del anfitrión; el coordinador verifica sus recibos y artefactos, pero no convierte un callback arbitrario en código confiable ni autentica una firma humana. Esta entrega no implementa un anfitrión específico de PowerShell/.NET ni abre sesiones para completarlo: proporciona la coordinación, el contrato y el adaptador nativo del monitor. El empaquetado y su verificación están integrados directamente.

## Evidencias y lectura del resultado

La raíz nueva conserva intent.json, plan.json, approval.json, recibos Started/Completed por etapa, provisioning.jsonl, approved-suite, suite-work, monitor-capture y package/ZIP cuando corresponda. La aprobación archivada no contiene conexiones ni SecureString. Un fallo conserva failure.json si puede escribirse, con código genérico y marca de limpieza, sin excepción del proveedor. Una cancelación permanece como error principal si también falla Stop.

Solo después de Stop confirmado se escribe orchestration-result.json, con el hash del ZIP cuando existe. Una carpeta con archivos parciales o incluso un ZIP no constituye éxito del orquestador si falta ese resultado. Verificar siempre el ZIP mediante su huella aprobada; las etiquetas por sí solas no prueban autenticidad. EvidenceAuthenticityVerified y GlobalPhase7Closed permanecen false. Las simulaciones conservan los bloqueos de evidencia no SQL y nunca cierran fase 7.

Pruebas: `Phase7SqlValidationOrchestratorTests`, usando diarios y laboratorios fabricados, puerto de ejecución simulado, ejecutor offline del monitor y empaquetador real de archivos. No se ejecutan scripts SQL ni Facts SQL/SAP. Evidencias en `artifacts/backend-solid-phase7-sql-orchestrator/`.

Resultado final offline: **32 pruebas nuevas y 1791 pruebas totales aprobadas, 0 fallidas y 0 omitidas**. Solución Release sin restauración: 0 errores y 0 advertencias. Se excluyeron las integraciones SQL/SAP y se mantuvieron sus gates desactivados. Los 4817 archivos preexistentes inventariados conservaron sus hashes; esta entrega solo añade código y documentación. Rama y HEAD se conservaron. Revisión independiente y regresiones de limpieza cerradas. No se abrieron conexiones ni se ejecutó SQL o SAP. Quedan pendientes el anfitrión autorizado de la plataforma, las aprobaciones por etapa y el ensayo real del laboratorio; no se declara autenticidad ni cierre global.

## Anfitrión operativo disponible

`Phase7SqlOperationalHost` implementa ahora los bindings de plataforma: invocación del ejecutor PowerShell instalado y lanzamiento restringido de la suite, además de limpieza y transferencia de recursos. Ver [anfitrión operativo](BACKEND-SOLID-PHASE-7-SQL-OPERATIONAL-HOST.md). El proceso real de tests se identifica mediante un enlace local validado antes de abrir conexiones; se conserva ese handle y no se confunde con el runner vstest. El plan/configuración aprobados, los recursos de sesión y el ensayo SQL real siguen pendientes de entrega y autorización por el responsable.
