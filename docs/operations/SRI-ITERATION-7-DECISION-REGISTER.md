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
| D7-02 | Host Windows dedicado | **Diferido / bloqueado para producción** | El propietario cerró la etapa actual como desarrollo. El host productivo todavía no está definido. |
| D7-03 | Identidad del servicio | **Diferido / bloqueado para producción** | No existe Active Directory, por lo que gMSA no es viable actualmente. Una identidad productiva alternativa exige decisión y excepción futuras. |
| D7-04 | Modalidad de publicación | **Validado** | Framework-dependent `Release/win-x64`, cinco hosts separados, manifests, hashes y rollback `pilot1 -> pilot2 -> pilot1`. |
| D7-05 | Proveedor de secretos | **Validado para desarrollo / bloqueado para producción** | `appsettings.Local.json` ignorado por Git y TLS estricto durante desarrollo. Vault, rotación y recuperación productivos quedan diferidos. |
| D7-06 | Alertamiento push | **Validado para desarrollo / bloqueado para producción** | Logs, Windows Event Log y Monitor WinForms aprobados para desarrollo; no existe canal push productivo. |
| D7-07 | Cobertura de soporte | **Validado para desarrollo / bloqueado para producción** | Soporte en horario laboral, sin cobertura 24x7. Un SLA productivo queda diferido. |
| D7-08 | RPO/RTO | **Diferido / bloqueado para producción** | El propietario aplazó valores y restore integral hasta disponer del host productivo. |
| D7-09 | Retención/legal hold | **Validado para el alcance actual** | Retención indefinida de XML y auditorías; cero eliminación automática o manual. |
| D7-10 | Primer canario DEMO | **Alcance aprobado / activación bloqueada** | Solo `NuanSystem_DEMO`; Remigio y Cañaris excluidos. Worker deshabilitado y cada llamada real exige autorización con ambiente, documento, ventana y límites. |
| D7-11 | Alta independiente de tenants | **Validado** | Cada tenant requiere aprobación, checklist y change independientes; no existe habilitación automática. |
| D7-12 | HA futura | **Validado para desarrollo** | Una sola instancia; HA queda diferida hasta una etapa productiva futura. |

## Decisiones de cierre de desarrollo

El propietario aprobó:

1. cerrar Iteración 7 como **desarrollo validado**;
2. no declarar ni inferir un host productivo;
3. registrar que el entorno actual no dispone de Active Directory;
4. mantener `NuanSystem.SriWorker` deshabilitado;
5. conservar secrets de desarrollo únicamente en configuración local ignorada
   por Git, con TLS estricto;
6. usar logs, Event Log y Monitor WinForms como observabilidad de desarrollo;
7. limitar el soporte a horario laboral, sin 24x7;
8. aplazar RPO/RTO y restore integral hasta disponer de infraestructura
   productiva;
9. retener indefinidamente XML y auditorías, sin eliminación;
10. operar una sola instancia;
11. no instalar permanentemente el worker en este computador;
12. exigir autorización independiente para cada tenant;
13. exigir autorización explícita para toda llamada real al SRI;
14. reservar el futuro canario únicamente para `NuanSystem_DEMO`.

Estas decisiones cierran el alcance de desarrollo, pero no convierten un gate
productivo bloqueado en validado.

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

El cierre productivo futuro exige:

- D7-01 a D7-10 tienen owner y estado explícito;
- D7-02, D7-03 y D7-05 cuentan con valores productivos sin incluir secretos;
- alertas/soporte tienen destinatarios y SLA;
- restore integral acredita RPO/RTO;
- legal aprueba retención y hold;
- el change DEMO contiene ventana y límites;
- el worker continúa deshabilitado.

**Estado actual:** **NO-GO para instalación productiva**. D7-01 y D7-04 están
validados; los valores de desarrollo D7-05 a D7-07 y D7-09 no sustituyen sus
equivalentes productivos. D7-02, D7-03, D7-05 a D7-08 y la activación D7-10
permanecen bloqueados.

**Cierre de desarrollo:** **VALIDADO**. El worker permanece deshabilitado, no
hay instalación permanente y cualquier nueva ejecución requiere autorización
independiente.
