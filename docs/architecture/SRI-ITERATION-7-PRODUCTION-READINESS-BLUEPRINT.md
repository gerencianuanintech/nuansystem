# Iteración 7 — Production readiness de NuanSystem.SriWorker

## Estado, autoridad y decisión de fase

- **Fecha del blueprint:** 2026-07-24.
- **Alcance:** Discovery, arquitectura y planificación productiva.
- **Estado actual:** **no apta para instalación productiva**.
- **Motivo:** faltan decisiones del propietario sobre host, identidad, proveedor de secretos, alertamiento, soporte, backup/restore, retención legal y canario. Ninguna de estas brechas autoriza cambios runtime.
- **Autoridad:** `ENGINEERING-CONSTITUTION` > `ENGINEERING-KERNEL` > catálogos/grafo > skills > implementación.

Este documento parte de la evidencia cerrada de Iteración 6 en
[SRI-ITERATION-6-OPERATIONS-BLUEPRINT.md](SRI-ITERATION-6-OPERATIONS-BLUEPRINT.md) y
[SRI-WORKER-OPERATIONS.md](../operations/SRI-WORKER-OPERATIONS.md). No repite ni reabre sus gates
runtime aprobados. La evidencia end-to-end histórica sigue en
[SRI-WORKER-DEPLOYMENT.md](SRI-WORKER-DEPLOYMENT.md) y no autoriza otra llamada al SRI.

## Discovery Record

**Outcome:** definir la arquitectura, decisiones, evidencia y gates que deben existir antes de implementar o autorizar el despliegue productivo gradual de `NuanSystem.SriWorker`.

**Work type:** integración/worker operativo; continuidad, seguridad, observabilidad, despliegue y gobierno documental.

**Domain:** consulta y almacenamiento de comprobantes autorizados SRI, independiente de SAP Business One y de Sync Master/Sucursal.

**Explicit domain decisions and exclusions:**

- Master gobierna empresas, integraciones, capacidades y configuración protegida.
- Cada tenant conserva su cola, intentos, XML inmutable y auditoría.
- Solo `NuanSystem.SriWorker` realiza la consulta remota aprobada.
- El piloto continúa limitado a consulta de autorización por clave de acceso.
- Firma, emisión, envío, anulación, scraping y certificado de firma están excluidos.
- No se crea un pipeline, heartbeat, cola ni monitor paralelo.
- No se elimina, archiva ni modifica XML en esta fase.
- No se habilita un tenant por inferencia a partir de la evidencia de otro.

**Affected layers:** documentación/operación en esta fase; para una futura implementación: hosting, configuración, seguridad, infraestructura, SQL grants, API/monitor, frontend operativo, backups y soporte.

**Risk:** bajo para el commit documental; alto para cualquier implementación o activación posterior por tratarse de datos tributarios, secretos, multiempresa, servicio Windows y sistema externo.

**Evidence inspected:**

- `src/Backend/NuanSystem.SriWorker/NuanSystem.SriWorker.csproj` — el worker usa `net10.0`.
- `src/Backend/NuanSystem.SriWorker/Program.cs` — Generic Host, Windows Service, configuración externa, Serilog, validación de opciones, TLS y shutdown existentes.
- `src/Backend/NuanSystem.SriWorker/appsettings.json` y `appsettings.Production.json` — `Enabled=false`, TLS estricto y endpoints oficiales seguros por defecto.
- `SriBackgroundWorker`, `SriHeartbeatWorker`, `SriWorkerRuntimeState` y `WorkerOperationalEvents` — lifecycle, mutex local, heartbeat, drain y eventos implementados.
- `Application/Features/Operations/WorkerHeartbeat*` y `Persistence/Repositories/Operations/WorkerHeartbeatRepository.cs` — health compartido y plano de control en Master.
- `docs/operations/templates/sri-worker/*` — instalación, start/stop, update, rollback y uninstall parametrizados; son ejemplos, no instalador productivo.
- `tests/NuanSystem.Application.Tests/Features/Operations` y `Features/SriDocuments` — contratos automatizados de worker, heartbeat, SQL, provider y monitor.
- Documentos de Iteraciones 5 y 6 — límites, evidencia saneada, operaciones y rollback ya validados.
- `docs/architecture/DOTNET-10-MIGRATION-EXECUTION.md` — migración, build, pruebas y runtime .NET 10 validados.
- `docs/operations/DOTNET-10-RELEASE-ARTIFACTS.md` — modalidad framework-dependent, manifests, hashes y rollback validados.

**Selected pattern:** promover el piloto validado mediante un plano de producción dedicado: host Windows soportado, identidad gMSA, proveedor de secretos sin bootstrap estático, artefactos versionados, heartbeat Master existente, activación por allowlist tenant y rollback no destructivo.

**Permitted reuse boundary:**

- Reutilizar Generic Host, `UseWindowsService`, heartbeat/health compartidos, contratos tenant, TLS estricto, logs, eventos, monitor y plantillas.
- Reutilizar la evidencia de Iteración 6 solo para los gates exactos que ejecutó.
- No reutilizar cuentas temporales, rutas de piloto, secretos, certificados, tenants ni aprobaciones como decisiones productivas.
- No reutilizar colas o workers SAP/Sync.

**Components to reuse:**

- `NuanSystem.SriWorker` — único host del procesamiento SRI.
- `dbo.WorkerHeartbeat` y `WorkerHealthEvaluator` — única superficie de health.
- Cola, intentos, auditoría y store XML tenant — fuentes de verdad existentes.
- API protegida y pestaña health del monitor — superficie operativa existente.
- Releases versionadas y configuración externa bajo `%ProgramData%` — patrón de despliegue.

**Alternatives rejected:**

- Producción permanente sobre `net9.0` sin plan de migración — su horizonte de soporte es insuficiente.
- `LocalSystem` o cuenta administrativa — privilegio excesivo.
- Cuenta local compartida o identidad humana — rotación, auditoría y aislamiento insuficientes.
- Secrets en variables globales, argumentos, release o `appsettings.Local.json` — exposición y recovery débiles.
- Alertamiento solo por consulta manual de WinForms — no entrega incidentes cuando nadie observa.
- Backup sin restore probado — no demuestra RPO/RTO.
- Purga por fecha sin legal hold y doble aprobación — riesgo legal y de pérdida irreversible.
- Habilitación simultánea de tenants — impide atribuir fallos y generaliza evidencia indebidamente.

**Gaps/new code:** no se autoriza código en esta fase. Una futura iteración deberá cubrir integración con el proveedor de secretos elegido, allowlist de despliegue si no queda cubierta por configuración existente y adaptador de alertamiento aprobado.

**Differences/constraints:** la cuenta `NuanSriWorkerSvc`, la instalación SCM y los certificados de Iteración 6 fueron temporales; los objetivos RPO/RTO no tienen restore integral medido; no hay canal push de alertas ni política legal de retención aprobada.

**Confidence:** alta para el inventario y los límites; media para sizing y topología hasta conocer infraestructura, AD, SQL, monitoreo y obligaciones legales reales.

**Validation required:** gates delta de plataforma LTS, host, identidad, vault, TLS, restore medido, alertamiento entregado, soporte aceptado, retención aprobada, soak deshabilitado y despliegue canario DEMO autorizado.

## Clasificación de riesgo

| Área | Riesgo | Razón | Tratamiento |
|---|---|---|---|
| Cambio documental actual | Bajo | No altera runtime ni datos | Revisión, enlaces, build y pruebas |
| Plataforma/host | Alto | Disponibilidad, capacidad, patching y superficie de ataque | .NET 10 LTS, hardening y patching |
| Identidad/secrets | Alto | Acceso a Master, tenants y configuración protegida | gMSA + vault + mínimo privilegio |
| XML/backup/restore | Alto | Evidencia tributaria y datos sensibles | Cifrado, restore probado, legal hold |
| Activación tenant | Alto | Trabajo remoto y cambios documentales persistentes | Allowlist, canario y abort criteria |
| Monitoreo/soporte | Alto | Falla silenciosa o reacción tardía | Alertas push, on-call y escalamiento |

## Alcance y exclusiones

### Incluye

1. Host objetivo y requisitos mínimos.
2. Identidad del servicio y privilegios mínimos.
3. Secrets, certificados, backup/restore y continuidad.
4. Monitoreo, alertas, responsables y soporte.
5. Retención, archivo, legal hold y condiciones de una eliminación futura.
6. Despliegue gradual, DEMO primero, incorporación independiente de tenants y rollback.
7. Quality gates productivos y evidencia requerida.

### Excluye

- Código, SQL, infraestructura, cuentas, servicios, certificados y configuración efectiva.
- Ejecución de API, WinForms o workers.
- Llamadas o pruebas contra SRI.
- Procesamiento o alteración de documentos, incluido QueueId `10004`.
- Inspección o cambio de Remigio y Cañaris.
- Retención o eliminación automática.
- Repetición de gates runtime cerrados en Iteración 6.

## Estado actual comprobado

| Capacidad | Estado comprobado | Límite |
|---|---|---|
| Host Windows Service | Implementado y validado temporalmente | No existe host productivo declarado |
| Identidad | Cuenta local dedicada temporal validada | No es identidad productiva definitiva |
| Lifecycle/rollback | Piloto `pilot1 -> pilot2 -> pilot1` validado | No demuestra pipeline productivo |
| Health/heartbeat | Implementado y visible por API/WinForms | Sin backend push de alertas |
| TLS | Estricto para SQL/SRI en el piloto | Certificados y trust productivos no inventariados |
| Secrets | Configuración externa y `AesSecretProtector` existentes | No hay proveedor productivo ni recovery probado |
| Backup/restore | Política propuesta, no restore integral | RPO 15 min/RTO 4 h no demostrados |
| Retención | Indefinida, sin purge | Falta plazo legal, archivo y legal hold |
| Multi-tenant | Contratos aislados y claims validados | Solo DEMO tiene evidencia E2E; no se generaliza |
| Plataforma | Proyecto `net10.0`, build/runtime validados | Falta owner de patching del host productivo |

## Arquitectura productiva propuesta

```text
Operaciones/Monitoreo
  -> API protegida + alerta push aprobada
      -> dbo.WorkerHeartbeat en Master

Host Windows dedicado y parchado
  -> servicio SCM NuanSystem.SriWorker
      -> identidad gMSA dedicada
      -> vault mediante identidad, sin secreto bootstrap
      -> configuración no secreta en ProgramData (read-only)
      -> logs/diagnóstico en volumen operativo (write-only para servicio)
      -> SQL Master/tenant por TLS estricto y grants EXECUTE mínimos
      -> HTTPS oficial SRI por trust store de Windows
      -> cola/attempt/XML/auditoría de cada tenant

Backups coordinados y cifrados
  -> repositorio fuera del host
  -> copia inmutable/offline
  -> restore periódico aislado y medido
```

### Host productivo y mínimo

Baseline propuesto, sujeto a benchmark y aprobación de Infraestructura:

- VM dedicada Windows Server x64 soportada por Microsoft y por el runtime LTS seleccionado; preferencia Windows Server 2022 o posterior.
- No compartir el proceso con SQL Server, SAP DI API ni una estación interactiva.
- 2 vCPU, 4 GiB RAM y 10 GiB libres dedicados a releases/logs/diagnóstico como mínimo inicial; los datos XML permanecen en SQL.
- Volumen operativo con alerta al 20 % y estado crítico al 10 % libre.
- Sin login interactivo para la identidad; administración solo por grupo autorizado.
- Sincronización horaria corporativa, EDR/antimalware, parchado mensual y reinicio controlado.
- Salida de red allowlist a DNS, CRL/OCSP, SQL autorizado, vault/monitor aprobado y hosts oficiales SRI.
- Runtime objetivo: **.NET 10 LTS con último patch soportado**. La migración está cerrada; el host debe asignar patch ownership.
- Modalidad seleccionada: framework-dependent `win-x64`, con inventario, manifests y hashes; cambiarla exige reabrir D7-04 y repetir Fase 7.3.

El sizing es un mínimo de entrada, no garantía. La aceptación exige medir CPU, memoria, disco y latencia durante soak y canario.

### Identidad definitiva del servicio

- Nombre SCM: `NuanSystem.SriWorker`.
- Nombre visible: `NuanSystem SRI Worker`.
- Identidad recomendada y objetivo productivo: gMSA dedicada `DOMINIO\gmsa-nuan-sri$` (nombre final lo define Seguridad).
- Si AD/gMSA no está disponible, la cuenta local dedicada requiere una excepción formal con rotación automática, escrow y prueba de recuperación; no es el default.
- Prohibidos `LocalSystem`, administradores, identidades humanas y cuentas compartidas.

Privilegios mínimos:

- `Log on as a service`; denegar login local/RDP y ejecución interactiva.
- Lectura/ejecución sobre release activa; ninguna escritura en binarios.
- Lectura de configuración no secreta; modificación solo de logs/diagnóstico.
- Lectura de secrets únicamente por identidad desde el proveedor.
- SQL Master/tenant mediante procedimientos/grants explícitos; sin `db_owner`, DDL o acceso a otras bases.
- Lectura del trust store; sin exportación ni administración de certificados.
- Event Log solo sobre el source precreado.
- Red únicamente a destinos autorizados.

## Matriz de decisiones

| ID | Decisión propuesta | Recomendación | Estado/propietario |
|---|---|---|---|
| D7-01 | Runtime productivo | Migrar y validar en .NET 10 LTS antes de producción | **Validado:** .NET 10, runtime autenticado y artefactos aprobados |
| D7-02 | Host | VM Windows Server dedicada, soportada y parchada | **Bloqueante:** Infraestructura |
| D7-03 | Identidad | gMSA dedicada; cuenta local solo por excepción | **Bloqueante:** Seguridad/Infra |
| D7-04 | Publicación | Framework-dependent `win-x64` administrado | **Validado:** manifests, hashes y rollback de Fase 7.3 |
| D7-05 | Secrets | Vault con autenticación por identidad y auditoría | **Bloqueante:** Seguridad |
| D7-06 | Alertamiento | Plataforma push corporativa + API/WinForms | **Bloqueante:** Operaciones |
| D7-07 | Soporte | Cobertura, on-call, severidades y SLA internos | **Bloqueante:** Propietario/Operaciones |
| D7-08 | RPO/RTO | 15 min / 4 h solo si restore medido lo acredita | **Bloqueante:** DBA/Propietario |
| D7-09 | Retención | Indefinida hasta dictamen legal; luego política versionada | **Bloqueante:** Legal/Propietario |
| D7-10 | Primer canario | Solo `NuanSystem_DEMO`, ventana y límites expresos | **Bloqueante:** Propietario |
| D7-11 | Otros tenants | Alta independiente por checklist y change record | Propuesto; aprobar modelo |
| D7-12 | HA | Singleton inicial; HA solo después de capacidad y prueba | Propuesto; no requerido para primer canario |

## Matriz de capas afectadas

| Capa | Estado de esta fase | Acción futura |
|---|---|---|
| Domain | Verificada sin cambios | Ninguna dependencia SRI/infra |
| Application | Verificada sin cambios | Solo si el vault/allowlist exige contrato |
| Persistence | Verificada sin cambios | Grants productivos, no nuevo acceso directo |
| API | Verificada sin cambios | Integrar alertas solo mediante contrato aprobado |
| Database | No aplicable a esta fase | Backup/restore y grants; SQL solo bajo autorización |
| Worker | Verificada sin cambios | Runtime LTS y secret provider en futura implementación |
| Frontend | Verificada sin cambios | Reutilizar monitor; no lógica operativa local |
| Security | Diseño | gMSA, vault, ACL, red y certificados |
| Operations | Diseño | host, monitoreo, soporte, backup, despliegue |
| Tests | Verificada sin cambios | Añadir gates delta cuando haya implementación |
| Documentation | Cambio | Blueprint y runbook de readiness |

## Modelo de amenazas operacional

| Amenaza | Impacto | Control propuesto | Evidencia exigida |
|---|---|---|---|
| Robo de secreto/bootstrap | Acceso multiempresa | Vault por identidad, sin secreto en disco/release | Audit log del vault y escaneo de artefacto |
| Cuenta comprometida | Claims, XML o configuración expuestos | gMSA, deny interactive, grants mínimos, EDR | ACL/grants efectivos bajo la identidad |
| MITM SQL/SRI | Lectura/alteración o respuesta falsa | TLS, hostname y cadena; sin bypass | Handshake desde identidad y trust inventariado |
| Tenant no autorizado | Procesamiento fuera de alcance | Capability + integración + allowlist/change record | Snapshot saneado y aprobación por tenant |
| Segundo proceso | Doble carga o diagnóstico confuso | Mutex local, heartbeat y lease tenant | Alerta y claim exclusivo |
| Artefacto manipulado | Ejecución de código no aprobado | Hash/firma/manifest y releases inmutables | Verificación predeploy |
| Backup expuesto | Fuga masiva de XML/secrets | Cifrado, ACL, repositorio separado/inmutable | Restore auditado sin exposición |
| Restore descoordinado | Master/tenant inconsistente | Runbook coordinado, LSN/time y worker detenido | Restore integral medido |
| Logs sensibles | Fuga tributaria | Redacción, ACL, retención y escaneo | Búsqueda negativa automatizada |
| Certificado vencido | Caída silenciosa | Inventario, alertas 30/14 días, rollback | Renovación ensayada |
| Alerta no entregada | Incidente no atendido | Canal push, deduplicación, synthetic test | Delivery/ack/resolution |
| Purga sin autorización | Pérdida legal irreversible | Legal hold, doble aprobación, dry-run, forward-only | Restore y auditoría de cada lote |

## Secretos y certificados

### Proveedor y almacenamiento de secretos

Objetivo:

1. Vault corporativo con autenticación mediante gMSA/workload identity.
2. Secrets separados por ambiente y función; ninguna credencial compartida entre API, worker y operadores.
3. Auditoría de lectura/rotación, versionado, fecha de expiración y owner.
4. Caché solo en memoria; no registrar valor, hash reversible ni presencia detallada por tenant.
5. Break-glass con doble control y evidencia.

Rotación:

- Ensayar con versión nueva y anterior durante una ventana controlada.
- Arrancar deshabilitado, validar Master/tenant/TLS/heartbeat, luego retirar la versión anterior.
- Periodicidad máxima propuesta: 90 días para secretos estáticos; rotación inmediata por exposición.
- gMSA rota según AD y no almacena contraseña en el runbook.

Recuperación:

- Backup/escrow del proveedor bajo control de Seguridad, no dentro de backup SQL o release.
- Restore aislado del vault/metadata y prueba semestral de acceso bajo identidad recuperada.
- Pérdida de `Security:EncryptionKey` es incidente crítico: no recrear ni sobrescribir; recuperar la versión correcta antes de operar.

### Certificados TLS

| Frontera | Propietario | Renovación | Rollback |
|---|---|---|---|
| SQL Server | DBA | Certificado con SAN; ventana y prueba Master/tenant | Certificado anterior válido y binding documentado |
| Trust del host worker | Seguridad/Infra | Distribución Root/Intermediate controlada | Revertir trust solo con cadena aprobada |
| HTTPS SRI | SRI + trust Windows | Patching de roots y monitoreo de handshake | No pinning local ni bypass |
| Vault/monitor | Seguridad/Infra | Según plataforma corporativa | Endpoint/cadena anterior aprobados |
| Firma electrónica | Fuera de alcance | No aplica | No aplica |

No se importa, cambia o elimina ningún certificado en esta fase.

## Backup, restore, RPO y RTO

Baseline propuesta:

- Master y cada tenant habilitado en el mismo change record.
- Full diario con `CHECKSUM`; diferencial cada 6 horas donde aplique; log cada 15 minutos en recovery Full.
- Backup cifrado, copia fuera del host y una copia inmutable/offline.
- `COPY_ONLY` predeploy sin romper la cadena.
- Verificación automática y restore integral trimestral en entorno aislado.
- Recalcular una muestra autorizada de SHA-256/tamaño sin exponer XML.
- Incluir manifest de release/configuración no secreta; secrets se recuperan desde su proveedor.

Objetivos:

- RPO: 15 minutos.
- RTO: 4 horas.

Son objetivos **no aceptados** hasta que un restore coordinado Master + DEMO mida ambos. Si el resultado no cumple, el propietario debe aumentar capacidad o aprobar otros objetivos antes de habilitar.

## Retención, archivo, legal hold y eliminación futura

Decisión interina segura: retención indefinida en tenant, inmutable y sin purge.

| Alternativa | Ventaja | Riesgo/condición |
|---|---|---|
| Indefinida en SQL | Acceso y restore simples | Crecimiento, coste y backups más largos |
| Plazo fijo en SQL | Predecible | Requiere plazo legal aprobado |
| Archivo externo inmutable | Reduce SQL | Debe conservar bytes, hash, metadata y lectura probada |
| Tiering SQL + archivo | Equilibra coste/acceso | Mayor complejidad de inventario y restore |
| Exportar y purgar | Reduce almacenamiento | No permitido sin legal hold, doble control y restore |

Antes de diseñar eliminación:

1. Dictamen legal vigente de Ecuador y contratos aplicables, con fuente y fecha.
2. Política versionada por tipo documental/tenant, sin defaults implícitos.
3. Registro de legal hold que prevalece sobre cualquier vencimiento.
4. Archivo inmutable con bytes originales, SHA-256, metadata y cadena de custodia.
5. Dry-run, conteo, muestra, doble aprobación segregada y ventana.
6. Purga por procedimiento/API autorizados, acotada, idempotente y auditada; nunca SQL manual.
7. Restore probado desde archivo antes de la primera purga.
8. Evidencia de quién, cuándo, por qué y qué política se aplicó.

La duración legal no se fija en este blueprint; requiere aprobación jurídica externa.

## Despliegue gradual

### Etapa 0 — Preconditions

- D7-01 a D7-10 aprobadas.
- Artefacto LTS, hash/manifest y pruebas aprobados.
- Host/gMSA/vault/TLS/grants/alertas/backup listos.
- Restore integral dentro de RPO/RTO.
- Worker y tenants deshabilitados.

### Etapa 1 — Instalado y deshabilitado

- Instalar release versionada con startup SCM deshabilitado.
- Validar ACL, identidad, Event Source, configuración no secreta y cero procesos.
- No ejecutar SQL ni modificar capacidades como parte de la instalación binaria.

### Etapa 2 — Health y heartbeat en observación

- Inicio manual con `SriWorker:Enabled=false`.
- Observar mínimo 24 horas: heartbeat `Disabled`, versión exacta, TLS, vault, disco, logs y alertas synthetic.
- Exigir cero claims, intentos, cambios documentales y llamadas SRI.

### Etapa 3 — Habilitación controlada de DEMO

- Change separado que nombre `NuanSystem_DEMO`, ambiente, ventana, volumen máximo y criterios de aborto.
- Comenzar con una instancia, batch/concurrencia baseline y allowlist solo DEMO.
- Ventana inicial de 60 minutos y observación reforzada de 24 horas; ampliar solo tras aceptación.
- Detener al primer Critical o desviación de tenant, integridad, TLS, secret, lease o alerta.

### Etapa 4 — Estabilización

- Siete días sin incidentes críticos atribuibles al worker.
- Tendencias de queue age, retry, DeadLetter, CPU/memoria/disco y tiempos de proveedor dentro de umbrales.
- Post-implementation review y aceptación de Operaciones, Seguridad, DBA y propietario.

### Etapa 5 — Otros tenants

- Un tenant por change record.
- No heredar aprobación, secrets, certificado, backup, volumen o ventana desde DEMO.
- Soak y rollback independientes; mantener posibilidad de deshabilitar solo el tenant afectado.

## Criterios para habilitar primero NuanSystem_DEMO

Todos obligatorios:

- Runtime LTS y host productivo aprobados.
- Feature/integración/ambiente revisados sin modificar QueueId `10004`.
- Backup y restore de Master + DEMO dentro de objetivos.
- gMSA, vault, ACL y grants validados bajo identidad real.
- TLS estricto y certificados con más de 30 días o excepción aprobada.
- Heartbeat, health y alerta push entregada/acknowledged.
- Cola, leases, retry y DeadLetter en baseline aceptado.
- Sin otra instancia activa ni otro tenant en allowlist.
- Ventana, operador, DBA, Seguridad, soporte y rollback owner presentes.
- Autorización expresa para cualquier documento o llamada real futura.
- Retención interina y tratamiento de datos aceptados.

## Condiciones independientes para incorporar otros tenants

Por cada tenant:

1. Propietario de negocio y responsable de datos.
2. Capability/integración/ambiente aprobados.
3. Contratos/schema/grants verificados.
4. Backup, restore y capacidad medidos.
5. Certificados/trust y rutas de red validados.
6. Volumen, antigüedad de cola, retries y DeadLetter inventariados.
7. Retención/legal hold y permisos de descarga aceptados.
8. Alertas con routing y soporte del tenant.
9. Change record, ventana, límites, abort criteria y rollback.
10. Evidencia de que habilitarlo no amplía acciones SRI ni dependencia SAP.

## Rollback

Orden:

1. Deshabilitar tenant/procesamiento y detener nuevos claims.
2. Detener servicio de forma cooperativa; registrar leases sin editarlos.
3. Confirmar cero procesos.
4. Volver a release/configuración no secreta anterior y recuperar secrets desde vault.
5. No revertir SQL destructivamente; solo forward-fix autorizado.
6. Iniciar deshabilitado y validar versión, heartbeat, TLS y schema.
7. Conservar cola, intentos, XML, auditoría, logs y alertas.
8. Rehabilitar únicamente mediante nuevo change aprobado.

Restore SQL no es rollback rutinario; solo procede ante daño confirmado y aprobación formal.

## Quality gates delta de Iteración 7

La evidencia de Iteración 6 se referencia como antecedente; no se repite salvo que cambie su contrato o entorno.

### Seguridad

- Runtime/OS soportados y parchados.
- gMSA real, deny interactive, ACL y grants mínimos.
- Vault integrado, rotación y recovery probados.
- Escaneo negativo de secrets/XML/claves en release, logs y evidencia.
- Egress allowlist y TLS bajo la identidad real.

### SQL y datos

- Backup cifrado y restore coordinado medido.
- RPO/RTO aceptados.
- Grants por procedimiento, sin DDL/db_owner.
- Integridad de muestra XML post-restore.
- Política de retención/legal hold aprobada antes de cualquier archive/purge.

### Servicio

- Artefacto LTS firmado/hasheado y versión exacta.
- Instalación deshabilitada, soak de 24 h y patch ownership.
- Recovery actions y capacidad/espacio monitoreados.
- Rollback productivo ensayado sin tocar datos.

### API/frontend

- Health protegido disponible desde red operativa.
- Monitor muestra versión/tenant/estado sin datos sensibles.
- Roles 401/403/200 bajo identidad/usuario productivos.
- No se agrega ejecución remota del worker desde WinForms.

### Operación

- Alertas synthetic llegan, se deduplican, se reconocen y resuelven.
- On-call/escalamiento y runbook firmados.
- Restore y canario DEMO con criterios de aborto.
- Cada nuevo tenant completa su checklist independiente.

## Evidencia necesaria

- Aprobaciones D7 con fecha/owner.
- Inventario de host, OS/runtime/patch y modalidad de publicación.
- Manifest, hash/firma, SBOM o inventario de dependencias disponible.
- ACL/grants/egress efectivos saneados.
- Audit de vault, rotación y recovery sin valores.
- Inventario de certificados y prueba de renovación/rollback.
- Backup sets y restore report con RPO/RTO observados.
- Delivery/ack/resolution de alertas synthetic.
- Soak deshabilitado y canario con métricas.
- Revisión de logs/artefactos sensibles.
- Cierre por Operaciones, Seguridad, DBA, Desarrollo y propietario.

## Riesgos pendientes

- Ownership de patching y mantenimiento de .NET 10 productivo aún no asignado.
- Topología y capacidad productivas desconocidas.
- Dependencia de `Security:EncryptionKey` sin recovery productivo demostrado.
- Ausencia de canal push y cobertura acordada.
- RPO/RTO no medidos.
- Falta de plazo legal y legal hold implementable.
- Evidencia E2E limitada a DEMO y a una acción SRI.
- HA no diseñada ni requerida para el primer canario.

## Decisiones bloqueantes del propietario

1. .NET 10 LTS: **cerrado** mediante Fases 7.1 a 7.3.
2. Confirmar host Windows dedicado, dominio/AD y modalidad de publicación.
3. Aprobar gMSA y nombre/owner definitivos, o una excepción documentada.
4. Elegir vault/proveedor de secretos, rotación, escrow y break-glass.
5. Elegir plataforma de alertas push, destinatarios y cobertura de soporte.
6. Aceptar RPO/RTO o financiar capacidad tras restore medido.
7. Obtener dictamen legal de retención, archivo y legal hold.
8. Aprobar DEMO como primer canario, ambiente, ventana, volumen y criterios de éxito.
9. Aprobar el modelo de incorporación independiente de tenants.
10. Definir si singleton es suficiente y cuándo se estudiará HA.

## Criterio de aptitud para implementación

Iteración 7 puede declararse **apta para implementación** únicamente cuando:

- D7-01 a D7-10 tengan owner y aprobación registrada;
- no queden decisiones de seguridad, legalidad, restore o soporte implícitas;
- el plan de implementación esté separado de este blueprint y no incluya habilitación automática;
- los gates delta tengan ambiente, comando/procedimiento, evidencia y responsable definidos;
- la futura implementación mantenga `Enabled=false` hasta un change de activación independiente.

El estado detallado y los datos faltantes se mantienen en
[SRI-ITERATION-7-DECISION-REGISTER.md](../operations/SRI-ITERATION-7-DECISION-REGISTER.md).

**Evaluación al 2026-07-25:** **NO APTA PARA INSTALACIÓN PRODUCTIVA**. D7-01 y
D7-04 están validadas; D7-02, D7-03 y D7-05 a D7-10 continúan bloqueantes.
