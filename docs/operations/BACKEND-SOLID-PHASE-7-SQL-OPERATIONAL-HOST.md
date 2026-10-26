# Anfitrión operativo de validación SQL aislada

Implementación del harness .NET 10, probada offline. No se abrieron bases ni se ejecutó SQL/SAP. Se comprobó la invocación real del runtime PowerShell instalado únicamente en modo Review, sobre un paquete de ejemplo caducado, sin credenciales ni conexión.

## Entrada y activación

Construir `Phase7SqlOperationalHost` con opciones fijadas por hash, aprobación del anfitrión y proveedores de recursos/aprobaciones. `RunAsync(plan, coordinatorApproval)` revisa por defecto: verifica archivos y plan, no carga PowerShell ni solicita secretos o lanza procesos. La rama futura `execute: true` exige las aprobaciones propias del orquestador y del anfitrión, además de las de cada etapa. Los componentes no instalan runtimes, paquetes, servicios o configuración global.

La aprobación del anfitrión vincula RunId, huella del plan y `Fingerprint(options)`, con referencia independiente. Las opciones fijan rutas absolutas y SHA-256 de siete archivos: Invoke-Phase7SqlProvisioning.ps1, Phase7SqlProvisioningExecutor.psm1, System.Management.Automation.dll, dotnet ejecutable, NuanSystem.Application.Tests.dll, su .deps.json y su .runtimeconfig.json. El assembly de pruebas debe formar parte de los binarios del candidato congelado. Las dependencias y runtime local instalados son parte de la plataforma de confianza del responsable; la huella de un archivo principal no autentica toda la instalación.

Usar PowerShell compatible con .NET 10; la comprobación local utilizó PowerShell 7.6.5 y runtime 10.0.11 ya instalados. No hay rutas de máquina o versiones instaladas codificadas como fallback. El motor carga por reflexión la instalación aprobada y verifica que el assembly efectivamente utilizado corresponda a esa ruta. No inicia pwsh.exe ni interpreta código recibido como texto.

## Recursos suministrados por el responsable

`Phase7SqlOperationalResources` contiene cuatro puntos de entrega: recursos de provisión, aprobación de suite, cuatro conexiones de suite y aprobación/recursos del monitor. Son decisiones y credenciales externas, no implementaciones pendientes del lanzamiento o provisión. El anfitrión implementa los bindings de plataforma y los conecta al orquestador.

- Provisión: SqlConnection administrativa ya abierta, aprobación completa requerida por el ejecutor PowerShell y cinco SecureString distintos con claves AppA, ObserverA, AppB, ObserverB y Monitor. No abrir ni resolver secretos desde el constructor o Review. El ejecutor existente conserva su validación de identidad, servidor, ventana, TLS, opciones y permisos. Un recurso cerrado es rechazado por ese ejecutor, nunca se abre automáticamente.
- Suite: aprobación basada en inventario real posterior a la provisión, o null para mantener el estado pendiente. Se conservan los archivos preparados por el orquestador. La lista de conexiones contiene exactamente los cuatro nombres env: admitidos por la suite. Se valida cada cadena contra servidor, base, usuario, TLS y límites, sin abrirla.
- Monitor: aprobación independiente y conexión ya abierta en master conforme a su ejecutor, o null para mantener la etapa pendiente. Se entrega el Process del testhost capturado durante el enlace local validado; no se sustituye después por otro PID.

No poner credenciales en JSON, argumentos, comentarios, repositorio o logs. Los SecureString se copian para la invocación y se liberan al terminar la operación real; no se convierten a texto por el anfitrión. La conversión necesaria para crear logins pertenece al ejecutor ya revisado. Las cadenas de suite existen en memoria y en el entorno privado del hijo; .NET/Windows no garantizan borrado inmediato de todas las copias administradas o del entorno. No habilitar transcripciones, trazas de parámetros ni volcados de memoria.

## Adaptador PowerShell concreto

Se invoca el archivo de entrada existente mediante un script fijo `param($entry,$parameters) ... & $entry @parameters`, con objetos pasados por AddParameter. No se concatena código, se usa Invoke-Expression, se carga un perfil personal ni se pasan secretos en consola. La revisión devuelve RunId/hash/lotes sin SQL; solo tras esa respuesta válida se permite provisión. Se rechazan respuestas que declaren SAP, cierre global, ejecución durante Review, otro hash o conteos inconsistentes.

Los scripts, módulo y runtime se fijan por hash y se mantienen abiertos sin permiso de escritura mientras dura la invocación real. La cancelación solicita Stop del runspace y llega al token del ejecutor. Si la operación no coopera, el orquestador limita la espera; las referencias y bloqueos no se liberan antes de terminar la operación real. Los streams del motor no se registran ni se propagan como texto del proveedor. Se usa exclusivamente el diario de provisión esperado y se calcula su hash tras recibir el resultado terminado.

## Adaptador concreto de suite

Se inicia el dotnet aprobado con `vstest`, el assembly congelado y `Phase7SqlExecutionCatalog.RestrictedFilter`. Se fija el TRX sql.trx y ResultsDirectory; no se usa shell, dotnet test con build, restore ni selección libre de pruebas. Los cinco archivos preparados se comparan con la aprobación; se bloquean por hash durante el hijo, igual que sus binarios de candidato y archivos de runtime aprobados.

El entorno se construye desde cero con una lista pequeña de variables de sistema/runtime. Los dos gates SQL se activan solamente en el hijo y SAP se fija en 0. Solo se añaden sus cuatro conexiones y las rutas aprobadas; no se heredan otras credenciales, startup hooks, profilers o gates del padre. La consola se drena sin persistir texto y se limpian buffers. Se distinguen runner y testhost: se esperan ambos y se conservan sus exit codes por separado. Antes de cualquier conexión del fixture, el testhost envía RunId, nonce, PID, inicio y huella de assembly por un named pipe CurrentUserOnly. El padre verifica desafío, sesión y ascendencia Windows, captura una sola vez el handle del testhost vivo y responde ACK. El monitor recibe ese handle, no el del runner. La búsqueda inicial por PID solo se realiza tras validar el desafío y mientras el testhost espera ACK; no hay enumeración ni sustitución tardía de procesos. Una salida no cero del runner o testhost mantiene bloqueos de diagnóstico.

SuiteLoader admite `NUANSYSTEM_PHASE7_RUN_ROOT`, fijada al SuiteRoot del contexto, para que los artefactos no dependan del CWD asignado por vstest. Exige ruta absoluta, RunId final y estructura artifacts/phase7-sql-executions/RunId, sin enlaces. Si no se suministra, conserva la ruta anterior. La variable se lee solo dentro de un Fact SQL autorizado, no durante descubrimiento ni pruebas offline.

## Limpieza y límites

Stop cancela la vida del anfitrión, pide Stop de PowerShell y termina únicamente el árbol del hijo iniciado por este launcher, si sigue activo. No enumera procesos ni mata por nombre/PID; no borra bases ni hace rollback global. Conserva el Process para el monitor. Los recursos devueltos después de Stop se rechazan y se intenta cerrar su conexión, sin reactivar el flujo.

La limpieza es idempotente y tiene un token/plazo propio máximo de cinco segundos, independiente de una cancelación previa. Se solicita cierre de las conexiones entregadas, sin Open. El orquestador conserva el fallo principal y registra fallos de limpieza. Si un proveedor o proceso no confirma su cierre, la ejecución queda incompleta para intervención autorizada; no se declara éxito ni se inicia otra etapa.

## Validación de esta entrega

Las pruebas `Phase7SqlOperationalHostTests` usan un motor y launcher simulados. No lanzan dotnet/vstest ni PowerShell con Execute. Las SqlConnection construidas en fixtures permanecen cerradas. Se comprueba el entorno y argumentos del hijo, autorización/pins, archivos preparados, respuestas del motor, cancelación, comandos rechazados y ruta explícita. La revisión nativa PowerShell separada verificó 639 lotes con SqlExecuted=false y SapExecuted=false; no sustituyó ninguna prueba real de SQL.

Los ensayos reales de provisión, apertura de sesiones por el responsable y suite SQL siguen pendientes de autorización y laboratorio. Autenticidad y cierre global de fase 7 permanecen false. Evidencias en `artifacts/backend-solid-phase7-sql-operational-host/`.

Resultado final: **33 pruebas nuevas y 1824 pruebas offline totales aprobadas, 0 fallidas y 0 omitidas**. Compilación Release sin restauración: 0 errores y 0 advertencias. La revisión nativa del paquete validó 639 lotes con SQL/SAP desactivados; el enlace local identificó al testhost existente sin iniciar otro proceso. Se preservaron 4819 archivos preexistentes sin cambios; los tres ajustes de integración existentes tienen copias previas verificadas. La primera regresión conservada falló en una carrera de cancelación de limpieza; la corrección pasó el caso específico y toda la repetición final. No se abrieron conexiones ni se ejecutaron SQL, SAP o la suite real del laboratorio. Quedan pendientes configuración y aprobaciones operativas, permisos efectivos, ensayo SQL real y revisión de autenticidad/cierre de fase 7.
