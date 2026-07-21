# SRI Worker - despliegue y validacion controlada

## Estado y limite

Este runbook aplica a la Fase 5.3 implementada y conserva la evidencia operativa de la Fase 5.4. El script `117` fue desplegado y SQL-validado en los tres tenants piloto. La consulta end-to-end autorizada para `NuanSystem_DEMO` se completo el 2026-07-20; este antecedente no autoriza repetirla, habilitar permanentemente el worker ni operar otros tenants. Cada ejecucion futura requiere alcance explicito del propietario.

## Evidencia SQL cerrada el 2026-07-20

- script `117` ejecutado dos veces sin duplicados en `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`;
- `NuanSystem_Master` sin version ni objetos de tenant;
- claim concurrente exclusivo y filtrado por ambiente;
- recuperacion de lease vencido y proteccion contra propietario ajeno;
- almacenamiento `Authorized` atomico, repeticion idempotente y conflicto SHA-256 estable;
- rechazo SQL de contenido mayor de 5 MiB;
- fixtures eliminados y working tree limpio;
- worker deshabilitado, cero procesos y cero llamadas al SRI.

## Evidencia end-to-end de Fase 5.4 cerrada el 2026-07-20

La validacion controlada se ejecuto en la rama `refactor/codex-skills-v5-sri-worker`, commit `2e4233c30d8383ccb65af26e89b4370329ea1143`, con working tree limpio. Se uso una sola fila preparada mediante el procedimiento oficial de enqueue en `NuanSystem_DEMO`; la evidencia se identifica por `QueueId = 10004` sin registrar la clave de acceso completa ni el XML.

- solo `DEMO` estaba habilitada para SRI y su ambiente configurado era `Production`; `NuanSystem_Master`, Remigio y Canaris no recibieron escrituras durante la prueba;
- el snapshot previo tenia cero filas elegibles, cero locks, cero intentos y cero documentos autorizados; la clave aprobada no estaba registrada;
- el worker se habilito exclusivamente mediante configuracion temporal del proceso desde PowerShell normal, con `Encrypt=true`, `TrustServerCertificate=false` y validacion completa del certificado;
- se consulto una vez el endpoint HTTPS oficial `cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline` y el proveedor devolvio ambiente `PRODUCCION`;
- la cola transiciono `Pending -> Querying -> Authorized` con un solo claim, `AttemptCount = 1`, un intento `AuthorizationLookup` cerrado como `Authorized` y un solo registro en `SriAuthorizedDocuments`;
- `QueueId` y `AttemptId` del documento coincidieron con la ejecucion; `SizeBytes = 10027` coincidio con `DATALENGTH(XmlContent)`;
- el SHA-256 persistido tenia exactamente 32 bytes y coincidio con `HASHBYTES('SHA2_256', XmlContent)`;
- la auditoria contenia exactamente `Enqueue`, `Claim` y `Authorized`, con las transiciones esperadas;
- al finalizar, los campos de lease quedaron nulos, no habia locks activos y no quedaban procesos `NuanSystem.SriWorker`;
- los logs capturados y el log persistido del worker no contenian la clave completa, XML, credenciales, cadenas de conexion ni secretos;
- un segundo ciclo controlado mantuvo estado `Authorized`, un intento, un claim, tres auditorias y un documento; produjo cero trabajos procesados y, por tanto, no repitio la consulta al SRI;
- la primera ejecucion del worker duro 1.673 segundos, el ciclo de idempotencia 6.556 segundos y la ventana completa de validacion y auditoria 172.919 segundos;
- no se detectaron defectos funcionales, de integridad, tenancy, TLS, redaccion ni ciclo de vida en el alcance ejecutado.

Resultado: la Fase 5.4 queda `Validada` para el recorrido oficial `Production` expresamente autorizado. Esta evidencia no valida nuevas claves, otros tenants, emision, firma, recepcion, anulacion ni ejecucion permanente del worker.

## Evidencia de hosting y bloqueo de contexto

El arranque con `Enabled=false` fue validado: inicio, cancelacion, salida con codigo 0, cero conexiones, cero claims y cero procesos residuales. El intento temporal con `Enabled=true` iniciado desde Codex se bloqueo antes del primer ciclo al abrir Master mediante `Microsoft.Data.SqlClient`.

El proceso se ejecuto como `Proyectos\CodexSandboxOffline`. Este mismo limite de Schannel/contexto ya esta documentado para otros workers: una consola normal del usuario Windows puede conectar mientras el proceso aislado de Codex no adquiere la capacidad TLS requerida. No cambiar codigo, `Encrypt`, `TrustServerCertificate` ni certificados para compensar el sandbox.

Usar `docs/operations/templates/run-sri-worker-empty-poll-local-proye.example.ps1` desde una consola PowerShell normal. La plantilla rechaza el contexto de Codex, exige `Encrypt=true` y `TrustServerCertificate=false`, no imprime secretos y habilita el worker solo durante el proceso manual.

## Precondiciones

1. Rama y commit aprobados; working tree limpio.
2. Respaldo y ventana de cambio para cada base tenant objetivo.
3. `115_tenant_sri_document_queue.sql` ya instalado y verificado.
4. Feature `SRI_DOCUMENTS` e integracion `SRI` habilitadas solo en la empresa piloto.
5. `SqlServerAdmin` y `Security:EncryptionKey` provistos fuera del repositorio.
6. Red/TLS hacia los hosts oficiales autorizada; no se permite omitir validacion de certificado.

## Orden de despliegue

1. Mantener `SriWorker:Enabled=false`; iniciar el proceso y comprobar configuracion/health sin claims ni llamadas remotas.
2. Confirmar nuevamente que no existan filas elegibles, locks ni intentos nuevos.
3. Ejecutar desde PowerShell normal la plantilla de polling vacio durante al menos dos ciclos y detener con Ctrl+C.
4. Confirmar Master/tenant resolution, cero claims, cero intentos, cero locks y cero conexiones al SRI.
5. Conservar `Enabled=false` en toda configuracion persistida y guardar evidencia saneada.
6. Solicitar autorizacion separada antes de preparar un trabajo y efectuar cualquier round trip al ambiente oficial Test.

## Evidencia minima

- build y pruebas ejecutadas;
- version SQL y objetos instalados;
- valores efectivos no secretos de batch, concurrencia, lease, timeout e intentos;
- una sola consulta por lease vigente con dos instancias;
- recuperacion de lease vencido;
- intento, auditoria y estado final coherentes;
- XML autorizado con `SizeBytes`, SHA-256 e identidad unica;
- logs sin XML, RUC completo, clave completa, secretos ni tokens;
- resultado separado para Test y Production; una prueba Test no valida Production.

## Rollback operativo

Deshabilitar el worker detiene nuevos claims. No eliminar cola, intentos, auditoria ni XML. Script `117` es forward-only; cualquier correccion de esquema requiere un nuevo script versionado.

## Relacion con Fase 5.5

La descarga protegida lee exclusivamente `SriAuthorizedDocuments` mediante el API tenant y no levanta ni invoca `NuanSystem.SriWorker`. El script tenant `118` agrega proyecciones del monitor y auditoria de acceso reutilizando `AuditSriDocumentChanges`; el script Master `119` registra formulario, menu y operaciones. Desplegar Fase 5.5 no autoriza una nueva consulta al SRI ni alterar la fila de evidencia de Fase 5.4.
