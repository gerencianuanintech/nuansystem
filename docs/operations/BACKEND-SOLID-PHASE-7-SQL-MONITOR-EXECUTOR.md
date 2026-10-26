# Ejecución controlada del monitor independiente — fase 7

Estado: implementación y pruebas offline. No se abrió ninguna conexión ni se ejecutó SQL o SAP. El ensayo real, permisos efectivos y cierre de fase 7 siguen pendientes.

## Punto de entrada y propiedad

`Phase7SqlMonitorExecutor` pertenece a la infraestructura de validación de `NuanSystem.Application.Tests`, junto al lector existente. Se invoca desde el mismo anfitrión .NET 10 que coordina la suite y conserva su objeto Process hijo. No es un worker, endpoint, servicio ni un comando PowerShell que cargue un ensamblado de pruebas en otro runtime.

`ExecuteAsync(inputs, suiteApproval, monitorApproval, isolation)` revisa por defecto. No requiere conexión/proceso y no crea salida. Para capturar datos en una ejecución futura autorizada se proporcionan `connection`, `child` y `execute: true`. No hay Open, búsqueda de credenciales, lectura de variables de entorno, inicio de procesos, Kill ni concesión de permisos dentro del ejecutor.

La conexión debe estar ya abierta en master, ser exclusiva del monitor, coincidir con el DataSource aprobado y tener TLS con certificado validado, Pooling=false, Enlist=false, FireInfoMessageEventOnUserErrors=false y ninguna transacción ambiental. El lector verifica servidor, versión, login original y actual, ausencia de sysadmin y transacciones, y visibilidad DMV. Los permisos se preparan mediante el paquete de [aprovisionamiento](BACKEND-SOLID-PHASE-7-SQL-PROVISIONING.md), usando una identidad independiente de App/Observer. La aprobación de esta implementación no autoriza abrir esa conexión.

Al aceptar recursos para ejecutar, el ejecutor asume el intento de cierre de la sesión; el llamador no debe usarla concurrentemente. Un fallo de cierre impide publicar el índice final. Si coinciden un error principal o cancelación y un fallo de cierre, se conserva el principal y se marca `MonitorCleanupFailed` sin adjuntar datos del proveedor. Un recurso rechazado antes de su admisión sigue bajo responsabilidad del llamador.

## Entradas y autorización independiente

El responsable aporta, por separado de los archivos, seis hashes SHA-256 aprobados con estas rutas exactas relativas a la raíz de corrida:

| Archivo | Validación |
| --- | --- |
| manifest.json | Huella y aprobación de la suite; dos destinos y empresas aprobados |
| global/start.json | RunId, PID, inicio UTC y catálogo |
| global/end.json | Mismo RunId/PID, límites temporales, gates desactivados y sin SAP, workers o llamadas externas |
| sql.trx | Ventana del test y 21 resultados del protocolo; el juicio completo de resultados corresponde al empaquetador |
| global/final-A.json | Snapshot físico de las 16 tablas |
| global/final-B.json | Snapshot físico de las 16 tablas |

Se rechazan rutas ambiguas, enlaces, tamaños excesivos, hashes incorrectos, JSON duplicado y snapshots inválidos con los componentes ya existentes. Los bytes verificados se retienen; no se vuelven a leer los snapshots durante la comparación. La raíz de salida debe ser absoluta, nueva y estar separada de la raíz de entrada. No se sobrescriben archivos ni se borran datos.

`Phase7SqlControlledMonitorApproval` contiene la aprobación existente `Phase7SqlMonitorApproval`, `InputsSha256`, `CommandTimeoutSeconds`, `TotalTimeoutSeconds` y `ExecutionAuthorized`. El hash del índice canónico se obtiene mediante `InputFingerprint(inputs)` y debe revisarse y aprobarse externamente; calcularlo no constituye autorización. La referencia del monitor debe ser distinta de las referencias de ejecución y aprovisionamiento de la suite. Se rechazan sa, dbo, guest y los cuatro principales App/Observer. El objeto aprobado es una entrada de confianza del responsable, no una firma verificada contra un servicio externo.

La ventana del monitor es UTC, de hasta 24 horas; la captura ocurre después del fin del TRX y dentro de esa ventana. El presupuesto por comando es de 1–120 segundos y el total de 1–3600 segundos, con el primero no mayor que el segundo. El límite efectivo es el menor entre presupuesto total, fin de autorización y fin de aislamiento de workers. La cancelación llega al lector y al proveedor. La limpieza tiene un plazo independiente de cinco segundos, incluso si CloseAsync bloquea síncronamente antes de devolver una tarea. No se programa un segundo cierre concurrente. Si el proveedor no termina, el cierre puede seguir pendiente fuera del ejecutor: se informa fallo, no se publica el índice y el responsable debe revisar el recurso; no se declara una sesión cerrada.

`Phase7SqlMonitorIsolation` declara inspector, referencia de autorización y ventana en que SyncWorker, MasterBranchSyncWorker, SriWorker y Relay permanecen desactivados. Debe cubrir el TRX y la captura. Es una atestación del inspector: el ejecutor no administra ni inspecciona automáticamente esos servicios.

## Flujo futuro autorizado

1. Terminar la suite y conservar el objeto del proceso hijo original. No buscar un PID arbitrario para sustituirlo. El ejecutor comprueba PID, RunId, inicio anterior al arranque de la suite y salida observada posterior al TRX; no espera indefinidamente ni termina procesos.
2. Congelar las seis entradas, aprobar sus hashes y la autorización del monitor; efectuar Review. Revisar que se trata de la corrida y el servidor aprobados.
3. El responsable abre la conexión independiente fuera del ejecutor y entrega su propiedad para una única captura con activación explícita.
4. El ejecutor registra intención local, verifica el proceso terminado y consulta sesiones de los cuatro ApplicationName del RunId, transacciones de A/B, marcadores y snapshots. No modifica SQL ni intenta reparar sesiones o datos.
5. Compara huellas exactas y conteos de tablas contra los finales congelados. Un exit code no cero, sesiones/transacciones pendientes o diferencia de retención produce una captura negativa para revisión, sin aceptación de los controles posteriores.
6. Solicita el cierre de la conexión y solo después de confirmarlo publica el índice de archivos. No reintenta fallos. Si falla la lectura, cancelación, presupuesto, auditoría o cierre, no se publica un índice completo; conservar los archivos parciales para diagnóstico.

## Salidas y consumo

Una captura completa genera intent.json, observation.json, witness.json, inputs-index.json, report.json y capture-index.json. El resultado devuelve el SHA-256 del índice; guardar ese valor por el canal de revisión aprobado. Verificar el hash del índice y cada ruta, tamaño y hash que contiene antes de consumir objetos. La mera existencia del archivo no demuestra completitud: una interrupción de escritura puede dejar un archivo parcial.

El lector conserva solamente conteos y hashes de los snapshots en la observación; el ejecutor no duplica sus filas en esta salida. No registra cadenas de conexión, SQL ni detalles del proveedor. Los errores se expresan mediante códigos estables. Los archivos finales de la suite pueden contener datos sensibles y deben permanecer en el laboratorio aprobado.

`SimulateAsync` usa un transporte de pruebas y fuerza Origin=Offline tanto en el resultado como en el testigo persistido, aunque el fake entregue otra etiqueta. Puede demostrar coherencia del protocolo, nunca evidencia SQL real. Review y simulación indican SqlExecuted=false. Toda salida mantiene EvidenceAuthenticityVerified=false y GlobalPhase7Closed=false. ObservationsConform se limita a los controles de terminación/retención y declaraciones recibidas; no certifica las 21 pruebas, permisos, autenticidad ni cierre global.

Tras una captura real y la verificación independiente del índice, los objetos Observation y Witness, junto con la aprobación del monitor, se entregan a `Phase7SqlEvidencePackager.Build` según el [plan de evidencias](BACKEND-SOLID-PHASE-7-SQL-EVIDENCE.md). El empaquetador y la revisión humana siguen evaluando las demás pruebas, fuentes y atestaciones. No hay empaquetado ni publicación automáticos desde el monitor.

## Validación offline

Se ejecutan las pruebas `Phase7SqlMonitorExecutorTests` con transporte simulado; se excluyen todas las integraciones SQL y SAP. Las pruebas de recursos reales solo construyen una SqlConnection cerrada para comprobar su rechazo, sin Open ni consultas. La simulación no ejecuta procesos externos ni inicia servicios. Evidencias en `artifacts/backend-solid-phase7-sql-monitor-executor/`.

Resultado final: **37 pruebas nuevas del monitor y 1726 pruebas totales offline aprobadas, 0 fallidas y 0 omitidas**. Solución Release sin restauración: 0 errores y 0 advertencias. Filtros de exclusión SQL/SAP y sus gates permanecieron desactivados. La revisión independiente y sus regresiones se cerraron; autenticidad, permisos y ejecución SQL reales siguen sin validar. Los archivos previos se conservaron y no se cambió rama ni HEAD.

## Integración verificada con el empaquetador

Para consumir una captura completa usar ahora `BuildFromMonitorCapture`, con la huella externa aprobada de capture-index.json y la aprobación independiente del monitor. Esta entrada verifica integridad y vínculo con la corrida, conserva los originales y los revalida dentro del ZIP. El método Build basado en objetos permanece compatible. Procedimiento y límites: [integración de capturas](BACKEND-SOLID-PHASE-7-SQL-MONITOR-PACKAGING.md). Ninguna de estas comprobaciones verifica autenticidad ni cierra fase 7.
