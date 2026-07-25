# Ensayo del SRI Worker en host de desarrollo — Iteración 7

## Propósito

Registrar el ensayo temporal y reversible de `pilot1` en el equipo de
desarrollo autorizado. Esta evidencia no aprueba un host productivo, una
identidad gMSA, un canario DEMO ni una llamada al SRI.

## Alcance autorizado

- release `7.1.0-dotnet10-pilot1+9275f7c2`;
- cuenta local, ACL, Event Source, ProgramData y servicio SCM temporales;
- `SriWorker:Enabled=false`;
- configuración SQL con `Encrypt=true` y
  `TrustServerCertificate=false`;
- heartbeat con identidad exclusiva del ensayo;
- cero claims, leases, reintentos, procesamiento y llamadas SRI;
- eliminación exacta del heartbeat temporal;
- rollback material completo.

La API que ya estaba activa en el equipo quedó fuera del alcance y no fue
detenida ni reconfigurada.

## Discovery Record

**Outcome:** smoke test local sin SRI.

**Work type:** despliegue temporal de worker deshabilitado.

**Domain:** operación técnica SRI.

**Explicit exclusions:** API, Monitor WinForms, Designer, actualización a
`pilot2`, canario, proveedor SRI, Remigio y Cañaris.

**Risk:** alto por recursos SCM y acceso a Master; mitigado con identidad
única, worker deshabilitado, TLS estricto y limpieza en `finally`.

**Selected pattern:** instalación temporal validada en Iteración 6, reducida al
smoke test sin SRI definido en `SRI-WORKER-OPERATIONS.md`.

**Permitted reuse boundary:** lifecycle Windows Service, cuenta mínima, ACL,
configuración externa, heartbeat y Event Log. No se reutilizó procesamiento
documental ni activación del proveedor.

## Preflight

| Gate | Resultado |
|---|---|
| Rama y working tree | Correctos y limpios |
| Verificación de release | 645 archivos, 135 dependencias y manifiesto SHA-256 aprobado |
| Secretos en release | 0 |
| Workers habilitados en release | 0 |
| Configuración local requerida | Presente, sin imprimir valores |
| TLS local | `Encrypt=true`, `TrustServerCertificate=false` |
| Servicio/cuenta/ProgramData/Event Source previos | Ausentes |
| Procesos SRI Worker previos | 0 |
| API existente | 1; preservada y fuera del alcance |

## Ejecución validada

RunId: `ITER7DEV-20260725-B`.

| Evidencia | Resultado |
|---|---|
| Cuenta local temporal | Creada con clave aleatoria sólo en memoria |
| `Log on as a service` | Concedido temporalmente |
| ACL release/config/logs | Aplicadas con mínimo acceso requerido |
| Servicio temporal | Iniciado y detenido correctamente |
| Lifecycle | `Disabled` |
| `SriWorker:Enabled` | `false` |
| Versión heartbeat | `7.1.0-dotnet10-pilot1+9275f7c2` |
| Identidad lógica | Única; evidencia conservada como hash saneado |
| Empresas habilitadas leídas | 1 |
| Pending / Retry / DeadLetter | `0 / 0 / 0` |
| Leases activos / vencidos | `0 / 0` |
| TLS efectivo | `true / false` |
| Logs con secretos/XML | 0 coincidencias |
| Evento `SRI_WORKER_DISABLED` | Presente |
| Evento `SRI_WORKER_STOPPED` | Presente |
| Llamadas al SRI | 0 |

## Reconciliación del Event Log

El resultado automático inicial marcó únicamente
`DISABLED_EVENT_PRESENT=false`. El evento sí existía: Windows conservó
`SRI_WORKER_DISABLED` en las propiedades crudas del registro, pero la propiedad
formateada `Message` quedó indisponible después de retirar el Event Source.

Una consulta posterior de sólo lectura confirmó:

- proveedor temporal correcto;
- un evento `SRI_WORKER_DISABLED`;
- un evento `SRI_WORKER_STOPPED`;
- cero patrones sensibles.

No se repitió el runtime. La herramienta externa quedó corregida para aceptar
el payload crudo cuando Windows no pueda formatear `Message`.

## Limpieza y baseline final

| Recurso | Estado final |
|---|---|
| Heartbeat del RunId | Eliminado exactamente 1; no residual |
| Servicio temporal | Ausente |
| Cuenta temporal | Ausente |
| Derecho de servicio | Retirado con la cuenta |
| Event Source temporal | Ausente |
| ProgramData temporal | Ausente |
| ACL temporal sobre release | Retirada |
| Procesos `NuanSystem.SriWorker` | 0 |
| API preexistente | Conservada |
| Git | Limpio antes de registrar esta evidencia |

`QueueId=10004` no fue usada como fixture ni modificada. Remigio y Cañaris
quedaron fuera del alcance. El worker permaneció deshabilitado durante toda la
corrida, por lo que no ejecutó claims ni llamó al proveedor SRI.

## Quality gates

| Gate | Estado |
|---|---|
| Artefacto pilot1 exacto | **Validado** |
| Instalación/lifecycle temporal | **Validado** |
| Heartbeat y versión exacta | **Validado** |
| TLS estricto | **Validado** |
| Cero procesamiento/SRI | **Validado** |
| Logs y Event Log saneados | **Validado** |
| Limpieza material y heartbeat | **Validado** |
| Host productivo D7-02 | **Bloqueado** |
| gMSA D7-03 | **Bloqueado** |
| Canario D7-10 | **Bloqueado** |

## Conclusión

**Validado como ensayo de desarrollo deshabilitado y reversible.**

Esta evidencia confirma que el artefacto .NET 10 `pilot1` puede instalarse,
arrancar, reportar su versión y limpiarse correctamente en `PROYECTOS`. No
convierte el computador en host productivo ni autoriza habilitar el worker.
