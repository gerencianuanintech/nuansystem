# NuanSystem SRI Worker — Production readiness

## Propósito y estado

Runbook de preparación para convertir el piloto operativo de Iteración 6 en un despliegue productivo controlado.

**Estado:** alcance de desarrollo de Fase 7.4 cerrado y validado; producción
diferida y no autorizada.

**Decisión:** no apto para implementación ni activación hasta cerrar las decisiones D7 del
[blueprint de Iteración 7](../architecture/SRI-ITERATION-7-PRODUCTION-READINESS-BLUEPRINT.md).

Este documento no autoriza SQL, infraestructura, cuentas, SCM, certificados, procesos, configuración, llamadas SRI ni cambios documentales. La evidencia runtime anterior se consulta en
[SRI-WORKER-OPERATIONS.md](SRI-WORKER-OPERATIONS.md); no se repite.

El computador actual fue aceptado exclusivamente como host de desarrollo. Su
inventario y límites están en
[SRI-ITERATION-7-DEVELOPMENT-HOST-BASELINE.md](SRI-ITERATION-7-DEVELOPMENT-HOST-BASELINE.md).
No sustituye el host productivo, el dominio ni la gMSA.

El propietario decidió mantener el worker deshabilitado, sin instalación
permanente en el computador de desarrollo. Los secrets locales siguen fuera de
Git, la observabilidad se limita a logs/Event Log/Monitor, el soporte es en
horario laboral y cada tenant o llamada real al SRI requiere autorización
independiente. El primer canario futuro, si se autoriza, será solo DEMO.

## Principios operativos

- `SriWorker:Enabled=false` es el baseline de artefacto y despliegue.
- Un cambio instala; otro observa; otro autoriza procesamiento.
- DEMO es el único candidato inicial y no autoriza Remigio, Cañaris u otro tenant.
- Secrets, XML, claves de acceso, conexiones y tokens nunca se copian al ticket.
- Rollback conserva cola, intentos, XML, auditoría y evidencia.
- SQL es forward-only; restore no es rollback rutinario.
- Toda llamada real futura requiere autorización explícita independiente.

## Readiness board

| Gate | Owner | Estado actual | Evidencia para cerrar |
|---|---|---|---|
| Plataforma LTS | Desarrollo | **Validado** | .NET 10, runtime autenticado, build y pruebas aprobados |
| Artefacto/reversión | Desarrollo | **Validado** | Publicación framework-dependent y rollback de Fase 7.3 |
| Host/capacidad | Infraestructura | Bloqueado | Inventario, patching, sizing y egress |
| Identidad/ACL | Seguridad/Infra | Bloqueado | gMSA y permisos efectivos |
| Secrets | Seguridad | Bloqueado | Vault, rotación y recovery |
| TLS/certificados | DBA/Seguridad | Bloqueado | Inventario, handshake y rollback |
| Alertas | Operaciones | Bloqueado | Synthetic delivery/ack/resolution |
| Soporte | Propietario/Ops | Bloqueado | Cobertura y escalamiento firmados |
| Backup/restore | DBA | Bloqueado | Restore integral y RPO/RTO medidos |
| Retención/legal hold | Legal/Propietario | Bloqueado | Política aprobada |
| Canario DEMO | Propietario | Bloqueado | Change con ventana/límites/abort |

No convertir `Bloqueado` en `Validado` por evidencia documental o de Iteración 6.
El estado y los campos exigidos para cada decisión se registran en
[SRI-ITERATION-7-DECISION-REGISTER.md](SRI-ITERATION-7-DECISION-REGISTER.md).

## Inventario obligatorio sin secretos

Registrar:

- change/ticket y aprobadores;
- host, Windows Server, dominio, patch level y zona horaria;
- runtime .NET y modalidad self-contained/framework-dependent;
- servicio técnico/visible y gMSA;
- versión, commit, hash/firma y release anterior;
- rutas de release, config, logs y diagnóstico;
- vault/secret IDs opacos y owner, nunca valores;
- Master y tenant autorizado;
- ambiente SRI permitido;
- versiones schema requeridas;
- certificados/trust chains y fechas, sin claves privadas;
- plataforma de alertas, routing y on-call;
- backup sets, último restore y RPO/RTO medidos;
- capacidad CPU/RAM/disco y umbrales;
- ventana, rollback owner y criterios de aborto.

## Roles y segregación

| Rol | Autoriza/ejecuta | Restricción |
|---|---|---|
| Propietario | Alcance, tenant, ambiente, ventana y aceptación | No entrega secrets |
| Operaciones | SCM, observación, alertas y evidencia | No altera datos SRI |
| Infraestructura | Host, AD/gMSA, red, ACL y patching | No concede admin al servicio |
| Seguridad | Vault, trust, certificados y revisión sensible | No exporta claves privadas |
| DBA | Grants, backup/restore y certificados SQL | No concede `db_owner` |
| Desarrollo | Artefacto, manifest, diagnóstico y forward-fix | No opera producción sin change |
| Legal/Compliance | Retención, archivo, hold y eliminación | No delega plazo al equipo técnico |
| Soporte funcional | Clasifica cola/retry/DeadLetter | No usa SQL manual |

Las aprobaciones de quien implementa y quien valida deben ser independientes para secrets, restore y eliminación futura.

## Preflight de implementación

Detener si cualquiera falla:

1. D7-01 a D7-10 aprobadas.
2. Artefacto sobre .NET 10 LTS con último patch soportado.
3. Hash/firma/manifest y release de rollback verificados.
4. Host dedicado, parchado, sincronizado y con capacidad.
5. gMSA con deny interactive, ACL y grants mínimos.
6. Vault accesible bajo gMSA; rotación/recovery ensayados.
7. TLS estricto desde gMSA hacia Master, DEMO, vault y destinos aprobados.
8. Certificados con owner, renovación y rollback.
9. Egress allowlist y Event Source preparados.
10. Alertas synthetic entregadas y reconocidas.
11. Backup Master + DEMO cifrado, verificado y restaurado.
12. RPO/RTO medidos y aceptados.
13. Retención interina/legal hold aceptados.
14. Worker, feature/integración y procesamiento deshabilitados.
15. Sin otra instancia ni change competidor.

## Fase A — Preparar host sin instalar

Evidencia:

- OS/runtime soportados y patch ownership;
- 2 vCPU/4 GiB/10 GiB libres como baseline mínimo o sizing superior;
- `%ProgramFiles%\NuanSystem\SriWorker\releases\<version>` solo lectura/ejecución;
- `%ProgramData%\NuanSystem\SriWorker\config` lectura para gMSA y modificación solo de administradores;
- logs/diagnóstico modificables por gMSA;
- sin secretos en rutas de release/configuración no secreta;
- red limitada a destinos autorizados.

Resultado requerido: host listo, sin servicio ni proceso.

## Fase B — Instalar deshabilitado

Usar una automatización productiva derivada de las plantillas, revisada y aprobada; las plantillas existentes no son el instalador.

1. Copiar release inmutable y verificar hash.
2. Aplicar ACL y Event Source.
3. Registrar SCM `NuanSystem.SriWorker` con startup `Disabled`.
4. Asociar gMSA sin contraseña en scripts/tickets.
5. Configurar recovery actions, pero no iniciar.
6. Validar `BinaryPathName`, identidad, ACL y cero procesos.

Resultado requerido: instalado, deshabilitado, sin heartbeat, claims ni llamadas.

## Fase C — Observación de health/heartbeat

1. Cambiar temporalmente a startup manual.
2. Mantener `SriWorker:Enabled=false`.
3. Iniciar bajo gMSA.
4. Observar mínimo 24 horas.
5. Validar versión, heartbeat `Disabled`, TLS, vault, disco, logs, Event Log y API/WinForms.
6. Disparar alertas synthetic no documentales y comprobar delivery/ack/resolution.
7. Confirmar cero claims, intentos, auditorías documentales, cambios XML y llamadas SRI.
8. Detener y confirmar cero procesos.

No se repiten mutex, lifecycle, Designer o update/rollback de Iteración 6 salvo que la implementación los cambie.

## Fase D — Readiness de DEMO

Checklist:

- [ ] Solo `NuanSystem_DEMO` aparece en el change.
- [ ] Ambiente y acción SRI están escritos explícitamente.
- [ ] QueueId `10004` no se modifica ni usa como fixture.
- [ ] Remigio/Cañaris no se consultan ni incluyen.
- [ ] Baseline de queue/retry/DeadLetter/leases aceptado.
- [ ] Backup/restore Master + DEMO aprobado.
- [ ] Operador, DBA, Seguridad, soporte y rollback owner presentes.
- [ ] Canal Critical probado.
- [ ] Volumen máximo, batch, concurrencia y duración fijados.
- [ ] Documento/fixture futuro tiene autorización separada.
- [ ] Criterios de aborto y resultado esperado firmados.

## Fase E — Habilitación controlada futura

No ejecutar con este documento. Cuando exista autorización:

1. Capturar snapshot saneado.
2. Confirmar una sola instancia y allowlist DEMO.
3. Habilitar mediante configuración/change gestionado, sin cambiar defaults del artefacto.
4. Observar cada claim, intento, lease, resultado, heartbeat y alerta.
5. Detener al llegar al límite, a 60 minutos o al primer abort criterion.
6. Restaurar `Enabled=false`.
7. Confirmar cero proceso/locks anómalos y conservar evidencia.
8. Abrir observación reforzada de 24 horas.

## Fase F — Estabilización

Durante siete días:

- versión/heartbeat estables;
- cero Critical atribuibles al worker;
- queue age, retries y DeadLetter dentro de umbrales;
- CPU, RAM y disco dentro del sizing;
- backups exitosos;
- sin secretos/XML/claves completas en evidencia;
- soporte y alertas funcionando.

Un fallo reinicia la ventana después de corregir y reaprobar.

## Alta independiente de otro tenant

Crear un readiness board nuevo por tenant. Exigir:

- owner de negocio/datos;
- capability, integración y ambiente;
- schema/grants;
- backup/restore;
- trust/red;
- volumen/baseline;
- retención/legal hold;
- alert routing;
- ventana/límites/rollback;
- confirmación de que no agrega acciones SRI.

DEMO no prueba readiness de otro tenant.

## Monitoreo y alertas

### Señales mínimas

- heartbeat age y lifecycle;
- versión/instancia;
- último ciclo exitoso;
- queue depth/oldest pending;
- retry/DeadLetter;
- leases activos/vencidos;
- SQL/TLS/provider;
- espacio de logs;
- backup;
- expiración de certificados.

### Umbrales iniciales

Reutilizar como baseline los umbrales de Iteración 6: heartbeat 90/180 s, pending 10/30 min, retry 5/20, certificado 30/14 días y disco 20/10 %. Calibrarlos con soak/canario; no debilitarlos para silenciar alertas.

### Flujo de alerta

```text
Signal -> código estable -> deduplicación -> canal push
  -> acknowledge con owner -> diagnóstico -> resolve
  -> post-incident si fue Critical
```

API/WinForms son superficies de consulta, no sustituyen el canal push. El propietario debe seleccionar la plataforma antes de implementación.

## Backup y restore drill

1. Detener y mantener deshabilitado el worker.
2. Capturar backups coordinados de Master y DEMO con checksum.
3. Restaurar en red/entorno aislado y nombres no productivos.
4. Validar schema, conteos y relaciones sin exponer XML.
5. Recalcular hash/tamaño de una muestra autorizada.
6. Validar acceso del worker deshabilitado solo si el drill lo autoriza.
7. Medir desde incidente declarado hasta servicio ready.
8. Calcular pérdida máxima posible por LSN/time.
9. Registrar RTO/RPO observados.
10. Destruir el entorno restaurado según política.

No aceptar `RESTORE VERIFYONLY` como sustituto de restore integral.

## Rotación y recuperación de secrets

1. Crear nueva versión en vault.
2. Autorizar acceso a gMSA.
3. Iniciar deshabilitado y validar dependencias.
4. Activar nueva versión durante ventana.
5. Observar heartbeat/logs/alertas.
6. Revocar versión anterior después de aceptación.
7. Probar break-glass/restore semestral sin revelar valores.

Exposición en log, paquete o ticket: Critical, deshabilitar, rotar, preservar evidencia y revisar alcance.

## Renovación de certificados

1. Inventariar frontera/owner/consumidores/fecha.
2. Preparar certificado/cadena y rollback.
3. Aplicar por propietario autorizado.
4. Validar hostname, chain, revocation y trust bajo gMSA.
5. Iniciar deshabilitado y observar.
6. Retirar material anterior solo tras aceptación.

Nunca usar `TrustServerCertificate=true`, accept-all, `IgnoreSslErrors` ni exportar una clave privada.

## Retención y legal hold

Mientras no exista decisión legal:

- retención indefinida;
- cero archive/purge/delete;
- backup incluye XML y auditoría;
- legal hold se registra fuera del runtime mediante proceso aprobado.

Una futura operación de archivo/eliminación requiere política versionada, hold consultable, dry-run, doble aprobación, hash verificado, restore desde archivo y auditoría. No forma parte de este runbook.

## Criterios de aborto

- Plataforma/runtime fuera de soporte.
- Secret, XML, clave completa, token o conexión expuestos.
- TLS estricto no disponible.
- gMSA/ACL/grant distinto al aprobado.
- Tenant/ambiente fuera del change.
- Segunda instancia no autorizada.
- Claim duplicado, lease incoherente o integridad XML fallida.
- Backup/restore o alerta Critical sin owner.
- RPO/RTO excedido sin aceptación.
- Queue/retry/DeadLetter supera umbral.
- QueueId `10004`, Remigio o Cañaris aparecen en la ejecución.

## Rollback operativo

1. Establecer `Enabled=false` y detener nuevos claims.
2. Detener cooperativamente y registrar leases.
3. Confirmar cero procesos.
4. Volver a release/configuración no secreta anterior.
5. Recuperar secrets desde vault.
6. No revertir SQL ni editar estados.
7. Iniciar deshabilitado y validar versión/heartbeat/TLS/schema.
8. Mantener deshabilitado hasta nuevo change.
9. Conservar datos, logs, alertas y reporte.

## Evidencia de cierre

Registrar por gate:

```text
Gate:
Estado: Validated | Not validated | Not applicable | Blocked
Owner:
Fecha:
Comando/procedimiento:
Evidencia saneada:
Resultado:
Riesgo residual:
```

El cierre productivo necesita firma de Propietario, Operaciones, Infraestructura, Seguridad, DBA, Desarrollo y Legal/Compliance para su área.

## Estado de este documento

Al 2026-07-25:

- Discovery y planificación: **Validated** mediante inspección de repositorio y antecedentes.
- Cierre de desarrollo: **Validated**; worker deshabilitado, singleton y sin instalación permanente.
- Implementación productiva: **Not validated**; no ejecutada.
- Activación DEMO: **Not validated**; no autorizada.
- Otros tenants: **Excluded**; requieren aprobación independiente.
- SQL/SCM/certificados/procesos/SRI: **Not executed**.
