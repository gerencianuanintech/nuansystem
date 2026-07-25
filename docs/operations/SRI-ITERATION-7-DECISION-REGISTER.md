# Registro de decisiones productivas — Iteración 7

## Propósito

Este registro convierte las decisiones D7 del blueprint de production readiness
en gates verificables. No instala infraestructura, no habilita el worker y no
autoriza procesamiento SRI.

Fuentes:

- [Blueprint de Iteración 7](../architecture/SRI-ITERATION-7-PRODUCTION-READINESS-BLUEPRINT.md)
- [Runbook de production readiness](SRI-WORKER-PRODUCTION-READINESS.md)
- [Ejecución de migración .NET 10](../architecture/DOTNET-10-MIGRATION-EXECUTION.md)
- [Artefactos de release .NET 10](DOTNET-10-RELEASE-ARTIFACTS.md)

## Estados

- **Validado:** existe evidencia ejecutada suficiente para la decisión.
- **Aprobado pendiente de evidencia:** el propietario decidió, pero falta probar.
- **Bloqueado:** falta decisión, propietario o evidencia imprescindible.
- **No aplicable:** decisión descartada explícitamente con justificación.

Una propuesta o un valor de ejemplo no equivale a aprobación.

## Estado actual

| ID | Decisión | Estado | Evidencia o dato faltante |
|---|---|---|---|
| D7-01 | Runtime productivo .NET 10 LTS | **Validado** | SDK `10.0.302`, runtimes `10.0.10`, build, pruebas, runtime autenticado y publicaciones `win-x64` validadas. |
| D7-02 | Host Windows dedicado | **Bloqueado** | Nombre/FQDN, Windows Server y build, dominio, zona, patch level, CPU/RAM/disco, egress y responsables. |
| D7-03 | Identidad gMSA | **Bloqueado** | El host de desarrollo está en `WORKGROUP`; falta dominio, nombre de gMSA, owner, hosts autorizados, deny-interactive y matriz ACL/grants. |
| D7-04 | Modalidad de publicación | **Validado** | Framework-dependent `Release/win-x64`, cinco hosts separados, manifests, hashes y rollback `pilot1 -> pilot2 -> pilot1`. |
| D7-05 | Proveedor de secretos | **Bloqueado** | Producto/vault, identidad de acceso, IDs opacos, owner, rotación, recovery y break-glass. |
| D7-06 | Alertamiento push | **Bloqueado** | Plataforma, canal, destinatarios, severidades, deduplicación, acknowledge y synthetic test. |
| D7-07 | Cobertura de soporte | **Bloqueado** | Horario, on-call, niveles, SLA internos, escalamiento y responsables. |
| D7-08 | RPO/RTO | **Bloqueado** | Restore integral coordinado Master + DEMO y aceptación de valores medidos. |
| D7-09 | Retención/legal hold | **Bloqueado** | Dictamen legal, retención, archivo, legal hold y prohibiciones de eliminación. |
| D7-10 | Primer canario DEMO | **Bloqueado** | Ambiente, ventana, volumen, concurrencia, duración, operadores, rollback y criterios de aborto/éxito. |
| D7-11 | Alta independiente de tenants | **Pendiente** | Aprobación del modelo de checklist/change separado para cada tenant. |
| D7-12 | HA futura | **Pendiente** | Aceptación de singleton inicial y condición para estudiar HA. |

## Evidencia cerrada

### Host de desarrollo local

El propietario autorizó usar el computador actual únicamente para desarrollo.
El baseline saneado se encuentra en
[SRI-ITERATION-7-DEVELOPMENT-HOST-BASELINE.md](SRI-ITERATION-7-DEVELOPMENT-HOST-BASELINE.md).

La estación cumple capacidad para build, pruebas, publicación y preflight,
pero no cierra D7-02: utiliza Windows 11 Pro, está en `WORKGROUP` y no representa
el host Windows Server dedicado. Tampoco cierra D7-03 porque no puede acreditar
una gMSA productiva.

El ensayo temporal de `pilot1`, deshabilitado y sin llamadas SRI, está
documentado en
[SRI-ITERATION-7-DEVELOPMENT-WORKER-REHEARSAL.md](SRI-ITERATION-7-DEVELOPMENT-WORKER-REHEARSAL.md).
Valida el lifecycle local de desarrollo y su rollback, pero no cambia el estado
de D7-02, D7-03 ni D7-10.

### D7-01 — .NET 10 LTS

- 16 proyectos migrados a `net10.0` / `net10.0-windows`.
- SDK `10.0.302` fijado por `global.json`.
- Runtimes `10.0.10`.
- Build Release sin errores ni advertencias.
- 483 pruebas superadas, 5 diagnósticas omitidas y 0 fallidas.
- API y Monitor SRI comprobados en runtime controlado.
- Visual Studio Designer revisado por el propietario.

### D7-04 — Publicación framework-dependent

- API, SyncWorker, MasterBranchSyncWorker, SriWorker y WinForms publicados por
  separado.
- `Release`, `win-x64`, framework-dependent, sin trimming ni single-file.
- Manifests de release, inventario de dependencias y SHA-256 por archivo.
- Configuración local, secretos, certificados, logs y payloads ausentes.
- Workers publicados deshabilitados.
- `pilot1` y `pilot2` inmutables y rollback a `pilot1` verificado.

Esta evidencia selecciona la modalidad de publicación para el primer canario.
Cambiar a self-contained exige una nueva decisión D7-04 y repetir Fase 7.3.

## Datos requeridos para D7-02 y D7-03

### Host

```text
Hostname/FQDN:
Windows Server edición/build:
Dominio:
Zona horaria:
CPU:
RAM:
Disco libre para release/logs:
Owner de patching:
Ventana de mantenimiento:
Destinos egress permitidos:
```

### gMSA

```text
Nombre calificado:
Owner:
Host autorizado:
Logon as a service:
Deny interactive/RDP:
ACL release:
ACL configuración:
ACL logs/diagnóstico:
Grants Master:
Grants DEMO:
```

La cuenta local temporal usada en Iteración 6 no es una identidad productiva y
no puede reutilizarse.

## Datos requeridos para D7-05 a D7-07

### Vault

```text
Proveedor:
Owner:
Autenticación por gMSA:
IDs opacos de secretos:
Auditoría de lectura:
Rotación:
Recovery/escrow:
Break-glass:
```

### Alertas y soporte

```text
Plataforma push:
Canal Critical:
Canal Warning:
Destinatarios:
Horario de cobertura:
On-call:
SLA de acknowledge:
SLA de resolución:
Escalamiento:
```

## Gate D7-08 — restore integral

`RESTORE VERIFYONLY` y backups existentes no cierran D7-08. Se exige:

1. worker detenido y deshabilitado;
2. backups coordinados y con checksum de Master y DEMO;
3. restore en nombres y red aislados;
4. validación de schema, relaciones y conteos saneados;
5. muestra autorizada de tamaño/SHA sin revelar XML;
6. medición de pérdida máxima posible;
7. medición desde incidente declarado hasta servicio listo;
8. limpieza del entorno restaurado;
9. aceptación de RPO/RTO observados.

No se ejecutará este drill sin autorización que identifique servidor, bases,
rutas de backup, destino aislado y política de limpieza.

## Gate D7-09 — retención

Hasta contar con decisión legal:

- retención indefinida;
- cero purge, archive o delete;
- ningún job de limpieza;
- legal hold administrado fuera del runtime mediante proceso aprobado;
- backups incluyen XML autorizado y auditoría.

## Gate D7-10 — canario DEMO

El change futuro debe completar:

```text
Tenant: NuanSystem_DEMO
Ambiente SRI:
Acción SRI autorizada:
Inicio/fin de ventana:
Volumen máximo:
Batch:
Concurrencia:
Duración máxima:
Operador:
DBA:
Seguridad:
Soporte:
Rollback owner:
Criterios de aborto:
Criterios de éxito:
```

No se incluirán `NuanSystem_DEMO_REMIGIO`, `NuanSystem_DEMO_CANARIS` ni otro
tenant. `QueueId=10004` permanece como evidencia protegida y no será fixture.

## Orden de cierre

```text
D7-02 host
  + D7-03 gMSA
  + D7-05 vault
  -> preflight de identidad, ACL, red y TLS

D7-06 alertas
  + D7-07 soporte
  -> synthetic delivery/ack/resolution

D7-08 restore
  + D7-09 retención
  -> aceptación DBA/Legal/Propietario

todos los anteriores
  + D7-10 canario
  -> change de instalación deshabilitada
```

## Criterio de salida de Fase 7.4

Fase 7.4 termina cuando:

- D7-01 a D7-10 tienen owner y estado explícito;
- D7-02, D7-03 y D7-05 cuentan con valores productivos sin incluir secretos;
- alertas/soporte tienen destinatarios y SLA;
- restore integral acredita RPO/RTO;
- legal aprueba retención y hold;
- el change DEMO contiene ventana y límites;
- el worker continúa deshabilitado.

**Estado actual:** **NO-GO para instalación productiva**. D7-01 y D7-04 están
validados; D7-02, D7-03 y D7-05 a D7-10 permanecen bloqueados.
