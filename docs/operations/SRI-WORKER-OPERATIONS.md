# Operacion de NuanSystem SRI Worker

## Estado

**Runbook propuesto para Iteracion 6; no ejecutado ni aprobado para produccion.**

Este documento describe como debera operarse `NuanSystem.SriWorker` una vez aprobadas e implementadas las decisiones de [SRI-ITERATION-6-OPERATIONS-BLUEPRINT.md](../architecture/SRI-ITERATION-6-OPERATIONS-BLUEPRINT.md). No autoriza instalar el servicio, ejecutar SQL, habilitar tenants, consultar al SRI, alterar XML ni modificar colas.

La evidencia historica de Fases 5.3/5.4 se conserva en [SRI-WORKER-DEPLOYMENT.md](../architecture/SRI-WORKER-DEPLOYMENT.md). No repetir ese recorrido sin nueva autorizacion.

## Reglas permanentes

- Mantener `Encrypt=true` y `TrustServerCertificate=false` en QA/produccion.
- No desactivar validacion TLS ni aceptar certificados arbitrarios.
- Mantener `SriWorker:Enabled=false` en artefactos y configuracion persistida por defecto.
- No imprimir XML, claves de acceso completas, RUC, credenciales, tokens, cadenas de conexion o claves de cifrado.
- No ejecutar emision, firma, envio, anulacion ni scraping; el piloto solo consulta autorizaciones previamente aprobadas.
- No borrar cola, intentos, documentos, auditorias ni DeadLetter para corregir una incidencia.
- No editar estados, leases o payloads con SQL manual. Usar contratos versionados y acciones autorizadas.
- No usar `NuanSystem.SyncWorker`, `NuanSystem.MasterBranchSyncWorker` ni sus colas para SRI.
- WinForms consulta la API; nunca SQL o SRI directamente.

## Roles operativos propuestos

| Rol | Responsabilidad | No debe hacer |
|---|---|---|
| Propietario | Aprueba alcance, tenants, ambiente y activacion | Compartir secretos o ampliar el piloto informalmente. |
| Operaciones | Instala, inicia/detiene, monitorea y conserva evidencia | Cambiar datos SRI o SQL sin runbook. |
| DBA | Backups/restores, permisos SQL, scripts y certificado SQL | Dar `db_owner` permanente al worker. |
| Seguridad | Cuenta, secrets, ACL, certificados y revision de logs | Exportar claves privadas o habilitar bypass TLS. |
| Soporte funcional | Clasifica queue/retry/DeadLetter y coordina reproceso | Reprocesar masivamente o alterar XML. |
| Desarrollo | Entrega artefactos, diagnostica defectos y forward-fixes | Operar produccion sin cambio aprobado. |

La matriz RACI y contactos reales estan pendientes.

## Inventario por ambiente

Antes de cualquier despliegue registrar, sin secretos:

- host, SO y dominio;
- nombre tecnico/visible del servicio;
- cuenta del servicio y propietario;
- version/commit/hash del artefacto;
- directorio de release, logs y diagnosticos;
- Master y codigos de tenants autorizados;
- ambiente SRI permitido;
- versiones Master/tenant requeridas;
- proveedor de secretos;
- certificados/cadenas requeridos y responsables;
- ventana, operador, DBA, Seguridad y rollback owner;
- RPO/RTO, ubicacion del backup y ultimo restore test.

## Preflight de despliegue

Estado requerido antes de continuar:

1. Commit y artefacto firmados/aprobados; working tree no es fuente de despliegue.
2. Build Release y pruebas aprobados.
3. Decisiones bloqueantes del blueprint aprobadas.
4. Backups Master/tenants con `CHECKSUM`; `COPY_ONLY` para la captura predeploy manual.
5. Restore test reciente y RPO/RTO aceptados.
6. Scripts pendientes revisados, autorizados, forward-safe e idempotentes.
7. Cuenta sin privilegios administrativos, con `Log on as a service` y ACL minimas.
8. Secrets disponibles para esa identidad sin almacenarlos en el paquete.
9. SQL con certificado confiable; conexiones efectivas usan TLS estricto.
10. Red saliente limitada a SQL, servicios de confianza/certificados y hosts oficiales SRI aprobados.
11. Directorios de logs/diagnosticos con espacio, ACL y retencion.
12. Servicio inexistente o detenido; no hay otro proceso/instancia no autorizada.
13. Cola, locks, leases, retries y DeadLetter inventariados con consultas saneadas.
14. Worker deshabilitado y sin autorizacion de llamada remota durante instalacion/smoke test.

Si un gate falla, detener el cambio. No relajar TLS, secretos, permisos o backup para avanzar.

## Instalacion inicial propuesta

Este procedimiento es un contrato; los comandos de automatizacion se crearan y revisaran en la fase de implementacion.

1. Copiar el artefacto verificado a un directorio de release versionado.
2. Aplicar ACL: lectura/ejecucion a la cuenta; escritura solo en logs/diagnosticos.
3. Registrar el servicio con nombre tecnico `NuanSystem.SriWorker`, nombre visible `NuanSystem SRI Worker`, inicio automatico retrasado y cuenta aprobada.
4. Configurar failure actions y timeout segun valores aprobados.
5. Inyectar configuracion no secreta y referencias al proveedor de secretos.
6. Confirmar `Enabled=false`, batch/concurrencia/lease/timeout/attempts y `WorkerInstance` unico.
7. Iniciar sin procesamiento remoto.
8. Verificar proceso, version, heartbeat `Disabled`, logs saneados y conectividad necesaria.
9. Detener y reiniciar para validar lifecycle sin claims.
10. Conservar manifest, hashes, operador, tiempos y evidencia.

No crear variables de entorno globales con secretos si el proveedor aprobado permite un scope mas limitado. No registrar comandos que contengan secretos en historial o transcript.

## Actualizacion

1. Congelar el alcance y capturar snapshot operativo.
2. Ejecutar preflight y confirmar rollback package.
3. Mantener `Enabled=false` o deshabilitar antes de detener.
4. Esperar drain controlado; inventariar leases restantes.
5. Detener servicio y confirmar cero procesos.
6. Instalar nueva release sin borrar la anterior.
7. Aplicar configuracion no secreta y SQL autorizado.
8. Iniciar deshabilitado; validar version, heartbeat y schema.
9. Ejecutar smoke test sin SRI.
10. Habilitar solo mediante cambio separado que nombre tenants/ambiente.
11. Observar la ventana aprobada y cerrar evidencia.

## Desinstalacion

1. Obtener aprobacion y confirmar que no existe reemplazo dependiente de la misma identidad/secrets.
2. Establecer `Enabled=false`, drenar, detener y confirmar cero procesos.
3. Inventariar leases, colas, alertas y evidencia; no eliminar datos SQL.
4. Exportar manifest de version/configuracion no secreta y conservar logs segun politica.
5. Eliminar el registro SCM y luego los binarios autorizados; no borrar backups ni release de rollback antes del cierre.
6. Seguridad/DBA retiran cuenta, grants, secrets y ACL solo tras comprobar consumidores compartidos.
7. Registrar estado final, restaurabilidad y cualquier componente deliberadamente conservado.

## Smoke test sin SRI

Debe demostrar:

- servicio arranca y se detiene como la cuenta definitiva;
- heartbeat transita `Unknown -> Disabled` o `Healthy` segun configuracion;
- Master y tenants autorizados resuelven con TLS estricto;
- versiones SQL requeridas existen;
- conteos de cola/locks pueden leerse sin XML ni claves completas;
- con `Enabled=false` hay cero claims, intentos, auditorias documentales y llamadas proveedor;
- logs y eventos no contienen secretos o XML;
- no quedan procesos tras detener.

Un smoke test sin SRI no valida proveedor ni procesamiento real.

## Activacion controlada

Solo con autorizacion expresa que indique tenant, ambiente, ventana, documento/fixture permitido y limites:

1. Confirmar snapshot y ausencia de instancia no autorizada.
2. Confirmar health, backups, alertas y responsables.
3. Aplicar configuracion temporal o gestionada de activacion sin cambiar defaults del artefacto.
4. Iniciar una instancia inicial con lote/concurrencia aprobados.
5. Observar claim, intento, resultado, auditoria, lease y heartbeat.
6. Detener al vencer la ventana o al primer criterio de aborto.
7. Validar idempotencia solo si esta incluida en la autorizacion.
8. Restaurar `Enabled=false` y confirmar cero procesos/locks anormales.

Ningun antecedente de Iteracion 5 autoriza una nueva llamada.

## Operacion diaria

### Inicio de turno

- Revisar heartbeat, version y estado del servicio.
- Revisar ultimo ciclo/ultimo proveedor exitoso.
- Revisar profundidad/edad por ambiente, RetryScheduled, DeadLetter y locks.
- Revisar eventos Warning/Error/Critical abiertos.
- Revisar backup, espacio y vencimiento de certificados.
- Confirmar que tenants/ambientes habilitados coinciden con el cambio vigente.

### Cierre de turno

- Registrar incidentes, retries manuales, DeadLetter y cambios.
- Confirmar alertas reconocidas/resueltas y responsable siguiente.
- No copiar XML o claves completas al ticket.

## Diagnostico seguro

### Servicio sin heartbeat

1. Consultar SCM y proceso; no iniciar una segunda instancia a ciegas.
2. Revisar ultimo evento/log seguro, espacio y permisos.
3. Validar cuenta, secret provider y TLS SQL.
4. Si el proceso murio, inventariar leases; permitir recuperacion vencida antes de reactivar.
5. Escalar como Error/Critical segun umbral.

### Cola creciendo

1. Separar por tenant, ambiente y estado sin mostrar claves.
2. Comparar tasa de entrada/salida y edad maxima.
3. Revisar worker habilitado, heartbeat, leases, SQL y proveedor.
4. No aumentar concurrencia sin verificar rate limits, SQL y locks.

### Retry repetidos o DeadLetter

1. Clasificar error: validacion/contrato, transport/TLS, SQL, proveedor o integridad.
2. Corregir causa raiz antes del reproceso.
3. Usar accion backend permitida, motivo y auditoria; nunca SQL manual.
4. No reprocesar terminales masivamente sin plan y limite.

### Lease vencido

1. Confirmar que `LockExpiresAt` esta vencido y que el propietario no sigue ejecutando.
2. Dejar que el procedimiento atomico existente recupere el lease.
3. Verificar cierre del intento anterior y nuevo estado.
4. Investigar shutdown, timeout, host o conectividad si se repite.

### Error TLS

1. Identificar frontera: SQL server, trust del cliente o HTTPS SRI.
2. Validar hostname, cadena, fechas, revocacion y almacenes bajo la cuenta del servicio.
3. Coordinar renovacion/importacion con DBA/Seguridad.
4. Nunca usar `TrustServerCertificate=true`, `IgnoreSslErrors` o callbacks accept-all.

### Conflicto XML/SHA-256

1. Detener procesamiento del tenant afectado.
2. Conservar fila, bytes, intento y auditoria; no reemplazar ni borrar.
3. Capturar IDs, hashes truncados/identificadores seguros y TraceId.
4. Escalar Critical a Desarrollo, Seguridad y propietario.

## Criterios de aborto

- XML, claves completas, credenciales o connection strings en logs/eventos.
- TLS estricto no puede establecerse.
- Schema incompatible o script sin autorizacion.
- Duplicado de documento, claim simultaneo o lease inconsistente.
- Estado `Authorized` sin XML/hash/tamano coherentes.
- Instancia no autorizada o `WorkerInstance` duplicado.
- Backup/restore gate fallido.
- Alertas Critical sin responsable.
- Tenant/ambiente fuera del alcance aprobado.
- Error/DeadLetter o cola supera umbral aprobado.

Al abortar: deshabilitar, detener, confirmar proceso, conservar evidencia y ejecutar rollback no destructivo.

## Backup y restauracion

### Backup

- Incluir Master y todos los tenants habilitados en el mismo change record.
- Usar backup normal programado; `COPY_ONLY` solo para validacion manual/predeploy.
- Exigir cifrado, `CHECKSUM`, ACL, retencion y copia separada.
- Registrar backup set, timestamps, LSN/recovery model, resultado y responsable sin rutas sensibles innecesarias.

### Restore test

1. Restaurar en entorno aislado con nombres no productivos.
2. Verificar checksum e integridad SQL.
3. Verificar versiones de schema y conteos queue/attempt/document/audit.
4. Recalcular muestra autorizada de `SizeBytes`/SHA-256 sin imprimir XML.
5. Iniciar worker deshabilitado contra el entorno restaurado si la prueba lo autoriza.
6. Registrar RPO/RTO observado y destruir el entorno conforme a politica.

La restauracion productiva requiere aprobacion formal; no se usa para corregir un unico evento.

## Rotacion de certificados y secrets

1. Inventariar dependencia, propietario, vencimiento y consumidores.
2. Crear cambio y rollback; no exportar claves privadas.
3. Instalar/rotar mediante el proveedor o store aprobado.
4. Validar bajo la identidad del servicio.
5. Iniciar deshabilitado, comprobar TLS/heartbeat y luego habilitar bajo autorizacion.
6. Revocar/eliminar material anterior solo despues de la ventana y segun politica.
7. Rotar inmediatamente si un secreto aparece en log, paquete o ticket; tratarlo como incidente.

## Rollback

1. Establecer `Enabled=false` y detener el servicio.
2. Confirmar cero procesos y registrar leases pendientes.
3. Seleccionar release anterior verificada y restaurar configuracion no secreta anterior.
4. Recuperar secrets desde su proveedor, no desde archivos del paquete.
5. No deshacer SQL destructivamente; aplicar solo forward-fix autorizado.
6. Iniciar deshabilitado, validar version/heartbeat/schema y smoke test sin SRI.
7. Rehabilitar solo con nueva decision de cambio.
8. Conservar logs, auditorias, cola, intentos, XML y reporte de incidente.

## Evidencia de cierre

Registrar:

- change/ticket y aprobadores;
- host, cuenta categorizada, servicio, version/commit/hash;
- inicio/fin/duracion;
- bases y schema versions sin conexiones;
- estado inicial/final de health, queue, attempts, documents, audits y locks;
- backups y restore evidence;
- alertas observadas y resolucion;
- resultado de shutdown/restart/rollback cuando aplique;
- confirmacion de cero secretos/XML en evidencia;
- procesos finales y estado `Enabled`;
- resultado `Validado`, `Fallido`, `Bloqueado` o `No ejecutado` por gate.

## Escalamiento pendiente

Antes de usar este runbook deben completarse: contactos, horario, severidades, SLA, canal primario/secundario, autoridad para detener, autoridad para reprocesar, RPO/RTO, retencion, tenant/ambiente piloto y ventana de mantenimiento.
